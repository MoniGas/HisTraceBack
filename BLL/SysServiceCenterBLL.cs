/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-22

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 主要用于服务中心业务逻辑层

*********************************************************************************/   
using System;
using System.Collections.Generic;
using System.Configuration;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 主要用于服务中心业务逻辑层
    /// </summary>
    public class SysServiceCenterBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取服务中心列表
        /// </summary>
        /// <param name="sName">名称</param>
        /// <param name="selStatus">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public LinqModel.BaseResultList GetEnterpriseList(string sName, string selStatus, int? pageIndex)
        {
            SysServiceCenterDAL dal = new SysServiceCenterDAL();
            long totalCount = 0;
            List<LinqModel.View_PRRU_PlatFormUser> dataList = dal.GetEnterpriseList(sName, selStatus,
                pageIndex, out totalCount);
            return ToJson.NewListToJson(dataList, pageIndex.Value, _PageSize, totalCount, "");
        }

        /// <summary>
        /// 添加服务中心
        /// </summary>
        /// <param name="objPRRU_PlatForm">服务中心实体</param>
        /// <param name="objobjPRRU_PlatForm_User">用户实体</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel Add(LinqModel.PRRU_PlatForm objPRRU_PlatForm,
            LinqModel.PRRU_PlatForm_User objobjPRRU_PlatForm_User)
        {
            SysServiceCenterDAL dal = new SysServiceCenterDAL();
            RetResult result = dal.Add(objPRRU_PlatForm, objobjPRRU_PlatForm_User);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        /// <summary>
        /// 修改服务中心信息
        /// </summary>
        /// <param name="objPRRU_PlatForm">服务中心实体</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel Edit(LinqModel.PRRU_PlatForm objPRRU_PlatForm)
        {
            SysServiceCenterDAL dal = new SysServiceCenterDAL();
            RetResult result = dal.Edit(objPRRU_PlatForm);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }
    }
}
