/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于投诉管理控制器

*********************************************************************************/
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using System;
using Common.Log;
using System.Configuration;

namespace iagric_plant.Controllers
{
    public class ComplaintController : Controller
    {
        /// <summary>
        /// 获取投诉管理未读信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public JsonResult GetList()
        {
            LoginInfo pf = SessCokie.Get;
            ComplaintBLL objComplaintBll = new ComplaintBLL();
            return Json(objComplaintBll.GetList(pf.EnterpriseID));
        }

        public ActionResult Index(string search, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList result = new ComplaintBLL().GetList(user.EnterpriseID, search, pageIndex);
            return Json(result);
        }
        /// <summary>
        /// 修改投诉信息为已读
        /// </summary>
        /// <param name="id">投诉列表ID</param>
        /// <returns></returns>
        public JsonResult UpdateStatus(long id)
        {
            ComplaintBLL objComplaintBll = new ComplaintBLL();
            return Json(objComplaintBll.UpdateStatus(id));
        }
        public ActionResult Delete(long id)
        {
            BaseResultModel result = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            try
            {
                result = new ComplaintBLL().Del(user.EnterpriseID, id);
            }
            catch
            { }
            return Json(result);
        }

        /// <summary>
        /// 获取主页统计数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetHomeDataStatis()
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                ComplaintBLL bll = new ComplaintBLL();
               // string logoUrl = string.Empty;
                result = bll.GetHomeDataStatis(user.EnterpriseID);
                result.Msg = user.EnterpriseName;
            }
            catch (Exception ex)
            {
                string errData = "ComplaintController.GetHomeDataStatis";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 从配置文件中查找升级数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUpdate()
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                string update = ConfigurationManager.AppSettings["Update"];
                result.code = "1";
                result.Msg = update;
            }
            catch
            {
                result.code = "0";
                result.Msg = "";
            }
            return Json(result);
        }

        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChartData()
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                ComplaintBLL bll = new ComplaintBLL();
                result = bll.GetChartData(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "ComplaintController.GetChartData";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
