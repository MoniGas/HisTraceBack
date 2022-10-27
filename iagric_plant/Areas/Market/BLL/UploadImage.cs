/*****************************************************************
代码功能：警察信息控制器
开发日期：2016年03月29日
作    者：陈志钢（复用其他系统个代码）
联系方式：13933876661
版权所有：河北广联信息技术有限公司研发一部    
******************************************************************/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;

namespace MvcWebCommon.Tools
{
    public class UploadImage
    {
        #region 判断上传格式
        /// <summary>
        /// 判断上传文件格式是否正确
        /// </summary>
        /// <param name="UPFile">HttpPostedFile</param>
        /// <param name="MaxSize">上传文件最大值，单位K</param>
        /// <param name="type">允许的上传文类型</param>
        /// <param name="message">返回错误/正确的信息</param>
        /// <returns></returns>
        public static bool CheckFile(System.Web.HttpPostedFileBase UPFile, int MaxSize, string[] type, out string message)
        {
            //获取文件扩展名
            string _extension = Path.GetExtension(UPFile.FileName).ToLower();

            bool isTrue = false;
            //获取指定的扩展名
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < type.Length; i++)
            {
                if (_extension == type[i].ToLower())
                {
                    isTrue = true;
                }
                sb.Append(type[i].ToLower());
                if (i < type.Length - 1)
                    sb.Append("/");
            }

            if (isTrue == false)
            {
                message = "上传文件格式错误，上传文件格式只支持：" + sb.ToString();
                return false;
            }
            else
            {
                if (UPFile.ContentLength / 1024 > MaxSize)
                {
                    message = "上传图片文件不能超过" + MaxSize.ToString() + "K，请重新填写后再提交";
                    return false;
                }
                else
                {
                    message = "验证正确";
                    return true;
                }
            }
        }
        #endregion

