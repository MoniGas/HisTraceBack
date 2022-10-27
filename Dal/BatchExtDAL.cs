using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class BatchExtDAL : DALBase
    {
        /// <summary>
        /// 获取子批次列表By张
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="batchextName"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<BatchExt> GetList(long batchId, string batchextName, out long totalCount, int pageIndex)
        {
            List<BatchExt> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.BatchExt.Where(m => m.Batch_ID == batchId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(batchextName))
                    {
                        data = data.Where(m => m.BatchExtName.Contains(batchextName));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.BatchExt_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "BatchExtDAL.GetList():BatchExt表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        public List<View_BatchAndBatchExt> GetListB(long enterpriseId, string batchextName, out long totalCount, int pageIndex)
        {
            List<View_BatchAndBatchExt> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_BatchAndBatchExt.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(batchextName))
                    {
                        data = data.Where(m => m.BatchExtName.Contains(batchextName));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.BatchExt_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "BatchExtDAL.GetList():View_BatchAndBatchExt视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        public List<View_BatchExt> GetListBE(long batchId, string batchextName)
        {
            List<View_BatchExt> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_BatchExt.Where(m => m.Batch_ID == batchId && m.Status != (int)Common.EnumFile.Status.delete);
                    if (!string.IsNullOrEmpty(batchextName))
                    {
                        data = data.Where(m => m.BatchExtName.Contains(batchextName));
                    }
                    result = data.OrderByDescending(m => m.BatchExt_ID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "BatchExtDAL.GetList():View_BatchExt视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 添加子批次
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(BatchExt model)
        {

            string Msg = "添加子批次信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from d in dataContext.BatchExt where d.BatchExtName == model.BatchExtName && d.Batch_ID == model.Batch_ID && d.Status != (int)Common.EnumFile.Status.delete select d;
                    if (data.Count() > 0)
                    {
                        Msg = "已存在该子批次信息！";
                    }
                    else
                    {
                        dataContext.BatchExt.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Msg = "添加子批次信息成功！";
                        error = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        /// <summary>
        /// 修改子批次
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Edit(BatchExt model)
        {
            string Msg = "修改子批次信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from d in dataContext.BatchExt where d.BatchExt_ID != model.BatchExt_ID && d.BatchExtName == model.BatchExtName && d.Batch_ID == model.Batch_ID && d.Status != (int)Common.EnumFile.Status.delete select d;
                    if (data.Count() > 0)
                    {
                        Msg = "已存在该子批次！";
                    }
                    else
                    {
                        BatchExt oldBatch = dataContext.BatchExt.FirstOrDefault(d => d.BatchExt_ID == model.BatchExt_ID);
                        if (oldBatch == null)
                        {
                            Msg = "没有找到要修改的子批次信息请刷新列表！";
                        }
                        else
                        {
                            error = CmdResultError.NONE;
                            oldBatch.lastuser = model.lastuser;
                            oldBatch.lastdate = model.lastdate;
                            oldBatch.BatchDate = model.BatchDate;
                            oldBatch.BatchExtName = model.BatchExtName;

                            dataContext.SubmitChanges();
                            Msg = "修改子批次信息成功！";
                        }
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public RetResult Del(long id)
        {
            string Msg = "删除批次信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    BatchExt batchext = dataContext.BatchExt.FirstOrDefault(m => m.BatchExt_ID == id);
                    if (batchext == null)
                    {
                        Msg = "没有找到要删除的批次信息请刷新列表！";
                    }
                    else if (batchext.Status == (int)Common.EnumFile.Status.saled)
                    {
                        Msg = "该子批次已经销售，目前无法删除！";
                    }
                    else
                    {
                        batchext.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Msg = "删除批次信息成功！";
                        error = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Msg = "该子批次已经销售，目前无法删除！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public BatchExt GetModel(long id)
        {
            BatchExt model = new BatchExt();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    model = dataContext.BatchExt.FirstOrDefault(m => m.BatchExt_ID == id);
                }
                catch { }
            }
            return model;
        }

        public List<BatchExt> GetSelectList(long bId)
        {
            List<BatchExt> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.BatchExt.Where(m => m.Batch_ID == bId && m.Status == (int)Common.EnumFile.Status.used);
                    result = data.ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "BatchExtDAL.GetSelectList():BatchExt表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
    }
}
