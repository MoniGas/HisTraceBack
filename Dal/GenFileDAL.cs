using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Argument;
using System.IO;

namespace Dal
{
    public class GenFileDAL:DALBase
    {
        /// <summary>
        /// 获取所有需要生成码包的数据
        /// </summary>
        /// <returns></returns>
        public List<RequestCodeSettingAdd> GetsSettingAdd(string conStr)
        {
            List<RequestCodeSettingAdd> model = new List<RequestCodeSettingAdd>();
            using (DataClassesDataContext db = GetContext(conStr))
            {
                try
                {
                    model = db.RequestCodeSettingAdd.Where(p => p.State < 2).ToList();
                }
                catch
                { }
            }
            return model;
        }
        public RequestCodeSetting GetSetting(long id, string conStr)
        {
            RequestCodeSetting model = new RequestCodeSetting();
            using (DataClassesDataContext dct = GetContext(conStr))
            {
                try
                {
                    model = dct.RequestCodeSetting.Where(p => p.ID==id).FirstOrDefault();
                }
                catch(Exception ex)
                {
                    WriteLogTest(conStr + ":" + ex.Message,"GenFile");
                }
            }
            return model;
        }
        public RequestCode GetRequestCode(long requestId, string conStr)
        {
            RequestCode model = new RequestCode();
            using (DataClassesDataContext dct = GetContext(conStr))
            {
                try
                {
                    model = dct.RequestCode.Where(p => p.RequestCode_ID==requestId).FirstOrDefault();
                }
                catch
                { }
            }
            return model;
        }
        public Material GetMaterial(long materialId, string conStr)
        {
            Material model = new Material();
            using (DataClassesDataContext dct = GetContext(conStr))
            {
                try
                {
                    model = dct.Material.Where(p => p.Material_ID == materialId).FirstOrDefault();
                }
                catch
                { }
            }
            return model;
        }
        /// <summary>
        /// 获取码数据
        /// </summary>
        /// <param name="routeId"></param>
        /// <returns></returns>
        public List<Enterprise_FWCode_00> GetFWCode(RequestCode rCode, RequestCodeSetting codeSetting, string conStr,out string sqlStr)
        {
            sqlStr = "";
            string sql;
            List<Enterprise_FWCode_00> model = new List<Enterprise_FWCode_00>();
            //查找码库
            Route_DataBase dataBase = null;
            using (DataClassesDataContext dct = GetContext(conStr))
            {
                dataBase = dct.Route_DataBase.Where(p => p.Route_DataBase_ID == rCode.Route_DataBase_ID).FirstOrDefault();
            }
            if (dataBase == null)
            {
                return model;
            }
            string codeConStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                            dataBase.DataSource.ToString(), dataBase.DataBaseName.ToString(),
                            dataBase.UID.ToString(), dataBase.PWD.ToString());
            using (DataClassesDataContext dataContextDynamic = GetContext(codeConStr))
            {
                //自定义拆分：主批次，查找码表中RequestSetID为null的数据；子批次，查找码表中RequestSetID不为null的数据
                if (codeSetting.BathPartType == (int)Common.EnumFile.BatchPartType.Custom && codeSetting.BatchType==1)//自定义拆分的主批次
                {
                     sql = string.Format("select* from {0} " +
                        " where RequestCode_ID={1} and  RequestSetID is null", dataBase.TableName, codeSetting.RequestID);
                    model = dataContextDynamic.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                }
                else if (codeSetting.BathPartType == (int)Common.EnumFile.BatchPartType.Custom && codeSetting.BatchType == 2)//自定义拆分的子批次
                {
                     sql = string.Format("select* from {0} " +
                        "where Enterprise_FWCode_ID>=(select Enterprise_FWCode_ID from  {0} where RequestCode_ID={3} " +
                        "and EWM ='{1}')" +
                        "and Enterprise_FWCode_ID<=(select Enterprise_FWCode_ID from  {0} where RequestCode_ID={3} " +
                        "and EWM ='{2}') and  RequestSetID={4}", dataBase.TableName, codeSetting.beginNum, codeSetting.endNum, codeSetting.RequestID, codeSetting.ID);
                    model = dataContextDynamic.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                }
                else//顺序拆分或者没有拆分的数据
                {
                    long totalCount = codeSetting.Count;
                    long minusCount = codeSetting.beginCode.Value - rCode.StartNum.Value;
                     sql = string.Format("select top " + totalCount + " * from {0} where RequestCode_ID={1}"
                                    + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                    + " Enterprise_FWCode_ID  from {0} where RequestCode_ID={1}"
                                    + " order by Enterprise_FWCode_ID  )  order by Enterprise_FWCode_ID ",
                                    dataBase.TableName, codeSetting.RequestID, codeSetting.endNum, codeSetting.RequestID, codeSetting.ID);
                    model = dataContextDynamic.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                }
            }
            sqlStr = sql;
            return model;
        }
        public RetResult UpeateState(long Id,string fileUrl,string webUrl, string conStr)
        {
            RetResult model = new RetResult();
            using (DataClassesDataContext dct = GetContext(conStr))
            {
                try
                {
                   string sql = string.Format(@"update RequestCodeSettingAdd set FileURL='{0}',
                                               WebURL='{1}',State={2} where ID={3}",fileUrl,webUrl, 2,Id.ToString());
                   int update = dct.ExecuteQuery<int>(sql).FirstOrDefault();
                    
                }
                catch
                { }
            }
            return model;
        }
        public static void WriteLogTest(string msg, string directoryName)
        {
            string errlogpath = System.Configuration.ConfigurationManager.AppSettings["ErrLogPath"];
            errlogpath = errlogpath + "\\" + directoryName + "\\" + DateTime.Now.ToString("yyyyMM");
            string test = System.Configuration.ConfigurationManager.AppSettings["test"];
            if (test == "1")
            {
                if (!Directory.Exists(errlogpath))
                {
                    Directory.CreateDirectory(errlogpath);
                }
                using (StreamWriter sw = new StreamWriter(errlogpath + "\\" + DateTime.Now.ToString("dd") + "errorLog.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "------" + msg);
                }
            }
        }
    }
}
