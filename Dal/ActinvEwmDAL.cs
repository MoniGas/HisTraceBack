//激活二维码
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Argument;
using Common.Log;
using System.Data.Common;
using Common;
using LinqModel.InterfaceModels;

namespace Dal
{
    public class ActinvEwmDAL : DALBase
    {
        /// <summary>
        /// 新增接收码包记录
        /// </summary>
        /// <param name="recPack"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public bool AddRecPack(ActiveEwmRecord recPack, string loginName, string loginPwd, out string Msg, out long recId)
        {
            Msg = "保存失败！";
            recId = 0;
            try
            {
                using (DataClassesDataContext db = GetDataContext())
                {
                    var enterprise = db.Enterprise_User.Where(p => p.LoginName == loginName && p.LoginPassWord == loginPwd).FirstOrDefault();
                    if (enterprise == null)
                    {
                        Msg = "用户名或者密码失败！";
                        return false;
                    }
                    recPack.UpUserID = enterprise.Enterprise_User_ID;
                    recPack.EnterpriseId = enterprise.Enterprise_Info_ID;
                    db.ActiveEwmRecord.InsertOnSubmit(recPack);
                    db.SubmitChanges();
                    recId = recPack.RecID;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// 激活上传文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult AddActiveRecPack(ActiveEwmRecord model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                MaterialExportExcelRecord newModel = new MaterialExportExcelRecord();
                try
                {
                    dataContext.ActiveEwmRecord.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "上传成功");
                }
                catch
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "上传失败");
                }
            }
            return Ret;
        }

        public List<ActiveEwmRecord> GetActiveEwmList(long enterpriseId, int type, string beginDate, string endDate, out long totalCount, int pageIndex)
        {
            List<ActiveEwmRecord> result = new List<ActiveEwmRecord>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.ActiveEwmRecord.Where(m => m.EnterpriseId == enterpriseId);
                    if (type > 0)
                    {
                        data = data.Where(m => m.OperationType == type);
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.UploadDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.UploadDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RecID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "ActinvEwmDAL.GetActiveEwmList():ActiveEwmRecord表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取批次列表
        /// </summary>
        /// <param name="enterpriseID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<RequestCodeSetting> GetBatchList(long enterpriseID, out long totalCount)
        {
            List<RequestCodeSetting> list = new List<RequestCodeSetting>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.RequestCodeSetting
                               where m.EnterpriseId == enterpriseID && m.BatchName != null
                               select m;
                    totalCount = data.Count();
                    data = data.OrderByDescending(m => m.ID);
                    list = data.ToList();
                    ClearLinqModel(list);
                }
                catch (Exception e)
                {
                    string errData = "ActinvEwmDAL.GetBatchList():RequestCodeSetting表";
                    WriteLog.WriteErrorLog(errData + ":" + e.Message);
                }
            }
            return list;
        }

        /// <summary>
        /// 输入批次激活
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="batchNameID">批次对应的requseCode表中的ID</param>
        /// <returns></returns>
        public RetResult ActiveEWM(ActiveEwmRecord model, long batchNameID)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                string strSql = "";
                SalesInformation sModel = new SalesInformation();
                try
                {
                    if (batchNameID > 0)
                    {
                        RequestCodeSetting remodel = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == batchNameID && m.EnterpriseId == model.EnterpriseId);
                        if (remodel != null)
                        {
                            model.BatchName = remodel.BatchName;
                            model.MateialCount = remodel.Count;
                            ActiveEwmRecord tempAc = dataContext.ActiveEwmRecord.FirstOrDefault(m => m.EnterpriseId == model.EnterpriseId && m.BatchName == model.BatchName);
                            if (tempAc != null)
                            {
                                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "该批次已被激活！");
                                Ret.Msg = "该批次已被激活！";
                                return Ret;
                            }
                        }
                        if (model.DealerID > 0)
                        {
                            model.Detail = "总共激活" + remodel.Count + "个码，全部销售给" + model.DealerName;
                        }
                        else
                        {
                            model.Detail = "总共激活" + remodel.Count + "个码！";
                        }
                    }
                    dataContext.ActiveEwmRecord.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    //执行销售或者激活操作
                    RequestCodeSetting reTemomodel = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == batchNameID && m.EnterpriseId == model.EnterpriseId);
                    RequestCode reCode = dataContext.RequestCode.FirstOrDefault(m => m.Enterprise_Info_ID == model.EnterpriseId && m.RequestCode_ID == reTemomodel.RequestID);
                    if (reCode != null)
                    {
                        string beginCode = "";
                        string endCode = "";
                        #region 激活或者销售单品产品码
                        //单品产品码IDCode码
                        if (reCode.Type == (int)Common.EnumFile.GenCodeType.single && reCode.CodeOfType == (int)Common.EnumFile.CodeOfType.IDCode)
                        {
                            Enterprise_Info eInfo = new Enterprise_Info();
                            eInfo = new Dal.EnterpriseInfoDAL().GetModel(model.EnterpriseId.Value);
                            sModel.ProductionTime = model.ProductionDate;
                            sModel.MaterialShelfLife = "";
                            sModel.SalesDate = DateTime.Now;
                            if (reCode.Type == (int)Common.EnumFile.GenCodeType.single)
                            {
                                sModel.Type = (int)Common.EnumFile.CodeType.single;
                                beginCode = reCode.FixedCode + "." + reTemomodel.beginCode.ToString().PadLeft(6, '0') + "." + (int)Common.EnumFile.CodeType.single;
                                endCode = reCode.FixedCode + "." + reTemomodel.endCode.ToString().PadLeft(6, '0') + "." + (int)Common.EnumFile.CodeType.single;
                            }
                            Enterprise_FWCode_00 codeBegin = new RequestCodeDAL().GetEWM(beginCode);
                            Enterprise_FWCode_00 codeEnd = new RequestCodeDAL().GetEWM(endCode);
                            string materialFullName = dataContext.Material.FirstOrDefault(w => w.Material_ID == reCode.Material_ID).MaterialName;
                            sModel.MaterialFullName = materialFullName;
                            sModel.DealerName = model.DealerName;
                            //销售数量
                            long count = 0;
                            Route_DataBase tableBegin = dataContext.Route_DataBase.Where(p => p.Route_DataBase_ID == reCode.Route_DataBase_ID).FirstOrDefault();
                            if (tableBegin == null)
                            {
                                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "获取路由表失败");
                            }
                            using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                            {
                                if (model.DealerID > 0)
                                {
                                    dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                                    dataContextEWM.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    //更新码的销售数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and Enterprise_FWCode_ID between "
                                        + codeBegin.Enterprise_FWCode_ID + " and " + codeEnd.Enterprise_FWCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表销售数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                              + " and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID +
                              " and " + codeEnd.Enterprise_FWCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += Convert.ToInt64(item.saleCount);
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                            {
                                                rc.saleCount = rc.saleCount + model.MateialCount;
                                                rc.Status = (int)Common.EnumFile.UsingStateCode.HasBeenUsed;
                                                dataContext.SubmitChanges();
                                            }
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                else
                                {
                                    //更新码的激活数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.Activated
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=0"
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and Enterprise_FWCode_ID between "
                                        + codeBegin.Enterprise_FWCode_ID + " and " + codeEnd.Enterprise_FWCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表激活数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.Activated
                              + " and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID +
                              " and " + codeEnd.Enterprise_FWCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += 0;
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.Activated + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                                rc.saleCount = 0;
                                            dataContext.SubmitChanges();
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    sModel.SellCount = count;
                                    sModel.EwmTableIdArray = (tableBegin.Route_DataBase_ID - 1).ToString();
                                }
                                dataContextEWM.SubmitChanges();
                            }
                        }
                        //单品产品码简码
                        if (reCode.Type == (int)Common.EnumFile.GenCodeType.single && reCode.CodeOfType == (int)Common.EnumFile.CodeOfType.SCode)
                        {
                            Enterprise_Info eInfo = new Enterprise_Info();
                            eInfo = new Dal.EnterpriseInfoDAL().GetModel(model.EnterpriseId.Value);
                            sModel.ProductionTime = model.ProductionDate;
                            sModel.MaterialShelfLife = "";
                            sModel.SalesDate = DateTime.Now;
                            string materialFullName = dataContext.Material.FirstOrDefault(w => w.Material_ID == reCode.Material_ID).MaterialName;
                            sModel.MaterialFullName = materialFullName;
                            sModel.DealerName = model.DealerName;
                            //销售数量
                            long count = 0;
                            Route_DataBase tableBegin = dataContext.Route_DataBase.Where(p => p.Route_DataBase_ID == reCode.Route_DataBase_ID).FirstOrDefault();
                            if (tableBegin == null)
                            {
                                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "获取路由表失败");
                            }
                            using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                            {
                                if (model.DealerID > 0)
                                {
                                    dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                                    dataContextEWM.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    //更新码的销售数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and RequestCode_ID=" + reCode.RequestCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表销售数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                              + " and RequestCode_ID=" + reCode.RequestCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += Convert.ToInt64(item.saleCount);
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                            {
                                                rc.saleCount = rc.saleCount + model.MateialCount;
                                                rc.Status = (int)Common.EnumFile.UsingStateCode.HasBeenUsed;
                                                dataContext.SubmitChanges();
                                            }
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                else
                                {
                                    //更新码的激活数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.Activated
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=0"
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and  RequestCode_ID=" + reCode.RequestCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表激活数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.Activated
                              + " and  RequestCode_ID=" + reCode.RequestCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += 0;
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.Activated + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                                rc.saleCount = 0;
                                            dataContext.SubmitChanges();
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    sModel.SellCount = count;
                                    sModel.EwmTableIdArray = (tableBegin.Route_DataBase_ID - 1).ToString();
                                }
                                dataContextEWM.SubmitChanges();
                            }
                        }
                        //操作农药码
                        if (reCode.Type == (int)Common.EnumFile.GenCodeType.pesticides)
                        {
                            Enterprise_Info eInfo = new Enterprise_Info();
                            eInfo = new Dal.EnterpriseInfoDAL().GetModel(model.EnterpriseId.Value);
                            sModel.ProductionTime = model.ProductionDate;
                            sModel.MaterialShelfLife = "";
                            sModel.SalesDate = DateTime.Now;
                            string materialFullName = dataContext.Material.FirstOrDefault(w => w.Material_ID == reCode.Material_ID).MaterialName;
                            sModel.MaterialFullName = materialFullName;
                            sModel.DealerName = model.DealerName;
                            //销售数量
                            long count = 0;
                            Route_DataBase tableBegin = dataContext.Route_DataBase.Where(p => p.Route_DataBase_ID == reCode.Route_DataBase_ID).FirstOrDefault();
                            if (tableBegin == null)
                            {
                                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "获取路由表失败");
                            }
                            using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                            {
                                if (model.DealerID > 0)
                                {
                                    dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                                    dataContextEWM.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    //更新码的销售数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and RequestCode_ID=" + reCode.RequestCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表销售数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                              + " and RequestCode_ID=" + reCode.RequestCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += Convert.ToInt64(item.saleCount);
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                            {
                                                rc.saleCount = rc.saleCount + model.MateialCount;
                                                rc.Status = (int)Common.EnumFile.UsingStateCode.HasBeenUsed;
                                                dataContext.SubmitChanges();
                                            }
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                else
                                {
                                    //更新码的激活数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.Activated
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=0"
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and  RequestCode_ID=" + reCode.RequestCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表激活数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.Activated
                              + " and  RequestCode_ID=" + reCode.RequestCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += 0;
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.Activated + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                                rc.saleCount = 0;
                                            dataContext.SubmitChanges();
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    sModel.SellCount = count;
                                    sModel.EwmTableIdArray = (tableBegin.Route_DataBase_ID - 1).ToString();
                                }
                                dataContextEWM.SubmitChanges();
                            }
                        }
                        #endregion
                        #region 套标的码
                        //套标码IDcode码
                        if (reCode.Type == (int)Common.EnumFile.GenCodeType.trap && reCode.CodeOfType == (int)Common.EnumFile.CodeOfType.IDCode)
                        {
                            #region 获取销售数据
                            sModel.ProductionTime = model.ProductionDate;
                            sModel.SalesDate = DateTime.Now;
                            sModel.Type = (int)Common.EnumFile.CodeType.bGroup;
                            beginCode = reCode.FixedCode + "." + reTemomodel.beginCode.ToString().PadLeft(6, '0') + "." + (int)Common.EnumFile.CodeType.bGroup;
                            endCode = reCode.FixedCode + "." + reTemomodel.endCode.ToString().PadLeft(6, '0') + "." + (int)Common.EnumFile.CodeType.bSingle;
                            Enterprise_FWCode_00 codeBegin = new RequestCodeDAL().GetEWM(beginCode);
                            Enterprise_FWCode_00 codeEnd = new RequestCodeDAL().GetEWM(endCode);
                            #endregion
                            string materialFullName = dataContext.Material.FirstOrDefault(w => w.Material_ID == reCode.Material_ID).MaterialName;
                            sModel.MaterialFullName = materialFullName;
                            sModel.DealerName = model.DealerName;
                            sModel.EnterpriseId = model.EnterpriseId;
                            Enterprise_Info eInfo = dataContext.Enterprise_Info.FirstOrDefault(w => w.Enterprise_Info_ID == model.EnterpriseId);
                            sModel.Enterprise_InfoName = eInfo.EnterpriseName;
                            var tableBegin = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == reCode.Route_DataBase_ID);
                            long count = 0;
                            string TableIdArray = string.Empty;
                            // 创建二维码表的数据库连接
                            using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                            {
                                if (model.DealerID > 0)
                                {
                                    dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                                    dataContextEWM.SubmitChanges();
                                }
                                TableIdArray += (tableBegin.Route_DataBase_ID - 1).ToString();
                                if (model.DealerID > 0)
                                {
                                    // 激活箱标码SQL
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status=" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + ""
                                        + ",ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",Material_ID=" + reCode.Material_ID
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',Dealer_ID=" + model.DealerID.ToString()
                                        + ",SalesInformation_ID=" + sModel.SalesInformation_ID
                                        + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID
                                        + " and " + codeEnd.Enterprise_FWCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 获取销售数量
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                         + " where Enterprise_Info_ID=" + model.EnterpriseId
                                         + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                                         + " and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID + " and " + codeEnd.Enterprise_FWCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += Convert.ToInt64(item.saleCount);
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        //更新RequestCode表中的销售数量字段
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID in (" + strRequestCodeID.Substring(1) + ") and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                                            + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                            {
                                                rc.saleCount = rc.saleCount + model.MateialCount;
                                                rc.Status = (int)Common.EnumFile.UsingStateCode.HasBeenUsed;
                                                dataContext.SubmitChanges();
                                            }
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                else
                                {
                                    // 激活箱标码SQL
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status=" + (int)Common.EnumFile.UsingStateCode.Activated + ""
                                        + ",ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",Material_ID=" + reCode.Material_ID
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',Dealer_ID=" + model.DealerID.ToString()
                                        + ",SalesInformation_ID=" + sModel.SalesInformation_ID
                                        + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID
                                        + " and " + codeEnd.Enterprise_FWCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 获取销售数量
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                         + " where Enterprise_Info_ID=" + model.EnterpriseId
                                         + " and Status = " + (int)Common.EnumFile.UsingStateCode.Activated
                                         + " and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID + " and " + codeEnd.Enterprise_FWCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += 0;
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        //更新RequestCode表中的销售数量字段
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID in (" + strRequestCodeID.Substring(1) + ") and Status = " + (int)Common.EnumFile.UsingStateCode.Activated
                                            + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                                rc.saleCount = 0;
                                            dataContext.SubmitChanges();
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    //sModel.SellCount = (count / (specification + 1));
                                    sModel.SellCount = count;
                                    sModel.EwmTableIdArray = TableIdArray;

                                }
                                dataContextEWM.SubmitChanges();
                            }
                        }
                        //套标码简码
                        if (reCode.Type == (int)Common.EnumFile.GenCodeType.trap && reCode.CodeOfType == (int)Common.EnumFile.CodeOfType.SCode)
                        {
                            #region 获取销售数据
                            sModel.ProductionTime = model.ProductionDate;
                            sModel.SalesDate = DateTime.Now;
                            sModel.Type = (int)Common.EnumFile.CodeType.bGroup;
                            #endregion
                            string materialFullName = dataContext.Material.FirstOrDefault(w => w.Material_ID == reCode.Material_ID).MaterialName;
                            sModel.MaterialFullName = materialFullName;
                            sModel.DealerName = model.DealerName;
                            sModel.EnterpriseId = model.EnterpriseId;
                            Enterprise_Info eInfo = dataContext.Enterprise_Info.FirstOrDefault(w => w.Enterprise_Info_ID == model.EnterpriseId);
                            sModel.Enterprise_InfoName = eInfo.EnterpriseName;
                            var tableBegin = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == reCode.Route_DataBase_ID);
                            if (tableBegin == null)
                            {
                                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "获取路由表失败");
                            }
                            long count = 0;
                            string TableIdArray = string.Empty;
                            // 创建二维码表的数据库连接
                            using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                            {
                                if (model.DealerID > 0)
                                {
                                    dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                                    dataContextEWM.SubmitChanges();
                                }
                                TableIdArray += (tableBegin.Route_DataBase_ID - 1).ToString();
                                if (model.DealerID > 0)
                                {
                                    //更新码的销售数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and RequestCode_ID=" + reCode.RequestCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表销售数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                              + " and RequestCode_ID=" + reCode.RequestCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += Convert.ToInt64(item.saleCount);
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                            {
                                                rc.saleCount = rc.saleCount + model.MateialCount;
                                                rc.Status = (int)Common.EnumFile.UsingStateCode.HasBeenUsed;
                                                dataContext.SubmitChanges();
                                            }
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                else
                                {
                                    // 激活箱标码SQL
                                    //更新码的激活数据
                                    strSql = "update " + tableBegin.TableName + " set SalesTime='" + model.ProductionDate
                                        + "',Status='" + (int)Common.EnumFile.UsingStateCode.Activated
                                        + "',ScanCount=0"
                                        + ",FWCount=0"
                                        + ",ValidateTime=NULL"
                                        + ",UseTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        + "',SalesInformation_ID=0"
                                        + ",Dealer_ID=" + model.DealerID.ToString() + " where Enterprise_Info_ID=" + model.EnterpriseId
                                        + " and  RequestCode_ID=" + reCode.RequestCode_ID;
                                    dataContextEWM.ExecuteCommand(strSql);
                                    #region 更新申请码表销售数据
                                    //更新申请码表激活数据
                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                              + " where Enterprise_Info_ID=" + model.EnterpriseId
                              + " and Status = " + (int)Common.EnumFile.UsingStateCode.Activated
                              + " and  RequestCode_ID=" + reCode.RequestCode_ID + " group by RequestCode_ID";
                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                    if (listRC != null && listRC.Count > 0)
                                    {
                                        string strRequestCodeID = string.Empty;
                                        foreach (SaleRequestCodeCount item in listRC)
                                        {
                                            count += 0;
                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                        }
                                        count -= 0;
                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                            + " where Enterprise_Info_ID=" + model.EnterpriseId
                                            + " and RequestCode_ID=" + reCode.RequestCode_ID
                                        + "and Status = " + (int)Common.EnumFile.UsingStateCode.Activated + " group by RequestCode_ID";
                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                        foreach (SaleRequestCodeCount item in listRC2)
                                        {
                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                            if (rc != null)
                                                rc.saleCount = 0;
                                            dataContext.SubmitChanges();
                                        }
                                    }
                                    #endregion
                                    dataContext.SubmitChanges();
                                }
                                if (model.DealerID > 0)
                                {
                                    //sModel.SellCount = (count / (specification + 1));
                                    sModel.SellCount = count;
                                    sModel.EwmTableIdArray = TableIdArray;

                                }
                                dataContextEWM.SubmitChanges();
                            }
                        }
                        #endregion
                        tran.Commit();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
                }
                return Ret;
            }
        }

        public RetResults UploadPIPrivate(PrivatePIRequest model)
        {
            RetResults ret = new RetResults();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var ewmmo = new LinqModel.ActiveEwmRecord();
                    ewmmo.PackName = model.packName;
                    ewmmo.UploadDate = model.uploadDate;
                    ewmmo.UpUserID = model.upUserID;
                    ewmmo.EnterpriseId = model.enterpriseId;
                    ewmmo.RecPath = model.recPath;
                    ewmmo.Status = 0;
                    ewmmo.AddDate = DateTime.Now;
                    ewmmo.StrAddTime = model.strAddTime;
                    ewmmo.AddUserName = model.addUserName;
                    ewmmo.OperationType = model.operationType;
                    ewmmo.MateialCount = model.mateialCount;
                    ewmmo.Detail = model.detail;
                    ewmmo.URL = model.uRL;
                    ewmmo.Material_ID = model.materialID;
                    ewmmo.MaterialUDI = model.materialUDI;
                    ewmmo.IsUpload = model.isUpload;
                    dataContext.ActiveEwmRecord.InsertOnSubmit(ewmmo);
                    dataContext.SubmitChanges();
                    ret.code = 1;
                    ret.Msg = "添加成功";
                }
                catch (Exception)
                {
                    ret.code = -1;
                    ret.Msg = "程序出错，添加失败";
                    throw;
                }
            }
            return ret;
        }

        #region 刘晓杰于2019年11月4日从CFBack项目移入此

        /// <summary>
        /// 获取上传文件记录 默认取前20条
        /// </summary>
        /// <returns></returns>
        public List<ActiveEwmRecord> GetActiveEwmList()
        {
            List<ActiveEwmRecord> result = new List<ActiveEwmRecord>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.ActiveEwmRecord; ;
                    result = data.OrderByDescending(m => m.RecID).Take(20).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "ActinvEwmDAL.GetActiveEwmList():ActiveEwmRecord表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 激活上传文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResults UpdateActiveRecPack(ActiveEwmRecord model)
        {
            RetResults res = new RetResults();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.ActiveEwmRecord.InsertOnSubmit(model);
                    dataContext.SubmitChanges();

                    res.code = 0;
                    res.Msg = "上传成功";
                }
                catch
                {
                    res.code = -1;
                    res.Msg = "上传失败";
                }
            }
            return res;
        }

        /// <summary>
        /// 查询上传是否文件名是否重复
        /// </summary>
        /// <param name="packName">上传文件名</param>
        /// <returns></returns>
        public ActiveEwmRecord IsActiveRecPack(string packName)
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return db.ActiveEwmRecord.FirstOrDefault(p => p.PackName == packName & p.Status != (int)EnumFile.RecEwm.激活失败);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 查询上传是否文件名是否重复
        /// </summary>
        /// <param name="ProduceBathNo">生产批次编号</param>
        /// <returns></returns>
        public ActiveEwmRecord getModelByProduceBathNo(string ProduceBathNo, long eId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.ActiveEwmRecord.FirstOrDefault(p => p.ProducBatch == ProduceBathNo
                    && p.EnterpriseId == eId && p.Status == (int)Common.EnumFile.RecEwm.已接收);
            }
        }
        #endregion
    }
}
