using System.Web.Mvc;

namespace iagric_plant.Controllers
{
    public class Admin_WapDepartController : Controller
    {
        //
        // GET: /Admin_WapDepart/

        public ActionResult Index()
        {
            //string ewm = Request["ewm"];
            //if (string.IsNullOrEmpty(Request["ewm"]))
            //{
            //    return Content("<script>alert('请拍码访问此页面！');window.close();</script>");
            //}
            //string[] nodes = ewm.Split('.');
            //if (!(nodes.Length == 8 || nodes.Length == 7))
            //{
            //    return Content("<script>alert('二维码格式不正确 ！');window.close();</script>");
            //}
            //int codeType = 0;
            //if (!int.TryParse(nodes[nodes.Length - 2], out codeType))
            //{
            //    return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            //}
            //if (codeType != (int)Common.EnumFile.ShowCodeType.Dept)
            //{
            //    return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            //}
            //long id = 0;
            //if (!long.TryParse(nodes[nodes.Length - 1], out id))
            //{
            //    return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            //}
            //ShowDept dept = Dal.ShowDeptDAL.GetModel(id);
            //if (dept == null)
            //{
            //    return Content("<script>alert('部门信息不存在！');window.close();</script>");
            //}
            //if (Session["comapny"] == null)
            //{
            //    long companyId = 0;
            //    companyId = dept.CompanyID.Value;
            //    Session["company"] = Dal.ShowCompanyDAL.GetModel(companyId);
            //}
            //ShowCompany company = Session["company"] as ShowCompany;
            //View_EnterprisePlatForm info = Dal.EnterpriseInfoDAL.GetModelView(company.CompanyID);
            //if (info == null)
            //{
            //    return Content("<script>alert('企业信息不存在！');window.close();</script>");
            //}
            //ViewBag.CompanyName = info.EnterpriseName;
            //ViewBag.TemplateIDs = company.TemplateIDs;
            //return View(dept);

            return View();
        }

    }
}
