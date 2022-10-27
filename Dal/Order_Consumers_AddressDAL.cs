/********************************************************************************

** 作者：张翠霞

** 开发时间：2015-11-17

** 联系方式:15630136020

** 代码功能：主要用于消费者收货地址管理底层

** 版本：v1.0

** 版权：研一农业项目组   

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;
using System.Configuration;

namespace Dal
{
    /// <summary>
    /// 消费者地址管理
    /// </summary>
    public class Order_Consumers_AddressDAL : DALBase
    {
        /// <summary>
        /// 获取消费者地址列表
        /// </summary>
        /// <param name="order_Consumers_id">消费者ID</param>
        /// <returns></returns>
        public List<Order_Consumers_Address> GetList(long order_Consumers_id)
        {
            List<Order_Consumers_Address> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Order_Consumers_Address.Where(m => m.Order_Consumers_ID == order_Consumers_id && m.status == (int)Common.EnumFile.Status.used);
                    result = data.OrderByDescending(m => m.Order_Consumers_Address_ID).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "Order_Consumers_AddressDAL.GetList():Order_Consumers_Address表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 消费者添加收货地址
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isDefault">是否设为默认</param>
        /// <param name="consumers">实体</param>
        /// <returns>返回结果</returns>
        public RetResult AddConAddress(Order_Consumers_Address model, bool isDefault, ref View_Order_Consumers consumers, bool updateUser)
        {
            string Msg = "添加收货地址失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    if (updateUser)
                    {
                        Order_Consumers c=dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == model.Order_Consumers_ID);
                        c.UserName = model.LinkMan;
                    }
                    dataContext.Order_Consumers_Address.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    if (isDefault)
                    {
                        Order_Consumers updateConsumers = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == model.Order_Consumers_ID);
                        updateConsumers.Order_Consumers_Address_ID = model.Order_Consumers_Address_ID;
                        dataContext.SubmitChanges();

                        consumers = dataContext.View_Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == model.Order_Consumers_ID);
                    }
                    Msg = "添加收货地址成功";
                    error = CmdResultError.NONE;
                }
            }
            catch
            {
                Msg = "连接服务器失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 消费者修改收货地址 
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>返回结果</returns>
        public RetResult EditConAddress(Order_Consumers_Address model, bool isDefault, ref View_Order_Consumers consumers)
        {
            string Msg = "修改收货地址失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Order_Consumers_Address.FirstOrDefault(m => m.Order_Consumers_ID == model.Order_Consumers_ID && m.Order_Consumers_Address_ID == model.Order_Consumers_Address_ID);
                    //
                    if (data == null)
                    {
                        Msg = "没有找到要修改的数据！";
                    }
                    else
                    {
                        data.LinkMan = model.LinkMan;
                        data.LinkPhone = model.LinkPhone;
                        data.Address = model.Address;
                        data.AreaID = model.AreaID;
                        data.CityID = model.CityID;
                        data.Postcode = model.Postcode;
                        data.ProvinceID = model.ProvinceID;
                        if (isDefault)
                        {
                            Order_Consumers updateConsumers = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == data.Order_Consumers_ID);
                            updateConsumers.Order_Consumers_Address_ID = data.Order_Consumers_Address_ID;

                        }
                        dataContext.SubmitChanges();
                        if (isDefault)
                        {
                            consumers = dataContext.View_Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == model.Order_Consumers_ID);
                        }
                        Msg = "修改收货地址成功！";
                        error = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
                Ret.SetArgument(error, Msg, Msg);
                return Ret;
            }
        }

        /// <summary>
        /// 根据收货地址ID获取信息
        /// </summary>
        /// <param name="consumerAdressId">收货地址ID</param>
        /// <returns>返回结果</returns>
        public Order_Consumers_Address GetModelConAddress(long consumerAdressId)
        {
            Order_Consumers_Address result = new Order_Consumers_Address();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.Order_Consumers_Address.FirstOrDefault(m => m.Order_Consumers_Address_ID == consumerAdressId);
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// 消费者删除收货地址
        /// </summary>
        /// <param name="consumerAdressId">收货地址ID</param>
        /// <param name="order_Consumers_id">消费者ID</param>
        /// <returns>返回结果</returns>
        public RetResult DeleteConAddress(long consumerAdressId, long order_Consumers_id)
        {
            string Msg = "删除收货地址失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Order_Consumers_Address model = dataContext.Order_Consumers_Address.SingleOrDefault(m => m.Order_Consumers_Address_ID == consumerAdressId);

                    if (model == null)
                    {
                        Msg = "没有找到要删除的信息请刷新列表！";
                    }
                    else if (model.Order_Consumers_ID != order_Consumers_id)
                    {
                        Msg = "您无权对该条信息进行操作！";
                    }
                    else
                    {
                        //获取地址判断是否为默认地址
                        Order_Consumers conAddress = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == order_Consumers_id);
                        if (conAddress.Order_Consumers_Address_ID == consumerAdressId)
                        {
                            Msg = "默认地址不可删除！";
                        }
                        else
                        {
                            model.status = (int)Common.EnumFile.Status.delete;
                            //dataContext.Order_Consumers_Address.DeleteOnSubmit(model);
                            dataContext.SubmitChanges();
                            Msg = "删除收货地址成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
            }
            catch
            {
                Msg = "删除收货地址失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
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
                    string now = DateTime.Now.ToString("yyyy-MM-dd");
                    msgInfo.sms.sendDate = now;
                    msgInfo.sms.content = content;
                    msgInfo.sms.msType = 1;
                    msgInfo.sms.sendTime = now;
                    msgInfo.phoneNumbers[0] = phone;
                    //实例对象
                    SendMsg.SPortService client = new SendMsg.SPortService();
                    client.Url = webServiceUrl;
                    jsonStr = JsonHelper.DataContractJsonSerialize(msgInfo);
                    //发送短信
                    jsonStr = jsonStr.Replace(now, "");
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
    }
}
