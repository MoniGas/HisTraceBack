using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_BatchExtController : BaseController
    {
        //
        // GET: /Admin_BatchExt/
        //LoginInfo pf = SessCokie.Get;
        public JsonResult Index(int id)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BatchExtBLL bll = new BatchExtBLL();
                result = bll.GetList(id, "");
            }
            catch (Exception ex)
            {
                string errData = "Admin_BatchExt.Index():View_BatchExt视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult Add(string batchid,string batchExtName)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                BatchExt model = new BatchExt
                {
                    BatchDate = DateTime.Now,
                    BatchExtName = batchExtName,
                    adduser = pf.UserID,
                    AddDate = DateTime.Now,
                    lastuser = pf.UserID
                };
                model.lastdate = model.AddDate;
                model.Batch_ID = Convert.ToInt64(batchid);
                model.Status = (int)Common.EnumFile.Status.used;
                BatchExtBLL bll = new BatchExtBLL();
                result = bll.Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_BatchExt.Add():BatchExt表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
 
        [HttpPost]
        public JsonResult Edit(long id,long bid, string batchExtName)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                BatchExt model = new BatchExt
                {
                    lastuser = pf.UserID,
                    BatchExt_ID = id,
                    Batch_ID = bid,
                    BatchExtName = batchExtName,
                    lastdate = DateTime.Now
                };
                //model.Batch_ID = 141;
                result = new BatchExtBLL().Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_BatchExt.Edit():BatchExt表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Delete(long batchextId)
        {
            BaseResultModel result;
            try
            {
                BatchExtBLL bll = new BatchExtBLL();
                result = bll.Del(batchextId);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_BatchExt.Delete():BatchExt表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(result);
        }
        public JsonResult GetModel(long id)
        {
            BaseResultModel result;
            try
            {
                BatchExtBLL bll = new BatchExtBLL();

                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_BatchExt.GetModel():BatchExt表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
