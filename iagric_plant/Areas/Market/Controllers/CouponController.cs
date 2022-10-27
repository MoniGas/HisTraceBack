/********************************************************************************
** 作者：优惠券
** 开发时间：2017-9-8
** 联系方式:13313318725
** 代码功能：新建活动数据层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using BLL;
using Common.Argument;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 优惠券
    /// </summary>
    public class CouponController : Controller
    {
        /// <summary>
        /// 添加或修改试图
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult AddOrEdit(long activityId = 0)
        {
            //获取配置id
            if (!string.IsNullOrEmpty(Request["mainCode"]) && !string.IsNullOrEmpty(Request["settingId"])
                && !string.IsNullOrEmpty(Request["trace"]) && !string.IsNullOrEmpty(Request["enterpriseID"]))
            {
                string mainCode = Request["mainCode"];
                string settingId = Request["settingId"];
                string trace = Request["trace"];
                string enterpriseID = Request["enterpriseID"];
                UserInfo userInfo = new UserInfo();
                userInfo.MainCode = mainCode;
                userInfo.EnterpriseID = Convert.ToInt64(enterpriseID);
                userInfo.SetingID = Convert.ToInt64(settingId);
                //追溯管理跳转
                userInfo.LoginType = (int)Common.EnumText.LoginType.parameter;
                CurrentUser.User = userInfo;
                ViewBag.LoginType = (int)Common.EnumText.LoginType.parameter;
                ViewBag.SetingID = settingId;
            }
            else
            {
                CurrentUser.User = new UserInfo();
                CurrentUser.User.LoginType = (int)Common.EnumText.LoginType.common;
                ViewBag.LoginType = (int)Common.EnumText.LoginType.common;
            }
            var model = new CouponBLL().GetActivitySub(activityId);
            if (model == null)
            {
                model = new View_ActivityCoupon() { ActivityID = activityId };
            }
            return View(model);
        }

        /// <summary>
        /// 添加或修改试图
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrEdit(YX_ActivitySub sub, YX_ActivityCoupon coupon, string SetingID)
        {
            coupon.CouponLastCount=coupon.CouponCount;
            sub.CreateDate = DateTime.Now;
            sub.ActiveStatus = (int)Common.EnumText.ActivityState.NoStart;
            sub.MainCode = SessCokie.Get.MainCode;
            sub.CompanyID = SessCokie.Get.EnterpriseID;
            sub.SetStatus = (int)Common.EnumText.SetStatus.NoSet;
            RetResult result = sub.ActivityID == 0 ? new CouponBLL().AddModel(sub, coupon, Convert.ToInt64(SetingID)) : new CouponBLL().EditActivitySub(sub, coupon);
            return Json(new { flag = result.IsSuccess, msg = result.Msg, activityId = sub.ActivityID });
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetSrc(long activityId)
        {
            Image img = MvcWebCommon.Tools.CreateEwmImg.GetQRCodeImageEx(System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Market/Coupon/ViewCoupon?activityId=" + activityId.ToString(), 200, 200);
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                return File(ms.ToArray(), "image/Jpeg", DateTime.Now.Ticks.ToString() + ".jpg");
            }
        }

        /// <summary>
        /// 查看优惠券
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult ViewCoupon(long activityId)
        {
            var model = new CouponBLL().GetActivitySub(activityId);
            return View(model);
        }
    }
}
