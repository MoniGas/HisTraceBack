/********************************************************************************

** 作者： 张翠霞

** 创始时间：2017-02-08

** 联系方式 :13313318725

** 描述：追溯码信息管理

** 版本：v2.5.1

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;
using InterfaceWeb;
using Common;

namespace Dal
{
    /// <summary>
    /// 追溯码信息管理数据访问层
    /// </summary>
    public class RequestCodeMaDAL : DALBase
    {
        /// <summary>
        /// 获取追溯码生成记录
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="searchName">名称信息</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<View_RequestCodeMa> GetList(long enterpriseId, string searchName, string materialName, string bName, string beginDate, string endDate, out long totalCount, int pageIndex)
        {
            List<View_RequestCodeMa> result = new List<View_RequestCodeMa>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_RequestCodeMa.Where(m => m.Enterprise_Info_ID == enterpriseId);

                    if (!string.IsNullOrEmpty(searchName))
                    {
                        //data = data.Where(m => m.BatchName.Contains(searchName.Trim()) || m.MaterialFullName.Contains(searchName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        data = data.Where(m => m.MaterialFullName == materialName);
                    }
                    if (!string.IsNullOrEmpty(bName))
                    {
                        //data = data.Where(m => m.BatchName == bName);
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.RequestDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.RequestDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RequestCode_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetList():View_RequestCodeMa视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 分段码列表根据requestcodeID查询列表
        /// </summary>
        /// <param name="requestcodeID">申请码表ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="searchName">名称信息</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<View_RequestCodeSetting> GetRequestCodeSettingList(long enterpriseId, long requestcodeID, string searchName, string materialName, string bName, string beginDate, string endDate, out long totalCount, int pageIndex)
        {
            List<View_RequestCodeSetting> result = new List<View_RequestCodeSetting>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_RequestCodeSetting.Where(m => m.EnterpriseId == enterpriseId && m.RequestID == requestcodeID);

                    if (!string.IsNullOrEmpty(searchName))
                    {
                        data = data.Where(m => m.BatchName.Contains(searchName.Trim()) || m.MaterialFullName.Contains(searchName.Trim()));
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
                        data = data.Where(m => m.SetDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.SetDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.SetDate).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetRequestCodeSettingList():View_RequestCodeSetting视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 查询企业所有配置码信息
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="searchName">名称信息</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<View_RequestCodeSetting> GetRequestCodeSettingListAll(long enterpriseId, string searchName, string materialName, string bName, string beginDate, string endDate, out long totalCount, int pageIndex)
        {
            List<View_RequestCodeSetting> result = new List<View_RequestCodeSetting>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_RequestCodeSetting.Where(m => m.EnterpriseId == enterpriseId 
                        && (m.Type == (int)Common.EnumFile.GenCodeType.localCreate || m.Type == (int)Common.EnumFile.GenCodeType.single
                        || m.Type == (int)Common.EnumFile.GenCodeType.trap || m.Type==(int)Common.EnumFile.GenCodeType.pesticides)
                        );//CodingClientType为1代表是MA码  0代表GS1码；ISUpload为0表示该批码未上传
                    if (!string.IsNullOrEmpty(searchName))
                    {
                        data = data.Where(m => m.BatchName.Contains(searchName.Trim()) || m.MaterialFullName.Contains(searchName.Trim()));
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
                        data = data.Where(m => m.SetDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.SetDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    data = data.Where(m => m.BatchType == 1);
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetRequestCodeSettingList():View_RequestCodeSetting视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        public List<View_RequestCodeSetting> GetRequestCodeSettingListAll(long materialId, int pageIndex, out long totalCount)
        {
            List<View_RequestCodeSetting> result = new List<View_RequestCodeSetting>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_RequestCodeSetting.Where(m => m.MaterialID == materialId);
                    data = data.Where(m => m.BatchType == 1 && m.RequestCodeType != (int)Common.EnumFile.RequestCodeType.SecurityCode);
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetRequestCodeSettingList():View_RequestCodeSetting视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 接口查询企业所有配置码信息
        /// </summary>
        /// <param name="packagingIine"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="materialName"></param>
        /// <param name="count"></param>
        /// <param name="batchName"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public List<View_RequestCodeSetting> GetInRequestCodeSettingListAll(string beginDate, string endDate, string materialName, string count, string batchName, string spec)
        {
            List<View_RequestCodeSetting> result = new List<View_RequestCodeSetting>();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_RequestCodeSetting.Where(m => (m.Type == (int)Common.EnumFile.GenCodeType.localCreate || m.Type == (int)Common.EnumFile.GenCodeType.single
                        || m.Type == (int)Common.EnumFile.GenCodeType.trap || m.Type == (int)Common.EnumFile.GenCodeType.pesticides));
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        data = data.Where(m => m.MaterialFullName == materialName);
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.SetDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.SetDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    if (!string.IsNullOrEmpty(count))
                    {
                        data = data.Where(m => m.Count==Convert.ToInt32(count));
                    }
                    if (!string.IsNullOrEmpty(batchName))
                    {
                        data = data.Where(m => m.BatchName == batchName);
                    }
                    if (!string.IsNullOrEmpty(spec))
                    {
                        data = data.Where(m => m.Type == Convert.ToInt32(spec));
                    }
                    data = data.Where(m => m.BatchType == 1);
                    result = data.OrderByDescending(m => m.ID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetInRequestCodeSettingListAll():View_RequestCodeSetting视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取子批次列表
        /// </summary>
        /// <param name="requestId">申请码记录标识列</param>
        /// <returns>子批次列表</returns>
        public List<View_RequestCodeSetting> GetRequestCodeSettingListSub(long requestId)
        {
            List<View_RequestCodeSetting> result = new List<View_RequestCodeSetting>();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_RequestCodeSetting.Where(m => m.RequestID == requestId && m.BatchType == 2);
                    result = data.OrderByDescending(m => m.ID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetRequestCodeSettingList():View_RequestCodeSetting视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        public List<View_RequestCodeSetting> GetRequestCodeSettingListSubR(long requestId)
        {
            List<View_RequestCodeSetting> result = new List<View_RequestCodeSetting>();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_RequestCodeSetting.Where(m => m.RequestID == requestId
                        && m.BatchType == 2 && m.RequestCodeType != (int)Common.EnumFile.RequestCodeType.SecurityCode);
                    result = data.OrderByDescending(m => m.ID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetRequestCodeSettingList():View_RequestCodeSetting视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 开通追溯/开通防伪修改类型
        /// </summary>
        /// <param name="id">码标识</param>
        /// <param name="eId">企业ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public RetResult EditType(long id, long eId, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting newModel = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == id && m.EnterpriseId == eId);
                    if (newModel != null)
                    {
                        newModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.fwzsCode;
                        //if (type == (int)Common.EnumFile.RequestCodeType.SecurityCode)
                        //{
                        //    newModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.TraceCode;
                        //}
                        //else
                        //{
                        //    newModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.SecurityCode;
                        //}
                        dataContext.SubmitChanges();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "操作成功");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
                    }
                }
            }
            catch
            {
                Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
            }
            return Ret;
        }

        /// <summary>
        /// 选择追溯/防伪修改类型
        /// </summary>
        /// <param name="id">码标识</param>
        /// <param name="eId">企业ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public RetResult EditTypeTwo(long id, long eId, long materialId, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting newModel = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == id && m.EnterpriseId == eId);
                    if (newModel != null)
                    {
                        newModel.MaterialID = materialId;
                        newModel.RequestCodeType = type;
                        dataContext.SubmitChanges();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "保存成功");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "保存失败");
                    }
                }
            }
            catch
            {
                Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "保存失败");
            }
            return Ret;
        }

        #region  刘晓杰于2019年11月4日从CFBack项目移入此

        /// <summary>
        /// 获取接口需要的下载码记录
        /// </summary>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public List<View_InterFaceMaterialCode> GetInterFaceMaterialCode(string gentype, string stime, string etime, long enterpriseId)
        {
            List<View_InterFaceMaterialCode> result = new List<View_InterFaceMaterialCode>();
            using (DataClassesDataContext db = GetDataContext())
            {
                try
                {
                    var data = db.View_InterFaceMaterialCode.Where(m => m.Enterprise_Info_ID == enterpriseId && m.type == gentype);
                    if (!string.IsNullOrEmpty(stime))
                    {
                        data = data.Where(m => m.SetDate >= Convert.ToDateTime(stime + " 00:00:00"));
                    }
                    if (!string.IsNullOrEmpty(etime))
                    {
                        data = data.Where(m => m.SetDate <= Convert.ToDateTime(etime + " 23:59:59"));
                    }
                    data = data.Where(m => m.WebURL != null || m.WebURL != "");
                    result = data.OrderByDescending(m => m.SetDate).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeMaDAL.GetMaterialCode():GetInterFaceMaterialCode";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 获取pi数据（同步发码机构的）
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public RetResult GetPIInfo(string mainCode, string conStr)
        {
            string Msg = "同步PI数据失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext(conStr))
                {
                    string start_date = "";
                    Enterprise_Info enInfo = dataContext.Enterprise_Info.Where(p => p.MainCode == mainCode).FirstOrDefault();
                    EnterpriseJKDate dateInfo = dataContext.EnterpriseJKDate.Where(p => p.EnterpriseID == enInfo.Enterprise_Info_ID).FirstOrDefault();
                    long TotalNum = 0;
                    if (dateInfo != null)
                    {
                        start_date = Convert.ToDateTime(dateInfo.PIHuoQuDate).ToString("yyyy-MM-dd");
                    }
                    string entoken = "";
                    string access_token_code = "";
                    EnterpriseShopLink enEx = new Dal.ScanCodeDAL().ShopEn(enInfo.Enterprise_Info_ID);
                    if (enEx != null)
                    {
                        entoken = enEx.access_token == null ? "" : enEx.access_token;
                        access_token_code = enEx.access_token_code == null ? "" : enEx.access_token_code;
                    }
                    else
                    {
                        return Ret;
                    }
                    ListHisPI piList = BaseDataDAL.GetPIInfo(entoken, access_token_code, mainCode, "", start_date);
                    #region  循环数据插入数据库
                    if (piList != null && piList.data != null)
                    {
                        foreach (HisPI sub in piList.data)
                        {
                            MaterialDI diSub = dataContext.MaterialDI.Where(p => p.MaterialUDIDI == sub.completecode).FirstOrDefault();
                            if (diSub == null)
                            {
                                continue;
                            }
                            //如果有该批次的数据，则跳过，不插入数据库中
                            RequestCode requestCode = dataContext.RequestCode.Where(p => p.IDCodeBatchNo == sub.batch_no).FirstOrDefault();
                            if (requestCode != null)
                                continue;
                            //未完待续
                            int index = dataContext.RequestCodeSetting.Where(m => m.EnterpriseId == enInfo.Enterprise_Info_ID
                            && m.SetDate > Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01 00:00:00"))).Count() + 1;
                            string batchName = DateTime.Now.ToString("yyyy") + index;
                            while (dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == enInfo.Enterprise_Info_ID && m.BatchName == batchName) != null)
                            {
                                batchName = DateTime.Now.ToString("yyyy") + (index++);
                            }
                            RequestCode ObjRequestCode = new RequestCode();
                            ObjRequestCode.Enterprise_Info_ID = enInfo.Enterprise_Info_ID;
                            ObjRequestCode.adddate = DateTime.Now;
                            ObjRequestCode.IDCodeBatchNo = sub.batch_no;
                            ObjRequestCode.RequestDate = DateTime.Now;
                            ObjRequestCode.ShengChanPH = sub.batchnumber;
                            if (!string.IsNullOrEmpty(sub.enddate))
                            {
                                ObjRequestCode.ShiXiaoDate = sub.enddate;
                            }
                            if (!string.IsNullOrEmpty(sub.effectivedate))
                            {
                                ObjRequestCode.YouXiaoDate = sub.effectivedate;
                            }
                            ObjRequestCode.TotalNum = Convert.ToInt32(sub.codenum);
                            ObjRequestCode.FixedCode = sub.completecode;
                            int status = Convert.ToInt32(EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated);
                            ObjRequestCode.Status = status;
                            ObjRequestCode.IsRead = (int)Common.EnumFile.IsRead.noRead;
                            ObjRequestCode.IsLocal = (int)Common.EnumFile.Islocal.cloud;
                            ObjRequestCode.Material_ID = diSub.MaterialID;
                            ObjRequestCode.StartNum = 1;
                            ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
                            ObjRequestCode.Type = (int)Common.EnumFile.GenCodeType.single;
                            ObjRequestCode.MaterialXH = diSub.MaterialXH;
                            ObjRequestCode.createtype = sub.createtype;
                            ObjRequestCode.ISUpload = (int)Common.EnumFile.RequestISUpload.DownIDCode;
                            ObjRequestCode.CodingClientType = 1;
                            dataContext.RequestCode.InsertOnSubmit(ObjRequestCode);
                            dataContext.SubmitChanges();
                            RequestCodeSetting setModel = new RequestCodeSetting();
                            setModel.MaterialID = ObjRequestCode.Material_ID;
                            setModel.RequestID = ObjRequestCode.RequestCode_ID;
                            setModel.BatchName = batchName;
                            setModel.EnterpriseId = enInfo.Enterprise_Info_ID;
                            setModel.MaterialID = diSub.MaterialID;
                            setModel.Count = sub.codenum;
                            setModel.beginCode = 1;
                            setModel.endCode = sub.codenum;
                            setModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.TraceCode;
                            setModel.SetDate = DateTime.Now;
                            setModel.BatchType = 1;
                            setModel.MaterialXH = diSub.MaterialXH;
                            dataContext.RequestCodeSetting.InsertOnSubmit(setModel);
                            dataContext.SubmitChanges();
                        }
                    }
                    #endregion
                    if (TotalNum > 0)
                    {
                        //为统计表添加数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = enInfo.Enterprise_Info_ID;
                        homeData.RequestCodeTimes = 1;
                        homeData.RequestCodeCount = TotalNum;
                        ComplaintDAL complaintDal = new ComplaintDAL();
                        RetResult result = complaintDal.Update(homeData);
                        //更新企业表中已用码的数量
                        if (enInfo.RequestCodeCount > TotalNum)
                        {
                            enInfo.RequestCodeCount = enInfo.RequestCodeCount - TotalNum;
                        }
                        else
                        {
                            enInfo.RequestCodeCount = 0;
                            enInfo.OverDraftCount = enInfo.RequestCodeCount + enInfo.OverDraftCount - TotalNum;
                        }
                        enInfo.UsedCodeCount = (long)enInfo.UsedCodeCount + TotalNum;
                    }
                    if (dateInfo == null)
                    {
                        dateInfo = new EnterpriseJKDate();
                        dateInfo.PIHuoQuDate = DateTime.Now;
                        dateInfo.EnterpriseID = enInfo.Enterprise_Info_ID;
                        dataContext.EnterpriseJKDate.InsertOnSubmit(dateInfo);
                    }
                    else
                    {
                        dateInfo.PIHuoQuDate = DateTime.Now;
                    }
                    dataContext.SubmitChanges();
                    Msg = "同步成功！";
                    error = CmdResultError.NONE;
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, "同步成功", "同步成功");
                }
            }
            catch
            {
                Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 根据申请码编号查询码明细
        /// </summary>
        /// <param name="requesetId"></param>
        /// <returns></returns>
        public ListPICode GetPICode(string requesetId)
        {
            ListPICode piCode = new ListPICode();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext("Constr"))
                {
                    RequestCode request = dataContext.RequestCode.Where(p => p.RequestCode_ID ==Convert.ToInt64(requesetId)).FirstOrDefault();
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == request.Enterprise_Info_ID).FirstOrDefault();
                    string entoken = "";
                    string entokenCode = "";
                    EnterpriseShopLink enEx = new Dal.ScanCodeDAL().ShopEn(enterprise.Enterprise_Info_ID);
                    if (enEx != null)
                    {
                        entoken = enEx.access_token == null ? "" : enEx.access_token;
                        entokenCode = enEx.access_token_code == null ? "" : enEx.access_token_code;
                    }
                    else
                    {
                        return piCode;
                    }
                    if (request != null && enterprise!=null)
                    {
                        piCode = BaseDataDAL.GetPICode(entoken,entokenCode,enterprise.MainCode, request.IDCodeBatchNo);
                    }
                }
            }
            catch
            {
                Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "操作失败");
            }
            return piCode;
        }
    }
}
