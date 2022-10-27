/********************************************************************************
** 作者： 靳晓聪

** 创始时间：2016-10-25

** 联系方式 :15031109901

** 描述：人员业务层

** 版本：v1.1.0

** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Dal;
using Common.Argument;
using System.Configuration;

namespace BLL
{
    /// <summary>
    /// 人员业务层
    /// </summary>
    public class TeamUsersBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 查找人员
        /// </summary>
        /// <param name="teamName">人员名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>人员信息</returns>
        public BaseResultList GetList(long id, long enterpriseId, string teamName, int pageIndex)
        {
            long totalCount = 0;
            TeamUsersDAL dal = new TeamUsersDAL();
            List<TeamUsers> model = dal.GetList(id, enterpriseId, teamName, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 查找人员
        /// </summary>
        /// <param name="teamName">人员名称</param>
        /// <param name="enterpriseId">企业id</param>
        /// <returns>人员信息</returns>
        public List<TeamUsers> GetList(long id, long enterpriseId)
        {
            return new TeamUsersDAL().GetList(id, enterpriseId);
        }

        /// <summary>
        /// 添加人员信息
        /// </summary>
        /// <param name="model">人员实体类</param>
        /// <returns></returns>
        public BaseResultModel Add(TeamUsers model)
        {
            TeamUsersDAL dal = new TeamUsersDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.UserName))
            {
                ret.Msg = "人员名称不能为空！";
            }
            else
            {
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取人员实体类信息
        /// </summary>
        /// <param name="teamUsersID">人员ID</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long teamUsersID)
        {
            TeamUsersDAL dal = new TeamUsersDAL();
            TeamUsers model = dal.GetTeamUsersByID(teamUsersID);
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <param name="newModel">要修改的人员实体类</param>
        /// <returns></returns>
        public BaseResultModel Edit(TeamUsers newModel)
        {
            TeamUsersDAL dal = new TeamUsersDAL();
            RetResult ret = new RetResult(); ;
            if (string.IsNullOrEmpty(newModel.UserName))
            {
                ret.Msg = "人员名称不能为空！";
            }
            else
            {
                ret = dal.Edit(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除人员信息
        /// </summary>
        /// <param name="teamUsersID">人员ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultModel Del(long teamUsersID, long enterpriseId)
        {
            TeamUsersDAL dal = new TeamUsersDAL();
            RetResult ret = dal.Delete(teamUsersID, enterpriseId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
    }
}
