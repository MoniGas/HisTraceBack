/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-16

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于用户管理数据层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common;
using LinqModel;
using InterfaceWeb;
using LinqModel.InterfaceModels;
using System.Configuration;
using System.Data.Common;

namespace Dal
{
    public class Enterprise_UserDAL : DALBase
    {
        #region 获取用户信息集合方法
        /// <summary>
        /// 获取用户信息集合方法
        /// </summary>
        /// <param name="id">农企ID</param>
        /// <param name="pageIndex">分页码</param>
        /// <param name="totalCount">集合总数</param>
        /// <returns></returns>
        public List<View_EnterpriseUserAndRole> GetList(long id, int pageIndex, out long totalCount, string userName, string userRole)
        {
            totalCount = 1;
            List<View_EnterpriseUserAndRole> dataList = new List<View_EnterpriseUserAndRole>();

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {


                    dataList = (from data in dataContext.View_EnterpriseUserAndRole
                                where data.Enterprise_Info_ID == id && data.Status == (int)EnumFile.Status.used && data.UserType != "默认"
                                select data).ToList();

                    if (!string.IsNullOrEmpty(userName))
                    {
                        dataList = dataList.Where(w => w.UserName.Contains(userName.Trim())).ToList();
                    }

                    if (!string.IsNullOrEmpty(userRole))
                    {
                        dataList = dataList.Where(w => w.RoleName == userRole).ToList();
                    }
                    if (dataList.Count != 0)
                    {
                        totalCount = dataList.Count;
                    }
                    if (pageIndex > 0)
                    {
                        dataList = dataList.OrderByDescending(m => m.Enterprise_User_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    ClearLinqModel(dataList);

                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        public List<View_EnterpriseLevelUser> GetLeveUser(long id, out long totalCount)
        {
            totalCount = 0;
            List<View_EnterpriseLevelUser> dataList = new List<View_EnterpriseLevelUser>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataList = (from data in dataContext.View_EnterpriseLevelUser
                                where data.Enterprise_Info_ID == id && data.Status == (int)EnumFile.Status.used && data.UserType != "注册"
                                select data).ToList();
                    totalCount = dataList.Count;
                    ClearLinqModel(dataList);
                    return dataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region 新增用户方法
        /// <summary>
        /// 新增用户方法
        /// </summary>
        /// <param name="objEnterprise_User">用户linq model对象</param>
        /// <returns>返回操作结果</returns>
        public RetResult Add(Enterprise_User objEnterprise_User)
        {
            string msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.Enterprise_User.InsertOnSubmit(objEnterprise_User);
                    dataContext.SubmitChanges();

                    msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                msg = "数据库连接失败！";
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, msg, msg);
            return Ret;
        }
        #endregion

        #region 删除方法
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public RetResult Del(long id)
        {
            LoginInfo user = SessCokie.Get;
            string msg = "删除失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User data = (from d in dataContext.Enterprise_User
                                            where d.Enterprise_User_ID == id
                                            select d).FirstOrDefault();
                    if (id == user.UserID)
                    {
                        msg = "您无权删除自己账号！";
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        data.Status = (int)EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        msg = "删除成功！";
                        error = CmdResultError.NONE;
                        List<EnterprsieDI_User> userList = dataContext.EnterprsieDI_User.Where(p => p.Enterprise_User_ID == data.Enterprise_User_ID).ToList();
                        foreach (var sub in userList)
                        {
                            sub.Status = (int)EnumFile.Status.delete;
                            dataContext.SubmitChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg = "数据库连接失败！";
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, msg, msg);
            return Ret;
        }
        #endregion

        #region 更新方法
        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="objEnterprise_User">用户信息对象</param>
        /// <returns>返回操作结果</returns>
        public RetResult Update(Enterprise_User objEnterprise_User)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.Enterprise_User
                                    where data.Enterprise_User_ID == objEnterprise_User.Enterprise_User_ID
                                    select data).FirstOrDefault();

                    if (objEnterprise_User.Enterprise_Role_ID != null)
                    {
                        dataInfo.Enterprise_Role_ID = objEnterprise_User.Enterprise_Role_ID;
                    }

                    if (!string.IsNullOrEmpty(objEnterprise_User.UserName))
                    {
                        dataInfo.UserName = objEnterprise_User.UserName;
                    }

                    if (!string.IsNullOrEmpty(objEnterprise_User.UserCode))
                    {
                        dataInfo.UserCode = objEnterprise_User.UserCode;
                    }

                    if (!string.IsNullOrEmpty(objEnterprise_User.LoginName))
                    {
                        dataInfo.LoginName = objEnterprise_User.LoginName;
                    }

                    if (!string.IsNullOrEmpty(objEnterprise_User.UserPhone))
                    {
                        dataInfo.UserPhone = objEnterprise_User.UserPhone;
                    }

                    if (!string.IsNullOrEmpty(objEnterprise_User.UserAddress))
                    {
                        dataInfo.UserAddress = objEnterprise_User.UserAddress;
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "更新成功！", "更新成功！");
                }
            }
            catch (Exception e)
            {
                Ret.SetArgument(CmdResultError.NONE, "数据库连接失败！", "数据库连接失败！");
            }

            return Ret;
        }
        #endregion

