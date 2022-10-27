/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-12-15

** 联系方式 :13313318725

** 描述：主要用于配置导航模块信息的控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using System.ServiceModel.Dispatcher;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 配置产品信息控制器
    /// </summary>
    public class Admin_PageNavigationController : Controller
    {
        //
        // GET: /Admin_PageNavigation/

        /// <summary>
        /// 获取关联导航信息列表方法
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <returns>返回关联信息列表</returns>
        public JsonResult Index(string MaterialId,int PageIndex = 1)
        {
            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LoginInfo pf = Common.Argument.SessCokie.Get;
            LinqModel.BaseResultList DataList = ObjPageNavigationBLL.GetNavigationForEnterpriseList(pf.EnterpriseID, MaterialId, PageIndex);
            return Json(DataList);
        }

        /// <summary>
        /// 获取关联导航信息列表集合方法
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <returns>返回关联信息列表</returns>
        public JsonResult GetList(string MaterialId, int PageIndex = 1) 
        {
            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LoginInfo pf = Common.Argument.SessCokie.Get;
            LinqModel.BaseResultList DataList = ObjPageNavigationBLL.GetList(pf.EnterpriseID, MaterialId, PageIndex);
            return Json(DataList);
        }

        /// <summary>
        /// 删除关联导航信息方法
        /// </summary>
        /// <param name="Id">关联导航信息表Id</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult DelNavigationForEnterprise(string Id)
        {
            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LinqModel.BaseResultModel ObjResult = ObjPageNavigationBLL.DelNavigationForEnterprise(Id);
            return Json(ObjResult);
        }

        /// <summary>
        /// 删除关联导航所有信息方法
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult DelMaterialList(string MaterialId)
        {
            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LinqModel.BaseResultModel ObjResult = ObjPageNavigationBLL.DelMaterialList(MaterialId);
            return Json(ObjResult);
        }

        /// <summary>
        /// 获取导航列表方法
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回导航集合对象</returns>
        public JsonResult GetNavigationList(string MaterialId)
        {
            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LinqModel.BaseResultList DataList = ObjPageNavigationBLL.GetNavigationList(null);
            return Json(DataList);
        }

        /// <summary>
        /// 修改导航信息显示循序方法
        /// </summary>
        /// <param name="Id">关联导航信息表Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="Type">操作类型：“up”代表上移操作 “down”代表下移操作</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult UpdateViewNum(string Id, string MaterialId, string Type) 
        {
            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LinqModel.BaseResultModel ObjResult = ObjPageNavigationBLL.UpdateViewNum(Id,MaterialId, Type);
            return Json(ObjResult);
        }

        /// <summary>
        /// 更新排序后的导航模块方法
        /// </summary>
        /// <param name="ModelList">排序信息json字符串</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult UpdateList(string ModelList) 
        {
            JsonQueryStringConverter JsonString = new JsonQueryStringConverter();
            List<PageNavigationRequset> DataList = (List<PageNavigationRequset>)JsonString.ConvertStringToValue(ModelList, typeof(List<PageNavigationRequset>));
            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LinqModel.BaseResultModel ObjModel = ObjPageNavigationBLL.UpdateList(DataList);
            return Json(ObjModel);
        }

        /// <summary>
        /// 保存关联导航信息方法
        /// </summary>
        /// <param name="NavigationIdArray">导航Id集合字符串</param>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult SaveNavigationForEnterpriseList(string NavigationIdArray, string MaterialId) 
        {
            LinqModel.BaseResultModel ObjResult = new BaseResultModel();

            if (NavigationIdArray.Split(',').Length > 6)
            {
                ObjResult.code = "0";
                ObjResult.Msg = "最多选择6个导航模块！";    
                return Json(ObjResult);
            }

            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ObjResult = ObjPageNavigationBLL.SaveNavigationForEnterpriseList(NavigationIdArray, MaterialId, pf.EnterpriseID);
            return Json(ObjResult);
        }

        public JsonResult GetNavigationForMaterialList(string MaterialId)
        {
            BaseResultModel ObjBaseResultModel = new BaseResultModel();

            if (string.IsNullOrEmpty(MaterialId))
            {
                ObjBaseResultModel.code = "0";
                ObjBaseResultModel.Msg = "数据错误请刷新后重试！";

                return Json(ObjBaseResultModel);
            }

            PageNavigationBLL ObjPageNavigationBLL = new PageNavigationBLL();

            ObjBaseResultModel = ObjPageNavigationBLL.GetNavigationForMaterialList(Convert.ToInt64(MaterialId));

            return Json(ObjBaseResultModel);
        }
    }
}
