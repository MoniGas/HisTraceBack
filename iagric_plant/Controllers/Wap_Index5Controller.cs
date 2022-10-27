/*
   陈志钢 2019年9月6日11:17:02 添加
   为配合上海公司演示开发中英文静态页面
   只有在配置文件<add key="TestID" value="1,2,3"/>写入企业ID的企业用户可以选择本模板
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace iagric_plant.Controllers
{
    public class Wap_Index5Controller : Controller
    {
        //
        // GET: /Wap_Index5/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chines()
        {
            return View();
        }
    }
}
