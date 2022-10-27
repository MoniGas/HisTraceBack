/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-16

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于农企和监管角色权限管理控制器

*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;

namespace iagric_plant.Controllers
{
    public class Admin_Enterprise_RoleController : BaseController
    {
        //
        // GET: /PRRU_PlatForm_RoleBLL/

        private readonly Enterprise_RoleBLL _bll = new Enterprise_RoleBLL();
        /// <summary>
        /// 获取角色列表信息方法
        /// </summary>
        /// <param name="id">农企ID</param>
        /// <returns></returns>
        public JsonResult Index(string name, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultList strResult = new LinqModel.BaseResultList();
            try
            {
                strResult = _bll.GetList(user.EnterpriseID, name, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Enterprise_Role.Index():Enterprise_Role表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(strResult);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoleList()
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultList strResult = new LinqModel.BaseResultList();
            try
            {
                strResult = _bll.GetList(user.EnterpriseID, "", 0);
            }
            catch (Exception ex) 
            {
                string errData = "Admin_Enterprise_Role.GetRoleList():Enterprise_Role表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }


        /// <summary>
        /// 更新角色信息方法
        /// </summary>
        /// <param name="objPRRU_PlatForm_Role">角色linq model对象</param>
        /// <returns></returns>
        public JsonResult Update(long rId, string roleName, string modelIdArray)
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultModel strResult;
            try
            {
                strResult = _bll.Update(rId, roleName, modelIdArray, user.UserID, user.EnterpriseID);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_Role.Update():Enterprise_Role表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        /// <summary>
        /// 获取角色权限信息方法
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public JsonResult GetModel(int id)
        {
            LinqModel.BaseResultModel strResult;
            try
            {
                strResult = _bll.GetModel(id);
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_Role.GetModel():Enterprise_Role表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        /// <summary>
        /// 获取可选模块信息方法
        /// </summary>
        /// <returns></returns>
        public ActionResult GetModelList()
        {
            LinqModel.BaseResultList strResult = new LinqModel.BaseResultList();
            try
            {
                strResult = _bll.GetModelList();
            }
            catch (Exception ex)
            {
                string errData = "Admin_Enterprise_Role.GetModelList():PRRU_Modual表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        /// <summary>
        /// 删除角色信息方法
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public JsonResult Del(string id)
        {
            LinqModel.BaseResultModel strResult;
            try
            {
                strResult = _bll.Del(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                strResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_Role.Del():Enterprise_Role表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="modelIdArray">角色串；只包含一级和二级的串</param>
        /// <returns></returns>
        public JsonResult Save(string roleName, string modelIdArray)
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultModel objResult;
            try
            {
                LinqModel.Enterprise_Role objEnterpriseRole =
                    new LinqModel.Enterprise_Role
                    {
                        Enterprise_Info_ID = user.EnterpriseID,
                        RoleName = roleName,
                        Modual_ID_Array = modelIdArray,
                        adduser = user.UserID,
                        lastuser = user.UserID,
                        adddate = DateTime.Now,
                        lastdate = DateTime.Now,
                        Status = (int) Common.EnumFile.Status.used
                    };
                objResult = _bll.Save(objEnterpriseRole);
            }
            catch (Exception ex)
            {
                objResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Enterprise_Role.Save():Enterprise_Role表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(objResult);
        }
    }
}
