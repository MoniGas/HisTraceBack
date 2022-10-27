/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-11-13

** 联系方式 :13313318725

** 描述：主要用于登录、设置默认信息的控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;

namespace TeaBack.Controllers
{
    /// <summary>
    /// 登录、设置默认信息控制器
    /// </summary>
    public class Wap_OrderController : Controller
    {
        #region 登录
        /// <summary>
        /// 登录Get页面
        /// </summary>
        /// <param name="pageType">记录上级页面</param>
        /// <returns>视图</returns>
        public ActionResult Login(string MaterialSpecId, string Count, string uppage,int pageType = 3)
        {
            View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            //判断是否登录
            if (consumers != null)
            {
                //设置跳转页面
                string controller = "Wap_Index";
                string action = "MaterialOrder";
                //判断是否填写过地址，如果未填写跳转到设置默认地址页面
                if (!consumers.Order_Consumers_Address_ID.HasValue)
                {
                    controller = "Wap_Order";
                    action = "Edit";
                    //return Content("<script>alert('请输入收货地址！');history.go(-1);</script>");
                }
                else if (pageType == 3)
                {
                    controller = "Wap_Consumers";
                    action = "Index";
                }
                else if (pageType == 4)
                {
                    controller = "Public";
                   //action = "ConfirmOrder";
                    action = "ConfirmOrder?MaterialSpecId=" + MaterialSpecId + "&Count=" + Count + "&uppage=" + uppage + "";
                }
                return RedirectToAction(action, controller, new { pageType = pageType });
            }
            ViewBag.pageType = pageType;
            ViewBag.MaterialSpecId = MaterialSpecId;
            ViewBag.Count = Count;
            ViewBag.uppage = uppage;
            return View();
        }
        /// <summary>
        /// 登录Post页面
        /// </summary>
        /// <param name="phone">手机号即登录名</param>
        /// <param name="password">密码</param>
        /// <param name="pageType">记录上级页面</param>
        /// <param name="pageType">登录方式</param>
        /// <returns>视图</returns>
        [HttpPost]
        public ActionResult Login(string phone, string password,string messageWord, int pageType, int loginType,string MaterialSpecId, string Count, string uppage)
        {
            JsonResult js = new JsonResult();
            string strMsg = "";
            View_Order_Consumers consumers = new BLL.OrderConsumersBLL().Login(phone, password, loginType,messageWord, out strMsg);
            if (consumers == null)
            {
                js.Data = new { res = false, info = strMsg };
            }
            else
            {
                //初始化返回页面
                string action = "IndexTwo";
                string controller = "Wap_IndexTwo";
                #region 登陆成功记录session和cookie
                consumers.AllArea = Common.Argument.Public.GetAllArea(decimal.Parse(consumers.AreaID ?? "0"));
                Common.Argument.SessCokieOrder.Logout();
                Common.Argument.SessCokieOrder.Set(consumers);
                #endregion
                //判断是否填写过地址，如果未填写跳转到设置默认地址页面
                if (consumers.Order_Consumers_Address_ID.HasValue)
                {
                    if (pageType == 2)
                    {
                        action = "MaterialOrder";
                    }
                    else if (pageType == 3)
                    {
                        controller = "Wap_Consumers";
                        action = "Index";
                    }
                    else if (pageType == 4)
                    {
                        controller = "Public";
                        action = "ConfirmOrder?MaterialSpecId=" + MaterialSpecId + "&Count=" + Count + "&uppage=" + uppage + "";
                        ViewBag.MaterialSpecId = MaterialSpecId;
                        ViewBag.Count = Count;
                        ViewBag.uppage = uppage;
                        
                    }
                }
                else
                {
                    controller = "Wap_Order";
                    action = "Edit?pageType=" + pageType;
                }
                js.Data = new { res = true, url = "/" + controller + "/" + action };
            }

            return js;
        }
        #endregion

        #region 验证是否注册
        /// <summary>
        /// 验证是否注册
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>判断结果</returns>
        public JsonResult IsRegister(string phone)
        {
            Result result = new BLL.OrderConsumersBLL().IsRegister(phone);
            return Json(result);
        }
        #endregion

        #region 获取密码
        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>密码</returns>
        public JsonResult GetPassWord(string phone)
        {
            Result result = new BLL.OrderConsumersBLL().GetPassWord(phone);
            //Result result = new Result();
            //result.ResultCode = 0;
            //result.ResultMsg = "";
            return Json(result);
        }
        #endregion

        #region 设置个人信息
        /// <summary>
        /// 设置个人信息Get
        /// </summary>
        /// <param name="pageType">设置完成后返回地址</param>
        /// <returns>视图</returns>
        public ActionResult Edit(int pageType = 1)
        {
            //读取登录信息
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                ViewBag.pageType = pageType; 
                return View(consumers);
            }
        }

