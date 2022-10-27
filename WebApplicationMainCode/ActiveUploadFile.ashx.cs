using System;
using System.Web;
using LinqModel;
using System.Web.Script.Serialization;

namespace WebApplicationMainCode
{
    /// <summary>
    /// ActiveUploadFile 的摘要说明
    /// </summary>
    public class ActiveUploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string BaseUrl = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SaveAddress"].ToString();
            HttpFileCollection files = context.Request.Files;
            string BatchName = context.Request["BatchName"];
            long EnterpriseId = Convert.ToInt64(context.Request["EnterpriseId"]);
            long UpUserID = Convert.ToInt64(context.Request["UpUserID"]);
            try
            {
                HttpPostedFile file = files[0];
                if (string.IsNullOrEmpty(file.FileName) == false)
                {
                    file.SaveAs(BaseUrl + DateTime.Now.ToString("yyyyMMddhhmmss") + file.FileName);//保存文件

                }
                ActiveEwmRecord ewmRecord = new ActiveEwmRecord();
                int Status = (int)Common.EnumFile.RecEwm.已接收;
                ewmRecord.PackName = file.FileName;
                ewmRecord.UpUserID = UpUserID;
                ewmRecord.EnterpriseId = EnterpriseId;
                ewmRecord.UploadDate = DateTime.Now;
                ewmRecord.RecPath = BaseUrl + DateTime.Now.ToString("yyyyMMddhhmmss") + file.FileName;
                ewmRecord.URL = "/FtpFiles/" + DateTime.Now.ToString("yyyyMMddhhmmss") + file.FileName;
                ewmRecord.Status = Status;
                ewmRecord.AddDate = DateTime.Now;
                ewmRecord.StrAddTime = DateTime.Now.ToString("yyyy-MM-dd");
                ewmRecord.OperationType = (int)Common.EnumFile.OperationType.流水线;
                ewmRecord.BatchName = BatchName;
                BLL.ActinvEwmBLL bll = new BLL.ActinvEwmBLL();
                BaseResultModel result = new BaseResultModel();
                result = bll.AddActiveRecPack(ewmRecord);
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
            }
            catch
            {
                context.Response.Write(new { message="未上传文件！"});
            }
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