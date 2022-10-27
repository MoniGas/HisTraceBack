/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-6-8
** 联系方式:15533621896
** 代码功能：拍码红包页
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using BLL;
using Common.Tools;
using MarketActive.BLL;
using System.IO;
using System.Text;
using LinqModel;
using iagric_plant.Areas.Market.Models;
using Common.Argument;
using Common.Log;
using Common;

namespace MarketActive.Controllers
{
    /// <summary>
    /// 拍码红包页
    /// </summary>
    public class Wap_IndexMarketController : Controller
    {
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string _RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"];

        /// <summary>
        /// 进入领取红包页
        /// </summary>
        /// <param name="settingId"></param>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                CodeInfo codeInfo = GetSession(null, 0);
                if (null != codeInfo && codeInfo.CodeType == (int)EnumFile.AnalysisBased.Seting)
                {
                    //对接微信授权登录接口
                    WxEnAccountBLL wxbll = new WxEnAccountBLL();
                    YX_WxEnAccount wxzh = wxbll.GetModel(codeInfo.EnterpriseID);
                    string url = string.Empty;
                    if (wxzh != null)
                    {
                        Session["wxzh"] = wxzh;
                        url = GetCodeUrlBypayId(wxzh.WxAppId);
                    }
                    else
                    {
                        Session["wxzh"] = null;
                        url = GetCodeUrl();
                    } 
                    WriteLog.WriteWxLog("【领取红包：" + DateTime.Now + "】" + codeInfo.FwCode.EWM, "Wx");
                    return Content("<script>location.href='" + url + "'</script>");
                }
                else
                {
                    string url = _RedirectUrl + "Market/Wap_IndexMarket/Activity";
                    return Content("<script>location.href='" + url + "'</script>");
                }
            }
            catch
            {
                return Content("<script>alert('网络异常，请重新拍码！')</script>");
            }
        }

        /// <summary>
        /// 活动页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Activity()
        {
            View_Activity model = null;
            try
            {
                CodeInfo codeInfo = GetSession(null, 0);
                //微信返回code
                string code = Request["code"] as string;
                //正式需注释
                //code = "021TUg3P0H85Ob250z5P01N63P0TUg3E";
                if (codeInfo==null || string.IsNullOrWhiteSpace(code))
                {
                    model = new View_Activity {ActivityID = 0};
                    //预览模式
                    ViewBag.PreView = "1";
                    return View(model);
                }
                //获取accesstoken
                string data = string.Empty;
                ResponseAccessTokenModel accModel = null;
                if (Session["wxzh"] != null)
                {
                    YX_WxEnAccount wxzh = Session["wxzh"] as YX_WxEnAccount;
                    data = GetAccessTokenBypayId(code, wxzh.WxAppId, wxzh.AppSecret);
                }
                else
                {
                    data = GetAccessToken(code);
                }
                accModel = JsonHelper.DeserializeJsonToObject<ResponseAccessTokenModel>(data);
                Session["AccessToken"] = accModel.access_token;
                Session["OpenID"] = accModel.openid;
                Session.Timeout = 7200;
                //获取用户信息
                data = GetUserInfo(accModel.access_token, accModel.openid);
                if (!string.IsNullOrWhiteSpace(data))
                {
                    WriteLog.WriteWxLog("【获取授权成功GetAccessToken" + DateTime.Now + "】" + data, "Wx");
                    Session["UserInfo"] = JsonHelper.DeserializeJsonToObject<ResponseUserInfoModel>(data);
                }
                //获取活动信息
                model = new ScanCodeMarketBLL().GetActivity(Convert.ToInt32(codeInfo.CodeSeting.ID),0);
                if (model != null)
                {
                    if (model.IsYZCode == 1)
                    {
                        //需要输入手机号
                        ViewBag.IsYZTel = "1";
                    }
                }
                else
                {
                    model = new View_Activity();
                    model.ActivityID = 0;
                    //预览模式
                    ViewBag.PreView = "1";
                    return View(model);
                }
                ViewBag.PreView = "0";
            }
            catch (Exception ex)
            {
                WriteLog.WriteWxLog("【Index:" + DateTime.Now + "】" + ex.ToString(), "Wx");
            }
            return View(model);
        }

        /// <summary>
        /// 发送红包/发送提示页面
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult ActivitySuccess(long activityId, string tel)
        {
            SendPacketResult result = new SendPacketResult();
            result.Ok = false;
            result.Msg = "您没有抢到红包，谢谢您的参与！";
            ScanCodeMarketBLL bll = new ScanCodeMarketBLL();
            CodeInfo codeInfo = GetSession(null, 0);
            if (codeInfo == null)
            {
                result.EnterpriseName = "河北广联信息技术有限公司";
                return View(result);
            }
            if (Session["OpenID"] == null)
            {
                return View(result);
            }
            string ewm = codeInfo.FwCode.EWM;
            //是否可以抢红包
            int canGet=bll.CanGetRedPacket(0, codeInfo.FwCode, 0, activityId).Code;
            if (canGet!= 2 && canGet!=3)
            {
                return View(result);
            }
            View_Activity activity = bll.GetActivity(0, activityId);
            if (activity == null)
            {
                return View(result);
            }
            double money = 0;
            if (activity.RedStyle == (int)EnumText.RedStyle.藏)
            {
                YX_AcivityDetail detail = bll.GetDetail((long)codeInfo.FwCode.ActivitySubID);
                if (detail != null)
                {
                    money = ((double)detail.RedValue) * 100;
                }
            }
            else if (activity.RedStyle == (int)EnumText.RedStyle.抢)
            {
                money = GetMoney(activity.ActivityID);
            }
            if (money <= 0)
            {
                return View(result);
            }
            string data = SendPacket(activity,ewm,money);
            WriteLog.WriteWxLog("【对接微信接口发送红包返回信息:" + data + "" + DateTime.Now + "】", "Wx");
            if (data.Contains("NoMoney"))
            {
                result.Ok = false;
                result.Msg = "您没有抢到红包，谢谢您的参与！";
                return View(result);
            }
            if (data.Contains("余额不足"))
            {
                result.Ok = false;
                result.Msg = "您没有抢到红包，谢谢您的参与！";
                return View(result);
            }
            if (data.Contains("NoEwm"))
            {
                result.Ok = false;
                result.Msg = "您没有抢到红包，谢谢您的参与！";
                return View(result);
            }
            #region 发送成功，记日志
            ResponseSendRedPacket sendPacket = XmlHelper.Deserialize(typeof(ResponseSendRedPacket), data) as ResponseSendRedPacket;
            if (sendPacket != null && sendPacket.result_code == "SUCCESS")
            {
                double txValue = sendPacket.total_amount;
                //查询企业是否设置了微信红包支付账户
                YX_WxEnAccount wxzh = new YX_WxEnAccount();
                if (Session["wxzh"] != null)
                {
                    wxzh = Session["wxzh"] as YX_WxEnAccount;
                }
                YX_RedSendRecord sendRecord = new YX_RedSendRecord();
                sendRecord.IDcode = ewm;
                sendRecord.BillNumber = sendPacket.mch_billno;
                sendRecord.WeiXinUserID = sendPacket.re_openid;
                sendRecord.WxListId = sendPacket.send_listid;
                sendRecord.SendRedValue = money / 100;
                sendRecord.MarId = sendPacket.mch_id;//20190103这句新加
                //发送红包成功后更新下发送记录
                long recordeDetailId = 0;
                long sendId = bll.UpdateSendRecord(sendRecord, out recordeDetailId);
                //如果发送数量日志表编号大于0，更新下日记表中的发送编号
                if (recordeDetailId > 0)
                {
                    bool updateState = bll.UpdateDetailRecord(recordeDetailId, sendId);
                }
                data = GetRedPacket(sendPacket.mch_billno, sendRecord.MarId);
                ResponseGetRedPacket getPacket = XmlHelper.Deserialize(typeof(ResponseGetRedPacket), data) as ResponseGetRedPacket;
                if (getPacket != null && getPacket.result_code == "SUCCESS")
                {
                    int state = 0;
                    switch (getPacket.status)
                    {
                        case "SENDING": state = 1; break;
                        case "SENT": state = 2; break;
                        case "FAILED": state = 3; break;
                        case "RECEIVED": state = 4; break;
                        case "RFUND_ING": state = 5; break;
                        case "REFUND": state = 6; break;
                    }
                    string getDate = getPacket.hblist != null && getPacket.hblist.Length > 0 ? getPacket.hblist[0].rcv_time : "";
                    YX_RedGetRecord getRecode = new YX_RedGetRecord
                    {
                        ActivityID = Convert.ToInt32(activity.ActivityID),
                        FailReason = getPacket.reason,
                        GetDate = getDate,
                        GetRedValue = sendRecord.SendRedValue,
                        GetState = state,
                        HbType = getPacket.hb_type == "GROUP" ? 1 : 2,
                        IDcode = ewm,
                        MarId = getPacket.mch_id,
                        ReFundtime = getPacket.refund_time,
                        SendTime = getPacket.send_time,
                        SendType = (int)EnumFile.RedSendType.red,
                        WeiXinUserID = sendPacket.re_openid,
                        WxListId = getPacket.detail_id,
                        BillNumber = getPacket.mch_billno,
                        SendRecordID = sendId
                    };
                    long getId = bll.AddRecord(null, getRecode);
                }
                else
                {
                    WeiXinCommon.WriteLog(data, "ActivitySuccess", "");
                }
                result.Ok = true;
                result.Msg = "发送成功！";
                result.Money = money / 100;
                result.EnterpriseName = activity.SendCompany;
                return View(result);
            }
            else
            {
                //RetResult rusultDel = bll.DeleteSendRecord(ewm, activity.ActivityID);
                return View(result);
            }
            #endregion
        }

        #region js-sdk相关接口
        /// <summary>
        /// 获取基础accesstoken
        /// </summary>
        /// <returns></returns>
        public string GetBaseAccessToken()
        {
            try
            {
                //获取基础accesstoken
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("grant_type", "client_credential");
                dic.Add("appid", WeiXinCommon._PayId);
                dic.Add("secret", WeiXinCommon._AppSecret);
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                WriteLog.WriteWxLog("【获取基础accesstoken GetBaseAccessToken " + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(ex.Message + "GetBaseAccessToken");
                sw.Close();
                fs.Close();
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取jsapi_ticket
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string GetTicket(string accessToken)
        {
            try
            {
                //获取基础accesstoken
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("access_token", accessToken);
                dic.Add("type", "jsapi");
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                WriteLog.WriteWxLog("【获取jsapi_ticket" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(ex.Message + "GetTicket");
                sw.Close();
                fs.Close();
                return string.Empty;
            }
        }
        #endregion

        #region 授权接口
        /// <summary>
        /// 获取Code地址
        /// </summary>
        /// <returns></returns>
        public string GetCodeUrl()
        {
            try
            {
                //本次项目写死为用户显示授权登录
                string sansapiType = "snsapi_userinfo";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", WeiXinCommon._PayId);
                //用微信扫码时，跳转到的相应的界面
                dic.Add("redirect_uri", _RedirectUrl + "Market/Wap_IndexMarket/Activity");
                dic.Add("response_type", "code");
                dic.Add("scope", sansapiType);//snsapi_userinfo snsapi_base
                string url = "https://open.weixin.qq.com/connect/oauth2/authorize";
                string data = APIHelper.GetUrl(url, dic, "#wechat_redirect");
                //WriteLog.WriteWxLog("【获取Code地址GetCodeUrl" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(ex.Message + "GetCodeUrl");
                sw.Close();
                fs.Close();
                return string.Empty;
            }
        }
        /// <summary>
        /// 根据自定义payId获取用户授权信息
        /// </summary>
        /// <param name="sansapiType"></param>
        /// <param name="payId"></param>
        /// <returns></returns>
        public string GetCodeUrlBypayId(string payId)
        {
            try
            {
                //本次项目写死为用户显示授权登录
                string sansapiType = "snsapi_userinfo";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", payId);
                //用微信扫码时，跳转到的相应的界面
                dic.Add("redirect_uri", _RedirectUrl + "Market/Wap_IndexMarket/Activity");
                dic.Add("response_type", "code");
                dic.Add("scope", sansapiType);
                string url = "https://open.weixin.qq.com/connect/oauth2/authorize";
                string data = APIHelper.GetUrl(url, dic, "#wechat_redirect");
                //WriteLog.WriteWxLog("【获取Code地址GetCodeUrlBypayId" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(ex.Message + "GetCodeUrl");
                sw.Close();
                fs.Close();
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取授权AccessToken 默认2小时
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string GetAccessToken(string code)
        {
            try
            {
                WriteLog.WriteWxLog("Code内容:" + code, "Wx");
                //获取accesstoken
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", WeiXinCommon._PayId);
                dic.Add("secret", WeiXinCommon._AppSecret);
                dic.Add("code", code);
                dic.Add("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                //WriteLog.WriteWxLog("【获取授权GetAccessToken" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                WriteLog.WriteWxLog("【GetAccessToken:" + DateTime.Now + "】" + ex.ToString(), "Wx");
                return string.Empty;
            }
        }
        /// <summary>
        /// 根据企业自定义信息获取授权AccessToken 默认2小时
        /// </summary>
        /// <param name="code"></param>
        /// <param name="payId"></param>
        /// <param name="appScret"></param>
        /// <returns></returns>
        private string GetAccessTokenBypayId(string code,string payId,string appScret)
        {
            try
            {
                WriteLog.WriteWxLog("Code内容:" + code, "Wx");
                //获取accesstoken
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", payId);
                dic.Add("secret", appScret);
                dic.Add("code", code);
                dic.Add("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                WriteLog.WriteWxLog("【获取授权GetAccessTokenBypayId" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                WriteLog.WriteWxLog("【GetAccessTokenBypayId:" + DateTime.Now + "】" + ex.ToString(), "Wx");
                return string.Empty;
            }
        }
        /// <summary>
        /// 刷新授权AccessToken后，默认30天
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private string GetRefreshToken(string refreshToken)
        {
            try
            {
                //获取accesstoken
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", WeiXinCommon._PayId);
                dic.Add("grant_type", "refresh_token");
                dic.Add("refresh_token", refreshToken);
                string url = "https://api.weixin.qq.com/sns/oauth2/refresh_token";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                WriteLog.WriteWxLog("【刷新授权GetRefreshToken" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                WriteLog.WriteWxLog("【GetRefreshToken:" + DateTime.Now + "】" + ex.ToString(), "Wx");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="accModel"></param>
        /// <returns></returns>
        public string GetUserInfo(string accessToken, string openId)
        {
            try
            {
                //获取accesstoken
                WriteLog.WriteWxLog("【获取用户信息GetUserInfo" + DateTime.Now + "】accessToken:" + accessToken + ";  openId" + openId, "Wx");
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("access_token", accessToken);
                dic.Add("openid", openId);
                dic.Add("lang", "zh_CN");
                string url = "https://api.weixin.qq.com/sns/userinfo";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                WriteLog.WriteWxLog("【获取用户信息GetUserInfo" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                WriteLog.WriteWxLog("【GetUserInfo" + DateTime.Now + "】" + ex.ToString(), "Wx");
                return string.Empty;
            }
        }

        /// <summary>
        /// 验证凭据是否过期
        /// </summary>
        /// <param name="token">凭据值</param>
        /// <param name="openId">用户id</param>
        /// <returns></returns>
        public string IsAccessTokenValid(string token, string openId)
        {
            try
            {
                //获取accesstoken
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("access_token", token);
                dic.Add("openid", openId);
                string url = "https://api.weixin.qq.com/sns/auth";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                WriteLog.WriteWxLog("【验证凭据是否过期IsAccessTokenValid" + DateTime.Now + "】" + data, "Wx");
                return data;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(ex.Message + "IsAccessTokenValid");
                sw.Close();
                fs.Close();
                return string.Empty;
            }
        }
        #endregion

        #region 发送红包接口
        /// <summary>
        /// 对接微信接口发送红包,如果为小额，则返回ChangeMoney
        /// </summary>
        /// <returns></returns>
        public string SendPacket(View_Activity activity, string ewm,double money)
        {
            try
            {
                double dmoney = money / 100;
                var sendData = new SortedList<string, string>(StringComparer.Ordinal);
                //查询企业是否设置了微信红包支付账户
                YX_WxEnAccount wxzh = new YX_WxEnAccount();
                if (Session["wxzh"] != null)
                {
                    wxzh = Session["wxzh"] as YX_WxEnAccount;
                }
                #region 发送红包前，先更新红包剩余数量,新增红包发送记录
                YX_RedSendRecord sendRecord = new YX_RedSendRecord();
                if (Session["wxzh"] != null)
                {
                    //WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:赋值" + DateTime.Now + " MarId:】" + wxzh.MarId, "Wx");
                    sendRecord.ActivityID = Convert.ToInt32(activity.ActivityID);
                    sendRecord.IDcode = ewm;
                    sendRecord.IPAddress = GetVisitorIPHelper.ClientIp();
                    sendRecord.MarId = wxzh.MarId;
                    sendRecord.SendDate = DateTime.Now;
                    sendRecord.SendRedValue = dmoney;
                    sendRecord.WxAppId = wxzh.WxAppId;
                    sendRecord.SendType = (int)EnumFile.RedSendType.red;
                }
                else
                {
                    //WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:赋值" + DateTime.Now + " MarId:】" + WeiXinCommon._MerId, "Wx");
                    sendRecord.ActivityID = Convert.ToInt32(activity.ActivityID);
                    sendRecord.IDcode = ewm;
                    sendRecord.IPAddress = GetVisitorIPHelper.ClientIp();
                    sendRecord.MarId = WeiXinCommon._MerId;
                    sendRecord.SendDate = DateTime.Now;
                    sendRecord.SendRedValue = dmoney;
                    sendRecord.WxAppId = WeiXinCommon._PayId;
                    sendRecord.SendType = (int)EnumFile.RedSendType.red;
                }
                if (Session["UserInfo"] as ResponseUserInfoModel != null)
                {
                    //WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:Session[UserInfo]不是null " + DateTime.Now + " 】", "Wx");
                    ResponseUserInfoModel user = Session["UserInfo"] as ResponseUserInfoModel;
                    sendRecord.Province = user.province;
                    sendRecord.City = user.city;
                    sendRecord.Country = user.country;
                    string sex = "";
                    switch (user.sex)
                    {
                        case "0": sex = "未知"; break;
                        case "1": sex = "男"; break;
                        case "2": sex = "女"; break;
                    }
                    sendRecord.Sex = sex;
                    sendRecord.WeiXinName = user.nickname;
                    //WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:用户昵称" + DateTime.Now + " 】" + user.nickname, "Wx");
                }
                else
                {
                    WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:Session[UserInfo]是null " + DateTime.Now + " 】", "Wx"); 
                }
                long sendId = new ScanCodeMarketBLL().AddSendRecord(sendRecord);
                //sendId为0，说明没有红包数量可领
                if (sendId <= 0)
                {
                    return "NoMoney";
                }
                //插入拍码记录
                YX_Redactivity_ScanRecord scanRecord =
                    new YX_Redactivity_ScanRecord
                    {
                        IDcode = ewm,
                        IPAddress = GetVisitorIPHelper.ClientIp(),
                        ActivityID = activity.ActivityID,
                        WeiXinUserID = Session["OpenID"] as string
                    };
                new ScanCodeMarketBLL().AddRecord(scanRecord, null);
                #endregion
                //不为空说明企业设置了自己企业的微信账户，发红包是用自己企业的微信账户
                if (Session["wxzh"] != null)
                {
                    WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:找到企业自定义账号:"+wxzh.MarId+" " + DateTime.Now + "】", "Wx");
                    //更新数量后，对接接口发送红包
                    string billNo = WeiXinCommon.GetShopBill(wxzh.MarId);
                    var newData = new SortedList<string, string>(StringComparer.Ordinal);
                    string newGiud = Guid.NewGuid().ToString("N");
                    newData.Add("nonce_str", newGiud);
                    newData.Add("mch_billno", billNo);
                    newData.Add("mch_id", wxzh.MarId);
                    newData.Add("wxappid", wxzh.WxAppId);
                    newData.Add("send_name", activity.SendCompany);
                    newData.Add("re_openid", Session["OpenID"] as string);
                    newData.Add("total_amount", money.ToString());
                    newData.Add("total_num", "1");
                    newData.Add("wishing", activity.BlessingWords);
                    newData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    newData.Add("act_name", activity.ActivityTitle);
                    newData.Add("remark", "发送红包");
                    newData.Add("scene_id", "PRODUCT_1");
                    //发送参数
                    sendData.Add("nonce_str", newGiud);
                    sendData.Add("sign", WeiXinCommon.Sign(newData, wxzh.Key));
                    sendData.Add("mch_billno", billNo);
                    sendData.Add("mch_id", wxzh.MarId);
                    sendData.Add("wxappid", wxzh.WxAppId);
                    sendData.Add("send_name", activity.SendCompany);
                    sendData.Add("re_openid", Session["OpenID"] as string);
                    sendData.Add("total_amount", money.ToString());
                    sendData.Add("total_num", "1");
                    sendData.Add("wishing", activity.BlessingWords);
                    sendData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    sendData.Add("act_name", activity.ActivityTitle);
                    sendData.Add("remark", "发送红包");
                    sendData.Add("scene_id", "PRODUCT_1");
                    //发送红包接口
                    string data = APIHelper.SendRequest("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack", WeiXinCommon.DictionaryToXml(sendData), GetLocalPath() + wxzh.APIFileURL.Replace('/', '\\'), wxzh.MarId);
                    return data;
                }
                else //否则用我们广联的微信账户发红包
                {
                    WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:使用默认账号:"+WeiXinCommon._MerId+"  " + DateTime.Now + "】", "Wx");
                    //更新数量后，对接接口发送红包
                    string billNo = WeiXinCommon.GetShopBill(WeiXinCommon._MerId);
                    var newData = new SortedList<string, string>(StringComparer.Ordinal);
                    string newGiud = Guid.NewGuid().ToString("N");
                    newData.Add("nonce_str", newGiud);
                    newData.Add("mch_billno", billNo);
                    newData.Add("mch_id", WeiXinCommon._MerId);
                    newData.Add("wxappid", WeiXinCommon._PayId);
                    newData.Add("send_name", activity.SendCompany);
                    newData.Add("re_openid", Session["OpenID"] as string);
                    newData.Add("total_amount", money.ToString());
                    newData.Add("total_num", "1");
                    newData.Add("wishing", activity.BlessingWords);
                    newData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    newData.Add("act_name", activity.ActivityTitle);
                    newData.Add("remark", "发送红包");
                    newData.Add("scene_id", "PRODUCT_1");
                    //发送参数
                    sendData.Add("nonce_str", newGiud);
                    sendData.Add("sign", WeiXinCommon.Sign(newData, WeiXinCommon._Key));
                    sendData.Add("mch_billno", billNo);
                    sendData.Add("mch_id", WeiXinCommon._MerId);
                    sendData.Add("wxappid", WeiXinCommon._PayId);
                    sendData.Add("send_name", activity.SendCompany);
                    sendData.Add("re_openid", Session["OpenID"] as string);
                    sendData.Add("total_amount", money.ToString());
                    sendData.Add("total_num", "1");
                    sendData.Add("wishing", activity.BlessingWords);
                    sendData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    sendData.Add("act_name", activity.ActivityTitle);
                    sendData.Add("remark", "发送红包");
                    sendData.Add("scene_id", "PRODUCT_1");
                    //发送红包接口
                    string data = APIHelper.SendRequest("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack", WeiXinCommon.DictionaryToXml(sendData), GetLocalPath() + "apiclient_cert.p12", WeiXinCommon._MerId);
                    return data;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteWxLog("【对接微信接口发送红包SendPacket:异常" + DateTime.Now + "】"+ex.ToString(), "Wx");
                return string.Empty;
            }
        }

        /// <summary>
        /// 消费者登录后零钱提现
        /// </summary>
        /// <returns></returns>
        public ActionResult SendRedChangePacket(double money)
        {
            if (SessCokieOrder.Get == null)
            {
                return Json(new { msg = "请先登录" });
            }
            ScanCodeMarketBLL bll = new ScanCodeMarketBLL();
            //获取用户微信id，才可以发送红包
            var model = bll.GetRedChangeInfo(SessCokieOrder.Get.Order_Consumers_ID);
            JsonResult json = new JsonResult();
            if (model == null || string.IsNullOrEmpty(model.WeiXinUserID))
            {
                json.Data = new { msg = "该用户信息有误，请重新操作" };
            }
            else
            {
                DateTime now = DateTime.Now;
                string data = SendChangePacket(money, model.WeiXinUserID, model.ActivityId.Value);
                if (data.Contains("余额不足"))
                {
                    return Json(new { ok = "false", msg = "系统原因，请两天后重试！" });
                }
                ScanCodeMarketBLL acbll = new ScanCodeMarketBLL();
                View_Activity activity = acbll.GetActivity(0, model.ActivityId.Value);
                WxEnAccountBLL wxbll = new WxEnAccountBLL();
                YX_WxEnAccount wxzh = wxbll.GetModel(activity.CompanyID.Value);
                ResponseSendRedPacket sendPacket = XmlHelper.Deserialize(typeof(ResponseSendRedPacket), data) as ResponseSendRedPacket;
                if (sendPacket != null && sendPacket.result_code == "SUCCESS")
                {
                    double txValue = sendPacket.total_amount;
                    YX_RedSendChangeRecord sendRecord = new YX_RedSendChangeRecord();
                    sendRecord.BillNumber = sendPacket.mch_billno;
                    sendRecord.IPAddress = GetVisitorIPHelper.ClientIp();
                    if (activity != null && wxzh != null)
                    {
                        sendRecord.MarId = wxzh.MarId;
                    }
                    else
                    {
                        sendRecord.MarId = WeiXinCommon._MerId;
                    }
                    sendRecord.SendDate = now;
                    sendRecord.SendRedValue = txValue / Convert.ToDouble(100);
                    if (activity != null && wxzh != null)
                    {
                        sendRecord.WxAppId = wxzh.WxAppId;
                    }
                    else
                    {
                        sendRecord.WxAppId = WeiXinCommon._PayId;
                    }
                    sendRecord.WeiXinUserID = sendPacket.re_openid;
                    sendRecord.WxListId = sendPacket.send_listid;
                    sendRecord.SendType = (int)EnumFile.RedSendType.changeRed;

                    sendRecord.Province = model.Province;
                    sendRecord.City = model.City;
                    sendRecord.Country = model.Country;
                    sendRecord.Sex = model.Sex;
                    sendRecord.WeiXinName = model.WeiXinName;
                    long sendId = bll.AddSendChangeRecord(sendRecord, SessCokieOrder.Get.Order_Consumers_ID);
                    data = GetRedPacket(sendPacket.mch_billno, sendRecord.MarId);
                    ResponseGetRedPacket getPacket = XmlHelper.Deserialize(typeof(ResponseGetRedPacket), data) as ResponseGetRedPacket;
                    if (getPacket != null && getPacket.result_code == "SUCCESS")
                    {
                        int state = 0;
                        switch (getPacket.status)
                        {
                            case "SENDING": state = 1; break;
                            case "SENT": state = 2; break;
                            case "FAILED": state = 3; break;
                            case "RECEIVED": state = 4; break;
                            case "RFUND_ING": state = 5; break;
                            case "REFUND": state = 6; break;
                        }
                        string getDate = getPacket.hblist != null && getPacket.hblist.Length > 0 ? getPacket.hblist[0].rcv_time : "";
                        YX_RedGetChangeRecord getRecode = new YX_RedGetChangeRecord
                        {
                            FailReason = getPacket.reason,
                            GetDate = getDate,
                            GetRedValue = sendRecord.SendRedValue,
                            GetState = state,
                            HbType = getPacket.hb_type == "GROUP" ? 1 : 2,
                            MarId = getPacket.mch_id,
                            ReFundtime = getPacket.refund_time,
                            SendTime = getPacket.send_time,
                            SendType = (int)EnumFile.RedSendType.changeRed,
                            WeiXinUserID = sendPacket.re_openid,
                            WxListId = getPacket.detail_id,
                            BillNumber = getPacket.mch_billno,
                            SendRecordID = sendId,
                        };
                        long getId = bll.AddChangeRecord(getRecode);
                    }
                    else
                    {
                        WeiXinCommon.WriteLog(data, "SendRedChangePacket", "");
                    }
                    return Json(new { ok = "true", msg = "发送成功！" });
                }
                else
                {
                    WeiXinCommon.WriteLog(data, "SendRedChangePacket", "");
                    return Json(new { ok = "false", msg = "网络原因发送失败，请重新操作！" });
                }
            }
            return Json(new { ok = "false", msg = "网络原因发送失败，请重新操作！" });
        }

        /// <summary>
        /// 零钱提现时发送提现红包，对接微信接口
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public string SendChangePacket(double money, string openId, long activeID)
        {
            try
            {
                var sendData = new SortedList<string, string>(StringComparer.Ordinal);
                var newData = new SortedList<string, string>(StringComparer.Ordinal);
                string newGiud = Guid.NewGuid().ToString("N");
                //查询企业是否设置了微信红包支付账户
                WxEnAccountBLL wxbll = new WxEnAccountBLL();
                ScanCodeMarketBLL bll = new ScanCodeMarketBLL();
                View_Activity activity = bll.GetActivity(0, activeID);
                YX_WxEnAccount wxzh = wxbll.GetModel(activity.CompanyID.Value);
                //不为空说明企业设置了自己企业的微信账户，发红包是用自己企业的微信账户
                if (wxzh != null && activity != null)
                {
                    string billNo = WeiXinCommon.GetShopBill(wxzh.MarId);
                    newData.Add("nonce_str", newGiud);
                    newData.Add("mch_billno", billNo);
                    newData.Add("mch_id", wxzh.MarId);
                    newData.Add("wxappid", wxzh.WxAppId);
                    newData.Add("send_name", "产品追溯平台发放");
                    newData.Add("re_openid", openId);
                    newData.Add("total_amount", (money * 100).ToString());
                    newData.Add("total_num", "1");
                    newData.Add("wishing", "恭喜发财");
                    newData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    newData.Add("act_name", "红包提现");
                    newData.Add("remark", "发送红包");
                    //发送参数
                    sendData.Add("nonce_str", newGiud);
                    sendData.Add("sign", WeiXinCommon.Sign(newData, wxzh.Key));
                    sendData.Add("mch_billno", billNo);
                    sendData.Add("mch_id", wxzh.MarId);
                    sendData.Add("wxappid", wxzh.WxAppId);
                    sendData.Add("send_name", "产品追溯平台发放");
                    sendData.Add("re_openid", openId);
                    sendData.Add("total_amount", (money * 100).ToString());
                    sendData.Add("total_num", "1");
                    sendData.Add("wishing", "恭喜发财");
                    sendData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    sendData.Add("act_name", "红包提现");
                    sendData.Add("remark", "发送红包");
                    //发送红包接口
                    string data = APIHelper.SendRequest("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack", WeiXinCommon.DictionaryToXml(sendData), GetLocalPath() + wxzh.APIFileURL.Replace('/', '\\'), wxzh.MarId);
                    WriteLog.WriteWxLog("【零钱提现时发送提现红包SendChangePacket" + DateTime.Now + "】" + data, "Wx");
                    return data;
                }
                else //否则用广联的微信账户
                {
                    string billNo = WeiXinCommon.GetShopBill(WeiXinCommon._MerId);
                    newData.Add("nonce_str", newGiud);
                    newData.Add("mch_billno", billNo);
                    newData.Add("mch_id", WeiXinCommon._MerId);
                    newData.Add("wxappid", WeiXinCommon._PayId);
                    newData.Add("send_name", "产品追溯平台发放");
                    newData.Add("re_openid", openId);
                    newData.Add("total_amount", (money * 100).ToString());
                    newData.Add("total_num", "1");
                    newData.Add("wishing", "恭喜发财");
                    newData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    newData.Add("act_name", "红包提现");
                    newData.Add("remark", "发送红包");
                    //发送参数
                    sendData.Add("nonce_str", newGiud);
                    sendData.Add("sign", WeiXinCommon.Sign(newData, WeiXinCommon._Key));
                    sendData.Add("mch_billno", billNo);
                    sendData.Add("mch_id", WeiXinCommon._MerId);
                    sendData.Add("wxappid", WeiXinCommon._PayId);
                    sendData.Add("send_name", "产品追溯平台发放");
                    sendData.Add("re_openid", openId);
                    sendData.Add("total_amount", (money * 100).ToString());
                    sendData.Add("total_num", "1");
                    sendData.Add("wishing", "恭喜发财");
                    sendData.Add("client_ip", GetVisitorIPHelper.ClientIp());
                    sendData.Add("act_name", "红包提现");
                    sendData.Add("remark", "发送红包");
                    //发送红包接口
                    string data = APIHelper.SendRequest("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack", WeiXinCommon.DictionaryToXml(sendData), GetLocalPath() + "apiclient_cert.p12", WeiXinCommon._MerId);
                    WriteLog.WriteWxLog("【零钱提现时发送提现红包SendChangePacket" + DateTime.Now + "】" + data, "Wx");
                    return data;
                }
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(ex.Message + "SendChangePacket");
                sw.Close();
                fs.Close();
                return string.Empty;
            }
        }

        /// <summary>
        /// 调取微信接口获取红包发送记录，回调函数
        /// </summary>
        /// <returns></returns>
        public string GetRedPacket(string billNo, string marId)
        {
            try
            {
                WxEnAccountBLL wxbll = new WxEnAccountBLL();
                YX_WxEnAccount wxzh = wxbll.GetModelM(marId);
                var sendData = new SortedList<string, string>(StringComparer.Ordinal);
                var newData = new SortedList<string, string>(StringComparer.Ordinal);
                string newGiud = Guid.NewGuid().ToString("N");
                if (wxzh != null)
                {
                    newData.Add("nonce_str", newGiud);
                    newData.Add("mch_billno", billNo);
                    newData.Add("mch_id", wxzh.MarId);
                    newData.Add("appid", wxzh.WxAppId);
                    newData.Add("bill_type", "MCHT");
                    //发送参数
                    sendData.Add("nonce_str", newGiud);
                    sendData.Add("sign", WeiXinCommon.Sign(newData, wxzh.Key));
                    sendData.Add("mch_billno", billNo);
                    sendData.Add("mch_id", wxzh.MarId);
                    sendData.Add("appid", wxzh.WxAppId);
                    sendData.Add("bill_type", "MCHT");
                    //发送红包接口
                    string data = APIHelper.SendRequest("https://api.mch.weixin.qq.com/mmpaymkttransfers/gethbinfo", WeiXinCommon.DictionaryToXml(sendData), GetLocalPath() + wxzh.APIFileURL.Replace('/', '\\'), wxzh.MarId);
                    if (data.Contains("操作超时"))
                    {
                        FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                        sw.WriteLine(data + "GetRedPacket");
                        sw.Close();
                        fs.Close();
                    }
                    WriteLog.WriteWxLog("【调取微信接口获取红包发送记录GetRedPacket" + DateTime.Now + "】" + data, "WxGet");
                    return data;
                }
                else
                {
                    newData.Add("nonce_str", newGiud);
                    newData.Add("mch_billno", billNo);
                    newData.Add("mch_id", WeiXinCommon._MerId);
                    newData.Add("appid", WeiXinCommon._PayId);
                    newData.Add("bill_type", "MCHT");
                    //发送参数
                    sendData.Add("nonce_str", newGiud);
                    sendData.Add("sign", WeiXinCommon.Sign(newData, WeiXinCommon._Key));
                    sendData.Add("mch_billno", billNo);
                    sendData.Add("mch_id", WeiXinCommon._MerId);
                    sendData.Add("appid", WeiXinCommon._PayId);
                    sendData.Add("bill_type", "MCHT");
                    //发送红包接口
                    string data = APIHelper.SendRequest("https://api.mch.weixin.qq.com/mmpaymkttransfers/gethbinfo", WeiXinCommon.DictionaryToXml(sendData), GetLocalPath() + "apiclient_cert.p12", WeiXinCommon._MerId);
                    if (data.Contains("操作超时"))
                    {
                        FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                        sw.WriteLine(data + "GetRedPacket");
                        sw.Close();
                        fs.Close();
                    }
                    WriteLog.WriteWxLog("【调取微信接口获取红包发送记录GetRedPacket" + DateTime.Now + "】" + data, "WxGet");
                    return data;
                }
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(GetLocalPath() + DateTime.Now.Date.ToString("yyyyMMdd") + "exception.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(ex.Message + "GetRedPacket");
                sw.Close();
                fs.Close();
                return string.Empty;
            }
        }

        /// <summary>
        /// 随机获取金额
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public double GetMoney(long activityId)
        {
            double money = 0;
            double?[] arr = new ScanCodeMarketBLL().GetActivityDetail(activityId);
            if (arr.Length > 0)
            {
                Random ran = new Random();
                int i = ran.Next(0, arr.Length);
                money = (arr[i]).Value * 100;
                return money;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取网站根目录
        /// </summary>
        /// <returns></returns>
        public static string GetLocalPath()
        {
            return HttpRuntime.AppDomainAppPath.ToString();
        }
        #endregion

        /// <summary>
        /// 获取码
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 获取session中的二维码数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public CodeInfo GetSession(string ewm, int type)
        {
            CodeInfo code = null;
            if (ewm != null)
            {
                code = new ScanCodeBLL().GetCode(ewm, type);
            }
            else
            {
                code = Session["code"] as CodeInfo;
            }
            return code;
        }

        /// <summary>
        /// 红包零钱领取插入记录表，更新红包剩余数量
        /// </summary>
        /// <param name="money"></param>
        /// <param name="activityId"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public SendPacketResult SendChange(double money, long activityId, string tel)
        {
            SendPacketResult resultPacket = new SendPacketResult();
            ScanCodeMarketBLL bll = new ScanCodeMarketBLL();
            string ewm = Session["ewm"] as string;
            YX_RedSendChange change = new YX_RedSendChange { ActivityId = activityId, IDcode = ewm, RedValue = money, WeiXinUserID = Session["OpenID"] as string, SendRedID = 0 };
            if (Session["UserInfo"] as ResponseUserInfoModel != null)
            {
                ResponseUserInfoModel user = Session["UserInfo"] as ResponseUserInfoModel;
                change.Province = user.province;
                change.City = user.city;
                change.Country = user.country;
                string sex = "";
                switch (user.sex)
                {
                    case "0": sex = "未知"; break;
                    case "1": sex = "男"; break;
                    case "2": sex = "女"; break;
                }
                change.Sex = sex;
                change.WeiXinName = user.nickname;
            }
            RetResult result = bll.AddChangeRed(change, tel);
            resultPacket.Ok = result.IsSuccess;
            resultPacket.Msg = result.Msg;
            resultPacket.Money = money;
            resultPacket.Phone = tel;
            return resultPacket;
        }

        /// <summary>
        /// 领取规则
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public ActionResult ActivityRule(long activityId)
        {
            var model = new ScanCodeMarketBLL().GetActivity(0, activityId);
            if (model != null)
            {
                ViewBag.Rule = new ScanCodeMarketBLL().GetActivity(0, activityId).Content;
            }
            else
            {
                ViewBag.Rule = "参数错误！";
            }
            return View();
        }

        /// <summary>
        /// 查看活动
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public ActionResult SearchActivity(long activityId = 0, long settingId = 0)
        {
            View_Activity model = new ScanCodeMarketBLL().GetActivity(settingId, activityId);
            return View(model);
        }
    }
}
