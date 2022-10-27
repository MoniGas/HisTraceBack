/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-10-18

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于监管部门登录的控制器层

*********************************************************************************/
using System;
using System.Collections.Generic;
using Common.Argument;
using Dal;
using LinqModel;
using System.Linq;
using Common.Log;

namespace BLL
{
    /// <summary>
    /// 赵慧敏
    /// </summary>
    public class LoginBLL
    {
        /// <summary>
        /// IDcode用户农企登录
        /// </summary>
        /// <param name="lName"></param>
        /// <param name="pwd"></param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        public BaseResultModel EnterpriseLogin(string lName, string pwd)
        {
            BaseResultModel result = null;
            //该企业批次数量
            int batchCount;
            LoginDAL dal = new LoginDAL();
            PRRU_PlatFormLevelDAL levelDAL = new PRRU_PlatFormLevelDAL();
            LoginInfo loginInfo = new LoginInfo();
            BaseResultModel dataInfo = dal.LoginVerify(lName, pwd);
            if (dataInfo.code.Equals("1"))
            {
                Enterprise_User data = dataInfo.ObjModel as Enterprise_User;
                BaseResultModel model = new BaseResultModel();
                if (data.UserType.Equals("默认") || data.UserType.Equals("GS1"))
                {
                    model = dal.LoginRoleModel(lName, pwd, out batchCount);
                    View_EnterpriseLevelUser user = model.ObjModel as View_EnterpriseLevelUser;
                    if (user != null)
                    {
                        #region 为登录原型赋值
                        loginInfo.EnterpriseID = user.Enterprise_Info_ID;
                        loginInfo.PRRU_PlatFormLevel_ID = Convert.ToInt16(user.Enterprise_Level);
                        loginInfo.UserType = user.UserType;
                        loginInfo.Modual_ID_Array = levelDAL.GetModel(Convert.ToInt16(user.Enterprise_Level)).Modual_ID_Array;
                        loginInfo.RoleModual_ID_Array = user.Modual_ID_Array;
                        loginInfo.Parent_ID = (long)user.PRRU_PlatForm_ID;
                        loginInfo.ApprovalCodeType = user.ApprovalCodeType;
                        loginInfo.MainCode = user.MainCode;
                        loginInfo.UserID = user.Enterprise_User_ID;
                        loginInfo.UserName = user.UserName;
                        loginInfo.EnterpriseName = user.EnterpriseName;
                        loginInfo.NewUser = false;
                        loginInfo.Verify = (int)user.Verify;
                        loginInfo.CodeType = user.CodeType == null ? 0 : (int)user.CodeType;
                        string time = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        string week = DateTime.Today.DayOfWeek.ToString();
                        switch (week)
                        {
                            case "Monday":
                                week = "星期一";
                                break;
                            case "Tuesday":
                                week = "星期二";
                                break;
                            case "Wednesday":
                                week = "星期三";
                                break;
                            case "Thursday":
                                week = "星期四";
                                break;
                            case "Friday":
                                week = "星期五";
                                break;
                            case "Saturday":
                                week = "星期六";
                                break;
                            case "Sunday":
                                week = "星期日";
                                break;
                            default:
                                break;
                        }
                        loginInfo.Date = time + " " + week;
                        #endregion
                        result = ToJson.NewModelToJson(user, "1", "登录成功！");
                    }
                    else
                    {
                        result = ToJson.NewModelToJson(user, "0", model.Msg);
                    }
                    SessCokie.Set(loginInfo);
                }
                else if (data.UserType.Equals("注册") || data.UserType.Equals("经销商"))
                {
                    model = dal.EnterpriseLogin(lName, pwd, out batchCount);
                    View_EnterpriseInfoUser user = model.ObjModel as View_EnterpriseInfoUser;
                    if (user != null)
                    {
                        #region 为登录原型赋值
                        loginInfo.EnterpriseID = user.Enterprise_Info_ID;
                        loginInfo.PRRU_PlatFormLevel_ID = Convert.ToInt16(user.Enterprise_Level);
                        loginInfo.UserType = user.UserType;
                        loginInfo.Modual_ID_Array = levelDAL.GetModel(Convert.ToInt16(user.Enterprise_Level)).Modual_ID_Array;
                        loginInfo.RoleModual_ID_Array = user.RoleModual_ID_Array;
                        loginInfo.Parent_ID = (long)user.PRRU_PlatForm_ID;
                        loginInfo.ApprovalCodeType = user.ApprovalCodeType;
                        loginInfo.MainCode = user.MainCode;
                        loginInfo.UserID = user.Enterprise_User_ID;
                        loginInfo.UserName = user.UserName;
                        loginInfo.EnterpriseName = user.EnterpriseName;
                        loginInfo.NewUser = false;
                        loginInfo.Verify = (int)user.Verify;
                        loginInfo.CodeType = user.CodeType == null ? 0 : (int)user.CodeType;
                        // loginInfo.NewUser = batchCount > 0 ? false : true;
                        string time = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        string week = DateTime.Today.DayOfWeek.ToString();
                        switch (week)
                        {
                            case "Monday":
                                week = "星期一";
                                break;
                            case "Tuesday":
                                week = "星期二";
                                break;
                            case "Wednesday":
                                week = "星期三";
                                break;
                            case "Thursday":
                                week = "星期四";
                                break;
                            case "Friday":
                                week = "星期五";
                                break;
                            case "Saturday":
                                week = "星期六";
                                break;
                            case "Sunday":
                                week = "星期日";
                                break;
                            default:
                                break;
                        }
                        loginInfo.Date = time + " " + week;
                        #endregion
                        result = ToJson.NewModelToJson(user, "1", "登录成功！");
                    }
                    else
                    {
                        result = ToJson.NewModelToJson(user, "0", model.Msg);
                    }
                    SessCokie.Set(loginInfo);
                }
            }
            else
            {
                result = ToJson.NewModelToJson(dataInfo, "0", dataInfo.Msg);
            }
            return result;
        }

