/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-11
** 联系方式:13313318725
** 代码功能：红包充值记录管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;

namespace MarketActive.Controllers
{
    public class RedRechargeController : Controller
    {
        //
        // GET: /RedRecharge/
        /// <summary>
        /// 获取二维码未审核的订单列表
        /// </summary>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult AdminIndex(int? id)
        {
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            string comName = Request["comName"];
            ViewBag.Name = comName;
            RedRechargeBLL bll = new RedRechargeBLL();
            PagedList<View_RedRecharge> list = bll.GetList(comName, pageIndex);
            return View(list);
        }

        /// <summary>
        /// 红包充值
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 红包充值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(YX_RedRecharge model)
        {
            BuyCodeOrderBLL buybll = new BuyCodeOrderBLL();
            Enterprise_Info enmodel = buybll.GetComModel(model.CompanyID.Value);
            if (enmodel != null)
            {
                model.PayMan = enmodel.EnterpriseName;
            }
            model.CollectMan = "河北广联";
            model.RechargeValue = Convert.ToDouble((Request["sum"]));
            model.CreateDate = DateTime.Now;
            model.RechargeMode = (int)Common.EnumText.PayType.OffLinePay;
            RedRechargeBLL bll = new RedRechargeBLL();
            RetResult ret = bll.AddModel(model);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Market/RedRecharge/AdminIndex" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public ActionResult GetEnList(long? id)
        {
            List<SelectListItem> eid = new List<SelectListItem>();
            SelectListItem select = new SelectListItem();
            select.Text = "请选择";
            select.Value = "0";
            select.Selected = true;
            eid.Add(select);
            List<Enterprise_Info> liEn = new List<Enterprise_Info>();
            RedRechargeBLL bll = new RedRechargeBLL();
            liEn = bll.GetEnList(1);
            return View(liEn);
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetEnModel(long id)
        {
            BuyCodeOrderBLL buybll = new BuyCodeOrderBLL();
            Enterprise_Info enmodel = buybll.GetComModel(id);
            return Json(new { linkman = enmodel.LinkMan, linkphone = enmodel.LinkPhone });
        }

        /// <summary>
        /// 企业帐户查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CompanyMoney(int? id)
        {
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            string comName = Request["comName"];
            ViewBag.Name = comName;
            RedRechargeBLL bll = new RedRechargeBLL();
            PagedList<View_CompanyMoney> list = bll.GetListMoney(comName, pageIndex);
            return View(list);
        }
    }
}
