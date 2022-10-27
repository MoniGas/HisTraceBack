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
using Common.Argument;
using Common.Tools;
using System.Text;
using System.Runtime.InteropServices;

namespace MvcWeb.BLL
{
    /// <summary>
    /// FilesUpLoad 的摘要说明
    /// </summary>
    public class FilesUpLoad : IHttpHandler
    {
        /// <summary>
        /// 上传处理
        /// </summary>
        /// <param name="context">上传内容</param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Charset = "utf-8";
            if (context.Request["type"] == "1")
            {
                //获取上传文件队列  
               // HttpPostedFile oFile = context.Request.Files["Filedata"];
                HttpPostedFile oFile = context.Request.Files[0];
                if (oFile != null)
                {
                    HttpPostedFileBase oFile1 = new HttpPostedFileWrapper(oFile);
                    string topDir = context.Request["folder"];  // 获取uploadify的folder配置，在此示例中，客户端配置了上传到 Files/ 文件夹
                    // 检测并创建目录:当月上传的文件放到以当月命名的文件夹中，例如2011年11月的文件放到网站根目录下的 /Files/201111 里面
                    string dateFolder = HttpContext.Current.Server.MapPath(topDir) + "\\" + DateTime.Now.Date.ToString("yyyyMM");
                    if (!Directory.Exists(dateFolder))  // 检测是否存在磁盘目录
                    {
                        Directory.CreateDirectory(dateFolder);  // 不存在的情况下，创建这个文件目录 例如 C:/wwwroot/Files/201111/
                    }
                    // 使用Guid命名文件，确保每次文件名不会重复
                    string guidFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(oFile.FileName).ToLower();
                    //原图片地址
                    MvcWebCommon.Tools.UploadImage.ZoomAutoStream(oFile1.InputStream, topDir + DateTime.Now.ToString("yyyyMM") + "/" + guidFileName,
                        600, 600, "", "", MvcWebCommon.Tools.UploadImage.WaterMarkPosition.WMP_Right_Top, MvcWebCommon.Tools.UploadImage.WaterMarkPosition.WMP_Right_Top);
                    string mess = topDir + DateTime.Now.Date.ToString("yyyyMM") + "/" + guidFileName;
                    context.Response.Write(mess);
                }
                else
                {
                    context.Response.Write("0");
                }
            }
            else if (context.Request["type"] == "3")
            {
                LoginInfo user = SessCokie.Get;
                //获取上传文件队列  
                //HttpPostedFile oFile = context.Request.Files["Filedata"];
                HttpPostedFile oFile = context.Request.Files[0];
                if (oFile != null)
                {
                    HttpPostedFileBase oFile1 = new HttpPostedFileWrapper(oFile);
                    //string path = "\\FtpFiles\\" + DateTime.Now.Date.ToString("yyyyMM") + "\\" + user.EnterpriseID + "\\" + "\\" + oFile1.FileName;
                    //path = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                    string topDir = context.Request["folder"];  // 获取uploadify的folder配置，在此示例中，客户端配置了上传到 Files/ 文件夹
                    // 检测并创建目录:当月上传的文件放到以当月命名的文件夹中，例如2011年11月的文件放到网站根目录下的 /Files/201111 里面
                    string dateFolder = HttpContext.Current.Server.MapPath(topDir) + DateTime.Now.Date.ToString("yyyyMM") + "\\" + user.EnterpriseID;
                    if (!Directory.Exists(dateFolder))  // 检测是否存在磁盘目录
                    {
                        Directory.CreateDirectory(dateFolder);  // 不存在的情况下，创建这个文件目录 例如 C:/wwwroot/Files/201111/
                    }
                    string path = dateFolder + "\\" + oFile1.FileName;
                    //保存文件
                    oFile1.SaveAs(path);
                    //string mess = topDir + DateTime.Now.Date.ToString("yyyyMM") + "/" + user.EnterpriseID + "/" + oFile1.FileName;
                    string mess = "Areas/Market/Files/APIZShu/" + DateTime.Now.Date.ToString("yyyyMM") + "/" + user.EnterpriseID + "/" + oFile1.FileName;
                    context.Response.Write(mess);
                }
                else
                {
                    context.Response.Write("0");
                }
            }
            else if (context.Request["type"] == "2")
            {
                string path = HttpContext.Current.Server.MapPath("~" + context.Request["imgPath"] ?? "");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            #region 20190213新加excel
            else if (context.Request["type"] == "4")
            {
                //获取上传文件队列  
                HttpPostedFile oFile = context.Request.Files["Filedata"];
                HttpPostedFileBase oFile1 = new HttpPostedFileWrapper(oFile);
                if (oFile != null)
                {
                    string topDir = context.Request["folder"];  // 获取uploadify的folder配置，在此示例中，客户端配置了上传到 Files/ 文件夹
                    string dateFolder = HttpContext.Current.Server.MapPath(topDir) + "\\" + DateTime.Now.Date.ToString("yyyyMM");
                    if (!Directory.Exists(dateFolder))  // 检测是否存在磁盘目录
                    {
                        Directory.CreateDirectory(dateFolder);  // 不存在的情况下，创建这个文件目录 例如 C:/wwwroot/Files/201111/
                    }
                    // 使用Guid命名文件，确保每次文件名不会重复
                    string guidFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(oFile.FileName).ToLower();
                    string mess = dateFolder + "\\" + guidFileName;
                    oFile1.SaveAs(mess);
                    string result = topDir + DateTime.Now.Date.ToString("yyyyMM") + "/" + guidFileName + "~" + oFile.FileName + "#" + mess;
                    //result = System.Web.HttpUtility.UrlEncode(result);
                    context.Response.Write(result);
                }
                else
                {
                    context.Response.Write("0");
                }
            }
            #endregion
            else if (context.Request["type"] == "5")//Excel上传20200819
            {
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
                    string guidFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(oFile.FileName).ToLower();
                    string path = dateFolder + "\\" + oFile1.FileName;
                    //保存文件
                    oFile1.SaveAs(path);
                    //string mess = topDir + DateTime.Now.Date.ToString("yyyyMM") + "/" + user.EnterpriseID + "/" + oFile1.FileName;
                    //string mess = "/Areas/Admin/Files/Excel/" + DateTime.Now.Date.ToString("yyyyMM") + "/"  + oFile1.FileName;
                    string mess = path;
                    context.Response.Write(mess);
                }
                else
                {
                    context.Response.Write("0");
                }
            }
            #region 20200514 新加 标签模板压缩文件
            else if (context.Request["type"] == "6")
            {
                //获取上传文件队列  
                // HttpPostedFile oFile = context.Request.Files["Filedata"];
                HttpPostedFile oFile = context.Request.Files[0];
                if (oFile != null)
                {
                    try
                    {
                        int code = 1;
                        string msg = "";
                        string extension = Path.GetExtension(oFile.FileName).ToLower().Substring(1);
                        if (extension != "zip") 
                        {
                            code = 0;
                            msg = "产生错误：模板文件需为.zip文件";
                            context.Response.Write(code + ";" + msg);
                            return;
                        }
                        byte[] content = new byte[oFile.InputStream.Length];
                        oFile.InputStream.Read(content, 0, (int)oFile.InputStream.Length);
                        HttpPostedFileBase oFile1 = new HttpPostedFileWrapper(oFile);
                        string topDir = context.Request["folder"];  // 获取uploadify的folder配置，在此示例中，客户端配置了上传到 Files/ 文件夹
                        // 检测并创建目录:当月上传的文件放到以当月命名的文件夹中，例如2011年11月的文件放到网站根目录下的 /Files/201111 里面
                        string filenamedate=DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string dateFolder = HttpContext.Current.Server.MapPath(topDir) + filenamedate;
                        if (!Directory.Exists(dateFolder))  // 检测是否存在磁盘目录
                        {
                            Directory.CreateDirectory(dateFolder);  // 不存在的情况下，创建这个文件目录 例如 C:/wwwroot/Files/201111/
                        }
                        string guidFileName = DateTime.Now.ToString("yyyyMMdd") + Path.GetExtension(oFile.FileName).ToLower();
                        string mess = dateFolder + "\\" + guidFileName;
                        oFile.SaveAs(mess);
                        UnZipClass.UnZip(mess, dateFolder, "123456");
                        if (!File.Exists(dateFolder + "\\setting.ini"))
                        {
                            code = 0;
                            msg = "产生错误：模板文件错误,缺少setting.ini";
                            context.Response.Write(code + ";" + msg);
                            return;
                        }
                        string width = ReadIniData("Setting", "Width", "99", dateFolder + "\\setting.ini");
                        string height = ReadIniData("Setting", "Height", "43", dateFolder + "\\setting.ini");
                        string Name = ReadIniData("Setting", "Name", "code", dateFolder + "\\setting.ini");
                        if (!File.Exists(dateFolder + "\\Modal.frx"))
                        {
                            code = 0;
                            msg = "产生错误：模板文件错误,缺少Modal.frx";
                            context.Response.Write(code + ";" + msg + ";" + width + ";" + height + ";" + Name );
                            return;
                        }
                        if (!File.Exists(dateFolder + "\\ModalData.xml"))
                        {
                            code = 0;
                            msg = "产生错误：模板文件错误,缺少ModalData.xml";
                            context.Response.Write(code + ";" + msg + ";" + width + ";" + height + ";" + Name + ";" + "" + ";" + "");
                            return;
                        }
                        if (!File.Exists(dateFolder + "\\show.jpg"))
                        {
                            code = 0;
                            msg = "产生错误：模板文件错误,缺少show.jpg";
                            context.Response.Write(code + ";" + msg + ";" + width + ";" + height + ";" + Name );
                            return;
                        }
                        string value=code+";"+msg+";"+width +";"+ height +";" + Name;
                        string imgurl = "http://"+HttpContext.Current.Request.Url.Authority + topDir + filenamedate + "/show.jpg";
                        string urlzip = "http://"+HttpContext.Current.Request.Url.Authority + topDir  + filenamedate+"/" + guidFileName;
                        context.Response.Write(code + ";" + msg + ";" + width + ";" + height + ";" + Name + ";" + imgurl + ";" + urlzip);
                    }
                    catch (Exception ex) 
                    {
                    
                    }
                    
                }
                else
                {
                    context.Response.Write("0");
                }
            }
            #endregion
            else
            {
                string path = HttpContext.Current.Server.MapPath("~" + context.Request["imgPath"] ?? "");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
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