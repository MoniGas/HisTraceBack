/********************************************************************************
** 作者： 赵慧敏

** 创始时间：2016-10-21

** 联系方式 :13313318725

** 描述：班组数据层

** 版本：v1.1.0

** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 班组数据层
    /// </summary>
    public class TeamDAL:DALBase
    {
        /// <summary>
        /// 查找班组
        /// </summary>
        /// <param name="teamName">班组名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>班组信息</returns>
        public List<View_Team> GetList(long enterpriseId, string teamName, out long totalCount, int pageIndex)
        {
            List<View_Team> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_Team.Where(m => m.EnterpriseID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(teamName))
                    {
                        data = data.Where(m => m.TeamName.Contains(teamName.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.TeamID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "TeamDAL.GetList():View_Team表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加班组信息
        /// </summary>
        /// <param name="model">班组实体类</param>
        /// <returns></returns>
        public RetResult Add(Team model)
        {
            string Msg = "添加班组信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Team.FirstOrDefault(m => m.TeamName == model.TeamName && m.EnterpriseID == model.EnterpriseID 
                        && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "已存在该班组名称！";
                    }
                    else
                    {
                        dataContext.Team.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = model.TeamID;
                        Msg = "添加班组信息成功";
                        error = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 根据班组ID获取班组信息
        /// </summary>
        /// <param name="teamID">班组ID</param>
        /// <returns></returns>
        public Team GetOriginByID(long teamID)
        {
            Team model = new Team();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.Team.FirstOrDefault(t => t.TeamID == teamID);
                    ClearLinqModel(model);
                }
            }
            catch
            {
            }
            return model;
        }

        /// <summary>
        /// 修改班组信息
        /// </summary>
        /// <param name="Team"></param>
        /// <returns></returns>
        public RetResult Edit(Team team)
        {
            string Msg = "修改班组信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = from m in dataContext.Team
                                where m.TeamName == team.TeamName && m.TeamID != team.TeamID
                                    && m.EnterpriseID == team.EnterpriseID && m.Status == (int)Common.EnumFile.Status.used
                                select m;
                    if (model.Count() > 0)
                    {
                        Msg = "已存在该班组！";
                    }
                    else
                    {
                        var data = dataContext.Team.FirstOrDefault(m => m.EnterpriseID == team.EnterpriseID && m.TeamID == team.TeamID);
                        if (data == null)
                        {
                            Msg = "没有找到要修改的数据！";
                        }
                        else
                        {
                            data.TeamName = team.TeamName;
                            data.Remark = team.Remark;
                            data.SetTime =DateTime.Now;
                            dataContext.SubmitChanges();
                            Msg = "修改班组信息成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
                Ret.SetArgument(error, Msg, Msg);
                return Ret;
            }
        }

        /// <summary>
        /// 删除班组信息
        /// </summary>
        /// <param name="teamID">班组ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public RetResult Delete(long teamID, long enterpriseId)
        {
            string Msg = "删除班组失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    Team model = dataContext.Team.SingleOrDefault(m => m.TeamID == teamID);
                    List<TeamUsers> teamUsersList = dataContext.TeamUsers.Where(m => m.TeamID == teamID).ToList();
                    if (model == null)
                    {
                        Msg = "没有找到要删除的班组信息请刷新列表！";
                    }
                    else
                    {
                        foreach (var item in teamUsersList)
                        {
                            item.Status = (int)Common.EnumFile.Status.delete;
                        }
                        model.Status = (int)Common.EnumFile.Status.delete;
                        //dataContext.Team.DeleteOnSubmit(model);
                        dataContext.SubmitChanges();
                        Msg = "删除班组信息成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
                //Msg = "删除失败，请首先删除已知关联的其他数据";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取班组（下拉列表）
        /// </summary>
        /// <param name="enterpriseId">企业Id</param>
        /// <returns></returns>
        public List<Team> GetList(long enterpriseId)
        {
            List<Team> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Team.Where(m => (m.EnterpriseID == enterpriseId  && m.Status==(int)Common.EnumFile.Status.used)
                        && m.Status == (int)Common.EnumFile.Status.used);
                    result = data.OrderByDescending(m => m.TeamID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "TeamDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取班组人员信息
        /// </summary>
        /// <param name="teamid">班组ID</param>
        /// <returns>班组人员信息列表</returns>
        public List<TeamUsers> GetPersonList(long teamid)
        {
            List<TeamUsers> DataList = new List<TeamUsers>();
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext())
                {
                    DataList = (from Data in DataContext.TeamUsers
                                where Data.TeamID == teamid && Data.Status == (int)Common.EnumFile.Status.used
                                select Data).ToList();
                    ClearLinqModel(DataList);
                    return DataList;
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
    }
}
