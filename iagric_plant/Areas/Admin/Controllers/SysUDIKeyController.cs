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
    public class SysUDIKeyController : Controller
    {
        //
        // GET: /Admin/SysUDIKey/

        public ActionResult Index(int? id)
        {
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            UDIMaterialBLL bll = new UDIMaterialBLL();
            PagedList<UDIKey> dataList = bll.GetUDIKeyList(sDate, eDate, pageIndex);
            return View(dataList);
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddUDIKey(int count, string linkMan, string linkPhone, string reason, string endDate)
        {
            JsonResult js = new JsonResult();
            LoginInfo user = SessCokie.GetMan;
            UDIMaterialBLL bll = new UDIMaterialBLL();
            UDIKey model = new UDIKey();
            model.AddDate = DateTime.Now;
            model.AddUserID = user.UserID;
            model.MaterialCount = count;
            model.LinkMan = linkMan;
            model.LinkPhone = linkPhone;
            model.AddReason = reason;
            model.EndDate = Convert.ToDateTime(endDate);
            model.Status = (int)Common.EnumFile.Status.used;
            LinqModel.BaseResultModel result = bll.Add(model);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        public ActionResult Edit(long id)
        {
            UDIKey result = new UDIKey();
            try
            {
                UDIMaterialBLL bll = new UDIMaterialBLL();
                result = bll.GetKeyInfo(id);
            }
            catch (Exception ex)
            {
            }
            return View(result);
        }

        [HttpPost]
        public JsonResult EditUDIKey(long id, int count, string linkMan, string linkPhone, string reason, string endDate)
        {
            JsonResult js = new JsonResult();
            UDIMaterialBLL bll = new UDIMaterialBLL();
            LoginInfo user = SessCokie.GetMan;
            UDIKey model = new UDIKey();
            model.MaterialCount = count;
            model.LinkMan = linkMan;
            model.LinkPhone = linkPhone;
            model.AddReason = reason;
            model.EndDate = Convert.ToDateTime(endDate);
            model.ID = id;
            LinqModel.BaseResultModel result = bll.Edit(model);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">1：启用；2：禁用</param>
        /// <returns></returns>
        public ActionResult EditStatus(long id, int type)
        {
            UDIMaterialBLL bll = new UDIMaterialBLL();
            RetResult ret = bll.EditStatus(id, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/SysUDIKey/Index" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
    }
}