        #region 根据ID获取用户信息
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回用户信息对象</returns>
        public View_EnterpriseUserAndRole GetModel(long id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    View_EnterpriseUserAndRole dataInfo =
                        dataContext.View_EnterpriseUserAndRole.FirstOrDefault(p => p.Enterprise_User_ID == id);
                    //(from data in dataContext.View_EnterpriseUserAndRole
                    //                                             where data.Enterprise_User_ID == id
                    //                                             select data).FirstOrDefault();
                    ClearLinqModel(dataInfo);
                    return dataInfo;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        public View_EnterpriseLevelUser GetLevelModel(long id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    View_EnterpriseLevelUser dataInfo = (from data in dataContext.View_EnterpriseLevelUser
                                                         where data.Enterprise_User_ID == id
                                                         select data).FirstOrDefault();
                    return dataInfo;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #region 修改密码方法
        /// <summary>
        /// 修改密码方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="loginPassWord">新密码</param>
        /// <returns>返回操作结果</returns>
        public RetResult UpdatePas(long id, string oldPwd, string newPwd, string surepwd)
        {
            string Msg = "更新失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.Enterprise_User
                                    where data.Enterprise_User_ID == id
                                    select data).FirstOrDefault();

                    if (!dataInfo.LoginPassWord.Trim().Equals(oldPwd.Trim()))
                    {
                        Msg = "更新失败！旧密码错误！";
                        error = CmdResultError.PARAMERROR;
                    }
                    else
                    {
                        dataInfo.LoginPassWord = newPwd;

                        dataContext.SubmitChanges();

                        Msg = "更新成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch (Exception e)
            {
                Msg = "连接数据库失败！";
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #endregion

        #region 重置密码方法
        /// <summary>
        /// 重置密码方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回操作结果</returns>
        public RetResult ResetPas(long id)
        {
            string Msg = "密码重置失败！";
            CmdResultError error = CmdResultError.EXCEPTION;

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User dataInfo = (from data in dataContext.Enterprise_User
                                                where data.Enterprise_User_ID == id
                                                select data).FirstOrDefault();

                    dataInfo.LoginPassWord = "123456";

                    dataContext.SubmitChanges();

                    Msg = "密码重置成功，密码为123456";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                Msg = "数据库连接失败！";
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #endregion

        #region 验证登录名是否可用
        /// <summary>
        /// 验证登录名是否可用
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns>true：可用 false:不可用</returns>
        public RetResult VerifyLoginName(string loginName, long id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User dataInfo = (from data in dataContext.Enterprise_User
                                                where data.LoginName == loginName && data.Enterprise_User_ID != id && data.Status != (int)EnumFile.Status.delete
                                                select data).FirstOrDefault();

                    if (dataInfo != null)
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "登录名已存在！", "登录名已存在！");
                    }
                    else
                    {
                        Ret.SetArgument(CmdResultError.NONE, "登录名可用！", "登录名可用！");
                    }

                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接错误！", "数据库连接错误！");
            }


            return Ret;
        }
        #endregion

        public Enterprise_User GetEntity(long id)
        {
            Enterprise_User result = new Enterprise_User();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.Enterprise_User.FirstOrDefault(m => m.Enterprise_User_ID == id);
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public Enterprise_User GetExcelUserModel(string userName, string pwd)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User dataInfo = dataContext.Enterprise_User.FirstOrDefault(m => m.LoginName == userName &&
                        m.LoginPassWord == pwd && m.Status == (int)EnumFile.Status.used && m.UserType == "默认");
                    ClearLinqModel(dataInfo);
                    return dataInfo;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #region 私有云登录
        public BaseResultModel PrivateLogin(string loginname, string password, string token, string tokencode, string serviceID,string enMainCode)
        {
            BaseResultModel brm = new BaseResultModel();
            PrivateLoginResponse usermodel = new PrivateLoginResponse();
            string idcodeUrl = ConfigurationManager.AppSettings["interfaceUrl"];
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    //1.根据账户名密码判断是否存在
                    var model = dataContext.View_EnterpriseInfoUser.FirstOrDefault(t => t.LoginName == loginname && t.LoginPassWord == password);
                    if (model != null)//存在判断状态
                    {
                        if (model.MainCode == null || model.MainCode == "" || model.UserStatus != (int)EnumFile.Status.used)
                        {
                            brm.code = "-1";
                            brm.Msg = "您的企业已被暂停使用";
                        }
                        else
                        {
                            var info = dataContext.Enterprise_Info.FirstOrDefault(t => t.Enterprise_Info_ID == model.Enterprise_Info_ID);
                            brm.code = "1";
                            brm.Msg = "登录成功";
                            #region 赋值
                            usermodel.Address = model.Address;
                            usermodel.BusinessLicence = info.BusinessLicence;
                            usermodel.Dictionary_AddressQu_ID = long.Parse(info.Dictionary_AddressQu_ID.ToString());
                            usermodel.Dictionary_AddressSheng_ID = long.Parse(info.Dictionary_AddressSheng_ID.ToString());
                            usermodel.Dictionary_AddressShi_ID = long.Parse(info.Dictionary_AddressShi_ID.ToString());
                            usermodel.Dictionary_UnitType_ID = model.Dictionary_UnitType_ID;
                            usermodel.Email = model.Email;
                            usermodel.EnterPriseID = model.Enterprise_Info_ID;
                            usermodel.EnterpriseName = model.EnterpriseName;
                            usermodel.Etrade_ID = long.Parse(info.Etrade_ID.ToString());
                            usermodel.LicenseEndDate = info.LicenseEndDate;
                            int compNum = DateTime.Compare(DateTime.Now, DateTime.Parse(info.LicenceEndDate.ToString()));
                            if (compNum >= 0)//大于代表还没过期
                            {
                                usermodel.IsExpired = 0;

                            }
                            else
                            {
                                usermodel.IsExpired = 1;
                            }
                            usermodel.IsSimple = 2;
                            if (model.Enterprise_Role_ID == 1)
                            {
                                usermodel.IsSubUser = "1";
                            }
                            else
                            {
                                usermodel.IsSubUser = "2";
                            }
                            usermodel.LinkMan = model.LinkMan;
                            usermodel.LinkPhone = model.LinkPhone;
                            usermodel.LoginName = model.LoginName;
                            usermodel.LoginPwd = model.LoginPassWord;
                            usermodel.MainCode = model.MainCode;
                            usermodel.Token = info.Token;
                            usermodel.TokenCode = info.TokenCode;
                            usermodel.Trade_ID = long.Parse(info.Trade_ID.ToString());
                            usermodel.UserID = model.Enterprise_User_ID;
                            usermodel.UserName = model.UserName;
                            #endregion
                            brm.ObjModel = usermodel;
                        }
                    }
                    else
                    {
                        //查不到，先判断是不是密码输错了
                        var modelbyname = dataContext.View_EnterpriseInfoUser.FirstOrDefault(t => t.LoginName == loginname);
                        if (modelbyname != null)
                        {
                            brm.code = "-1";
                            brm.Msg = "密码错误";
                        }
                        else//走到这儿那就是彻底查不到，走注册流程
                        {
                            //先去utc认证
                            IdcodeLoginverifyInfo info = new IdcodeLoginverifyInfo();
                            string functionUrl = "/sp/idcode/medical/loginverify";
                            Dictionary<string, string> dataDic = new Dictionary<string, string>
                            {
                                {"access_token", token},
                                {"login_name", loginname},
                                {"login_pswd", password},
                                {"time", DEncrypt.GetTimeStamp(DateTime.Now)}
                            };
                            dataDic.Add("hash", DEncrypt.GetHash(functionUrl, dataDic, token));
                            string rst = APIHelper.sendPost(idcodeUrl + functionUrl, dataDic, "post");
                            if (rst == "未能解析此远程名称: 'api.idcode.org.cn'" || rst == "未能解析此远程名称: 'api.utcgl.com'")
                            {
                                brm.code = "-1";
                                brm.Msg = "utc接口调用失败";
                            }
                            else
                            {
                                info = JsonDes.JsonDeserialize<IdcodeLoginverifyInfo>(rst);
                                if (info.result_code == 1)
                                {
                                    //认证成功后将当前企业添加到数据库中
                                    DbTransaction tran = dataContext.Connection.BeginTransaction();
                                    dataContext.Transaction = tran;
                                    try
                                    {
                                        #region Enterprise_Info
                                        Enterprise_Info infomodel = new Enterprise_Info();
                                        infomodel.MainCode = info.organunit_oid;
                                        infomodel.EnterpriseName = info.organunit_name;
                                        infomodel.Dictionary_UnitType_ID = info.unittype_id.ToString();
                                        infomodel.Dictionary_AddressSheng_ID = long.Parse(info.province_id.ToString());
                                        infomodel.Dictionary_AddressShi_ID = info.city_id;
                                        infomodel.Dictionary_AddressQu_ID = info.area_id;
                                        infomodel.Address = info.organunit_address;
                                        infomodel.RequestCodeCount = Convert.ToInt32(ConfigurationManager.AppSettings["RequestCodeCount"]);
                                        infomodel.OverDraftCount = Convert.ToInt32(ConfigurationManager.AppSettings["OverDraftCount"]);
                                        infomodel.UsedCodeCount = 0;
                                        infomodel.LinkMan = info.linkman;
                                        infomodel.LinkPhone = info.linkphone;
                                        infomodel.TraceEnMainCode = enMainCode;
                                        infomodel.Enterprise_Level = Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise);
                                        infomodel.PRRU_PlatForm_ID = 0;
                                        infomodel.adddate = DateTime.Now;
                                        infomodel.AddTime = DateTime.Now;
                                        infomodel.ApprovalCodeType = 0;
                                        infomodel.Status = (int)EnumFile.Status.used;
                                        infomodel.Verify = (int)EnumFile.EnterpriseVerify.Try;
                                        infomodel.ShopVerify = (int)EnumFile.ShopVerify.Close;
                                        infomodel.WareHouseVerify = 0;
                                        infomodel.IsOpenShop = false;
                                        infomodel.BusinessLicence = info.organization_code;
                                        infomodel.Token = token;
                                        infomodel.TokenCode = tokencode;
                                        infomodel.isPrivate = 1;
                                        infomodel.ServiceID = long.Parse(serviceID);
                                        infomodel.LicenceEndDate = DateTime.Now.AddYears(1);
                                        dataContext.Enterprise_Info.InsertOnSubmit(infomodel);
                                        dataContext.SubmitChanges();
                                        #endregion

                                        #region Enterprise_User
                                        Enterprise_User user = new Enterprise_User();
                                        user.Enterprise_Info_ID = infomodel.Enterprise_Info_ID;
                                        user.Enterprise_Role_ID = 1;//注册企业的第一个用户必定是主账号
                                        user.UserName = "管理员";
                                        user.UserCode = "admin";
                                        user.UserType = "默认";
                                        user.Status = Convert.ToInt16(EnumFile.Status.used);
                                        user.LoginName = loginname.Trim();
                                        user.LoginPassWord = password.Trim();
                                        user.adddate = DateTime.Now;
                                        dataContext.Enterprise_User.InsertOnSubmit(user);
                                        dataContext.SubmitChanges();
                                        #endregion

                                        #region Enterprise_License
                                        Enterprise_License licenseModel = new Enterprise_License();
                                        licenseModel.EnterpriseID = infomodel.Enterprise_Info_ID;
                                        licenseModel.AdminID = user.Enterprise_User_ID;
                                        licenseModel.OperateDate = DateTime.Now;
                                        licenseModel.State = Convert.ToInt16(EnumFile.Status.used);
                                        licenseModel.LicenseEndDate = DateTime.Now.AddYears(1);
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

                                        #region 赋值
                                        brm.code = "1";
                                        brm.Msg = "登录成功";
                                        PrivateLoginResponse res = new PrivateLoginResponse();
                                        #region 赋值
                                        res.Address = model.Address;
                                        res.BusinessLicence = info.organization_code;
                                        res.Dictionary_AddressQu_ID = long.Parse(info.area_id.ToString());
                                        res.Dictionary_AddressSheng_ID = long.Parse(info.province_id.ToString());
                                        res.Dictionary_AddressShi_ID = long.Parse(info.city_id.ToString());
                                        res.Dictionary_UnitType_ID = info.unittype_id.ToString()c;
                                        res.Email = info.email;
                                        res.EnterPriseID = infomodel.Enterprise_Info_ID;
                                        res.EnterpriseName = infomodel.EnterpriseName;
                                        res.LicenseEndDate = infomodel.LicenceEndDate.ToString();
                                        res.IsExpired = 0;
                                        res.IsSimple = 2;
                                        res.LinkMan = info.linkman;
                                        res.LinkPhone = info.linkphone;
                                        res.LoginName = loginname;
                                        res.LoginPwd = password;
                                        res.MainCode = info.organunit_oid;
                                        res.Token = token;
                                        res.TokenCode = tokencode;
                                        res.Trade_ID = long.Parse(info.trade_id.ToString());
                                        res.UserID = user.Enterprise_User_ID;
                                        res.UserName = user.UserName;
                                        brm.ObjModel = res;
                                        #endregion
                                        #endregion
                                    }
                                    catch (Exception)
                                    {
                                        brm.code = "-1";
                                        brm.Msg = "程序出错，登录失败";
                                        tran.Rollback();
                                        throw;
                                    }
                                }
                            }


                        }
                    }
                }
            }
            catch (Exception)
            {
                brm.code = "-1";
                brm.Msg = "程序出错，登录失败";
                throw;
            }
            return brm;
        }
        #endregion

