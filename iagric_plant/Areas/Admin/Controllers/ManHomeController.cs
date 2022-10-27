/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-8-9
** 联系方式:15533621896
** 代码功能：管理员后台
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Web.Mvc;
using iagric_plant.Areas.Admin.Filter;
using BLL;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 主页
    /// </summary>
    [ManAuthorizeAttribute]
    public class ManHomeController : Controller
    {
        /// <summary>
        /// 主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 框架页
        /// </summary>
        /// <returns></returns>
        public ActionResult MainFrame()
        {
            return View();
        }

        /// <summary>
        /// 左菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Left()
        {
            var modelLst = new LoginBLL().GetSysNewModuleList(-1, Common.Argument.SessCokie.GetMan.Modual_ID_Array);
            return View(modelLst);
        }

        /// <summary>
        /// 右菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Top()
        {
            return View();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit()
        {
            if (Common.Argument.SessCokie.GetMan != null)
            {
                Common.Argument.SessCokie.SetMan(null);
            }
            return Content("<script>top.location.href='/Admin'</script>");
        }
    }
}
