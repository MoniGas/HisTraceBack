using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel.InterfaceModels;
using Common.Log;
using LinqModel;
using System.Configuration;
using Common;
using InterfaceWeb;
using System.ComponentModel;
using System.Xml;

namespace Dal
{
	public class UdiDAL : DALBase
	{
		ApiDAL apiDal = new ApiDAL();
		UDITool UDITool = new UDITool();
		public int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
		public string access_token = ConfigurationManager.AppSettings["access_token"];
		public string access_token_code = ConfigurationManager.AppSettings["access_token_code"];
		public string idcodeUrl = ConfigurationManager.AppSettings["interfaceUrl"];

		#region 同步UDI-DI接口
		/// <summary>
		/// 同步UDI-DI接口
		/// </summary>
		/// <param name="accessToken">加密字符串</param>
		/// <returns>InterfaceResult</returns>
		public InterfaceResult DILst(DILstRequestParam Param, string accessToken)
		{
			InterfaceResult result = new InterfaceResult();
			DILstResult DILstResult = new DILstResult();
			List<DILst> DILst = new List<DILst>();
			int? totalPageCount = 0;
			try
			{
				#region 验证信息
				//第一步：先解密accessToken，根据解密到的数据执行后续逻辑
				Token token = apiDal.TokenDecrypt(accessToken);
				if (token == null || !token.isTokenOK)
				{
					result.retCode = 1;
					result.retMessage = "token失效，请重新获取!";
					result.isSuccess = false;
					result.retData = null;
					return result;
				}
				if (!string.IsNullOrEmpty(Param.specIfication) && Param.specIfication.Length != 1)
				{
					result.retCode = 3;
					result.retMessage += "包装规格值为0-9！";
				}
				else
				{
					if (!string.IsNullOrEmpty(Param.specIfication) && (Convert.ToInt32(Param.specIfication) < 0 || Convert.ToInt32(Param.specIfication) > 9))
					{
						result.retCode = 3;
						result.retMessage += "包装规格值为0-9！";
					}
				}
				if (result.retCode == 3)
				{
					result.retCode = 3;
					result.retMessage = "信息验证失败：" + result.retMessage;
					result.isSuccess = false;
					result.retData = null;
					return result;
				}
				#endregion
				using (DataClassesDataContext db = GetDataContext())
				{
					//按照升序排列
					var data = db.MaterialDI.Where(m => m.EnterpriseID == token.Enterprise_Info_ID && m.Status == (int)Common.EnumFile.Status.used).OrderBy(m => m.adddate).ToList();
					//产品名称
					if (!string.IsNullOrEmpty(Param.productName))
					{
						data = data.Where(m => m.MaterialName.Contains(Param.productName)).ToList();
					}
					//品类编码
					if (!string.IsNullOrEmpty(Param.categoryCode))
					{
						data = data.Where(m => m.CategoryCode.Contains(Param.categoryCode)).ToList();
					}
					//规格型号
					if (!string.IsNullOrEmpty(Param.specModel))
					{
						data = data.Where(m => m.MaterialXH.Contains(Param.specModel)).ToList();
					}
					//包装规格
					if (!string.IsNullOrEmpty(Param.specIfication))
					{
						data = data.Where(m => m.Specifications == Param.specIfication).ToList();
					}
					Param.pageNumber = string.IsNullOrEmpty(Param.pageNumber.ToString()) ? 1 : Param.pageNumber;//默认1页
					Param.pageSize = string.IsNullOrEmpty(Param.pageSize.ToString()) ? _pageSize : Param.pageSize;//默认1页20条记录，由配置文件决定
					totalPageCount = (data.Count % Param.pageSize) > 0 ? (data.Count / Param.pageSize) + 1 : (data.Count / Param.pageSize);
					data = data.Skip((Convert.ToInt32(Param.pageNumber) - 1) * Convert.ToInt32(Param.pageSize)).Take(Convert.ToInt32(Param.pageSize)).ToList();
					foreach (var item in data)
					{
						DILst DI = new DILst();
						DI.productName = item.MaterialName;
						//若品类编码带包装规格值与检验位，默认去掉
						DI.categoryCode = item.CategoryCode == item.MaterialUDIDI.Split('.')[4] ? item.CategoryCode.Substring(1, item.CategoryCode.Length - 2) : item.CategoryCode;
						DI.specIficationValue = item.Specifications;
						DI.specIficationName = item.SpecificationName;
						DI.modelName = item.MaterialXH;
						DI.completeCode = item.MaterialUDIDI;
						DI.GSIDI = item.GSIDI;
						DI.HisCode = item.HisCode;
						DILst.Add(DI);
					}
					DILstResult.data = DILst;
					DILstResult.totalPageCount = totalPageCount;
					result.retCode = 0;
					result.retMessage = "成功";
					result.isSuccess = true;
					result.retData = DILstResult;
					return result;
				}

			}
			catch (Exception ex)
			{
				string errData = "udi同步UDI-DI接口异常";
				result.retCode = 2;
				result.isSuccess = false;
				result.retMessage = errData;
				result.retData = null;
				WriteLog.WriteErrorLog(errData + ":" + ex.Message);
			}
			return result;
		}
		#endregion

		#region 同步UDI-PI接口
		/// <summary>
		/// 同步UDI-PI接口
		/// </summary>
		/// <param name="accessToken">加密字符串</param>
		/// <returns>InterfaceResult</returns>
		public InterfaceResult PILst(PILstRequestParam Param, string accessToken)
		{
			InterfaceResult result = new InterfaceResult();
			PILstResult PILstResult = new PILstResult();
			List<PILst> PILst = new List<PILst>();
			int? totalPageCount = 0;
			try
			{
				#region 验证信息
				//第一步：先解密accessToken，根据解密到的数据执行后续逻辑
				Token token = apiDal.TokenDecrypt(accessToken);
				if (token == null || !token.isTokenOK)
				{
					result.retCode = 1;
					result.retMessage = "token失效，请重新获取!";
					result.isSuccess = false;
					result.retData = null;
					return result;
				}
				if (!string.IsNullOrEmpty(Param.specIfication) && Param.specIfication.Length != 1)
				{
					result.retCode = 3;
					result.retMessage += "包装规格值为0-9！";
				}
				else
				{
					if (!string.IsNullOrEmpty(Param.specIfication) && (Convert.ToInt32(Param.specIfication) < 0 || Convert.ToInt32(Param.specIfication) > 9))
					{
						result.retCode = 3;
						result.retMessage += "包装规格值为0-9！";
					}
				}
				if (result.retCode == 3)
				{
					result.retCode = 3;
					result.retMessage = "信息验证失败：" + result.retMessage;
					result.isSuccess = false;
					result.retData = null;
					return result;
				}
				#endregion
				using (DataClassesDataContext db = GetDataContext())
				{
					//View_RequestCodeSetting和MaterialDI联合使用
					var data = db.View_RequestCodeSetting.Where(m => m.EnterpriseId == token.Enterprise_Info_ID && (m.Type == (int)Common.EnumFile.GenCodeType.localCreate || m.Type == (int)Common.EnumFile.GenCodeType.single
						|| m.Type == (int)Common.EnumFile.GenCodeType.trap || m.Type == (int)Common.EnumFile.GenCodeType.pesticides)).ToList();
					data = data.Where(m => m.FixedCode.StartsWith("MA.")).ToList();
					//产品名称
					if (!string.IsNullOrEmpty(Param.productName))
					{
						data = data.Where(m => m.MaterialFullName.Contains(Param.productName)).ToList();
					}
					//品类编码
					if (!string.IsNullOrEmpty(Param.categoryCode))
					{
						data = data.Where(m => m.FixedCode.Split('.')[4].Contains(Param.categoryCode)).ToList();
					}
					//规格型号
					if (!string.IsNullOrEmpty(Param.specModel))
					{
						data = data.Where(m => m.MaterialXH.Contains(Param.specModel)).ToList();
					}
					//包装规格
					if (!string.IsNullOrEmpty(Param.specIfication))
					{
						data = data.Where(m => m.FixedCode.Split('.')[4].Substring(0, 1) == Param.specIfication).ToList();
					}
					if (!string.IsNullOrEmpty(Param.completeCode))
					{
						data = data.Where(m => m.FixedCode == Param.completeCode).ToList();
					}
					Param.pageNumber = string.IsNullOrEmpty(Param.pageNumber.ToString()) ? 1 : Param.pageNumber;//默认1页
					Param.pageSize = string.IsNullOrEmpty(Param.pageSize.ToString()) ? _pageSize : Param.pageSize;//默认1页20条记录，由配置文件决定
					totalPageCount = (data.Count % Param.pageSize) > 0 ? (data.Count / Param.pageSize) + 1 : (data.Count / Param.pageSize);
					data = data.Skip((Convert.ToInt32(Param.pageNumber) - 1) * Convert.ToInt32(Param.pageSize)).Take(Convert.ToInt32(Param.pageSize)).ToList();
					foreach (var item in data)
					{
						PILst PI = new PILst();
						PI.productName = item.MaterialFullName;
						PI.completeCode = item.FixedCode;
						PI.batchNo = item.IDCodeBatchNo;
						PI.MaterialXH = item.MaterialXH;
						PI.codeNum = Convert.ToInt32(item.TotalNum);
						PILst.Add(PI);
					}
					PILstResult.data = PILst;
					PILstResult.totalPageCount = totalPageCount;
					result.retCode = 0;
					result.retMessage = "成功";
					result.isSuccess = true;
					result.retData = PILstResult;
					return result;
				}

			}
			catch (Exception ex)
			{
				string errData = "udi同步UDI-PI接口异常";
				result.retCode = 2;
				result.isSuccess = false;
				result.retMessage = errData;
				result.retData = null;
				WriteLog.WriteErrorLog(errData + ":" + ex.Message);
			}
			return result;
		}
		#endregion

		#region 同步UDI-PI明细（完整码内容）接口
		/// <summary>
		/// 同步UDI-PI明细（完整码内容）接口
		/// </summary>
		/// <param name="accessToken">加密字符串</param>
		/// <returns>InterfaceResult</returns>
		public InterfaceResult PIDetail(PIDetailRequestParam Param, string accessToken)
		{
			InterfaceResult result = new InterfaceResult();
			PIDetailResult PIDetailResult = new PIDetailResult();
			List<PIDetail> PIDetailLst = new List<PIDetail>();
			List<Enterprise_FWCode_00> resultCode = new List<Enterprise_FWCode_00>();
			int? totalPageCount = 0;
			try
			{
				//第一步：先解密accessToken，根据解密到的数据执行后续逻辑
				Token token = apiDal.TokenDecrypt(accessToken);
				if (token == null || !token.isTokenOK)
				{
					result.retCode = 1;
					result.retMessage = "token失效，请重新获取!";
					result.isSuccess = false;
					result.retData = null;
					return result;
				}

				using (LinqModel.DataClassesDataContext dataContextWeb = GetDataContext())
				{
					var data = dataContextWeb.RequestCode.ToList();
					//UDI-DI编码
					if (!string.IsNullOrEmpty(Param.completeCode))
					{
						data = data.Where(m => m.FixedCode == Param.completeCode).ToList();
					}
					//批量申请生成批次
					if (!string.IsNullOrEmpty(Param.batchNo))
					{
						data = data.Where(m => m.IDCodeBatchNo == Param.batchNo).ToList();
					}
					//产品名称
					if (!string.IsNullOrEmpty(Param.productName))
					{
						var MaterialData = dataContextWeb.MaterialDI.Where(m => m.MaterialName.Contains(Param.productName)).Select(m => new { m.ID }).ToList();
						List<long?> ids = new List<long?>();
						foreach (var item in MaterialData)
						{
							ids.Add(item.ID);
						}
						data = data.Where(m => ids.Contains(m.Material_ID)).ToList();
					}
					//计算页码，默认1页
					Param.pageNumber = string.IsNullOrEmpty(Param.pageNumber.ToString()) ? 1 : Param.pageNumber;
					//默认1页20条记录，由配置文件决定
					Param.pageSize = string.IsNullOrEmpty(Param.pageSize.ToString()) ? _pageSize : Param.pageSize;
					//计算总页数
					totalPageCount = (data.Count % Param.pageSize) > 0 ? (data.Count / Param.pageSize) + 1 : (data.Count / Param.pageSize);
					data = data.Skip((Convert.ToInt32(Param.pageNumber) - 1) * Convert.ToInt32(Param.pageSize)).Take(Convert.ToInt32(Param.pageSize)).ToList();
					foreach (var item in data)
					{
						string tablename = "";
						PIDetail PIDetail = new PIDetail();
						long totalCount = item.TotalNum.Value;//二维码总数量
						long minusCount = 0;
						using (DataClassesDataContext dataContext = GetCodeNewDataContext(item.RequestCode_ID, out tablename))
						{
							try
							{
								dataContext.CommandTimeout = 600;
								//查找Code库的二维码内容
								StringBuilder strSql = new StringBuilder();
								strSql.Append("select top " + totalCount + " * from " + tablename + " where RequestCode_ID=" + item.RequestCode_ID
									+ " and Enterprise_FWCode_ID not in (select  top " + minusCount
									+ " Enterprise_FWCode_ID  from " + tablename + " where RequestCode_ID=" + item.RequestCode_ID
									+ " order by Enterprise_FWCode_ID  )  ");
								strSql.Append("  order by Enterprise_FWCode_ID ");

								resultCode = dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();
								List<string> str = new List<string>();
								foreach (var m in resultCode)
								{
									str.Add(m.EWM.ToString().Trim());
								}
								PIDetail.completeCode = item.FixedCode;
								PIDetail.batchNo = item.IDCodeBatchNo;
								PIDetail.codeData = str;
								PIDetailLst.Add(PIDetail);
							}
							catch (Exception ex)
							{
								resultCode = null;
								string errData = "接口udi同步UDI-PI明细（完整码内容）接口异常";
								result.retCode = 2;
								result.isSuccess = false;
								result.retMessage = errData;
								result.retData = null;
								WriteLog.WriteErrorLog(errData + ":" + ex.Message);
								return result;
							}

						}
					}

					//返回PI明细（包含码内容）信息
					PIDetailResult.data = PIDetailLst;
					PIDetailResult.totalPageCount = totalPageCount;

					result.retCode = 0;
					result.retMessage = "成功";
					result.isSuccess = true;
					result.retData = PIDetailResult;
					return result;
				}

			}
			catch (Exception ex)
			{
				string errData = "udi同步UDI-PI明细（完整码内容）接口异常";
				result.retCode = 2;
				result.isSuccess = false;
				result.retMessage = errData;
				result.retData = null;
				WriteLog.WriteErrorLog(errData + ":" + ex.Message);
			}
			return result;
		}

