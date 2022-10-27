/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-8-9
** 联系方式:15533621896
** 代码功能：管理员后台
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;

namespace iagric_plant.Areas.Admin.Filter
{
    /// <summary>
    /// 管理员登录验证
    /// </summary>
    public class ManAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 重写过滤方法
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute),true))
            {
                if (Common.Argument.SessCokie.GetMan==null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult() { Data = new { ok = false, url = "/Admin", msg = "登录超时，请重新登录！" } };
                    }
                    else
                    {
                        filterContext.Result = new ContentResult() { Content = "<script>alert('登录超时，请重新登录！');top.location.href=\"/Admin\"</script>" };
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AllowAnonymousAttribute : Attribute
    {
        public AllowAnonymousAttribute()
        {
        }
    }
}