using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    public class SysSetAnalysisBLL
    {
        /// <summary>
        /// 获取解析mac地址列表
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<SysSetAnalysis> GetAnalysisList(string mac, string beginDate, string endDate, int? pageIndex)
        {
            SysSetAnalysisDAL dal = new SysSetAnalysisDAL();
            PagedList<SysSetAnalysis> dataList = dal.GetAnalysisList(mac, beginDate, endDate, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 添加Mac地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel Add(SysSetAnalysis model)
        {
            SysSetAnalysisDAL dal = new SysSetAnalysisDAL();
            RetResult result = dal.Add(model);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public SysSetAnalysis GetAnalysisInfo(long id)
        {
            SysSetAnalysisDAL dal = new SysSetAnalysisDAL();
            SysSetAnalysis data = dal.GetAnalysisInfo(id);
            return data;
        }

        public BaseResultModel Edit(SysSetAnalysis model)
        {
            SysSetAnalysisDAL dal = new SysSetAnalysisDAL();
            RetResult result = dal.Edit(model);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public RetResult EditMacStatus(long id, int type)
        {
            SysSetAnalysisDAL dal = new SysSetAnalysisDAL();
            RetResult result = new RetResult();
            result = dal.EditMacStatus(id, type);
            return result;
        }
    }
}
