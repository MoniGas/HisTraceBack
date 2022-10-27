using System.Web.Mvc;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace iagric_plant.Controllers
{
    public class Wap_NewsController : Controller
    {
        public ActionResult Index()
        {
            string ewm = Request["ewm"];
            if (string.IsNullOrEmpty(Request["ewm"]))
            {
                return Content("<script>alert('请拍码访问此页面！');window.close();</script>");
            }
            string[] nodes = ewm.Split('.');
            if (!(nodes.Length == 8 || nodes.Length == 7))
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            int codeType = 0;
            if (!int.TryParse(nodes[nodes.Length - 2], out codeType))
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            if (codeType != (int)Common.EnumFile.ShowCodeType.News)
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            long id = 0;
            if (!long.TryParse(nodes[nodes.Length - 1], out id))
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            View_NewsChannel news = new View_NewsChannel();
            if (Session["comapny"] == null)
            {
                news = new BLL.ShowNewsBLL().GetModel(ewm).ObjModel as View_NewsChannel;
                if (news == null)
                {
                    return Content("<script>alert('二维码不存在！');window.close();</script>");
                }
                long companyId = news.CompanyID.Value;
                Session["company"] = new BLL.ShowCompanyBLL().GetModel(companyId).ObjModel as ShowCompany;
            }
            return RedirectToAction("NewsShow/" + news.ID, "Wap_News");
        }

        public ActionResult NewsList(int? id = 1)
        {
            ShowCompany company = Session["company"] as ShowCompany;
            if (company == null)
            {
                return Content("<script>alert('企业信息不存在！');window.close();</script>");
            }
            ViewBag.TemplateIDs = company.TemplateIDs;
            ViewBag.ewm = company.EWM;
            ViewBag.CompanyName = company.CompanyName;

            long cId = long.Parse(Request["channelId"]);
            string channelName = Request["channelName"];
            PagedList<View_NewsChannel> liNews = new BLL.ShowNewsBLL().GetPagedList(company.CompanyID, cId, id);
            //if (liNews != null && liNews.Count == 1)
            //{
            //    return RedirectToAction("NewsShow/" + liNews[0].ID, "Wap_News");
            //}
            ViewBag.ChannelName = channelName;
            return View(liNews);
        }
        public ActionResult NewsShow(long id,string ewm)
        {
            ShowCompany company = Session["company"] as ShowCompany;
            if (company == null)
            {
                return Content("<script>alert('企业信息不存在！');window.close();</script>");
            }
            ViewBag.TemplateIDs = company.TemplateIDs;
            ViewBag.ewm = ewm;
            ViewBag.CompanyName = company.CompanyName;

            View_NewsChannel news = new BLL.ShowNewsBLL().GetModel(id).ObjModel as View_NewsChannel;
            ViewBag.ChannelID = news.ChannelID;
            return View(news);
        }
    }
}
