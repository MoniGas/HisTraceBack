using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using Dal;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class SysEnLicenseBLL
    {
        /// <summary>
        /// 获取授权企业信息列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <returns></returns>
        public PagedList<Enterprise_License> GetEnLicenseList(string name, string beginDate, string endDate, int? pageIndex)
        {
            SysEnLicenseDAL dal = new SysEnLicenseDAL();
            PagedList<Enterprise_License> dataList = dal.GetEnLicenseList(name, beginDate, endDate, pageIndex);
            return dataList;
        }
        /// <summary>
        /// 获取接口查询记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PagedList<Enterprise_LicenseGYJRecord> GetEnRecordList(string name, string beginDate, string endDate, int? pageIndex)
        {
            SysEnLicenseDAL dal = new SysEnLicenseDAL();
            PagedList<Enterprise_LicenseGYJRecord> dataList = dal.GetEnRecordList(name, beginDate, endDate, pageIndex);
            return dataList;
        }
    }
}
