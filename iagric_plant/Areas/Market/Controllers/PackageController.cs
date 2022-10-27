/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-5
** 联系方式:13313318725
** 代码功能：二维码套餐管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;

namespace MarketActive.Controllers
{
    public class PackageController : Controller
    {
        //
        // GET: /Package/
        /// <summary>
        /// 获取二维码套餐列表
        /// </summary>
        /// <param name="id">分页</param>
        /// <returns></returns>
        public ActionResult AdminIndex(int? id)
        {
            string acName = Request["tcName"];
            ViewBag.Name = acName;
            string status = Request["status"] ?? "0";
            ViewBag.Status = status;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            PackageBLL bll = new PackageBLL();
            PagedList<YX_Package> list = bll.GetList(acName, Convert.ToInt32(status), pageIndex);
            return View(list);
        }

        /// <summary>
        /// 添加二维码套餐
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 添加二维码套餐提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(YX_Package model)
        {
            model.PackbageName = Request["tcName"];
            model.PackbageCodeCount = Convert.ToInt64(Request["amount"]);
            model.PackagePrice = Convert.ToDouble(Request["sum"]);
            model.PackageStatus = (int)Common.EnumText.PackageStatus.qiyong;
            PackageBLL bll = new PackageBLL();
            RetResult ret = bll.AddModel(model);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Market/Package/AdminIndex" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 修改二维码套餐
        /// </summary>
        /// <param name="id">套餐ID</param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            PackageBLL bll = new PackageBLL();
            YX_Package model = bll.GetModel(id);
            return View(model);
        }
    }
}
