using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using InterfaceWeb;
using LinqModel.InterfaceModels;
using BLL;

namespace iagric_plant.Controllers
{
    public class dealerController : Controller
    {
        /// <summary>
        /// 同步经销商信息接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult saveDealer()
        {
            DealerBLL bll = new DealerBLL();
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string str = Encoding.UTF8.GetString(b);
            DealerModel Obj = JsonDes.JsonDeserialize<DealerModel>(str);
            string accessToken = this.Request.Headers["accessToken"];
            InterfaceResult result = bll.InterfaceAdd(Obj, accessToken);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}