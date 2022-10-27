/********************************************************************************

** 作者：张翠霞

** 开发时间：2017-7-6

** 联系方式:13313318725

** 代码功能：支付宝在线支付、异步通知

** 版本：v1.0

** 版权：追溯     

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.AlipayClass;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 支付宝在线支付、异步通知
    /// </summary>
    public class OlineAlipayController : Controller
    {
        #region 付款
        /// <summary>
        /// 支付宝接口实现支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="subject">商品</param>
        /// <param name="total_fee">金额</param>
        /// <param name="show_url">商品展示地址</param>
        /// <param name="return_url">同步通知地址</param>
        public void Alipay(string out_trade_no, string subject, string total_fee, string show_url, string return_url)
        {
            #region 获取请求数据
            //支付类型
            //必填，不能修改
            string payment_type = "1";
            //服务器异步通知页面路径
            //需http://格式的完整路径，不能加?id=123这类自定义参数
            string notify_url = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Market/OlineAlipay/Notify_url";
            //订单描述
            //选填
            string body = string.Empty;
            //超时时间
            //选填
            string it_b_pay = string.Empty;
            //钱包token
            //选填
            string extern_token = string.Empty;
            #endregion
            //拼接支付串，表单提交
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("seller_id", Config.Seller_id);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "alipay.wap.create.direct.pay.by.user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("body", body);
            sParaTemp.Add("it_b_pay", it_b_pay);
            sParaTemp.Add("extern_token", extern_token);
            //建立请求
            string sHtmlText = "";
            try
            {
                sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            }
            catch { }
            Common.Log.WriteLog.WriteAlipayLog("支付购买套餐信息：" + "订单编号out_trade_no【" + sParaTemp["out_trade_no"] + "】，购买套餐subject【" + sParaTemp["subject"] + "】"
                + ",支付金额【" + sParaTemp["total_fee"] + "】");
            Response.Write(sHtmlText);
        }

        /// <summary>
        /// 异步通知模块
        /// </summary>
        public ActionResult Notify_url()
        {
            RedirectToRouteResult rtr = null;
            RetResult result = new RetResult();
            CmdResultError error = CmdResultError.EXCEPTION;
            string msg = "支付失败！";
            SortedDictionary<string, string> sPara = GetRequestPost();
            long activityId = 0;
            //判断是否有带返回参数
            if (sPara.Count > 0)
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);
                //验证成功
                if (verifyResult)
                {
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                    //商户订单号
                    string out_trade_no = Request.Form["out_trade_no"];
                    //支付宝交易号
                    string trade_no = Request.Form["trade_no"];
                    //交易状态
                    string trade_status = Request.Form["trade_status"];
                    //买家支付宝账号
                    string BuyerEmail = Request.Form["buyer_email"];
                    //买家支付宝唯一用户号
                    string BuyerId = Request.Form["buyer_id"];
                    if (Request.Form["subject"] != null)
                    {
                        activityId = Convert.ToInt32(Request.Form["subject"]);
                    }
                    if (Request.Form["trade_status"] == "TRADE_FINISHED" || Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        BuyCodeOrdePayBLL bll = new BuyCodeOrdePayBLL();
                        result = bll.PaySuccess(out_trade_no, (int)Common.EnumText.PayType.OlineAlipay, trade_no, BuyerEmail, BuyerId);
                        msg = result.Msg;
                    }
                    else
                    {
                        msg = "交易失败，支付宝交易状态返回显示不成功!";
                    }
                    Common.Log.WriteLog.WriteAlipayLog("交易异步通知信息：商户订单号out_trade_no【" + out_trade_no +
                        "】,支付宝交易号trade_no【" + trade_no + "】,交易状态trade_status【" + trade_status + "】,买家支付宝账号BuyerEmail【" + BuyerEmail
                        + "】,买家支付宝唯一用户号BuyerId【" + BuyerId + "】");
                }
                else//验证失败
                {
                    msg = "验证失败，签名或者认证有问题!";
                }
            }
            else
            {
                msg = "出现异常错误！";
                Common.Log.WriteLog.WriteErrorLog("支付宝交易失败,出现异常错误！");
            }
            Common.Log.WriteLog.WriteAlipayLog("订单编号out_trade_no的交易结果：" + msg);
            //创建红包支付，跳转到拍码页
            if (Session["SetPay"] as string == "SetPay")
            {
                rtr = RedirectToAction("MainFrame", "HomeMarket", new { flag = 1 });
            }
            else
            {
                MarketActive.BLL.WeiXinCommon.WriteLog("出现异常！", "Notify", "活动能够id" + Session["ActivityID"] + "      " + activityId);
                rtr = RedirectToAction("MainFrame", "HomeMarket", new { flag =string.Format("{0}_{1}", 2,Session["ActivityID"]) });
            }
            return rtr;
        }
        #endregion

        #region 转账
        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="payAccount">支付账号</param>
        /// <param name="payUserName">支付名称</param>
        /// <param name="totalMoney">总价</param>
        /// <param name="getAccount">收款账号</param>
        /// <param name="getTrueName">收款账号户主</param>
        public ActionResult Transfer(long checkId, string totalMoney, string getAccount, string getTrueName)
        {
            #region 获取请求数据
            //服务器异步通知页面路径
            string notify_url = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "OlineAlipay/Transfer_url"; ;
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //付款账号
            string email = Config.Seller_email;
            //必填

            //付款账户名
            string account_name = Config.Seller_trueName;
            //必填，个人支付宝账号是真实姓名公司支付宝账号是公司名称

            DateTime now = DateTime.Now;
            //付款当天日期
            string pay_date = now.ToString("yyyyMMdd");
            //必填，格式：年[4位]月[2位]日[2位]，如：20100801

            //批次号
            string batch_no = now.ToString("yyyyMMddHHmmss") + checkId.ToString().PadLeft(3, '0');
            //必填，格式：当天日期[8位]+序列号[3至16位]，如：201008010000001

            //付款总金额
            string batch_fee = totalMoney;
            //必填，即参数detail_data的值中所有金额的总和

            //付款笔数
            string batch_num = "1";
            //必填，即参数detail_data的值中，“|”字符出现的数量加1，最大支持1000笔（即“|”字符出现的数量999个）

            //付款详细数据
            string detail_data = batch_no + "^" + getAccount + "^" + getTrueName + "^" + totalMoney + "^" + checkId;
            //必填，格式：流水号1^收款方帐号1^真实姓名^付款金额1^备注说明1|流水号2^收款方帐号2^真实姓名^付款金额2^备注说明2....
            #endregion

            //拼接支付串，表单提交
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "batch_trans_notify");
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("email", email);
            sParaTemp.Add("account_name", account_name);
            sParaTemp.Add("pay_date", pay_date);
            sParaTemp.Add("batch_no", batch_no);
            sParaTemp.Add("batch_fee", batch_fee);
            sParaTemp.Add("batch_num", batch_num);
            sParaTemp.Add("detail_data", detail_data);
            //建立请求
            string sHtmlText = "";
            try
            {
                sHtmlText = Submit.BuildRequest(sParaTemp, "post", "确认");
            }
            catch { }
            Common.Log.WriteLog.WriteAlipayLog("交易结果：" + sParaTemp);
            //Response.Write(sHtmlText);
            return Content(sHtmlText);
        }

        /// <summary>
        /// 异步通知模块
        /// </summary>
        public ActionResult Transfer_url()
        {
            RedirectToRouteResult rtr = null;
            RetResult result = new RetResult();
            string msg = "结算失败！";
            SortedDictionary<string, string> sPara = GetRequestPost();
            //判断是否有带返回参数
            if (sPara.Count > 0)
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);
                //验证成功
                if (verifyResult)
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码

                    //BaseResultModel myResult = new BLL.Material_OnlineOrderBLL().BalanceOrder(1);

                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //批量付款数据中转账成功的详细信息
                    string success_details = Request.Form["success_details"];

                    //批量付款数据中转账失败的详细信息
                    string fail_details = Request.Form["fail_details"];
                    if (!string.IsNullOrEmpty(success_details))
                    {
                        try
                        {
                            string[] successArray = success_details.Split('^');
                            //20080808001^gonglei1@handsome.com.cn^龚本林^20.00^S^null^200810248427067^20081024143652|
                            string checkId = successArray[0].Substring(15).TrimStart('0');
                            //Material_OnlineOrderBLL bll = new Material_OnlineOrderBLL();
                            //new BLL.Material_OnlineOrderBLL().BalanceOrder(Convert.ToInt64(checkId));
                        }
                        catch { }
                    }

                    //判断是否在商户网站中已经做过了这次通知返回的处理
                    //如果没有做过处理，那么执行商户的业务程序
                    //如果有做过处理，那么不执行商户的业务程序

                    Response.Write("success");  //请不要修改或删除

                }
                else//验证失败
                {
                    msg = "验证失败，签名或者认证有问题！";
                }
            }
            else
            {
                msg = "出现异常错误！";
                Common.Log.WriteLog.WriteErrorLog("支付宝结算失败,出现异常错误！");
            }
            Common.Log.WriteLog.WriteErrorLog("交易结果：" + msg);
            Common.Log.WriteLog.WriteAlipayLog("交易结果：" + msg);
            rtr = RedirectToAction("Index", "Wap_Consumers");
            return rtr;
        }
        #endregion

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            coll = Request.Form;

            String[] requestItem = coll.AllKeys;
            StringBuilder LogStr = new StringBuilder();
            LogStr.Append("【" + DateTime.Now.ToString() + "】");

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
                LogStr.Append("requestItem[" + i + "]:" + Request.Form[requestItem[i]] + "；");
            }
            Common.Log.WriteLog.WriteAlipayLog(LogStr.ToString());
            return sArray;
        }
    }
}
