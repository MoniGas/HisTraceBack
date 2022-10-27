using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Common.Argument;
using LinqModel;

namespace Dal
{
    public class DALBase
    {
        public DALBase()
        {
            Ret = new RetResult();
            Arg = new ResultArg();
            PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        }

        protected static RetResult Ret;
        protected static ResultArg Arg;
        protected static int PageSize;

        /// <summary>
        /// 链接宣传营销数据库
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public DataClassesDataContext GetDataContext(string para = "WebConnect")
        {
            string conString = ConfigurationManager.AppSettings[para];
            return new DataClassesDataContext(conString);
        }

        /// <summary>
        /// 链接二维码数据库
        /// </summary>
        /// <param name="server">服务器地址</param>
        /// <param name="dataBase">数据库名称</param>
        /// <param name="uName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public DataClassesDataContext GetDataContext(string server, string dataBase, string uName, string pwd)
        {
            string conString = ConfigurationManager.AppSettings["CodeConnect"];
            conString = conString.Replace("datasource", server);//替换服务器地址
            conString = conString.Replace("databasename", dataBase);//替换数据库名称
            conString = conString.Replace("username", uName);//替换数据库登录名
            conString = conString.Replace("password", pwd);//替换数据库密码
            return new DataClassesDataContext(conString);
        }
        public DataClassesDataContext GetContext(string ConString)
        {
            return new DataClassesDataContext(ConString);
        }
        #region 获取公共追溯平台库

        /// <summary>
        /// 链接公共追溯平台数据库
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public DataClassesDataContext GetPublicDataContext(string para = "PublicTraceConnect")
        {
            string ConString = ConfigurationManager.AppSettings[para];
            return new DataClassesDataContext(ConString);
        }
        #endregion

        #region 清除主外键字段
        public void ClearLinqModel<T>(T t)
        {
            PropertyInfo[] propertys = t.GetType().GetProperties();
            for (int i = 0; i < propertys.Length; i++)
            {
                try
                {
                    string type = propertys[i].PropertyType.Name;
                    string full = propertys[i].PropertyType.FullName;
                    if (type.IndexOf("EntitySet") >= 0)
                    {
                        propertys[i].SetValue(t, null, null);
                        continue;
                    }
                    if (type.IndexOf("EntityRef") >= 0)
                    {
                        propertys[i].SetValue(t, null, null);
                        continue;
                    }
                    if (full.IndexOf("LinqModel") >= 0)
                    {
                        propertys[i].SetValue(t, null, null);
                        continue;
                    }
                    if (full.IndexOf("XElement") >= 0)
                    {
                        if (propertys[i].GetValue(t, null) != null)
                        {
                            string value = propertys[i].GetValue(t, null).ToString();
                            t.GetType().GetProperty("Str" + propertys[i].Name).SetValue(t, value, null);
                        }
                        propertys[i].SetValue(t, null, null);
                        continue;
                    }
                    if (full.IndexOf("DateTime") >= 0)
                    {
                        if (propertys[i].GetValue(t, null) != null)
                        {
                            string value = Convert.ToDateTime(propertys[i].GetValue(t, null)).ToString("yyyy-MM-dd HH:mm:ss");
                            t.GetType().GetProperty("Str" + propertys[i].Name).SetValue(t, value, null);
                        }
                        propertys[i].SetValue(t, null, null);
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }

        }
        public void ClearLinqModel<T>(List<T> t)
        {
            List<T> newT = t;
            Type addType = typeof(T);
            if (t != null && t.Count > 0)
            {
                for (int i = 0; i < newT.Count; i++)
                {
                    try
                    {
                        PropertyInfo[] propertys = newT[i].GetType().GetProperties();
                        for (int j = 0; j < propertys.Length; j++)
                        {
                            try
                            {
                                string type = propertys[j].PropertyType.Name;
                                string full = propertys[j].PropertyType.FullName;
                                if (type.IndexOf("EntitySet") >= 0)
                                {
                                    propertys[j].SetValue(newT[i], null, null);
                                    continue;
                                }
                                if (type.IndexOf("EntityRef") >= 0)
                                {
                                    propertys[j].SetValue(newT[i], null, null);
                                    continue;
                                }
                                if (full.IndexOf("LinqModel") >= 0)
                                {
                                    propertys[j].SetValue(newT[i], null, null);
                                    continue;
                                }
                                if (full.IndexOf("XElement") >= 0)
                                {
                                    if (propertys[j].GetValue(newT[i], null) != null)
                                    {
                                        string value = propertys[j].GetValue(newT[i], null).ToString();
                                        newT[i].GetType().GetProperty("Str" + propertys[j].Name).SetValue(newT[i], value, null);
                                    }
                                    propertys[j].SetValue(newT[i], null, null);
                                    continue;
                                }
                                if (full.IndexOf("DateTime") >= 0)
                                {
                                    if (propertys[j].GetValue(newT[i], null) != null)
                                    {
                                        string value = Convert.ToDateTime(propertys[j].GetValue(newT[i], null)).ToString("yyyy-MM-dd HH:mm:ss");
                                        newT[i].GetType().GetProperty("Str" + propertys[j].Name).SetValue(newT[i], value, null);
                                    }
                                    propertys[j].SetValue(newT[i], null, null);
                                    continue;
                                }
                            }
                            catch { continue; }
                        }
                    }
                    catch { continue; }
                }
            }
        }
        #endregion
    }
}
