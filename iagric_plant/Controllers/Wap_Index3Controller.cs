using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LinqModel;
using BLL;
using Common.Argument;
using Common.Tools;
using InterfaceWeb;
using iagric_plant.Areas.Market.Models;

namespace iagric_plant.Controllers
{
    public class Wap_Index3Controller : Controller
    {
        readonly ScanCodeBLL _bll = new ScanCodeBLL();
        public string RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"];
        public ActionResult P(string ewm)
        {
            try
            {
                CodeInfo codeInfo = GetSession(ewm, 0);
                if (codeInfo == null || (codeInfo.CodeType == (int)Common.EnumFile.AnalysisBased.Seting && codeInfo.FwCode.Enterprise_FWCode_ID == 0))
                {
                    return Content("<script>alert('二维码错误，不是该平台的二维码！')</script>");
                }
                else
                {
                    //对接微信授权登录接口
                    WxEnAccountBLL wxbll = new WxEnAccountBLL();
                    EnterpriseWxGZH wxzh = wxbll.GetGzModel(codeInfo.EnterpriseID);
                    string url;
                    if (wxzh != null)
                    {
                        Session["wxgzh"] = wxzh;
                        WriteLog1.WriteWxLog("【获取wxgzh" + DateTime.Now + "】" + wxzh.WxAppId + "  AppSecret:" + wxzh.AppSecret, "Gzh");
                        url = WxDataDAL.GetCodeUrlBypayId(wxzh.WxAppId, RedirectUrl + "Wap_Index3/Index");
                        WriteLog1.WriteWxLog("【获取url" + DateTime.Now + "】" + url, "Gzh");
                        return Content("<script>location.href='" + url + "'</script>");
                    }
                    else
                    {
                        url = RedirectUrl + "Wap_Index3/Material";
                        return Content("<script>location.href='" + url + "'</script>");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public ActionResult Index()
        {
            CodeInfo codeInfo = GetSession(null, 0);
            //微信返回code
            string code = Request["code"];
            if (Session["wxgzh"] != null && !string.IsNullOrWhiteSpace(code))
            {
                EnterpriseWxGZH wxzh = Session["wxgzh"] as EnterpriseWxGZH;
                WriteLog1.WriteWxLog("【Index获取wxgzh" + DateTime.Now + "】" + wxzh.WxAppId + "  AppSecret:" + wxzh.AppSecret, "Gzh");
                string data = WxDataDAL.GetAccessTokenBypayId(code, wxzh.WxAppId, wxzh.AppSecret);
                WriteLog1.WriteWxLog("【Index获取data" + DateTime.Now + "】" + data, "Gzh");
                if (!string.IsNullOrWhiteSpace(data))
                {
                    ResponseAccessTokenModel accModel = JsonHelper.DeserializeJsonToObject<ResponseAccessTokenModel>(data);
                    if (accModel != null)
                    {
                        string AccToken = WxDataDAL.GetGzhUser1(wxzh.WxAppId, wxzh.AppSecret);
                        WriteLog1.WriteWxLog("【获取GetGzhUser1  Token" + DateTime.Now + "】" + AccToken, "Gzh");
                        if (!string.IsNullOrWhiteSpace(AccToken))
                        {
                            AccToken token = JsonHelper.DeserializeJsonToObject<AccToken>(AccToken);
                            if (token != null)
                            {
                                string ss = WxDataDAL.GetGzhUser(token.access_token, accModel.openid);
                                WriteLog1.WriteWxLog("【获取GetGzhUser  Token" + DateTime.Now + "】" + token.access_token + "  openid" + accModel.openid, "Gzh");
                                WriteLog1.WriteWxLog("【获取ss  Token" + DateTime.Now + "】" + ss, "Gzh");
                                ResponseGzhUser wxUser = JsonHelper.DeserializeJsonToObject<ResponseGzhUser>(ss);
                                if (wxUser != null && wxUser.subscribe != "0" && wxUser.subscribe != null)
                                {
                                    string aa = "已关注";
                                    string url = RedirectUrl + "Wap_Index3/Material";
                                    return Content("<script>location.href='" + url + "'</script>");
                                }
                                else
                                {
                                    if (codeInfo != null && codeInfo.Enterprise != null)
                                    {
                                        ViewBag.Enterprise = codeInfo.Enterprise;
                                        Enterprise_Info eninfo = new BLL.EnterpriseInfoBLL().GetModel(codeInfo.EnterpriseID);
                                        if (eninfo.wxlogoimgs.Count != 0)
                                        {
                                            ViewBag.WXLogo = eninfo.wxlogoimgs[0].fileUrls;
                                        }
                                        data = WxDataDAL.GetGzhUrl(token.access_token, accModel.openid);
                                        WriteLog1.WriteWxLog("【获取GetGzhUrl" + DateTime.Now + "】" + data, "Gzh");
                                        Ticket ticModel = JsonHelper.DeserializeJsonToObject<Ticket>(data);
                                        if (ticModel != null && !string.IsNullOrEmpty(ticModel.url))
                                        {
                                            ViewBag.url = ticModel.url;
                                            //ViewBag.url = "http://weixin.qq.com/q/02P33YsScYdq41C1lHxr1K";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (codeInfo != null && codeInfo.Enterprise != null)
                {
                    ViewBag.Enterprise = codeInfo.Enterprise;
                    Enterprise_Info eninfo = new BLL.EnterpriseInfoBLL().GetModel(codeInfo.EnterpriseID);
                    if (eninfo.wxlogoimgs.Count != 0)
                    {
                        ViewBag.WXLogo = eninfo.wxlogoimgs[0].fileUrls;
                    }
                    //data = WxDataDAL.GetGzhUrl(accModel.access_token, accModel.openid);
                    //Ticket ticModel = JsonHelper.DeserializeJsonToObject<Ticket>(data);
                    //if (ticModel != null && !string.IsNullOrEmpty(ticModel.url))
                    //{
                    //    ViewBag.url = ticModel.url;
                    //}
                }
            }
            #region 红包相关内容 藏红包 和抢红包两种情况
            if (codeInfo != null && codeInfo.Enterprise != null)
            {
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
                        ViewBag.PacketUrl = "/Market/Wap_IndexMarket/Index?settingId=" + codeInfo.CodeSeting.ID.ToString() + "&ewm=" + codeInfo.FwCode.EWM;
                    }
                }
            }
            #endregion
            return View();
        }
        /// <summary>
        /// 产品追溯信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Material()
        {
            CodeInfo code = GetSession(null, 0);
            if (code != null)
            {
                string ewm = code.FwCode.EWM;
                ScanCodeBLL scan = new ScanCodeBLL();
                ScanMaterial material = scan.GetMaterialNew(code.MaterialID, code.FwCode.EWM);
                ViewBag.material = material;
                ViewBag.EWM = ewm;
                EnterpriseShopLink shopEn = _bll.ShopEn(Convert.ToInt64(code.EnterpriseID));
                ViewBag.ShopEn = shopEn;
                var enterprise = _bll.GetEnterprise(Convert.ToInt64(code.EnterpriseID));
                List<View_MaterialSpecForMarket> materialSpecList =
                      new Material_OnlineOrderBLL().GetMarketMaterialSpecList(code.MaterialID);
                ViewBag.MaterialSpecList = materialSpecList;
                ViewBag.enterprise = enterprise;
                View_Order_Consumers consumers = SessCokieOrder.Get;
                if (consumers != null)
                {
                    ViewBag.Consumers = true;
                }
                else
                {
                    ViewBag.Consumers = false;
                }
                //防伪验证次数
                if (code.FwCode.FWCount == null || code.FwCode.FWCount == 0)
                {
                    ViewBag.FWCount = 1;
                    RetResult ret = new ScanCodeBLL().UpdateCount(ewm, false, true, code.FwCode.FWCount);
                }
                else
                {
                    ViewBag.FWCount = code.FwCode.FWCount + 1;
                    ViewBag.ValidateTime = code.FwCode.ValidateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    RetResult ret = new ScanCodeBLL().UpdateCount(ewm, false, true, code.FwCode.FWCount);
                }
                //拍码次数
                //是否显示拍码次数
                ViewBag.BoolScanCount = true;
                if (code.Display.ScanCount == false)
                {
                    //是否显示拍码次数
                    ViewBag.BoolScanCount = false;
                }
                else
                {
                    ViewBag.BoolScanCount = true;
                }
                ViewBag.ScanCount = code.FwCode.ScanCount == null ? 0 : code.FwCode.ScanCount;
                //获取模板三所需要的图片
                EnterpriseMuBanThreeImgBLL mubanbll = new EnterpriseMuBanThreeImgBLL();
                EnterpriseMuBanThreeImg mubanImgs = mubanbll.GetMuBan3Model(code.EnterpriseID);
                ViewBag.MuBanImgs = mubanImgs;
                return View();
            }
            else
            {
                return Content("<script>alert('请拍码访问页面！')</script>");
            }
        }
        /// <summary>
        /// 溯源信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Sy()
        {
            List<Batch_JianYanJianYi> batchJianYanJianCe = new List<Batch_JianYanJianYi>();
            List<Batch_XunJian> batchXunJian = new List<Batch_XunJian>();
            List<View_ZuoYeAndZuoYeType> shengChan = new List<View_ZuoYeAndZuoYeType>();
            List<View_ZuoYeAndZuoYeType> jiaGong = new List<View_ZuoYeAndZuoYeType>();
            List<View_ZuoYeAndZuoYeType> feed = new List<View_ZuoYeAndZuoYeType>();
            List<View_RequestOrigin> origin = new List<View_RequestOrigin>();
            ScanSubstation banzu = new ScanSubstation();
            ScanWareHouseInfo cunchu = new ScanWareHouseInfo();
            ScanLogistics wuliu = new ScanLogistics();
            Dealer dealer = new Dealer();
            ViewBag.BoolOrigin = false;
            ViewBag.BoolReport = false;
            ViewBag.BoolWork = false;
            ViewBag.BoolCheck = false;
            ViewBag.BoolAmbient = false;
            ViewBag.BoolWuLiu = false;
            try
            {
                CodeInfo code = GetSession(null, 0);
                ViewBag.ewm = code.FwCode.EWM;
                if (code != null)
                {
                    ScanCodeBLL scan = new ScanCodeBLL();
                    if (code.CodeType == (int)Common.EnumFile.AnalysisBased.Seting)//配置数据
                    {
                        if (code.Display.Origin == true)
                        {
                            ViewBag.BoolOrigin = true;
                            origin = scan.GetYuanliao(code.CodeSeting.ID);
                        }
                        if (code.Display.Work == true)
                        {
                            ViewBag.BoolWork = true;
                            shengChan = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Produce, code.CodeSeting.ID);
                            jiaGong = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Processing, code.CodeSeting.ID);
                            feed = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Feed, code.CodeSeting.ID);
                            banzu = _bll.GetSubstation(code.CodeSeting.ID);
                        }
                        if (code.Display.Report == true)
                        {
                            ViewBag.BoolReport = true;
                            batchJianYanJianCe = scan.GetJianYanJianCe(0, 0, code.CodeType, code.CodeSeting.ID);
                        }
                        if (code.Display.Check == true)
                        {
                            ViewBag.BoolCheck = true;
                            batchXunJian = scan.GetXunJian(0, 0, code.CodeType, code.CodeSeting.ID);
                        }
                        if (code.Display.Ambient == true)
                        {
                            ViewBag.BoolAmbient = true;
                            cunchu = _bll.GetWareHouse(code.CodeSeting.ID);
                        }
                        if (code.Display.WuLiu == true)
                        {
                            ViewBag.BoolWuLiu = true;
                            wuliu = _bll.GetLogistics(code.CodeSeting.ID);
                        }
                    }
                    else if (code.CodeType == (int)Common.EnumFile.AnalysisBased.setEwm && code.CodeSeting.ID > 0)
                    {
                        if (code.Display.Origin == true)
                        {
                            ViewBag.BoolOrigin = true;
                            origin = scan.GetYuanliao(code.CodeSeting.ID);
                        }
                        if (code.Display.Work == true)
                        {
                            ViewBag.BoolWork = true;
                            shengChan = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Produce, code.CodeSeting.ID);
                            jiaGong = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Processing, code.CodeSeting.ID);
                            feed = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Feed, code.CodeSeting.ID);
                            banzu = _bll.GetSubstation(code.CodeSeting.ID);
                        }
                        if (code.Display.Report == true)
                        {
                            ViewBag.BoolReport = true;
                            batchJianYanJianCe = scan.GetJianYanJianCe(0, 0, code.CodeType, code.CodeSeting.ID);
                        }
                        if (code.Display.Check == true)
                        {
                            ViewBag.BoolCheck = true;
                            batchXunJian = scan.GetXunJian(0, 0, code.CodeType, code.CodeSeting.ID);
                        }
                        if (code.Display.Ambient == true)
                        {
                            ViewBag.BoolAmbient = true;
                            cunchu = _bll.GetWareHouse(code.CodeSeting.ID);
                        }
                        if (code.Display.WuLiu == true)
                        {
                            ViewBag.BoolWuLiu = true;
                            wuliu = _bll.GetLogistics(code.CodeSeting.ID);
                        }
                    }
                    dealer = scan.GetDealer(code.DealerID);
                }
            }
            catch { }
            ViewBag.batchJianYanJianCe = batchJianYanJianCe;
            ViewBag.batchXunJian = batchXunJian;
            ViewBag.batchShengChan = shengChan;
            ViewBag.batchJiaGong = jiaGong;
            ViewBag.feed = feed;
            ViewBag.origin = origin;
            ViewBag.dealer = dealer;
            ViewBag.banzu = banzu;
            ViewBag.cunchu = cunchu;
            ViewBag.wuliu = wuliu;
            return View();
        }
        /// <summary>
        /// 企业信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Enterprise()
        {
            CodeInfo model = GetSession(null, 0);
            Enterprise_Info ObjEnterprise_Info = new Enterprise_Info();
            ViewBag.Logo = null;
            try
            {
                ObjEnterprise_Info = new BLL.EnterpriseInfoBLL().GetModel(model.EnterpriseID);
                if (ObjEnterprise_Info.imgs.Count != 0)
                {
                    ViewBag.Logo = ObjEnterprise_Info.imgs[0].fileUrls;
                }
                LinqModel.ShowCompany ShowCompanyModel = new BLL.ShowCompanyBLL().GetModel(model.EnterpriseID).ObjModel as LinqModel.ShowCompany;
                ViewBag.ShowCompanyModel = ShowCompanyModel;
            }
            catch (Exception ex)
            { }
            return View(ObjEnterprise_Info);
        }

        /// <summary>
        /// 获取session中的二维码数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public CodeInfo GetSession(string ewm, int type)
        {
            CodeInfo code = null;
            if (!string.IsNullOrEmpty(ewm) && Session["code"]==null)
            {
                ScanCodeBLL bll = new ScanCodeBLL();
                code = bll.GetCode(ewm, type);
            }
            else
            {
                code = Session["code"] as CodeInfo;
                //if (code.FwCode.EWM != ewm)
                //{
                //    code = null;
                //}
            }
            return code;
        }
    }
}
