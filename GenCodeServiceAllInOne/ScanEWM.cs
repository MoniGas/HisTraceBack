using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Common.Log;
using Dal;

namespace GenCodeServiceAllInOne
{
    public static class ScanEwm
    {

        public static void Start()
        {
            ThreadStart start = DoScanEwm;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }

        public static void DoScanEwm()
        {
            while (true)
            {
                try
                {
                    var sql = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);

                    DataView dvGenCodeInfo = DbHelperSQL.Query(sql).Tables[0].DefaultView;
                    for (int i = 0; i < dvGenCodeInfo.Count; i++)
                    {
                        //数据库连接串
                        string conStr1 = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                            dvGenCodeInfo[i]["DataBaseIP"].ToString(), dvGenCodeInfo[i]["DataBaseName"].ToString(),
                            dvGenCodeInfo[i]["DatabaseUserID"].ToString(), dvGenCodeInfo[i]["DatabasePWD"].ToString());
                        using (SqlConnection conn = new SqlConnection(conStr1))
                        {
                            conn.Open();
                            try
                            {
                                sql = "select  top 1 * from Route_DataBase";
                                DataSet dtConn = new DataSet();
                                SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                                ////commandConn.SelectCommand.Transaction = trans;
                                commandConn.Fill(dtConn);
                                DataRow dr = dtConn.Tables[0].Rows[0];

                                string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                                         dr["DataSource"].ToString(), dr["DataBaseName"].ToString(),
                                         dr["UID"].ToString(), dr["PWD"].ToString());
                                var id = Convert.ToInt32(dvGenCodeInfo[i]["ID"]);

                                #region MyRegion

                                //string URL = "http://ip.taobao.com/service/getIpInfo.php?ip={0}";
                                string URL = ConfigurationManager.AppSettings["getipinfo"];
                                DataScanSumDal dal = new DataScanSumDal();
                                Dictionary<string, long> mainCodeInfo = new Dictionary<string, long>();
                                try
                                {
                                    if (dvGenCodeInfo[i]["BeginTime"] == null)
                                    {
                                        return;
                                    }
                                    string beginTime = Convert.ToDateTime(dvGenCodeInfo[i]["BeginTime"])
                                        .ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    DataView dataLst = dal.GetScanLst(beginTime, conStr);
                                    DateTime scanDate = Convert.ToDateTime(beginTime);
                                    if (null != dataLst)
                                    {
                                        if (dataLst.Count > 0)
                                        {
                                            for (int k = 0; k < dataLst.Count; k++)
                                            {
                                                if (!string.IsNullOrEmpty(dataLst[k]["Ip"].ToString()))
                                                {
                                                    string result = GetHtml(string.Format(URL,
                                                        dataLst[k]["Ip"].ToString()));
                                                    Regex regex = new Regex(
                                                        @"\""ip\"":""(?<IP>[^\""]*)"",\""country\"":""(?<Country>[^\""]*)"",\""area\"":""(?<area>[^\""]*)"",\""region\"":""(?<Provice>[^\""]*)"",\""city\"":""(?<City>[^\""]*)"",\""county\"":""(?<county>[^\""]*)"",\""isp\"":""(?<isp>[^\""]*)"",\""country_id\"":""(?<countryCode>[^\""]*)"",\""area_id\"":""(?<carea_id>[^\""]*)"",\""region_id\"":""(?<ProviceCode>[^\""]*)"",\""city_id\"":""(?<CityCode>[^\""]*)""",
                                                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant |
                                                        RegexOptions.Multiline | RegexOptions.Singleline);
                                                    MatchCollection matchCollection = regex.Matches(result);
                                                    foreach (Match m in matchCollection)
                                                    {
                                                        if (string.IsNullOrEmpty(m.Groups["country"].ToString()))
                                                        {
                                                            string provice;
                                                            string proviceCode;
                                                            string city;
                                                            string cityCode;
                                                            if (m.Groups["country"].ToString().ToLower() == "xx" ||
                                                                m.Groups["ProviceCode"].ToString().ToLower() == "xx")
                                                            {
                                                                provice = "内部网络";
                                                                proviceCode = "local";
                                                                city = "内部网络";
                                                                cityCode = "local";
                                                            }
                                                            else
                                                            {
                                                                provice = m.Groups["Provice"].ToString();
                                                                proviceCode = m.Groups["ProviceCode"].ToString();
                                                                city = m.Groups["City"].ToString();
                                                                cityCode = m.Groups["CityCode"].ToString();
                                                            }
                                                            long enterpriseId = 0;
                                                            var code = dataLst[k]["EWM"].ToString();
                                                            string mainsCode;
                                                            if (code.Contains("/")) //判断是否是全码
                                                            {
                                                                mainsCode = code.Split('/')[0];
                                                                if (code.Substring(0, 2).Equals("MA"))
                                                                {
                                                                    var fixcode = code.Split('/')[0] + "/" + code.Split('/')[1] + "/" + code.Split('/')[2].Substring(0, 6);
                                                                    DataView dv = dal.GetRequestCode(fixcode, conStr1);
                                                                    if (dv.Count > 0)
                                                                    {
                                                                        DataRow drr = dv[0].Row;
                                                                        if (null != drr)
                                                                        {
                                                                            enterpriseId = Convert.ToInt64(drr["Enterprise_Info_ID"]);
                                                                            DataView dre = dal.GetenterPriseInfo(enterpriseId, conStr1);
                                                                            if (null != dre)
                                                                                mainsCode = dre[0]["MainCode"].ToString();
                                                                        }
                                                                    }

                                                                }
                                                                if (mainCodeInfo.ContainsKey(mainsCode))
                                                                {
                                                                    enterpriseId = mainCodeInfo[mainsCode];
                                                                }
                                                                else
                                                                {
                                                                    DataView enterprise =
                                                                        dal.GetenterPriseInfo1(mainsCode, conStr1);
                                                                    if (enterprise.Count > 0)
                                                                    {
                                                                        enterpriseId = long.Parse(enterprise[0]["Enterprise_Info_ID"].ToString());
                                                                        mainCodeInfo.Add(mainsCode, enterpriseId);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (code.Length == 32) //判断是否是农药码
                                                                {
                                                                    var nycpwym = code.Substring(11, 6); //农药产品IDCode唯一码
                                                                    //string s = dal.GetCat(nycpwym, conStr1)["Enterprise_Info_ID"].ToString();

                                                                    DataView dvs = dal.GetCat(nycpwym, conStr1);
                                                                    if (dvs.Count > 0)
                                                                    {
                                                                        string s = dvs[0]["Enterprise_Info_ID"].ToString();
                                                                        var enterId = long.Parse(s);
                                                                        DataView dvv = dal.GetenterPriseInfo(enterId, conStr1);
                                                                        if (dvv.Count > 0)
                                                                        {
                                                                            mainsCode = dvv[0]["MainCode"].ToString();
                                                                            if (mainCodeInfo.ContainsKey(mainsCode))
                                                                            {
                                                                                enterpriseId = mainCodeInfo[mainsCode];
                                                                            }
                                                                            else
                                                                            {
                                                                                DataView enterprise =
                                                                                    dal.GetenterPriseInfo1(mainsCode, conStr1);
                                                                                if (enterprise.Count > 0)
                                                                                {
                                                                                    enterpriseId = long.Parse(enterprise[0]["Enterprise_Info_ID"].ToString());
                                                                                    mainCodeInfo.Add(mainsCode, enterpriseId);
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    var mainsCodejm = code.Substring(0, 4);
                                                                    DataView dv = dal.GetenterPriseInfo(mainsCodejm, conStr1);
                                                                    if (dv.Count > 0)
                                                                    {
                                                                        mainsCode = dv[0]["MainCode"].ToString();
                                                                        if (mainCodeInfo.ContainsKey(mainsCode))
                                                                        {
                                                                            enterpriseId = mainCodeInfo[mainsCode];
                                                                        }
                                                                        else
                                                                        {
                                                                            DataView enterprise =
                                                                                dal.GetenterPriseInfo1(mainsCode, conStr1);
                                                                            if (enterprise.Count > 0)
                                                                            {
                                                                                enterpriseId = long.Parse(enterprise[0][
                                                                                    "Enterprise_Info_ID"].ToString());
                                                                                mainCodeInfo.Add(mainsCode, enterpriseId);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            var scanDate1 = Convert.ToDateTime(dataLst[k]["ScanDate"])
                                                                .ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                            scanDate = DateTime.ParseExact(scanDate1,
                                                                "yyyy-MM-dd HH:mm:ss.fff",
                                                                System.Globalization.CultureInfo.InvariantCulture);
                                                            DataView dayModeldv = dal.IsExistsDay(enterpriseId,
                                                                proviceCode, scanDate.ToString("yyyy-MM-dd"), conStr);
                                                            if (dayModeldv.Count == 0)
                                                            {
                                                                LinqModel.ScanSumDay dayModel =
                                                                    new LinqModel.ScanSumDay();
                                                                dayModel.City = cityCode;
                                                                dayModel.CityName = city;
                                                                dayModel.ScanCount = 1;
                                                                dayModel.Days = scanDate.ToString("yyyy-MM-dd");
                                                                dayModel.EnterpriseId = enterpriseId;
                                                                dayModel.Province = proviceCode;
                                                                dayModel.ProvinceName = provice;
                                                                dayModel.LastTime = scanDate;
                                                                dal.AddSanEwmSumDay(dayModel, conStr);
                                                            }
                                                            else
                                                            {
                                                                dal.AddSanEwmSumDayCount(dayModeldv[0]["ID"].ToString(), dayModeldv[0]["ScanCount"].ToString(), scanDate, conStr);
                                                            }

                                                            DataView monthModeldv = dal.IsExistsMonth(enterpriseId,
                                                                proviceCode, scanDate.ToString("yyyy-MM"), conStr);
                                                            if (monthModeldv.Count == 0)
                                                            {
                                                                LinqModel.ScanSumMonth monthModel =
                                                                    new LinqModel.ScanSumMonth();
                                                                monthModel.City = cityCode;
                                                                monthModel.CityName = city;
                                                                monthModel.ScanCount = 1;
                                                                monthModel.Months = scanDate.ToString("yyyy-MM");
                                                                monthModel.EnterpriseId = enterpriseId;
                                                                monthModel.Province = proviceCode;
                                                                monthModel.ProvinceName = provice;
                                                                monthModel.LastTime = scanDate;
                                                                dal.AddSanEwmSumMonth(monthModel, conStr);
                                                            }
                                                            else
                                                            {
                                                                dal.AddSanEwmSumMonthCount(monthModeldv[0]["ID"].ToString(), monthModeldv[0]["ScanCount"].ToString(), scanDate, conStr);
                                                            }
                                                            DataView yearModelDv = dal.IsExistsYear(enterpriseId,
                                                                proviceCode, scanDate.ToString("yyyy"), conStr);
                                                            if (yearModelDv.Count == 0)
                                                            {
                                                                LinqModel.ScanSumYear yearModel =
                                                                    new LinqModel.ScanSumYear();
                                                                yearModel.City = cityCode;
                                                                yearModel.CityName = city;
                                                                yearModel.ScanCount = 1;
                                                                yearModel.Years = scanDate.ToString("yyyy");
                                                                yearModel.EnterpriseId = enterpriseId;
                                                                yearModel.Province = proviceCode;
                                                                yearModel.ProvinceName = provice;
                                                                yearModel.LastTime = scanDate;
                                                                dal.AddSanEwmSumYear(yearModel, conStr);
                                                            }
                                                            else
                                                            {
                                                                dal.AddSanEwmSumYearCount(yearModelDv[0]["ID"].ToString(), yearModelDv[0]["ScanCount"].ToString(), scanDate, conStr);
                                                            }
                                                        }
                                                        string sqlstr = string.Format(
                                                            "update GenCondeInfoSetting set BeginTime='{0:yyyy-MM-dd HH:mm:ss.fff}' where ID={1}",
                                                            scanDate, id);
                                                        bool value = DbHelperSQL.ExecuteSql(sqlstr) > 0;
                                                        WriteLog.WriteSeriveLog(scanDate + "    " + value, "ScanEWM");
                                                    }
                                                }
                                            }
                                            Thread.Sleep(1000 * 60);
                                        }
                                    }
                                    else
                                    {
                                        WriteLog.WriteSeriveLog("个数:0", "ScanEWM");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Thread.Sleep(1000 * 60);
                                    WriteLog.WriteSeriveLog(ex.Message, "ScanEWM");
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                WriteLog.WriteSeriveLog(ex.Message, "ScanEWM");
                            }
                        }
                    }
                    Thread.Sleep(1000 * 1);
                }
                catch (Exception ex)
                {
                    WriteLog.WriteSeriveLog(ex.Message, "ScanEWM");
                }
            }
        }

        public static void InitHeader(WebClient webClient)
        {
            webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
        }
        public static string GetHtml(string url)
        {
            WebClient webClientBd = new WebClient();
            Uri uri = new Uri(url);
            InitHeader(webClientBd);
            webClientBd.Encoding = Encoding.UTF8;
            var html = webClientBd.DownloadString(uri);
            return html;
        }
    }
}
