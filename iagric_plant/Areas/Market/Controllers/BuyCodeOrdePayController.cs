/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-6
** 联系方式:13313318725
** 代码功能：二维码订单管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MarketActive.Filter;
using Common.Argument;
using BLL;
using LinqModel;
using MarketActive.BLL;
using Common.Tools;
using System.Text;
using System.IO;
using System.Drawing;
using iagric_plant.Areas.Market.Models;

namespace MarketActive.Controllers
{
    [AdminAuthorize]
    public class BuyCodeOrdePayController : Controller
    {
        /// <summary>
        /// 购买二维码
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyEWMOrder()
        {
            PackageBLL bll = new PackageBLL();
            List<YX_Package> packageList = bll.GetPackageList();
            ViewBag.packageList = packageList;
            return View();
        }

        /// <summary>
        /// 查看二维码
        /// </summary>
        /// <returns></returns>
        public ActionResult LookEwm(string tradeNo, double jg)
        {
            ViewBag.TradeNo = tradeNo;
            ViewBag.Jg = jg;
            return View();
        }

        /// <summary>
        /// 生成图片
        /// </summary>
        /// <param name="tcid"></param>
        /// <returns></returns>
        public ActionResult CreateImage(string tradeNo, double jg)
        {
            ResponseUnifiedorder order = XmlHelper.Deserialize(typeof(ResponseUnifiedorder), CreateOrder((jg * 100).ToString(), tradeNo)) as ResponseUnifiedorder;
            if (order != null)
            {
                Image img = MvcWebCommon.Tools.CreateEwmImg.GetQRCodeImageEx(order.code_url, 200, 200);
                if (img != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return File(ms.ToArray(), "image/Jpeg", DateTime.Now.Ticks.ToString() + ".jpg");
                    }
                }
                else
                {
                    return Content("<script>alert('请重新操作');location.href='/Market/BuyCodeOrdePay/BuyEWMOrder'<script>");
                }
            }
            else
            {
                return Content("<script>alert('请重新操作');location.href='/Market/BuyCodeOrdePay/BuyEWMOrder'<script>");
            }
        }

