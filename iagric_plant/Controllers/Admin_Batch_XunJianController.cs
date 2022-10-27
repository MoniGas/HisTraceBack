using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_Batch_XunJianController : BaseController
    {
        //
        // GET: /Admin_Batch_XunJian/

        //LoginInfo pf = SessCokie.Get;
        public JsonResult Index(long batchid, long batchextid, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                Batch_XunJianBLL bll = new Batch_XunJianBLL();
                result = bll.GetList(pf.EnterpriseID, batchid, batchextid, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch_XunJian.Index():View_XunJianBatchMaterial视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult Add(string batchid, string addDate, string content, string files, string batchextid, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                Batch_XunJian model = new Batch_XunJian
                {
                    Enterprise_Info_ID = pf.EnterpriseID,
                    Status = (int) Common.EnumFile.Status.used,
                    UserName = pf.UserName,
                    UserCode = "",
                    Batch_ID = Convert.ToInt64(batchid),
                    AddDate = Convert.ToDateTime(addDate),
                    Content = content,
                    StrFiles = files, //图片 视频
                    BatchExt_ID = Convert.ToInt64(batchextid),
                    adduser = pf.UserID,
                    lastuser = pf.UserID
                };
                //JsonResult js = new JsonResult();
               
                model.lastdate = model.AddDate;
                strResult = new Batch_XunJianBLL().Add(model, video);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_XunJian.Add():Batch_XunJian表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        public JsonResult AddCheck(string settingId, string userName, string addDate, string content, string files, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                Batch_XunJian model = new Batch_XunJian
                {
                    Enterprise_Info_ID = pf.EnterpriseID,
                    Status = (int) Common.EnumFile.Status.used,
                    UserName = userName,
                    UserCode = "",
                    AddDate = Convert.ToDateTime(addDate),
                    Content = content,
                    StrFiles = files, //图片 视频
                    SettingID = Convert.ToInt64(settingId),
                    adduser = pf.UserID,
                    lastuser = pf.UserID
                };
                //JsonResult js = new JsonResult();

                model.lastdate = model.AddDate;
                strResult = new Batch_XunJianBLL().Add(model, video);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_XunJian.Add():Batch_XunJian表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        public ActionResult Edit(long id)
        {
            //long batchId = Convert.ToInt64(Request["batchid"]);
            //ViewBag.batchid = batchId;
            //LoginInfo pf = Common.Argument.SessCokie.Get;
            //string strResult = new Batch_XunJianBLL().GetModelView(id);
            //return Json(strResult);
            return View();
        }
        [HttpPost]
        public JsonResult Edit(long id, string addDate, string content, string files, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch_XunJian model = new Batch_XunJian
                {
                    UserName = pf.UserName,
                    UserCode = pf.UserID.ToString(),
                    Batch_XunJian_ID = id,
                    AddDate = Convert.ToDateTime(addDate),
                    Content = content,
                    StrFiles = files,
                    lastdate = DateTime.Now,
                    lastuser = pf.UserID
                };
                //model.Files = files;
                result = new Batch_XunJianBLL().Edit(model, video);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_XunJian.Edit():Batch_XunJian表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult EditCheck(long id, string userName, string addDate, string content, string files, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch_XunJian model = new Batch_XunJian
                {
                    UserName = userName,
                    Batch_XunJian_ID = id,
                    AddDate = Convert.ToDateTime(addDate),
                    Content = content,
                    StrFiles = files,
                    lastdate = DateTime.Now,
                    lastuser = pf.UserID
                };
                result = new Batch_XunJianBLL().Edit(model, video);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_XunJian.Edit():Batch_XunJian表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Del(long id)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                result = new Batch_XunJianBLL().Del(id, pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_XunJian.Del():Batch_XunJian表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public ActionResult GetModelXJ(string id)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch_XunJianBLL bll = new Batch_XunJianBLL();
                long xjid = Convert.ToInt64(id);
                result = bll.GetModel(xjid);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_XunJian.GetModelXJ():Batch_XunJian表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
