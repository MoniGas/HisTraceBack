using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_BatchController : BaseController
    {
        //
        // GET: /Admin_Batch/


        public JsonResult Index(string searchName, string mName, string bName, string beginDate, string endDate, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BatchBLL bll = new BatchBLL();
                result = bll.GetList(pf.EnterpriseID, searchName, mName, bName, beginDate, endDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch.Index():View_Batch视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Add(string materialId, string batchName, string greenhousesId)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch batch = new Batch
                {
                    Enterprise_Info_ID = pf.EnterpriseID,
                    Status = (int) Common.EnumFile.Status.used,
                    BatchName = batchName,
                    Material_ID = Convert.ToInt64(materialId),
                    BatchCount = 0,
                    BatchDate = DateTime.Now,
                    adduser = pf.UserID,
                    adddate = DateTime.Now,
                    lastuser = pf.UserID
                };
                //batch.validation = (int)Common.EnumFile.Validation.defaultt;
                batch.lastdate = batch.adddate;
                Greenhouses_Batch ghbatch = new Greenhouses_Batch {Greenhouses_ID = Convert.ToInt64(greenhousesId)};
                result = new BatchBLL().Add(batch, ghbatch);
                //更新用户状态，以判断是否加载引导页
                if (result.code == "0")
                {
                    pf.NewUser = false;
                    SessCokie.Set(pf);
                }
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch.Add():Batch表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult Edit(long id, long materialId, string batchName, long greenhousesId)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                Batch batch = new Batch
                {
                    Batch_ID = id,
                    Status = (int) Common.EnumFile.Status.used,
                    BatchCount = 0,
                    BatchName = batchName,
                    Material_ID = materialId,
                    BatchDate = DateTime.Now,
                    Enterprise_Info_ID = pf.EnterpriseID,
                    lastuser = pf.UserID,
                    lastdate = DateTime.Now
                };
                Greenhouses_Batch ghbatch = new Greenhouses_Batch {Greenhouses_ID = greenhousesId};
                result = new BatchBLL().Edit(batch, ghbatch);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch.Edit():Batch表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Del(long id)
        {
            BaseResultModel result;
            try
            {
                result = new BatchBLL().Del(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Batch.Del():Batch表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public ActionResult GetModelB(long id)
        {
            BatchBLL bll = new BatchBLL();

            string result = bll.GetModel(id);

            return Json(result);
        }

        #region 获取产品列表
        [HttpPost]
        public JsonResult MaterialList()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialBLL bll = new MaterialBLL();
                result = bll.GetMList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch.MaterialList():Material表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);

        }
        #endregion

        #region 获取产品列表（引导页用）
        [HttpPost]
        public JsonResult GuidMaterialList(string brandid)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialBLL bll = new MaterialBLL();
                result = bll.GetGMList(pf.EnterpriseID, brandid);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch.MaterialList():Material表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);

        }
        #endregion

        #region 获取生产单元列表
        [HttpPost]
        public JsonResult GreenHouseList()
        {
            LoginInfo pf = SessCokie.Get;
            GreenhouseBLL bll = new GreenhouseBLL();
            BaseResultList result = new BaseResultList();
            try
            {
                result = bll.GetList(pf.EnterpriseID, "", "", 1);
            }
            catch(Exception ex)
            {
                string errData = "Admin_Batch.GreenHouseList():Greenhouses表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion

        #region 获取产品规格列表
        [HttpPost]
        public JsonResult MaterialSpecList()
        {
            LoginInfo pf = SessCokie.Get;
            MaterialSpcificationBLL bll = new MaterialSpcificationBLL();
            BaseResultList result = new BaseResultList();
            try
            {
                result = bll.GetListMaS(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch.MaterialSpecList():MaterialSpcification表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
    }
}
