using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using Common;
using Common.Argument;
using BLL;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class ShowImageController : Controller
    {
        //
        // GET: /ShowImage/
        /// <summary>
        /// 二维码查看二维码
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowImg()
        {
            LoginInfo pf = SessCokie.Get;
            string ewm = Request["ewm"];

            string type = Request["type"];
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateOIDCodeImage(pf.Verify, ewm, type, 100, 100).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg");
        }

        public ActionResult ShowImgNoUrl()
        {
            string ewm = Request["ewm"];
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateCode(ewm, 100, 100).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg");
        }

        public ActionResult DownEWMImg(string ewm, string type, int size)
        {
            LoginInfo pf = SessCokie.Get;
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateOIDCodeImage(pf.Verify, ewm, type, size, size).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg", string.Format("{0}.jpg", ewm));
        }

        /// <summary>
        /// 追溯信息配置查看预览效果
        /// </summary>
        /// <param name="v"></param>
        /// <param name="subId"></param>
        /// <returns></returns>
        public ActionResult ShowPreview(string v, long subId)
        {
            LinqModel.RequestCodeSetting setting = new BLL.RequestCodeSettingBLL().GetModel(subId);
            long materialId = 0;
            if (setting != null)
            {
                materialId = setting.MaterialID.GetValueOrDefault(0);
            }
            //i.1.130105.1/10.54014001.1j/171110.000002.2
            LoginInfo pf = SessCokie.Get;
            string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/wap_index/index?";
            //string ewm = url + "ewm=" + pf.MainCode + "." + materialId + "." + subId + ".10";
            string ewm = "";
            RequestCodeBLL rbll = new RequestCodeBLL();
            Enterprise_FWCode_00 model = rbll.GetCodeModel(subId);
            if (model != null)
            {
                ewm = url + "ewm=" + model.EWM + "." + subId + ".10";
            }
            else
            {
                ewm = url + "ewm=" + pf.MainCode + "." + materialId + "." + subId + ".10";
            }
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateCode(ewm, 200, 200).Save(stream, ImageFormat.Jpeg);
            //OIDImage.CreateCodeImageHasBorder(ewm, 300, 300).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg");
        }

        /// <summary>
        /// 产品管理中查看产品码
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowImgMaterial()
        {
            //string time = DateTime.Now.AddDays(-1).ToString("yyMMdd");
            //string ewm = Request["ewm"] + time + ".000001";
            string ewm = Request["ewm"];
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateCodeImageHasBorder(ewm, 300, 300).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg");
        }

        /// <summary>
        /// 采集记录里面查看二维码图
        /// </summary>
        /// <returns></returns>
        public ActionResult CollectShowImg()
        {
            LoginInfo pf = SessCokie.Get;
            string ewm = Request["ewm"];

            string type = Request["type"];
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateCollectImage(pf.Verify, ewm, type, 100, 100).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg");
        }

        /// <summary>
        /// 采集记录里面下载二维码图
        /// </summary>
        /// <returns></returns>
        public ActionResult CollectDownEWMImg(string ewm, string type, int size)
        {
            LoginInfo pf = SessCokie.Get;
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateCollectImage(pf.Verify, ewm, type, size, size).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg", string.Format("{0}.jpg", ewm));
        }

        /// <summary>
        /// 微信公众号的二维码图
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowImgGZH(string url, int w, int h)
        {
            string ewm = Request["ewm"];
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateCodeGZH(url, 100, 100).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg");
        }
    }
}
