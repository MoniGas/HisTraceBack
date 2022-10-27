/********************************************************************************
** 作者： 靳晓聪

** 创始时间：2016-10-25

** 联系方式 :15031109901

** 描述：人员控制器

** 版本：v1.1.0

** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 人员控制器
    /// </summary>
    public class TeamUsersController : Controller
    {
        //
        // GET: /TeamUsers/

        /// <summary>
        /// 查找人员
        /// </summary>
        /// <param name="teamID">班组ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>人员信息</returns>
        public JsonResult Index(long teamID, int pageIndex = 1)
        {
            string usersName = "";
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                TeamUsersBLL bll = new TeamUsersBLL();
                result = bll.GetList(teamID, pf.EnterpriseID, usersName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "TeamUsers.Index()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="teamID">班组ID</param>
        /// <param name="userName">人员名称</param>
        /// <param name="userPhone">电话号码</param>
        /// <param name="userNumber">工牌号</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(long teamID,string userName, string userPhone, string userNumber)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                TeamUsers model = new TeamUsers();
                model.TeamID = teamID;
                model.EnterpriseID = pf.EnterpriseID;
                model.UserName = userName;
                model.UserPhone = userPhone;
                model.UserNumber = userNumber;
                model.SetTime = DateTime.Now;
                model.Status = (int)Common.EnumFile.Status.used;
                TeamUsersBLL bll = new TeamUsersBLL();
                result = bll.Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "TeamUsers.Add():TeamUsers表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取修改的人员实体类信息
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                TeamUsersBLL bll = new TeamUsersBLL();
                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "TeamUsers.GetModel():TeamUsers表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <param name="userName">人员名称</param>
        /// <param name="userPhone">人员电话</param>
        /// <param name="userNumber">工牌号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string id, string userName, string userPhone, string userNumber)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                TeamUsers model = new TeamUsers();
                model.TeamUsersID = Convert.ToInt64(id);
                model.EnterpriseID = pf.EnterpriseID;
                model.UserName = userName;
                model.UserPhone = userPhone;
                model.UserNumber = userNumber;
                TeamUsersBLL bll = new TeamUsersBLL();
                result = bll.Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "TeamUsers.Edit():TeamUsers表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="teamID">人员ID</param>
        /// <returns></returns>
        public JsonResult Delete(long teamUsersID)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                TeamUsersBLL bll = new TeamUsersBLL();
                result = bll.Del(teamUsersID, pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "TeamUsers.Delete():TeamUsers表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
