using System;
using System.Collections.Generic;

namespace InterfaceWeb
{
    public class WxDataDAL
    {
        /// <summary>
        /// 根据自定义payId获取用户授权信息
        /// </summary>
        /// <param name="sansapiType"></param>
        /// <param name="payId"></param>
        /// <returns></returns>
        public static string GetCodeUrlBypayId(string payId,string rectUrl)
        {
            try
            {
                //本次项目写死为用户显示授权登录
                string sansapiType = "snsapi_userinfo";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", payId);
                //用微信扫码时，跳转到的相应的界面
                dic.Add("redirect_uri",rectUrl);
                dic.Add("response_type", "code");
                dic.Add("scope", sansapiType);
                string url = "https://open.weixin.qq.com/connect/oauth2/authorize";
                string data = APIHelper.GetUrl(url, dic, "#wechat_redirect");
                return data;
            }
            catch (Exception ex)
            {
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
        public static string GetAccessTokenBypayId(string code, string payId, string appScret)
        {
            try
            {
                //获取accesstoken
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", payId);
                dic.Add("secret", appScret);
                dic.Add("code", code);
                dic.Add("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                return data;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据公众号获取用户是否关注该公众号
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static string GetGzhUser(string access_token, string OpenID)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic = new Dictionary<string, string>();
                dic.Add("access_token", access_token);
                dic.Add("openid", OpenID);
                dic.Add("lang", "zh_CN");
                string url = "https://api.weixin.qq.com/cgi-bin/user/info";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                return data;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetGzhUser1(string appid, string secret)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("grant_type", "client_credential");
                dic.Add("appid", appid);
                dic.Add("secret", secret);
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string data = APIHelper.SendRequest(url, dic, "get", "");
                return data;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取关注公众号路径
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static string GetGzhUrl(string access_token, string OpenID)
        {
            try
            {
                string s = "{\"expire_seconds\": 604800, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": 123}}}";
                string url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" +access_token;
                string data = APIHelper.PostMoths(url, s);
                return data;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
