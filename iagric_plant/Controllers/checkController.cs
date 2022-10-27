using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using LinqModel.InterfaceModels;
using InterfaceWeb;
using BLL;

namespace iagric_plant.Controllers
{
    public class checkController : Controller
    {
        //
        // GET: /Interface/
        CheckBLL bll = new CheckBLL();
        /// <summary>
        /// 扫码获取稽查信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getCodeInfo()
        {
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string str = Encoding.UTF8.GetString(b);
            CheckCodeRequest Obj = JsonDes.JsonDeserialize<CheckCodeRequest>(str);
            string accessToken = this.Request.Headers["accessToken"];
            InterfaceResult result = bll.getCodeInfo(Obj.codeValue, accessToken);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交稽查结果
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult addCheckResult()
        {
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string str = Encoding.UTF8.GetString(b);
            AddCheckRequest Obj = JsonDes.JsonDeserialize<AddCheckRequest>(str);
            string accessToken = this.Request.Headers["accessToken"];
            InterfaceResult result = bll.MarketCheck(Obj, accessToken);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取稽查记录
        /// </summary>
        /// <returns></returns>
        public JsonResult checLst()
        {
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string str = Encoding.UTF8.GetString(b);
            CheckRecordRequest Obj = JsonDes.JsonDeserialize<CheckRecordRequest>(str);
            string accessToken = this.Request.Headers["accessToken"];
            InterfaceResult result = bll.GetCheckList(Obj, accessToken);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
