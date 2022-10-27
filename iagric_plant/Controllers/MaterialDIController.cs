using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;
using System.IO;

namespace iagric_plant.Controllers
{
    public class MaterialDIController : BaseController
    {
        //
        // GET: /MaterialDI/

        public JsonResult Index(string searchName, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialDIBLL bll = new MaterialDIBLL();
                result = bll.GetList(pf.EnterpriseID, searchName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "MaterialDIController.Index():MaterialDI表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 同步DI
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncUDIDI()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                MaterialDIBLL bll = new MaterialDIBLL();
                result = bll.SyncUDIDI(pf.MainCode);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "MaterialDIController.SyncUDIDI():MaterialDI表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <param name="excelurl"></param>
        /// <param name="excelpath"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddExcel(string excelurl, string excelpath)
        {
            BaseResultModel result = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            try
            {
                GS1DIExcelRecord model = new GS1DIExcelRecord();
                model.EnterpriseID = user.EnterpriseID;
                model.ExcelURL = excelurl;
                model.ExcelPath = excelpath;
                model.AddDate = DateTime.Now;
                model.AddUser = user.UserID;
                model.Status = (int)Common.EnumFile.Status.used;
                MaterialDIBLL bll = new MaterialDIBLL();
                result = bll.AddExcelR(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "MaterialDIController.AddExcel()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        //excel格式下载
        public void ExcleDIDown()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "FiesData\\";
                string fileName = "GS1企业DI格式.xlsx";
                path = path + fileName;
                //FileName--要下载的文件名
                FileInfo DownloadFile = new FileInfo(path);
                if (DownloadFile.Exists)
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = false;
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode((fileName))));
                    Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                    Response.WriteFile(DownloadFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch
            {
            }
        }
    }
}
