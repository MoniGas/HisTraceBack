using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class Batch_jianYanJianYiDAL : DALBase
    {
        /// <summary>
        /// 获取检测报告列表
        /// </summary>
        /// <param name="batchID"></param>
        /// <param name="totlaCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<View_JianYanJianYiBatchMaterial> GetList(long enterpriseId, long batchID, long batchextid, out long totalCount, int pageIndex)
        {
            List<View_JianYanJianYiBatchMaterial> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_JianYanJianYiBatchMaterial.Where(m => m.Enterprise_Info_ID == enterpriseId && m.XunJianStatus == (int)Common.EnumFile.Status.used && m.Batch_ID == batchID && m.BatchExt_ID == batchextid);
                    //var data = from m in dataContext.View_JianYanJianYiBatchMaterial where m.Enterprise_Info_ID == enterpriseId && m.XunJianStatus == (int)Common.EnumFile.Status.used select m;
                    if (batchID > 0)
                    {
                        data = data.Where(m => m.Batch_ID == batchID);
                    }
                    if (batchextid > 0)
                    {
                        data = data.Where(m => m.BatchExt_ID == batchextid);
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Batch_JianYanJianYi_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "Batch_jianYanJianYiDAL.GetList():View_JianYanJianYiBatchMaterial视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 添加检测报告
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(Batch_JianYanJianYi model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.Batch_JianYanJianYi.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.Batch_JianYanJianYi_ID;
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
        /// 修改检测报告信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Edit(Batch_JianYanJianYi model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                Batch_JianYanJianYi newModel = dataContext.Batch_JianYanJianYi.FirstOrDefault(m => m.Batch_JianYanJianYi_ID == model.Batch_JianYanJianYi_ID);
                if (newModel != null)
                {
                    newModel.AddDate = model.AddDate;
                    newModel.lastdate = model.lastdate;
                    newModel.lastuser = model.lastuser;
                    newModel.Content = model.Content;
                    newModel.Files = model.Files;
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
        /// 删除检测报告信息
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
                    Batch_JianYanJianYi model = dataContext.Batch_JianYanJianYi.FirstOrDefault(m => m.Batch_JianYanJianYi_ID == id);
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
                    //dataContext.Batch_JianYanJianYi.DeleteOnSubmit(model);
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
        /// <param name="id">巡检ID</param>
        /// <returns>巡检实体</returns>
        public Batch_JianYanJianYi GetModel(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                Batch_JianYanJianYi model = dataContext.Batch_JianYanJianYi.FirstOrDefault(m => m.Batch_JianYanJianYi_ID == id);
                ClearLinqModel(model);
                return model;
            }
        }
        public View_JianYanJianYiBatchMaterial GetModelView(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                View_JianYanJianYiBatchMaterial model = dataContext.View_JianYanJianYiBatchMaterial.FirstOrDefault(m => m.Batch_JianYanJianYi_ID == id);
                return model;
            }
        }

        public List<View_JianYanJianYiBatchMaterial> GetListByBatchID(long bId, long? extId, int pageIndex, out bool IsHasMore)
        {
            int pageSize = int.MaxValue;// int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSizeWapData"].ToString());//
            IsHasMore = true;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data1 = from m in dataContext.View_JianYanJianYiBatchMaterial where m.Batch_ID == bId && m.XunJianStatus == 1 && (m.BatchExt_ID == null || m.BatchExt_ID == 0) orderby m.JCAddDate ascending select m;
                if (extId != null && extId > 0)
                {
                    var data2 = from m in dataContext.View_JianYanJianYiBatchMaterial where m.Batch_ID == bId && m.XunJianStatus == 1 && m.BatchExt_ID == extId orderby m.JCAddDate ascending select m;
                    if (data2 != null && data2.Count() > 0)
                    {
                        List<View_JianYanJianYiBatchMaterial> a1 = data1.ToList();
                        a1.AddRange(data2.ToList());
                        return a1;
                    }
                }
                IsHasMore = data1.Count() >= (pageIndex * pageSize);
                //List<View_JianYanJianYiBatchMaterial> a = data1.ToPagedList(pageIndex, pageSize).ToList();

                List<View_JianYanJianYiBatchMaterial> a = data1.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                return a;
            }
        }
    }
}
