/********************************************************************************
** 作者： 赵慧敏
** 创始时间：2019-5-28
** 联系方式 :13313318725
** 描述：黑名单
*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using BLL;

namespace iagric_plant.Controllers
{
    public class BackListController : Controller
    {
        public ActionResult Index()
        {
            BaseResultModel Reuslt = new BaseResultModel();
            try
            {
                LoginInfo User = SessCokie.Get;
                BackListBLL bll = new BackListBLL();
                Reuslt = bll.GetModel(User.EnterpriseID);
            }
            catch (Exception Ex)
            {
                string ErrData = "EnterpriseMuBanThreeImgController.Index";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(Reuslt);
        }

        [HttpPost]
        public JsonResult Edit(string files, string codeInfo)
        {
            BaseResultModel Result = new BaseResultModel();
            try
            {
                LoginInfo User = SessCokie.Get;
                BackList Model = new BackList();
                Model.EnterpriseId = User.EnterpriseID;
                Model.StrBackImg = files;
                Model.StrBackCode = codeInfo;
                Model.AddDate = DateTime.Now;
                Result = new BackListBLL().Edit(Model);
            }
            catch (Exception Ex)
            {
                string ErrData = "Admin_ShowCompanyController.Edit";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(Result);
        }
    }
}