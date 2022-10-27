/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-12-10

** 联系方式 :13313318725

** 描述：主要用于配置产品信息的业务层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/

using System;
using System.Collections.Generic;
using Common.Argument;
using System.Configuration;
using Dal;

namespace BLL
{
    /// <summary>
    /// 配置产品信息业务层类
    /// </summary>
    public class ProductInfoBLL
    {
        // 获取页码条数
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 查询该产品配置的信息
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <returns>返回产品信息集合</returns>
        public LinqModel.BaseResultList GetProductInfoForEnterprise(long EnterpriseId,string MaterialId,int PageIndex)
        {
            Dal.ProductInfoDAL ObjProductInfoDAL = new Dal.ProductInfoDAL();
            List<LinqModel.View_ProductInfoForEnterprise> DataList = ObjProductInfoDAL.GetProductInfoForEnterprise(EnterpriseId, MaterialId, PageIndex, _PageSize);
            return ToJson.NewListToJson(DataList, PageIndex, _PageSize, DataList == null ? 0 : DataList.Count, "");
        }

        /// <summary>
        /// 获取关联的产品信息对象方法
        /// </summary>
        /// <param name="Id">关联的产品信息表ID</param>
        /// <returns>返回关联信息对象</returns>
        public LinqModel.BaseResultModel GetProductInfoForEnterpriseModel(string Id) 
        {
            Dal.ProductInfoDAL ObjProductInfoDAL = new Dal.ProductInfoDAL();
            // 判断ID为空
            if (string.IsNullOrEmpty(Id))
            {
                return ToJson.NewModelToJson(new LinqModel.ProductInfoForEnterprise(), "0", "数据错误，请刷新后重试！");
            }
            LinqModel.View_ProductInfoForEnterprise DataModel = ObjProductInfoDAL.GetProductInfoForEnterpriseModel(Convert.ToInt64(Id));
            return ToJson.NewModelToJson(DataModel, Convert.ToInt32(DataModel == null ? "0" : "1").ToString(), DataModel == null ? "关联信息获取失败！请刷新后重试或检查网络！" : "获取信息成功！");
        }

        /// <summary>
        /// 获取活动模块集合方法
        /// </summary>
        /// <returns>返回活动模块集合</returns>
        public LinqModel.BaseResultList GetMaterialSpecList(long EnterpriseId, long MaterialId) 
        {
            Dal.ProductInfoDAL ObjProductInfoDAL = new Dal.ProductInfoDAL();
            List<LinqModel.View_MaterialSpecAndProperty> DataList = ObjProductInfoDAL.GetMaterialSpecList(EnterpriseId, MaterialId);
            return ToJson.NewListToJson(DataList, 1, DataList.Count, DataList.Count, "");
        }

