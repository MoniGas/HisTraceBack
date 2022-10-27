/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2016-12-09

** 联系方式 :15031109901

** 描述：主要用于仓库信息管理控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using BLL;
using LinqModel.InterfaceModels;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 主要用于仓库信息管理控制器
    /// </summary>
    public class StoreController : Controller
    {
        //
		// GET: /Store/
		StoreBLL StoreBLL = new StoreBLL();
		#region 获取企业仓库接口（用于小程序和其他企业使用）
		[HttpPost]
		public ActionResult storeLst(StoreRequestParam Param)
		{
			InterfaceResult result = new InterfaceResult();
			string accessToken = this.Request.Headers["accessToken"].ToString();
			result = StoreBLL.storeLst(Param, accessToken);
			return Json(result);
		}
		#endregion


		/// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <param name="storeName">仓库名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(string storeName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new StoreBLL().GetList(user.EnterpriseID, storeName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Store.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取仓库模型
        /// </summary>
        /// <param name="id">仓库标识</param>
        /// <returns></returns>
        public JsonResult GetModel(int id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new StoreBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.GetModel";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加仓库
        /// </summary>
        /// <param name="storeName">仓库名称</param>
        /// <param name="storeCode">仓库编码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(string storeName, string storeCode, int type, int id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Store model = new Store();
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                model.StoreName = storeName;
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                model.StoreDate = DateTime.Now;
                if (type == (int)Common.EnumFile.StoreType.Store)
                {
                    model.ParentCode = 0;
                    model.Type = (int)Common.EnumFile.StoreType.Store;
                    model.StoreCode = storeCode;
                    model.EwmUrl = user.MainCode + "." + (int)Common.EnumFile.TerraceEwm.slotting + ".";
                }
                else
                {
                    model.ParentCode = id;
                    model.Type = (int)Common.EnumFile.StoreType.Slotting;
                    model.EwmUrl = user.MainCode + "." + (int)Common.EnumFile.TerraceEwm.slotting + ".";
                }
                model.Status = (int)Common.EnumFile.Status.used;
                result = new StoreBLL().Add(model);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改仓库
        /// </summary>
        /// <param name="storeId">仓库Id</param>
        /// <param name="storeName">仓库名称</param>
        /// <param name="storeCode">仓库编码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(long storeId, string storeName, string storeCode)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                JsonResult js = new JsonResult();
                Store model = new Store();
                model.Store_ID = storeId;
                model.StoreName = storeName;
                if (storeCode != "")
                {
                    model.StoreCode = storeCode;
                }
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                model.StoreDate = DateTime.Now;
                model.Status = (int)Common.EnumFile.Status.used;
                model.Enterprise_Info_ID = user.EnterpriseID;
                result = new StoreBLL().Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new StoreBLL().Del(user.EnterpriseID, id);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.Delete";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="storeId">仓库标识</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult SlottingList(long storeId, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new StoreBLL().GetSlottingList(storeId, user.EnterpriseID, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.SlottingList";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        #region 20170327垛位管理
        /// <summary>
        /// 获取垛位码列表20170327
        /// </summary>
        /// <param name="cribName">垛位名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult GetCribList(string cribName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new StoreBLL().GetCribList(user.EnterpriseID, cribName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Store.GetCribList";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加垛位
        /// </summary>
        /// <param name="storeName">垛位名称</param>
        /// <param name="storeCode">垛位编码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddCrib(string cribName, string cribCode, int type, int id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Store model = new Store();
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                model.StoreName = cribName;
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                model.StoreDate = DateTime.Now;
                model.ParentCode = 0;
                model.Type = (int)Common.EnumFile.StoreType.Crib;
                model.StoreCode = cribCode;
                model.EwmUrl = user.MainCode + "." + (int)Common.EnumFile.TerraceEwm.cribCode + ".";
                model.Status = (int)Common.EnumFile.Status.used;
                result = new StoreBLL().AddCrib(model);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.AddCrib";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改垛位
        /// </summary>
        /// <param name="storeId">垛位Id</param>
        /// <param name="cribName">垛位名称</param>
        /// <param name="cribCode">垛位编码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditCrib(long storeId, string cribName, string cribCode)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                JsonResult js = new JsonResult();
                Store model = new Store();
                model.Store_ID = storeId;
                model.StoreName = cribName;
                if (cribCode != "")
                {
                    model.StoreCode = cribCode;
                }
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                model.StoreDate = DateTime.Now;
                model.Status = (int)Common.EnumFile.Status.used;
                model.Enterprise_Info_ID = user.EnterpriseID;
                result = new StoreBLL().EditCrib(model);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.EditCrib";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除垛位
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <returns></returns>
        public JsonResult DeleteCrib(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new StoreBLL().DelCrib(user.EnterpriseID, id);
            }
            catch (Exception ex)
            {
                string errData = "StoreController.DeleteCrib";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion

        #region 20170412库存查询
        public JsonResult GetInventory(string storeId, string maName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new StoreBLL().GetInventoryInfo(user.EnterpriseID, maName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Store.GetInventory";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult SelectStore(int pageIndex)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                StoreBLL bll = new StoreBLL();
                result = bll.GetList(pf.EnterpriseID, "", pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Store.SelectStore():Store表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
    }
}
