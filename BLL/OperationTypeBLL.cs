/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-12

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于生产环节信息管理业务层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class OperationTypeBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取生产环节信息方法
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="operationName">企业名称</param>
        /// <param name="type">企业类型</param>
        /// <returns>返回结果结合</returns>
        public BaseResultList GetList(long enterpriseID, int pageIndex, string operationName, int type = -1)
        {
            List<Batch_ZuoYeType> objList = new List<Batch_ZuoYeType>();
            if (enterpriseID < 0)
            {
                return ToJson.NewListToJson(objList, pageIndex, PageSize, 0, "");
            }

            OperationTypeDAL objOperationTypeDAL = new OperationTypeDAL();
            long totalCount = 0;
            objList = objOperationTypeDAL.GetList(enterpriseID, pageIndex, operationName, out totalCount, type);

            return ToJson.NewListToJson(objList, pageIndex, PageSize, totalCount, "");
        }
        public BaseResultList GetListOp(long enterpriseID, int type)
        {
            List<Batch_ZuoYeType> objList = new List<Batch_ZuoYeType>();
            OperationTypeDAL objOperationTypeDAL = new OperationTypeDAL();
            objList = objOperationTypeDAL.GetListOp(enterpriseID, type);

            return ToJson.NewListToJson(objList, 1, PageSize, 1, "");
        }

        /// <summary>
        /// 根据ID获取生产环节信息方法
        /// </summary>
        /// <param name="id">生产环节ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel SearchData(long id)
        {
            OperationTypeDAL objOperationTypeDAL = new OperationTypeDAL();
            Batch_ZuoYeType objBatch_ZuoYeType = objOperationTypeDAL.SearchData(id);
            return ToJson.NewModelToJson(objBatch_ZuoYeType, objBatch_ZuoYeType == null ? "0" : "1", "");
        }

        /// <summary>
        /// 添加生产环节方法
        /// </summary>
        /// <param name="operationType">生产环节实体对象</param>
        /// <returns>返回操作结果</returns>
        public LinqModel.BaseResultModel Add(LinqModel.Batch_ZuoYeType operationType)
        {
            OperationTypeDAL objOperationTypeDAL = new OperationTypeDAL();
            RetResult objRetResult = objOperationTypeDAL.Add(operationType);

            return ToJson.NewModelToJson(objRetResult.CrudCount,Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        /// <summary>
        /// 修改生产环节信息方法
        /// </summary>
        /// <param name="operationType">生产环节实体对象</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel Edit(LinqModel.Batch_ZuoYeType operationType)
        {
            OperationTypeDAL objOperationTypeDAL = new OperationTypeDAL();
            RetResult objRetResult = objOperationTypeDAL.Edit(operationType);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        /// <summary>
        /// 删除生产环节方法
        /// </summary>
        /// <param name="operationTypeID">生产环节ID</param>
        /// <returns>返回结果集合</returns>
        public BaseResultModel Del(string operationTypeID)
        {
            OperationTypeDAL objOperationTypeDAL = new OperationTypeDAL();
            RetResult ret = objOperationTypeDAL.Del(operationTypeID);
            BaseResultModel objRetResult = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return objRetResult;
        }
    }
}
