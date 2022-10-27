/********************************************************************************

** 作者：张翠霞

** 开发时间：2015-11-17

** 联系方式:15630136020

** 代码功能：主要用于消费者收货地址管理

** 版本：v1.0

** 版权：研一农业项目组   

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using BLL;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 消费者收货地址管理
    /// </summary>
    public class Wap_AddressController : Controller
    {
        //
        // GET: /Order_Consumers_Address/

        /// <summary>
        /// 获取消费者收货地址列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                //return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
                return Content("<script>window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                long order_Consumers_id = Consumers.Order_Consumers_ID;
                //ViewBag.order_Consumers_id = order_Consumers_id;
                ViewBag.ConsumersAddressId = Consumers.Order_Consumers_Address_ID;
                ViewBag.linkPhone = Consumers.LinkPhone;
                //ViewBag.userPhoto = consumers.UserPhoto;
                Order_Consumers_AddressBLL bll = new Order_Consumers_AddressBLL();
                List<Order_Consumers_Address> liConsumersAddress = bll.GetList(order_Consumers_id);
                return View(liConsumersAddress);
            }
        }

        #region 消费者添加收货地址
        /// <summary>
        /// 消费者添加收货地址
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                ViewBag.linkPhone = consumers.LinkPhone;
                //ViewBag.userPhoto = consumers.UserPhoto;
                return View();
            }
        }
        [HttpPost]
        public ActionResult Add(Order_Consumers_Address model)
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                long order_Consumers_id = Consumers.Order_Consumers_ID;
                //long order_Consumers_id = 2;
                model.Order_Consumers_ID = order_Consumers_id;
                model.LinkMan = Request["LinkMan"];
                model.LinkPhone = Request["LinkPhone"];
                model.Postcode = Request["Postcode"];
                model.Address = Request["Address"];
                model.status = (int)Common.EnumFile.Status.used;
                model.ProvinceID = Request["Province"];
                model.AreaID = Request["selArea"];
                model.CityID = Request["selCity"];
                JsonResult js = new JsonResult();
                Order_Consumers_AddressBLL bll = new Order_Consumers_AddressBLL();
                RetResult ret = bll.AddConAddress(model, Request["isDefaultAddress"] != null, ref Consumers);
                if (ret.IsSuccess)
                {
                    #region 重新给Session赋值
                    Consumers.AllArea = Common.Argument.Public.GetAllArea(decimal.Parse(Consumers.AreaID ?? "0"));
                    Common.Argument.SessCokieOrder.Logout();
                    Common.Argument.SessCokieOrder.Set(Consumers);
                    #endregion
                    js.Data = new { res = true, info = ret.Msg, url = "/Wap_Address/Index" };
                }
                else
                {
                    js.Data = new { res = false, info = ret.Msg };
                }
                return js;
            }
        }
        #endregion

        #region 消费者修改收货地址
        /// <summary>
        /// 消费者修改收货地址
        /// </summary>
        /// <param name="consumerAdressId"></param>
        /// <returns></returns>
        public ActionResult Edit(long consumerAdressId, string Random)
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                Order_Consumers_AddressBLL bll = new Order_Consumers_AddressBLL();
                Order_Consumers_Address model = bll.GetModel(consumerAdressId);
                if (model == null)
                {
                    return Content("<script>alert('获取数据失败');history.go(-1);</script>");
                }
                ViewBag.linkPhone = Consumers.LinkPhone;
                ViewBag.defaultId = Consumers.Order_Consumers_Address_ID;
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult Edit(Order_Consumers_Address model)
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                long order_Consumers_id = Consumers.Order_Consumers_ID;
                model.Order_Consumers_ID = order_Consumers_id;
                model.Order_Consumers_Address_ID = Convert.ToInt64(Request["consumerAdressId"]);
                model.LinkMan = Request["LinkMan"];
                model.LinkPhone = Request["LinkPhone"];
                model.Postcode = Request["Postcode"];
                model.Address = Request["Address"];
                model.status = (int)Common.EnumFile.Status.used;
                model.ProvinceID = Request["Province"];
                model.AreaID = Request["selArea"];
                model.CityID = Request["selCity"];
                JsonResult js = new JsonResult();
                Order_Consumers_AddressBLL bll = new Order_Consumers_AddressBLL();
                RetResult ret = bll.EditConAddress(model, Request["isDefaultAddress"] != null, ref Consumers);
                if (ret.IsSuccess)
                {
                    #region 重新给Session赋值
                    Consumers.AllArea = Common.Argument.Public.GetAllArea(decimal.Parse(Consumers.AreaID ?? "0"));
                    Common.Argument.SessCokieOrder.Logout();
                    Common.Argument.SessCokieOrder.Set(Consumers);
                    #endregion
                    js.Data = new { res = true, info = ret.Msg, url = "/Wap_Address/Index" };
                }
                else
                {
                    js.Data = new { res = false, info = ret.Msg };
                }
                return js;
            }
        }
        #endregion

        /// <summary>
        /// 消费者删除收货地址
        /// </summary>
        /// <param name="consumerAdressId"></param>
        /// <returns></returns>
        public ActionResult Delete(long consumerAdressId)
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                Order_Consumers_AddressBLL bll = new Order_Consumers_AddressBLL();
                RetResult ret = bll.Delete(consumerAdressId, Consumers.Order_Consumers_ID);
                JsonResult js = new JsonResult();
                if (ret.IsSuccess)
                {
                    js.Data = new { res = true, info = ret.Msg, url = "/Wap_Address/Index" };
                }
                else
                {
                    js.Data = new { res = false, info = ret.Msg };
                }
                return js;
            }
        }
    }
}
