/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于温馨提醒业务层

*********************************************************************************/
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class ReminderController : Controller
    {
        //
        // GET: /Reminder/
        /// <summary>
        /// 获取该企业是否有产品信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public JsonResult GetMaterial()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ReminderBLL objReminderBLL = new ReminderBLL();
            return Json(objReminderBLL.GetMaterial(pf.EnterpriseID));
        }

        /// <summary>
        /// 该企业是否有生产环节
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public JsonResult GetZuoYe()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ReminderBLL objReminderBLL = new ReminderBLL();
            return Json(objReminderBLL.GetZuoYe(pf.EnterpriseID));
        }

        /// <summary>
        /// 该企业下是否有品牌
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public JsonResult GetBrand()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ReminderBLL objReminderBLL = new ReminderBLL();
            return Json(objReminderBLL.GetBrand(pf.EnterpriseID));
        }

        /// <summary>
        /// 该企业下是否有生产基地
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public JsonResult GetGreenhouses()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ReminderBLL objReminderBLL = new ReminderBLL();
            return Json(objReminderBLL.GetGreenhouses(pf.EnterpriseID));
        }

        /// <summary>
        /// 该企业下是否有经销商
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public JsonResult GetDealer()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ReminderBLL objReminderBLL = new ReminderBLL();
            return Json(objReminderBLL.GetDealer(pf.EnterpriseID));
        }
        /// <summary>
        /// 该企业码数量是否触发阀值
        /// </summary>
        /// <returns></returns>
        public JsonResult GetThresholdWarning()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ReminderBLL objReminderBLL = new ReminderBLL();
            return Json(objReminderBLL.GetThresholdWarning(pf.EnterpriseID));
        }

        /// <summary>
        /// 企业是否认证
        /// </summary>
        /// <returns></returns>
        public JsonResult CompleteEnterpriseInfo() 
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            //ReminderBLL objReminderBLL = new ReminderBLL();
            //bool flag = objReminderBLL.GetEnterpriseInfo(pf.EnterpriseID);
            //return Json(flag);

            OrganUnitStatusInfo result = InterfaceWeb.BaseDataDAL.GetStatus(pf.MainCode);
            BaseResultModel objRetResult = new BaseResultModel();
            if (result != null)
            {
                objRetResult = ToJson.NewModelToJson(result, result.ResultCode.ToString(), result.ResultMsg.ToString());
            }

            return Json(objRetResult);
        }
    }
}
