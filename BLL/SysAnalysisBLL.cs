/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-13

** 联系方式 :13313318725

** 描述：主要用于解析设置业务逻辑层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using Dal;
using Common.Argument;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 主要用于解析设置业务逻辑层
    /// </summary>
    public class SysAnalysisBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 通过企业id获取可以解析的产品
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="eId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="IsAnalyse">解析类型：1是未停止解析,2是已停止解析</param>
        /// <returns></returns>
        public BaseResultList GetAnalysisList(string name, long eId, int? pageIndex, string IsAnalyse)
        {
            SysAnalysisDAL dal = new SysAnalysisDAL();
            long totalCount = 0;
            List<LinqModel.View_Analysis> dataList = dal.GetAnalysisList(name, eId, pageIndex, out totalCount,IsAnalyse);

            return ToJson.NewListToJson(dataList, pageIndex.Value, _PageSize, totalCount, "");
        }

        /// <summary>
        /// 通过企业id获取企业名称
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long eId)
        {
            SysAnalysisDAL dal = new SysAnalysisDAL();
            View_Analysis model = dal.GetModel(eId);          
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        /// <summary>
        /// 设置企业产品解析
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="materialIdArray">获取复选框（产品）id字符串,例如：1,2,3</param>
        /// <param name="IsAnalyse">解析类型：1是未停止解析,2是已停止解析</param>
        /// <returns></returns>
        public BaseResultModel SetAnalysis(Analysis model, string materialIdArray, string IsAnalyse)
        {
            SysAnalysisDAL dal = new SysAnalysisDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(materialIdArray))
            {
                ret.Msg = "请至少选择一条数据！";
            }
            else
            {
                ret = dal.SetAnalysis(model, materialIdArray, IsAnalyse);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 根据企业信息和产品信息查询产品解析
        /// </summary>
        /// <param name="codeInfo"></param>
        /// <returns></returns>
        public Analysis GetAnalysis(CodeInfo codeInfo)
        {
            SysAnalysisDAL dal = new SysAnalysisDAL();
            return dal.GetAnalysis(codeInfo);
        }
    }      
}
