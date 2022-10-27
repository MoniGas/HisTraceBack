using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using Dal;
using System.Data;
using Common.Argument;

namespace GenCodeServiceAllInOne
{
    /// <summary>
    /// 服务定时上传DI信息到发码机构
    /// </summary>
    public class UpDIInfo 
    {
        public static void Start()
        {
            ThreadStart start = DOUpDIInfo;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }
        public static void DOUpDIInfo()
        {
            Log.WriteLog("--------------开始----------------", "GetDIInfo");
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
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        MaterialDIDAL dal = new MaterialDIDAL();
                        string sql = @" select * from dbo.Enterprise_Info  ";
                        DataSet dtConn = new DataSet();
                        SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                        commandConn.Fill(dtConn);
                        DataView dvEnterpriseInfo = dtConn.Tables[0].DefaultView;
                        if (dvEnterpriseInfo.Count > 0)
                        {
                            for (int iEnterprise = 0; iEnterprise < dvEnterpriseInfo.Count; iEnterprise++)
                            {
                                RetResult result = dal.UpUDIDI(dvEnterpriseInfo[iEnterprise]["MainCode"].ToString(), "Constr");
                                Log.WriteLog("结果："+result.Msg, "GetDIInfo");
                            }
                        }
                        Log.WriteLog("--------------结束------------", "GetDIInfo");
                    }
                    Thread.Sleep(1000 * timeSpan * 60 * 5);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, "GetDIInfo");
                }
            }
        }
    }
}
