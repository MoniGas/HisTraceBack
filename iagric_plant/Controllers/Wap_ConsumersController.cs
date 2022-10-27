using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace iagric_plant.Controllers
{
    public class Wap_ConsumersController : Controller
    {
        //
        // GET: /Wap_Consumers/

        /// <summary>
        /// 订单列表页面
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns>视图</returns>
        public ActionResult Index(int pageIndex = 1)
        {
            View_Order_Consumers pf = Common.Argument.SessCokieOrder.Get;
            if (pf == null)
            {
                //return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
                return Content("<script>window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                OrderIntegralBLL bll = new OrderIntegralBLL();
                Order_Consumers consumersIntegral = bll.GetConIntegral(pf.Order_Consumers_ID);
                ViewBag.IntegralCount = consumersIntegral.IntegralCount;
                string OrderType = (Request["OrderType"] ?? "0");
                OrderType = string.IsNullOrEmpty(OrderType) ? "0" : OrderType;
                ViewBag.consumers = pf;
                ViewBag.OrderStatus = OrderType;
                ViewBag.TotalMoney = new ScanCodeBLL().GetSendChange(SessCokieOrder.Get.Order_Consumers_ID).Sum(a => a.RedValue);
                PagedList<View_Material_OnlineOrder> liOrder = new BLL.Material_OnlineOrderBLL().GetList(pf.Order_Consumers_ID, int.Parse(OrderType ?? "0"), pageIndex, 10);
                return View(liOrder);
            }
        }

        #region 消费者修改密码（输入原密码）
        /// <summary>
        /// 消费修改密码（输入原密码修改）
        /// </summary>
        /// <param name="consumersId">消费者ID</param>
        /// <returns></returns>
        public ActionResult EditPwd(long consumersId)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                //consumersId = consumers.Order_Consumers_ID;
                //ViewBag.consumersId = 2;
                ViewBag.consumersId = consumersId;
                ViewBag.linkPhone = consumers.LinkPhone;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditPwd(string oldPwd, string newPwd)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                long consumersId = Convert.ToInt64(Request["TxtConsumersId"]);
                oldPwd = Request["TxtOldPwd"];
                newPwd = Request["TxtNewPwd"];
                OrderConsumersBLL bll = new OrderConsumersBLL();
                Result ret = bll.EditPwd(consumersId, oldPwd, newPwd);
                string url = "/Wap_Order/Login?pageType=3";
                JsonResult js = new JsonResult();
                if (ret.ResultCode == 1)
                {
                    js.Data = new { res = true, info = ret.ResultMsg, url = url };
                    Common.Argument.SessCokieOrder.Logout();
                }
                else
                {
                    js.Data = new { res = false, info = ret.ResultMsg };
                }
                return js;
            }
        }
        #endregion

        #region 消费者修改密码（忘记原密码）
        /// <summary>
        /// 消费修改密码（忘记原密码修改）
        /// </summary>
        /// <param name="consumersId">消费者ID</param>
        /// <returns></returns>
        public ActionResult UpdataPwd(long consumersId)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                ViewBag.consumersId = consumersId;
                ViewBag.linkPhone = consumers.LinkPhone;
                return View();
            }
        }
        [HttpPost]
        public ActionResult UpdataPwd(string pwd, string surePwd)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                long consumersId = Convert.ToInt64(Request["TxtConsumersId"]);
                pwd = Request["TxtPwd"];
                OrderConsumersBLL bll = new OrderConsumersBLL();
                Result ret = bll.UpdataPwd(consumersId, pwd);
                string url = "/Wap_Order/Login?pageType=3";
                JsonResult js = new JsonResult();
                if (ret.ResultCode == 1)
                {
                    js.Data = new { res = true, info = ret.ResultMsg, url = url };
                    Common.Argument.SessCokieOrder.Logout();
                }
                else
                {
                    js.Data = new { res = false, info = ret.ResultMsg };
                }
                return js;
            }
        }
        #endregion

        public Enterprise_FWCode_00 GetSession()
        {
            Enterprise_FWCode_00 result = null;
            result = (Session["code"] as CodeInfo).FwCode;
            return result;
        }

        #region 消费者投诉
        public ActionResult AddComplaint(string orderNum)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                OrderConsumersBLL bll = new OrderConsumersBLL();
                View_Material_OnlineOrder model = bll.GetConsumersOrder(orderNum);
                if (model == null)
                {
                    return Content("<script>alert('获取数据失败');history.go(-1);</script>");
                }
                ViewBag.linkPhone = consumers.LinkPhone;
                ViewBag.enterpriseName = model.EnterpriseName;
                ViewBag.materialName = model.MaterialName;
                return View();
            }
        }
        [HttpPost]
        public ActionResult AddComplaint(Complaint model)
        {
            JsonResult js = new JsonResult();
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                Enterprise_FWCode_00 code = GetSession();
                if (code != null)
                {
                    //Complaint model = new Complaint();
                    model.adddate = DateTime.Now;
                    model.ComplaintContent = Request["Textcontent"];
                    model.ComplaintDate = DateTime.Now;
                    model.ComplaintType_ID = 1;
                    model.Enterprise_Info_ID = long.Parse(code.Enterprise_Info_ID);
                    model.lastdate = DateTime.Now;
                    model.LInkMan = consumers.GetLinkMan;
                    model.LinkPhone = consumers.LinkPhone;
                    model.Material_ID = code.Material_ID;
                    model.Status = (int)Common.EnumFile.Status.used;
                    model.IsRead = (int)Common.EnumFile.IsRead.noRead;
                    ComplaintBLL bll = new ComplaintBLL();
                    RetResult ret = bll.AddComplaint(model);
                    if (ret.IsSuccess)
                    {
                        js.Data = new { res = true, info = ret.Msg, url = "/Wap_Consumers/Index" };
                    }
                    else
                    {
                        js.Data = new { res = false, info = ret.Msg };
                    }
                }
                else
                {
                    js.Data = new { info = "请重新拍码访问页面！" };
                }
                return js;
            }
        }
        #endregion

        /// <summary>
        /// 更改订单状态
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <param name="status">更改为Status</param>
        /// <returns>操作结果</returns>
        public ActionResult ChangeStatus(string orderNum, int status)
        {
            JsonResult js = new JsonResult();
            RetResult ret = new BLL.Material_OnlineOrderBLL().PaySuccess(orderNum, status);
            js.Data = new { res = ret.IsSuccess, info = ret.Msg };
            return js;
        }

        /// <summary>
        /// 查看物流信息
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <returns>结果</returns>
        public ActionResult CheckExpress(string orderNum)
        {
            string ExpressComp = "";
            string ExpressNum = "";
            View_Material_ReturnOrder returnModel = new BLL.MaterialReturnOrderBLL().GetReturnOrder(orderNum);
            if (returnModel != null && returnModel.OrderType == (int)Common.EnumFile.PayStatus.Returned)
            {
                ExpressComp = returnModel.ExpressComp;
                ExpressNum = returnModel.ExpressNum;
            }
            else
            {
                Material_OnlineOrder orderModel = new BLL.Material_OnlineOrderBLL().GetModel(orderNum);
                ExpressComp = orderModel.ExpressComp;
                ExpressNum = orderModel.ExpressNum;
            }
            ViewBag.ExpressComp = ExpressComp;
            ViewBag.ExpressNum = ExpressNum;
            return View();
        }

        /// <summary>
        /// 付款get
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <returns>视图</returns>
        public ActionResult PayOrder(string orderNum)
        {
            View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers != null)
            {
                Material_OnlineOrder orderModel = new BLL.Material_OnlineOrderBLL().GetModel(orderNum);
                ScanCodeBLL scan = new ScanCodeBLL();
                Material material = scan.GetMaterial(orderModel.MateriaID.GetValueOrDefault(0));
                Enterprise_Info enterprise = scan.GetEnterprise(orderModel.Enterprise_ID);
                //Material_Spec spec = new Material_Spec();
                //spec = new BLL.Material_SpecBLL().GetModelBySpecId((long)orderModel.SpecID);
                //ViewBag.specid = spec.ID;
                ViewBag.material = material;
                ViewBag.enterprise = enterprise;
                ViewBag.consumers = consumers;
                ViewBag.orderModel = orderModel;
                return View();
            }
            else
            {
                return Content("<script>alert('请登录后访问页面！');window.location.href = '/wap_order/login';</script>");
            }
        }

        /// <summary>
        /// 付款post提交数据
        /// </summary>
        /// <param name="specName">规格</param>
        /// <param name="price">单价</param>
        /// <param name="count">数量</param>
        /// <param name="total">总价</param>
        /// <param name="address">地址</param>
        /// <param name="type">支付方式</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult PayOrder(string orderNum, string specName, string price, string count, string total, string address, string type)
        {
            Material_OnlineOrder model = new Material_OnlineOrder();
            JsonResult js = new JsonResult();
            try
            {
                View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
                if (consumers != null)
                {
                    model.OrderNum = orderNum;
                    model.Consumers_Address = address;
                    model.MaterialCount = Convert.ToInt32(count);
                    model.MaterialPrice = Convert.ToDecimal(price);
                    model.PayType = Convert.ToInt32(type);
                    model.SpecID = Convert.ToInt64(specName.Split('_')[0]);
                    model.TotalMoney = model.MaterialPrice * model.MaterialCount;
                    model.OrderType = (int)Common.EnumFile.PayStatus.PayDelivery;
                    if (model.PayType != 1)
                    {
                        model.OrderType = (int)Common.EnumFile.PayStatus.NotPay;
                    }
                    RetResult ret = new BLL.Material_OnlineOrderBLL().ChangeOrder(model);
                    if (ret.IsSuccess)
                    {
                        switch (model.PayType)
                        {
                            case 1:
                                js.Data = new { res = true, info = "订购成功", url = "/Wap_Consumers/Index" };
                                break;
                            case 2://支付宝
                                string SiteUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"];
                                js.Data = new
                                {
                                    res = true,
                                    info = ret.Msg,
                                    url = "/OlineAlipay/Alipay?out_trade_no=" + model.OrderNum +
                                    "&subject=" + model.MaterialName +
                                    "&total_fee=" + model.TotalMoney +
                                    "&show_url=" + SiteUrl + "Wap_Consumers/Index"
                                };
                                break;
                            default:
                                js.Data = new { res = true, info = "订购成功", url = "/Wap_Consumers/Index" };
                                break;
                        }
                    }
                    else
                    {
                        js.Data = new { info = ret.Msg };
                    }
                }
                else
                {
                    return Content("<script>alert('请登录后访问页面！');window.location.href = '/wap_order/login?pageType=3';</script>");
                }
            }
            catch
            {
                js.Data = new { info = "暂未开放该功能！" };
            }
            return js;
        }

        #region 申请退货
        public ActionResult ReturnMaterial(string OrderNum)
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                View_Material_OnlineOrder model = new OrderConsumersBLL().GetConsumersOrder(OrderNum);
                ViewBag.TelPhone = Consumers.LinkPhone;
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult ReturnMaterial(string OrderNum, string ReturnContent)
        {
            JsonResult JsonResult = new JsonResult();
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers != null)
            {
                RetResult RetResult = new BLL.MaterialReturnOrderBLL().AddMaterialReturnOrder(OrderNum, ReturnContent);
                if (RetResult.IsSuccess)
                {
                    JsonResult.Data = new { res = true, info = RetResult.Msg, url = "/Wap_Consumers/Index" };
                }
                else
                {
                    JsonResult.Data = new { res = false, info = RetResult.Msg };
                }
                return JsonResult;
            }
            else
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
        }
        #endregion

        #region 拒收
        public ActionResult DotSend(string OrderNum)
        {
            JsonResult js = new JsonResult();
            RetResult ret = new BLL.MaterialReturnOrderBLL().EditStatus(OrderNum);
            js.Data = new { res = ret.IsSuccess, info = ret.Msg };
            return js;
        }
        #endregion

        #region 填写运单信息
        public ActionResult SetExpressInfo(string OrderNum)
        {
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                View_Material_ReturnOrder model = new MaterialReturnOrderBLL().GetReturnOrder(OrderNum);
                ViewBag.TelPhone = Consumers.LinkPhone;
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult SetExpressInfo(string OrderNum, string ExpressComp, string ExpressNum)
        {
            JsonResult JsonResult = new JsonResult();
            View_Order_Consumers Consumers = SessCokieOrder.Get;
            if (Consumers != null)
            {
                RetResult RetResult = new BLL.MaterialReturnOrderBLL().EditMaterialReturnOrder(OrderNum, ExpressComp, ExpressNum);
                if (RetResult.IsSuccess)
                {
                    JsonResult.Data = new { res = true, info = RetResult.Msg, url = "/Wap_Consumers/Index" };
                }
                else
                {
                    JsonResult.Data = new { res = false, info = RetResult.Msg };
                }
                return JsonResult;
            }
            else
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
        }
        #endregion

        public ActionResult LogOut()
        {
            Common.Argument.SessCokieOrder.Logout();
            if (Session["ewm"] != null)
            {
                return Content("<script>window.location.href = '/Wap_Index/Index';</script>");
            }
            else
            {
                return Content("<script>window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
        }

        //20160601添加在消费者订单页面点击产品查看产品信息
        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="material_id">产品ID</param>
        /// <returns></returns>
        public ActionResult ConMaterialInfo(long material_id)
        {
            Material material = null;
            Brand brand = null;
            Brand areaBrand = null;
            Enterprise_Info enterprise = null;
            ViewBag.seleTime = DateTime.Now;
            try
            {

                if (material_id != 0)
                {
                    ScanCodeBLL scan = new ScanCodeBLL();

                    material = scan.GetMaterial(material_id);

                    brand = scan.GetBrand(material.Brand_ID.GetValueOrDefault(0));

                    areaBrand = scan.GetAreaBrand(material_id);

                    enterprise = scan.GetEnterprise(Convert.ToInt64(material.Enterprise_Info_ID));

                    List<View_ProductInfoForMaterial> PropertyList =
                        new BLL.ProductInfoBLL().GetMaterialProperty(Convert.ToInt64(material.Enterprise_Info_ID),
                        material.Material_ID);
                    ViewBag.PropertyList = PropertyList;
                }
                else
                {
                    return Content("<script>alert('产品信息错误！')</script>");
                }
            }
            catch { }
            ViewBag.material = material;
            ViewBag.brand = brand;
            ViewBag.areaBrand = areaBrand;
            ViewBag.enterprise = enterprise;
            ViewBag.materialType = material.Dictionary_MaterialType_ID ?? 0;
            return View();
        }

        /// <summary>
        /// 零钱汇总详情
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetRedChange()
        {
            if (SessCokieOrder.Get == null)
            {
                return Content("<script>alert('请先登录再操作！');location.href='/Wap_Order/Login'</script>");
            }
            ViewBag.TotalMoney = new ScanCodeBLL().GetSendChange(SessCokieOrder.Get.Order_Consumers_ID).Sum(a => a.RedValue);
            ViewBag.User = SessCokieOrder.Get;
            var modelLst = new ScanCodeBLL().GetSendChange(SessCokieOrder.Get.Order_Consumers_ID).ToList();
            return View(modelLst);
        }

        /// <summary>
        /// 零钱红包详情
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult RedChangeDetail(int flag = 0)
        {
            if (SessCokieOrder.Get == null)
            {
                return Content("<script>alert('请先登录再操作！');location.href='/'</script>");
            }

            if (flag == 0)
            {
                var modelLst = new ScanCodeBLL().GetSendChange(SessCokieOrder.Get.Order_Consumers_ID);
                return Json(modelLst, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var modelLst = new ScanCodeBLL().SendedChange(SessCokieOrder.Get.Order_Consumers_ID);
                return Json(modelLst, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 红包说明
        /// </summary>
        /// <returns></returns>
        public ActionResult RedDesc()
        {
            return View();
        }

        public ActionResult SheZhi()
        {
            View_Order_Consumers pf = Common.Argument.SessCokieOrder.Get;
            if (pf == null)
            {
                return Content("<script>window.location.href = '/Wap_Order/Login?pageType=3';</script>");
            }
            else
            {
                ViewBag.Order_Consumers_ID = pf.Order_Consumers_ID;
            }
            return View();
        }

        public ActionResult Txsuccess(string money,string ok)
        {
            ViewBag.Ok = ok;
            ViewBag.Money = money;
            return View();
        }
    }
}
