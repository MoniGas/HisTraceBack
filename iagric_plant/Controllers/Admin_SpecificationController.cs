/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-11-05

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于规格管理控制器层

*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using BLL;

namespace web.Controllers
{
    public class Admin_SpecificationController : Controller
    {
        //
        // GET: /Admin_Specification/
        int pageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);

        public JsonResult Index(int? Value, int? pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;

            SpecificationBLL ObjSpecificationBLL = new SpecificationBLL();
            LinqModel.BaseResultList dataList = ObjSpecificationBLL.GetList(pf.EnterpriseID, Value, pageIndex, pageSize);

            return Json(dataList);
        }

        public JsonResult AddSave(string Value, string GuiGe)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            JsonResult js = new JsonResult();

            int intValue = Convert.ToInt32(Value);

            LinqModel.Specification ObjSpecification = new LinqModel.Specification();
            ObjSpecification.Value = intValue;
            ObjSpecification.GuiGe = GuiGe;
            ObjSpecification.Enterprise_Info_ID = pf.EnterpriseID;

            SpecificationBLL ObjSpecificationBLL = new SpecificationBLL();
            LinqModel.BaseResultModel DataResult = ObjSpecificationBLL.Add(ObjSpecification);

            return Json(DataResult);
        }

        public JsonResult GetModel(long id)
        {
            SpecificationBLL ObjSpecificationBLL = new SpecificationBLL();

            LinqModel.BaseResultModel DataModel = ObjSpecificationBLL.GetInfo(id);

            return Json(DataModel);
        }

        public JsonResult EditSave(long id, string Value,string GuiGe)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;

            int intValue = Convert.ToInt32(Value);

            LinqModel.Specification ObjSpecification = new LinqModel.Specification();
            ObjSpecification.ID = id;
            ObjSpecification.Value = intValue;
            ObjSpecification.GuiGe = GuiGe;
            ObjSpecification.Enterprise_Info_ID = pf.EnterpriseID;

            SpecificationBLL ObjSpecificationBLL = new SpecificationBLL();
            LinqModel.BaseResultModel DataResult = ObjSpecificationBLL.Edit(ObjSpecification);

            return Json(DataResult);
        }

        public JsonResult Delete(string Id)
        {
            SpecificationBLL ObjSpecificationBLL = new SpecificationBLL();
            LinqModel.BaseResultModel DataResult = ObjSpecificationBLL.Del(Id);

            return Json(DataResult);
        }

        public JsonResult GetSelectList()
        {
            SpecificationBLL ObjSpecificationBLL = new SpecificationBLL();

            LoginInfo pf = Common.Argument.SessCokie.Get;

            LinqModel.BaseResultList DataList = ObjSpecificationBLL.GetSelectList(pf.EnterpriseID);

            return Json(DataList);
        }
    }
}
