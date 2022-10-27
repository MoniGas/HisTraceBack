using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Webdiyer.WebControls.Mvc;
using LinqModel;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysEnLicenseController : Controller
    {
        //
        // GET: /Admin/SysEnLicense/

        public ActionResult Index(int? id)
        {
            string enName = Request["enName"];
            ViewBag.Name = enName;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnLicenseBLL bll = new SysEnLicenseBLL();
            PagedList<Enterprise_License> dataList = bll.GetEnLicenseList(enName, sDate, eDate, pageIndex);
            return View(dataList);
        }

        /// <summary>
        /// 获取接口查询记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GYJRecord(int? id)
        {
            string enName = Request["enName"];
            ViewBag.Name = enName;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnLicenseBLL bll = new SysEnLicenseBLL();
            PagedList<Enterprise_LicenseGYJRecord> dataList = bll.GetEnRecordList(enName, sDate, eDate, pageIndex);
            return View(dataList);
        }
    }
}
