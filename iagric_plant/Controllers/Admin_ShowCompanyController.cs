using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_ShowCompanyController : BaseController
    {
        //
        // GET: /Admin_ShowCompany/

        public JsonResult Index()
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new ShowCompanyBLL().GetModel(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowCompanyController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Edit(string infos,string files)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;

                ShowCompany model = new ShowCompany();
                model.CompanyID = user.EnterpriseID;
                model.Infos = infos;
                model.StrFiles = files;
                result = new ShowCompanyBLL().Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowCompanyController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
