/********************************************************************************

** 作者： 张翠霞

** 创始时间：2017-05-05

** 联系方式 :13313318725

** 描述：主要用于出库统计信息管理控制器

** 版本：v2.5

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using BLL;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 主要用于出库统计信息管理控制器
    /// </summary>
    public class OutStorageController : Controller
    {
        //
        // GET: /OutStorage/

        /// <summary>
        /// 出库统计列表
        /// </summary>
        /// <param name="storeName">相关名称搜索</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult Index(string storeName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new OutStorageBLL().GetList(user.EnterpriseID, storeName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "OutStorage.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 出库详情
        /// </summary>
        /// <param name="oId">出库ID</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult OutStorageDetail(string oId, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new OutStorageBLL().OutStorageDetail(Convert.ToInt64(oId), pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "OutStorage.OutStorageDetail";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
