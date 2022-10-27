/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-24

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 主要用于设置监管部门审核码数量控制器层

*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using Common.Log;
using BLL;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysSetAuditCountController : Controller
    {
        //
        // GET: /Admin/SysSetAuditCount/

        /// <summary>
        /// 设置监管部门审核码数量
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult Index(string pid, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                SetAuditCountBLL bll = new SetAuditCountBLL();
                result = bll.GetList(pf.EnterpriseID, Convert.ToInt64(pid), pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "SetAuditCount.Index():SetAuditCount表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        
        /// <summary>
        /// 获取监管部门审核码数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetModelCount(long pid)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                SetAuditCountBLL bll = new SetAuditCountBLL();
                result = bll.GetModel(pid);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "SetAuditCount.GetModelCount():SetAuditCount表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 设置审核码数量
        /// </summary>
        /// <param name="pid">监管部门ID</param>
        /// <param name="zhuijiaCode">数量</param>
        /// <returns></returns>
        public JsonResult Add(string pid, string zhuijiaCode)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                SetAuditCount model = new SetAuditCount();
                model.AuditCountAll =Convert.ToInt64(zhuijiaCode);
                model.PRRU_PlatForm_ID = Convert.ToInt64(pid);
                model.PRRU_PlatFormUP_ID = pf.EnterpriseID;
                model.SetTime = DateTime.Now;
                SetAuditCountBLL bll = new SetAuditCountBLL();
                result = bll.Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "SetImg.Add():SetImg表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
