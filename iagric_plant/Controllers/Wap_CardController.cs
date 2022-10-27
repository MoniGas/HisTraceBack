using System.Web.Mvc;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Wap_CardController : Controller
    {
        //
        // GET: /Wap_Card/

        public ActionResult Index()
        {
            string ewm = Request["ewm"];
            if (string.IsNullOrEmpty(Request["ewm"]))
            {
                return Content("<script>alert('请拍码访问此页面！');window.close();</script>");
            }
            string[] nodes = ewm.Split('.');
            if (nodes.Length !=6)
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            int codeType = 0;
            if (!int.TryParse(nodes[nodes.Length - 2], out codeType))
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            if (codeType != (int)Common.EnumFile.ShowCodeType.User)
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            long id = 0;
            if (!long.TryParse(nodes[nodes.Length - 1], out id))
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            ShowUser user = new BLL.ShowUserBLL().GetModel(id).ObjModel as ShowUser;
            if (user == null)
            {
                return Content("<script>alert('信息不存在！');window.close();</script>");
            }
            if (Session["comapny"] == null)
            {
                long companyId = 0;
                companyId = user.CompanyID.Value;
                Session["company"] = new BLL.ShowCompanyBLL().GetModel(companyId).ObjModel as ShowCompany;
            }
            ShowCompany company = Session["company"] as ShowCompany;
            View_EnterprisePlatForm info = new BLL.EnterpriseInfoBLL().GetModelView(company.CompanyID).ObjModel as View_EnterprisePlatForm;
            if (info == null)
            {
                return Content("<script>alert('企业信息不存在！');window.close();</script>");
            }

            if (user != null)
            {
                try
                {
                    ViewBag.uId = user.UserID;
                    ViewBag.name = user.Infos;
                    ViewBag.position = user.position;
                    ViewBag.telPhone = user.telPhone;
                    ViewBag.mail = user.mail;
                    ViewBag.qq = user.qq;
                    ViewBag.hometown = user.hometown;
                    ViewBag.location = user.location;
                    ViewBag.memo = user.memo;
                    ViewBag.headimg = user.headimg;
                }
                catch
                {
                }
            }
            ViewBag.mainewm = company.EWM;
            ViewBag.CompanyName = info.CompanyName;
            return View(info);
        }

    }
}
