using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common.AlipayClass;
using System.Collections.Specialized;
using Common.Argument;
using Common.Log;
using System.Text;

namespace iagric_plant.Controllers
{
    public class RefundController : Controller
    {
        //
        // GET: /Refund/
        /// <summary>
        /// 支付宝退款接口
        /// </summary>
        /// <param name="Detail_data">原付款支付宝交易号^退款总金额^退款理由，多笔时用“#”字符隔开,如：2011011201037066^5.00^协商退款</param>
        /// <param name="Batch_num">退款笔数，“#”字符加1</param>
        /// <param name="Batch_no">批次号，保证唯一 格式：当天日期[8位]+序列号[3至24位]</param>
        public void Refund(string Detail_data, string Batch_num, string Batch_no)
        {
            //Detail_data = "2016011521001004450065234744^0.01^协商退款";
            //Batch_num = "1";
            //Batch_no = "201601190000001";
            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //服务器异步通知页面路径
            string notify_url = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"] + "Refund/RefundNotify";
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //退款当天日期
            //string refund_date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            string refund_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //必填，格式：年[4位]-月[2位]-日[2位] 小时[2位 24小时制]:分[2位]:秒[2位]，如：2007-10-01 13:13:13
            //2016/1/15 11:27:14

            //批次号
            string batch_no = Batch_no;
            //必填，格式：当天日期[8位]+序列号[3至24位]，如：201008010000001

            //退款笔数
            string batch_num = Batch_num;
            //必填，参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的数量999个）

            //退款详细数据
            string detail_data = Detail_data;
            //必填，具体格式请参见接口技术文档


            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "refund_fastpay_by_platform_pwd");
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("seller_email", Config.Seller_email);
            sParaTemp.Add("refund_date", refund_date);
            sParaTemp.Add("batch_no", batch_no);
            sParaTemp.Add("batch_num", batch_num);
            sParaTemp.Add("detail_data", detail_data);

            //建立请求
            string sHtmlText = "";
            try
            {
                sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            }
            catch { }
            Response.Write(sHtmlText);
        }

        /// <summary>
        /// 异步调用返回路径
        /// </summary>
        /// <returns></returns>
        public ActionResult RefundNotify()
        {
            RedirectToRouteResult rtr = null;
            RetResult result = new RetResult();
            string msg = "批量退款失败！";
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    string batch_no = Request.Form["batch_no"];

                    //批量退款数据中转账成功的笔数

                    string success_num = Request.Form["success_num"];

                    //批量退款数据中的详细信息
                    string result_details = Request.Form["result_details"];
                    if (Convert.ToInt16(success_num) > 0)
                    {
                        string[] orders = result_details.Split('#');
                        if (orders != null && orders.Count() > 0)
                        {
                            List<string> PayOrderNums = new List<string>();
                            foreach (var item in orders)
                            {
                                string[] order = item.Split('^');
                                if (order[2].ToUpper() == "SUCCESS")
                                    PayOrderNums.Add(order[0]);
                            }
                            BLL.MaterialReturnOrderBLL bll = new BLL.MaterialReturnOrderBLL();
                            result = bll.EditStatus(PayOrderNums, (int)Common.EnumFile.PayStatus.ReturnMoney);
                            msg = result.Msg;
                        }
                    }
                    else
                    {
                        msg = "批量退款失败，支付宝交易状态返回显示不成功!";
                    }
                }
                else//验证失败
                {
                    msg = "验证失败，签名或者认证有问题!";
                }
            }
            else
            {
                msg = "出现异常错误！";
                WriteLog.WriteErrorLog("支付宝交易失败,出现异常错误！");
            }
            WriteLog.WriteRefundLog("批量退款结果：" + msg);

            return Content("<script>alert('" + msg + "');window.close();</script>");
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;
            StringBuilder LogStr = new StringBuilder();
            LogStr.Append("【" + DateTime.Now.ToString() + "】");
            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
                LogStr.Append("requestItem[" + i + "]:" + Request.Form[requestItem[i]] + "；");
            }
            WriteLog.WriteRefundLog(LogStr.ToString());
            return sArray;
        }
    }
}
