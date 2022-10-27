using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_Batch_JianYanJianYiController : BaseController
    {
        //
        // GET: /Admin_Batch_JianYanJianYi/

        LoginInfo pf = Common.Argument.SessCokie.Get;
        public JsonResult Index(long batchid, long batchextid, int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                Batch_JianYanJianYiBLL jianyanjianyiBLL = new Batch_JianYanJianYiBLL();
                result = jianyanjianyiBLL.GetList(pf.EnterpriseID, batchid, batchextid, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch_JianYanJianYi.Index():View_JianYanJianYiBatchMaterial视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 添加检测报告
        /// </summary>
        /// <param name="batchid">批次ID</param>
        /// <param name="addDate"></param>
        /// <param name="content"></param>
        /// <param name="files"></param>
        /// <param name="batchextid">子批次ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(string batchid, string addDate, string content, string files, string batchextid)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Batch_JianYanJianYi model = new Batch_JianYanJianYi();
                //model.Enterprise_Info_ID = pf.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                model.Enterprise_Info_ID = pf.EnterpriseID;
                model.Batch_ID = Convert.ToInt64(batchid);
                model.Content = content;
                model.BatchExt_ID = Convert.ToInt64(batchextid);
                model.StrFiles = files;
                model.UserName = pf.UserName;
                model.UserCode = pf.UserID.ToString();
                model.adduser = pf.UserID;
                model.AddDate = Convert.ToDateTime(addDate);
                model.lastuser = pf.UserID;
                model.lastdate = model.AddDate;
                Batch_JianYanJianYiBLL jianyanjianyiBLL = new Batch_JianYanJianYiBLL();
                result = jianyanjianyiBLL.Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_JianYanJianYi.Add():Batch_JianYanJianYi表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult AddReport(string settingId, string addDate, string content, string files)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Batch_JianYanJianYi model = new Batch_JianYanJianYi();
                model.SettingID = Convert.ToInt64(settingId);
                model.Status = (int)Common.EnumFile.Status.used;
                model.Enterprise_Info_ID = pf.EnterpriseID;
                model.Content = content;
                model.StrFiles = files;
                model.UserName = pf.UserName;
                model.UserCode = pf.UserID.ToString();
                model.adduser = pf.UserID;
                model.AddDate = Convert.ToDateTime(addDate);
                model.lastuser = pf.UserID;
                model.lastdate = model.AddDate;
                Batch_JianYanJianYiBLL jianyanjianyiBLL = new Batch_JianYanJianYiBLL();
                result = jianyanjianyiBLL.Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_JianYanJianYi.Add():Batch_JianYanJianYi表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Edit(long id)
        {
            //long batchId = Convert.ToInt64(Request["batchid"]);
            //ViewBag.batchid = batchId;
            //LoginInfo pf = Common.Argument.SessCokie.Get;
            Batch_JianYanJianYiBLL jianyanjianyiBLL = new Batch_JianYanJianYiBLL();
            BaseResultModel result = jianyanjianyiBLL.GetModelView(id);
            //View_JianYanJianYiBatchMaterial model = new Dal.Batch_jianYanJianYiDAL().GetModelView(id);
            return Json(result);

        }
        [HttpPost]
        public JsonResult Edit(long id, string addDate, string content, string files)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Batch_JianYanJianYi model = new Batch_JianYanJianYi();
                model.UserName = pf.UserName;
                model.UserCode = pf.UserID.ToString();
                //model.Batch_ID =Convert.ToInt64(batchid);
                model.Batch_JianYanJianYi_ID = id;
                model.Content = content;
                model.AddDate = Convert.ToDateTime(addDate);
                model.StrFiles = files;
                model.lastdate = DateTime.Now;
                model.lastuser = pf.UserID;
                Batch_JianYanJianYiBLL jianyanjianyiBLL = new Batch_JianYanJianYiBLL();
                result = jianyanjianyiBLL.Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_JianYanJianYi.Edit():Batch_JianYanJianYi表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult EditReport(long id, string addDate, string content, string files)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Batch_JianYanJianYi model = new Batch_JianYanJianYi();
                model.UserName = pf.UserName;
                model.UserCode = pf.UserID.ToString();
                model.Batch_JianYanJianYi_ID = id;
                model.Content = content;
                model.AddDate = Convert.ToDateTime(addDate);
                model.StrFiles = files;
                model.lastdate = DateTime.Now;
                model.lastuser = pf.UserID;
                Batch_JianYanJianYiBLL jianyanjianyiBLL = new Batch_JianYanJianYiBLL();
                result = jianyanjianyiBLL.Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_JianYanJianYi.Edit():Batch_JianYanJianYi表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 删除检测报告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(long id)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Batch_JianYanJianYiBLL jianyanjianyibll = new Batch_JianYanJianYiBLL();
                result = jianyanjianyibll.Del(id, pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_JianYanJianYi.Delete():Batch_JianYanJianYi表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public ActionResult GetModelJC(long id)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Batch_JianYanJianYiBLL bll = new Batch_JianYanJianYiBLL();

                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_JianYanJianYi.GetModelJC():Batch_JianYanJianYi表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(result);
        }
    }
}
