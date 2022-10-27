using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using System.Configuration;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysEnterpriseManageController : Controller
    {
        //
        // GET: /Admin/SysEnterpriseManage/

        public JsonResult Index(string name, int? pageIndex)
        {
            SysEnterpriseManageBLL objSysEnterpriseManageBLL = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.Get;

            LinqModel.BaseResultList dataList = objSysEnterpriseManageBLL.GetEnterpriseInfoList(name, pf.EnterpriseID, pageIndex);

            return Json(dataList);
        }

        public JsonResult SearchEnterprise()
        {
            LoginInfo pf = SessCokie.Get;
            SysEnterpriseManageBLL objSysEnterpriseManageBLL = new SysEnterpriseManageBLL();

            LinqModel.BaseResultList dataList = objSysEnterpriseManageBLL.GetAreaEnterprise(pf.shengId, pf.shiId);

            return Json(dataList);
        }

        public JsonResult GetEnterprise()
        {
            LoginInfo pf = SessCokie.Get;
            SysEnterpriseManageBLL objSysEnterpriseManageBLL = new SysEnterpriseManageBLL();

            LinqModel.BaseResultList dataList = objSysEnterpriseManageBLL.GetAreaEnterprise(pf.shengId, pf.shiId, pf.EnterpriseID);

            return Json(dataList);
        }

        public JsonResult Save(string arrayId, string falseId)
        {
            LoginInfo pf = SessCokie.Get;
            SysEnterpriseManageBLL objSysEnterpriseManageBLL = new SysEnterpriseManageBLL();

            LinqModel.BaseResultModel result = objSysEnterpriseManageBLL.Save(pf.EnterpriseID, arrayId, falseId);

            return Json(result);
        }

        public JsonResult VerifyEnterprise(string enterpriseid, string type)
        {
            LinqModel.BaseResultModel result = new SysEnterpriseManageBLL().VerifyEnterprise(enterpriseid, type);
            return Json(result);
        }

        public JsonResult GetTryDays(string enterpriseid)
        {
            Result result = new Result();
            Enterprise_Info enterprise = new EnterpriseInfoBLL().GetModel(Convert.ToInt32(enterpriseid));
            string date = "永久";
            if (enterprise.Verify != 1)
            {
                int tryDays = Convert.ToInt32(ConfigurationManager.AppSettings["trydays"]);
                date = Convert.ToDateTime(enterprise.Stradddate).AddDays(tryDays).ToString("yyyy-MM-dd");
            }
            result.ResultMsg = date;
            return Json(result);
        }

        public JsonResult GetEnterpriseShop(string name, int? pageIndex)
        {
            SysEnterpriseManageBLL objSysEnterpriseManageBLL = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.Get;

            LinqModel.BaseResultList dataList = objSysEnterpriseManageBLL.GetEnterprise(name, pf.EnterpriseID, pageIndex);

            return Json(dataList);
        }

        public JsonResult VerifyShop(string enterpriseid, string type)
        {
            LinqModel.BaseResultModel result = new SysEnterpriseManageBLL().VerifyShop(enterpriseid, type);
            return Json(result);
        }

        public JsonResult GetEnterpriseWareHouse(string name, int? pageIndex)
        {
            SysEnterpriseManageBLL objSysEnterpriseManageBLL = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.Get;

            LinqModel.BaseResultList dataList = objSysEnterpriseManageBLL.GetEnterpriseInfoList(name, pf.EnterpriseID, pageIndex, true);

            return Json(dataList);
        }

        public JsonResult VerifyWareHouse(string enterpriseid, string type)
        {
            LinqModel.BaseResultModel result = new SysEnterpriseManageBLL().VerifyWareHouse(enterpriseid, type);
            return Json(result);
        }

        /// <summary>
        /// 设置申请码数量
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="count">审核数量</param>
        /// <returns></returns>
        public JsonResult SetRequestCount(long eId, long count)
        {
            BaseResultModel result = new SysEnterpriseManageBLL().SetRequestCount(eId, count);
            return Json(result);
        }

        /// <summary>
        /// 管理员/监管部门给企业重置密码
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="password">重置的密码</param>
        /// <returns></returns>
        public JsonResult SetPassWord(long eId, string password)
        {
            BaseResultModel result = new SysEnterpriseManageBLL().SetPassWord(eId, password);
            return Json(result);
        }
    }
}
