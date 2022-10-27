using System.Drawing;

namespace Common
{
    public class OIDImage
    {
        /// <summary>
        /// 生成标准的OID二维码码有网址
        /// </summary>
        /// <param name="oidCode"></param>
        /// <returns></returns>
        public static Image CreateOIDCodeImage(int verify, string code, string type, int width, int height)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["idcodeURL"].ToString();
            string[] arrCode = code.Split('.');
            if (verify == (int)EnumFile.EnterpriseVerify.Try || (arrCode.Length > 5 && (arrCode[4] == ((int)EnumFile.TerraceEwm.slotting).ToString() || arrCode[4] == ((int)EnumFile.TerraceEwm.cribCode).ToString())))
            {
                url = System.Configuration.ConfigurationManager.AppSettings["ncpURL"].ToString();
            }
            if (type == "1")
            {
                url = System.Configuration.ConfigurationManager.AppSettings["cardURL"].ToString();
            }
            return CreateCodeImage(url + code, width, height);
        }
        /// <summary>
        /// 生成标准的OID二维码码没有网址
        /// </summary>
        /// <param name="oidCode"></param>
        /// <returns></returns>
        public static Image CreateCode(string code, int width, int height)
        {
            return CreateCodeImage(code, width, height);
        }
        public static Image CreateCodeGZH(string url, int width, int height)
        {
            return CreateCodeImageGZH(url, width, height);
        }

        /// <summary>
        /// 生成普通的二维码图
        /// </summary>
        /// <param name="code">二维码号</param>
        /// <returns></returns>
        public static Image CreateCodeImage(string code, int width, int height)
        {
            Image img = null;
            try
            {
                ZXingCode.common.ByteMatrix m = new ZXingCode.MultiFormatWriter().encode(code, ZXingCode.BarcodeFormat.QR_CODE, 140, 140);
                img = toBitmap(m, code);
                try
                {
                    Image onlyone = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("/newmenu/images/onlyone.jpg"));
                    Graphics g = Graphics.FromImage(onlyone);
                    g.DrawImage(img, 80, 80, 140, 140);
                    img = onlyone;
                }
                catch { }
            }
            catch
            {
            }
            return img;
        }

        public static Image CreateCodeImageGZH(string url, int width, int height)
        {
            Image img = null;
            try
            {
                ZXingCode.common.ByteMatrix m = new ZXingCode.MultiFormatWriter().encode(url, ZXingCode.BarcodeFormat.QR_CODE, 140, 140);
                img = toBitmap(m, url);
                try
                {
                    Image onlyone = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(""));
                    Graphics g = Graphics.FromImage(onlyone);
                    g.DrawImage(img, 80, 80, 140, 140);
                    img = onlyone;
                }
                catch { }
            }
            catch
            {
            }
            return img;
        }

        public static Image CreateCodeImageHasBorder(string code, int width, int height)
        {
            Image img = null;
            try
            {
                string url = System.Configuration.ConfigurationManager.AppSettings["idcodeURL"];
                ZXingCode.common.ByteMatrix m = new ZXingCode.MultiFormatWriter().encode(url + code, ZXingCode.BarcodeFormat.QR_CODE, 140, 140);
                img = toBitmap(m, url + code);
                try
                {
                    Image onlyone = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("/newmenu/images/onlyone.jpg"));
                    Graphics g = Graphics.FromImage(onlyone);
                    g.DrawImage(img, 80, 80, 140, 140);
                    img = onlyone;
                }
                catch { }
            }
            catch
            {
            }
            return img;
        }

        static Bitmap toBitmap(ZXingCode.common.ByteMatrix matrix, string str)
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

        /// <summary>
        /// 采集记录里面查看二维码图
        /// </summary>
        /// <returns></returns>
        public static Image CreateCollectImage(int verify, string code, string type, int width, int height)
        {
            return CreateCodeImage(code, width, height);
        }
    }
}
