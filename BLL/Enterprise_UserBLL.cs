/********************************************************************************
** 作者： 高世聪
** 创始时间：2015-6-17
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：xxx
** 修改时间：xxx-xx-xx     
** 描述：
** 主要用于用户管理业务层
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class Enterprise_UserBLL
    {
        private readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        #region 获取用户信息集合方法
        /// <summary>
        /// 获取用户信息集合方法
        /// </summary>
        /// <param name="id">农企ID</param>
        /// <param name="pageIndex">分页码</param>
        /// <returns></returns>
        public BaseResultList GetList(long id, int pageIndex, string userName, string userRole)
        {
            long totalCount = 1;
            List<View_EnterpriseUserAndRole> dataList = new Enterprise_UserDAL().GetList(id, pageIndex, out totalCount, userName, userRole);

            return ToJson.NewListToJson(dataList, pageIndex, _pageSize, totalCount, "");
        }
        #endregion

        public BaseResultList GetLeveUser(long id)
        {
            long totalCount;
            List<View_EnterpriseLevelUser> dataList = new Enterprise_UserDAL().GetLeveUser(id, out totalCount);

            return ToJson.NewListToJson(dataList, 1, dataList.Count, totalCount, "");
        }

        #region 新增用户方法
        /// <summary>
        /// 新增用户方法
        /// </summary>
        /// <param name="objEnterpriseUser">用户linq model对象</param>
        /// <returns>返回操作结果的json串</returns>
        public BaseResultModel Add(Enterprise_User objEnterpriseUser)
        {
            Enterprise_UserDAL objPrruPlatFormUserDal = new Enterprise_UserDAL();
            RetResult objRetResult = objPrruPlatFormUserDal.Add(objEnterpriseUser);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        #endregion

        #region 删除方法
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public BaseResultModel Del(long id)
        {
            RetResult objRetResult = new Enterprise_UserDAL().Del(id);
            return ToJson.NewModelToJson(objRetResult, Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        #endregion

        #region 根据ID获取用户信息
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回用户信息对象</returns>
        public BaseResultModel GetModel(long id)
        {
            View_EnterpriseUserAndRole data = new Enterprise_UserDAL().GetModel(id);
            return ToJson.NewModelToJson(data, data == null ? "0" : "1", "");
        }
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回用户信息对象</returns>
        public Enterprise_User GetEntity(long id)
        {
            Enterprise_User data = new Enterprise_UserDAL().GetEntity(id);
            return data;
        }
        #endregion

        #region 根据ID获取企业用户信息
        /// <summary>
        /// 根据ID获取企业用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回用户信息对象</returns>
        public BaseResultModel GetLevelModel(long id)
        {
            View_EnterpriseLevelUser data = new Enterprise_UserDAL().GetLevelModel(id);
            return ToJson.NewModelToJson(data, data == null ? "0" : "1", "");
        }
        #endregion

        #region 修改密码方法

        /// <summary>
        /// 修改密码方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <param name="surepwd"></param>
        /// <returns>返回操作结果</returns>
        public BaseResultModel UpdatePas(long id, string oldPwd, string newPwd, string surepwd)
        {
            BaseResultModel objBaseResultModel = new BaseResultModel();

            if (string.IsNullOrEmpty(oldPwd.Trim()))
            {
                objBaseResultModel.code = "0";
                objBaseResultModel.Msg = "更新失败！旧密码不能为空！";
            }
            else if (string.IsNullOrEmpty(newPwd.Trim()))
            {
                objBaseResultModel.code = "0";
                objBaseResultModel.Msg = "更新失败！新密码不能为空！";
            }
            else if (string.IsNullOrEmpty(surepwd.Trim()))
            {
                objBaseResultModel.code = "0";
                objBaseResultModel.Msg = "更新失败！请确认新密码！";
            }
            else if (!newPwd.Trim().Equals(surepwd.Trim()))
            {
                objBaseResultModel.code = "0";
                objBaseResultModel.Msg = "更新失败！两次输入的密码不一致！";
            }
            else
            {
                Enterprise_UserDAL objPrruPlatFormUserDal = new Enterprise_UserDAL();
                RetResult objRetResult = objPrruPlatFormUserDal.UpdatePas(id, oldPwd, newPwd, surepwd);
                objBaseResultModel = ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
            }
            return objBaseResultModel;
        }
        #endregion

        #region 重置密码方法
        /// <summary>
        /// 重置密码方法
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>返回操作结果</returns>
        public BaseResultModel ResetPas(long id)
        {
            Enterprise_UserDAL objPrruPlatFormUserDal = new Enterprise_UserDAL();
            RetResult objRetResult = objPrruPlatFormUserDal.ResetPas(id);
            return ToJson.NewModelToJson(objRetResult, Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        #endregion

        #region 更新方法

        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="objEnterpriseUser">用户信息对象</param>
        /// <returns>返回操作结果</returns>
        public BaseResultModel Update(Enterprise_User objEnterpriseUser)
        {
            Enterprise_UserDAL objPrruPlatFormUserDal = new Enterprise_UserDAL();
            RetResult objRetResult = objPrruPlatFormUserDal.Update(objEnterpriseUser);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        #endregion

        #region 根据企业ID获取角色列表
        /// <summary>
        /// 根据企业ID获取角色列表
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetRoleList(long id)
        {
            Enterprise_UserDAL objPrruPlatFormUserDal = new Enterprise_UserDAL();
            long totalCount;
            List<View_EnterpriseUserAndRole> data = objPrruPlatFormUserDal.GetList(id, 0, out totalCount, "", "");
            data = data.Distinct(new MyComparer()).ToList();
            return ToJson.NewListToJson(data, 1, data.Count, totalCount, "");
        }
        #endregion

        #region 验证登录名是否可用
        /// <summary>
        /// 验证登录名是否可用
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns>1：可用 0：不可用</returns>
        public BaseResultModel VerifyLoginName(string loginName, long id)
        {
            Enterprise_UserDAL objEnterpriseUserDal = new Enterprise_UserDAL();
            RetResult result = objEnterpriseUserDal.VerifyLoginName(loginName, id);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }
        #endregion

        /// <summary>
        /// 根据用户名称获取用户实体   
        /// 陈志钢 WinCE
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns></returns>
        public Enterprise_User GetExcelUserModel(string userName, string pwd)
        {
            Enterprise_UserDAL dal = new Enterprise_UserDAL();
            var usermodel = dal.GetExcelUserModel(userName, pwd);
            return usermodel;
        }

        public Enterprise_User GetTxtUserModel(long eid, string userName, string pwd)
        {
            Enterprise_UserDAL dal = new Enterprise_UserDAL();
            var usermodel = dal.GetTxtUserModel(eid, userName, pwd);
            return usermodel;
        }

        public View_WinCe_EnterpriseInfoUser GetEntityByLoginName(string userName)
        {
            Enterprise_UserDAL dal = new Enterprise_UserDAL();
            return dal.GetEntityByLoginName(userName);
        }

        public View_WinCe_EnterpriseInfoUser GetEntityByLoginNameEx(string userName)
        {
            Enterprise_UserDAL dal = new Enterprise_UserDAL();
            return dal.GetEntityByLoginNameEx(userName);
        }
        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public BaseResultModel LoginWinCe(string loginName, string pwd)
        {
            BaseResultModel result = null;
            LoginDAL dal = new LoginDAL();
            Enterprise_UserDAL edal = new Enterprise_UserDAL();
            PRRU_PlatFormLevelDAL levelDAL = new PRRU_PlatFormLevelDAL();
            LoginInfo loginInfo = new LoginInfo();
            BaseResultModel dataInfo = dal.LoginVerify(loginName, pwd);
            if (dataInfo.code.Equals("1"))
            {
                Enterprise_User data = dataInfo.ObjModel as Enterprise_User;
                BaseResultModel model = new BaseResultModel();
                if (data != null)
                {
                    model = edal.LoginWinCe(loginName, pwd);
                    result = ToJson.NewModelToJson(model.ObjModel, model.code, model.Msg);
                }
            }
            else
            {
                result = ToJson.NewModelToJson(dataInfo, "0", dataInfo.Msg);
            }
            return result;
        }

        public BaseResultModel LoginWinCeEx(string loginName, string pwd)
        {
            BaseResultModel result = null;
            LoginDAL dal = new LoginDAL();
            Enterprise_UserDAL edal = new Enterprise_UserDAL();
            PRRU_PlatFormLevelDAL levelDAL = new PRRU_PlatFormLevelDAL();
            LoginInfo loginInfo = new LoginInfo();
            BaseResultModel dataInfo = dal.LoginVerifyEx(loginName, pwd);
            if (dataInfo.code.Equals("1"))
            {
                Enterprise_User data = dataInfo.ObjModel as Enterprise_User;
                BaseResultModel model = new BaseResultModel();
                if (data != null)
                {
                    model = edal.LoginWinCe(loginName, pwd);
                    result = ToJson.NewModelToJson(model.ObjModel, model.code, model.Msg);
                }
            }
            else
            {
                result = ToJson.NewModelToJson(dataInfo, "0", dataInfo.Msg);
            }
            return result;
        }
        #region 根据企业ID获取默认企业用户信息 2019-08-20 王坤
        /// <summary>
        /// 根据企业ID获取默认企业用户信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public Enterprise_User GetUserByEnterpriseId(long id)
        {
            Enterprise_User data = new Enterprise_UserDAL().GetUserByEnterpriseId(id);
            return data;
        }
        #endregion

        #region 二维码系统登录

        /// <summary>
        /// 二维码系统登录账号
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
            try
            {
                Enterprise_UserDAL dal = new Enterprise_UserDAL();
                return dal.GetEwmSysLoginInfo(enterpriseId, enterpriseName, mainCode, accountType);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion

        /// <summary>
        /// 获取子用户列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userRole"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList GetSubList(long id, int pageIndex, string userName)
        {
            long totalCount = 1;
            List<Enterprise_User> dataList = new Enterprise_UserDAL().GetSubList(id, pageIndex, out totalCount, userName);

            return ToJson.NewListToJson(dataList, pageIndex, _pageSize, totalCount, "");
        }
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public BaseResultList GetMaterial(long eid)
        {
            Enterprise_UserDAL dal = new Enterprise_UserDAL();
            long totalCount = 1;
            List<Material> dataList = dal.GetMaterial(eid, out totalCount);
            return ToJson.NewListToJson(dataList, 1, dataList.Count, totalCount, "");
        }
        /// <summary>
        /// 获取DI列表
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public BaseResultList GetSetDIList(long eid, long materialId)
        {
            Enterprise_UserDAL dal = new Enterprise_UserDAL();
            long totalCount = 1;
            List<MaterialDI> dataList = dal.GetSetDIList(eid, materialId, out totalCount);
            return ToJson.NewListToJson(dataList, 1, dataList.Count, totalCount, "");
        }
        public BaseResultModel AddSub(Enterprise_User objEnterpriseUser, string diList)
        {
            Enterprise_UserDAL objPrruPlatFormUserDal = new Enterprise_UserDAL();
            RetResult objRetResult = objPrruPlatFormUserDal.AddSub(objEnterpriseUser, diList);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        //string token = DTRequest.GetPostParam("Token");
        //string tokencode = DTRequest.GetPostParam("TokenCode");
        //string serviceID = DTRequest.GetPostParam("ServiceID");
        public BaseResultModel PrivateLogin(string loginname, string password, string token, string tokencode, string serviceID,string enMaincode)
        {
            BaseResultModel bsm = new BaseResultModel();
            Dal.Enterprise_UserDAL dal = new Enterprise_UserDAL();
            return dal.PrivateLogin(loginname, password, token,tokencode,serviceID,enMaincode);
            
        }
    }

    public class MyComparer : IEqualityComparer<View_EnterpriseUserAndRole>
    {
        public bool Equals(View_EnterpriseUserAndRole x, View_EnterpriseUserAndRole y)
        {
            return (x.RoleName == y.RoleName);
        }

        public int GetHashCode(View_EnterpriseUserAndRole gsbm)
        {
            return gsbm.ToString().GetHashCode();
        }
    }

}
