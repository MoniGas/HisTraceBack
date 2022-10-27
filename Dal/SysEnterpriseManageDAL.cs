using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using System.Text;
using System.Configuration;
using Common.Tools;
using System.IO;
using System.Data.Common;

namespace Dal
{
    public class SysEnterpriseManageDAL : DALBase
    {
        public List<Enterprise_Info> GetEnterpriseInfoList(string name, long eId, int? pageIndex, out long totalCount, bool wareHouseStatus)
        {
            List<Enterprise_Info> dataInfoList = null;
            totalCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Enterprise_Info
                                   select data;
                    if (wareHouseStatus)
                    {
                        dataList = dataList.Where(m => m.WareHouseVerify > 0);
                    }
                    if (eId != 16)
                    {
                        dataList = dataList.Where(w => w.PRRU_PlatForm_ID == eId);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.Enterprise_Info_ID).Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();
                    foreach (var item in dataInfoList)
                    {
                        item.StrRequestCount = item.RequestCodeCount.HasValue ? item.RequestCodeCount.Value.ToString() : "未设置";
                        item.StrUseCount =
                            Convert.ToString
                            (
                            dataContext.RequestCode.Where(
                                m => m.Enterprise_Info_ID == item.Enterprise_Info_ID &&
                                    (m.Status >= (int)EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated ||
                                    m.Status == (int)EnumFile.RequestCodeStatus.GenerationIsComplete)
                                ).Sum(m => m.TotalNum) ?? 0
                            + dataContext.RequestCode.Where(
                                m => m.Enterprise_Info_ID == item.Enterprise_Info_ID &&
                                    m.Status == (int)EnumFile.RequestCodeStatus.Unaudited
                                ).Sum(m => m.TotalNum) ?? 0);
                        item.StrRemainCount = "无限";
                        if (item.RequestCodeCount.HasValue)
                        {
                            item.StrRemainCount = (item.RequestCodeCount.Value - Convert.ToInt64(item.StrUseCount)).ToString();
                        }
                    }
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }

            return dataInfoList;
        }

