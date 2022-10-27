/********************************************************************************
** 作者： 赵慧敏

** 创始时间：2016-10-21

** 联系方式 :13313318725

** 描述：班组控制器

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
    /// 班组控制器
    /// </summary>
    public class TeamController : Controller
    {
        /// <summary>
        /// 查找班组
        /// </summary>
        /// <param name="teamName">班组名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>班组信息</returns>
        public JsonResult Index(string teamName, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                TeamBLL bll = new TeamBLL();
                result = bll.GetList(pf.EnterpriseID, teamName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Team.Index()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加班组
        /// </summary>
        /// <param name="teamName">班组名称</param>
        /// <param name="remark">班组备注</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(string teamName, string remark)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Team model = new Team();
                model.EnterpriseID = pf.EnterpriseID;
                model.TeamName = teamName;
                model.Remark = remark;
                model.SetTime = DateTime.Now;
                model.Status = (int)Common.EnumFile.Status.used;
                TeamBLL bll = new TeamBLL();
                result = bll.Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Team.Add():Team表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取修改的班组实体类信息
        /// </summary>
        /// <param name="id">班组ID</param>
        /// <returns></returns>
        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                TeamBLL bll = new TeamBLL();
                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "AdminTeam.GetModel():Team表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改班组信息
        /// </summary>
        /// <param name="id">班组ID</param>
        /// <param name="originName">班组名称</param>
        /// <param name="descriptions">班组描述</param>
        /// <param name="originOriginImgInfo">班组图片</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string id, string teamName, string remark)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Team model = new Team();
                model.TeamID = Convert.ToInt64(id);
                model.EnterpriseID = pf.EnterpriseID;
                model.TeamName = teamName;
                model.Remark = remark;
                TeamBLL bll = new TeamBLL();
                result = bll.Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Team.Edit():Team表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除班组
        /// </summary>
        /// <param name="teamID">班组ID</param>
        /// <returns></returns>
        public JsonResult Delete(long teamID)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                TeamBLL bll = new TeamBLL();
                result = bll.Del(teamID, pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Team.Delete():Team表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取班组列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult TeamList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            TeamBLL bll = new TeamBLL();
            BaseResultList result = new BaseResultList();
            try
            {
                result = bll.GetList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Team.TeamList():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        //[HttpPost]
        //public JsonResult GetZuoyeModel(long materialID)
        //{
        //    LoginInfo pf = Common.Argument.SessCokie.Get;
        //    SetTeamBLL bll = new SetTeamBLL();
        //    BaseResultList result = new BaseResultList();
        //    try
        //    {
        //        result = bll.GetModel(materialID,pf.EnterpriseID);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errData = "Team.TeamList():";
        //        WriteLog.WriteErrorLog(errData + ":" + ex.Message);
        //    }
        //    return Json(result);
        //}
    }
}
