using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LinqModel;
using BLL;
using Common.Argument;

namespace iagric_plant.Controllers
{
    public class Wap_IndexTwoController : Controller
    {
        //
        // GET: /Wap_IndexTwo/
        ScanCodeBLL _Bll = new ScanCodeBLL();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexTwo(string ewm)
        {
            RedirectToRouteResult rtr = null;
            long settingId = 0;
            try
            {
                CodeInfo codeInfo = new CodeInfo();
                //0:取session二维码1:拍码二维码
                int type = 1;
                string uppage = Request["uppage"] ?? "";
                //根据二维码查询二维码数据
                codeInfo = GetSession(ewm, 0);
                if (codeInfo == null || (codeInfo.CodeType == (int)Common.EnumFile.AnalysisBased.Seting && codeInfo.FwCode.Enterprise_FWCode_ID == 0))
                {
                    return Content("<script>alert('二维码错误，不是该平台的二维码！')</script>");
                }
                ViewBag.MaterialId = codeInfo.MaterialID;
                ViewBag.EnterpriseId = codeInfo.EnterpriseID;
                settingId = codeInfo.CodeSeting.ID;
                // 获取导航列表
                List<View_NavigationForMaterial> NavigationForMaterialList = new List<View_NavigationForMaterial>();
                AddDefaultNavigation(NavigationForMaterialList);
                ViewBag.NavigationForMaterialList = NavigationForMaterialList;
                ViewBag.ewm = ewm;
                ViewBag.uppage = uppage;
                ViewBag.VideoUrl = _Bll.GetMaterialShopLike(codeInfo.MaterialID);
                #region 红包相关内容
                var resultModel = new ScanCodeMarketBLL().CanGetRedPacket(codeInfo.CodeSeting.ID, codeInfo.FwCode, 0);
                if (resultModel == null)
                {
                    ViewBag.IsDiplayPacket = false;
                }
                else
                {
                    ViewBag.IsDiplayPacket = resultModel.Code == 2 ? true : false;
                    ViewBag.PacketUrl = "/Market/Wap_IndexMarket/Index?settingId=" + codeInfo.CodeSeting.ID.ToString() + "&ewm=" + ViewBag.ewm;
                }

                #endregion
            }
            catch
            {
                rtr = RedirectToAction("NoSales", "Wap_Index");
                return rtr;
            }

            return View();
        }

