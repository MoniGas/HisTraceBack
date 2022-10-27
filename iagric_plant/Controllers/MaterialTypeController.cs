/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-10-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx  

** 描述：主要用于产品类型维护的控制器

*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using LinqModel;
using Common.Log;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 产品类型维护的控制器
    /// </summary>
    public class MaterialTypeController : Controller
    {
        //
        // GET: /MaterialType/

        /// <summary>
        /// 获取产品类别列表
        /// </summary>
        /// <returns></returns>
        public JsonResult Index()
        {
            //List<MaterialType> result = new List<MaterialType>();
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialTypeBLL bll = new MaterialTypeBLL();
                result = bll.GetList();
            }
            catch (Exception ex)
            {
                string errData = "MaterialType.Index():MaterialType表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