        /// <summary> 
        /// 图片上传，原图+缩放后图片
        /// </summary> 
        /// <remarks>吴剑 2011-01-21</remarks> 
        /// <param name="postedFile">原图HttpPostedFile对象</param> 
        /// <param name="savePath">缩略图存放地址,如/UpFile/File//</param> 
        /// <param name="targetWidth">缩略图宽度</param> 
        /// <param name="normalWidth">大图宽度</param> 
        /// <param name="saveNormal">是否保存原图</param>
        /// <param name="newName">所生成图片名称，newName[0]缩放后图片，newName[1]原图</param>
        /// <returns>是否操作成功</returns>
        public static bool ZoomAuto(System.Web.HttpPostedFileBase postedFile, string savePath, System.Double targetWidth, System.Double normalWidth, bool saveNormal, out string[] newName)
        {
            newName = new string[2];
            string ext = Path.GetExtension(postedFile.FileName);
            string sDate = DateTime.Now.ToString("yyyyMMddhhMMssfff");
            string autoName = sDate + "_" + targetWidth.ToString() + ext;//小图名称
            string name = sDate + ext;//大图名称
            try
            {
                ZoomAutoControlWide(postedFile, savePath + autoName, targetWidth, "", "", WaterMarkPosition.WMP_Left_Bottom, WaterMarkPosition.WMP_Left_Top);
                newName[0] = autoName;
                if (saveNormal)
                {
                    //postedFile.SaveAs(HttpContext.Current.Server.MapPath(savePath + name));
                    string normalName = sDate + normalWidth.ToString() + ext;//大图名称
                    ZoomAutoControlWide(postedFile, savePath + normalName, normalWidth, "", "", WaterMarkPosition.WMP_Left_Bottom, WaterMarkPosition.WMP_Left_Top);
                    newName[1] = normalName;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 等比缩放上传图片
        /// <summary> 
        /// 图片等比缩放单个上传
        /// </summary> 
        /// <remarks>吴剑 2011-01-21</remarks> 
        /// <param name="postedFile">原图HttpPostedFile对象</param> 
        /// <param name="savePath">缩略图存放地址</param> 
        /// <param name="targetWidth">指定的最大宽度</param> 
        /// <param name="targetHeight">指定的最大高度</param> 
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param> 
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param> 
        public static void ZoomAuto(System.Web.HttpPostedFileBase postedFile, string savePath, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage, WaterMarkPosition waterTextPosition, WaterMarkPosition waterImagePosition)
        {
            //创建目录 
            string dir = System.Web.HttpContext.Current.Server.MapPath(Path.GetDirectoryName(savePath));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息） 
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(postedFile.InputStream, true);

            //原图宽高均小于模版，不作处理，直接保存 
            if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
            {
                //文字水印 
                if (watermarkText != "")
                {
                    AddWaterText(watermarkText, initImage, waterTextPosition);
                }

                //透明图片水印 
                if (watermarkImage != "")
                {
                    AddWaterImage(watermarkImage, initImage, waterImagePosition);
                }

                //保存 
                initImage.Save(System.Web.HttpContext.Current.Server.MapPath(savePath));
            }
            else
            {
                //缩略图宽、高计算 
                double newWidth;
                double newHeight;
                double[] arrWH = GetNewWH(initImage, targetWidth, targetHeight);
                newWidth = arrWH[0];
                newHeight = arrWH[1];

                CreateNewImage(newWidth, newHeight, initImage, watermarkText, watermarkImage, waterTextPosition, waterImagePosition, savePath);
            }
        }

        /// <summary> 
        /// 图片等比缩放单个上传
        /// </summary> 
        /// <remarks>吴剑 2011-01-21</remarks> 
        /// <param name="postedFile">原图HttpPostedFile对象</param> 
        /// <param name="savePath">缩略图存放地址</param> 
        /// <param name="targetWidth">指定的最大宽度</param> 
        /// <param name="targetHeight">指定的最大高度</param> 
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param> 
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param> 
        public static void ZoomAutoStream(Stream postedFile, string savePath, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage, WaterMarkPosition waterTextPosition, WaterMarkPosition waterImagePosition)
        {
            //创建目录 
            string dir = System.Web.HttpContext.Current.Server.MapPath(Path.GetDirectoryName(savePath));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息） 
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(postedFile, true);

            //原图宽高均小于模版，不作处理，直接保存 
            if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
            {
                //文字水印 
                if (watermarkText != "")
                {
                    AddWaterText(watermarkText, initImage, waterTextPosition);
                }

                //透明图片水印 
                if (watermarkImage != "")
                {
                    AddWaterImage(watermarkImage, initImage, waterImagePosition);
                }

                //保存 
                initImage.Save(System.Web.HttpContext.Current.Server.MapPath(savePath));
            }
            else
            {
                //缩略图宽、高计算 
                double newWidth;
                double newHeight;
                double[] arrWH = GetNewWH(initImage, targetWidth, targetHeight);
                newWidth = arrWH[0];
                newHeight = arrWH[1];

                CreateNewImage(newWidth, newHeight, initImage, watermarkText, watermarkImage, waterTextPosition, waterImagePosition, savePath);
            }
        }

        /// <summary> 
        /// 生成缩略图 
        /// </summary> 
        /// <param name="originalImagePath">源图路径（物理路径）</param> 
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param> 
        /// <param name="width">缩略图宽度</param> 
        /// <param name="height">缩略图高度</param> 
        /// <param name="mode">生成缩略图的方式</param>     
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(originalImagePath));

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            //新建一个bmp图片 
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);
            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(System.Web.HttpContext.Current.Server.MapPath(thumbnailPath), System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        /// <summary> 
        /// 图片等比缩放单个上传
        /// </summary> 
        /// <remarks>吴剑 2011-01-21</remarks> 
        /// <param name="postedFile">原图HttpPostedFile对象</param> 
        /// <param name="savePath">缩略图存放地址</param> 
        /// <param name="targetWidth">指定的最大宽度</param>
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param> 
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param> 
        public static void ZoomAutoControlWide(System.Web.HttpPostedFileBase postedFile, string savePath, System.Double targetWidth, string watermarkText, string watermarkImage, WaterMarkPosition waterTextPosition, WaterMarkPosition waterImagePosition)
        {
            //创建目录 
            string dir = System.Web.HttpContext.Current.Server.MapPath(Path.GetDirectoryName(savePath));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息） 
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(postedFile.InputStream, true);
            //原图宽高均小于模版，不作处理，直接保存 
            if (initImage.Width <= targetWidth)
            {
                //文字水印 
                if (watermarkText != "")
                {
                    AddWaterText(watermarkText, initImage, waterTextPosition);
                }
                //透明图片水印 
                if (watermarkImage != "")
                {
                    AddWaterImage(watermarkImage, initImage, waterImagePosition);
                }
                //保存 
                initImage.Save(System.Web.HttpContext.Current.Server.MapPath(savePath));
            }
            else
            {
                //缩略图宽、高计算 
                double newWidth;
                double newHeight;
                double[] arrWH = GetNewW(initImage, targetWidth);
                newWidth = arrWH[0];
                newHeight = arrWH[1];
                CreateNewImage(newWidth, newHeight, initImage, watermarkText, watermarkImage, waterTextPosition, waterImagePosition, savePath);
            }
        }

        public static void CreateNewImage(double newWidth, double newHeight, Image initImage, string watermarkText, string watermarkImage,
            WaterMarkPosition waterTextPosition, WaterMarkPosition waterImagePosition, string savePath)
        {
            //生成新图 
            //新建一个bmp图片 
            System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
            //新建一个画板 
            System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);
            //设置质量 
            newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //置背景色 
            newG.Clear(Color.White);
            //画图 
            newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height),
                new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
            //文字水印 
            if (watermarkText != "")
            {
                AddWaterText(watermarkText, newImage, waterTextPosition);
            }
            //透明图片水印 
            if (watermarkImage != "")
            {
                AddWaterImage(watermarkImage, newImage, waterImagePosition);
            }
            //保存缩略图 
            newImage.Save(HttpContext.Current.Server.MapPath("~" + savePath), System.Drawing.Imaging.ImageFormat.Jpeg);
            //释放资源 
            newG.Dispose();
            newImage.Dispose();
            initImage.Dispose();
        }

