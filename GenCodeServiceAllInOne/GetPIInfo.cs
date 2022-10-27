using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Argument;
using Dal;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace GenCodeServiceAllInOne
{
    /// <summary>
    /// 获取发码机构的PI信息
    /// </summary>
    public static class GetPIInfo
    {
        public static void Start()
        {
            ThreadStart start = DOGetPIInfo;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }
        public static void DOGetPIInfo()
        {
            Log.WriteLog("--------------DOGetPIInfo----------------", "GetPIInfo");
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
                    // Log.WriteLog("--------------DOGetPIInfo----------1------", "GetPIInfo");
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        RequestCodeMaDAL dal = new RequestCodeMaDAL();
                        string sql = @" select * from dbo.Enterprise_Info  ";
                        DataSet dtConn = new DataSet();
                        SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                        // Log.WriteLog("--------------DOGetPIInfo---------2-------", "GetPIInfo");
                        commandConn.Fill(dtConn);
                        DataView dvEnterpriseInfo = dtConn.Tables[0].DefaultView;
                        if (dvEnterpriseInfo.Count > 0)
                        {
                            for (int iEnterprise = 0; iEnterprise < dvEnterpriseInfo.Count; iEnterprise++)
                            {
                                RetResult result = dal.GetPIInfo(dvEnterpriseInfo[iEnterprise]["MainCode"].ToString(), "Constr");
                                Log.WriteLog(result.Msg, "GetPIInfo");
                            }
                        }
                        Log.WriteLog("--------------DOGetPIInfo------------3----", "GetPIInfo");
                    }
                    Thread.Sleep(1000 * timeSpan*60*5);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, "GetPIInfo");
                }
            }
        }
    }
}
