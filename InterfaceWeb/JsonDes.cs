using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System;

namespace InterfaceWeb
{
    public class JsonDes
    {
        public static T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>

        /// JSON反序列化

        /// </summary>

        /// <param name="jsonString"></param>

        /// <returns></returns>

        public static T JsonDeserializewithDt<T>(string jsonString)
        {

            string pattern = @"(\d{4}-\d{2}-\d{2})|(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2})";

            //string pattern = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";

            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);

            Regex reg = new Regex(pattern);

            jsonString = reg.Replace(jsonString, matchEvaluator);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            T obj = (T)ser.ReadObject(ms);

            return obj;

        }





        /// <summary>

        /// 将时间字符串转为Json时间

        /// </summary>

        private static string ConvertDateStringToJsonDate(Match m)
        {

            string result = string.Empty;

            DateTime dt = DateTime.Parse(m.Groups[0].Value);

            dt = dt.ToUniversalTime();

            TimeSpan ts = dt - DateTime.Parse("1970-01-01");

            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);

            return result;

        }
    }
}
