/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-12-20

** 联系方式 :13313318725

** 描述：生产流程管理控制器 移植

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;

namespace iagric_plant.Controllers
{
    public class AdminProcessController : Controller
    {
        //
        // GET: /AdminProcess/

        /// <summary>
        /// 获取生产流程列表
        /// </summary>
        /// <param name="processName">生产流程名称</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult Index(string processName, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                ProcessBLL bll = new ProcessBLL();
                result = bll.GetList(pf.EnterpriseID, processName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "AdminProcess.Index():Process表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加生产流程
        /// </summary>
        /// <param name="processName">生产流程名称</param>
        /// <param name="operationList">生产环节</param>
        /// <param name="memo">描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(string processName, string operationList, string memo)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Process model = new Process();
                model.EnterpriseID = user.EnterpriseID;
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.ProcessName = processName;
                model.StrOperationList = operationList;
                model.Memo = memo;
                model.status = (int)Common.EnumFile.Status.used;
                ProcessBLL bll = new ProcessBLL();
                result = bll.Add(model);
            }
            catch (Exception ex)
            {
                string errData = "AdminProcess.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改生产流程
        /// </summary>
        /// <param name="processName">生产流程名称</param>
        /// <param name="operationList">生产环节</param>
        /// <param name="memo">描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string processId, string processName, string operationList, string memo)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Process model = new Process();
                model.EnterpriseID = user.EnterpriseID;
                model.ProcessID = Convert.ToInt64(processId);
                model.ProcessName = processName;
                model.StrOperationList = operationList;
                model.Memo = memo;
                ProcessBLL bll = new ProcessBLL();
                result = bll.Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "AdminProcess.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除生产流程
        /// </summary>
        /// <param name="processId">生产流程ID</param>
        /// <returns></returns>
        public JsonResult Delete(long processId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                ProcessBLL bll = new ProcessBLL();
                result = bll.Del(processId);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "AdminProcess.Delete():Process表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 选择生产环节列表
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectOp()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                OperationTypeBLL bll = new OperationTypeBLL();
                result = bll.GetListOp(pf.EnterpriseID, -1);
            }
            catch (Exception ex)
            {
                string errData = "AdminProcess.SelectOp():Process表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 根据生产流程ID获取生产流程信息
        /// </summary>
        /// <param name="id">生产流程ID</param>
        /// <returns></returns>
        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                ProcessBLL bll = new ProcessBLL();
                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "AdminProcess.GetModel():Process表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
