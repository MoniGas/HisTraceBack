using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using System.Configuration;
using Common.Argument;
using Common;

namespace Dal
{
    /// <summary>
    /// 赵慧敏
    /// </summary>
    public class LoginDAL : DALBase
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="lName">登录名</param>
        /// <param name="pwd">登录密码</param>
        /// <param name="loginType">登录类型：1农企2平台内用户</param>
        /// <returns></returns>
        public BaseResultModel EnterpriseLogin(string lName, string pwd, out int batchCount)
        {
            BaseResultModel userLogin = new BaseResultModel();
            batchCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    View_EnterpriseInfoUser model = dataContext.View_EnterpriseInfoUser.FirstOrDefault(m => m.LoginName == lName && m.LoginPassWord.Equals(pwd) && m.UserStatus == (int)EnumFile.Status.used);
                    if (model != null)
                    {
                        batchCount = dataContext.Batch.Count(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID);
                        #region 添加操作日志
                        Log_Info log_Info = new Log_Info();
                        log_Info.afterData = null;
                        log_Info.edittype = "登录";
                        log_Info.editUserID = model.Enterprise_User_ID;
                        log_Info.editUserName = model.UserName;
                        log_Info.platId = model.Enterprise_Info_ID;
                        log_Info.platLevel = (int)model.Enterprise_Level;
                        log_Info.execDate = DateTime.Now;
                        log_Info.tableName = model.ToString();
                        log_Info.platName = model.EnterpriseName;
                        dataContext.Log_Info.InsertOnSubmit(log_Info);
                        dataContext.SubmitChanges();
                        #endregion
                        userLogin.ObjModel = model;
                        userLogin.code = "1";
                        userLogin.Msg = "登录成功！";
                    }
                    else
                    {
                        model = dataContext.View_EnterpriseInfoUser.FirstOrDefault(m => m.LoginName == lName);
                        if (model == null)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，用户名错误！";
                        }
                        else
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，密码错误！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                userLogin.code = "0";
                userLogin.Msg = "登录失败，出现异常！";
                string errData = "LoginDAL.EnterpriseLogin():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return userLogin;
        }

        public BaseResultModel LoginRoleModel(string lName, string pwd, out int batchCount)
        {
            BaseResultModel userLogin = new BaseResultModel();
            batchCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var model = (from d in dataContext.View_EnterpriseLevelUser
                                 where d.LoginName == lName && d.LoginPassWord == pwd
                                 select d).FirstOrDefault();
                    if (model != null)
                    {
                        batchCount = dataContext.Batch.Count(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID);
                        #region 添加操作日志
                        Log_Info log_Info = new Log_Info();
                        log_Info.afterData = null;
                        log_Info.edittype = "登录";
                        log_Info.editUserID = model.Enterprise_User_ID;
                        log_Info.editUserName = model.UserName;
                        log_Info.platId = model.Enterprise_Info_ID;
                        log_Info.platLevel = (int)model.Enterprise_Level;
                        log_Info.execDate = DateTime.Now;
                        log_Info.tableName = model.ToString();
                        log_Info.platName = model.EnterpriseName;
                        dataContext.Log_Info.InsertOnSubmit(log_Info);
                        dataContext.SubmitChanges();
                        #endregion
                        userLogin.ObjModel = model;
                        userLogin.code = "1";
                        userLogin.Msg = "登录成功！";
                    }
                    else
                    {
                        model = dataContext.View_EnterpriseLevelUser.FirstOrDefault(m => m.LoginName == lName);
                        if (model == null)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，用户名错误！";
                        }
                        else
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，密码错误！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                userLogin.code = "0";
                userLogin.Msg = "登录失败，出现异常！";
                string errData = "LoginDAL.EnterpriseLogin():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return userLogin;
        }

