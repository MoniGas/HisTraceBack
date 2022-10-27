/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-13

** 联系方式 :13313318725

** 描述：主要用于解析设置控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using Common.Log;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 主要用于解析设置控制器
    /// </summary>
    public class SysAnalysisController : Controller
    {
        //
        // GET: /Admin/SysAnalysis/

        /// <summary>
        /// 通过企业id获取可以解析的产品
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="eId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="IsAnalyse">解析类型：1是未停止解析,2是已停止解析</param>
        /// <returns></returns>
        public JsonResult Index(string name, long id, int? pageIndex, string IsAnalyse)
        {
            SysAnalysisBLL bll = new SysAnalysisBLL();
            LinqModel.BaseResultList dataList = bll.GetAnalysisList(name, id, pageIndex, IsAnalyse);
            return Json(dataList);
        }

        /// <summary>
        /// 通过企业id获取企业名称
        /// </summary>
        /// <param name="id">企业标识</param>
        /// <returns></returns>
        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                SysAnalysisBLL bll = new SysAnalysisBLL();
                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "SysAnalysis.GetModel():View_Analysis表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 设置企业产品解析
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="materialIdArray">获取复选框（产品）id字符串,例如：1,2,3</param>
        /// <param name="IsAnalyse">解析类型：1是未停止解析,2是已停止解析</param>
        /// <returns></returns>
        public JsonResult SetAnalysis(long eId, string materialIdArray, string IsAnalyse)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                Analysis model = new Analysis();               
                model.Enterprise_Info_ID = eId;
                result = new SysAnalysisBLL().SetAnalysis(model, materialIdArray, IsAnalyse);
            }
            catch (Exception ex)
            {
                string errData = "SysAnalysisController.SetAnalysis";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

    }
}