        /// <summary>
        /// 本平台用户登录，包含监管部门
        /// </summary>
        /// <returns></returns>
        public string PlatUserLogin(string lName, string pwd)
        {
            string result = null;
            LoginDAL dal = new LoginDAL();
            PRRU_PlatFormLevelDAL levelDAL = new PRRU_PlatFormLevelDAL();
            LoginInfo loginInfo = new LoginInfo();
            int batchCount;
            object model = dal.EnterpriseLogin(lName, pwd, out batchCount);
            View_PRRU_PlatFormUser user = model as View_PRRU_PlatFormUser;
            #region 获取登录数据
            loginInfo.EnterpriseID = user.PRRU_PlatForm_ID;
            loginInfo.RoleModual_ID_Array = user.Modual_ID_Array;
            loginInfo.PRRU_PlatFormLevel_ID = Convert.ToInt16(user.PRRU_PlatFormLevel_ID);
            //loginInfo.UserRoleID = Convert.ToInt16(user.PRRU_PlatForm_Role_ID);
            loginInfo.UserType = user.UserType;
            loginInfo.Modual_ID_Array = levelDAL.GetModel(Convert.ToInt16(user.PRRU_PlatFormLevel_ID)).Modual_ID_Array;
            loginInfo.Parent_ID = (long)user.PRRU_PlatForm_ID;
            //loginInfo.ApprovalCodeType = user.ApprovalCodeType;// 赵慧敏
            //loginInfo.MainCode = user.MainCode;
            loginInfo.UserID = user.PRRU_PlatForm_User_ID;
            loginInfo.UserName = user.UserName;
            loginInfo.EnterpriseName = user.CompanyName;
            //loginInfo.RoleName = user.RoleName;
            #endregion
            return result;
        }
        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns></returns>
        public BaseResultModel GetLoginInfo()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = null;
            try
            {
                result = ToJson.NewModelToJson(pf, "1", "获取成功！");
            }
            catch (Exception ex)
            {
                result = ToJson.NewModelToJson(pf, "-1", "获取失败！");
            }

