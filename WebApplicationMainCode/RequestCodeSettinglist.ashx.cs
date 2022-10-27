using System.Web;
using System.Text;
using System.IO;
using Dal;
using BLL;
using LinqModel;
using System.Web.Script.Serialization;

namespace WebApplicationMainCode
{
    public class ReParam
    {
        public string PackagingIine { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string MaterialFullName { get; set; }
        public string Count { get; set; }
        public string BatchName { get; set; }
        public string Spec { get; set; }
    }

    /// <summary>
    /// RequestCodeSettinglist 的摘要说明
    /// </summary>
    public class RequestCodeSettinglist : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            Stream inputStream = context.Request.InputStream;
            Encoding encoding = context.Request.ContentEncoding;
            StreamReader streamReader = new StreamReader(inputStream, encoding);

            string strJson = streamReader.ReadToEnd();
            ReParam r = JsonHelper.DataContractJsonDeserialize<ReParam>(strJson);
            RequestCodeMaBLL bll = new RequestCodeMaBLL();
            BaseResultList result = bll.GetInRequestCodeSettingListAll(r.PackagingIine, r.BeginDate, r.EndDate, r.MaterialFullName, r.Count, r.BatchName, r.Spec);
            context.Response.Write(new JavaScriptSerializer().Serialize(result.ObjList));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}