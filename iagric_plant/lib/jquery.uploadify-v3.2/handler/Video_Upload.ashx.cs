using System;
using System.Web;
using System.IO;

namespace HebSociaOrg.Admin.lib.jQueryUpLoadify
{
    /// <summary>
    /// Video_Upload 的摘要说明
    /// </summary>
    public class Video_Upload : IHttpHandler
    {


        #region  旧

        public void ProcessRequest(HttpContext context)
        {
            HttpPostedFile file = context.Request.Files["FileData"];
            //string uploadpath = context.Server.MapPath(context.Request["folder"] + "\\");
            string strFileDir = "/attached/";
            string uploadpath = context.Server.MapPath("~" + strFileDir);
            if (file != null)
            {
                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }
                ///后缀
                string fileExt = Path.GetExtension(file.FileName).ToString().ToLower();
                string name = DateTime.Now.ToString("yyyyMMddhhmmssfff") + fileExt;
                //图片路径
                string path = context.Server.MapPath(string.Format("~{0}{1}", strFileDir, name));
                //string path = uploadpath + name;
                file.SaveAs(path);
                //数据库保存相对路径
                //string savepath = context.Request["folder"] + "/" + name;
                string savepath = string.Format("{0}{1}", strFileDir, name);
                context.Response.Write("1|" + savepath); //标志位1标识上传成功，后面的可以返回前台的参数，比如上传后的路径等，中间使用|隔开
            }
            else
            {
                context.Response.Write("0|");
            }
        }


        #endregion


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}