using System;
using System.Linq;
using System.Web.Mvc;
using Common.Argument;
using BLL;
using LinqModel;
using Common;
using System.Configuration;

namespace iagric_plant.Controllers
{
    public class PController : Controller
    {
        //
        // GET: /P/

        readonly ScanCodeBLL _bll = new ScanCodeBLL();
        /// <summary>
        /// 拍码追溯页面
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeIndex(string e)
        {
            RedirectToRouteResult rtr = null;
            try
            {
                string ewm = Request["e"];
                string baseCode = Request["BaseCode"];
                string myCode = Request["code"];
                if (!string.IsNullOrEmpty(baseCode) && !string.IsNullOrEmpty(myCode))
                {
                    ewm = baseCode + myCode;
                }
                string uppage = Request["uppage"] ?? "";
                // 加密二维码转换成明文二维码
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
                string[] arrCode = ewm.Split('.');
                if (arrCode.Length == 6 && (arrCode[4] == ((int)EnumFile.TerraceEwm.slotting).ToString() ||
                   arrCode[4] == ((int)EnumFile.TerraceEwm.cribCode).ToString() || arrCode[4] == ((int)EnumFile.TerraceEwm.greenHouse).ToString()))
                {
                    return Content("<script>alert('请使用对应的客户端扫描该二维码！')</script>");
                }
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
                    ToJsonProperty backModel = list.propertys.FirstOrDefault(p => p.pName == codeInfo.FwCode.EWM);
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
            if (codeInfo.EnterpriseID>0)
            {
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
                else if (myEnterprise.IsActive >0)
                {
                    result.SetArgument(CmdResultError.PARAMERROR, "", "未激活");
                    return result;
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
            }
            else
            {
                result.SetArgument(CmdResultError.PARAMERROR, "", "");
            }
            return result;
        }
    }
}
