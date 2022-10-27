using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GenCodeService
{
    public class Log
    {
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
    }
}
