/*****************************************************************
代码功能：身份验证特性
开发日期：2017年6月06日
作    者：苏凯丽
联系方式：15533621896
版权所有：追溯   
******************************************************************/

using System.Web.Mvc;
using Common.Argument;

namespace MarketActive.Filter
{
    /// <summary>
    /// 身份验证特性
    /// </summary>
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 重写操作执行前方法
        /// </summary>
        /// <param name="filterContext">上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                LoginInfo loginInfo = SessCokie.Get;
                if (loginInfo == null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult() { Data = new { ok = false, url = "/Market/HomeMarket/Exit", msg = "登录超时，请重新登录！" } };
                    }
                    else
                    {
                        filterContext.Result = new ContentResult() { Content = "<script>alert('登录超时，请重新登录！');parent.parent.parent.location.href=\"/Home/Index\"</script>" };
                    }
                }
            }
        }
    }

    /// <summary>
    /// 监管部门
    /// </summary>
    public class AdminAuthorizeAdminAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 重写操作执行前方法
        /// </summary>
        /// <param name="filterContext">上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AdminCurrentUser.AdminUser == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult() { Data = new { ok = false, url = "/Market/HomeMarket/AdminExit", msg = "登录超时，请重新登录！" } };
                }
                else
                {
                    filterContext.Result = new ContentResult() { Content = "<script>alert('登录超时，请重新登录！');parent.parent.parent.location.href=\"/market/Loginmarket/AdminLogin\"</script>" };
                }
            }
        }
    }
}