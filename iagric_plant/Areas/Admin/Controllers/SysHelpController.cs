/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-19

** 联系方式 :13313318725

** 描述：主要用于帮助管理控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using BLL;
using Common.Log;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 主要用于帮助管理控制器
    /// </summary>
    public class SysHelpController : Controller
    {
        //
        // GET: /Admin/SysHelp/

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="helpTitle">帮助标题</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(string helpTitle, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new HelpBLL().GetList(helpTitle, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "HelpController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取帮助模型
        /// </summary>
        /// <param name="id">帮助标识</param>
        /// <returns></returns>
        public JsonResult GetModel(int id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new HelpBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "HelpController.GetModel";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加帮助
        /// </summary>
        /// <param name="helpTitle">帮助标题</param>
        /// <param name="descriptions">帮助内容</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(string helpTitle, string descriptions,long type)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Help model = new Help();
                model.Status = (int)Common.EnumFile.Status.used;
                model.HelpTitle = helpTitle;
                model.HelpDescriptions = descriptions;
                model.TypeId = type;
                model.Sort = (int)Common.EnumFile.TopType.Cancel;
                model.Count = 0;
                model.UsefulCount = 0;
                model.NoCount = 0;
                model.AddDate = DateTime.Now;
                model.AddUser = user.UserID;
                model.Status = (int)Common.EnumFile.Status.used;
                result = new HelpBLL().Add(model);
            }
            catch (Exception ex)
            {
                string errData = "HelpController.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改帮助
        /// </summary>
        /// <param name="HelpId">帮助Id</param>
        /// <param name="helpTitle">帮助标题</param>
        /// <param name="HelpCode">帮助内容</param>
        /// <param name="HelpCode">帮助类型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(long helpId, string helpTitle, string descriptions, long type)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                JsonResult js = new JsonResult();
                Help model = new Help();
                model.HelpId = helpId;
                model.HelpTitle = helpTitle;
                model.HelpDescriptions = descriptions;
                model.TypeId = type;
                model.Sort = (int)Common.EnumFile.TopType.Cancel;
                model.AddDate = DateTime.Now;
                model.Status = (int)Common.EnumFile.Status.used;
                result = new HelpBLL().Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "HelpController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除帮助
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new HelpBLL().Del(id);
            }
            catch (Exception ex)
            {
                string errData = "HelpController.Delete";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取帮助类型列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList()
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new HelpBLL().GetList();
            }
            catch (Exception ex)
            {
                string errData = "HelpController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改帮助
        /// </summary>
        /// <param name="HelpId">帮助Id</param>
        /// <param name="sort">帮助排序</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditSort(long id, int sort)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                JsonResult js = new JsonResult();
                Help model = new Help();
                model.HelpId = id;
                model.Sort = sort;
                model.AddDate = DateTime.Now;
                model.Status = (int)Common.EnumFile.Status.used;
                result = new HelpBLL().EditSort(model);
            }
            catch (Exception ex)
            {
                string errData = "HelpController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="helpTitle">帮助标题</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult GetHelpList(string helpTitle, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new HelpBLL().GetHelpList(helpTitle, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "HelpController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
