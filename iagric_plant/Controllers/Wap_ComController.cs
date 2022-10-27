using System.Collections.Generic;
using System.Web.Mvc;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Wap_ComController : Controller
    {
        //
        // GET: /Wap_Com/

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
            if (codeType != (int)Common.EnumFile.ShowCodeType.Company)
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            long id = 0;
            if (!long.TryParse(nodes[nodes.Length - 1], out id))
            {
                return Content("<script>alert('二维码格式不正确！');window.close();</script>");
            }
            if (Session["comapny"] == null)
            {
                long companyId = id;
                Session["company"] = new BLL.ShowCompanyBLL().GetModel(companyId).ObjModel as LinqModel.ShowCompany;
            }
            LinqModel.ShowCompany company = Session["company"] as LinqModel.ShowCompany;
            LinqModel.View_EnterprisePlatForm info = new BLL.EnterpriseInfoBLL().GetModelView(company.CompanyID).ObjModel as LinqModel.View_EnterprisePlatForm;
            if (info == null)
            {
                return Content("<script>alert('企业信息不存在！');window.close();</script>");
            }
            ViewBag.CompanyName = info.EnterpriseName;
            ViewBag.TemplateIDs = company.TemplateIDs;
            ViewBag.ewm = ewm;
            return View();
        }

        public ActionResult Info()
        {
            string ewm = Request["ewm"];
            string channelName = Request["channelName"];
            ShowCompany company = Session["company"] as ShowCompany;
            ViewBag.TemplateIDs = company.TemplateIDs;
            ViewBag.CompanyName = company.CompanyName;
            ViewBag.ChannelName = channelName;
            ViewBag.ewm = ewm;
            return View(company);
        }

        /// <summary>
        /// 企业信息追溯页
        /// </summary>
        /// <returns></returns>
        public ActionResult Company()
        {
            string Ewm = Request["ewm"];
            if (string.IsNullOrEmpty(Request["ewm"]))
            {
                return Content("<script>alert('请拍码访问此页面！');</script>");
            }
            string[] Nodes = Ewm.Split('.');
            if (Nodes.Length != 6)
            {
                return Content("<script>alert('二维码格式不正确！');</script>");
            }
            int codeType = 0;
            if (!int.TryParse(Nodes[Nodes.Length - 2], out codeType))
            {
                return Content("<script>alert('二维码格式不正确！');</script>");
            }
            if (codeType != (int)Common.EnumFile.ShowCodeType.Company)
            {
                return Content("<script>alert('二维码格式不正确！');</script>");
            }
            long CompanyId = 0;
            //判断最后一位是否为企业ID
            if (!long.TryParse(Nodes[Nodes.Length - 1], out CompanyId))
            {
                return Content("<script>alert('二维码格式不正确！');</script>");
            }
            View_EnterpriseShow EnterpriseShowModel = new BLL.Enterprise_ShowBLL().GetModelView(CompanyId).ObjModel as View_EnterpriseShow;

            if (EnterpriseShowModel == null)
            {
                return Content("<script>alert('二维码错误，不是该平台的二维码！');</script>");
            }

            //Session["Enterprise"] = EnterpriseShowModel;
            //Session["List"] = new BLL.ConfigureBLL().GetModel(CompanyId).ObjModel as Configure;
            //Session["BrandList"] = new BLL.ConfigureBLL().GetBrandList(CompanyId).ObjList as List<Brand>;
            //Session["UserList"] = new BLL.ConfigureBLL().GetUserList(CompanyId).ObjList as List<ShowUser>;
            //Session["NewsList"] = new BLL.ConfigureBLL().GetNewsList(CompanyId).ObjList as List<ShowNews>;
            Configure List = new BLL.ConfigureBLL().GetModel(CompanyId).ObjModel as Configure;
            //判断Configure是否为空
            if (List == null)
            {
                return Content("<script>alert('请先进行企业宣传码配置！');</script>");
            }
            List<Brand> BrandList = new BLL.ConfigureBLL().GetBrandList(CompanyId).ObjList as List<Brand>;
            List<ShowUser> UserList = new BLL.ConfigureBLL().GetUserList(CompanyId).ObjList as List<ShowUser>;
            List<ShowNews> NewsList = new BLL.ConfigureBLL().GetNewsList(CompanyId).ObjList as List<ShowNews>;
            ViewBag.BrandList = BrandList;
            ViewBag.UserList = UserList;
            ViewBag.NewsList = NewsList;
            ViewBag.Company = EnterpriseShowModel;
            if (EnterpriseShowModel.imgs.Count != 0)
            {
                ViewBag.Logo = EnterpriseShowModel.imgs[0].fileUrls;
            }
            ViewBag.Brand = List.Brand_ID_Array.Split(',');
            ViewBag.User = List.User_ID_Array.Split(',');
            ViewBag.News = List.News_ID_Array.Split(',');
            ViewBag.ewm = Ewm;
            return View();
        }

        /// <summary>
        /// 企业品牌展示
        /// </summary>
        /// <param name="BrandID">品牌ID</param>
        /// <returns></returns>
        public ActionResult BrandShow(long BrandID)
        {
            Brand ObjBrandModel = new BLL.ConfigureBLL().GetBrandModel(BrandID);
            return View(ObjBrandModel);
        }

        /// <summary>
        /// 企业员工展示
        /// </summary>
        /// <param name="UserID">员工ID</param>
        /// <returns></returns>
        public ActionResult UserShow(long UserID)
        {
            ShowUser ObjUserModel = new BLL.ConfigureBLL().GetUserModel(UserID);
            return View(ObjUserModel);
        }

        /// <summary>
        /// 企业新闻展示
        /// </summary>
        /// <param name="NewsID">新闻ID</param>
        /// <returns></returns>
        public ActionResult ShowNews(long NewsID)
        {
            ShowNews ObjNewsModel = new BLL.ConfigureBLL().GetNewsModel(NewsID);
            return View(ObjNewsModel);
        }
    }
}
