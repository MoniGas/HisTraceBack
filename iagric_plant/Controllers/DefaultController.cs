/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-19

** 联系方式 :13313318725

** 描述：主要用于帮助管理控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System.Web.Mvc;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using BLL;
using Common.Argument;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 主要用于帮助管理控制器
    /// </summary>
    public class DefaultController : Controller
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Bottom()
        {
            return View();
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="typeId">帮助类型typeId</param>
        /// <param name="id">页码</param>
        /// <returns>操作结果</returns>
        public ActionResult HelpList(long typeId, int? id = 1)
        {
            PagedList<View_Help> dataList = new BLL.HelpBLL().GetPagedList(typeId, id);
            return View(dataList);
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="typeId">帮助类型typeId</param>
        /// <param name="index">用于记录查看更多次数</param>
        /// <param name="id">页码</param>
        /// <returns>操作结果</returns>
        public ActionResult GetHelpList(long typeId, int index, int? id = 1)
        {
            PagedList<View_Help> dataList = new BLL.HelpBLL().GetMoreList(typeId, index, "", id);
            return Json(dataList);
        }

        public ActionResult UpdateInfo()
        {
            return View();
        }

        /// <summary>
        /// 搜索获取帮助列表
        /// </summary>
        /// <param name="typeId">帮助类型typeId</param>
        /// <param name="index">用于记录查看更多次数</param>
        /// <param name="name">帮助名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>操作结果</returns>
        public ActionResult GetSearchList(long typeId, int index, string name, int? id = 1)
        {
            PagedList<View_Help> dataList = new BLL.HelpBLL().GetMoreList(typeId, index,name, id);
            return Json(dataList);
        }

        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="typeId">帮助类型typeId</param>
        /// <param name="helpId">帮助id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public ActionResult HelpDetails(long typeId, long helpId, int? id = 1)
        {
            PagedList<View_Help> dataList = new BLL.HelpBLL().GetPagedList(typeId, id);
            ViewData["TypeId"] = typeId;
            ViewData["HelpId"] = helpId;
            return View(dataList);
        }

        /// <summary>
        /// 设置访问量
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateUsefulCount(int type,long helpId)
        {
            JsonResult js = new JsonResult();
            HelpBLL bll = new HelpBLL();
            RetResult ret = bll.UpdateUsefulCount(type,helpId);
            if (ret.IsSuccess)
            {
                //js.Data = new { res = true, info = ret.Msg, url = "/Default/HelpDetails" };
                js.Data = new { res = true, info = ret.Msg};
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        public ActionResult GetDetails( long helpId)
        {
            View_Help model = new BLL.HelpBLL().GetDetails(helpId);
            return Json(model);
        }

        public ActionResult Video()
        {
            return View();
        }
    }
}
