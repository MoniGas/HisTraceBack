/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-7-10
** 联系方式:15533621896
** 代码功能：我的二维码
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;
using System.Data.Common;

namespace Dal
{
    /// <summary>
    /// 我的二维码
    /// </summary>
    public class CompanyIDcodeDAL : DALBase
    {
        /// <summary>
        /// 获取二维码订单列表
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<NewCompanyIDCode> GetListEwm(long eid, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    //var data = dataContext.YX_BuyCodeOrder.Join(dataContext.YX_CompanyIDcode, a => a.BuyCodeOrderID, b => b.BuyCodeOrderID, (a, b) => new NewCompanyIDCode { BuyCodeOrderID = a.BuyCodeOrderID, CompanyIDcodeID = b.CompanyIDcodeID, BuyDate = a.CreateDate, CodeCount = b.CodeCount, CodeFileURL = b.CodeFileURL, CodeMain = b.CodeMain, CompanyID = b.CompanyID, EndCode = b.EndCode, FromCode = b.FromCode, UseState = b.UseState, OrderStatus = a.OrderStatus, PackageName = a.PackageName }).Where(a => a.OrderStatus == (int)Common.EnumText.OrderState.Payed).OrderByDescending(m => m.BuyCodeOrderID);
                    //return data.ToPagedList(pageIndex ?? 1, PageSize);
                    return null;
                }
                catch (Exception ex)
                {
                    string errData = "BuyCodeOrderPayDAL.GetListEwm()";
                    Common.Log.WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取企业二维码信息
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public YX_CompanyIDcode GetModel(long companyIDcodeID,long activityId=0)
        {
            YX_CompanyIDcode model = new YX_CompanyIDcode();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.YX_CompanyIDcode.FirstOrDefault(a => a.CompanyIDcodeID == companyIDcodeID||a.ActivityID==activityId);
                }
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrderPayDAL.GetModel()";
                Common.Log.WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return model;
        }

        /// <summary>
        /// 二维码关联活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult AddRelationActivity(YX_ActvitiyRelationCode model)
        {
            RetResult result = new RetResult() { Msg = "操作失败！", CmdError = CmdResultError.EXCEPTION };
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null) dataContext.Connection.Open();
                DbTransaction dr = dataContext.Connection.BeginTransaction();
                try
                {
                    dataContext.Transaction = dr;
                    model.Flag = (int)Common.EnumText.CodeType.MarketCode;
                    //有问题
                    //if (dataContext.YX_BuyCodeOrder.Join(dataContext.YX_CompanyIDcode, a => a.BuyCodeOrderID, b => b.BuyCodeOrderID, (a, b) => new { BuyCodeOrderID = a.BuyCodeOrderID, CompanyIDcodeID = b.CompanyIDcodeID, PayMode = a.PayMode }).FirstOrDefault(a => a.CompanyIDcodeID == model.CompanyIDcodeID).PayMode == (int)Common.EnumText.PayType.OffLinePay)
                    //{
                    //    model.PayState = (int)Common.EnumText.PayState.NoPay;
                    //}
                    //else
                    //{
                    //    model.PayState = (int)Common.EnumText.PayState.Payed;
                    //}
                    dataContext.YX_ActvitiyRelationCode.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    long? count = dataContext.YX_ActvitiyRelationCode.Where(a => a.CompanyIDcodeID == model.CompanyIDcodeID).Sum(a => a.CodeCount);
                    var idCode = dataContext.YX_CompanyIDcode.FirstOrDefault(a => a.CompanyIDcodeID == model.CompanyIDcodeID);
                    if (idCode != null && idCode.CodeCount == count)
                    {
                        idCode.UseState = (int)Common.EnumText.UserState.Used;
                    }
                    else
                    {
                        idCode.UseState = (int)Common.EnumText.UserState.UserPart;
                    }
                    dataContext.SubmitChanges();
                    dr.Commit();
                    result.Msg = "操作成功！";
                    result.CmdError = CmdResultError.NONE;
                }
                catch (Exception)
                {
                    dr.Rollback();
                    result.Msg = "网络出现异常，请重新操作！";
                }
            }
            return result;
        }

        /// <summary>
        /// 获取可以关联二维码的活动
        /// </summary>
        /// <param name="eid">企业id</param>
        /// <returns></returns>
        public List<View_RelationActivityEwm> GetEwmRelationActivity(long eid)
        {
            List<View_RelationActivityEwm> modelLst = new List<View_RelationActivityEwm>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    modelLst = dataContext.View_RelationActivityEwm.Where(a => a.CompanyID == eid).ToList();
                }
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrderPayDAL.GetEwmRelationActivity()";
                Common.Log.WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return modelLst;
        }

        /// <summary>
        /// 获取关联活动详情
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public View_RelationActivityEwm GetRelationActivityEwmModel(long activityId)
        {
            View_RelationActivityEwm model = new View_RelationActivityEwm();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.View_RelationActivityEwm.FirstOrDefault(a => a.ActivityID == activityId);
                }
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrderPayDAL.GetRelationActivityEwmModel()";
                Common.Log.WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return model;
        }

        /// <summary>
        /// 获取开始码
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public long? GetStartCode(long companyIDcodeID)
        {
            long? code = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    code = dataContext.YX_ActvitiyRelationCode.Where(a => a.CompanyIDcodeID == companyIDcodeID).Sum(a => a.CodeCount);
                }
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrderPayDAL.GetStartCode()";
                Common.Log.WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return code;
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public View_BuyCodeOrder GetOrderModel(long companyIDcodeID)
        {
            View_BuyCodeOrder model = new View_BuyCodeOrder();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.View_BuyCodeOrder.FirstOrDefault(a => a.CompanyIDcodeID == companyIDcodeID);
                }
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrderPayDAL.GetOrderModel()";
                Common.Log.WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return model;
        }

        /// <summary>
        /// 我的二维码-详情
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public DetailCompanyIDCode GetOrderDetail(long companyIDcodeID)
        {
            DetailCompanyIDCode model = new DetailCompanyIDCode();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var companyCode = dataContext.YX_CompanyIDcode.FirstOrDefault(a => a.CompanyIDcodeID == companyIDcodeID);
                    model.CodeContent = string.Format("{0}~{1}", companyCode.FromCode, companyCode.EndCode);
                    model.CodeCount = companyCode.CodeCount ?? 0;
                    model.UsedCount = dataContext.YX_ActvitiyRelationCode.Where(a => a.CompanyIDcodeID == companyIDcodeID).Sum(a => a.CodeCount) ?? 0;
                    model.RestCount = model.CodeCount.Value - model.UsedCount.Value;
                    model.List = dataContext.View_ActvitiyRelationCode.Where(a => a.CompanyIDcodeID == companyIDcodeID).ToList();
                }
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrderPayDAL.GetOrderModel()";
                Common.Log.WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return model;
        }
    }
}
