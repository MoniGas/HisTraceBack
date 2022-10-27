/********************************************************************************
** 作者： 高世聪
** 创始时间：2015-6-17
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：xxx
** 修改时间：xxx-xx-xx     
** 描述：
** 主要用于用户管理控制器

*********************************************************************************/

using System;
using System.Web.Mvc;
using BLL;
using Common;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_Enterprise_UserController : Controller
    {
        //
        // GET: /PRRU_PlatForm_User/

        #region 获取用户信息集合方法
        /// <summary>
        /// 获取用户信息集合方法
        /// </summary>
        /// <param name="id">农企ID</param>
        /// <param name="pageIndex">分页码</param>
        /// <param name="levelType">1：农企用户 2：监管部门用户 3：平台用户</param>
        /// <returns></returns>
        public JsonResult GetList(string userName,string userRole,int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList strResult = new BaseResultList();
            try
            {
                strResult = new Enterprise_UserBLL().GetList(user.EnterpriseID, pageIndex, userName, userRole);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Enterprise_User.GetList():View_EnterpriseUserAndRole视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion

        public JsonResult GetLevelUser()
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList strResult = new BaseResultList();
            try
            {
                strResult = new Enterprise_UserBLL().GetLeveUser(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Enterprise_User.GetLevelUser():View_EnterpriseLevelUser视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        #region 更新方法
        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="objPRRU_PlatForm_User">用户信息对象</param>
        /// <returns>返回操作结果</returns>
        public JsonResult Update(long id, int userRole, string userName, string userCode, string loginName, string loginPass, string telephone, string address) 
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                Enterprise_User objEnterpriseUser = new Enterprise_User
                {
                    Enterprise_User_ID = id,
                    Enterprise_Role_ID = userRole,
                    UserName = userName,
                    UserCode = userCode,
                    LoginName = loginName,
                    LoginPassWord = loginPass,
                    UserPhone = telephone,
                    UserAddress = address,
                    lastuser = user.UserID,
                    lastdate = DateTime.Now
                };

                strResult = new Enterprise_UserBLL().Update(objEnterpriseUser);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.Update():Enterprise_User表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion

        #region 新增用户方法
        /// <summary>
        /// 新增用户方法
        /// </summary>
        /// <param name="objPRRU_PlatForm_User">用户linq model对象</param>
        /// <returns>返回操作结果的json串</returns>
        public JsonResult Add(int userRole,string userName, string userCode, string loginName, string loginPass, string telephone, string address) 
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                Enterprise_User objEnterpriseUser = new Enterprise_User
                {
                    Enterprise_Role_ID = userRole,
                    Enterprise_Info_ID = user.EnterpriseID,
                    UserName = userName,
                    UserCode = userCode,
                    LoginName = loginName,
                    LoginPassWord = loginPass,
                    Status = (int) EnumFile.Status.used,
                    UserPhone = telephone,
                    UserAddress = address,
                    UserType = "注册",
                    adduser = user.UserID,
                    adddate = DateTime.Now,
                    lastuser = user.UserID,
                    lastdate = DateTime.Now
                };

                strResult = new Enterprise_UserBLL().Add(objEnterpriseUser);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.Add():Enterprise_User表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion

        #region 删除方法
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public JsonResult Del(long id)
        {
            BaseResultModel strResult;
            try
            {
                strResult = new Enterprise_UserBLL().Del(id);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.Del():Enterprise_User表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion

        #region 根据ID获取用户信息
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回用户信息对象</returns>
        public JsonResult GetModel(long id)
        {
            BaseResultModel strResult;
            try
            {
                strResult = new Enterprise_UserBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.GetModel():View_EnterpriseUserAndRole视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion

        public JsonResult GetLevelModel(long id)
        {
            BaseResultModel strResult;
            try
            {
                strResult = new Enterprise_UserBLL().GetLevelModel(id);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.GetModel():View_EnterpriseUserAndRole视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        #region 修改密码方法
        /// <summary>
        /// 修改密码方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="loginPassWord">新密码</param>
        /// <returns>返回操作结果</returns>
        public JsonResult UpdatePas(long id, string oldpwd, string newPwd, string surepwd) 
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                strResult = new Enterprise_UserBLL().UpdatePas(id, oldpwd, newPwd, surepwd);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.UpdatePas():Enterprise_User表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(strResult);
        }
        #endregion

        public JsonResult GetLoginInfo() 
        {
            LoginInfo user = SessCokie.Get;

            return Json(user.UserID);
        }

        #region 重置密码方法
        /// <summary>
        /// 重置密码方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回操作结果</returns>
        public JsonResult ResetPas(long id) 
        {
            BaseResultModel strResult = new BaseResultModel();
            try
            {
                strResult = new Enterprise_UserBLL().ResetPas(id);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.ResetPas():Enterprise_User表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion

        #region 根据企业ID获取角色列表
        /// <summary>
        /// 根据企业ID获取角色列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoleList() 
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList strResult = new BaseResultList();
            try
            {
                strResult = new Enterprise_UserBLL().GetRoleList(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Enterprise_User.GetRoleList():View_EnterpriseUserAndRole视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion

        public JsonResult VerifyLoginName(string loginName,long id = -1)
        {
            BaseResultModel objBaseResultModel = new BaseResultModel();
            try
            {
                objBaseResultModel = new Enterprise_UserBLL().VerifyLoginName(loginName, id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Enterprise_User.VerifyLoginName():EnterpriseUser表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(objBaseResultModel);
        }
        #region 子用户功能2021-10-21
        /// <summary>
        /// 获取子用户列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userRole"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetSubList(string userName, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList strResult = new BaseResultList();
            try
            {
                strResult = new Enterprise_UserBLL().GetSubList(user.EnterpriseID, pageIndex, userName);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Enterprise_User.GetList():View_EnterpriseUserAndRole视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="greenhouseid"></param>
        /// <returns></returns>
        public JsonResult GetMaterial()
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList strResult = new BaseResultList();
            try
            {
                Enterprise_UserBLL bll = new Enterprise_UserBLL();
                strResult = bll.GetMaterial(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "EnEquipment.GetSetEquipmentList()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        /// <summary>
        /// 获取DI列表
        /// </summary>
        /// <param name="greenhouseid"></param>
        /// <returns></returns>
        public JsonResult GetSetDIList(long materialId=0)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList strResult = new BaseResultList();
            try
            {
                Enterprise_UserBLL bll = new Enterprise_UserBLL();
                strResult = bll.GetSetDIList(user.EnterpriseID,materialId);
            }
            catch (Exception ex)
            {
                string errData = "EnEquipment.GetSetEquipmentList()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        /// <summary>
        /// 新增子用户
        /// </summary>
        /// <param name="objPRRU_PlatForm_User">用户linq model对象</param>
        /// <returns>返回操作结果的json串</returns>
        public JsonResult AddSub(string diList, string userName,  string loginName, string loginPass)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel strResult;
            try
            {
                Enterprise_User objEnterpriseUser = new Enterprise_User
                {
                    Enterprise_Info_ID = user.EnterpriseID,
                    UserName = userName,
                    LoginName = loginName,
                    LoginPassWord = loginPass,
                    Status = (int)EnumFile.Status.used,
                    UserType = "子用户",
                    adduser = user.UserID,
                    adddate = DateTime.Now,
                    lastuser = user.UserID,
                    lastdate = DateTime.Now
                };

                strResult = new Enterprise_UserBLL().AddSub(objEnterpriseUser,diList);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_User.Add():Enterprise_User表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #endregion
    }
}
