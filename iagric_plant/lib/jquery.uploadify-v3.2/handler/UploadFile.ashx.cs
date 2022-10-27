using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Common.Argument;
using Common.Tools;
using System.Diagnostics;

namespace iagric_plant.ashx
{
    /// <summary>
    /// UploadFile 的摘要说明
    /// </summary>
    public class UploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            #region
            try
            {

                context.Response.ContentType = "text/plain";
                context.Response.Charset = "utf-8";

                // HttpPostedFile oFile = context.Request.Files["Filedata"];
                HttpPostedFile oFile = context.Request.Files[0];
                if (oFile != null)
                {
                    HttpPostedFileBase oFile1 = new HttpPostedFileWrapper(oFile);
                    LoginInfo pf = SessCokie.Get;

                    string ftpUrl = ConfigurationManager.AppSettings["FtpUrl"];
                    string e = string.Empty;
                    string date = DateTime.Now.ToString("yyyyMM");
                    if (pf == null)
                        e = "Public";
                    else if (pf.PRRU_PlatFormLevel_ID == (int)Common.EnumFile.PlatFormLevel.Enterprise)
                        e = "E" + pf.EnterpriseID;
                    else
                        e = "P" + pf.EnterpriseID;

                    string uid = ConfigurationManager.AppSettings["FtpLoginName"];
                    string pwd = ConfigurationManager.AppSettings["FtpPWD"];

                    string guidFileName = Guid.NewGuid() + Path.GetExtension(oFile.FileName).ToLower();

                    FtpUpFile.CreatW(ftpUrl + e, uid, pwd);
                    FtpUpFile.CreatW(ftpUrl + e + "/" + date, uid, pwd);
                    string msg = string.Empty;
                    string bfullName = ftpUrl + e + "/" + date + "/" + guidFileName;
                    string httpURL = ConfigurationManager.AppSettings["HttpUrl"];
                    string[] extArr = ConfigurationManager.AppSettings["PictureExtension"].Split('|');
                    string[] extArrVideo = ConfigurationManager.AppSettings["VideoExtension"].Split('|');
                    string[] extArrExcel = ConfigurationManager.AppSettings["ExcelExtension"].Split('|');
                    string[] extArrTxt = ConfigurationManager.AppSettings["TxtExtension"].Split('|');

                    string ext = Path.GetExtension(bfullName).Substring(1);
                    #region 是图片上传
                    if (extArr.Contains(ext))
                    {
                        if (ext == "gif")
                        {
                            string sfullName = ftpUrl + e + "/" + date + "/small_" + guidFileName;
                            Image yImage = Image.FromStream(oFile1.InputStream, true);
                            //Image sImage = UploadImage.GetAutoControlWideImg(yImage, 200, "", "", UploadImage.WaterMarkPosition.WMP_Left_Bottom, UploadImage.WaterMarkPosition.WMP_Left_Bottom);
                            //Image bImage = UploadImage.GetAutoControlWideImg(yImage, 600, "", "", UploadImage.WaterMarkPosition.WMP_Left_Bottom, UploadImage.WaterMarkPosition.WMP_Left_Bottom);
                            if (FtpUpFile.FtpUpload(yImage, bfullName, uid, pwd, out msg))
                            {
                                if (FtpUpFile.FtpUpload(yImage, sfullName, uid, pwd, out msg))
                                {
                                    StringBuilder strJSON = new StringBuilder("{");
                                    strJSON.Append("\"code\":0,\"Msg\":\"" + bfullName.Replace(ftpUrl, httpURL) + "\",\"sMsg\":\"" + sfullName.Replace(ftpUrl, httpURL) + "\"");
                                    strJSON.AppendLine("}");

                                    context.Response.Write(strJSON);
                                }
                                else
                                {
                                    StringBuilder strJSON = new StringBuilder("{");
                                    strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                                    strJSON.AppendLine("}");

                                    context.Response.Write(strJSON);
                                }
                            }
                            else
                            {
                                StringBuilder strJSON = new StringBuilder("{");
                                strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                                strJSON.AppendLine("}");

                                context.Response.Write(strJSON);
                            }
                        }
                        else
                        {
                            string sfullName = ftpUrl + e + "/" + date + "/small_" + guidFileName;
                            Image yImage = Image.FromStream(oFile1.InputStream, true);
                            Image sImage = UploadImage.GetAutoControlWideImg(yImage, 200, "", "", UploadImage.WaterMarkPosition.WMP_Left_Bottom, UploadImage.WaterMarkPosition.WMP_Left_Bottom);
                            Image bImage = UploadImage.GetAutoControlWideImg(yImage, 600, "", "", UploadImage.WaterMarkPosition.WMP_Left_Bottom, UploadImage.WaterMarkPosition.WMP_Left_Bottom);
                            if (FtpUpFile.FtpUpload(bImage, bfullName, uid, pwd, out msg))
                            {
                                if (FtpUpFile.FtpUpload(sImage, sfullName, uid, pwd, out msg))
                                {
                                    StringBuilder strJSON = new StringBuilder("{");
                                    strJSON.Append("\"code\":0,\"Msg\":\"" + bfullName.Replace(ftpUrl, httpURL) + "\",\"sMsg\":\"" + sfullName.Replace(ftpUrl, httpURL) + "\"");
                                    strJSON.AppendLine("}");

                                    context.Response.Write(strJSON);
                                }
                                else
                                {
                                    StringBuilder strJSON = new StringBuilder("{");
                                    strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                                    strJSON.AppendLine("}");

                                    context.Response.Write(strJSON);
                                }
                            }
                            else
                            {
                                StringBuilder strJSON = new StringBuilder("{");
                                strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                                strJSON.AppendLine("}");

                                context.Response.Write(strJSON);
                            }
                        }
                    }
                    #endregion

                    #region 视频上传
                    else if (extArrVideo.Contains(ext))
                    {
                        bool v = FtpUpFile.FtpUpload(oFile1, bfullName, uid, pwd, out msg);
                        if (v)
                        {
                            string pathtool = HttpContext.Current.Server.MapPath("//bll/ffmpeg.exe");

                            string path = HttpContext.Current.Server.MapPath("//FtpFiles/" + e + "/" + date + "/" + guidFileName);

                            string pathImg = HttpContext.Current.Server.MapPath("//FtpFiles/" + e + "/" + date + "/" + guidFileName + ".jpg");

                            string CutTimeFrame = "1";
                            string WidthAndHeight = "240*180";

                            ProcessStartInfo startInfo = new ProcessStartInfo(pathtool);
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            if (path.IndexOf(".mp4") > 0)
                            {
                                startInfo.Arguments = " -i " + path + " -y -f image2 -t 0.001 -s " + WidthAndHeight + " " + pathImg;
                            }
                            else
                            {
                                startInfo.Arguments = " -i " + path + " -y -f image2 -ss " + CutTimeFrame + " -t 0.001 -s " + WidthAndHeight + " " + pathImg;  //設定程式執行參數
                            }
                            Process.Start(startInfo);

                            System.Threading.Thread.Sleep(2000);

                            string abc = httpURL + e + "/" + date + "/" + guidFileName + ".jpg";
                            try
                            {
                                string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["FtpPath"] + e + "/" + date + "/", guidFileName + ".jpg");
                                if (files == null || files.Length <= 0)
                                {
                                    abc = "/images/video-def-app.jpg";
                                }
                            }
                            catch
                            {
                                abc = "/images/video-def-app.jpg";
                            }

                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":0,\"Msg\":\"" + bfullName.Replace(ftpUrl, httpURL) + "\"" + ",\"sMsg\":\"" + abc + "\"");
                            strJSON.AppendLine("}");
                            string ss = strJSON.ToString();

                            context.Response.Write(strJSON);
                        }
                        else
                        {
                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                            strJSON.AppendLine("}");

                            context.Response.Write(strJSON);
                        }

                    }
                    #endregion

