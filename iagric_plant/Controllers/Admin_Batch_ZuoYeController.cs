using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_Batch_ZuoYeController : BaseController
    {
        //
        // GET: /Admin_Batch_ZuoYe/

        [HttpPost]
        public JsonResult Index(long batchid, long batchextid, string opid, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                result = new Batch_ZuoYeBLL().GetList(pf.EnterpriseID, batchid, batchextid, Convert.ToInt64(opid), -1, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch_ZuoYe.Index():View_ZuoYeBatchMaterial视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult Add(string batchid, int type, long batch_ZuoYeType_ID, string addDate, string content, string batchextid, string files, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch_ZuoYe model = new Batch_ZuoYe
                {
                    Enterprise_Info_ID = pf.EnterpriseID,
                    Batch_ID = Convert.ToInt64(batchid),
                    type = type,
                    Status = (int) Common.EnumFile.Status.used,
                    UserName = pf.UserName,
                    UserCode = pf.UserID.ToString(),
                    Batch_ZuoYeType_ID = batch_ZuoYeType_ID,
                    Content = content,
                    BatchExt_ID = Convert.ToInt64(batchextid),
                    StrFiles = files,// 图片视频
                    adduser = pf.UserID,
                    AddDate = Convert.ToDateTime(addDate),
                    lastuser = pf.UserID
                };

                model.lastdate = model.AddDate;

                result = new Batch_ZuoYeBLL().Add(model, video);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_ZuoYe.Add():Batch_ZuoYe表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult AddWork(string settingid, long batch_ZuoYeType_ID, string addDate, string content, long teamID, string usersArray, string files, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch_ZuoYe model = new Batch_ZuoYe
                {
                    Enterprise_Info_ID = pf.EnterpriseID,
                    SettingID = Convert.ToInt64(settingid),
                    //type = type,
                    Status = (int) Common.EnumFile.Status.used,
                    UserCode = pf.UserID.ToString(),
                    Batch_ZuoYeType_ID = batch_ZuoYeType_ID,
                    Content = content,
                    TeamID = teamID,
                    UsersName = usersArray,
                    StrFiles = files,// 图片视频
                    adduser = pf.UserID,
                    AddDate = Convert.ToDateTime(addDate),
                    lastuser = pf.UserID
                };
                
                model.lastdate = model.AddDate;

                result = new Batch_ZuoYeBLL().Add(model, video);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_ZuoYe.Add():Batch_ZuoYe表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult Edit(long id, int type, long batch_ZuoYeType_ID, string addDate, string content,string files, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch_ZuoYe model = new Batch_ZuoYe
                {
                    Enterprise_Info_ID = pf.EnterpriseID,
                    Batch_ZuoYe_ID = id,
                    UserName = pf.UserName,
                    UserCode = pf.UserID.ToString(),
                    lastdate = DateTime.Now,
                    lastuser = pf.UserID,
                    type = type,
                    Batch_ZuoYeType_ID = batch_ZuoYeType_ID,
                    AddDate = Convert.ToDateTime(addDate),
                    Content = content,
                    StrFiles = files
                };

                result = new Batch_ZuoYeBLL().Edit(model, video);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_ZuoYe.Edit():Batch_ZuoYe表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult EditWork(long id, long batch_ZuoYeType_ID,string addDate, string content, long teamID, string usersArray, string files, string video)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch_ZuoYe model = new Batch_ZuoYe
                {
                    Batch_ZuoYe_ID = id,
                    UserCode = pf.UserID.ToString(),
                    lastdate = DateTime.Now,
                    lastuser = pf.UserID,
                    //type = type,
                    Batch_ZuoYeType_ID = batch_ZuoYeType_ID,
                    AddDate = Convert.ToDateTime(addDate),
                    Content = content,
                    TeamID = teamID,
                    UsersName = usersArray,
                    StrFiles = files
                };

                result = new Batch_ZuoYeBLL().Edit(model, video);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_ZuoYe.Edit():Batch_ZuoYe表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Delete(long id)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                result = new Batch_ZuoYeBLL().Del(id, pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_ZuoYe.Delete():Batch_ZuoYe表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public ActionResult GetModelZY(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                Batch_ZuoYeBLL bll = new Batch_ZuoYeBLL();

                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch_ZuoYe.GetModelZY():Batch_ZuoYe表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        #region 获取生产/加工类型列表
        [HttpPost]
        public JsonResult OpTypeList(int selecttype)
        {
            LoginInfo pf = SessCokie.Get;
            //List<Batch_ZuoYeType> result = new List<Batch_ZuoYeType>();
            BaseResultList result = new BaseResultList();
            try
            {
                OperationTypeBLL bll = new OperationTypeBLL();
                result = bll.GetListOp(pf.EnterpriseID, selecttype);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch_ZuoYe.OpTypeList():Batch_ZuoYeType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult OpTypeListAll()
        {
            LoginInfo pf = SessCokie.Get;
            //List<Batch_ZuoYeType> result = new List<Batch_ZuoYeType>();
            BaseResultList result = new BaseResultList();
            try
            {
                OperationTypeBLL bll = new OperationTypeBLL();
                result = bll.GetListOp(pf.EnterpriseID, -1);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch_ZuoYe.OpTypeListAll():Batch_ZuoYeType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
    }
}
