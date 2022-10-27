/********************************************************************************
** 作者：赵慧敏
** 开发时间：2017-6-7
** 联系方式:13313318725
** 代码功能：红包活动
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LinqModel;
using Common.Tools;
using BLL;
using Common.Argument;
using MarketActive.Filter;
using MarketActive.BLL;
using iagric_plant.Areas.Market.Models;
using System.IO;
using System.Text;
using System.Drawing;
using Common;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 红包控制器
    /// </summary>
    [AdminAuthorize]
    public class PacketController : Controller
    {
        /// <summary>
        /// 红包管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加红包
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Add(long activityId = 0)
        {
            //查找企业是否设置了微信红包账户
            WxEnAccountBLL wxbll = new WxEnAccountBLL();
            YX_WxEnAccount wxzh = wxbll.GetModel(SessCokie.Get.EnterpriseID);
            if (wxzh == null)
            {
                ViewBag.Flag = 0;
            }
            else
            {
                ViewBag.Flag = 1;
            }
            //获取配置id
            if (!string.IsNullOrEmpty(Request["mainCode"]) && !string.IsNullOrEmpty(Request["settingId"])
                && !string.IsNullOrEmpty(Request["trace"]) && !string.IsNullOrEmpty(Request["enterpriseID"]))
            {
                string mainCode = Request["mainCode"];
                string settingId = Request["settingId"];
                string trace = Request["trace"];
                string enterpriseID = Request["enterpriseID"];
                UserInfo userInfo = new UserInfo();
                userInfo.MainCode = mainCode;
                userInfo.EnterpriseID = Convert.ToInt64(enterpriseID);
                userInfo.SetingID = Convert.ToInt64(settingId);
                //追溯管理跳转
                userInfo.LoginType = (int)Common.EnumText.LoginType.parameter;
                CurrentUser.User = userInfo;
                ViewBag.LoginType = (int)Common.EnumText.LoginType.parameter;
                ViewBag.SetingID = settingId;
            }
            else
            {
                CurrentUser.User = new UserInfo();
                CurrentUser.User.LoginType = (int)Common.EnumText.LoginType.common;
                ViewBag.LoginType = (int)Common.EnumText.LoginType.common;
            }
            //if (SessCokie.Get == null)
            //{
            //    return RedirectToAction("Exit", "Home");
            //}
            ViewBag.ActivityID = activityId;
            return View();
        }

        /// <summary>
        /// 设置红包
        /// </summary>
        /// <returns></returns>
        public ActionResult SetPacket()
        {
            return View();
        }

        /// <summary>
        /// 新增红包活动
        /// </summary>
        /// <param name="SendCompany">商户名称</param>
        /// <param name="BlessingWords">祝福语</param>
        /// <param name="CompanyLogoURL">企业logo</param>
        /// <param name="ActivityStyleID">模板</param>
        /// <param name="ActivityTitle">活动名称</param>
        /// <param name="AtivityImageURL">活动图片</param>
        /// <param name="Content">规则</param>
        /// <param name="ActivityType">活动类型</param>
        /// <param name="StartDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="JoinMode">参与方式</param>
        /// <param name="ShareFriends">是否分享到朋友圈</param>
        /// <param name="setDetail">红包设置</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddActivity(string SendCompany, string BlessingWords, string CompanyLogoURL, string ActivityStyleID, string ActivityTitle,
            string AtivityImageURL, string Content, string ActivityType, DateTime? StartDate, DateTime? EndDate, string JoinMode,
            string ShareFriends, string setDetail, double? sumM, int payMethod, double sxf, string RedStyle, double total, int IsYZCode, string SetingID)
        {
            RetResult result = new RetResult();
            long activityID = 0;
            string orderNum = string.Empty;
            string name = string.Empty;
            double reValue = total;
            try
            {
                ActivityBLL bll = new ActivityBLL();
                YX_Activity model = new YX_Activity();
                model.SendCompany = SendCompany;
                model.BlessingWords = BlessingWords;
                model.CompanyLogoURL = CompanyLogoURL;
                model.MainCode = SessCokie.Get.MainCode;
                YX_ActivitySub modelSub = new YX_ActivitySub();
                modelSub.ActivityStyleID = Convert.ToInt32(ActivityStyleID);
                modelSub.ActivityTitle = ActivityTitle;
                modelSub.AtivityImageURL = AtivityImageURL;
                modelSub.Content = Content;
                modelSub.ActivityType = Convert.ToInt16(ActivityType);
                modelSub.StartDate = StartDate;
                modelSub.EndDate = EndDate;
                modelSub.JoinMode = Convert.ToInt16(JoinMode);
                modelSub.ShareFriends = Convert.ToBoolean(ShareFriends);
                modelSub.CreateDate = DateTime.Now;
                modelSub.RedStyle = Convert.ToInt32(RedStyle);
                modelSub.SetStatus = (int)EnumText.SetStatus.NoSet;
                modelSub.ActiveStatus = (int)Common.EnumText.ActivityState.NoStart;
                modelSub.IsYZCode = IsYZCode;
                YX_RedRecharge recharge = new YX_RedRecharge();
                //查找企业是否设置了微信红包账户
                WxEnAccountBLL wxbll = new WxEnAccountBLL();
                YX_WxEnAccount wxzh = wxbll.GetModel(Common.Argument.SessCokie.Get.EnterpriseID);
                if (wxzh != null)
                {
                    recharge.CollectMan = Common.Argument.SessCokie.Get.EnterpriseName;
                    recharge.CompanyID = Common.Argument.SessCokie.Get.EnterpriseID;
                    recharge.PayMan = Common.Argument.SessCokie.Get.EnterpriseName;
                    recharge.RechargeMode = (int)Common.EnumText.PayType.IndependentPay;
                    recharge.CreateDate = DateTime.Now;
                    recharge.PayState = (int)Common.EnumText.PayState.Payed;
                    recharge.RechargeValue = total;
                    modelSub.OpenMode = (int)Common.EnumText.OpenMode.Auto; ;//根据活动日期自动开始活动
                    modelSub.ActiveStatus = (int)Common.EnumText.ActivityState.Going;
                }
                else
                {
                    recharge.CollectMan = "河北广联";
                    recharge.CompanyID = Common.Argument.SessCokie.Get.EnterpriseID;
                    recharge.PayMan = Common.Argument.SessCokie.Get.EnterpriseName;
                    recharge.RechargeMode = payMethod;
                    recharge.CreateDate = DateTime.Now;
                    recharge.PayState = (int)Common.EnumText.PayState.NoPay;
                    recharge.RechargeValue = total;
                }
                //YX_RedRecharge recharge = new YX_RedRecharge { CollectMan = "河北广联", CompanyID = Common.Argument.SessCokie.Get.EnterpriseID, PayMan = Common.Argument.SessCokie.Get.EnterpriseName, RechargeMode = payMethod, CreateDate = DateTime.Now, PayState = (int)Common.EnumText.PayState.NoPay, RechargeValue = total };
                if (SessCokie.Get != null)
                {
                    model.CompanyID = (long)SessCokie.Get.EnterpriseID;
                    modelSub.CompanyID = (long)SessCokie.Get.EnterpriseID;
                }
                else
                {
                    return Json(new { ok = result.CmdError, msg = "请先登录" });
                }
                result = bll.AddModel(model, modelSub, setDetail, recharge, Convert.ToInt64(SetingID), out activityID);
                orderNum = recharge.OrderNum;
            }
            catch
            { }
            JsonResult js = new JsonResult();
            if (result.IsSuccess)
            {
                Session["ActivityID"] = activityID;
                switch (payMethod)
                {
                    case 1://支付宝
                        Session["SetPay"] = "NoSetPay";
                        string SiteUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"];
                        js.Data = new
                        {
                            res = true,
                            info = result.Msg,
                            flag = "zfb",
                            url = "/Market/OlineAlipay/Alipay?out_trade_no=" + orderNum +
                            "&subject=" + activityID +
                            "&total_fee=" + reValue +
                            "&show_url=" + SiteUrl + "Market/HomeMarket/MainFrame?flag=2_" + activityID
                        };
                        break;
                    case 2://微信
                        js.Data = new { res = true, flag = "wx", info = result.Msg, tradeNo = orderNum, sumMoney = reValue, id = activityID };
                        break;
                    case 3://线下支付
                        js.Data = new { res = true, flag = "xx", info = result.Msg, id = activityID };
                        break;
                    default:
                        js.Data = new { res = true, flag = "xx", info = result.Msg, id = activityID };
                        break;
                }
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }

        public JsonResult FindTitle(string ActivityTitle)
        {
            RetResult result = new RetResult();
            try
            {
                if (SessCokie.Get == null)
                {
                    return Json(new { ok = result.CmdError, msg = "请先登录" });
                }
                ActivityBLL bll = new ActivityBLL();
                result = bll.FindTitle(ActivityTitle, SessCokie.Get.EnterpriseID);
            }
            catch
            { }
            return Json(new { ok = result.CmdError, msg = result.Msg });
        }

        /// <summary>
        /// 修改活动
        /// </summary>
        /// <param name="activityId">活动编号</param>
        /// <returns></returns>
        public ActionResult Edit(long activityId)
        {

            //查找企业是否设置了微信红包账户
            WxEnAccountBLL wxbll = new WxEnAccountBLL();
            YX_WxEnAccount wxzh = wxbll.GetModel(SessCokie.Get.EnterpriseID);
            if (wxzh == null)
            {
                ViewBag.Flag = 0;
            }
            else
            {
                ViewBag.Flag = 1;
            }
            ActivityBLL bll = new ActivityBLL();
            ViewBag.YX_Activity = bll.GetActivity(activityId);
            ViewBag.YX_ActivitySub = bll.GetActivitySub(activityId);
            List<YX_AcivityDetail> details = bll.GetDetail(activityId);
            ViewBag.YX_AcivityDetail = details;
            //红包总额
            double? value = 0;
            //红包总个数
            long count = 0;
            string valueStr = string.Empty;
            string countStr = string.Empty;
            foreach (var sub in details)
            {
                value = value + sub.RedValue * sub.RedCount;
                count = count + (long)sub.RedCount;
                sub.TotalNum = sub.RedValue * sub.RedCount;
                if (string.IsNullOrEmpty(valueStr))
                {
                    valueStr = sub.RedValue.ToString();
                }
                else
                {
                    valueStr = valueStr + "," + sub.RedValue.ToString();
                }
                if (string.IsNullOrEmpty(countStr))
                {
                    countStr = sub.RedCount.ToString();
                }
                else
                {
                    countStr = countStr + "," + sub.RedCount.ToString();
                }
            }
            ViewBag.RedValue = value;
            ViewBag.RedCount = count;
            ViewBag.ValueStr = valueStr;
            ViewBag.CountStr = countStr;
            var model = new RedRechargeBLL().GetModel(activityId);
            ViewBag.PayState = model == null ? 1 : model.PayState;
            ViewBag.TotalCharge = model == null ? 0 : model.RechargeValue;
            ViewBag.PayWay = model == null ? 1 : model.RechargeMode;
            ViewBag.Sxf = model == null ? 0 : model.RechargeValue - value;
            return View();
        }

        /// <summary>
        /// 修改第一步
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <param name="ActivityStyleID">活动目标</param>
        /// <param name="ActivityTitle">活动名称</param>
        /// <param name="AtivityImageURL">活动图片</param>
        /// <param name="Content">规则</param>
        /// <param name="ActivityType">活动类型</param>
        /// <param name="StartDate">开始日期</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="JoinMode">参与方式</param>
        /// <param name="ShareFriends">是否分享到朋友圈</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditActivitySub(long activityID, string ActivityStyleID, string ActivityTitle,
            string AtivityImageURL, string Content, string ActivityType, DateTime? StartDate, DateTime? EndDate, string JoinMode, string ShareFriends, int IsYZCode)
        {
            RetResult result = new RetResult();
            try
            {
                ActivityBLL bll = new ActivityBLL();
                YX_ActivitySub modelSub = new YX_ActivitySub();
                modelSub.ActivityID = activityID;
                modelSub.ActivityStyleID = Convert.ToInt32(ActivityStyleID);
                modelSub.ActivityTitle = ActivityTitle;
                modelSub.AtivityImageURL = AtivityImageURL;
                modelSub.Content = Content;
                modelSub.ActivityType = Convert.ToInt16(ActivityType);
                modelSub.StartDate = StartDate;
                modelSub.EndDate = EndDate;
                modelSub.JoinMode = Convert.ToInt16(JoinMode);
                modelSub.ShareFriends = Convert.ToBoolean(ShareFriends);
                modelSub.CreateDate = DateTime.Now;
                modelSub.ActiveStatus = (int)Common.EnumText.Status.used;
                modelSub.IsYZCode = IsYZCode;
                if (SessCokie.Get != null)
                {
                    modelSub.CompanyID = (long)SessCokie.Get.EnterpriseID;
                }
                else
                {
                    return Json(new { ok = result.CmdError, msg = "请先登录" });
                }
                result = bll.EditModelSub(modelSub);
            }
            catch
            { }
            return Json(new { ok = result.CmdError, msg = result.Msg });
        }

        /// <summary>
        /// 修改第二步
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <param name="SendCompany">商户名称</param>
        /// <param name="BlessingWords">祝福语</param>
        /// <param name="CompanyLogoURL">企业logo</param>
        /// <param name="setDetail">红包设置</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditActivity(long activityID, string SendCompany, string BlessingWords, string CompanyLogoURL, string setDetail,
            double? sumM, int payMethod, double sxf, double total)
        {
            RetResult result = new RetResult();
            var modelRe = new RedRechargeBLL().GetModel(activityID);
            string orderNum = string.Empty;
            try
            {
                ActivityBLL bll = new ActivityBLL();
                YX_Activity model = new YX_Activity();
                model.ActivityID = activityID;
                model.SendCompany = SendCompany;
                model.BlessingWords = BlessingWords;
                model.CompanyLogoURL = CompanyLogoURL;
                LoginInfo user = SessCokie.Get;
                if (user != null)
                {
                    model.CompanyID = (long)user.EnterpriseID;
                }
                else
                {
                    return Json(new { ok = result.CmdError, msg = "请先登录" });
                }
                YX_RedRecharge recharge = new YX_RedRecharge { CollectMan = "河北广联", CompanyID = Common.Argument.SessCokie.Get.EnterpriseID, PayMan = Common.Argument.SessCokie.Get.EnterpriseName, RechargeMode = payMethod, CreateDate = DateTime.Now, PayState = (int)Common.EnumText.PayState.NoPay, RechargeValue = total, ActivityID = activityID };
                if (modelRe != null && modelRe.PayState == (int)Common.EnumText.PayState.Payed)
                {
                    recharge = null;
                }
                result = bll.EditModel(model, setDetail, recharge);
                orderNum = recharge == null ? "" : recharge.OrderNum;
            }
            catch
            { }
            if (modelRe != null && modelRe.PayState == (int)Common.EnumText.PayState.Payed)
            {
                return Json(new { ok = result.CmdError, msg = result.Msg, pay = "noPay" });
            }
            else
            {
                JsonResult js = new JsonResult();
                if (result.IsSuccess)
                {
                    Session["ActivityID"] = activityID;
                    switch (payMethod)
                    {
                        case 1://支付宝
                            Session["SetPay"] = "NoSetPay";
                            string SiteUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"];
                            js.Data = new
                            {
                                res = true,
                                info = result.Msg,
                                flag = "zfb",
                                url = "/Market/OlineAlipay/Alipay?out_trade_no=" + modelRe.OrderNum +
                                "&subject=" + activityID +
                                "&total_fee=" + total +
                                "&show_url=" + SiteUrl + "Market/HomeMarket/MainFrame?flag=2_" + activityID
                            };
                            break;
                        case 2://微信
                            js.Data = new { res = true, flag = "wx", info = result.Msg, tradeNo = modelRe.OrderNum, sumMoney = total, id = activityID };
                            break;
                        case 3://线下支付
                            js.Data = new { res = true, flag = "xx", info = result.Msg, id = activityID };
                            break;
                        default:
                            js.Data = new { res = true, flag = "xx", info = result.Msg, id = activityID };
                            break;
                    }
                }
                else
                {
                    js.Data = new { res = false, info = result.Msg };
                }
                return js;
            }
        }

        /// <summary>
        /// 码图
        /// </summary>
        /// <param name="content">码内容</param>
        /// <param name="w">图片宽度</param>
        /// <param name="h">图片高度</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult CenterEwmImg(string id, int w, int h, string settingId)
        {
            try
            {
                //优惠券和红包浏览
                string url = string.Empty;
                if (string.IsNullOrEmpty(settingId))
                {
                    settingId = "0";
                }
                var model = new ScanCodeMarketBLL().GetModel(Convert.ToInt32(settingId), 0);
                if (model.ActivityMethod == (int)Common.EnumText.ActivityMethod.Coupon)
                {
                    url = url = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Market/Coupon/ViewCoupon?activityId=" + model.ActivityID;
                }
                else
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Market/Wap_IndexMarket/SearchActivity?activityId=" + id + "&settingId=" + settingId;
                }
                System.Drawing.Image codeImg = MvcWebCommon.Tools.CreateEwmImg.GetQRCodeImageEx(url, w, h);
                if (codeImg != null)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        codeImg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        Response.ClearContent();
                        Response.ContentType = "image/Jpeg";
                        Response.BinaryWrite(ms.ToArray());
                    }
                }
            }
            catch (Exception ex) { }
            return View();
        }

        /// <summary>
        /// 查看二维码
        /// </summary>
        /// <returns></returns>
        public ActionResult LookEwm(string tradeNo, double jg)
        {
            ViewBag.TradeNo = tradeNo;
            ViewBag.Jg = jg;
            return View();
        }

        /// <summary>
        /// 生成图片
        /// </summary>
        /// <param name="tcid"></param>
        /// <returns></returns>
        public ActionResult CreateImage(string tradeNo, double jg)
        {
            ResponseUnifiedorder order = XmlHelper.Deserialize(typeof(ResponseUnifiedorder), CreateOrder((Math.Ceiling(jg * 100)).ToString(), tradeNo)) as ResponseUnifiedorder;
            if (order != null)
            {
                Image img = MvcWebCommon.Tools.CreateEwmImg.GetQRCodeImageEx(order.code_url, 200, 200);
                if (img != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        return File(ms.ToArray(), "image/Jpeg", DateTime.Now.Ticks.ToString() + ".jpg");
                    }
                }
                else
                {
                    return Content("<script>alert('请重新操作');location.href='history.go(-1)'<script>");
                }
            }
            else
            {
                return Content("<script>alert('请重新操作');location.href='history.go(-1)'<script>");
            }
        }

        /// <summary>
        /// 手动回调
        /// </summary>
        /// <returns></returns>
        public ActionResult HandNotify(string tradeNo)
        {
            try
            {
                string data = SearchOrder("", tradeNo);
                //点击支付完成调一下查询订单
                ResponseOrderQuery queryNew = XmlHelper.Deserialize(typeof(ResponseOrderQuery), data) as ResponseOrderQuery;
                Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + ":HandNotify】" + data + " \r\n", "WxPay");
                if (queryNew.trade_state == "SUCCESS")
                {
                    //数据库操作
                    BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
                    RetResult result = bll.PaySuccess(queryNew.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, queryNew.out_trade_no, queryNew.openid);
                    int i = 5;
                    while (!result.IsSuccess && --i > 0)
                    {
                        result = bll.PaySuccess(queryNew.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, queryNew.out_trade_no, queryNew.openid);
                    }
                    Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + ":" + tradeNo + "】方法：HandNotify微信支付成功" + " \r\n", "WxPay");
                }
                else
                {
                    Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + ":" + tradeNo + "】方法：HandNotify未支付" + " \r\n", "WxPay");
                }
            }
            catch (Exception ex)
            {
                Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + tradeNo + "】方法：HandNotify异常:" + ex.ToString() + " \r\n", "WxPay");
            }
            return Json(new { });
        }
        public string HandNotifyAuto(string tradeNo)
        {
            try
            {
                string data = SearchOrder("", tradeNo);
                Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + ":HandNotifyAuto】" + data + " \r\n", "WxPay");
                //点击支付完成调一下查询订单
                ResponseOrderQuery queryNew = XmlHelper.Deserialize(typeof(ResponseOrderQuery), data) as ResponseOrderQuery;
                if (queryNew.trade_state == "SUCCESS")
                {
                    //数据库操作
                    BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
                    RetResult result = bll.PaySuccess(queryNew.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, queryNew.out_trade_no, queryNew.openid);
                    int i = 5;
                    while (!result.IsSuccess && --i > 0)
                    {
                        result = bll.PaySuccess(queryNew.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, queryNew.out_trade_no, queryNew.openid);
                    }
                    Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + ":" + tradeNo + "】方法：HandNotifyAuto微信支付成功" + " \r\n", "WxPay");
                }
                else
                {
                    Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + ":" + tradeNo + "】方法：HandNotifyAuto未支付" + " \r\n", "WxPay");
                }
            }
            catch (Exception ex)
            {
                Common.Log.WriteLog.WriteWxLog("【" + DateTime.Now.ToString("yyyy-MM-dd") + tradeNo + "】异常:" + ex.ToString() + " \r\n", "WxPay");
            }
            return null;
        }
        #region 微信接口
        /// <summary>
        /// 回调函数
        /// </summary>
        public void Notify()
        {
            try
            {
                //接收从微信后台POST过来的数据
                Stream s = Request.InputStream;
                byte[] buffer = new byte[1024];
                StringBuilder sb = new StringBuilder();
                int count = 0;
                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
                WeiXinCommon.WriteLog("微信支付返回：" + sb.ToString(), "Notify", "");
                ResponseOrderQuery query = XmlHelper.Deserialize(typeof(ResponseOrderQuery), sb.ToString()) as ResponseOrderQuery;
                //检查支付结果中transaction_id是否存在
                if (string.IsNullOrWhiteSpace(query.transaction_id))
                {
                    //若transaction_id不存在，则立即返回结果给微信支付后台
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "FAIL");
                    res.Add("return_msg", "支付结果中微信订单号不存在");
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
                if (query.return_code == "SUCCESS" && query.result_code == "SUCCESS")
                {
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "SUCCESS");
                    res.Add("return_msg", "OK");
                    string msg = "";
                    //数据库操作
                    BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
                    RetResult result = bll.PaySuccess(query.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, query.out_trade_no, query.openid);
                    WeiXinCommon.WriteLog("数据库操作：订单号" + query.out_trade_no, "Notify", "");
                    msg = result.Msg;
                    //点击支付完成调一下查询订单
                    //ResponseOrderQuery queryNew0 = XmlHelper.Deserialize(typeof(ResponseOrderQuery), SearchOrder(query.transaction_id, Session["TradeNo"] as string)) as ResponseOrderQuery;
                    //if (queryNew0.trade_type == "SUCCESS")
                    //{
                    //    //数据库操作
                    //}
                    WeiXinCommon.WriteLog(WeiXinCommon.DictionaryToXml(res), "Notify", msg);
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
                else
                {
                    //若订单查询失败，则立即返回结果给微信支付后台
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "FAIL");
                    res.Add("return_msg", "查询订单失败！");
                    WeiXinCommon.WriteLog("查询订单失败！", "Notify", "");
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WeiXinCommon.WriteLog("出现异常！", "Notify", "");
                SortedList<string, string> res = new SortedList<string, string>();
                res.Add("return_code", "FAIL");
                res.Add("return_msg", "出现异常");
                Response.Write(WeiXinCommon.DictionaryToXml(res));
                Response.End();
            }

        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <returns></returns>
        public string CreateOrder(string money, string tradeNo)
        {
            string data = string.Empty;
            try
            {
                SortedList<string, string> dic = new SortedList<string, string>();
                SortedList<string, string> dicSign = new SortedList<string, string>();
                string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
                string productId = DateTime.Now.Ticks.ToString();
                //tradeNo = WeiXinCommon.GenerateOutTradeNo();
                dicSign.Add("appid", WeiXinCommon._PayId);
                dicSign.Add("mch_id", WeiXinCommon._MerIdPay);
                dicSign.Add("device_info", "WEB");
                dicSign.Add("nonce_str", nonceStr);
                dicSign.Add("body", "支付红包金额");
                dicSign.Add("out_trade_no", tradeNo);
                dicSign.Add("total_fee", money);
                dicSign.Add("spbill_create_ip", System.Configuration.ConfigurationManager.AppSettings["IpAddress"]);
                dicSign.Add("notify_url", System.Configuration.ConfigurationManager.AppSettings["NotifyUrl"] + "Packet/Notify");
                dicSign.Add("trade_type", "NATIVE");
                dicSign.Add("product_id", productId);

                dic.Add("appid", WeiXinCommon._PayId);
                dic.Add("mch_id", WeiXinCommon._MerIdPay);
                dic.Add("device_info", "WEB");
                dic.Add("nonce_str", nonceStr);
                dic.Add("sign", WeiXinCommon.Sign(dicSign, WeiXinCommon._Key));
                dic.Add("body", "支付红包金额");
                dic.Add("out_trade_no", tradeNo);
                dic.Add("total_fee", money);
                dic.Add("spbill_create_ip", System.Configuration.ConfigurationManager.AppSettings["IpAddress"]);
                dic.Add("notify_url", System.Configuration.ConfigurationManager.AppSettings["NotifyUrl"] + "Packet/Notify");
                dic.Add("trade_type", "NATIVE");
                dic.Add("product_id", productId);
                data = APIHelper.sendPost("https://api.mch.weixin.qq.com/pay/unifiedorder", WeiXinCommon.DictionaryToXml(dic));
            }
            catch (Exception ex)
            {
                WeiXinCommon.WriteLog(data, "CreateOrder", ex.Message);
            }
            return data;
        }

        /// <summary>
        /// 查询订单是否微信支付
        /// </summary>
        /// <param name="tranId">商户单号</param>
        /// <returns></returns>
        public string SearchOrder(string tranId, string tradeNo)
        {
            string data = string.Empty;
            try
            {
                SortedList<string, string> dic = new SortedList<string, string>();
                SortedList<string, string> dicSign = new SortedList<string, string>();
                string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
                //20190103新加
                WxEnAccountBLL wxbll = new WxEnAccountBLL();
                YX_RedRecharge wxCompany = wxbll.GetEnID(tradeNo);
                if (wxCompany != null)
                {
                    YX_WxEnAccount wxzh = wxbll.GetModel(wxCompany.CompanyID.Value);
                    if (wxzh != null)
                    {
                        dicSign.Add("appid", wxzh.WxAppId);
                        dicSign.Add("mch_id", wxzh.MarId);
                        dicSign.Add("transaction_id", tranId);
                        dicSign.Add("out_trade_no", tradeNo);
                        dicSign.Add("nonce_str", nonceStr);

                        dic.Add("appid", wxzh.WxAppId);
                        dic.Add("mch_id", wxzh.MarId);
                        dic.Add("transaction_id", tranId);
                        dic.Add("nonce_str", nonceStr);
                        dic.Add("out_trade_no", tradeNo);
                        dic.Add("sign", WeiXinCommon.Sign(dicSign, wxzh.Key));
                        data = APIHelper.sendPost("https://api.mch.weixin.qq.com/pay/orderquery", WeiXinCommon.DictionaryToXml(dic));
                        WeiXinCommon.WriteLog(data, "SearchOrder", "");
                        ResponseOrderQuery order = XmlHelper.Deserialize(typeof(ResponseOrderQuery), data) as ResponseOrderQuery;
                    }
                    else
                    {
                        dicSign.Add("appid", WeiXinCommon._PayId);
                        dicSign.Add("mch_id", WeiXinCommon._MerIdPay);
                        dicSign.Add("transaction_id", tranId);
                        dicSign.Add("out_trade_no", tradeNo);
                        dicSign.Add("nonce_str", nonceStr);

                        dic.Add("appid", WeiXinCommon._PayId);
                        dic.Add("mch_id", WeiXinCommon._MerIdPay);
                        dic.Add("transaction_id", tranId);
                        dic.Add("nonce_str", nonceStr);
                        dic.Add("out_trade_no", tradeNo);
                        dic.Add("sign", WeiXinCommon.Sign(dicSign, WeiXinCommon._Key));
                        data = APIHelper.sendPost("https://api.mch.weixin.qq.com/pay/orderquery", WeiXinCommon.DictionaryToXml(dic));
                        WeiXinCommon.WriteLog(data, "SearchOrder", "");
                        ResponseOrderQuery order = XmlHelper.Deserialize(typeof(ResponseOrderQuery), data) as ResponseOrderQuery;
                    }
                }
            }
            catch (Exception ex)
            {
                WeiXinCommon.WriteLog(data, "SearchOrder", ex.Message);
            }
            return data;
        }
        #endregion
    }
}