        public Enterprise_User GetTxtUserModel(long eid, string userName, string pwd)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User dataInfo = dataContext.Enterprise_User.FirstOrDefault(m => m.LoginName == userName &&
                        m.LoginPassWord == pwd && m.Status == (int)EnumFile.Status.used && m.Enterprise_Info_ID == eid);
                    ClearLinqModel(dataInfo);
                    return dataInfo;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据用户名称获取用户实体   
        /// 陈志钢 WinCE
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns></returns>
        public View_WinCe_EnterpriseInfoUser GetEntityByLoginName(string userName)
        {
            View_WinCe_EnterpriseInfoUser result = new View_WinCe_EnterpriseInfoUser();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.View_WinCe_EnterpriseInfoUser.FirstOrDefault(m => m.LoginName.ToLower() == userName.ToLower());
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public View_WinCe_EnterpriseInfoUser GetEntityByLoginNameEx(string userName)
        {
            View_WinCe_EnterpriseInfoUser result = new View_WinCe_EnterpriseInfoUser();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.View_WinCe_EnterpriseInfoUser.FirstOrDefault(m => m.LoginName.ToLower() == userName.ToLower());
                    if (result != null)
                    {
                        result = dataContext.View_WinCe_EnterpriseInfoUser.FirstOrDefault(m => m.Enterprise_Info_ID == result.Enterprise_Info_ID && m.UserType == "默认");
                    }
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public BaseResultModel LoginWinCe(string loginName, string pwd)
        {
            BaseResultModel userLogin = new BaseResultModel();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var model = (from d in dataContext.Enterprise_User
                                 where d.LoginName == loginName && d.LoginPassWord == pwd && d.Status == (int)EnumFile.Status.used
                                 select d).FirstOrDefault();
                    if (model != null)
                    {
                        userLogin.ObjModel = model;
                        userLogin.code = "1";
                        userLogin.Msg = "登录成功！";
                    }
                    else
                    {
                        model = dataContext.Enterprise_User.FirstOrDefault(m => m.LoginName == loginName);
                        if (model == null)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，用户名不存在！";
                        }
                        else if (model != null && model.LoginPassWord != pwd)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，密码错误！";
                        }
                        else
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，出现异常！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                userLogin.code = "0";
                userLogin.Msg = "登录失败，出现异常！";
            }
            return userLogin;
        }

