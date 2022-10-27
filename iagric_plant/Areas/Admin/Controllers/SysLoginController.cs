/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-10-28

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于监管部门登录的控制器层

*********************************************************************************/

using System.Web.Mvc;
using BLL;
using LinqModel;
using Common.Argument;


namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysLoginController : Controller
    {
        //
        // GET: /Admin/SysLogin/

        public ActionResult SysLogin()
        {
            return View();
        }

        //
        // GET: /Login/
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="uName">用户名</param>
        /// <param name="uPwd">密码</param>
        /// <param name="yzCode">验证码</param>
        /// <returns>登录结果</returns>
        public JsonResult LoginMethod(string uName, string uPwd, string yzCode)
        {
            // 实例化登录业务层对象
            LoginBLL bll = new LoginBLL();
            // 实例化返回值对象 
            BaseResultModel result;
            // 判断验证码有值
            if (Session["CheckCode"] != null && yzCode == Session["CheckCode"].ToString())
            {
                // 调用登录方法
                result = bll.SysLogin(uName, uPwd);
            }
            else
            {
                // 反悔验证码错误
                result = ToJson.NewRetResultToJson("0", "验证码错误！");
            }
            // 返回登录结果
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public JsonResult ExitSignOut()
        {
            SessCokie.Set(null);
            var result = ToJson.NewRetResultToJson("1", "");
            return Json(result);
        }

        
    }
}
