/********************************************************************************
** 作者：赵慧敏
** 开发时间：2017-6-7
** 联系方式:13313318725
** 代码功能：公共类
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using System.Web.Mvc;
using LinqModel;
using BLL;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 公共类
    /// </summary>
    public class PublicController : Controller
    {
        /// <summary>
        /// 模板类型
        /// </summary>
        /// <param name="ModuleType">模板vaule</param>
        /// <param name="FirstDisplay"></param>
        /// <returns></returns>
        public ActionResult Style(long ParentID, string FirstDisplay = "")
        {
            PublicBLL bll = new PublicBLL();
            List<YX_Style> ModuleTypeList = bll.GetStyle(0);
            List<SelectListItem> Result = new List<SelectListItem>();
            foreach (var item in ModuleTypeList)
            {
                SelectListItem ListItem = new SelectListItem();
                ListItem.Value = item.ActivityStyeID.ToString();
                ListItem.Text = item.StyleName;
                if (item.ActivityStyeID == ParentID)
                    ListItem.Selected = true;
                else
                    ListItem.Selected = false;
                Result.Add(ListItem);
            }
            return View(Result);
        }
    }
}
