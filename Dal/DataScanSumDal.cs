using System;
using System.Data;
using Common.Log;

namespace Dal
{
    public class DataScanSumDal
    {
        public DataView GetScanLst(string beginTime, string conStr)
        {
            string sql = string.Format("SELECT * FROM  ScanEwm  m  WHERE m.ScanDate>'{0}'  order by ScanDate ", beginTime);
            return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
        }

        /// <summary>
        /// 判断日期扫码记录是否存在
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="province">省份编码</param>
        /// <param name="day">日期</param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView IsExistsDay(long enterpriseId, string province, string day, string conStr)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM  ScanSumDay m  WHERE m.EnterpriseID ='{0}'
               and  m.Province ='{1}' and  m.Days='{2}'", enterpriseId, province, day);
                return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                string errData = "DataScanSumDal.isExistsDay()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 判断月份扫码记录是否存在
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="province">省份编码</param>
        /// <param name="month">月份</param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView IsExistsMonth(long enterpriseId, string province, string month, string conStr)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM  ScanSumMonth m  WHERE m.EnterpriseID ='{0}'
               and  m.Province ='{1}' and m.Months='{2}'", enterpriseId, province, month);
                return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                string errData = "DataScanSumDal.isExistsMonth()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 判断月份扫码记录是否存在
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="province">省份编码</param>
        /// <param name="year">年</param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView IsExistsYear(long enterpriseId, string province, string year, string conStr)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM  ScanSumYear m  WHERE m.EnterpriseID ='{0}'
               and  m.Province ='{1}' and  m.Years='{2}'", enterpriseId, province, year);
                return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                string errData = "DataScanSumDal.isExistsYear()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 添加日记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public bool AddSanEwmSumDay(LinqModel.ScanSumDay model, string conStr)
        {
            try
            {

                string sql = string.Format(@"insert into  ScanSumDay ( EnterpriseID ,ScanCount, Province, ProvinceName, City, CityName, DAYs,LastTime)
	            values( {0},1,'{1}','{2}','{3}','{4}','{5}','{6}')", model.EnterpriseId, model.Province,
                model.ProvinceName, model.City, model.CityName, model.Days, model.LastTime);
                //WriteLog.WriteErrorLog(sql);
                return DbHelperSQL.ExecuteSql(sql, conStr, "") > 1;
            }
            catch (Exception ex)
            {
                string errData = "DataScanSumDal.addSanEwmSumDay()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 添加月记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public bool AddSanEwmSumMonth(LinqModel.ScanSumMonth model, string conStr)
        {
            try
            {
                string sql = string.Format(@"insert into  ScanSumMonth	( EnterpriseID,ScanCount, Province, ProvinceName, City, CityName, Months,LastTime	)
	        values( {0},1,'{1}','{2}','{3}','{4}','{5}','{6}')", model.EnterpriseId, model.Province,
            model.ProvinceName, model.City, model.CityName, model.Months, model.LastTime);
                //WriteLog.WriteErrorLog(sql);
                return DbHelperSQL.ExecuteSql(sql, conStr, "") > 1;
            }
            catch (Exception ex)
            {
                string errData = "DataScanSumDal.addSanEwmSumMonth()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 添加年记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public bool AddSanEwmSumYear(LinqModel.ScanSumYear model, string conStr)
        {
            try
            {
                string sql = string.Format(@"insert into  ScanSumYear	( EnterpriseId,ScanCount, Province, ProvinceName, City, CityName, Years,LastTime	)
	        values( {0},1,'{1}','{2}','{3}','{4}','{5}','{6}')", model.EnterpriseId, model.Province,
            model.ProvinceName, model.City, model.CityName, model.Years, model.LastTime);
                //WriteLog.WriteErrorLog(sql);
                return DbHelperSQL.ExecuteSql(sql, conStr, "") > 1;
            }
            catch (Exception ex)
            {
                string errData = "DataScanSumDal.addSanEwmSumYear()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 日记录加1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="scanData"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public bool AddSanEwmSumDayCount(string id, string count, DateTime scanData, string conStr)
        {
            string sql = string.Format(@"update  ScanSumDay set ScanCount={1},LastTime='{2}' where ID={0}", id, int.Parse(count) + 1, scanData);
            //WriteLog.WriteErrorLog(sql);
            return DbHelperSQL.ExecuteSql(sql, conStr, "") > 1;
        }

        /// <summary>
        /// 月记录加1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="scanData"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public bool AddSanEwmSumMonthCount(string id, string count, DateTime scanData, string conStr)
        {
            string sql = string.Format(@"update  scansumMonth set ScanCount={1},LastTime='{2}' where ID={0}", id, int.Parse(count) + 1, scanData);
            //WriteLog.WriteErrorLog(sql);
            return DbHelperSQL.ExecuteSql(sql, conStr, "") > 1;
        }


        /// <summary>
        /// 年记录加1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="scanData"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public bool AddSanEwmSumYearCount(string id, string count, DateTime scanData, string conStr)
        {
            string sql = string.Format(@"update  scansumYear set ScanCount={1},LastTime='{2}' where ID={0}", id, int.Parse(count) + 1, scanData);
            //WriteLog.WriteErrorLog(sql);
            return DbHelperSQL.ExecuteSql(sql, conStr, "") > 1;
        }

        /// <summary>
        /// 简码方法
        /// </summary>
        /// <param name="mainCode">企业主码</param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView GetenterPriseInfo(string mainCode, string conStr)
        {
            string sql = string.Format(@"SELECT * FROM  Enterprise_Info e WHERE e.TraceEnMainCode='{0}'", mainCode);
            return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
        }

        /// <summary>
        /// 企业ID
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView GetenterPriseInfo(long id, string conStr)
        {
            string sql = string.Format(@"SELECT * FROM  Enterprise_Info e WHERE e.Enterprise_Info_ID='{0}'", id);
            return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
        }

        /// <summary>
        /// 全码方法
        /// </summary>
        /// <param name="mainCode">企业主码简码</param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView GetenterPriseInfo1(string mainCode, string conStr)
        {
            string sql = string.Format(@"SELECT * FROM  Enterprise_Info e WHERE e.MainCode='{0}'", mainCode);
            return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
        }

        /// <summary>
        /// 根据农药类型IDCode码
        /// </summary>
        /// <param name="nyZhengHao"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView GetCat(string nyZhengHao, string conStr)
        {
            string sql = string.Format(@"SELECT Top 1 * FROM  Category e WHERE e.SCategoryIDcode='{0}'", nyZhengHao);
            return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
        }

        /// <summary>
        /// 根据码号获取RequestCode
        /// </summary>
        /// <param name="fixcode"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public DataView GetRequestCode(string fixcode, string conStr)
        {
            string sql = string.Format(@"SELECT Top 1 * FROM  RequestCode  WHERE FixedCode='{0}'", fixcode);
            return DbHelperSQL.Query(sql, conStr).Tables[0].DefaultView;
        }
        
    }
}