		/// <summary>
		/// 获取code库的连接串，获取码内容
		/// </summary>
		/// <param name="rId"></param>
		/// <param name="tablename"></param>
		/// <returns></returns>
		private DataClassesDataContext GetCodeNewDataContext(long rId, out string tablename)
		{
			string datasource = "";
			string database = "";
			string username = "";
			string pass = "";
			tablename = "";
			DataClassesDataContext result = null;
			try
			{
				using (DataClassesDataContext dataContext = GetDataContext())
				{
					RequestCode code = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
					long table_id = code.Route_DataBase_ID.Value;
					Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
					if (table == null)
					{
						return null;
					}
					datasource = table.DataSource;
					database = table.DataBaseName;
					username = table.UID;
					pass = table.PWD;
					tablename = table.TableName;

				}
				result = GetDataContext(datasource, database, username, pass);
			}
			catch { throw; }
			return result;
		}


		#endregion

		#region 生成UDI-DI接口
		/// <summary>
		/// 生成UDI-DI接口
		/// </summary>
		/// <param name="accessToken">加密字符串</param>
		/// <returns>InterfaceResult</returns>
		public InterfaceResult SaveDI(SaveDIRequestParam Param, string accessToken)
		{
			InterfaceResult result = new InterfaceResult();
			try
			{
				//第一步：先解密accessToken，根据解密到的数据执行后续逻辑
				Token token = apiDal.TokenDecrypt(accessToken);
				//第二步：验证信息
				#region 验证信息
				if (token == null || !token.isTokenOK)
				{
					result.retCode = 1;
					result.retMessage = "token失效，请重新获取!";
					result.isSuccess = false;
					result.retData = null;
					return result;
				}
				if (string.IsNullOrEmpty(Param.categoryCode))
				{
					result.retCode = 3;
					result.retMessage += "品类编码不能为空！";
				}
				else if (Param.categoryCode.Trim().Length < 6 || Param.categoryCode.Trim().Length > 10)
				{
					result.retCode = 3;
					result.retMessage += "品类编码为6-10位字母、数字、下划线等部分字符！";
				}
				else
				{
					string verifyCode = UDITool.GenVerifyCode(Param.categoryCode.Trim());
					if (verifyCode.Length > 1)
					{
						result.retCode = 3;
						result.retMessage += "品类编码" + verifyCode + "，该字符不在码表中！";
					}
				}
				if (string.IsNullOrEmpty(Param.productName))
				{
					result.retCode = 3;
					result.retMessage += "产品名称不能为空！";
				}
				if (string.IsNullOrEmpty(Param.specIfication))
				{
					result.retCode = 3;
					result.retMessage += "包装规格不能为空！";
				}
				else if (Param.specIfication.Length != 1)
				{
					result.retCode = 3;
					result.retMessage += "包装规格值为0-9！";
				}
				else
				{
					if (Convert.ToInt32(Param.specIfication) < 0 || Convert.ToInt32(Param.specIfication) > 9)
					{
						result.retCode = 3;
						result.retMessage += "包装规格值为0-9！";
					}
				}
				if (string.IsNullOrEmpty(Param.specModel))
				{
					result.retCode = 3;
					result.retMessage += "产品规格型号不能为空！";
				}
				if (result.retCode == 3)
				{
					result.retCode = 3;
					result.retMessage = "信息验证失败：" + result.retMessage;
					result.isSuccess = false;
					result.retData = null;
					return result;
				}
				#endregion
				using (DataClassesDataContext db = GetDataContext())
				{
					string UDIDI = token.MainCode + "." + Param.specIfication + Param.categoryCode;
					string jy = UDITool.GenVerifyCode(UDIDI);
					if (jy.Length > 1)
					{
						result.retCode = 3;
						result.retMessage += "品类编码" + jy + "，该字符不在码表中！";
						result.isSuccess = false;
						result.retData = null;
						return result;
					}
					UDIDI = UDIDI + jy;
					//先上传
					MedicalRegMsg info = IDCodeMedicalReg(token.MainCode, Param.categoryCode, Param.productName, Param.specIfication, Param.specModel);
					//成功,或者重复申请的表示已经上传过了，在平台保存一次
					if (info.result_code == 1 || info.result_code == 50002)
					{
						var data = db.MaterialDI.Where(m => m.MaterialUDIDI == UDIDI).SingleOrDefault();
						if (data == null)
						{
							//DI实体
							MaterialDI model = new MaterialDI();
							model.adddate = DateTime.Now;
							model.adduser = token.Enterprise_User_ID;
							model.CategoryCode = Param.categoryCode;
							model.EnterpriseID = token.Enterprise_Info_ID;
							model.MaterialName = Param.productName;
							model.MaterialUDIDI = UDIDI;
							model.Specifications = Param.specIfication;
							model.SpecificationName = BZGG[Convert.ToInt32(Param.specIfication)];
							model.Status = (int)Common.EnumFile.Status.used;
							model.MaterialXH = Param.specModel;
							Material madate = db.Material.FirstOrDefault(m => m.MaterialName == model.MaterialName && m.Status == (int)Common.EnumFile.Status.used);
							if (madate != null)
							{
								model.MaterialID = madate.Material_ID;
								db.MaterialDI.InsertOnSubmit(model);
								db.SubmitChanges();
							}
							else
							{
								Material temp = new Material();
								temp.adddate = DateTime.Now;
								temp.adduser = token.Enterprise_User_ID;
								temp.Enterprise_Info_ID = token.Enterprise_Info_ID;
								temp.MaterialName = Param.productName;
								temp.MaterialFullName = Param.productName;
								temp.Status = (int)Common.EnumFile.Status.used;
								db.Material.InsertOnSubmit(temp);
								db.SubmitChanges();
								model.MaterialID = temp.Material_ID;
								Category tempca = new Category();
								tempca.CategoryCode = Param.categoryCode;
								tempca.AddTime = DateTime.Now;
								tempca.AddUser = token.Enterprise_User_ID;
								tempca.Enterprise_Info_ID = Convert.ToInt64(token.Enterprise_Info_ID);
								tempca.MaterialID = temp.Material_ID;
								tempca.MaterialName = temp.MaterialName;
								tempca.Status = (int)Common.EnumFile.Status.used;
								db.Category.InsertOnSubmit(tempca);
								db.SubmitChanges();
								db.MaterialDI.InsertOnSubmit(model);
								db.SubmitChanges();
							}
						}
					}
					else
					{
						result.retCode = 2;
						result.retMessage = "失败";
						result.isSuccess = false;
						result.retData = null;
						return result;
					}
					result.retCode = 0;
					result.retMessage = "成功";
					result.isSuccess = true;
					result.retData = UDIDI;
					return result;
				}

			}
			catch (Exception ex)
			{
				string errData = "udi生成UDI-DI接口异常";
				result.retCode = 2;
				result.isSuccess = false;
				result.retMessage = errData + ":" + ex.Message;
				result.retData = null;
				WriteLog.WriteErrorLog(errData + ":" + ex.Message);
			}
			return result;
		}

		public static Dictionary<int, string> BZGG
		{
			get
			{
				return new Dictionary<int, string>(){
                    { 0,"初级包装:0"},
                    { 1,"一级包装:1"},
                    { 2,"二级包装:2"},
                    { 3,"三级包装:3"},
					{ 4,"四级包装:4"},
                    { 5,"五级包装:5"},
                    { 6,"六级包装:6"},
                    { 7,"七级包装:7"},
					{ 8,"八级包装:8"},
                    { 9,"九级包装:9"}
                };
			}
		}

