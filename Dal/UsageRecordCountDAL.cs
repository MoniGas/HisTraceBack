/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-17

** 联系方式 :13313318725

** 描述：主要用于二维码（使用记录）统计管理数据层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 主要用于二维码（使用记录）统计管理数据层
    /// </summary>
    public class UsageRecordCountDAL : DALBase
    {
        /// <summary>
        /// 获取二维码使用记录列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        public List<View_MaterialUsageRecord> GetList(int index, long enterpriseId, string beginDate, string endDate, int pageIndex, out long totalCount)
        {
            List<View_MaterialUsageRecord> result = new List<View_MaterialUsageRecord>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_MaterialUsageRecord.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    DateTime date = DateTime.Now;
                    if (index == (int)Common.EnumFile.DateType.FirstSevenDay)//前7天 
                    {
                        tempResult = tempResult.Where(m => m.CreateDate >= Convert.ToDateTime(date.AddDays(-7).ToShortDateString()) && m.CreateDate <= date);
                    }
                    else if (index == (int)Common.EnumFile.DateType.LastMonth)//前30天 
                    {
                        tempResult = tempResult.Where(m => m.CreateDate >= Convert.ToDateTime(date.AddDays(-30).ToShortDateString()) && m.CreateDate <= date);
                    }
                    else if (index == (int)Common.EnumFile.DateType.FirstSixMonth)//前6个月 
                    {
                        tempResult = tempResult.Where(m => m.CreateDate >= Convert.ToDateTime(date.AddMonths(-6).ToString()) && m.CreateDate <= date);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(beginDate))
                        {
                            tempResult = tempResult.Where(m => m.CreateDate >= Convert.ToDateTime(beginDate));
                        }
                        if (!string.IsNullOrEmpty(endDate))
                        {
                            tempResult = tempResult.Where(m => m.CreateDate <= Convert.ToDateTime(endDate).AddDays(1));
                        }
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Usage_ID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "UsageRecordCountDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取产品模型
        /// </summary>
        /// <param name="beginCode">起始码</param>
        /// <returns></returns>
        public Material GetMaterial(string beginCode)
        {
            Material result = new Material();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Enterprise_FWCode_00 codeBegin = new RequestCodeDAL().GetEWM(beginCode);
                    if(codeBegin!=null)
                    {
                        result = dataContext.Material.FirstOrDefault(d => d.Material_ID == codeBegin.Material_ID);
                        ClearLinqModel(result);                    
                    }
                }
                catch (Exception ex)
                {
                    string errData = "UsageRecordCountDAL.GetMaterial()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id">记录id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        public List<Enterprise_FWCode_00> GetRecordDetail(long id, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    List<Enterprise_FWCode_00> DataList = new List<Enterprise_FWCode_00>();
                    MaterialUsageRecord model = dataContext.MaterialUsageRecord.FirstOrDefault(d => d.Usage_ID == id);
                    if (model != null)
                    {
                        string fixCode = model.BeginCode.Substring(0, model.BeginCode.Length - 12);
                        string[] arrStart = model.BeginCode.Split('.');
                        int start = Convert.ToInt32(arrStart[model.BeginCode.Split('.').Length - 2]);
                        string[] arrEnd = model.EndCode.Split('.');
                        int end = Convert.ToInt32(arrEnd[model.BeginCode.Split('.').Length - 2]);
                        for (int i = start; i <= end; i++)
                        {
                            string code = fixCode + i.ToString() + ".1";
                            using (LinqModel.DataClassesDataContext dataLinq = GetDataContext("Code_Connect"))
                            {
                                Enterprise_FWCode_00 codeInfo = dataLinq.Enterprise_FWCode_00.FirstOrDefault(d => d.EWM == code);
                                if (codeInfo != null)
                                {
                                    DataList.Add(codeInfo);
                                }
                            }
                        }
                    }
                    totalCount = DataList.Count;
                    DataList = DataList.Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(DataList);
                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取要导出的码内容
        /// </summary>
        /// <param name="id">记录id</param>
        /// <returns></returns>
        public string GetExportTxt(long id)
        {
            string str = "";
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    List<Enterprise_FWCode_00> DataList = new List<Enterprise_FWCode_00>();
                    MaterialUsageRecord model = dataContext.MaterialUsageRecord.FirstOrDefault(d => d.Usage_ID == id);
                    if (model != null)
                    {
                        string fixCode = model.BeginCode.Substring(0, model.BeginCode.Length - 12);
                        string[] arrStart = model.BeginCode.Split('.');
                        int start = Convert.ToInt32(arrStart[model.BeginCode.Split('.').Length - 2]);
                        string[] arrEnd = model.EndCode.Split('.');
                        int end = Convert.ToInt32(arrEnd[model.BeginCode.Split('.').Length - 2]);
                        for (int i = start; i <= end; i++)
                        {
                            string code = fixCode + i.ToString() + ".1";
                            using (LinqModel.DataClassesDataContext dataLinq = GetDataContext("Code_Connect"))
                            {
                                Enterprise_FWCode_00 codeInfo = dataLinq.Enterprise_FWCode_00.FirstOrDefault(d => d.EWM == code);
                                if (codeInfo.Type == (int)Common.EnumFile.GenCodeType.boxCode)
                                {
                                    string urlStr = System.Configuration.ConfigurationManager.AppSettings["idcodeURL"];
                                    LoginInfo pf = Common.Argument.SessCokie.Get;
                                    if (pf.Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                                    {
                                        urlStr = System.Configuration.ConfigurationManager.AppSettings["ncpURL"];
                                    }
                                    str = urlStr + codeInfo.EWM + "\t\r\n";
                                }
                            }
                        }
                    }
                    return str;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
