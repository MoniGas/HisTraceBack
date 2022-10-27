/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-4

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于生产环节信息管理数据层

*********************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class OperationTypeDAL : DALBase
    {
        /// <summary>
        /// 获取生产环节列表信息方法
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="operationName">生产环节名称</param>
        /// <param name="id">生产环节类型</param>
        /// <returns>检索结果结合</returns>
        public List<Batch_ZuoYeType> GetList(long enterpriseID, int pageIndex, string operationName, out long totalCount, int type = -1)
        {
            List<Batch_ZuoYeType> list = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Batch_ZuoYeType.Where(d => d.Enterprise_Info_ID == enterpriseID && d.state == (int)Common.EnumFile.Status.used);

                    if (!string.IsNullOrEmpty(operationName))
                    {
                        data = data.Where(m => m.OperationTypeName.Contains(operationName.Trim()));
                    }
                    if (type != -1)
                    {
                        data = data.Where(m => m.type == type);
                    }
                    totalCount = data.Count();
                    if (pageIndex > 0)
                    {
                        list = data.OrderByDescending(m => m.Batch_ZuoYeType_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    ClearLinqModel(list);
                }
                catch (Exception e)
                {
                    string errData = "OperationTypeDAL.GetList():Batch_ZuoYeType表";
                    WriteLog.WriteErrorLog(errData + ":" + e.Message);
                }
            }
            return list;
        }
        public List<Batch_ZuoYeType> GetListOp(long enterpriseID, int type)
        {
            List<Batch_ZuoYeType> list = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Batch_ZuoYeType.Where(d => d.Enterprise_Info_ID == enterpriseID && d.state == (int)Common.EnumFile.Status.used);
                    //if (type != -1)
                    //{
                    //    data = data.Where(m => m.type == type);
                    //}
                    list = data.ToList();
                    ClearLinqModel(list);
                }
                catch (Exception e)
                {
                    string errData = "OperationTypeDAL.GetList():Batch_ZuoYeType表";
                    WriteLog.WriteErrorLog(errData + ":" + e.Message);
                }
            }
            return list;
        }
        /// <summary>
        /// 根据ID查询生产环节信息
        /// </summary>
        /// <param name="id">生产环节ID</param>
        /// <returns></returns>
        public Batch_ZuoYeType SearchData(long id)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Batch_ZuoYeType.Where(d => d.Batch_ZuoYeType_ID == id && d.state == 0).FirstOrDefault();
                    ClearLinqModel(data);
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 添加生产环节方法
        /// </summary>
        /// <param name="operationType">生产环节LINQ模型</param>
        /// <returns>操作结果</returns>
        public RetResult Add(LinqModel.Batch_ZuoYeType operationType)
        {
            string Msg = "添加生产/加工类型失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = from d in dataContext.Batch_ZuoYeType
                               where d.Enterprise_Info_ID == operationType.Enterprise_Info_ID
                               && d.OperationTypeName == operationType.OperationTypeName
                               && d.state == 0
                               select d;
                    if (data.Count() > 0)
                    {
                        Msg = "已存在该生产环节类型！";
                    }
                    else
                    {
                        dataContext.Batch_ZuoYeType.InsertOnSubmit(operationType);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = operationType.Batch_ZuoYeType_ID;
                        Msg = "添加生产环节成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
                Msg = "添加生产/加工类型失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 修改生产环节信息方法
        /// </summary>
        /// <param name="objBatch_ZuoYeType">生产环节linq模型</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Batch_ZuoYeType objBatch_ZuoYeType)
        {
            string Msg = "修改失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Batch_ZuoYeType.FirstOrDefault(p => p.Batch_ZuoYeType_ID != objBatch_ZuoYeType.Batch_ZuoYeType_ID &&
                        p.Enterprise_Info_ID == objBatch_ZuoYeType.Enterprise_Info_ID &&
                        p.OperationTypeName == objBatch_ZuoYeType.OperationTypeName && p.state == (int)EnumFile.Status.used);
                    if (null != data)
                    {
                        Msg = "已存在该生产环节！";
                    }
                    else
                    {
                        Batch_ZuoYeType dataInfo = dataContext.Batch_ZuoYeType.FirstOrDefault(p =>
                            p.Enterprise_Info_ID == objBatch_ZuoYeType.Enterprise_Info_ID &&
                            p.Batch_ZuoYeType_ID == objBatch_ZuoYeType.Batch_ZuoYeType_ID && p.state == (int)EnumFile.Status.used);
                        if (dataInfo != null)
                        {
                            dataInfo.OperationTypeName = objBatch_ZuoYeType.OperationTypeName;
                            dataInfo.lastdate = objBatch_ZuoYeType.lastdate;
                            dataInfo.lastuser = objBatch_ZuoYeType.lastuser;
                            dataInfo.Memo = objBatch_ZuoYeType.Memo;
                            dataContext.SubmitChanges();
                            Msg = "修改成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
            }
            catch
            {
                Msg = "修改失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 删除生产环节信息方法
        /// </summary>
        /// <param name="operationTypeID">生产环节ID</param>
        /// <returns></returns>
        public RetResult Del(string operationTypeID)
        {
            string Msg = "删除失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    Batch_ZuoYeType operationType = dataContext.Batch_ZuoYeType.FirstOrDefault(m => m.Batch_ZuoYeType_ID == Convert.ToInt32(operationTypeID.Trim()));
                    int count = dataContext.Batch_ZuoYe.Where(w => w.Batch_ZuoYeType_ID == Convert.ToInt64(operationTypeID) && w.Status == (int)EnumFile.Status.used).Count();
                    if (operationType == null)
                    {
                        Msg = "您删除的数据不存在，请刷新后重试！";
                        error = CmdResultError.PARAMERROR;
                    }
                    else
                    {
                        if (count != 0)
                        {
                            Msg = "该生产环节已经被使用，目前无法删除！";
                            error = CmdResultError.Other;
                        }
                        else
                        {
                            operationType.state = (int)Common.EnumFile.Status.delete;

                            dataContext.SubmitChanges();
                            Msg = "删除成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
            }
            catch
            {
                Msg = "数据库连接失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 根据ID获取生产环节信息
        /// </summary>
        /// <param name="id">生产环节ID</param>
        /// <returns>返回生产环节信息</returns>
        public Batch_ZuoYeType GetInfo(long id)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    Batch_ZuoYeType data = (from d in dataContext.Batch_ZuoYeType
                                            where d.Batch_ZuoYeType_ID == id
                                            select d).FirstOrDefault();

                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
