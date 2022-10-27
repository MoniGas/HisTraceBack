//
//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                       0\ = /0
//                    ___/`---'\___
//                    .' \\| |// '.
//                  / \\||| : |||// \
//                / _||||| -:- |||||- \
//                  | | \\\ - /// | |
//                | \_| ''\---/'' |_/ |
//                 \ .-\__ '-' ___/-. /
//              ___'. .' /--.--\ `. .'___
//           ."" '< `.___\_<|>_/___.' >' "".
//          | | : `- \`.;`\ _ /`;.`/ - ` : | |
//            \ \ `_. \_ __\ /__ _/ .-` / /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//
//    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
//                  佛祖保佑 永无BUG
//
//
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinqModel;

namespace Common.Argument
{
    /// <summary>
    /// code=0 成功；其余失败
    /// </summary>
    public static class ToJson
    {
        
        #region 返回类型转JSON
        /// <summary>
        /// 返回类型转JSON
        /// </summary>
        /// <param name="model">返回RetResult实体</param>
        /// <returns>JSON</returns>
        public static string ResultToJson(RetResult model)
        {
            StringBuilder strJSON = new StringBuilder("{");

            strJSON.Append("\"code\":" + Convert.ToInt32(model.IsSuccess) + ",\"Msg\":\"" + model.Msg + "\"");

            strJSON.AppendLine("}");
            return strJSON.ToString();
        }
        #endregion

        #region 模型转JSON
        /// <summary>
        /// 模型转JSON
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">模型</param>
        /// <param name="code">成功/失败</param>
        /// <param name="msg">返回消息</param>
        /// <returns>JSON</returns>
        public static string ModelToJson<T>(T t, int code, string msg)
        {
            StringBuilder strJSON = new StringBuilder("{");
            strJSON.Append("\"code\":" + code + ",\"Msg\":\"" + msg + "\",\"ObjModel\":{");

            T newT = t;
            if (newT == null)
            {
                strJSON.AppendLine("null");
            }
            else
            {
                PropertyInfo[] propertys = newT.GetType().GetProperties();
                for (int i = 0; i < propertys.Length; i++)
                {
                    try
                    {
                        string type = propertys[i].PropertyType.Name.ToString();
                        string full = propertys[i].PropertyType.FullName.ToString();
                        if (type.IndexOf("EntitySet") >= 0)
                        {
                            continue;
                        }
                        if (type.IndexOf("EntityRef") >= 0)
                        {
                            continue;
                        }
                        if (full.IndexOf("LinqModel") >= 0)
                        {
                            continue;
                        }
                        string name = propertys[i].Name;
                        object value = propertys[i].GetValue(newT, null);
                        strJSON.Append("\"" + name + "\":\"" + value + "\"");
                        strJSON.Append(",");
                    }
                    catch
                    {
                        continue;
                    }
                }
                strJSON.Remove(strJSON.Length - 1, 1);
            }
            strJSON.AppendLine("}}");
            return strJSON.ToString();
        }
        #endregion

        #region 列表转JSON
        /// <summary>
        /// 列表转JSON
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">列表</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="msg">返回消息</param>
        /// <returns>JSON</returns>
        public static string ListToJson<T>(List<T> t, int pageIndex, int pageSize, long totalCount, string msg)
        {
            StringBuilder strJSON = new StringBuilder("{");
            strJSON.Append("\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"totalCounts\":" + totalCount + ",\"Msg\":\"" + msg + "\",\"ObjList\":");
            List<T> newT = t;
            if (t == null || t.Count <= 0)
            {
                strJSON.Append("null");
            }
            else
            {
                strJSON.Append("[");
                for (int i = 0; i < newT.Count; i++)
                {
                    try
                    {
                        strJSON.Append("{");
                        PropertyInfo[] propertys = newT[i].GetType().GetProperties();
                        for (int j = 0; j < propertys.Length; j++)
                        {
                            string type = propertys[j].PropertyType.Name.ToString();
                            string full = propertys[j].PropertyType.FullName.ToString();
                            if (type.IndexOf("EntitySet") >= 0)
                            {
                                continue;
                            }
                            if (type.IndexOf("EntityRef") >= 0)
                            {
                                continue;
                            }
                            if (full.IndexOf("LinqModel") >= 0)
                            {
                                continue;
                            }
                            string name = propertys[j].Name;
                            object value = propertys[j].GetValue(newT[i], null);
                            strJSON.Append("\"" + name + "\":\"" + value + "\"");
                            //if (j < propertys.Length - 1) 
                            strJSON.Append(",");
                        }
                        strJSON.Remove(strJSON.Length - 1, 1);
                        strJSON.Append("}");
                        strJSON.Append(",");
                    }
                    catch { continue; }
                }
                strJSON.Remove(strJSON.Length - 1, 1);
                strJSON.Append("]");
            }
            strJSON.Append("}");

            return strJSON.ToString();
        }
        #endregion


        #region 列表转JSON
        /// <summary>
        /// 列表转JSON(适用查询列表页)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static BaseResultList NewListToJson<T>(List<T> t, int pageIndex, int pageSize, long totalCount, string msg)
        {
            BaseResultList result = new BaseResultList();
            result.Msg = msg;
            result.ObjList = t;
            result.pageSize = pageSize;
            result.pageIndex = pageIndex;
            result.totalCounts = totalCount;
            return result;
        }

        public static BaseResultList NewListToJson<T>(List<T> t, int pageIndex, int pageSize, long totalCount,object objModel, string msg)
        {
            BaseResultList result = new BaseResultList();
            result.Msg = msg;
            result.ObjList = t;
            result.pageSize = pageSize;
            result.pageIndex = pageIndex;
            result.totalCounts = totalCount;
            result.ObjModel = objModel;
            return result;
        }
        #endregion
        #region 模型转JSON
        /// <summary>
        /// 模型转JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static BaseResultModel NewModelToJson<T>(T t, string code, string msg)
        {
            BaseResultModel result = new BaseResultModel();
            result.Msg = msg;
            result.ObjModel = t;
            result.code = code;
            return result;
        }
        #endregion

        #region 模型转JSON
        /// <summary>
        /// 模型转JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ExtenResultModel NewModelToJson<T>(T modual, LoginInfo login, string code, string msg)
        {
            ExtenResultModel result = new ExtenResultModel();
            result.Msg = msg;
            result.Modual =modual;
            result.Login = login;
            result.code = code;
            return result;
        }
        #endregion
        #region 操作结果转JSON
        /// <summary>
        /// 操作结果转JSON(适用增加/修改/删除)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static BaseResultModel NewRetResultToJson(string code, string msg)
        {
            BaseResultModel result = new BaseResultModel();
            result.Msg = msg;
            result.ObjModel = null;
            result.code = code;
            return result;
        }

        //public static BaseResultModel NewRetResultToJsonNew(string code, string msg,object objModel)
        //{
        //    BaseResultModel result = new BaseResultModel();
        //    result.Msg = msg;
        //    result.ObjModel = objModel;
        //    result.code = code;
        //    return result;
        //}
        #endregion
    }
}