            return result;
        }

        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="moduleArray"></param>
        /// <returns></returns>
        public BaseResultList GetModuleList(int parentId, string moduleArray, long eId)
        {
            BaseResultList result = null;
            try
            {
                LoginDAL loginDAL = new LoginDAL();
                List<PRRU_NewModual> moduleList = loginDAL.GetModuleList(parentId);
                result = ToJson.NewListToJson(moduleList, 1, moduleList.Count, moduleList.Count, "获取成功！");

                List<PRRU_NewModual> resultModel = moduleList.Where(m => m.PRRU_Modual_ID == 10000 || moduleArray.Contains(m.PRRU_Modual_ID.ToString())).ToList();

                List<PRRU_NewModual> mymodule = moduleList.Where(m => m.PRRU_Modual_ID == 10000 || moduleArray.Contains(m.PRRU_Modual_ID.ToString())).ToList();
                foreach (var item in mymodule)
                {
                    PRRU_NewModual sub = moduleList.FirstOrDefault(m => m.PRRU_Modual_ID == item.Parent_ID);
                    if (sub == null) continue;
                    if (mymodule.FirstOrDefault(m => m.PRRU_Modual_ID == sub.PRRU_Modual_ID) == null)
                    {
                        resultModel.Add(sub);
                    }
                }
                #region 查找企业是否开通子企业功能 2021-11-08
                Enterprise_SetMoule setmoule = loginDAL.GetSetMoule(eId);
                if (setmoule != null)
                {
                    PRRU_NewModual pModual = loginDAL.GetSubModual();
                    pModual.IsDisplay = 1;
                    resultModel.Add(pModual);
                }
                #endregion
                foreach (var item in resultModel)
                {
                    if (item.Parent_ID == 0)
                    {
                        PRRU_NewModual first = resultModel.OrderBy(m => m.SortOrder).FirstOrDefault(m => m.Parent_ID == item.PRRU_Modual_ID);
                        if (first == null) continue;
                        if (resultModel.FirstOrDefault(m => m.hash == item.hash && m.PRRU_Modual_ID != item.PRRU_Modual_ID) != null) continue;
                        if (item.route == "home") continue;
                        if (!string.IsNullOrEmpty(first.hash))
                        {
                            item.hash = first.hash;
                            item.route = first.route;
                            item.url = first.url;
                        }
                        else
                        {
                            first = resultModel.OrderBy(m => m.SortOrder).FirstOrDefault(m => m.Parent_ID == first.PRRU_Modual_ID);
                            if (first == null) continue;
                            if (!string.IsNullOrEmpty(first.hash))
                            {
                                item.hash = first.hash;
                                item.route = first.route;
                                item.url = first.url;
                            }
                        }
                    }
                }

                result = ToJson.NewListToJson(resultModel, 1, resultModel.Count, resultModel.Count, "获取成功！");
            }
            catch (Exception ex)
            {
                List<View_Module> mList = new List<View_Module>();
                result = ToJson.NewListToJson(mList, 0, 0, 0, "获取失败！");
            }
            return result;
        }

