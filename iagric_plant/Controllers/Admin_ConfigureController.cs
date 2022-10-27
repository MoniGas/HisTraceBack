/********************************************************************************

** 作者： 郭心宇

** 创始时间：2015-12-15

** 联系方式 :13313318725

** 描述：主要用于配置宣传码信息的控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using BLL;
using System.Web.Mvc;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_ConfigureController : BaseController
    {
        /// <summary>
        /// 获取企业品牌信息
        /// </summary>
        /// <returns>企业品牌信息列表</returns>
        public ActionResult GetBrandList()
        {
            BaseResultList StrResult = new BaseResultList();
            try
            {
                LoginInfo Pf = SessCokie.Get;
                ConfigureBLL ObjConfigureBLL = new ConfigureBLL();
                StrResult = ObjConfigureBLL.GetBrandList(Pf.EnterpriseID);
            }
            catch (Exception Ex)
            {
                string ErrData = "Admin_Configure.GetModelList():Brand表";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(StrResult);
        }

        /// <summary>
        /// 获取企业员工信息
        /// </summary>
        /// <returns>企业员工信息列表</returns>
        public ActionResult GetUserList()
        {
            BaseResultList StrResult = new BaseResultList();
            try
            {
                LoginInfo Pf = SessCokie.Get;
                ConfigureBLL ObjConfigureBLL = new ConfigureBLL();
                StrResult = ObjConfigureBLL.GetUserList(Pf.EnterpriseID);
            }
            catch (Exception Ex)
            {
                string ErrData = "Admin_Configure.GetUserList():User表";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(StrResult);
        }

        /// <summary>
        /// 获取企业新闻信息
        /// </summary>
        /// <returns>企业新闻信息列表</returns>
        public ActionResult GetNewsList()
        {
            BaseResultList StrResult = new BaseResultList();
            try
            {
                LoginInfo Pf = SessCokie.Get;
                ConfigureBLL ObjConfigureBLL = new ConfigureBLL();
                StrResult = ObjConfigureBLL.GetNewsList(Pf.EnterpriseID);
            }
            catch(Exception Ex)
            {
                string ErrData = "Admin_Configure.GetNewsList():News表";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(StrResult);
        }

        /// <summary>
        /// 获取企业配置信息
        /// </summary>
        /// <returns>企业配置信息模型</returns>
        public JsonResult GetModel()
        {
            LoginInfo User = SessCokie.Get;
            BaseResultModel StrResult = new BaseResultModel();
            try
            {
                ConfigureBLL ObjConfigureBLL = new ConfigureBLL();
                StrResult = ObjConfigureBLL.GetModel(User.EnterpriseID);
            }
            catch (Exception Ex)
            {
                StrResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string ErrData = "Admin_Configure.GetModel():Configure表";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(StrResult);
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <returns>企业信息模型</returns>
        public ActionResult Index()
        {
            BaseResultModel Result = new BaseResultModel();
            try
            {
                LoginInfo User = SessCokie.Get;
                ConfigureBLL ObjConfigureBLL = new ConfigureBLL();
                Result = ObjConfigureBLL.GetCompanyModel(User.EnterpriseID);
            }
            catch (Exception Ex)
            {
                string ErrData = "Admin_Configure.Index():ShowCompany表";
                WriteLog.WriteErrorLog(ErrData + ":" +Ex.Message);
            }
            return Json(Result);
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="UserIdArray">员工ID数组</param>
        /// <param name="NewsIdArray">新闻ID数组</param>
        /// <param name="BrandIdArray">品牌ID数组</param>
        /// <returns></returns>
        public JsonResult Update(string UserIdArray, string NewsIdArray, string BrandIdArray)
        {
            LoginInfo User = SessCokie.Get;
            BaseResultModel StrResult = new BaseResultModel();
            try
            {
                Configure ObjConfigure = new Configure();
                ObjConfigure.User_ID_Array = UserIdArray;
                ObjConfigure.Brand_ID_Array = BrandIdArray;
                ObjConfigure.News_ID_Array = NewsIdArray;
                ObjConfigure.Company_ID = User.EnterpriseID;
                ObjConfigure.CompanyName = User.EnterpriseName;
                ConfigureBLL ObjconfigureBLL = new ConfigureBLL();
                StrResult = ObjconfigureBLL.Update(ObjConfigure,User.EnterpriseID, UserIdArray, NewsIdArray, BrandIdArray);
            }
            catch (Exception Ex)
            {
                StrResult = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string ErrData = "Admin_Enterprise_Role.Update():Enterprise_Role表";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(StrResult);
        }
    }
}