                    #region Excel上传
                    else if (extArrExcel.Contains(ext))
                    {

                        //HttpPostedFileBase oFile2 = new HttpPostedFileWrapper(oFile);
                        string pathtool = HttpContext.Current.Server.MapPath("//bll/ffmpeg.exe");
                        string path = "\\FtpFiles\\" + "\\" + "\\" + guidFileName;
                        path = HttpContext.Current.Server.MapPath("~" + path);
                        oFile1.SaveAs(path);
                        bool v = FtpUpFile.FtpUpload(oFile1, bfullName, uid, pwd, out msg);
                        if (v)
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo(pathtool);
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                            Process.Start(startInfo);

                            System.Threading.Thread.Sleep(2000);

                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":0,\"Msg\":\"" + bfullName.Replace(ftpUrl, httpURL) + "\"" + ",\"sMsg\":\"" + path + "\"");
                            strJSON.AppendLine("}");
                            string ss = strJSON.ToString();

                            context.Response.Write(strJSON);
                        }
                        else
                        {
                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                            strJSON.AppendLine("}");

                            context.Response.Write(strJSON);
                        }

                    }
                    #endregion
                    #region txt上传
                    else if (extArrTxt.Contains(ext))
                    {

                        //HttpPostedFileBase oFile2 = new HttpPostedFileWrapper(oFile);
                        string pathtool = HttpContext.Current.Server.MapPath("//bll/ffmpeg.exe");
                        string aaa = oFile.FileName;
                        string path1 = "\\FtpFiles\\" + "\\" + "\\" + guidFileName;
                        string path2 = "\\FtpFiles\\" + guidFileName;
                        string path = HttpContext.Current.Server.MapPath("~" + path1);
                        oFile1.SaveAs(path);
                        bool v = FtpUpFile.FtpUpload(oFile1, bfullName, uid, pwd, out msg);
                        if (v)
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo(pathtool);
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                            Process.Start(startInfo);

                            System.Threading.Thread.Sleep(2000);

                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":0,\"Msg\":\"" + aaa + "\"" + ",\"sMsg\":\"" + path + "\",\"url\":\"" + path2 + "\"");
                            strJSON.AppendLine("}");
                            string ss = strJSON.ToString();

                            context.Response.Write(strJSON);
                        }
                        else
                        {
                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                            strJSON.AppendLine("}");

                            context.Response.Write(strJSON);
                        }

                    }
                    #endregion
                    else
                    {
                        bool v = FtpUpFile.FtpUpload(oFile1, bfullName, uid, pwd, out msg);
                        if (v)
                        {
                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":0,\"Msg\":\"" + bfullName.Replace(ftpUrl, httpURL) + "\"");
                            strJSON.AppendLine("}");

                            context.Response.Write(strJSON);
                        }
                        else
                        {
                            StringBuilder strJSON = new StringBuilder("{");
                            strJSON.Append("\"code\":-1,\"Msg\":\"" + msg + "\",\"sMsg\":\"\"");
                            strJSON.AppendLine("}");

                            context.Response.Write(strJSON);
                        }
                    }
                }
                else
                {
                    StringBuilder strJSON = new StringBuilder("{");
                    strJSON.Append("\"code\":-1,\"Msg\":\"没有要上传的文件！\",\"sMsg\":\"\"");
                    strJSON.AppendLine("}");

                    context.Response.Write(strJSON);
                }
            }
            catch (Exception ex)
            {
                StringBuilder strJSON = new StringBuilder("{");
                strJSON.Append("\"code\":-1,\"Msg\":\"" + ex + "\",\"sMsg\":\"\"");
                strJSON.AppendLine("}");

                context.Response.Write(strJSON);
            }
            #endregion
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