        /// <summary>
        /// 获取产品信息170209改动
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="CodeValidation">验证</param>
        /// <param name="OnlyView">是否演示码</param>
        /// <returns></returns>
        public ActionResult MaterialInfo(string ewm, string CodeValidation = "true")
        {
            //根据二维码查询二维码数据、优惠券相关
            var codeInfo = GetSession(null, 0);
            RetResult resultCoupon = new CouponBLL().CouponCanGet(ewm, codeInfo.CodeSeting.ID, 0, 0);
            if (resultCoupon.Code == 5 || (resultCoupon.Code == 7 && codeInfo.FwCode.ActivityCouponID > 0))
            {
                var model = new ScanCodeMarketBLL().GetModel(codeInfo.CodeSeting.ID, 0);
                ViewBag.TitleCoupon = model.ActivityTitle;
                ViewBag.SettingId = codeInfo.CodeSeting.ID;
                ViewBag.Ewm = ewm;
            }
            else
            {
                ViewBag.TitleCoupon = "";
            }
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers != null)
            {
                ViewBag.Consumers = true;
            }
            else
            {
                ViewBag.Consumers = false;
            }
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
                if (codeInfo != null)
                {
                    material = _Bll.GetMaterial(codeInfo.MaterialID);
                    //显示品牌再查询数据
                    if (codeInfo.Display.Brand == false)
                    {
                        ViewBag.BoolBrand = false;
                    }
                    else
                    {
                        brand = _Bll.GetBrand(codeInfo.BrandID);
                        areaBrand = _Bll.GetAreaBrand(codeInfo.MaterialID);
                    }
                    if (codeInfo.Display.CreateDate == false)
                    {
                        ViewBag.BoolCreateDate = false;
                    }
                    if (codeInfo.Display.ScanCount == false)
                    {
                        //是否显示生产日期
                        ViewBag.BoolScanCount = false;
                    }
                    if (codeInfo.Display.Verification == false)
                    {
                        CodeValidation = "false";
                    }
                    else
                    {
                        CodeValidation = "true";
                    }
                    enterprise = _Bll.GetEnterprise(Convert.ToInt64(codeInfo.EnterpriseID));
                    List<View_ProductInfoForMaterial> PropertyList =
                        new BLL.ProductInfoBLL().GetMaterialProperty(Convert.ToInt64(codeInfo.EnterpriseID),
                        codeInfo.MaterialID);
                    List<View_MaterialSpecForMarket> MaterialSpecList =
                       new BLL.Material_OnlineOrderBLL().GetMarketMaterialSpecList(material.Material_ID);
                    ViewBag.MaterialSpecList = MaterialSpecList;
                    ViewBag.ScanCount = codeInfo.FwCode.ScanCount == null ? 0 : codeInfo.FwCode.ScanCount;
                    ViewBag.FWCount = codeInfo.FwCode.FWCount;
                    ViewBag.seleTime = codeInfo.FwCode.SalesTime;
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
            //ViewBag.OnlyView = Convert.ToBoolean(OnlyView);
            // 图形防伪标识
            ViewBag.CodeValidation = Convert.ToBoolean(CodeValidation);
            //20200207加
            if (!string.IsNullOrEmpty(codeInfo.CodeRequest.startdate))
            {
                string a1 = "20" + codeInfo.CodeRequest.startdate;
                string a2 = a1.Substring(0, 4) + "-" + a1.Substring(4, 2) + "-" + a1.Substring(6, 2);
                ViewBag.ShengChanDate = a2;//生产日期
            }
            else
            {
                ViewBag.ShengChanDate = null;
            }
            ViewBag.ShengChanPH = codeInfo.CodeRequest.ShengChanPH;//生产批号
            if (!string.IsNullOrEmpty(codeInfo.CodeRequest.YouXiaoDate))
            {
                string a1 = "20" + codeInfo.CodeRequest.YouXiaoDate;
                string a2 = a1.Substring(0, 4) + "-" + a1.Substring(4, 2) + "-" + a1.Substring(6, 2);
                ViewBag.YouXiaoDate = a2;//有效日期
            }
            else
            {
                ViewBag.YouXiaoDate = null;
            }
            if (!string.IsNullOrEmpty(codeInfo.CodeRequest.ShiXiaoDate))
            {
                string a1 = "20" + codeInfo.CodeRequest.YouXiaoDate;
                string a2 = a1.Substring(0, 4) + "-" + a1.Substring(4, 2) + "-" + a1.Substring(6, 2);
                ViewBag.ShiXiaoDate = a2;//失效日期
            }
            else
            {
                ViewBag.ShiXiaoDate = null;
            }
            ViewBag.MieJunNo = codeInfo.CodeRequest.dbatchnumber;//灭菌批号
            ViewBag.XuLieHao = codeInfo.CodeRequest.serialnumber;//序列号
            return View();
        }

