using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using LinqModel;
using MarketActive.Controllers;
using Common.Tools;
using iagric_plant.Areas.Market.Models;
using InterfaceWeb;
using System;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using iagric_plant.Models;

namespace iagric_plant
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
               new { controller = "Login", action = "HomeIndex", id = UrlParameter.Optional },
               new string[] { "iagric_plant.Controllers" }); // 参数默认值
            //       new[] { "iagric_plant.Controllers" }
            //).DataTokens["UseNamespaceFallback"] = false;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //#region 加载行业信息、地区信息、机构类型
            Common.Argument.BaseData.listAddress = InterfaceWeb.BaseDataDAL.GetAllAreas();
            Common.Argument.BaseData.listTrade = InterfaceWeb.BaseDataDAL.GetAllTrade();
            //Common.Argument.BaseData.unitType = InterfaceWeb.BaseDataDAL.GetUnitType();
            // string result = InterfaceWeb.BaseDataDAL.GetCategory();
            //Common.Tools.Public.listCategory = new BLL.CategoryBLL().GetList(result);
            // Common.Argument.BaseData.categoryScode = InterfaceWeb.BaseDataDAL.SetCode();
            //InterFaceHisCodeFileUrlInfo result1 = InterfaceWeb.BaseDataDAL.GetUploadBatch("MA.156.M0.100008", "20191101100301881", "hbgl123456");
            //Thread thread = new Thread(new ThreadStart(UpdateRedPacket));
            //Common.Argument.BaseData.HisunitType = InterfaceWeb.BaseDataDAL.GetHisUnitType();
            //string result = InterfaceWeb.BaseDataDAL.GetHisIndustryCategory();
            //Common.Tools.Public.listCategory = new BLL.CategoryBLL().GetHisCategoryList(result);
            ////Common.Argument.BaseData.HisCategory = InterfaceWeb.BaseDataDAL.GetHisIndustryCategory();
            //thread.Start();
           
        }
        /// <summary>
        /// 更新红包状态
        /// </summary>
        private void UpdateRedPacket()
        {
            while (true)
            {
                BLL.ScanCodeMarketBLL bll = new BLL.ScanCodeMarketBLL();
                //更新红包发送状态
                List<YX_RedGetRecord> lst = bll.GetRecord();
                string data = string.Empty;
                foreach (var item in lst)
                {
                    data = new Wap_IndexMarketController().GetRedPacket(item.BillNumber, item.MarId);
                    ResponseGetRedPacket getPacket = XmlHelper.Deserialize(typeof(ResponseGetRedPacket), data) as ResponseGetRedPacket;
                    if (getPacket != null && getPacket.result_code == "SUCCESS")
                    {
                        int state = 0;
                        switch (getPacket.status)
                        {
                            case "SENDING": state = 1; break;
                            case "SENT": state = 2; break;
                            case "FAILED": state = 3; break;
                            case "RECEIVED": state = 4; break;
                            case "RFUND_ING": state = 5; break;
                            case "REFUND": state = 6; break;
                        }
                        string getDate = getPacket.hblist != null && getPacket.hblist.Length > 0 ? getPacket.hblist[0].rcv_time : "";
                        new BLL.ScanCodeMarketBLL().UpdateGetRecordState(item.GetRecordID, state, getDate);
                    }
                }
                BLL.RedRechargeBLL redBll = new BLL.RedRechargeBLL();
                List<YX_RedRecharge> recharge = redBll.GetRechargeList();
                foreach (var item in recharge)
                {
                    string a = new PacketController().HandNotifyAuto(item.OrderNum);
                }
                Thread.Sleep(300000);
            }
        }


      

    }
}