using System;
using System.Web;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace web.Controllers
{
    public class Wap_ProbeController : Controller
    {
        //
        // GET: /Wap_Probe/
        public ActionResult Login(string ewm)
        {
            ViewBag.ewm = ewm;
            return View();
        }
        [HttpPost]
        public ActionResult Login()
        {
            string loginName = Request["loginname"];
            string pwd = Request["pwd"];
            string ewm = Request["ewm"];
            if (string.IsNullOrEmpty(ewm))
            {
                return Content("<script>alert('请拍码访问页面！');</script>");
            }
            string[] uArray = ewm.Split('.');
            long uId = Convert.ToInt64(uArray[uArray.Length - 1]);
            Enterprise_User user = new BLL.Enterprise_UserBLL().GetEntity(uId);
            if (user.LoginName == loginName && user.LoginPassWord == pwd)
            {
                Common.Argument.LoginInfo loginInfo = new Common.Argument.LoginInfo();
                HttpCookie cookie = new HttpCookie("loginType");
                cookie.Expires = DateTime.Now.AddYears(10);
                cookie.Value = "0";
                Response.AppendCookie(cookie);
                loginInfo.EnterpriseID = user.Enterprise_Info_ID.GetValueOrDefault(1);
                //loginInfo.RoleModual_ID_Array = user.RoleModual_ID_Array;
                loginInfo.PRRU_PlatFormLevel_ID = 1;
                //loginInfo.UserRoleID = user.Enterprise_Role_ID;
                loginInfo.UserType = user.UserType;
                //loginInfo.Modual_ID_Array = Dal.PRRU_PlatFormLevelDAL.GetModel(1).Modual_ID_Array;
                //loginInfo.Parent_ID = (long)user.PRRU_PlatForm_ID;
                //loginInfo.ApprovalCodeType = user.ApprovalCodeType;
                //loginInfo.MainCode = user.MainCode;
                loginInfo.UserID = user.Enterprise_User_ID;
                loginInfo.UserName = user.UserName;
                //loginInfo.EnterpriseName = user.EnterpriseName;
                //loginInfo.RoleName = user.RoleName;
                Common.Argument.SessCokie.Set(loginInfo);
                return RedirectToAction("Index");
            }
            else
            {
                return Content("<script>alert('用户名密码错误！');</script>");
            }
        }
        public ActionResult Index(int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            string bDate = Request["bDate"];
            string eDate = Request["eDate"];
            PagedList<View_Greenhouses_Probe> liGreenBatch = new BLL.GreenhouseBLL().GetProbeListEntity(pf.EnterpriseID, pf.UserID, "", bDate, eDate, pageIndex);
            ViewBag.bDate = bDate;
            ViewBag.eDate = eDate;
            return View(liGreenBatch);
        }
        public ActionResult Info(long gpid, string bDate, string eDate, int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            PagedList<Greenhouses_Probe_Data> liGreenBatch = new BLL.GreenhouseBLL().GetDataListEntity(pf.EnterpriseID, gpid, bDate, eDate, pageIndex);
            ViewBag.bDate = bDate;
            ViewBag.eDate = eDate;
            ViewBag.gpid = gpid;
            return View(liGreenBatch);
        }
    }
}
