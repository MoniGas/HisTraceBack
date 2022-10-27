using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class Batch_XunJianDAL : DALBase
    {
        /// <summary>
        /// 获取巡检信息列表
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="batchID"></param>
        /// <param name="totlaCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<View_XunJianBatchMaterial> GetList(long enterpriseId, long batchID, long batchextid, out long totlaCount, int pageIndex)
        {
            List<View_XunJianBatchMaterial> result = null;
            totlaCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_XunJianBatchMaterial.Where(m => m.Enterprise_Info_ID == enterpriseId && m.XunJianStatus == (int)Common.EnumFile.Status.used && m.Batch_ID == batchID && m.BatchExt_ID == batchextid);
                    if (batchID > 0)
                    {
                        data = data.Where(m => m.Batch_ID == batchID);
                    }
                    if (batchextid > 0)
                    {
                        data = data.Where(m => m.BatchExt_ID == batchextid);
                    }
                    totlaCount = data.Count();
                    result = data.OrderByDescending(m => m.Batch_XunJian_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "Batch_XunJianDAL.GetList():View_XunJianBatchMaterial视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取企业巡检信息
        /// </summary>
        /// <param name="eID">企业ID</param>
        /// <param name="status">审批状态</param>
        /// <returns>数据列表</returns>
        public List<Batch_XunJian> GetList(long eID, int status, long batch_id)
        {
            List<Batch_XunJian> list = new List<Batch_XunJian>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data = from m in dataContext.Batch_XunJian select m;
                if (eID > 0)
                    data = data.Where(m => m.Enterprise_Info_ID == eID);
                if (status > -3)
                    data = data.Where(m => m.Status == status);
                if (batch_id > 0)
                    data = data.Where(m => m.Batch_ID == batch_id);
                if (data.Count() > 0)
                {
                    list = data.ToList();
                }
            }
            return list;
        }
        
        /// <summary>
        /// 添加巡检信息 By张
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(Batch_XunJian model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.Batch_XunJian.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.Batch_XunJian_ID;
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
        /// 修改巡检信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Edit(Batch_XunJian model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                Batch_XunJian newModel = dataContext.Batch_XunJian.FirstOrDefault(m => m.Batch_XunJian_ID == model.Batch_XunJian_ID);
                if (newModel != null)
                {
                    newModel.lastdate = model.lastdate;
                    newModel.lastuser = model.lastuser;
                    newModel.Content = model.Content;
                    newModel.AddDate = model.AddDate;
                    newModel.Files = model.Files;
                    newModel.UserName = model.UserName;
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
        /// 删除巡检信息
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
                    Batch_XunJian model = dataContext.Batch_XunJian.FirstOrDefault(m => m.Batch_XunJian_ID == id);
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
                    //dataContext.Batch_XunJian.DeleteOnSubmit(model);
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
        /// 获取巡检实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Batch_XunJian GetModel(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                Batch_XunJian model = dataContext.Batch_XunJian.FirstOrDefault(m => m.Batch_XunJian_ID == id);
                ClearLinqModel(model);
                return model;
            }
        }
        public View_XunJianBatchMaterial GetModelView(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                View_XunJianBatchMaterial model = dataContext.View_XunJianBatchMaterial.FirstOrDefault(m => m.Batch_XunJian_ID == id);
                return model;
            }
        }

        public List<View_XunJianBatchMaterial> GetListByBatchID(long bId, long? extId, int pageIndex, out bool IsHasMore)
        {
            int pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSizeWapData"].ToString());
            IsHasMore = true;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data1 = from m in dataContext.View_XunJianBatchMaterial where m.Batch_ID == bId && m.XunJianStatus == 1 && (m.BatchExt_ID == null || m.BatchExt_ID == 0) orderby m.XJAddDate ascending select m;
                if (extId != null && extId > 0)
                {
                    var data2 = from m in dataContext.View_XunJianBatchMaterial where m.Batch_ID == bId && m.XunJianStatus == 1 && m.BatchExt_ID == extId orderby m.XJAddDate ascending select m;
                    if (data2 != null && data2.Count() > 0)
                    {
                        List<View_XunJianBatchMaterial> a1 = data1.OrderBy(m => m.XJAddDate).ToList();
                        a1.AddRange(data2.ToList());
                        IsHasMore = a1.Count() > (pageIndex * pageSize);
                        a1 = a1.Skip(pageSize * pageIndex).Take(pageSize).ToList();
                        return a1;
                    }
                }
                IsHasMore = data1.Count() > (pageIndex * pageSize);
                //List<View_XunJianBatchMaterial> a = data1.OrderBy(m => m.XJAddDate).ToPagedList(pageIndex, pageSize).ToList();
                List<View_XunJianBatchMaterial> a = data1.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                return a;
            }
        }
    }
}
