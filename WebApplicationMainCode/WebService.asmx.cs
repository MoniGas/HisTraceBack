using System.Web.Services;
using Dal;

namespace WebApplicationMainCode
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "http://localhost/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string GetEnterpriseMainCode(string traceCode)
        {
            TraceMainCodeDAL dal = new TraceMainCodeDAL();
            string mailcode = dal.GetEnterpriseMainCode(traceCode);
            return mailcode;
        }
    }
}