        /// <summary>
        /// 获取session中的二维码数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public CodeInfo GetSession(string ewm, int type)
        {
            CodeInfo code = null;
            if (!string.IsNullOrEmpty(ewm) && Session["code"] == null)
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

        /// <summary>
        /// 获取溯源信息
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public ActionResult Information(string ewm)
        {
            ViewBag.ewm = ewm;
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
                            ViewBag.BoolOrigin = true;
                            origin = scan.GetYuanliao(code.CodeSeting.ID);
                        }
                        if (code.Display.Work == true)
                        {
                            ViewBag.BoolWork = true;
                            shengChan = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Produce, code.CodeSeting.ID);
                            jiaGong = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Processing, code.CodeSeting.ID);
                            feed = scan.GetProduce(0, 0, code.CodeType, (int)Common.EnumFile.ZuoYeType.Feed, code.CodeSeting.ID);
                            banzu = _Bll.GetSubstation(code.CodeSeting.ID);
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
                            cunchu = _Bll.GetWareHouse(code.CodeSeting.ID);
                        }
                        if (code.Display.WuLiu == true)
                        {
                            ViewBag.BoolWuLiu = true;
                            wuliu = _Bll.GetLogistics(code.CodeSeting.ID);
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
                            banzu = _Bll.GetSubstation(code.CodeSeting.ID);
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
                            cunchu = _Bll.GetWareHouse(code.CodeSeting.ID);
                        }
                        if (code.Display.WuLiu == true)
                        {
                            ViewBag.BoolWuLiu = true;
                            wuliu = _Bll.GetLogistics(code.CodeSeting.ID);
                        }
                    }
                    ////陈志钢 2022年4月22日  解决模板2不显示 内容
                    else if (code.CodeSeting.ID > 0)
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
                            banzu = _Bll.GetSubstation(code.CodeSeting.ID);
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
                            cunchu = _Bll.GetWareHouse(code.CodeSeting.ID);
                        }
                        if (code.Display.WuLiu == true)
                        {
                            ViewBag.BoolWuLiu = true;
                            wuliu = _Bll.GetLogistics(code.CodeSeting.ID);
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
        /// 获取详细溯源信息
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public ActionResult InformationSub(long typeId, long dataId, string ewm)
        {
            ScanInfo result = new ScanInfo();
            ViewBag.ewm = ewm;
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
                CodeInfo code = GetSession(null, 0);
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
                CodeInfo model = GetSession(null, 0);
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

        public ActionResult OtherProductInfo(string ewm)
        {
            ViewBag.ewm = ewm;
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

        public ActionResult MaterialOrder(string ewm, string uppage = "1")
        {
            ViewBag.uppage = uppage;
            CodeInfo code = GetSession(null, 0);
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
            ViewBag.ProductionTime = DateTime.Now.ToString("yyyy-MM-dd");
            bool Verification = false;
            if (fwCode.FwCode.FWCode.Equals(VerificationCode.Trim()))
            {
                Verification = true;
                RetResult ret = new ScanCodeBLL().UpdateCount(ewm, false, true, fwCode.FwCode.FWCount);

                if (fwCode.FwCode.FWCount == null || fwCode.FwCode.FWCount == 0)
                {
                    ViewBag.FWCount = 1;
                }
                else
                {
                    ViewBag.FWCount = fwCode.FwCode.FWCount + 1;
                    ViewBag.ValidateTime = fwCode.FwCode.ValidateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }


            // 获取企业信息
            Enterprise_Info EnterpriseModel = new BLL.EnterpriseInfoBLL().GetModel(Convert.ToInt64(fwCode.EnterpriseID));
            ViewBag.EnterpriseModel = EnterpriseModel;
            // 获取产品信息
            Material MaterialModel = new BLL.MaterialBLL().GetMaterial(fwCode.MaterialID).ObjModel as Material;
            // 获取该产品的特殊活动信息
            List<View_ProductInfoForMaterial> PropertyList =
                        new BLL.ProductInfoBLL().GetMaterialProperty(Convert.ToInt64(fwCode.EnterpriseID),
                        fwCode.MaterialID);
            ViewBag.PropertyList = PropertyList;

            // 获取导航列表
            List<View_NavigationForMaterial> NavigationForMaterialList = new PageNavigationBLL().GetNavigationForMaterialList(Convert.ToInt64(fwCode.EnterpriseID), fwCode.MaterialID);
            AddDefaultNavigation(NavigationForMaterialList);
            ViewBag.NavigationForMaterialList = NavigationForMaterialList;
            ViewBag.Verification = Verification;
            return View(MaterialModel);
        }
    }
}
