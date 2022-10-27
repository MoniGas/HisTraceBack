using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class BatchDAL : DALBase
    {
        /// <summary>
        /// 获取批次列表By张
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="greenName">生产单元名称</param>
        /// <param name="beginDate">开始时间（查询用）</param>
        /// <param name="endDate">结束时间（查询用）</param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<View_Batch> GetList(long enterpriseId, string searchName, string materialName, string bName, string beginDate, string endDate, out long totalCount, int pageIndex)
        {
            List<View_Batch> result = new List<View_Batch>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_Batch.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status != (int)Common.EnumFile.Status.delete);

                    if (!string.IsNullOrEmpty(searchName))
                    {
                        data = data.Where(m => m.BatchName.Contains(searchName.Trim()) || m.MaterialFullName.Contains(searchName.Trim()) || m.BrandName.Contains(searchName.Trim()) || m.Greenhouseslist.Contains(searchName.Trim()) || m.nearType.Contains(searchName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        data = data.Where(m => m.MaterialFullName == materialName);
                    }
                    if (!string.IsNullOrEmpty(bName))
                    {
                        data = data.Where(m => m.BatchName == bName);
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.batchadddate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.batchadddate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Batch_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "BatchDAL.GetList():View_Batch视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 添加批次信息By张
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public RetResult Add(Batch batch, Greenhouses_Batch ghbatch, out Batch bt)
        {
            string Msg = "添加批次信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            bt = new Batch();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    var data = from d in dataContext.Batch where d.Enterprise_Info_ID == batch.Enterprise_Info_ID && d.BatchName == batch.BatchName && d.Status != (int)Common.EnumFile.Status.delete select d;
                    if (data.Count() > 0)
                    {
                        Msg = "已存在该批次信息！";
                    }
                    else
                    {
                        dataContext.Batch.InsertOnSubmit(batch);
                        dataContext.SubmitChanges();
                        //Greenhouses_Batch ghbatch = new Greenhouses_Batch();
                        ghbatch.adddate = batch.adddate;
                        ghbatch.adduser = batch.adduser;
                        ghbatch.Batch_ID = batch.Batch_ID;
                        ghbatch.lastdate = batch.lastdate;
                        ghbatch.lastuser = batch.lastuser;
                        dataContext.Greenhouses_Batch.InsertOnSubmit(ghbatch);
                        dataContext.SubmitChanges();
                        tran.Commit();
                        bt.Batch_ID = batch.Batch_ID;
                        Msg = "添加批次信息成功！";
                        error = CmdResultError.NONE;


                        LoginInfo loginInfo = SessCokie.Get;
                        loginInfo.NewUser = false;
                        Common.Argument.SessCokie.Set(loginInfo);
                    }
                }
                catch
                {
                    tran.Rollback();
                    Msg = "连接服务器失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        /// <summary>
        /// 修改批次信息
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public RetResult Edit(Batch batch, Greenhouses_Batch ghbatch)
        {
            string Msg = "修改批次信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from d in dataContext.Batch where d.Batch_ID != batch.Batch_ID && d.Enterprise_Info_ID == batch.Enterprise_Info_ID && d.BatchName == batch.BatchName && d.Status != (int)Common.EnumFile.Status.delete select d;
                    if (data.Count() > 0)
                    {
                        Msg = "已存在该批次！";
                    }
                    else
                    {
                        Batch oldBatch = dataContext.Batch.FirstOrDefault(d => d.Batch_ID == batch.Batch_ID);
                        Greenhouses_Batch oldghbatch = dataContext.Greenhouses_Batch.FirstOrDefault(d => d.Batch_ID == batch.Batch_ID);
                        if (oldBatch == null)
                        {
                            Msg = "没有找到要修改的批次信息请刷新列表！";
                        }
                        else
                        {
                            error = CmdResultError.NONE;
                            oldBatch.lastuser = batch.lastuser;
                            oldBatch.lastdate = batch.lastdate;
                            oldBatch.BatchCount = batch.BatchCount;
                            oldBatch.BatchDate = batch.BatchDate;
                            oldBatch.BatchName = batch.BatchName;
                            oldBatch.Material_ID = batch.Material_ID;
                            oldBatch.Status = batch.Status;
                            //oldBatch.validation = batch.validation;
                            oldghbatch.Greenhouses_ID = ghbatch.Greenhouses_ID;
                            oldghbatch.adddate = batch.adddate;
                            oldghbatch.adduser = batch.adduser;
                            oldghbatch.lastdate = batch.lastdate;
                            oldghbatch.lastuser = batch.lastuser;
                            dataContext.SubmitChanges();
                            Msg = "修改批次信息成功！";
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
        /// <summary>
        /// 删除批次信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RetResult Del(long id)
        {
            string Msg = "删除批次信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Batch batch = dataContext.Batch.FirstOrDefault(m => m.Batch_ID == id);
                    if (batch == null)
                    {
                        Msg = "没有找到要删除的批次信息请刷新列表！";
                    }
                    else if (batch.Status == (int)Common.EnumFile.Status.saled)
                    {
                        Msg = "该批次已经销售，目前无法删除！";
                    }
                    else
                    {
                        batch.Status = (int)Common.EnumFile.Status.delete;
                        //dataContext.Batch.DeleteOnSubmit(batch);
                        dataContext.SubmitChanges();
                        Msg = "删除批次信息成功！";
                        error = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Msg = "该批次已经销售，目前无法删除！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public View_Greenhouses_Batch GetModel(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_Greenhouses_Batch.FirstOrDefault(m => m.Batch_ID == id);
            }
        }
        public View_BatchMaterial GetModelByView(long id)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_BatchMaterial.FirstOrDefault(m => m.Batch_ID == id);
            }
        }

        public List<View_BatchMaterial> GetSelectList(long enterpriseId)
        {
            List<View_BatchMaterial> result = new List<View_BatchMaterial>();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.View_BatchMaterial.Where(m => m.Enterprise_Info_ID == enterpriseId &&
                        m.Status != (int)Common.EnumFile.Status.delete).OrderByDescending(m => m.Batch_ID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "BatchDAL.GetSelectList():Batch";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        //public View_WCF_BatchAndMaterialAndBrandAndDMT GetBatchModelWCF(long batch_ID)
        //{
        //    using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
        //    {
        //        return dataContext.View_WCF_BatchAndMaterialAndBrandAndDMT.FirstOrDefault(m => m.Batch_ID == batch_ID);
        //    }
        //}
    }
}
