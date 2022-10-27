using System;
using System.Web.Mvc;
using LinqModel;
using BLL;
using Common.Argument;

namespace iagric_plant.Controllers
{
    public class Wap_Index4Controller : Controller
    {
        //
        // GET: /Wap_Index4/
        private readonly ScanCodeBLL _bll = new ScanCodeBLL();
        public ActionResult Index(string ewm)
        {
            RequestCodeSettingMuBan mubanModel = new RequestCodeSettingMuBan();
            try
            {
                CodeInfo code = GetSession(ewm, 0);
                if (code == null || (code.CodeType == (int)Common.EnumFile.AnalysisBased.Seting && code.FwCode.Enterprise_FWCode_ID == 0))
                {
                    return Content("<script>alert('二维码错误，不是该平台的二维码！')</script>");
                }
                else
                {
                    mubanModel = _bll.GetMuBanModel(code.EnterpriseID, code.CodeSeting.ID);
                    ViewBag.EWM = code.FwCode.EWM;
                    #region 红包相关内容 藏红包 和抢红包两种情况
                    RetResult resultModel = new ScanCodeMarketBLL().CanGetRedPacket(code.CodeSeting.ID, code.FwCode, 0);
                    if (resultModel == null)
                    {
                        ViewBag.IsDiplayPacket = false;
                    }
                    else
                    {
                        if (resultModel.Code == 2 || (resultModel.Code == 3 && code.FwCode.ActivitySubID != null && code.FwCode.ActivitySubID > 0))
                        {
                            ViewBag.IsDiplayPacket = true;
                            ViewBag.PacketUrl = "/Market/Wap_IndexMarket/Index?settingId=" + code.CodeSeting.ID.ToString() + "&ewm=" + code.FwCode.EWM;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
            }
            return View(mubanModel);
        }
        /// <summary>
        /// 获取session中的二维码数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public CodeInfo GetSession(string ewm, int type)
        {
            CodeInfo code;
            if (!string.IsNullOrEmpty(ewm) && Session["code"] == null)
            {
                code = _bll.GetCode(ewm, type);
            }
            else
            {
                code = Session["code"] as CodeInfo;
            }
            return code;
        }

        public ActionResult IndexOne()
        {
            RedirectToRouteResult rtr = null;
            try
            {
                CodeInfo codeInfo =  GetSession(null, 0);
                ViewBag.MaterialId = codeInfo.MaterialID;
                ViewBag.EnterpriseId = codeInfo.EnterpriseID;
                ViewBag.ewm = codeInfo.FwCode.EWM;
                ViewBag.VideoUrl = _bll.GetMaterialShopLike(codeInfo.MaterialID);
                ViewBag.MaterialVideo = _bll.MaterialVideo(codeInfo.MaterialID);
                #region 红包相关内容 藏红包 和抢红包两种情况
                RetResult resultModel = new ScanCodeMarketBLL().CanGetRedPacket(codeInfo.CodeSeting.ID, codeInfo.FwCode, 0);
                if (resultModel == null)
                {
                    ViewBag.IsDiplayPacket = false;
                }
                else
                {
                    if (resultModel.Code == 2 || (resultModel.Code == 3 && codeInfo.FwCode.ActivitySubID != null && codeInfo.FwCode.ActivitySubID > 0))
                    {
                        ViewBag.IsDiplayPacket = true;
                        ViewBag.PacketUrl = "/Market/Wap_IndexMarket/Index?settingId=" + codeInfo.CodeSeting.ID.ToString() + "&ewm=" + ViewBag.ewm;
                    }
                }
                #endregion
                if (rtr != null) return rtr;
                //ViewBag.StyleModel = codeInfo.CodeSeting.StyleModel;
            }
            catch
            {
                rtr = RedirectToAction("NoSales", "Wap_Index");
                return rtr;
            }
            return View();
        }
    }
}
