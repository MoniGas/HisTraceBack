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
    public class SysJYEnterpriseController : Controller
    {
        //
        // GET: /Admin/SysJYEnterprise/

        public ActionResult Index(int? id)
        {
            string sName = Request["comName"];
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            PagedList<View_DealerUser> list = bll.GetJYEnterpriseList(sName, pageIndex);
            return View(list);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddJYEn(string dealerName,string diCount, string loginName, string pwd, string address, string linkMan, string linkPhone)
        {
            JsonResult js = new JsonResult();
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            LoginInfo user = SessCokie.GetMan;
            if (string.IsNullOrEmpty(dealerName))
            {
                js.Data = new { res = "-1", info = "经营企业名称不能为空" };
                return js;
            }
            Dealer dealer = new Dealer();
            dealer.adddate = DateTime.Now;
            dealer.Address = address;
            dealer.adduser = user.UserID;
            dealer.DealerLevel = 101;
            dealer.DealerName = dealerName;
            dealer.Enterprise_Info_ID = 0;
            dealer.lastdate = DateTime.Now;
            dealer.lastuser = user.UserID;
            dealer.Person = linkMan;
            dealer.Phone = linkPhone;
            dealer.Status = (int)Common.EnumFile.Status.used;
            if (!string.IsNullOrEmpty(diCount))
            {
                dealer.DICount = Convert.ToInt64(diCount);
            }
            else
            {
                dealer.DICount = 0;
            }
            Enterprise_User dealerUser = new Enterprise_User();
            dealerUser.UserName = "经营企业";
            dealerUser.UserType = "经营企业";
            dealerUser.LoginName = loginName;
            dealerUser.LoginPassWord = pwd;
            dealerUser.Status = (int)Common.EnumFile.Status.used;
            dealerUser.adddate = DateTime.Now;
            dealerUser.adduser = user.UserID;
            dealerUser.Enterprise_Info_ID = 0;
            dealerUser.lastdate = DateTime.Now;
            dealerUser.lastuser = user.UserID;
            dealerUser.UserPhone = linkPhone;

            LinqModel.BaseResultModel result = bll.Add(dealer, dealerUser);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        public ActionResult Edit(long dId)
        {
            View_DealerUser result = new View_DealerUser();
            try
            {
                SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
                result = bll.GetModelInfo(dId);
            }
            catch (Exception ex)
            {
            }
            return View(result);
        }
        [HttpPost]
        public JsonResult EditJYEn(long dId, string dealerName, string diCount, string loginName, string pwd, string address, string linkMan, string linkPhone)
        {
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            LoginInfo user = SessCokie.GetMan;
            JsonResult js = new JsonResult();
            Dealer dealer = new Dealer();
            dealer.Dealer_ID = dId;
            dealer.DealerName = dealerName;
            dealer.Address = address;
            dealer.Person = linkMan;
            dealer.Phone = linkPhone;
            dealer.lastdate = DateTime.Now;
            dealer.lastuser = user.UserID;
            if (!string.IsNullOrEmpty(diCount))
            {
                dealer.DICount = Convert.ToInt64(diCount);
            }
            else
            {
                dealer.DICount = 0;
            }
            LinqModel.BaseResultModel result = bll.Edit(dealer, loginName, pwd);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <param name="type">1：启用；2：禁用</param>
        /// <returns></returns>
        public ActionResult EditStatus(long id, int type)
        {
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            RetResult ret = bll.EditJYEnStatus(id, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/SysJYEnterprise/Index" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
    }
}
