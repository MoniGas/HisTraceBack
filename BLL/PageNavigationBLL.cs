/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-12-15

** 联系方式 :13313318725

** 描述：主要用于配置导航的业务操作

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Dal;
using Common.Argument;
using System.Configuration;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 配置导航模块业务类
    /// </summary>
    public class PageNavigationBLL
    {      
        // 获取页码条数
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取导航配置信息列表
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <returns>返回导航配置信息列表集合</returns>
        public LinqModel.BaseResultList GetNavigationForEnterpriseList(long EnterpriseId, string MaterialId, int PageIndex)
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            List<LinqModel.NavigationForMaterialGroup> DataList = ObjPageNavigationDAL.GetNavigationForEnterpriseList(EnterpriseId, MaterialId, PageIndex, _PageSize);
            return ToJson.NewListToJson(DataList, PageIndex, _PageSize, DataList.Count, "");
        }

        /// <summary>
        /// 获取关联导航模块列表集合方法
        /// </summary>
        /// <param name="EnterpriseId">企业ID</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <returns>返回关联导航模块列表集合</returns>
        public LinqModel.BaseResultList GetList(long EnterpriseId, string MaterialId, int PageIndex)
        {
            if (MaterialId == null)
            {
                return ToJson.NewListToJson(new List<LinqModel.View_NavigationForMaterial>(), PageIndex, _PageSize, 0, "");
            }
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            List<LinqModel.View_NavigationForMaterial> DataList = ObjPageNavigationDAL.GetList(EnterpriseId, Convert.ToInt64(MaterialId), PageIndex, _PageSize);
            return ToJson.NewListToJson(DataList, PageIndex, _PageSize, DataList.Count, "");
        }

        /// <summary>
        /// 删除产品关联的导航信息
        /// </summary>
        /// <param name="Id">产品关联导航信息表Id</param>
        /// <returns>返回操作结果对象</returns>
        public LinqModel.BaseResultModel DelNavigationForEnterprise(string Id)
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            // 验证Id为空
            if (string.IsNullOrEmpty(Id))
            {
                return ToJson.NewRetResultToJson("0", "数据错误，请刷新后重试！");
            }
            RetResult ObjRetResult = ObjPageNavigationDAL.DelNavigationForEnterprise(Convert.ToInt64(Id));
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 删除产品关联的所有导航信息
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回操作结果对象</returns>
        public LinqModel.BaseResultModel DelMaterialList(string MaterialId)
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            // 验证MaterialId为空
            if (string.IsNullOrEmpty(MaterialId))
            {
                return ToJson.NewRetResultToJson("0", "数据错误，请刷新后重试！");
            }
            RetResult ObjRetResult = ObjPageNavigationDAL.DelMaterialList(Convert.ToInt64(MaterialId));
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 获取导航模块信息列表
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回导航信息模块列表集合</returns>
        public LinqModel.BaseResultList GetNavigationList(string MaterialId)
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            List<LinqModel.Navigation> DataList = ObjPageNavigationDAL.GetNavigationList(MaterialId);
            return ToJson.NewListToJson(DataList, 1, 100000, DataList.Count, "");
        }

        /// <summary>
        /// 更新导航模块的显示序号方法
        /// </summary>
        /// <param name="Id">配置导航信息模块Id</param>
        /// <param name="Value">显示序号</param>
        /// <returns>返回操作结果</returns>
        public LinqModel.BaseResultModel UpdateViewNum(string Id,string MaterialId, string Type)
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            // 验证产品Id和表Id为空
            if (string.IsNullOrEmpty(MaterialId) || string.IsNullOrEmpty(Id))
            {
                return ToJson.NewRetResultToJson("0", "数据错误，请刷新后重试！");
            }
            RetResult ObjRetResult = ObjPageNavigationDAL.UpdateViewNum(Convert.ToInt64(Id),Convert.ToInt64(MaterialId), Type);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 更新排序后的导航模块方法
        /// </summary>
        /// <param name="DataList">排序信息集合</param>
        /// <returns>返回操作结果对象</returns>
        public LinqModel.BaseResultModel UpdateList(List<PageNavigationRequset> DataList) 
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            RetResult ObjRetResult = ObjPageNavigationDAL.UpdateList(DataList);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(),ObjRetResult.Msg);
        }

        /// <summary>    
        /// 保存关联的导航页信息方法
        /// </summary>
        /// <param name="DataList">关联的导航页信息集合</param>
        /// <returns>返回操作结果</returns>
        public LinqModel.BaseResultModel SaveNavigationForEnterpriseList(string NavigationIdArray, string MaterialId, long EnterpriseId) 
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();
            // 验证关联导航Id为空
            if (string.IsNullOrEmpty(NavigationIdArray))
            {
                return ToJson.NewRetResultToJson("0", "请选择导航模块！");
            }
            RetResult ObjRetResult = ObjPageNavigationDAL.SaveNavigationForEnterpriseList(NavigationIdArray.Split(',').ToList(), Convert.ToInt64(MaterialId), EnterpriseId);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 获取产品拍码页的导航列表
        /// </summary>
        /// <param name="EnterpriseId">企业ID</param>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回导航列表</returns>
        public List<View_NavigationForMaterial> GetNavigationForMaterialList(long EnterpriseId, long MaterialId) 
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();

            List<LinqModel.View_NavigationForMaterial> DataList = ObjPageNavigationDAL.GetNavigationForMaterialList(EnterpriseId, MaterialId);

            return DataList;
        }

        public BaseResultModel GetNavigationForMaterialList(long MaterialId) 
        {
            PageNavigationDAL ObjPageNavigationDAL = new PageNavigationDAL();

            string strNavigationId = ObjPageNavigationDAL.GetNavigationForMaterialList(MaterialId);

            return ToJson.NewRetResultToJson(string.IsNullOrEmpty(strNavigationId) ? "0" : "1", strNavigationId);
        }
    }
}
