/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-8
** 联系方式:13313318725
** 代码功能：用于系统登录
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Argument;
using System.Configuration;

namespace Dal
{
    /// <summary>
    /// 用于系统登录
    /// </summary>
    public class LoginMarketDAL : DALBase
    {
        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="uName">用户名</param>
        /// <param name="uPwd">密码</param>
        /// <returns></returns>
        public RetResult EnterpriseLogin(string uName, string uPwd)
        {
            RetResult result = new RetResult { CmdError = CmdResultError.EXCEPTION };
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var model = dataContext.View_EnterpriseInfoUser.FirstOrDefault(m => m.LoginName == uName.Trim());
                    if (model == null)
                    {
                        result.Msg = "登录失败，不存在该用户！";
                    }
                    else
                    {
                        if (model.LoginPassWord != uPwd)
                        {
                            result.Msg = "登录失败，密码错误！";
                        }
                        else
                        {
                            UserInfo loginInfo = new UserInfo();
                            #region 为登录原型赋值
                            loginInfo.EnterpriseID = model.Enterprise_Info_ID;
                            loginInfo.UserType = model.UserType;
                            if (model.Enterprise_Role_ID != null)
                            {
                                loginInfo.Role_ID = model.Enterprise_Role_ID.Value;
                            }
                            loginInfo.UserID = model.Enterprise_User_ID;
                            loginInfo.UserName = model.LoginName;
                            loginInfo.EnterpriseName = model.EnterpriseName;
                            loginInfo.MainCode = model.MainCode;
                            loginInfo.LoginType = (int)Common.EnumText.LoginType.common;
                            #endregion
                            CurrentUser.User = loginInfo;
                            result.Msg = "登录成功！";
                            result.CmdError = CmdResultError.NONE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Msg = "数据库连接失败，请联系管理员！";
                result.CmdError = CmdResultError.EXCEPTION;
            }
            return result;
        }

        /// <summary>
        /// 获取最新进行中的活动
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public YX_ActivitySub GetAcSub(long eid)
        {
            YX_ActivitySub model = new YX_ActivitySub();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                string sql = "select top 1 * from YX_ActivitySub where CompanyID=" + eid + " order by ActivityID desc";
                model = dataContext.ExecuteQuery<YX_ActivitySub>(sql).FirstOrDefault();
            }
            return model;
        }

        /// <summary>
        /// 获取今日扫码用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_Redactivity_ScanRecord> GetScanRecord(long eid)
        {
            List<YX_Redactivity_ScanRecord> model = new List<YX_Redactivity_ScanRecord>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                string scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string scanDateL = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                string sql = "select ScanRecordID, WeiXinUserID ,YX_Redactivity_ScanRecord.ActivityID  from YX_Redactivity_ScanRecord inner join  YX_ActivitySub on YX_Redactivity_ScanRecord.ActivityID=YX_ActivitySub.ActivityID where (YX_ActivitySub.CompanyID=" + eid + " and YX_Redactivity_ScanRecord.ScanDate<'" + scanDate + "'and YX_Redactivity_ScanRecord.ScanDate>'" + scanDateL + "')";
                //string sql = "select ScanRecordID, WeiXinUserID  from YX_Redactivity_ScanRecord where ScanDate='" + scanDate + "' group by ScanRecordID, WeiXinUserID";
                model = dataContext.ExecuteQuery<YX_Redactivity_ScanRecord>(sql).ToList();
            }
            return model;
        }

