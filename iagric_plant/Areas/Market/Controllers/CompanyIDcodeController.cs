/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-7-10
** 联系方式:15533621896
** 代码功能：我的二维码
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using MarketActive.Filter;
using Webdiyer.WebControls.Mvc;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 我的二维码
    /// </summary>
    [AdminAuthorize]
    public class CompanyIDcodeController : Controller
    {
        /// <summary>
        /// 二维码关联活动
        /// </summary>
        /// <returns></returns>
        public ActionResult RelationActivity(long companyIDcodeID = 0)
        {
            var modelLst = new CompanyIDcodeBLL().GetEwmRelationActivity(SessCokie.Get.EnterpriseID);
            ViewBag.CompanyIDcodeModel = new CompanyIDcodeBLL().GetModel(companyIDcodeID);
            return View(modelLst);
        }

        /// <summary>
        /// 二维码关联活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RelationActivity(YX_ActvitiyRelationCode model, long code)
        {
            model.RelationDate = DateTime.Now;
            //model.UseState = code == model.EndCode - model.StartCode + 1 ? (int)Common.EnumText.UserState.Used : (int)Common.EnumText.UserState.UserPart;
            model.CompanyIDcode = SessCokie.Get.MainCode;
            model.CompanyID = SessCokie.Get.EnterpriseID;
            RetResult result = new CompanyIDcodeBLL().AddRelationActivity(model);
            return Json(new { ok = result.IsSuccess, msg = result.Msg });
        }

        /// <summary>
        /// 获取活动
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public JsonResult GetActivityModel(long activityId, long companyIDcodeID)
        {
            var model = new CompanyIDcodeBLL().GetModel(companyIDcodeID);
            long? code = new CompanyIDcodeBLL().GetStartCode(companyIDcodeID) ?? 0;
            if (model == null || model.CodeCount == 0)
            {

            }
            return Json(new { startCode = code + 1, endCode = model.EndCode, model = new CompanyIDcodeBLL().GetRelationActivityEwmModel(activityId) });
        }

        /// <summary>
        /// 订单
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public ActionResult GetOrder(long companyIDcodeID)
        {
            var model = new CompanyIDcodeBLL().GetOrderModel(companyIDcodeID);
            return View(model);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public ActionResult GetDetail(long companyIDcodeID)
        {
            var model = new CompanyIDcodeBLL().GetOrderDetail(companyIDcodeID);
            return View(model);
        }

        #region 获取企业用码情况
        /// <summary>
        /// 获取企业用码情况
        /// </summary>
        /// <param name="id">分页</param>
        /// <returns></returns>
        public ActionResult GetEnUsedCode(int? id = 1)
        {
            LoginInfo user = SessCokie.Get;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            PagedList<View_UsedCodeSituation> dataList = bll.GetUsedCodeSituation(user.EnterpriseID, sDate, eDate, id);
            return View(dataList);
        }

        /// <summary>
        /// 获取企业续码记录
        /// </summary>
        /// <param name="id">分页</param>
        /// <returns></returns>
        public ActionResult GetEnContinneCodeRecord(int? id = 1)
        {
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            LoginInfo user = SessCokie.Get;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            PagedList<ContinneCodeRecord> dataList = bll.GetEnContinneCodeRecord(user.EnterpriseID, sDate, eDate, id);
            return View(dataList);
        }
        #endregion
    }
}
