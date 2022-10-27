/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-6-7
** 联系方式:15533621896
** 代码功能：xml帮助类
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Common.Tools
{
    /// <summary>
    /// xml帮助类
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            string result = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    XmlSerializer xml = new XmlSerializer(type);
                    xml.Serialize(sw, obj);
                    result = sb.ToString();
                }
            }
            catch (Exception)
            {
                
            }
            return result;
        }
    }
}
