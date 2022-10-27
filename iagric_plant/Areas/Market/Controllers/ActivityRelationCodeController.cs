/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-14
** 联系方式:13313318725
** 代码功能：活动关联二维码操作
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using BLL;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using MarketActive.Filter;

namespace MarketActive.Controllers
{
    [AdminAuthorize]
    public class ActivityRelationCodeController : Controller
    {
        /// <summary>
        /// 获取关联活动的二维码
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回列表</returns>
        public ActionResult Index(int? id)
        {
            LoginInfo user = SessCokie.Get;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            ActivityRelationCodeBLL bll = new ActivityRelationCodeBLL();
            PagedList<YX_ActvitiyRelationCode> list = bll.GetList(user.EnterpriseID, pageIndex);
            return View(list);
        }

        /// <summary>
        /// 查看关联活动
        /// </summary>
        /// <param name="id">关联ID</param>
        /// <returns></returns>
        public ActionResult Info(long id)
        {
            ActivityRelationCodeBLL bll = new ActivityRelationCodeBLL();
            View_ActvitiyRelationCode info = bll.Info(id);
            return View(info);
        }
    }
}
