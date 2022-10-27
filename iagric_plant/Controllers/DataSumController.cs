using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class DataSumController : Controller
    {
        /// <summary>
        /// 日扫码量分析
        /// </summary>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ScanDayLines(string sdate, string edate)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultLists results = new BaseResultLists();
            try
            {
                DateTime beginTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM") + "-01");
                if (!string.IsNullOrEmpty(sdate))
                {
                    beginTime = DateTime.Parse(sdate + " 00:00:00");
                }

                DateTime endTime = DateTime.Now;
                if (!string.IsNullOrEmpty(edate))
                {
                    endTime = DateTime.Parse(edate + " 23:59:59");
                }
                DataScanSumBLL dal = new DataScanSumBLL();
                List<ScanSumDay> lst = dal.GetDaySum(beginTime, endTime.AddDays(1), user.EnterpriseID);
                if (null != lst)
                {
                    results.code = "1";
                    var result = (from c in lst
                                  group c by c.Days
                                      into g
                                      select new
                                      {
                                          timeInfo = g.Key,
                                          AreaInfo = "",
                                          sumCount = g.Sum(p => p.ScanCount)
                                      }).ToList();
                    List<sumData> rLst = new List<sumData>();
                    foreach (var r in result)
                    {
                        sumData d = new sumData();
                        d.AreaInfo = r.AreaInfo;
                        d.timeInfo = r.timeInfo;
                        d.sumCount = (long)r.sumCount;
                        rLst.Add(d);
                    }
                    string[] arrTime = (from c in rLst select c.timeInfo).ToArray();
                    long[] arrCount = (from c in rLst select c.sumCount).ToArray();
                    results.timeInfo = string.Join(",", arrTime);
                    results.countInfo = string.Join(",", arrCount);
                    var resultPie = (from c in lst
                                     group c by c.ProvinceName
                                         into g
                                         select new
                                         {
                                             timeInfo = "",
                                             AreaInfo = g.Key,
                                             sumCount = g.Sum(p => p.ScanCount)
                                         }).ToList();
                    List<sumData> rPieLst = new List<sumData>();
                    foreach (var r in resultPie)
                    {
                        sumData d = new sumData();
                        d.AreaInfo = r.AreaInfo;
                        d.timeInfo = r.timeInfo;
                        d.sumCount = (long)r.sumCount;
                        rPieLst.Add(d);
                    }
                    string[] arrAreaPie = (from c in rPieLst select c.AreaInfo).ToArray();
                    long[] arrCountPie = (from c in rPieLst select c.sumCount).ToArray();
                    results.pieArea = string.Join(",", arrAreaPie);
                    results.piecount = string.Join(",", arrCountPie);
                }
            }
            catch (Exception ex)
            {
                string errData = "DataSumController.DataSumList():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                results.code = "0";
            }
            return Json(results);

        }

        /// <summary>
        /// 月扫码量分析
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DataSumList(string year)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultLists result = new BaseResultLists();
            try
            {
                string yeartest = Request["year"];
                if (String.IsNullOrEmpty(year))
                {
                    year = DateTime.Now.ToString("yyyy");
                }
                DateTime beginTime = DateTime.Parse(year + "-01-01 00:00:00");
                DateTime endTime = DateTime.Parse(beginTime.AddYears(1).AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:59");
                var dal = new DataScanSumBLL();
                List<ScanSumMonth> lst = dal.GetMonthSum(beginTime, endTime.AddDays(1), user.EnterpriseID);
                if (null != lst)
                {
                    result.code = "1";
                    List<sumData> rLst = new List<sumData>();
                    for (int i = 1; i <= 12; i++)
                    {
                        string month = year + "-" + i.ToString().PadLeft(2, '0');
                        sumData d = new sumData();
                        d.AreaInfo = "";
                        d.timeInfo = i + "月";
                        //d.sumCount = (long)(from c in lst where c.Months.Equals(month) select c.ScanCount).Sum();
                        ScanSumMonth monthsum = lst.FirstOrDefault(c => c.Months == month);
                        d.sumCount = 0;
                        if (monthsum != null)
                        {
                            d.sumCount = (long)monthsum.ScanCount;
                        }
                        rLst.Add(d);
                    }
                    List<sumData> rPieLst = new List<sumData>();
                    string[] arrTime = (from c in rLst select c.timeInfo).ToArray();
                    long[] arrCount = (from c in rLst select c.sumCount).ToArray();
                    result.timeInfo = string.Join(",", arrTime);
                    result.countInfo = string.Join(",", arrCount);
                    var resultPie = (from c in lst
                                     group c by c.ProvinceName into g
                                     select new
                                     {
                                         timeInfo = "",
                                         AreaInfo = g.Key,
                                         sumCount = g.Sum(p => p.ScanCount)
                                     }).ToList();
                    foreach (var r in resultPie)
                    {
                        sumData d = new sumData();
                        d.AreaInfo = r.AreaInfo;
                        d.timeInfo = r.timeInfo;
                        d.sumCount = (long)r.sumCount;
                        rPieLst.Add(d);
                    }
                    string[] arrAreaPie = (from c in rPieLst select c.AreaInfo).ToArray();
                    long[] arrCountPie = (from c in rPieLst select c.sumCount).ToArray();
                    result.pieArea = string.Join(",", arrAreaPie);
                    result.piecount = string.Join(",", arrCountPie);
                }
            }
            catch (Exception ex)
            {
                string errData = "DataSumController.DataSumList():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                result.code = "0";
            }
            return Json(result);
        }

        public class sumData
        {
            public string timeInfo { get; set; }
            public string AreaInfo { get; set; }
            public long sumCount { get; set; }
        }

        public class BaseResultLists
        {
            public string code { set; get; }
            public string timeInfo { get; set; }
            public string countInfo { get; set; }
            public string pieArea { get; set; }
            public string piecount { get; set; }
        }
    }
}