/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-24

** 主要用于代理管理控制器层

*********************************************************************************/

using System.Web.Mvc;
using BLL;
using Common.Argument;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 用于关联监管部门控制器层
    /// </summary>
    public class SysPlatFormController : Controller
    {
        /// <summary>
        ///  获取该服务中心下的监管部门列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(string name, int? pageIndex)
        {
            SysPlatFormManageBLL bll = new SysPlatFormManageBLL();
            LoginInfo pf = SessCokie.Get;
            LinqModel.BaseResultList dataList = bll.GetPlatFormList(name, pf.EnterpriseID, pageIndex);
            return Json(dataList);
        }

        /// <summary>
        /// 关联监管部门列表
        /// </summary>
        /// <returns></returns>
        public JsonResult SearchPlatForm()
        {
            LoginInfo pf = SessCokie.Get;
            SysPlatFormManageBLL bll = new SysPlatFormManageBLL();
            LinqModel.BaseResultList dataList = bll.GetAreaPlatForm(pf.shengId, pf.shiId);
            return Json(dataList);
        }

        /// <summary>
        /// 关联监管部门列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPlatForm()
        {
            LoginInfo pf = SessCokie.Get;
            SysPlatFormManageBLL bll = new SysPlatFormManageBLL();
            LinqModel.BaseResultList dataList = bll.GetAreaPlatForm(pf.shengId, pf.shiId, pf.EnterpriseID);
            return Json(dataList);
        }

        /// <summary>
        /// 保存关联
        /// </summary>
        /// <param name="arrayId"></param>
        /// <param name="falseId"></param>
        /// <returns></returns>
        public JsonResult Save(string arrayId, string falseId)
        {
            LoginInfo pf = SessCokie.Get;
            SysPlatFormManageBLL bll = new SysPlatFormManageBLL();
            LinqModel.BaseResultModel result = bll.Save(pf.EnterpriseID, arrayId, falseId);
            return Json(result);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult EditPWD()
        {
            return View();
        }
        /// <summary>
        /// 管理员修改自己密码
        /// </summary>
        /// <param name="eId">标识ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="surePassword">确认密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditPWD(string oldPassword, string newPassword, string surePassword)
        {
            LoginInfo pf = SessCokie.GetMan;
            SysPlatFormManageBLL bll = new SysPlatFormManageBLL();
            RetResult ret = bll.EditPWD(pf.EnterpriseID, oldPassword, newPassword, surePassword);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
    }
}
