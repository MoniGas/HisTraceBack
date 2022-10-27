using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel.InterfaceModels;
using Common.Log;
using LinqModel;
using Common;

namespace Dal
{
	public class ApiDAL : DALBase
	{
		DEncrypt DEncrypt = new DEncrypt();
		int AddHours = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AddHours"]);

		#region 登录验证及加密数据接口
		/// <summary>
		/// 登录验证及加密数据接口
		/// </summary>
		/// <param name="userName">用户名</param>
		/// <param name="passWord">密码</param>
		/// <returns>InterfaceResult</returns>
		public InterfaceResult login(Login model)
		{
			string retMsg = "";
			InterfaceResult result = new InterfaceResult();
			try
			{
				using (DataClassesDataContext db = GetDataContext())
				{
					//根据用户名筛选用户信息
					var item = db.View_EnterpriseInfoUser.Where(m => m.LoginName == model.userName).ToList();
					if (item.Count == 0 || item == null)
					{
						result.retCode = 1;
						result.isSuccess = false;
						result.retMessage = "用户不存在！";
						result.retData = null;
						return result;
					}
					else
					{
						//用筛选到的数据再根据密码进行第二次筛选
						item = item.Where(m => m.LoginPassWord == model.passWord).ToList();

						if (item.Count == 0 || item == null)
						{
							result.retCode = 2;
							result.isSuccess = false;
							result.retMessage = "密码错误！";
							result.retData = null;
							return result;
						}
						else
						{
							//若有多条，默认选中第一条数据（理论不会有多条用户信息）
							View_EnterpriseInfoUser info = item[0];
							//加密数据：登录名、登录密码、用户ID、企业ID、主码、用户类型、经销商ID和超期时间（当前时间往后以小时制）
							retMsg = info.LoginName + "&" + info.LoginPassWord+"&"+info.UserName + "&" + info.Enterprise_User_ID + "&" + info.Enterprise_Info_ID + "&" + info.MainCode + "&" + info.UserType + "&" + info.DealerID + "&" + DateTime.Now.AddHours(AddHours).ToString("yyyy-MM-dd HH:mm:ss");

							result.retCode = 0;
							result.isSuccess = true;
							result.retMessage = "成功！";
							result.retData = DEncrypt.Encrypt(retMsg);
							return result;
						}
					}
				}
			}
			catch (Exception ex)
			{
				string errData = "接口api登录异常：";
				result.retCode = 3;
				result.isSuccess = false;
				result.retMessage = errData;
				result.retData = null;
				WriteLog.WriteErrorLog(errData + ex.Message);
			}
			return result;
		}
		#endregion

		#region Token解密接口
		/// <summary>
		/// Token解密接口，返回解密数据Token,若返回值为null，则accessToken失效
		/// </summary>
		/// <param name="accessToken">加密字符串</param>
		/// <returns>返回解密数据Token,若返回值为null，则accessToken失效</returns>
		public Token TokenDecrypt(string accessToken)
		{
			Token token = new Token();
			try
			{
				//第一步：解密
				string str = DEncrypt.Decrypt(accessToken);
				//第二步：拆分
				string[] paramArr = str.Split(new char[] { '&' });

				if (paramArr.Length == 9)
				{
					token.LoginName = paramArr[0];
					token.LoginPassWord = paramArr[1];
					token.UserName = paramArr[2];
					token.Enterprise_User_ID = string.IsNullOrEmpty(paramArr[3]) ? 0 : Convert.ToInt64(paramArr[3]);
					token.Enterprise_Info_ID = string.IsNullOrEmpty(paramArr[4]) ? 0 : Convert.ToInt64(paramArr[4]);
					token.MainCode = paramArr[5];
					token.UserType = paramArr[6];
					token.DealerID = string.IsNullOrEmpty(paramArr[7]) ? 0 : Convert.ToInt64(paramArr[7]);
					token.ExprieTime = DateTime.Parse(paramArr[8]);
					if (DateTime.Now > token.ExprieTime)
					{
						token.isTokenOK = false;
					}
					else 
					{
						token.isTokenOK = true;
					}
					return token;
				}
				else
				{
					token = null;
					return token;
				}
			}
			catch (Exception ex)
			{
				token = null;
				WriteLog.WriteErrorLog("解密Token异常:" + ex.Message + ";Token值：" + accessToken);
				return token;
			}
			return token;
		}
		#endregion

	}
}