        public List<Enterprise_Info> GetAreaEnterprise(long shengId, long shiId)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = (from data in dataContext.Enterprise_Info
                                    where data.Dictionary_AddressSheng_ID == shengId &&
                                        //data.Dictionary_AddressShi_ID == shiId &&
                                    data.Status == 0
                                    select data).ToList();
                    List<Enterprise_Info> templist = new List<Enterprise_Info>();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        Enterprise_Info temp = new Enterprise_Info();
                        temp = dataList[i];
                        var prruinfo = dataContext.PRRU_PlatForm.FirstOrDefault(m => m.PRRU_PlatForm_ID == temp.PRRU_PlatForm_ID);
                        if (prruinfo != null && prruinfo.PRRU_PlatFormLevel_ID != 2)
                        {
                            templist.Add(temp);
                        }
                    }
                    dataList = templist;
                    ClearLinqModel(dataList);
                    return dataList;
                }
            }
            catch
            {
                return null;
            }
        }

        public List<string> GetAreaEnterprise(long shengId, long shiId, long eId)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    List<long> dataList = (from data in dataContext.Enterprise_Info
                                           where
                                               data.Dictionary_AddressSheng_ID == shengId &&
                                               //data.Dictionary_AddressShi_ID == shiId &&
                                               data.PRRU_PlatForm_ID == eId &&
                                               data.Status == 0
                                           select data.Enterprise_Info_ID).ToList();
                    ClearLinqModel(dataList);

                    return dataList.ConvertAll(o => string.Format("{0}", o));
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取默认监管部门（平台）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PRRU_PlatForm GetPlatForm()
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.PRRU_PlatForm.FirstOrDefault();
            }
        }

        public RetResult Save(long eId, string arrayId, string falseId)
        {
            Ret.Msg = "保存失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    string[] falseArray = falseId.Split(',');
                    string[] idArray = arrayId.Split(',');

                    foreach (string id in falseArray)
                    {
                        if (id == null || id.Trim().Equals(""))
                        {
                            continue;
                        }
                        var data = (from d in dataContext.Enterprise_Info
                                    where d.Enterprise_Info_ID == Convert.ToInt64(id)
                                    select d).FirstOrDefault();

                        data.PRRU_PlatForm_ID = GetPlatForm().PRRU_PlatForm_ID;
                    }

                    foreach (string id in idArray)
                    {
                        if (id == null || id.Trim().Equals(""))
                        {
                            continue;
                        }
                        var data = (from d in dataContext.Enterprise_Info
                                    where d.Enterprise_Info_ID == Convert.ToInt64(id)
                                    select d).FirstOrDefault();

                        data.PRRU_PlatForm_ID = eId;
                    }

                    dataContext.SubmitChanges();

                    Ret.SetArgument(CmdResultError.NONE, "保存成功！", "保存成功！");
                }
            }
            catch
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败！", "数据库连接失败！");
            }

            return Ret;
        }

        public RetResult VerifyEnterprise(string enterpriseid, string type)
        {
            string msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == long.Parse(enterpriseid));
                    if (enterprise != null)
                    {
                        int verify = int.Parse(type);
                        if (verify == (int)EnumFile.EnterpriseVerify.passVerify)
                        {
                            //OrganUnitStatusInfo statusInfo = InterfaceWeb.BaseDataDAL.GetStatus(enterprise.MainCode);
                            //if (statusInfo.IsAudit != 100)
                            //{
                            //    msg = "IDcode平台尚未审核通过！";
                            //}
                            //else
                            //{
                            dataContext.EnterpriseVerify.DeleteAllOnSubmit(
                                dataContext.EnterpriseVerify.Where(m => m.Enterprise_ID == enterprise.Enterprise_Info_ID)
                            );
                            enterprise.Verify = verify;
                            dataContext.SubmitChanges();
                            msg = "操作成功！";
                            CmdError = CmdResultError.NONE;
                            //}
                        }
                        else
                        {
                            EnterpriseVerify v = dataContext.EnterpriseVerify.FirstOrDefault(m => m.Enterprise_ID == enterprise.Enterprise_Info_ID);
                            if (v == null)
                            {
                                EnterpriseVerify newv = new EnterpriseVerify();
                                newv.AddDate = DateTime.Now;
                                newv.Enterprise_ID = enterprise.Enterprise_Info_ID;
                                newv.VerifyContent = "您的企业未通过审核";
                                dataContext.EnterpriseVerify.InsertOnSubmit(newv);
                            }
                            else
                            {
                                v.AddDate = DateTime.Now;
                            }
                            enterprise.Verify = verify;
                            dataContext.SubmitChanges();
                            msg = "操作成功！";
                            CmdError = CmdResultError.NONE;
                        }
                    }
                    else
                    {
                        msg = "没有找到企业！";
                    }
                }
            }
            catch
            {

            }
            Ret.SetArgument(CmdError, msg, msg);
            return Ret;
        }

        public List<View_Order_EnterpriseAccount> GetEnterprise(string name, long enterpriseid, int? pageIndex, out long totalCount)
        {
            List<View_Order_EnterpriseAccount> dataInfoList = null;
            totalCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_Order_EnterpriseAccount
                                   where data.IsOpenShop == true
                                   select data;
                    if (enterpriseid != 16)
                    {
                        dataList = dataList.Where(w => w.PRRU_PlatForm_ID == enterpriseid);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.Enterprise_Info_ID).Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();

                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }

            return dataInfoList;
        }

        public RetResult VerifyShop(string enterpriseid, string type)
        {
            string msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == long.Parse(enterpriseid));
                    if (enterprise != null)
                    {
                        int verify = int.Parse(type);
                        enterprise.ShopVerify = verify;
                        dataContext.SubmitChanges();
                        msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        msg = "没有找到企业！";
                    }
                }
            }
            catch
            {

            }
            Ret.SetArgument(CmdError, msg, msg);
            return Ret;
        }

        public RetResult VerifyWareHouse(string enterpriseid, string type)
        {
            string msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == long.Parse(enterpriseid));
                    if (enterprise != null)
                    {
                        int verify = int.Parse(type);
                        enterprise.WareHouseVerify = verify;
                        dataContext.SubmitChanges();
                        msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        msg = "没有找到企业！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, msg, msg);
            return Ret;
        }

        public RetResult SetRequestCount(long eId, long count)
        {
            string msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eId);
                    if (enterprise != null)
                    {
                        enterprise.RequestCodeCount = count;
                        dataContext.SubmitChanges();
                        msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        msg = "没有找到企业！";
                    }
                }
            }
            catch
            {

            }
            Ret.SetArgument(CmdError, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 管理员/监管部门给企业重置密码
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="password">重置的密码</param>
        /// <returns></returns>
        public RetResult SetPassWord(long eId, string password, RecordLog recordLog)
        {
            string msg = "重置密码失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User enterpriseUser = dataContext.Enterprise_User.FirstOrDefault(m => m.Enterprise_Info_ID == eId && m.UserType == "默认");
                    if (enterpriseUser != null)
                    {
                        enterpriseUser.LoginPassWord = password;
                        dataContext.RecordLog.InsertOnSubmit(recordLog);
                        dataContext.SubmitChanges();
                        msg = "重置密码成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        msg = "没有找到企业信息！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, msg, msg);
            return Ret;
        }

        #region 20170810 代理商/管理后台新改版
        /// <summary>
        /// 获取我的企业列表
        /// </summary>
        /// <param name="name">企业名称</param>
        /// <param name="eId">ID</param>
        /// <param name="beginDate">加入时间</param>
        /// <param name="endDate">加入时间</param>
        /// <param name="pageIndex">分页</param>
        /// <param name="wareHouseStatus"></param>
        /// <returns></returns>
        public PagedList<Enterprise_Info> GetEnterpriseInfoListMan(string name, long eId, string beginDate, string endDate, int? pageIndex, bool wareHouseStatus)
        {
            PagedList<Enterprise_Info> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Enterprise_Info
                                   where data.PRRU_PlatForm_ID != 0
                                   select data;
                    if (wareHouseStatus)
                    {
                        dataList = dataList.Where(m => m.WareHouseVerify > 0);
                    }
                    if (eId != 16)
                    {
                        dataList = dataList.Where(w => w.PRRU_PlatForm_ID == eId);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.adddate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.adddate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    //totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.Enterprise_Info_ID).ToPagedList(pageIndex ?? 1, PageSize);
                    foreach (var item in dataInfoList)
                    {
                        item.StrRequestCount = item.RequestCodeCount.HasValue ? item.RequestCodeCount.Value.ToString() : "未设置";
                        item.StrUseCount =
                            Convert.ToString
                            (
                            dataContext.RequestCode.Where(
                                m => m.Enterprise_Info_ID == item.Enterprise_Info_ID &&
                                    (m.Status >= (int)EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated ||
                                    m.Status == (int)EnumFile.RequestCodeStatus.GenerationIsComplete)
                                ).Sum(m => m.TotalNum) ?? 0
                            + dataContext.RequestCode.Where(
                                m => m.Enterprise_Info_ID == item.Enterprise_Info_ID &&
                                    m.Status == (int)EnumFile.RequestCodeStatus.Unaudited
                                ).Sum(m => m.TotalNum) ?? 0);
                        item.StrRemainCount = "无限";
                        if (item.RequestCodeCount.HasValue)
                        {
                            item.StrRemainCount = (item.RequestCodeCount.Value - Convert.ToInt64(item.StrUseCount)).ToString();
                        }
                    }
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }
        /// <summary>
        /// 获取我的所有企业列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="wareHouseStatus"></param>
        /// <returns></returns>
        public List<Enterprise_Info> GetAllEnterpriseInfoListMan(string name, long eId, bool wareHouseStatus)
        {
            string CodeThreshold = System.Configuration.ConfigurationManager.AppSettings["CodeThreshold"];
            List<Enterprise_Info> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Enterprise_Info
                                   where data.PRRU_PlatForm_ID != 0 && (data.RequestCodeCount - data.UsedCodeCount + data.OverDraftCount) < Convert.ToInt32(CodeThreshold) && data.OverDraftCount >= 0
                                   select data;
                    if (wareHouseStatus)
                    {
                        dataList = dataList.Where(m => m.WareHouseVerify > 0);
                    }
                    if (eId != 16)
                    {
                        dataList = dataList.Where(w => w.PRRU_PlatForm_ID == eId);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    //totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.Enterprise_Info_ID).ToList();
                    foreach (var item in dataInfoList)
                    {
                        item.StrRequestCount = item.RequestCodeCount.HasValue ? item.RequestCodeCount.Value.ToString() : "未设置";
                        item.StrUseCount =
                            Convert.ToString
                            (
                            dataContext.RequestCode.Where(
                                m => m.Enterprise_Info_ID == item.Enterprise_Info_ID &&
                                    (m.Status >= (int)EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated ||
                                    m.Status == (int)EnumFile.RequestCodeStatus.GenerationIsComplete)
                                ).Sum(m => m.TotalNum) ?? 0
                            + dataContext.RequestCode.Where(
                                m => m.Enterprise_Info_ID == item.Enterprise_Info_ID &&
                                    m.Status == (int)EnumFile.RequestCodeStatus.Unaudited
                                ).Sum(m => m.TotalNum) ?? 0);
                        item.StrRemainCount = "无限";
                        if (item.RequestCodeCount.HasValue)
                        {
                            item.StrRemainCount = (item.RequestCodeCount.Value - Convert.ToInt64(item.StrUseCount)).ToString();
                        }
                    }
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 获取新入驻的企业列表
        /// </summary>
        /// <param name="name">企业名称（查询用）</param>
        /// <param name="beginDate">加入时间</param>
        /// <param name="endDate">加入时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<Enterprise_Info> GetEnterpriseInfoListNewAdd(string name, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<Enterprise_Info> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Enterprise_Info
                                   where data.PRRU_PlatForm_ID == 0
                                   select data;
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.adddate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.adddate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.Enterprise_Info_ID).ToPagedList(pageIndex ?? 1, PageSize);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public View_EnterpriseInfoUser GetEnInfo(long eid)
        {
            View_EnterpriseInfoUser result;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                result = dataContext.View_EnterpriseInfoUser.FirstOrDefault(m => m.Enterprise_Info_ID == eid &&
                    (m.UserType == "默认" || m.UserType == "GS1") && m.EnterpriseStatus == (int)EnumFile.Status.used);
            }
            return result;
        }

        /// <summary>
        /// 获取合同最新的记录
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public Contract GetContractInfo(long eid)
        {
            Contract result;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                //string sql = "select top 1 * from Contract where EnterpriseID=" + eid + "order by ContractID desc";
                //result = dataContext.ExecuteQuery<Contract>(sql).FirstOrDefault();
                result = dataContext.Contract.FirstOrDefault(p => p.EnterpriseID == eid);
            }
            return result;
        }

        /// <summary>
        /// 给企业管理账户重置密码
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public RetResult ResetPassword(long eid)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_User model = dataContext.Enterprise_User.FirstOrDefault(m => m.Enterprise_Info_ID == eid && m.UserType == "默认");
                    if (model == null)
                    {
                        Ret.SetArgument(CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    model.LoginPassWord = "123456";
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, null, "重置密码成功，重置密码为：123456");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }

        /// <summary>
        /// 获取企业码情况
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public Enterprise_Info GetEnInfoCodeCount(long eid)
        {
            Enterprise_Info result;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                result = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eid &&
                    m.Status == (int)EnumFile.Status.used);
            }
            return result;
        }

        /// <summary>
        /// 企业用码量设置
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="sqcount">企业最多申请数量</param>
        /// <param name="tzcount">企业可透支的数量</param>
        /// <returns></returns>
        public RetResult SetAmountCode(long eId, long sqcount, long tzcount)
        {
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eId);
                    if (enterprise != null)
                    {
                        enterprise.RequestCodeCount = sqcount;
                        enterprise.OverDraftCount = tzcount;
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "没有找到企业！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 给企业续码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult ContinneCode(ContinneCodeRecord model)
        {
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == model.EnterpriseID);
                    if (enterprise != null)
                    {
                        enterprise.RequestCodeCount = enterprise.RequestCodeCount + model.CodeCount;
                        dataContext.ContinneCodeRecord.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "没有找到企业！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取企业续码记录
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_ContinueCode> GetContinneCodeRecord(long eid, long platId, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<View_ContinueCode> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_ContinueCode

                                   select data;
                    if (eid > 0)
                    {
                        dataList = dataList.Where(p => p.EnterpriseID == eid);
                    }
                    if (platId > 0)
                    {
                        dataList = dataList.Where(p => p.PRRU_PlatForm_ID == platId);
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.AddDate.GetValueOrDefault() >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.AddDate.GetValueOrDefault() <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.ContinneCodeRecordID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 获取企业用码
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_UsedCodeSituation> GetUsedCodeSituation(long eid, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<View_UsedCodeSituation> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_UsedCodeSituation
                                   where data.Eid == eid
                                   select data;
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.AddDate.GetValueOrDefault() >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.AddDate.GetValueOrDefault() <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.AddDate).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 关联心入驻的企业
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="pid">登录ID</param>
        /// <returns></returns>
        public RetResult GuanLian(long eid, long pid)
        {
            string Msg = "关联失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eid &&
                        m.Status == (int)EnumFile.Status.used);
                    if (enterprise != null)
                    {
                        enterprise.PRRU_PlatForm_ID = pid;
                        dataContext.SubmitChanges();
                        Msg = "关联成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "没有找到企业！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 停用企业
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public RetResult TingYong(long eid)
        {
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eid &&
                        m.Status == (int)EnumFile.Status.used);
                    if (enterprise != null)
                    {
                        enterprise.Verify = (int)EnumFile.EnterpriseVerify.pauseVerify;
                        dataContext.SubmitChanges();
                        //
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "没有找到企业！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="sqcount">设置用码量</param>
        /// <param name="tzcount">设置透支数量</param>
        /// <param name="type">类型（1：只是审核，2：审核并保存）</param>
        /// <returns></returns>
        public RetResult SetAudit(long eId, long sqcount, long tzcount, int type)
        {
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eId);
                    if (null != enterprise)
                    {
                        if (type == 2)
                        {
                            enterprise.RequestCodeCount = sqcount;
                            enterprise.OverDraftCount = tzcount;
                            enterprise.Verify = (int)EnumFile.EnterpriseVerify.passVerify;
                            dataContext.SubmitChanges();
                            Msg = "操作成功！";
                            CmdError = CmdResultError.NONE;
                        }
                        else if (type == 1)
                        {
                            enterprise.Verify = (int)EnumFile.EnterpriseVerify.passVerify;
                            dataContext.SubmitChanges();
                            //企业生成二维码
                            Enterprise_EwmSysLogin enterpriseEwmSysLogin = dataContext.Enterprise_EwmSysLogin.FirstOrDefault(p => p.MainCode == enterprise.MainCode);
                            if (null != enterpriseEwmSysLogin)
                            {
                                DateTime expirateTime = DateTime.Now.AddYears(1);
                                var firstOrDefault = dataContext.Contract.FirstOrDefault(p => p.EnterpriseID == eId);
                                if (null != firstOrDefault)
                                {
                                    expirateTime = firstOrDefault.EndDate ?? expirateTime;
                                }
                                Enterprise_EwmSysLogin model = new Enterprise_EwmSysLogin();
                                model.Enterprise_Info_ID = enterprise.Enterprise_Info_ID;
                                model.EnterpriseName = enterprise.EnterpriseName;
                                model.MainCode = enterprise.MainCode;
                                model.ExpirateTime = expirateTime;
                                model.AccountType = (int)EnumFile.AccountType.PlatAccount;
                                dataContext.Enterprise_EwmSysLogin.InsertOnSubmit(model);
                                dataContext.SubmitChanges();
                            }
                            Msg = "操作成功！";
                            CmdError = CmdResultError.NONE;
                        }
                    }
                    else
                    {
                        Msg = "没有找到企业！";
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 签订合同
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Contract(Contract model)
        {
            string Msg = "签订失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.Contract.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Msg = "签订成功！";
                    CmdError = CmdResultError.NONE;
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取企业签订合同列表
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<Contract> GetContractList(long eid, int? pageIndex)
        {
            PagedList<Contract> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Contract
                                   where data.EnterpriseID == eid
                                   select data;
                    dataInfoList = dataList.OrderByDescending(m => m.ContractID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 获取企业商城管理（是否开通商城）
        /// </summary>
        /// <param name="name">企业名称</param>
        /// <param name="enterpriseid">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_Order_EnterpriseAccount> GetEnterpriseShopList(string name, long enterpriseid, int? pageIndex)
        {
            PagedList<View_Order_EnterpriseAccount> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_Order_EnterpriseAccount
                                   where data.IsOpenShop == true
                                   select data;
                    if (enterpriseid != 16)
                    {
                        dataList = dataList.Where(w => w.PRRU_PlatForm_ID == enterpriseid);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.Enterprise_Info_ID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 确认开通/暂停使用
        /// </summary>
        /// <param name="enterpriseid">企业ID</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public RetResult EditVerifyShop(long enterpriseid, int type)
        {
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == enterpriseid);
                    if (enterprise != null)
                    {
                        enterprise.ShopVerify = type;
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "没有找到企业！";
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取企业续码记录
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<ContinneCodeRecord> GetEnContinneCodeRecord(long eid, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<ContinneCodeRecord> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.ContinneCodeRecord
                                   select data;
                    if (eid > 0)
                    {
                        dataList = dataList.Where(p => p.EnterpriseID == eid);
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.AddDate.GetValueOrDefault() >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.AddDate.GetValueOrDefault() <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.ContinneCodeRecordID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }
        #endregion
        /// <summary>
        /// 获取授权码信息
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public Enterprise_License GetEnInfoLicense(long eid)
        {
            Enterprise_License result;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                result = dataContext.Enterprise_License.FirstOrDefault(m => m.EnterpriseID == eid &&
                    m.State == (int)EnumFile.Status.used);
            }
            return result;
        }
        /// <summary>
        /// 设置授权码
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="adminId"></param>
        /// <param name="mainCode"></param>
        /// <param name="setDate"></param>
        /// <returns></returns>
        public RetResult SetAuthorizationCode(long eid, long adminId, string LicenseType, string setDate, string fileurl)
        {
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    string setDatestr = Convert.ToDateTime(setDate).ToString("yyyyMMdd");
                    string licenseCode = setDatestr;
                    AuthorCode author = dataContext.AuthorCode.Where(p => p.LicenseType == Convert.ToInt32(LicenseType)).FirstOrDefault();
                    if (author == null)
                    {
                        Ret.SetArgument(CmdError, Msg, Msg);
                        return Ret;
                    }
                    Enterprise_Info enterpriseInfo = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == eid).FirstOrDefault();
                    if (enterpriseInfo == null)
                    {
                        Ret.SetArgument(CmdError, Msg, Msg);
                        return Ret;
                    }
                    licenseCode = licenseCode + "&" + author.appId + "&" + author.appSecret;
                    byte[] key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["key"]);
                    byte[] iv = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["iv"]);
                    licenseCode = SecurityDES.Encrypt(licenseCode, key, iv);
                    Enterprise_License license = new Enterprise_License();
                    license.State = (int)Common.EnumText.Status.used;
                    license.LicenseEndDate = Convert.ToDateTime(setDate);
                    license.LicenseCode = licenseCode;
                    license.EnterpriseID = eid;
                    license.EnterpriseName = enterpriseInfo.EnterpriseName;
                    license.OperateDate = DateTime.Now;
                    license.AdminID = adminId;
                    license.LicenseFile = fileurl;
                    license.LicenseType = Convert.ToInt32(LicenseType);
                    #region 生成TXT文档
                    string filePath = string.Format(AppDomain.CurrentDomain.BaseDirectory + "File");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string docFileName = Guid.NewGuid().ToString() + ".txt";
                    using (StreamWriter sw = new StreamWriter(filePath + "\\" + docFileName, true))
                    {
                        sw.WriteLine(licenseCode);
                    }
                    #endregion
                    license.LicenseTxt = filePath + "\\" + docFileName;
                    license.LicenseTxtURL = "/" + "File" + "/" + docFileName;
                    Enterprise_License enterprise = dataContext.Enterprise_License.FirstOrDefault(m => m.EnterpriseID == eid && m.State != -1);
                    if (enterprise != null)
                    {
                        enterprise.State = (int)Common.EnumText.Status.delete;
                        dataContext.Enterprise_License.InsertOnSubmit(license);
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        dataContext.Enterprise_License.InsertOnSubmit(license);
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        #region 20200817新加企业PDA设备信息
        /// <summary>
        /// 获取设备串号列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<PRRU_EnEquipmentInfo> GetEnEquipmentList(string name, long eId, int? pageIndex)
        {
            PagedList<PRRU_EnEquipmentInfo> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.PRRU_EnEquipmentInfo
                                   where data.SetEnterpriseID == eId
                                   select data;
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(m => m.EnterpriseName.Contains(name) || m.EquipmentNo.Contains(name));
                    }
                    //totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.ID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }

            return dataInfoList;
        }

        /// <summary>
        /// 添加设备串号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(PRRU_EnEquipmentInfo model)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    PRRU_EnEquipmentInfo temp = dataContext.PRRU_EnEquipmentInfo.FirstOrDefault(p => p.EquipmentNo == model.EquipmentNo && p.EnterpriseID == model.EnterpriseID);
                    if (temp != null)
                    {
                        Msg = "该设备串号已存在！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                    dataContext.PRRU_EnEquipmentInfo.InsertOnSubmit(model);
                    dataContext.SubmitChanges();

                    Msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取设备串号信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public PRRU_EnEquipmentInfo GetEnEquipmentInfo(long Id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_EnEquipmentInfo
                                where d.ID == Id
                                select d).FirstOrDefault();
                    ClearLinqModel(data);
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 编辑设备串号信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult EditEnEquipmentInfo(PRRU_EnEquipmentInfo model)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    PRRU_EnEquipmentInfo temp = dataContext.PRRU_EnEquipmentInfo.FirstOrDefault(p => p.ID == model.ID);
                    if (temp != null)
                    {
                        PRRU_EnEquipmentInfo tempY = dataContext.PRRU_EnEquipmentInfo.FirstOrDefault(p => p.EquipmentNo == model.EquipmentNo && p.ID != model.ID && p.EnterpriseID == p.EnterpriseID);
                        if (tempY != null)
                        {
                            Msg = "该设备串号已存在！";
                            error = CmdResultError.NONE;
                            Ret.SetArgument(error, Msg, Msg);
                            return Ret;
                        }
                        else
                        {
                            temp.EnterpriseID = model.EnterpriseID;
                            temp.EnterpriseName = model.EnterpriseName;
                            temp.EquipmentNo = model.EquipmentNo;
                            temp.LastDate = model.LastDate;
                            temp.LastUserID = model.LastUserID;
                            temp.LastUserName = model.LastUserName;
                            dataContext.SubmitChanges();
                            Msg = "保存成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                    else
                    {
                        Msg = "未找到要修改的设备信息！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 启用/禁用设备
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public RetResult EditEquipmentStatus(long id, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    PRRU_EnEquipmentInfo model = dataContext.PRRU_EnEquipmentInfo.FirstOrDefault(m => m.ID == id);
                    if (model == null)
                    {
                        Ret.SetArgument(CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    if (type == 1)
                    {
                        model.Status = (int)Common.EnumFile.Status.used;
                    }
                    else
                    {
                        model.Status = (int)Common.EnumFile.Status.delete;
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, null, "操作成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }

        /// <summary>
        /// 上传的Excel
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public RetResult ImportExcel(System.Data.DataSet ds)
        {
            RetResult result = new RetResult();
            LoginInfo user = SessCokie.GetMan;
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "导入信息失败！";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    if (ds != null)
                    {
                        StringBuilder strBuilder = new StringBuilder();
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            #region  赋值
                            PRRU_EnEquipmentInfo excelde = new PRRU_EnEquipmentInfo();
                            excelde.EnterpriseName = ds.Tables[0].Rows[i][0].ToString().Trim();
                            excelde.EquipmentNo = ds.Tables[0].Rows[i][1].ToString().Trim();
                            Enterprise_Info enInfo = dataContext.Enterprise_Info.FirstOrDefault(m => m.EnterpriseName == excelde.EnterpriseName);
                            if (enInfo != null)
                            {
                                excelde.EnterpriseID = enInfo.Enterprise_Info_ID;
                            }
                            else
                            {
                                excelde.EnterpriseID = 0;
                            }
                            PRRU_EnEquipmentInfo temp = dataContext.PRRU_EnEquipmentInfo.FirstOrDefault(m => m.EquipmentNo == excelde.EquipmentNo);
                            if (temp != null && temp.Status == (int)Common.EnumFile.Status.delete)
                            {
                                temp.Status = (int)Common.EnumFile.Status.used;
                                temp.LastDate = DateTime.Now;
                                temp.LastUserID = user.UserID;
                                temp.LastUserName = user.UserName;
                                dataContext.SubmitChanges();
                            }
                            else if (temp == null)
                            {
                                excelde.LastDate = DateTime.Now;
                                excelde.LastUserID = user.UserID;
                                excelde.LastUserName = user.UserName;
                                excelde.SetDate = DateTime.Now;
                                excelde.SetEnterpriseID = user.EnterpriseID;
                                excelde.SetUserID = user.UserID;
                                excelde.SetUserName = user.EnterpriseName;
                                excelde.Status = (int)Common.EnumFile.Status.used;
                            #endregion
                                dataContext.PRRU_EnEquipmentInfo.InsertOnSubmit(excelde);
                                dataContext.SubmitChanges();
                            }
                        }
                        result.Msg = strBuilder.ToString();
                        result.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    result.Msg = "上传失败，请检查数据格式！";
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="peid">上级监管企业ID</param>
        /// <returns></returns>
        public List<Enterprise_Info> GetEnterpriseList(long peid)
        {
            List<Enterprise_Info> dataInfoList = new List<Enterprise_Info>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.Enterprise_Info
                                   where data.PRRU_PlatForm_ID == peid && data.Status == (int)Common.EnumFile.Status.used
                                   select data;
                    dataInfoList = dataList.ToList();
                }
            }
            catch { }
            return dataInfoList;
        }
        #endregion

        #region 获取企业扩展表打码客户端用的简版/完整版
        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public EnterpriseShopLink GetEnKhd(long eid)
        {
            EnterpriseShopLink result;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                result = dataContext.EnterpriseShopLink.FirstOrDefault(m => m.EnterpriseID == eid);
            }
            return result;
        }

        /// <summary>
        /// 设置使用客户端是简版/完整版(type=2完整版，1简版)
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public RetResult SetKHDType(long eid, int type)
        {
            LoginInfo pf = SessCokie.GetMan;
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    EnterpriseShopLink enterprise = dataContext.EnterpriseShopLink.FirstOrDefault(m => m.EnterpriseID == eid);
                    if (enterprise != null)
                    {
                        if (type == 1)
                        {
                            enterprise.IsSimple = (int)EnumFile.EnKHDType.Simple;
                        }
                        else if (type == 2)
                        {
                            enterprise.IsSimple = (int)EnumFile.EnKHDType.Complete;
                        }
                        else if (type == 3)
                        {
                            enterprise.IsSimple = (int)EnumFile.EnKHDType.Standard;
                        }
                        else
                        {
                            enterprise.IsSimple = (int)EnumFile.EnKHDType.JYEnterprise;
                        }
                        dataContext.SubmitChanges();
                        //
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        EnterpriseShopLink en = new EnterpriseShopLink();
                        en.AddDate = DateTime.Now;
                        en.AddUser = pf.UserID;
                        en.EnterpriseID = eid;
                        if (type == 1)
                        {
                            en.IsSimple = (int)Common.EnumFile.EnKHDType.Simple;
                        }
                        else if (type == 2)
                        {
                            en.IsSimple = (int)EnumFile.EnKHDType.Complete;
                        }
                        else if (type == 3)
                        {
                            en.IsSimple = (int)EnumFile.EnKHDType.Standard;
                        }
                        else
                        {
                            en.IsSimple = (int)EnumFile.EnKHDType.JYEnterprise;
                        }
                        dataContext.EnterpriseShopLink.InsertOnSubmit(en);
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        public RetResult SetJKToken(long eid, string token, string tokencode)
        {
            LoginInfo pf = SessCokie.GetMan;
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    EnterpriseShopLink enterprise = dataContext.EnterpriseShopLink.FirstOrDefault(m => m.EnterpriseID == eid);
                    if (enterprise != null)
                    {
                        enterprise.access_token = token;
                        enterprise.access_token_code = tokencode;
                        dataContext.SubmitChanges();
                        //
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        EnterpriseShopLink en = new EnterpriseShopLink();
                        en.AddDate = DateTime.Now;
                        en.AddUser = pf.UserID;
                        en.EnterpriseID = eid;
                        en.IsSimple = (int)EnumFile.EnKHDType.Complete;
                        en.access_token = token;
                        en.access_token_code = tokencode;
                        dataContext.EnterpriseShopLink.InsertOnSubmit(en);
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }
        #endregion

        /// <summary>
        /// 2021-9-29获取子用户配置的DI信息
        /// 还未做完，先做GS1企业
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<string> GetSubUserDI(long uid)
        {
            List<string> result = new List<string>();
            List<EnterprsieDI_User> model = new List<EnterprsieDI_User>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                model = dataContext.EnterprsieDI_User.Where(m => m.Enterprise_User_ID == uid &&
                    m.Status == (int)EnumFile.Status.used).ToList();
            }
            foreach (var sub in model)
            {
                result.Add(sub.MaterialUDIDI);
            }
            return result;
        }
        /// <summary>
        /// 获取GS1企业列表 21-10-19
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_EnterpriseInfoUser> GetEnterpriseInfoListGS1(string name, long eId, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<View_EnterpriseInfoUser> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_EnterpriseInfoUser select data;
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.EnterpriseName.Contains(name));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.adddate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.adddate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    dataInfoList = dataList.Where(p => p.Verify == (int)EnumFile.EnterpriseVerify.gs1).OrderByDescending(m => m.Enterprise_Info_ID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        public LinqModel.Enterprise_Info GetModelInfo(long eId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.Enterprise_Info
                                where d.Enterprise_Info_ID == eId
                                select d).FirstOrDefault();
                    if (data != null)
                    {
                        Enterprise_License license = dataContext.Enterprise_License.Where(p => p.EnterpriseID == eId).FirstOrDefault();
                        if (license != null && !string.IsNullOrEmpty(license.LicenseEndDate.ToString()))
                        {
                            data.LicenseEndDate = Convert.ToDateTime(license.LicenseEndDate.ToString()).ToString("yyyy-MM-dd");
                        }
                        Enterprise_SetMoule setModule = dataContext.Enterprise_SetMoule.Where(p => p.Entetprise_Info_ID == eId).FirstOrDefault();
                        if (setModule != null)
                        {
                            data.SetClient = setModule.SetClient == null ? (int)Common.EnumFile.ShopVerify.Close : (int)setModule.SetClient;
                            data.SetSy = setModule.SetClient == null ? (int)Common.EnumFile.ShopVerify.Close : (int)setModule.SetSy;
                        }
                        ClearLinqModel(data);
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 新增GS1企业
        /// </summary>
        /// <returns></returns>
        public RetResult AddGS(Enterprise_Info model, Enterprise_User user, string youxiaoDate)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    if (dataContext.Connection != null)
                        dataContext.Connection.Open();
                    DbTransaction tran = dataContext.Connection.BeginTransaction();
                    dataContext.Transaction = tran;
                    Enterprise_Info plat = dataContext.Enterprise_Info.Where(p => p.EnterpriseName == model.EnterpriseName).FirstOrDefault();
                    if (plat != null)
                    {
                        Msg = "企业名称重复！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                    dataContext.Enterprise_Info.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Enterprise_License license = new Enterprise_License();
                    license.EnterpriseID = model.Enterprise_Info_ID;
                    license.LicenseEndDate = Convert.ToDateTime(youxiaoDate);
                    license.OperateDate = DateTime.Now;
                    license.State = (int)EnumFile.Status.used;
                    dataContext.Enterprise_License.InsertOnSubmit(license);
                    user.Enterprise_Info_ID = model.Enterprise_Info_ID;
                    user.LoginName = "gs1" + model.Enterprise_Info_ID.ToString();
                    user.LoginPassWord = "123456";
                    dataContext.Enterprise_User.InsertOnSubmit(user);
                    dataContext.SubmitChanges();
                    tran.Commit();
                    Msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public RetResult EditGS(Enterprise_Info model, string youxiaoDate)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info plat = dataContext.Enterprise_Info.Where(p => p.EnterpriseName == model.EnterpriseName && p.Enterprise_Info_ID != model.Enterprise_Info_ID).FirstOrDefault();
                    if (plat != null)
                    {
                        Msg = "企业名称重复！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                    plat = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == model.Enterprise_Info_ID).FirstOrDefault();
                    if (plat != null)
                    {
                        plat.EnterpriseName = model.EnterpriseName;
                        plat.Dictionary_AddressQu_ID = model.Dictionary_AddressQu_ID;
                        plat.Dictionary_AddressSheng_ID = model.Dictionary_AddressSheng_ID;
                        plat.Dictionary_AddressShi_ID = model.Dictionary_AddressShi_ID;
                        plat.Address = model.Address;
                        plat.BusinessLicence = model.BusinessLicence;
                        plat.LinkMan = model.LinkMan;
                        plat.LinkPhone = model.LinkPhone;
                        Enterprise_License license = dataContext.Enterprise_License.Where(p => p.EnterpriseID == model.Enterprise_Info_ID).FirstOrDefault();
                        if (license != null)
                        {
                            license.LicenseEndDate = Convert.ToDateTime(youxiaoDate);
                        }
                        else
                        {
                            license = new Enterprise_License();
                            license.State = (int)Common.EnumFile.Status.used;
                            license.EnterpriseID = model.Enterprise_Info_ID;
                            license.LicenseEndDate = Convert.ToDateTime(youxiaoDate);
                            license.OperateDate = DateTime.Now;
                            dataContext.Enterprise_License.InsertOnSubmit(license);
                        }
                        dataContext.SubmitChanges();
                        Msg = "保存成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public RetResult EditStatus(long id, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info model = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == id);
                    if (model == null)
                    {
                        Ret.SetArgument(CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    if (type == 1)
                    {
                        model.Status = (int)Common.EnumFile.Status.used;
                    }
                    else
                    {
                        model.Status = (int)Common.EnumFile.Status.delete;
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, null, "操作成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }

        #region 维护企业是否开通子企业功能
        /// <summary>
        /// 开通子企业的企业列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PagedList<View_Enterprise_SetMoule> GetSubEnterpriseList(string name, int? pageIndex)
        {
            PagedList<View_Enterprise_SetMoule> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_Enterprise_SetMoule
                                   where data.SetSub == (int)Common.EnumFile.ShopVerify.Open
                                   select data;
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(p => p.EnterpriseName.Contains(name));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.Entetprise_Info_ID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }
        /// <summary>
        /// 为企业开通子用户功能
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public RetResult Add(Enterprise_SetMoule model)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_SetMoule temp = dataContext.Enterprise_SetMoule.FirstOrDefault(p => p.Entetprise_Info_ID == model.Entetprise_Info_ID);
                    if (temp != null)
                    {
                        temp.SetSub = (int)Common.EnumFile.ShopVerify.Open;
                    }
                    else
                    {
                        dataContext.Enterprise_SetMoule.InsertOnSubmit(model);
                    }
                    dataContext.SubmitChanges();
                    dataContext.SubmitChanges();
                    Msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 修改企业开通子企业状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public RetResult EditSubStatus(long id, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_SetMoule model = dataContext.Enterprise_SetMoule.FirstOrDefault(m => m.Entetprise_Info_ID == id);
                    if (model == null)
                    {
                        Ret.SetArgument(CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    if (type == 1)
                    {
                        model.SetSub = (int)Common.EnumFile.ShopVerify.Open;
                    }
                    else
                    {
                        model.SetSub = (int)Common.EnumFile.ShopVerify.Close;
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, null, "操作成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }
        #endregion

        #region 20211230企业设置码制
        public RetResult SetCodeType(long eid, int CodeType)
        {
            LoginInfo pf = SessCokie.GetMan;
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eid);
                    if (enterprise != null)
                    {
                        enterprise.CodeType = CodeType;
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }
        #endregion

        public RetResult SetTokenForCilent(string loginname, string password, string token, string tokencode)
        {
            LoginInfo pf = SessCokie.GetMan;
            string Msg = "操作失败！";
            CmdResultError CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var model = (from d in dataContext.Enterprise_User
                                 where d.LoginName == loginname && d.LoginPassWord == password
                                 select d).FirstOrDefault();
                    if (model != null)
                    {
                        long? eid = model.Enterprise_Info_ID;
                        EnterpriseShopLink enterprise = dataContext.EnterpriseShopLink.FirstOrDefault(m => m.EnterpriseID == eid);
                        if (enterprise != null)
                        {
                            enterprise.access_token = token;
                            enterprise.access_token_code = tokencode;
                            Msg = "操作成功！";
                            CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            EnterpriseShopLink en = new EnterpriseShopLink();
                            en.AddDate = DateTime.Now;
                            en.AddUser = 1;
                            en.EnterpriseID = eid;
                            en.IsSimple = (int)EnumFile.EnKHDType.Complete;
                            en.access_token = token;
                            en.access_token_code = tokencode;
                            dataContext.EnterpriseShopLink.InsertOnSubmit(en);
                            Msg = "操作成功！";
                            CmdError = CmdResultError.NONE;
                        }
                        //更新授权码信息
                        string setDatestr = DateTime.Now.AddYears(1).ToString("yyyyMMdd");
                        string licenseCode = setDatestr;
                        AuthorCode author = dataContext.AuthorCode.Where(p => p.LicenseType == 1).FirstOrDefault();
                        if (author == null)
                        {
                            Ret.SetArgument(CmdError, Msg, Msg);
                            return Ret;
                        }
                        Enterprise_Info enterpriseInfo = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == eid).FirstOrDefault();
                        if (enterpriseInfo == null)
                        {
                            Ret.SetArgument(CmdError, Msg, Msg);
                            return Ret;
                        }
                        licenseCode = licenseCode + "&" + author.appId + "&" + author.appSecret;
                        byte[] key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["key"]);
                        byte[] iv = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["iv"]);
                        licenseCode = SecurityDES.Encrypt(licenseCode, key, iv);
                        Enterprise_License license = new Enterprise_License();
                        license.State = (int)Common.EnumText.Status.used;
                        license.LicenseEndDate = DateTime.Now.AddYears(1);
                        license.LicenseCode = licenseCode;
                        license.EnterpriseID = eid;
                        license.EnterpriseName = enterpriseInfo.EnterpriseName;
                        license.OperateDate = DateTime.Now;
                        license.AdminID = 1;
                        license.LicenseType = 1;
                        #region 生成TXT文档
                        string filePath = string.Format(AppDomain.CurrentDomain.BaseDirectory + "File");
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string docFileName = Guid.NewGuid().ToString() + ".txt";
                        using (StreamWriter sw = new StreamWriter(filePath + "\\" + docFileName, true))
                        {
                            sw.WriteLine(licenseCode);
                        }
                        #endregion
                        license.LicenseTxt = filePath + "\\" + docFileName;
                        license.LicenseTxtURL = "/" + "File" + "/" + docFileName;
                        Enterprise_License enter_License = dataContext.Enterprise_License.FirstOrDefault(m => m.EnterpriseID == eid && m.State != -1);
                        if (enterprise != null)
                        {
                            enter_License.State = (int)Common.EnumText.Status.delete;
                            dataContext.Enterprise_License.InsertOnSubmit(license);
                        }
                        else
                        {
                            dataContext.Enterprise_License.InsertOnSubmit(license);
                        }
                        dataContext.SubmitChanges();
                        Msg = "操作成功！";
                        CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        //若企业信息不存在，则先判断该企业是否真的存在

                        Msg = "用户名密码不正确！";
                    }
                   
                }
            }
            catch
            {
            }
            Ret.SetArgument(CmdError, Msg, Msg);
            return Ret;
        }

        public string GetEnterpriseIsExpired(long enterpriseID)
        {
            Enterprise_License elmo;
            string result = "";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                elmo = dataContext.Enterprise_License.Where(t => t.EnterpriseID == enterpriseID).OrderByDescending(t => t.OperateDate).FirstOrDefault();
                if (elmo != null)
                {
                    if (elmo.StrLicenseEndDate.ToString() != "")
                    {
                        if (DateTime.Now > DateTime.Parse(elmo.StrLicenseEndDate))
                        {
                            result = "授权已到期";
                        }
                        else
                        {
                            result = "10086";
                        }
                    }
                    else
                    {
                        result = "授权已到期";
                    }
                   
                }
            }
            return result;
            
        }
    }
}
