/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-10
** 联系方式:13313318725
** 代码功能：二维码订单管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using BLL;

namespace MarketActive.Controllers
{
    public class BuyCodeOrderController : Controller
    {
        //
        // GET: /BuyCodeOrder/
        /// <summary>
        /// 获取二维码未审核的订单列表
        /// </summary>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult AdminIndex(int? id)
        {
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            BuyCodeOrderBLL bll = new BuyCodeOrderBLL();
            PagedList<View_BuyCodeOrderEnterprise> list = bll.GetList(pageIndex);
            //if (list.Count > 0)
            //{
            //    foreach (var item in list)
            //    {
            //        Enterprise_Info comModel = bll.GetComModel(item.CompanyID.Value);
            //        if (comModel != null)
            //        {
            //            ViewBag.CompanyName = comModel.EnterpriseName;
            //        }
            //        else
            //        {
            //            ViewBag.CompanyName = "";
            //        }
            //    }
            //}
            return View(list);
        }

        #region 查询订单
        /// <summary>
        /// 查询全部订单
        /// </summary>
        /// <param name="id">页面</param>
        /// <returns></returns>
        public ActionResult SeachAllIndex(int? id)
        {
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            string comName = Request["comName"];
            ViewBag.Name = comName;
            string status = Request["status"] ?? "0";
            ViewBag.Status = status;
            BuyCodeOrderBLL bll = new BuyCodeOrderBLL();
            PagedList<View_BuyCodeOrderEnterprise> list = bll.GetAllList(comName, Convert.ToInt32(status), pageIndex);
            return View(list);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        public ActionResult InfoA(long id)
        {
            BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
            YX_BuyCodeOrdePay info = bll.GetOrderPayInfoA(id);
            return View(info);
        }
        #endregion
    }
}
