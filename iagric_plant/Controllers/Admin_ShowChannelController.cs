using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_ShowChannelController : BaseController
    {
        //
        // GET: /Admin_ShowChannel/

        public JsonResult Index(string channelName)
        {
            BaseResultList liChannel = new BaseResultList();
            try
            {
                LoginInfo pf = Common.Argument.SessCokie.Get;
                //string name = Request["channelName"];
                liChannel = new BLL.ShowChannelBLL().GetList(pf.EnterpriseID, channelName);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowChannelController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(liChannel);
        }

        public JsonResult Add(string channelName)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                ShowChannel model = new ShowChannel();
                model.ChannelName = channelName;
                model.CompanyID = user.EnterpriseID;
                result = new ShowChannelBLL().Add(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowChannelController.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Edit(string channelName, long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                ShowChannel model = new ShowChannel();
                model.ChannelName = channelName;
                model.ID = id;
                result = new ShowChannelBLL().Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowChannelController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Del(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new ShowChannelBLL().Del(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowChannelController.Del";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Info(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new ShowChannelBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowChannelController.Info";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
