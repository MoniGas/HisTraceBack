using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysMaterial_ReturnController : Controller
    {
        //
        // GET: /Admin/SysMaterial_Return/

        public ActionResult Index(string Name, int Status, string BeginDate, string EndDate, int PageIndex = 1)
        {
            BaseResultList list = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                list = new BLL.MaterialReturnOrderBLL().GetReturnOrderList(Name, 0, BeginDate, EndDate, (int)Common.EnumFile.PayStatus.ReturnFinsh, PageIndex);
            }
            catch (Exception ex)
            {
                string errData = "SysMaterial_ReturnController.Index():Material_ReturnMaterial表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(list);
        }

    }
}