        /// <summary>
        /// 配置产品信息方法
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="MaterialSpecId">产品信息模块Id字符串</param>
        /// <param name="ViewPropertyIdArray">商品Id</param>
        /// <param name="ViewOrderHotline">是否显示订购热线</param>
        /// <param name="ViewMaterialPrice">是否显示产品价格</param>
        /// <param name="ViewProductionTime">是否显示生产日期</param>
        /// <param name="ViewFactory">是否显示生产厂家</param>
        /// <param name="ViewComplaintPhone">是否显示投诉电话</param>
        /// <returns>操作结果对象</returns>
        public LinqModel.BaseResultModel AddProductInfoEnterprise(long EnterpriseId, long MaterialId, string ViewPropertyIdArray, string ViewOrderHotline,
            string ViewMaterialPrice, string ViewProductionTime, string ViewFactory, string ViewComplaintPhone)
        {
            // 创建产品关联信息模块表对象
            LinqModel.ProductInfoForEnterprise ObjProductInfoForEnterprise = new LinqModel.ProductInfoForEnterprise();
            ObjProductInfoForEnterprise.EnterpriseId = EnterpriseId;
            ObjProductInfoForEnterprise.MaterialId = MaterialId;

            if (!string.IsNullOrEmpty(ViewPropertyIdArray))
            {
                ViewPropertyIdArray = "," + ViewPropertyIdArray + ",";
            }

            ObjProductInfoForEnterprise.ViewPropertyIdArray = ViewPropertyIdArray;
            if (!string.IsNullOrEmpty(ViewOrderHotline))
            {
                ObjProductInfoForEnterprise.ViewOrderHotline = Convert.ToBoolean(ViewOrderHotline);
            }
            if (!string.IsNullOrEmpty(ViewMaterialPrice))
            {
                ObjProductInfoForEnterprise.ViewMaterialPrice = Convert.ToBoolean(ViewMaterialPrice);
            }

            if (!string.IsNullOrEmpty(ViewProductionTime))
            {
                ObjProductInfoForEnterprise.ViewProductionTime = Convert.ToBoolean(ViewProductionTime);
            }
            if (!string.IsNullOrEmpty(ViewFactory))
            {
                ObjProductInfoForEnterprise.ViewFactory = Convert.ToBoolean(ViewFactory);
            }
            if (!string.IsNullOrEmpty(ViewComplaintPhone))
            {
                ObjProductInfoForEnterprise.ViewComplaintPhone = Convert.ToBoolean(ViewComplaintPhone);
            }

            Dal.ProductInfoDAL ObjProductInfoDAL = new ProductInfoDAL();
            // 调用配置产品信息方法
            RetResult ObjRetsult = ObjProductInfoDAL.AddProductInfoEnterprise(ObjProductInfoForEnterprise);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetsult.IsSuccess).ToString(), ObjRetsult.Msg);
        }

        /// <summary>
        /// 修改关联产品信息方法
        /// </summary>
        /// <param name="Id">关联产品信息表Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="ViewPropertyIdArray">商品Id</param>
        /// <param name="ViewOrderHotline">是否显示订购热线</param>
        /// <param name="ViewMaterialPrice">是否显示产品价格</param>
        /// <param name="ViewProductionTime">是否显示生产日期</param>
        /// <param name="ViewFactory">是否显示生产厂家</param>
        /// <param name="ViewComplaintPhone">是否显示投诉电话</param>
        /// <returns>返回操作结果集合</returns>
        public LinqModel.BaseResultModel EditProductInfoForEnterprise(string Id, string MaterialId, string ViewPropertyIdArray,
            string ViewOrderHotline, string ViewMaterialPrice, string ViewProductionTime, string ViewFactory, string ViewComplaintPhone) 
        {
            // 判断表ID为空
            if (string.IsNullOrEmpty(Id)) 
            {
                return ToJson.NewRetResultToJson("0", "数据错误，请刷新后重试！");
            }
            // 判断产品Id为空
            if (string.IsNullOrEmpty(MaterialId))
            {
                return ToJson.NewRetResultToJson("0", "请选择产品！");
            }
            // 创建产品关联信息模块表对象
            LinqModel.ProductInfoForEnterprise ObjProductInfoForEnterprise = new LinqModel.ProductInfoForEnterprise();
            ObjProductInfoForEnterprise.Id = Convert.ToInt64(Id);
            ObjProductInfoForEnterprise.MaterialId = Convert.ToInt64(MaterialId);

            if (!string.IsNullOrEmpty(ViewPropertyIdArray))
            {
                ViewPropertyIdArray = "," + ViewPropertyIdArray + ",";
            }

            ObjProductInfoForEnterprise.ViewPropertyIdArray = ViewPropertyIdArray;
            if (!string.IsNullOrEmpty(ViewOrderHotline))
            {
                ObjProductInfoForEnterprise.ViewOrderHotline = Convert.ToBoolean(ViewOrderHotline);
            }
            if (!string.IsNullOrEmpty(ViewMaterialPrice))
            {
                ObjProductInfoForEnterprise.ViewMaterialPrice = Convert.ToBoolean(ViewMaterialPrice);
            }
            if (!string.IsNullOrEmpty(ViewProductionTime))
            {
                ObjProductInfoForEnterprise.ViewProductionTime = Convert.ToBoolean(ViewProductionTime);
            }
            if (!string.IsNullOrEmpty(ViewFactory))
            {
                ObjProductInfoForEnterprise.ViewFactory = Convert.ToBoolean(ViewFactory);
            }
            if (!string.IsNullOrEmpty(ViewComplaintPhone))
            {
                ObjProductInfoForEnterprise.ViewComplaintPhone = Convert.ToBoolean(ViewComplaintPhone);
            }
            Dal.ProductInfoDAL ObjProductInfoDAL = new ProductInfoDAL();

            RetResult ObjRetResult = ObjProductInfoDAL.EditProductInfoForEnterprise(ObjProductInfoForEnterprise);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 删除关联产品信息方法
        /// </summary>
        /// <param name="Id">关联产品信息表Id</param>
        /// <returns>返回操作结果对象</returns>
        public LinqModel.BaseResultModel DelProductInfoForEnterprise(string Id) 
        {
            // 判断表ID为空
            if (string.IsNullOrEmpty(Id))
            {
                return ToJson.NewRetResultToJson("0", "数据错误，请刷新后重试！");
            }
            Dal.ProductInfoDAL ObjProductInfoDAL = new ProductInfoDAL();
            RetResult ObjRetResult = ObjProductInfoDAL.DelProductInfoForEnterprise(Convert.ToInt64(Id));
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 获取推荐的商品活动集合
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>推荐的商品活动集合</returns>
        public List<LinqModel.View_ProductInfoForMaterial> GetMaterialProperty(long EnterpriseId, long MaterialId)
        {
            Dal.ProductInfoDAL ObjProductInfoDAL = new ProductInfoDAL();

            List<LinqModel.View_ProductInfoForMaterial> DataList = ObjProductInfoDAL.GetMaterialProperty(EnterpriseId, MaterialId);

            return DataList;
        }

        public LinqModel.View_ProductInfoForMaterial GetMaterialSpecPrice(long MaterialSpecId)
        {
            Dal.ProductInfoDAL ObjProductInfoDAL = new ProductInfoDAL();

            var DataModel = ObjProductInfoDAL.GetMaterialSpecPrice(MaterialSpecId);

            return DataModel;
        }

        public LinqModel.View_ProductInfoForMaterial GetProductInfoForMaterialModel(long MaterialSpecId)
        {
            Dal.ProductInfoDAL ObjProductInfoDAL = new ProductInfoDAL();

            return ObjProductInfoDAL.GetMaterialSpecPrice(MaterialSpecId);
        }
    }
}
