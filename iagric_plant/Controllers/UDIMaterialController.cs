using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LinqModel;
using BLL;
using Common;
using Common.Log;
using System.Data;
using System.Text;
using Common.Tools;
using Common.Argument;

namespace iagric_plant.Controllers
{
    public class UDIMaterialController : Controller
    {
        //
		// GET: /UDIMaterial/
		#region 此处两个方法仅用于获取国药监数据
		UDIMaterialBLL bll = new UDIMaterialBLL();
		#region 读取Excel文件内容信息入库到本地数据库
		/// <summary>
		/// UDI 读取Excel文件内容信息入库到本地数据库
		/// </summary>
		/// <param name="Type">是否清库默认不清库</param>
		/// <returns></returns>
		public ActionResult ReadFile()
		{
			BaseResultModel result = new BaseResultModel();
			string strFileDir = "/UDIExcel/";
			string path = Server.MapPath("~" + strFileDir);
			string[] files = Directory.GetFiles(path + @"\", "*.xls");
			foreach (string file in files)
			{
				result = bll.SaveUDIMaterial(file);
			}
			return Content("<script>alert('" + result.Msg + "')</script>"); ;
		}

		#endregion

		#region 按天、月、全量获取国药监数据存库
		/// <summary>
		/// 按天、月、全量获取国药监数据存库
		/// </summary>
		/// <param name="Type">类型1：按天、2：按月、3：全量</param>
		/// <param name="date">对应的日期，yyyy-MM-dd;yyyy-MM;""</param>
		/// <returns></returns>
		public ActionResult GetUDI(string Type, string date)
		{
			RetResult result = new RetResult();
			result=bll.GetUDI(Type, date);
			return Content("<script>alert('" + result.Msg + "')</script>"); 
		}

		#endregion

		#endregion



	}
}
