using System;
using System.Text;
using System.IO;

namespace InterfaceWeb
{
    public class WriteLog1
    {
        /// <summary>
        /// 同步对象，保证对日志文件的同步性
        /// </summary>
        private static object _Syn = new object();
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <returns></returns>
        public static bool WriteErrorLog(string logStr)
        {
            lock (_Syn)
            {
                string logDir = System.AppDomain.CurrentDomain.BaseDirectory + "RunLog\\";
                string strFileName = logDir + DateTime.Now.Date.ToShortDateString().Replace("/", "_") + ".ini";
                try
                {
                    if (!System.IO.File.Exists(strFileName))
                    {
                        System.IO.File.Create(strFileName).Close();
                    }
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(strFileName, true, Encoding.Default);
                    sw.WriteLine(logStr);
                    sw.Flush();
                    sw.Close();
                    return true;
                }
                catch (System.Exception err)
                {
                    System.Diagnostics.Trace.WriteLine(err);
                }
                return false;
            }
        }

        /// <summary>
        /// 支付宝交易日志
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <returns></returns>
        public static bool WriteAlipayLog(string logStr)
        {
            lock (_Syn)
            {
                string logDir = System.AppDomain.CurrentDomain.BaseDirectory + "RunLog\\";
                string strFileName = logDir + DateTime.Now.Date.ToShortDateString().Replace("/", "_") + "_Alipay.ini";
                try
                {
                    if (!System.IO.File.Exists(strFileName))
                    {
                        System.IO.File.Create(strFileName).Close();
                    }
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(strFileName, true, Encoding.Default);
                    sw.WriteLine(logStr);
                    sw.Flush();
                    sw.Close();
                    return true;
                }
                catch (System.Exception err)
                {
                    System.Diagnostics.Trace.WriteLine(err);
                }
                return false;
            }
        }

        /// <summary>
        /// 支付宝交易日志
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <returns></returns>
        public static bool WriteRefundLog(string logStr)
        {
            lock (_Syn)
            {
                string logDir = System.AppDomain.CurrentDomain.BaseDirectory + "RunLog\\";
                string strFileName = logDir + DateTime.Now.Date.ToShortDateString().Replace("/", "_") + "_Refund.ini";
                try
                {
                    if (!System.IO.File.Exists(strFileName))
                    {
                        System.IO.File.Create(strFileName).Close();
                    }
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(strFileName, true, Encoding.Default);
                    sw.WriteLine(logStr);
                    sw.Flush();
                    sw.Close();
                    return true;
                }
                catch (System.Exception err)
                {
                    System.Diagnostics.Trace.WriteLine(err);
                }
                return false;
            }
        }

        /// <summary>
        /// 微信接口相关日志
        /// </summary>
        /// <param name="logStr">日志内容</param>
        /// <param name="logName"></param>
        /// <returns></returns>
        public static bool WriteWxLog(string logStr, string logName)
        {
            lock (_Syn)
            {
                string logDir = AppDomain.CurrentDomain.BaseDirectory + "RunLog\\";
                string strFileName = logDir + DateTime.Now.Date.ToShortDateString().Replace("/", "_") + "_" + logName + ".ini";
                try
                {
                    if (!File.Exists(strFileName))
                    {
                        File.Create(strFileName).Close();
                    }
                    StreamWriter sw = new StreamWriter(strFileName, true, Encoding.Default);
                    sw.WriteLine(logStr);
                    sw.Flush();
                    sw.Close();
                    return true;
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.WriteLine(err);
                }
                return false;
            }
        }
    }
}
