using System.Web.Mvc;
using LinqModel;
using BLL;

namespace MarketActive.Controllers
{
    public class Wap_LotteryController : Controller
    {
        //
        // GET: /Market/Wap_Lottery/

        /// <summary>
        /// 跳转地址
        /// </summary>
        public string _RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"];

        public ActionResult Index()
        {
            //try
            //{
            //    CodeInfo codeInfo = GetSession(null, 0);
            //    if (null != codeInfo && codeInfo.CodeType == (int)EnumFile.AnalysisBased.Seting)
            //    {
            //        //对接微信授权登录接口
            //        WxEnAccountBLL wxbll = new WxEnAccountBLL();
            //        YX_WxEnAccount wxzh = wxbll.GetModel(codeInfo.EnterpriseID);
            //        string url = string.Empty;
            //        if (wxzh != null)
            //        {
            //            Session["wxzh"] = wxzh;
            //            url = GetCodeUrlBypayId(wxzh.WxAppId);
            //        }
            //        else
            //        {
            //            Session["wxzh"] = null;
            //            url = GetCodeUrl();
            //        }
            //        WriteLog.WriteWxLog("【领取红包：" + DateTime.Now + "】" + codeInfo.FwCode.EWM, "Wx");
            //        return Content("<script>location.href='" + url + "'</script>");
            //    }
            //}
            //catch
            //{
            //    return Content("<script>alert('网络异常，请重新拍码！')</script>");
            //}
            return View();
        }

        /// <summary>
        /// 获取码
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 获取session中的二维码数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
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
