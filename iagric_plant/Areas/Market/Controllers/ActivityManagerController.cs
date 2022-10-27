/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-9
** 联系方式:13313318725
** 代码功能：红包活动管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using BLL;
using MarketActive.Filter;
using System.Configuration;
using System.Text;
using Dal;

namespace MarketActive.Controllers
{
    [AdminAuthorize]
    public class ActivityManagerController : Controller
    {
        /// <summary>
        ///  获取红包活动列表
        /// </summary>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult Index(int? id, int activityStatus = -1)
        {
            LoginInfo user = SessCokie.Get;
            string acName = Request["acName"];
            ViewBag.Name = acName;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            string hbType = Request["hbType"] ?? "-1";
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"hbType\" name=\"hbType\">");
            sb.Append("<option value=\"-1\">全部</option>");
            if (hbType == "1")
            {
                sb.Append("<option value=\"1\" selected=\"selected\">红包活动</option>");
                sb.Append("<option value=\"2\">优惠券活动</option>");
            }
            else if (hbType == "2")
            {
                sb.Append("<option value=\"1\">红包活动</option>");
                sb.Append("<option value=\"2\" selected=\"selected\">优惠券活动</option>");
            }
            else
            {
                sb.Append("<option value=\"1\">红包活动</option>");
                sb.Append("<option value=\"2\">优惠券活动</option>");
            }
            sb.Append("</select>");
            ViewBag.hbType = sb.ToString();
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            ViewBag.Status = activityStatus;
            if (activityStatus == 0)
            {
                activityStatus = (int)Common.EnumText.ActivityState.Going;
            }
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            ActivityManagerBLL bll = new ActivityManagerBLL();
            PagedList<View_ActivityManager> list = bll.GetList(user.EnterpriseID, acName, sDate, eDate, activityStatus, Convert.ToInt32(hbType), pageIndex);
            return View(list);
        }

        /// <summary>
        /// 查看活动信息
        /// </summary>
        /// <param name="id">活动id</param>
        /// <returns></returns>
        public ActionResult Info(long id)
        {
            ActivityManagerBLL bll = new ActivityManagerBLL();
            View_ActivityManager activityInfo = bll.GetActivityInfo(id);
            List<YX_AcivityDetail> hbDetail = bll.HbDetail(id);
            Double sumCount = 0;
            int hbCount = 0;
            if (hbDetail.Count > 0)
            {
                foreach (var item in hbDetail)
                {
                    YX_AcivityDetail hbInfo = new YX_AcivityDetail();
                    hbInfo.RedValue = item.RedValue;
                    hbInfo.RedCount = item.RedCount;
                    ViewBag.xiaoji = (hbInfo.RedValue) * (hbInfo.RedCount);
                    sumCount += ViewBag.xiaoji;
                    hbCount += hbInfo.RedCount.Value;
                }
            }
            ViewBag.sumCount = sumCount;
            ViewBag.hbCount = hbCount;
            ViewBag.hbDetail = hbDetail;
            List<View_RedGetRecord> hbGetRecord = bll.HbGetRecord(id);
            ViewBag.hbGetRecord = hbGetRecord;
            return View(activityInfo);
        }