        public static double[] GetNewWH(Image initImage, System.Double targetWidth, System.Double targetHeight)
        {
            //缩略图宽、高计算 
            double newWidth = initImage.Width;
            double newHeight = initImage.Height;

            //宽大于高或宽等于高（横图或正方） 
            if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
            {
                //如果宽大于模版 
                if (initImage.Width > targetWidth)
                {
                    //宽按模版，高按比例缩放 
                    newWidth = targetWidth;
                    newHeight = initImage.Height * (targetWidth / initImage.Width);
                }
            }
            //高大于宽（竖图） 
            else
            {
                //如果高大于模版 
                if (initImage.Height > targetHeight)
                {
                    //高按模版，宽按比例缩放 
                    newHeight = targetHeight;
                    newWidth = initImage.Width * (targetHeight / initImage.Height);
                }
            }

            return new double[] { newWidth, newHeight };
        }

        public static double[] GetNewW(Image initImage, System.Double targetWidth)
        {
            //缩略图宽、高计算 
            double newWidth = initImage.Width;
            double newHeight = initImage.Height;

            //如果宽大于模版 
            if (initImage.Width > targetWidth)
            {
                //宽按模版，高按比例缩放 
                newWidth = targetWidth;
                newHeight = initImage.Height * (targetWidth / initImage.Width);
            }
            return new double[] { newWidth, newHeight };
        }

        private static void AddWaterImage(string watermarkImage, Image initImage, WaterMarkPosition waterImagePosition)
        {
            if (File.Exists(HttpContext.Current.Server.MapPath("~" + watermarkImage)))
            {
                //获取水印图片 
                using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~" + watermarkImage)))
                {
                    //水印绘制条件：原始图片宽高均大于或等于水印图片 
                    if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                    {
                        Graphics gWater = Graphics.FromImage(initImage);
                        //透明属性 
                        ImageAttributes imgAttributes = new ImageAttributes();
                        ColorMap colorMap = new ColorMap();
                        colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                        colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                        ColorMap[] remapTable = { colorMap };
                        imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                        float[][] colorMatrixElements = {  
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f}, 
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f}, 
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f}, 
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5 
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f} 
                                };

                        ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                        imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                        int wrp_X = 0;
                        int wrp_Y = 0;
                        switch (waterImagePosition)
                        {
                            case WaterMarkPosition.WMP_Left_Top:
                                wrp_X = 0;
                                wrp_Y = 0;
                                break;
                            case WaterMarkPosition.WMP_Left_Bottom:
                                wrp_X = 0;
                                wrp_Y = initImage.Height - wrImage.Height;
                                break;
                            case WaterMarkPosition.WMP_Right_Top:
                                wrp_X = initImage.Width - wrImage.Width;
                                wrp_Y = 0;
                                break;
                            case WaterMarkPosition.WMP_Right_Bottom:
                                wrp_X = initImage.Width - wrImage.Width;
                                wrp_Y = initImage.Height - wrImage.Height;
                                break;
                            default:
                                break;
                        }
                        gWater.DrawImage(wrImage, new Rectangle(wrp_X, wrp_Y, wrImage.Width, wrImage.Height),
                            0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                        gWater.Dispose();
                    }
                    wrImage.Dispose();
                }
            }
        }

        private static void AddWaterText(string watermarkText, Image initImage, WaterMarkPosition waterTextPosition)
        {
            using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(initImage))
            {
                System.Drawing.Font fontWater = new Font("黑体", 10);
                System.Drawing.Brush brushWater = new SolidBrush(Color.White);

                float wrp_X = 0;
                float wrp_Y = 0;
                switch (waterTextPosition)
                {
                    case WaterMarkPosition.WMP_Left_Top:
                        wrp_X = 0;
                        wrp_Y = 0;
                        break;
                    case WaterMarkPosition.WMP_Left_Bottom:
                        wrp_X = 0;
                        wrp_Y = initImage.Height - (fontWater.Size + 8);
                        break;
                    case WaterMarkPosition.WMP_Right_Top:
                        wrp_X = initImage.Width - watermarkText.Length * (fontWater.Size + 8);
                        wrp_Y = 0;
                        break;
                    case WaterMarkPosition.WMP_Right_Bottom:
                        wrp_X = initImage.Width - watermarkText.Length * (fontWater.Size + 8);
                        wrp_Y = initImage.Height - (fontWater.Size + 8);
                        break;
                    default:
                        break;
                }

                gWater.DrawString(watermarkText, fontWater, brushWater, wrp_X, wrp_Y);
                gWater.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 水印的位置
        /// </summary>
        public enum WaterMarkPosition
        {
            /**/
            /// <summary>
            /// 左上角
            /// </summary>
            WMP_Left_Top,
            /**/
            /// <summary>
            /// 左下角
            /// </summary>
            WMP_Left_Bottom,
            /**/
            /// <summary>
            /// 右上角
            /// </summary>
            WMP_Right_Top,
            /**/
            /// <summary>
            /// 右下角
            /// </summary>
            WMP_Right_Bottom
        };
    }
}
