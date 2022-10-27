using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using Common.Argument;
using System.Data;
using InterfaceWeb;
using LinqModel;
using Dal;

namespace GenCodeServiceAllInOne
{
    public class GetMaincode
    {
        public static void start()
        {
            ThreadStart start = DOGetMainCode;
            Thread th = new Thread(start);
            th.IsBackground = true;
            th.Start();
        }
        public static void DOGetMainCode()
        {
            Log.WriteLog("--------------DOMainCode----------------", "MainCode");
            while (true)
            {
                //生成码的配置信息表
                try
                {
                    //服务运行时间间隔
                    int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
                    //连接库
                    string conStr = System.Configuration.ConfigurationManager.AppSettings["Constr"];
                    // Log.WriteLog("--------------DOGetPIInfo----------1------", "GetPIInfo");
                    Enterprise_UserDAL dal = new Enterprise_UserDAL();

                    RetResult result = dal.GetCompanyMainCode("Constr");
                    if (!string.IsNullOrEmpty(result.Msg))
                    {
                        Log.WriteLog(result.Msg, "MainCode");
                    }
                    Thread.Sleep(1000 * timeSpan*60);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, "MainCode");
                }
            }
        }
    }
}
