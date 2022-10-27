/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-12-20

** 联系方式 :13313318725

** 描述：生产流程管理数据访问 移植

** 版本：v1.0

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
    /// 生产流程管理数据访问
    /// </summary>
    public class ProcessDAL : DALBase
    {
        /// <summary>
        /// 获取生产流程列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="processName">生产流程名称</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public List<Process> GetList(long enterpriseId, string processName, out long totalCount, int pageIndex)
        {
            List<Process> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Process.Where(m => m.EnterpriseID == enterpriseId &&
                        m.status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(processName))
                    {
                        data = data.Where(m => m.ProcessName.Contains(processName.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ProcessID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "ProcessDAL.GetList():Process表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加生产流程
        /// </summary>
        /// <param name="process">生产流程实体</param>
        /// <returns></returns>
        public RetResult Add(Process process)
        {
            string Msg = "添加生产流程失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Process.FirstOrDefault(m => m.ProcessName == process.ProcessName &&
                        m.status == (int)Common.EnumFile.Status.used && m.EnterpriseID == process.EnterpriseID);
                    //判断添加的生产流程是否重复
                    if (data != null)
                    {
                        Msg = "已存在该生产流程！";
                    }
                    else
                    {
                        dataContext.Process.InsertOnSubmit(process);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = process.ProcessID;
                        Msg = "添加生产流程成功";
                        error = CmdResultError.NONE;
                        //为统计表添加数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = process.EnterpriseID;
                        homeData.ProcessCount = 1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult result = dal.Update(homeData);
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
        /// 修改生产流程
        /// </summary>
        /// <param name="newModel">生产流程信息</param>
        /// <returns></returns>
        public RetResult Edit(Process newModel)
        {
            Ret.Msg = "修改生产流程信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Process.FirstOrDefault(p => p.ProcessID == newModel.ProcessID &&
                        p.EnterpriseID == newModel.EnterpriseID);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的生产流程信息！";
                    }
                    else
                    {
                        var a = dataContext.Process.FirstOrDefault(p => p.ProcessName == newModel.ProcessName &&
                            p.ProcessID != newModel.ProcessID && p.EnterpriseID == newModel.EnterpriseID &&
                            p.status == (int)Common.EnumFile.Status.used);
                        if (a == null)
                        {
                            model.ProcessName = newModel.ProcessName;
                            model.OperationList = newModel.OperationList;
                            model.Memo = newModel.Memo;
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改生产流程信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "已存在该生产流程！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ProcessDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 删除生产流程（修改状态为-1）
        /// </summary>
        /// <param name="processID">生产流程ID</param>
        /// <returns>返回结果正确/错误</returns>
        public RetResult Delete(long processID)
        {
            string Msg = "删除生产流程失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Process process = dataContext.Process.SingleOrDefault(m => m.ProcessID == processID);
                    //判断生产流程是否存在
                    if (process == null)
                    {
                        Msg = "没有找到要删除的数据请刷新列表！";
                    }
                    else
                    {
                        process.status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Msg = "删除生产流程成功！";
                        error = CmdResultError.NONE;
                        //为统计表删除数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = process.EnterpriseID;
                        homeData.ProcessCount = -1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult result = dal.Update(homeData);
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

        /// <summary>
        /// 获取生产流程信息
        /// </summary>
        /// <param name="processId">生产流程ID</param>
        /// <returns></returns>
        public Process GetModelByID(long processId)
        {
            Process process = new Process();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    process = dataContext.Process.FirstOrDefault(t => t.ProcessID == processId);
                    ClearLinqModel(process);
                }
            }
            catch
            {
            }
            return process;
        }

        /// <summary>
        /// 获取生产流程列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public List<Process> GetProcessList(long enterpriseId)
        {
            List<Process> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Process.Where(m => m.EnterpriseID == enterpriseId && m.status == (int)Common.EnumFile.Status.used).OrderByDescending(m => m.ProcessID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "ProcessDAL.GetProcessList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
    }
}
