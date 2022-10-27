/********************************************************************************
** 作者：追溯补
** 开发时间：2017-6-9
** 联系方式:13313318725
** 代码功能：登录首页
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using LinqModel;
using Common.Argument;
using BLL;
using MarketActive.Filter;

namespace MarketActive.Controllers
{
    [AdminAuthorize]
    public class HomeMarketController : Controller
    {
        //
        // GET: /Home/

        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            LoginInfo user = SessCokie.Get;
            Enterprise_Info model = new EnterpriseInfoBLL().GetModel(user.EnterpriseID);
            if (model != null)
            {
                if (model.imgs != null)
                {
                    foreach (var item in model.imgs)
                    {
                        ViewBag.xmlimg = item.fileUrl;
                        ViewBag.xmlsamllimg = item.fileUrls;
                    }
                }
            }
            LoginMarketBLL bll = new LoginMarketBLL();
            YX_ActivitySub activiveSub = bll.GetAcSub(user.EnterpriseID);
            if (activiveSub != null)
            {
                //活动名称
                ViewBag.ActivityTitle = activiveSub.ActivityTitle;
            }
            //昨日扫码用户数量
            List<YX_Redactivity_ScanRecord> scanRecordZ = bll.GetScanRecordZ();
            ViewBag.ZuoScanRecord = scanRecordZ.Select(a => a.WeiXinUserID).Distinct().Count();
            //今日扫码用户数量
            List<YX_Redactivity_ScanRecord> scanRecord = bll.GetScanRecord();
            ViewBag.JinScanRecord = scanRecord.Select(a => a.WeiXinUserID).Distinct().Count();
            //获取今日领取红包的用户数量
            List<YX_RedGetRecord> reduserRecord = bll.RedGetRecord();
            ViewBag.JinRedUserRecord = reduserRecord.Select(a => a.WeiXinUserID).Distinct().Count();
            Double sumJin = 0;
            if (reduserRecord.Count > 0)
            {
                foreach (var item in reduserRecord)
                {
                    sumJin += item.GetRedValue.Value;
                }
            }
            ViewBag.sumJin = sumJin;
            //获取昨日领取红包的用户数量
            List<YX_RedGetRecord> reduserRecordZ = bll.RedGetRecordZ();
            ViewBag.ZuoRedUserRecord = reduserRecordZ.Select(a => a.WeiXinUserID).Distinct().Count();
            Double sumZuo = 0;
            if (reduserRecordZ.Count > 0)
            {
                foreach (var item in reduserRecordZ)
                {
                    sumZuo += item.GetRedValue.Value;
                }
            }
            ViewBag.sumZuo = sumZuo;
            return View(model);
        }

        /// <summary>
        /// 左侧菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Left()
        {
            return View();
        }

        /// <summary>
        /// 头部
        /// </summary>
        /// <returns></returns>
        public ActionResult Top()
        {
            LoginInfo pf = SessCokie.Get;
            LoginBLL bll = new LoginBLL();
            List<PRRU_NewModual> menu = new List<PRRU_NewModual>();
            try
            {
                menu = bll.GetModuleListYX(0, pf.RoleModual_ID_Array);
            }
            catch
            { }
            return View(menu);
        }

        /// <summary>
        /// 内容部分
        /// </summary>
        /// <returns></returns>
        public ActionResult MainFrame(string flag = "")
        {
            string[] arrFlag = flag.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (arrFlag.Length==1)
            {
                ViewBag.Flag = arrFlag[0];
            }
            else if (arrFlag.Length==2)
            {
                ViewBag.Flag = arrFlag[0];
                ViewBag.ActivityId = arrFlag[1];                
            }
            return View();
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult CheckImg()
        {
            string code = CreateValidateCode(4);
            Session["CheckCode"] = code;
            byte[] bytes = CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        //// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        [AllowAnonymous]
        public string CreateValidateCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="containsPage">要输出到的page对象</param>
        /// <param name="validateNum">验证码</param>
        [AllowAnonymous]
        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void Exit()
        {
            if (SessCokie.Get != null)
            {
                SessCokie.Set(null);
                Response.Write("<script type=\"text/javascript\">top.location.href = '/'</script>");
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write("<script>top.location.href=\"/\"</script>");
                Response.End();
            }
        }
    }
}
