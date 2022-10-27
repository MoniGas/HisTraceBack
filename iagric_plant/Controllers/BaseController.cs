using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using Newtonsoft.Json;

namespace iagric_plant.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //LoginInfo user = new LoginInfo();
            //user.PRRU_Modual_ID = 16;
            //user.EnterpriseID = 508;
            //SessCokie.Set(user);
            LoginInfo loginInfo = SessCokie.Get;
            if (loginInfo == null)
            {
                BaseResultModel result = new BaseResultModel
                {
                    code = "-100",
                    Msg = "请重新登录"
                };
                string strSerializeJson = JsonConvert.SerializeObject(result);
                //filterContext.HttpContext.Response.ContentType = "applicatin/json";
                //filterContext.HttpContext.Response.Write(strSerializeJSON);
                //filterContext.HttpContext.Response.End();
                //base.OnActionExecuting(filterContext);

                filterContext.Result = Content(strSerializeJson);
                //不用跳转方法，直接返回json数据
                //filterContext.Result = Content("<script>alert('请重新登录！');window.location.href='/Login/Login'</script>");
            }
        }
    }
}
