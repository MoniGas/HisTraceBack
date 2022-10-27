using System;
using System.Drawing;

namespace MvcWebCommon.Tools
{
    public class CreateEwmImg
    {
        /// <summary>
        /// 生成码图
        /// </summary>
        /// <param name="i_oid">i_oid</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="isOldCode">是否是新码</param>
        /// <returns></returns>
        public static System.Drawing.Image GetQRCodeImageEx(string i_oid, int width, int height)
        {
            Image img = null;
            try
            {

                i_oid = MvcWebModel.GlobleData._EwmUrl + i_oid.ToString().Trim();
                ZXingCode.common.ByteMatrix m = new ZXingCode.MultiFormatWriter().encode(i_oid, ZXingCode.BarcodeFormat.QR_CODE, 140, 140);
                img = toBitmap(m, i_oid);
                try
                {
                    Image onlyone = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("/newmenu/images/onlyone.jpg"));
                    Graphics g = Graphics.FromImage(onlyone);
                    g.DrawImage(img, 80, 80, 140, 140);
                    img = onlyone;
                }
                catch { }
            }
            catch (Exception ex)
            { }
            return img;
        }

        private static Bitmap toBitmap(ZXingCode.common.ByteMatrix matrix, string str)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmap.GetHbitmap(Color.White);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bmap.SetPixel(x, y, matrix.get_Renamed(x, y) != -1 ? ColorTranslator.FromHtml("0xFF000000") : ColorTranslator.FromHtml("0xFFFFFFFF"));
                    if (matrix.get_Renamed(x, y) != -1)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFFFFFFFF"));
                    }
                    if (matrix.get_Renamed(x, y) != 1)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFFFFFFFF"));
                    }

                    if (matrix.get_Renamed(x, y) == -1)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFFFFFFFF"));
                    }

                    if (matrix.get_Renamed(x, y) == 1)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFF000000"));
                    }
                    if (matrix.get_Renamed(x, y) == 0)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFFFFFFFF"));
                    }

                    if (matrix.get_Renamed(x, y) == 7)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFFB60005"));
                    }
                    if (matrix.get_Renamed(x, y) == 8)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFF003F98"));
                    }
                    if (matrix.get_Renamed(x, y) == 9)
                    {
                        bmap.SetPixel(x, y, ColorTranslator.FromHtml("0xFF00873C"));
                    }
                }

            }
            return bmap;
        }
    }
}
