using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class DataScanSum : DALBase
    {
        /// <summary>
        /// 获取给定时间后的扫码记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public List<ScanEwm> GetScanLst(string beginTime, int pageSize)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    List<ScanEwm> lst;
                    if (!String.IsNullOrEmpty(beginTime))
                    {
                        lst = db.ScanEwm.Where(p => p.ScanDate > DateTime.Parse(beginTime)).OrderBy(p => p.ScanDate).Take(10).ToList();
                    }
                    else
                    {
                        lst = db.ScanEwm.OrderBy(p => p.ScanDate).Take(10).ToList();
                    }
                    return lst;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.getScanLst()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 判断日期扫码记录是否存在
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="province">省份编码</param>
        /// <param name="day">日期</param>
        /// <returns></returns>
        public ScanSumDay IsExistsDay(long enterpriseId, string province, string day)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    var model = db.ScanSumDay.FirstOrDefault(p => p.EnterpriseId == enterpriseId
                                                                         && p.Days == day && p.Province == province);
                    return model;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.isExistsDay()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }


        /// <summary>
        /// 判断月份扫码记录是否存在
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="province">省份编码</param>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public ScanSumMonth IsExistsMonth(long enterpriseId, string province, string month)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    var model = db.ScanSumMonth.FirstOrDefault(p => p.EnterpriseId == enterpriseId
                                                                             && p.Months == month && p.Province == province);
                    return model;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.isExistsMonth()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 判断月份扫码记录是否存在
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="province">省份编码</param>
        /// <param name="year">年</param>
        /// <returns></returns>
        public ScanSumYear IsExistsYear(long enterpriseId, string province, string year)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    var model = db.ScanSumYear.FirstOrDefault(p => p.EnterpriseId == enterpriseId
                                                                           && p.Years == year && p.Province == province);
                    return model;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.isExistsYear()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 添加日记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddScanEwmSumDay(ScanSumDay model)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    db.ScanSumDay.InsertOnSubmit(model);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.addScanEwmSumDay()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 添加月记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddSanEwmSumMonth(ScanSumMonth model)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    db.ScanSumMonth.InsertOnSubmit(model);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.addSanEwmSumMonth()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 添加年记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddSanEwmSumYear(ScanSumYear model)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    db.ScanSumYear.InsertOnSubmit(model);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.addSanEwmSumYear()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 日记录加1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddScanEwmSumDayCount(long id)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    ScanSumDay m = db.ScanSumDay.FirstOrDefault(p => p.Id == id);
                    m.ScanCount++;
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.addScanEwmSumDayCount()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 月记录加1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddScanEwmSumMonthCount(long id)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    ScanSumMonth m = db.ScanSumMonth.FirstOrDefault(p => p.Id == id);
                    m.ScanCount++;
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.addScanEwmSumMonthCount()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 年记录加1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddSanEwmSumYearCount(long id)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    ScanSumYear m = db.ScanSumYear.FirstOrDefault(p => p.Id == id);
                    m.ScanCount++;
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.addSanEwmSumYearCount()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return false;
                }
            }
        }


        public List<ScanSumDay> GetDaySum(DateTime beginTime, DateTime endTime, long enterpriseId)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    return db.ScanSumDay.Where(p => p.LastTime >= beginTime &&
                    p.LastTime <= endTime && p.EnterpriseId == enterpriseId).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.getDaySum()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }
        public List<ScanSumMonth> GetMonthSum(DateTime beginTime, DateTime endTime, long enterpriseId)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    return db.ScanSumMonth.Where(p => p.LastTime >= beginTime &&
                    p.LastTime <= endTime && p.EnterpriseId == enterpriseId).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.getMonthSum()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        public List<ScanSumYear> GetYearSum(DateTime beginTime, DateTime endTime, long enterpriseId)
        {
            string conString = ConfigurationManager.AppSettings["Code_Connect"];
            using (var db = GetContext(conString))
            {
                try
                {
                    return db.ScanSumYear.Where(p => p.LastTime >= beginTime &&
                    p.LastTime <= endTime && p.EnterpriseId == enterpriseId).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "DataScanSum.getYearSum()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }
    }
}
