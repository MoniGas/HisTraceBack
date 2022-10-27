using System;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.IO;
using System.Text;

namespace jqUploadify.scripts
{
    /// <summary>
    /// $codebehindclassname$ 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class upload : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                context.Response.Charset = "utf-8";

                HttpPostedFile file = context.Request.Files["Filedata"];
                string strFileDir = "/attached/images/";
                string uploadPath = context.Server.MapPath("~"+strFileDir);
                
                if (file != null)
                {
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    //生成缩略图
                    //MakeThumbnail(uploadPath + file.FileName, uploadPath + "\\s\\" + file.FileName, 80, 80);
                   
                    //后缀
                    string fileExt = Path.GetExtension(file.FileName).ToLower();
                    string strFileName = DateTime.Now.ToString("yyyyMMddhhmmssfff") + fileExt;
                    string path = context.Server.MapPath(string.Format("~{0}{1}",strFileDir,strFileName));
                    file.SaveAs(path);

                    System.Drawing.Image ig = System.Drawing.Image.FromFile(path);
                    int ow = ig.Width;
                    int oh = ig.Height;

                    //数据库保存相对路径
                    string savepath = string.Format("{0}{1}",strFileDir,strFileName);
                    string msg = string.Format("{0}{1}", uploadPath, strFileName);

                    StringBuilder strJSON = new StringBuilder("{");
                    strJSON.Append("\"code\":0,\"Msg\":\"" + msg + "\",\"sMsg\":\"" + savepath + "\"");
                    strJSON.AppendLine("}");
                    //"/attached/image/20150804024059664.jpg"

                    context.Response.Write("1|" + savepath);

                    //context.Response.Write("1|" + savepath+"|"+ow+"|"+oh); //标志位1标识上传成功，后面的可以返回前台的参数，比如上传后的路径等，中间使用|隔开
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("0||0|0");
            }
        }

        private void MakeThumbnail(string sourcePath, string newPath, int width, int height)
        {
            System.Drawing.Image ig = System.Drawing.Image.FromFile(sourcePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = ig.Width;
            int oh = ig.Height;
            if ((double)ig.Width / (double)ig.Height > (double)towidth / (double)toheight)
            {
                oh = ig.Height;
                ow = ig.Height * towidth / toheight;
                y = 0;
                x = (ig.Width - ow) / 2;

            }
            else
            {
                ow = ig.Width;
                oh = ig.Width * height / towidth;
                x = 0;
                y = (ig.Height - oh) / 2;
            }
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(ig, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ig.Dispose();
                bitmap.Dispose();
                g.Dispose();
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
