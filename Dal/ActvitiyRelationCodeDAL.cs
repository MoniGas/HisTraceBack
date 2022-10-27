/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-17
** 联系方式:13313318725
** 代码功能：活动关联码操作
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Linq;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 活动关联码
    /// </summary>
    public class ActvitiyRelationCodeDAL : DALBase
    {
        /// <summary>
        /// 获取关联活动的二维码
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回列表</returns>
        public PagedList<YX_ActvitiyRelationCode> GetList(long enterpriseId, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.YX_ActvitiyRelationCode.Where(m => m.CompanyID == enterpriseId);
                    data = data.OrderByDescending(m => m.ActvitiyRelationCodeID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "ActvitiyRelationCodeDAL.GetList():YX_ActvitiyRelationCode";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 查看关联活动
        /// </summary>
        /// <param name="id">关联ID</param>
        /// <returns></returns>
        public View_ActvitiyRelationCode Info(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_ActvitiyRelationCode.FirstOrDefault(m => m.ActvitiyRelationCodeID == id);
            }
        }

        /// <summary>
        /// 获取model
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public YX_ActvitiyRelationCode GetModel(long activityId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.YX_ActvitiyRelationCode.FirstOrDefault(m => m.ActivityID == activityId);
            }
        }

        /// <summary>
        /// 获取model
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public YX_CompanyIDcode GetModelComanyId(long activityId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.YX_CompanyIDcode.FirstOrDefault(m => m.ActivityID == activityId);
            }
        }
        #region 管理员后台红包活动
        /// <summary>
        /// 获取关联活动的二维码
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回列表</returns>
        public PagedList<View_ActivityManager> AdminGetList(int? pageIndex, string comName, string startTime, string endTime, string title, long enterId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_ActivityManager
                               select m;
                    if (!string.IsNullOrEmpty(comName))
                    {
                        data = data.Where(m => m.EnterpriseName.Contains(comName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(startTime))
                    {
                        data = data.Where(m => m.EndDate.GetValueOrDefault() >= Convert.ToDateTime(startTime));
                    }
                    if (!string.IsNullOrEmpty(endTime))
                    {
                        data = data.Where(m => m.StartDate.GetValueOrDefault() <= Convert.ToDateTime(endTime));
                    }
                    if (!string.IsNullOrEmpty(title))
                    {
                        data = data.Where(m => m.ActivityTitle.Contains(title.Trim()));
                    }
                    if (enterId > 0 && enterId != 16)
                    {
                        data = data.Where(m => m.PRRU_PlatForm_ID == enterId);
                    }
                    return data.OrderByDescending(a => a.CreateDate).ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "ActvitiyRelationCodeDAL.AdminGetList():View_AcRelationCodeEnterprise";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 修改支付状态
        /// </summary>
        /// <param name="id">关联ID</param>
        /// <returns></returns>
        public RetResult EditStatus(long id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    YX_ActvitiyRelationCode model = dataContext.YX_ActvitiyRelationCode.FirstOrDefault(m => m.ActvitiyRelationCodeID == id);
                    if (model == null)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
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
        /// 获取信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public View_ActivityManager GetInfo(long activityId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_ActivityManager.FirstOrDefault(m => m.ActivityID == activityId);
            }
        }

        /// <summary>
        /// 设置活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="openMode"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public RetResult SetActivity(long activityId, int openMode, string url, int payState)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                {
                    dataContext.Connection.Open();
                }
                dataContext.Transaction = dataContext.Connection.BeginTransaction();
                try
                {
                    var model = dataContext.YX_RedRecharge.FirstOrDefault(a => a.ActivityID == activityId);
                    if (model!=null)
                    {
                        model.BillUrl = url;
                        model.PayState = payState;
                    }
                    var activity = dataContext.YX_ActivitySub.FirstOrDefault(a => a.ActivityID == activityId);
                    activity.OpenMode = openMode;
                    if (openMode == (int)Common.EnumText.OpenMode.Auto)
                    {
                        activity.ActiveStatus = (int)Common.EnumText.ActivityState.Going;
                    }
                    dataContext.SubmitChanges();
                    dataContext.Transaction.Commit();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                }
                catch (Exception ex)
                {
                    dataContext.Transaction.Rollback();
                    Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "操作失败");
                }
                return Ret;
            }
        }
        #endregion
    }
}
