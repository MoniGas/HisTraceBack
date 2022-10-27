/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-24

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 主要用于关联监管部门业务逻辑层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;


namespace BLL
{
    /// <summary>
    /// 用于关联监管部门业务逻辑层
    /// </summary>
    public class SysPlatFormManageBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取该服务中心下的监管部门列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="eId">ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetPlatFormList(string name, long eId, int? pageIndex)
        {
            SysPlatFormManageDAL dal = new SysPlatFormManageDAL();
            long totalCount = 0;
            List<LinqModel.PRRU_PlatForm> dataList = dal.GetPlatFormList(name, eId,
                pageIndex, out totalCount);
            return ToJson.NewListToJson(dataList, pageIndex.Value, _PageSize, totalCount, "");
        }

        /// <summary>
        /// 关联监管部门列表
        /// </summary>
        /// <param name="shengId">省</param>
        /// <param name="shiId">市</param>
        /// <returns></returns>
        public BaseResultList GetAreaPlatForm(long shengId, long shiId)
        {
            SysPlatFormManageDAL dal = new SysPlatFormManageDAL();
            List<LinqModel.PRRU_PlatForm> dataList = dal.GetAreaPlatForm(shengId, shiId);
            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataList.Count, "");
        }

        /// <summary>
        /// 关联监管部门列表
        /// </summary>
        /// <param name="shengId">省</param>
        /// <param name="shiId">市</param>
        /// <param name="eId">服务中心ID</param>
        /// <returns></returns>
        public BaseResultList GetAreaPlatForm(long shengId, long shiId, long eId)
        {
            SysPlatFormManageDAL dal = new SysPlatFormManageDAL();
            List<string> dataList = dal.GetAreaPlatForm(shengId, shiId, eId);
            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataList.Count, "");
        }

        /// <summary>
        /// 保存关联
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="arrayId"></param>
        /// <param name="falseId"></param>
        /// <returns></returns>
        public BaseResultModel Save(long pId, string arrayId, string falseId)
        {
            SysPlatFormManageDAL dal = new SysPlatFormManageDAL();
            RetResult result = dal.Save(pId, arrayId, falseId);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        /// <summary>
        /// 管理员修改自己密码
        /// </summary>
        /// <param name="eId">标识ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="surePassword">确认密码</param>
        /// <returns></returns>
        public RetResult EditPWD(long eid, string oldPassword, string newPassword, string surePassword)
        {
            SysPlatFormManageDAL dal = new SysPlatFormManageDAL();
            RetResult result = new RetResult();
            result = dal.EditPWD(eid, oldPassword, newPassword, surePassword);
            return result;
        }
    }
}
