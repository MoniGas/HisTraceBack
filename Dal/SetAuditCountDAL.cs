/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-25

** 修改人：xxx

** 修改时间：xxxx-xx-xx  

** 描述：主要用于设置监管部门审核码数量的数据库操作

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Log;
using LinqModel;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 主要用于设置监管部门审核码数量的数据库操作
    /// </summary>
    public class SetAuditCountDAL : DALBase
    {
        /// <summary>
        /// 获取设置监管部门审核码列表
        /// </summary>
        /// <param name="genId">标记ID</param>
        /// <param name="totalCount">总数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public List<View_SetAuditCount> GetList(long upid, long pId, out long totalCount, int pageIndex)
        {
            List<View_SetAuditCount> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_SetAuditCount.Where(m => m.PRRU_PlatFormUP_ID == upid && m.PRRU_PlatForm_ID == pId);
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.SetAuditID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "SetAuditCountDAL.GetList():SetAuditCount表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取设置码数量
        /// </summary>
        /// <param name="pid">监管部门ID</param>
        /// <returns></returns>
        public SetAuditCount GetModel(long pid)
        {
            SetAuditCount model = new SetAuditCount();
            List<SetAuditCount> countAll = new List<SetAuditCount>();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    long? all = 0;
                    long? allaudit = 0;
                    countAll = dataContext.SetAuditCount.Where(m => m.PRRU_PlatForm_ID == pid).ToList();
                    List<AuditCodeRecord> auditedcount = dataContext.AuditCodeRecord.Where(m => m.PRRU_PlatForm_ID == pid).ToList();
                    if (countAll.Count > 0)
                    {
                        foreach (var item in countAll)
                        {
                            model.AuditCountAll = item.AuditCountAll;
                            all += model.AuditCountAll;
                        }
                    }
                    if (auditedcount.Count > 0)
                    {
                        foreach (var item in auditedcount)
                        {
                            model.AuditedCount = item.AuditCount;
                            allaudit += model.AuditedCount;
                        }
                    }
                    model.AuditCountAll = all;
                    if (allaudit == null)
                    {
                        model.AuditedCount = 0;
                    }
                    else
                    {
                        model.AuditedCount = allaudit;
                    }
                    //countAll = dataContext.SetAuditCount.Where(m => m.PRRU_PlatForm_ID == pid).ToList();
                    ClearLinqModel(model);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
                return model;
            }
        }

        /// <summary>
        /// 设置审核码数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(SetAuditCount model)
        {
            string Msg = "设置失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.SetAuditCount.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.SetAuditID;
                    Msg = "设置成功";
                    error = CmdResultError.NONE;
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
    }
}
