/********************************************************************************
** 作者： 赵慧敏
** 创始时间：2017-04-21
***联系方式 :13313318725
** 描述：拍码追溯页面控制器
***版本：v2.0
***版权：农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common.Argument;
using BLL;
using LinqModel;
using Common;
using System.Configuration;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 拍码追溯页面控制器
    /// </summary>
    public class Wap_IndexController : Controller
    {
        private readonly ScanCodeBLL _bll = new ScanCodeBLL();
        /// <summary>
        /// 拍码追溯页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            RedirectToRouteResult rtr = null;
            try
            {
                string ewm = DTRequest.GetString("ewm");
                string baseCode = DTRequest.GetString("BaseCode");
                string myCode = DTRequest.GetString("code");
                if (!string.IsNullOrWhiteSpace(baseCode) && !string.IsNullOrWhiteSpace(myCode))
                {
                    ewm = baseCode + myCode;
                }
                string uppage = DTRequest.GetString("uppage");
                // 加密二维码转换成明文二维码
                //根据二维码查询二维码信息，赋予 CodeInfo
                CodeInfo codeInfo = new CodeInfo();
                //0:取session二维码1:拍码二维码
                int type = 0;
                // 二维码为空，session二维码为空
                if (string.IsNullOrWhiteSpace(ewm) && Session["ewm"] == null)
                {
                    return Content("<script>alert('请拍码访问页面！')</script>");
                }

                // 二维码为空，session二维码不为空 取session二维码;二维码不为空，并且与session二维码相同 不需要增加查询次数
                if (string.IsNullOrEmpty(ewm) && Session["ewm"] != null
                    || !string.IsNullOrEmpty(ewm) && Session["ewm"] != null && Session["ewm"].ToString().Equals(ewm))
                {
                    ewm = Session["ewm"].ToString();
                }

                // 二维码不为空，并且session二维码不相同或者session二维码为空 正常拍码操作
                else if (!string.IsNullOrEmpty(ewm) && (Session["ewm"] != null
                                                        && !Session["ewm"].ToString().Equals(ewm) || Session["ewm"] == null))
                {
                    type = 1;
                }

                #region 判断二维码类型
                string[] arrCode = ewm.Split('.');
                if (arrCode.Length < 6&&!ewm.Contains("L"))
                {
                    return Content("<script>alert('该二维码不正确！')</script>");
                }

                //if (arrCode.Length == 6 && (arrCode[4] == ((int)EnumFile.TerraceEwm.slotting).ToString() ||
                //   arrCode[4] == ((int)EnumFile.TerraceEwm.cribCode).ToString() || arrCode[4] == ((int)EnumFile.TerraceEwm.greenHouse).ToString()))
                //{
                //    return Content("<script>alert('请使用对应的客户端扫描该二维码！')</script>");
                //}
                //根据二维码查询二维码数据
                codeInfo = _bll.GetCode(ewm, type);
                if (codeInfo == null || (codeInfo.CodeType == (int)EnumFile.AnalysisBased.Seting && codeInfo.FwCode.Enterprise_FWCode_ID == 0))
                {
                    return Content("<script>alert('二维码格式不正确！');</script>");
                }
                if (codeInfo.CodeType != (int)EnumFile.AnalysisBased.Seting)
                {
                    type = 0;
                    ViewBag.OnlyView = true;
                }
                else
                {
                    ViewBag.OnlyView = false;
                }
                //二维码内容
                Session["ewm"] = ewm;
                //二维码数据
                Session["code"] = codeInfo;
                //判断产品是否被停止解析
                RetResult result = GetAnalysis(codeInfo);
                if (result != null && result.CmdError == CmdResultError.PARAMERROR)
                {
                    rtr = RedirectToAction("NoSales", "Wap_Index");
                    return rtr;
                }
                //判断是否是黑名单二维码
                BaseResultModel backList = new BaseResultModel();
                backList = new BackListBLL().GetModel(codeInfo.EnterpriseID);
                if (backList != null && backList.ObjModel != null)
                {
                    BackList list = backList.ObjModel as BackList;
                    ToJsonProperty backModel = list.propertys.Where(p => p.pName == codeInfo.FwCode.EWM).FirstOrDefault();
                    if (backModel != null)
                    {
                        rtr = RedirectToAction("SecurityFalse", "Wap_Index", new { imageUrl = list.BackImgs[0].fileUrl });
                        if (rtr != null) return rtr;
                    }
                }
                if (codeInfo.RequestCodeType == (int)EnumFile.RequestCodeType.SecurityCode)
                {
                    rtr = RedirectToAction("SecurityCode", "Wap_Index", new { ewm = ewm });
                    if (rtr != null) return rtr;
                }
                ViewBag.StyleModel = codeInfo.CodeSeting.StyleModel;
                if (codeInfo.CodeSeting.StyleModel == (int)EnumFile.SettingSkin.mubaner)
                {
                    rtr = RedirectToAction("IndexTwo", "Wap_IndexTwo", new { ewm = ewm });
                    if (rtr != null) return rtr;
                }
                //else if (codeInfo.CodeSeting.StyleModel == (int)EnumFile.SettingSkin.Three)
                //{
                //    rtr = RedirectToAction("P", "Wap_Index3", new { ewm = ewm });
                //    if (rtr != null) return rtr;
                //}
                //else if (codeInfo.CodeSeting.StyleModel == (int)EnumFile.SettingSkin.Four)
                //{
                //    rtr = RedirectToAction("Index", "Wap_Index4", new { ewm = ewm });
                //    if (rtr != null) return rtr;
                //}
                //else if (codeInfo.CodeSeting.StyleModel == (int)EnumFile.SettingSkin.Five)
                //{
                //    rtr = RedirectToAction("Index", "Wap_Index5", new { ewm = ewm });
                //    if (rtr != null) return rtr;
                //}
                //else if (codeInfo.CodeSeting.StyleModel == (int)EnumFile.SettingSkin.FangWei)
                //{
                //    rtr = RedirectToAction("SecurityCode", "Wap_Index", new { ewm = ewm });
                //    if (rtr != null) return rtr;
                //}
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
                #endregion
                ViewBag.ewm = ewm;
                ViewBag.uppage = uppage;
                ViewBag.VideoUrl = _bll.GetMaterialShopLike(codeInfo.MaterialID);
                ViewBag.MaterialVideo = _bll.MaterialVideo(codeInfo.MaterialID);
                #region 红包相关内容和优惠券
                var resultModel = new ScanCodeMarketBLL().CanGetRedPacket(codeInfo.CodeSeting.ID, codeInfo.FwCode, 0);
                if (resultModel == null)
                {
                    ViewBag.IsDiplayPacket = false;
                }
                else
                {
                    if (resultModel.Code == 2 || (resultModel.Code == 3 && codeInfo.FwCode.ActivitySubID != null && codeInfo.FwCode.ActivitySubID > 0))
                    {
                        ViewBag.IsDiplayPacket = true;
                        ViewBag.PacketUrl = "/Market/Wap_IndexMarket/Index";
                    }
                }
                #endregion
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
            Enterprise_Info myEnterprise = new EnterpriseInfoBLL().GetModel(codeInfo.EnterpriseID);
            if (myEnterprise.Verify != 1)
            {
                int tryDays = Convert.ToInt32(ConfigurationManager.AppSettings["trydays"]);
                //-2审核不通过；-1未审核；0正常、审核通过
                if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.noVerify
                    || myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.Try)
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
            if (codeInfo.FwCode != null && myEnterprise.IsActive > 0)
            {
                if (codeInfo.FwCode.Status == (int)EnumFile.UsingStateCode.NotUsed)
                {
                    if (codeInfo.FwCode.Type == (int)EnumFile.CodeType.bSingle || codeInfo.FwCode.Type == (int)EnumFile.CodeType.pesticides
                || codeInfo.FwCode.Type == (int)EnumFile.CodeType.single)
                    {
                        result.SetArgument(CmdResultError.PARAMERROR, "", "未激活");
                        return result;
                    }
                }
            }
            Analysis model = new Analysis();
            SysAnalysisBLL bll = new SysAnalysisBLL();
            model = bll.GetAnalysis(codeInfo);
            if (model != null && model.IsAnalyse == Convert.ToInt64(EnumFile.AnalysisType.StopAnalysis))
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
        /// 评价
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialPj()
        {
            CodeInfo code = GetSession(null, 0);
            List<NewComplaint> result = new List<NewComplaint>();
            try
            {
                if (code.MaterialID > 0)
                {
                    result = _bll.GetEvaluation(code.MaterialID, code.EnterpriseID);
                    return View(result);
                }
            }
            catch
            { }
            return View(result);
        }

        /// <summary>
        /// 添加评价
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MaterialPj(string content)
        {
            CodeInfo code = GetSession(null, 0);
            if (code != null)
            {
                Complaint plaint = new Complaint
                {
                    ComplaintContent = content,
                    Enterprise_Info_ID = code.EnterpriseID,
                    Material_ID = code.MaterialID,
                    ComplaintDate = DateTime.Now,
                    ComplaintType_ID = (int)EnumFile.ComplaintType.Visitor,
                    Status = 0
                };
                return Json(new { ok = _bll.AddComplaint(plaint).IsSuccess });
            }
            return Json(new { ok = false });
        }

        /// <summary>
        /// 查找产品信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMaterial()
        {
            CodeInfo code = GetSession(null, 0);
            ScanMaterial result = new ScanMaterial();
            //优惠券相关
            RetResult resultCoupon = new CouponBLL().CouponCanGet(code.FwCode.EWM, code.CodeSeting.ID, 0, 0);
            if (resultCoupon.Code == 5 || (resultCoupon.Code == 7 && code.FwCode.ActivityCouponID > 0))
            {
                ViewBag.TitleCoupon = new ScanCodeMarketBLL().GetModel(code.CodeSeting.ID, 0).ActivityTitle;
            }
            else
            {
                ViewBag.TitleCoupon = "";
            }
            ViewBag.Ewm = code.FwCode.EWM;
            ViewBag.SettingId = code.CodeSeting.ID;
            ViewBag.IsShengChanDate = code.Display.CreateDate;
            try
            {
                if (code.MaterialID > 0)
                {
                    result = _bll.GetMaterialNew(code.MaterialID, code.FwCode.EWM);
                    result.ProductDate = code.ProductDate;
                    ViewBag.AllCode = code.FwCode.EWM;
                    result.ShengChanPH = code.CodeRequest.ShengChanPH;//生产批号
                    result.Speciton= code.CodeSeting.MaterialXH;//产品型号
                    ViewBag.Spection = code.CodeSeting.ID;
                    //result.YouXiaoDate = code.CodeRequest.YouXiaoDate;//有效日期
                    //result.ShiXiaoDate = code.CodeRequest.ShiXiaoDate;//失效日期
                    if (code.CodeRequest.startdate != null)
                    {
                        string a1 = "20" + code.CodeRequest.startdate;
                        string a2 = a1.Substring(0, 4) + "-" + a1.Substring(4, 2) + "-" + a1.Substring(6, 2);
                        result.ShengChanDate = a2;
                    }
                    result.MieJunNo = code.CodeRequest.dbatchnumber;//灭菌批号
                    result.XuLieNo = code.CodeRequest.serialnumber;//序列号
                    if (code.CodeRequest.YouXiaoDate != null)
                    {
                        string a1 = "20" + code.CodeRequest.YouXiaoDate;
                        string a2 = a1.Substring(0, 4) + "-" + a1.Substring(4, 2) + "-" + a1.Substring(6, 2);
                        //string a1="20" + day.Substring(1, 2) + "-" + day.Substring(3, 2) + "-" + day.Substring(5, 2);
                        result.YouXiaoDate = a2;
                    }
                    if (code.CodeRequest.ShiXiaoDate != null)
                    {
                        string a1 = "20" + code.CodeRequest.ShiXiaoDate;
                        string a2 = a1.Substring(0, 4) + "-" + a1.Substring(4, 2) + "-" + a1.Substring(6, 2);
                        result.ShiXiaoDate = a2;
                    }
                    return View(result);
                }
            }
            catch
            { }
            return View(result);
        }

        /// <summary>
        /// 查看更多视频
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialVideo()
        {
            CodeInfo code = GetSession(null, 0);
            MaterialShopLink result = new MaterialShopLink();
            try
            {
                if (code.MaterialID > 0)
                {
                    result = _bll.GetMaterialShopLike(code.MaterialID);
                    return View(result);
                }
            }
            catch
            { }
            return View(result);
        }

        /// <summary>
        /// 企业商城
        /// </summary>
        /// <returns></returns>
        public ActionResult Shop()
        {
            ShopInfo result = null;
            try
            {
                CodeInfo code = GetSession(null, 0);
                if (code != null)
                {
                    result = _bll.GetShop(code.EnterpriseID);
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 获取原材料信息
        /// </summary>
        /// <returns></returns>
        public ActionResult RawMaterial()
        {
            List<View_RequestOrigin> origin = new List<View_RequestOrigin>();
            try
            {
                CodeInfo code = GetSession(null, 0);
                ViewBag.BoolOrign = false;
                if (code.Display.Origin)
                {
                    ViewBag.BoolOrign = true;
                    origin = _bll.GetYuanliao(code.CodeSeting.ID);
                }
            }
            catch { }
            return View(origin);
        }

        /// <summary>
        /// 查看更多原材料质检报告
        /// </summary>
        /// <returns></returns>
        public ActionResult MoreRaw()
        {
            List<View_RequestOrigin> result = new List<View_RequestOrigin>();
            try
            {
                CodeInfo code = GetSession(null, 0);
                if (code.CodeSeting.ID > 0)
                {
                    result = _bll.GetYuanliao(code.CodeSeting.ID);
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 班组
        /// </summary>
        /// <returns></returns>
        public ActionResult Substation()
        {
            ScanSubstation result = null;
            try
            {
                CodeInfo code = GetSession(null, 0);
                ViewBag.Work = code.Display.Work;
                if (code.CodeSeting.ID > 0 && code.Display.Work)
                {
                    result = _bll.GetSubstation(code.CodeSeting.ID);
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 更多班组信息
        /// </summary>
        /// <returns></returns>
        public ActionResult MoreSubstation(long batchZyId)
        {
            View_BatchZuoye result = new View_BatchZuoye();
            try
            {
                result = _bll.GetBatchZuoye(batchZyId);
                if (result != null)
                {
                    CodeInfo code = GetSession(null, 0);
                    if (code != null)
                    {
                        ViewBag.TeamLst = new TeamUsersBLL().GetList(result.TeamID.Value, code.EnterpriseID);
                    }
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 产品描述
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialMemo()
        {
            ScanMaterialMemo result = null;
            try
            {
                CodeInfo code = GetSession(null, 0);
                if (code.MaterialID > 0)
                {
                    result = _bll.GetMaterialMemo(code.MaterialID);
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 存储环境
        /// </summary>
        /// <returns></returns>
        public ActionResult Warehouse()
        {
            ScanWareHouseInfo result = null;
            try
            {
                CodeInfo code = GetSession(null, 0);
                ViewBag.Ambient = code.Display.Ambient;
                if (code.CodeSeting.ID > 0 && code.Display.Ambient)
                {
                    result = _bll.GetWareHouse(code.CodeSeting.ID);
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 物流信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Logistics()
        {
            ScanLogistics result = null;
            try
            {
                CodeInfo code = GetSession(null, 0);
                ViewBag.Wuliu = code.Display.WuLiu;
                if (code.CodeSeting.ID > 0 && code.Display.WuLiu)
                {
                    result = _bll.GetLogistics(code.CodeSeting.ID);
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 企业信息
        /// </summary>
        /// <returns></returns>
        public ActionResult EnterpriseInfo()
        {
            CodeInfo code = GetSession(null, 0);
            if (code != null)
            {
                ScanEnterprise result = null;
                result = new ScanCodeBLL().GetEnterpriseInfo(code.EnterpriseID);
                ViewBag.EnterpriseID = code.EnterpriseID;
                return View(result);
            }
            else
            {
                return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
            }
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
                code = _bll.GetCode(ewm, type);
            }
            else
            {
                code = Session["code"] as CodeInfo;
            }
            return code;
        }
        /// <summary>
        /// 防伪码拍码页面
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public ActionResult SecurityCode(string ewm)
        {
            CodeInfo code = GetSession(ewm, 0);
            if (code == null || (code.CodeType == (int)EnumFile.AnalysisBased.Seting && code.FwCode.Enterprise_FWCode_ID == 0) || code.FwCode.EWM != ewm)
            {
                return Content("<script>alert('二维码错误，不是该平台的二维码！')</script>");
            }
            else
            {
                //香港红花黑名单码
                if (code.FwCode.EWM == "i.86.810000.1259/10.29063000.01/180507.030395.2"
                    || code.FwCode.EWM == "i.86.810000.1259/10.29063000.01/180507.219139.2" || code.FwCode.EWM == "00110020003069A688CBWEV")
                {
                    var rtr = RedirectToAction("SecurityFalse", "Wap_Index", new { ewm = ewm });
                    if (rtr != null) return rtr;
                }
                ScanCodeBLL scan = new ScanCodeBLL();
                Material material = scan.GetMaterial(code.MaterialID);
                Enterprise_Info enterprise = scan.GetEnterprise(code.EnterpriseID);
                ViewBag.material = material;
                ViewBag.materialName = material.MaterialName;
                ViewBag.memo = material.Memo;
                ViewBag.enterprise = enterprise;
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
                #region 红包相关内容和优惠券
                var resultModel = new ScanCodeMarketBLL().CanGetRedPacket(code.CodeSeting.ID, code.FwCode, 0);
                if (resultModel == null)
                {
                    ViewBag.IsDiplayPacket = false;
                }
                else
                {
                    if (resultModel.Code == 2 || (resultModel.Code == 3 && code.FwCode.ActivitySubID != null && code.FwCode.ActivitySubID > 0))
                    {
                        ViewBag.IsDiplayPacket = true;
                        ViewBag.PacketUrl = "/Market/Wap_IndexMarket/Index";
                    }
                }
                #endregion
                return View();
            }
        }
        /// <summary>
        /// 黑名单页面
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public ActionResult SecurityFalse(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ViewBag.imageUrl = imageUrl;
            }
            return View();
        }

        /// <summary>
        /// 未销售显示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult NoSales()
        {
            CodeInfo code = Session["code"] as CodeInfo;
            return View(code);
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
                ViewBag.ewm = ewm;
                if (model != null)
                {
                    List<View_Material> dataList = new MaterialBLL().GetList(Convert.ToInt64(model.EnterpriseID), "", 1).ObjList as List<View_Material>;
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

        /// <summary>
        /// 查询更多产品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OtherProduct(int pageIndex)
        {
            if (Session["ewm"] == null)
            {
                return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
            }
            string ewm = Session["ewm"].ToString();
            if (ewm != null)
            {
                CodeInfo model = GetSession(null, 0);
                ViewBag.ewm = ewm;
                if (model != null)
                {
                    List<View_Material> dataList = new MaterialBLL().GetList(Convert.ToInt64(model.EnterpriseID), "", pageIndex).ObjList as List<View_Material>;
                    return Json(dataList);
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
        /// <summary>
        /// 查询更多产品信息
        /// </summary>
        /// <returns></returns>
        public ActionResult OtherProductInfo()
        {
            string materialId = Request["MaterialId"];
            CodeInfo model = GetSession(null, 0);
            if (model == null)
            {
                return Content("<script>alert('请重新拍码访问此页面');history.go(-1)</script>");
            }
            if (!string.IsNullOrEmpty(materialId))
            {
                ScanMaterial result = _bll.GetMaterialNew(Convert.ToInt32(materialId), model.FwCode.EWM);
                return View(result);
            }
            else
            {
                return Content("<script>alert('数据错误，请刷新重试！');history.go(-1)</script>");
            }
        }

        /// <summary>
        /// 检测报告
        /// </summary>
        /// <returns></returns>
        public ActionResult Check()
        {
            ScanInfo result = null;
            try
            {
                CodeInfo code = GetSession(null, 0);
                ViewBag.Wuliu = code.Display.Report;
                if (code.CodeSeting.ID > 0 && code.Display.Report)
                {
                    result = _bll.GetJianYanJian(code.CodeSeting.ID);
                }
            }
            catch { }
            return View(result);
        }

        /// <summary>
        /// 实时视频
        /// </summary>
        /// <returns></returns>
        public ActionResult VideoList()
        {
            ScanInfo result = null;
            try
            {
                CodeInfo code = GetSession(null, 0);
                result = _bll.GetMaterialVideo(code.MaterialID);
            }
            catch { }
            return View(result);
        }
        /// <summary>
        /// 实时视频
        /// </summary>
        /// <returns></returns>
        public ActionResult Video(string url, string name)
        {
            ViewBag.url = url;
            ViewBag.name = name;
            return View();
        }
    }
}
