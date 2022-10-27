/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-29

** 修改人：xxx

** 修改时间：xxxx-xx-xx  

** 描述：主要用于企业生成二维码统计的控制器

*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using BLL;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 主要用于统计企业二维码生成数量的控制器
    /// </summary>
    public class SysCodeStatisController : Controller
    {
        //
        // GET: /Admin/SysCodeStatis/
        //
        /// <summary>
        /// 获取二维码数据
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="matype">产品类别</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(string eid,string mlx, string materialName, string beginDate, string endDate, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                ViewCodeStatisBLL bll = new ViewCodeStatisBLL();
                result = bll.GetEnCodeList(Convert.ToInt64(eid), pageIndex,Convert.ToInt32(mlx), materialName, beginDate, endDate + " 23:59:59");
            }
            catch (Exception ex)
            {
                string errData = "SetOrigin.GetOriginList():SetOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
