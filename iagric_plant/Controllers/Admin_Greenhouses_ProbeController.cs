using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_Greenhouses_ProbeController : BaseController
    {
        public ActionResult Index(int pageIndex, string title, string bDate, string eDate)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                result = new BLL.GreenhouseBLL().GetProbeList(pf.EnterpriseID, pf.UserID, title, bDate, eDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Greenhouses_ProbeController.Index():Greenhouses_Probe表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public ActionResult Data(int? pageIndex, long gpId, string sDate, string eDate)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                result = new BLL.GreenhouseBLL().GetDataList(pf.EnterpriseID, gpId, sDate, eDate, pageIndex ?? 1);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Greenhouses_ProbeController.Data():Greenhouses_Probe_Data表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public ActionResult ShowEwm()
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo pf = Common.Argument.SessCokie.Get;
                List<EWMInfo> ewmInfo = new List<EWMInfo>();
                string idcodeurl = System.Configuration.ConfigurationManager.AppSettings["idcodeURL"].ToString();
                if (pf.Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                {
                    idcodeurl = System.Configuration.ConfigurationManager.AppSettings["ncpURL"].ToString();
                }
                EWMInfo eEWM = new EWMInfo();
                eEWM.infoName = "企业二维码";
                eEWM.ewm = pf.MainCode;
                eEWM.fullewm = idcodeurl + pf.MainCode;
                ewmInfo.Add(eEWM);

                EWMInfo uEWM = new EWMInfo();
                uEWM.infoName = "用户二维码";
                uEWM.ewm = pf.MainCode + ".16." + pf.UserID;
                uEWM.fullewm = idcodeurl + uEWM.ewm;
                ewmInfo.Add(uEWM);
                BaseResultList List = new BLL.GreenhouseBLL().GetMList(pf.EnterpriseID);
                foreach (var item in (List.ObjList as List<Greenhouses>))
                {
                    EWMInfo gEWM = new EWMInfo();
                    gEWM.infoName = item.GreenhousesName;
                    gEWM.ewm = item.EWM;
                    gEWM.fullewm = idcodeurl + gEWM.ewm;
                    ewmInfo.Add(gEWM);
                }
                result.Msg = "";
                result.ObjList = ewmInfo;
                result.pageIndex = 1;
                result.pageSize = int.MaxValue;
                result.totalCounts = ewmInfo.Count;
            }
            catch { }
            return Json(result);
        }
    }
}
