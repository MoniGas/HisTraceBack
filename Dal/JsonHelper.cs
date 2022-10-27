/********************************************************************************

** 作者： 赵慧敏

** 创始时间：2015-11-20

** 联系方式 :13313318725

** 描述：Json帮助类

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/

using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Dal
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public  class JsonHelper
    {
        #region DataContractJsonSerializer

        /// <summary>
        /// 对象转换成json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject">需要格式化的对象</param>
        /// <returns>Json字符串</returns>
        public static string DataContractJsonSerialize<T>(T jsonObject)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            string json = null;
            using (MemoryStream ms = new MemoryStream()) //定义一个stream用来存发序列化之后的内容
            {
                serializer.WriteObject(ms, jsonObject);
                json = Encoding.UTF8.GetString(ms.GetBuffer()); //将stream读取成一个字符串形式的数据，并且返回
                ms.Close();
            }
            return json;
        }

        /// <summary>
        /// json字符串转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">要转换成对象的json字符串</param>
        /// <returns></returns>
        public static T DataContractJsonDeserialize<T>(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T obj = default(T);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
            }
            return obj;
        }
        /// <summary>
        /// 将对象序列化为JSON串
        /// </summary>
        /// <typeparam name="T">对象的数据类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>JSON串</returns>
        public static string ObjectToJSON<T>(T obj)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                JsonSerializer ser = new JsonSerializer();
                jw.WriteStartObject();
                T o = Activator.CreateInstance<T>();
                PropertyInfo[] pi = o.GetType().GetProperties();

                foreach (PropertyInfo p in pi)
                {
                    jw.WritePropertyName(p.Name);

                    ser.Serialize(jw, p.GetValue(obj, null));
                }
                jw.WriteEndObject();

                sw.Close();
                jw.Close();
            }

            string a = sb.ToString();
            return sb.ToString();
        }
        #endregion
    }
}
