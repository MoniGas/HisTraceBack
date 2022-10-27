/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-08-02

** 联系方式 :13313318725

** 描述：产品规格管理控制器

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
    public class Admin_MaterialSpcificationController : BaseController
    {
        //
        // GET: /Admin_MaterialSpcification/

        /// <summary>
        /// 产品规格管理控制器
        /// </summary>
        /// <returns></returns>
        public JsonResult Index(string spection,int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialSpcificationBLL bll = new MaterialSpcificationBLL();
                result = bll.GetList(pf.EnterpriseID,spection, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialSpcification.Index():MaterialSpcification表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加产品规格post提交
        /// </summary>
        /// <param name="maSName">产品规格</param>
        /// <param name="maSCode">规格码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(string Value, string maSName, string maSCode)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                MaterialSpcification msModel = new MaterialSpcification();
                msModel.Enterprise_Info_ID = pf.EnterpriseID;
                msModel.Value = Convert.ToDecimal(Value);
                msModel.MaterialSpcificationName = maSName;
                msModel.MaterialSpcificationCode = maSCode;
                msModel.AddDate = DateTime.Now;
                msModel.AddUser = pf.UserID;
                msModel.Status = (int)Common.EnumFile.Status.used;
                MaterialSpcificationBLL bll = new MaterialSpcificationBLL();
                result = bll.Add(msModel);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_MaterialSpcification.Add():MaterialSpcification表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改产品规格（post提交）
        /// </summary>
        /// <param name="id">产品规格ID</param>
        /// <param name="maSName">产品规格</param>
        /// <param name="maSCode">规格码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string id, string Value, string maSName, string maSCode)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                MaterialSpcification msModel = new MaterialSpcification();
                msModel.Enterprise_Info_ID = pf.EnterpriseID;
                msModel.MaterialSpcificationID = Convert.ToInt64(id);
                msModel.Value = Convert.ToDecimal(Value);
                msModel.MaterialSpcificationName = maSName;
                msModel.MaterialSpcificationCode = maSCode;
                MaterialSpcificationBLL bll = new MaterialSpcificationBLL();
                result = bll.Edit(msModel);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_MaterialSpcification.Edit():MaterialSpcification表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除产品规格
        /// </summary>
        /// <param name="maSId">产品规格ID</param>
        /// <returns></returns>
        public JsonResult Delete(long maSId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                MaterialSpcificationBLL bll = new MaterialSpcificationBLL();
                result = bll.Del(maSId);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_MaterialSpcification.Delete():MaterialSpcification表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 根据产品规格ID获取产品规格信息
        /// </summary>
        /// <param name="id">产品规格ID</param>
        /// <returns></returns>
        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                MaterialSpcificationBLL bll = new MaterialSpcificationBLL();
                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_MaterialSpcification.GetModel():MaterialSpcification表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
