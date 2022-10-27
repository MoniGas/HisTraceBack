using System;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using Common;
using Common.Argument;
using LinqModel;

namespace Dal
{
    /// <summary>
    /// 赵慧敏
    /// </summary>
    public class RegistDAL : DALBase
    {
        /// <summary>
        /// 农企注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Regist(Enterprise_Info model, string userName, string pwd)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    if (dataContext.Connection != null)
                        dataContext.Connection.Open();
                    DbTransaction tran = dataContext.Connection.BeginTransaction();
                    dataContext.Transaction = tran;
                    var enterprise = from d in dataContext.Enterprise_Info where d.EnterpriseName == model.EnterpriseName.Trim() select d;
                    if (enterprise.Any())
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "", "您的企业已经在本平台注册!");
                        return Ret;
                    }
                    var liUser = from d in dataContext.Enterprise_User where d.LoginName == userName.Trim() select d;
                    if (liUser.Any())
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "", "用户名已被注册！");
                        return Ret;
                    }
                    #region 添加企业信息
                    model.MainCode = model.MainCode;
                    model.Enterprise_Level = Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise);
                    if (model.PRRU_PlatForm_ID <= 0)
                    {
                        model.PRRU_PlatForm_ID = 0;
                    }
                    model.adddate = DateTime.Now;
                    model.AddTime = DateTime.Now;
                    model.ApprovalCodeType = 0;
                    model.Status = (int)EnumFile.Status.used;
                    model.Verify = (int)EnumFile.EnterpriseVerify.Try;
                    model.ShopVerify = (int)EnumFile.ShopVerify.Close;
                    model.WareHouseVerify = 0;
                    model.IsOpenShop = false;
                    dataContext.Enterprise_Info.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Enterprise_User user = new Enterprise_User();
                    user.Enterprise_Info_ID = model.Enterprise_Info_ID;
                    //企业管理员登录角色为平台角色
                    user.Enterprise_Role_ID = GetModel(Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise)).PRRU_PlatFormLevel_ID;
                    user.UserName = "管理员";
                    user.UserCode = "admin";
                    user.UserType = "默认";
                    user.Status = Convert.ToInt16(EnumFile.Status.used);
                    user.LoginName = userName.Trim();
                    user.LoginPassWord = pwd.Trim();
                    user.adddate = DateTime.Now;
                    dataContext.Enterprise_User.InsertOnSubmit(user);
                    dataContext.SubmitChanges();
                    tran.Commit();
                    #endregion

                    Ret.SetArgument(CmdResultError.NONE, "", "注册成功！");
                    return Ret;
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, ex.ToString(), "注册失败！");
            }
            return Ret;
        }

        public RetResult Check(long platId)
        {
            string Msg = "没有查到编码为【" + platId + "】的代理信息";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var enterprise = from d in dataContext.PRRU_PlatForm where d.PRRU_PlatForm_ID == platId select d;
                    if (enterprise.Any())
                    {
                        PRRU_PlatForm palt = enterprise.FirstOrDefault();
                        Msg = palt.CompanyName;
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        error = CmdResultError.NO_RESULT;
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        /// <summary>
        /// 判断农企是否已经在本平台注册
        /// </summary>
        /// <param name="enterpriseName"></param>
        /// <returns></returns>
        public RetResult RegistModify(string enterpriseName)
        {
            string Msg = "您的企业已经在本平台注册！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var enterprise = from d in dataContext.Enterprise_Info where d.EnterpriseName == enterpriseName.Trim() select d;
                    if (enterprise.Any())
                    {
                        Msg = "您的企业已经在本平台注册！";
                    }
                    else
                    {
                        error = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        /// <summary>
        /// 获取角色实体
        /// </summary>
        /// <param name="id">1:企业2监管部门3平台商</param>
        /// <returns>实体</returns>
        public PRRU_PlatFormLevel GetModel(int id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.PRRU_PlatFormLevel.FirstOrDefault(m => m.PRRU_PlatFormLevel_ID == id);
            }
        }
        /// <summary>
        /// 获取默认监管部门（平台）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PRRU_PlatForm GetPlatForm()
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.PRRU_PlatForm.FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取默认监管部门（平台）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UserName(string userName)
        {
            int count = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var liUser = from d in dataContext.Enterprise_User where d.LoginName == userName.Trim() select d;
                count = liUser.Count();
            }
            return count;
        }

        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>结果</returns>
        public Result GetPassWord(string phone, out string code)
        {
            code = string.Empty; ;
            Result result = new Result();
            //短信内容
            string content = string.Empty;
            //返回Json字符串
            string jsonStr = string.Empty;
            //发短信返回的Json串
            string jsonReturn = string.Empty;
            //发短信秘钥
            string secretKey = ConfigurationManager.AppSettings["secretKey"];
            string webServiceUrl = ConfigurationManager.AppSettings["WebServiceUrl"];
            //短信内容类
            MsgInfo msgInfo = new MsgInfo();
            MsgReturn msgReturn = new MsgReturn();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User user = dataContext.Enterprise_User.FirstOrDefault(m => m.LoginName == phone);
                    if (user != null)
                    {
                        result.ResultMsg = "您的手机号已经在平台注册，请重新填写手机号码!";
                        result.ResultCode = -1;
                        return result;
                    }
                    dataContext.SubmitChanges();
                    code = new Random().Next(100000, 999999).ToString();
                    //组织发送短信内容，转成Json串
                    content = "您好！您在农产品追溯公共服务平台的随机密码为：" + code + "，有效时间3分钟【广联信息】";
                    msgInfo.secretKey = secretKey;
                    string now = DateTime.Now.ToString("yyyy-MM-dd");
                    msgInfo.sms.sendDate = now;
                    msgInfo.sms.content = content;
                    msgInfo.sms.msType = 1;
                    msgInfo.sms.sendTime = now;
                    msgInfo.phoneNumbers[0] = phone;
                    //实例对象
                    SendMsg.SPortService client = new SendMsg.SPortService();
                    client.Url = webServiceUrl;
                    jsonStr = JsonHelper.DataContractJsonSerialize(msgInfo);
                    //发送短信
                    jsonStr = jsonStr.Replace(now, "");
                    jsonReturn = client.sendSMS(jsonStr);
                    msgReturn = JsonHelper.DataContractJsonDeserialize<MsgReturn>(jsonReturn);
                    if (msgReturn.errorCode == "0")
                    {
                        result.ResultMsg = "";
                        result.ResultCode = 0;
                    }
                    else
                    {
                        result.ResultMsg = "短信发送失败";
                        result.ResultCode = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = -2;
                result.ResultMsg = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 试用户注册
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="tel">电话</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public RetResult RegisterTry(Enterprise_Info model, string tel, string pwd)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    if (dataContext.Connection != null)
                        dataContext.Connection.Open();
                    DbTransaction tran = dataContext.Connection.BeginTransaction();
                    dataContext.Transaction = tran;
                    var liUser = from d in dataContext.Enterprise_User where d.LoginName == tel.Trim() select d;
                    if (liUser.Any())
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "", "手机号已被注册，请重新填写手机号码！");
                        return Ret;
                    }
                    #region 添加企业信息
                    string tryCode = ConfigurationManager.AppSettings["tryCode"];
                    model.Enterprise_Level = Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise);
                    model.PRRU_PlatForm_ID = Convert.ToInt32(GetPlatForm().PRRU_PlatForm_ID);
                    model.adddate = DateTime.Now;
                    model.AddTime = DateTime.Now;
                    model.ApprovalCodeType = 0;
                    model.Status = (int)EnumFile.EnterpriseVerify.Try;
                    model.validation = 2;
                    model.Verify = (int)EnumFile.EnterpriseVerify.Try;
                    model.ShopVerify = (int)EnumFile.ShopVerify.Close;
                    model.WareHouseVerify = 0;
                    model.IsOpenShop = false;
                    dataContext.Enterprise_Info.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    model.MainCode = tryCode + model.Enterprise_Info_ID.ToString();
                    Enterprise_User user = new Enterprise_User();
                    user.Enterprise_Info_ID = model.Enterprise_Info_ID;
                    //企业管理员登录角色为平台角色
                    user.Enterprise_Role_ID = GetModel(Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise)).PRRU_PlatFormLevel_ID;
                    user.UserName = "管理员";
                    user.UserCode = "admin";
                    user.UserType = "默认";
                    user.Status = Convert.ToInt16(EnumFile.Status.used);
                    user.LoginName = tel;
                    user.LoginPassWord = pwd.Trim();
                    user.adddate = DateTime.Now;
                    dataContext.Enterprise_User.InsertOnSubmit(user);
                    dataContext.SubmitChanges();
                    tran.Commit();
                    #endregion
                    Ret.SetArgument(CmdResultError.NONE, "", "注册成功！您的登录账号为：" + user.LoginName);
                    return Ret;
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, ex.ToString(), "注册失败！");
            }
            return Ret;
        }

        #region 企业注册接口（企业已通过Idcode验证）winform调用20200108张
        public int AddEnterpriseIdcodeHas(EnterpriseInfoRequest request, string enMainCode, out Enterprise_License licenseModel)
        {
            licenseModel = new Enterprise_License();
            int result;
            try
            {
                Enterprise_Info model = new Enterprise_Info();
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    if (dataContext.Connection != null)
                        dataContext.Connection.Open();
                    DbTransaction tran = dataContext.Connection.BeginTransaction();
                    dataContext.Transaction = tran;
                    var enterprise = from d in dataContext.Enterprise_Info where d.EnterpriseName == request.EnterpriseName.Trim() select d;
                    if (enterprise.Any())
                    {
                        result = -1;//您的企业已经在本平台注册!
                        return result;
                    }
                    var liUser = from d in dataContext.Enterprise_User where d.LoginName == request.LoginName.Trim() select d;
                    if (liUser.Any())
                    {
                        RetEnterpriseResult tempresult = new RetEnterpriseResult();
                        result = -2;//用户名已被注册！
                        return result;
                    }
                    #region 添加企业信息
                    model.MainCode = request.MainCode;
                    model.EnterpriseName = request.EnterpriseName;
                    model.Dictionary_UnitType_ID = request.Dictionary_UnitType_ID;
                    model.Dictionary_AddressSheng_ID = request.Dictionary_AddressSheng_ID;
                    model.Dictionary_AddressShi_ID = request.Dictionary_AddressShi_ID;
                    model.Dictionary_AddressQu_ID = request.Dictionary_AddressQu_ID;
                    model.Address = request.Address;
                    model.RequestCodeCount = Convert.ToInt32(ConfigurationManager.AppSettings["RequestCodeCount"]);
                    model.OverDraftCount = Convert.ToInt32(ConfigurationManager.AppSettings["OverDraftCount"]);
                    model.UsedCodeCount = 0;
                    model.LinkMan = request.LinkMan;
                    model.LinkPhone = request.LinkPhone;
                    model.TraceEnMainCode = enMainCode;
                    model.Enterprise_Level = Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise);
                    model.PRRU_PlatForm_ID = 0;
                    model.adddate = DateTime.Now;
                    model.AddTime = DateTime.Now;
                    model.ApprovalCodeType = 0;
                    model.Status = (int)EnumFile.Status.used;
                    model.Verify = (int)EnumFile.EnterpriseVerify.Try;
                    model.ShopVerify = (int)EnumFile.ShopVerify.Close;
                    model.WareHouseVerify = 0;
                    model.IsOpenShop = false;
                    model.BusinessLicence = request.BusinessLicence;
                    dataContext.Enterprise_Info.InsertOnSubmit(model);
                    dataContext.SubmitChanges();

                    Enterprise_User user = new Enterprise_User();
                    user.Enterprise_Info_ID = model.Enterprise_Info_ID;
                    //企业管理员登录角色为平台角色
                    user.Enterprise_Role_ID = GetModel(Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise)).PRRU_PlatFormLevel_ID;
                    user.UserName = "管理员";
                    user.UserCode = "admin";
                    user.UserType = "默认";
                    user.Status = Convert.ToInt16(EnumFile.Status.used);
                    user.LoginName = request.LoginName.Trim();
                    user.LoginPassWord = request.LoginPassWord.Trim();
                    user.adddate = DateTime.Now;
                    dataContext.Enterprise_User.InsertOnSubmit(user);
                    dataContext.SubmitChanges();

                    #region 添加企业授权记录（默认一个月） 2020年2月4日 刘晓杰 添加
                    licenseModel.EnterpriseID = model.Enterprise_Info_ID;
                    licenseModel.AdminID = user.Enterprise_User_ID;
                    licenseModel.OperateDate = DateTime.Now;
                    licenseModel.State = Convert.ToInt16(EnumFile.Status.used);
                    licenseModel.LicenseEndDate = DateTime.Now.AddMonths(1);
                    string dateNow = Convert.ToString(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")), 16);
                    string setDatestr = Convert.ToString(Convert.ToInt32(DateTime.Now.AddMonths(1).ToString("yyyyMMdd")), 16);
                    string licenseCode = model.MainCode + "&" + dateNow + "&" + setDatestr;
                    byte[] key = System.Text.Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["key"]);
                    byte[] iv = System.Text.Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["iv"]);
                    licenseModel.LicenseCode = Common.Tools.SecurityDES.Encrypt(licenseCode, key, iv);
                    dataContext.Enterprise_License.InsertOnSubmit(licenseModel);
                    dataContext.SubmitChanges();
                    #endregion

                    tran.Commit();
                    #endregion
                    //注册完调用IDcode接口添加授权key和主码
                    Enterprise_UserDAL useDal=new Enterprise_UserDAL();
                    RetResult retResult = useDal.GetCompanyMainCode("WebConnect");
                    result = 1;//注册成功！
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = -3;//注册出现异常！
                return result;
            }
        }

        public RetEnterpriseResult GetFhInfo(string mainCode)
        {
            RetEnterpriseResult result = new RetEnterpriseResult();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var entemp = dataContext.Enterprise_Info.FirstOrDefault(m => m.MainCode == mainCode && m.Status == (int)Common.EnumFile.Status.used);
                if (entemp != null)
                {
                    result.EnterpriseId = entemp.Enterprise_Info_ID;
                    var userTemp = dataContext.Enterprise_User.FirstOrDefault(d => d.Enterprise_Info_ID == entemp.Enterprise_Info_ID && d.UserType == "默认");
                    if (userTemp != null)
                    {
                        result.AdminId = userTemp.Enterprise_User_ID;
                    }
                    var enstemp = dataContext.Enterprise_License.FirstOrDefault(m => m.EnterpriseID == entemp.Enterprise_Info_ID && m.State == (int)EnumFile.Status.used);
                    if (enstemp != null)
                    {
                        result.LicenseEndDate = enstemp.LicenseEndDate.ToString();
                    }
                    else
                    {
                        #region 添加企业授权记录（默认企业添加时间加一个月） 2020年04月15日
                        Enterprise_License licenseModel = new Enterprise_License();
                        licenseModel.EnterpriseID = entemp.Enterprise_Info_ID;
                        licenseModel.AdminID = userTemp.Enterprise_User_ID;
                        licenseModel.OperateDate = DateTime.Now;
                        licenseModel.State = Convert.ToInt16(EnumFile.Status.used);
                        licenseModel.LicenseEndDate = entemp.adddate.Value.AddMonths(1);
                        string dateNow = Convert.ToString(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")), 16);
                        string setDatestr = Convert.ToString(Convert.ToInt32(DateTime.Now.AddMonths(1).ToString("yyyyMMdd")), 16);
                        string licenseCode = entemp.MainCode + "&" + dateNow + "&" + setDatestr;
                        byte[] key = System.Text.Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["key"]);
                        byte[] iv = System.Text.Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["iv"]);
                        licenseModel.LicenseCode = Common.Tools.SecurityDES.Encrypt(licenseCode, key, iv);
                        dataContext.Enterprise_License.InsertOnSubmit(licenseModel);
                        dataContext.SubmitChanges();
                        result.LicenseEndDate = licenseModel.LicenseEndDate.ToString();
                        #endregion
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 修改企业统一社会信用代码
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <param name="BusinessLicence"></param>
        /// <returns></returns>
        public RetResult EditEnterprise(long eId, string BusinessLicence)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == eId).FirstOrDefault();
                    if (enterprise != null && !string.IsNullOrEmpty(BusinessLicence))
                    {
                        enterprise.BusinessLicence = BusinessLicence;
                        dataContext.SubmitChanges();
                    }
                    Ret.SetArgument(CmdResultError.NONE, "", "操作成功：");
                    return Ret;
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, ex.ToString(), "注册失败！");
            }
            return Ret;
        }
    }
}
