/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-09-14
** 联系方式:13313318725
** 代码功能：消费者积分查询管理
** 版本：v1.0
** 版权：项目二部   
*********************************************************************************/

using System.Collections.Generic;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using BLL;

namespace iagric_plant.Controllers
{
    public class Wap_IntegralController : Controller
    {
        //
        // GET: /Wap_Integral/

        /// <summary>
        /// 消费者积分页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                return Content("<script>window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                long order_Consumers_id = Consumers.Order_Consumers_ID;
                ViewBag.OrderConsumersId = Consumers.Order_Consumers_ID;
                OrderIntegralBLL bll = new OrderIntegralBLL();
                Order_Consumers consumersIntegral = bll.GetConIntegral(order_Consumers_id);
                ViewBag.IntegralCount = consumersIntegral.IntegralCount;
                List<Order_Integral> integralList = bll.GetModelList(order_Consumers_id);
                Order_Integral integralModel = new Order_Integral();
                if (integralList.Count > 0)
                {
                    integralModel = integralList[0];
                }
                ViewBag.integralModel = integralModel;
            }
            return View();
        }

        /// <summary>
        /// 积分说明页面
        /// </summary>
        /// <returns></returns>
        public ActionResult IntegralShuoM()
        {
            return View();
        }

        /// <summary>
        /// 积分明细查询
        /// </summary>
        /// <param name="OrderConsumersId">消费者ID</param>
        /// <returns></returns>
        public ActionResult IntegralDetail(long OrderConsumersId)
        {
            OrderIntegralBLL bll = new OrderIntegralBLL();
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            ViewBag.OrderConsumersId = OrderConsumersId;
            List<Order_Integral> integralList = bll.GetList(OrderConsumersId, sDate, eDate);
            return View(integralList);
        }
    }
}
