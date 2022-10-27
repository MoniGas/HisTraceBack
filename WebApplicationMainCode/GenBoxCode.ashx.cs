using System.Web;
using BLL;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using Dal;

namespace WebApplicationMainCode
{
    public class Param
    {
        public string mName { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public string batchname { get; set; }
    }
    /// <summary>
    /// GenBoxCode 的摘要说明
    /// </summary>
    public class GenBoxCode : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            Stream inputStream = context.Request.InputStream;
            Encoding encoding = context.Request.ContentEncoding;
            StreamReader streamReader = new StreamReader(inputStream, encoding);

            string strJson = streamReader.ReadToEnd();
            Param p = JsonHelper.DataContractJsonDeserialize<Param>(strJson);
            LinqModel.BaseResultList result = new RequestCodeBLL().InGetBoxList(p.mName, p.beginDate, p.endDate, p.batchname);
            context.Response.Write( new JavaScriptSerializer().Serialize(result.ObjList));
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