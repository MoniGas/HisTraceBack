using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_ShowNewsController : BaseController
    {
        //
        // GET: /Admin_ShowNews/

        public JsonResult Index(/*string channelId,*/ string title, int pageIndex)
        {
            BaseResultList liNews = new BaseResultList();
            try
            {
                LoginInfo pf = Common.Argument.SessCokie.Get;
                //long channel = Convert.ToInt64(channelId ?? "0");
                liNews = new ShowNewsBLL().GetList(pf.EnterpriseID, /*channel,*/ title, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowNewsController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(liNews);
        }

        public JsonResult Add(string newsTitle, /*long channelId,*/ string newsContent)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = Common.Argument.SessCokie.Get;
                ShowNews model = new ShowNews();
                //model.ChannelID = channelId;
                model.CompanyID = pf.EnterpriseID;
                model.EWM = "";
                model.Infos = newsContent;
                model.TimeAdd = DateTime.Now;
                model.Title = newsTitle;
                model.TopTime = model.TimeAdd;
                model.Url = "";
                model.UserName = pf.UserName;
                result = new ShowNewsBLL().Add(model, pf.MainCode);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowNewsController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Edit(long id, string newsTitle, /*long channelId,*/ string newsContent)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                ShowNews model = new ShowNews();
                //model.ChannelID = channelId;
                model.Infos = newsContent;
                model.Title = newsTitle;
                model.ID = id;
                result = new ShowNewsBLL().Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowNewsController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Del(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new ShowNewsBLL().Del(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowNewsController.Del";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Info(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new ShowNewsBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowNewsController.Info";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
