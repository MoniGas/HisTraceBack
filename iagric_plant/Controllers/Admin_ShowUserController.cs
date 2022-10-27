using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_ShowUserController : Controller
    {
        //
        // GET: /Admin_ShowUser/

        public JsonResult GetList(string name,int pageIndex)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList dataList = new BaseResultList();
            try
            {
                ShowUserBLL objShowUserBLL = new ShowUserBLL();
                dataList = objShowUserBLL.GetList(user.EnterpriseID, name, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowUserController.GetList():ShowUser表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        public JsonResult GetModel(long id)
        {
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                ShowUserBLL objShowUserBLL = new ShowUserBLL();
                dataList = objShowUserBLL.GetModel(id);
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowUserController.GetModel():ShowUser表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(dataList);
        }

        public JsonResult Add(string img, string name, string position, string telPhone, string mail, string qq, string hometown, string location, string memo, string infos) 
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                ShowUserBLL objShowUserBLL = new ShowUserBLL();
                dataList = objShowUserBLL.Add(user.EnterpriseID, img, name, position, telPhone, mail, qq, hometown, location, memo, infos, user.MainCode);
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowUserController.Add():ShowUser表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        public JsonResult Edit(long uId, string img, string name, string position, string telPhone, string mail, string qq, string hometown, string location, string memo, string infos) 
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                ShowUserBLL objShowUserBLL = new ShowUserBLL();
                dataList = objShowUserBLL.Edit(user.EnterpriseID, uId, img, name, position, telPhone, mail, qq, hometown, location, memo, infos, user.MainCode);
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowUserController.Edit():ShowUser表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        public JsonResult Del(long id) 
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                ShowUserBLL objShowUserBLL = new ShowUserBLL();
                dataList = objShowUserBLL.Del(user.EnterpriseID, id);
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowUserController.Del():ShowUser表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }
    }
}
