using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace InterfaceWeb
{
    public static class APIHelper
    {
        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <param name="anchor">描点</param>
        /// <returns>响应内容</returns>
        public static string SendRequest(string url, IDictionary<string, string> parameters, string method, string anchor = "", string type = "application/x-www-form-urlencoded")
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 15000;
                    req.ContentType = string.Format("{0};charset=utf-8", type);
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {
                try
                {
                    //创建请求
                    string newUrl = url + "?" + BuildQuery(parameters, "utf8") + anchor;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newUrl);

                    //GET请求
                    request.Method = "GET";
                    request.ReadWriteTimeout = 15000;
                    request.ContentType = "text/html;charset=UTF-8";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("UTF-8"));

                    //返回内容
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        public static string PostMoths(string url, string param)
        {
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// Http (GET/POST)带有证书的请求方式
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        public static string SendRequest(string url, string parameters, string certPath, string passWord)
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            System.IO.Stream reqStream = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.KeepAlive = false;
                req.ProtocolVersion = HttpVersion.Version10;
                req.Timeout = 15000;
                if (!string.IsNullOrEmpty(certPath))
                {
                    X509Certificate2 cer = new X509Certificate2(certPath, passWord, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                    req.ClientCertificates.Add(cer);
                }
                req.ContentType = string.Format("text/xml;charset=utf-8");
                byte[] postData = Encoding.UTF8.GetBytes(parameters);
                reqStream = req.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                rsp = (HttpWebResponse)req.GetResponse();
                Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                return GetResponseAsString(rsp, encoding);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (reqStream != null) reqStream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// 获取请求链接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetUrl(string url, Dictionary<string, string> parameters, string anchor)
        {
            try
            {
                //创建请求
                string newUrl = url + "?" + BuildQuery(parameters, "utf8") + anchor;
                return newUrl;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        public static string sendPost(string url, string parameters)
        {
            ServicePointManager.DefaultConnectionLimit = 200;
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            System.IO.Stream reqStream = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.KeepAlive = false;
                req.ProtocolVersion = HttpVersion.Version10;
                req.Timeout = 15000;
                req.ContentType = string.Format("text/xml");
                byte[] postData = Encoding.UTF8.GetBytes(parameters);
                reqStream = req.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                rsp = (HttpWebResponse)req.GetResponse();
                Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                return GetResponseAsString(rsp, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (reqStream != null) reqStream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        public static string sendPost(string url, IDictionary<string, string> parameters, string method)
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 15000;
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {
                //创建请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + BuildQuery(parameters, "utf8"));

                //GET请求
                request.Method = "GET";
                request.ReadWriteTimeout = 15000;
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

                //返回内容
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))//&& !string.IsNullOrEmpty(value)
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    if (encode == "gb2312")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                    }
                    else if (encode == "utf8")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    }
                    else
                    {
                        postData.Append(value);
                    }
                    hasParam = true;
                }
            }
            return postData.ToString();
        }
    }
}
