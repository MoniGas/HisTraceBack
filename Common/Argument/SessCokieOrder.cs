using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace Common.Argument
{
    public class SessCokieOrder
    {
        /// <summary>
        /// 设置SessionCokies值
        /// </summary>
        /// <param name="card">SessionCokies实体</param>
        /// <returns></returns>
        public static bool Set(LinqModel.View_Order_Consumers card)
        {
            return MemoryCardSave(card);
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public static void Logout()
        {
            //HttpContext.Current.Request.Cookies["LoginInfo"].Value = null;
            HttpContext.Current.Session["LoginInfo"] = null;
        }

        /// <summary>
        /// 获取Session和Cookies 内存卡
        /// </summary>
        public static LinqModel.View_Order_Consumers Get
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
        private static LinqModel.View_Order_Consumers MemoryCardGet()
        {
            try
            {
                //if (HttpContext.Current.Request.Cookies["LoginInfo"] == null || HttpContext.Current.Request.Cookies["LoginInfo"].Value == "")
                //{
                if (HttpContext.Current.Session["LoginInfo"] != null)
                {
                    return DeserializeObject(HttpContext.Current.Session["LoginInfo"].ToString());
                }
                else
                {
                    return null;
                }
                //}
                //else
                //{
                //    return DeserializeObject(HttpContext.Current.Request.Cookies["LoginInfo"].Value);
                //}
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
        private static bool MemoryCardSave(LinqModel.View_Order_Consumers obj)
        {
            try
            {
                string result = SerializeObject(obj);

                if (HttpContext.Current.Session["LoginInfo"] == null)
                {
                    HttpContext.Current.Session["LoginInfo"] = result;//保存到Session
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

            //try
            //{
            //    string result = SerializeObject(obj);
            //    HttpContext.Current.Response.Cookies["LoginInfo"].Value = "";// result;//保存到Cookies
            //    if (HttpContext.Current.Request.Cookies["LoginInfo"] == null || HttpContext.Current.Request.Cookies["LoginInfo"].Value == "")
            //    {
            //        HttpContext.Current.Session["LoginInfo"] = result;//保存到Session
            //    }
            //    string a = HttpContext.Current.Request.Cookies["LoginInfo"].Value;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            //return true;
        }

        /// <summary>
        /// 对象序列化字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string SerializeObject(LinqModel.View_Order_Consumers obj)
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
        private static LinqModel.View_Order_Consumers DeserializeObject(string str)
        {
            LinqModel.View_Order_Consumers obj;
            System.Runtime.Serialization.IFormatter bf = new BinaryFormatter();
            byte[] byt = Convert.FromBase64String(str);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byt, 0, byt.Length))
            {
                obj = (LinqModel.View_Order_Consumers)bf.Deserialize(ms);
            }
            return obj;
        }
    }
}
