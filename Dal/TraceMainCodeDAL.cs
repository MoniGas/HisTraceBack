using System;
using System.Linq;
using Common.Log;
using LinqModel;
using Common.Tools;

namespace Dal
{
    public class TraceMainCodeDAL : DALBase
    {
        /// <summary>
        /// 获取企业4位36进制的简码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetEnterpriseMainCode(string code)
        {
            string maincode = "";
            int codeint = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetPublicDataContext("PublicTraceConnect"))
            {
                try
                {
                    int count = dataContext.TraceMainCode.Count();
                    if (count > 0)
                    {
                        string lastdataSql = "select top 1 * from TraceMainCode order by AddDate desc";
                        TraceMainCode lastdata = dataContext.ExecuteQuery<TraceMainCode>(lastdataSql).FirstOrDefault();
                        string lamacode = lastdata.MainCode;
                        codeint = new BinarySystem36().Convert36ToNo(lamacode);
                        maincode = new BinarySystem36().gen36No((codeint + 1), 4);
                    }
                    else
                    {
                        string encode = "0001";
                        int codeNum = Convert.ToInt32(encode.Replace("0", ""));
                        maincode = new BinarySystem36().gen36No(codeNum, 4);
                    }
                    TraceMainCode model = new TraceMainCode();
                    model.AddDate = DateTime.Now;
                    model.MainCode = maincode;
                    model.TraceCode = code;
                    dataContext.TraceMainCode.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    string errData = "TraceMainCodeDAL.GetEnterpriseMainCode():TraceMainCode表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return maincode;
        }
    }
}
