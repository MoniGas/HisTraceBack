/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-12-10

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于配置产品信息的控制器

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Common.Argument;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 配置产品信息控制器
    /// </summary>
    public class Admin_ProductInfoController : Controller
    {
        //
        // GET: /Admin_ProductInfo/

        /// <summary>
        /// 获取产品关联的信息模块集合方法
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <returns>返回产品信息模块集合</returns>
        public JsonResult Index(string MaterialId,int? PageIndex =1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BLL.ProductInfoBLL objProductInfoBLL = new BLL.ProductInfoBLL();
            // 调用获取企业关联的产品信息模块方法
            LinqModel.BaseResultList dataList = objProductInfoBLL.GetProductInfoForEnterprise(pf.EnterpriseID,MaterialId, PageIndex.Value);
            return Json(dataList);
        }

        /// <summary>
        /// 获取关联的产品信息对象方法
        /// </summary>
        /// <param name="Id">关联产品信息表Id</param>
        /// <returns>返回关联产品信息对象</returns>
        public JsonResult GetProductInfoForEnterpriseModel(string Id) 
        {
            BLL.ProductInfoBLL objProductInfoBLL = new BLL.ProductInfoBLL();
            LinqModel.BaseResultModel DataModel = objProductInfoBLL.GetProductInfoForEnterpriseModel(Id);
            return Json(DataModel);
        }

        /// <summary>
        /// 获取商品信息模块集合方法
        /// </summary>
        /// <returns>返回产品信息模块集合对象</returns>
        public JsonResult GetMaterialSpecList(string MaterialId) 
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BLL.ProductInfoBLL objProductInfoBLL = new BLL.ProductInfoBLL();
            // 调用获取产品信息模块方法
            LinqModel.BaseResultList dataList = objProductInfoBLL.GetMaterialSpecList(pf.EnterpriseID, Convert.ToInt64(MaterialId));
            return Json(dataList);
        }

        /// <summary>
        /// 添加产品信息模块方法
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="ViewPropertyIdArray">活动Id数组</param>
        /// <param name="ViewOrderHotline">是否显示订购热线</param>
        /// <param name="ViewMaterialPrice">是否显示产品价格</param>
        /// <param name="ViewProductionTime">是否显示生产日期</param>
        /// <param name="ViewFactory">是否显示生产厂家</param>
        /// <param name="ViewComplaintPhone">是否显示投诉电话</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult AddProductInfoForEnterprise(string MaterialId, string ViewPropertyIdArray, string ViewOrderHotline, 
            string ViewMaterialPrice, string ViewProductionTime, string ViewFactory, string ViewComplaintPhone) 
        {
            BLL.ProductInfoBLL ObjProductInfoBLL = new BLL.ProductInfoBLL();
            LoginInfo pf = Common.Argument.SessCokie.Get;
            // 调用添加产品信息模块方法
            LinqModel.BaseResultModel ObjBaseResultModel = ObjProductInfoBLL.AddProductInfoEnterprise(pf.EnterpriseID, Convert.ToInt64(MaterialId),
                ViewPropertyIdArray, ViewOrderHotline, ViewMaterialPrice, ViewProductionTime, ViewFactory, ViewComplaintPhone);
            return Json(ObjBaseResultModel);
        }

        /// <summary>
        /// 修改关联产品信息方法
        /// </summary>
        /// <param name="Id">关联产品信息表Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="ViewPropertyIdArray">活动Id数组</param>
        /// <param name="ViewOrderHotline">是否显示订购热线</param>
        /// <param name="ViewMaterialPrice">是否显示产品价格</param>
        /// <param name="ViewProductionTime">是否显示生产日期</param>
        /// <param name="ViewFactory">是否显示生产厂家</param>
        /// <param name="ViewComplaintPhone">是否显示投诉电话</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult EditProductInfoForEnterprise(string Id, string MaterialId, string ViewPropertyIdArray, 
            string ViewOrderHotline, string ViewMaterialPrice, string ViewProductionTime, string ViewFactory, string ViewComplaintPhone) 
        {
            BLL.ProductInfoBLL ObjProductInfoBLL = new BLL.ProductInfoBLL();
            LinqModel.BaseResultModel ObjBaseResultModel = ObjProductInfoBLL.EditProductInfoForEnterprise(Id, MaterialId, ViewPropertyIdArray,
                ViewOrderHotline, ViewMaterialPrice, ViewProductionTime, ViewFactory, ViewComplaintPhone);
            return Json(ObjBaseResultModel);
        }

        /// <summary>
        /// 删除关联产品信息方法
        /// </summary>
        /// <param name="Id">关联查询信息表Id</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult DelProductInfoForEnterprise(string Id) 
        {
            BLL.ProductInfoBLL ObjProductInfoBLL = new BLL.ProductInfoBLL();
            LinqModel.BaseResultModel ObjBaseResultModel = ObjProductInfoBLL.DelProductInfoForEnterprise(Id);
            return Json(ObjBaseResultModel);
        }

        /// <summary>
        /// 获取推广的商品活动信息
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public List<LinqModel.View_ProductInfoForMaterial> GetMaterialProperty(long MaterialId) 
        {
            BLL.ProductInfoBLL ObjProductInfoBLL = new BLL.ProductInfoBLL();
            LoginInfo pf = Common.Argument.SessCokie.Get;
            return ObjProductInfoBLL.GetMaterialProperty(pf.EnterpriseID, MaterialId);
        }
    }
}
