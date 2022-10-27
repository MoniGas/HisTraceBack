/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-11-16
** 联系方式:13313318725
** 代码功能：企业微信账户设置控制器
** 版本：v1.0
** 版权：项目二部
*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using BLL;
using MarketActive.Filter;

//namespace iagric_plant.Areas.Market.Controllers
namespace MarketActive.Controllers
{
    [AdminAuthorize]
    /// <summary>
    /// 企业微信账户设置控制器
    /// </summary>
    public class WxEnAccountController : Controller
    {
        //
        // GET: /Market/WxEnAccount/

        /// <summary>
        /// 企业微信账户设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            YX_WxEnAccount result = new YX_WxEnAccount();
            try
            {
                LoginInfo user = SessCokie.Get;
                if (user != null)
                {
                    ViewBag.eId = user.EnterpriseID;
                }
                result = new WxEnAccountBLL().GetModel(user.EnterpriseID);
                if (result == null)
                {
                    result = new YX_WxEnAccount();
                }
            }
            catch (Exception ex)
            {
                string errData = "WxEnAccountController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return View(result);
        }

        /// <summary>
        /// post提交保存企业微信账户设置
        /// </summary>
        /// <param name="AppID">开发者ID</param>
        /// <param name="AppSecret">开发者密码</param>
        /// <param name="WxShangHuHao">微信商户号</param>
        /// <param name="QMApiMiYao">签名API秘钥</param>
        /// <param name="APIFileURL">API证书</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddEditEnAccount(string AppID, string AppSecret, string WxShangHuHao, string QMApiMiYao, string APIFileURL)
        {
            RetResult result = new RetResult();
            try
            {
                WxEnAccountBLL bll = new WxEnAccountBLL();
                YX_WxEnAccount model = new YX_WxEnAccount();
                model.AddDate = DateTime.Now;
                model.APIFileURL = APIFileURL;
                model.WxAppId = AppID;
                model.AppSecret = AppSecret;
                model.Key = QMApiMiYao;
                model.MarId = WxShangHuHao;
                model.Status = (int)Common.EnumText.Status.used;
                if (SessCokie.Get != null)
                {
                    model.Enterprise_Info_ID = (long)SessCokie.Get.EnterpriseID;
                }
                else
                {
                    return Json(new { ok = result.CmdError, msg = "请先登录！" });
                }
                result = bll.AddEditEnAccount(model);
            }
            catch
            { }
            return Json(new { ok = result.CmdError, msg = result.Msg, url = "/Market/WxEnAccount/Index" });
        }
    }
}
