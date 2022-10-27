using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class Batch_ZuoYeDAL : DALBase
    {
        /// <summary>
        /// 获取作业信息列表
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="batchID"></param>
        /// <param name="type"></param>
        /// <param name="totlaCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<View_ZuoYeBatchMaterial> GetList(long enterpriseId, long batchID, long batchextid, long opid, int type, out long totlaCount, int pageIndex)
        {
            List<View_ZuoYeBatchMaterial> result = null;
            totlaCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_ZuoYeBatchMaterial.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used && m.Batch_ID == batchID && m.BatchExt_ID == batchextid);
                    if (batchID > 0)
                    {
                        data = data.Where(m => m.Batch_ID == batchID);
                    }
                    if (batchextid > 0)
                    {
                        data = data.Where(m => m.BatchExt_ID == batchextid);
                    }
                    if (opid > 0)
                    {
                        data = data.Where(m => m.Batch_ZuoYeType_ID == opid);
                    }
                    if (type > 0)
                    {
                        data = data.Where(m => m.type == type);
                    }
                    totlaCount = data.Count();
                    result = data.OrderByDescending(m => m.Batch_ZuoYe_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "Batch_ZuoYeDAL.GetList():View_ZuoYeBatchMaterial视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取企业作业信息
        /// </summary>
        /// <param name="eID">企业ID</param>
        /// <param name="status">审批状态</param>
        /// <returns>数据列表</returns>
        public List<View_ZuoYeAndZuoYeType> GetList(long eID, int status, long batch_ID)
        {
            List<View_ZuoYeAndZuoYeType> list = new List<View_ZuoYeAndZuoYeType>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data = from m in dataContext.View_ZuoYeAndZuoYeType select m;
                if (eID > 0)
                    data = data.Where(m => m.Enterprise_Info_ID == eID);
                if (status > -3)
                    data = data.Where(m => m.Status == status);
                if (batch_ID > 0)
                    data = data.Where(m => m.Batch_ID == batch_ID);
                if (data.Count() > 0)
                {
                    list = data.ToList();
                }
            }
            return list;
        }
        
        /// <summary>
        /// 添加作业信息By张
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(Batch_ZuoYe model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.Batch_ZuoYe.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.Batch_ZuoYe_ID;
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "添加成功");
                }
                catch (Exception ex)
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, ex.ToString(), "添加失败");
                }
            }
            return Ret;
        }
        /// <summary>
        /// 修改作业信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Edit(Batch_ZuoYe model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                Batch_ZuoYe newModel = dataContext.Batch_ZuoYe.FirstOrDefault(m => m.Batch_ZuoYe_ID == model.Batch_ZuoYe_ID);
                if (newModel != null)
                {
                    newModel.type = model.type;
                    newModel.Batch_ZuoYeType_ID = model.Batch_ZuoYeType_ID;
                    newModel.Content = model.Content;
                    newModel.AddDate = model.AddDate;
                    newModel.Files = model.Files;
                    newModel.UserName = model.UserName;
                    newModel.TeamID = model.TeamID;
                    newModel.UsersName = model.UsersName;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "修改成功");
                }
                else
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "修改失败");
                }
            }
            return Ret;
        }
        /// <summary>
        /// 删除作业信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eID"></param>
        /// <returns></returns>
        public RetResult Del(long id, long eID)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    Batch_ZuoYe model = dataContext.Batch_ZuoYe.FirstOrDefault(m => m.Batch_ZuoYe_ID == id);
                    if (model == null)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }

                    if (model.Enterprise_Info_ID != eID)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "您无权对该条信息进行操作");
                        return Ret;
                    }
                    model.Status = (int)Common.EnumFile.Status.delete;
                    //dataContext.Batch_ZuoYe.DeleteOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "删除成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "删除失败");
            }
            return Ret;
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Batch_ZuoYe GetModel(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                Batch_ZuoYe model = dataContext.Batch_ZuoYe.FirstOrDefault(m => m.Batch_ZuoYe_ID == id);
                model.zuoye_typeId = model.Batch_ZuoYeType_ID;
                model.bid = model.Batch_ID;
                model.eid = model.Enterprise_Info_ID;
                ClearLinqModel(model);
                return model;
            }
        }
        public View_ZuoYeBatchMaterial GetModelView(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                View_ZuoYeBatchMaterial model = dataContext.View_ZuoYeBatchMaterial.FirstOrDefault(m => m.Batch_ZuoYe_ID == id);
                return model;
            }
        }

        public bool IsHasData(long bid, int type, long? extId)
        {
            bool has = false;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data = dataContext.Batch_ZuoYe.Where(m => m.Batch_ID == bid && m.type == type && m.Status == 1 && (m.BatchExt_ID == null || m.BatchExt_ID == 0 || m.BatchExt_ID == extId));
                if (data.Count() > 0)
                    has = true;
            }
            return has;
        }

        public List<View_ZuoYeAndZuoYeType> GetList(long bId, long? extId, int type, int pageIndex, out bool IsHasMore)
        {
            List<View_ZuoYeAndZuoYeType> a = new List<View_ZuoYeAndZuoYeType>();
            int pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSizeWapData"].ToString());
            IsHasMore = true;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data1 = from m in dataContext.View_ZuoYeAndZuoYeType where m.Batch_ID == bId && m.Status == 1 && m.type == type && (m.BatchExt_ID == null || m.BatchExt_ID == 0) orderby m.AddDate ascending select m;
                if (extId != null && extId > 0)
                {
                    var data2 = from m in dataContext.View_ZuoYeAndZuoYeType where m.Batch_ID == bId && m.Status == 1 && m.type == type && m.BatchExt_ID == extId orderby m.AddDate ascending select m;
                    if (data2 != null && data2.Count() > 0)
                    {
                        List<View_ZuoYeAndZuoYeType> a1 = data1.OrderBy(m => m.AddDate).ToList();
                        a1.AddRange(data2.ToList());
                        IsHasMore = a1.Count() > (pageIndex * pageSize);
                        a = a1.Skip(pageSize * pageIndex).Take(pageSize).ToList();
                        return a;
                    }
                }
                IsHasMore = data1.Count() > (pageIndex * pageSize);
                //a = data1.OrderBy(m => m.AddDate).ToPagedList(pageIndex, pageSize).ToList();
                a = data1.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                return a;
            }
        }
    }
}
