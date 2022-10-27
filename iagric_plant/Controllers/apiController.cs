using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinqModel.InterfaceModels;
using BLL;

namespace iagric_plant.Controllers
{
    public class apiController : Controller
    {
        //
        // GET: /api/

        public ActionResult Index()
        {
            return View();
        }

		ApiBLL bll = new ApiBLL();

		//登录接口
		[HttpPost]
		public ActionResult login(Login model)
		{

			InterfaceResult result = new InterfaceResult();
			result = bll.login(model);
			return Json(result);
		}

    }
}