        /// <summary>
        /// 设置个人信息Post
        /// </summary>
        /// <param name="model">个人信息</param>
        /// <returns>视图操作结果</returns>
        [HttpPost]
        public ActionResult Edit(View_Order_Consumers model)
        {
            View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            JsonResult js = new JsonResult();
            int pageType = int.Parse(Request["pageType"] ?? "1");
            //判断是否登录
            if (consumers == null)
            {
                js.Data = new { res = true, info = "请先登录后再进行此操作！", url = "/Wap_Order/Login?pageType=" + pageType };
            }
            else
            {
                #region 给实体赋值
                Order_Consumers_Address Address = new Order_Consumers_Address();
                Address.Address = model.Address;
                Address.AreaID = Request["selArea"];
                Address.CityID = Request["selCity"];
                Address.LinkMan = Request["GetLinkMan"];
                Address.LinkPhone = Request["LinkPhone"];
                Address.Order_Consumers_ID = consumers.Order_Consumers_ID;
                Address.Postcode = Request["Postcode"];
                Address.ProvinceID = Request["Province"];
                Address.status = (int)Common.EnumFile.Status.used;
                #endregion
                RetResult result = new BLL.Order_Consumers_AddressBLL().AddConAddress(Address, true, ref consumers,true);
                string url = "/Wap_Index/Index";
                if (result.IsSuccess)
                {
                    Result changePwdResult = new BLL.OrderConsumersBLL().UpdataPwd(consumers.Order_Consumers_ID, Request["TxtPassWord"] ?? consumers.PassWord);
                    if (changePwdResult.ResultCode == 1)
                    {
                        #region 重新给Session赋值
                        consumers.AllArea = Common.Argument.Public.GetAllArea(decimal.Parse(consumers.AreaID ?? "0"));
                        Common.Argument.SessCokieOrder.Logout();
                        Common.Argument.SessCokieOrder.Set(consumers);
                        #endregion
                        //判断返回页面地址
                        if (pageType == 2)
                        {
                            url = "/Wap_IndexTwo/MaterialOrder";
                        }
                        else if (pageType == 3)
                        {
                            url = "/Wap_Consumers/Index";
                        }
                        else if (pageType == 4)
                        {
                            url = "/Public/ConfirmOrder?";
                        }
                    }
                }
                js.Data = new { res = result.IsSuccess, info = "设置个人信息失败！", url = url };
            }
            return js;
        }
        #endregion

        #region 获取省级列表
        /// <summary>
        /// 获取省级列表
        /// </summary>
        /// <param name="provinceCode">默认省ID（修改用）</param>
        /// <param name="cityCode">默认市ID（修改用）</param>
        /// <param name="areaCode">默认区ID（修改用）</param>
        /// <returns>省级地址列表</returns>
        public ActionResult GetAddress(string provinceCode = "-2", string cityCode = "-2", string areaCode = "-2")
        {
            //获取地区列表
            List<SelectListItem> itemSheng = new List<SelectListItem>();
            AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
            if (AllAddress == null)
            {
                return View();
            }
            List<AddressInfo> sheng = AllAddress.AddressList.Where(p => p.AddressLevel == 1).OrderBy(p => p.AddressCode).ToList();
            if (sheng != null)
            {
                #region 添加“请选择”选项
                AddressInfo playChoose = new AddressInfo();
                playChoose.Address_ID = -2;
                playChoose.AddressCode = "-2";
                playChoose.AddressName = "请选择";
                sheng.Insert(0, playChoose);
                #endregion
                //遍历地区列表数据
                foreach (AddressInfo sub in sheng)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = sub.AddressCode;
                    item.Text = sub.AddressName;
                    //判断是否要选中
                    if (sub.Address_ID.ToString() == provinceCode)
                    {
                        item.Selected = true;
                    }
                    itemSheng.Add(item);
                }
            }
            #region 给页面所需参数赋值
            ViewBag.itemSheng = itemSheng;
            ViewBag.provinceCode = provinceCode;
            ViewBag.cityCode = cityCode;
            ViewBag.areaCode = areaCode;
            #endregion
            return View();
        }
        #endregion

        #region 获取用户所有收货地址
        /// <summary>
        /// 获取用户所有收货地址
        /// </summary>
        /// <returns>地址列表</returns>
        public ActionResult GetAddressInfo()
        {
            //判断是否登录
            View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                //查询列表
                List<Order_Consumers_Address> result = new BLL.Order_Consumers_AddressBLL().GetList(consumers.Order_Consumers_ID);
                foreach (var item in result)
                {
                    item.AllArea = Common.Argument.Public.GetAllArea(decimal.Parse(item.AreaID));
                }
                return Json(result);
            }
        }
        #endregion

        #region 获取商品活动
        /// <summary>
        /// 获取商品活动
        /// </summary>
        /// <param name="MaterialSpecId">规格标识</param>
        /// <returns>商品活动Json数据</returns>
        public ActionResult GetMaterialProperty(string MaterialSpecId)
        {
            if (string.IsNullOrEmpty(MaterialSpecId))
            {
                return Content("<script>alert('数据错误请刷新后重试！');</script>");
            }

            //查询列表
            List<View_Material_Property> result = new BLL.MaterialPropertyBLL().GetMaterialPropertyList(Convert.ToInt64(MaterialSpecId));
            return Json(result);
            ////判断是否登录
            //View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            //if (consumers == null)
            //{
            //    return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            //}
            //else
            //{
            //    //查询列表
            //    List<View_Material_Property> result = new BLL.MaterialPropertyBLL().GetMaterialPropertyList(MaterialSpecId);
            //    return Json(result);
            //}
        }
        #endregion
    }
}
