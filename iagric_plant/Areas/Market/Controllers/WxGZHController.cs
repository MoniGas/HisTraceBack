using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using BLL;
using Common.Log;

//namespace iagric_plant.Areas.Market.Controllers
namespace MarketActive.Controllers
{
    public class WxGZHController : Controller
    {
        //
        // GET: /Market/WxGZH/

        public ActionResult Index()
        {
            EnterpriseWxGZH result = new EnterpriseWxGZH();
            try
            {
                LoginInfo user = SessCokie.Get;
                if (user != null)
                {
                    ViewBag.eId = user.EnterpriseID;
                }
                result = new WxGZHBLL().GetModel(user.EnterpriseID);
                if (result == null)
                {
                    result = new EnterpriseWxGZH();
                }
            }
            catch (Exception ex)
            {
                string errData = "WxGZHController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return View(result);
        }

        /// <summary>
        /// post提交保存企业微信公众号设置
        /// </summary>
        /// <param name="AppID">开发者ID</param>
        /// <param name="AppSecret">开发者密码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddEditEnGZH(string AppID, string AppSecret)
        {
            RetResult result = new RetResult();
            try
            {
                WxGZHBLL bll = new WxGZHBLL();
                EnterpriseWxGZH model = new EnterpriseWxGZH();
                model.AddDate = DateTime.Now;
                model.WxAppId = AppID;
                model.AppSecret = AppSecret;
                model.Status = (int)Common.EnumText.Status.used;
                if (SessCokie.Get != null)
                {
                    model.Enterprise_Info_ID = (long)SessCokie.Get.EnterpriseID;
                }
                else
                {
                    return Json(new { ok = result.CmdError, msg = "请先登录！" });
                }
                result = bll.AddEditEnGZH(model);
            }
            catch
            { }
            return Json(new { ok = result.CmdError, msg = result.Msg, url = "/Market/WxGZH/Index" });
        }
    }
}
