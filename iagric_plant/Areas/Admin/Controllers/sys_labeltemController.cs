using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;
using Webdiyer.WebControls.Mvc;
using System.IO;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class sys_labeltemController : Controller
    {
        //
        // GET: /Admin/sys_labeltem/
        SysLabelTemBLL syslabelbll = new SysLabelTemBLL();
        public ActionResult Index(int? LabelTem_ID)
        {
            string LabelName = Request["LabelName"];
            int pageIndex = LabelTem_ID == null ? 1 : Convert.ToInt32(LabelTem_ID.ToString());
            PagedList<LabelTem> list = syslabelbll.GetLabelTemList(LabelName, pageIndex);
            return View(list);
        }

        public ActionResult Add()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Add(string LabelName, string LabelWidht,string LabelHeight,string Remarks, string LabelImg,string LocalUrl)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                LabelTem labeltem = new LabelTem();
                //brand.Enterprise_Info_ID = pf.EnterpriseID;
                labeltem.LabelName = LabelName;
                labeltem.LabelWidht = LabelWidht;
                labeltem.LabelHeight = LabelHeight;
                labeltem.AddData = DateTime.Now;
                labeltem.Remarks = Remarks;
                //labeltem.AddUser = pf.UserID.ToString();
                labeltem.LabelImg = LabelImg;
                labeltem.LocalUrl = LocalUrl;
                //labeltem.MainCode = pf.MainCode;
                labeltem.Status = (int)Common.EnumFile.Status.used;
                BrandBLL brandbll = new BrandBLL();
                result = syslabelbll.Add(labeltem);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "sys_labeltem.Add():LabelTem表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Delete(long LabelTem_ID)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = syslabelbll.Del(LabelTem_ID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "sys_labeltem.Delete():labeltem表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
