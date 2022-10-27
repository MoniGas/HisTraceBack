/********************************************************************************
** 作者： 李子巍
** 创始时间：2017-03-15
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：赵慧敏
** 修改时间：2017-02-10
** 描述：
**  主要用于开通推广管理控制器层 
*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using BLL;
using Common.Argument;
using Common.Log;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 控制器
    /// </summary>
    public class EnterpriseSwitchController : Controller
    {
        /// <summary>
        /// 查询是否开通推广
        /// </summary>
        /// <param name="switchCode">代码</param>
        /// <returns></returns>
        public ActionResult GetIsOn(int switchCode)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                EnterpriseSwitchBLL bll = new EnterpriseSwitchBLL();
                result = bll.GetIsOn(pf.EnterpriseID, switchCode);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "EnterpriseSwitch.GetIsOn():Enterprise_Switch表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 开通、关闭服务
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="switchCode">开通服务代码</param>
        /// <param name="type">1开通 其他值均为关闭</param>
        /// <returns>操作结果</returns>
        public ActionResult TrunSwitch(int switchCode, int type)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                EnterpriseSwitchBLL bll = new EnterpriseSwitchBLL();
                result = bll.TrunSwitch(pf.EnterpriseID, switchCode, type);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "EnterpriseSwitch.TrunSwitch():Enterprise_Switch表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取企业是否激活
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIsActive()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                EnterpriseSwitchBLL bll = new EnterpriseSwitchBLL();
                result = bll.GetIsActive(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                WriteLog.WriteErrorLog("EnterpriseSwitch.GetIsActive():Enterprise_Switch表" + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 企业激活开关
        /// </summary>
        public ActionResult SwitchActive()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result;
            try
            {
                EnterpriseSwitchBLL bll = new EnterpriseSwitchBLL();
                result = bll.SwitchActive(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "EnterpriseSwitch.SwitchActive():Enterprise_Switch表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public ActionResult GetLicense()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
                Enterprise_License model = bll.GetEnInfoLicense(pf.EnterpriseID);
                if (model == null)
                {
                    model = new Enterprise_License();
                    model.LicenseCode = "";
                    model.StrLicenseEndDate = "";
                }
                else
                {
                    model.StrLicenseEndDate =Convert.ToDateTime(model.LicenseEndDate).ToString("yyyy-MM-dd");
                }
                result = ToJson.NewModelToJson(model, "1", "");
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
            }
            return Json(result);
        }
    }
}
