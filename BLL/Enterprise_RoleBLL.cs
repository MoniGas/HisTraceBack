/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-16

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

** 主要用于农企和监管角色权限管理业务层

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;

namespace BLL
{
    public class Enterprise_RoleBLL
    {
        private readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        readonly Enterprise_RoleDAL _dal = new Enterprise_RoleDAL();
        /// <summary>
        /// 获取角色列表信息方法
        /// </summary>
        /// <param name="id">农企ID</param>
        /// <param name="pageIndex">分页码</param>
        /// <returns>返回角色列表json信息集合</returns>
        public LinqModel.BaseResultList GetList(long id,string name,int pageIndex) 
        {
            long totalCount;
            List<LinqModel.Enterprise_Role> dataList = _dal.GetList(id, name, pageIndex, out totalCount);
            return ToJson.NewListToJson(dataList, pageIndex, _pageSize, totalCount, "");
        }

        /// <summary>
        /// 根据ID获取该角色的权限信息
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>返回角色的json信息</returns>
        public LinqModel.BaseResultModel GetModel(int id) 
        {
            LinqModel.Enterprise_Role data = _dal.GetModel(id);
            return ToJson.NewModelToJson(data, data == null ? "0" : "1", "");
        }
        
        /// <summary>
        /// 获取可选模块集合方法
        /// </summary>
        /// <returns>返回可选模块json信息</returns>
        public LinqModel.BaseResultList GetModelList() 
        {
            PRRU_PlatFormLevelDAL objPrruPlatFormLevelDal = new PRRU_PlatFormLevelDAL();
            PRRU_ModualDAL objPrruModualDal = new PRRU_ModualDAL();
            LinqModel.PRRU_PlatFormLevel data = objPrruPlatFormLevelDal.GetModel(1);
            List<LinqModel.PRRU_Modual> dataList = objPrruModualDal.GetModelList(data.Modual_ID_Array);
            return ToJson.NewListToJson(dataList, 1, 100000, dataList.Count, "");
        }

        /// <summary>
        /// 更新角色信息方法
        /// </summary>
        /// <param name="objPRRU_PlatForm_Role">角色信息LINQ MODEL</param>
        /// <returns>操作结果</returns>
        public LinqModel.BaseResultModel Update(long rId, string roleName, string modelIdArray, long userId, long enterpriseID) 
        {
            string tempModelId = "10000,10001,10002";

            if (!modelIdArray.Equals("") && !modelIdArray.Substring(0, 1).Equals(","))
            {
                tempModelId += ",";
            }
            tempModelId += modelIdArray;
          
            RetResult objRetResult = _dal.Update(rId, roleName, tempModelId, userId,enterpriseID);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        /// <summary>
        /// 删除角色信息方法
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>返回操作结果json信息</returns>
        public LinqModel.BaseResultModel Del(int id)
        {
            RetResult objRetResult = _dal.Del(id);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        public LinqModel.BaseResultModel Save(LinqModel.Enterprise_Role objEnterpriseRole) 
        {
            string tempModelId = "10000";
            if (!objEnterpriseRole.Modual_ID_Array.Equals("") && !objEnterpriseRole.Modual_ID_Array.Substring(0, 1).Equals(",")) 
            {
                tempModelId += ",";
            }
            tempModelId += objEnterpriseRole.Modual_ID_Array;
            objEnterpriseRole.Modual_ID_Array = tempModelId;
            RetResult objResult = _dal.Save(objEnterpriseRole);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objResult.IsSuccess).ToString(), objResult.Msg);
        }
    }
}
