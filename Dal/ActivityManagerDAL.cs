/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-9
** 联系方式:13313318725
** 代码功能：红包活动管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Argument;
using Webdiyer.WebControls.Mvc;
using Common.Log;

namespace Dal
{
    public class ActivityManagerDAL : DALBase
    {
        /// <summary>
        /// 获取红包活动列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="AcName">活动名称</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_ActivityManager> GetList(long enterpriseId, string AcName, string sDate, string eDate, int activityStatus, int hbType, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_ActivityManager.Where(m => m.CompanyID == enterpriseId);
                    if (!string.IsNullOrEmpty(sDate))
                    {
                        data = data.Where(m => m.EndDate.GetValueOrDefault() >= Convert.ToDateTime(sDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => m.StartDate.GetValueOrDefault() <= Convert.ToDateTime(eDate));
                    }
                    if (!string.IsNullOrEmpty(AcName))
                    {
                        data = data.Where(m => m.ActivityTitle.Contains(AcName));
                    }
                    if (activityStatus != 0)
                    {
                        //手动，自动时直接更改活动状态
                        if (activityStatus == (int)Common.EnumText.ActivityState.Going)
                        {
                            data = data.Where(m => m.ActiveStatus == activityStatus && m.StartDate <= DateTime.Now.Date && m.EndDate >= DateTime.Now.Date);
                        }
                        else if (activityStatus == (int)Common.EnumText.ActivityState.NoStart)
                        {
                            data = data.Where(m => m.ActiveStatus == activityStatus || (m.ActiveStatus == (int)Common.EnumText.ActivityState.Going && (m.StartDate > DateTime.Now.Date || m.EndDate < DateTime.Now.Date)));
                        }
                        else
                        {
                            data = data.Where(m => m.ActiveStatus == activityStatus);
                        }
                    }
                    if (hbType != -1)
                    {
                        data = data.Where(m => m.ActivityMethod == hbType);
                    }
                    data = data.OrderByDescending(m => m.ActivityID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "ActivityManagerDAL.GetList():View_ActivityManager";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 查看活动信息
        /// </summary>
        /// <param name="id">活动id</param>
        /// <returns></returns>
        public View_ActivityManager GetActivityInfo(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_ActivityManager.FirstOrDefault(m => m.ActivityID == id);
            }
        }

        /// <summary>
        /// 获取红包明细
        /// </summary>
        /// <param name="redId">活动红包ID</param>
        /// <returns></returns>
        public List<YX_AcivityDetail> HbDetail(long activityId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.YX_AcivityDetail.Where(m => m.ActivityID == activityId).ToList();
            }
        }

        /// <summary>  
        /// 获取用户领取记录
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <returns></returns>
        public List<View_RedGetRecord> HbGetRecord(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_RedGetRecord.Where(m => m.ActivityID == id).OrderByDescending(m => m.SendTime).ToList();
            }
        }
         
