/*
  将数据库中二维码生成文本文件供用户下载
 */
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;

namespace GenCodeServiceAllInOne
{
    public class GenFile
    {
        /// <summary>
        /// 生成码包服务
        /// </summary>
        public static void start()
        {
            ThreadStart start = doGenFile;
            Thread th = new Thread(start);
            th.IsBackground = true;
            th.Start();
        }
        public static void WriteLog(string msg, string directoryName)
        {
            string errlogpath = System.Configuration.ConfigurationManager.AppSettings["ErrLogPath"];
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
        public static void WriteLogTest(string msg, string directoryName)
        {
            string errlogpath = System.Configuration.ConfigurationManager.AppSettings["ErrLogPath"];
            errlogpath = errlogpath + "\\" + directoryName + "\\" + DateTime.Now.ToString("yyyyMM");
            string test = System.Configuration.ConfigurationManager.AppSettings["test"];
            if (test == "1")
            {
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
        //旧生成码包服务，不用状态
        public static void doGenFile()
        {
            string sqlRoute = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);
            ///服务运行时间间隔
            int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
            WriteLog("开始运行", "GenFile");
            while (true)
            {
                DataView dvGenCodeInfo = Dal.DbHelperSQL.Query(sqlRoute).Tables[0].DefaultView;
                for (int iGenCodeInfo = 0; iGenCodeInfo < dvGenCodeInfo.Count; iGenCodeInfo++)
                {
                    #region 循环处理每一个数据库的业务
                    string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                       dvGenCodeInfo[iGenCodeInfo]["DataBaseIP"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DataBaseName"].ToString(),
                       dvGenCodeInfo[iGenCodeInfo]["DatabaseUserID"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DatabasePWD"].ToString());
                    WriteLogTest(conStr, "GenFile");
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                string sql = "select * from RequestCodeSettingAdd where state<2";
                                DataSet ds = new DataSet();
                                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                                //WriteLogTest("查找设置1", "GenFile");
                                WriteLogTest(sql, "GenFile");
                                command.SelectCommand.Transaction = trans;
                                //WriteLogTest("查找设置1.1", "GenFile");
                                command.Fill(ds, "ds");
                                ///码配置信息
                                DataView dvCodeSettint = ds.Tables[0].DefaultView;
                                //WriteLogTest("查找设置1.1记录数" + dvCodeSettint.Count.ToString(), "GenFile");
                                for (int iCodeSettng = 0; iCodeSettng < dvCodeSettint.Count; iCodeSettng++)
                                {
                                    sql = string.Format(@"select  S.MaterialID,S.Count,S.EnterpriseId,S.beginCode,S.endCode,
                                      C.RequestCode_ID,C.Route_DataBase_ID,C.type,C.CodeOfType from  RequestCodeSetting S left join RequestCode  C on s.RequestID=C.RequestCode_ID
                                        where S.ID={0}", dvCodeSettint[iCodeSettng]["ID"].ToString());
                                    ds = new DataSet();
                                    WriteLogTest(sql, "GenFile");
                                    command = new SqlDataAdapter(sql, conn);
                                    command.SelectCommand.Transaction = trans;
                                    command.Fill(ds, "ds" + dvCodeSettint[iCodeSettng]["ID"]);
                                    ///生成码信息
                                    DataView dvInfo = ds.Tables[0].DefaultView;
                                    sql = string.Format(@"select  * from  Material where Material_ID={0}",
                                        dvInfo[0]["MaterialID"].ToString());
                                    ds = new DataSet();
                                    WriteLogTest(sql, "GenFile");
                                    command = new SqlDataAdapter(sql, conn);
                                    command.SelectCommand.Transaction = trans;
                                    command.Fill(ds, "ds" + dvInfo[0]["MaterialID"]);
                                    ///产品信息
                                    DataView dvMaterial = ds.Tables[0].DefaultView;
                                    sql = string.Format(@"select  * from  Route_DataBase where Route_DataBase_ID= {0}",
                                        dvInfo[0]["Route_DataBase_ID"].ToString());
                                    ds = new DataSet();
                                    WriteLogTest(sql, "GenFile");
                                    command = new SqlDataAdapter(sql, conn);
                                    command.SelectCommand.Transaction = trans;
                                    command.Fill(ds, "ds" + dvInfo[0]["Route_DataBase_ID"]);
                                    //路由信息
                                    DataView dvRounte = ds.Tables[0].DefaultView;
                                    string codeConstr = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
                                              dvRounte[0]["DataSource"].ToString(), dvRounte[0]["DataBaseName"].ToString(),
                                              dvRounte[0]["UID"].ToString(), dvRounte[0]["PWD"].ToString());
                                    WriteLogTest(codeConstr, "GenFile");
                                    using (SqlConnection connCode = new SqlConnection(codeConstr))
                                    {
                                        //码表名称
                                        string tableName = dvRounte[0]["TableName"].ToString();
                                        WriteLogTest(tableName, "GenFile");
                                        string sqltmp = string.Format(@"select top 1 *  from {0} where RequestCode_ID={1} ",
                                        tableName, dvInfo[0]["RequestCode_ID"].ToString());
                                        WriteLogTest(sqltmp, "GenFile");
                                        SqlCommand _cmd = new SqlCommand();
                                        _cmd.Connection = connCode;
                                        DataSet _ds = new DataSet();
                                        SqlDataAdapter _command = new SqlDataAdapter(sqltmp, connCode);
                                        _command.Fill(_ds, "ds");
                                        DataView dvTmp = _ds.Tables[0].DefaultView;
                                        //string beginNO=dvInfo[0]["beginCode"].ToString();
                                        //string endNO = dvInfo[0]["endCode"].ToString();
                                        string endID, beginID;
                                        if (dvTmp[0]["type"].ToString() == "12")
                                        {
                                            sql = string.Format(@"select EWM,FWCode,Type from {0} where RequestCode_ID={1}  ",
                                            tableName, dvInfo[0]["RequestCode_ID"].ToString());
                                        }
                                        else
                                        {
                                            sqltmp = string.Format(@"select top 1 Enterprise_FWCode_ID from {0} where RequestCode_ID={1}   order by Enterprise_FWCode_ID ",
                                            tableName, dvInfo[0]["RequestCode_ID"].ToString());
                                            WriteLogTest(sqltmp, "GenFile");
                                            _cmd = new SqlCommand();
                                            _cmd.Connection = connCode;
                                            _ds = new DataSet();
                                            _command = new SqlDataAdapter(sqltmp, connCode);
                                            _command.Fill(_ds, "ds");
                                            dvTmp = _ds.Tables[0].DefaultView;
                                            beginID = dvTmp[0][0].ToString();
                                            sqltmp = string.Format(@"select  top 1 Enterprise_FWCode_ID from {0} where RequestCode_ID={1}  order by Enterprise_FWCode_ID  desc ",
                                                tableName, dvInfo[0]["RequestCode_ID"].ToString());
                                            WriteLogTest(sqltmp, "GenFile");
                                            _cmd = new SqlCommand();
                                            _cmd.Connection = connCode;
                                            _ds = new DataSet();
                                            _command = new SqlDataAdapter(sqltmp, connCode);
                                            _command.Fill(_ds, "ds");
                                            dvTmp = _ds.Tables[0].DefaultView;
                                            endID = dvTmp[0][0].ToString();
                                            sql = string.Format(@"select EWM,FWCode,Type
                                                from  {0} a where a.RequestCode_ID={1}   and a.Enterprise_FWCode_ID between {2} and  {3}  order by  Enterprise_FWCode_ID ",
                                              tableName, dvInfo[0]["RequestCode_ID"].ToString(), beginID, endID);
                                        }
                                        DataSet dsCode = new DataSet();

                                        WriteLogTest(sql, "GenFile");
                                        SqlDataAdapter commandCode = new SqlDataAdapter(sql, connCode);
                                        commandCode.Fill(dsCode, "dsCode");
                                        DataView dvCode = dsCode.Tables[0].DefaultView;
                                        //StringBuilder sbCode = new StringBuilder();
                                        string codeInfo = string.Empty;
                                        int Gentype = int.Parse(dvInfo[0]["Type"].ToString());
                                        string ncpURL = dvGenCodeInfo[iGenCodeInfo]["FixURL"].ToString();
                                        //新加IDCode码网址前缀
                                        string idCodeEWMURL = dvGenCodeInfo[iGenCodeInfo]["IDCodeFixURL"].ToString();
                                        //新加区分是简码/IDCode码/农药码
                                        int codeOfType = int.Parse(dvInfo[0]["CodeOfType"].ToString());
                                        string baseFilePatch = dvGenCodeInfo[iGenCodeInfo]["CodeDirectot"].ToString();
                                        string filePath = string.Format(baseFilePatch + "\\{0}\\{1}", dvInfo[0]["EnterpriseId"].ToString(),
                                        dvCodeSettint[iCodeSettng]["ID"].ToString());
                                        if (!Directory.Exists(filePath))
                                        {
                                            Directory.CreateDirectory(filePath);
                                        }
                                        string fileName = dvCodeSettint[iCodeSettng]["ID"] + ".txt";
                                        string zipfileName = dvCodeSettint[iCodeSettng]["ID"] + ".zip";
                                        if (File.Exists(filePath + "\\" + fileName))
                                        {
                                            File.Delete(filePath + "\\" + fileName);
                                        }
                                        for (int k = 0; k < dvCode.Count; k++)
                                        {
                                            if (Gentype == 9 && codeOfType != 1)//套标码(codeOfType=1为IDCode码2为简码3为农药码)
                                            {
                                                if (dvCode[k]["type"].ToString() == "4")
                                                {
                                                    codeInfo = dvMaterial[0]["MaterialName"] + "\t" + ncpURL
                                                    + dvCode[k]["EWM"] + "\t" + dvCode[k]["FWCode"] + "\t" + "箱码";
                                                }
                                                else if (dvCode[k]["type"].ToString() == "3")
                                                {
                                                    codeInfo = dvMaterial[0]["MaterialName"] + "\t" + ncpURL
                                                    + dvCode[k]["EWM"] + "\t" + dvCode[k]["FWCode"] + "\t" + "产品码";
                                                }
                                            }
                                            else if (Gentype == 9 && codeOfType == 1)//IDCode套标码
                                            {
                                                if (dvCode[k]["type"].ToString() == "4")
                                                {
                                                    codeInfo = dvMaterial[0]["MaterialName"] + "\t" + idCodeEWMURL
                                                    + dvCode[k]["EWM"] + "\t" + dvCode[k]["FWCode"] + "\t" + "箱码";
                                                }
                                                else if (dvCode[k]["type"].ToString() == "3")
                                                {
                                                    codeInfo = dvMaterial[0]["MaterialName"] + "\t" + idCodeEWMURL
                                                    + dvCode[k]["EWM"] + "\t" + dvCode[k]["FWCode"] + "\t" + "产品码";
                                                }
                                            }
                                            else if (codeOfType == 1)//IDCode码
                                            {
                                                codeInfo = dvMaterial[0]["MaterialName"] + "\t" + idCodeEWMURL 
                                                    + dvCode[k]["EWM"] + "\t" + dvCode[k]["FWCode"];
                                            }
                                            else
                                            {
                                                codeInfo = dvMaterial[0]["MaterialName"] + "\t" + ncpURL
                                                    + dvCode[k]["EWM"] + "\t" + dvCode[k]["FWCode"];
                                            }
                                            //sbCode.AppendLine(codeInfo);
                                            using (StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true))
                                            {
                                                if (k == 0)
                                                {
                                                    sw.WriteLine("每一行为一个二维码数据，自左向右三列分别为：产品名称、二维码内容和防伪校验码，印刷二维码时注意不要弄错！");
                                                    sw.WriteLine(codeInfo);
                                                }
                                                else
                                                {
                                                    sw.WriteLine(codeInfo);
                                                }
                                            }
                                        }

                                        Common.Tools.ZipClass.Zip(filePath + "\\" + fileName, filePath + "\\" + fileName.Replace("txt", "zip"), "");
                                        sql = string.Format(@"update RequestCodeSettingAdd set FileURL='{0}',
                                               WebURL='{1}',State={2} where ID={3}", filePath + "\\" + zipfileName,
                                        string.Format("/CodeFile/{0}/{1}", dvInfo[0]["EnterpriseId"].ToString(),
                                           dvCodeSettint[iCodeSettng]["ID"].ToString()) + "/" + zipfileName, 2, dvCodeSettint[iCodeSettng]["ID"].ToString());
                                        WriteLogTest(sql, "GenFile");
                                        SqlCommand cmd = new SqlCommand(sql, conn);
                                        cmd.Transaction = trans;
                                        cmd.ExecuteNonQuery();
                                        File.Delete(filePath + "\\" + fileName);
                                    }
                                }
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                WriteLog(ex.Message, "GenFile");
                                continue;
                            }
                        }
                    }
                    #endregion
                }
                Thread.Sleep(1000 * timeSpan);
            }
        }
    }
}