        /// <summary>
        /// 获取昨日扫码用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_Redactivity_ScanRecord> GetScanRecordZ(long eid)
        {
            List<YX_Redactivity_ScanRecord> model = new List<YX_Redactivity_ScanRecord>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                string scanDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                string scanDateL = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                string sql = "select ScanRecordID, WeiXinUserID ,YX_Redactivity_ScanRecord.ActivityID  from YX_Redactivity_ScanRecord inner join  YX_ActivitySub on YX_Redactivity_ScanRecord.ActivityID=YX_ActivitySub.ActivityID where (YX_ActivitySub.CompanyID=" + eid + " and YX_Redactivity_ScanRecord.ScanDate<'" + scanDate + "'and YX_Redactivity_ScanRecord.ScanDate>'" + scanDateL + "')";
                //string sql = "select ScanRecordID, WeiXinUserID  from YX_Redactivity_ScanRecord  where ScanDate='" + scanDate + "' group by ScanRecordID, WeiXinUserID";
                model = dataContext.ExecuteQuery<YX_Redactivity_ScanRecord>(sql).ToList();
            }
            return model;
        }

        /// <summary>
        /// 获取今日领取红包的用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_RedGetRecord> RedGetRecord(long eid)
        {
            List<YX_RedGetRecord> result = new List<YX_RedGetRecord>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                string scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string scanDateL = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                string sql = "select GetRecordID, WeiXinUserID, GetRedValue,YX_RedGetRecord.ActivityID from YX_RedGetRecord inner join  YX_ActivitySub on YX_RedGetRecord.ActivityID=YX_ActivitySub.ActivityID where (YX_ActivitySub.CompanyID=" + eid + " and YX_RedGetRecord.[GetState]=" + (int)Common.EnumText.GetState.RECEIVED + " and YX_RedGetRecord.[GetDate]<'" + scanDate + "'and YX_RedGetRecord.[GetDate]>'" + scanDateL + "')";
                //string sql = "select GetRecordID, WeiXinUserID, GetRedValue  from YX_RedGetRecord  where GetState=" + (int)Common.EnumText.GetState.RECEIVED + " and [GetDate]='" + scanDate + "' group by GetRecordID, WeiXinUserID, GetRedValue";
                result = dataContext.ExecuteQuery<YX_RedGetRecord>(sql).ToList();
            }
            return result;
        }


        /// <summary>
        /// 获取昨日领取红包的用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_RedGetRecord> RedGetRecordZ(long eid)
        {
            List<YX_RedGetRecord> result = new List<YX_RedGetRecord>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                string scanDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                string scanDateL = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                string sql = "select GetRecordID, WeiXinUserID, GetRedValue,YX_RedGetRecord.ActivityID from YX_RedGetRecord inner join  YX_ActivitySub on YX_RedGetRecord.ActivityID=YX_ActivitySub.ActivityID where (YX_ActivitySub.CompanyID=" + eid + " and YX_RedGetRecord.GetState=" + (int)Common.EnumText.GetState.RECEIVED + " and YX_RedGetRecord.[GetDate]<'" + scanDate + "'and YX_RedGetRecord.[GetDate]>'" + scanDateL + "')";
                //string sql = "select GetRecordID, WeiXinUserID, GetRedValue  from YX_RedGetRecord  where GetState=" + (int)Common.EnumText.GetState.RECEIVED + " and [GetDate]='" + scanDate + "' group by GetRecordID, WeiXinUserID, GetRedValue";
                result = dataContext.ExecuteQuery<YX_RedGetRecord>(sql).ToList();
            }
            return result;
        }

        /// <summary>
        /// 监管登录
        /// </summary>
        /// <param name="uName">用户名</param>
        /// <param name="uPwd">密码</param>
        /// <returns></returns>
        public RetResult AdminLogin(string uName, string uPwd)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                RetResult result = new RetResult { CmdError = CmdResultError.EXCEPTION };
                string yhm = ConfigurationManager.AppSettings["Name"];
                string mm = ConfigurationManager.AppSettings["Pwd"];
                if (uName.Trim() != yhm)
                {
                    result.Msg = "登录失败，用户名错误！";
                }
                else
                {
                    if (uPwd.Trim() != mm)
                    {
                        result.Msg = "登录失败，密码错误！";
                    }
                    else
                    {
                        AdminUserInfo adminloginInfo = new AdminUserInfo();
                        #region 为登录原型赋值
                        adminloginInfo.LoginName = yhm;
                        adminloginInfo.LoginPwd = mm;
                        #endregion
                        AdminCurrentUser.AdminUser = adminloginInfo;
                        result.Msg = "登录成功！";
                        result.CmdError = CmdResultError.NONE;
                    }
                }
                return result;
            }
        }
    }
}
