/********************************************************************************
** 作者： 赵慧敏

** 创始时间：2016-10-21

** 联系方式 :13313318725

** 描述：班组业务层

** 版本：v1.1.0

** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using Dal;
using LinqModel;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 班组业务层
    /// </summary>
    public class TeamBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 查找班组
        /// </summary>
        /// <param name="teamName">班组名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>班组信息</returns>
        public BaseResultList GetList(long enterpriseId, string teamName, int pageIndex)
        {
            long totalCount = 0;
            TeamDAL dal = new TeamDAL();
            List<View_Team> model = dal.GetList(enterpriseId, teamName, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 添加班组信息
        /// </summary>
        /// <param name="model">班组实体类</param>
        /// <returns></returns>
        public BaseResultModel Add(Team model)
        {
            TeamDAL dal = new TeamDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.TeamName))
            {
                ret.Msg = "班组名称不能为空！";
            }
            else
            {
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取班组实体类信息
        /// </summary>
        /// <param name="originId">班组ID</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long teamID)
        {
            TeamDAL dal = new TeamDAL();
            Team model = dal.GetOriginByID(teamID);
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        /// <summary>
        /// 修改班组信息
        /// </summary>
        /// <param name="newModel">要修改的班组实体类</param>
        /// <returns></returns>
        public BaseResultModel Edit(Team newModel)
        {
            TeamDAL dal = new TeamDAL();
            RetResult ret = new RetResult(); ;
            if (string.IsNullOrEmpty(newModel.TeamName))
            {
                ret.Msg = "班组名称不能为空！";
            }
            else
            {
                ret = dal.Edit(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除班组信息
        /// </summary>
        /// <param name="teamID">班组ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultModel Del(long teamID, long enterpriseId)
        {
            TeamDAL dal = new TeamDAL();
            RetResult ret = dal.Delete(teamID, enterpriseId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        ///  获取班组（下拉列表）
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId)
        {
            TeamDAL dal = new TeamDAL();
            List<Team> model = dal.GetList(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }

        /// <summary>
        /// 获取班组人员信息
        /// </summary>
        /// <param name="teamid">班组ID</param>
        /// <returns>班组人员信息列表</returns>
        public BaseResultList GetPersonList(long Id)
        {
            TeamDAL dal = new TeamDAL();
            List<TeamUsers> DataList = dal.GetPersonList(Id);
            return ToJson.NewListToJson(DataList, 1, 100000, DataList.Count, "");
        }
    }
}
