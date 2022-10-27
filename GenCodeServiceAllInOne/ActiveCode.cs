/*
  二维码激活接口
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;

namespace GenCodeServiceAllInOne
{
    public class ActiveCode
    {
        /// <summary>
        /// 激活码服务
        /// </summary>
        public static void start()
        {
			ThreadStart start = doActiveCodeContain;
            Thread th = new Thread(start);
            th.IsBackground = true;
            th.Start();
        }

		/// <summary>
		/// 2021-08-10 温森 修改原因：doActiveCodeNew依据‘-’进行分割判断激活码，UDI码中批号或灭菌批号或品类编码可能包含‘-’，原方法先分割再判断有问题，
		/// 现改为用Contains先判断字符中是否包含‘-产品’、‘-箱’、‘-瓶’、‘-垛’，再进行替换字符置空插入数据库；如果异常或部分功能障碍，
		/// 请修改此方法或采取原方法doActiveCodeNew
		/// </summary>
		public static void doActiveCodeContain()
		{
			string sqlRoute = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);
			///服务运行时间间隔
			int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
			int Hour = 0;
			int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Hour"], out Hour);
			if (Hour == 0)
			{
				Hour = 1;
			}
			//Dictionary<string, List<string>> errorDic = new Dictionary<string, List<string>>();
			WriteLogTest("开始运行", "ActiveCode");
			while (true)
			{
				int currHour = 1;//DateTime.Now.Hour;
				if (currHour <= Hour)
				{
					string sql = string.Empty;
					DataView dvGenCodeInfo = Dal.DbHelperSQL.Query(sqlRoute).Tables[0].DefaultView;
					for (int iGenCodeInfo = 0; iGenCodeInfo < dvGenCodeInfo.Count; iGenCodeInfo++)
					{
						//List<string> errorIDlst = new List<string>();
						//errorIDlst = errorDic[dvGenCodeInfo[iGenCodeInfo]["ID"].ToString()];
						#region 循环处理每一个数据库的业务
						string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
						   dvGenCodeInfo[iGenCodeInfo]["DataBaseIP"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DataBaseName"].ToString(),
						   dvGenCodeInfo[iGenCodeInfo]["DatabaseUserID"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DatabasePWD"].ToString());
						WriteLogTest(conStr, "ActiveCode");
						char[] splitChar = new char[] { '=' };
						char[] splitCharTwo = new char[] { '-' };
						using (SqlConnection conn = new SqlConnection(conStr))
						{
							conn.Open();
							try
							{
								sql = String.Format("select  * from  ActiveEwmRecord where status={0}  and  (OperationType is null or  OperationType=2 )",
									(int)Common.EnumFile.RecEwm.已接收);
								DataSet ds = new DataSet();
								SqlDataAdapter command = new SqlDataAdapter(sql, conn);
								WriteLogTest(sql, "ActiveCode");
								command.Fill(ds, "ActiveCode");
								DataView dvActive = ds.Tables[0].DefaultView;
								if (dvActive.Count > 0)
								{
									string filePath = "";
									for (int iActive = 0; iActive < dvActive.Count; iActive++)
									{
										using (SqlTransaction trans = conn.BeginTransaction())
										{
											try
											{
												filePath = dvActive[iActive]["RecPath"].ToString();
												string codeCon = string.Empty;
												string tableName = string.Empty;
												List<string> ewmlst = new List<string>();
												List<string> boxEwmlst = new List<string>();
												int codeCount = 0, boxCount = 0, bigBoxCount = 0;
												WriteLogTest("开始循环读取二维码", "ActiveCode");
												#region 读取二维码信息
												using (FileStream fs = new FileStream(filePath, FileMode.Open))
												{
													using (StreamReader reader = new StreamReader(fs, Encoding.Default))
													{
														string bindSql = string.Empty;
														string sLineInfo = string.Empty;
														///箱码列表 装垛后清空
														List<string> boxLst = new List<string>();
														//临时的产品码 装箱后清空
														List<string> codeLst = new List<string>();
														// WriteLog("开始读取数据");

														DataSet CheckDs = new DataSet();
														SqlDataAdapter CheckCommand = new SqlDataAdapter();
														DataView CheckDv = new DataView();
														while ((sLineInfo = reader.ReadLine()) != null)
														{
															if (!string.IsNullOrEmpty(sLineInfo))
															{
																SqlCommand cmdBind = new SqlCommand();
																string codeInfo = null;
																if (sLineInfo.Contains("="))
																{
																	codeInfo = sLineInfo.Split(splitChar)[1];
																}
																else
																{
																	codeInfo = sLineInfo;
																}
																if (codeInfo.Contains("-产品") || codeInfo.Contains("-瓶"))
																{
																	codeInfo = codeInfo.Replace("-产品", "").Replace("-瓶", "");
																	ewmlst.Add(codeInfo);
																	codeCount++;
																	codeLst.Add(codeInfo);
																}
																else if (codeInfo.Contains("-箱"))
																{
																	codeInfo = codeInfo.Replace("-箱", "");
																	boxLst.Add(codeInfo);
																	boxEwmlst.Add(codeInfo);
																	boxCount++;
																	if (codeLst.Count() > 0)
																	{
																		foreach (string s in codeLst)
																		{
																			string checkSql = string.Format(@"select isnull(count(*),0) as count from  BindCodeRecords where BoxCode='{0}' and 
                                                                              SingleCode='{1}'  and  Status!=-1 ", codeInfo, s);
																			//  WriteLogTest("checkSql:" + checkSql, "ActiveCode");
																			CheckDs = new DataSet();
																			CheckCommand = new SqlDataAdapter(checkSql, conn);
																			CheckCommand.SelectCommand.Transaction = trans;
																			CheckCommand.Fill(CheckDs, "tmp");
																			CheckDv = CheckDs.Tables[0].DefaultView;
																			if (CheckDv[0][0].ToString() == "0")
																			{
																				bindSql = string.Format(@"INSERT INTO BindCodeRecords  ([EnterpriseID] ,[BoxCode] ,[SingleCode]
                                                                           ,[BindDate] ,[UserID],[EquipType] ,[Status] ,[BindType]) values({0},'{1}','{2}','{3}',{4},{5},{6},{7})"
																				  , dvActive[iActive]["EnterpriseId"].ToString(), codeInfo, s, dvActive[iActive]["UploadDate"].ToString(),
																				  dvActive[iActive]["UpUserID"].ToString(), (int)Common.EnumFile.BindCodeType.server, 0, (int)Common.EnumFile.BindCodeRecordsType.AddBox);
																				WriteLogTest(bindSql, "ActiveCode");
																				cmdBind = new SqlCommand(bindSql, conn);
																				cmdBind.Transaction = trans;
																				cmdBind.ExecuteNonQuery();
																			}
																			else
																			{
																				WriteLogTest(string.Format("数据已绑定,箱码：{0}，产品码：{1}", codeInfo, s), "ActiveCode");
																			}
																		}
																		codeLst = new List<string>();
																	}
																}
																else if (codeInfo.Contains("垛"))
																{
																	codeInfo = codeInfo.Replace("-垛", "");
																	boxEwmlst.Add(codeInfo);
																	bigBoxCount++;
																	if (boxLst.Count() > 0)
																	{
																		foreach (string s in boxLst)
																		{
																			string checkSql = string.Format(@"select isnull(count(*),0) as count from  BindCodeRecords where BoxCode='{0}' and 
                                                                  SingleCode='{1}'   and  Status!=-1 ", codeInfo, s);
																			CheckDs = new DataSet();
																			CheckCommand = new SqlDataAdapter(checkSql, conn);
																			CheckCommand.SelectCommand.Transaction = trans;
																			CheckCommand.Fill(CheckDs, "tmp");
																			CheckDv = CheckDs.Tables[0].DefaultView;
																			if (CheckDv[0][0].ToString() == "0")
																			{
																				bindSql = string.Format(@"INSERT INTO BindCodeRecords  ([EnterpriseID] ,[BoxCode] ,[SingleCode]
                                                              ,[BindDate] ,[UserID],[EquipType] ,[Status] ,[BindType]) values({0},'{1}','{2}','{3}',{4},{5},{6},{7})"
																			  , dvActive[iActive]["EnterpriseId"].ToString(), codeInfo, s, dvActive[iActive]["UploadDate"].ToString(),
																			  dvActive[iActive]["UpUserID"].ToString(), (int)Common.EnumFile.BindCodeType.server, 0,
																			  (int)Common.EnumFile.BindCodeRecordsType.AddCrib);
																				WriteLogTest(bindSql, "ActiveCode");
																				cmdBind = new SqlCommand(bindSql, conn);
																				cmdBind.Transaction = trans;
																				cmdBind.ExecuteNonQuery();
																			}
																			else
																			{
																				WriteLogTest(string.Format("数据已绑定,垛码：{0}，箱码：{1}", codeInfo, s), "ActiveCode");
																			}
																		}
																		boxLst = new List<string>();
																	}
																}
															}
														}
													}
												}
												#endregion
												WriteLogTest(ewmlst.Count().ToString(), "ActiveCode");
												WriteLogTest(ewmlst[0], "ActiveCode");
												#region //考虑到UDI的DI和PI上传同步后才存在咱们数据库，这块先不做校验
												//#region 读取路由
												//if (ewmlst[0].Contains("."))
												//{
												//    string[] info = ewmlst[0].Split('.');
												//    if (info.Length > 4)
												//    {
												//        WriteLogTest(info[0], "ActiveCode");
												//        WriteLogTest(info[1], "ActiveCode");
												//        WriteLogTest(info[2], "ActiveCode");
												//        WriteLogTest(info[0] + "." + info[1] + "." + info[2] + "." + info[3] + "." + info[4], "ActiveCode");

												//        sql = String.Format("select  RequestCode_ID,Route_DataBase_ID from  RequestCode where FixedCode='{0}'",
												//             info[0] + "." + info[1] + "." + info[2] + "." + info[3] + "." + info[4]);
												//    }
												//}
												//ds = new DataSet();
												//command = new SqlDataAdapter(sql, conn);
												//WriteLogTest(sql, "ActiveCode");
												//command.SelectCommand.Transaction = trans;
												//command.Fill(ds, "ActiveCode");
												//DataView dvRequest = ds.Tables[0].DefaultView;
												//if (dvRequest.Count > 0)
												//{
												//    sql = string.Format(@"select  * from  Route_DataBase where Route_DataBase_ID= {0}",
												//   dvRequest[0]["Route_DataBase_ID"].ToString());
												//    ds = new DataSet();
												//    WriteLogTest(sql, "ActiveCode");
												//    command = new SqlDataAdapter(sql, conn);
												//    command.SelectCommand.Transaction = trans;
												//    command.Fill(ds, "ds" + dvRequest[0]["Route_DataBase_ID"].ToString());
												//    ///路由信息
												//    DataView dvRounte = ds.Tables[0].DefaultView;
												//    codeCon = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
												//              dvRounte[0]["DataSource"].ToString(), dvRounte[0]["DataBaseName"].ToString(),
												//              dvRounte[0]["UID"].ToString(), dvRounte[0]["PWD"].ToString());
												//    tableName = dvRounte[0]["TableName"].ToString();
												//#endregion
												//    WriteLogTest(codeCon, "ActiveCode");
												//    #region 循环激活二维码
												//    //using (SqlConnection connCode = new SqlConnection(codeCon))
												//    //{
												//    //    connCode.Open();
												//    //    List<string> _lst = new List<string>();
												//    //    int i = 0;
												//    //    _lst = ewmlst.Skip(50 * i).Take(50).ToList();
												//    //    while (_lst.Count() > 0)
												//    //    {
												//    //        sql = string.Format("update {0} set Status={1},ScanCount=0,FWCount=0,ValidateTime=null where EWM in ({2})",
												//    //            tableName, (int)Common.EnumFile.UsingStateCode.Activated,
												//    //            "'" + string.Join("','", _lst.ToArray()) + "'");
												//    //        WriteLogTest(sql, "ActiveCode");
												//    //        SqlCommand cmd = new SqlCommand(sql, connCode);
												//    //        cmd.ExecuteNonQuery();
												//    //        i++;
												//    //        _lst = ewmlst.Skip(50 * i).Take(50).ToList();
												//    //    }
												//    //}
												//    #endregion

												//#region 激活箱码和垛码 箱码和垛码可能不在一个数据库表
												//#region 目前只针对IDcode/简码格式的箱码
												//if (boxEwmlst.Count > 0)
												//{
												//    #region 1.找出所有箱码的路由信息
												//    string FixCode = "";
												//    if (boxEwmlst[0].Contains("."))
												//    {
												//        string[] info = boxEwmlst[0].Split('.');
												//        FixCode = info[0] + "." + info[1] + "." + info[2] + "." + info[3] + "." + info[4];
												//    }
												//    sql = String.Format("select  RequestCode_ID,Route_DataBase_ID from  RequestCode where FixedCode='{0}'",
												//            FixCode);
												//    ds = new DataSet();
												//    command = new SqlDataAdapter(sql, conn);
												//    WriteLogTest(sql, "ActiveCode");
												//    command.SelectCommand.Transaction = trans;
												//    command.Fill(ds, "ActiveCode");
												//    DataView dvBoxRequest = ds.Tables[0].DefaultView;
												//    sql = string.Format(@"select  * from  Route_DataBase where Route_DataBase_ID= {0}",
												//        dvBoxRequest[0]["Route_DataBase_ID"].ToString());
												//    ds = new DataSet();
												//    WriteLogTest(sql, "ActiveCode");
												//    command = new SqlDataAdapter(sql, conn);
												//    command.SelectCommand.Transaction = trans;
												//    command.Fill(ds, "ds" + dvRequest[0]["Route_DataBase_ID"].ToString());
												//    ///路由信息
												//    DataView dvBoxRounte = ds.Tables[0].DefaultView;
												//    codeCon = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
												//                dvBoxRounte[0]["DataSource"].ToString(), dvBoxRounte[0]["DataBaseName"].ToString(),
												//                dvBoxRounte[0]["UID"].ToString(), dvBoxRounte[0]["PWD"].ToString());
												//    tableName = dvBoxRounte[0]["TableName"].ToString();
												//    List<string> activeBox = new List<string>();
												//    Dictionary<string, List<string>> activeLst = new Dictionary<string, List<string>>();
												//    foreach (string s in boxEwmlst)
												//    {
												//        string[] _info = null;
												//        string _FixCode = "";
												//        if (!s.Contains("."))
												//        {
												//            if (s.Length == 16)//简码20180813
												//            {
												//                _FixCode = s.Substring(0, 16 - 5);
												//            }
												//            else
												//            {
												//                _FixCode = s.Substring(0, 32 - 9);
												//            }
												//        }
												//        else
												//        {
												//            _info = s.Split('.');
												//            _FixCode = _info[0] + "." + _info[1] + "." + _info[2] + "." + _info[3] + "." + _info[4];
												//        }

												//        if (FixCode == _FixCode)
												//        {
												//            activeBox.Add(s + "," + tableName);
												//        }
												//        else
												//        {
												//            activeLst.Add(codeCon + "", activeBox);
												//            sql = String.Format("select  RequestCode_ID,Route_DataBase_ID from  RequestCode where FixedCode='{0}'",
												//            _FixCode);
												//            ds = new DataSet();
												//            command = new SqlDataAdapter(sql, conn);
												//            WriteLogTest(sql, "ActiveCode");
												//            command.SelectCommand.Transaction = trans;
												//            command.Fill(ds, "ActiveCode");
												//            dvBoxRequest = ds.Tables[0].DefaultView;
												//            sql = string.Format(@"select  * from  Route_DataBase where Route_DataBase_ID= {0}",
												//                dvBoxRequest[0]["Route_DataBase_ID"].ToString());
												//            ds = new DataSet();
												//            WriteLogTest(sql, "ActiveCode");
												//            command = new SqlDataAdapter(sql, conn);
												//            command.SelectCommand.Transaction = trans;
												//            command.Fill(ds, "ds" + dvRequest[0]["Route_DataBase_ID"].ToString());
												//            ///路由信息
												//            dvBoxRounte = ds.Tables[0].DefaultView;
												//            codeCon = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
												//                        dvBoxRounte[0]["DataSource"].ToString(), dvBoxRounte[0]["DataBaseName"].ToString(),
												//                        dvBoxRounte[0]["UID"].ToString(), dvBoxRounte[0]["PWD"].ToString());
												//            tableName = dvBoxRounte[0]["TableName"].ToString();
												//            activeBox = new List<string>();
												//            activeBox.Add(s + "," + tableName);
												//        }

												//    }
												//    activeLst.Add(codeCon + "", activeBox);
												//    #endregion

												//    #region 2. 循环激活箱码
												//    char[] boxSplitChar = new char[] { ',' };
												//    foreach (string boxCon in activeLst.Keys)
												//    {
												//        List<string> lstOrign = activeLst[boxCon];
												//        string boxTableName = lstOrign[0].Split(boxSplitChar)[1];
												//        List<string> lst = (from c in lstOrign select c.Split(boxSplitChar)[0]).ToList();
												//        using (SqlConnection connBoxCode = new SqlConnection(boxCon))
												//        {
												//            connBoxCode.Open();
												//            List<string> _lst = new List<string>();
												//            int i = 0;
												//            _lst = lst.Skip(20 * i).Take(20).ToList();
												//            while (_lst.Count() > 0)
												//            {

												//                sql = string.Format("update {0} set Status={1},ScanCount=0,FWCount=0,ValidateTime=null where EWM in ({2})",
												//                    boxTableName, (int)Common.EnumFile.UsingStateCode.Activated,
												//                    "'" + string.Join("','", _lst.ToArray()) + "'");
												//                WriteLogTest(sql, "ActiveCode");
												//                SqlCommand cmd = new SqlCommand(sql, connBoxCode);
												//                cmd.ExecuteNonQuery();
												//                i++;
												//                _lst = lst.Skip(20 * i).Take(20).ToList();
												//            }
												//        }
												//    }
												//    #endregion
												//}

												//#endregion
												//#endregion
												string desc = string.Format("共激活产品码{0}个，包装箱码{1}个，垛码{2}个。", codeCount, boxCount, bigBoxCount);
												sql = string.Format("update ActiveEwmRecord set Status={0},MateialCount={2},Detail='{3}' where RecID={1}",
													(int)Common.EnumFile.RecEwm.已激活, dvActive[iActive]["RecID"].ToString(), codeCount, desc);
												WriteLogTest(sql, "ActiveCode");
												SqlCommand cmdConn = new SqlCommand(sql, conn);
												cmdConn.Transaction = trans;
												cmdConn.ExecuteNonQuery();
												WriteLogTest("Next", "ActiveCode");
												#endregion
												trans.Commit();
												Thread.Sleep(1000 * timeSpan * 10);
												//}
											}
											catch (Exception ex)
											{
												trans.Rollback();
												WriteLog(dvActive[iActive]["RecID"].ToString() + ":" + ex.Message, "ActiveCode");
											}
										}
									}
								}
							}
							catch (Exception ex)
							{

								WriteLog(ex.Message, "ActiveCode");
							}

						}
						#endregion
					}
				}
				Thread.Sleep(1000 * timeSpan * 30);
			}
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

        public static void doActiveCodeNew()
        {
            string sqlRoute = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);
            ///服务运行时间间隔
            int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
            int Hour = 0;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Hour"], out Hour);
            if (Hour == 0)
            {
                Hour = 1;
            }
            //Dictionary<string, List<string>> errorDic = new Dictionary<string, List<string>>();
            WriteLogTest("开始运行", "ActiveCode");
            while (true)
            {
                int currHour = 1;//DateTime.Now.Hour;
                if (currHour <= Hour)
                {
                    string sql = string.Empty;
                    DataView dvGenCodeInfo = Dal.DbHelperSQL.Query(sqlRoute).Tables[0].DefaultView;
                    for (int iGenCodeInfo = 0; iGenCodeInfo < dvGenCodeInfo.Count; iGenCodeInfo++)
                    {
                        //List<string> errorIDlst = new List<string>();
                        //errorIDlst = errorDic[dvGenCodeInfo[iGenCodeInfo]["ID"].ToString()];
                        #region 循环处理每一个数据库的业务
                        string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                           dvGenCodeInfo[iGenCodeInfo]["DataBaseIP"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DataBaseName"].ToString(),
                           dvGenCodeInfo[iGenCodeInfo]["DatabaseUserID"].ToString(), dvGenCodeInfo[iGenCodeInfo]["DatabasePWD"].ToString());
                        WriteLogTest(conStr, "ActiveCode");
                        char[] splitChar = new char[] { '=' };
                        char[] splitCharTwo = new char[] { '-' };
                        using (SqlConnection conn = new SqlConnection(conStr))
                        {
                            conn.Open();
                            try
                            {
                                sql = String.Format("select  * from  ActiveEwmRecord where status={0}  and  (OperationType is null or  OperationType=2 )",
                                    (int)Common.EnumFile.RecEwm.已接收);
                                DataSet ds = new DataSet();
                                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                                WriteLogTest(sql, "ActiveCode");
                                command.Fill(ds, "ActiveCode");
                                DataView dvActive = ds.Tables[0].DefaultView;
                                if (dvActive.Count > 0)
                                {
                                    string filePath = "";
                                    for (int iActive = 0; iActive < dvActive.Count; iActive++)
                                    {
                                        using (SqlTransaction trans = conn.BeginTransaction())
                                        {
                                            try
                                            {
                                                filePath = dvActive[iActive]["RecPath"].ToString();
                                                string codeCon = string.Empty;
                                                string tableName = string.Empty;
                                                List<string> ewmlst = new List<string>();
                                                List<string> boxEwmlst = new List<string>();
                                                int codeCount = 0, boxCount = 0, bigBoxCount = 0;
                                                WriteLogTest("开始循环读取二维码", "ActiveCode");
                                                #region 读取二维码信息
                                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                                {
                                                    using (StreamReader reader = new StreamReader(fs, Encoding.Default))
                                                    {
                                                        string bindSql = string.Empty;
                                                        string sLineInfo = string.Empty;
                                                        ///箱码列表 装垛后清空
                                                        List<string> boxLst = new List<string>();
                                                        //临时的产品码 装箱后清空
                                                        List<string> codeLst = new List<string>();
                                                        // WriteLog("开始读取数据");

                                                        DataSet CheckDs = new DataSet();
                                                        SqlDataAdapter CheckCommand = new SqlDataAdapter();
                                                        DataView CheckDv = new DataView();
                                                        while ((sLineInfo = reader.ReadLine()) != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(sLineInfo))
                                                            {
                                                                SqlCommand cmdBind = new SqlCommand();
                                                                string[] codeInfo = null;
                                                                if (sLineInfo.Contains("="))
                                                                {
                                                                    codeInfo = sLineInfo.Split(splitChar)[1].Split(splitCharTwo);
                                                                }
                                                                else
                                                                {
                                                                    codeInfo = sLineInfo.Split(splitCharTwo);
                                                                }
                                                                if (codeInfo[1].Contains("产品") || codeInfo[1].Contains("瓶"))
                                                                {
                                                                    ewmlst.Add(codeInfo[0]);
                                                                    codeCount++;
                                                                    codeLst.Add(codeInfo[0]);
                                                                }
                                                                else if (codeInfo[1].Contains("箱"))
                                                                {
                                                                    boxLst.Add(codeInfo[0]);
                                                                    boxEwmlst.Add(codeInfo[0]);
                                                                    boxCount++;
                                                                    if (codeLst.Count() > 0)
                                                                    {
                                                                        foreach (string s in codeLst)
                                                                        {
                                                                            string checkSql = string.Format(@"select isnull(count(*),0) as count from  BindCodeRecords where BoxCode='{0}' and 
                                                                              SingleCode='{1}' ", codeInfo[0], s);
                                                                            //  WriteLogTest("checkSql:" + checkSql, "ActiveCode");
                                                                            CheckDs = new DataSet();
                                                                            CheckCommand = new SqlDataAdapter(checkSql, conn);
                                                                            CheckCommand.SelectCommand.Transaction = trans;
                                                                            CheckCommand.Fill(CheckDs, "tmp");
                                                                            CheckDv = CheckDs.Tables[0].DefaultView;
                                                                            if (CheckDv[0][0].ToString() == "0")
                                                                            {
                                                                                bindSql = string.Format(@"INSERT INTO BindCodeRecords  ([EnterpriseID] ,[BoxCode] ,[SingleCode]
                                                                           ,[BindDate] ,[UserID],[EquipType] ,[Status] ,[BindType]) values({0},'{1}','{2}','{3}',{4},{5},{6},{7})"
                                                                                  , dvActive[iActive]["EnterpriseId"].ToString(), codeInfo[0], s, dvActive[iActive]["UploadDate"].ToString(),
                                                                                  dvActive[iActive]["UpUserID"].ToString(), (int)Common.EnumFile.BindCodeType.server, 0, (int)Common.EnumFile.BindCodeRecordsType.AddBox);
                                                                                WriteLogTest(bindSql, "ActiveCode");
                                                                                cmdBind = new SqlCommand(bindSql, conn);
                                                                                cmdBind.Transaction = trans;
                                                                                cmdBind.ExecuteNonQuery();
                                                                            }
                                                                            else
                                                                            {
                                                                                WriteLogTest(string.Format("数据已绑定,箱码：{0}，产品码：{1}", codeInfo[0], s), "ActiveCode");
                                                                            }
                                                                        }
                                                                        codeLst = new List<string>();
                                                                    }
                                                                }
                                                                else if (codeInfo[1].Contains("垛"))
                                                                {
                                                                    boxEwmlst.Add(codeInfo[0]);
                                                                    bigBoxCount++;
                                                                    if (boxLst.Count() > 0)
                                                                    {
                                                                        foreach (string s in boxLst)
                                                                        {
                                                                            string checkSql = string.Format(@"select isnull(count(*),0) as count from  BindCodeRecords where BoxCode='{0}' and 
                                                                  SingleCode='{1}' ", codeInfo[0], s);
                                                                            CheckDs = new DataSet();
                                                                            CheckCommand = new SqlDataAdapter(checkSql, conn);
                                                                            CheckCommand.SelectCommand.Transaction = trans;
                                                                            CheckCommand.Fill(CheckDs, "tmp");
                                                                            CheckDv = CheckDs.Tables[0].DefaultView;
                                                                            if (CheckDv[0][0].ToString() == "0")
                                                                            {
                                                                                bindSql = string.Format(@"INSERT INTO BindCodeRecords  ([EnterpriseID] ,[BoxCode] ,[SingleCode]
                                                              ,[BindDate] ,[UserID],[EquipType] ,[Status] ,[BindType]) values({0},'{1}','{2}','{3}',{4},{5},{6},{7})"
                                                                              , dvActive[iActive]["EnterpriseId"].ToString(), codeInfo[0], s, dvActive[iActive]["UploadDate"].ToString(),
                                                                              dvActive[iActive]["UpUserID"].ToString(), (int)Common.EnumFile.BindCodeType.server, 0,
                                                                              (int)Common.EnumFile.BindCodeRecordsType.AddCrib);
                                                                                WriteLogTest(bindSql, "ActiveCode");
                                                                                cmdBind = new SqlCommand(bindSql, conn);
                                                                                cmdBind.Transaction = trans;
                                                                                cmdBind.ExecuteNonQuery();
                                                                            }
                                                                            else
                                                                            {
                                                                                WriteLogTest(string.Format("数据已绑定,垛码：{0}，箱码：{1}", codeInfo[0], s), "ActiveCode");
                                                                            }
                                                                        }
                                                                        boxLst = new List<string>();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion
                                                WriteLogTest(ewmlst.Count().ToString(), "ActiveCode");
                                                WriteLogTest(ewmlst[0], "ActiveCode");
                                                #region //考虑到UDI的DI和PI上传同步后才存在咱们数据库，这块先不做校验
                                                string desc = string.Format("共激活产品码{0}个，包装箱码{1}个，垛码{2}个。", codeCount, boxCount, bigBoxCount);
                                                sql = string.Format("update ActiveEwmRecord set Status={0},MateialCount={2},Detail='{3}' where RecID={1}",
                                                    (int)Common.EnumFile.RecEwm.已激活, dvActive[iActive]["RecID"].ToString(), codeCount, desc);
                                                WriteLogTest(sql, "ActiveCode");
                                                SqlCommand cmdConn = new SqlCommand(sql, conn);
                                                cmdConn.Transaction = trans;
                                                cmdConn.ExecuteNonQuery();
                                                WriteLogTest("Next", "ActiveCode");
                                                #endregion
                                                trans.Commit();
                                                Thread.Sleep(1000 * timeSpan * 10);
                                                //}
                                            }
                                            catch (Exception ex)
                                            {
                                                trans.Rollback();
                                                WriteLog(dvActive[iActive]["RecID"].ToString() + ":" + ex.Message, "ActiveCode");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                WriteLog(ex.Message, "ActiveCode");
                            }

                        }
                        #endregion
                    }
                }
                Thread.Sleep(1000 * timeSpan * 30);
            }
        }
    }
}
