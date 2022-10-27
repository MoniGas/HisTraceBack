/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-12

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于修改企业信息数据层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Configuration;
using InterfaceWeb;

namespace Dal
{
    public class EnterpriseInfoDAL : DALBase
    {
        public Enterprise_Info GetModel(string mainCode)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.Enterprise_Info.FirstOrDefault(m => m.MainCode == mainCode);
            }
        }

        /// <summary>
        /// 根据企业ID获取企业实体
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>实体</returns>
        public View_EnterprisePlatForm GetModelView(long id)
        {
            AreaInfo listAddress = Common.Argument.BaseData.listAddress;

            using (DataClassesDataContext dataContext = GetDataContext())
            {
                View_EnterprisePlatForm data = (from d in dataContext.View_EnterprisePlatForm
                                                where d.Enterprise_Info_ID == id
                                                select d).FirstOrDefault();
                if (data.Dictionary_AddressSheng_ID != null && data.Dictionary_AddressShi_ID != null && data.Dictionary_AddressQu_ID != null)
                {
                    data.sheng = listAddress.AddressList.FirstOrDefault(w => w.Address_ID == data.Dictionary_AddressSheng_ID).AddressName;
                    data.shi = listAddress.AddressList.FirstOrDefault(w => w.Address_ID == data.Dictionary_AddressShi_ID).AddressName;
                    data.qu = listAddress.AddressList.FirstOrDefault(w => w.Address_ID == data.Dictionary_AddressQu_ID).AddressName;
                }
                ClearLinqModel(data);
                return data;
            }
        }

        /// <summary>
        /// 根据企业ID获取企业实体
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>实体</returns>
        public Enterprise_Info GetModel(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                Enterprise_Info model = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == id);
                ClearLinqModel(model);
                return model;
            }
        }

        public PRRU_PlatForm GetPRRU_PlatForm(long PRRU_PlatForm_ID)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataModel = (from data in dataContext.PRRU_PlatForm
                                     where data.PRRU_PlatForm_ID == PRRU_PlatForm_ID
                                     select data).FirstOrDefault();

                    return DataModel;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 修改企业信息方法
        /// </summary>
        /// <param name="myinfo">企业信息model类</param>
        /// <returns></returns>
        public Common.Argument.RetResult Edit(string mainCode, string personName, string telephone, string email, string address, string memo, string webUrl, XElement logo, string file, string trade, string etrade)
        {
            string Msg = "企业认证败！";
            Common.Argument.CmdResultError error = Common.Argument.CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = (from d in dataContext.Enterprise_Info
                                where d.MainCode == mainCode
                                select d).FirstOrDefault();

                    if (data == null)
                    {
                        Msg = "数据错误！数据库中不存在该数据！";
                    }
                    else
                    {
                        data.zhizhao = file;
                        data.LinkMan = personName;
                        data.LinkPhone = telephone;
                        data.Email = email;
                        data.Address = address;
                        data.Memo = memo;
                        data.WebURL = webUrl;
                        data.Logo = logo;
                        data.Trade_ID = Convert.ToInt32(trade);
                        data.Etrade_ID = Convert.ToInt32(etrade);
                        dataContext.SubmitChanges();
                        Msg = "企业认证成功！";
                        error = Common.Argument.CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "EnterpriseInfoDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public Common.Argument.RetResult Edit(string mainCode, string yyzz)
        {
            string Msg = "企业认证失败！";
            Common.Argument.CmdResultError error = Common.Argument.CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = (from d in dataContext.Enterprise_Info
                                where d.MainCode == mainCode
                                select d).FirstOrDefault();

                    if (data == null)
                    {
                        Msg = "数据错误！数据库中不存在该数据！";
                    }
                    else
                    {
                        data.zhizhao = yyzz;
                        dataContext.SubmitChanges();
                        Msg = "企业认证成功！";
                        error = Common.Argument.CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "EnterpriseInfoDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 开通（关闭）商城
        /// </summary>
        /// <param name="mainCode">企业主码</param>
        /// <returns>返回结果</returns>
        public Common.Argument.RetResult OpenShop(string mainCode, string accountNum, string accountName, string linkPhone)
        {

            string Msg = "操作失败！";
            Common.Argument.CmdResultError error = Common.Argument.CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.Enterprise_Info
                                where d.MainCode == mainCode
                                select d).FirstOrDefault();

                    if (data == null)
                    {
                        Msg = "数据错误！数据库中不存在该数据！";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(accountNum))
                        {
                            Order_EnterpriseAccount account = new Order_EnterpriseAccount();
                            account.AccountNum = accountNum;
                            account.AccountName = accountName;
                            account.Enterprise_ID = data.Enterprise_Info_ID;
                            account.Status = (int)Common.EnumFile.Status.used;
                            account.LinkPhone = linkPhone;
                            new EnterpriseAccountDAL().Add(account);
                        }
                        data.IsOpenShop = !data.IsOpenShop;
                        dataContext.SubmitChanges();
                        if (data.IsOpenShop)
                        {
                            Msg = "开通商城成功！";
                        }
                        else
                        {
                            Msg = "关闭商城成功！";
                        }
                        error = Common.Argument.CmdResultError.NONE;
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "EnterpriseInfoDAL.OpenShop()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取企业主码简码20180815
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="enCode">简码主码4位</param>
        /// <returns></returns>
        public Common.Argument.RetResult EditEnJMainCode(long eid, string enCode)
        {
            string Msg = "获取失败！";
            Common.Argument.CmdResultError error = Common.Argument.CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = (from d in dataContext.Enterprise_Info
                                where d.Enterprise_Info_ID == eid
                                select d).FirstOrDefault();

                    if (data == null)
                    {
                        Msg = "数据错误！数据库中不存在该数据！";
                    }
                    else
                    {
                        data.TraceEnMainCode = enCode;
                        dataContext.SubmitChanges();
                        Msg = "获取成功！";
                        error = Common.Argument.CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "EnterpriseInfoDAL.EditEnJMainCode()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        #region 添加码标

        /// <summary>
        /// 添加码标
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void AddMb(List<Enterprise_MB> list)
        {
            using (var db = GetDataContext())
            {
                try
                {
                    list.ForEach(p =>
                    {
                        Enterprise_MB firstOrDefault = db.Enterprise_MB.FirstOrDefault(m => m.mb == p.mb && m.EnterpriseId == p.EnterpriseId);
                        if (null != firstOrDefault)
                        {
                            if (firstOrDefault.end_date != p.end_date)
                            {
                                firstOrDefault.end_date = p.end_date;
                                db.SubmitChanges();
                            }
                            else
                            {
                                list.Remove(firstOrDefault);
                            }
                            //db.Enterprise_MB.InsertOnSubmit(firstOrDefault);
                        }
                    });
                    db.Enterprise_MB.InsertAllOnSubmit(list);
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
        }

        public List<Enterprise_MB> GetMBList(long EnterpriseId)
        {
            using (var db = GetDataContext())
            {
                return db.Enterprise_MB.Where(m => m.EnterpriseId == EnterpriseId).ToList();
            }
        }
        #endregion

        #region
        public TokenInfo GetGYJToken(string mainCode, string appId, string appSecret)
        {
            TokenInfo result = new TokenInfo();
            try
            {
                //Enterprise_LicenseGYJ model = new Enterprise_EwmSysLogin();
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    //var enInfo = (from info in dataContext.Enterprise_Info
                    //              join license in dataContext.Enterprise_License on info.Enterprise_Info_ID equals license.EnterpriseID
                    //              where info.MainCode == mainCode
                    //              select new
                    //              {
                    //                  EnterpriseID = info.Enterprise_Info_ID,
                    //                  EnterpriseName = info.EnterpriseName,
                    //                  LicenseType = license.LicenseType,
                    //                  LicenseEndDate = license.LicenseEndDate
                    //              }).FirstOrDefault();
                    Enterprise_Info enInfo = dataContext.Enterprise_Info.FirstOrDefault(m => m.MainCode == mainCode);
                    if (enInfo != null)
                    {
                        AuthorCode authorCode = dataContext.AuthorCode.Where(m => m.appId == appId && m.appSecret == appSecret).FirstOrDefault();
                        if (authorCode != null)
                        {
                            var data = dataContext.Enterprise_LicenseGYJ.Where(m => m.LicenseType == authorCode.LicenseType);
                            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
                            if (data.Count() > 0 && !string.IsNullOrEmpty(currDate))
                            {
                                Enterprise_LicenseGYJ temp = data.Where(m => m.expiresInDateTime >= Convert.ToDateTime(currDate)).OrderByDescending(m => m.ID).FirstOrDefault();
                                if (temp != null)
                                {
                                    string intcurrDate = currDate.Replace("-","");
                                    if (Convert.ToInt32(intcurrDate) > Convert.ToInt32(temp.currentTime))
                                    {
                                        TokenResult ret = GetToken(appId, appSecret);
                                        if (ret.returnCode == 1)
                                        {
                                            Enterprise_LicenseGYJ gyjInfo = new Enterprise_LicenseGYJ();
                                            gyjInfo.accessToken = ret.accessToken;
                                            gyjInfo.currentTime = ret.currentTime;
                                            gyjInfo.expiresIn = ret.expiresIn.ToString();
                                            string time = ret.currentTime.Substring(0, 4) + "-" + ret.currentTime.Substring(4, 2) + "-" + ret.currentTime.Substring(6, 2) + " 23:59:59";
                                            //gyjInfo.expiresInDateTime = Convert.ToDateTime(time).AddDays(ret.expiresIn);
                                            gyjInfo.expiresInDateTime = Convert.ToDateTime(time);
                                            gyjInfo.LicenseType = authorCode.LicenseType.Value;
                                            gyjInfo.todayRemainVisitCount = ret.todayRemainVisitCount;
                                            gyjInfo.APPID = appId;
                                            gyjInfo.AppSecret = appSecret;
                                            Enterprise_LicenseGYJRecord record = new Enterprise_LicenseGYJRecord();
                                            record.accessToken = ret.accessToken;
                                            record.AddDate = DateTime.Now;
                                            record.APPID = appId;
                                            record.AppSecret = appSecret;
                                            record.DiaoInterfaceDate = DateTime.Now;
                                            record.EnterpriseID = enInfo.Enterprise_Info_ID;
                                            record.EnterpriseName = enInfo.EnterpriseName;
                                            record.LicenseType = authorCode.LicenseType;
                                            dataContext.Enterprise_LicenseGYJ.InsertOnSubmit(gyjInfo);
                                            dataContext.Enterprise_LicenseGYJRecord.InsertOnSubmit(record);
                                            dataContext.SubmitChanges();
                                            result.accessToken = gyjInfo.accessToken;
                                            result.currentTime = gyjInfo.currentTime;
                                            result.expiresIn = gyjInfo.expiresIn;
                                            result.expiresInDateTime = gyjInfo.expiresInDateTime.Value.ToString("yyyy-MM-dd");
                                            result.LicenseType = authorCode.LicenseType.Value;
                                            result.todayRemainVisitCount = gyjInfo.todayRemainVisitCount;
                                            result.returnCode = 1;
                                            result.returnMsg = "成功";
                                        }
                                        else
                                        {
                                            result.returnCode = ret.returnCode;
                                            result.returnMsg = ret.returnMsg;
                                        }
                                    }
                                    else
                                    {
                                        result.accessToken = temp.accessToken;
                                        result.currentTime = temp.currentTime;
                                        result.expiresIn = temp.expiresIn;
                                        result.expiresInDateTime = temp.expiresInDateTime.Value.ToString("yyyy-MM-dd");
                                        result.LicenseType = authorCode.LicenseType.Value;
                                        result.todayRemainVisitCount = temp.todayRemainVisitCount;
                                        result.returnCode = 1;
                                        result.returnMsg = "成功";
                                        Enterprise_LicenseGYJRecord record = new Enterprise_LicenseGYJRecord();
                                        record.accessToken = temp.accessToken;
                                        record.AddDate = DateTime.Now;
                                        record.APPID = appId;
                                        record.AppSecret = appSecret;
                                        record.DiaoInterfaceDate = DateTime.Now;
                                        record.EnterpriseID = enInfo.Enterprise_Info_ID;
                                        record.EnterpriseName = enInfo.EnterpriseName;
                                        record.LicenseType = authorCode.LicenseType;
                                        dataContext.Enterprise_LicenseGYJRecord.InsertOnSubmit(record);
                                        dataContext.SubmitChanges();
                                    }
                                }
                                else
                                {
                                    TokenResult ret = GetToken(appId, appSecret);
                                    if (ret.returnCode == 1)
                                    {
                                        Enterprise_LicenseGYJ gyjInfo = new Enterprise_LicenseGYJ();
                                        gyjInfo.accessToken = ret.accessToken;
                                        gyjInfo.currentTime = ret.currentTime;
                                        gyjInfo.expiresIn = ret.expiresIn.ToString();
                                        string time = ret.currentTime.Substring(0, 4) + "-" + ret.currentTime.Substring(4, 2) + "-" + ret.currentTime.Substring(6, 2) + " 23:59:59";
                                        //gyjInfo.expiresInDateTime = Convert.ToDateTime(time).AddDays(ret.expiresIn);
                                        gyjInfo.expiresInDateTime = Convert.ToDateTime(time);
                                        gyjInfo.LicenseType = authorCode.LicenseType.Value;
                                        gyjInfo.todayRemainVisitCount = ret.todayRemainVisitCount;
                                        gyjInfo.APPID = appId;
                                        gyjInfo.AppSecret = appSecret;
                                        Enterprise_LicenseGYJRecord record = new Enterprise_LicenseGYJRecord();
                                        record.accessToken = ret.accessToken;
                                        record.AddDate = DateTime.Now;
                                        record.APPID = appId;
                                        record.AppSecret = appSecret;
                                        record.DiaoInterfaceDate = DateTime.Now;
                                        record.EnterpriseID = enInfo.Enterprise_Info_ID;
                                        record.EnterpriseName = enInfo.EnterpriseName;
                                        record.LicenseType = authorCode.LicenseType;
                                        dataContext.Enterprise_LicenseGYJ.InsertOnSubmit(gyjInfo);
                                        dataContext.Enterprise_LicenseGYJRecord.InsertOnSubmit(record);
                                        dataContext.SubmitChanges();
                                        result.accessToken = gyjInfo.accessToken;
                                        result.currentTime = gyjInfo.currentTime;
                                        result.expiresIn = gyjInfo.expiresIn;
                                        result.expiresInDateTime = gyjInfo.expiresInDateTime.Value.ToString("yyyy-MM-dd");
                                        result.LicenseType = authorCode.LicenseType.Value;
                                        result.todayRemainVisitCount = gyjInfo.todayRemainVisitCount;
                                        result.returnCode = 1;
                                        result.returnMsg = "成功";
                                    }
                                    else
                                    {
                                        result.returnCode = ret.returnCode;
                                        result.returnMsg = ret.returnMsg;
                                    }
                                }
                            }
                            else
                            {
                                TokenResult ret = GetToken(appId, appSecret);
                                if (ret.returnCode == 1)
                                {
                                    Enterprise_LicenseGYJ gyjInfo = new Enterprise_LicenseGYJ();
                                    gyjInfo.accessToken = ret.accessToken;
                                    gyjInfo.currentTime = ret.currentTime;
                                    gyjInfo.expiresIn = ret.expiresIn.ToString();
                                    string time = ret.currentTime.Substring(0, 4) + "-" + ret.currentTime.Substring(4, 2) + "-" + ret.currentTime.Substring(6, 2) + " 23:59:59";
                                    //gyjInfo.expiresInDateTime = Convert.ToDateTime(time).AddDays(ret.expiresIn);
                                    gyjInfo.expiresInDateTime = Convert.ToDateTime(time);
                                    gyjInfo.LicenseType = authorCode.LicenseType.Value;
                                    gyjInfo.todayRemainVisitCount = ret.todayRemainVisitCount;
                                    gyjInfo.APPID = appId;
                                    gyjInfo.AppSecret = appSecret;
                                    Enterprise_LicenseGYJRecord record = new Enterprise_LicenseGYJRecord();
                                    record.accessToken = ret.accessToken;
                                    record.AddDate = DateTime.Now;
                                    record.APPID = appId;
                                    record.AppSecret = appSecret;
                                    record.DiaoInterfaceDate = DateTime.Now;
                                    record.EnterpriseID = enInfo.Enterprise_Info_ID;
                                    record.EnterpriseName = enInfo.EnterpriseName;
                                    record.LicenseType = authorCode.LicenseType;
                                    dataContext.Enterprise_LicenseGYJ.InsertOnSubmit(gyjInfo);
                                    dataContext.Enterprise_LicenseGYJRecord.InsertOnSubmit(record);
                                    dataContext.SubmitChanges();
                                    result.accessToken = gyjInfo.accessToken;
                                    result.currentTime = gyjInfo.currentTime;
                                    result.expiresIn = gyjInfo.expiresIn;
                                    result.expiresInDateTime = gyjInfo.expiresInDateTime.Value.ToString("yyyy-MM-dd");
                                    result.LicenseType = authorCode.LicenseType.Value;
                                    result.todayRemainVisitCount = gyjInfo.todayRemainVisitCount;
                                    result.returnCode = 1;
                                    result.returnMsg = "成功";
                                }
                                else
                                {
                                    result.returnCode = ret.returnCode;
                                    result.returnMsg = ret.returnMsg;
                                }
                            }
                        }
                        else
                        {
                            result = null;
                        }
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        #region 调取国药监接口
        /// <summary>
        /// UDI接口地址
        /// </summary>
        private static string _URL = ConfigurationManager.AppSettings["UDI_URL"];
        public static TokenResult GetToken(string appId, string appSecret)
        {
            RequestBase r = new RequestBase();
            r.appId = appId;
            r.appSecret = appSecret;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("params", JsonHelper.ObjectToJSON(r));
            string retMsg = WebClient.sendPost(_URL + "/token/get", param, "post");
            retMsg = retMsg.Replace("\\", "");
            TokenResult retResult = JsonDes.JsonDeserialize<TokenResult>(retMsg);
            return retResult;
        }

        public class JsonDes
        {
            public static T JsonDeserialize<T>(string jsonString)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }
        #endregion
        #endregion

    }
}
