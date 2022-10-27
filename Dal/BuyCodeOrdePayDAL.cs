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
using System.Linq;
using LinqModel;
using Common.Argument;
using System.Configuration;
using Common.Tools;
using Common.Log;

namespace Dal
{
    public class BuyCodeOrdePayDAL : DALBase
    {
        /// <summary>
        /// 更该订单状态
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <param name="payType">状态</param>
        /// <param name="TradeNo">支付宝订单号</param>
        /// <param name="ConsumerLogin">消费者登录名</param>
        /// <param name="ConsumerNum">消费者支付宝16位唯一标识</param>
        /// <returns>操作结果</returns>
        public RetResult PaySuccess(string orderNum, int payType, string TradeNo, string ConsumerLogin, string ConsumerNum)
        {
            CmdResultError error = CmdResultError.EXCEPTION;
            string msg = "支付失败！";
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    //YX_BuyCodeOrder order = dataContext.YX_BuyCodeOrder.FirstOrDefault(m => m.OrderNum == orderNum);
                    YX_RedRecharge order = dataContext.YX_RedRecharge.FirstOrDefault(m => m.OrderNum == orderNum.Replace("C",""));
                    if (order != null)
                    {
                        #region 添加交易记录
                        if (payType == (int)Common.EnumText.PayType.OlineAlipay || payType == (int)Common.EnumText.PayType.WeiXinPay)
                        {
                            YX_OrderRecordGet orderGet = new YX_OrderRecordGet();
                            orderGet = dataContext.YX_OrderRecordGet.FirstOrDefault(m => m.PayOrderNum == TradeNo);
                            //异步通知第一次新增一条数据
                            if (orderGet == null)
                            {
                                orderGet = new YX_OrderRecordGet();
                                orderGet.ConsumersAccount = ConsumerLogin;
                                orderGet.Money = Convert.ToDecimal(order.RechargeValue);
                                orderGet.Order_Consumers_ID = order.RedRechargeID;
                                orderGet.OrderNum = order.OrderNum;
                                orderGet.PayTime = DateTime.Now;
                                //支付方式
                                orderGet.PayType = order.RechargeMode;
                                orderGet.PayOrderNum = TradeNo;
                                orderGet.ConsumersNum = ConsumerNum;
                                dataContext.YX_OrderRecordGet.InsertOnSubmit(orderGet);
                                #region 支付宝发送短信通知
                                //try
                                //{
                                //    MsgInfo msgInfo = new MsgInfo();
                                //    msgInfo.secretKey = ConfigurationManager.AppSettings["secretKey"];
                                //    string now = DateTime.Now.ToString("yyyy-MM-dd");
                                //    msgInfo.sms.sendDate = now;
                                //    msgInfo.sms.content = "中国农业追溯公共服务平台商城有新的订单，请及时处理。【广联信息】";
                                //    msgInfo.sms.msType = 1;
                                //    msgInfo.sms.sendTime = now;
                                //    msgInfo.phoneNumbers[0] =
                                //        dataContext.Order_EnterpriseAccount.FirstOrDefault(m => m.Enterprise_ID == order.Enterprise_ID).LinkPhone;
                                //    //实例对象
                                //    SPortService client = new SPortService();
                                //    client.Url = ConfigurationManager.AppSettings["WebServiceUrl"];
                                //    string jsonStr = JsonHelper.DataContractJsonSerialize(msgInfo);
                                //    //发送短信
                                //    jsonStr = jsonStr.Replace(now, "");
                                //    client.sendSMS(jsonStr);
                                //}
                                //catch { }
                                #endregion
                            }
                            //YX_BuyCodeOrdePay payModel = new YX_BuyCodeOrdePay();
                            //payModel = dataContext.YX_BuyCodeOrdePay.FirstOrDefault(m => m.PayOderNum == order.OrderNum);
                            //if (payModel == null)
                            //{
                            //    payModel = new YX_BuyCodeOrdePay();
                            //    payModel.BuyCodeOrderID = order.RedRechargeID;
                            //    //收款账号
                            //    //payModel.CollectAccount=
                            //    payModel.CompanyID = order.CompanyID;
                            //    //付款账号
                            //    payModel.PayAccount = ConsumerLogin;
                            //    payModel.PayDate = orderGet.PayTime;
                            //    payModel.PayMode = payType;
                            //    payModel.PayOderNum = order.OrderNum;
                            //    payModel.RealPrice = order.RechargeValue;
                            //    dataContext.YX_BuyCodeOrdePay.InsertOnSubmit(payModel);
                            //}
                            order.PayState = (int)Common.EnumText.PayState.Payed;
                        }
                        #endregion
                        dataContext.SubmitChanges();
                        error = CmdResultError.NONE;
                        msg = "支付成功！";
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "Material_OnlineOrderDAL.PaySuccess";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        public YX_BuyCodeOrdePay GetOrderPayInfo(long id, long eid)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.YX_BuyCodeOrdePay.FirstOrDefault(m => m.BuyCodeOrderID == id && m.CompanyID == eid);
            }
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        public YX_BuyCodeOrdePay GetOrderPayInfoA(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.YX_BuyCodeOrdePay.FirstOrDefault(m => m.BuyCodeOrderID == id);
            }
        }

        #region 备案 标志人事物
        public RetResult RecordCode(string mainCode, long enterpriseId, string codeUseID, string packageName)
        {
            string msg = "备案品类码失败！";
            string errorMemo = "";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                string mailcodeIdcode = mainCode.Substring(0, mainCode.LastIndexOf("."));
                string ms = DateTime.Now.ToString("fff");
                string action = ConfigurationManager.AppSettings["RecordCode"];
                string access_token = ConfigurationManager.AppSettings["access_token"];
                string parseUrl = ConfigurationManager.AppSettings["IpRedirect"];
                Dictionary<string, string> paras = new Dictionary<string, string>();
                paras.Add("access_token", access_token);
                paras.Add("companyIDcode", mailcodeIdcode);
                paras.Add("codeUse_ID", "90");// codeUseID);
                paras.Add("industryCategory_ID", "10133");
                paras.Add("categoryCode", "12000000");
                paras.Add("modelNumber", packageName);
                paras.Add("modelNumberEn", "");
                paras.Add("introduction", "");
                paras.Add("codePayType", "5");
                paras.Add("goToUrl", parseUrl + "Market/Wap_IndexMarket/Index");
                string strBack = APIHelper.sendPost(action, paras, "get");
                JsonObject jsonObject = new JsonObject(strBack);
                string result = jsonObject["ResultCode"].Value;
                string resultMsg = jsonObject["ResultMsg"].Value;
                string organUnitIDcode = jsonObject["OrganUnitIDcode"].Value;
                if (result == "1")
                {
                    //msg = "备案品类码成功！";
                    error = CmdResultError.NONE;
                    msg = "1";
                    errorMemo = organUnitIDcode;

                }
                else if (result == "0")
                {
                    msg = "备案失败！";
                }
                Ret.SetArgument(error, errorMemo, msg);
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrdePayDAL.RecordCode()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            Ret.SetArgument(error, errorMemo, msg);
            return Ret;
        }
        #endregion
    }
}
