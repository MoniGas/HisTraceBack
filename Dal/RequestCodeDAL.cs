/********************************************************************************
** 作者： 李子巍
** 创始时间：2015-06-15
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：赵慧敏
** 修改时间：2017-02-10
** 描述：
**  主要用于码信息管理数据层 
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Argument;
using LinqModel;
using Common.Log;
using Common.Tools;
using InterfaceWeb;
using Common;

namespace Dal
{
    #region
    public class SellCodeDAL : DALBase
    {
        /// <summary>
        /// 获取已销售列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<RequestCode> GetSaleList1(long eId, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<RequestCode> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data = from m in dataContext.RequestCode where m.saleCount > 0 && (m.Status == (int)Common.EnumFile.RequestCodeStatus.GenerationIsComplete || m.Status == (int)Common.EnumFile.RequestCodeStatus.PackagingFailure || m.Status == (int)Common.EnumFile.RequestCodeStatus.PackToSuccess) select m;
                if (eId > 0)
                    data = data.Where(m => m.Enterprise_Info_ID == eId);
                data = data.OrderByDescending(m => m.RequestCode_ID);
                totalCount = data.Count();
                result = data.Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                ClearLinqModel(result);
            }
            return result;
        }

        public List<LinqModel.Enterprise_FWCode_00> GetCode(long RequestCodeId)
        {
            try
            {
                string DataSource = string.Empty;
                string DataBaseName = string.Empty;
                string TableName = string.Empty;
                string UID = string.Empty;
                string PWD = string.Empty;

                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var RequestModel = (from data in dataContext.RequestCode
                                        where data.RequestCode_ID == RequestCodeId
                                        select data).FirstOrDefault();
                    if (RequestModel == null || RequestModel.Route_DataBase_ID == null)
                    {
                        return null;
                    }

                    var DataBaseModel = (from data in dataContext.Route_DataBase
                                         where data.Route_DataBase_ID == RequestModel.Route_DataBase_ID
                                         select data).FirstOrDefault();

                    if (DataBaseModel == null)
                    {
                        return null;
                    }

                    DataSource = DataBaseModel.DataSource;
                    DataBaseName = DataBaseModel.DataBaseName;
                    TableName = DataBaseModel.TableName;
                    UID = DataBaseModel.UID;
                    PWD = DataBaseModel.PWD;
                }

                using (LinqModel.DataClassesDataContext dataContext = GetDataContext(DataSource, DataBaseName, UID, PWD))
                {
                    string sql = "select * from " + DataBaseName + ".dbo." + TableName + " where RequestCode_ID=" + RequestCodeId + " and Status =1040000008 order by Enterprise_FWCode_ID";

                    List<LinqModel.Enterprise_FWCode_00> DataModel = dataContext.ExecuteQuery<LinqModel.Enterprise_FWCode_00>(sql).ToList();

                    // 验证查询结果不为null
                    if (DataModel != null)
                    {
                        // 验证查询结果为套标码
                        if (DataModel[0].Type == 1 || DataModel[0].Type == 2)
                        {
                            DataModel = DataModel.Where(w => w.Type == 1).ToList();
                        }


                        DataModel = DataModel.OrderBy(o => o.Enterprise_FWCode_ID).ToList();


                    }

                    return DataModel;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    #endregion

    public class RequestCodeDAL : DALBase
    {
        /// <summary>
        /// 获取申请码列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">上级部门</param>
        /// <param name="ewm">产品二维码</param>
        /// <param name="name">产品名称</param>
        /// <param name="beginDate">申请时间开始</param>
        /// <param name="endDate">申请时间结束</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>申请码列表</returns>
        public List<View_RequestCodeAndEnterprise_Info> GetList(long? eId, long? upId, string mId, string mName, string beginDate, string endDate, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<View_RequestCodeAndEnterprise_Info> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_RequestCodeAndEnterprise_Info
                               where m.Type == (int)Common.EnumFile.GenCodeType.single ||
                                   m.Type == (int)Common.EnumFile.GenCodeType.localCreate
                               select m;
                    if (eId != null && eId > 0)
                    {
                        data = data.Where(m => m.Enterprise_Info_ID == eId);
                    }
                    if (!string.IsNullOrEmpty(mId))
                    {
                        data = data.Where(m => m.Material_ID == Convert.ToInt64(mId));
                    }
                    if (!string.IsNullOrEmpty(mName))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(mName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.RequestDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.RequestDate <= Convert.ToDateTime(endDate));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RequestCode_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获取申请码(包材码)列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">上级部门</param>
        /// <param name="mId">产品id</param>
        /// <param name="mName">产品名称</param>
        /// <param name="beginDate">申请时间开始</param>
        /// <param name="endDate">请时间结束</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>申请码列表</returns>
        public List<View_RequestCodeAndEnterprise_Info> GetBoxList(long? eId, string mId, string mName, string beginDate, string endDate, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<View_RequestCodeAndEnterprise_Info> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_RequestCodeAndEnterprise_Info
                               where (m.Type == (int)Common.EnumFile.GenCodeType.boxCode
                                   || m.Type == (int)Common.EnumFile.GenCodeType.localCreateBox ||
                                   m.Type == (int)Common.EnumFile.GenCodeType.gift ||
                                   m.Type == (int)Common.EnumFile.GenCodeType.localGift)
                               select m;
                    if (eId != null && eId > 0)
                    {
                        data = data.Where(m => m.Enterprise_Info_ID == eId);
                    }
                    if (!string.IsNullOrEmpty(mId))
                    {
                        data = data.Where(m => m.Material_ID == Convert.ToInt64(mId));
                    }
                    if (!string.IsNullOrEmpty(mName))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(mName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.RequestDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.RequestDate <= Convert.ToDateTime(endDate));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RequestCode_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }
        /// <summary>
        /// 接口获取申请码(包材码)列表
        /// </summary>
        /// <param name="eId"></param>
        /// <param name="mName"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="batchname"></param>
        /// <returns></returns>
        public List<View_RequestCodeSettingAndEnterprise_Info> InGetBoxList(string mName, string beginDate, string endDate, string batchname)
        {
            List<View_RequestCodeSettingAndEnterprise_Info> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_RequestCodeSettingAndEnterprise_Info
                               where (m.Type == (int)Common.EnumFile.GenCodeType.boxCode
                                   || m.Type == (int)Common.EnumFile.GenCodeType.localCreateBox ||
                                   m.Type == (int)Common.EnumFile.GenCodeType.gift ||
                                   m.Type == (int)Common.EnumFile.GenCodeType.localGift)
                               select m;
                    if (!string.IsNullOrEmpty(mName))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(mName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.RequestDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.RequestDate <= Convert.ToDateTime(endDate));
                    }
                    if (!string.IsNullOrEmpty(batchname))
                    {
                        data = data.Where(m => m.BatchName.Contains(batchname));
                    }
                    result = data.OrderByDescending(m => m.RequestCode_ID).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获取申请码列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">上级部门</param>
        /// <param name="ewm">产品二维码</param>
        /// <param name="name">产品名称</param>
        /// <param name="beginDate">申请时间开始</param>
        /// <param name="endDate">申请时间结束</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>申请码列表</returns>
        public List<View_RequestCodeAndEnterprise_Info> GetSysList(long? eId, string eName, string mName, string beginDate, string endDate, int pageIndex, int levelId, out long totalCount)
        {
            totalCount = 0;
            List<View_RequestCodeAndEnterprise_Info> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_RequestCodeAndEnterprise_Info select m;
                    if (eId != null && eId > 0 && levelId != 3)
                    {
                        data = data.Where(m => m.PRRU_PlatForm_ID == eId);
                    }
                    if (eId != null && eId > 0 && levelId == 4)
                    {
                        var codeInfo = from p2 in dataContext.PRRU_PlatForm
                                       where p2.Parent_ID == eId
                                       select p2.PRRU_PlatForm_ID;
                    }
                    if (!string.IsNullOrEmpty(eName))
                    {
                        data = data.Where(m => m.EnterpriseName.Contains(eName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(mName))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(mName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.RequestDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.RequestDate <= Convert.ToDateTime(endDate));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RequestCode_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 申请码
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">企业上级部门标识</param>
        /// <param name="mId">产品标识</param>
        /// <param name="codeCount">申请码数量</param>
        /// <returns>操作结果</returns>
        public RetResult Add(long eId, long upId, long mId, int codeCount, long userId)
        {
            Ret.Msg = "申请二维码失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    bool isContinue = true;
                    long allCount = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eId).RequestCodeCount ?? 0;
                    if (allCount != 0)
                    {
                        long allUseCount = dataContext.RequestCode.Where(
                            m => m.Enterprise_Info_ID == eId &&
                                (m.Status >= (int)Common.EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated ||
                                m.Status == (int)Common.EnumFile.RequestCodeStatus.GenerationIsComplete)
                            ).Sum(m => m.TotalNum) ?? 0
                        + dataContext.RequestCode.Where(
                            m => m.Enterprise_Info_ID == eId &&
                                m.Status == (int)Common.EnumFile.RequestCodeStatus.Unaudited
                            ).Sum(m => m.TotalNum) ?? 0;
                        if (allUseCount + codeCount > allCount)
                        {
                            Ret.Msg = "申请二维码失败！超过监管部门设置最大申请数量，最多还可申请" + (allCount - allUseCount) + "个码！";
                            isContinue = false;
                        }
                    }
                    if (isContinue)
                    {
                        RequestCode code = new RequestCode();
                        code.Enterprise_Info_ID = eId;
                        code.Material_ID = mId;
                        code.TotalNum = codeCount;
                        code.RequestDate = DateTime.Now;
                        code.saleCount = 0;
                        code.Status = (int)Common.EnumFile.RequestCodeStatus.Unaudited;
                        code.IsRead = (int)Common.EnumFile.IsRead.noRead;
                        dataContext.RequestCode.InsertOnSubmit(code);
                        dataContext.SubmitChanges();
                        Ret.Msg = "申请二维码成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }


        /// <summary>
        /// 修改码状态
        /// </summary>
        /// <param name="rId">申请码表标识</param>
        /// <param name="status">状态</param>
        /// <returns>操作结果</returns>
        public RetResult ChangeStatus(long rId, int status, string downLoadUrl)
        {
            string para = Common.EnumText.EnumToText(typeof(Common.EnumFile.RequestCodeStatus), status);
            para = para.Substring(0, 2);
            Ret.Msg = para + "失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    RequestCode model = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                    if (model != null && model.RequestCode_ID > 0)
                    {
                        model.Status = status;
                        if (!string.IsNullOrEmpty(downLoadUrl))
                        {
                            model.DownLoadURL = downLoadUrl;
                        }
                        dataContext.SubmitChanges();
                        Ret.Msg = para + "成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        /// <summary>
        /// 修改码状态
        /// </summary>
        /// <param name="rId">申请码表标识</param>
        /// <param name="status">状态</param>
        /// <param name="pf">登陆信息</param>
        /// <returns>操作结果</returns>
        public RetResult ChangeStatus(long rId, int status, LoginInfo pf)
        {
            string para = Common.EnumText.EnumToText(typeof(Common.EnumFile.RequestCodeStatus), status);
            para = para.Substring(0, 2);
            Ret.Msg = para + "失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    RequestCode model = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                    PRRU_PlatForm platForm = dataContext.PRRU_PlatForm.FirstOrDefault(m => m.PRRU_PlatForm_ID == pf.EnterpriseID);
                    //List<RequestCode> listRequest = dataContext.RequestCode.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID
                    //    && m.Status != (int)EnumFile.RequestCodeStatus.Unaudited).ToList();
                    List<AuditCodeRecord> listRequest = dataContext.AuditCodeRecord.Where(m => m.PRRU_PlatForm_ID == platForm.PRRU_PlatForm_ID).ToList();
                    List<SetAuditCount> setAudit = dataContext.SetAuditCount.Where(d => d.PRRU_PlatForm_ID == platForm.PRRU_PlatForm_ID).ToList();
                    if (model != null && model.RequestCode_ID > 0)
                    {
                        //bool flag = true;//监管部门有服务中心监管，审核通过的话，需要添加审核记录
                        if (pf.PRRU_PlatFormLevel_ID == 2)//监管部门
                        {
                            //判断监管部门如果有服务中心监管时，审核码的数量要受限制
                            bool flag = true;
                            int levelId = (int)dataContext.PRRU_PlatForm.FirstOrDefault(m => m.PRRU_PlatForm_ID == platForm.Parent_ID).PRRU_PlatFormLevel_ID;
                            if (levelId == 4)
                            {
                                if (setAudit.Count > 0)
                                {
                                    long auditCountAll = 0;//总的受限制的审核数量
                                    foreach (var item in setAudit)
                                    {
                                        auditCountAll += (long)item.AuditCountAll;
                                    }
                                    if (listRequest.Count > 0)
                                    {
                                        foreach (var item in listRequest)//剩下的受限制的审核数量
                                        {
                                            if (model.Specifications > 0)
                                            {
                                                auditCountAll -= (long)(item.AuditCount * model.Specifications);
                                            }
                                            else
                                            {
                                                auditCountAll -= (long)item.AuditCount;
                                            }
                                        }
                                    }
                                    if (model.Specifications > 0)//套标
                                    {
                                        long requestCount = (long)(model.Specifications * model.TotalNum);//申请的总个数
                                        if (requestCount > auditCountAll)//提示超过受限制的审核数量,不可申请
                                        {
                                            Ret.Msg = "还能审核" + Math.Floor((double)(auditCountAll / model.Specifications)) + "套,超出了审核数量,不能进行审核！";
                                            flag = false;
                                        }
                                    }
                                    else
                                    {
                                        long requestCount = (long)model.TotalNum;//申请的总个数
                                        if (requestCount > auditCountAll)//提示超过受限制的审核数量,不可申请
                                        {
                                            Ret.Msg = "还能审核" + auditCountAll + "个,超出了审核数量,不能进行审核！";
                                            flag = false;
                                        }
                                    }
                                }
                                if (flag == true)
                                {
                                    //添加审核记录AuditCodeRecord
                                    AuditCodeRecord record = new AuditCodeRecord();
                                    record.PRRU_PlatForm_ID = platForm.PRRU_PlatForm_ID;
                                    record.RequestCode_ID = model.RequestCode_ID;
                                    if (model.Specifications > 0)//套标
                                    {
                                        record.AuditCount = model.Specifications * model.TotalNum;
                                    }
                                    else
                                    {
                                        record.AuditCount = model.TotalNum;
                                    }
                                    record.AuditTime = DateTime.Now;
                                    dataContext.AuditCodeRecord.InsertOnSubmit(record);
                                    dataContext.SubmitChanges();
                                }
                                else
                                {
                                    return Ret;
                                }
                            }
                        }
                        model.Status = status;
                        dataContext.SubmitChanges();
                        Ret.Msg = para + "成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        public RetResult ChangeStatus(long rId, int status, string downLoadUrl, string filePassword, bool IsEncryption, bool Check_Image)
        {
            string para = Common.EnumText.EnumToText(typeof(Common.EnumFile.RequestCodeStatus), status);
            para = para.Substring(0, 2);
            Ret.Msg = para + "失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    RequestCode model = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                    if (model != null && model.RequestCode_ID > 0)
                    {
                        model.Status = status;
                        if (!string.IsNullOrEmpty(downLoadUrl))
                        {
                            model.DownLoadURL = downLoadUrl;
                        }
                        dataContext.SubmitChanges();
                        Ret.Msg = para + "成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        public RetResult UpdateDownLoadNum(long RequestId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataModel = dataContext.RequestCode.Where(w => w.RequestCode_ID == RequestId).FirstOrDefault();
                    DataModel.DownLoadNum = DataModel.DownLoadNum == null ? 1 : DataModel.DownLoadNum.Value + 1;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "更新成功！", "更新成功！");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "更新失败！", "更新失败！");
            }

            return Ret;
        }

        /// <summary>
        /// 获取已销售列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<View_RequestCodeAndEnterprise_Info> GetSaleList(long eId, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<View_RequestCodeAndEnterprise_Info> result = new List<View_RequestCodeAndEnterprise_Info>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data = from m in dataContext.View_RequestCodeAndEnterprise_Info where m.saleCount > 0 && m.Status == (int)Common.EnumFile.RequestCodeStatus.GenerationIsComplete || m.Status == (int)Common.EnumFile.RequestCodeStatus.PackagingFailure || m.Status == (int)Common.EnumFile.RequestCodeStatus.PackToSuccess select m;
                if (eId > 0)
                    data = data.Where(m => m.Enterprise_Info_ID == eId);
                data = data.OrderByDescending(m => m.RequestCode_ID);
                totalCount = data.Count();
                result = data.Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                ClearLinqModel(result);
            }
            return result;
        }
        /// <summary>
        /// 获取已销售列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<LinqModel.SalesInformation> GetSaleList(string beginDate, string endDate, long eId, int pageIndex, out long totalCount)
        {
            List<SalesInformation> result = new List<SalesInformation>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext("Code_Connect"))
            {
                try
                {
                    var data = dataContext.SalesInformation.Where(m => m.EnterpriseId == eId && m.SellCount > 0);
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.SalesDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.SalesDate <= Convert.ToDateTime(endDate));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.SalesDate).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeDAL.GetSaleList():SalesInformation表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取二维码列表
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="rId">申请码标识</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCoount">总条数</param>
        /// <returns>列表</returns>
        public List<Enterprise_FWCode_00> GetEWMCode(string ewm, long rId, int status, int pageIndex, int pageSize, out long totalCoount)
        {
            List<Enterprise_FWCode_00> result = null;
            long beginCode = 0;
            long endCode = 0;
            long createCount = 0;
            long saleCount = 0;
            long pageCount = createCount;
            string tablename = "";
            using (DataClassesDataContext dataContext = GetDynamicDataContext(rId, out beginCode, out endCode, out createCount, out saleCount, out tablename))
            {
                try
                {
                    dataContext.CommandTimeout = 600;
                    if (!String.IsNullOrEmpty(ewm))
                    {
                        string sql = "select top 1 * from " + tablename + " where RequestCode_ID=" + rId + " and ewm='" + ewm.Trim() + "'";
                        result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                        ClearLinqModel(result);
                        pageCount = result.Count();
                    }
                    else
                    {
                        StringBuilder strSql = new StringBuilder();
                        if (status <= 0)//没有选择状态
                        {
                            long endCount = (beginCode + (PageSize * pageIndex));
                            //strSql.Append("select * from " + tablename + " where RequestCode_ID=" + rId + " and Enterprise_FWCode_ID>=" + (beginCode + (PageSize * (pageIndex - 1))) + " and Enterprise_FWCode_ID<" + endCount + " order by UseTime desc");
                            strSql.Append("select * from " + tablename + " where RequestCode_ID=" + rId + " order by Enterprise_FWCode_ID");
                        }
                        else//选择状态
                        {
                            //strSql.Append("select top " + PageSize + " * from " + tablename + " where Enterprise_FWCode_ID>=" + beginCode + " and Enterprise_FWCode_ID<=" + endCode);
                            strSql.Append("select top " + PageSize + " * from " + tablename + " where 1=1");
                            //strSql.Append(" and Enterprise_FWCode_ID not in(select top " + (PageSize * (pageIndex - 1)) + " Enterprise_FWCode_ID from " + tablename + " where Enterprise_FWCode_ID>=" + beginCode + " and Enterprise_FWCode_ID<=" + endCode);
                            strSql.Append(" and Enterprise_FWCode_ID not in(select top " + (PageSize * (pageIndex - 1)) + " Enterprise_FWCode_ID from " + tablename + " where RequestCode_ID=" + rId);
                            strSql.Append(" and Status=" + status);
                            strSql.Append(" order by UseTime desc");
                            strSql.Append(")");
                            strSql.Append(" and RequestCode_ID=" + rId + " and Status=" + status);
                            strSql.Append(" order by UseTime desc");
                        }

                        if (status == (int)Common.EnumFile.UsingStateCode.HasBeenUsed)
                        {
                            pageCount = Convert.ToInt32(saleCount);
                        }
                        else if (status == (int)Common.EnumFile.UsingStateCode.NotUsed)
                        {
                            pageCount = createCount - Convert.ToInt32(saleCount);
                        }
                        else if (status == 0)
                        {
                            pageCount = createCount;
                        }
                        result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();

                        if (result != null && (result[0].Type == 1 || result[0].Type == 2))
                        {
                            string codeSum = (from x in result[0].codeXML.Descendants("boxcode")
                                              select x.Attribute("codeSum").Value).FirstOrDefault();
                            pageCount = createCount * (Convert.ToInt32(codeSum) + 1);
                        }

                        result = result.Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                        ClearLinqModel(result);
                    }
                }
                catch (Exception ex) { }
            }
            totalCoount = pageCount;
            return result;
        }


        public List<Enterprise_FWCode_00> GetSalesDetail(string ewm, string strTableIdArray, long SalesId, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext("Code_Connect"))
                {
                    string[] TableIdArray = strTableIdArray.Split(',');
                    List<Enterprise_FWCode_00> DataList = new List<Enterprise_FWCode_00>();
                    for (int i = 0; i < TableIdArray.Length; i++)
                    {
                        string strTableName = "Enterprise_FWCode_" + TableIdArray[i].Trim().PadLeft(2, '0');
                        string strSql = @"select * from " + strTableName + " where SalesInformation_ID=" + SalesId;
                        if (!string.IsNullOrEmpty(ewm))
                        {
                            strSql = @"select * from " + strTableName + " where SalesInformation_ID=" + SalesId
                               + " and EWM='" + ewm + "'";
                        }
                        DataList.AddRange(dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql).ToList());
                    }
                    totalCount = DataList.Count;
                    DataList = DataList.Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(DataList);
                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #region 销售

        /// <summary>
        /// 销售
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="batchId">批次标识</param>
        /// <param name="batchext_id">小批次标识</param>
        /// <param name="DealerId">经销商标识</param>
        /// <param name="EWMBegin">起始码</param>
        /// <param name="EWMEnd">结束码</param>
        /// <returns>操作结果</returns>
        public RetResult SaleCode(long eId, long batchId, long? batchext_id, long DealerId, string saleDate, string EWMBegin, string EWMEnd)
        {
            Ret.Msg = "销售失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            SalesInformation sModel = new SalesInformation();
            #region 码判断
            Enterprise_FWCode_00 codeBegin = GetEWM(EWMBegin);
            Enterprise_FWCode_00 codeEnd = GetEWM(EWMEnd);

            bool codeI = false;
            if (codeBegin == null)
            {
                Ret.Msg = "起始码错误";
            }
            else if (codeBegin.Status == 1040000009)
            {
                Ret.Msg = "起始码已销售";
            }
            else if (codeEnd == null)
            {
                Ret.Msg = "结束码错误";
            }
            else if (codeEnd.Status == 1040000009)
            {
                Ret.Msg = "结束码已销售";
            }
            else if (codeBegin.Enterprise_FWCode_ID > codeEnd.Enterprise_FWCode_ID)
            {
                Ret.Msg = "起始码不能大于结束码！";
            }
            else
            {
                codeI = true;
            }
            #endregion
            if (codeI)
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    BatchExt be = null;
                    Batch batch = dataContext.Batch.FirstOrDefault(m => m.Batch_ID == batchId);
                    if (batch == null)
                    {
                        Ret.Msg = "没有找到该批次";
                    }
                    else
                    {
                        string[] arrBegin = EWMBegin.Split('.');
                        string split_tableBegin = arrBegin[arrBegin.Length - 2].Substring(0, arrBegin[arrBegin.Length - 2].Length - 9);
                        //string yuyanBegin = arrBegin[arrBegin.Length - 2];

                        string[] arrEnd = EWMEnd.Split('.');
                        string split_tableEnd = arrEnd[arrEnd.Length - 2].Substring(0, arrEnd[arrEnd.Length - 2].Length - 9);
                        //string yuyanEnd = arrEnd[arrEnd.Length - 2];

                        var tableBegin = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == long.Parse(split_tableBegin));
                        var tableEnd = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == long.Parse(split_tableEnd));

                        if (tableBegin == null || tableEnd == null || tableEnd.Route_DataBase_ID < tableBegin.Route_DataBase_ID)
                        {
                            Ret.Msg = "获取数据失败";
                        }
                        else
                        {
                            bool isContinue = true;
                            if (batchext_id != null && batchext_id != 0)
                            {
                                be = dataContext.BatchExt.FirstOrDefault(m => m.BatchExt_ID == batchext_id && m.Batch_ID == batchId);
                                if (be == null)
                                {
                                    Ret.Msg = "获取子批次数据失败！";
                                    isContinue = false;
                                }
                            }
                            if (isContinue)
                            {
                                var dealerTemp = dataContext.Dealer.FirstOrDefault(m => m.Dealer_ID == DealerId && m.Enterprise_Info_ID == eId);
                                if (dealerTemp == null)
                                {
                                    Ret.Msg = "获取经销商数据失败！";
                                }
                                else
                                {
                                    string strSql = "";
                                    DateTime now = DateTime.Now;
                                    int count = 0;
                                    if (tableBegin.Route_DataBase_ID == tableEnd.Route_DataBase_ID)
                                    {
                                        using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                                        {
                                            strSql = "update " + tableBegin.TableName + " set SalesTime='" + saleDate// now.ToString("yyyy-MM-dd HH:mm:ss")
                                                + "',Status=1040000009"
                                                + ",Batch_ID=" + batchId
                                                + ",BatchExt_ID=" + (batchext_id.HasValue ? batchext_id.ToString() : "null")
                                                + ",Material_ID=" + batch.Material_ID
                                                + ",UseTime='" + now.ToString("yyyy-MM-dd HH:mm:ss")
                                                + "',SalesInformation_ID=null"//+ "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                                + ",Dealer_ID=" + DealerId.ToString() + " where Enterprise_Info_ID=" + eId + " and Status != 1040000009 and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID + " and " + codeEnd.Enterprise_FWCode_ID;
                                            count += dataContextEWM.ExecuteCommand(strSql);

                                            #region 获取销售数量
                                            string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName + " where Enterprise_Info_ID=" + eId + " and Status = 1040000009 and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID + " and " + codeEnd.Enterprise_FWCode_ID + " group by RequestCode_ID";
                                            List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                            if (listRC != null && listRC.Count > 0)
                                            {
                                                string strRequestCodeID = string.Empty;
                                                foreach (SaleRequestCodeCount item in listRC)
                                                {
                                                    strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                                }

                                                string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName + " where Enterprise_Info_ID=" + eId + " and RequestCode_ID in (" + strRequestCodeID.Substring(1) + ") and Status = 1040000009 group by RequestCode_ID";
                                                List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                                foreach (SaleRequestCodeCount item in listRC2)
                                                {
                                                    var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                                    if (rc != null)
                                                        rc.saleCount = item.saleCount == null ? 0 : Convert.ToInt64(item.saleCount);
                                                    dataContext.SubmitChanges();
                                                }
                                            }
                                            #endregion

                                            sModel.ProductionTime = DateTime.Now;
                                            sModel.EnterpriseId = eId;
                                            sModel.SellCount = count;
                                            dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                                            dataContextEWM.SubmitChanges();
                                            Ret.CmdError = CmdResultError.NONE;
                                            Ret.Msg = "销售成功，总共销售" + count + "个码";
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i <= tableEnd.Route_DataBase_ID - tableBegin.Route_DataBase_ID; i++)
                                        {
                                            var newTable = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == tableBegin.Route_DataBase_ID + i);
                                            using (DataClassesDataContext dataContextEWM = GetDataContext(newTable.DataSource, newTable.DataBaseName, newTable.UID, newTable.PWD))
                                            {
                                                if (newTable.Route_DataBase_ID == tableBegin.Route_DataBase_ID)
                                                {
                                                    strSql = "update " + newTable.TableName + " set SalesTime='" + saleDate// now.ToString("yyyy-MM-dd HH:mm:ss")
                                                        + "',Status=1040000009"
                                                        + ",Batch_ID=" + batchId
                                                        + ",BatchExt_ID=" + (batchext_id.HasValue ? batchext_id.ToString() : "null")
                                                        + ",Material_ID=" + batch.Material_ID
                                                        + ",UseTime='" + now.ToString("yyyy-MM-dd HH:mm:ss")
                                                        + "',SalesInformation_ID=null"//+ "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                                        + ",Dealer_ID=" + DealerId.ToString() + " where Enterprise_Info_ID=" + eId + " and Status != 1040000009 and Enterprise_FWCode_ID>=" + codeBegin.Enterprise_FWCode_ID;
                                                    count += dataContextEWM.ExecuteCommand(strSql);

                                                    #region 获取销售数量
                                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + newTable.TableName + " where Enterprise_Info_ID=" + eId + " and Status = 1040000009 and Enterprise_FWCode_ID>=" + codeBegin.Enterprise_FWCode_ID + " group by RequestCode_ID";
                                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                                    if (listRC != null && listRC.Count > 0)
                                                    {
                                                        string strRequestCodeID = string.Empty;
                                                        foreach (SaleRequestCodeCount item in listRC)
                                                        {
                                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                                        }

                                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + newTable.TableName + " where Enterprise_Info_ID=" + eId + " and RequestCode_ID in (" + strRequestCodeID.Substring(1) + ") and Status = 1040000009 group by RequestCode_ID";
                                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                                        foreach (SaleRequestCodeCount item in listRC2)
                                                        {
                                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                                            if (rc != null)
                                                                rc.saleCount = item.saleCount == null ? 0 : Convert.ToInt64(item.saleCount);
                                                            dataContext.SubmitChanges();
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else if (newTable.Route_DataBase_ID == tableEnd.Route_DataBase_ID)
                                                {
                                                    strSql = "update " + newTable.TableName + " set SalesTime='" + saleDate//now.ToString("yyyy-MM-dd HH:mm:ss")
                                                        + "',Status=1040000009"
                                                        + ",Batch_ID=" + batchId
                                                        + ",BatchExt_ID=" + (batchext_id.HasValue ? batchext_id.ToString() : "null")
                                                        + ",Material_ID=" + batch.Material_ID
                                                        + ",UseTime='" + now.ToString("yyyy-MM-dd HH:mm:ss")
                                                        + "',SalesInformation_ID=null"//+ "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                                        + ",Dealer_ID=" + DealerId.ToString() + " where Enterprise_Info_ID=" + eId + " and Status != 1040000009 and Enterprise_FWCode_ID<=" + codeEnd.Enterprise_FWCode_ID;
                                                    count += dataContextEWM.ExecuteCommand(strSql);

                                                    #region 获取销售数量
                                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + newTable.TableName + " where Enterprise_Info_ID=" + eId + " and Status = 1040000009 and Enterprise_FWCode_ID<=" + codeEnd.Enterprise_FWCode_ID + " group by RequestCode_ID";
                                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList(); ;
                                                    if (listRC != null && listRC.Count > 0)
                                                    {
                                                        string strRequestCodeID = string.Empty;
                                                        foreach (SaleRequestCodeCount item in listRC)
                                                        {
                                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                                        }

                                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + newTable.TableName + " where Enterprise_Info_ID=" + eId + " and RequestCode_ID in (" + strRequestCodeID.Substring(1) + ") and Status = 1040000009 group by RequestCode_ID";
                                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                                        foreach (SaleRequestCodeCount item in listRC2)
                                                        {
                                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                                            if (rc != null)
                                                                rc.saleCount = item.saleCount == null ? 0 : Convert.ToInt64(item.saleCount);
                                                            dataContext.SubmitChanges();
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    strSql = "update " + newTable.TableName + " set SalesTime='" + saleDate//now.ToString("yyyy-MM-dd HH:mm:ss")
                                                        + "',Status=1040000009"
                                                        + ",Batch_ID=" + batchId
                                                        + ",BatchExt_ID=" + (batchext_id.HasValue ? batchext_id.ToString() : "null")
                                                        + ",Material_ID=" + batch.Material_ID
                                                        + ",UseTime='" + now.ToString("yyyy-MM-dd HH:mm:ss")
                                                        + "',SalesInformation_ID=null"//+ "',SalesInformation_ID=" + sModel.SalesInformation_ID
                                                        + ",Dealer_ID=" + DealerId.ToString() + " where Enterprise_Info_ID=" + eId + " and Status != 1040000009";
                                                    count += dataContextEWM.ExecuteCommand(strSql);

                                                    #region 获取销售数量
                                                    string saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + newTable.TableName + " where Enterprise_Info_ID=" + eId + " and Status = 1040000009 group by RequestCode_ID";
                                                    List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                                                    if (listRC != null && listRC.Count > 0)
                                                    {
                                                        string strRequestCodeID = string.Empty;
                                                        foreach (SaleRequestCodeCount item in listRC)
                                                        {
                                                            strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                                                        }

                                                        string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + newTable.TableName + " where Enterprise_Info_ID=" + eId + " and RequestCode_ID in (" + strRequestCodeID.Substring(1) + ") and Status = 1040000009 group by RequestCode_ID";
                                                        List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                                                        foreach (SaleRequestCodeCount item in listRC2)
                                                        {
                                                            var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                                            if (rc != null)
                                                                rc.saleCount = item.saleCount == null ? 0 : Convert.ToInt64(item.saleCount);
                                                            dataContext.SubmitChanges();
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                sModel.ProductionTime = DateTime.Now;
                                                sModel.EnterpriseId = eId;
                                                sModel.SellCount = count;
                                                dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                                                dataContextEWM.SubmitChanges();
                                            }
                                        }
                                        Ret.CmdError = CmdResultError.EXCEPTION;
                                        Ret.Msg = "销售成功，总共销售" + count + "个码";
                                    }
                                    try
                                    {
                                        batch.Status = (int)Common.EnumFile.Status.saled;
                                        dataContext.SubmitChanges();
                                        be.Status = (int)Common.EnumFile.Status.saled;
                                        dataContext.SubmitChanges();
                                    }
                                    catch { }
                                }

                            }
                        }
                    }
                }
            }
            return Ret;
        }

        /// <summary>
        /// 销售方法
        /// </summary>
        /// <param name="eId">企业编号</param>
        /// <param name="CodeType">二维码类型</param>
        /// <param name="DealerId">经销商编号</param>
        /// <param name="EWMBegin">起始码</param>
        /// <param name="EWMEnd">结束码</param>
        /// <param name="productionTime">生产日期</param>
        /// <returns>销售结果</returns>
        public RetResult SaleCodeSingle(long eId, int CodeType, long DealerId, Enterprise_FWCode_00 codeBegin, Enterprise_FWCode_00 codeEnd,
            long rountId, DateTime productionTime)
        {
            string errData = "销售失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            DateTime now = DateTime.Now;
            string strSql = "";
            SalesInformation sModel = new SalesInformation();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    #region 获取销售数据
                    Dealer dealerModel = new Dealer();
                    Enterprise_Info eInfo = new Enterprise_Info();
                    string msg = "";
                    eInfo = new Dal.EnterpriseInfoDAL().GetModel(eId);
                    bool isGetM = GetSalesCodeInformation(eId, DealerId, out dealerModel, out eInfo, out msg);
                    if (!isGetM)
                    {
                        Ret.SetArgument(error, errData, errData);
                        return Ret;
                    }
                    GetDealerXml(dealerModel, ref sModel);
                    GetEnterpriseInfoXml(eInfo, ref sModel);
                    sModel.ProductionTime = productionTime;
                    sModel.MaterialShelfLife = "";
                    sModel.SalesDate = now;
                    sModel.Type = codeBegin.Type;
                    #endregion
                    string materialFullName = dataContext.Material.FirstOrDefault(w => w.Material_ID == codeBegin.Material_ID).MaterialName;
                    var dealerTemp = dataContext.Dealer.FirstOrDefault(m => m.Dealer_ID == DealerId && m.Enterprise_Info_ID == eId);
                    if (dealerTemp == null)
                    {
                        errData = "获取经销商数据失败！";
                        Ret.SetArgument(error, errData, errData);
                        return Ret;
                    }
                    sModel.MaterialFullName = materialFullName;
                    sModel.DealerName = dealerTemp.DealerName;
                    //销售数量
                    long count = 0;
                    Route_DataBase tableBegin = dataContext.Route_DataBase.Where(p => p.Route_DataBase_ID == rountId).FirstOrDefault();
                    if (tableBegin == null)
                    {
                        errData = "获取路由信息失败！";
                        Ret.SetArgument(error, errData, errData);
                        return Ret;
                    }
                    using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                    {
                        dataContextEWM.SalesInformation.InsertOnSubmit(sModel);
                        dataContextEWM.SubmitChanges();
                        //查找在码段中间销售过的码，去掉中间数量
                        string saleRequsetCountSql = "select count(1) as saleCount from " + tableBegin.TableName
                       + " where Enterprise_Info_ID=" + eId
                       + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + " and Enterprise_FWCode_ID between " + codeBegin.Enterprise_FWCode_ID +
                       " and " + codeEnd.Enterprise_FWCode_ID + " group by Status";
                        SaleRequestCodeCount dataRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).FirstOrDefault();
                        long isHaveNum = 0;
                        if (dataRC != null && dataRC.saleCount != null)
                        {
                            isHaveNum = Convert.ToInt64(dataRC.saleCount);
                        }
                        int status = (int)Common.EnumFile.UsingStateCode.NotUsed;
                        //如果企业需要激活二维码，则查找所有已激活的二维码
                        if (eInfo.IsActive > 0)
                        {
                            status = (int)Common.EnumFile.UsingStateCode.Activated;
                        }
                        //更新码的销售数据
                        strSql = "update " + tableBegin.TableName + " set SalesTime='" + productionTime
                            + "',Status='" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                            + "',ScanCount=0"
                            + ",FWCount=0"
                            + ",ValidateTime=NULL"
                            + ",UseTime='" + now.ToString("yyyy-MM-dd HH:mm:ss")
                            + "',SalesInformation_ID=" + sModel.SalesInformation_ID
                            + ",Dealer_ID=" + DealerId.ToString() + " where Enterprise_Info_ID=" + eId
                            + " and Status = " + status + " and Enterprise_FWCode_ID between "
                            + codeBegin.Enterprise_FWCode_ID + " and " + codeEnd.Enterprise_FWCode_ID;
                        dataContextEWM.ExecuteCommand(strSql);
                        #region 获取销售数量
                        //考虑到首尾码不在同一个申请批次的情况
                        saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                            + " where Enterprise_Info_ID=" + eId
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
                            count -= isHaveNum;
                            //更新申请码表销售数据
                            string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                + " where Enterprise_Info_ID=" + eId
                                + " and RequestCode_ID in (" + strRequestCodeID.Substring(1)
                                + ") and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + " group by RequestCode_ID";
                            List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                            foreach (SaleRequestCodeCount item in listRC2)
                            {
                                var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                if (rc != null)
                                    rc.saleCount = item.saleCount == null ? 0 : Convert.ToInt64(item.saleCount);
                                dataContext.SubmitChanges();
                            }
                        }
                        #endregion
                        dataContext.SubmitChanges();
                        error = CmdResultError.NONE;
                        errData = "操作成功,总共销售" + count + "个码";
                        Ret.SetArgument(error, errData, errData);
                        sModel.SellCount = count;
                        sModel.EwmTableIdArray = (tableBegin.Route_DataBase_ID - 1).ToString();
                        dataContextEWM.SubmitChanges();
                        return Ret;
                    }
                }
            }
            catch (Exception ex)
            {
                error = CmdResultError.EXCEPTION;
                errData = "销售时出现错误！";
                Ret.SetArgument(error, errData, errData);
                return Ret;
            }
        }

        #region 销售套标码方法20170829修改
        /// <summary>
        /// 销售套标码方法
        /// </summary>
        /// <param name="startCode">开始箱标码</param>
        /// <param name="endCode">结束箱标码</param>
        /// <param name="dealerid">经销商ID</param>
        /// <param name="eId">企业ID</param>
        /// <param name="productionTime">销售日期</param>
        /// <param name="shelfLife">保质期</param>
        /// <returns>返回激活结果</returns>
        public RetResult SaleCodeGroup(int CodeType, string materialName, Enterprise_FWCode_00 startCodeInfo, Enterprise_FWCode_00 endCodeInfo,
            long dealerid, long Route_DataBase_IDB, long eId, DateTime? productionTime)
        {
            SalesInformation objSalesInformation = new SalesInformation();
            string materialFullName = string.Empty;
            string dealerName = string.Empty;
            string errData = "销售失败！";
            string strSql = string.Empty;
            DateTime dateTimeNow = DateTime.Now;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    #region 获取销售数据
                    Dealer dealerModel = new Dealer();
                    GetDealerXml(dealerModel, ref objSalesInformation);
                    objSalesInformation.ProductionTime = productionTime;
                    objSalesInformation.SalesDate = dateTimeNow;
                    objSalesInformation.Type = (int)Common.EnumFile.GenCodeType.boxCode;
                    #endregion
                    materialFullName = dataContext.Material.FirstOrDefault(w => w.Material_ID == startCodeInfo.Material_ID).MaterialName;
                    dealerName = dataContext.Dealer.FirstOrDefault(w => w.Dealer_ID == dealerid).DealerName;
                    objSalesInformation.MaterialFullName = materialFullName;
                    objSalesInformation.DealerName = dealerName;
                    objSalesInformation.EnterpriseId = Convert.ToInt64(startCodeInfo.Enterprise_Info_ID);
                    Enterprise_Info eInfo = dataContext.Enterprise_Info.FirstOrDefault(w => w.Enterprise_Info_ID == objSalesInformation.EnterpriseId);
                    objSalesInformation.Enterprise_InfoName = eInfo.EnterpriseName;
                    // 获取结束码的规格
                    int specification = dataContext.RequestCode.FirstOrDefault(w => w.RequestCode_ID == endCodeInfo.RequestCode_ID).Specifications.Value;
                    // 计算结束码中的最后一个瓶码
                    endCodeInfo.Enterprise_FWCode_ID = endCodeInfo.Enterprise_FWCode_ID + specification;
                    // 获取开始码的表信息
                    var tableBegin = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == Route_DataBase_IDB);
                    // 获取经销商信息
                    var dealerTemp = dataContext.Dealer.FirstOrDefault(m => m.Dealer_ID == dealerid && m.Enterprise_Info_ID == eId);
                    if (dealerTemp == null)
                    {
                        errData = "获取经销商数据失败！";
                        Ret.SetArgument(CmdResultError.EXCEPTION, errData, errData);
                        return Ret;
                    }
                    long count = 0;
                    string TableIdArray = string.Empty;
                    // 创建二维码表的数据库连接
                    using (DataClassesDataContext dataContextEWM = GetDataContext(tableBegin.DataSource, tableBegin.DataBaseName, tableBegin.UID, tableBegin.PWD))
                    {
                        dataContextEWM.SalesInformation.InsertOnSubmit(objSalesInformation);
                        dataContextEWM.SubmitChanges();
                        TableIdArray += (tableBegin.Route_DataBase_ID - 1).ToString();
                        string saleRequsetCountSql = "select count(1) as saleCount from " + tableBegin.TableName
                           + " where Enterprise_Info_ID=" + eId
                           + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + " and Enterprise_FWCode_ID between " + startCodeInfo.Enterprise_FWCode_ID + " and " + endCodeInfo.Enterprise_FWCode_ID + " group by Status";
                        SaleRequestCodeCount dataRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).FirstOrDefault();
                        long isHaveNum = 0;
                        if (dataRC != null && dataRC.saleCount != null)
                        {
                            isHaveNum = Convert.ToInt64(dataRC.saleCount);
                        }
                        int status = (int)Common.EnumFile.UsingStateCode.NotUsed;
                        //如果企业需要激活二维码，则查找所有已激活的二维码
                        if (eInfo.IsActive > 0)
                        {
                            status = (int)Common.EnumFile.UsingStateCode.Activated;
                        }
                        // 激活箱标码SQL
                        strSql = "update " + tableBegin.TableName + " set SalesTime='" + productionTime
                            + "',Status=" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed + ""
                            + ",ScanCount=0"
                            + ",FWCount=0"
                            + ",ValidateTime=NULL"
                            + ",Material_ID=" + startCodeInfo.Material_ID
                            + ",UseTime='" + dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss")
                            + "',Dealer_ID=" + dealerid.ToString()
                            + ",SalesInformation_ID=" + objSalesInformation.SalesInformation_ID
                            + " where Enterprise_Info_ID=" + eId
                            + " and Status = " + status + "  and Enterprise_FWCode_ID between " + startCodeInfo.Enterprise_FWCode_ID
                            + " and " + endCodeInfo.Enterprise_FWCode_ID;
                        dataContextEWM.ExecuteCommand(strSql);
                        #region 获取销售数量
                        saleRequsetCountSql = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                            + " where Enterprise_Info_ID=" + eId
                            + " and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                            + " and Enterprise_FWCode_ID between " + startCodeInfo.Enterprise_FWCode_ID + " and " + endCodeInfo.Enterprise_FWCode_ID + " group by RequestCode_ID";
                        List<SaleRequestCodeCount> listRC = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql).ToList();
                        if (listRC != null && listRC.Count > 0)
                        {
                            string strRequestCodeID = string.Empty;
                            foreach (SaleRequestCodeCount item in listRC)
                            {
                                count += Convert.ToInt64(item.saleCount);
                                strRequestCodeID = strRequestCodeID + "," + item.RequestCode_ID;
                            }
                            count -= isHaveNum;
                            //更新RequestCode表中的销售数量字段
                            string saleRequsetCountSql2 = "select RequestCode_ID,count(1) as saleCount from " + tableBegin.TableName
                                + " where Enterprise_Info_ID=" + eId
                                + " and RequestCode_ID in (" + strRequestCodeID.Substring(1) + ") and Status = " + (int)Common.EnumFile.UsingStateCode.HasBeenUsed
                                + " group by RequestCode_ID";
                            List<SaleRequestCodeCount> listRC2 = dataContextEWM.ExecuteQuery<SaleRequestCodeCount>(saleRequsetCountSql2).ToList();
                            foreach (SaleRequestCodeCount item in listRC2)
                            {
                                var rc = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == item.RequestCode_ID);
                                if (rc != null)
                                    rc.saleCount = item.saleCount == null ? 0 : Convert.ToInt64(item.saleCount);
                                dataContext.SubmitChanges();
                            }
                        }
                        #endregion
                        dataContext.SubmitChanges();
                        errData = "操作成功！总共销售" + count + "个码！其中箱码" + (count / (specification + 1)).ToString() + "个,子码" + (count - (count / (specification + 1))).ToString() + "个";
                        Ret.SetArgument(CmdResultError.NONE, errData, errData);
                        objSalesInformation.SellCount = (count / (specification + 1));
                        objSalesInformation.EwmTableIdArray = TableIdArray;
                        dataContextEWM.SubmitChanges();
                        return Ret;
                    }
                }
            }
            catch (Exception ex)
            {
                errData = "获取数据失败";
                Ret.SetArgument(CmdResultError.EXCEPTION, ex.ToString(), errData);
                return Ret;
            }
        }
        #endregion

        /// <summary>
        /// 获取销售数据
        /// </summary>
        /// <param name="enterprise_Info_ID">企业编号</param>
        /// <param name="DealerId">经销商编号</param>
        /// <param name="dealerModel">经销商</param>
        /// <param name="eInfo">企业</param>
        /// <param name="msg">消息</param>
        /// <returns>返回数据</returns>
        public bool GetSalesCodeInformation(long enterprise_Info_ID, long DealerId, out Dealer dealerModel, out Enterprise_Info eInfo, out string msg)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                dealerModel = null;
                msg = string.Empty;
                eInfo = null;
                //获取经销商信息
                dealerModel = new Dal.DealerDAL().GetModel(DealerId);
                //企业信息
                eInfo = new Dal.EnterpriseInfoDAL().GetModel(enterprise_Info_ID);
                return true;
            }
        }
        /// <summary>
        /// 读取经销商信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="objSalesInformation"></param>
        private void GetDealerXml(Dealer model, ref SalesInformation objSalesInformation)
        {
            if (model != null)
            {
                objSalesInformation.DealerName = model.DealerName;
            }
        }
        /// <summary>
        /// 返回企业实体XML字段属性组合
        /// </summary>
        /// <param name="model">企业实体</param>
        /// <returns>XML字符串</returns>
        private void GetEnterpriseInfoXml(Enterprise_Info model, ref SalesInformation objSalesInformation)
        {
            if (model == null)
                return;
            objSalesInformation.EnterpriseId = model.Enterprise_Info_ID;
            objSalesInformation.Enterprise_InfoName = model.EnterpriseName;
        }

        /// <summary>
        /// 获取二维码信息
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns>模型</returns>
        public Enterprise_FWCode_00 GetEWM(string ewm)
        {
            long Route_DataBase_ID = 0;
            string tableName = "";
            Enterprise_FWCode_00 result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    //查找路由信息
                    int index = 0;
                    RequestCode codeModel = new RequestCode();
                    string[] bigPara = ewm.Split('/');
                    string fixedCode = string.Empty;
                    int codeType = 0;
                    try
                    {
                        //正常码
                        if (bigPara.Length == 3)
                        {
                            string[] smallPara = bigPara[2].Split('.');
                            //查找路由信息
                            index = smallPara[1].Length == 3 ?
                                new BinarySystem62().Convert62ToNo(smallPara[1]) : Convert.ToInt32(smallPara[1]);
                            //码类型
                            if (smallPara.Length == 3)
                            {
                                codeType = CodeNodeToType.ToType(smallPara[2]);
                            }
                            fixedCode = bigPara[0] + "/" + bigPara[1] + "/" + smallPara[0];
                            codeModel = dataContext.RequestCode.FirstOrDefault(
                                m => m.Type == codeType && m.StartNum <= index && m.EndNum >= index
                                && m.FixedCode == fixedCode);
                        }
                        //农药码
                        else if (bigPara.Length == 1)
                        {
                            fixedCode = ewm.Substring(0, ewm.Length - 9);
                            codeType = (int)Common.EnumFile.GenCodeType.pesticides;
                            index = Convert.ToInt32(ewm.Substring(23, 9));
                            codeModel = dataContext.RequestCode.FirstOrDefault(
                                m => m.Type == codeType && m.StartNum <= index && m.EndNum >= index
                                && m.FixedCode == fixedCode);
                        }
                        else
                        {
                            return null;
                        }
                        if (codeModel != null && codeModel.Route_DataBase_ID > 0)
                        {
                            Route_DataBase_ID = (long)codeModel.Route_DataBase_ID;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    var data_split_table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == codeModel.Route_DataBase_ID);
                    Route_DataBase_ID = data_split_table.Route_DataBase_ID;
                }
                using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
                {
                    string sql = string.Format("select * from {0} where ewm='{1}'", tableName, ewm);
                    result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 获取二维码数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="codeModel"></param>
        /// <returns></returns>
        public Enterprise_FWCode_00 GetEWMModel(string ewm, out RequestCode codeModel)
        {
            long Route_DataBase_ID = 0;
            codeModel = new RequestCode();
            RequestCode requestCode = new RequestCode();
            string tableName = "";
            Enterprise_FWCode_00 result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    //查找路由信息
                    int index = 0;
                    string[] bigPara = ewm.Split('/');
                    string fixedCode = string.Empty;
                    int codeType = 0;
                    try
                    {
                        //正常码
                        if (bigPara.Length == 3)
                        {
                            string[] smallPara = bigPara[2].Split('.');
                            //查找路由信息
                            index = smallPara[1].Length == 3 ?
                                new BinarySystem62().Convert62ToNo(smallPara[1]) : Convert.ToInt32(smallPara[1]);
                            //码类型
                            if (smallPara.Length == 3)
                            {
                                codeType = CodeNodeToType.ToType(smallPara[2]);
                            }
                            fixedCode = bigPara[0] + "/" + bigPara[1] + "/" + smallPara[0];
                            requestCode = dataContext.RequestCode.FirstOrDefault(
                                m => m.Type == codeType && m.StartNum <= index && m.EndNum >= index
                                && m.FixedCode == fixedCode);
                        }
                        //农药码
                        else if (bigPara.Length == 1)
                        {
                            fixedCode = ewm.Substring(0, ewm.Length - 9);
                            codeType = (int)Common.EnumFile.GenCodeType.pesticides;
                            index = Convert.ToInt32(ewm.Substring(23, 9));
                            requestCode = dataContext.RequestCode.FirstOrDefault(
                                m => m.Type == codeType && m.StartNum <= index && m.EndNum >= index
                                && m.FixedCode == fixedCode);

                        }
                        else
                        {
                            return null;
                        }
                        if (requestCode != null && requestCode.Route_DataBase_ID > 0)
                        {
                            Route_DataBase_ID = (long)requestCode.Route_DataBase_ID;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    var data_split_table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == requestCode.Route_DataBase_ID);
                    Route_DataBase_ID = data_split_table.Route_DataBase_ID;
                }
                using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
                {
                    string sql = string.Format("select * from {0} where ewm='{1}'", tableName, ewm);
                    result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();
                }
            }
            catch { }
            codeModel = requestCode;
            return result;
        }

        public Enterprise_FWCode_00 GetSellEWM(string ewm)
        {
            long Route_DataBase_ID = 0;
            string tableName = "";
            Enterprise_FWCode_00 result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    string[] arr = ewm.Split('.');
                    string split_table = arr[arr.Length - 2].Substring(0, arr[arr.Length - 2].Length - 6);
                    var data_split_table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == long.Parse(split_table));
                    Route_DataBase_ID = data_split_table.Route_DataBase_ID;

                }
                using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
                {
                    string sql = string.Format("select * from {0} where ewm='{1}'", tableName, ewm);
                    result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();
                }
            }
            catch { }
            return result;
        }
        #endregion

        #region 动态获取连接
        private DataClassesDataContext GetDynamicDataContext(long rId, out long beginCode, out long endCode, out long createCount, out long saleCount, out string tablename)
        {
            string datasource = "";
            string database = "";
            string username = "";
            string pass = "";
            tablename = "";
            beginCode = 0;
            endCode = 0;
            createCount = 0;
            saleCount = 0;
            DataClassesDataContext result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCode code = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                    if (code == null)
                    {
                        return null;
                    }
                    beginCode = code.StartNum.Value;
                    endCode = code.EndNum.Value;
                    createCount = code.TotalNum.Value;
                    saleCount = code.saleCount.Value;
                    long table_id = code.Route_DataBase_ID.Value;
                    Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
                    if (table == null)
                    {
                        return null;
                    }
                    datasource = table.DataSource;
                    database = table.DataBaseName;
                    username = table.UID;
                    pass = table.PWD;
                    tablename = table.TableName;

                }
                result = GetDataContext(datasource, database, username, pass);
            }
            catch { throw; }
            return result;
        }
        private DataClassesDataContext GetCodeNewDataContext(long rId, out string tablename)
        {
            string datasource = "";
            string database = "";
            string username = "";
            string pass = "";
            tablename = "";
            DataClassesDataContext result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCode code = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                    long table_id = code.Route_DataBase_ID.Value;
                    Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
                    if (table == null)
                    {
                        return null;
                    }
                    datasource = table.DataSource;
                    database = table.DataBaseName;
                    username = table.UID;
                    pass = table.PWD;
                    tablename = table.TableName;

                }
                result = GetDataContext(datasource, database, username, pass);
            }
            catch { throw; }
            return result;
        }
        private DataClassesDataContext GetDynamicDataContext(long Route_DataBase_ID, out string tablename)
        {
            string datasource = "";
            string database = "";
            string username = "";
            string pass = "";
            tablename = "";
            DataClassesDataContext result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    long table_id = Route_DataBase_ID;
                    Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
                    if (table == null)
                    {
                        return null;
                    }
                    datasource = table.DataSource;
                    database = table.DataBaseName;
                    username = table.UID;
                    pass = table.PWD;
                    tablename = table.TableName;

                }
                result = GetDataContext(datasource, database, username, pass);
            }
            catch { throw; }
            return result;
        }
        #endregion

        public List<LinqModel.Material> SearchNameList(long eId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = (from data in dataContext.Material
                                    where data.Enterprise_Info_ID == eId && data.Status == (int)Common.EnumFile.Status.used
                                    orderby data.Material_ID descending
                                    select data).ToList();
                    ClearLinqModel(dataList);
                    return dataList;

                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<LinqModel.Material> GetSysEnterspriseList(List<long> idList)
        {
            List<LinqModel.Material> dataInfoList = new List<Material>();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = (from data in dataContext.Material
                                    where data.Status == (int)Common.EnumFile.Status.used
                                    select data).ToList();
                    foreach (long id in idList)
                    {
                        var temp = dataList.Where(w => w.Enterprise_Info_ID == id).ToList();
                        ClearLinqModel(temp);
                        dataInfoList.AddRange(temp);
                    }

                    return dataInfoList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public LinqModel.RequestCode GetModel(long rId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.RequestCode
                                where d.RequestCode_ID == rId
                                select d).FirstOrDefault();
                    ClearLinqModel(data);
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Enterprise_Info GetMainCode(long eid)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from m in dataContext.Enterprise_Info where m.Enterprise_Info_ID == eid select m).FirstOrDefault();
                    return data;

                }
            }
            catch
            {
                return null;
            }
        }

        public List<Enterprise_FWCode_00> GetCodeList(long rId, int pageIndex, int pageSize)
        {
            string datasource = "";
            string database = "";
            string username = "";
            string pass = "";
            string tablename = "";
            long beginCode = 0;
            long endCode = 0;
            long createCount = 0;
            long saleCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                RequestCode code = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                if (code == null)
                {
                    return null;
                }
                beginCode = code.StartNum.Value;
                endCode = code.EndNum.Value;
                createCount = code.TotalNum.Value;
                saleCount = code.saleCount.Value;
                long table_id = code.Route_DataBase_ID.Value;
                Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
                if (table == null)
                {
                    return null;
                }
                datasource = table.DataSource;
                database = table.DataBaseName;
                username = table.UID;
                pass = table.PWD;
                tablename = table.TableName;
            }
            using (DataClassesDataContext dataContext = GetDataContext(datasource, database, username, pass))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from " + tablename + " where RequestCode_ID=" + rId + " and Enterprise_FWCode_ID>=" + (beginCode + (pageSize * (pageIndex - 1)))
                    + " and Enterprise_FWCode_ID<" + ((beginCode + (pageSize * pageIndex))));
                return dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 查看销售记录
        /// </summary>
        /// <param name="requestCode_ID"></param>
        /// <param name="ewm"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Enterprise_FWCode_00> GetSalesPageList(long requestCode_ID, string ewm, int? pageIndex, int pageSize, out long saleCount)
        {
            string datasource = "";
            string database = "";
            string username = "";
            string pass = "";
            string tablename = "";
            long beginCode = 0;
            long endCode = 0;
            long createCount = 0;
            saleCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                RequestCode code = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == requestCode_ID);
                if (code == null)
                {
                    return null;
                }
                beginCode = code.StartNum.Value;
                endCode = code.EndNum.Value;
                createCount = code.TotalNum.Value;
                saleCount = code.saleCount.Value;
                long table_id = code.Route_DataBase_ID.Value;
                Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
                if (table == null)
                {
                    return null;
                }
                datasource = table.DataSource;
                database = table.DataBaseName;
                username = table.UID;
                pass = table.PWD;
                tablename = table.TableName;
            }
            using (DataClassesDataContext dataContext = GetDataContext(datasource, database, username, pass))
            {
                int pageCount = Convert.ToInt32(saleCount);
                if (!String.IsNullOrEmpty(ewm))
                {
                    //string sql = @"SELECT * FROM View_" + tablename + " where ewm='" + ewm + "' and RequestCode_ID=" + requestCode_ID + " order by SalesTime desc";
                    string sql = @"SELECT * FROM " + tablename + " where ewm='" + ewm.Trim() + "' and RequestCode_ID=" + requestCode_ID + " order by SalesTime desc";

                    var liHasEWM = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                    saleCount = liHasEWM.Count;
                    ClearLinqModel(liHasEWM);
                    return liHasEWM;
                }
                else
                {
                    //string sql = @"SELECT top " + pageSize + " * FROM View_" + tablename + " where status=" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed +
                    //    " and Enterprise_FWCode_ID between " + beginCode + " and " + endCode + " and RequestCode_ID=" + requestCode_ID +
                    //    " and Enterprise_FWCode_ID not in (" +
                    //    "SELECT top " + (pageIndex - 1) * pageSize + " Enterprise_FWCode_ID FROM View_" + tablename + " where status=" + (int)Common.EnumFile.UsingStateCode.NotUsed +
                    //    " and Enterprise_FWCode_ID between " + beginCode + " and " + endCode + " and RequestCode_ID=" + requestCode_ID +
                    //    ")" +
                    //    " order by SalesTime desc";

                    string sql = @"SELECT top " + pageSize + " * FROM " + tablename + " where status=" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed +
                        " and Enterprise_FWCode_ID between " + beginCode + " and " + endCode + " and RequestCode_ID=" + requestCode_ID +
                        " and Enterprise_FWCode_ID not in (" +
                        "SELECT top " + (pageIndex - 1) * pageSize + " Enterprise_FWCode_ID FROM " + tablename + " where status=" + (int)Common.EnumFile.UsingStateCode.HasBeenUsed +
                        " and Enterprise_FWCode_ID between " + beginCode + " and " + endCode + " and RequestCode_ID=" + requestCode_ID +
                        " order by SalesTime desc)" +
                        " order by SalesTime desc";

                    List<Enterprise_FWCode_00> log = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                    ClearLinqModel(log);
                    return log;
                }
            }
        }

        public List<Enterprise_FWCode_00> GetEwmList(long id, int pageIndex, int pageSize)
        {
            string datasource = "";
            string database = "";
            string username = "";
            string pass = "";
            string tablename = "";
            long beginCode = 0;
            long endCode = 0;
            long createCount = 0;
            long saleCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                RequestCode code = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == id);
                if (code == null)
                {
                    return null;
                }
                beginCode = code.StartNum.Value;
                endCode = code.EndNum.Value;
                createCount = code.TotalNum.Value;
                saleCount = code.saleCount.Value;
                long table_id = code.Route_DataBase_ID.Value;
                Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
                if (table == null)
                {
                    return null;
                }
                datasource = table.DataSource;
                database = table.DataBaseName;
                username = table.UID;
                pass = table.PWD;
                tablename = table.TableName;
            }
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext(datasource, database, username, pass))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from " + tablename + " where RequestCode_ID=" + id + " and Enterprise_FWCode_ID>=" + (beginCode + (pageSize * (pageIndex - 1)))
                    + " and Enterprise_FWCode_ID<" + ((beginCode + (pageSize * pageIndex))));
                return dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();
            }
        }

        public List<long> GetEnterpriseInfo(long eId)
        {
            List<long> dataList = null;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    dataList = (from d in dataContext.View_EnterpriseInfoUser
                                where d.PRRU_PlatForm_ID == eId
                                select d.Enterprise_Info_ID).ToList();


                }
            }
            catch (Exception ex)
            {

            }

            return dataList;
        }

        public RetResult ChangeStatus(long rId, long passCount)
        {
            int status = Convert.ToInt32(Common.EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated);
            string para = Common.EnumText.EnumToText(typeof(Common.EnumFile.RequestCodeStatus), status);
            para = para.Substring(0, 2);
            string Msg = para + "失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCode model = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                    LoginInfo pf = Common.Argument.SessCokie.Get;
                    PRRU_PlatForm platForm = dataContext.PRRU_PlatForm.FirstOrDefault(m => m.PRRU_PlatForm_ID == pf.EnterpriseID);
                    var listRequest = dataContext.AuditCodeRecord.Where(m => m.PRRU_PlatForm_ID == platForm.PRRU_PlatForm_ID);
                    var setAudit = dataContext.SetAuditCount.Where(d => d.PRRU_PlatForm_ID == platForm.PRRU_PlatForm_ID);
                    int isSum = 0;
                    PRRU_PlatForm platFormParent = dataContext.PRRU_PlatForm.FirstOrDefault(m => m.PRRU_PlatForm_ID == platForm.Parent_ID);
                    if (platFormParent != null)
                    {
                        int? level = platFormParent.PRRU_PlatFormLevel_ID;
                        if ((int)Common.EnumFile.PlatFormLevel.Service == level)
                        {
                            var allCount = dataContext.SetAuditCount.Where(m => m.PRRU_PlatForm_ID == platForm.PRRU_PlatForm_ID);
                            if (allCount.Count() > 0)
                            {
                                long auditCountAll = allCount.Sum(m => m.AuditCountAll).GetValueOrDefault(0);
                                long auditedCount = dataContext.AuditCodeRecord.Where(m =>
                                    m.PRRU_PlatForm_ID == platForm.PRRU_PlatForm_ID && m.IsSum == 1).Sum(m => m.AuditCount).GetValueOrDefault(0);
                                if (model.Specifications > 0)
                                {
                                    passCount = (model.Specifications * model.TotalNum).Value;
                                }
                                if (auditCountAll >= auditedCount + passCount)//可以审核
                                {
                                    isSum = 1;
                                }
                                else//超过
                                {
                                    if (model.Specifications > 0)
                                    {
                                        Msg = "还能审核" + Math.Floor((double)(auditCountAll / model.Specifications)) + "套,超出了审核数量,不能进行审核！";
                                    }
                                    else
                                    {
                                        Msg = "还能审核" + auditCountAll + "个,超出了审核数量,不能进行审核！";
                                    }
                                }
                            }
                        }
                    }
                    model.TotalNum = passCount;
                    model.Status = status;

                    #region 添加审核记录
                    AuditCodeRecord record = new AuditCodeRecord();
                    record.PRRU_PlatForm_ID = platForm.PRRU_PlatForm_ID;
                    record.RequestCode_ID = model.RequestCode_ID;
                    if (model.Specifications > 0)
                    {
                        record.AuditCount = model.Specifications * model.TotalNum;
                    }
                    else
                    {
                        record.AuditCount = model.TotalNum;
                    }
                    record.AuditTime = DateTime.Now;
                    record.EnterpriseID = model.Enterprise_Info_ID;
                    record.IsRead = 0;
                    record.IsSum = isSum;
                    #endregion

                    dataContext.SubmitChanges();
                    Msg = para + "成功！";
                    CmdError = CmdResultError.NONE;
                }
            }
            catch { }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取申请码记录ID
        /// </summary>
        /// <param name="bName">批次名称</param>
        /// <returns></returns>
        public RequestCode GetRequestID(string bName)
        {
            RequestCode result = new RequestCode();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.RequestCode.FirstOrDefault();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeDAL.GetRequestID()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        ///  生成码方法new20170217
        ///  20180803新加简码规则
        ///  20191101医疗器械生成码修改
        /// </summary>
        /// <param name="ObjRequestCode">申请码表</param>
        /// <param name="setModel">配置码表</param>
        /// <returns></returns>
        public RetResult Generate(RequestCode ObjRequestCode, RequestCodeSetting setModel, string scleixing)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    bool isContinue = true;
                    long allCount = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID).RequestCodeCount ?? 0;
                    Enterprise_Info enInfo = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID);
                    if (allCount != 0 && enInfo.OverDraftCount != -1)
                    {
                        //20170817修改生成码
                        if (enInfo.UsedCodeCount + ObjRequestCode.TotalNum + enInfo.OverDraftCount > allCount)
                        {
                            string count = "0";
                            if ((allCount - enInfo.UsedCodeCount) > 0)
                            {
                                count = (allCount - enInfo.UsedCodeCount + enInfo.OverDraftCount).ToString();
                            }
                            Ret.SetArgument(CmdResultError.EXCEPTION, "申请二维码失败！超过代理商设置最大申请数量，最多还可申请" + count + "个码！", "申请二维码失败！超过监管部门设置最大申请数量，最多还可申请" + count + "个码！");
                            isContinue = false;
                        }
                    }
                    if (isContinue)
                    {
                        Category category = dataContext.Category.Where(p => p.MaterialID == ObjRequestCode.Material_ID &&
                            p.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                        MaterialDAL dal = new MaterialDAL();
                        Material material = dal.GetModel(ObjRequestCode.Material_ID.Value);
                        if (category != null)
                        {
                            int index = dataContext.RequestCodeSetting.Where(m => m.EnterpriseId == setModel.EnterpriseId
                           && m.SetDate > Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01 00:00:00"))).Count() + 1;
                            string batchName = DateTime.Now.ToString("yyyy") + index;
                            while (dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == setModel.EnterpriseId && m.BatchName == batchName) != null)
                            {
                                batchName = DateTime.Now.ToString("yyyy") + (index++);
                            }
                            #region 存储前缀(码其中的一个节点)
                            string qzq = new BinarySystem36().gen36No(Convert.ToInt32(DateTime.Now.ToString("yyyy").Substring(2, 2)), 2) +
                                new BinarySystem36().gen36No(Convert.ToInt32(DateTime.Now.ToString("MM")), 2) +
                                new BinarySystem36().gen36No(Convert.ToInt32(DateTime.Now.ToString("dd")), 2);
                            int ph = 0;
                            string sph = "";
                            int tempCount = dataContext.RequestCode.Where(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID &&
                                m.Material_ID == ObjRequestCode.Material_ID && m.CodeOfType == ObjRequestCode.CodeOfType &&
                                m.RequestDate == ObjRequestCode.RequestDate).Count();
                            if (tempCount > 0)
                            {
                                //批次号为4位
                                ph = tempCount + 1;
                                sph = new BinarySystem36().gen36No(ph, 2);
                                ObjRequestCode.FixedCode = qzq + sph;
                            }
                            else
                            {
                                ph = 1;
                                sph = new BinarySystem36().gen36No(ph, 2);
                                ObjRequestCode.FixedCode = qzq + sph;
                            }
                            #endregion
                            ObjRequestCode.StartNum = 1;
                            ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
                            int status = Convert.ToInt32(Common.EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated);
                            string para = Common.EnumText.EnumToText(typeof(Common.EnumFile.RequestCodeStatus), status);
                            ObjRequestCode.Status = status;
                            ObjRequestCode.IsLocal = (int)Common.EnumFile.Islocal.cloud;
                            #region 调用批量注册品类接口
                            string resultJK = BaseDataDAL.ReCategory(enInfo.MainCode, category.CategoryID.ToString(),
                                category.CategoryCode, material.MaterialName,
                                ObjRequestCode.StartNum.Value, ObjRequestCode.EndNum.Value, ObjRequestCode.BZSpecType.Value, ObjRequestCode.ShengChanPH, ObjRequestCode.FixedCode, ObjRequestCode.RequestDate,
                                Convert.ToDateTime(ObjRequestCode.YouXiaoDate), Convert.ToDateTime(ObjRequestCode.ShiXiaoDate));
                            JsonObject jsonObject = new JsonObject(resultJK);
                            resultJK = jsonObject["result_code"].Value;
                            #endregion
                            if (resultJK == "1")
                            {
                                ObjRequestCode.IDCodeBatchNo = jsonObject["data"].Value;
                                dataContext.RequestCode.InsertOnSubmit(ObjRequestCode);
                                dataContext.SubmitChanges();
                                setModel.BrandID = material.Brand_ID;
                                setModel.MaterialID = ObjRequestCode.Material_ID;
                                setModel.RequestID = ObjRequestCode.RequestCode_ID;
                                setModel.BatchName = batchName;
                                setModel.beginNum = "1";
                                setModel.endNum = ObjRequestCode.TotalNum.ToString();
                                setModel.beginCode = 1;
                                setModel.endCode = ObjRequestCode.TotalNum;
                                dataContext.RequestCodeSetting.InsertOnSubmit(setModel);
                                //更新企业表中已用码的数量
                                enInfo.UsedCodeCount = enInfo.UsedCodeCount + ObjRequestCode.TotalNum;
                                //为统计表添加数据
                                HomeDataStatis homeData = new HomeDataStatis();
                                homeData.EnterpriseID = ObjRequestCode.Enterprise_Info_ID;
                                homeData.RequestCodeTimes = 1;
                                homeData.RequestCodeCount = ObjRequestCode.TotalNum;
                                ComplaintDAL complaintDal = new ComplaintDAL();
                                RetResult result = complaintDal.Update(homeData);
                                dataContext.SubmitChanges();
                                Ret.SetArgument(CmdResultError.NONE, "生成成功！", "生成成功！");
                            }
                            else
                            {
                                Ret.SetArgument(CmdResultError.EXCEPTION, jsonObject["result_msg"].Value, jsonObject["result_msg"].Value);
                                return Ret; ;
                            }
                        }
                        else
                        {
                            Ret.SetArgument(CmdResultError.EXCEPTION, "没有找到对应的品类编码！", "没有找到对应的品类编码！");
                            return Ret;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败，请检查网络！", "数据库连接失败，请检查网络！");
            }
            return Ret;
        }

        /// <summary>
        /// 生成包材码
        /// </summary>
        /// <param name="ObjRequestCode">包材信息</param>
        /// <returns></returns>
        public RetResult GeneratePackCode(LinqModel.RequestCode ObjRequestCode)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    bool isContinue = true;
                    long allCount = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID).RequestCodeCount ?? 0;
                    Enterprise_Info enInfo = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID);
                    if (allCount != 0 && enInfo.OverDraftCount != -1)
                    {
                        //20170817修改生成码
                        if (enInfo.UsedCodeCount + ObjRequestCode.TotalNum + enInfo.OverDraftCount > allCount)
                        {
                            string count = "0";
                            if ((allCount - enInfo.UsedCodeCount) > 0)
                            {
                                count = (allCount - enInfo.UsedCodeCount + enInfo.OverDraftCount).ToString();
                            }
                            Ret.SetArgument(CmdResultError.EXCEPTION, "申请二维码失败！超过代理商设置最大申请数量，最多还可申请" + count + "个码！", "申请二维码失败！超过监管部门设置最大申请数量，最多还可申请" + count + "个码！");
                            isContinue = false;
                        }
                    }
                    if (isContinue)
                    {
                        Category category = dataContext.Category.Where(p => p.MaterialID == ObjRequestCode.Material_ID &&
                            p.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                        MaterialDAL dal = new MaterialDAL();
                        Material material = dal.GetModel(ObjRequestCode.Material_ID.Value);
                        if (category != null)
                        {
                            //20180803新加简码
                            if (ObjRequestCode.CodeOfType == (int)Common.EnumFile.CodeOfType.SCode)
                            {
                                //旧产品不支持生成简码
                                if (string.IsNullOrEmpty(enInfo.TraceEnMainCode) || string.IsNullOrEmpty(material.Material_Code)
                                    || enInfo.TraceEnMainCode.Length != 4 || material.Material_Code.Length != 3)
                                {
                                    Ret.SetArgument(CmdResultError.EXCEPTION, "该产品不支持生成简码！", "该产品不支持生成简码！");
                                    return Ret;
                                }
                                int ph = 0;
                                string sph = "";
                                int tempCount = dataContext.RequestCode.Where(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID &&
                                    m.Material_ID == ObjRequestCode.Material_ID && m.CodeOfType == ObjRequestCode.CodeOfType).Count();
                                if (tempCount > 0)
                                {
                                    ph = tempCount + 1;
                                    sph = new BinarySystem36().gen36No(ph, 4);
                                    ObjRequestCode.FixedCode = enInfo.TraceEnMainCode + material.Material_Code + sph;
                                }
                                else
                                {
                                    ph = 1;
                                    sph = new BinarySystem36().gen36No(ph, 4);
                                    ObjRequestCode.FixedCode = enInfo.TraceEnMainCode + material.Material_Code + sph;
                                }
                            }
                            else
                            {
                                ObjRequestCode.FixedCode = category.CategoryIDcode + DateTime.Now.ToString("yyMMdd");
                            }
                        }
                        else
                        {
                            Ret.SetArgument(CmdResultError.NO_RESULT, "没有找到对应的品类编码！", "没有找到对应的品类编码！");
                            return Ret;
                        }
                        if (ObjRequestCode.CodeOfType == (int)Common.EnumFile.CodeOfType.SCode)
                        {
                            ObjRequestCode.StartNum = 1;
                            ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
                        }
                        else
                        {
                            //查找库里是否有相同固定节点的生成码数据
                            RequestCode data = dataContext.RequestCode.Where(p => p.FixedCode ==
                                ObjRequestCode.FixedCode.Trim() && p.Type == Convert.ToInt32(ObjRequestCode.Type)).OrderByDescending(p => p.adddate).FirstOrDefault();
                            if (data == null)
                            {
                                ObjRequestCode.StartNum = 1;
                                ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
                            }
                            else
                            {
                                ObjRequestCode.StartNum = data.EndNum + 1;
                                ObjRequestCode.EndNum = data.EndNum + ObjRequestCode.TotalNum;
                            }
                        }
                        int status = Convert.ToInt32(Common.EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated);
                        string para = Common.EnumText.EnumToText(typeof(Common.EnumFile.RequestCodeStatus), status);
                        ObjRequestCode.Status = status;
                        ObjRequestCode.IsLocal = (int)Common.EnumFile.Islocal.cloud;
                        dataContext.RequestCode.InsertOnSubmit(ObjRequestCode);
                        dataContext.SubmitChanges();
                        RequestCodeSetting setting = new RequestCodeSetting { EnterpriseId = ObjRequestCode.Enterprise_Info_ID, Count = ObjRequestCode.TotalNum.Value, endCode = ObjRequestCode.EndNum, beginCode = ObjRequestCode.StartNum, MaterialID = ObjRequestCode.Material_ID, SetDate = DateTime.Now, RequestCodeType = ObjRequestCode.RequestCodeType, RequestID = ObjRequestCode.RequestCode_ID };
                        dataContext.RequestCodeSetting.InsertOnSubmit(setting);
                        dataContext.SubmitChanges();
                        //更新企业表中已用码的数量
                        enInfo.UsedCodeCount = enInfo.UsedCodeCount + ObjRequestCode.TotalNum;
                        Material materialTemp = dataContext.Material.FirstOrDefault(p => p.Material_ID == ObjRequestCode.Material_ID);
                        if (materialTemp != null)
                        {
                            materialTemp.PackCount = ObjRequestCode.PackCount;
                        }
                        dataContext.SubmitChanges();
                        //为统计表添加数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = ObjRequestCode.Enterprise_Info_ID;
                        homeData.RequestCodeTimes = 1;
                        homeData.RequestCodeCount = ObjRequestCode.TotalNum;
                        ComplaintDAL complaintDal = new ComplaintDAL();
                        RetResult result = complaintDal.Update(homeData);
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "生成成功！", "生成成功！");
                        //Ret.SetArgument(CmdResultError.NONE, "生成成功！请您耐心等待二维码审核。", "生成成功！请您耐心等待二维码审核。");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败，请检查网络！", "数据库连接失败，请检查网络！");
            }

            return Ret;
        }

        /// <summary>
        /// 追溯码管理详情查看码
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="sId">设置表ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">条数</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public List<Enterprise_FWCode_00> GetSettingCode(string ewm, long sId, int status, int pageIndex, int pageSize, out long totalCount)
        {
            List<Enterprise_FWCode_00> result = new List<Enterprise_FWCode_00>();
            RequestCodeSetting settingCode = new RequestCodeSetting();
            RequestCode rCode = new RequestCode();
            long pageCount = 0;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContextWeb = GetDataContext())
            {
                settingCode = dataContextWeb.RequestCodeSetting.FirstOrDefault(m => m.ID == sId);
                long rId = settingCode.RequestID;
                rCode = dataContextWeb.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                long beginCode = settingCode.beginCode.Value;
                long endCode = settingCode.endCode.Value;
                totalCount = settingCode.Count;
                long createCount = rCode.TotalNum.GetValueOrDefault(0);
                long saleCount = rCode.saleCount.GetValueOrDefault(0);
                pageCount = totalCount;
                long minusCount = settingCode.beginCode.Value - rCode.StartNum.Value;
                string tablename = "";
                using (DataClassesDataContext dataContext = GetCodeNewDataContext(rId, out tablename))
                {
                    try
                    {
                        dataContext.CommandTimeout = 600;
                        if (!String.IsNullOrEmpty(ewm))
                        {
                            string sql = "select top 1 * from " + tablename + " where RequestCode_ID=" + rId
                                + "  and ewm='" + ewm.Trim() + "'";
                            result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                            ClearLinqModel(result);
                            pageCount = result.Count();
                        }
                        else
                        {
                            StringBuilder strSql = new StringBuilder();
                            if (status <= 0)//没有选择状态
                            {
                                strSql.Append("select top " + totalCount + " * from " + tablename + " where RequestCode_ID=" + rId
                                    + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                    + " Enterprise_FWCode_ID  from " + tablename + " where RequestCode_ID=" + rId
                                    + " order by Enterprise_FWCode_ID  )  ");
                                if (settingCode.BathPartType == (int)Common.EnumFile.BatchPartType.Custom && settingCode.BatchType == 1)
                                {
                                    strSql.Append(" and RequestSetID is null ");
                                }
                                strSql.Append("  order by Enterprise_FWCode_ID ");
                            }
                            else//选择状态
                            {
                                strSql.Append("select top " + totalCount + " * from " + tablename + " where RequestCode_ID=" + rId
                                    + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                    + " Enterprise_FWCode_ID  from " + tablename + " where RequestCode_ID=" + rId
                                    + " order by Enterprise_FWCode_ID  ) and Status=" + status);
                                if (settingCode.BathPartType == (int)Common.EnumFile.BatchPartType.Custom && settingCode.BatchType == 1)
                                {
                                    strSql.Append(" and RequestSetID is null ");
                                }
                                strSql.Append("  order by Enterprise_FWCode_ID ");
                            }
                            result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();
                            pageCount = result.Count();
                            result = result.Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                            ClearLinqModel(result);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            totalCount = pageCount;
            return result;
        }

        /// <summary>
        /// 下载产品二维码
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="sId">表识ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public List<Enterprise_FWCode_00> GetSettingCodeTxt(string ewm, long sId, int status, int pageIndex, int pageSize, out long totalCount)
        {
            List<Enterprise_FWCode_00> result = new List<Enterprise_FWCode_00>();
            RequestCodeSetting settingCode = new RequestCodeSetting();
            RequestCode rCode = new RequestCode();
            long pageCount = 0;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContextWeb = GetDataContext())
            {
                settingCode = dataContextWeb.RequestCodeSetting.FirstOrDefault(m => m.ID == sId);
                long rId = settingCode.RequestID;
                rCode = dataContextWeb.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                long sbeginCode = settingCode.beginCode.Value;
                long sendCode = settingCode.endCode.Value;
                long rbeginCode = rCode.StartNum.Value;
                long rendCode = rCode.EndNum.Value;
                long beginCode = rbeginCode + sbeginCode - 1;
                long endCode = rbeginCode + sendCode - 1;
                totalCount = settingCode.Count;
                long minusCount = settingCode.beginCode.Value - rCode.StartNum.Value;
                int createCount = 0;
                long saleCount = 0;
                pageCount = totalCount;
                string tablename = "";
                using (DataClassesDataContext dataContext = GetCodeNewDataContext(rId, out tablename))
                {
                    try
                    {
                        dataContext.CommandTimeout = 600;
                        if (!String.IsNullOrEmpty(ewm))
                        {
                            string sql = "select top 1 * from " + tablename + " where RequestCode_ID=" + rId + "and Enterprise_FWCode_ID>=" + beginCode + "and Enterprise_FWCode_ID<=" + endCode + " and ewm='" + ewm.Trim() + "'";
                            result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).ToList();
                            ClearLinqModel(result);
                            pageCount = result.Count();
                        }
                        else
                        {
                            StringBuilder strSql = new StringBuilder();
                            if (status <= 0)//没有选择状态
                            {
                                strSql.Append("select top " + totalCount + " * from " + tablename + " where RequestCode_ID=" + rId
                                    + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                    + " Enterprise_FWCode_ID  from Enterprise_FWCode_00 where RequestCode_ID=" + rId + " order by Enterprise_FWCode_ID  ) order by Enterprise_FWCode_ID ");
                            }
                            else//选择状态
                            {
                                strSql.Append("select top " + PageSize + " * from " + tablename + " where 1=1");
                                strSql.Append(" and Enterprise_FWCode_ID not in(select top " + (PageSize * (pageIndex - 1)) + " Enterprise_FWCode_ID from " + tablename + " where RequestCode_ID=" + rId);
                                strSql.Append(" and Status=" + status);
                                strSql.Append(" order by UseTime desc");
                                strSql.Append(")");
                                strSql.Append(" and RequestCode_ID=" + rId + "and Enterprise_FWCode_ID>=" + beginCode + "and Enterprise_FWCode_ID<=" + endCode + " and Status=" + status);
                                strSql.Append(" order by UseTime desc");
                            }

                            if (status == (int)Common.EnumFile.UsingStateCode.HasBeenUsed)
                            {
                                pageCount = Convert.ToInt32(saleCount);
                            }
                            else if (status == (int)Common.EnumFile.UsingStateCode.NotUsed)
                            {
                                pageCount = createCount - Convert.ToInt32(saleCount);
                            }
                            else if (status == 0)
                            {
                                pageCount = totalCount;
                            }
                            result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();
                            ClearLinqModel(result);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            totalCount = pageCount;
            return result;
        }

        /// <summary>
        /// 服务执行存储过程
        /// </summary>
        /// <returns></returns>
        public RetResult ServiceGenerated()
        {
            string Msg = "生成失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.CommandTimeout = 0;
                    int dataCount = dataContext.GeneratedCode();
                    Msg = "生成成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                Msg = "生成失败！";
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 服务执行报表存储过程
        /// </summary>
        /// <returns></returns>
        public void GenReport()
        {
            string Msg = "生成失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.CommandTimeout = 0;
                    int dataCount = dataContext.CreateOrderCheck();
                    Msg = "生成成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                Msg = "生成失败！";
                error = CmdResultError.EXCEPTION;
            }
            WriteLog.WriteReportLog("时间：" + DateTime.Now.ToString() + ",结果:" + Msg);
        }

        #region 2018-09-04新加
        /// <summary>
        ///  20180803新加简码规则
        ///  (在此基础上修改为模板4添加图片而添加)
        ///  2018-09-04新增两个模板为模板4添加图片和链接
        /// </summary>
        /// <param name="ObjRequestCode">申请码表</param>
        /// <param name="setModel">配置码表</param>
        /// <returns></returns>
        public RetResult GenerateMuBan(RequestCode ObjRequestCode, RequestCodeSetting setModel, RequestCodeSettingMuBan mubanModel, string scleixing)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    bool isContinue = true;
                    long allCount = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID).RequestCodeCount ?? 0;
                    Enterprise_Info enInfo = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID);
                    if (allCount != 0 && enInfo.OverDraftCount != -1)
                    {
                        //20170817修改生成码
                        if (enInfo.UsedCodeCount + ObjRequestCode.TotalNum + enInfo.OverDraftCount > allCount)
                        {
                            string count = "0";
                            if ((allCount - enInfo.UsedCodeCount) > 0)
                            {
                                count = (allCount - enInfo.UsedCodeCount + enInfo.OverDraftCount).ToString();
                            }
                            Ret.SetArgument(CmdResultError.EXCEPTION, "申请二维码失败！超过代理商设置最大申请数量，最多还可申请" + count + "个码！", "申请二维码失败！超过监管部门设置最大申请数量，最多还可申请" + count + "个码！");
                            isContinue = false;
                        }
                    }
                    if (isContinue)
                    {
                        Category category = dataContext.Category.Where(p => p.MaterialID == ObjRequestCode.Material_ID &&
                            p.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                        MaterialDAL dal = new MaterialDAL();
                        Material material = dal.GetModel(ObjRequestCode.Material_ID.Value);
                        if (category != null)
                        {
                            if (ObjRequestCode.Type == (int)Common.EnumFile.GenCodeType.pesticides)
                            {
                                if (string.IsNullOrEmpty(category.SCategoryIDcode) || string.IsNullOrEmpty(material.NYZhengHao)
                                    || material.NYZhengHao.Length < 6
                                    || material.NYType <= 0 || string.IsNullOrEmpty(scleixing) || string.IsNullOrEmpty(ObjRequestCode.GuiGe))
                                {
                                    Ret.SetArgument(CmdResultError.EXCEPTION, "请重新维护农药产品相关信息！", "请重新维护农药产品相关信息！");
                                    return Ret;
                                }
                                ObjRequestCode.FixedCode = material.NYType.ToString() + material.NYZhengHao.Substring(material.NYZhengHao.Length - 6, 6)
                                    + scleixing
                                    + ObjRequestCode.GuiGe + category.SCategoryIDcode + ObjRequestCode.RequestDate.Value.ToString("yyMMdd");
                                //查找库里是否有相同固定节点的生成码数据
                                RequestCode data = dataContext.RequestCode.Where(p => p.FixedCode.Substring(0, p.FixedCode.Length - 3) ==
                                    ObjRequestCode.FixedCode.Trim() && p.Type == Convert.ToInt32(ObjRequestCode.Type)).OrderByDescending(p => p.adddate).FirstOrDefault();
                                ObjRequestCode.StartNum = 1;
                                ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
                                setModel.beginCode = 1;
                                setModel.endCode = setModel.Count;
                                if (data == null)
                                {
                                    ObjRequestCode.FixedCode = ObjRequestCode.FixedCode + "001";
                                }
                                else
                                {
                                    int count = dataContext.RequestCode.Where(p => p.FixedCode.Substring(0, p.FixedCode.Length - 3) ==
                                    ObjRequestCode.FixedCode.Trim() && p.Type == Convert.ToInt32(ObjRequestCode.Type)).OrderByDescending(p => p.adddate).ToList().Count();
                                    ObjRequestCode.FixedCode = ObjRequestCode.FixedCode + (count + 1).ToString().PadLeft(3, '0');
                                }
                            }
                            else
                            {
                                //20180803新加简码
                                //企业主码：4位+产品编码3位+批次号4位
                                if (ObjRequestCode.CodeOfType == (int)Common.EnumFile.CodeOfType.SCode)
                                {
                                    //旧产品不支持生成简码
                                    if (string.IsNullOrEmpty(enInfo.TraceEnMainCode) || string.IsNullOrEmpty(material.Material_Code)
                                        || enInfo.TraceEnMainCode.Length != 4 || material.Material_Code.Length != 3)
                                    {
                                        Ret.SetArgument(CmdResultError.EXCEPTION, "该产品不支持生成简码！", "该产品不支持生成简码！");
                                        return Ret;
                                    }
                                    int ph = 0;
                                    string sph = "";
                                    int tempCount = dataContext.RequestCode.Where(m => m.Enterprise_Info_ID == ObjRequestCode.Enterprise_Info_ID &&
                                        m.Material_ID == ObjRequestCode.Material_ID && m.CodeOfType == ObjRequestCode.CodeOfType).Count();
                                    if (tempCount > 0)
                                    {
                                        //批次号为4位
                                        ph = tempCount + 1;
                                        sph = new BinarySystem36().gen36No(ph, 4);
                                        ObjRequestCode.FixedCode = enInfo.TraceEnMainCode + material.Material_Code + sph;
                                    }
                                    else
                                    {
                                        ph = 1;
                                        sph = new BinarySystem36().gen36No(ph, 4);
                                        ObjRequestCode.FixedCode = enInfo.TraceEnMainCode + material.Material_Code + sph;
                                    }
                                    ObjRequestCode.StartNum = 1;
                                    ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
                                    setModel.beginCode = 1;
                                    setModel.endCode = setModel.Count;
                                }
                                else
                                {
                                    ObjRequestCode.FixedCode = category.CategoryIDcode + ObjRequestCode.RequestDate.Value.ToString("yyMMdd");
                                    //查找库里是否有相同固定节点的生成码数据
                                    RequestCode data = dataContext.RequestCode.Where(p => p.FixedCode ==
                                        ObjRequestCode.FixedCode.Trim() && p.Type == Convert.ToInt32(ObjRequestCode.Type)).OrderByDescending(p => p.adddate).FirstOrDefault();
                                    if (data == null)
                                    {
                                        ObjRequestCode.StartNum = 1;
                                        ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
                                        setModel.beginCode = 1;
                                        setModel.endCode = setModel.Count;
                                    }
                                    else
                                    {
                                        ObjRequestCode.StartNum = data.EndNum + 1;
                                        ObjRequestCode.EndNum = data.EndNum + ObjRequestCode.TotalNum;
                                        setModel.beginCode = data.EndNum + 1;
                                        setModel.endCode = data.EndNum + setModel.Count;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Ret.SetArgument(CmdResultError.EXCEPTION, "没有找到对应的品类编码！", "没有找到对应的品类编码！");
                            return Ret;
                        }
                        int index = dataContext.RequestCodeSetting.Where(m => m.EnterpriseId == setModel.EnterpriseId).Count() + 1;
                        string batchName = DateTime.Now.ToString("yyyy") + index;
                        while (dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == setModel.EnterpriseId && m.BatchName == batchName) != null)
                        {
                            batchName = "yyyy" + (index++);
                        }
                        int status = Convert.ToInt32(Common.EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated);
                        string para = Common.EnumText.EnumToText(typeof(Common.EnumFile.RequestCodeStatus), status);
                        ObjRequestCode.Status = status;
                        ObjRequestCode.IsLocal = (int)Common.EnumFile.Islocal.cloud;
                        dataContext.RequestCode.InsertOnSubmit(ObjRequestCode);
                        dataContext.SubmitChanges();
                        setModel.BrandID = material.Brand_ID;
                        setModel.MaterialID = ObjRequestCode.Material_ID;
                        setModel.RequestID = ObjRequestCode.RequestCode_ID;
                        setModel.BatchName = batchName;
                        dataContext.RequestCodeSetting.InsertOnSubmit(setModel);
                        //更新企业表中已用码的数量
                        enInfo.UsedCodeCount = enInfo.UsedCodeCount + ObjRequestCode.TotalNum;
                        //为统计表添加数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = ObjRequestCode.Enterprise_Info_ID;
                        homeData.RequestCodeTimes = 1;
                        homeData.RequestCodeCount = ObjRequestCode.TotalNum;
                        ComplaintDAL complaintDal = new ComplaintDAL();
                        RetResult result = complaintDal.Update(homeData);
                        dataContext.SubmitChanges();
                        //2018-09-04新加模板4图片和图片链接
                        mubanModel.BatchName = setModel.BatchName;
                        mubanModel.RequestCodeSettingID = setModel.ID;
                        dataContext.RequestCodeSettingMuBan.InsertOnSubmit(mubanModel);
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "生成成功！", "生成成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败，请检查网络！", "数据库连接失败，请检查网络！");
            }
            return Ret;
        }
        #endregion
        #region 医疗器械验证生成码是生产批号是否重复
        public RetResult YanZhengPH(long eid, int bzSpecType, long mid, string shengchanPH)
        {
            string Msg = "该产品的生产批号已存在，请重新输入！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    if (!string.IsNullOrEmpty(shengchanPH))
                    {
                        //RequestCode temprc = dataContext.RequestCode.FirstOrDefault(m => m.Enterprise_Info_ID == eid &&
                        //    m.Material_ID == mid && m.ShengChanPH == shengchanPH && m.BZSpecType == bzSpecType);
                        RequestCode temprc = dataContext.RequestCode.FirstOrDefault(m => m.Enterprise_Info_ID == eid && m.ShengChanPH == shengchanPH);
                        if (temprc != null)
                        {
                            Msg = "该产品的生产批号已存在，请重新输入！";
                            error = CmdResultError.EXCEPTION;
                            Ret.SetArgument(error, Msg, Msg);
                            return Ret;
                        }
                    }
                    dataContext.CommandTimeout = 0;
                    int dataCount = dataContext.GeneratedCode();
                    Msg = "成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                Msg = "该产品的生产批号已存在，请重新输入！";
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #endregion

        #region 20200609修改追溯配置信息那预览码效果显示的码为第一个码
        public Enterprise_FWCode_00 GetCodeModel(long sId, out long totalCount)
        {
            Enterprise_FWCode_00 result = new Enterprise_FWCode_00();
            RequestCodeSetting settingCode = new RequestCodeSetting();
            RequestCode rCode = new RequestCode();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContextWeb = GetDataContext())
            {
                settingCode = dataContextWeb.RequestCodeSetting.FirstOrDefault(m => m.ID == sId);
                long rId = settingCode.RequestID;
                rCode = dataContextWeb.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                long beginCode = settingCode.beginCode.Value;
                long endCode = settingCode.endCode.Value;
                long createCount = rCode.TotalNum.GetValueOrDefault(0);
                long saleCount = rCode.saleCount.GetValueOrDefault(0);
                long minusCount = settingCode.beginCode.Value - rCode.StartNum.Value;
                totalCount = settingCode.Count;
                string tablename = "";
                using (DataClassesDataContext dataContext = GetCodeNewDataContext(rId, out tablename))
                {
                    try
                    {
                        dataContext.CommandTimeout = 600;
                        StringBuilder strSql = new StringBuilder(); strSql.Append("select top " + totalCount + " * from " + tablename + " where RequestCode_ID=" + rId
                                 + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                 + " Enterprise_FWCode_ID  from " + tablename + " where RequestCode_ID=" + rId
                                 + " order by Enterprise_FWCode_ID  )  ");
                        if (settingCode.BathPartType == (int)Common.EnumFile.BatchPartType.Custom && settingCode.BatchType == 1)
                        {
                            strSql.Append(" and RequestSetID is null ");
                        }
                        strSql.Append("  order by Enterprise_FWCode_ID ");
                        result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).FirstOrDefault();
                        ClearLinqModel(result);
                    }
                    catch (Exception ex) { }
                }
            }
            return result;
        }
        #endregion

        #region 20210421打码客户端注册PI调取追溯接口
        /// <summary>
        /// 添加生成码记录
        /// </summary>
        /// <param name="rModel"></param>
        /// <param name="sModel"></param>
        /// <param name="materialName"></param>
        /// <returns></returns>
        public RetResult AddPIInfo(RequestCode rModel, RequestCodeSetting sModel, string materialName)
        {
            Ret.Msg = "添加失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
					Material ma = dataContext.Material.FirstOrDefault(m => m.Enterprise_Info_ID == rModel.Enterprise_Info_ID && m.MaterialName == materialName && m.Status == (int)Common.EnumFile.Status.used);
                    WriteLog.WriteErrorLog("-------------------" + ma.Enterprise_Info_ID + "------------------------------");
                    WriteLog.WriteErrorLog("2799Material 查询");
                    if (ma != null)
                    {
                        sModel.MaterialID = ma.Material_ID;
                        rModel.Material_ID = ma.Material_ID;
                    }
                    else
                    {
                        Ret.Msg = "没有找到产品ID！";
                        Ret.CmdError = CmdResultError.EXCEPTION;
                        Ret.Code = -1;
                        Ret.id = 0;
                        return Ret;
                    }
                    MaterialDI maDI=null;
                    if (rModel.CodingClientType == 1)
                    {
                       maDI  = dataContext.MaterialDI.FirstOrDefault(m => m.EnterpriseID == rModel.Enterprise_Info_ID && m.MaterialUDIDI == rModel.FixedCode);
                       WriteLog.WriteErrorLog("2817MaterialDI 查询");
                    }
                    else
                    {
                        maDI = dataContext.MaterialDI.FirstOrDefault(m => m.EnterpriseID == rModel.Enterprise_Info_ID && m.GSIDI == rModel.FixedCode);
                        WriteLog.WriteErrorLog("2822MaterialDI 查询");
                    }
                    if (maDI != null)
                    {
                        sModel.MaterialXH = maDI.MaterialXH;
                        rModel.MaterialXH = maDI.MaterialXH;
                    }
                    else
                    {
                        Ret.Msg = "没有找到产品DI信息！";
                        Ret.CmdError = CmdResultError.EXCEPTION;
                        Ret.Code = -2;
                        Ret.id = 0;
                        return Ret;
                    }
                    dataContext.RequestCode.InsertOnSubmit(rModel);
                    WriteLog.WriteErrorLog("2838RequestCode 新增");
                    dataContext.SubmitChanges();
                    sModel.RequestID = rModel.RequestCode_ID;
                    //未完待续
                    int index = dataContext.RequestCodeSetting.Where(m => m.EnterpriseId == rModel.Enterprise_Info_ID
                    && m.SetDate > Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01 00:00:00"))).Count() + 1;
                    WriteLog.WriteErrorLog("2843RequestCodeSetting 查询");
                    string batchName = DateTime.Now.ToString("yyyy") + index;
                    while (dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == rModel.Enterprise_Info_ID && m.BatchName == batchName) != null)
                    {
                        batchName = DateTime.Now.ToString("yyyy") + (index++);
                    }
                    sModel.BatchName = batchName;
                    dataContext.RequestCodeSetting.InsertOnSubmit(sModel);
                    dataContext.SubmitChanges();
                    WriteLog.WriteErrorLog("2854 RequestCodeSetting 新增");
                    //为统计表添加数据
                    if (rModel.TotalNum > 0)
                    {
                        Enterprise_Info enInfo = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == rModel.Enterprise_Info_ID).FirstOrDefault();
                        WriteLog.WriteErrorLog("2858 Enterprise_Info 查询");
                        //为统计表添加数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = rModel.Enterprise_Info_ID;
                        homeData.RequestCodeTimes = 1;
                        homeData.RequestCodeCount = rModel.TotalNum;
                        ComplaintDAL complaintDal = new ComplaintDAL();
                        RetResult result = complaintDal.Update(homeData);
                        WriteLog.WriteErrorLog("2866 complaint 更新");
                        //更新企业表中已用码的数量
                        if (enInfo != null)
                        {
                            if (enInfo.RequestCodeCount > rModel.TotalNum)
                            {
                                enInfo.RequestCodeCount = enInfo.RequestCodeCount - rModel.TotalNum;
                            }
                            else
                            {
                                enInfo.RequestCodeCount = 0;
                                enInfo.OverDraftCount = enInfo.RequestCodeCount + enInfo.OverDraftCount - rModel.TotalNum;
                            }
                            if (enInfo.UsedCodeCount == null)
                            {
                                enInfo.UsedCodeCount = rModel.TotalNum;
                            }
                            else
                            {
                                enInfo.UsedCodeCount = enInfo.UsedCodeCount + rModel.TotalNum;
                            }
                        }
                    }
                    dataContext.SubmitChanges();
                    WriteLog.WriteErrorLog("2890 Enterprise_Info 更新");
                    Ret.Msg = "添加成功！";
                    Ret.CmdError = CmdResultError.NONE;
                    Ret.Code = 1;
                    Ret.id = rModel.RequestCode_ID;
                }
                catch (Exception ex)
                {
                    Ret.Msg = "异常！";
                    Ret.CmdError = CmdResultError.EXCEPTION;
                    Ret.Code = 0;
                    Ret.id = 0;
                    string errData = "RequestCode.AddPIInfo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            WriteLog.WriteErrorLog("-------------------结束了------------------------------");
            return Ret;
        }

        /// <summary>
        /// 获取生成码
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<RequestCode> GetRequestCodeList(long enterpriseId, string date)
        {
            List<RequestCode> list = new List<RequestCode>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    if (!string.IsNullOrEmpty(date))
                    {
                        list = dataContext.RequestCode.Where(m => m.Enterprise_Info_ID == enterpriseId && m.RequestDate >= Convert.ToDateTime(date)
							&& ((m.CodingClientType == (int)Common.EnumFile.CodingClientType.MA && m.Status == (int)EnumFile.RequestCodeStatus.GenerationIsComplete) 
                            || m.CodingClientType==(int)Common.EnumFile.CodingClientType.GS1)).ToList();
                    }
                    else
                    {
                        list = dataContext.RequestCode.Where(m => m.Enterprise_Info_ID == enterpriseId
						   && ((m.CodingClientType == (int)Common.EnumFile.CodingClientType.MA && m.Status == (int)EnumFile.RequestCodeStatus.GenerationIsComplete) || m.CodingClientType == (int)Common.EnumFile.CodingClientType.GS1)).ToList();
                    }
					foreach (var model in list) 
					{
						//2022-1-11 温森 如果批量申请生成批次为空标明企业已过期无法向idcode传值，将IDCodeBatchNo值改为RequestCode_ID，获取PIcode使用
						if (string.IsNullOrEmpty(model.IDCodeBatchNo) && model.CodingClientType == (int)Common.EnumFile.CodingClientType.MA) 
						{
							model.IDCodeBatchNo = model.RequestCode_ID.ToString();
						}
					}
                    ClearLinqModel(list);
                }
                catch (Exception ex)
                {
                    string errData = "打码客户端获取同步PI数据";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return list;
        }

        public List<string> GetPICodeList(long enterpriseId, string batchNo)
        {
            List<string> codeList = new List<string>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    RequestCode rRecord = dataContext.RequestCode.FirstOrDefault(m => m.Enterprise_Info_ID == enterpriseId && m.IDCodeBatchNo == batchNo);
					if (rRecord == null && batchNo.Length != 17 && !string.IsNullOrEmpty(batchNo)) 
					{
						long id = string.IsNullOrEmpty(batchNo)?0:Convert.ToInt64(batchNo);
						rRecord = dataContext.RequestCode.FirstOrDefault(m => m.Enterprise_Info_ID == enterpriseId && m.RequestCode_ID == id);
					}
                    if (rRecord != null)
                    {
                        List<Enterprise_FWCode_00> result = new List<Enterprise_FWCode_00>();
                        long pageCount = 0;
                        long beginCode = rRecord.StartNum.Value;
                        long endCode = rRecord.EndNum.Value;
                        long createCount = rRecord.TotalNum.GetValueOrDefault(0);
                        long saleCount = rRecord.saleCount.GetValueOrDefault(0);
                        string tablename = "";
                        using (DataClassesDataContext dataContextCode = GetCodeNewDataContext(rRecord.RequestCode_ID, out tablename))
                        {
                            dataContextCode.CommandTimeout = 600;
                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("select * from " + tablename + " where RequestCode_ID=" + rRecord.RequestCode_ID);
                            strSql.Append("  order by Enterprise_FWCode_ID ");
                            result = dataContextCode.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();
                            pageCount = result.Count();
                            result = result.ToList();
                            if (result.Count > 0)
                            {
                                codeList = result.Select(t => t.EWM).ToList();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "打码客户端获取同步PICode数据异常";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return codeList;
        }
        #endregion
    }
}
