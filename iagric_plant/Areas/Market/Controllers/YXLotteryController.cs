/********************************************************************************
** 作者：张翠霞
** 开发时间：2019-2-12
** 联系方式:
** 代码功能：大转盘活动控制器
** 版本：v1.0
** 版权：研发部
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Common.Argument;
using Dal;
using LinqModel;
using Common.Log;
using MarketActive.Filter;
using BLL;
using Common;

namespace MarketActive.Controllers
{
    public class YXLotteryController : Controller
    {
        //
        // GET: /Market/YXLottery/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加大转盘
        /// </summary>
        /// <param name="spliteId">拆分ID</param>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Add(long activityId = 0)
        {
            LoginInfo user = SessCokie.Get;
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
            ViewBag.ActivityID = activityId;
            return View();
        }

        /// <summary>
        /// 添加第一步
        /// </summary>
        /// <param name="SplitID">拆分ID</param>
        /// <param name="ActivityTitle">活动名称</param>
        /// <param name="Content">活动规则</param>
        /// <param name="StartDate">活动开始时间</param>
        /// <param name="EndDate">活动结束时间</param>
        /// <param name="JoinMode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddActivity(string SetingID, string ActivityTitle, string Content, DateTime? StartDate, DateTime? EndDate, string JoinMode)
        {
            LoginInfo user = SessCokie.Get;
            RetResult result = new RetResult();
            JsonResult js = new JsonResult();
            try
            {
                YXLotteryDal dal = new YXLotteryDal();
                YX_ActivitySub model = new YX_ActivitySub();
                model.ActivityTitle = ActivityTitle;
                model.Content = Content;
                model.StartDate = StartDate;
                model.EndDate = EndDate;
                model.JoinMode = Convert.ToInt16(JoinMode);
                model.ActivityMethod = (int)Common.EnumText.ActivityMethod.Lottery;
                model.CreateDate = DateTime.Now;
                model.ActiveStatus = (int)Common.EnumText.ActivityState.NoStart;
                model.OpenMode = (int)Common.EnumText.OpenMode.Hand; ;
                model.RedStyle = (int)Common.EnumText.RedStyle.藏;
                model.ActivityType = (int)Common.EnumText.ActiveType.Multi;//一个码只能领取一次（按码领取）
                if (user != null)
                {
                    model.CompanyID = (long)user.EnterpriseID;
                    model.MainCode = user.MainCode;
                }
                else
                {
                    string msg = "请先登录！";
                    js.Data = new { res = false, info = msg };
                    return js;
                }
                if (model == null)
                {
                    result.Msg = "数据错误";
                }
                else
                {
                    result = dal.AddActive(model, Convert.ToInt64(SetingID));
                }
            }
            catch
            {
                js.Data = new { res = false, info = "出现异常！" };
            }
            if (result.IsSuccess)
            {
                Session["ActivityID"] = result.id;
                js.Data = new { res = true, info = result.Msg, id = result.id };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }
        
        /// <summary>
        /// 设置奖项
        /// </summary>
        /// <returns></returns>
        public ActionResult SetLottery(long ActiveID)
        {
            ViewBag.ActiveID = ActiveID;
            return View();
        }

        /// <summary>
        /// 添加奖项
        /// </summary>
        /// <param name="activeID">活动ID</param>
        /// <param name="lotteryName">奖项名称</param>
        /// <param name="lotteryPic">奖项图片</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="lotteryFilePath">文件绝对路径</param>
        /// <param name="lotteryFileURL">文件相对路径</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddLottery(long activeID, string lotteryName, string lotteryPic, string fileName, string lotteryFilePath, string lotteryFileURL, string picHS)
        {
            LoginInfo user = SessCokie.Get;
            RetResult result = new RetResult();
            JsonResult js = new JsonResult();
            try
            {
                YXLotteryDal dal = new YXLotteryDal();
                YX_ActivityLottery model = new YX_ActivityLottery();
                model.ActivityID = activeID;
                model.AddDate = DateTime.Now;
                model.AddUserID = user.UserID;
                model.AddUserName = user.UserName;
                model.LotteryFile = fileName;
                model.LotteryFilePath = lotteryFilePath.Split('#')[1];
                model.LotteryFileURL = lotteryFileURL.Split('~')[0];
                model.LotteryName = lotteryName;
                model.LotteryPic = lotteryPic;
                model.Status = (int)Common.EnumFile.Status.used;
                model.ReceiveStatus = (int)Common.EnumText.ReceiveStatus.NoReceive;
                model.LotteryPicHS = Convert.ToInt32(picHS);
                if (user != null)
                {
                    model.EnterpriseID = user.EnterpriseID;
                }
                else
                {
                    string msg = "请先登录！";
                    js.Data = new { res = false, info = msg };
                    return js;
                }
                result = dal.AddLottery(model);
                if (result.IsSuccess)
                {
                    js.Data = new { res = true, info = result.Msg };
                }
                else
                {
                    js.Data = new { res = false, info = result.Msg };
                }
            }
            catch
            {
                js.Data = new { res = false, info = "出现异常！" };
            }
            return js;
        }

        /// <summary>
        /// 获取奖项列表
        /// </summary>
        /// <param name="ActiveID">活动ID</param>
        /// <returns></returns>
        public ActionResult GetJXList(long ActiveID)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                YXLotteryDal dal = new YXLotteryDal();
                List<YX_ActivityLottery> modellist = dal.GetJXList(ActiveID);
                long lotteryCount = 0;
                if (modellist.Count > 0)
                {
                    foreach (var item in modellist)
                    {
                        lotteryCount += item.LotteryCount.Value;
                    }
                }
                result = ToJson.NewListToJson(modellist, 1, modellist.Count, lotteryCount, "");
            }
            catch (Exception ex)
            {
                string errData = "YXLotteryController.GetJXList()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除奖项
        /// </summary>
        /// <param name="lotteryID">标识ID</param>
        /// <returns></returns>
        public JsonResult DelLottery(long lotteryID)
        {
            JsonResult js = new JsonResult();
            YXLotteryDal dal = new YXLotteryDal();
            RetResult result = dal.DelLottery(lotteryID);
            if (result.IsSuccess)
            {
                js.Data = new { res = true, info = result.Msg };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }

        /// <summary>
        /// 新建第二步比较奖项数量
        /// </summary>
        /// <param name="splitID"></param>
        /// <param name="jxSumCount"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CompareJXCount(string settingID,long ActiveID, string jxSumCount)
        {
            LoginInfo user = SessCokie.Get;
            JsonResult js = new JsonResult();
            YXLotteryDal dal = new YXLotteryDal();
            RetResult result = dal.CompareJXCount(user.EnterpriseID, settingID,ActiveID, Convert.ToInt64(jxSumCount));
            if (result.IsSuccess)
            {
                js.Data = new { res = true, info = result.Msg };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }

        /// <summary>
        /// 编辑奖项
        /// </summary>
        /// <param name="lotteryID"></param>
        /// <returns></returns>
        public ActionResult EditLottery(long lotteryID)
        {
            YXLotteryDal dal = new YXLotteryDal();
            YX_ActivityLottery m = dal.GetLotteryModel(lotteryID);
            if (m != null)
            {
                ViewBag.ActiveID = m.ActivityID;
                ViewBag.LotteryID = lotteryID;
                return View(m);
            }
            return null;
        }

        /// <summary>
        /// 编辑奖项
        /// </summary>
        /// <param name="activeID">活动ID</param>
        /// <param name="lotteryID">奖项ID</param>
        /// <param name="lotteryName">奖项名称</param>
        /// <param name="lotteryPic">奖项图片</param>
        /// <param name="fileName">奖项文件</param>
        /// <param name="lotteryFilePath">文件绝对路径</param>
        /// <param name="lotteryFileURL">文件相对路径</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditLottery(long activeID, long lotteryID, string lotteryName, string lotteryPic, string fileName, string lotteryFilePath, string lotteryFileURL, string picHS)
        {
            LoginInfo user = SessCokie.Get;
            RetResult result = new RetResult();
            JsonResult js = new JsonResult();
            try
            {
                YXLotteryDal dal = new YXLotteryDal();
                YX_ActivityLottery model = new YX_ActivityLottery();
                model.ActivityID = activeID;
                model.ActivitylotteryID = lotteryID;
                model.LotteryFile = fileName;
                if (lotteryFilePath.Contains("#"))
                {
                    model.LotteryFilePath = lotteryFilePath.Split('#')[1];
                }
                else
                {
                    model.LotteryFilePath = lotteryFilePath;
                }
                if (lotteryFileURL.Contains("~"))
                {
                    model.LotteryFileURL = lotteryFileURL.Split('~')[0];
                }
                else
                {
                    model.LotteryFileURL = lotteryFileURL;
                }
                model.LotteryName = lotteryName;
                model.LotteryPic = lotteryPic;
                model.LotteryPicHS = Convert.ToInt32(picHS);
                if (user != null)
                {
                    model.EnterpriseID = user.EnterpriseID;
                }
                else
                {
                    string msg = "请先登录！";
                    js.Data = new { res = false, info = msg };
                    return js;
                }
                result = dal.EditLottery(model);
                if (result.IsSuccess)
                {
                    js.Data = new { res = true, info = result.Msg };
                }
                else
                {
                    js.Data = new { res = false, info = result.Msg };
                }
            }
            catch
            {
                js.Data = new { res = false, info = "出现异常！" };
            }
            return js;
        }

        /// <summary>
        /// 编辑大转盘活动
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public ActionResult Edit(long activityId)
        {
            LoginInfo user = SessCokie.Get;
            ActivityDAL dal = new ActivityDAL();
            ViewBag.ActivityID = activityId;
            YX_ActivitySub active = dal.GetActivitySub(activityId);
            return View(active);
        }

        /// <summary>
        /// 编辑第一步
        /// </summary>
        /// <param name="activeID">拆分ID</param>
        /// <param name="ActivityTitle">活动名称</param>
        /// <param name="Content">活动规则</param>
        /// <param name="StartDate">活动开始时间</param>
        /// <param name="EndDate">活动结束时间</param>
        /// <param name="JoinMode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditActivity(long activeID, string ActivityTitle, string Content, DateTime? StartDate, DateTime? EndDate, string JoinMode)
        {
            LoginInfo user = SessCokie.Get;
            RetResult result = new RetResult();
            JsonResult js = new JsonResult();
            try
            {
                YXLotteryDal dal = new YXLotteryDal();
                YX_ActivitySub model = new YX_ActivitySub();
                model.ActivityID = activeID;
                model.ActivityTitle = ActivityTitle;
                model.Content = Content;
                model.StartDate = StartDate;
                model.EndDate = EndDate;
                model.JoinMode = Convert.ToInt16(JoinMode);
                if (user == null)
                {
                    string msg = "请先登录！";
                    js.Data = new { res = false, info = msg };
                    return js;
                }
                if (model == null)
                {
                    result.Msg = "数据错误";
                }
                else
                {
                    result = dal.EditActive(model);
                }
            }
            catch
            {
                js.Data = new { res = false, info = "出现异常！" };
            }
            if (result.IsSuccess)
            {
                js.Data = new { res = true, info = result.Msg };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }

        public ActionResult EGetActive(long activeID)
        {
            BaseResultModel js = new BaseResultModel();
            RetResult result = new RetResult();
            result = new YXLotteryDal().EGetActive(activeID);
            js = ToJson.NewRetResultToJson((Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
            return Json(js);
        }

        [HttpPost]
        public JsonResult ECompareJXCount(long activeID, string jxSumCount)
        {
            LoginInfo user = SessCokie.Get;
            JsonResult js = new JsonResult();
            YXLotteryDal dal = new YXLotteryDal();
            RetResult result = dal.ECompareJXCount(user.EnterpriseID, activeID, Convert.ToInt64(jxSumCount));
            if (result.IsSuccess)
            {
                js.Data = new { res = true, info = result.Msg };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
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
                else if (model.ActivityMethod == (int)EnumText.ActivityMethod.Packet)
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Market/Wap_IndexMarket/SearchActivity?activityId=" + id + "&settingId=" + settingId;
                }
                else
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Market/Wap_IndexMarket/getLottery?activityId=" + model.ActivityID;
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
    }
}
