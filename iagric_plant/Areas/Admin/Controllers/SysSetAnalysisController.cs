using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysSetAnalysisController : Controller
    {
        //
        // GET: /Admin/SysSetAnalysis/

        public ActionResult Index(int? id)
        {
            string mac = Request["mac"];
            ViewBag.Mac = mac;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysSetAnalysisBLL bll = new SysSetAnalysisBLL();
            PagedList<SysSetAnalysis> dataList = bll.GetAnalysisList(mac, sDate, eDate, pageIndex);
            return View(dataList);
        }
        /// <summary>
        /// 添加Mac地址
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddMacAddress(string macAddress, string endDate)
        {
            JsonResult js = new JsonResult();
            LoginInfo user = SessCokie.GetMan;
            SysSetAnalysisBLL bll = new SysSetAnalysisBLL();
            if (string.IsNullOrEmpty(macAddress))
            {
                js.Data = new { res = "-1", info = "Mac地址不能为空" };
                return js;
            }
            SysSetAnalysis model = new SysSetAnalysis();
            model.AddDate = DateTime.Now;
            model.AddUserID = user.UserID;
            model.EndDate = Convert.ToDateTime(endDate);
            model.MacAddress = macAddress;
            model.Status = (int)Common.EnumFile.Status.used;
            LinqModel.BaseResultModel result = bll.Add(model);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            SysSetAnalysis result = new SysSetAnalysis();
            try
            {
                SysSetAnalysisBLL bll = new SysSetAnalysisBLL();
                result = bll.GetAnalysisInfo(id);
            }
            catch (Exception ex)
            {
            }
            return View(result);
        }

        [HttpPost]
        public JsonResult EditMacAddress(long id, string macAddress, string endDate)
        {
            JsonResult js = new JsonResult();
            SysSetAnalysisBLL bll = new SysSetAnalysisBLL();
            LoginInfo user = SessCokie.GetMan;
            if (string.IsNullOrEmpty(macAddress))
            {
                js.Data = new { res = "-1", info = "Mac地址不能为空" };
                return js;
            }
            SysSetAnalysis model = new SysSetAnalysis();
            model.EndDate = Convert.ToDateTime(endDate);
            model.MacAddress = macAddress;
            model.ID = id;
            LinqModel.BaseResultModel result = bll.Edit(model);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        /// <summary>
        /// 启用/禁用Mac地址
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <param name="type">1：启用；2：禁用</param>
        /// <returns></returns>
        public ActionResult EditStatus(long id, int type)
        {
            SysSetAnalysisBLL bll = new SysSetAnalysisBLL();
            RetResult ret = bll.EditMacStatus(id, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/SysSetAnalysis/Index" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
    }
}
