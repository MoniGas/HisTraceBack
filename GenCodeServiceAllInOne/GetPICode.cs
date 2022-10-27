using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using LinqModel;
using InterfaceWeb;
using Dal;
using System.Threading;
using Common;

namespace GenCodeServiceAllInOne
{
    /// <summary>
    /// 获取PI码明细
    /// </summary>
    public static class GetPICode
    {
        public static void Start()
        {
            ThreadStart start = GetCodeInfo;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }
        /// <summary>
        /// 2021-9-22
        /// 企业用码量如果超过限制无法获取码，改为从码包读取生成码
        /// </summary>
        public static void GetCodeInfo()
        {
            Log.WriteLog("-----------------GenPICode---------------------", "GenPICode");
            while (true)
            {
                //生成码的配置信息表
                try
                {
                    //服务运行时间间隔
                    int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
                    //连接库
                    string conStr = System.Configuration.ConfigurationManager.AppSettings["Constr"];
                    //单表最大数据量
                    int tableMaxCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TableMaxCount"]);
                    // Log.WriteLog("-----------------GenPICode---------------1------", "GenPICode");
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        conn.Open();
                        //查询未下载码明细的PI记录
                        // Log.WriteLog("-----------------GenPICode---------------2------", "GenPICode");
                        //string sql = @" select * from dbo.RequestCode  where  [Status]='1040000008'  and IDCodeBatchNo is not null";
                        string sql = @" select * from dbo.RequestCode  where  [Status]='1040000008'   and Route_DataBase_ID is null  ";
                        DataSet dtConn = new DataSet();
                        SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
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
                                        int genCodeCount = 0;
                                        //Log.WriteLog("-----------------GenPICode---------3------------", "GenPICode");
                                        #region 1.调用接口获取PI码明细
                                        RequestCodeMaDAL dal = new RequestCodeMaDAL();
                                        ListPICode piList = new ListPICode();
                                        if (!string.IsNullOrEmpty(dvRequest[iRequest]["IDCodeBatchNo"].ToString()))
                                        {
                                            piList = dal.GetPICode(dvRequest[iRequest]["RequestCode_ID"].ToString());
                                            genCodeCount = piList.data.Count();
                                        }
                                        else if (!string.IsNullOrEmpty(dvRequest[iRequest]["CodeNumPath"].ToString()))
                                        {
                                            if (dvRequest[iRequest]["CodingClientType"].ToString() == "1")//Ma码
                                            {
                                                #region 2021-9-22改 如果没有通过IDcode平台取到信息就读取码包文件
                                                piList.data = new List<string>();
                                                string filePath = dvRequest[iRequest]["CodeNumPath"].ToString();
                                                string fixCode = dvRequest[iRequest]["FixedCode"].ToString();
                                                string startdate = dvRequest[iRequest]["startdate"].ToString();
                                                string ShiXiaoDate = dvRequest[iRequest]["ShiXiaoDate"].ToString();
                                                string YouXiaoDate = dvRequest[iRequest]["YouXiaoDate"].ToString();
                                                string ShengChanPH = dvRequest[iRequest]["ShengChanPH"].ToString();
                                                string dbatchnumber = dvRequest[iRequest]["dbatchnumber"].ToString();
                                                string sign = string.Empty;
                                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                                {
                                                    using (StreamReader reader = new StreamReader(fs, Encoding.Default))
                                                    {
                                                        while ((sign = reader.ReadLine()) != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(sign))
                                                            {
                                                                UDITool tool = new UDITool();
                                                                string code = tool.GenUDI(fixCode, sign, startdate, ShiXiaoDate, YouXiaoDate, ShengChanPH, dbatchnumber);
                                                                if (!string.IsNullOrEmpty(code))
                                                                {
                                                                    piList.data.Add(code);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            else if(dvRequest[iRequest]["CodingClientType"].ToString() == "2")//GS1码
                                            {
                                                piList.data = new List<string>();
                                                string filePath = dvRequest[iRequest]["CodeNumPath"].ToString();
                                                string sign = string.Empty;
                                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                                {
                                                    using (StreamReader reader = new StreamReader(fs, Encoding.Default))
                                                    {
                                                        while ((sign = reader.ReadLine()) != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(sign))
                                                            {
                                                                piList.data.Add(sign);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            genCodeCount = piList.data.Count();//重新为生成码数量赋值
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                        #endregion
                                        //Log.WriteLog("-----------------GenPICode----------4-----------", "GenPICode");
                                        #region  2.查找路由
                                        DataSet ds = new DataSet();
                                        sql = @"select * from Route_DataBase where isuse=1 order by Route_DataBase_id desc";
                                        SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                                        command.SelectCommand.Transaction = trans;
                                        command.Fill(ds, "ds");
                                        DataView dvRounte = ds.Tables[0].DefaultView;
                                        if (dvRounte.Count == 0)
                                        {
                                            string sqltmp = @"update dbo.Route_DataBase set isuse=1 where Route_DataBase_id=1";
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
                                        //Log.WriteLog("-----------------GenPICode---------5------------", "GenPICode");
                                        #region 3.插入表数据
                                        string codeConstr = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
                                                dvRounte[0]["DataSource"].ToString(), dvRounte[0]["DataBaseName"].ToString(),
                                                dvRounte[0]["UID"].ToString(), dvRounte[0]["PWD"].ToString());
                                        using (SqlConnection connCode = new SqlConnection(codeConstr))
                                        {
                                            string tableName = dvRounte[0]["TableName"].ToString();
                                            DataSet dsCode = new DataSet();
                                            sql = string.Format(@"select ISNULL(max(Enterprise_FWCode_ID),0)  from  {0}", tableName);
                                            SqlDataAdapter commandCode = new SqlDataAdapter(sql, connCode);
                                            commandCode.Fill(dsCode, "dsCode");
                                            DataView dvCode = dsCode.Tables[0].DefaultView;
                                            ///当前码表的的最大ID
                                            long maxID = long.Parse(dvCode[0][0].ToString()) + 1;
                                            DataTable dtCode = CreateDataTable(maxID, dvRequest[iRequest]["RequestCode_ID"].ToString());
                                            try
                                            {
                                                Random rnNo = new Random();
                                                int status = 1040000008;
                                                foreach (string ewm in piList.data)
                                                {
                                                    int no = rnNo.Next(1, 999999);
                                                    DataRow dr;
                                                    dr = dtCode.NewRow();
                                                    dr["Enterprise_Info_ID"] = dvRequest[iRequest]["Enterprise_Info_ID"].ToString();
                                                    dr["Material_ID"] = dvRequest[iRequest]["Material_ID"].ToString();
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
                                                    dr["RequestCode_ID"] = dvRequest[iRequest]["RequestCode_ID"].ToString();
                                                    dr["SalesInformation_ID"] = DBNull.Value;
                                                    dr["latitude"] = DBNull.Value;
                                                    dr["longitude"] = DBNull.Value;
                                                    dr["Type"] = (int)Common.EnumFile.CodeType.single;
                                                    dr["codeXML"] = DBNull.Value;
                                                    dtCode.Rows.Add(dr);
                                                }
                                                SqlBulkCopy bulkCopy = new SqlBulkCopy(codeConstr);
                                                bulkCopy.DestinationTableName = tableName;
                                                bulkCopy.BatchSize = dtCode.Rows.Count;
                                                bulkCopy.BulkCopyTimeout = 600;
                                                if (dtCode != null && dtCode.Rows.Count != 0 )
                                                {
                                                    bulkCopy.WriteToServer(dtCode);
                                                    if (dvRequest[iRequest]["CodingClientType"].ToString() == "1")
                                                    {
                                                        #region 分割第一个二维码，更新requestcode表
                                                        string ewm = piList.data[0];
                                                        //生产日期M开头
                                                        string scriqi = string.Empty;
                                                        //有效期V开头
                                                        string youxiaoqi = string.Empty;
                                                        //失效日期E开头
                                                        string shixiaoqi = string.Empty;
                                                        //生产批号L开头
                                                        string scBatchNo = string.Empty;
                                                        //流水号S开头
                                                        string flowNo = string.Empty;
                                                        //灭菌批号D开头
                                                        string mjNo = string.Empty;
                                                        string[] arr = ewm.Split('.');
                                                        for (int i = 3; i < arr.Length; i++)
                                                        {
                                                            if (arr[i].Substring(0, 1) == "P")//生产日期P开头旧规则
                                                            {
                                                                scriqi = arr[i].Substring(1, arr[i].Length - 1);
                                                            }
                                                            if (arr[i].Substring(0, 1) == "M")//生产日期M开头新规则
                                                            {
                                                                scriqi = arr[i].Substring(1, arr[i].Length - 1);
                                                            }
                                                            if (arr[i].Substring(0, 1) == "V")//有效期V开头
                                                            {
                                                                youxiaoqi = arr[i].Substring(1, arr[i].Length - 1);
                                                            }
                                                            if (arr[i].Substring(0, 1) == "E")//失效日期E开头
                                                            {
                                                                shixiaoqi = arr[i].Substring(1, arr[i].Length - 1);
                                                            }
                                                            if (arr[i].Substring(0, 1) == "L")//生产批号L开头
                                                            {
                                                                scBatchNo = arr[i].Substring(1, arr[i].Length - 1);
                                                            }
                                                            if (arr[i].Substring(0, 1) == "D")//灭菌批号D开头
                                                            {
                                                                mjNo = arr[i].Substring(1, arr[i].Length - 1);
                                                            }
                                                            if (arr[i].Substring(0, 1) == "S")//流水号S开头
                                                            {
                                                                flowNo = arr[i].Substring(1, arr[i].Length - 1);
                                                            }
                                                        }
                                                        sql = string.Format(@"update dbo.RequestCode set serialnumber='" + flowNo + "'");
                                                        if (!string.IsNullOrEmpty(scriqi))
                                                        {
                                                            sql = sql + ", [startdate]='" + scriqi + "'";
                                                        }
                                                        if (!string.IsNullOrEmpty(youxiaoqi))
                                                        {
                                                            sql = sql + " ,YouXiaoDate='" + youxiaoqi + "'";
                                                        }
                                                        if (!string.IsNullOrEmpty(shixiaoqi))
                                                        {
                                                            sql = sql + ",ShiXiaoDate='" + shixiaoqi + "'";
                                                        }
                                                        if (!string.IsNullOrEmpty(scBatchNo))
                                                        {
                                                            sql = sql + ",ShengChanPH='" + scBatchNo + "'";
                                                        }
                                                        if (!string.IsNullOrEmpty(mjNo))
                                                        {
                                                            sql = sql + ",dbatchnumber='" + mjNo + "'";
                                                        }
                                                        sql = sql + " where RequestCode_ID=" + dvRequest[iRequest]["RequestCode_ID"].ToString();
                                                        SqlCommand cmdRequest = new SqlCommand(sql, conn);
                                                        cmdRequest.Transaction = trans;
                                                        cmdRequest.ExecuteNonQuery();
                                                        if (!string.IsNullOrEmpty(scBatchNo))
                                                        {
                                                            sql = string.Format(@"update dbo.RequestCodeSetting set ShengChanPH='" + scBatchNo + "'  where RequestID={0}", dvRequest[iRequest]["RequestCode_ID"].ToString());
                                                            SqlCommand cmdSet = new SqlCommand(sql, conn);
                                                            cmdSet.Transaction = trans;
                                                            cmdSet.ExecuteNonQuery();
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    { 
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.WriteLog(ex.Message, "GenPICode");
                                            }
                                        }
                                        #endregion
                                        //Log.WriteLog("-----------------GenPICode-----------6----------", "GenPICode");
                                        #region 4.写入数据库状态
                                        sql = string.Format(@"update dbo.RequestCode set [Status]=1040000005 where RequestCode_ID={0}", dvRequest[iRequest]["RequestCode_ID"].ToString());
                                        SqlCommand cmdTop = new SqlCommand(sql, conn);
                                        cmdTop.Transaction = trans;
                                        cmdTop.ExecuteNonQuery();
                                        sql = string.Format(@"update RequestCode set Route_DataBase_ID={0},saleCount=0 where RequestCode_ID={1}", dvRounte[0]["Route_DataBase_id"].ToString(), dvRequest[iRequest]["RequestCode_ID"].ToString());
                                        cmdTop = new SqlCommand(sql, conn);
                                        cmdTop.Transaction = trans;
                                        cmdTop.ExecuteNonQuery();
                                        sql = string.Format(@"update dbo.Route_DataBase set lastTableMaxCount=lastTableMaxCount+{0} where Route_DataBase_ID={1}", genCodeCount, dvRounte[0]["Route_DataBase_id"].ToString());
                                        cmdTop = new SqlCommand(sql, conn);
                                        cmdTop.Transaction = trans;
                                        cmdTop.ExecuteNonQuery();

                                        sql = @"insert into RequestCodeSettingAdd(ID) select ID from  RequestCodeSetting R where R.ID  not in (select ID from RequestCodeSettingAdd )";
                                        cmdTop = new SqlCommand(sql, conn);
                                        cmdTop.Transaction = trans;
                                        cmdTop.ExecuteNonQuery();
                                        #endregion
                                        //Log.WriteLog("-----------------GenPICode-------------7--------", "GenPICode");
                                        trans.Commit();
                                        //Log.WriteLog("-----------------GenPICode-------------8--------", "GenPICode");
                                    }
                                    catch (Exception ex)
                                    {
                                        trans.Rollback();
                                        Log.WriteLog(ex.Message, "GenPICode");
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(1000 * timeSpan * 60*5);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, "GenPICode");
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
    }
}
