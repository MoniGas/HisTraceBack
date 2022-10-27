/*
 根据批量申请生成批次获取下载码内容地址，提取二维码编码，写入Code库
 2019-10-31
 刘晓杰
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Common.Tools;
using Common.Log;
using InterfaceWeb;
using LinqModel;


namespace GenCodeServiceAllInOne
{
    public static class UploadBatch
    {
        //IDCode接口地址
        static string interfaceUrl = System.Configuration.ConfigurationManager.AppSettings["interfaceUrl"].ToString().Trim();
        //授权key（用于访问接口）
        static string access_token = System.Configuration.ConfigurationManager.AppSettings["access_token"].ToString().Trim();
        //系统授权码（用于加密HTTP通信）
        static string access_token_code = System.Configuration.ConfigurationManager.AppSettings["access_token_code"].ToString().Trim();
        //下载zip解压密码
        static string zip_password = System.Configuration.ConfigurationManager.AppSettings["ZipPassword"].ToString().Trim();
        //服务运行时间间隔
        static int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
        //单表最大数据量
        static int tableMaxCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TableMaxCount"]);

        static WebClient _WebClient = null;

        public static void Start()
        {
            ThreadStart start = doUploadBatch;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }

        public static void doUploadBatch()
        {
            while (true)
            {
                try
                {
                    //生成码的配置信息表
                    string sql = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);
                    WriteLog.WriteSeriveLog(sql, "UploadBatch");
                    DataView dvGenCodeInfo = Dal.DbHelperSQL.Query(sql).Tables[0].DefaultView;
                    for (int iGenCodeInfo = 0; iGenCodeInfo < dvGenCodeInfo.Count; iGenCodeInfo++)
                    {
                        string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                            dvGenCodeInfo[iGenCodeInfo]["DataBaseIP"].ToString(),
                            dvGenCodeInfo[iGenCodeInfo]["DataBaseName"].ToString(),
                            dvGenCodeInfo[iGenCodeInfo]["DatabaseUserID"].ToString(),
                            dvGenCodeInfo[iGenCodeInfo]["DatabasePWD"].ToString());
                        WriteLog.WriteSeriveLog(conStr, "UploadBatch");

                        //下载包暂时存放路径
                        string codeZipTempPath = dvGenCodeInfo[iGenCodeInfo]["CodeDirectot"].ToString();
                        using (SqlConnection conn = new SqlConnection(conStr))
                        {
                            conn.Open();
                            try
                            {
                                sql = @" SELECT c.RequestCode_ID,c.Material_ID,c.Enterprise_Info_ID,c.TotalNum,c.RequestDate,c.[Status],
                                               c.FixedCode,c.StartNum,c.EndNum,c.[Type],c.CodeOfType,c.SCodeLength,c.[IDCodeBatchNo],e.MainCode
                                         FROM [RequestCode] as c 
                                         left join Enterprise_Info as e on c.Enterprise_Info_ID=e.Enterprise_Info_ID
                                         WHERE c.[ISDowm]=1";
                                //生成码信息
                                WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                DataSet dtConn = new DataSet();
                                SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                                commandConn.Fill(dtConn);
                                DataView dvRequest = dtConn.Tables[0].DefaultView;

                                if (dvRequest.Count > 0) 
                                {
                                    string idcodeBatchNo;
                                    for (var iRequest = 0; iRequest < dvRequest.Count; iRequest++)
                                    {
                                        idcodeBatchNo = dvRequest[iRequest]["IDCodeBatchNo"].ToString();
                                        WriteLog.WriteSeriveLog("IDCodeBatchNo：" + idcodeBatchNo, "UploadBatch");

                                        if (idcodeBatchNo == "") continue;

                                        //下载->解压->提取
                                        List<string> codeList = GetEwmCodes(dvRequest[iRequest]["MainCode"].ToString(),
                                            idcodeBatchNo,
                                            codeZipTempPath);

                                        if (null == codeList || codeList.Count == 0) continue;

                                        #region 入库
                                        using (SqlTransaction trans = conn.BeginTransaction())
                                        {
                                            try
                                            {
                                                //生成码的数量
                                                int genCodeCount = codeList.Count;
                                                //int.TryParse(dvRequest[iRequest]["TotalNum"].ToString(), out genCodeCount);
                                                int genType = int.Parse(dvRequest[iRequest]["type"].ToString());

                                                #region  1.查找路由

                                                DataSet ds = new DataSet();
                                                sql = @"select * from Route_DataBase where isuse=1 order by Route_DataBase_id desc";
                                                WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                                                command.SelectCommand.Transaction = trans;
                                                command.Fill(ds, "ds");
                                                DataView dvRounte = ds.Tables[0].DefaultView;

                                                if (dvRequest.Count == 0)
                                                {
                                                    string sqltmp = @"update dbo.Route_DataBase set isuse=1 where Route_DataBase_id=1";
                                                    WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                    SqlCommand cmd = new SqlCommand(sqltmp, conn);
                                                    cmd.Transaction = trans;
                                                    cmd.ExecuteNonQuery();
                                                    ds = new DataSet();
                                                    command = new SqlDataAdapter(sql, conn);
                                                    command.SelectCommand.Transaction = trans;
                                                    command.Fill(ds, "ds");
                                                    dvRounte = ds.Tables[0].DefaultView;
                                                }
                                                else
                                                {
                                                    if (genCodeCount + int.Parse(dvRounte[0]["lastTableMaxCount"].ToString()) > tableMaxCount)
                                                    {
                                                        //大于单表容量则使用新的路由
                                                        string sqltmp = string.Format(@"update dbo.Route_DataBase set isuse=1 where Route_DataBase_id={0}",
                                                           int.Parse(dvRounte[0]["Route_DataBase_id"].ToString()) + 1);
                                                        WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                        SqlCommand cmd = new SqlCommand(sqltmp, conn);
                                                        cmd.Transaction = trans;
                                                        cmd.ExecuteNonQuery();
                                                        command = new SqlDataAdapter(sql, conn);
                                                        command.SelectCommand.Transaction = trans;
                                                        ds = new DataSet();
                                                        command.Fill(ds, "ds");
                                                        dvRounte = ds.Tables[0].DefaultView;
                                                    }
                                                }
                                                #endregion //1.查找路由

                                                #region 2.判断码类型
                                                int endType = 0;
                                                int codeLength = 6;
                                                switch (genType)
                                                {
                                                    case (int)Common.EnumFile.GenCodeType.boxCode:
                                                        endType = (int)Common.EnumFile.CodeType.boxCode;
                                                        break;
                                                    case (int)Common.EnumFile.GenCodeType.gift:
                                                        endType = (int)Common.EnumFile.CodeType.gift;
                                                        break;
                                                    case (int)Common.EnumFile.GenCodeType.localCreate:
                                                        endType = (int)Common.EnumFile.CodeType.localSingle;
                                                        break;
                                                    case (int)Common.EnumFile.GenCodeType.localCreateBox:
                                                        endType = (int)Common.EnumFile.CodeType.localBox;
                                                        break;
                                                    case (int)Common.EnumFile.GenCodeType.localGift:
                                                        endType = (int)Common.EnumFile.CodeType.localGift;
                                                        break;
                                                    case (int)Common.EnumFile.GenCodeType.pesticides:
                                                        endType = -1; ///农药的需要单独处理
                                                        codeLength = 6;
                                                        break;
                                                    case (int)Common.EnumFile.GenCodeType.single:
                                                        endType = (int)Common.EnumFile.CodeType.single;
                                                        break;
                                                    case (int)Common.EnumFile.GenCodeType.trap:
                                                        endType = -2;///套标箱码需要单独处理
                                                        break;
                                                }
                                                #endregion //2.判断码类型

                                                #region 3.生成码
                                                ///码库链接字符串
                                                string codeConstr = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
                                                                        dvRounte[0]["DataSource"].ToString(),
                                                                        dvRounte[0]["DataBaseName"].ToString(),
                                                                        dvRounte[0]["UID"].ToString(),
                                                                        dvRounte[0]["PWD"].ToString());
                                                using (SqlConnection connCode = new SqlConnection(codeConstr))
                                                {
                                                    ///码表名称
                                                    string tableName = dvRounte[0]["TableName"].ToString();
                                                    DataSet dsCode = new DataSet();
                                                    sql = string.Format(@"select ISNULL(max(Enterprise_FWCode_ID),0)  from  {0}", tableName);
                                                    WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                    SqlDataAdapter commandCode = new SqlDataAdapter(sql, connCode);
                                                    commandCode.Fill(dsCode, "dsCode");
                                                    DataView dvCode = dsCode.Tables[0].DefaultView;
                                                    ///当前码表的的最大ID
                                                    long maxID = long.Parse(dvCode[0][0].ToString()) + 1;
                                                    WriteLog.WriteSeriveLog("创建数据集", "dsCode");

                                                    DataTable dtCode = GenCode.CreateDataTable(maxID, dvRequest[iRequest]["RequestCode_ID"].ToString());

                                                    #region 码固定内容声明及赋值

                                                    BinarySystem36 binary = new BinarySystem36();
                                                    Random rn = new Random();
                                                    long Enterprise_Info = long.Parse(dvRequest[iRequest]["Enterprise_Info_ID"].ToString());
                                                    long Material_ID = long.Parse(dvRequest[iRequest]["Material_ID"].ToString());
                                                    string FixedCode = dvRequest[iRequest]["FixedCode"].ToString();
                                                    long RequestCode_ID = long.Parse(dvRequest[iRequest]["RequestCode_ID"].ToString());
                                                    int status = 1040000008;
                                                    int beginCode = int.Parse(dvRequest[iRequest]["StartNum"].ToString());
                                                    //判断是简码还是IDcode码
                                                    int CodeOfType = int.Parse(dvRequest[iRequest]["CodeOfType"].ToString());

                                                    #endregion

                                                    //普通码
                                                    Random rnNo = new Random();
                                                    //调整随机码长度用户可控 增加代码
                                                    int SCodeLength = 1;
                                                    int.TryParse(dvRequest[iRequest]["SCodeLength"].ToString(), out SCodeLength);
                                                    if (SCodeLength == 0)
                                                    {
                                                        SCodeLength = 1;
                                                    }
                                                    //调整随机码长度用户可控 增加代码
                                                    foreach(string ewm in codeList)
                                                    {
                                                        int no = rnNo.Next(1, 999999);
                                                        DataRow dr;
                                                        dr = dtCode.NewRow();
                                                        dr["Enterprise_Info_ID"] = Enterprise_Info.ToString();
                                                        dr["Material_ID"] = Material_ID;
                                                        dr["Batch_ID"] = DBNull.Value;
                                                        dr["Dealer_ID"] = DBNull.Value;
                                                        dr["EWM"] = ewm;
                                                        dr["EWM_Info"] = DBNull.Value;
                                                        dr["FWCode"] = no.ToString().PadLeft(6, '0');
                                                        dr["ScanCount"] = 0;
                                                        dr["FWCount"] = 0;
                                                        dr["ValidateTime"] = DBNull.Value;
                                                        dr["UseTime"] = DBNull.Value;
                                                        dr["SalesTime"] = DBNull.Value;
                                                        dr["CreateDate"] = DateTime.Now;
                                                        dr["Status"] = status;
                                                        dr["BatchExt_ID"] = DBNull.Value;
                                                        dr["RequestCode_ID"] = RequestCode_ID;
                                                        dr["SalesInformation_ID"] = DBNull.Value;
                                                        dr["latitude"] = DBNull.Value;
                                                        dr["longitude"] = DBNull.Value;
                                                        dr["Type"] = endType;
                                                        dr["codeXML"] = DBNull.Value;
                                                        dtCode.Rows.Add(dr);
                                                    }

                                                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(codeConstr))
                                                    {
                                                        bulkCopy.DestinationTableName = tableName;
                                                        bulkCopy.BatchSize = dtCode.Rows.Count;
                                                        bulkCopy.BulkCopyTimeout = 600;
                                                        WriteLog.WriteSeriveLog("开始写入数据库" + dtCode.Rows.Count.ToString(), "UploadBatch");
                                                        try
                                                        {
                                                            if (dtCode != null && dtCode.Rows.Count != 0)
                                                                bulkCopy.WriteToServer(dtCode);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            WriteLog.WriteSeriveLog(ex.Message, "UploadBatch");
                                                        }
                                                        WriteLog.WriteSeriveLog("完成写入数据库", "UploadBatch");
                                                    }
                                                }
                                                #endregion //3.生成码

                                                #region 4.写入数据库状态

                                                WriteLog.WriteSeriveLog("写入数据库状态开始", "UploadBatch");

                                                sql = string.Format(@"update dbo.RequestCode set [Status]=1040000005,[ISDowm]=2 where RequestCode_ID={0}", dvRequest[iRequest]["RequestCode_ID"].ToString());
                                                WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                SqlCommand cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                cmdTop.ExecuteNonQuery();

                                                sql = string.Format(@"update RequestCode set Route_DataBase_ID={0},saleCount=0 where RequestCode_ID={1}", dvRounte[0]["Route_DataBase_id"].ToString(), dvRequest[iRequest]["RequestCode_ID"].ToString());
                                                WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                cmdTop.ExecuteNonQuery();

                                                sql = string.Format(@"update dbo.Route_DataBase set lastTableMaxCount=lastTableMaxCount+{0} where Route_DataBase_ID={1}", genCodeCount, dvRounte[0]["Route_DataBase_id"].ToString());
                                                WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                cmdTop.ExecuteNonQuery();

                                                sql = @"insert into RequestCodeSettingAdd(ID) select ID from  RequestCodeSetting R where R.ID  not in (select ID from RequestCodeSettingAdd )";
                                                WriteLog.WriteSeriveLog(sql, "UploadBatch");
                                                cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                cmdTop.ExecuteNonQuery();

                                                WriteLog.WriteSeriveLog("写入数据库状态结束", "UploadBatch");

                                                #endregion

                                                trans.Commit();
                                            }
                                            catch (Exception ex)
                                            {
                                                trans.Rollback();
                                                WriteLog.WriteSeriveLog(ex.Message, "UploadBatch");
                                            }
                                        }
                                        #endregion

                                        Thread.Sleep(1000 * timeSpan);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // trans.Rollback();
                                WriteLog.WriteSeriveLog(ex.Message, "UploadBatch");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.WriteSeriveLog(ex.Message, "UploadBatch");
                }
                Thread.Sleep(1000 * timeSpan);
            }
        }

        /// <summary>
        /// 下载->解压->提取
        /// </summary>
        /// <param name="companyMainCode">单位主码</param>
        /// <param name="batchNo">批量生成申请批次</param>
        /// <returns></returns>
        private static List<string> GetEwmCodes(string companyMainCode, string batchNo, string codePath) 
        {
            try
            {
                //请求IDCode接口
                InterFaceHisCodeFileUrlInfo codeFile = InterfaceWeb.BaseDataDAL.GetUploadBatch(companyMainCode, batchNo, zip_password);

                //测试数据
                //InterFaceHisCodeFileUrlInfo codeFile = new InterFaceHisCodeFileUrlInfo()
                //{
                //    result_code = 1,
                //    result_msg = "成功",
                //    codefileurl_info = "http://apitest.idcode.org.cn/UploadCode/20191101100301881_new.zip"
                //};

                WriteLog.WriteSeriveLog("下载包路径：" + codeFile.codefileurl_info, "UploadBatch");

                if (string.IsNullOrEmpty(codeFile.codefileurl_info))
                    return new List<string>();

                //下载
                string filePath = Path.Combine(codePath, "codezip");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string zipFileName = codeFile.codefileurl_info.Substring(codeFile.codefileurl_info.LastIndexOf("/") + 1);
                string zipFilePath = Path.Combine(filePath, zipFileName);
                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                _WebClient.DownloadFile(codeFile.codefileurl_info, zipFilePath);

                //解压
                string txtFilePath = Path.Combine(filePath, zipFileName.Replace(".zip", ".txt"));
                Common.Tools.UnZipClass.UnZip(zipFilePath, filePath, zip_password);

                //提取
                List<string> ewmCodes = new List<string>();
                if (File.Exists(txtFilePath))
                {
                    string line, code;
                    using (StreamReader sr = new StreamReader(txtFilePath, Encoding.Default))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            code = line.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1];
                            ewmCodes.Add(code);
                        }
                    }
                    //File.Delete(zipFilePath);
                    //File.Delete(txtFilePath);
                }

                return ewmCodes;

            }
            catch (Exception ex)
            {
                WriteLog.WriteSeriveLog(ex.Message, "UploadBatch");
                return null;
            }
        }
    }
}