		public MedicalRegMsg IDCodeMedicalReg(string company_idcode,
			string category_code, string model_number, string specification, string product_model
			//, string serial_number, string start_date, string batch_number, string end_date, string effective_date
			)
		{
			string functionUrl = "/sp/idcode/medical/idcodeinfo/reg";
			Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"company_idcode", company_idcode},
                    {"category_code", category_code},
                    {"model_number", model_number},
                    {"specification", specification},
                    {"product_model", product_model},
                    //{"serial_number", code_list_str},
                    //{"start_date", start_date},
                    //{"batch_number", batch_number},
                    //{"d_batch_number", d_batch_number},
                    //{"end_date", end_date},
                    //{"effective_date", effective_date},
                    {"gotourl", "http://udi2.com/Wap_Index/Index"},
                    {"sample_url", "http://udi2.com/Wap_Index/Index"},
                    {"time", DEncrypt.GetTimeStamp(DateTime.Now)}
                };
			dataDic.Add("hash", DEncrypt.GetHash(functionUrl, dataDic, access_token_code));
			string rst = APIHelper.sendPost(idcodeUrl + functionUrl, dataDic, "post");
			//WriteLog("time:" + dataDic["time"] + "\t hash:" + dataDic["hash"] + "\t result:" + rst + "\t interfaceUrl:" + functionUrl, "IDCodeLog");
			if (rst.Contains("DOCTYPE"))
				return new MedicalRegMsg();
			var info = JsonDes.JsonDeserialize<MedicalRegMsg>(rst);
			return info;
		}

		#endregion

		#region 解析码
		private Dictionary<int, string> DictPackType
		{
			get
			{
				return new Dictionary<int, string>() {
                    {0,"初级包装:0"},
                    {1,"一级包装:1"},
                    {2,"二级包装:2"},
                    {3,"三级包装:3"},
                    {4,"四级包装:4"},
                    {5,"五级包装:5"},
                    {6,"六级包装:6"},
                    {7,"七级包装:7"},
                    {8,"八级包装:8" },
                    {9, "九级包装:9" }
                };
			}
		}

		public enum CodeFormats
		{
			/// <summary>
			/// 默认正常码格式
			/// </summary>
			[Description("默认正常码格式")]
			defaultcode = 0,
			[Description("xml格式")]
			XML = 1,
			[Description("json格式")]
			json = 2
		}


		public UDIAnalyseResult UDIAnalyse(string CodeValue, string key, int? CodeFormat)
		{
			UDIAnalyseDAL dal = new UDIAnalyseDAL();
			//UDIAnalyseBLL UDIAnalyseBLL = new UDIAnalyseBLL();
			UDIAnalyseResult result = new UDIAnalyseResult();
			UDIAnalyseInfo info = new UDIAnalyseInfo();
			UDIMaterial UDIMaterialModel = new UDIMaterial();
			try
			{
				#region 码格式及token判断
				if (string.IsNullOrEmpty(CodeValue))
				{
					result.code = 1006;
					result.Msg = "码内容不能为空！";
					result.data = info;
					return result;
				}
				if (string.IsNullOrEmpty(key))
				{
					result.code = 1006;
					result.Msg = "授权Token不能为空！";
					result.data = info;
					return result;
				}
				UDIKey UDIKey = dal.getUDIKey(key);
				if (UDIKey == null)
				{
					result.code = 1001;
					result.Msg = "解析失败：授权Token不正确！";
					result.data = info;
					return result;
				}
				if (UDIKey.Status != (int)Common.EnumFile.Status.used)
				{
					result.code = 1002;
					result.Msg = "解析失败：该授权Token已停用！";
					result.data = info;
					return result;
				}
				bool IsLicenseExpire = DateTime.Compare(Convert.ToDateTime(UDIKey.EndDate), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0 ? false : true;
				if (IsLicenseExpire)
				{
					result.code = 1003;
					result.Msg = "解析失败：该授权Token已到期！";
					result.data = info;
					return result;
				}
				int num = dal.getUDIAnalyseCount(UDIKey.ID);
				//UDIKey.MaterialCount==-1为不限制解析数量，为UDI解析小程序专用，此key虽不限制，但每次解析产品及数量依旧入库，以便后期统计查询使用
				if (num > UDIKey.MaterialCount && UDIKey.MaterialCount!=-1)
				{
					result.code = 1004;
					result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
					result.data = info;
					return result;
				}
				#endregion

				#region AHM码
				//MF：产品DI（不定长,不带MF最多13位）；BA：生产批号（不定长）；SN：序列号（不定长）；MD：生产日期；BD：保质日期；PL：包装规格；ED：有效期
				if (CodeValue.Substring(0, 2) == "MF")
				{
					if (CodeFormat == (int)CodeFormats.defaultcode)
					{
						string _MD = "", _BD = "", _ED = "", _PL = "", _SN = "", _BA = "";
						if (CodeValue.IndexOf("BA") > 0)
						{
							string[] str = CodeValue.Split(new string[] { "BA" }, StringSplitOptions.RemoveEmptyEntries);
							if (str.Length > 0)
							{
								info.DI = str[0];
								if (str.Length >= 2)
								{
									string[] strs = str[1].Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
									if (strs.Length > 0)
									{
										_BA = strs[0];
										info.BatchNumber = _BA;
									}
									foreach (string item in strs)
									{
										if (item.IndexOf("SN") > 0)
										{
											_SN = item.Substring(CodeValue.IndexOf("SN") + 2);
											info.SerialNumber = _SN;
										}
									}
								}
							}
						}


						if (CodeValue.IndexOf("MD") > 0)
						{
							_MD = CodeValue.Substring(CodeValue.IndexOf("MD") + 2, 6);
							info.ProductDate = _MD;
						}
						if (CodeValue.IndexOf("BD") > 0)
						{
							_BD = CodeValue.Substring(CodeValue.IndexOf("BD") + 2, 6);
							info.InvalDate = _BD;
						}
						if (CodeValue.IndexOf("ED") > 0)
						{
							_ED = CodeValue.Substring(CodeValue.IndexOf("ED") + 2, 6);
							info.EffectivitDate = _ED;
						}
						if (CodeValue.IndexOf("PL") > 0)
						{
							_PL = CodeValue.Substring(CodeValue.IndexOf("PL") + 2, 1);
							info.Package = DictPackType[Convert.ToInt32(_PL)];
						}
						result.code = 0;
						result.Msg = "暂未开通阿里健康码解析功能，稍后再试！";
						result.data = info;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.XML)
					{
						result.code = 1009;
						result.Msg = "阿里健康码暂未开通XML解析！";
						result.data = info;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.json)
					{
						result.code = 1009;
						result.Msg = "阿里健康码暂未开通JSON解析！";
						result.data = info;
						return result;
					}
					else
					{
						result.code = 1008;
						result.Msg = "码格式输入错误，请重新输入！";
						result.data = info;
						return result;
					}
				}
				#endregion

				#region MA码
				else if (CodeValue.Substring(0, 2) == "MA" || CodeValue.Contains("<UDI IAC=\"MA\">"))
				{
					if (CodeFormat == (int)CodeFormats.defaultcode)
					{
						if (CodeValue.Contains("<UDI IAC=\"MA\">"))
						{
							result.code = 1008;
							result.Msg = "当前码为XML格式，参数码格式输入错误，请重新输入！";
							result.data = info;
							return result;
						}
						if (CodeValue.Contains("="))
						{
							int m = CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Count();
							if (m != 2)
							{
								result.code = 1005;
								result.Msg = "请输入正确的码内容！";
								result.data = info;
								return result;
							}
						}
						string _mcode = CodeValue.Contains("=") ? CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1] : CodeValue;
						List<string> codes = _mcode.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
						if (codes.Count < 5)
						{
							result.code = 1005;
							result.Msg = "请输入正确的码内容！";
							result.data = info;
							return result;
						}
						info.Package = DictPackType[Convert.ToInt32(codes[4].Substring(0, 1))];
						string MADICODE = String.Join(".", codes.Take(5).ToArray());
						string MAPICODE = _mcode.Replace(MADICODE + ".", "");
						string _m = "", _v = "", _e = "", _l = "", _s = "", _d = "";
						info.DI = MADICODE;
						var _backCodes = codes.Skip(5).ToList();
						foreach (string c in _backCodes)
						{
							if (c.StartsWith("M"))
							{
								//_m = "生产日期:" + c.Substring(1);
								//_m = "20" + c.Substring(1);
								info.ProductDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("P"))
							{
								//_m = "生产日期:" + c.Substring(1);
								info.ProductDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("V"))
							{
								//_v = "有效期:" + c.Substring(1);
								info.EffectivitDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("E"))
							{
								//_e = "失效日期:" + c.Substring(1);
								info.InvalDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("L"))
							{
								//_l = "生产批号:" + c.Substring(1);
								info.BatchNumber = c.Substring(1);
							}
							else if (c.StartsWith("D"))
							{
								//_d = "灭菌批号:" + c.Substring(1);
								info.SterilizationNumber = c.Substring(1);
							}
							else if (c.StartsWith("S"))
							{
								//_s = "序列号:" + c.Substring(1);
								info.SerialNumber = c.Substring(1);
							}
						}
						UDIMaterial model = dal.getUDIMaterial(MADICODE, "MA码（IDcode）");
						if (model != null)
						{
							if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
							{
								bool IsHav = dal.GetUDIAnalyse(model);
								if (IsHav == false)
								{
									result.code = 1004;
									result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
									result.data = new UDIAnalyseInfo();
									return result;
								}
							}
							UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
							if (res.code != 0)
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = res.code;
								result.Msg = res.Msg;
								result.data = info;
								return result;
							}
							info.EnterpriseName = model.EnterpriseName;
							info.BusinessLicence = model.BusinessLicence;
							info.MaterialName = model.MaterialName;
							info.Specification = model.MaterialSpec;
							info.CodeTypeName = model.CodeTypeName;
							if(UDIKey.MaterialCount==-1)
								info.MaterialMS = model.cpms;
						}
						else
						{
							UDIMaterialModel = QueryDI(info.DI);
							if (UDIMaterialModel != null)
							{
								if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
								{
									bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
									if (IsHav == false)
									{
										result.code = 1004;
										result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
										result.data = new UDIAnalyseInfo();
										return result;
									}
								}
								info.EnterpriseName = UDIMaterialModel.EnterpriseName;
								info.BusinessLicence = UDIMaterialModel.BusinessLicence;
								info.MaterialName = UDIMaterialModel.MaterialName;
								info.Specification = UDIMaterialModel.MaterialSpec;
								info.CodeTypeName = UDIMaterialModel.CodeTypeName;
								if (UDIKey.MaterialCount == -1)
									info.MaterialMS = UDIMaterialModel.cpms;
							}
							else
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = 0;
								result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
								result.data = info;
								return result;
							}
						}

						result.code = 0;
						result.Msg = "解析成功！";
						result.data = info;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.XML)
					{
						if (!CodeValue.Contains("<UDI IAC=\"MA\">"))
						{
							result.code = 1005;
							result.Msg = "XML码格式错误，请输入正确的码内容！";
							result.data = info;
							return result;
						}
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(CodeValue);
						XmlNode xn = xmlDoc.SelectSingleNode("UDI");
						XmlNodeList xnl = xn.ChildNodes;
						foreach (XmlNode xn1 in xnl)
						{
							XmlElement xe = (XmlElement)xn1;
							switch (xe.Name)
							{
								case "DI":
									info.DI = xe.InnerText;
									break;
								case "SN":
									info.SerialNumber = xe.InnerText;
									break;
								case "MFG":
									info.ProductDate = xe.InnerText;
									break;
								case "LOT":
									info.BatchNumber = xe.InnerText;
									break;
								case "EXP":
									info.InvalDate = xe.InnerText;
									break;
								case "VAL":
									info.EffectivitDate = xe.InnerText;
									break;

							}
						}
						List<string> codes = info.DI.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
						if (codes.Count < 5)
						{
							result.code = 1005;
							result.Msg = "请输入正确的码内容！";
							result.data = info;
							return result;
						}
						info.Package = DictPackType[Convert.ToInt32(codes[4].Substring(0, 1))];
						UDIMaterial model = dal.getUDIMaterial(info.DI, "MA码（IDcode）");
						if (model != null)
						{
							if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
							{
								bool IsHav = dal.GetUDIAnalyse(model);
								if (IsHav == false)
								{
									result.code = 1004;
									result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
									result.data = new UDIAnalyseInfo();
									return result;
								}
							}
							UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
							if (res.code != 0)
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = res.code;
								result.Msg = res.Msg;
								result.data = info;
								return result;
							}
							info.EnterpriseName = model.EnterpriseName;
							info.BusinessLicence = model.BusinessLicence;
							info.MaterialName = model.MaterialName;
							info.Specification = model.MaterialSpec;
							info.CodeTypeName = model.CodeTypeName;
							if (UDIKey.MaterialCount == -1)
								info.MaterialMS = model.cpms;
						}
						else
						{
							UDIMaterialModel = QueryDI(info.DI);
							if (UDIMaterialModel != null)
							{
								if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
								{
									bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
									if (IsHav == false)
									{
										result.code = 1004;
										result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
										result.data = new UDIAnalyseInfo();
										return result;
									}
								}
								info.EnterpriseName = UDIMaterialModel.EnterpriseName;
								info.BusinessLicence = UDIMaterialModel.BusinessLicence;
								info.MaterialName = UDIMaterialModel.MaterialName;
								info.Specification = UDIMaterialModel.MaterialSpec;
								info.CodeTypeName = UDIMaterialModel.CodeTypeName;
								if (UDIKey.MaterialCount == -1)
									info.MaterialMS = UDIMaterialModel.cpms;
							}
							else
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = 0;
								result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
								result.data = info;
								return result;
							}
						}
						result.code = 0;
						result.Msg = "解析成功！";
						result.data = info;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.json)
					{
						result.code = 1009;
						result.Msg = "暂未开通JSON解析！";
						result.data = info;
						return result;
					}
					else
					{
						result.code = 1008;
						result.Msg = "码格式输入错误，请重新输入！";
						result.data = info;
						return result;
					}
				}
				#endregion

				#region GS1码
				else
				{
					//以正常码内容来解析
					if (CodeFormat == (int)CodeFormats.defaultcode)
					{
						//因商品编码是13位，该接口可直接解析商品编码，所以码最短是13位；
						if (CodeValue.Length >= 13)
						{
							if (CodeValue.Contains("="))
							{
								int m = CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Count();
								if (m != 2)
								{
									result.code = 1005;
									result.Msg = "输入的GS1码包含'='，但该码并未解析成两段，请输入正确的码内容！";
									result.data = info;
									return result;
								}
							}

							//扫码可能携带网址，需去掉网址后获取码内容解析
							string _mcode = CodeValue.Contains("=") ? CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1] : CodeValue;
							
							//分别是生产日期、有效期、批号、序列号和DI 
							string _scrq = "", _yxq = "", _ph = "", _xlh = "", _DI = "";

							//第一步去掉不可见特殊符号
							_mcode = _mcode.Contains("\u001d") ? _mcode.Replace("\u001d", "") : _mcode;

							//GS1码前几位字符;根据前几位判断，一种带括号的（01），一种不带括号的 01
							string BeforeCode = _mcode.Substring(0, 4);

							//GS1码位数；13位的为普通GS1商品编码；大于13位的为GS1-UDI码（DI部分或整个UDI码）
							int GS1CodeNum = _mcode.Length;

							#region 兼容旧模式（可解析GS1码带括号的或13位的商品编码）
							if (BeforeCode == "(01)" || GS1CodeNum == 13)
							{
								//依据括号分割GS1码
								List<string> codes = _mcode.Split(new Char[] { '(' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

								//仅有DI部分
								if (codes.Count == 1)
								{
									if (codes[0].Length == 13)
									{
										info.DI = codes[0];
									}
									else if (codes[0].Contains("01)"))
									{
										info.Package = DictPackType[Convert.ToInt32(codes[0].Substring(3, 1))];
									}
									else if (codes[0].Length == 14)
									{
										info.DI = codes[0];
										info.Package = DictPackType[Convert.ToInt32(codes[0].Substring(0, 1))];
									}
									else
									{
										result.code = 1005;
										result.Msg = "请输入正确的码内容！";
										result.data = info;
										return result;
									}
								}

								foreach (string c in codes)
								{
									if (c.StartsWith("01)"))
									{
										_DI = c.Substring(3);
										info.DI = _DI;
										if (_DI.Length == 14)
										{
											info.Package = DictPackType[Convert.ToInt32(c.Substring(3, 1))];
										}
									}
									if (c.StartsWith("11)"))
									{
										//_scrq = "生产日期:" + "20" + c.Substring(3);
										info.ProductDate = "20" + c.Substring(3, 2) + "-" + c.Substring(5, 2) + "-" + c.Substring(7, 2);
									}
									else if (c.StartsWith("17)"))
									{
										//_yxq = "有效期:" + c.Substring(3);
										info.EffectivitDate = "20" + c.Substring(3, 2) + "-" + c.Substring(5, 2) + "-" + c.Substring(7, 2);
									}
									else if (c.StartsWith("10)"))
									{
										_ph = c.Substring(3);
										info.BatchNumber = _ph;
									}
									else if (c.StartsWith("21)"))
									{
										_xlh = c.Substring(3);
										info.SerialNumber = _xlh;
									}
								}

								UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
								if (model != null)
								{
									if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
									{
										bool IsHav = dal.GetUDIAnalyse(model);
										if (IsHav == false)
										{
											result.code = 1004;
											result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
											result.data = new UDIAnalyseInfo();
											return result;
										}
									}
									UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
									if (res.code != 0)
									{
										info.CodeTypeName = "GS1";
										result.code = res.code;
										result.Msg = res.Msg;
										result.data = info;
										return result;
									}
									info.EnterpriseName = model.EnterpriseName;
									info.BusinessLicence = model.BusinessLicence;
									info.MaterialName = model.MaterialName;
									info.Specification = model.MaterialSpec;
									info.CodeTypeName = model.CodeTypeName;
									if (UDIKey.MaterialCount == -1)
										info.MaterialMS = model.cpms;
								}
								else
								{
									UDIMaterialModel = QueryDI(info.DI);
									if (UDIMaterialModel != null)
									{
										if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
										{
											bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
											if (IsHav == false)
											{
												result.code = 1004;
												result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
												result.data = new UDIAnalyseInfo();
												return result;
											}
										}
										info.EnterpriseName = UDIMaterialModel.EnterpriseName;
										info.BusinessLicence = UDIMaterialModel.BusinessLicence;
										info.MaterialName = UDIMaterialModel.MaterialName;
										info.Specification = UDIMaterialModel.MaterialSpec;
										info.CodeTypeName = UDIMaterialModel.CodeTypeName;
										if (UDIKey.MaterialCount == -1)
											info.MaterialMS = UDIMaterialModel.cpms;
									}
									else
									{
										info.CodeTypeName = "GS1";
										result.code = 0;
										result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
										result.data = info;
										return result;
									}

								}
								result.code = 0;
								result.Msg = "解析成功！";
								result.data = info;
								return result;

							}
							#endregion

							#region 新模式（可解析GS1码不带括号的、13位商品编码）
							else
							{
								//只要GS1码大于等于13位即可
								if (GS1CodeNum >= 13)
								{
									//13位为GS1普通商品编码
									if (GS1CodeNum == 13)
									{
										info.DI = _mcode;
										UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
										if (model != null)
										{
											if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
											{
												bool IsHav = dal.GetUDIAnalyse(model);
												if (IsHav == false)
												{
													result.code = 1004;
													result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
													result.data = new UDIAnalyseInfo();
													return result;
												}
											}
											UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
											if (res.code != 0)
											{
												result.code = res.code;
												result.Msg = res.Msg;
												result.data = info;
												return result;
											}
											info.EnterpriseName = model.EnterpriseName;
											info.BusinessLicence = model.BusinessLicence;
											info.MaterialName = model.MaterialName;
											info.Specification = model.MaterialSpec;
											info.CodeTypeName = model.CodeTypeName;
											if (UDIKey.MaterialCount == -1)
												info.MaterialMS = model.cpms;
										}
										else
										{
											UDIMaterialModel = QueryDI(info.DI);
											if (UDIMaterialModel != null)
											{
												if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
												{
													bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
													if (IsHav == false)
													{
														result.code = 1004;
														result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
														result.data = new UDIAnalyseInfo();
														return result;
													}
												}
												info.EnterpriseName = UDIMaterialModel.EnterpriseName;
												info.BusinessLicence = UDIMaterialModel.BusinessLicence;
												info.MaterialName = UDIMaterialModel.MaterialName;
												info.Specification = UDIMaterialModel.MaterialSpec;
												info.CodeTypeName = UDIMaterialModel.CodeTypeName;
												if (UDIKey.MaterialCount == -1)
													info.MaterialMS = UDIMaterialModel.cpms;
											}
											else
											{
												info.CodeTypeName = "GS1";
												result.code = 0;
												result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
												result.data = info;
												return result;
											}
										}
										result.code = 0;
										result.Msg = "解析成功！";
										result.data = info;
										return result;
									}
									else
									{
										//举例：01 12345678901234 17 230105 11 220105 10 220105001 21 0101
										//01产品DI；17 有效期；11 生产日期；10 生产批号；21 序列号
										//_mcode重新赋值，因需要其中的不可见特殊字符来解析21和10段
										//此段代码解析完整UDI，若是DM码，首位为不可见特殊字符，先去掉
										_mcode = CodeValue.TrimStart('\u001d');
										//先把DI截取出来
										if (_mcode.Substring(0, 2) == "01")
										{
											//如果DI是13位商品编码在生成完整UDI码时会自动补0，生成14位UDI-DI编码，
											info.DI = _mcode.Substring(2, 14);
											info.Package = DictPackType[Convert.ToInt32(_mcode.Substring(2, 1))];

											//第一个PI标识
											string FirstAI = "";
											//第二个PI标识
											string SecondAI = "";
											//第三个PI标识
											string ThirdAI = "";
											//第四个PI标识
											string FourthAI = "";

											//截取后剩下的字符串
											string SubAfterCode = "";
											
											//解析第一个PI标识
											if (_mcode.Length >= 18)
											{
												//去掉DI部分后剩下的字符串
												SubAfterCode = _mcode.Substring(16);
												FirstAI = SubAfterCode.Substring(0, 2);
												if (FirstAI == "11")
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FirstAI == "17") 
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FirstAI == "10" || FirstAI == "21")
												{
													//根据不可见字符\u001d进行分割
													string[] PI = SubAfterCode.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
													if (PI.Length > 1) 
													{
														//生产批号
														if (PI[0].Substring(0, 2) == "10")
														{
															info.BatchNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d","");
														}
														//序列号
														if (PI[0].Substring(0, 2) == "21")
														{
															info.SerialNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
													}
												}
											}

											//解析第二个PI标识
											//截取DI和一个PI信息后如果后面有值，一定剩下2个PI标识字符
											if (SubAfterCode.Length > 2) 
											{
												SecondAI = SubAfterCode.Substring(0, 2);
												if (SecondAI == "11") 
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (SecondAI == "17")
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (SecondAI == "10" || SecondAI=="21")
												{
													if (FirstAI == "10" || FirstAI == "21")
													{
														string SpecialPI = "";
														//大于等于18说明包含11和17
														if (SubAfterCode.Length > 18 && string.IsNullOrEmpty(SpecialPI))
														{
															SpecialPI = SubAfterCode.Substring(0, SubAfterCode.Length - 14);
														}
														//大于等于10说明包含11或17
														if (SubAfterCode.Length > 10 && string.IsNullOrEmpty(SpecialPI))
														{
															SpecialPI = SubAfterCode.Substring(0, SubAfterCode.Length - 8);
														}
														//大于等于2说明仅包含10或21
														if (SubAfterCode.Length > 2 && string.IsNullOrEmpty(SpecialPI))
														{
															SpecialPI = SubAfterCode;
														}
														//生产批号
														if (SpecialPI.Substring(0, 2) == "10")
														{
															info.BatchNumber = SpecialPI.Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
														//序列号
														if (SpecialPI.Substring(0, 2) == "21")
														{
															info.SerialNumber = SpecialPI.Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
													}
													else 
													{
														//根据不可见字符\u001d进行分割
														string[] PI = SubAfterCode.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
														if (PI.Length > 1)
														{
															//生产批号
															if (PI[0].Substring(0, 2) == "10")
															{
																info.BatchNumber = PI[0].Substring(2);
																SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
																SubAfterCode = SubAfterCode.Replace("\u001d", "");
															}
															//序列号
															if (PI[0].Substring(0, 2) == "21")
															{
																info.SerialNumber = PI[0].Substring(2);
																SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
																SubAfterCode = SubAfterCode.Replace("\u001d", "");
															}
														}
													}
												}
											}

											//解析第三个PI标识
											//截取DI和两个PI信息后如果后面有值，一定剩下2个PI标识字符
											if (SubAfterCode.Length > 2) 
											{
												ThirdAI = SubAfterCode.Substring(0, 2);
												if (ThirdAI == "11")
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (ThirdAI == "17")
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (ThirdAI == "10" || ThirdAI == "21")
												{

													//根据不可见字符\u001d进行分割
													string[] PI = SubAfterCode.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
													if (PI.Length > 1)
													{
														//生产批号
														if (PI[0].Substring(0, 2) == "10")
														{
															info.BatchNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
														//序列号
														if (PI[0].Substring(0, 2) == "21")
														{
															info.SerialNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
													}
												}
											}

											//解析第四个PI标识
											//截取DI和三个PI信息后如果后面有值，一定剩下2个PI标识字符
											if (SubAfterCode.Length > 2) 
											{
												FourthAI = SubAfterCode.Substring(0, 2);
												if (FourthAI == "11")
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FourthAI == "17")
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FourthAI == "10")
												{
													info.BatchNumber = SubAfterCode.Substring(2);
												}
												if (FourthAI == "21")
												{
													info.SerialNumber = SubAfterCode.Substring(2);
												}
											}

											UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
											if (model != null)
											{
												if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
												{
													bool IsHav = dal.GetUDIAnalyse(model);
													if (IsHav == false)
													{
														result.code = 1004;
														result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
														result.data = new UDIAnalyseInfo();
														return result;
													}
												}
												UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
												if (res.code != 0)
												{
													info.CodeTypeName = "GS1";
													result.code = res.code;
													result.Msg = res.Msg;
													result.data = info;
													return result;
												}
												info.EnterpriseName = model.EnterpriseName;
												info.BusinessLicence = model.BusinessLicence;
												info.MaterialName = model.MaterialName;
												info.Specification = model.MaterialSpec;
												info.CodeTypeName = model.CodeTypeName;
												if (UDIKey.MaterialCount == -1)
													info.MaterialMS = model.cpms;
											}
											else
											{
												UDIMaterialModel = QueryDI(info.DI);
												if (UDIMaterialModel != null)
												{
													if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
													{
														bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
														if (IsHav == false)
														{
															result.code = 1004;
															result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
															result.data = new UDIAnalyseInfo();
															return result;
														}
													}
													info.EnterpriseName = UDIMaterialModel.EnterpriseName;
													info.BusinessLicence = UDIMaterialModel.BusinessLicence;
													info.MaterialName = UDIMaterialModel.MaterialName;
													info.Specification = UDIMaterialModel.MaterialSpec;
													info.CodeTypeName = UDIMaterialModel.CodeTypeName;
													if (UDIKey.MaterialCount == -1)
														info.MaterialMS = UDIMaterialModel.cpms;
												}
												else
												{
													info.CodeTypeName = "GS1";
													result.code = 0;
													result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
													result.data = info;
													return result;
												}
											}
											result.code = 0;
											result.Msg = "解析成功！";
											result.data = info;
											return result;
										}
										else
										{
											result.code = 1005;
											result.Msg = "请输入正确的码内容！";
											result.data = info;
											return result;
										}
									}
								}
								else
								{
									result.code = 1005;
									result.Msg = "GS1最小位数为13位，请重新输入正确的GS1码内容！";
									result.data = info;
									return result;
								}
							}
							#endregion

						}
						else
						{
							result.code = 1005;
							result.Msg = "GS1最小位数为13位，请重新输入正确的GS1码内容！";
							result.data = info;
							return result;
						}

					}
					//XML格式的GS1码解析
					else if (CodeFormat == (int)CodeFormats.XML)
					{
						if (CodeValue.Contains("<UDI IAC=\"GS1\">"))
						{
							if (CodeValue.Count(i => "<UDI IAC=\"GS1\">".Contains(i)) > 1)
							{
								CodeValue = CodeValue.Replace("</UDI><UDI IAC=\"GS1\">", "");
							}
							XmlDocument xmlDoc = new XmlDocument();
							xmlDoc.LoadXml(CodeValue);
							XmlNode xn = xmlDoc.SelectSingleNode("UDI");
							XmlNodeList xnl = xn.ChildNodes;
							foreach (XmlNode xn1 in xnl)
							{
								XmlElement xe = (XmlElement)xn1;
								switch (xe.Name)
								{
									case "DI":
										info.DI = xe.InnerText;
										break;
									case "SN":
										info.SerialNumber = xe.InnerText;
										break;
									case "MFG":
										info.ProductDate = xe.InnerText;
										break;
									case "LOT":
										info.BatchNumber = xe.InnerText;
										break;
									case "EXP":
										info.InvalDate = xe.InnerText;
										break;
									case "VAL":
										info.EffectivitDate = xe.InnerText;
										break;
								}
							}
							if (info.DI.Length > 13)
							{
								info.Package = DictPackType[Convert.ToInt32(info.DI.Substring(0, 1))];
							}
							UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
							if (model != null)
							{
								if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
								{
									bool IsHav = dal.GetUDIAnalyse(model);
									if (IsHav == false)
									{
										result.code = 1004;
										result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
										result.data = new UDIAnalyseInfo();
										return result;
									}
								}
								UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
								if (res.code != 0)
								{
									info.CodeTypeName = "GS1";
									result.code = res.code;
									result.Msg = res.Msg;
									result.data = info;
									return result;
								}
								info.EnterpriseName = model.EnterpriseName;
								info.BusinessLicence = model.BusinessLicence;
								info.MaterialName = model.MaterialName;
								info.Specification = model.MaterialSpec;
								info.CodeTypeName = model.CodeTypeName;
								if (UDIKey.MaterialCount == -1)
									info.MaterialMS = model.cpms;
							}
							else
							{
								UDIMaterialModel = QueryDI(info.DI);
								if (UDIMaterialModel != null)
								{
									if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
									{
										bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
										if (IsHav == false)
										{
											result.code = 1004;
											result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
											result.data = new UDIAnalyseInfo();
											return result;
										}
									}
									info.EnterpriseName = UDIMaterialModel.EnterpriseName;
									info.BusinessLicence = UDIMaterialModel.BusinessLicence;
									info.MaterialName = UDIMaterialModel.MaterialName;
									info.Specification = UDIMaterialModel.MaterialSpec;
									info.CodeTypeName = UDIMaterialModel.CodeTypeName;
									if (UDIKey.MaterialCount == -1)
										info.MaterialMS = UDIMaterialModel.cpms;
								}
								else
								{
									info.CodeTypeName = "GS1";
									result.code = 0;
									result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
									result.data = info;
									return result;
								}
							}
							result.code = 0;
							result.Msg = "解析成功！";
							result.data = info;
							return result;
						}
						else
						{
							result.code = 1005;
							result.Msg = "XML码格式错误，请输入正确的码内容！";
							result.data = info;
							return result;
						}

					}
					//JSON格式的GS1码解析
					else if (CodeFormat == (int)CodeFormats.json)
					{
						result.code = 1009;
						result.Msg = "暂未开通JSON解析！";
						result.data = info;
						return result;
					}
					else
					{
						result.code = 1008;
						result.Msg = "码格式输入错误，请重新输入！";
						result.data = info;
						return result;
					}
				}
				#endregion

				#region 若码解析成功，code=0，但没有企业信息

				#endregion
			}
			catch (Exception ex)
			{
				result.code = -1;
				result.Msg = "解析失败：" + ex.Message;
				result.data = info;
				return result;
			}
		}


		public UDIBindResult UDIBind(string CodeValue, string key, int CodeFormat = 0)
		{
			UDIAnalyseDAL dal = new UDIAnalyseDAL();
			UDIBindResult result = new UDIBindResult();
			UDIAnalyseInfo UDIAnalyseInfo = new UDIAnalyseInfo();
			List<string> info = new List<string>();
			List<string> SameBindData = new List<string>();
			try
			{
				if (string.IsNullOrEmpty(CodeValue))
				{
					result.code = 1006;
					result.Msg = "码内容不能为空！";
					result.data = info;
					result.SameBindData = SameBindData;
					return result;
				}
				if (string.IsNullOrEmpty(key))
				{
					result.code = 1006;
					result.Msg = "授权Token不能为空！";
					result.data = info;
					result.SameBindData = SameBindData;
					return result;
				}
				UDIKey UDIKey = dal.getUDIKey(key);
				if (UDIKey == null)
				{
					result.code = 1001;
					result.Msg = "解析失败：授权Token不正确！";
					result.data = info;
					result.SameBindData = SameBindData;
					return result;
				}
				if (UDIKey.Status != (int)Common.EnumFile.Status.used)
				{
					result.code = 1002;
					result.Msg = "解析失败：该授权Token已停用！";
					result.data = info;
					result.SameBindData = SameBindData;
					return result;
				}
				bool IsLicenseExpire = DateTime.Compare(Convert.ToDateTime(UDIKey.EndDate), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0 ? false : true;
				if (IsLicenseExpire)
				{
					result.code = 1003;
					result.Msg = "解析失败：该授权Token已到期！";
					result.data = info;
					result.SameBindData = SameBindData;
					return result;
				}
				#region MA码
				if (CodeValue.Substring(0, 2) == "MA" || CodeValue.Contains("<UDI IAC=\"MA\">"))
				{
					if (CodeFormat == (int)CodeFormats.defaultcode)
					{
						if (CodeValue.Contains("="))
						{
							int m = CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Count();
							if (m != 2)
							{
								result.code = 1005;
								result.Msg = "请输入正确的码内容！";
								result.data = info;
								result.SameBindData = SameBindData;
								return result;
							}
						}
						string _mcode = CodeValue.Contains("=") ? CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1] : CodeValue;
						List<string> codes = _mcode.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
						if (codes.Count < 5)
						{
							result.code = 1005;
							result.Msg = "请输入正确的码内容！";
							result.data = info;
							result.SameBindData = SameBindData;
							return result;
						}
						using (DataClassesDataContext db = GetDataContext())
						{
							//下级
							var bindB = db.BindCodeRecords.FirstOrDefault(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
							//找上级
							var bindS = db.BindCodeRecords.FirstOrDefault(m => m.SingleCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
							if (bindB != null)
							{
								List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete).ToList();
								foreach (var item in singCodeLst)
								{
									info.Add(item.SingleCode);
								}
								result.code = 1;
								result.Msg = "查询成功！";
								result.BoxCode = CodeValue;
								result.data = info;
								if (bindS != null)
								{
									singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										SameBindData.Add(item.SingleCode);
									}
									result.SameBindData = SameBindData;
									result.TopBoxCode = bindS.BoxCode;
								}
								else
								{
									result.SameBindData = SameBindData;
								}
								return result;
							}
							else if (bindS != null)
							{
								List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
								foreach (var item in singCodeLst)
								{
									SameBindData.Add(item.SingleCode);
								}
								result.TopBoxCode = bindS.BoxCode;
								result.code = 1;
								result.Msg = "查询成功！";
								result.data = info;
								result.SameBindData = SameBindData;
								return result;
							}
							else
							{
								result.code = 3;
								result.Msg = "无此码包装记录！";
								result.data = info;
								result.SameBindData = SameBindData;
								return result;
							}
						}
					}
					else if (CodeFormat == (int)CodeFormats.XML)
					{
						info = new List<string>();
						if (!CodeValue.Contains("<UDI IAC=\"MA\">"))
						{
							result.code = 1005;
							result.Msg = "XML码格式错误，请输入正确的码内容！";
							result.BoxCode = "";
							result.data = info;
							result.SameBindData = SameBindData;
							return result;
						}
						string MA_Code = "";
						UDIAnalyseInfo.DI = "";
						UDIAnalyseInfo.SerialNumber = "";
						UDIAnalyseInfo.ProductDate = "";
						UDIAnalyseInfo.BatchNumber = "";
						UDIAnalyseInfo.InvalDate = "";
						UDIAnalyseInfo.EffectivitDate = "";
						UDIAnalyseInfo.SterilizationNumber = "";
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(CodeValue);
						XmlNode xn = xmlDoc.SelectSingleNode("UDI");
						XmlNodeList xnl = xn.ChildNodes;
						foreach (XmlNode xn1 in xnl)
						{
							XmlElement xe = (XmlElement)xn1;
							switch (xe.Name)
							{
								case "DI":
									UDIAnalyseInfo.DI = xe.InnerText;
									break;
								case "SN":
									UDIAnalyseInfo.SerialNumber = xe.InnerText;
									break;
								case "MFG":
									UDIAnalyseInfo.ProductDate = xe.InnerText;
									break;
								case "LOT":
									UDIAnalyseInfo.BatchNumber = xe.InnerText;
									break;
								case "EXP":
									UDIAnalyseInfo.InvalDate = xe.InnerText;
									break;
								case "VAL":
									UDIAnalyseInfo.EffectivitDate = xe.InnerText;
									break;
								default:
									UDIAnalyseInfo.SterilizationNumber = xe.InnerText;
									break;
							}
						}

						if (!string.IsNullOrEmpty(UDIAnalyseInfo.DI))
						{
							MA_Code += UDIAnalyseInfo.DI;
						}
						if (!string.IsNullOrEmpty(UDIAnalyseInfo.SerialNumber))
						{
							MA_Code += ".S" + UDIAnalyseInfo.SerialNumber;
						}
						if (!string.IsNullOrEmpty(UDIAnalyseInfo.ProductDate))
						{
							MA_Code += ".M" + UDIAnalyseInfo.ProductDate;
						}
						if (!string.IsNullOrEmpty(UDIAnalyseInfo.BatchNumber))
						{
							MA_Code += ".L" + UDIAnalyseInfo.BatchNumber;
						}
						if (!string.IsNullOrEmpty(UDIAnalyseInfo.SterilizationNumber))
						{
							MA_Code += ".D" + UDIAnalyseInfo.SterilizationNumber;
						}
						if (!string.IsNullOrEmpty(UDIAnalyseInfo.InvalDate))
						{
							MA_Code += ".E" + UDIAnalyseInfo.InvalDate;
						}
						if (!string.IsNullOrEmpty(UDIAnalyseInfo.EffectivitDate))
						{
							MA_Code += ".V" + UDIAnalyseInfo.EffectivitDate;
						}

						List<string> codes = UDIAnalyseInfo.DI.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
						if (codes.Count < 5)
						{
							result.code = 1005;
							result.Msg = "请输入正确的码内容！";
							result.BoxCode = "";
							result.data = info;
							result.SameBindData = SameBindData;
							return result;
						}
						using (DataClassesDataContext db = GetDataContext())
						{
							var bindB = db.BindCodeRecords.FirstOrDefault(m => m.BoxCode.Contains(MA_Code) && m.Status != (int)EnumFile.Status.delete);
							var bindS = db.BindCodeRecords.FirstOrDefault(m => m.SingleCode.Contains(MA_Code) && m.Status != (int)EnumFile.Status.delete);
							if (bindB != null)
							{
								List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode.Contains(MA_Code) && m.Status != (int)EnumFile.Status.delete).ToList();
								foreach (var item in singCodeLst)
								{
									info.Add(item.SingleCode);
								}
								if (singCodeLst.Count > 0)
								{
									result.BoxCode = singCodeLst[0].BoxCode;
								}
								result.code = 1;
								result.Msg = "查询成功！";
								result.data = info;
								if (bindS != null)
								{
									singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										SameBindData.Add(item.SingleCode);
									}
									result.SameBindData = SameBindData;
									result.TopBoxCode = bindS.BoxCode;
								}
								else
								{
									result.SameBindData = SameBindData;
								}
								return result;
							}
							else if (bindS != null)
							{
								List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
								foreach (var item in singCodeLst)
								{
									SameBindData.Add(item.SingleCode);
								}
								result.TopBoxCode = bindS.BoxCode;
								result.code = 1;
								result.Msg = "查询成功！";
								result.data = info;
								result.SameBindData = SameBindData;
								return result;
							}
							else
							{
								result.code = 3;
								result.Msg = "无此码包装记录！";
								result.BoxCode = "";
								result.data = info;
								result.SameBindData = SameBindData;
								return result;
							}
						}
					}
					else if (CodeFormat == (int)CodeFormats.json)
					{
						result.code = 1009;
						result.Msg = "暂未开通JSON解析！";
						result.data = info;
						result.SameBindData = SameBindData;
						return result;
					}
					else
					{
						result.code = 1008;
						result.Msg = "码格式输入错误，请重新输入！";
						result.data = info;
						result.SameBindData = SameBindData;
						return result;
					}

				}
				else
				{
					result.code = 0;
					result.Msg = "不支持此类型码查询！";
					result.data = info;
					result.SameBindData = SameBindData;
					return result;
				}
				#endregion
			}
			catch (Exception ex)
			{
				result.code = -1;
				result.Msg = "查询失败：" + ex.Message;
				result.data = info;
				result.SameBindData = SameBindData;
				return result;
			}
		}


		public UDIMergeResult UDIMerge(string CodeValue, string key, int? CodeFormat)
		{
			UDIAnalyseDAL dal = new UDIAnalyseDAL();
			//UDIAnalyseBLL UDIAnalyseBLL = new UDIAnalyseBLL();
			UDIMergeResult result = new UDIMergeResult();
			UDIAnalyseInfo info = new UDIAnalyseInfo();
			UDIMaterial UDIMaterialModel = new UDIMaterial();
			List<string> CodeBindInfo = new List<string>();
			List<string> SameBindData = new List<string>();
			try
			{
				#region 码格式及token判断
				if (string.IsNullOrEmpty(CodeValue))
				{
					result.code = 1006;
					result.Msg = "码内容不能为空！";
					result.data = info;
					return result;
				}
				if (string.IsNullOrEmpty(key))
				{
					result.code = 1006;
					result.Msg = "授权Token不能为空！";
					result.data = info;
					return result;
				}
				UDIKey UDIKey = dal.getUDIKey(key);
				if (UDIKey == null)
				{
					result.code = 1001;
					result.Msg = "解析失败：授权Token不正确！";
					result.data = info;
					return result;
				}
				if (UDIKey.Status != (int)Common.EnumFile.Status.used)
				{
					result.code = 1002;
					result.Msg = "解析失败：该授权Token已停用！";
					result.data = info;
					return result;
				}
				bool IsLicenseExpire = DateTime.Compare(Convert.ToDateTime(UDIKey.EndDate), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0 ? false : true;
				if (IsLicenseExpire)
				{
					result.code = 1003;
					result.Msg = "解析失败：该授权Token已到期！";
					result.data = info;
					return result;
				}
				int num = dal.getUDIAnalyseCount(UDIKey.ID);
				if (num > UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
				{
					result.code = 1004;
					result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
					result.data = info;
					return result;
				}
				#endregion

				#region AHM码
				//MF：产品DI（不定长,不带MF最多13位）；BA：生产批号（不定长）；SN：序列号（不定长）；MD：生产日期；BD：保质日期；PL：包装规格；ED：有效期
				if (CodeValue.Substring(0, 2) == "MF")
				{
					if (CodeFormat == (int)CodeFormats.defaultcode)
					{
						string _MD = "", _BD = "", _ED = "", _PL = "", _SN = "", _BA = "";
						if (CodeValue.IndexOf("BA") > 0)
						{
							string[] str = CodeValue.Split(new string[] { "BA" }, StringSplitOptions.RemoveEmptyEntries);
							if (str.Length > 0)
							{
								info.DI = str[0];
								if (str.Length >= 2)
								{
									string[] strs = str[1].Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
									if (strs.Length > 0)
									{
										_BA = strs[0];
										info.BatchNumber = _BA;
									}
									foreach (string item in strs)
									{
										if (item.IndexOf("SN") > 0)
										{
											_SN = item.Substring(CodeValue.IndexOf("SN") + 2);
											info.SerialNumber = _SN;
										}
									}
								}
							}
						}


						if (CodeValue.IndexOf("MD") > 0)
						{
							_MD = CodeValue.Substring(CodeValue.IndexOf("MD") + 2, 6);
							info.ProductDate = _MD;
						}
						if (CodeValue.IndexOf("BD") > 0)
						{
							_BD = CodeValue.Substring(CodeValue.IndexOf("BD") + 2, 6);
							info.InvalDate = _BD;
						}
						if (CodeValue.IndexOf("ED") > 0)
						{
							_ED = CodeValue.Substring(CodeValue.IndexOf("ED") + 2, 6);
							info.EffectivitDate = _ED;
						}
						if (CodeValue.IndexOf("PL") > 0)
						{
							_PL = CodeValue.Substring(CodeValue.IndexOf("PL") + 2, 1);
							info.Package = DictPackType[Convert.ToInt32(_PL)];
						}
						try
						{
							using (DataClassesDataContext db = GetDataContext())
							{
								var bindB = db.BindCodeRecords.FirstOrDefault(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
								var bindS = db.BindCodeRecords.FirstOrDefault(m => m.SingleCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
								if (bindB != null)
								{
									List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										CodeBindInfo.Add(item.SingleCode);
									}
									if (bindS != null)
									{
										singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
										foreach (var item in singCodeLst)
										{
											SameBindData.Add(item.SingleCode);
										}
										result.SameBindData = SameBindData;
										result.TopBoxCode = bindS.BoxCode;
									}
									else
									{
										result.SameBindData = SameBindData;
									}
									result.BoxCode = CodeValue;
								}
								else if (bindS != null)
								{
									List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										SameBindData.Add(item.SingleCode);
									}
									result.TopBoxCode = bindS.BoxCode;
									result.SameBindData = SameBindData;
									result.BoxCode = bindS.BoxCode;
								}
							}
						}
						catch (Exception ex)
						{
							CodeBindInfo = new List<string>();
						}
						result.code = 0;
						result.Msg = "暂未开通阿里健康码解析功能，稍后再试！";
						result.data = info;
						result.BindData = CodeBindInfo;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.XML)
					{
						result.code = 1009;
						result.Msg = "阿里健康码暂未开通XML解析！";
						result.data = info;
						result.BindData = CodeBindInfo;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.json)
					{
						result.code = 1009;
						result.Msg = "阿里健康码暂未开通JSON解析！";
						result.data = info;
						result.BindData = CodeBindInfo;
						return result;
					}
					else
					{
						result.code = 1008;
						result.Msg = "码格式输入错误，请重新输入！";
						result.data = info;
						result.BindData = CodeBindInfo;
						return result;
					}
				}
				#endregion

				#region MA码
				else if (CodeValue.Substring(0, 2) == "MA" || CodeValue.Contains("<UDI IAC=\"MA\">"))
				{
					if (CodeFormat == (int)CodeFormats.defaultcode)
					{
						CodeBindInfo = new List<string>();
						if (CodeValue.Contains("<UDI IAC=\"MA\">"))
						{
							result.code = 1008;
							result.Msg = "当前码为XML格式，参数码格式输入错误，请重新输入！";
							result.data = info;
							result.BindData = CodeBindInfo;
							result.SameBindData = SameBindData;
							return result;
						}
						if (CodeValue.Contains("="))
						{
							int m = CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Count();
							if (m != 2)
							{
								result.code = 1005;
								result.Msg = "请输入正确的码内容！";
								result.data = info;
								result.BindData = CodeBindInfo;
								result.SameBindData = SameBindData;
								return result;
							}
						}
						string _mcode = CodeValue.Contains("=") ? CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1] : CodeValue;
						List<string> codes = _mcode.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
						if (codes.Count < 5)
						{
							result.code = 1005;
							result.Msg = "请输入正确的码内容！";
							result.data = info;
							result.BindData = CodeBindInfo;
							result.SameBindData = SameBindData;
							return result;
						}
						info.Package = DictPackType[Convert.ToInt32(codes[4].Substring(0, 1))];
						string MADICODE = String.Join(".", codes.Take(5).ToArray());
						string MAPICODE = _mcode.Replace(MADICODE + ".", "");
						string _m = "", _v = "", _e = "", _l = "", _s = "", _d = "";
						info.DI = MADICODE;
						var _backCodes = codes.Skip(5).ToList();
						foreach (string c in _backCodes)
						{
							if (c.StartsWith("M"))
							{
								//_m = "生产日期:" + c.Substring(1);
								//_m = "20" + c.Substring(1);
								info.ProductDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("P"))
							{
								//_m = "生产日期:" + c.Substring(1);
								info.ProductDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("V"))
							{
								//_v = "有效期:" + c.Substring(1);
								info.EffectivitDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("E"))
							{
								//_e = "失效日期:" + c.Substring(1);
								info.InvalDate = Convert.ToDateTime(("20" + c.Substring(1)).Substring(0, 4) + "-" + ("20" + c.Substring(1)).Substring(4, 2) + "-" + ("20" + c.Substring(1)).Substring(6, 2)).ToString("yyyy-MM-dd");
							}
							else if (c.StartsWith("L"))
							{
								//_l = "生产批号:" + c.Substring(1);
								info.BatchNumber = c.Substring(1);
							}
							else if (c.StartsWith("D"))
							{
								//_d = "灭菌批号:" + c.Substring(1);
								info.SterilizationNumber = c.Substring(1);
							}
							else if (c.StartsWith("S"))
							{
								//_s = "序列号:" + c.Substring(1);
								info.SerialNumber = c.Substring(1);
							}
						}

						try
						{
							using (DataClassesDataContext db = GetDataContext())
							{
								var bindB = db.BindCodeRecords.FirstOrDefault(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
								var bindS = db.BindCodeRecords.FirstOrDefault(m => m.SingleCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
								if (bindB != null)
								{
									List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										CodeBindInfo.Add(item.SingleCode);
									}
									if (bindS != null)
									{
										singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
										foreach (var item in singCodeLst)
										{
											SameBindData.Add(item.SingleCode);
										}
										result.TopBoxCode = bindS.BoxCode;
										result.SameBindData = SameBindData;
									}
									else
									{
										result.SameBindData = SameBindData;
									}
									result.BoxCode = CodeValue;
								}
								else if (bindS != null)
								{
									List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										SameBindData.Add(item.SingleCode);
									}
									result.TopBoxCode = bindS.BoxCode;
									result.SameBindData = SameBindData;
									result.BoxCode = bindS.BoxCode;
								}
							}
						}
						catch (Exception ex)
						{
							CodeBindInfo = new List<string>();
						}

						UDIMaterial model = dal.getUDIMaterial(MADICODE, "MA码（IDcode）");
						if (model != null)
						{
							if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
							{
								bool IsHav = dal.GetUDIAnalyse(model);
								if (IsHav == false)
								{
									result.code = 1004;
									result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
									result.data = new UDIAnalyseInfo();
									result.BindData = CodeBindInfo;
									result.SameBindData = SameBindData;
									return result;
								}
							}
							UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
							if (res.code != 0)
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = res.code;
								result.Msg = res.Msg;
								result.data = info;
								result.BindData = CodeBindInfo;
								result.SameBindData = SameBindData;
								return result;
							}
							info.EnterpriseName = model.EnterpriseName;
							info.BusinessLicence = model.BusinessLicence;
							info.MaterialName = model.MaterialName;
							info.Specification = model.MaterialSpec;
							info.CodeTypeName = model.CodeTypeName;
							if (UDIKey.MaterialCount == -1)
								info.MaterialMS = model.cpms;
						}
						else
						{
							UDIMaterialModel = QueryDI(info.DI);
							if (UDIMaterialModel != null)
							{
								if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
								{
									bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
									if (IsHav == false)
									{
										result.code = 1004;
										result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
										result.data = new UDIAnalyseInfo();
										return result;
									}
								}
								info.EnterpriseName = UDIMaterialModel.EnterpriseName;
								info.BusinessLicence = UDIMaterialModel.BusinessLicence;
								info.MaterialName = UDIMaterialModel.MaterialName;
								info.Specification = UDIMaterialModel.MaterialSpec;
								info.CodeTypeName = UDIMaterialModel.CodeTypeName;
								if (UDIKey.MaterialCount == -1)
									info.MaterialMS = UDIMaterialModel.cpms;
							}
							else
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = 0;
								result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
								result.data = info;
								result.BindData = CodeBindInfo;
								result.SameBindData = SameBindData;
								return result;
							}
						}

						result.code = 0;
						result.Msg = "解析成功！";
						result.data = info;
						result.BindData = CodeBindInfo;
						result.SameBindData = SameBindData;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.XML)
					{
						CodeBindInfo = new List<string>();
						if (!CodeValue.Contains("<UDI IAC=\"MA\">"))
						{
							result.code = 1005;
							result.Msg = "XML码格式错误，请输入正确的码内容！";
							result.data = info;
							result.BindData = CodeBindInfo;
							result.SameBindData = SameBindData;
							return result;
						}
						string MA_Code = "";
						info.DI = "";
						info.SerialNumber = "";
						info.ProductDate = "";
						info.BatchNumber = "";
						info.InvalDate = "";
						info.EffectivitDate = "";
						info.SterilizationNumber = "";
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(CodeValue);
						XmlNode xn = xmlDoc.SelectSingleNode("UDI");
						XmlNodeList xnl = xn.ChildNodes;
						foreach (XmlNode xn1 in xnl)
						{
							XmlElement xe = (XmlElement)xn1;
							switch (xe.Name)
							{
								case "DI":
									info.DI = xe.InnerText;
									break;
								case "SN":
									info.SerialNumber = xe.InnerText;
									break;
								case "MFG":
									info.ProductDate = xe.InnerText;
									break;
								case "LOT":
									info.BatchNumber = xe.InnerText;
									break;
								case "EXP":
									info.InvalDate = xe.InnerText;
									break;
								case "VAL":
									info.EffectivitDate = xe.InnerText;
									break;
								default:
									info.SterilizationNumber = xe.InnerText;
									break;
							}
						}

						if (!string.IsNullOrEmpty(info.DI))
						{
							MA_Code += info.DI;
						}
						if (!string.IsNullOrEmpty(info.SerialNumber))
						{
							MA_Code += ".S" + info.SerialNumber;
						}
						if (!string.IsNullOrEmpty(info.ProductDate))
						{
							MA_Code += ".M" + info.ProductDate;
						}
						if (!string.IsNullOrEmpty(info.BatchNumber))
						{
							MA_Code += ".L" + info.BatchNumber;
						}
						if (!string.IsNullOrEmpty(info.SterilizationNumber))
						{
							MA_Code += ".D" + info.SterilizationNumber;
						}
						if (!string.IsNullOrEmpty(info.InvalDate))
						{
							MA_Code += ".E" + info.InvalDate;
						}
						if (!string.IsNullOrEmpty(info.EffectivitDate))
						{
							MA_Code += ".V" + info.EffectivitDate;
						}

						List<string> codes = info.DI.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
						if (codes.Count < 5)
						{
							result.code = 1005;
							result.Msg = "请输入正确的码内容！";
							result.data = info;
							result.BindData = CodeBindInfo;
							result.SameBindData = SameBindData;
							return result;
						}
						info.Package = DictPackType[Convert.ToInt32(codes[4].Substring(0, 1))];

						try
						{
							using (DataClassesDataContext db = GetDataContext())
							{
								//下级
								var bindB = db.BindCodeRecords.FirstOrDefault(m => m.BoxCode.Contains(MA_Code) && m.Status != (int)EnumFile.Status.delete);
								var bindS = db.BindCodeRecords.FirstOrDefault(m => m.SingleCode.Contains(MA_Code) && m.Status != (int)EnumFile.Status.delete);
								if (bindB != null)
								{
									List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode.Contains(MA_Code) && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										CodeBindInfo.Add(item.SingleCode);
									}
									if (bindS != null)
									{
										singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
										foreach (var item in singCodeLst)
										{
											SameBindData.Add(item.SingleCode);
										}
										result.TopBoxCode = bindS.BoxCode;
										result.SameBindData = SameBindData;
									}
									else
									{
										result.SameBindData = SameBindData;
									}
									if (singCodeLst.Count > 0)
									{
										result.BoxCode = singCodeLst[0].BoxCode;
									}
								}
								else if (bindS != null)
								{
									List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
									foreach (var item in singCodeLst)
									{
										SameBindData.Add(item.SingleCode);
									}
									result.TopBoxCode = bindS.BoxCode;
									result.SameBindData = SameBindData;
									result.BoxCode = bindS.BoxCode;
								}
							}
						}
						catch (Exception ex)
						{
							CodeBindInfo = new List<string>();
						}

						UDIMaterial model = dal.getUDIMaterial(info.DI, "MA码（IDcode）");
						if (model != null)
						{
							if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
							{
								bool IsHav = dal.GetUDIAnalyse(model);
								if (IsHav == false)
								{
									result.code = 1004;
									result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
									result.data = new UDIAnalyseInfo();
									result.BindData = CodeBindInfo;
									result.SameBindData = SameBindData;
									return result;
								}
							}
							UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
							if (res.code != 0)
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = res.code;
								result.Msg = res.Msg;
								result.data = info;
								result.BindData = CodeBindInfo;
								result.SameBindData = SameBindData;
								return result;
							}
							info.EnterpriseName = model.EnterpriseName;
							info.BusinessLicence = model.BusinessLicence;
							info.MaterialName = model.MaterialName;
							info.Specification = model.MaterialSpec;
							info.CodeTypeName = model.CodeTypeName;
							if (UDIKey.MaterialCount == -1)
								info.MaterialMS = model.cpms;
						}
						else
						{
							UDIMaterialModel = QueryDI(info.DI);
							if (UDIMaterialModel != null)
							{
								if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
								{
									bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
									if (IsHav == false)
									{
										result.code = 1004;
										result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
										result.data = new UDIAnalyseInfo();
										return result;
									}
								}
								info.EnterpriseName = UDIMaterialModel.EnterpriseName;
								info.BusinessLicence = UDIMaterialModel.BusinessLicence;
								info.MaterialName = UDIMaterialModel.MaterialName;
								info.Specification = UDIMaterialModel.MaterialSpec;
								info.CodeTypeName = UDIMaterialModel.CodeTypeName;
								if (UDIKey.MaterialCount == -1)
									info.MaterialMS = UDIMaterialModel.cpms;
							}
							else
							{
								info.CodeTypeName = "MA码（IDcode）";
								result.code = 0;
								result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
								result.data = info;
								result.BindData = CodeBindInfo;
								result.SameBindData = SameBindData;
								return result;
							}
						}

						result.code = 0;
						result.Msg = "解析成功！";
						result.data = info;
						result.BindData = CodeBindInfo;
						result.SameBindData = SameBindData;
						return result;
					}
					else if (CodeFormat == (int)CodeFormats.json)
					{
						CodeBindInfo = new List<string>();
						result.code = 1009;
						result.Msg = "暂未开通JSON解析！";
						result.data = info;
						result.BindData = CodeBindInfo;
						result.SameBindData = SameBindData;
						return result;
					}
					else
					{
						CodeBindInfo = new List<string>();
						result.code = 1008;
						result.Msg = "码格式输入错误，请重新输入！";
						result.BindData = CodeBindInfo;
						result.data = info;
						result.SameBindData = SameBindData;
						return result;
					}
				}
				#endregion

				#region GS1码
				else
				{
					CodeBindInfo = new List<string>();
					//以正常码内容来解析
					if (CodeFormat == (int)CodeFormats.defaultcode)
					{
						if (CodeValue.Length >= 13)
						{
							if (CodeValue.Contains("="))
							{
								int m = CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Count();
								if (m != 2)
								{
									result.code = 1005;
									result.Msg = "输入的GS1码包含'='，但该码并未解析成两段，请输入正确的码内容！";
									result.data = info;
									return result;
								}
							}

							//扫码可能携带网址，需去掉网址后获取码内容解析
							string _mcode = CodeValue.Contains("=") ? CodeValue.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1] : CodeValue;

							//分别是生产日期、有效期、批号、序列号和DI 
							string _scrq = "", _yxq = "", _ph = "", _xlh = "", _DI = "";

							//第一步去掉不可见特殊符号
							_mcode = _mcode.Contains("\u001d") ? _mcode.Replace("\u001d", "") : _mcode;

							//GS1码前几位字符;根据前几位判断，一种带括号的（01），一种不带括号的 01
							string BeforeCode = _mcode.Substring(0, 4);

							//GS1码位数；13位的为普通GS1商品码；大于13位的为GS1-UDI码（DI部分或整个UDI码）
							int GS1CodeNum = _mcode.Length;

							#region 兼容旧模式（可解析GS1码带括号的、13位的商品编码）
							if (BeforeCode == "(01)" || GS1CodeNum == 13)
							{
								List<string> codes = _mcode.Split(new Char[] { '(' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

								if (codes.Count == 1)
								{
									if (codes[0].Length == 13)
									{
										info.DI = codes[0];
									}
									else if (codes[0].Contains("01)"))
									{
										info.Package = DictPackType[Convert.ToInt32(codes[0].Substring(3, 1))];
									}
									else if (codes[0].Length == 14)
									{
										info.DI = codes[0];
										info.Package = DictPackType[Convert.ToInt32(codes[0].Substring(0, 1))];
									}
									else
									{
										result.code = 1005;
										result.Msg = "请输入正确的码内容！";
										result.data = info;
										result.BindData = CodeBindInfo;
										result.SameBindData = SameBindData;
										return result;
									}
								}

								foreach (string c in codes)
								{
									if (c.StartsWith("01)"))
									{
										_DI = c.Substring(3);
										info.DI = _DI;
										if (_DI.Length == 14)
										{
											info.Package = DictPackType[Convert.ToInt32(c.Substring(3, 1))];
										}
									}
									if (c.StartsWith("11)"))
									{
										//_scrq = "生产日期:" + "20" + c.Substring(3);
										info.ProductDate = "20" + c.Substring(3, 2) + "-" + c.Substring(5, 2) + "-" + c.Substring(7, 2);
									}
									else if (c.StartsWith("17)"))
									{
										//_yxq = "有效期:" + c.Substring(3);
										info.EffectivitDate = "20" + c.Substring(3, 2) + "-" + c.Substring(5, 2) + "-" + c.Substring(7, 2);
									}
									else if (c.StartsWith("10)"))
									{
										_ph = c.Substring(3);
										info.BatchNumber = _ph;
									}
									else if (c.StartsWith("21)"))
									{
										_xlh = c.Substring(3);
										info.SerialNumber = _xlh;
									}
								}
								try
								{
									string dkcode = CodeValue.Replace("(", "").Replace(")", "");
									using (DataClassesDataContext db = GetDataContext())
									{
										var bindB = db.BindCodeRecords.FirstOrDefault(m => (m.BoxCode == CodeValue || m.BoxCode == dkcode) && m.Status != (int)EnumFile.Status.delete);
										var bindS = db.BindCodeRecords.FirstOrDefault(m => (m.SingleCode == CodeValue || m.SingleCode == dkcode) && m.Status != (int)EnumFile.Status.delete);
										if (bindB != null)
										{
											List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => (m.BoxCode == CodeValue || m.BoxCode == dkcode) && m.Status != (int)EnumFile.Status.delete).ToList();
											foreach (var item in singCodeLst)
											{
												CodeBindInfo.Add(item.SingleCode);
											}
											if (bindS != null)
											{
												singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
												foreach (var item in singCodeLst)
												{
													SameBindData.Add(item.SingleCode);
												}
												result.TopBoxCode = bindS.BoxCode;
												result.SameBindData = SameBindData;
											}
											else
											{
												result.SameBindData = SameBindData;
											}
											result.BoxCode = CodeValue;
										}
										else if (bindS != null)
										{
											List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
											foreach (var item in singCodeLst)
											{
												SameBindData.Add(item.SingleCode);
											}
											result.TopBoxCode = bindS.BoxCode;
											result.SameBindData = SameBindData;
											result.BoxCode = bindS.BoxCode;
										}
									}
								}
								catch (Exception ex)
								{
									CodeBindInfo = new List<string>();
								}
								UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
								if (model != null)
								{
									if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
									{
										bool IsHav = dal.GetUDIAnalyse(model);
										if (IsHav == false)
										{
											result.code = 1004;
											result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
											result.data = new UDIAnalyseInfo();
											result.BindData = CodeBindInfo;
											result.SameBindData = SameBindData;
											return result;
										}
									}
									UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
									if (res.code != 0)
									{
										info.CodeTypeName = "GS1";
										result.code = res.code;
										result.Msg = res.Msg;
										result.data = info;
										result.SameBindData = SameBindData;
										result.BindData = CodeBindInfo;
										return result;
									}
									info.EnterpriseName = model.EnterpriseName;
									info.BusinessLicence = model.BusinessLicence;
									info.MaterialName = model.MaterialName;
									info.Specification = model.MaterialSpec;
									info.CodeTypeName = model.CodeTypeName;
									if (UDIKey.MaterialCount == -1)
										info.MaterialMS = model.cpms;
								}
								else
								{
									UDIMaterialModel = QueryDI(info.DI);
									if (UDIMaterialModel != null)
									{
										if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
										{
											bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
											if (IsHav == false)
											{
												result.code = 1004;
												result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
												result.data = new UDIAnalyseInfo();
												return result;
											}
										}
										info.EnterpriseName = UDIMaterialModel.EnterpriseName;
										info.BusinessLicence = UDIMaterialModel.BusinessLicence;
										info.MaterialName = UDIMaterialModel.MaterialName;
										info.Specification = UDIMaterialModel.MaterialSpec;
										info.CodeTypeName = UDIMaterialModel.CodeTypeName;
										if (UDIKey.MaterialCount == -1)
											info.MaterialMS = UDIMaterialModel.cpms;
									}
									else
									{
										info.CodeTypeName = "GS1";
										result.code = 0;
										result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
										result.data = info;
										result.BindData = CodeBindInfo;
										result.SameBindData = SameBindData;
										return result;
									}
								}
								result.code = 0;
								result.Msg = "解析成功！";
								result.BindData = CodeBindInfo;
								result.data = info;
								result.SameBindData = SameBindData;
								return result;

							}
							#endregion

							#region 新模式（可解析GS1码不带括号的、13位商品编码）
							else
							{
								//只要GS1码大于等于13位即可
								if (GS1CodeNum >= 13)
								{
									//13位为GS1普通商品编码
									if (GS1CodeNum == 13)
									{
										info.DI = _mcode;
										try
										{
											using (DataClassesDataContext db = GetDataContext())
											{
												var bindB = db.BindCodeRecords.FirstOrDefault(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
												var bindS = db.BindCodeRecords.FirstOrDefault(m => m.SingleCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
												if (bindB != null)
												{
													List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete).ToList();
													foreach (var item in singCodeLst)
													{
														CodeBindInfo.Add(item.SingleCode);
													}
													if (bindS != null)
													{
														singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
														foreach (var item in singCodeLst)
														{
															SameBindData.Add(item.SingleCode);
														}
														result.TopBoxCode = bindS.BoxCode;
														result.SameBindData = SameBindData;
													}
													else
													{
														result.SameBindData = SameBindData;
													}
													result.BoxCode = CodeValue;
												}
												else if (bindS != null)
												{
													List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
													foreach (var item in singCodeLst)
													{
														SameBindData.Add(item.SingleCode);
													}
													result.TopBoxCode = bindS.BoxCode;
													result.SameBindData = SameBindData;
													result.BoxCode = bindS.BoxCode;
												}
											}
										}
										catch (Exception ex)
										{
											CodeBindInfo = new List<string>();
										}
										UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
										if (model != null)
										{
											if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
											{
												bool IsHav = dal.GetUDIAnalyse(model);
												if (IsHav == false)
												{
													result.code = 1004;
													result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
													result.data = new UDIAnalyseInfo();
													result.BindData = CodeBindInfo;
													result.SameBindData = SameBindData;
													return result;
												}
											}
											UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
											if (res.code != 0)
											{
												result.code = res.code;
												result.Msg = res.Msg;
												result.data = info;
												result.BindData = CodeBindInfo;
												result.SameBindData = SameBindData;
												return result;
											}
											info.EnterpriseName = model.EnterpriseName;
											info.BusinessLicence = model.BusinessLicence;
											info.MaterialName = model.MaterialName;
											info.Specification = model.MaterialSpec;
											info.CodeTypeName = model.CodeTypeName;
											if (UDIKey.MaterialCount == -1)
												info.MaterialMS = model.cpms;
										}
										else
										{

											UDIMaterialModel = QueryDI(info.DI);
											if (UDIMaterialModel != null)
											{
												if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
												{
													bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
													if (IsHav == false)
													{
														result.code = 1004;
														result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
														result.data = new UDIAnalyseInfo();
														return result;
													}
												}
												info.EnterpriseName = UDIMaterialModel.EnterpriseName;
												info.BusinessLicence = UDIMaterialModel.BusinessLicence;
												info.MaterialName = UDIMaterialModel.MaterialName;
												info.Specification = UDIMaterialModel.MaterialSpec;
												info.CodeTypeName = UDIMaterialModel.CodeTypeName;
												if (UDIKey.MaterialCount == -1)
													info.MaterialMS = UDIMaterialModel.cpms;
											}
											else
											{
												info.CodeTypeName = "GS1";
												result.code = 0;
												result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
												result.data = info;
												result.BindData = CodeBindInfo;
												result.SameBindData = SameBindData;
												return result;
											}
										}
										result.code = 0;
										result.Msg = "解析成功！";
										result.data = info;
										result.BindData = CodeBindInfo;
										result.SameBindData = SameBindData;
										return result;
									}
									else
									{
										//举例：01 12345678901234 17 230105 11 220105 10 220105001 21 0101
										//01产品DI；17 有效期；11 生产日期；10 生产批号；21 序列号
										//_mcode重新赋值，因需要其中的不可见特殊字符来解析21和10段
										//此段代码解析完整UDI，若是DM码，首位为不可见特殊字符，先去掉
										_mcode = CodeValue.TrimStart('\u001d');

										//先把DI截取出来
										if (_mcode.Substring(0, 2) == "01")
										{
											//如果DI是13位商品编码在生成完整UDI码时会自动补0，生成14位UDI-DI编码，
											info.DI = _mcode.Substring(2, 14);
											info.Package = DictPackType[Convert.ToInt32(_mcode.Substring(2, 1))];


											//第一个PI标识
											string FirstAI = "";
											//第二个PI标识
											string SecondAI = "";
											//第三个PI标识
											string ThirdAI = "";
											//第四个PI标识
											string FourthAI = "";

											//截取后剩下的字符串
											string SubAfterCode = "";

											//解析第一个PI标识
											if (_mcode.Length >= 18)
											{
												//去掉DI部分后剩下的字符串
												SubAfterCode = _mcode.Substring(16);
												FirstAI = SubAfterCode.Substring(0, 2);
												if (FirstAI == "11")
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FirstAI == "17")
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FirstAI == "10" || FirstAI == "21")
												{
													//根据不可见字符\u001d进行分割
													string[] PI = SubAfterCode.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
													if (PI.Length > 1)
													{
														//生产批号
														if (PI[0].Substring(0, 2) == "10")
														{
															info.BatchNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
														//序列号
														if (PI[0].Substring(0, 2) == "21")
														{
															info.SerialNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
													}
												}
											}

											//解析第二个PI标识
											//截取DI和一个PI信息后如果后面有值，一定剩下2个PI标识字符
											if (SubAfterCode.Length > 2)
											{
												SecondAI = SubAfterCode.Substring(0, 2);
												if (SecondAI == "11")
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (SecondAI == "17")
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (SecondAI == "10" || SecondAI == "21")
												{
													if (FirstAI == "10" || FirstAI == "21")
													{
														string SpecialPI = "";
														//大于等于18说明包含11和17
														if (SubAfterCode.Length > 18 && string.IsNullOrEmpty(SpecialPI))
														{
															SpecialPI = SubAfterCode.Substring(0, SubAfterCode.Length - 14);
														}
														//大于等于10说明包含11或17
														if (SubAfterCode.Length > 10 && string.IsNullOrEmpty(SpecialPI))
														{
															SpecialPI = SubAfterCode.Substring(0, SubAfterCode.Length - 8);
														}
														//大于等于2说明仅包含10或21
														if (SubAfterCode.Length > 2 && string.IsNullOrEmpty(SpecialPI))
														{
															SpecialPI = SubAfterCode;
														}
														//生产批号
														if (SpecialPI.Substring(0, 2) == "10")
														{
															info.BatchNumber = SpecialPI.Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
														//序列号
														if (SpecialPI.Substring(0, 2) == "21")
														{
															info.SerialNumber = SpecialPI.Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
													}
													else
													{
														//根据不可见字符\u001d进行分割
														string[] PI = SubAfterCode.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
														if (PI.Length > 1)
														{
															//生产批号
															if (PI[0].Substring(0, 2) == "10")
															{
																info.BatchNumber = PI[0].Substring(2);
																SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
																SubAfterCode = SubAfterCode.Replace("\u001d", "");
															}
															//序列号
															if (PI[0].Substring(0, 2) == "21")
															{
																info.SerialNumber = PI[0].Substring(2);
																SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
																SubAfterCode = SubAfterCode.Replace("\u001d", "");
															}
														}
													}
												}
											}

											//解析第三个PI标识
											//截取DI和两个PI信息后如果后面有值，一定剩下2个PI标识字符
											if (SubAfterCode.Length > 2)
											{
												ThirdAI = SubAfterCode.Substring(0, 2);
												if (ThirdAI == "11")
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (ThirdAI == "17")
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (ThirdAI == "10" || ThirdAI == "21")
												{

													//根据不可见字符\u001d进行分割
													string[] PI = SubAfterCode.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
													if (PI.Length > 1)
													{
														//生产批号
														if (PI[0].Substring(0, 2) == "10")
														{
															info.BatchNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
														//序列号
														if (PI[0].Substring(0, 2) == "21")
														{
															info.SerialNumber = PI[0].Substring(2);
															SubAfterCode = SubAfterCode.Substring(2 + info.BatchNumber.Length);
															SubAfterCode = SubAfterCode.Replace("\u001d", "");
														}
													}
												}
											}

											//解析第四个PI标识
											//截取DI和三个PI信息后如果后面有值，一定剩下2个PI标识字符
											if (SubAfterCode.Length > 2)
											{
												FourthAI = SubAfterCode.Substring(0, 2);
												if (FourthAI == "11")
												{
													info.ProductDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉11PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FourthAI == "17")
												{
													info.EffectivitDate = "20" + SubAfterCode.Substring(2, 2) + "-" + SubAfterCode.Substring(4, 2) + "-" + SubAfterCode.Substring(6, 2);
													//去掉17PI后剩下的字符串
													SubAfterCode = SubAfterCode.Substring(8);
												}
												if (FourthAI == "10")
												{
													info.BatchNumber = SubAfterCode.Substring(2);
												}
												if (FourthAI == "21")
												{
													info.SerialNumber = SubAfterCode.Substring(2);
												}
											}

											#region 
											//if (_mcode.Length >= 18)
											//{
											//    string FirstAI = _mcode.Substring(16, 2);
											//    if (_mcode.Length >= 24)
											//    {
											//        if (FirstAI == "11")
											//            info.ProductDate = "20" + _mcode.Substring(18, 2) + "-" + _mcode.Substring(20, 2) + "-" + _mcode.Substring(22, 2);

											//        if (FirstAI == "17")
											//            info.EffectivitDate = "20" + _mcode.Substring(18, 2) + "-" + _mcode.Substring(20, 2) + "-" + _mcode.Substring(22, 2);
											//        if (FirstAI == "10" || FirstAI == "21")
											//        {
											//            //截取剩下的PI信息
											//            string ThirdAI = _mcode.Substring(16);
											//            //根据不可见字符\u001d进行分割
											//            string[] PI = ThirdAI.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
											//            foreach (string str in PI)
											//            {
											//                //生产批号
											//                if (str.Substring(0, 2) == "10")
											//                {
											//                    info.BatchNumber = str.Substring(2);
											//                }
											//                //序列号
											//                if (str.Substring(0, 2) == "21")
											//                {
											//                    info.SerialNumber = str.Substring(2);
											//                }
											//            }
											//        }
											//    }

											//}
											//if (_mcode.Length >= 26)
											//{
											//    string SecondAI = _mcode.Substring(24, 2);
											//    if (_mcode.Length >= 32)
											//    {
											//        if (SecondAI == "11")
											//            info.ProductDate = "20" + _mcode.Substring(26, 2) + "-" + _mcode.Substring(28, 2) + "-" + _mcode.Substring(30, 2);
											//        if (SecondAI == "17")
											//            info.EffectivitDate = "20" + _mcode.Substring(26, 2) + "-" + _mcode.Substring(28, 2) + "-" + _mcode.Substring(30, 2);
											//        if (SecondAI == "10" || SecondAI == "21")
											//        {
											//            //截取剩下的PI信息
											//            string ThirdAI = _mcode.Substring(24);
											//            //根据不可见字符\u001d进行分割
											//            string[] PI = ThirdAI.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
											//            foreach (string str in PI)
											//            {
											//                //生产批号
											//                if (str.Substring(0, 2) == "10")
											//                {
											//                    info.BatchNumber = str.Substring(2);
											//                }
											//                //序列号
											//                if (str.Substring(0, 2) == "21")
											//                {
											//                    info.SerialNumber = str.Substring(2);
											//                }
											//            }
											//        }
											//    }
											//}
											//if (_mcode.Length >= 34)
											//{
											//    //截取剩下的PI信息
											//    string ThirdAI = _mcode.Substring(32);
											//    //根据不可见字符\u001d进行分割
											//    string[] PI = ThirdAI.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
											//    foreach (string str in PI)
											//    {
											//        //生产批号
											//        if (str.Substring(0, 2) == "10")
											//        {
											//            info.BatchNumber = str.Substring(2);
											//        }
											//        //序列号
											//        if (str.Substring(0, 2) == "21")
											//        {
											//            info.SerialNumber = str.Substring(2);
											//        }
											//    }
											//}
											#endregion

											try
											{
												//拼接带括号GS1码；
												string dkcode = "(01)"+info.DI;
												if (info.EffectivitDate != "-") 
												{
													dkcode += "(17)" + info.EffectivitDate;
												}
												if (info.ProductDate != "-")
												{
													dkcode += "(11)" + info.ProductDate;
												}
												if (info.BatchNumber != "-")
												{
													dkcode += "(10)" + info.BatchNumber;
												}
												if (info.SerialNumber != "-")
												{
													dkcode += "\u001d(21)" + info.SerialNumber;
												}
												using (DataClassesDataContext db = GetDataContext())
												{
													var bindB = db.BindCodeRecords.FirstOrDefault(m => (m.BoxCode == CodeValue || m.BoxCode == dkcode) && m.Status != (int)EnumFile.Status.delete);
													var bindS = db.BindCodeRecords.FirstOrDefault(m => (m.SingleCode == CodeValue || m.SingleCode == dkcode) && m.Status != (int)EnumFile.Status.delete);
													if (bindB != null)
													{
														List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => (m.BoxCode == CodeValue || m.BoxCode == dkcode) && m.Status != (int)EnumFile.Status.delete).ToList();
														foreach (var item in singCodeLst)
														{
															CodeBindInfo.Add(item.SingleCode);
														}
														if (bindS != null)
														{
															singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
															foreach (var item in singCodeLst)
															{
																SameBindData.Add(item.SingleCode);
															}
															result.TopBoxCode = bindS.BoxCode;
															result.SameBindData = SameBindData;
														}
														else
														{
															result.SameBindData = SameBindData;
														}
														result.BoxCode = CodeValue;
													}
													else if (bindS != null)
													{
														List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
														foreach (var item in singCodeLst)
														{
															SameBindData.Add(item.SingleCode);
														}
														result.TopBoxCode = bindS.BoxCode;
														result.SameBindData = SameBindData;
														result.BoxCode = bindS.BoxCode;
													}
												}
											}
											catch (Exception ex)
											{
												CodeBindInfo = new List<string>();
											}
											UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
											if (model != null)
											{
												if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
												{
													bool IsHav = dal.GetUDIAnalyse(model);
													if (IsHav == false)
													{
														result.code = 1004;
														result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
														result.data = new UDIAnalyseInfo();
														result.BindData = CodeBindInfo;
														result.SameBindData = SameBindData;
														return result;
													}
												}
												UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
												if (res.code != 0)
												{
													info.CodeTypeName = "GS1";
													result.code = res.code;
													result.Msg = res.Msg;
													result.data = info;
													result.BindData = CodeBindInfo;
													result.SameBindData = SameBindData;
													return result;
												}
												info.EnterpriseName = model.EnterpriseName;
												info.BusinessLicence = model.BusinessLicence;
												info.MaterialName = model.MaterialName;
												info.Specification = model.MaterialSpec;
												info.CodeTypeName = model.CodeTypeName;
												if (UDIKey.MaterialCount == -1)
													info.MaterialMS = model.cpms;
											}
											else
											{
												UDIMaterialModel = QueryDI(info.DI);
												if (UDIMaterialModel != null)
												{
													if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
													{
														bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
														if (IsHav == false)
														{
															result.code = 1004;
															result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
															result.data = new UDIAnalyseInfo();
															return result;
														}
													}
													info.EnterpriseName = UDIMaterialModel.EnterpriseName;
													info.BusinessLicence = UDIMaterialModel.BusinessLicence;
													info.MaterialName = UDIMaterialModel.MaterialName;
													info.Specification = UDIMaterialModel.MaterialSpec;
													info.CodeTypeName = UDIMaterialModel.CodeTypeName;
													if (UDIKey.MaterialCount == -1)
														info.MaterialMS = UDIMaterialModel.cpms;
												}
												else
												{
													info.CodeTypeName = "GS1";
													result.code = 0;
													result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
													result.data = info;
													result.BindData = CodeBindInfo;
													result.SameBindData = SameBindData;
													return result;
												}
											}
											result.code = 0;
											result.Msg = "解析成功！";
											result.data = info;
											result.BindData = CodeBindInfo;
											result.SameBindData = SameBindData;
											return result;
										}
										else
										{
											result.code = 1005;
											result.Msg = "请输入正确的码内容！";
											result.data = info;
											result.BindData = CodeBindInfo;
											result.SameBindData = SameBindData;
											return result;
										}
									}
								}
								else
								{
									result.code = 1005;
									result.Msg = "GS1最小位数为13位，请重新输入正确的GS1码内容！";
									result.data = info;
									result.BindData = CodeBindInfo;
									result.SameBindData = SameBindData;
									return result;
								}
							}
							#endregion

						}
						else
						{
							result.code = 1005;
							result.Msg = "GS1最小位数为13位，请重新输入正确的GS1码内容！";
							result.data = info;
							result.BindData = CodeBindInfo;
							result.SameBindData = SameBindData;
							return result;
						}

					}
					//XML格式的GS1码解析
					else if (CodeFormat == (int)CodeFormats.XML)
					{
						if (CodeValue.Contains("<UDI IAC=\"GS1\">"))
						{
							if (CodeValue.Count(i => "<UDI IAC=\"GS1\">".Contains(i)) > 1)
							{
								CodeValue = CodeValue.Replace("</UDI><UDI IAC=\"GS1\">", "");
							}
							XmlDocument xmlDoc = new XmlDocument();
							xmlDoc.LoadXml(CodeValue);
							XmlNode xn = xmlDoc.SelectSingleNode("UDI");
							XmlNodeList xnl = xn.ChildNodes;
							foreach (XmlNode xn1 in xnl)
							{
								XmlElement xe = (XmlElement)xn1;
								switch (xe.Name)
								{
									case "DI":
										info.DI = xe.InnerText;
										break;
									case "SN":
										info.SerialNumber = xe.InnerText;
										break;
									case "MFG":
										info.ProductDate = xe.InnerText;
										break;
									case "LOT":
										info.BatchNumber = xe.InnerText;
										break;
									case "EXP":
										info.InvalDate = xe.InnerText;
										break;
									case "VAL":
										info.EffectivitDate = xe.InnerText;
										break;
								}
							}
							if (info.DI.Length > 13)
							{
								info.Package = DictPackType[Convert.ToInt32(info.DI.Substring(0, 1))];
							}
							try
							{
								using (DataClassesDataContext db = GetDataContext())
								{
									var bindB = db.BindCodeRecords.FirstOrDefault(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
									var bindS = db.BindCodeRecords.FirstOrDefault(m => m.SingleCode == CodeValue && m.Status != (int)EnumFile.Status.delete);
									if (bindB != null)
									{
										List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == CodeValue && m.Status != (int)EnumFile.Status.delete).ToList();
										foreach (var item in singCodeLst)
										{
											CodeBindInfo.Add(item.SingleCode);
										}
										if (bindS != null)
										{
											singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
											foreach (var item in singCodeLst)
											{
												SameBindData.Add(item.SingleCode);
											}
											result.TopBoxCode = bindS.BoxCode;
											result.SameBindData = SameBindData;
										}
										else
										{
											result.SameBindData = SameBindData;
										}
										result.BoxCode = CodeValue;
									}
									else if (bindS != null)
									{
										List<BindCodeRecords> singCodeLst = db.BindCodeRecords.Where(m => m.BoxCode == bindS.BoxCode && m.Status != (int)EnumFile.Status.delete).ToList();
										foreach (var item in singCodeLst)
										{
											SameBindData.Add(item.SingleCode);
										}
										result.TopBoxCode = bindS.BoxCode;
										result.SameBindData = SameBindData;
										result.BoxCode = bindS.BoxCode;
									}
								}
							}
							catch (Exception ex)
							{
								CodeBindInfo = new List<string>();
							}
							UDIMaterial model = dal.getUDIMaterial(info.DI, "GS1");
							if (model != null)
							{
								if (num == UDIKey.MaterialCount && UDIKey.MaterialCount != -1)
								{
									bool IsHav = dal.GetUDIAnalyse(model);
									if (IsHav == false)
									{
										result.code = 1004;
										result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
										result.data = new UDIAnalyseInfo();
										result.BindData = CodeBindInfo;
										result.SameBindData = SameBindData;
										return result;
									}
								}
								UDIAnalyseResult res = dal.UpdateUDIAnalyse(model, UDIKey.ID);
								if (res.code != 0)
								{
									info.CodeTypeName = "GS1";
									result.code = res.code;
									result.Msg = res.Msg;
									result.data = info;
									result.BindData = CodeBindInfo;
									result.SameBindData = SameBindData;
									return result;
								}
								info.EnterpriseName = model.EnterpriseName;
								info.BusinessLicence = model.BusinessLicence;
								info.MaterialName = model.MaterialName;
								info.Specification = model.MaterialSpec;
								info.CodeTypeName = model.CodeTypeName;
								if (UDIKey.MaterialCount == -1)
									info.MaterialMS = model.cpms;
							}
							else
							{
								UDIMaterialModel = QueryDI(info.DI);
								if (UDIMaterialModel != null)
								{
									if (num == UDIKey.MaterialCount&&UDIKey.MaterialCount!=-1)
									{
										bool IsHav = dal.GetUDIAnalyse(UDIMaterialModel);
										if (IsHav == false)
										{
											result.code = 1004;
											result.Msg = "解析失败：该授权Token解析产品个数超限，请续购！";
											result.data = new UDIAnalyseInfo();
											return result;
										}
									}
									info.EnterpriseName = UDIMaterialModel.EnterpriseName;
									info.BusinessLicence = UDIMaterialModel.BusinessLicence;
									info.MaterialName = UDIMaterialModel.MaterialName;
									info.Specification = UDIMaterialModel.MaterialSpec;
									info.CodeTypeName = UDIMaterialModel.CodeTypeName;
									if (UDIKey.MaterialCount == -1)
										info.MaterialMS = UDIMaterialModel.cpms;
								}
								else
								{
									info.CodeTypeName = "GS1";
									result.code = 0;
									result.Msg = "暂未查询到该码详细的企业信息，仅返回基本信息！";
									result.data = info;
									result.BindData = CodeBindInfo;
									result.SameBindData = SameBindData;
									return result;
								}
							}
							result.code = 0;
							result.Msg = "解析成功！";
							result.data = info;
							result.BindData = CodeBindInfo;
							result.SameBindData = SameBindData;
							return result;
						}
						else
						{
							result.code = 1005;
							result.Msg = "XML码格式错误，请输入正确的码内容！";
							result.data = info;
							result.BindData = CodeBindInfo;
							result.SameBindData = SameBindData;
							return result;
						}

					}
					//JSON格式的GS1码解析
					else if (CodeFormat == (int)CodeFormats.json)
					{
						result.code = 1009;
						result.Msg = "暂未开通JSON解析！";
						result.data = info;
						result.BindData = CodeBindInfo;
						result.SameBindData = SameBindData;
						return result;
					}
					else
					{
						result.code = 1008;
						result.Msg = "码格式输入错误，请重新输入！";
						result.data = info;
						result.BindData = CodeBindInfo;
						result.SameBindData = SameBindData;
						return result;
					}
				}
				#endregion
			}
			catch (Exception ex)
			{
				result.code = -1;
				result.Msg = "解析失败：" + ex.Message;
				result.data = info;
				result.BindData = CodeBindInfo;
				result.SameBindData = SameBindData;
				return result;
			}
		}

		public UDIMaterial QueryDI(string MaterialDI)
		{
			try
			{
				using (DataClassesDataContext db = GetDataContext())
				{
					UDIMaterialPackage item = db.UDIMaterialPackage.Where(m => m.bzcpbs == MaterialDI).FirstOrDefault();
					if (item != null)
					{
						UDIMaterial model = db.UDIMaterial.Where(m => m.deviceRecordKey == item.deviceRecordKey).FirstOrDefault();
						return model;
					}
					else
					{
						return null;
					}
				}
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		#endregion

	}
}
