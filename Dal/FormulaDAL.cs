/********************************************************************************

** 作者： 李子巍

** 创始时间：2017-02-27

** 联系方式 :13313318725

** 描述：主要用于配方管理数据层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;
using System.Xml.Linq;

namespace Dal
{
    /// <summary>
    /// 主要用于配方管理数据层
    /// </summary>
    public class FormulaDAL : DALBase
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="materialId">产品ID</param>
        /// <returns>列表</returns>
        public List<View_Formula> GetSelectList(long materialId)
        {
            List<View_Formula> result = new List<View_Formula>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_Formula.Where(m => m.Status == (int)Common.EnumFile.Status.used);
                    if (materialId > 0)
                    {
                        data = data.Where(m => m.MaterialID == materialId);
                    }
                    result = data.ToList();
                }
                catch (Exception ex)
                {
                    string errData = "FormulaDAL.GetList():View_Formula表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取配方列表
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="name">检索</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总数</param>
        /// <returns>列表</returns>
        public List<View_Formula> GetList(long enterpriseID, string name, int pageIndex, out long totalCount)
        {
            List<View_Formula> result = new List<View_Formula>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_Formula.Where(m => m.EnterpriseID == enterpriseID 
                        && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(name.Trim()) || m.FormulaName.Contains(name.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.FormulaID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "FormulaDAL.GetList():View_Formula表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取配料列表
        /// </summary>
        /// <param name="formulaID">配方</param>
        /// <returns>；列表</returns>
        public List<View_FormulaDetail> GetSubList(long formulaID)
        {
            List<View_FormulaDetail> result = new List<View_FormulaDetail>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.View_FormulaDetail.Where(m => m.FormulaID == formulaID 
                        && m.Status == (int)Common.EnumFile.Status.used).ToList();
                }
            }
            catch (Exception ex)
            {
                string errData = "FormulaDAL.GetSubList():View_FormulaDetail表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 添加配方
        /// </summary>
        /// <param name="model">配方信息</param>
        /// <param name="liSub">原料列表</param>
        /// <returns>操作结果</returns>
        public RetResult Add(Formula model, List<FormulaDetail> liSub)
        {
            string Msg = "添加配方信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Formula.FirstOrDefault(m => m.FormulaName == model.FormulaName && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "配方名称重复！";
                    }
                    else
                    {
                        dataContext.Formula.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        for (int i = 0; i < liSub.Count; i++)
                        {
                            liSub[i].FormulaID = model.FormulaID;
                        }
                        dataContext.FormulaDetail.InsertAllOnSubmit(liSub);
                        dataContext.SubmitChanges();
                        error = CmdResultError.NONE;
                        Msg = "添加配方信息成功！";
                    }
                }
            }
            catch
            {
                Msg = "链接数据库失败！";
                if (model.FormulaID > 0)
                {
                    try
                    {
                        using (DataClassesDataContext dataContext = GetDataContext())
                        {
                            dataContext.Formula.DeleteOnSubmit(
                                dataContext.Formula.FirstOrDefault(m => m.FormulaID == model.FormulaID)
                            );
                            dataContext.SubmitChanges();
                        }
                    }
                    catch { }
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 修改原料
        /// </summary>
        /// <param name="model">配方信息</param>
        /// <param name="liSub">原料列表</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Formula model, List<FormulaDetail> liSub)
        {
            string Msg = "修改配方信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Formula.FirstOrDefault(m => m.FormulaID != model.FormulaID &&
                        m.FormulaName == model.FormulaName && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "配方名称重复！";
                    }
                    else
                    {
                        data = dataContext.Formula.FirstOrDefault(m => m.FormulaID == model.FormulaID);
                        data.FormulaName = model.FormulaName;
                        data.MaterialID = model.MaterialID;
                        data.Status = (int)Common.EnumFile.Status.used;
                        data.AddTime = model.AddTime;
                        data.AddUser = model.AddUser;
                        data.Spec = model.Spec;
                        dataContext.FormulaDetail.DeleteAllOnSubmit(
                            dataContext.FormulaDetail.Where(m => m.FormulaID == model.FormulaID)
                        );
                        for (int i = 0; i < liSub.Count; i++)
                        {
                            liSub[i].FormulaID = model.FormulaID;
                        }
                        dataContext.FormulaDetail.InsertAllOnSubmit(liSub);
                        dataContext.SubmitChanges();
                        error = CmdResultError.NONE;
                        Msg = "修改配方信息成功！";
                    }
                }
            }
            catch
            {
                Ret.Msg = "链接数据库失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 删除配方
        /// </summary>
        /// <param name="formulaId">配方ID</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long formulaId)
        {
            string Msg = "删除配方信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Formula.FirstOrDefault(m => m.FormulaID == formulaId);
                    if (data == null)
                    {
                        Msg = "没有找到要删除的配方！";
                    }
                    else
                    {
                        data.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        error = CmdResultError.NONE;
                        Msg = "删除配方信息成功！";
                    }
                }
            }
            catch
            {
                Ret.Msg = "链接数据库失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 从配方获取原料
        /// </summary>
        /// <param name="settingId">追溯信息ID</param>
        /// <param name="formulaId">配方ID</param>
        /// <returns>操作结果</returns>
        public RetResult GetOriginByFormula(long settingId, long formulaId)
        {
            string Msg = "获取原材料信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.RequestOrigin.DeleteAllOnSubmit(
                        dataContext.RequestOrigin.Where(m => m.SettingID == settingId && m.Type == 1)
                    );
                    Formula formula = dataContext.Formula.FirstOrDefault(m => m.FormulaID == formulaId);
                    var liSub = dataContext.FormulaDetail.Where(m => m.FormulaID == formula.FormulaID);
                    List<RequestOrigin> li = new List<RequestOrigin>();
                    foreach (var item in liSub)
                    {
                        RequestOrigin ro = new RequestOrigin();
                        ro.AddDate = DateTime.Now;
                        ro.CarNum = "";
                        ro.CheckUser = "";
                        ro.EnterpriseID = formula.EnterpriseID;
                        ro.InDate = DateTime.Now;
                        ro.OriginID = item.OriginID;
                        ro.SettingID = settingId;
                        ro.Status = (int)Common.EnumFile.Status.used;
                        ro.Supplier = item.Supplier;
                        ro.Img = XElement.Parse("<infos />");
                        ro.BatchNum = item.Batch;
                        ro.Factory = item.Factory;
                        ro.Level = item.Level;
                        ro.Type = 1;
                        li.Add(ro);
                    }
                    if (li != null && li.Count > 0)
                    {
                        dataContext.RequestOrigin.InsertAllOnSubmit(li);
                        dataContext.SubmitChanges();
                    }
                    error = CmdResultError.NONE;
                    Msg = "获取原材料信息成功！";
                }
            }
            catch
            {
                Ret.Msg = "链接数据库失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
    }
}
