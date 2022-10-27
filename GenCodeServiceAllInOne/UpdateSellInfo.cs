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
    public class UpdateSellInfo
    {
        public static void Start()
        {
            ThreadStart start = DOSell;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }

        public static void DOSell()
        {
            while (true)
            {
                Log.WriteLog("更新经销商信息服务启动", "DOSell");
                //生成码的配置信息表
                try
                {
                    //服务运行时间间隔
                    int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
                    //连接库
                    string conStr = System.Configuration.ConfigurationManager.AppSettings["Constr"];
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        Log.WriteLog("--------------DODOSell---------开始-------", "DOSell");
                        UpdateSellDAL dal = new UpdateSellDAL();
                        RetResult result = dal.OutStorage();
                        Log.WriteLog("确认出库操作结果：" + result.Msg, "DOSell");
                        result = dal.OutStorageBack();
                        Log.WriteLog("出库退返操作结果：" + result.Msg, "DOSell");
                        Log.WriteLog("--------------结束----------------", "DOSell");
                    }
                    Thread.Sleep(1000 * timeSpan * 60 * 5);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, "DOSell");
                }
            }
        }
    }
}
