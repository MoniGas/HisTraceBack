using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;
using System.Data;
using Aspose.Cells;
using System.IO;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 李子巍
    /// 经销商控制器
    /// </summary>
    public class Admin_DealerController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult Index(string dealerName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new DealerBLL().GetList(user.EnterpriseID, dealerName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_DealerController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Info(int id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new DealerBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_DealerController.Info";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 赵慧敏改-新增 返回RetResult模型的json字符串
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(string dealerName, long dealerSheng, long dealerShi, long dealerQu, string dealerAddress, string dealerPerson, string dealerPhone, string dealerLocation)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Dealer model = new Dealer();
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                model.DealerName = dealerName;
                model.Dictionary_AddressSheng_ID = dealerSheng;
                model.Dictionary_AddressShi_ID = dealerShi;
                model.Dictionary_AddressQu_ID = dealerQu;
                model.Address = dealerAddress;
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                model.languageid = 1;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Person = dealerPerson;
                model.Phone = dealerPhone;
                model.location = dealerLocation;
                model.Status = (int)Common.EnumFile.Status.used;
                result = new DealerBLL().Add(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_DealerController.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Edit(long dealerId, string dealerName, long dealerSheng, long dealerShi, long dealerQu, string dealerAddress, string dealerPerson, string dealerPhone, string dealerLocation)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                JsonResult js = new JsonResult();
                Dealer model = new Dealer();
                model.Dealer_ID = dealerId;
                model.DealerName = dealerName;
                model.Dictionary_AddressSheng_ID = dealerSheng;
                model.Dictionary_AddressShi_ID = dealerShi;
                model.Dictionary_AddressQu_ID = dealerQu;
                model.Address = dealerAddress;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Person = dealerPerson;
                model.Phone = dealerPhone;
                model.location = dealerLocation;
                model.Enterprise_Info_ID = user.EnterpriseID;
                result = new DealerBLL().Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_DealerController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Delete(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new DealerBLL().Del(user.EnterpriseID, id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_DealerController.Delete";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult AddExcelDealer(string excelurl, string excelpath)
        {
            BaseResultModel result = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            try
            {
                DealerExcelRecord model = new DealerExcelRecord();
                model.EnterpriseID = user.EnterpriseID;
                model.ExcelURL = excelurl;
                model.ExcelPath = excelpath;
                model.AddDate = DateTime.Now;
                model.AddUser = user.UserID;
                model.Status = (int)Common.EnumFile.Status.unaudited;
                DealerBLL bll = new DealerBLL();
                result = bll.AddExcelR(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material.AddExcelR():MaterialExportExcelRecord表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 产品导出Excel
        /// </summary>
        /// <returns></returns>
        public int ExportExcel()
        {
            int json = 0;
            try
            {
                string path = Server.MapPath(@"/File/template.xlsx");
                System.IO.FileInfo DownloadFile = new System.IO.FileInfo(path);
                string strFileName = Path.GetFileName(path);
                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = false;
                Response.ContentType = "application/octet-stream";
                //Response.ContentType = "application/ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + strFileName);
                Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                Response.WriteFile(DownloadFile.FullName);
                Response.Flush();
                Response.End();
                json = 1;
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(ex.Message);
            }
            return json;
        }
    }
}
