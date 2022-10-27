using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_OperationTypeController : BaseController
    {
        //
        // GET: /Admin_OperationType/

        private readonly OperationTypeBLL _objOperationTypeBll = new OperationTypeBLL();

        public JsonResult Index(string operationName, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get ?? new LoginInfo();
            BaseResultList strResult = new BaseResultList();
            try
            {
                string operationType = Request["operationType"] ?? "-1";
                strResult = _objOperationTypeBll.GetList(user.EnterpriseID, pageIndex, operationName, Convert.ToInt32(operationType));
            }
            catch (Exception ex)
            {
                string errData = "Admin_OperationTypeController.Index():Batch_ZuoYeType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        /// <summary>
        /// 根据ID获取生产环节信息
        /// </summary>
        /// <param name="id">生产环节ID</param>
        /// <returns></returns>
        public JsonResult SearchData(long id)
        {
            BaseResultModel objResult;
            try
            {
                objResult = _objOperationTypeBll.SearchData(id);
            }
            catch (Exception ex)
            {
                objResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_OperationTypeController.SearchData():Batch_ZuoYeType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(objResult);
        }

        public JsonResult Add(string name, string memo)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                Batch_ZuoYeType operationType = new Batch_ZuoYeType();
                operationType.Enterprise_Info_ID = pf.EnterpriseID;
                operationType.adduser = pf.UserID;
                operationType.adddate = DateTime.Now;
                operationType.lastuser = pf.UserID;
                operationType.lastdate = operationType.adddate;
                //operationType.type = 0;
                operationType.Memo = memo;
                operationType.OperationTypeName = name ?? "";
                operationType.state = 0;
                strResult = _objOperationTypeBll.Add(operationType);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_OperationTypeController.Add():Batch_ZuoYeType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        public JsonResult Edit(long id, string name, string memo)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                Batch_ZuoYeType model = new Batch_ZuoYeType();
                model.Enterprise_Info_ID = pf.EnterpriseID;
                model.lastuser = pf.UserID;
                model.lastdate = DateTime.Now;
                model.Batch_ZuoYeType_ID = id;
                model.Memo = memo;
                model.OperationTypeName = name;
                model.state = 0;
                strResult = _objOperationTypeBll.Edit(model);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_OperationTypeController.Edit():Batch_ZuoYeType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        public ActionResult Del(string id)
        {
            BaseResultModel result;
            try
            {
                result = _objOperationTypeBll.Del(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_OperationTypeController.Del():Batch_ZuoYeType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