        /// <summary>
        /// 手动回调
        /// </summary>
        /// <returns></returns>
        public ActionResult HandNotify(string tradeNo)
        {
            try
            {
                string data = SearchOrder("", tradeNo);
                //点击支付完成调一下查询订单
                ResponseOrderQuery queryNew = XmlHelper.Deserialize(typeof(ResponseOrderQuery), data) as ResponseOrderQuery;
                if (queryNew.trade_state == "SUCCESS")
                {
                    //数据库操作
                    BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
                    RetResult result = bll.PaySuccess(queryNew.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, queryNew.out_trade_no, queryNew.openid);
                    int i = 5;
                    while (!result.IsSuccess && --i > 0)
                    {
                        result = bll.PaySuccess(queryNew.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, queryNew.out_trade_no, queryNew.openid);
                    }
                }
                else
                {
                    WeiXinCommon.WriteLog(data, "HandNotify", "");
                }
            }
            catch (Exception ex)
            {
                WeiXinCommon.WriteLog("手动查询订单", "HandNotify", ex.Message);
            }

            return Json(new { });
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        public ActionResult Info(long id)
        {
            LoginInfo user = SessCokie.Get;
            BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
            YX_BuyCodeOrdePay info = bll.GetOrderPayInfo(id, user.EnterpriseID);
            return View(info);
        }
        //public ActionResult Info()
        //{
        //    return View();
        //}

        #region 微信接口
        /// <summary>
        /// 回调函数
        /// </summary>
        public void Notify()
        {
            try
            {
                //接收从微信后台POST过来的数据
                WeiXinCommon.WriteLog("微信支付返回回调！", "Notify", "");
                Stream s = Request.InputStream;
                byte[] buffer = new byte[1024];
                StringBuilder sb = new StringBuilder();
                int count = 0;
                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
                WeiXinCommon.WriteLog("微信支付返回：" + sb.ToString(), "Notify", "");
                ResponseOrderQuery query = XmlHelper.Deserialize(typeof(ResponseOrderQuery), sb.ToString()) as ResponseOrderQuery;
                //检查支付结果中transaction_id是否存在
                if (string.IsNullOrWhiteSpace(query.transaction_id))
                {
                    //若transaction_id不存在，则立即返回结果给微信支付后台
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "FAIL");
                    res.Add("return_msg", "支付结果中微信订单号不存在");
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
                if (query.return_code == "SUCCESS" && query.result_code == "SUCCESS")
                {
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "SUCCESS");
                    res.Add("return_msg", "OK");
                    string msg = "";
                    //数据库操作
                    BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
                    RetResult result = bll.PaySuccess(query.out_trade_no, (int)Common.EnumText.PayType.WeiXinPay, query.out_trade_no, query.openid);
                    WeiXinCommon.WriteLog("数据库操作：订单号" + query.out_trade_no, "Notify", "");
                    msg = result.Msg;
                    //点击支付完成调一下查询订单
                    //ResponseOrderQuery queryNew0 = XmlHelper.Deserialize(typeof(ResponseOrderQuery), SearchOrder(query.transaction_id, Session["TradeNo"] as string)) as ResponseOrderQuery;
                    //if (queryNew0.trade_type == "SUCCESS")
                    //{
                    //    //数据库操作
                    //}
                    WeiXinCommon.WriteLog(WeiXinCommon.DictionaryToXml(res), "Notify", msg);
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
                else
                {
                    //若订单查询失败，则立即返回结果给微信支付后台
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "FAIL");
                    res.Add("return_msg", "查询订单失败！");
                    WeiXinCommon.WriteLog("查询订单失败！", "Notify", "");
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WeiXinCommon.WriteLog("出现异常！", "Notify", "");
                SortedList<string, string> res = new SortedList<string, string>();
                res.Add("return_code", "FAIL");
                res.Add("return_msg", "出现异常");
                Response.Write(WeiXinCommon.DictionaryToXml(res));
                Response.End();
            }

        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <returns></returns>
        public string CreateOrder(string money, string tradeNo)
        {
            string data = string.Empty;
            try
            {
                SortedList<string, string> dic = new SortedList<string, string>();
                SortedList<string, string> dicSign = new SortedList<string, string>();
                string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
                string productId = DateTime.Now.Ticks.ToString();
                //tradeNo = WeiXinCommon.GenerateOutTradeNo();
                dicSign.Add("appid", WeiXinCommon._PayId);
                dicSign.Add("mch_id", WeiXinCommon._MerIdPay);
                dicSign.Add("device_info", "WEB");
                dicSign.Add("nonce_str", nonceStr);
                dicSign.Add("body", "河北广联-购买二维码");
                dicSign.Add("out_trade_no", tradeNo);
                dicSign.Add("total_fee", money);
                dicSign.Add("spbill_create_ip", System.Configuration.ConfigurationManager.AppSettings["IpAddress"]);
                dicSign.Add("notify_url", System.Configuration.ConfigurationManager.AppSettings["NotifyUrl"] + "Packet/Notify");
                dicSign.Add("trade_type", "NATIVE");
                dicSign.Add("product_id", productId);

                dic.Add("appid", WeiXinCommon._PayId);
                dic.Add("mch_id", WeiXinCommon._MerIdPay);
                dic.Add("device_info", "WEB");
                dic.Add("nonce_str", nonceStr);
                dic.Add("sign", WeiXinCommon.Sign(dicSign, WeiXinCommon._Key));
                dic.Add("body", "BuyCode");
                dic.Add("out_trade_no", tradeNo);
                dic.Add("total_fee", money);
                dic.Add("spbill_create_ip", System.Configuration.ConfigurationManager.AppSettings["IpAddress"]);
                dic.Add("notify_url", System.Configuration.ConfigurationManager.AppSettings["NotifyUrl"] + "Packet/Notify");
                dic.Add("trade_type", "河北广联-购买二维码");
                dic.Add("product_id", productId);
                data = APIHelper.sendPost("https://api.mch.weixin.qq.com/pay/unifiedorder", WeiXinCommon.DictionaryToXml(dic));
            }
            catch (Exception ex)
            {
                WeiXinCommon.WriteLog(data, "CreateOrder", ex.Message);
            }
            return data;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="tranId">商户单号</param>
        /// <returns></returns>
        public string SearchOrder(string tranId, string tradeNo)
        {
            string data = string.Empty;
            try
            {
                SortedList<string, string> dic = new SortedList<string, string>();
                SortedList<string, string> dicSign = new SortedList<string, string>();
                string nonceStr = Guid.NewGuid().ToString().Replace("-", "");

                dicSign.Add("appid", WeiXinCommon._PayId);
                dicSign.Add("mch_id", WeiXinCommon._MerIdPay);
                dicSign.Add("transaction_id", tranId);
                dicSign.Add("out_trade_no", tradeNo);
                dicSign.Add("nonce_str", nonceStr);

                dic.Add("appid", WeiXinCommon._PayId);
                dic.Add("mch_id", WeiXinCommon._MerIdPay);
                dic.Add("transaction_id", tranId);
                dic.Add("nonce_str", nonceStr);
                dic.Add("out_trade_no", tradeNo);
                dic.Add("sign", WeiXinCommon.Sign(dicSign, WeiXinCommon._Key));
                data = APIHelper.sendPost("https://api.mch.weixin.qq.com/pay/orderquery", WeiXinCommon.DictionaryToXml(dic));
                WeiXinCommon.WriteLog(data, "SearchOrder", "");
                ResponseOrderQuery order = XmlHelper.Deserialize(typeof(ResponseOrderQuery), data) as ResponseOrderQuery;
            }
            catch (Exception ex)
            {
                WeiXinCommon.WriteLog(data, "SearchOrder", ex.Message);
            }
            return data;
        }
        #endregion
    }
}
