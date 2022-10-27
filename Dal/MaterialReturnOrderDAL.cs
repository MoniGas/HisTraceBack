using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using Common.Log;

namespace Dal
{
    public class MaterialReturnOrderDAL : DALBase
    {
        public RetResult AddMaterialReturnOrder(string OrderNum, string Content)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Material_OnlineOrder OnlineOrder = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == OrderNum);
                    if (OnlineOrder != null)
                    {
                        bool isAdd = false;
                        Material_ReturnOrder Order = dataContext.Material_ReturnOrder.FirstOrDefault(m => m.OrderNum == OrderNum);
                        if (Order == null)
                        {
                            isAdd = true;
                            Order = new Material_ReturnOrder();
                        }
                        Order.Addtime = DateTime.Now;
                        Order.Enterprise_ID = OnlineOrder.Enterprise_ID;
                        Order.Material_OnlineOrder_ID = OnlineOrder.id;
                        Order.OrderNum = OrderNum;
                        Order.PayOrderNum = OnlineOrder.PayOrderNum;
                        Order.Status = (int)Common.EnumFile.Status.used;
                        Order.OrderType = (int)Common.EnumFile.PayStatus.ReturnMaterial;
                        Order.Content = Content;
                        OnlineOrder.OrderType = (int)Common.EnumFile.PayStatus.ReturnMaterial;
                        if (isAdd)
                        {
                            dataContext.Material_ReturnOrder.InsertOnSubmit(Order);
                        }
                        dataContext.SubmitChanges();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, "申请退货成功！", "申请退货成功！");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, "没有找到要退货的订单！", "没有找到要退货的订单！");
                    }
                }
                catch (Exception ex)
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, ex.ToString(), "暂未开放该功能！");
                }
            }
            return Ret;
        }

        public List<View_Material_ReturnOrder> GetReturnOrderList(string Name, long EnterpriseId, string BeginDate, string EndDate, int Status, int PageIndex, out long TotalCount)
        {
            List<View_Material_ReturnOrder> result = new List<View_Material_ReturnOrder>();
            TotalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var Data = from m in dataContext.View_Material_ReturnOrder
                               where m.Status == (int)Common.EnumFile.Status.used
                               select m;
                    if (EnterpriseId > 0)
                    {
                        Data = Data.Where(m => m.Enterprise_ID == EnterpriseId);
                    }
                    if (!string.IsNullOrEmpty(Name))
                    {
                        Data = Data.Where(m => m.Consumers_Name.Contains(Name)
                            || m.Consumers_Phone.Contains(Name)
                            || m.Consumers_Address.Contains(Name)
                            || m.MaterialSpec.Contains(Name)
                            || m.MaterialName.Contains(Name));
                    }
                    if (!string.IsNullOrEmpty(BeginDate))
                    {
                        Data = Data.Where(m => Convert.ToDateTime(m.Addtime) > Convert.ToDateTime(BeginDate));
                    }
                    if (!string.IsNullOrEmpty(EndDate))
                    {
                        Data = Data.Where(m => Convert.ToDateTime(m.Addtime) < Convert.ToDateTime(EndDate).AddDays(1));
                    }
                    if (Status > -1)
                    {
                        Data = Data.Where(m => m.OrderType == Status);
                    }
                    Data = Data.OrderByDescending(m => m.Addtime);
                    TotalCount = Data.Count();
                    result = Data.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                    for (int i = 0; i < result.Count; i++)
                    {
                        result[i].textStatus = Common.EnumText.EnumToText(typeof(Common.EnumFile.PayStatus), result[i].OrderType);
                    }
                }
                catch { }
            }
            return result;
        }
        public View_Material_ReturnOrder GetReturnOrder(string OrderNum)
        {
            View_Material_ReturnOrder Result = new View_Material_ReturnOrder();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Result = dataContext.View_Material_ReturnOrder.FirstOrDefault(m => m.OrderNum == OrderNum);
                }
            }
            catch { }
            return Result;
        }

        /// <summary>
        /// 获取退货
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_Material_ReturnOrder> GetList(string name, long enterpriseId, string beginDate, string endDate, int status, int pageIndex, long agentCode)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Material_ReturnOrder
                               where m.Status == (int)Common.EnumFile.Status.used
                               select m;
                    if (enterpriseId > 0)
                    {
                        data = data.Where(m => m.Enterprise_ID == enterpriseId);
                    }
                    if (agentCode > 0)
                    {
                        //代理号
                        data = data.Where(m => m.PRRU_PlatForm_ID == agentCode);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.Consumers_Name.Contains(name)
                            || m.Consumers_Phone.Contains(name)
                            || m.Consumers_Address.Contains(name)
                            || m.MaterialSpec.Contains(name)
                            || m.MaterialName.Contains(name));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.Addtime) > Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.Addtime) < Convert.ToDateTime(endDate).AddDays(1));
                    }
                    if (status > -1)
                    {
                        data = data.Where(m => m.OrderType == status);
                    }
                    data = data.OrderByDescending(m => m.Addtime);
                    return data.ToPagedList(pageIndex, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "ActivityManagerDAL.GetList():View_ActivityManager";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        public RetResult EditStatus(long MaterialReturnOrderID, long UserId, int OrderType)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Material_ReturnOrder newModel = dataContext.Material_ReturnOrder.FirstOrDefault(m => m.Material_ReturnOrder_ID == MaterialReturnOrderID);
                    if (newModel != null)
                    {
                        Material_OnlineOrder OnlineModel = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == newModel.OrderNum);
                        OnlineModel.OrderType = OrderType;
                        newModel.OrderType = OrderType;
                        if (OrderType == (int)Common.EnumFile.PayStatus.ReturnAgree
                            || OrderType == (int)Common.EnumFile.PayStatus.PayDelivery)
                        {
                            newModel.HanderUser = UserId;
                            newModel.HanderTime = DateTime.Now;
                        }
                        else if (OrderType == (int)Common.EnumFile.PayStatus.ReturnFinsh)
                        {
                            newModel.ConfirmUser = UserId;
                            newModel.ConfirmTime = DateTime.Now;
                            newModel.DeliveredTime = DateTime.Now;
                            if (OnlineModel.PayType == 1)
                            {
                                newModel.OrderType = (int)Common.EnumFile.PayStatus.ReturnMoney;
                            }
                        }
                        else if (OrderType == (int)Common.EnumFile.PayStatus.Returned)
                        {
                            newModel.DeliveryTime = DateTime.Now;
                        }
                        dataContext.SubmitChanges();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
                    }
                }
            }
            catch
            {
                Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
            }
            return Ret;
        }
        public RetResult EditStatus(List<string> PayOrderNum, int status)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    foreach (var item in PayOrderNum)
                    {
                        Material_ReturnOrder newModel = dataContext.Material_ReturnOrder.FirstOrDefault(m => m.PayOrderNum == item);
                        if (newModel != null)
                        {
                            Material_OnlineOrder OnlineModel = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.PayOrderNum == item);
                            if (OnlineModel != null)
                            {
                                OnlineModel.OrderType = status;
                            }
                            newModel.OrderType = status;
                            newModel.RePayTime = DateTime.Now;
                            if (status == (int)Common.EnumFile.PayStatus.ReturnMoney)
                            {
                                try
                                {
                                    Order_RecordPay pay = new Order_RecordPay();
                                    pay.Enterprise_ID = newModel.Enterprise_ID;
                                    pay.OrderNum = newModel.OrderNum + "-" + item;
                                    pay.PayTime = DateTime.Now;
                                    pay.Money = OnlineModel.TotalMoney;
                                    pay.Order_EnterpriseAccount_ID = 0;
                                    pay.PayType = 1;
                                    pay.SystemAccount = "";
                                    dataContext.Order_RecordPay.InsertOnSubmit(pay);
                                    dataContext.SubmitChanges();
                                }
                                catch { }
                            }
                            dataContext.SubmitChanges();
                            Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "退款成功！");
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            catch { Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "更改订单状态时出现异常！"); }
            return Ret;
        }
        public RetResult EditStatus(string OrderNum)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                Material_ReturnOrder newModel = dataContext.Material_ReturnOrder.FirstOrDefault(m => m.OrderNum == OrderNum);
                if (newModel != null)
                {
                    Material_OnlineOrder OnlineModel = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == OrderNum);
                    if (OnlineModel != null)
                    {
                        OnlineModel.OrderType = (int)Common.EnumFile.PayStatus.Returned;
                    }
                    newModel.OrderType = (int)Common.EnumFile.PayStatus.Returned;
                    newModel.ExpressComp = OnlineModel.ExpressComp;
                    newModel.ExpressNum = OnlineModel.ExpressNum;
                    newModel.DeliveryTime = DateTime.Now;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                }
                else
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
                }
            }
            return Ret;
        }

        public RetResult EditMaterialReturnOrder(string OrderNum, string ExpressComp, string ExpressNum)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                Material_ReturnOrder newModel = dataContext.Material_ReturnOrder.FirstOrDefault(m => m.OrderNum == OrderNum);
                if (newModel != null)
                {

                    Material_OnlineOrder OnlineOrder = dataContext.Material_OnlineOrder.FirstOrDefault(m => m.OrderNum == OrderNum);
                    if (OnlineOrder != null)
                    {
                        OnlineOrder.OrderType = (int)Common.EnumFile.PayStatus.Returned;
                    }
                    newModel.OrderType = (int)Common.EnumFile.PayStatus.Returned;
                    newModel.ExpressNum = ExpressNum;
                    newModel.ExpressComp = ExpressComp;
                    newModel.DeliveryTime = DateTime.Now;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                }
                else
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
                }
            }
            return Ret;
        }

        public List<View_Material_ReturnOrder> ExportExcel(string name, long eID, string sDate, string eDate, int status, int delStatus)
        {
            List<View_Material_ReturnOrder> result = new List<View_Material_ReturnOrder>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Material_ReturnOrder where m.Enterprise_ID == eID select m;
                    if (!string.IsNullOrEmpty(sDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.Addtime) > Convert.ToDateTime(sDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.Addtime) < Convert.ToDateTime(eDate).AddDays(1));
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.Consumers_Name.Contains(name) || m.Consumers_Phone.Contains(name) || m.Consumers_Address.Contains(name) || m.MaterialSpec.Contains(name) || m.MaterialName.Contains(name));
                    }
                    if (status > -1)
                    {
                        data = data.Where(m => m.Status == status);
                    }
                    data = data.OrderByDescending(m => m.Addtime);
                    result = data.ToList();
                }
                catch { }
            }
            return result;
        }
    }
}
