/********************************************************************************
** 作者：赵慧敏
** 开发时间：2017-6-7
** 联系方式:13313318725
** 代码功能：用于系统登录
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Web.Mvc;
using Common.Argument;
using BLL;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 系统登录控制器
    /// </summary>
    public class LoginMarketController : Controller
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="loginPass">登录密码</param>
        [HttpPost]
        public JsonResult Login(string loginName, string loginPass, string yzCode)
        {
            RetResult result = new RetResult();
            LoginMarketBLL bll = new LoginMarketBLL();
            JsonResult js = new JsonResult();
            if (Session["CheckCode"] != null && yzCode == Session["CheckCode"].ToString())
            {
                result = bll.EnterpriseLogin(loginName, loginPass);
                if (result.IsSuccess)
                {
                    js.Data = new { res = true, info = result.Msg, url = "/Market/Home/MainFrame" };
                }
                else
                {
                    js.Data = new { res = false, info = result.Msg };
                }
            }
            else
            {
                js.Data = new { res = false, info = "验证码错误！" };
            }
            return js;
        }

        /// <summary>
        /// 监管登录
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminLogin()
        {
            return View();
        }
        /// <summary>
        /// 监管登录方法
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="loginPass">登录密码</param>
        [HttpPost]
        public JsonResult AdminLogin(string loginName, string loginPass, string yzCode)
        {
            RetResult result = new RetResult();
            LoginMarketBLL bll = new LoginMarketBLL();
            JsonResult js = new JsonResult();
            if (Session["CheckCode"] != null && yzCode == Session["CheckCode"].ToString())
            {
                result = bll.AdminLogin(loginName, loginPass);
                if (result.IsSuccess)
                {
                    js.Data = new { res = true, info = result.Msg, url = "/Market/HomeMarket/AdminMainFrame" };
                }
                else
                {
                    js.Data = new { res = false, info = result.Msg };
                }
            }
            else
            {
                js.Data = new { res = false, info = "验证码错误！" };
            }
            return js;
        }
    }
}
