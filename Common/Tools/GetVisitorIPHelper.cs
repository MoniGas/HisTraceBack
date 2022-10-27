/*****************************************************************
代码功能：获取访问者的IP地址的帮助类
开发日期：2016年08月02日
作    者：孟凡
联系方式：13581544769
版权所有：河北广联信息技术有限公司研发二部    
******************************************************************/
using System;
using System.Web;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

namespace Common.Tools
{
    /// <summary>
    /// 获取访问者的IP地址的帮助类
    /// </summary>
    public class GetVisitorIPHelper
    {
        /// <summary>
        /// 获取客户端的IP地址
        /// </summary>
        /// <returns>IP字符串</returns>
        public static string ClientIp()
        {
            string result = "";
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (result != null && result != String.Empty)
            {
                //可能有代理 
                if (result.IndexOf(".") == -1)
                    //没有"."肯定是非IPv4格式 
                    result = null;
                else
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有","，估计多个代理。取第一个不是内网的IP。 
                        result = result.Replace(" ", "").Replace("", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (IsIPAddress(temparyip[i]) && temparyip[i].Substring(0, 3) != "10." && temparyip[i].Substring(0, 7) != "192.168" && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                return temparyip[i];//找到不是内网的地址 
                            }
                        }
                    }
                    else if (IsIPAddress(result)) //代理即是IP格式 
                        return result;
                    else
                        result = null;//代理中的内容 非IP，取IP 
                }
            }
            string IpAddress =
            (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null &&
            HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? HttpContext.Current.Request.ServerVariables
            ["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (null == result || result == String.Empty)
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (result == null || result == String.Empty)
                result = HttpContext.Current.Request.UserHostAddress;
            return result;
        }

        /// <summary>
        /// 验证是否是IP地址
        /// </summary>
        /// <param name="ipStr">IP字符串</param>
        /// <returns></returns>
        private static bool IsIPAddress(string ipStr)
        {
            if (ipStr == null || ipStr == string.Empty || ipStr.Length < 7 || ipStr.Length > 15)
            {
                return false;
            }
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(ipStr);
        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="sDataIn">输入字符串</param>
        /// <returns>加密地址</returns>
        public static string GetMD5(string sDataIn)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }

        /// <summary>
        /// 网络接口对象
        /// </summary>
        /// <returns></returns>
        public static NetworkInterface[] NetCardInfo()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }

        /// <summary>
        /// mac字符串
        /// </summary>
        /// <returns></returns>
        public static string GetMacString()
        {
            string strMac = "";
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    strMac += ni.GetPhysicalAddress().ToString() + "|";
                }
            }
            return strMac;
        }
    }
}