        public BaseResultModel LoginVerify(string lName, string pwd)
        {
            BaseResultModel userLogin = new BaseResultModel();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.Enterprise_User
                                where d.LoginName == lName && d.LoginPassWord == pwd
                                select d).FirstOrDefault(); 
                    if (data != null)
                    {
                        Enterprise_Info myEnterprise = new EnterpriseInfoDAL().GetModel(data.Enterprise_Info_ID.Value);
                        if ((string.IsNullOrEmpty(myEnterprise.MainCode) || myEnterprise==null) && data.UserType!="GS1")
                        {
                            WriteLog.WriteErrorLog(data.UserType);
                            userLogin.code = "0";
                            userLogin.Msg = "无法登录，请购买授权！";
                        }
                        else
                        {
                            if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.noVerify || myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.Try)
                            {
                                int tryDays = Convert.ToInt32(ConfigurationManager.AppSettings["trydays"]);
                                if (Convert.ToDateTime(myEnterprise.Stradddate).AddDays(tryDays) < DateTime.Now)
                                {
                                    userLogin.code = "0";
                                    userLogin.Msg = "您的企业试用期限已过，请联系监管部门！";
                                }
                                else
                                {
                                    userLogin.ObjModel = data;
                                    userLogin.code = "1";
                                    userLogin.Msg = "登录成功！";
                                }
                            }
                            else if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.noPassVerify)
                            {
                                userLogin.code = "0";
                                userLogin.Msg = "您的企业未通过审核，请联系监管部门！";
                            }
                            else if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.passVerify)
                            {
                                userLogin.ObjModel = data;
                                userLogin.code = "1";
                                userLogin.Msg = "登录成功！";
                            }
                            else if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.pauseVerify)
                            {
                                userLogin.code = "0";
                                userLogin.Msg = "您的企业已被暂停使用，请联系监管部门！";
                            }
                            else if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.gs1 && myEnterprise.Status == (int)Common.EnumFile.Status.used)
                            {
                                userLogin.ObjModel = data;
                                userLogin.code = "1";
                                userLogin.Msg = "登录成功！";
                            }
                            else
                            {
                                userLogin.code = "0";
                                userLogin.Msg = "登录失败！";
                            }
                        }
                    }
                    else
                    {
                        data = dataContext.Enterprise_User.FirstOrDefault(m => m.LoginName == lName);
                        if (data == null)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，用户名错误！";
                        }
                        else
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，密码错误！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                userLogin.code = "0";
                userLogin.Msg = "登录失败，出现异常！";
                string errData = "LoginDAL.EnterpriseLogin():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return userLogin;
        }

        public BaseResultModel LoginVerifyEx(string lName, string pwd)
        {
            BaseResultModel userLogin = new BaseResultModel();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.Enterprise_User
                                where d.LoginName == lName && d.LoginPassWord == pwd
                                select d).FirstOrDefault();
                    if (data != null && data.UserType == "注册")
                    {
                        Enterprise_Info myEnterprise = new EnterpriseInfoDAL().GetModel(data.Enterprise_Info_ID.Value);
                        if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.noVerify || myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.Try)
                        {
                            int tryDays = Convert.ToInt32(ConfigurationManager.AppSettings["trydays"]);
                            if (Convert.ToDateTime(myEnterprise.Stradddate).AddDays(tryDays) < DateTime.Now)
                            {
                                userLogin.code = "0";
                                userLogin.Msg = "您的企业试用期限已过，请联系监管部门！";
                            }
                            else
                            {
                                Enterprise_User tempUser = dataContext.Enterprise_User.FirstOrDefault(m => m.Enterprise_Info_ID == myEnterprise.Enterprise_Info_ID && m.UserType == "默认");
                                if (tempUser != null)
                                {
                                    data.LoginName = tempUser.LoginName;
                                    data.LoginPassWord = tempUser.LoginPassWord;
                                    userLogin.ObjModel = data;
                                    userLogin.code = "1";
                                    userLogin.Msg = "登录成功！";
                                }
                                else
                                {
                                    userLogin.code = "0";
                                    userLogin.Msg = "未找到您企业的默认用户！";
                                }
                            }
                        }
                        else if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.noPassVerify)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "您的企业未通过审核，请联系监管部门！";
                        }
                        else if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.passVerify)
                        {
                            userLogin.ObjModel = data;
                            userLogin.code = "1";
                            userLogin.Msg = "登录成功！";
                        }
                        else if (myEnterprise.Verify == (int)EnumFile.EnterpriseVerify.pauseVerify)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "您的企业已被暂停使用，请联系监管部门！";
                        }
                        else
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败！";
                        }
                    }
                    else
                    {
                        data = dataContext.Enterprise_User.FirstOrDefault(m => m.LoginName == lName);
                        if (data == null)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，用户名错误！";
                        }
                        else
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，密码错误！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                userLogin.code = "0";
                userLogin.Msg = "登录失败，出现异常！";
                string errData = "LoginDAL.EnterpriseLogin():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return userLogin;
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
        /// 获取
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
        /// 获取模块列表
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public List<PRRU_NewModual> GetModuleList(int parentID)
        {
            List<PRRU_NewModual> result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var query = from p in dataContext.PRRU_NewModual select p;
                    if (parentID != -1) //-1为全部
                    {
                        query = query.Where(p => p.Parent_ID == parentID);
                    }
                    query = query.Where(p => p.IsDisplay == 1 && p.PlatModual == 1);
                    result = query.OrderBy(m => m.SortOrder).ToList();
                    //ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "LoginDAL.GetModuleList():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }

        public List<SYS_Modual> GetSysModuleList(int parentID)
        {
            List<SYS_Modual> result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var query = from p in dataContext.SYS_Modual select p;
                    if (parentID != -1) //-1为全部
                    {
                        query = query.Where(p => p.Parent_ID == parentID);
                    }
                    query = query.Where(p => p.IsDisplay != 0);
                    result = query.OrderByDescending(m => m.SortOrder).ToList();
                    //ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "LoginDAL.GetModuleList():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }

        public List<SYS_NewModual> GetSysNewModuleList(int parentID)
        {
            List<SYS_NewModual> result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var query = from p in dataContext.SYS_NewModual select p;
                    if (parentID != -1) //-1为全部
                    {
                        query = query.Where(p => p.Parent_ID == parentID);
                    }
                    query = query.Where(p => p.IsDisplay != 0);
                    result = query.OrderBy(m => m.SortOrder).ToList();
                    //ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "LoginDAL.GetModuleList():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 监管部门登录
        /// </summary>
        /// <param name="lName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public View_PRRU_PlatFormUser SysLogin(string lName, string pwd)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    View_PRRU_PlatFormUser data = (from d in dataContext.View_PRRU_PlatFormUser
                                                   where d.LoginName == lName && d.LoginPassWord == pwd
                                                   select d).FirstOrDefault();
                    if (data != null)
                    {
                        #region 添加操作日志
                        Log_Info log_Info = new Log_Info();
                        log_Info.afterData = null;
                        log_Info.edittype = "登录";
                        log_Info.editUserID = data.PRRU_PlatForm_User_ID;
                        log_Info.editUserName = lName;
                        log_Info.platId = data.PRRU_PlatForm_ID;
                        log_Info.platLevel = 1;
                        log_Info.execDate = DateTime.Now;
                        log_Info.tableName = data.ToString();
                        log_Info.platName = data.LevelName;
                        dataContext.Log_Info.InsertOnSubmit(log_Info);
                        dataContext.SubmitChanges();
                        #endregion
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                string errData = "LoginDAL.SysLogin():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);

                return null;
            }
        }

        public int SetSeting(string enterpriseID)
        {
            int number = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    List<RequestCode> list = dataContext.RequestCode.Where(p => p.Enterprise_Info_ID == Convert.ToInt64(enterpriseID)).ToList();
                    if (list.Count > 0)
                    {
                        foreach (var sub in list)
                        {
                            RequestCodeSetting setModel = new RequestCodeSetting();
                            setModel = dataContext.RequestCodeSetting.FirstOrDefault(p => p.RequestID == sub.RequestCode_ID);
                            if (setModel == null)
                            {
                                setModel = new RequestCodeSetting();
                                setModel.beginCode = 0;
                                setModel.endCode = sub.TotalNum;
                                setModel.RequestID = sub.RequestCode_ID;
                                setModel.EnterpriseId = sub.Enterprise_Info_ID;
                                setModel.DisplayOption = "0,1,2,3,4,5,6,7,8,9,";
                                setModel.StyleModel = 0;
                                setModel.SetDate = sub.RequestDate;
                                setModel.BatchType = 1;
                                setModel.Count = (long)sub.TotalNum;
                                setModel.MaterialID = sub.Material_ID;
                                int index = dataContext.RequestCodeSetting.Where(m => m.EnterpriseId == setModel.EnterpriseId).Count() + 1;
                                string batchName = DateTime.Now.ToString("yyyy") + index;
                                while (dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == setModel.EnterpriseId && m.BatchName == batchName) != null)
                                {
                                    setModel.BatchName = "yyyy" + (index++);
                                }
                                dataContext.RequestCodeSetting.InsertOnSubmit(setModel);
                                dataContext.SubmitChanges();
                            }
                        }
                        number = list.Count();
                    }
                }
            }
            catch
            { }
            return number;
        }

        public RetResult SetHome()
        {
            RetResult ret = new RetResult();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    List<HomeDataStatis> addModels = new List<HomeDataStatis>();
                    var liEnterprise = dataContext.Enterprise_Info;
                    foreach (var item in liEnterprise)
                    {
                        HomeDataStatis sub = new HomeDataStatis();
                        sub.EnterpriseID = item.Enterprise_Info_ID;

                        long index = 0;
                        long time = 0;
                        try
                        {
                            index = dataContext.RequestCode.Where(m => m.Enterprise_Info_ID == sub.EnterpriseID).Sum(m => m.TotalNum).GetValueOrDefault(0);
                            time = dataContext.RequestCode.Count(m => m.Enterprise_Info_ID == sub.EnterpriseID);
                        }
                        catch { index = 0; time = 0; }
                        sub.RequestCodeCount = index;
                        sub.RequestCodeTimes = time;

                        time = 0;
                        try
                        {

                        }
                        catch { }
                        sub.ScanCodeTimes = time;

                        try
                        {
                            index = dataContext.Brand.Count(m => m.Enterprise_Info_ID == sub.EnterpriseID && m.Status == (int)EnumFile.Status.used);
                        }
                        catch { index = 0; }
                        sub.BrancCount = index;

                        try
                        {
                            index = dataContext.Material.Count(m => m.Enterprise_Info_ID == sub.EnterpriseID && m.Status == (int)EnumFile.Status.used);
                        }
                        catch { index = 0; }
                        sub.MaterialCount = index;

                        try
                        {
                            index = dataContext.Origin.Count(m => m.Enterprise_Info_ID == sub.EnterpriseID && m.Status == (int)EnumFile.Status.used);
                        }
                        catch { index = 0; }
                        sub.OriginCount = index;

                        sub.OriginCount = 0;

                        addModels.Add(sub);
                    }
                    dataContext.HomeDataStatis.InsertAllOnSubmit(addModels);
                }
            }
            catch { }
            return ret;
        }

        #region 营销监管后台
        public List<SYS_NewModual> GetSysModuleListMan(int parentID)
        {
            List<SYS_NewModual> result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var query = from p in dataContext.SYS_NewModual select p;
                    if (parentID != -1) //-1为全部
                    {
                        query = query.Where(p => p.Parent_ID == parentID);
                    }
                    query = query.Where(p => p.IsDisplay != 0);
                    result = query.OrderByDescending(m => m.SortOrder).ToList();
                    //ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "LoginDAL.GetSysModuleListMan():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        #endregion

        #region
        public FWZSNCP_CodeVersion GetVersion(int type)
        {
            FWZSNCP_CodeVersion model = new FWZSNCP_CodeVersion();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                model = dataContext.FWZSNCP_CodeVersion.Where(m => m.Type == type).OrderByDescending(m => m.VersionID).FirstOrDefault();
            }
            return model;
        }
        #endregion

        public Enterprise_SetMoule GetSetMoule(long eId)
        {
            Enterprise_SetMoule model =null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                model = dataContext.Enterprise_SetMoule.Where(p => p.Entetprise_Info_ID == eId && p.SetSub == (int)Common.EnumFile.ShopVerify.Open).FirstOrDefault();
            }
            return model;
        }
        public PRRU_NewModual GetSubModual()
        {
            PRRU_NewModual model = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                model = dataContext.PRRU_NewModual.Where(p => p.PRRU_Modual_ID == 81500).FirstOrDefault();
            }
            return model;
        }
    }
}
