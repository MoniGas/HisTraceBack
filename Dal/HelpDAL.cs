/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-19

** 联系方式 :13313318725

** 描述：主要用于帮助管理数据层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    /// <summary>
    /// 主要用于帮助管理数据层
    /// </summary>
    public class HelpDAL : DALBase
    {
        /// <summary>
        /// 帮助管理——获取帮助列表
        /// </summary>
        /// <param name="HelpTitle">帮助名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>操作结果</returns>
        public List<View_Help> GetList(string HelpTitle, int pageIndex, out long totalCount)
        {
            List<View_Help> result = new List<View_Help>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_Help.Where(m => m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(HelpTitle))
                    {
                        tempResult = tempResult.Where(m => m.HelpTitle.Contains(HelpTitle.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Sort).ThenByDescending(m => m.Count).
                        ThenByDescending(m => m.HelpId).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "HelpDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 帮助管理——添加帮助
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Add(Help model)
        {
            Ret.Msg = "添加信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempModel = dataContext.Help.FirstOrDefault(p => p.HelpTitle == model.HelpTitle && p.Status == (int)Common.EnumFile.Status.used);
                    if (tempModel != null)
                    {
                        Ret.Msg = "已存在该名称！";
                    }
                    else
                    {
                        dataContext.Help.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Ret.Msg = "添加信息成功";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        /// <summary>
        /// 帮助管理——获取帮助模型
        /// </summary>
        /// <param name="HelpId">帮助标识</param>
        /// <returns>实体</returns>
        public Help GetModel(long HelpId)
        {
            Help result = new Help();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Help.FirstOrDefault(m => m.HelpId == HelpId && m.Status == (int)Common.EnumFile.Status.used);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "HelpDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 帮助管理——修改帮助
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Help newModel)
        {
            Ret.Msg = "修改信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Help.FirstOrDefault(p => p.HelpId == newModel.HelpId);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的信息！";
                    }
                    else
                    {
                        var a = dataContext.Help.FirstOrDefault(p => p.HelpTitle == newModel.HelpTitle && p.HelpId != newModel.HelpId
                            && p.Status == (int)Common.EnumFile.Status.used);
                        if (a == null)
                        {
                            model.HelpTitle = newModel.HelpTitle;
                            model.TypeId = newModel.TypeId;
                            model.HelpDescriptions = newModel.HelpDescriptions;
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "已存在该名称！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "HelpDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 帮助管理——删除帮助
        /// </summary>
        /// <param name="HelpId">帮助标识</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long HelpId)
        {
            Ret.Msg = "删除信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Help.FirstOrDefault(p => p.HelpId == HelpId);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要删除的信息！";
                    }
                    else
                    {
                        model.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Ret.Msg = "删除信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "HelpDAL.Del()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;


        }

        /// <summary>
        ///  帮助管理——获取帮助类型列表（添加编辑的下拉列表）
        /// </summary>
        /// <returns></returns>
        public List<HelpType> GetList()
        {
            List<HelpType> result = new List<HelpType>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = from c in dataContext.HelpType
                                     select c;
                    result = tempResult.ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "HelpDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 帮助管理——置顶
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>操作结果</returns>
        public RetResult EditSort(Help newModel)
        {
            Ret.Msg = "置顶失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Help.FirstOrDefault(p => p.HelpId == newModel.HelpId);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要置顶的信息！";
                    }
                    else
                    {
                        var a = dataContext.Help.FirstOrDefault(p => p.HelpId != newModel.HelpId
                            && p.Status == (int)Common.EnumFile.Status.used);
                        if (a != null)
                        {
                            model.Sort = (int)Common.EnumFile.TopType.Top;
                            List<Help> helpList = dataContext.Help.Where(p => p.HelpId != newModel.HelpId).ToList();
                            foreach (var item in helpList)
                            {
                                item.Sort = (int)Common.EnumFile.TopType.Cancel;
                            }
                            dataContext.SubmitChanges();
                            Ret.Msg = "置顶成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "置顶失败！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "HelpDAL.EditSort()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 页面展示1——获取帮助列表
        /// </summary>
        /// <param name="HelpTitle">帮助名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>操作结果</returns>
        public List<View_Help> GetHelpList(string HelpTitle, int pageIndex, out long totalCount)
        {
            List<View_Help> result = new List<View_Help>();
            totalCount = 0;
            PageSize = 4;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_Help.Where(m => m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(HelpTitle))
                    {
                        tempResult = tempResult.Where(m => m.HelpTitle.Contains(HelpTitle.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Sort).ThenByDescending(m => m.Count).
                        ThenByDescending(m => m.HelpId).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "HelpDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 页面展示2——通过帮助类型获取帮助列表
        /// </summary>
        /// <param name="typeId">帮助类型typeId</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>操作结果</returns>
        public PagedList<View_Help> GetPagedList(long typeId, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var tempResult = dataContext.View_Help.Where(m => m.Status == (int)Common.EnumFile.Status.used);
                if (typeId != 0)
                {
                    tempResult = tempResult.Where(m => m.TypeId == typeId);
                }
                tempResult = tempResult.OrderByDescending(m => m.Sort).ThenByDescending(m => m.Count).
                        ThenByDescending(m => m.HelpId);
                return tempResult.ToPagedList(pageIndex ?? 1, PageSize);
            }
        }

        /// <summary>
        /// 页面展示2——通过帮助类型获取帮助列表
        /// </summary>
        /// <param name="typeId">帮助类型typeId</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>操作结果</returns>
        public PagedList<View_Help> GetMoreList(long typeId, int index, string name, int? pageIndex)
        {
            PageSize = PageSize * index;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var tempResult = dataContext.View_Help.Where(m => m.Status == (int)Common.EnumFile.Status.used);
                if (typeId != 0)
                {
                    tempResult = tempResult.Where(m => m.TypeId == typeId);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    tempResult = tempResult.Where(m => m.HelpTitle.Contains(name));
                }
                tempResult = tempResult.OrderByDescending(m => m.Sort).ThenByDescending(m => m.Count).
                        ThenByDescending(m => m.HelpId);
                return tempResult.ToPagedList(pageIndex ?? 1, PageSize);
            }
        }

        /// <summary>
        /// 页面展示3——获取帮助详细信息
        /// </summary>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        public View_Help GetDetails(long helpId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                View_Help data =new View_Help();
                if (helpId != 0)
                {
                    data = dataContext.View_Help.FirstOrDefault(m => m.HelpId == helpId && m.Status == (int)Common.EnumFile.Status.used);
                }
                return data;
            }
        }

        /// <summary>
        /// 页面展示——设置访问量
        /// </summary>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        public Help UpdateCount(long helpId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Help data = new Help();
                    if (helpId != 0)
                    {
                        data = dataContext.Help.FirstOrDefault(m => m.HelpId == helpId && m.Status == (int)Common.EnumFile.Status.used);
                        if (data != null)
                        {
                            data.Count += 1;
                            dataContext.SubmitChanges();
                        }
                    }
                    return data;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 页面展示——设置有效数量
        /// </summary>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        public RetResult UpdateUsefulCount(int type,long helpId)
        {
            string Msg = "访问失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Help data = new Help();
                    if (helpId != 0)
                    {
                        data = dataContext.Help.FirstOrDefault(m => m.HelpId == helpId && m.Status == (int)Common.EnumFile.Status.used);
                        if (data == null)
                        {
                            Msg = "没有找到数据！";
                        }
                        else
                        {
                            if (type == (int)Common.EnumFile.UsefulType.Yes)
                            {
                                data.UsefulCount += 1;
                            }
                            else
                            {
                                data.NoCount += 1;
                            }
                            dataContext.SubmitChanges();
                            Msg = "提交成功，感谢您的评价！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
                catch (Exception)
                {
                    Msg = "连接服务器失败！";
                }
                Ret.SetArgument(error, Msg, Msg);
                return Ret;
            }
        }
    }
}
