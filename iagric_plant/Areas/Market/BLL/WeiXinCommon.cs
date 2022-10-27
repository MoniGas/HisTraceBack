/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-6-8
** 联系方式:15533621896
** 代码功能：微信帮助类
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace MarketActive.BLL
{
    /// <summary>
    /// 微信帮助类
    /// </summary>
    public class WeiXinCommon
    {
        /// <summary>
        /// 公众号APPID
        /// </summary>
        public static readonly string _PayId = "wx28218a4967512622";
        /// <summary>
        /// 服务商户/连续支付30天后，可换成普通商户
        /// </summary>
        //public static readonly string _MerId = "1436104602";
        //普通商户
        public static readonly string _MerId = "1429809402";
        /// <summary>
        /// 开发者密码
        /// </summary>
        public static readonly string _AppSecret = "b26945ba14576c4dad0c2bf672b9f83c";
        /// <summary>
        /// 签名API密钥
        /// </summary>
        public static readonly string _Key = "Hbgl123456Hbgl123456Hbgl12345678";
        /// <summary>
        /// 普通商户
        /// </summary>
        public static readonly string _MerIdPay = "1429809402";
        //public static readonly string _MerIdPay = "1436104602";

        /// <summary>
        /// 请求参数转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DictionaryToXml(SortedList<string, string> data)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<xml>");
            foreach (var item in data.Keys)
            {
                if (!string.IsNullOrWhiteSpace(item) && !string.IsNullOrWhiteSpace(data[item]))
                {
                    result.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", item, data[item]);
                }
            }
            result.Append("</xml>");
            return result.ToString();
        }

        /// <summary>
        /// 获取商户id
        /// </summary>
        /// <returns></returns>
        public static string GetShopBill(string merId)
        {
            string result = string.Empty;
            Random rdo = new Random();
            int i = rdo.Next(1, 1000000000);
            result = merId + DateTime.Now.ToString("yyyyMMdd") + i.ToString().PadLeft(10, '0');
            return result;
        }

        /// <summary>
        /// 获取sign
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string Sign(IDictionary<string, string> dict, string key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                    continue;
                sb.AppendFormat("{0}={1}&", item.Key, item.Value);
            }
            sb.Append("key=" + key);
            var bytesToHash = Encoding.UTF8.GetBytes(sb.ToString()); //注意，必须是UTF-8。
            var hashResult = ComputeMD5Hash(bytesToHash);
            var hash = BytesToString(hashResult, true);
            return hash;
        }

        /// <summary>
        /// 字节转换为md5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static byte[] ComputeMD5Hash(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var md5 = new MD5CryptoServiceProvider())
            {
                var result = md5.ComputeHash(input);
                return result;
            }
        }

        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <returns>返回40位UTF8 大写</returns>  
        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }

        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <param name="encode">指定加密编码</param>  
        /// <returns>返回40位大写字符串</returns>  
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 字节转换成字符串类型
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] input, bool lowercase = true)
        {
            if (input == null || input.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(input.Length * 2);
            for (var i = 0; i < input.Length; i++)
            {
                sb.AppendFormat(lowercase ? "{0:x2}" : "{0:X2}", input[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 日志书写
        /// </summary>
        public static void WriteLog(string data, string method, string exMessage)
        {
            try
            {
                string path = HttpRuntime.AppDomainAppPath.ToString() + "Log/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (FileStream fs = new FileStream(path + DateTime.Now.Date.ToString("yyyyMMdd") + "pay.txt", FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(string.IsNullOrEmpty(data) ? method + "没有读到数据" : string.Format("{2}：{0}异常：{1}", data, exMessage, method));
                        sw.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 获取订单号
        /// </summary>
        /// <returns></returns>
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", _MerIdPay, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }
    }
}