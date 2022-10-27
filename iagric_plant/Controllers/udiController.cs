using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using LinqModel.InterfaceModels;
using System.IO;
using System.Text;
using InterfaceWeb;
using Common;
using LinqModel;

namespace iagric_plant.Controllers
{
	public class udiController : Controller
	{
		//
		// GET: /udi/

		public ActionResult Index()
		{
			return View();
		}

		UdiBLL bll = new UdiBLL();

		#region 同步DI接口
		/// <summary>
		/// 同步DI接口
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public ActionResult DILst(DILstRequestParam model)
		{
			InterfaceResult result = new InterfaceResult();
			string accessToken = this.Request.Headers["accessToken"].ToString();
			result = bll.DILst(model, accessToken);
			return Json(result, JsonRequestBehavior.DenyGet);
		}
		#endregion

		#region 同步UDI-PI接口
		[HttpPost]
		public ActionResult PILst(PILstRequestParam Param)
		{
			InterfaceResult result = new InterfaceResult();
			string accessToken = this.Request.Headers["accessToken"].ToString();
			result = bll.PILst(Param, accessToken);
			return Json(result, JsonRequestBehavior.DenyGet);
		}
		#endregion

		#region 同步UDI-PI明细接口
		[HttpPost]
		public ActionResult PIDetail(PIDetailRequestParam Param)
		{
			InterfaceResult result = new InterfaceResult();
			string accessToken = this.Request.Headers["accessToken"].ToString();
			result = bll.PIDetail(Param, accessToken);
			return Json(result, JsonRequestBehavior.DenyGet);
		}
		#endregion

		#region 生成UDI-DI接口
		[HttpPost]
		public ActionResult SaveDI(SaveDIRequestParam Param)
		{
			InterfaceResult result = new InterfaceResult();
			string accessToken = this.Request.Headers["accessToken"].ToString();
			result = bll.SaveDI(Param, accessToken);
			return Json(result, JsonRequestBehavior.DenyGet);
		}
		#endregion

		#region 解析码接口
		/// <summary>
		/// 解析码接口
		/// </summary>
		/// <param name="CodeValue"></param>
		/// <param name="CodeFormat"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult UDIAnalyse(string CodeValue, int CodeFormat=0)
		{
			UDIAnalyseResult result = new UDIAnalyseResult();
			try
			{
				var item = this.Request.Headers.AllKeys.Select(m => new { value = m }).Where(m => m.value.ToUpper() == "TOKEN").SingleOrDefault();
				if (item == null) 
				{
					result.code = 1007;
					result.Msg = "未从Headers中检测到Token项的值，请检查是否通过请求头：Token 传入值！";
					return Json(result, JsonRequestBehavior.DenyGet);
				}
				string key = this.Request.Headers["Token"].ToString();
				result = bll.UDIAnalyse(CodeValue, key, CodeFormat);
				return Json(result, JsonRequestBehavior.DenyGet);
			}
			catch (Exception ex) 
			{
				result.code = -1;
				result.Msg = ex.Message;
				return Json(result, JsonRequestBehavior.DenyGet);
			}
		}

		/// <summary>
		/// UDI码绑定关系接口
		/// </summary>
		/// <param name="CodeValue"></param>
		/// <returns></returns>
        [HttpPost]
		public ActionResult UDIBind(string CodeValue, int CodeFormat = 0)
        {
            UDIBindResult result = new UDIBindResult();
            try
            {
				var item = this.Request.Headers.AllKeys.Select(m => new { value = m }).Where(m => m.value.ToUpper() == "TOKEN").SingleOrDefault();
                if (item == null)
                {
                    result.code = 1007;
                    result.Msg = "未从Headers中检测到Token项的值，请检查是否通过请求头：Token 传入值！";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                string key = this.Request.Headers["Token"].ToString();
				result = bll.UDIBind(CodeValue, key, CodeFormat);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                result.code = -1;
                result.Msg = ex.Message;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }

		/// <summary>
		/// UDI解析码与码绑定关系二合一接口
		/// </summary>
		/// <param name="CodeValue"></param>
		/// <param name="CodeFormat"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult UDIMerge(string CodeValue, int CodeFormat = 0) 
		{
			UDIMergeResult result = new UDIMergeResult();
			try
			{
				var item = this.Request.Headers.AllKeys.Select(m => new { value = m }).Where(m => m.value.ToUpper() == "TOKEN").SingleOrDefault();
				if (item == null)
				{
					result.code = 1007;
					result.Msg = "未从Headers中检测到Token项的值，请检查是否通过请求头：Token 传入值！";
					return Json(result, JsonRequestBehavior.DenyGet);
				}
				string key = this.Request.Headers["Token"].ToString();
				result = bll.UDIMerge(CodeValue, key, CodeFormat);
				return Json(result, JsonRequestBehavior.DenyGet);
			}
			catch (Exception ex)
			{
				result.code = -1;
				result.Msg = ex.Message;
				return Json(result, JsonRequestBehavior.DenyGet);
			}
		}
		#endregion

	}
}