        public List<SYS_NewModual> GetSysNewModuleList(int parentId, string moduleArray)
        {
            List<SYS_NewModual> result = null;
            try
            {
                LoginDAL loginDAL = new LoginDAL();
                List<SYS_NewModual> moduleList = loginDAL.GetSysNewModuleList(parentId);
                result = moduleList.Where(a => moduleArray.Contains(a.PRRU_Modual_ID.ToString())).ToList();
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public BaseResultList GetSysModuleList(int parentId)
        {
            BaseResultList result = null;
            try
            {
                LoginDAL loginDAL = new LoginDAL();
                List<SYS_Modual> moduleList = loginDAL.GetSysModuleList(parentId);
                result = ToJson.NewListToJson(moduleList, 1, moduleList.Count, moduleList.Count, "获取成功！");
            }
            catch (Exception ex)
            {
                List<SYS_Modual> mList = new List<SYS_Modual>();
                result = ToJson.NewListToJson(mList, 0, 0, 0, "获取失败！");
            }

            return result;
        }

        public BaseResultModel SysLogin(string lName, string pwd)
        {
            LoginDAL loginDAL = new LoginDAL();
            PRRU_PlatFormLevelDAL levelDAL = new PRRU_PlatFormLevelDAL();
            View_PRRU_PlatFormUser data = loginDAL.SysLogin(lName, pwd);
            LoginInfo loginInfo = new LoginInfo();
            BaseResultModel userLogin = new BaseResultModel();
            if (data != null)
            {
                #region 为登录原型赋值
                loginInfo.EnterpriseID = data.PRRU_PlatForm_ID;
                loginInfo.RoleModual_ID_Array = data.Modual_ID_Array;
                loginInfo.PRRU_PlatFormLevel_ID = Convert.ToInt16(data.PRRU_PlatFormLevel_ID);
                loginInfo.UserType = data.UserType;
                loginInfo.Modual_ID_Array = levelDAL.GetModel(Convert.ToInt16(data.PRRU_PlatFormLevel_ID)).Modual_ID_Array;
                loginInfo.Parent_ID = (long)data.PRRU_PlatForm_ID;
                //loginInfo.ApprovalCodeType = user.ApprovalCodeType;// 赵慧敏
                //loginInfo.MainCode = user.MainCode;
                loginInfo.UserID = data.PRRU_PlatForm_User_ID;
                //loginInfo.UserName = data.UserName;
                loginInfo.UserName = lName;
                loginInfo.EnterpriseName = data.CompanyName;
                loginInfo.shengId = data.Dictionary_AddressSheng_ID.Value;
                loginInfo.shiId = data.Dictionary_AddressShi_ID.Value;
                string time = DateTime.Now.Date.ToString("yyyy-MM-dd");
                string week = DateTime.Today.DayOfWeek.ToString();
                switch (week)
                {
                    case "Monday":
                        week = "星期一";
                        break;
                    case "Tuesday":
                        week = "星期二";
                        break;
                    case "Wednesday":
                        week = "星期三";
                        break;
                    case "Thursday":
                        week = "星期四";
                        break;
                    case "Friday":
                        week = "星期五";
                        break;
                    case "Saturday":
                        week = "星期六";
                        break;
                    case "Sunday":
                        week = "星期日";
                        break;
                    default:
                        break;
                }
                loginInfo.Date = time + " " + week;
                SessCokie.SetMan(loginInfo);
                #endregion

                userLogin.ObjModel = data;

                switch (data.UserStatus)
                {
                    case -2:
                        userLogin.code = "-2";
                        userLogin.Msg = "对不起该帐号已经被禁用！";
                        break;
                    case -1:
                        userLogin.code = "-1";
                        userLogin.Msg = "对不起该帐号没有审核通过！";
                        break;
                    case 0:
                        userLogin.code = "0";
                        userLogin.Msg = "该帐号正在等待审核，暂时不能登录！";
                        break;
                    case 1:
                        userLogin.code = "1";
                        userLogin.Msg = "登录成功！";
                        break;
                }
            }
            else
            {
                userLogin.code = "-3";
                userLogin.Msg = "帐号或密码错误！";
            }

            return userLogin;
        }

        public int SetInfo(string enterprise)
        {
            LoginDAL dal = new LoginDAL();
            int number = dal.SetSeting(enterprise);
            return number;
        }

        public RetResult SetHome()
        {
            LoginDAL dal = new LoginDAL();
            RetResult ret = dal.SetHome();
            return ret;
        }

        #region 营销管理平台获取top功能菜单
        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public List<PRRU_NewModual> GetModuleListYX(int parentID, string moduleArray)
        {
            List<PRRU_NewModual> moduleList = new List<PRRU_NewModual>();
            try
            {
                LoginDAL loginDAL = new LoginDAL();
                moduleList = loginDAL.GetModuleList(parentID);
                //result = ToJson.NewListToJson(moduleList, 1, moduleList.Count, moduleList.Count, "获取成功！");

                List<PRRU_NewModual> resultModel = moduleList.Where(m => m.PRRU_Modual_ID == 10000 || moduleArray.Contains(m.PRRU_Modual_ID.ToString())).ToList();

                List<PRRU_NewModual> mymodule = moduleList.Where(m => m.PRRU_Modual_ID == 10000 || moduleArray.Contains(m.PRRU_Modual_ID.ToString())).ToList();
                foreach (var item in mymodule)
                {
                    PRRU_NewModual sub = moduleList.FirstOrDefault(m => m.PRRU_Modual_ID == item.Parent_ID);
                    if (sub == null) continue;
                    if (mymodule.FirstOrDefault(m => m.PRRU_Modual_ID == sub.PRRU_Modual_ID) == null)
                    {
                        resultModel.Add(sub);
                    }
                }

                foreach (var item in resultModel)
                {
                    if (item.Parent_ID == 0)
                    {
                        PRRU_NewModual first = resultModel.OrderBy(m => m.SortOrder).FirstOrDefault(m => m.Parent_ID == item.PRRU_Modual_ID);
                        if (first == null) continue;
                        if (resultModel.FirstOrDefault(m => m.hash == item.hash && m.PRRU_Modual_ID != item.PRRU_Modual_ID) != null) continue;
                        if (item.route == "home") continue;
                        if (!string.IsNullOrEmpty(first.hash))
                        {
                            item.hash = first.hash;
                            item.route = first.route;
                            item.url = first.url;
                        }
                        else
                        {
                            first = resultModel.OrderBy(m => m.SortOrder).FirstOrDefault(m => m.Parent_ID == first.PRRU_Modual_ID);
                            if (first == null) continue;
                            if (!string.IsNullOrEmpty(first.hash))
                            {
                                item.hash = first.hash;
                                item.route = first.route;
                                item.url = first.url;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return moduleList;
        }

        /// <summary>
        /// 获取监管后台权限
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public List<SYS_NewModual> GetSysModuleListMan(int parentID)
        {
            List<SYS_NewModual> result = new List<SYS_NewModual>();
            try
            {
                LoginDAL loginDAL = new LoginDAL();
                List<SYS_NewModual> moduleList = loginDAL.GetSysModuleListMan(parentID);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        #endregion

        public FWZSNCP_CodeVersion GetVersion(int type)
        {
            FWZSNCP_CodeVersion result = new FWZSNCP_CodeVersion();
            try
            {
                LoginDAL dal = new LoginDAL();

                result = dal.GetVersion(type);
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("GetVersion" + ex.Message);
            }
            return result;
        }
    }
}
