/********************************************************************************
** 作者： 赵慧敏v2.5版本修改
** 创始时间：2017-2-09
** 联系方式 :15031109901
** 描述：拍码追溯页面
** 版本：v2.5
** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using LinqModel;
using Common.Argument;
using Common;
using System.Configuration;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 拍码追溯页面
    /// </summary>
    public class Wap_IndexOldController : Controller
    {
        ScanCodeBLL _Bll = new ScanCodeBLL();
        /// <summary>
        /// 拍码追溯页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            RedirectToRouteResult rtr = null;
            try
            {
                Encryption objEncryption = new Encryption();
                string ewm = Request["ewm"];
                string EnterpriseId = string.Empty;
                string uppage = Request["uppage"] ?? "";
                // 加密二维码转换成明文二维码
                ewm = objEncryption.CodeDecrypt(ewm);
                //根据二维码查询二维码信息，赋予 CodeInfo
                CodeInfo codeInfo = new CodeInfo();
                //0:取session二维码1:拍码二维码
                int type = 0;
                // 二维码为空，session二维码为空
                if (string.IsNullOrEmpty(ewm) && Session["ewm"] == null)
                {
                    return Content("<script>alert('请拍码访问页面！')</script>");
                }
                // 二维码为空，session二维码不为空 取session二维码;二维码不为空，并且与session二维码相同 不需要增加查询次数
                else if ((string.IsNullOrEmpty(ewm) && Session["ewm"] != null)
                    || ((!string.IsNullOrEmpty(ewm)) && Session["ewm"] != null && Session["ewm"].ToString().Equals(ewm)))
                {
                    ewm = Session["ewm"].ToString();
                }
                // 二维码不为空，并且session二维码不相同或者session二维码为空 正常拍码操作
                else if (!string.IsNullOrEmpty(ewm) && ((Session["ewm"] != null
                    && !Session["ewm"].ToString().Equals(ewm)) || Session["ewm"] == null))
                {
                    type = 1;
                }
                #region 判断二维码类型
                string[] infos = ewm.Split('.');
                if (!(infos.Length == 8 || infos.Length == 9 || infos.Length == 10 || infos.Length == 7))
                {
                    return Content("<script>alert('二维码格式不正确！');</script>");
                }
                switch (infos[infos.Length - 2])
                {
                    case "1":
                        rtr = RedirectToAction("Company", "Wap_Com", new { ewm = ewm });
                        break;
                    case "3":
                        rtr = RedirectToAction("Index", "Wap_Card", new { ewm = ewm });
                        break;
                    case "7":
                        return Content("<script>alert('该码为生产单元码，请用农业平台App识别！')</script>");
                    case "8":
                        ewm = string.Empty;
                        for (int i = 0; i < infos.Length - 2; i++)
                        {
                            ewm += infos[i] + ".";
                        }
                        ewm = ewm.Substring(0, ewm.Length - 1);
                        // 转换成明文二维码
                        ewm = new Encryption().CodeDecrypt(ewm);
                        Session["ewm"] = ewm;
                        rtr = RedirectToAction("Validate", "Wap_Index", new { VerificationCode = infos[infos.Length - 1], IsScan = true });
                        break;
                    case "4":
                        return Content("<script>alert('该码为仓库码，请用农业平台App识别！')</script>");
                    case "10":
                        return Content("<script>alert('该码为垛位码，请用农业平台App识别！')</script>");
                    default:
                        //预览模式下
                        if (infos[infos.Length - 2].Trim().Equals("9"))
                        {
                            ewm = ewm.Substring(0, ewm.LastIndexOf("9") - 1);
                            //预览模式下type赋值0，不增加拍码数据
                            type = 0;
                            ViewBag.OnlyView = true;
                        }
                        else if (infos[infos.Length - 2].Trim().Equals("10"))
                        {
                            type = 0;
                            ViewBag.OnlyView = true;
                        }
                        else
                        {
                            ViewBag.OnlyView = false;
                        }
                        //根据二维码查询二维码数据
                        codeInfo = _Bll.GetCode(ewm, type);
                        if (codeInfo == null)
                        {
                            return Content("<script>alert('二维码错误，不是该平台的二维码！')</script>");
                        }

                        //二维码内容
                        Session["ewm"] = ewm;
                        //二维码数据
                        Session["code"] = codeInfo;
                        //判断产品是否被停止解析
                        RetResult result = GetAnalysis(codeInfo);
                        if (codeInfo.RequestCodeType == (int)Common.EnumFile.RequestCodeType.SecurityCode)
                        {
                            rtr = RedirectToAction("SecurityCode", "Wap_Index", new { ewm = ewm });
                            //ViewBag.ewm = ewm;
                            //ViewBag.uppage = uppage;
                            if (rtr != null) return rtr;
                        }
                        if (result != null && result.CmdError == CmdResultError.NO_RIGHT)
                        {
                            return Content("<script>alert('" + result.Msg + "')</script>");
                        }
                        else if (result != null && result.CmdError == CmdResultError.Other)
                        {
                            ViewBag.OnlyView = true;
                        }
                        ViewBag.MaterialId = codeInfo.MaterialID;
                        ViewBag.EnterpriseId = codeInfo.EnterpriseID;
                        //是否为图片防伪，如果不是图片防伪，就要查询配置项
                        RequestCodeBLL bll = new RequestCodeBLL();
                        ViewBag.CodeValidation = true;
                        if (ViewBag.CodeValidation == true && codeInfo.CodeType == (int)Common.EnumFile.AnalysisBased.Seting && codeInfo.Display.Verification == false)
                        {
                            ViewBag.CodeValidation = false;
                        }
                        // 获取导航列表
                        List<View_NavigationForMaterial> NavigationForMaterialList = new List<View_NavigationForMaterial>();
                        AddDefaultNavigation(NavigationForMaterialList);
                        ViewBag.NavigationForMaterialList = NavigationForMaterialList;
                        break;
                }
                #endregion
                ViewBag.ewm = ewm;
                ViewBag.uppage = uppage;
                if (rtr != null) return rtr;
            }
            catch
            {
                rtr = RedirectToAction("NoSales", "Wap_Index");
                return rtr;
            }
            return View();
        }

        /// <summary>
        /// 根据企业信息和产品信息查询产品解析
        /// </summary>
        /// <param name="codeInfo"></param>
        /// <returns></returns>
        public RetResult GetAnalysis(CodeInfo codeInfo)
        {
            RetResult result = new RetResult();
            Enterprise_Info myEnterprise = new BLL.EnterpriseInfoBLL().GetModel(codeInfo.EnterpriseID);
            if (myEnterprise.Verify != 1)
            {
                int tryDays = Convert.ToInt32(ConfigurationManager.AppSettings["trydays"]);
                //-2审核不通过；-1未审核；0正常、审核通过
                if (myEnterprise.Verify == (int)Common.EnumFile.EnterpriseVerify.noVerify
                    || myEnterprise.Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                {
                    if (Convert.ToDateTime(myEnterprise.Stradddate).AddDays(tryDays) < DateTime.Now)
                    {
                        result.SetArgument(CmdResultError.NO_RIGHT, "", "该企业试用期限已过，无法查看产品追溯信息");
                        return result;
                    }
                    else
                    {
                        result.SetArgument(CmdResultError.Other, "", "该企业试用期在试用期限内");
                        return result;
                    }
                }
                else if (myEnterprise.Verify < 0)
                {
                    result.SetArgument(CmdResultError.NO_RIGHT, "", "该企业已经被停用，无法查看产品追溯信息");
                    return result;
                }
            }
            Analysis model = new Analysis();
            SysAnalysisBLL bll = new SysAnalysisBLL();
            model = bll.GetAnalysis(codeInfo);
            if (model != null && model.IsAnalyse == Convert.ToInt64(Common.EnumFile.AnalysisType.StopAnalysis))
            {
                result.SetArgument(CmdResultError.NO_RIGHT, "", "该产品已经被停用，无法查看产品追溯信息");
            }
            else
            {
                result.SetArgument(CmdResultError.NONE, "", "");
            }
            return result;
        }

        /// <summary>
        /// 获取产品信息170209改动
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="CodeValidation">验证</param>
        /// <param name="OnlyView">是否演示码</param>
        /// <returns></returns>
        public ActionResult MaterialInfo(string ewm, string CodeValidation = "true", string OnlyView = "false")
        {
            Material material = null;
            Brand brand = null;
            Brand areaBrand = null;
            Enterprise_Info enterprise = null;
            ViewBag.ScanCount = 0;
            ViewBag.seleTime = DateTime.Now;
            //是否显示品牌
            ViewBag.BoolBrand = true;
            //是否显示生产日期
            ViewBag.BoolCreateDate = true;
            //是否显示拍码次数
            ViewBag.BoolScanCount = true;
            try
            {
                CodeInfo code = GetSession(null, 0);
                if (code != null)
                {
                    //显示品牌再查询数据
                    if (code.Display.Brand == false)
                    {
                        ViewBag.BoolBrand = false;
                    }
                    else
                    {
                        brand = _Bll.GetBrand(code.BrandID);
                        areaBrand = _Bll.GetAreaBrand(code.MaterialID);
                    }
                    if (code.Display.CreateDate == false)
                    {
                        ViewBag.BoolCreateDate = false;
                    }
                    if (code.Display.ScanCount == false)
                    {
                        //是否显示生产日期
                        ViewBag.BoolScanCount = false;
                    }
                    if (code.Display.Verification == false)
                    {
                        CodeValidation = "false";
                    }
                    else
                    {
                        CodeValidation = "true";
                    }
                    enterprise = _Bll.GetEnterprise(Convert.ToInt64(code.EnterpriseID));
                    List<View_ProductInfoForMaterial> PropertyList =
                        new BLL.ProductInfoBLL().GetMaterialProperty(Convert.ToInt64(code.EnterpriseID),
                        code.MaterialID);
                    List<View_MaterialSpecForMarket> MaterialSpecList =
                       new BLL.Material_OnlineOrderBLL().GetMarketMaterialSpecList(material.Material_ID);
                    ViewBag.MaterialSpecList = MaterialSpecList;
                    ViewBag.ScanCount = code.FwCode.ScanCount == null ? 0 : code.FwCode.ScanCount;
                    ViewBag.FWCount = code.FwCode.FWCount;
                    ViewBag.seleTime = code.FwCode.SalesTime;
                    ViewBag.PropertyList = PropertyList;
                }
                else
                {
                    return Content("<script>alert('请拍码访问页面！')</script>");
                }
            }
            catch { }
            ViewBag.material = material;
            ViewBag.brand = brand;
            ViewBag.areaBrand = areaBrand;
            ViewBag.enterprise = enterprise;
            // 预览标识
            ViewBag.OnlyView = Convert.ToBoolean(OnlyView);
            // 图形防伪标识
            ViewBag.CodeValidation = Convert.ToBoolean(CodeValidation);
            return View();
        }

        /// <summary>
        /// 获取溯源信息
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public ActionResult Information(string ewm)
        {
            List<Batch_JianYanJianYi> batchJianYanJianCe = new List<Batch_JianYanJianYi>();
            List<Batch_XunJian> batchXunJian = new List<Batch_XunJian>();
            List<View_ZuoYeAndZuoYeType> shengChan = new List<View_ZuoYeAndZuoYeType>();
            List<View_ZuoYeAndZuoYeType> jiaGong = new List<View_ZuoYeAndZuoYeType>();
            List<View_ZuoYeAndZuoYeType> feed = new List<View_ZuoYeAndZuoYeType>();
            List<View_RequestOrigin> origin = new List<View_RequestOrigin>();
            Dealer dealer = new Dealer();
            ViewBag.BoolOrign = false;
            ViewBag.BoolReport = false;
            ViewBag.BoolWork = false;
            ViewBag.BoolCheck = false;
            try
            {
                CodeInfo code = GetSession(null, 0);
                if (code != null)
                {
                    ScanCodeBLL scan = new ScanCodeBLL();
                    if (code.CodeType == (int)Common.EnumFile.AnalysisBased.Batch)//销售批次
                    {
                        ViewBag.BoolReport = true;
                        batchJianYanJianCe = scan.GetJianYanJianCe(code.FwCode.Batch_ID.GetValueOrDefault(0), code.FwCode.BatchExt_ID, code.CodeType, 0);
                        ViewBag.BoolWork = true;
                        shengChan = scan.GetProduce(code.FwCode.Batch_ID.GetValueOrDefault(0), code.FwCode.BatchExt_ID, code.CodeType, (int)Common.EnumFile.ZuoYeType.Produce, 0);
                        jiaGong = scan.GetProduce(code.FwCode.Batch_ID.GetValueOrDefault(0), code.FwCode.BatchExt_ID, code.CodeType, (int)Common.EnumFile.ZuoYeType.Processing, 0);
                        ViewBag.BoolCheck = true;
                        batchXunJian = scan.GetXunJian(code.FwCode.Batch_ID.GetValueOrDefault(0), code.FwCode.BatchExt_ID, code.CodeType, 0);
                    }
                    else if (code.CodeType == (int)Common.EnumFile.AnalysisBased.Seting)//配置数据
                    {
                        if (code.Display.Origin == true)
                        {
                            ViewBag.BoolOrign = true;
                            origin = scan.GetYuanliao(code.CodeSeting.ID);
                        }
                        if (code.Display.Work == true)
                        {
                            ViewBag.BoolWork = true;
                            shengChan = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Produce, code.CodeSeting.ID);
                            jiaGong = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Processing, code.CodeSeting.ID);
                            feed = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Feed, code.CodeSeting.ID);
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
            return View();
        }

        /// <summary>
        /// 获取详细溯源信息
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public ActionResult InformationSub(long typeId, long dataId)
        {
            ScanInfo result = new ScanInfo();
            switch (typeId)
            {
                case 1:
                    result = _Bll.GetJianYanJianCe(dataId);
                    break;
                case 2:
                    result = _Bll.GetProduce(dataId);
                    break;
                case 4:
                    result = _Bll.GetXunJian(dataId);
                    break;
                case 5:
                    result = _Bll.GetYuanliaoSub(dataId);
                    break;
            }
            CodeInfo code = GetSession(null, 0);
            ViewBag.ewm = 1;
            if (code != null)
            {
                ViewBag.ewm = code.FwCode.EWM;
            }
            return View(result);
        }

        /// <summary>
        /// 获取销售信息
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public ActionResult SellInfo(string ewm)
        {
            Enterprise_Info enterprise = null;
            Dealer dealer = null;
            try
            {
                ScanCodeBLL scan = new ScanCodeBLL();
                CodeInfo code = GetSession(ewm, 0);
                if (code != null)
                {
                    enterprise = scan.GetEnterprise(code.EnterpriseID);
                    dealer = scan.GetDealer(code.DealerID);
                    List<View_ProductInfoForMaterial> PropertyList =
                        new BLL.ProductInfoBLL().GetMaterialProperty(Convert.ToInt64(code.EnterpriseID),
                        code.MaterialID);
                    if (PropertyList != null && PropertyList.Count > 0)
                    {
                        ViewBag.ViewLinkPhone = PropertyList[0].ViewComplaintPhone;
                    }
                    else
                    {
                        ViewBag.ViewLinkPhones = true;
                    }
                    LinqModel.ShowCompany ShowCompanyModel = new BLL.ShowCompanyBLL().GetModel(Convert.ToInt64(code.EnterpriseID)).ObjModel as LinqModel.ShowCompany;
                    ViewBag.ShowCompanyModel = ShowCompanyModel;
                    ViewBag.LinkPhone = new BLL.EnterpriseInfoBLL().GetPRRU_PlatForm(enterprise.PRRU_PlatForm_ID).LinkPhone;
                }
            }
            catch { }
            ViewBag.enterprise = enterprise;
            ViewBag.dealer = dealer;
            return View();
        }
        public ActionResult MaterialImgList(long id)
        {
            ScanCodeBLL scan = new ScanCodeBLL();
            Material result = scan.GetMaterial(id);
            return View(result);
        }

        /// <summary>
        /// 未销售显示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult NoSales()
        {
            if (Session["ewm"] == null)
            {
                return Content("<script>alert('请重新拍码访问页面！')</script>");
            }
            string ewm = Session["ewm"].ToString();
            Enterprise_FWCode_00 fwCode = new ScanCodeBLL().GetViewCode(ewm);
            if (fwCode == null)
            {
                return Content("<script>alert('二维码错误，不是该平台二维码！')</script>");
            }
            RequestCode RequestCodeModel = new RequestCode();
            if (fwCode.RequestCode_ID != null)
            {
                RequestCodeModel = new BLL.RequestCodeBLL().GetModel(fwCode.RequestCode_ID.Value);
            }
            ViewBag.RequestCodeModel = RequestCodeModel;
            return View(fwCode);
        }

        /// <summary>
        /// 规格信息
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public ActionResult ParameterInfo(string ewm = "")
        {
            if (string.IsNullOrEmpty(ewm))
            {
                return Content("<script>alert('请重新拍码访问页面！')</script>");
            }
            // 获取二维码信息
            CodeInfo codeInfo = GetSession(ewm, 0);
            Enterprise_FWCode_00 fwCode = codeInfo.FwCode;
            // 获取产品信息
            Material MaterialModel = new MaterialBLL().GetMaterial(codeInfo.MaterialID).ObjModel as Material;
            RequestCode RequestCodeModel = new RequestCode();
            if (fwCode.RequestCode_ID != null)
            {
                RequestCodeModel = new BLL.RequestCodeBLL().GetModel(fwCode.RequestCode_ID.Value);
            }
            ViewBag.RequestCodeModel = RequestCodeModel;
            ViewBag.MaterialModel = MaterialModel;
            return View(fwCode);
        }
        private void AddDefaultNavigation(List<View_NavigationForMaterial> NavigationForMaterialList)
        {
            if (NavigationForMaterialList == null || NavigationForMaterialList.Count == 0)
            {
                View_NavigationForMaterial ObjView_NavigationForMaterial1 = new View_NavigationForMaterial();
                ObjView_NavigationForMaterial1.NavigationId = "Info";
                ObjView_NavigationForMaterial1.NavigationName = "产品信息";
                NavigationForMaterialList.Add(ObjView_NavigationForMaterial1);

                View_NavigationForMaterial ObjView_NavigationForMaterial2 = new View_NavigationForMaterial();
                ObjView_NavigationForMaterial2.NavigationId = "Roots";
                ObjView_NavigationForMaterial2.NavigationName = "溯源";
                NavigationForMaterialList.Add(ObjView_NavigationForMaterial2);

                View_NavigationForMaterial ObjView_NavigationForMaterial3 = new View_NavigationForMaterial();
                ObjView_NavigationForMaterial3.NavigationId = "EnterpriseInfo";
                ObjView_NavigationForMaterial3.NavigationName = "企业介绍";
                NavigationForMaterialList.Add(ObjView_NavigationForMaterial3);
            }
        }

        public ActionResult MaterialOrder(string ewm, string uppage = "1")
        {
            ViewBag.uppage = uppage;
            CodeInfo code = GetSession(ewm, 0);
            if (code != null)
            {
                ScanCodeBLL scan = new ScanCodeBLL();
                Material material = scan.GetMaterial(code.MaterialID);
                Enterprise_Info enterprise = scan.GetEnterprise(code.EnterpriseID);
                ViewBag.material = material;
                ViewBag.enterprise = enterprise;
                ViewBag.code = code;
                return View();
            }
            else
            {
                return Content("<script>alert('请拍码访问页面！')</script>");
            }
        }
        [HttpPost]
        public ActionResult MaterualOrder(long enterpriseId, long materialId, string specName, string price, string count, string total, string address, string type, string code)
        {
            Material_OnlineOrder model = new Material_OnlineOrder();
            JsonResult js = new JsonResult();
            try
            {
                View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
                if (consumers != null)
                {
                    model.ewm = code;
                    model.Addtime = DateTime.Now;
                    model.Consumers_Address = address;
                    model.Enterprise_ID = enterpriseId;
                    model.MateriaID = materialId;
                    model.MaterialCount = Convert.ToInt32(count);
                    model.MaterialPrice = Convert.ToDecimal(price);
                    model.Order_Consumers_ID = consumers.Order_Consumers_ID;
                    model.PayType = Convert.ToInt32(type);
                    model.SpecID = Convert.ToInt64(specName.Split('_')[0]);
                    model.Status = (int)Common.EnumFile.Status.used;
                    model.TotalMoney = model.MaterialPrice * model.MaterialCount;
                    model.OrderNum = DateTime.Now.ToString("yyyyMMddhhmmss");
                    model.OrderType = (int)Common.EnumFile.PayStatus.PayDelivery;
                    if (model.PayType != 1)
                    {
                        model.OrderType = (int)Common.EnumFile.PayStatus.NotPay;
                    }
                    RetResult ret = new BLL.Material_OnlineOrderBLL().Add(model);
                    if (ret.IsSuccess)
                    {
                        switch (model.PayType)
                        {
                            case 1:
                                js.Data = new { res = true, info = ret.Msg, url = "/Wap_Consumers/Index" };
                                break;
                            case 2://支付宝
                                string SiteUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"];
                                js.Data = new
                                {
                                    res = true,
                                    info = ret.Msg,
                                    url = "/OlineAlipay/Alipay?out_trade_no=" + model.OrderNum +
                                    "&subject=" + model.MaterialName +
                                    "&total_fee=" + model.TotalMoney +
                                    "&show_url=" + SiteUrl + "Wap_Consumers/Index"
                                };
                                break;
                            default:
                                js.Data = new { res = true, info = ret.Msg, url = "/Wap_Consumers/Index" };
                                break;
                        }
                    }
                    else
                    {
                        js.Data = new { info = ret.Msg };
                    }
                }
                else
                {
                    return Content("<script>alert('请登录后访问页面！');window.location.href = '/wap_order/login?pageType=2';</script>");
                }
            }
            catch
            {
                js.Data = new { info = "暂未开放该功能！" };
            }
            return js;
        }
        [HttpPost]
        public ActionResult AddComplaint(string content, string linkMan, string linkPhone)
        {
            JsonResult js = new JsonResult();
            CodeInfo code = GetSession(null, 0);
            if (code != null)
            {
                Complaint model = new Complaint();
                model.adddate = DateTime.Now;
                model.ComplaintContent = Request["content"];
                model.ComplaintDate = DateTime.Now;
                model.ComplaintType_ID = 1;
                model.Enterprise_Info_ID = code.EnterpriseID;
                model.lastdate = DateTime.Now;
                model.LInkMan = Request["linkman"];
                model.LinkPhone = Request["linkphone"];
                model.Material_ID = code.MaterialID;
                model.Status = 0;
                RetResult ret = new BLL.ScanCodeBLL().AddComplaint(model);

                if (ret.IsSuccess)
                {
                    js.Data = new { info = ret.Msg };
                }
                else
                {
                    js.Data = new { info = ret.Msg };
                }
            }
            else
            {
                js.Data = new { info = "请重新拍码访问页面！" };
            }
            return js;
        }

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
                ScanCodeBLL bll = new ScanCodeBLL();
                code = bll.GetCode(ewm, type);
            }
            else
            {
                code = Session["code"] as CodeInfo;
            }
            return code;
        }

        public ActionResult ComplaintFangChuan(string PRRU_PlatForm_ID, string MaterialID, string MaterialFullName, string Enterprise_Info_ID, string ewm)
        {
            PRRU_PlatForm DataModel = new BLL.ComplaintBLL().GetPlatForm(Convert.ToInt64(PRRU_PlatForm_ID));
            ViewBag.Tel = DataModel.ComplaintPhone;
            ViewBag.MaterialID = MaterialID;
            ViewBag.MaterialFullName = MaterialFullName;
            ViewBag.Enterprise_Info_ID = Enterprise_Info_ID;
            ViewBag.ewm = ewm;
            CodeInfo CodeModel = GetSession(ewm, 0);
            List<View_ProductInfoForMaterial> PropertyList =
                        new BLL.ProductInfoBLL().GetMaterialProperty(Convert.ToInt64(CodeModel.EnterpriseID),
                        CodeModel.MaterialID);
            ViewBag.PropertyList = PropertyList;
            return View();
        }

        [HttpPost]
        public ActionResult AddComplaintFangChuan(Complaint ObjComplaint)
        {
            ObjComplaint.adddate = DateTime.Now;
            ObjComplaint.Status = (int)Common.EnumFile.PlatFormState.no_examine;
            ObjComplaint.Enterprise_Info_ID = Convert.ToInt64(Request["Enterprise_Info_ID"]);
            ObjComplaint.ComplaintType_ID = int.Parse(Request["ccctype"]);
            ObjComplaint.ComplaintDate = DateTime.Now;
            ObjComplaint.Material_ID = Convert.ToInt64(Request["MetailId"]);
            JsonResult js = new JsonResult();
            RetResult ret = new BLL.ComplaintBLL().AddComplaint(ObjComplaint);
            if (ret.IsSuccess)
            {
                string ewm = Request["ewm"].ToString();
                js.Data = new { res = true, info = ret.Msg, url = "/wap_index/index?ewm=" + ewm };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 查询更多产品
        /// </summary>
        /// <returns></returns>
        public ActionResult OtherProduct()
        {
            if (Session["ewm"] == null)
            {
                return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
            }
            string ewm = Session["ewm"].ToString();
            if (ewm != null)
            {
                CodeInfo model = GetSession(ewm, 0);
                // 获取导航列表
                List<View_NavigationForMaterial> NavigationForMaterialList = new PageNavigationBLL().GetNavigationForMaterialList(Convert.ToInt64(model.EnterpriseID), model.MaterialID);
                AddDefaultNavigation(NavigationForMaterialList);
                ViewBag.ewm = ewm;
                ViewBag.NavigationForMaterialList = NavigationForMaterialList;
                if (model != null)
                {
                    List<View_Material> dataList = new BLL.MaterialBLL().GetList(Convert.ToInt64(model.EnterpriseID), "", 0).ObjList as List<View_Material>;
                    ViewBag.DataList = dataList;
                    return View();
                }
                else
                {
                    return Content("<script>alert('二维码错误');history.go(-1)</script>");
                }
            }
            else
            {
                return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
            }
        }

        public ActionResult OtherProductInfo()
        {
            string MaterialId = Request["MaterialId"];
            CodeInfo model = GetSession(null, 0);

            if (model == null)
            {
                return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
            }

            // 获取导航列表
            PageNavigationBLL bll = new PageNavigationBLL();
            List<View_NavigationForMaterial> NavigationForMaterialList = bll.GetNavigationForMaterialList(Convert.ToInt64(model.EnterpriseID), model.MaterialID);
            AddDefaultNavigation(NavigationForMaterialList);
            if (NavigationForMaterialList == null)
            {
                return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
            }

            ViewBag.NavigationForMaterialList = NavigationForMaterialList;

            List<View_MaterialSpecForMarket> MaterialSpecList =
                       new BLL.Material_OnlineOrderBLL().GetMarketMaterialSpecList(Convert.ToInt64(MaterialId));
            ViewBag.MaterialSpecList = MaterialSpecList;

            if (!string.IsNullOrEmpty(MaterialId))
            {
                LinqModel.Material DataModel = new BLL.MaterialBLL().GetMaterial(Convert.ToInt64(MaterialId)).ObjModel as Material;

                return View(DataModel);
            }
            else
            {
                return Content("<script>alert('数据错误，请刷新重试！');history.go(-1)</script>");
            }
        }

        public ActionResult Validate(string VerificationCode = "", bool IsScan = false)
        {
            if (Session["ewm"] == null)
            {
                if (IsScan == true)
                {
                    return Content("<script>alert('请重新拍码访问此页面');</script>");
                }
                else
                {
                    return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
                }
            }
            if (string.IsNullOrEmpty(VerificationCode))
            {
                if (IsScan == true)
                {
                    return Content("<script>alert('请重新拍码访问此页面');</script>");
                }
                else
                {
                    return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
                }
            }
            string ewm = Session["ewm"].ToString();
            ViewBag.ewm = ewm;
            CodeInfo fwCode = new ScanCodeBLL().GetCode(ewm, 0);
            if (fwCode == null)
            {
                if (IsScan == true)
                {
                    return Content("<script>alert('二维码错误，不是该平台的二维码！');</script>");
                }
                else
                {
                    return Content("<script>alert('二维码错误，不是该平台的二维码！');history.go(-1)</script>");
                }
            }

            View_Enterprise_FWCode_00 DataModel = new ScanCodeBLL().GetSaleCodeInfo(ewm);

            if (DataModel == null)
            {
                if (IsScan == true)
                {
                    return Content("<script>alert('该二维码还没被销售，不能追溯！');</script>");
                }
                else
                {
                    return Content("<script>alert('该二维码还没被销售，不能追溯！');history.go(-1)</script>");
                }
            }

            //ViewBag.ProductionTime = DataModel.ProductionTime.Value.ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(DataModel.ProductionTime.ToString()))
            {
                ViewBag.ProductionTime = DataModel.ProductionTime.Value.ToString("yyyy-MM-dd");
            }
            else if (!string.IsNullOrEmpty(DataModel.SalesTime.ToString()))
            {
                ViewBag.ProductionTime = DataModel.SalesTime.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                ViewBag.ProductionTime = DateTime.Now.ToString("yyyy-MM-dd");
            }
            bool Verification = false;
            if (DataModel.FWCode.Equals(VerificationCode.Trim()))
            {
                Verification = true;
                RetResult ret = new ScanCodeBLL().UpdateCount(ewm, false, true, DataModel.FWCount);

                if (DataModel.FWCount == null || DataModel.FWCount == 0)
                {
                    ViewBag.FWCount = 1;
                }
                else
                {
                    ViewBag.FWCount = DataModel.FWCount + 1;
                    ViewBag.ValidateTime = DataModel.ValidateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }


            // 获取企业信息
            Enterprise_Info EnterpriseModel = new BLL.EnterpriseInfoBLL().GetModel(Convert.ToInt64(DataModel.Enterprise_Info_ID));
            ViewBag.EnterpriseModel = EnterpriseModel;
            // 获取产品信息
            Material MaterialModel = new BLL.MaterialBLL().GetMaterial(DataModel.Material_ID.Value).ObjModel as Material;
            // 获取该产品的特殊活动信息
            List<View_ProductInfoForMaterial> PropertyList =
                        new BLL.ProductInfoBLL().GetMaterialProperty(Convert.ToInt64(DataModel.Enterprise_Info_ID),
                        DataModel.Material_ID.Value);
            ViewBag.PropertyList = PropertyList;

            // 获取导航列表
            List<View_NavigationForMaterial> NavigationForMaterialList = new PageNavigationBLL().GetNavigationForMaterialList(Convert.ToInt64(DataModel.Enterprise_Info_ID), DataModel.Material_ID.Value);
            AddDefaultNavigation(NavigationForMaterialList);
            ViewBag.NavigationForMaterialList = NavigationForMaterialList;
            ViewBag.Verification = Verification;
            return View(MaterialModel);
        }

        public ActionResult GetPrice(string MaterialSpecId)
        {
            JsonResult js = new JsonResult();

            View_ProductInfoForMaterial DataModel = new BLL.ProductInfoBLL().GetMaterialSpecPrice(Convert.ToInt64(MaterialSpecId));

            js.Data = new
            {
                Price = DataModel == null || DataModel.Price == null ? "0.00" : DataModel.Price.Value.ToString("0.00"),
                ExpressPrice = DataModel == null || DataModel.ExpressPrice == null ? "0.00" : DataModel.ExpressPrice.Value.ToString("0.00")
            };
            return js;
        }

        public ActionResult EnterpriseInfo(string EnterpriseId)
        {
            Enterprise_Info ObjEnterprise_Info = new Enterprise_Info();
            ViewBag.Logo = null;
            try
            {
                ObjEnterprise_Info = new BLL.EnterpriseInfoBLL().GetModel(Convert.ToInt64(EnterpriseId));
                if (ObjEnterprise_Info.imgs.Count != 0)
                {
                    ViewBag.Logo = ObjEnterprise_Info.imgs[0].fileUrls;
                }
                LinqModel.ShowCompany ShowCompanyModel = new BLL.ShowCompanyBLL().GetModel(Convert.ToInt64(EnterpriseId)).ObjModel as LinqModel.ShowCompany;
                ViewBag.ShowCompanyModel = ShowCompanyModel;
            }
            catch (Exception ex)
            { }

            return View(ObjEnterprise_Info);
        }

        public ActionResult ViewPage1()
        {
            return View();
        }

        /// <summary>
        /// 防伪码拍码页面
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public ActionResult SecurityCode(string ewm)
        {
            CodeInfo code = GetSession(ewm, 0);
            if (code != null)
            {
                ScanCodeBLL scan = new ScanCodeBLL();
                Material material = scan.GetMaterial(code.MaterialID);
                Enterprise_Info enterprise = scan.GetEnterprise(code.EnterpriseID);
                ViewBag.material = material;
                ViewBag.materialName = material.MaterialName;
                ViewBag.memo = material.Memo;
                ViewBag.enterprise = enterprise;
                //ViewBag.code = code;
                View_Enterprise_FWCode_00 DataModel = new ScanCodeBLL().GetSaleCodeInfo(ewm);
                //ViewBag.ProductionTime = DataModel.ProductionTime.Value.ToString("yyyy-MM-dd");
                if (!string.IsNullOrEmpty(DataModel.ProductionTime.ToString()))
                {
                    ViewBag.ProductionTime = DataModel.ProductionTime.Value.ToString("yyyy-MM-dd");
                }
                else if (!string.IsNullOrEmpty(DataModel.SalesTime.ToString()))
                {
                    ViewBag.ProductionTime = DataModel.SalesTime.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    ViewBag.ProductionTime = DateTime.Now.ToString("yyyy-MM-dd");
                }
                RetResult ret = new ScanCodeBLL().UpdateCount(ewm, false, true, DataModel.FWCount);
                if (DataModel.FWCount == null || DataModel.FWCount == 0)
                {
                    ViewBag.FWCount = 1;
                }
                else
                {
                    ViewBag.FWCount = DataModel.FWCount + 1;
                    ViewBag.ValidateTime = DataModel.ValidateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return View();
            }
            else
            {
                return Content("<script>alert('请拍码访问页面！')</script>");
            }
        }
    }
}
