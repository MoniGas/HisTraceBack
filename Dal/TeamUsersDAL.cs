/********************************************************************************
** 作者： 靳晓聪

** 创始时间：2016-10-25

** 联系方式 :15031109901

** 描述：人员数据层

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
    /// 人员数据层
    /// </summary>
    public class TeamUsersDAL : DALBase
    {
        /// <summary>
        /// 查找人员
        /// </summary>
        /// <param name="teamName">人员名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>人员信息</returns>
        public List<TeamUsers> GetList(long id, long enterpriseId, string userName, out long totalCount, int pageIndex)
        {
            List<TeamUsers> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.TeamUsers.Where(m => m.EnterpriseID == enterpriseId && m.TeamID == id && m.Status == (int)Common.EnumFile.Status.used);
                    //if (!string.IsNullOrEmpty(userName))
                    //{
                    //    data = data.Where(m => m.UserName.Contains(userName.Trim()));
                    //}
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.TeamUsersID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "TeamUsersDAL.GetList():TeamUsers表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
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
            List<TeamUsers> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.TeamUsers.Where(m => m.EnterpriseID == enterpriseId && m.TeamID == id && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    result = data;
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "TeamUsersDAL.GetList():TeamUsers表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加人员信息
        /// </summary>
        /// <param name="model">人员实体类</param>
        /// <returns></returns>
        public RetResult Add(TeamUsers model)
        {
            string Msg = "添加人员信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.TeamUsers.FirstOrDefault(m => m.UserName == model.UserName && m.EnterpriseID == model.EnterpriseID
                        && m.TeamID == model.TeamID && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "已存在该人员名称！";
                    }
                    else
                    {
                        dataContext.TeamUsers.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = model.TeamUsersID;
                        Msg = "添加人员信息成功";
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
        /// 根据人员ID获取人员信息
        /// </summary>
        /// <param name="teamUsersID">人员ID</param>
        /// <returns></returns>
        public TeamUsers GetTeamUsersByID(long teamUsersID)
        {
            TeamUsers model = new TeamUsers();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.TeamUsers.FirstOrDefault(t => t.TeamUsersID == teamUsersID);
                    ClearLinqModel(model);
                }
            }
            catch
            {
            }
            return model;
        }

        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <param name="TeamUsers"></param>
        /// <returns></returns>
        public RetResult Edit(TeamUsers teamUsers)
        {
            string Msg = "修改人员信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = from m in dataContext.TeamUsers
                                where m.UserName == teamUsers.UserName && m.TeamUsersID != teamUsers.TeamUsersID
                                    && m.EnterpriseID == teamUsers.EnterpriseID && m.Status == (int)Common.EnumFile.Status.used
                                select m;
                    if (model.Count() > 0)
                    {
                        Msg = "已存在该人员！";
                    }
                    else
                    {
                        var data = dataContext.TeamUsers.FirstOrDefault(m => m.EnterpriseID == teamUsers.EnterpriseID
                           && m.TeamUsersID == teamUsers.TeamUsersID);
                        if (data == null)
                        {
                            Msg = "没有找到要修改的数据！";
                        }
                        else
                        {
                            data.UserName = teamUsers.UserName;
                            data.UserPhone = teamUsers.UserPhone;
                            data.UserNumber = teamUsers.UserNumber;
                            data.SetAccount = teamUsers.SetAccount;
                            data.SetTime = DateTime.Now;
                            dataContext.SubmitChanges();
                            Msg = "修改人员信息成功！";
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
        /// 删除人员信息
        /// </summary>
        /// <param name="teamUsersID">人员ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public RetResult Delete(long teamUsersID, long enterpriseId)
        {
            string Msg = "删除人员失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    TeamUsers model = dataContext.TeamUsers.SingleOrDefault(m => m.TeamUsersID == teamUsersID);
                    if (model == null)
                    {
                        Msg = "没有找到要删除的人员信息请刷新列表！";
                    }
                    else
                    {
                        model.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Msg = "删除人员信息成功！";
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
    }
}
