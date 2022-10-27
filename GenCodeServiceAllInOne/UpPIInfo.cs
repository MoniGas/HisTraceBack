using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using LinqModel;
using InterfaceWeb;
using Common.Argument;

namespace GenCodeServiceAllInOne
{
    public static class UpPIInfo
    {
        /// <summary>
        /// 上传PI信息至发码机构
        /// </summary>
        public static void Start()
        {
            ThreadStart start = UpPIDataInfo;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }

        public static void UpPIDataInfo()
        {
            while (true)
            {
                try
                {
                    //服务运行时间间隔
                    int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
                    //连接库
                    string conStr = System.Configuration.ConfigurationManager.AppSettings["Constr"];
                    //单表最大数据量
                    int tableMaxCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TableMaxCount"]);
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        WriteLogTest("开始执行", "UpPIInfo");
                        //CodingClientType为1代表是MA码  2代表GS1码；ISUpload为0表示该批码未上传;1表示已上传；2表示上传失败
                        string sql = string.Format(" select * from View_RequestCodeMa where ISUpload={0}  and CodingClientType=1", 0);
                        DataSet dtConn = new DataSet();
                        SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                        commandConn.Fill(dtConn);
                        DataView dvRequestCodeMa = Dal.DbHelperSQL.Query(sql).Tables[0].DefaultView;
                        WriteLogTest("总共" + dvRequestCodeMa.Count + "条数据上传", "UpPIInfo");
                        for (int iPIInfo = 0; iPIInfo < dvRequestCodeMa.Count; iPIInfo++)
                        {
                            conn.Open(); //打开连接 
                            #region 循环处理每一个数据库的业务
                            try
                            {
                                Enterprise_Info enInfo = new Dal.EnterpriseInfoDAL().GetModel(Convert.ToInt64(dvRequestCodeMa[iPIInfo]["Enterprise_Info_ID"].ToString()));
                                string entoken = "";
                                string entokenCode = "";
                                EnterpriseShopLink enEx = new Dal.ScanCodeDAL().ShopEn(Convert.ToInt64(dvRequestCodeMa[iPIInfo]["Enterprise_Info_ID"].ToString()));
                                if (enEx != null)
                                {
                                    entoken = enEx.access_token == null ? "" : enEx.access_token;
                                    entokenCode = enEx.access_token_code == null ? "" : enEx.access_token_code;
                                }
                                else
                                {
                                    conn.Close(); //关闭连接 
                                    continue;
                                }
                                #region 调取IDCode上传PI接口
                                string start_date=null; string ShiXiaoDate=null; string YouXiaoDate=null;
                                if (!string.IsNullOrEmpty(dvRequestCodeMa[iPIInfo]["startdate"].ToString()))
                                {
                                    start_date = DateTime.Now.Year.ToString().Substring(0, 2) + dvRequestCodeMa[iPIInfo]["startdate"].ToString().Substring(0, 2) + "-" + dvRequestCodeMa[iPIInfo]["startdate"].ToString().Substring(2, 2) + "-" + dvRequestCodeMa[iPIInfo]["startdate"].ToString().Substring(4, 2);
                                }
                                if (!string.IsNullOrEmpty(dvRequestCodeMa[iPIInfo]["ShiXiaoDate"].ToString()))
                                {
                                    ShiXiaoDate = DateTime.Now.Year.ToString().Substring(0, 2) + dvRequestCodeMa[iPIInfo]["ShiXiaoDate"].ToString().Substring(0, 2) + "-" + dvRequestCodeMa[iPIInfo]["ShiXiaoDate"].ToString().Substring(2, 2) + "-" + dvRequestCodeMa[iPIInfo]["ShiXiaoDate"].ToString().Substring(4, 2);
                                }
                                if (!string.IsNullOrEmpty(dvRequestCodeMa[iPIInfo]["YouXiaoDate"].ToString()))
                                {
                                    YouXiaoDate = DateTime.Now.Year.ToString().Substring(0, 2) + dvRequestCodeMa[iPIInfo]["YouXiaoDate"].ToString().Substring(0, 2) + "-" + dvRequestCodeMa[iPIInfo]["YouXiaoDate"].ToString().Substring(2, 2) + "-" + dvRequestCodeMa[iPIInfo]["YouXiaoDate"].ToString().Substring(4, 2);
                                }
                                if (string.IsNullOrEmpty(dvRequestCodeMa[iPIInfo]["CodeNumPath"].ToString()))
                                {
                                    conn.Close(); //关闭连接 
                                    continue;
                                }
                                //IDCodeUploadCodeListMsg result = BaseDataDAL.IDCodeMedicalUploadCodeList(entoken, entokenCode, enInfo.MainCode, dvRequestCodeMa[iPIInfo]["category_code"].ToString(), dvRequestCodeMa[iPIInfo]["MaterialName"].ToString(), dvRequestCodeMa[iPIInfo]["BZSpecType"].ToString(),
                                //        dvRequestCodeMa[iPIInfo]["code_list_str"].ToString(), start_date, dvRequestCodeMa[iPIInfo]["ShengChanPH"].ToString(), dvRequestCodeMa[iPIInfo]["dbatchnumber"].ToString(), ShiXiaoDate, YouXiaoDate, dvRequestCodeMa[iPIInfo]["product_model"].ToString());
                                IDCodeUploadCodeListMsg result = BaseDataDAL.IDCodeMedicalUploadCodeFile(entoken, entokenCode,
                                    enInfo.MainCode, dvRequestCodeMa[iPIInfo]["category_code"].ToString(), dvRequestCodeMa[iPIInfo]["MaterialName"].ToString(), dvRequestCodeMa[iPIInfo]["BZSpecType"].ToString(),
                                    start_date, dvRequestCodeMa[iPIInfo]["ShengChanPH"].ToString(),
                                    dvRequestCodeMa[iPIInfo]["dbatchnumber"].ToString(), ShiXiaoDate, YouXiaoDate,
                                    dvRequestCodeMa[iPIInfo]["product_model"].ToString(), dvRequestCodeMa[iPIInfo]["CodeNumPath"].ToString());
                                if (result.result_code == 1)
                                {
                                    string sqltmp = @"update dbo.RequestCode set ISUpload=1,IDCodeBatchNo=" + result.data + " where RequestCode_ID=" + Convert.ToInt64(dvRequestCodeMa[iPIInfo]["RequestCode_ID"]);
                                    SqlCommand cmd = new SqlCommand(sqltmp, conn);
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    string sqltmp = @"update dbo.RequestCode set ISUpload=2 " + " where RequestCode_ID=" + Convert.ToInt64(dvRequestCodeMa[iPIInfo]["RequestCode_ID"]);
                                    SqlCommand cmd = new SqlCommand(sqltmp, conn);
                                    cmd.ExecuteNonQuery();
                                    WriteLogTest("RequestCode编号为：" + dvRequestCodeMa[iPIInfo]["RequestCode_ID"].ToString() + ",IDCode接口返回信息：" + result.result_msg, "UpPIInfo");
                                }
                                conn.Close(); //关闭连接 
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                WriteLogTest(ex.Message, "UpPIInfo");
                            }
                        }
                            #endregion //循环处理每一个数据库的业务
                        Thread.Sleep(1000 * timeSpan);
                    }
                }
                catch (Exception ex)
                {
                    WriteLogTest(ex.Message, "UpPIInfo");
                }
            }
        }

        public static void WriteLogTest(string msg, string directoryName)
        {
            string errlogpath = System.Configuration.ConfigurationManager.AppSettings["ErrLogPath"];
            string test = System.Configuration.ConfigurationManager.AppSettings["test"];
            if (test == "1")
            {
                errlogpath = errlogpath + "\\" + directoryName + "\\" + DateTime.Now.ToString("yyyyMM");
                if (!Directory.Exists(errlogpath))
                {
                    Directory.CreateDirectory(errlogpath);
                }

                using (StreamWriter sw = new StreamWriter(errlogpath + "\\" + DateTime.Now.ToString("dd") + "errorLog.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "------" + msg);
                }
            }
        }
    }
}
