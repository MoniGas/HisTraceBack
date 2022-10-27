using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;

namespace Common.Tools
{
    public class FtpUpFile
    {
        public static void CreatW(string url, string uid, string pwd)
        {
            FtpWebRequest frequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            frequest.Credentials = new NetworkCredential(uid, pwd);
            frequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = frequest.GetResponse() as FtpWebResponse;
            }
            catch { }
        }

        public static bool FtpUpload(HttpPostedFileBase hpf, string fileName, string uid, string pwd, out string msg)
        {

            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(fileName));
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(uid, pwd);

            reqFTP.KeepAlive = false;
            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = hpf.InputStream.Length;

            // 缓冲大小设置为kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流(System.IO.FileStream) 去读上传的文件
            try
            {
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb
                contentLen = hpf.InputStream.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = hpf.InputStream.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                hpf.InputStream.Close();
                msg = "完成";
                return true;
            }
            catch (Exception ex)
            {
                msg = string.Format("因{0},无法完成上传", ex.Message);
                return false;
            }
        }

        public static bool FtpUpload(Image image, string fileName, string uid, string pwd, out string msg)
        {
            Byte[] bytes;
            //Image image = System.Drawing.Image.FromStream(hpf.InputStream, true);
            //object aa = image.RawFormat;
            MemoryStream ms = new MemoryStream();
            //image.Save(ms, image.RawFormat);
            if (fileName.Contains(".gif"))
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            }
            else
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            bytes = ms.ToArray();
            Stream ms2 = new MemoryStream(bytes);

            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(fileName));
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(uid, pwd);

            reqFTP.KeepAlive = false;
            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = ms2.Length;

            // 缓冲大小设置为kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流(System.IO.FileStream) 去读上传的文件
            try
            {
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb
                contentLen = ms2.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = ms2.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                ms2.Close();
                msg = "完成";
                return true;
            }
            catch (Exception ex)
            {
                msg = string.Format("因{0},无法完成上传", ex.Message);
                return false;
            }
        }

        private static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        private static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public static byte[] ImageToByteArray(Image img)
        {
            ImageConverter imgconv = new ImageConverter();
            byte[] b = (byte[])imgconv.ConvertTo(img, typeof(byte[]));
            return b;
        }

        public static Image ByteArrayToImage(byte[] b)
        {
            ImageConverter imgconv = new ImageConverter();
            Image img = (Image)imgconv.ConvertFrom(b);
            return img;
        }

        #region 保存web图片到本地
        /// <summary>
        /// 保存web图片到本地
        /// </summary>
        /// <param name="imgUrl">web图片路径</param>
        /// <param name="path">保存路径</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns></returns>
        public static string SaveImageFromWeb(string imgUrl, string path, string fileName)
        {
            if (path.Equals(""))
                throw new Exception("未指定保存文件的路径");
            string imgName = imgUrl.Substring(imgUrl.LastIndexOf("/") + 1);
            string defaultType = ".jpg";
            string[] imgTypes = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            string imgType = imgUrl.Substring(imgUrl.LastIndexOf("."));
            string imgPath = "";
            foreach (string it in imgTypes)
            {
                if (imgType.ToLower().Equals(it))
                    break;
                if (it.Equals(".bmp"))
                    imgType = defaultType;
            }
            WebClient myWebClient = new WebClient();
            //将头像保存到服务器
            string virPath = "/";
            // CreateDir(virPath);
           // string fileName = Guid.NewGuid()+ ".png";
            myWebClient.DownloadFile(imgUrl, HttpContext.Current.Request.PhysicalApplicationPath + virPath + fileName);
            //user.Portrait = virPath + fileName;
            imgPath = fileName + imgType;
            return imgPath;

        }
        #endregion
    }
}