        #region 根据企业ID获取默认企业用户信息

        /// <summary>
        /// 根据企业ID获取默认企业用户信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public Enterprise_User GetUserByEnterpriseId(long id)
        {
            Enterprise_User result = new Enterprise_User();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.Enterprise_User.FirstOrDefault(m => m.Enterprise_Info_ID == id && m.UserType == "默认");
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        #endregion

        #region 二维码系统登录

        /// <summary>
        /// 二维码系统登录账号校对
        /// 2019-11-05
        /// 刘晓杰
        /// </summary>
        /// <param name="enterpriseId">单位ID</param>
        /// <param name="enterpriseName">单位名称</param>
        /// <param name="mainCode">单位主码</param>
        /// <param name="accountType">账号类型（1-溯源账号，2-IDCode账号）</param>
        /// <returns>状态（1:成功,0:超期,-1:失败）</returns>
        public int GetEwmSysLoginInfo(long enterpriseId, string enterpriseName, string mainCode, int accountType)
        {
            int result;
            try
            {
                Enterprise_EwmSysLogin loginInfo = new Enterprise_EwmSysLogin();
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    loginInfo = dataContext.Enterprise_EwmSysLogin.FirstOrDefault(m =>
                        m.EnterpriseName == enterpriseName &&
                        m.MainCode == mainCode &&
                        m.AccountType == accountType);

                    // IDCode账号第一次登录，插入使用信息
                    if (null == loginInfo && accountType == 2)
                    {
                        loginInfo = new Enterprise_EwmSysLogin()
                        {
                            Enterprise_Info_ID = enterpriseId,
                            EnterpriseName = enterpriseName,
                            MainCode = mainCode,
                            ExpirateTime = DateTime.Now.AddMonths(1),
                            AccountType = accountType,
                            FirstTime = DateTime.Now
                        };
                        dataContext.Enterprise_EwmSysLogin.InsertOnSubmit(loginInfo);
                        dataContext.SubmitChanges();
                    }

                    // 溯源平台用户第一次登录，更新FirstTime字段
                    if (null != loginInfo && null == loginInfo.FirstTime && accountType == 1)
                    {
                        loginInfo.FirstTime = DateTime.Now;
                        dataContext.SubmitChanges();
                    }
                }

                // 失败:账号不存在
                if (null == loginInfo)
                    result = -1;
                else
                    result = 1;

                // IDCode账号是否超期
                if (loginInfo.AccountType == 2 && DateTime.Now.CompareTo(loginInfo.ExpirateTime) > 0)
                    result = 0;
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        #endregion

        public RetResult GetCompanyMainCode(string conStr)
        {
            string Msg = "";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext(conStr))
                {
                    var query = from f in dataContext.Enterprise_Info where f.MainCode == "" || Nullable<int>.Equals(f.MainCode, null) select f;

                    List<Enterprise_Info> eList = query.ToList();
                    foreach (Enterprise_Info sub in eList)
                    {
                        Enterprise_User user = dataContext.Enterprise_User.Where(p => p.Enterprise_Info_ID == sub.Enterprise_Info_ID).FirstOrDefault();
                        if (user == null)
                        {
                            Msg = sub.EnterpriseName + "没有找到相应的用户登录信息！";
                            error = CmdResultError.NONE;
                            Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, Msg);
                            continue;
                            //return Ret;
                        }
                        else if (user.UserType == "GS1")
                        {
                            Msg = sub.EnterpriseName + "是GS1企业！";
                            error = CmdResultError.NONE;
                            Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, Msg);
                            continue;
                            //return Ret;
                        }
                        HisOrganUnit organUnit = BaseDataDAL.GetCompanyMainCode(user.LoginName, user.LoginPassWord);
                        Msg = sub.EnterpriseName + ":" + user.LoginName + ";" + user.LoginPassWord + ";" + organUnit.result_code + "&" + organUnit.organunit_oid + "&" + organUnit.author_key + "&" + organUnit.author_code;
                        if (organUnit != null && organUnit.result_code == 1 && !string.IsNullOrEmpty(organUnit.organunit_oid))
                        {
                            if (!string.IsNullOrEmpty(organUnit.author_code) && !string.IsNullOrEmpty(organUnit.author_key))
                            {
                                EnterpriseShopLink enEx = new Dal.ScanCodeDAL().ShopEn(sub.Enterprise_Info_ID);
                                if (enEx != null)
                                {
                                    enEx.access_token = organUnit.author_key;
                                    enEx.access_token_code = organUnit.author_code;
                                }
                                else
                                {
                                    enEx = new EnterpriseShopLink();
                                    enEx.access_token = organUnit.author_key;
                                    enEx.access_token_code = organUnit.author_code;
                                    enEx.EnterpriseID = sub.Enterprise_Info_ID;
                                    enEx.AddDate = DateTime.Now;
                                    dataContext.EnterpriseShopLink.InsertOnSubmit(enEx);
                                }
                            }
                            sub.MainCode = organUnit.organunit_oid;
                            dataContext.SubmitChanges();
                            error = CmdResultError.NONE;
                            Msg = Msg + " 操作成功";
                            Ret.SetArgument(Common.Argument.CmdResultError.NONE, Msg, Msg);
                        }
                        else
                        {
                            Msg = Msg + " 失败";
                            error = CmdResultError.NONE;
                            Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, Msg, Msg);
                            continue;
                        }
                    }
                }
            }
            catch
            {
                Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取子用户列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userRole"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<Enterprise_User> GetSubList(long id, int pageIndex, out long totalCount, string userName)
        {
            totalCount = 1;
            List<Enterprise_User> dataList = new List<Enterprise_User>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataList = (from data in dataContext.Enterprise_User
                                where data.Enterprise_Info_ID == id && data.Status == (int)EnumFile.Status.used && data.UserType == "子用户"
                                select data).ToList();

                    if (!string.IsNullOrEmpty(userName))
                    {
                        dataList = dataList.Where(w => w.UserName.Contains(userName.Trim())).ToList();
                    }
                    if (dataList.Count != 0)
                    {
                        totalCount = dataList.Count;
                    }
                    if (pageIndex > 0)
                    {
                        dataList = dataList.OrderByDescending(m => m.Enterprise_User_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    ClearLinqModel(dataList);

                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Material> GetMaterial(long eid, out long totalCount)
        {
            totalCount = 1;
            List<Material> dataList = new List<Material>();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {


                    dataList = (from data in dataContext.Material
                                where data.Enterprise_Info_ID == eid &&
                                data.Status == (int)EnumFile.Status.used
                                select data).ToList();
                    if (dataList.Count != 0)
                    {
                        totalCount = dataList.Count;
                    }
                    ClearLinqModel(dataList);
                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取DI列表
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<MaterialDI> GetSetDIList(long eid, long materialId, out long totalCount)
        {
            totalCount = 1;
            List<MaterialDI> dataList = new List<MaterialDI>();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {


                    dataList = (from data in dataContext.MaterialDI
                                where data.EnterpriseID == eid &&
                                data.Status == (int)EnumFile.Status.used
                                select data).ToList();
                    if (dataList.Count != 0)
                    {
                        totalCount = dataList.Count;
                    }
                    if (materialId > 0)
                    {
                        dataList = dataList.Where(p => p.MaterialID == materialId).ToList();
                    }
                    ClearLinqModel(dataList);
                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 添加子账号
        /// </summary>
        /// <param name="objEnterprise_User"></param>
        /// <param name="?"></param>
        /// <param name="diList"></param>
        /// <returns></returns>
        public RetResult AddSub(Enterprise_User objEnterprise_User, string diList)
        {
            string msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.Enterprise_User.InsertOnSubmit(objEnterprise_User);
                    dataContext.SubmitChanges();
                    string[] diStr = diList.Split(',');
                    foreach (string sub in diStr)
                    {
                        EnterprsieDI_User user = new EnterprsieDI_User();
                        user.Enterprise_User_ID = objEnterprise_User.Enterprise_User_ID;
                        user.MaterialUDIDI = sub;
                        user.AddTime = DateTime.Now;
                        user.Status = (int)Common.EnumFile.Status.used;
                        dataContext.EnterprsieDI_User.InsertOnSubmit(user);
                        dataContext.SubmitChanges();
                    }
                    msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                msg = "数据库连接失败！";
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, msg, msg);
            return Ret;
        }
    }
}
