using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using BLL;
using Common.Log;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 主要用于入库统计信息管理控制器
    /// </summary>
    public class IntStorageController : Controller
    {
        //
        // GET: /IntStorage/

        public ActionResult Index(string storeName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new IntStorageBLL().GetList(user.EnterpriseID, storeName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "IntStorage.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 入库详情
        /// </summary>
        /// <param name="oId">入库ID</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult IntStorageDetail(string oId, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new IntStorageBLL().IntStorageDetail(Convert.ToInt64(oId), pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "IntStorage.IntStorageDetail";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
