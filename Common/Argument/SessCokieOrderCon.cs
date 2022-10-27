using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace Common.Argument
{
    public class SessCokieOrderCon
    {
        /// <summary>
        /// 设置SessionCokies值
        /// </summary>
        /// <param name="card">SessionCokies实体</param>
        /// <returns></returns>
        public static bool Set(Common.Argument.OrderLoginInfo card)
        {
            return MemoryCardSave(card);
        }

        /// <summary>
        /// 获取Session和Cookies 内存卡
        /// </summary>
        public static Common.Argument.OrderLoginInfo Get
        {
            get
            {
                return MemoryCardGet();
            }
        }

        /// <summary>
        /// 从Cookies或Session里获取会话GerMemoryCard对象
        /// </summary>
        /// <returns></returns>
        private static Common.Argument.OrderLoginInfo MemoryCardGet()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["LoginInfo"] == null || HttpContext.Current.Request.Cookies["LoginInfo"].Value == "")
                {
                    if (HttpContext.Current.Session["LoginInfo"] != null)
                    {
                        return DeserializeObject(HttpContext.Current.Session["LoginInfo"].ToString());
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return DeserializeObject(HttpContext.Current.Request.Cookies["LoginInfo"].Value);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 保存会话对象GerMemoryCard到Cookies和Session
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool MemoryCardSave(Common.Argument.OrderLoginInfo obj)
        {
            try
            {
                string result = SerializeObject(obj);
                HttpContext.Current.Response.Cookies["LoginInfo"].Value = result;//保存到Cookies
                if (HttpContext.Current.Request.Cookies["LoginInfo"] == null || HttpContext.Current.Request.Cookies["LoginInfo"].Value == "")
                {
                    HttpContext.Current.Session["LoginInfo"] = result;//保存到Session
                }
                string a = HttpContext.Current.Request.Cookies["LoginInfo"].Value;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 对象序列化字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string SerializeObject(Common.Argument.OrderLoginInfo obj)
        {
            System.Runtime.Serialization.IFormatter bf = new BinaryFormatter();
            string result = string.Empty;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bf.Serialize(ms, obj);
                byte[] byt = new byte[ms.Length];
                byt = ms.ToArray();
                result = System.Convert.ToBase64String(byt);
                ms.Flush();
            }
            return result;
        }

        /// <summary>
        /// 字符串反序列化成对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Common.Argument.OrderLoginInfo DeserializeObject(string str)
        {
            Common.Argument.OrderLoginInfo obj;
            System.Runtime.Serialization.IFormatter bf = new BinaryFormatter();
            byte[] byt = Convert.FromBase64String(str);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byt, 0, byt.Length))
            {
                obj = (Common.Argument.OrderLoginInfo)bf.Deserialize(ms);
            }
            return obj;
        }
    }
}
