/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2016-12-09

** 联系方式 :15031109901

** 描述：主要用于仓库信息管理逻辑层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Dal;
using Common.Argument;
using System.Configuration;
using LinqModel.InterfaceModels;

namespace BLL
{
    /// <summary>
    /// 主要用于仓库信息管理逻辑层
    /// </summary>
    public class StoreBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

		public InterfaceResult storeLst(StoreRequestParam Param, string accessToken)
		{
			StoreDAL dal = new StoreDAL();
			return dal.storeLst(Param, accessToken);
		}


        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="storeName">仓库名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetList(long enterpriseId, string storeName, int pageIndex)
        {
            StoreDAL dal = new StoreDAL();
            long totalCount = 0;
            List<Store> liDearer = dal.GetList(enterpriseId, storeName, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liDearer, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 添加仓库
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Add(Store model)
        {
            StoreDAL dal = new StoreDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.StoreName))
            {
                ret.Msg = "名称不能为空！";
            }
            else
            {
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取仓库模型
        /// </summary>
        /// <param name="storeId">仓库标识</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel GetModel(long storeId)
        {
            StoreDAL dal = new StoreDAL();
            Store store = dal.GetModel(storeId);
            string code = "1";
            string msg = "";
            if (store == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(store, code, msg);
            return model;
        }

        /// <summary>
        /// 修改仓库
        /// </summary>
        /// <param name="newModel">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Edit(Store newModel)
        {
            StoreDAL dal = new StoreDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(newModel.StoreName))
            {
                ret.Msg = "名称不能为空！";
            }
            //else if (string.IsNullOrEmpty(newModel.StoreCode))
            //{
            //    ret.Msg = "编码不能为空！";
            //}         
            else
            {
                ret = dal.Edit(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="dealerId">仓库标识</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Del(long enterpriseId, long storeId)
        {
            StoreDAL dal = new StoreDAL();
            RetResult ret = dal.Del(enterpriseId, storeId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="id">仓库id</param>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetSlottingList(long id, long enterpriseId, int pageIndex)
        {
            long totalCount = 0;
            StoreDAL dal = new StoreDAL();
            List<Store> model = dal.GetSlottingList(id, enterpriseId, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        #region
        /// <summary>
        /// 获取垛位码列表20170327
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="cribName">垛位名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetCribList(long enterpriseId, string cribName, int pageIndex)
        {
            StoreDAL dal = new StoreDAL();
            long totalCount = 0;
            List<Store> liDearer = dal.GetCribList(enterpriseId, cribName, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liDearer, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 添加垛位
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel AddCrib(Store model)
        {
            StoreDAL dal = new StoreDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.StoreName))
            {
                ret.Msg = "名称不能为空！";
            }
            else
            {
                ret = dal.AddCrib(model);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 修改垛位
        /// </summary>
        /// <param name="newModel">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel EditCrib(Store newModel)
        {
            StoreDAL dal = new StoreDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(newModel.StoreName))
            {
                ret.Msg = "名称不能为空！";
            }
            //else if (string.IsNullOrEmpty(newModel.StoreCode))
            //{
            //    ret.Msg = "编码不能为空！";
            //}         
            else
            {
                ret = dal.EditCrib(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除垛位
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="storeId">标识ID</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel DelCrib(long enterpriseId, long storeId)
        {
            StoreDAL dal = new StoreDAL();
            RetResult ret = dal.DelCrib(enterpriseId, storeId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        #endregion

        #region 20170412库存查询
        /// <summary>
        /// 获取仓库库存
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="storeName">仓库名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetInventory(long enterpriseId,  string maName, int pageIndex)
        {
            StoreDAL dal = new StoreDAL();
            long totalCount = 0;
            List<StoreInfo> liModel = dal.GetInventory(enterpriseId, maName, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liModel, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        public BaseResultList GetInventoryInfo(long enterpriseId, string maName, int pageIndex)
        {
            StoreDAL dal = new StoreDAL();
            long totalCount = 0;
            List<View_StoreInfo> liModel = dal.GetInventoryInfo(enterpriseId, maName, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liModel, pageIndex, _PageSize, totalCount, "");
            return result;
        }
        #endregion
    }
}