        /// <summary>
        /// 结束活动
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <returns></returns>
        public ActionResult EditStatusEnd(long id)
        {
            LoginInfo user = SessCokie.Get;
            ActivityManagerBLL bll = new ActivityManagerBLL();
            RetResult ret = bll.EditStatusEnd(id, user.EnterpriseID);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Market/ActivityManager/Index" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 开始活动
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <returns></returns>
        public ActionResult EditStatusStar(long id)
        {
            LoginInfo user = SessCokie.Get;
            ActivityManagerBLL bll = new ActivityManagerBLL();
            RetResult ret = bll.EditStatusStar(id, user.EnterpriseID);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Market/ActivityManager/Index" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 预览码
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult YuLan(long id = 0, long settingId = 0)
        {
            try
            {
                if (settingId > 0)
                {
                    ActivityManagerBLL bll = new ActivityManagerBLL();
                    YX_ActvitiyRelationCode code = bll.GetActivityID(settingId);
                    ViewBag.SettingId = settingId;
                    if (code != null)
                    {
                        ViewBag.id = code.ActivityID;
                    }
                }
                else
                {
                    ViewBag.id = id;
                    ViewBag.Method = new ActivityManagerBLL().GetActivityInfo(id).ActivityMethod == (int)Common.EnumText.ActivityMethod.Coupon ? "coupon" : "";
                }
            }
            catch (Exception ex) { }
            return View();
        }

        /// <summary>
        /// 红包充值记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult RechangeRecord(int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            var model = new ActivityManagerBLL().GetRedMoney(user.EnterpriseID, pageIndex);
            ViewBag.EnterName = user.EnterpriseName;
            return View(model);
        }

        /// <summary>
        /// 红包发送记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult SendRecord(int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            var model = new ActivityManagerBLL().GetSendMoney(user.EnterpriseID, pageIndex);
            return View(model);
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public ActionResult SetPay(long activityId)
        {
            var model = new RedRechargeBLL().GetModel(activityId);
            string url = "";
            string flag = "";
            string orderNum = model.OrderNumAgain + "C";
            new RedRechargeBLL().UpdateModel(activityId);
            if (model.RechargeMode == (int)Common.EnumText.PayType.OlineAlipay)
            {
                url = "/Market/OlineAlipay/Alipay?out_trade_no=" + orderNum +
                            "&subject=" + activityId +
                            "&total_fee=" + model.RechargeValue +
                            "&show_url=" + System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Market/HomeMarket/MainFrame?flag=1";
                flag = "zfb";
                Session["SetPay"] = "SetPay";
            }
            if (model.RechargeMode == (int)Common.EnumText.PayType.WeiXinPay)
            {
                flag = "wx";
            }
            return Json(new { url = url, flag = flag, tradeNo = orderNum, sumMoney = model.RechargeValue, id = activityId });
        }

        /// <summary>
        /// 下载码值
        /// </summary>
        public void DownCode(long activityId)
        {
            var model = new ActivityRelationCodeBLL().GetModelComanyId(activityId);
            Response.Clear();
            Response.Buffer = true;
            string fileName = DateTime.Now.Ticks.ToString();
            try
            {
                Response.ContentType = "text/plain";
                if (Request.Browser.Browser == "IE")
                {
                    Response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}.txt", HttpUtility.UrlDecode(HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8))));
                }
                else
                {
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".txt");
                }
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                string url = ConfigurationManager.AppSettings["marketURL"];
                for (long i = model.FromCode.Value; i <= model.EndCode; i++)
                {
                    Response.BinaryWrite(System.Text.Encoding.Default.GetBytes(string.Format("{3}{0}{1}.{2}\r\n", model.CodeMain, model.CompanyIDcodeID, i, url)));
                }
                Response.Flush();
                Response.End();
            }
            catch (Exception)
            {
                Response.ContentType = "text/html";
                Response.Write("<script>alert('下载过程中出现错误！');history.go(-1);</script>");
            }
        }

        /// <summary>
        /// 优惠券核销
        /// </summary>
        /// <returns></returns>
        public ActionResult YhqCancelOut()
        {
            return View();
        }

        /// <summary>
        /// 优惠券核销
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="yhqcode">优惠券码号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult YhqCancelOut(string yhqcode)
        {
            LoginInfo user = SessCokie.Get;
            yhqcode = Request["yhqcode"];
            ActivityManagerBLL bll = new ActivityManagerBLL();
            RetResult ret = bll.YhqCancelOut(user.EnterpriseID, yhqcode);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Market/ActivityManager/Index" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 查看优惠券详情
        /// </summary>
        /// <param name="id">活动id</param>
        /// <returns></returns>
        public ActionResult YhqInfo(long id, int? pageid)
        {
            ViewBag.id = id;
            ActivityManagerBLL bll = new ActivityManagerBLL();
            View_ActivityCoupon yhqInfo = bll.GetYhqInfo(id);
            int pageIndex = pageid == null ? 1 : Convert.ToInt32(pageid.ToString());
            string yhqType = Request["yhqType"] ?? "-1";
            StringBuilder sba = new StringBuilder();
            sba.Append("<select id=\"yhqType\" name=\"yhqType\">");
            sba.Append("<option value=\"-1\">全部</option>");
            if (yhqType == "1")
            {
                sba.Append("<option value=\"1\" selected=\"selected\">已核销</option>");
                sba.Append("<option value=\"2\">未核销</option>");
            }
            else if (yhqType == "2")
            {
                sba.Append("<option value=\"1\">已核销</option>");
                sba.Append("<option value=\"2\" selected=\"selected\">未核销</option>");
            }
            else
            {
                sba.Append("<option value=\"1\">已核销</option>");
                sba.Append("<option value=\"2\">未核销</option>");
            }
            sba.Append("</select>");
            ViewBag.yhqType = sba.ToString();
            ViewBag.Selected = yhqType;
            PagedList<View_CouponGetRecord> yhqGetDetail = bll.YhqGetDetail(id, Convert.ToInt32(yhqType), pageIndex);
            ViewBag.yhqGetRecord = yhqGetDetail;
            if (yhqGetDetail.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
                if (AllAddress != null)
                {
                    List<AddressInfo> citysheng = AllAddress.AddressList.Where(p => p.AddressLevel == 1).OrderBy(p => p.AddressCode).ToList();
                    List<AddressInfo> cityshi = AllAddress.AddressList.Where(p => p.AddressLevel == 2).OrderBy(p => p.AddressCode).ToList();
                    List<AddressInfo> cityqu = AllAddress.AddressList.Where(p => p.AddressLevel == 3).OrderBy(p => p.AddressCode).ToList();
                    //List<AddressInfo> cityAddress = Common.Argument.BaseData.listAddressNew;
                    #region 显示所需属性
                    long ShengID = 0;
                    long ShiID = 0;
                    long QuID = 0;
                    #endregion
                    foreach (var item in yhqGetDetail)
                    {
                        if (!string.IsNullOrEmpty(item.ProvinceID) && !string.IsNullOrEmpty(item.CityID) && !string.IsNullOrEmpty(item.AreaID))
                        {
                            string cityid = item.ProvinceID + "_" + item.CityID + "_" + item.AreaID;
                            string[] para = cityid.Split('_');
                            if (para.Length == 3)
                            {
                                ShengID = long.Parse(para[0]);
                                ShiID = long.Parse(para[1]);
                                QuID = long.Parse(para[2]);
                                var sheng = citysheng.FirstOrDefault(m => m.Address_ID == ShengID);
                                sb.Append(sheng == null ? "" : sheng.AddressName);
                                sb.Append("-");
                                if (ShengID == 110000 || ShengID == 310000 || ShengID == 500000 || ShengID == 120000 || ShengID == 810000 || ShengID == 820000 || ShengID == 710000)//直辖市
                                {
                                    sb.Append(sheng == null ? "" : sheng.AddressName);
                                }
                                else
                                {
                                    var shi = cityshi.FirstOrDefault(m => m.Address_ID == ShiID);
                                    sb.Append(shi == null ? "" : shi.AddressName);
                                }
                                sb.Append("-");
                                var qu = cityqu.FirstOrDefault(m => m.Address_ID == QuID);
                                sb.Append(qu == null ? "" : qu.AddressName);
                            }
                        }
                        item.Address = sb.ToString();
                        sb.Clear();
                    }
                }
            }
            return View(yhqInfo);
        }

        /// <summary>
        /// 新建活动页面（优惠券/微信红包）
        /// </summary>
        /// <returns></returns>
        public ActionResult AddActivity()
        {
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
                ViewBag.User = userInfo;
            }
            else
            {
                CurrentUser.User = new UserInfo();
                CurrentUser.User.LoginType = (int)Common.EnumText.LoginType.common;
                ViewBag.LoginType = (int)Common.EnumText.LoginType.common;
            }
            return View();
        }

        #region 20190214添加大转盘活动相关操作
        public ActionResult LotteryInfo(long activityId, int? pageid, long activityLotteryId = -1)
        {
            ViewBag.activityId = activityId;
            ActivityDAL dal = new ActivityDAL();
            YXLotteryDal ldal = new YXLotteryDal();
            YX_ActivitySub activityInfo = dal.GetActivitySub(activityId);//活动基本信息
            int pageIndex = pageid == null ? 1 : Convert.ToInt32(pageid.ToString());
            List<YX_ActivityLottery> LotteryDetail = ldal.GetJXList(activityId);//大转盘活动奖项明细
            ViewBag.LotteryDetail = LotteryDetail;
            //PagedList<view_lotterygetrecord> GetDetail = dal.GetLotteryGetRecordList(activityLotteryId, activityId, pageIndex);
            //ViewBag.GetDetail = GetDetail;
            return View(activityInfo);
        }

        /// <summary>
        /// 奖项明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrizeDetail(long activityId)
        {
            YXLotteryDal dal = new YXLotteryDal();
            List<YX_ActivityLottery> list = dal.GetJXList(activityId);
            return View(list);
        }
        #endregion
    }
}
