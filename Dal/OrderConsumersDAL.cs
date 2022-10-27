/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-11-13

** 联系方式 :13313318725

** 描述：主要用于登录、设置默认信息的数据访问

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/
using System;
using System.Linq;
using LinqModel;
using System.Configuration;

namespace Dal
{
    /// <summary>
    /// 主要用于登录、设置默认信息的数据访问
    /// </summary>
    public class OrderConsumersDAL : DALBase
    {
        /// <summary>
        /// 判断是否注册
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>结果</returns>
        public Result IsRegister(string phone)
        {
            Result result = new Result();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Order_Consumers consumers = dataContext.Order_Consumers.FirstOrDefault(m => m.LinkPhone == phone);
                    if (consumers == null)
                    {
                        result.ResultCode = 0;
                        result.ResultMsg = "未注册";
                    }
                    else
                    {
                        result.ResultCode = 1;
                        result.ResultMsg = "已注册";
                    }
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = -1;
                result.ResultMsg = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>结果</returns>
        public Result GetPassWord(string phone)
        {
            Result result = new Result();
            //短信内容
            string content = string.Empty;
            //返回Json字符串
            string jsonStr = string.Empty;
            //发短信返回的Json串
            string jsonReturn = string.Empty;
            //发短信秘钥
            string secretKey = ConfigurationManager.AppSettings["secretKey"];
            string webServiceUrl = ConfigurationManager.AppSettings["WebServiceUrl"];
            //短信内容类
            MsgInfo msgInfo = new MsgInfo();
            MsgReturn msgReturn = new MsgReturn();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var liCode = dataContext.Order_Code.Where(m => m.PhoneNum == phone);
                    foreach (var item in liCode)
                    {
                        item.IsUsed = 1;
                    }
                    dataContext.SubmitChanges();

                    Order_Code code = new Order_Code();
                    code.AddTime = DateTime.Now;
                    code.IsUsed = 0;
                    code.Code = new Random().Next(100000, 999999).ToString();
                    code.PhoneNum = phone;
                    dataContext.Order_Code.InsertOnSubmit(code);
                    dataContext.SubmitChanges();
                    //组织发送短信内容，转成Json串
                    content = "您好！您在农产品追溯公共服务平台的随机密码为：" + code.Code + "，有效时间3分钟【广联信息】";
                    msgInfo.secretKey = secretKey;
                    msgInfo.sms.sendDate = DateTime.Now.ToString();
                    msgInfo.sms.content = content;
                    msgInfo.sms.msType = 1;
                    msgInfo.sms.sendTime = DateTime.Now.ToString();
                    msgInfo.phoneNumbers[0] = phone;
                    //实例对象
                    SendMsg.SPortService client = new SendMsg.SPortService();
                    client.Url = webServiceUrl;
                    jsonStr = JsonHelper.DataContractJsonSerialize(msgInfo);
                    //发送短信
                    jsonReturn = client.sendSMS(jsonStr);
                    msgReturn = JsonHelper.DataContractJsonDeserialize<MsgReturn>(jsonReturn);
                    if (msgReturn.errorCode == "0")
                    {
                        result.ResultMsg = code.Code;
                        result.ResultCode = 0;
                    }
                    else
                    {
                        result.ResultMsg = "短信发送失败";
                        result.ResultCode = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = -2;
                result.ResultMsg = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="password">密码</param>
        /// <param name="strMsg">返回结果</param>
        /// <returns>登录成功后返回实体</returns>
        public View_Order_Consumers Login(string phone, string password, string messageWord, int loginType, out string strMsg)
        {
            View_Order_Consumers result = null;
            strMsg = "登录失败！";
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    switch (loginType)
                    {
                        case 1:
                            result = dataContext.View_Order_Consumers.FirstOrDefault(m => m.LinkPhone == phone && m.PassWord == password);
                            strMsg = "";
                            if (result == null)
                            {
                                strMsg = "手机号或密码错误，忘记密码可以暂时使用验证码方式登录！";
                            }
                            break;
                        case 2:
                            Order_Code code = dataContext.Order_Code.OrderByDescending(m => m.AddTime).FirstOrDefault(m => m.IsUsed == 0 && m.PhoneNum == phone);
                            //手机号或验证码错误重新获取
                            if (code == null)
                            {
                                strMsg = "没有您的手机号码信息！";
                            }
                            //验证码超时
                            else if (code.AddTime.AddMinutes(3 + 1) < DateTime.Now)
                            {
                                strMsg = "验证码已超时，请重新获取验证码！";
                            }
                            //验证码错误
                            else if (code.Code != messageWord)
                            {
                                strMsg = "验证码输入错误！";
                            }
                            else
                            {
                                Order_Consumers model = dataContext.Order_Consumers.FirstOrDefault(m => m.LinkPhone == phone);
                                if (model == null)
                                {
                                    model = new Order_Consumers();
                                    model.AddDate = DateTime.Now;
                                    model.LinkPhone = phone;
                                    model.PassWord = password;
                                    model.Status = (int)Common.EnumFile.Status.used;
                                    model.UserPhoto = "";
                                    model.IntegralCount = 0;
                                    dataContext.Order_Consumers.InsertOnSubmit(model);
                                    dataContext.SubmitChanges();
                                    Order_Integral consumerIntegral = new Order_Integral();
                                    consumerIntegral.AddDate = DateTime.Now;
                                    consumerIntegral.IntegralType = 1;
                                    consumerIntegral.IntergralChange = 100;
                                    consumerIntegral.Order_Consumers_ID = model.Order_Consumers_ID;
                                    model.IntegralCount = 100;
                                    dataContext.Order_Integral.InsertOnSubmit(consumerIntegral);
                                    dataContext.SubmitChanges();
                                }
                                else
                                {
                                    model.PassWord = password;
                                    dataContext.SubmitChanges();
                                }
                                result = dataContext.View_Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == model.Order_Consumers_ID);
                                strMsg = "";
                            }
                            break;
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 消费者修改密码（输入原密码）
        /// </summary> 
        /// <param name="consumersId">消费者ID</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        public Result EditPwd(long consumersId, string oldPwd, string newPwd)
        {
            Result result = new Result();
            result.ResultCode = -1;
            result.ResultMsg = "修改密码失败！";
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == consumersId);
                    if (!data.PassWord.Equals(oldPwd))
                    {
                        result.ResultCode = -1;
                        result.ResultMsg = "修改失败！旧密码错误！";
                    }
                    else
                    {
                        data.PassWord = newPwd;
                        dataContext.SubmitChanges();
                        result.ResultCode = 1;
                        result.ResultMsg = "密码修改成功，请重新登录！";
                    }
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = -1;
                result.ResultMsg = "连接服务器失败！";
            }
            return result;
        }

        /// <summary>
        /// 消费者修改密码（忘记原密码）
        /// </summary>
        /// <param name="consumersId">消费者ID</param>
        /// <param name="pwd">密码</param>
        /// <returns>返回结果</returns>
        public Result UpdataPwd(long consumersId, string pwd)
        {
            Result result = new Result();
            result.ResultCode = -1;
            result.ResultMsg = "修改密码失败！";
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == consumersId);
                    if (data == null)
                    {
                        result.ResultCode = -1;
                        result.ResultMsg = "没有找到此消费者信息！";
                    }
                    else
                    {
                        data.PassWord = pwd;
                        dataContext.SubmitChanges();
                        result.ResultCode = 1;
                        result.ResultMsg = "密码修改成功，请重新登录！";
                    }
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = -1;
                result.ResultMsg = "连接服务器失败！";
            }
            return result;
        }

        /// <summary>
        /// 根据订单编号获取订单信息
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <returns>返回结果</returns>
        public View_Material_OnlineOrder GetConsumersOrder(string orderNum)
        {
            View_Material_OnlineOrder result = new View_Material_OnlineOrder();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.View_Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == orderNum);
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}
