/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-13

** 联系方式 :13313318725

** 描述：主要用于解析设置管理数据层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 主要用于解析设置管理数据层
    /// </summary>
    public class SysAnalysisDAL : DALBase
    {
        /// <summary>
        /// 通过企业id获取可以解析的产品
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="eId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="IsAnalyse">解析类型：1是未停止解析,2是已停止解析</param>
        /// <returns></returns>
        public List<View_Analysis> GetAnalysisList(string name, long eId, int? pageIndex, out long totalCount, string IsAnalyse)
        {
            List<LinqModel.View_Analysis> dataInfoList = null;
            totalCount = 0;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_Analysis
                                   where data.Status == 0
                                   select data;
                    if (eId != null)
                    {
                        dataList = dataList.Where(w => w.Enterprise_Info_ID == eId);
                    }
                    if (IsAnalyse == ((int)Common.EnumFile.AnalysisType.StopAnalysis).ToString())
                    {
                        dataList = dataList.Where(w => w.IsAnalyse == Convert.ToInt64(IsAnalyse));
                    }
                    else
                    {
                        dataList = dataList.Where(w => w.IsAnalyse == Convert.ToInt64(IsAnalyse) || w.IsAnalyse == null);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.Enterprise_Info_ID).Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }

            return dataInfoList;
        }

        /// <summary>
        /// 通过企业id获取企业名称
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <returns></returns>
        public View_Analysis GetModel(long eId)
        {
            View_Analysis model = new View_Analysis();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.View_Analysis.FirstOrDefault(t => t.Enterprise_Info_ID == eId);
                    ClearLinqModel(model);
                }
            }
            catch
            {
            }
            return model;
        }

        /// <summary>
        /// 设置企业产品解析
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>操作结果</returns>
        public RetResult SetAnalysis(Analysis newModel, string materialIdArray, string IsAnalyse)
        {
            Ret.Msg = "产品解析失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            bool flag = false;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    string[] array = materialIdArray.Split(',');
                    foreach (var item in array)
                    {
                        long materialId = Convert.ToInt64(item);
                        var model = dataContext.Analysis.FirstOrDefault(p => p.Material_ID == materialId);
                        if (model == null)//未停止解析的产品，是可以停止解析的
                        {
                            newModel.IsAnalyse = Convert.ToInt64(Common.EnumFile.AnalysisType.StopAnalysis);
                            newModel.Material_ID = materialId;
                            dataContext.Analysis.InsertOnSubmit(newModel);
                            dataContext.SubmitChanges();
                            flag = true;                     
                        }
                        else
                        {
                            if (IsAnalyse == ((int)Common.EnumFile.AnalysisType.StopAnalysis).ToString())//已停止解析的产品，是可以允许解析的
                            {
                                model.IsAnalyse = Convert.ToInt64(Common.EnumFile.AnalysisType.AnalysisType);
                                flag = false;  
                            }
                            else
                            {
                                model.IsAnalyse = Convert.ToInt64(Common.EnumFile.AnalysisType.StopAnalysis);//未停止解析的产品，是可以停止解析的
                                flag = true;
                            }
                            dataContext.SubmitChanges();
                        }
                    }
                    if (flag == false)
                    {
                        Ret.Msg = "允许产品解析";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        Ret.Msg = "停止产品解析！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        /// <summary>
        /// 根据企业信息和产品信息查询产品解析
        /// </summary>
        /// <param name="codeInfo"></param>
        /// <returns></returns>
        public Analysis GetAnalysis(CodeInfo codeInfo)
        {
            Analysis model = new Analysis();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    model = dataContext.Analysis.Where(p => p.Enterprise_Info_ID == codeInfo.EnterpriseID && p.Material_ID == codeInfo.MaterialID).FirstOrDefault();
                }
                catch { }
            }
            return model;
        }
    }
}
