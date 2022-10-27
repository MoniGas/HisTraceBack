/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-9-8
** 联系方式:13313318725
** 代码功能：新建活动数据层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Argument;
using System.Data.Common;
using System.Configuration;
using Common.Tools;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 新建活动数据层
    /// </summary>
    public class CouponDAL : DALBase
    {
        /// <summary>
        /// 添加活动
        /// </summary>
        /// <param name="model">活动模型</param>
        /// <param name="modelSub">活动模型</param>
        /// <param name="details">红包配置</param>
        /// <returns></returns>
        public RetResult AddModel(YX_ActivitySub modelSub, YX_ActivityCoupon coupon, long SetingID)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "新建活动失败";

            int? codeNum = coupon.CouponCount;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    modelSub.ActivityMethod = (int)Common.EnumText.ActivityMethod.Coupon;
                    modelSub.OpenMode = (int)Common.EnumText.OpenMode.NoOpen;
                    modelSub.ActiveStatus = (int)Common.EnumText.ActivityState.NoStart;
                    dataContext.YX_ActivitySub.InsertOnSubmit(modelSub);
                    dataContext.SubmitChanges();
                    coupon.ActivityID = modelSub.ActivityID;
                    dataContext.YX_ActivityCoupon.InsertOnSubmit(coupon);
                    dataContext.SubmitChanges();
                    //if (CurrentUser.User.SetingID > 0)
                    if (SetingID > 0)
                    {
                        RequestCodeSetting seting = dataContext.RequestCodeSetting.Where(p => p.EnterpriseId == CurrentUser.User.EnterpriseID
                                && p.ID == CurrentUser.User.SetingID).FirstOrDefault();
                        if (seting != null)
                        {
                            YX_ActvitiyRelationCode relation = new YX_ActvitiyRelationCode();
                            relation.ActivityID = modelSub.ActivityID;
                            relation.CodeCount = seting.Count;
                            relation.CompanyIDcode = SessCokie.Get.MainCode;
                            relation.CompanyID = SessCokie.Get.EnterpriseID;
                            relation.EndCode = seting.endCode;
                            relation.StartCode = seting.beginCode;
                            relation.RelationDate = DateTime.Now;
                            relation.RequestSettingID = CurrentUser.User.SetingID;
                            relation.Flag = (int)Common.EnumText.CodeType.TraceCode;
                            dataContext.YX_ActvitiyRelationCode.InsertOnSubmit(relation);
                            var setting = dataContext.RequestCodeSetting.FirstOrDefault(t => t.ID == CurrentUser.User.SetingID);
                            if (seting != null)
                            {
                                setting.PacketState = (int)Common.EnumFile.PacketState.Success;
                            }
                            dataContext.SubmitChanges();
                            tran.Commit();
                            result.CmdError = CmdResultError.NONE;
                            result.Msg = "创建成功！";
                        }
                        else
                        {
                            tran.Rollback();
                            result.Msg = "创建失败,没有找到追溯码相关数据";
                            return result;
                        }
                    }
                    else
                    {
                        tran.Rollback();
                        result.Msg = "创建失败";
                        return result;
                    }
                }
                catch
                {
                    tran.Rollback();
                    result.Msg = "创建失败";
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public View_ActivityCoupon GetActivitySub(long activityID)
        {
            View_ActivityCoupon model = new View_ActivityCoupon();
            using (DataClassesDataContext dct = GetDataContext())
            {
                try
                {
                    model = dct.View_ActivityCoupon.Where(p => p.ActivityID == activityID).FirstOrDefault();
                }
                catch
                { }
            }
            return model;
        }

        /// <summary>
        /// 编辑活动
        /// </summary>
        /// <param name="modelSub">活动模型</param>
        /// <returns></returns>
        public RetResult EditActivitySub(YX_ActivitySub modelSub, YX_ActivityCoupon coupon)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "修改活动失败";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var activity = dataContext.YX_ActivitySub.Where(p => p.ActivityTitle == modelSub.ActivityTitle
                        && p.ActiveStatus < 2 && p.ActivityID != modelSub.ActivityID).FirstOrDefault();
                    var data = dataContext.YX_ActivitySub.FirstOrDefault(p => p.ActivityID == modelSub.ActivityID);
                    if (data == null)
                    {
                        result.Msg = "没有找到要修改的数据！";
                    }
                    else
                    {
                        data.ActivityTitle = modelSub.ActivityTitle;
                        data.ActivityType = modelSub.ActivityType;
                        data.AtivityImageURL = modelSub.AtivityImageURL;
                        data.Content = modelSub.Content;
                        data.ActivityType = modelSub.ActivityType;
                        data.StartDate = modelSub.StartDate;
                        data.EndDate = modelSub.EndDate;
                        var companyewm = dataContext.YX_CompanyIDcode.FirstOrDefault(a => a.ActivityID == modelSub.ActivityID);
                        if (companyewm != null)
                        {
                            if (modelSub.ActivityType == (int)Common.EnumText.ActiveType.Multi)
                            {

                                companyewm.FromCode = 1;
                                companyewm.EndCode = coupon.CouponCount;
                            }
                            else
                            {
                                companyewm.FromCode = coupon.CouponCount;
                                companyewm.EndCode = coupon.CouponCount;
                            }
                        }
                        var oldCoupon = dataContext.YX_ActivityCoupon.FirstOrDefault(a => a.ActivityID == modelSub.ActivityID);
                        if (oldCoupon != null)
                        {
                            oldCoupon.CouponAddress = coupon.CouponAddress;
                            oldCoupon.CouponContent = coupon.CouponContent;
                            oldCoupon.CouponType = coupon.CouponType;
                            oldCoupon.CouponCount = coupon.CouponCount;
                        }
                        dataContext.SubmitChanges();
                        result.Msg = "修改成功";
                        result.CmdError = CmdResultError.NONE;
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// 是否可以领取优惠券
        /// </summary>
        /// <param name="ewm"></param>
        /// <param name="settingId"></param>
        /// <param name="comIdCode"></param>
        /// <returns></returns>
        public RetResult CouponCanGet(string ewm, long settingId, long codeId, long activityId = 0)
        {
            RetResult result = new RetResult { };
            try
            {
                using (DataClassesDataContext dct = GetDataContext())
                {
                    var model = dct.YX_ActvitiyRelationCode.Join(dct.YX_ActivitySub, a => a.ActivityID, b => b.ActivityID,
                        (a, b) => new { RequestSettingID = a.RequestSettingID, CompanyIDcodeID = b.CompanyID, ActivityID = a.ActivityID,
                            ActivityMethod = b.ActivityMethod }).FirstOrDefault(a => 
                                (a.RequestSettingID == settingId || a.CompanyIDcodeID == codeId || a.ActivityID == activityId) 
                                && a.ActivityMethod == (int)Common.EnumText.ActivityMethod.Coupon);
                    if (model == null)
                    {
                        result.Code = 1;
                        result.Msg = "该活动未配置优惠券相关信息，谢谢参与！";
                    }
                    else
                    {
                        //活动是否在有效期
                        var activity = dct.YX_ActivitySub.FirstOrDefault(a => a.ActiveStatus == (int)Common.EnumText.ActivityState.Going
                            && a.StartDate <= DateTime.Now.Date && a.EndDate >= DateTime.Now.Date && a.ActivityID == model.ActivityID);
                        YX_ActivityCoupon ad = dct.YX_ActivityCoupon.FirstOrDefault(p => p.ActivityID == model.ActivityID);
                        if (activity == null)
                        {
                            result.Code = 2;
                            result.Msg = "该活动已经结束，谢谢参与";
                        }
                        else if (dct.YX_CouponGetRecord.FirstOrDefault(a => a.EwmCode == ewm) != null && activity.ActivityType == (int)Common.EnumText.ActiveType.Multi)
                        {
                            result.Code = 3;
                            result.Msg = "该优惠券已经被领取了，谢谢参与！";
                        }
                        else if (ad.CouponLastCount == 0)
                        {
                            result.Code = 2;
                            result.Msg = "该活动已经结束，谢谢参与";
                        }
                        else if (activity.ActivityType == (int)Common.EnumText.ActiveType.Multi)
                        {
                            result.Code = 5;
                            result.Msg = "可以领取优惠券！";
                            if (activity.SetStatus == (int)Common.EnumText.RedStyle.藏)
                            {
                                result.Code = 7;
                                result.Msg = "可以领取优惠券！";
                            }
                        }
                        else if (activity.ActivityType == (int)Common.EnumText.ActiveType.Single)
                        {
                            var coupon = dct.YX_ActivityCoupon.FirstOrDefault(a => a.ActivityID == activity.ActivityID);
                            if (coupon == null)
                            {
                                result.Code = 1;
                                result.Msg = "该活动未配置优惠券相关信息，谢谢参与！";
                            }
                            else
                            {
                                if (dct.YX_CouponGetRecord.Count(a => a.ActivityID == activity.ActivityID && a.EwmCode == ewm) < coupon.CouponCount)
                                {
                                    result.Code = 5;
                                    result.Msg = "可以领取优惠券！";
                                    if (activity.SetStatus == (int)Common.EnumText.RedStyle.藏)
                                    {
                                        result.Code = 7;
                                        result.Msg = "可以领取优惠券！";
                                    }
                                }
                                else
                                {
                                    result.Code = 6;
                                    result.Msg = "该优惠券已经被领完，谢谢参与！";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 5;
                result.Msg = "网络异常，请重新操作";
            }
            return result;
        }

        /// <summary>
        /// 记录优惠券
        /// </summary>
        /// <param name="tel"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public RetResult GetCoupon(string tel, YX_CouponGetRecord coupon)
        {
            RetResult result = new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "领取失败！" };
            try
            {
                using (var dataContext = GetDataContext())
                {
                    YX_ActivityCoupon ad = dataContext.YX_ActivityCoupon.FirstOrDefault(p => p.ActivityID == coupon.ActivityID);

                    ad.CouponLastCount--;
                    Order_Consumers consumer = dataContext.Order_Consumers.FirstOrDefault(a => a.LinkPhone == tel);
                    if (consumer == null)
                    {
                        consumer = new Order_Consumers { LinkPhone = tel, AddDate = DateTime.Now };
                        dataContext.Order_Consumers.InsertOnSubmit(consumer);
                    }
                    dataContext.SubmitChanges();
                    coupon.Order_Consumers_ID = consumer.Order_Consumers_ID;
                    dataContext.YX_CouponGetRecord.InsertOnSubmit(coupon);
                    dataContext.SubmitChanges();
                    result.CmdError = CmdResultError.NONE;
                    result.Msg = "领取成功！";
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 根据用户id获取优惠券列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<View_CouponGetRecord> GetList(long userId)
        {
            using (var dataContext = GetDataContext())
            {
                return dataContext.View_CouponGetRecord.Where(a => a.Order_Consumers_ID == userId).ToList();
            }
        }

        #region 备案 标志人事物
        public RetResult RecordCode(string mainCode, long enterpriseId, string codeUseID)
        {
            string msg = "备案品类码失败！";
            string errorMemo = "";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                //string mailcodeIdcode = mainCode.Substring(0, mainCode.LastIndexOf("."));
                string ms = DateTime.Now.ToString("fff");
                string action = ConfigurationManager.AppSettings["RegOtherIDcodeInfo"];
                string access_token = ConfigurationManager.AppSettings["access_token"];
                string parseUrl = ConfigurationManager.AppSettings["IpRedirect"];
                Dictionary<string, string> paras = new Dictionary<string, string>();
                paras.Add("access_token", access_token);
                paras.Add("companyIDcode", mainCode);
                paras.Add("codeUse_ID", "90");// codeUseID);
                paras.Add("industryCategory_ID", "10133");
                paras.Add("categoryCode", "12000000");
                paras.Add("modelNumber", "优惠券" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                paras.Add("modelNumberEn", "");
                paras.Add("introduction", "");
                paras.Add("codePayType", "5");
                paras.Add("goToUrl", parseUrl + "Market/Wap_Coupon/Index");
                string strBack = APIHelper.sendPost(action, paras, "get");
                JsonObject jsonObject = new JsonObject(strBack);
                string result = jsonObject["ResultCode"].Value;
                string resultMsg = jsonObject["ResultMsg"].Value;
                string organUnitIDcode = jsonObject["OrganUnitIDcode"].Value;
                if (result == "1")
                {
                    //msg = "备案品类码成功！";
                    error = CmdResultError.NONE;
                    msg = "1";
                    errorMemo = organUnitIDcode;

                }
                else if (result == "0")
                {
                    msg = "备案失败！";
                }
                Ret.SetArgument(error, errorMemo, msg);
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrdePayDAL.RecordCode()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            Ret.SetArgument(error, errorMemo, msg);
            return Ret;
        }
        #endregion

        #region 20171127优惠券领取成功
        /// <summary>
        /// 获取优惠券信息
        /// </summary>
        /// <param name="couponCode">优惠券码</param>
        /// <returns></returns>
        public View_ActivityCouponGetRecord GetCouponInfo(string couponCode)
        {
            View_ActivityCouponGetRecord result = new View_ActivityCouponGetRecord();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    result = dataContext.View_ActivityCouponGetRecord.FirstOrDefault(m => m.CouponCode == couponCode);
                }
            }
            catch (Exception ex)
            {
                string errData = "CouponDAL.GetCouponInfo()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        #endregion
    }
}
