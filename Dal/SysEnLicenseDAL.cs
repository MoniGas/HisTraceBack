using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    public class SysEnLicenseDAL : DALBase
    {
        /// <summary>
        /// 获取授权企业信息列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="wareHouseStatus"></param>
        /// <returns></returns>
        public PagedList<Enterprise_License> GetEnLicenseList(string name, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<Enterprise_License> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Enterprise_License where data.State==0
                                   select data;
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.LicenseEndDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.LicenseEndDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    //totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.LicenseID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 获取接口查询记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PagedList<Enterprise_LicenseGYJRecord> GetEnRecordList(string name, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<Enterprise_LicenseGYJRecord> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Enterprise_LicenseGYJRecord select data;
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.AddDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.AddDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.ID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }
    }
}
