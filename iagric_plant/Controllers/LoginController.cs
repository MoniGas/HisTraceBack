using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using Common.Tools;
using System;
using System.Collections.Generic;
using Common.Log;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 赵慧敏
    /// </summary>
    public class LoginController : Controller
    {
        //public ActionResult Login()
        //{
        //    return View();
        //}

        /// <summary>
        /// 修改解析地址
        /// </summary>
        /// <returns></returns>
        public ActionResult test()
        {
            return View();
        }
        public string GetUserInfo(string accessToken, string openId)
        {
            try
            {
                //获取accesstoken
                WriteLog.WriteWxLog("【获取用户信息GetUserInfo" + DateTime.Now + "】accessToken:" + accessToken + "openId" + openId, "Wx");
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
                WriteLog.WriteWxLog("【GetUserInfo" + DateTime.Now + "】" + ex, "Wx");
                return string.Empty;
            }
        }
        //
        // GET: /Login/
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="uName"></param>
        /// <param name="uPwd"></param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        public JsonResult LoginMethod(string uName, string uPwd, string yzCode)
        {
            LoginBLL bll = new LoginBLL();
            BaseResultModel result;
            //if (Session["CheckCode"] != null && yzCode == Session["CheckCode"].ToString())
            //{
            //MaterialDIBLL bll2 = new MaterialDIBLL();
            //int result2 = bll2.GetMaterialDICount("MA.156.M0.100032.0YE660A15 ");
            result = bll.EnterpriseLogin(uName, uPwd);
            //}
            //else
            //{
            //    result = ToJson.NewRetResultToJson("0", "验证码错误！");
            //}
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public JsonResult ExitSignOut()
        {
            BaseResultModel result = new BaseResultModel();
            SessCokie.Set(null);
            result = ToJson.NewRetResultToJson("1", "");
            return Json(result);
        }

        /// <summary>
        /// 获取企业是否为试用企业
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEnterpriseVerify()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            result = ToJson.NewRetResultToJson(pf.Verify.ToString(), "");
            return Json(result);
        }

        public JsonResult Set(string id)
        {
            BaseResultModel result = new BaseResultModel();
            LoginBLL bll = new LoginBLL();
            int number = bll.SetInfo(id);
            result.code = "";
            result.Msg = number.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetHome()
        {
            JsonResult js = new JsonResult();
            LoginBLL bll = new LoginBLL();
            RetResult ret = bll.SetHome();
            js.Data = new { res = ret.IsSuccess, info = ret.Msg };
            return js;
        }

        #region 20170726 医疗器械（UDI）服务云平台
        /// <summary>
        /// 平台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeIndex()
        {
            ViewBag.Value = 1;
            return View();
        }

        /// <summary>
        /// 平台介绍
        /// </summary>
        /// <returns></returns>
        public ActionResult Architecture()
        {
            ViewBag.Value = 2;
            return View();
        }

        /// <summary>
        /// 功能介绍
        /// </summary>
        /// <returns></returns>
        public ActionResult Function()
        {
            ViewBag.Value = 3;
            return View();
        }

        /// <summary>
        /// 赋码方式
        /// </summary>
        /// <returns></returns>
        public ActionResult Assignment()
        {
            ViewBag.Value = 4;
            return View();
        }

        /// <summary>
        /// 扫码激活
        /// </summary>
        /// <returns></returns>
        public ActionResult ScanCodeActivity()
        {
            ViewBag.Value = 5;
            return View();
        }

        /// <summary>
        /// 关于我们
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            ViewBag.Value = 6;
            return View();
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <returns></returns>
        public ActionResult Accessory()
        {
            return View();
        }

        /// <summary>
        /// 手机官网
        /// </summary>
        /// <returns></returns>
        public ActionResult WapIndex()
        {
            return View();
        }

        /// <summary>
        /// 资料下载
        /// </summary>
        /// <returns></returns>
        public ActionResult DataDown()
        {
            ViewBag.Value = 7;
            return View();
        }
        #endregion
    }
}
