using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;
using Webdiyer.WebControls.Mvc;
using System.Configuration;
using Dal.SendMsg;

namespace Dal
{
    public class Material_OnlineOrderDAL : DALBase
    {
        public List<Material> GetMaterialList(long enterpriseId)
        {
            List<Material> result = new List<Material>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.Material where (from s in dataContext.Material_Spec where s.Status == (int)Common.EnumFile.Status.used select s.Material_ID).Contains(m.Material_ID) select m;

                    result = data.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                }
                catch { }
            }
            return result;
        }
        public List<Material_Spec> GetMaterialSpecList(long materialId)
        {
            List<Material_Spec> result = new List<Material_Spec>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material_Spec.Where(m => m.Material_ID == materialId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                }
                catch { }
            }
            return result;
        }
        public List<View_MaterialSpecForMarket> GetMarketMaterialSpecList(long MaterialId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_MaterialSpecForMarket
                                    where data.Material_ID == MaterialId
                                    && data.Condition == null
                                    && data.Status == (int)Common.EnumFile.Status.used
                                    select data).ToList();

                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public View_MaterialSpecForMarket GetMarketMaterialSpecModel(long MaterialSpecId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_MaterialSpecForMarket
                                    where data.MaterialSpecId == MaterialSpecId
                                    && data.Condition == null
                                    && data.Status == (int)Common.EnumFile.Status.used
                                    select data).FirstOrDefault();

                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public RetResult Add(Material_OnlineOrder model)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Material material = dataContext.Material.FirstOrDefault(m => m.Material_ID == model.MateriaID);
                    model.MaterialName = material.MaterialFullName;

                    Material_Spec spec = dataContext.Material_Spec.FirstOrDefault(m => m.ID == model.SpecID);
                    model.MaterialSpec = spec.MaterialSpecification;

                    if (model.OrderType == (int)Common.EnumFile.PayStatus.PayDelivery)
                    {
                        model.Activity = "";
                        var propertySpec = dataContext.Material_Spec_Property.Where(m => m.Material_Spec_ID == model.SpecID);
                        if (propertySpec != null && propertySpec.Count() > 0)
                        {
                            foreach (var item in propertySpec)
                            {
                                Material_Property property = dataContext.Material_Property.FirstOrDefault(m => m.Material_Property_ID == item.Material_Property_ID);
                                if (property != null)
                                    model.Activity += property.PropertyName + ",";
                            }
                        }
                    }

                    Order_Consumers_Address address = dataContext.Order_Consumers_Address.FirstOrDefault(m => m.Order_Consumers_Address_ID == Convert.ToInt64(model.Consumers_Address));
                    model.Consumers_Address = Common.Argument.Public.GetAllArea(decimal.Parse(address.AreaID)) + address.Address;
                    model.Consumers_Postcode = address.Postcode;
                    model.Consumers_Name = address.LinkMan;
                    model.Consumers_Phone = address.LinkPhone;
                    model.Status = (int)Common.EnumFile.Status.used;

                    dataContext.Material_OnlineOrder.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    #region 货到付款发送短信通知
                    try
                    {
                        if (model.OrderType == (int)Common.EnumFile.PayStatus.PayDelivery)
                        {
                            MsgInfo msgInfo = new MsgInfo();
                            msgInfo.secretKey = ConfigurationManager.AppSettings["secretKey"];
                            string now = DateTime.Now.ToString("yyyy-MM-dd");
                            msgInfo.sms.sendDate = now;
                            msgInfo.sms.content = "医疗器械追溯公共服务云平台商城有新的订单，请及时处理。【广联信息】";
                            msgInfo.sms.msType = 1;
                            msgInfo.sms.sendTime = now;
                            msgInfo.phoneNumbers[0] =
                                dataContext.Order_EnterpriseAccount.FirstOrDefault(m => m.Enterprise_ID == model.Enterprise_ID).LinkPhone;
                            //实例对象
                            SPortService client = new SPortService();
                            client.Url = ConfigurationManager.AppSettings["WebServiceUrl"]; ;
                            string jsonStr = JsonHelper.DataContractJsonSerialize(msgInfo);
                            //发送短信
                            jsonStr = jsonStr.Replace(now, "");
                            client.sendSMS(jsonStr);
                        }
                    }
                    catch { }
                    #endregion
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, "订购成功！", "订购成功！");
                }
                catch (Exception ex)
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, ex.ToString(), "暂未开放该功能！");
                }
            }
            return Ret;
        }
        public List<View_Material_OnlineOrder> GetList(string name, long eID, string sDate, string eDate, int status, int delStatus, int pageIndex, out long totalCount)
        {
            List<View_Material_OnlineOrder> result = new List<View_Material_OnlineOrder>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Material_OnlineOrder where m.Enterprise_ID == eID && m.Status == delStatus select m;
                    if (!string.IsNullOrEmpty(sDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) > Convert.ToDateTime(sDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) < Convert.ToDateTime(eDate).AddDays(1));
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.Consumers_Name.Contains(name) || m.Consumers_Phone.Contains(name) || m.Consumers_Address.Contains(name) || m.MaterialSpec.Contains(name) || m.MaterialName.Contains(name));
                    }
                    if (status > -1)
                    {
                        data = data.Where(m => m.OrderType == status);
                    }
                    else
                    {
                        data = data.Where(m => m.OrderType <= 6 && m.OrderType >= 0);
                    }
                    data = data.OrderByDescending(m => m.AddTime);
                    totalCount = data.Count();
                    result = data.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    int autoConfim = int.Parse(ConfigurationManager.AppSettings["autoConfirmGoods"] ?? "7");
                    for (int i = 0; i < result.Count; i++)
                    {
                        result[i].textStatus = Common.EnumText.EnumToText(typeof(Common.EnumFile.PayStatus), result[i].OrderType);
                        try
                        {
                            if (result[i].OrderType == (int)Common.EnumFile.PayStatus.Delivered)
                            {
                                if (result[i].DeliveryTime.GetValueOrDefault(DateTime.Now).AddDays(autoConfim) <= DateTime.Now)
                                {
                                    Material_OnlineOrder changeOrder = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.id == result[i].id);
                                    changeOrder.OrderType = (int)Common.EnumFile.PayStatus.Confirm;
                                    changeOrder.DeliveredTime = result[i].DeliveryTime.GetValueOrDefault(DateTime.Now.AddDays(-7)).AddDays(7);
                                    if (Convert.ToInt64((result[i].TotalMoney.ToString()).Substring(0, result[i].TotalMoney.ToString().IndexOf('.'))) != 0)
                                    {
                                        Order_Integral integralModel = new Order_Integral();
                                        integralModel.AddDate = DateTime.Now;
                                        integralModel.IntegralType = 2;
                                        integralModel.IntergralChange = Convert.ToInt64((result[i].TotalMoney.ToString()).Substring(0, result[i].TotalMoney.ToString().IndexOf('.')));
                                        integralModel.Order_Consumers_ID = result[i].Order_Consumers_ID.Value;
                                        dataContext.Order_Integral.InsertOnSubmit(integralModel);
                                    }
                                    dataContext.SubmitChanges();
                                    Order_Consumers consumer = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == result[i].Order_Consumers_ID);
                                    if (consumer != null)
                                    {
                                        long consumerIn = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == result[i].Order_Consumers_ID && m.AddDate >= DateTime.Now.AddYears(-1) && m.AddDate <= DateTime.Now).ToList().Sum(m => m.IntergralChange).Value;
                                        //List<Order_Integral> conIntegral = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == result[i].Order_Consumers_ID && m.AddDate >= DateTime.Now.AddYears(-1) && m.AddDate <= DateTime.Now).ToList();
                                        //long consumerIn = 0;
                                        //if (conIntegral.Count > 0)
                                        //{
                                        //    foreach (var item in conIntegral)
                                        //    {
                                        //        consumerIn += item.IntergralChange.Value;
                                        //    }
                                        //}
                                        consumer.IntegralCount = consumerIn;
                                    }
                                    dataContext.SubmitChanges();
                                }
                            }
                        }
                        catch { continue; }
                    }
                }
                catch { }
            }
            return result;
        }
        public PagedList<View_Material_OnlineOrder> GetList(long ConsumersID, int type, int pageIndex, int pageSize)
        {
            PagedList<View_Material_OnlineOrder> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Material_OnlineOrder where m.Order_Consumers_ID == ConsumersID && m.Status == (int)Common.EnumFile.Status.used select m;
                    if (type > 0)
                    {
                        data = data.Where(m => m.OrderType == type);
                    }
                    data = data.OrderByDescending(m => m.id);
                    result = data.ToPagedList(pageIndex, pageSize);
                    int autoConfim = int.Parse(ConfigurationManager.AppSettings["autoConfirmGoods"] ?? "7");
                    for (int i = 0; i < result.Count; i++)
                    {
                        result[i].textStatus = Common.EnumText.EnumToText(typeof(Common.EnumFile.PayStatus), result[i].OrderType);
                        try
                        {
                            if (result[i].OrderType == (int)Common.EnumFile.PayStatus.Delivered)
                            {
                                if (result[i].DeliveryTime.GetValueOrDefault(DateTime.Now).AddDays(autoConfim) <= DateTime.Now)
                                {
                                    Material_OnlineOrder changeOrder = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.id == result[i].id);
                                    changeOrder.OrderType = (int)Common.EnumFile.PayStatus.Confirm;
                                    changeOrder.DeliveredTime = result[i].DeliveryTime.GetValueOrDefault(DateTime.Now.AddDays(-7)).AddDays(7);
                                    if (Convert.ToInt64((result[i].TotalMoney.ToString()).Substring(0, result[i].TotalMoney.ToString().IndexOf('.'))) != 0)
                                    {
                                        Order_Integral integralModel = new Order_Integral();
                                        integralModel.AddDate = DateTime.Now;
                                        integralModel.IntegralType = 2;
                                        integralModel.IntergralChange = Convert.ToInt64((result[i].TotalMoney.ToString()).Substring(0, result[i].TotalMoney.ToString().IndexOf('.')));
                                        integralModel.Order_Consumers_ID = result[i].Order_Consumers_ID.Value;
                                        dataContext.Order_Integral.InsertOnSubmit(integralModel);
                                    }
                                    dataContext.SubmitChanges();
                                    Order_Consumers consumer = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == result[i].Order_Consumers_ID);
                                    if (consumer != null)
                                    {
                                        long consumerIn = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == result[i].Order_Consumers_ID && m.AddDate >= DateTime.Now.AddYears(-1) && m.AddDate <= DateTime.Now).ToList().Sum(m => m.IntergralChange).Value;
                                        //List<Order_Integral> conIntegral = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == result[i].Order_Consumers_ID && m.AddDate >= DateTime.Now.AddYears(-1) && m.AddDate <= DateTime.Now).ToList();
                                        //long consumerIn = 0;
                                        //if (conIntegral.Count > 0)
                                        //{
                                        //    foreach (var item in conIntegral)
                                        //    {
                                        //        consumerIn += item.IntergralChange.Value;
                                        //    }
                                        //}
                                        consumer.IntegralCount = consumerIn;
                                    }
                                    dataContext.SubmitChanges();
                                }
                            }
                        }
                        catch { continue; }
                    }
                }
                catch
                {
                }
            }
            return result;
        }
        public RetResult Edit(long id, string yundanComp, string yundanNum, long userID)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                Material_OnlineOrder newModel = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.id == id);
                if (newModel != null)
                {
                    bool sendMsg = newModel.OrderType != (int)Common.EnumFile.PayStatus.Delivered;
                    newModel.Status = (int)Common.EnumFile.Status.used;
                    newModel.ExpressNum = yundanNum;
                    newModel.HanderUser = userID;
                    newModel.ExpressComp = yundanComp;

                    //Material_OnlineOrder order = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.id == id);
                    newModel.OrderType = (int)Common.EnumFile.PayStatus.Delivered;
                    newModel.DeliveryTime = DateTime.Now;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                    #region 支付宝发送短信通知
                    if (sendMsg)
                    {
                        try
                        {
                            MsgInfo msgInfo = new MsgInfo();
                            msgInfo.secretKey = ConfigurationManager.AppSettings["secretKey"];
                            string now = DateTime.Now.ToString("yyyy-MM-dd");
                            msgInfo.sms.sendDate = now;
                            msgInfo.sms.content = "您在医疗器械追溯公共服务云平台中购买的商品已发货，请注意查收。【广联信息】";
                            msgInfo.sms.msType = 1;
                            msgInfo.sms.sendTime = now;
                            msgInfo.phoneNumbers[0] =
                                dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == newModel.Order_Consumers_ID).LinkPhone;
                            //实例对象
                            SPortService client = new SPortService();
                            client.Url = ConfigurationManager.AppSettings["WebServiceUrl"];
                            string jsonStr = JsonHelper.DataContractJsonSerialize(msgInfo);
                            //发送短信
                            jsonStr = jsonStr.Replace(now, "");
                            client.sendSMS(jsonStr);
                        }
                        catch { }
                    }
                    #endregion
                }
                else
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
                }
            }
            return Ret;
        }
        public RetResult Del(long id, out int delStatus)
        {
            delStatus = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                Material_OnlineOrder newModel = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.id == id);
                if (newModel != null)
                {
                    if (newModel.OrderType == (int)Common.EnumFile.PayStatus.Trade_Closed)
                    {
                        delStatus = newModel.Status;
                        newModel.Status = newModel.Status == 1 ? 0 : 1;
                        dataContext.SubmitChanges();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "订单" + (delStatus == 0 ? "隐藏" : "显示") + "成功");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "只可以删除消费者交易关闭的订单！");
                    }
                }
                else
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "订单" + (delStatus == 0 ? "隐藏" : "显示") + "失败");
                }
            }
            return Ret;
        }

        public Material_OnlineOrder GetModel(long id)
        {
            Material_OnlineOrder result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.id == id);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        public Material_OnlineOrder GetModel(string orderNum)
        {
            Material_OnlineOrder result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == orderNum);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        public RetResult TrueDel(long id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Material_OnlineOrder newModel = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.id == id);
                    if (newModel != null && newModel.OrderType == (int)Common.EnumFile.PayStatus.Trade_Closed)
                    {
                        newModel.Status = 2;
                        dataContext.SubmitChanges();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "订单删除成功");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "只可以删除消费者交易关闭的订单！");
                    }
                }
            }
            catch
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "订单删除失败！");
            }
            return Ret;
        }

        public List<View_Material_OnlineOrder> ExportExcel(string name, long eID, string sDate, string eDate, int status, int delStatus)
        {
            List<View_Material_OnlineOrder> result = new List<View_Material_OnlineOrder>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Material_OnlineOrder where m.Enterprise_ID == eID select m;
                    if (!string.IsNullOrEmpty(sDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) > Convert.ToDateTime(sDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) < Convert.ToDateTime(eDate).AddDays(1));
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.Consumers_Name.Contains(name) || m.Consumers_Phone.Contains(name) || m.Consumers_Address.Contains(name) || m.MaterialSpec.Contains(name) || m.MaterialName.Contains(name));
                    }
                    if (status > -1)
                    {
                        data = data.Where(m => m.OrderType == status);
                    }
                    else
                    {
                        data = data.Where(m => m.OrderType <= 6 && m.OrderType >= 0);
                    }
                    data = data.OrderByDescending(m => m.AddTime);
                    result = data.ToList();
                }
                catch { }
            }
            return result;
        }
        /// <summary>
        /// 更该订单状态
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <param name="payType">状态</param>
        /// <param name="TradeNo">支付宝订单号</param>
        /// <param name="ConsumerLogin">消费者登录名</param>
        /// <param name="ConsumerNum">消费者支付宝16位唯一标识</param>
        /// <returns>操作结果</returns>
        public RetResult PaySuccess(string orderNum, int payType, string TradeNo, string ConsumerLogin, string ConsumerNum)
        {
            CmdResultError error = CmdResultError.EXCEPTION;
            string msg = "支付失败！";
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Material_OnlineOrder order = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == orderNum);
                    if (order != null)
                    {
                        #region 添加交易记录
                        if (payType == (int)Common.EnumFile.PayStatus.Paid)
                        {
                            Order_RecordGet orderGet = new Order_RecordGet();
                            orderGet = dataContext.Order_RecordGet.FirstOrDefault(m => m.PayOrderNum == TradeNo);
                            //异步通知第一次新增一条数据
                            if (orderGet == null)
                            {
                                orderGet = new Order_RecordGet();
                                orderGet.ConsumersAccount = ConsumerLogin;
                                orderGet.Money = order.TotalMoney;
                                orderGet.Order_Consumers_ID = order.Order_Consumers_ID;
                                orderGet.OrderNum = order.OrderNum;
                                orderGet.PayTime = DateTime.Now;
                                orderGet.PayType = order.PayType;
                                orderGet.PayOrderNum = TradeNo;
                                orderGet.ConsumersNum = ConsumerNum;
                                dataContext.Order_RecordGet.InsertOnSubmit(orderGet);
                                order.PayOrderNum = TradeNo;
                                order.PayTime = DateTime.Now;
                                order.Activity = "";
                                #region 支付宝发送短信通知
                                try
                                {
                                    MsgInfo msgInfo = new MsgInfo();
                                    msgInfo.secretKey = ConfigurationManager.AppSettings["secretKey"];
                                    string now = DateTime.Now.ToString("yyyy-MM-dd");
                                    msgInfo.sms.sendDate = now;
                                    msgInfo.sms.content = "医疗器械追溯公共服务云平台商城有新的订单，请及时处理。【广联信息】";
                                    msgInfo.sms.msType = 1;
                                    msgInfo.sms.sendTime = now;
                                    msgInfo.phoneNumbers[0] =
                                        dataContext.Order_EnterpriseAccount.FirstOrDefault(m => m.Enterprise_ID == order.Enterprise_ID).LinkPhone;
                                    //实例对象
                                    SPortService client = new SPortService();
                                    client.Url = ConfigurationManager.AppSettings["WebServiceUrl"];
                                    string jsonStr = JsonHelper.DataContractJsonSerialize(msgInfo);
                                    //发送短信
                                    jsonStr = jsonStr.Replace(now, "");
                                    client.sendSMS(jsonStr);
                                }
                                catch { }
                                #endregion
                            }
                            if (order.OrderType < payType)
                            {
                                order.OrderType = payType;
                            }
                        }
                        #endregion
                        else
                        {
                            order.OrderType = payType;
                            if (payType == (int)Common.EnumFile.PayStatus.Confirm)
                            {
                                order.DeliveredTime = DateTime.Now;
                                if (Convert.ToInt64((order.TotalMoney.ToString()).Substring(0, order.TotalMoney.ToString().IndexOf('.'))) != 0)
                                {
                                    Order_Integral integralModel = new Order_Integral();
                                    integralModel.AddDate = DateTime.Now;
                                    integralModel.IntegralType = 2;
                                    integralModel.IntergralChange = Convert.ToInt64((order.TotalMoney.ToString()).Substring(0, order.TotalMoney.ToString().IndexOf('.')));
                                    integralModel.Order_Consumers_ID = order.Order_Consumers_ID.Value;
                                    dataContext.Order_Integral.InsertOnSubmit(integralModel);
                                }
                                dataContext.SubmitChanges();
                                Order_Consumers consumer = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == order.Order_Consumers_ID);
                                if (consumer != null)
                                {
                                    long consumerIn = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == order.Order_Consumers_ID && m.AddDate >= DateTime.Now.AddYears(-1) && m.AddDate <= DateTime.Now).ToList().Sum(m => m.IntergralChange).Value;
                                    //List<Order_Integral> conIntegral = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == order.Order_Consumers_ID && m.AddDate >= DateTime.Now.AddYears(-1) && m.AddDate <= DateTime.Now).ToList();
                                    //long consumerIn = 0;
                                    //if (conIntegral.Count > 0)
                                    //{
                                    //    foreach (var item in conIntegral)
                                    //    {
                                    //        consumerIn += item.IntergralChange.Value;
                                    //    }
                                    //}
                                    consumer.IntegralCount = consumerIn;
                                }
                            }
                        }
                        dataContext.SubmitChanges();
                        error = CmdResultError.NONE;
                        msg = "支付成功！";
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "Material_OnlineOrderDAL.PaySuccess";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        public RetResult ChangeOrder(Material_OnlineOrder model)
        {
            CmdResultError error = CmdResultError.EXCEPTION;
            string msg = "暂未开放该功能！";
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Material_OnlineOrder order = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == model.OrderNum);

                    Order_Consumers_Address address = dataContext.Order_Consumers_Address.FirstOrDefault(m => m.Order_Consumers_Address_ID == Convert.ToInt64(model.Consumers_Address));
                    order.Consumers_Address = Common.Argument.Public.GetAllArea(decimal.Parse(address.AreaID)) + address.Address;
                    order.Consumers_Name = address.LinkMan;
                    order.Consumers_Phone = address.LinkPhone;
                    order.Consumers_Postcode = address.Postcode;
                    order.MaterialCount = model.MaterialCount;
                    order.MaterialPrice = model.MaterialPrice;

                    Material_Spec spec = dataContext.Material_Spec.FirstOrDefault(m => m.ID == model.SpecID);
                    model.MaterialSpec = spec.MaterialSpecification;

                    order.OrderType = model.OrderType;
                    order.PayType = model.PayType;
                    order.SpecID = model.SpecID;
                    order.TotalMoney = model.TotalMoney;

                    dataContext.SubmitChanges();
                    error = CmdResultError.NONE;
                    msg = "支付成功！";
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        #region 结算
        public List<View_Order_Check> GetOrderCheck(long EnterpriseId, int pageIndex, out long totalCount)
        {
            List<View_Order_Check> Result = new List<View_Order_Check>();
            totalCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var liOrder = dataContext.View_Order_Check.Where(m => m.EnterpriseID == m.EnterpriseID);
                    if (EnterpriseId > 0)
                    {
                        liOrder = liOrder.Where(m => m.EnterpriseID.GetValueOrDefault(0) == EnterpriseId);
                    }
                    liOrder = liOrder.OrderByDescending(m => Convert.ToInt32(m.Month));
                    totalCount = liOrder.Count();
                    Result = liOrder.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
            }
            catch
            {
            }
            return Result;
        }

        /// <summary>
        /// 获取对账单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_Order_Check> GetList(long enterpriseId, int pageIndex, long agentCode, string name)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Order_Check
                               select m;
                    if (enterpriseId > 0)
                    {
                        data = data.Where(m => m.EnterpriseID == enterpriseId);
                    }
                    if (agentCode > 0)
                    {
                        //代理号
                        data = data.Where(m => m.PRRU_PlatForm_ID == agentCode);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.EnterpriseName.Contains(name));
                    }
                    data = data.OrderByDescending(m => Convert.ToInt32(m.Month));
                    return data.ToPagedList(pageIndex, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "Material_OnlineOrderDAL.GetList():View_ActivityManager";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_Material_OnlineOrder> GetCheckList(long checkId, int pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Order_CheckOrder orderModel = dataContext.Order_CheckOrder.FirstOrDefault(m => m.OrderCheckID == checkId);
                    if (orderModel != null)
                    {
                        string sql = "select top " + PageSize + " * from View_Material_OnlineOrder " +
                                "where id in (" + orderModel.OrderIds + ") " +
                                "and id not in (" +
                                    "select top " + PageSize * (pageIndex - 1) + " id from View_Material_OnlineOrder where id in (" + orderModel.OrderIds + ") order by id desc"
                                    + ")" +
                                "order by id desc";
                        var data = dataContext.View_Material_OnlineOrder.Where(a => orderModel.OrderIds.IndexOf(a.id.ToString()) > -1).OrderByDescending(a => a.id);
                        return data.ToPagedList(pageIndex, PageSize);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "Material_OnlineOrderDAL.GetList():View_ActivityManager";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        public List<View_Material_OnlineOrder> GetMaterialCheck(long checkId, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<View_Material_OnlineOrder> Result = new List<View_Material_OnlineOrder>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Order_CheckOrder orderModel = dataContext.Order_CheckOrder.FirstOrDefault(m => m.OrderCheckID == checkId);
                    if (orderModel != null)
                    {
                        string sql = "select top " + PageSize + " * from View_Material_OnlineOrder " +
                                "where id in (" + orderModel.OrderIds + ") " +
                                "and id not in (" +
                                    "select top " + PageSize * (pageIndex - 1) + " id from View_Material_OnlineOrder where id in (" + orderModel.OrderIds + ") order by id desc"
                                    + ")" +
                                "order by id desc";
                        Result = dataContext.ExecuteQuery<View_Material_OnlineOrder>(sql).ToList();
                        totalCount = orderModel.OrderIds.Split(',').Length;
                    }
                }
            }
            catch { }
            return Result;
        }
        public RetResult BalanceOrder(long checkId)
        {
            CmdResultError error = CmdResultError.EXCEPTION;
            string msg = "结算中出现错误！";
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Order_CheckOrder order = dataContext.Order_CheckOrder.FirstOrDefault(m => m.OrderCheckID == checkId);
                    dataContext.ExecuteCommand("update Material_OnlineOrder set IsAccount=1,OrderType=" + (int)Common.EnumFile.PayStatus.Finished + " where id in (" + order.OrderIds + ")");
                    order.IsPay = true;
                    try
                    {
                        Order_RecordPay pay = new Order_RecordPay();
                        pay.Enterprise_ID = order.EnterpriseID;
                        pay.Money = order.PayMoney;
                        pay.OrderNum = order.OrderCheckID.ToString();
                        pay.PayTime = DateTime.Now;
                        pay.PayType = (int)Common.EnumFile.PayType.Alipay;
                        dataContext.Order_RecordPay.InsertOnSubmit(pay);
                    }
                    catch { }
                    dataContext.SubmitChanges();
                    error = CmdResultError.NONE;
                    msg = "结算成功！";
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }
        #endregion
    }
}