        /// <summary>
        /// 修改活动状态
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <param name="eID">企业ID</param>
        /// <returns></returns>
        public RetResult EditStatusEnd(long id, long eID)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    YX_ActivitySub model = dataContext.YX_ActivitySub.FirstOrDefault(m => m.ActivityID == id && m.CompanyID == eID);
                    if (model == null)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    if (model.CompanyID != eID)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "您无权对该条信息进行操作");
                        return Ret;
                    }
                    model.ActiveStatus = (int)Common.EnumText.ActivityState.Finish;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }

        /// <summary>
        /// 修改活动状态
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <param name="eID">企业ID</param>
        /// <returns></returns>
        public RetResult EditStatusStar(long id, long eID)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    YX_ActivitySub model = dataContext.YX_ActivitySub.FirstOrDefault(m => m.ActivityID == id && m.CompanyID == eID);
                    if (model == null)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    if (model.CompanyID != eID)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "您无权对该条信息进行操作");
                        return Ret;
                    }
                    model.ActiveStatus = (int)Common.EnumText.ActivityState.Going;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }

        /// <summary>
        /// 预览码
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <returns></returns>
        public YX_ActvitiyRelationCode GetActivityID(long setID)
        {
            YX_ActvitiyRelationCode code = new YX_ActvitiyRelationCode();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    if (setID > 0)
                    {
                        code = dataContext.YX_ActvitiyRelationCode.Where(p => p.RequestSettingID == setID).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return code;
        }

        /// <summary>
        /// 红包充值记录
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public RedPacketMoney GetRedMoney(long companyId, int pageIndex)
        {
            RedPacketMoney code = new RedPacketMoney();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var companyMoney = dataContext.View_CompanyMoney.FirstOrDefault(a => a.CompanyID == companyId);
                    code.AllMoney = companyMoney.AllMoney;
                    code.SendMoney = companyMoney.SendMoney;
                    code.LeftMoney = dataContext.YX_AcivityDetail.Join(dataContext.YX_RedRecharge.Where(a => a.PayState 
                        == (int)Common.EnumText.PayState.Payed),
                        a => a.ActivityID, b => b.ActivityID, (a, b) => new 
                        { a.CompanyID, a.RedLastCount, a.RedValue }).Where(a => a.CompanyID == companyId).Sum(a => a.RedLastCount * a.RedValue);
                    code.RechangeRecordLst = dataContext.View_RechangeRecord.Where(a => a.CompanyID == companyId && a.PayMode!=(int)Common.EnumText.PayType.IndependentPay
                        ).OrderByDescending(a => a.CreateDate).ToPagedList<View_RechangeRecord>(pageIndex, PageSize);
                }
            }
            catch (Exception)
            {
            }
            return code;
        }

        /// <summary>
        /// 红包发送记录
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public RedPacketMoney GetSendMoney(long companyId, int pageIndex)
        {
            RedPacketMoney code = new RedPacketMoney();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var companyMoney = dataContext.View_CompanyMoney.FirstOrDefault(a => a.CompanyID == companyId);
                    code.AllMoney = companyMoney.AllMoney;
                    code.SendMoney = companyMoney.SendMoney;
                    code.LeftMoney = dataContext.YX_AcivityDetail.Join(dataContext.YX_RedRecharge.Where(a => a.PayState == (int)Common.EnumText.PayState.Payed), a => a.ActivityID, b => b.ActivityID, (a, b) => new { a.CompanyID, a.RedLastCount, a.RedValue }).Where(a => a.CompanyID == companyId).Sum(a => a.RedLastCount * a.RedValue);
                    code.RedRecordLst = dataContext.View_RedSendRecord.Where(a => a.CompanyID == companyId).OrderByDescending(a => a.StartDate).ToPagedList<View_RedSendRecord>(pageIndex, PageSize);
                }
            }
            catch (Exception)
            {
            }
            return code;
        }

        /// <summary>
        /// 核销优惠券
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="yhqcode">优惠券码号</param>
        /// <returns></returns>
        public RetResult YhqCancelOut(long eid, string yhqcode)
        {
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    YX_CouponGetRecord couponGetRecord = dataContext.YX_CouponGetRecord.FirstOrDefault(m => m.CouponCode == yhqcode);
                    if (couponGetRecord != null)
                    {
                        YX_ActivitySub activity = dataContext.YX_ActivitySub.FirstOrDefault(m => m.CompanyID == eid && m.ActivityID == couponGetRecord.ActivityID);
                        if (activity != null)
                        {
                            couponGetRecord.CancelTime = DateTime.Now;
                            couponGetRecord.CancelState = (int)Common.EnumText.CancelOutStatus.YCancelOut;
                            dataContext.SubmitChanges();
                            Msg = "优惠券核销成功！";
                            CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Msg = "您无权核销该优惠券！";
                        }
                    }
                    else
                    {
                        Msg = "没有找到该优惠券！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 查看优惠券详情
        /// </summary>
        /// <param name="id">活动id</param>
        /// <returns></returns>
        public View_ActivityCoupon GetYhqInfo(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_ActivityCoupon.FirstOrDefault(m => m.ActivityID == id);
            }
        }

        /// <summary>
        /// 获取优惠券领取记录
        /// </summary>
        /// <param name="redId">活动红包ID</param>
        /// <returns></returns>
        public PagedList<View_CouponGetRecord> YhqGetDetail(long activityId, int yhqType, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_CouponGetRecord.Where(m => m.ActivityID == activityId);
                    if (yhqType != -1)
                    {
                        data = data.Where(m => m.CancelState == yhqType);
                    }
                    data = data.OrderByDescending(m => m.ActivityID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "ActivityManagerDAL.YhqGetDetail():View_CouponGetRecord";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }
    }
}
