/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-9-11
** 联系方式:15533621896
** 代码功能：拍码红包页
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using Common;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 优惠券拍码页
    /// </summary>
    public class Wap_CouponController : Controller
    {
        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string ewm, long settingId = 0)
        {
            View_ActivityCoupon couponModel = new View_ActivityCoupon();
            try
            {
                CodeInfo codeInfo = GetSession(null, 0);
                if (null != codeInfo && codeInfo.CodeType == (int)EnumFile.AnalysisBased.Seting)
                {
                    //View_Activity model = new ScanCodeMarketBLL().GetActivity(Convert.ToInt32(codeInfo.CodeSeting.ID), 0);
                    var model = new ScanCodeMarketBLL().GetModel(settingId, codeInfo.EnterpriseID);
                    ViewBag.settingId = settingId;
                    ViewBag.ActivityId = model.ActivityID;
                    ViewBag.Tel = SessCokieOrder.Get == null ? "" : SessCokieOrder.Get.GetLinkPhone;
                    couponModel = new CouponBLL().GetActivitySub(ViewBag.ActivityId);
                    if (couponModel == null)
                    {
                        couponModel = new View_ActivityCoupon();
                        couponModel.ActivityID = 0;
                        //预览模式
                        ViewBag.PreView = "0";
                        return View(couponModel);
                    }
                }
                else
                { 
                    couponModel = new View_ActivityCoupon();
                    couponModel.ActivityID = 0;
                    //预览模式
                    ViewBag.PreView = "0";
                    return View(couponModel);
                }
            }
            catch (Exception)
            {
                return Content("<script>alert('网络异常，请重新拍码访问')</script>");
            }
            //正常模式
            ViewBag.PreView = "0";
            return View(couponModel);
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <param name="tel">手机号</param>
        /// <returns></returns>
        public ActionResult GetCoupon(long activityId, string tel)
        {
            if (SessCokieOrder.Get != null)
            {
                tel = SessCokieOrder.Get.GetLinkPhone;
            }
            if (string.IsNullOrEmpty(tel))
            {
                return Json(new { msg = "请输入手机号！", flag = false });
            } 
            CodeInfo codeInfo = GetSession(null, 0);
            RetResult result = new CouponBLL().CouponCanGet(codeInfo.FwCode.EWM, 0, 0, activityId);
            JsonResult json = new JsonResult();
            View_ActivityCouponGetRecord model = new View_ActivityCouponGetRecord();
            if (result.Code == 5 || ((result.Code == 7 && codeInfo.FwCode.ActivityCouponID > 0)))
            {
                //插入优惠券记录
                string couponCode = "YHQ" + DateTime.Now.ToString("HHddssfff");
                YX_CouponGetRecord record = new YX_CouponGetRecord
                {
                    EwmCode = codeInfo.FwCode.EWM,
                    ActivityID = activityId,
                    GetTime = DateTime.Now,
                    CouponCode = couponCode,
                    CancelState = (int)Common.EnumText.CancelOutStatus.WCancelOut };
                result = new CouponBLL().GetCoupon(tel, record);
                json.Data = new { msg = result.Msg + "兑换码：" + couponCode, flag = result.IsSuccess, couponCode = couponCode };
                if (result.IsSuccess)
                {
                    model = new CouponBLL().GetCouponInfo(couponCode);
                    return View(model);
                }
            }
            else
            {
                json.Data = new { msg = result.Msg, flag = false };
                RedirectToRouteResult rtr = null;
                rtr = RedirectToAction("GetError", new { msg = result.Msg });
                return rtr;
            }
            //return json;
            return View(model);
        }

        /// <summary>
        /// 优惠券详情
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult CouponDetail(long activityId)
        {
            var model = new CouponBLL().GetActivitySub(activityId);
            return View(model);
        }

        /// <summary>
        /// 获取优惠券列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCouponLst()
        {
            if (SessCokieOrder.Get == null)
            {
                return RedirectToAction("Login", "Wap_Order", new { area = "" });
            }
            var modelLst = new CouponBLL().GetList(SessCokieOrder.Get.Order_Consumers_ID);
            return View(modelLst);
        }

        /// <summary>
        /// 优惠券领取错误页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GetError(string msg)
        {
            ViewBag.Msg = msg;
            return View();
        }

        public CodeInfo GetSession(string ewm, int type)
        {
            CodeInfo code = null;
            if (ewm != null)
            {
                code = new ScanCodeBLL().GetCode(ewm, type);
            }
            else
            {
                code = Session["code"] as CodeInfo;
            }
            return code;
        }
    }
}
