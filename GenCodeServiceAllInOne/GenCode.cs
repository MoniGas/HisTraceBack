/*
  向数据库中生成二维码数据
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using Common.Tools;
namespace GenCodeServiceAllInOne
{
    public static class GenCode
    {
        /// <summary>
        /// 目前不用这个生成码方法
        /// </summary>
        public static void DoGenCode()
        {
            string sql = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);
            //服务运行时间间隔
            int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
            //单表最大数据量
            int tableMaxCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TableMaxCount"]);
            while (true)
            {
                //生成码的配置信息表
                try
                {

                    sql = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);
                    WriteLogTest(sql, "GenCode");
                    DataView dvGenCodeInfo = Dal.DbHelperSQL.Query(sql).Tables[0].DefaultView;
                    for (int iGenCodeInfo = 0; iGenCodeInfo < dvGenCodeInfo.Count; iGenCodeInfo++)
                    {
                        #region 循环处理每一个数据库的业务
                        //数据库连接串
                        //WriteLogTest("1", "GenCode");
                        string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                            dvGenCodeInfo[iGenCodeInfo]["DataBaseIP"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DataBaseName"].ToString(),
                            dvGenCodeInfo[iGenCodeInfo]["DatabaseUserID"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DatabasePWD"].ToString());
                        WriteLogTest(conStr, "GenCode");
                        using (SqlConnection conn = new SqlConnection(conStr))
                        {
                            conn.Open();
                            //using (SqlTransaction trans = conn.BeginTransaction())
                            //{
                            try
                            {
                                sql = @" select top 1  SCodeLength,Material_ID,RequestCode_ID,Enterprise_Info_ID,TotalNum,RequestDate,[Status], FixedCode,StartNum,EndNum, [Type],requestCode_ID,CodeOfType 
                                         from dbo.RequestCode 
	                                     where  [Status] not in (1040000001,1040000003,1040000005,1040000009,1040000010,1040000011) ";
                                //生成码信息
                                WriteLogTest(sql, "GenCode");
                                DataSet dtConn = new DataSet();
                                SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                                //commandConn.SelectCommand.Transaction = trans;
                                commandConn.Fill(dtConn);
                                DataView dvRequest = dtConn.Tables[0].DefaultView;
                                if (dvRequest.Count > 0)
                                {
                                    for (int iRequest = 0; iRequest < dvRequest.Count; iRequest++)
                                    {
                                        using (SqlTransaction trans = conn.BeginTransaction())
                                        {
                                            try
                                            {
                                                //生成码的数量
                                                int genCodeCount = 0;
                                                int.TryParse(dvRequest[iRequest]["TotalNum"].ToString(), out genCodeCount);
                                                int genType = int.Parse(dvRequest[iRequest]["type"].ToString());
                                                //WriteLogTest("查找路由", "GenCode");
                                                #region  1.查找路由
                                                DataSet ds = new DataSet();
                                                sql = @"select * from Route_DataBase where isuse=1 order by Route_DataBase_id desc";
                                                WriteLogTest(sql, "GenCode");
                                                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                                                //WriteLogTest("查找路由1", "GenCode");
                                                command.SelectCommand.Transaction = trans;
                                                //WriteLogTest("查找路由1.1", "GenCode");
                                                command.Fill(ds, "ds");
                                                //WriteLogTest("查找路由2", "GenCode");
                                                DataView dvRounte = ds.Tables[0].DefaultView;
                                                //WriteLogTest("查找路由2.5" + dvRequest.Count.ToString(), "GenCode");
                                                if (dvRequest.Count == 0)
                                                {
                                                    string sqltmp = @"update dbo.Route_DataBase set isuse=1 where Route_DataBase_id=1";
                                                    WriteLogTest(sql, "GenCode");
                                                    SqlCommand cmd = new SqlCommand(sqltmp, conn);
                                                    cmd.Transaction = trans;
                                                    //WriteLogTest("查找路由3", "GenCode");
                                                    cmd.ExecuteNonQuery();
                                                    ds = new DataSet();
                                                    command = new SqlDataAdapter(sql, conn);
                                                    command.SelectCommand.Transaction = trans;
                                                    command.Fill(ds, "ds");
                                                    //WriteLogTest("查找路由4", "GenCode");
                                                    dvRounte = ds.Tables[0].DefaultView;
                                                }
                                                else
                                                {
                                                    if (genCodeCount + int.Parse(dvRounte[0]["lastTableMaxCount"].ToString()) > tableMaxCount)
                                                    {
                                                        //大于单表容量则使用新的路由
                                                        string sqltmp = string.Format(@"update dbo.Route_DataBase set isuse=1 where Route_DataBase_id={0}",
                                                           int.Parse(dvRounte[0]["Route_DataBase_id"].ToString()) + 1);
                                                        WriteLogTest(sql, "GenCode");
                                                        SqlCommand cmd = new SqlCommand(sqltmp, conn);
                                                        cmd.Transaction = trans;
                                                        //WriteLogTest("查找路由5", "GenCode");
                                                        cmd.ExecuteNonQuery();
                                                        //WriteLogTest("查找路由6", "GenCode");
                                                        command = new SqlDataAdapter(sql, conn);
                                                        command.SelectCommand.Transaction = trans;
                                                        ds = new DataSet();
                                                        command.Fill(ds, "ds");
                                                        dvRounte = ds.Tables[0].DefaultView;
                                                    }
                                                }
                                                #endregion //1.查找路由
                                                //WriteLogTest("判断码类型", "GenCode");
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
                                                //WriteLogTest("生成码", "GenCode");
                                                #region 3.生成码
                                                ///码库链接字符串
                                                string codeConstr = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
                                                 dvRounte[0]["DataSource"].ToString(), dvRounte[0]["DataBaseName"].ToString(),
                                                 dvRounte[0]["UID"].ToString(), dvRounte[0]["PWD"].ToString());
                                                //WriteLogTest(codeConstr, "GenCode");
                                                using (SqlConnection connCode = new SqlConnection(codeConstr))
                                                {
                                                    ///码表名称
                                                    string tableName = dvRounte[0]["TableName"].ToString();
                                                    DataSet dsCode = new DataSet();
                                                    sql = string.Format(@"select ISNULL(max(Enterprise_FWCode_ID),0)  from  {0}", tableName);
                                                    WriteLogTest(sql, "GenCode");
                                                    SqlDataAdapter commandCode = new SqlDataAdapter(sql, connCode);
                                                    // command.SelectCommand.Transaction = trans; 
                                                    commandCode.Fill(dsCode, "dsCode");
                                                    DataView dvCode = dsCode.Tables[0].DefaultView;
                                                    ///当前码表的的最大ID
                                                    long maxID = long.Parse(dvCode[0][0].ToString()) + 1;
                                                    WriteLogTest("创建数据集", "dsCode");
                                                    DataTable dtCode = CreateDataTable(maxID, dvRequest[iRequest]["RequestCode_ID"].ToString());
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
                                                    #region 3.1 农药码
                                                    if (endType == -1)
                                                    {
                                                        List<int> codeLst = new List<int>();
                                                        Random rnCode = new Random();
                                                        Random rnNo = new Random();
                                                        while (beginCode <= int.Parse(dvRequest[iRequest]["EndNum"].ToString()))
                                                        {
                                                            //  Random rnNo = new Random();
                                                            int no = rnNo.Next(1000, 999999);
                                                            DataRow dr;
                                                            dr = dtCode.NewRow();
                                                            dr["Enterprise_Info_ID"] = Enterprise_Info.ToString();
                                                            dr["Material_ID"] = Material_ID;
                                                            dr["Batch_ID"] = DBNull.Value;
                                                            dr["Dealer_ID"] = DBNull.Value;
                                                            int code = rnCode.Next(1, 999999);
                                                            while (codeLst.Contains(code))
                                                            {
                                                                code = rnCode.Next(1, 999999);

                                                            }
                                                            codeLst.Add(code);
                                                            dr["EWM"] = FixedCode + code.ToString().PadLeft(codeLength, '0');
                                                            beginCode++;
                                                            dr["EWM_Info"] = null;
                                                            dr["FWCode"] = no.ToString().PadLeft(6, '0');
                                                            dr["ScanCount"] = 0;
                                                            dr["FWCount"] = 0;
                                                            dr["ValidateTime"] = DBNull.Value;
                                                            dr["UseTime"] = DBNull.Value;
                                                            dr["SalesTime"] = DBNull.Value;
                                                            dr["CreateDate"] = DateTime.Now;
                                                            dr["Status"] = status;
                                                            dr["BatchExt_ID"] = DBNull.Value;
                                                            dr["SalesInformation_ID"] = DBNull.Value;
                                                            dr["RequestCode_ID"] = RequestCode_ID;
                                                            dr["latitude"] = DBNull.Value;
                                                            dr["longitude"] = DBNull.Value;
                                                            dr["Type"] = (int)Common.EnumFile.CodeType.pesticides;
                                                            dr["codeXML"] = DBNull.Value;
                                                            dtCode.Rows.Add(dr);
                                                        }
                                                    }
                                                    #endregion
                                                    #region 3.2 套标码
                                                    else if (endType == -2)
                                                    {
                                                        int PackCount = 0;
                                                        ds = new DataSet();
                                                        sql = string.Format(@"select Specifications  from  RequestCode where RequestCode_ID ={0}", dvRequest[iRequest]["RequestCode_ID"].ToString());
                                                        WriteLogTest(sql, "GenCode");
                                                        command = new SqlDataAdapter(sql, conn);
                                                        command.SelectCommand.Transaction = trans;
                                                        command.Fill(ds);
                                                        PackCount = int.Parse(ds.Tables[0].DefaultView[0][0].ToString());
                                                        Random rnNo = new Random();
                                                        //调整随机码长度用户可控 增加代码
                                                        int SCodeLength = 1;
                                                        int.TryParse(dvRequest[iRequest]["SCodeLength"].ToString(), out SCodeLength);
                                                        if (SCodeLength == 0)
                                                        {
                                                            SCodeLength = 1;
                                                        }
                                                        //调整随机码长度用户可控 增加代码
                                                        while (beginCode <= int.Parse(dvRequest[iRequest]["endNum"].ToString()))
                                                        {
                                                            string xmlStr = string.Format("<specification><boxcode codeSum=\"{0}\" tableid=\"{1}\" value=\"{2}\" />",
                                                                PackCount + 1, 0, FixedCode + "." + beginCode.ToString().PadLeft(6, '0') + ".4");
                                                            for (int k = beginCode + 1; k <= beginCode + PackCount; k++)
                                                            {
                                                                xmlStr += string.Format("<bottlecode id=\"{0}\" tableid=\"{1}\" value=\"{2}\" />",
                                                                    k, 0, FixedCode + "." + k.ToString().PadLeft(6, '0') + ".3");
                                                            }
                                                            if (CodeOfType == (int)Common.EnumFile.CodeOfType.SCode)
                                                            {
                                                                xmlStr = string.Format("<specification><boxcode codeSum=\"{0}\" tableid=\"{1}\" value=\"{2}\" />",
                                                                PackCount + 1, 0,FixedCode + binary.gen36No(beginCode, 4) + GenRandCode(SCodeLength));// SCodeLength位随机数;
                                                                for (int k = beginCode + 1; k <= beginCode + PackCount; k++)
                                                                {
                                                                    xmlStr += string.Format("<bottlecode id=\"{0}\" tableid=\"{1}\" value=\"{2}\" />",
                                                                        k, 0, FixedCode + binary.gen36No(beginCode, 4) + GenRandCode(SCodeLength));// SCodeLength位随机数;
                                                                }
                                                            }
                                                            xmlStr += "</specification>";
                                                            int no = rnNo.Next(1, 999999);
                                                            DataRow dr;
                                                            dr = dtCode.NewRow();
                                                            dr["Enterprise_Info_ID"] = Enterprise_Info.ToString();
                                                            dr["Material_ID"] = Material_ID;
                                                            dr["Batch_ID"] = DBNull.Value;
                                                            dr["Dealer_ID"] = DBNull.Value;
                                                            if (CodeOfType == (int)Common.EnumFile.CodeOfType.IDCode)
                                                            {
                                                                dr["EWM"] = FixedCode + "." + beginCode.ToString().PadLeft(codeLength, '0') + ".4";
                                                            }
                                                            else
                                                            {
                                                                dr["EWM"] = FixedCode + binary.gen36No(beginCode, 4) + GenRandCode(SCodeLength);// SCodeLength位随机数;
                                                            }
                                                            beginCode++;
                                                            dr["EWM_Info"] = null;
                                                            dr["FWCode"] = no.ToString().PadLeft(6, '0');
                                                            dr["ScanCount"] = 0;
                                                            dr["FWCount"] = 0;
                                                            dr["ValidateTime"] = DBNull.Value;
                                                            dr["UseTime"] = DBNull.Value;
                                                            dr["SalesTime"] = DBNull.Value;
                                                            dr["CreateDate"] = DateTime.Now;
                                                            dr["Status"] = status;
                                                            dr["BatchExt_ID"] = DBNull.Value;
                                                            dr["SalesInformation_ID"] = DBNull.Value;
                                                            dr["latitude"] = DBNull.Value;
                                                            dr["longitude"] = DBNull.Value;
                                                            dr["RequestCode_ID"] = RequestCode_ID;
                                                            dr["Type"] = (int)Common.EnumFile.CodeType.bGroup;
                                                            dr["codeXML"] = xmlStr;
                                                            dtCode.Rows.Add(dr);
                                                            for (int m = 0; m < PackCount; m++)
                                                            {
                                                                //  Random rnNoNew = new Random();
                                                                int noNew = rnNo.Next(1, 999999);
                                                                dr = dtCode.NewRow();
                                                                dr["Enterprise_Info_ID"] = Enterprise_Info.ToString();
                                                                dr["Material_ID"] = Material_ID;
                                                                dr["Batch_ID"] = DBNull.Value;
                                                                dr["Dealer_ID"] = DBNull.Value;
                                                                if (CodeOfType == (int)Common.EnumFile.CodeOfType.IDCode)
                                                                {
                                                                    dr["EWM"] = FixedCode + "." + beginCode.ToString().PadLeft(codeLength, '0') + ".3";
                                                                }
                                                                else
                                                                {
                                                                    dr["EWM"] = FixedCode + binary.gen36No(beginCode, 4) + GenRandCode(SCodeLength);// SCodeLength位随机数;
                                                                }
                                                                beginCode++;
                                                                dr["EWM_Info"] = null;
                                                                dr["FWCode"] = noNew.ToString().PadLeft(6, '0');
                                                                dr["ScanCount"] = 0;
                                                                dr["FWCount"] = 0;
                                                                dr["ValidateTime"] = DBNull.Value;
                                                                dr["UseTime"] = DBNull.Value;
                                                                dr["SalesTime"] = DBNull.Value;
                                                                dr["CreateDate"] = DateTime.Now;
                                                                dr["Status"] = status;
                                                                dr["BatchExt_ID"] = DBNull.Value;
                                                                dr["SalesInformation_ID"] = DBNull.Value;
                                                                dr["latitude"] = DBNull.Value;
                                                                dr["longitude"] = DBNull.Value;
                                                                dr["RequestCode_ID"] = RequestCode_ID;
                                                                dr["Type"] = (int)Common.EnumFile.CodeType.bSingle;
                                                                dr["codeXML"] = xmlStr;
                                                                dtCode.Rows.Add(dr);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    #region 3.3 普通码
                                                    else
                                                    {
                                                        Random rnNo = new Random();
                                                        //调整随机码长度用户可控 增加代码
                                                        int SCodeLength = 1;
                                                        int.TryParse(dvRequest[iRequest]["SCodeLength"].ToString(), out SCodeLength);
                                                        if (SCodeLength == 0)
                                                        {
                                                            SCodeLength = 1;
                                                        }
                                                        //调整随机码长度用户可控 增加代码
                                                        for (long i = maxID; i < maxID + genCodeCount; i++)
                                                        {
                                                            int no = rnNo.Next(1, 999999);
                                                            DataRow dr;
                                                            dr = dtCode.NewRow();
                                                            dr["Enterprise_Info_ID"] = Enterprise_Info.ToString();
                                                            dr["Material_ID"] = Material_ID;
                                                            dr["Batch_ID"] = DBNull.Value;
                                                            dr["Dealer_ID"] = DBNull.Value;
                                                            if (CodeOfType == (int)Common.EnumFile.CodeOfType.IDCode)
                                                            {
                                                                dr["EWM"] = FixedCode + "." + beginCode.ToString().PadLeft(codeLength, '0') + "." + endType.ToString();
                                                            }
                                                            else
                                                            {
                                                                //调整随机码长度用户可控 调整代码
                                                                dr["EWM"] = FixedCode + binary.gen36No(beginCode, 4) + GenRandCode(SCodeLength);// SCodeLength位随机数;
                                                                //调整随机码长度用户可控 调整代码
                                                            }
                                                            beginCode++;
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
                                                    }
                                                    #endregion
                                                    SqlBulkCopy bulkCopy = new SqlBulkCopy(codeConstr);
                                                    bulkCopy.DestinationTableName = tableName;
                                                    bulkCopy.BatchSize = dtCode.Rows.Count;
                                                    bulkCopy.BulkCopyTimeout = 600;
                                                    WriteLogTest("开始写入数据库" + dtCode.Rows.Count.ToString(), "GenCode");
                                                    try
                                                    {
                                                        if (dtCode != null && dtCode.Rows.Count != 0)
                                                            bulkCopy.WriteToServer(dtCode);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        WriteLog(ex.Message, "GenCode");
                                                    }
                                                    WriteLogTest("完成写入数据库", "GenCode");
                                                }
                                                #endregion //3.生成码
                                                #region 4.写入数据库状态
                                                WriteLogTest("写入数据库状态", "GenCode");
                                                sql = string.Format(@"update dbo.RequestCode set [Status]=1040000005 where RequestCode_ID={0}", dvRequest[iRequest]["RequestCode_ID"].ToString());
                                                WriteLogTest(sql, "GenCode");
                                                SqlCommand cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                cmdTop.ExecuteNonQuery();
                                                sql = string.Format(@"update RequestCode set Route_DataBase_ID={0},saleCount=0 where RequestCode_ID={1}", dvRounte[0]["Route_DataBase_id"].ToString(), dvRequest[iRequest]["RequestCode_ID"].ToString());
                                                WriteLogTest(sql, "GenCode");
                                                cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                cmdTop.ExecuteNonQuery();
                                                sql = string.Format(@"update dbo.Route_DataBase set lastTableMaxCount=lastTableMaxCount+{0} where Route_DataBase_ID={1}", genCodeCount, dvRounte[0]["Route_DataBase_id"].ToString());
                                                cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                WriteLogTest(sql, "GenCode");
                                                cmdTop.ExecuteNonQuery();

                                                sql = @"insert into RequestCodeSettingAdd(ID) select ID from  RequestCodeSetting R where R.ID  not in (select ID from RequestCodeSettingAdd )";
                                                cmdTop = new SqlCommand(sql, conn);
                                                cmdTop.Transaction = trans;
                                                WriteLogTest(sql, "GenCode");
                                                cmdTop.ExecuteNonQuery();

                                                #endregion
                                                trans.Commit();
                                            }
                                            catch (Exception ex)
                                            {
                                                trans.Rollback();
                                                WriteLog(ex.Message, "GenCode");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // trans.Rollback();
                                WriteLog(ex.Message, "GenCode");
                            }
                        }
                    }
                        #endregion //循环处理每一个数据库的业务
                    Thread.Sleep(1000 * timeSpan);
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message, "GenCode");
                }
            }
        }
        public static DataTable CreateDataTable(long beginID, string name)
        {
            DataTable tblDatas = new DataTable(name);
            DataColumn dc = tblDatas.Columns.Add("Enterprise_FWCode_ID", Type.GetType("System.Int64"));
            dc.AutoIncrement = true;//自动增加
            dc.AutoIncrementSeed = beginID;//起始为1
            dc.AutoIncrementStep = 1;//步长为1
            dc.AllowDBNull = false;//
            tblDatas.Columns.Add("Enterprise_Info_ID", Type.GetType("System.String"));
            tblDatas.Columns.Add("Batch_ID", Type.GetType("System.Int64"));
            tblDatas.Columns.Add("Material_ID", Type.GetType("System.Int64"));
            tblDatas.Columns.Add("Dealer_ID", Type.GetType("System.Int64"));
            tblDatas.Columns.Add("RequestCode_ID", Type.GetType("System.Int64"));
            tblDatas.Columns.Add("EWM", Type.GetType("System.String"));
            tblDatas.Columns.Add("EWM_Info", Type.GetType("System.String"));
            tblDatas.Columns.Add("FWCode", Type.GetType("System.String"));
            tblDatas.Columns.Add("ScanCount", Type.GetType("System.Int32"));
            tblDatas.Columns.Add("FWCount", Type.GetType("System.Int32"));
            tblDatas.Columns.Add("ValidateTime", Type.GetType("System.DateTime"));
            tblDatas.Columns.Add("UseTime", Type.GetType("System.DateTime"));
            tblDatas.Columns.Add("SalesTime", Type.GetType("System.DateTime"));
            tblDatas.Columns.Add("Status", Type.GetType("System.Int32"));
            tblDatas.Columns.Add("CreateDate", Type.GetType("System.DateTime"));
            tblDatas.Columns.Add("SalesInformation_ID", Type.GetType("System.Int64"));

            tblDatas.Columns.Add("BatchExt_ID", Type.GetType("System.Int64"));
            tblDatas.Columns.Add("latitude", Type.GetType("System.String"));
            tblDatas.Columns.Add("longitude", Type.GetType("System.String"));
            tblDatas.Columns.Add("Type", Type.GetType("System.Int32"));
            tblDatas.Columns.Add("codeXML", Type.GetType("System.String"));
            ////WriteLog("3");
            return tblDatas;
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
        /// <summary>
        /// 生成码服务
        /// </summary>
        public static void start()
        {
            ThreadStart start = DoGenCode;
            Thread th = new Thread(start);
            th.IsBackground = true;
            th.Start();
        }

        /// <summary>
        /// 生成给定长度的36进制加密串
        /// </summary>
        /// <param name="codeLength">长度</param>
        /// <returns></returns>
        public static String GenRandCode(int codeLength)
        {
            List<string> lst = new List<string>();
            
            BinarySystem36 binary = new BinarySystem36();
            for (int i = 0; i < codeLength; i++)
            {
                    lst.Add(binary.dNo[new Random(Guid.NewGuid().GetHashCode()).Next(0, 35)].ToString());
            }
            return string.Join("",lst.ToArray());
        }
    }
}
