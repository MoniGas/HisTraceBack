/*****************************************************************
代码功能：警察信息控制器
开发日期：2016年03月29日
作    者：陈志钢（复用其他系统代码）
联系方式：13933876661
版权所有：河北广联信息技术有限公司研发一部    
******************************************************************/
using System;
using System.Web;
using System.IO;

namespace MvcWeb.BLL
{
    /// <summary>
    /// FilesUpLoad 的摘要说明
    /// </summary>
    public class FilesUpLoadOriginal : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Charset = "utf-8";
            //获取上传文件队列  
            HttpPostedFile oFile = context.Request.Files["Filedata"];
            HttpPostedFileBase oFile1 = new HttpPostedFileWrapper(oFile);
            if (oFile != null)
            {
                string topDir = context.Request["folder"];  // 获取uploadify的folder配置，在此示例中，客户端配置了上传到 Files/ 文件夹
                // 检测并创建目录:当月上传的文件放到以当月命名的文件夹中，例如2011年11月的文件放到网站根目录下的 /Files/201111 里面
                string dateFolder = HttpContext.Current.Server.MapPath(topDir) + "\\" + DateTime.Now.Date.ToString("yyyyMM");
                if (!Directory.Exists(dateFolder))  // 检测是否存在磁盘目录
                {
                    Directory.CreateDirectory(dateFolder);  // 不存在的情况下，创建这个文件目录 例如 C:/wwwroot/Files/201111/
                }
                // 使用Guid命名文件，确保每次文件名不会重复
                string guidFileName = Guid.NewGuid() + Path.GetExtension(oFile.FileName).ToLower();
                oFile.SaveAs(dateFolder + "\\" + guidFileName);
                //}
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //////// TODO 在此，您可以添加自己的业务逻辑，比如保存这个文件信息到数据库
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // 上面的所有操作顺利完成，你就完成了一个文件的上传（和保存信息到数据库），返回成功，在此我返回1，表示上传了一个文件
                string mess = "/Files/" + DateTime.Now.Date.ToString("yyyyMM") + "/" + guidFileName;
                context.Response.Write(mess);
            }
            else
            {
                context.Response.Write("0");
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