/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-4

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于生产单元信息管理控制器

*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_GreenhouseController : BaseController
    {
        //
        // GET: /Admin_Greenhouse/

        public JsonResult Index(int pageIndex, string greenName, string greenewm)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList strResult = new BaseResultList();
            try
            {
                GreenhouseBLL objGreenhouseBLL = new GreenhouseBLL();
                strResult = objGreenhouseBLL.GetList(pf.EnterpriseID, greenewm, greenName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_GreenhouseController.Index():Greenhouses表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        #region 获取生产单元列表
        [HttpPost]
        public JsonResult GreenHouseList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            GreenhouseBLL bll = new GreenhouseBLL();
            BaseResultList result = new BaseResultList();
            try
            {
                result = bll.GetMList(pf.EnterpriseID);
            }
            catch
            { }
            return Json(result);
        }
        #endregion
        /// <summary>
        /// 根据ID获取生产基地信息
        /// </summary>
        /// <param name="id">生产基地ID</param>
        /// <returns></returns>
        public ActionResult SearchData(long id)
        {
            GreenhouseBLL objGreenhouseBLL = new GreenhouseBLL();

            LinqModel.BaseResultModel objResult = objGreenhouseBLL.SearchData(id);

            return Json(objResult);
        }

        public ActionResult Add(string name)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel strResult = new BaseResultModel();
            try
            {
                Greenhouses greenhouses = new Greenhouses();
                greenhouses.memo = Request["WebContent"];
                greenhouses.Enterprise_Info_ID = pf.EnterpriseID;
                greenhouses.adduser = pf.UserID;
                greenhouses.adddate = DateTime.Now;
                greenhouses.lastuser = pf.UserID;
                greenhouses.lastdate = greenhouses.adddate;
                greenhouses.EWM = pf.MainCode + "." + (int)Common.EnumFile.TerraceEwm.greenHouse + ".";
                greenhouses.GreenhousesName = name;
                greenhouses.state = (int)EnumFile.Status.used;
                GreenhouseBLL objGreenhouseBLL = new GreenhouseBLL();
                strResult = objGreenhouseBLL.Add(greenhouses);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_GreenhouseController.Add():Greenhouses表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        public JsonResult ShowTypeEWM(long id)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;

            OperationTypeBLL objOperationTypeBLL = new OperationTypeBLL();
            GreenhouseBLL objGreenhouseBLL = new GreenhouseBLL();
            BaseResultList strResult = objOperationTypeBLL.GetList(pf.EnterpriseID, 1, "");
            return Json(strResult);
        }

        public ActionResult Edit(long id, string name)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel strResult = new BaseResultModel();
            try
            {
                Greenhouses greenhouses = new Greenhouses();
                greenhouses.Greenhouses_ID = id;
                greenhouses.GreenhousesName = name;
                greenhouses.memo = Request["WebContent"];
                greenhouses.Enterprise_Info_ID = pf.EnterpriseID;
                greenhouses.lastdate = DateTime.Now;
                greenhouses.lastuser = 1;
                GreenhouseBLL objGreenhouseBLL = new GreenhouseBLL();
                strResult = objGreenhouseBLL.Edit(greenhouses);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_GreenhouseController.Edit():Greenhouses表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        public ActionResult Del(long operationTypeId)
        {
            BaseResultModel strResult = new BaseResultModel();
            try
            {
                GreenhouseBLL objGreenhouseBLL = new GreenhouseBLL();
                strResult = objGreenhouseBLL.Del(operationTypeId);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_GreenhouseController.Del():Greenhouses表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
    }
}
