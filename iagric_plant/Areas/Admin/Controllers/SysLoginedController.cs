using System.Web.Mvc;
using Common.Argument;
using BLL;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class SysLoginedController : BaseController
    {
        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLoginInfo()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            LoginBLL bll = new LoginBLL();
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = bll.GetLoginInfo();
            }
            catch
            { }
            return Json(result);
        }

        /// <summary>
        /// 获取模块信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSysModuleList(int parentID)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            LoginBLL bll = new LoginBLL();
            BaseResultList result = new BaseResultList();
            try
            {
                result = bll.GetSysModuleList(parentID);
            }
            catch
            { }
            return Json(result);
        }

        /// <summary>
        /// 判断用户是否进入引导页
        /// </summary>
        /// <returns></returns>
        public JsonResult Guide()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = ToJson.NewRetResultToJson(pf.NewUser.ToString(), "");
            }
            catch
            { }
            return Json(result);
        }

        /// <summary>
        /// 判断用户是否进入引导页
        /// </summary>
        /// <returns></returns>
        public JsonResult IsLogin()
        {
            LoginInfo loginInfo = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            if (loginInfo == null)
            {
                result.code = "-100";
                result.Msg = "请重新登录";
            }
            else
            {
                result.code = "100";
                result.Msg = "登录成功";
            }
            return Json(result);
        }

    }
}

