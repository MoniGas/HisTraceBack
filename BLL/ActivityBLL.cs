/********************************************************************************
** 作者：赵慧敏
** 开发时间：2017-6-7
** 联系方式:13313318725
** 代码功能：新建活动业务层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Dal;

namespace BLL
{
    /// <summary>
    /// 新建活动业务层
    /// </summary>
    public class ActivityBLL
    {
        /// <summary>
        /// 增加实体
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public RetResult AddModel(YX_Activity model, YX_ActivitySub modelSub, string setDetail,YX_RedRecharge recharge,long SetingID,out long activityID)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            activityID = 0;
            try
            {
                ActivityDAL dal = new ActivityDAL();
                if (string.IsNullOrEmpty(setDetail))
                {
                    result.Msg = "请进行红包设置";
                }
                else if (model == null)
                {
                    result.Msg = "数据错误";
                }
                else if (modelSub == null)
                {
                    result.Msg = "数据错误";
                }
                else
                {
                    List<YX_AcivityDetail> details = new List<YX_AcivityDetail>();
                    if (!string.IsNullOrEmpty(setDetail))
                    {
                        string[] detailArr = setDetail.Split(';');
                        if (detailArr.Count() == 2)
                        {
                            string[] sumArr = detailArr[0].Split(',');
                            string[] amountArr = detailArr[1].Split(',');
                            if (sumArr.Count() == amountArr.Count())
                            {
                                for (int i = 0; i < sumArr.Count(); i++)
                                {
                                    YX_AcivityDetail detail = new YX_AcivityDetail();
                                    detail.RedCount = Convert.ToInt32(amountArr[i]);
                                    detail.RedValue = Convert.ToDouble(sumArr[i]);
                                    detail.CompanyID = SessCokie.Get.EnterpriseID;
                                    detail.ActivityID = modelSub.ActivityID;
                                    detail.RedLastCount = Convert.ToInt32(amountArr[i]);
                                    details.Add(detail);
                                }
                            }
                        }
                    }
                    if (details.Count() > 0)
                    {
                        result = dal.AddModel(model, modelSub, details,recharge,SetingID,out activityID);
                    }
                    else
                    {
                        result.Msg = "请进行红包设置";
                    }
                }
            }
            catch
            {
                result.CmdError = CmdResultError.EXCEPTION;
                result.Msg = "异常错误，请重新操作！";
            }
            return result;
        }

        public RetResult FindTitle(string ActivityTitle, long CompanyID)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            try
            {
                ActivityDAL dal = new ActivityDAL();
                if (string.IsNullOrEmpty(ActivityTitle))
                {
                    result.Msg = "活动名称不能为空";
                }
                else
                {
                    result = dal.FindTitle(ActivityTitle, CompanyID);
                }
            }
            catch
            {
                result.CmdError = CmdResultError.EXCEPTION;
                result.Msg = "异常错误，请重新操作！";
            }
            return result;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public YX_Activity GetActivity(long activityID)
        {
            YX_Activity model = new YX_Activity();
            ActivityDAL dal = new ActivityDAL();
            model = dal.GetActivity(activityID);
            return model;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public YX_ActivitySub GetActivitySub(long activityID)
        {
            YX_ActivitySub model = new YX_ActivitySub();
            ActivityDAL dal = new ActivityDAL();
            model = dal.GetActivitySub(activityID);
            return model;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public List<YX_AcivityDetail> GetDetail(long activityID)
        {
            List<YX_AcivityDetail> detail = new List<YX_AcivityDetail>();
            ActivityDAL dal = new ActivityDAL();
            detail = dal.GetDetail(activityID);
            return detail;
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public RetResult EditModelSub(YX_ActivitySub modelSub)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            try
            {
                ActivityDAL dal = new ActivityDAL();
                if (modelSub == null)
                {
                    result.Msg = "数据错误";
                }
                else
                {
                    result = dal.EditActivitySub(modelSub);
                }
            }
            catch
            {
                result.CmdError = CmdResultError.EXCEPTION;
                result.Msg = "异常错误，请重新操作！";
            }
            return result;
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public RetResult EditModel(YX_Activity model, string setDetail,YX_RedRecharge change)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            try
            {
                ActivityDAL dal = new ActivityDAL();
                if (model == null)
                {
                    result.Msg = "数据错误";
                }
                else
                {
                    long redID=0;
                    result = dal.EditActivity(model,out redID);
                    if (result.CmdError != CmdResultError.NONE)
                    {
                        result.Msg = "修改失败";
                        return result;
                    }
                    List<YX_AcivityDetail> details = new List<YX_AcivityDetail>();
                    if (!string.IsNullOrEmpty(setDetail))
                    {
                        string[] detailArr = setDetail.Split(';');
                        if (detailArr.Count() == 2)
                        {
                            string[] sumArr = detailArr[0].Split(',');
                            string[] amountArr = detailArr[1].Split(',');
                            if (sumArr.Count() == amountArr.Count() && sumArr.Count()>0)
                            {
                                for (int i = 0; i < sumArr.Count(); i++)
                                {
                                    YX_AcivityDetail detail = new YX_AcivityDetail();
                                    detail.RedCount = Convert.ToInt32(amountArr[i]);
                                    detail.RedValue = Convert.ToDouble(sumArr[i]);
                                    detail.CompanyID = SessCokie.Get.EnterpriseID;
                                    detail.ActivityID = model.ActivityID;
                                    detail.RedLastCount = Convert.ToInt32(amountArr[i]);
                                    detail.AcivityRedID = redID;
                                    details.Add(detail);
                                }
                            }
                        }
                    }
                    if (details.Count() > 0)
                    {
                        result = dal.EditDetail(details, (long)model.ActivityID,change);
                    }
                    else
                    {
                        result.Msg = "请进行红包设置";
                    }
                }
            }
            catch
            {
                result.CmdError = CmdResultError.EXCEPTION;
                result.Msg = "异常错误，请重新操作！";
            }
            return result;
        }
    }
}
