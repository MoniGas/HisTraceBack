using System;
using System.Collections.Generic;
using Dal;
using LinqModel;

namespace BLL
{
    public class DataScanSumBLL
    {
        DataScanSum dal=new DataScanSum();
        public List<ScanSumDay> GetDaySum(DateTime beginTime, DateTime endTime, long enterpriseId)
        {
            return dal.GetDaySum(beginTime, endTime, enterpriseId);
        }

        public List<ScanSumMonth> GetMonthSum(DateTime beginTime, DateTime endTime, long enterpriseId)
        {
            return dal.GetMonthSum(beginTime, endTime, enterpriseId);
        }
    }
}
