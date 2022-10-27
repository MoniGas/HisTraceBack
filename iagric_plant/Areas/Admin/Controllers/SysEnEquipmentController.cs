using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Log;
using System.IO;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysEnEquipmentController : Controller
    {
        //
        // GET: /SysEnEquipment/

        public ActionResult Index()
        {
            return View();
        }
        #region 20200817新加企业PDA设备信息
        /// <summary>
        /// 获取设备列表信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EnEquipmentList(int? id)
        {
            string name = Request["enName"];
            ViewBag.Name = name;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.GetMan;
            PagedList<PRRU_EnEquipmentInfo> dataList = bll.GetEnEquipmentList(name, pf.EnterpriseID, pageIndex);
            return View(dataList);
        }

        /// <summary>
        /// 添加设备串号
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddEquipmentNo(long eid, string eName, string equipmentNo)
        {
            JsonResult js = new JsonResult();
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo user = SessCokie.GetMan;
            if (string.IsNullOrEmpty(equipmentNo))
            {
                js.Data = new { res = "-1", info = "设备串号不能为空" };
                return js;
            }
            PRRU_EnEquipmentInfo model = new PRRU_EnEquipmentInfo();
            model.EnterpriseID = eid;
            model.EnterpriseName = eName;
            model.EquipmentNo = equipmentNo;
            model.LastDate = DateTime.Now;
            model.LastUserID = user.UserID;
            model.LastUserName = user.UserName;
            model.SetDate = DateTime.Now;
            model.SetEnterpriseID = user.EnterpriseID;
            model.SetUserID = user.UserID;
            model.SetUserName = user.UserName;
            model.Status = (int)Common.EnumFile.Status.used;
            LinqModel.BaseResultModel result = bll.Add(model);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        /// <summary>
        /// 编辑设备串号信息
        /// </summary>
        /// <param name="id">设备标识ID</param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            PRRU_EnEquipmentInfo result = new PRRU_EnEquipmentInfo();
            try
            {
                SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
                result = bll.GetEnEquipmentInfo(id);
            }
            catch (Exception ex)
            {
            }
            return View(result);
        }

        [HttpPost]
        public JsonResult EditEquipmentNo(long id, long eid, string eName, string equipmentNo)
        {
            JsonResult js = new JsonResult();
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo user = SessCokie.GetMan;
            if (string.IsNullOrEmpty(equipmentNo))
            {
                js.Data = new { res = "-1", info = "设备串号不能为空" };
                return js;
            }
            PRRU_EnEquipmentInfo model = new PRRU_EnEquipmentInfo();
            model.EnterpriseID = eid;
            model.EnterpriseName = eName;
            model.EquipmentNo = equipmentNo;
            model.LastDate = DateTime.Now;
            model.LastUserID = user.UserID;
            model.LastUserName = user.UserName;
            model.ID = id;
            LinqModel.BaseResultModel result = bll.EditEnEquipmentInfo(model);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        /// <summary>
        /// 启用/禁用设备
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <param name="type">1：启用；2：禁用</param>
        /// <returns></returns>
        public ActionResult EditStatus(long id, int type)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.EditEnEquipmentStatus(id, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/SysEnEquipment/EnEquipmentList" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadExcelUp(string excelurl)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
                result = bll.UploadExcelUp(excelurl);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "SysEnEquipment.UploadExcelUp()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        //excel格式下载
        public void ExcleGSDown()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "FiesData\\";
                string fileName = "设备串号Excel格式.xlsx";
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
        #endregion
    }
}
