/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-4

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于消息查询数据层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Argument;
using LinqModel;
using System.Configuration;

namespace Dal
{
    public class NewsDAL : DALBase
    {
        #region 获取申请二维码的结果消息
        /// <summary>
        /// 获取申请二维码的结果消息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public List<View_RequestCodeAndEnterprise_Info> GetEMWList(long id, out int dataCount)
        {
            dataCount = 0;
            List<View_RequestCodeAndEnterprise_Info> dataList = new List<View_RequestCodeAndEnterprise_Info>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataList = (from data in dataContext.View_RequestCodeAndEnterprise_Info 
                                where data.Status != 1040000001 && data.IsRead == (int)EnumFile.IsRead.noRead && data.Enterprise_Info_ID == id
                                orderby data.RequestDate descending
                                select data).Take(5).ToList();
                    dataCount = dataList.Count;
                    ClearLinqModel(dataList);
                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region 修改申请二维码后的消息为已读
        /// <summary>
        /// 修改申请二维码后的消息为已读
        /// </summary>
        /// <param name="id">申请二维码消息ID</param>
        /// <returns></returns>
        public RetResult UpdateStatus(long id)
        {
            string Msg = "修改失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.RequestCode
                                    where data.RequestCode_ID == id
                                    select data).FirstOrDefault();

                    if (dataInfo != null)
                    {
                        dataInfo.IsRead = (int)EnumFile.IsRead.isRead;
                    }
                    dataContext.SubmitChanges();
                    Msg = "修改成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                Msg = "修改状态失败！";
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #endregion

        public RetResult IgnoreEwm()
        {
            string Msg = "修改失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.RequestCode
                                    where data.IsRead == (int)EnumFile.IsRead.noRead
                                    select data).ToList();
                    foreach (RequestCode data in dataInfo)
                    {
                        if (data != null)
                        {
                            data.IsRead = (int)EnumFile.IsRead.isRead;
                        }
                    }
                    dataContext.SubmitChanges();
                    Msg = "修改成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                Msg = "修改状态失败！";
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        #region 获取申请加入区域品牌消息列表
        /// <summary>
        /// 获取申请加入区域品牌消息列表
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public List<View_RequestBrand> GetBrandList(long id, out int dataCount)
        {
            dataCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.View_RequestBrand
                                    where data.BrandStatus == 0 && data.BrandEnterpriseEID == id && data.BrandEnterpriseStatus.Value == Convert.ToInt16(EnumFile.PlatFormState.pass) && data.IsRead == (int)EnumFile.IsRead.noRead
                                    select data).ToList();
                    dataCount = dataInfo.Count;

                    if (dataInfo != null)
                    {
                        dataInfo = dataInfo.OrderByDescending(o => o.lastdate.Value).ToList();
                    }

                    ClearLinqModel(dataInfo);
                    return dataInfo;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region 修改申请加入区域品牌结果为已读
        /// <summary>
        /// 修改申请加入区域品牌结果为已读
        /// </summary>
        /// <param name="id">申请加入区域品牌ID</param>
        /// <returns></returns>
        public RetResult UpdateBrand(long id)
        {
            CmdResultError error = CmdResultError.EXCEPTION;

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.Brand_Enterprise
                                    where data.Brand_Enterprise_ID == id
                                    select data).FirstOrDefault();

                    if (dataInfo != null)
                    {
                        dataInfo.IsRead = (int)EnumFile.IsRead.isRead;
                    }

                    dataContext.SubmitChanges();

                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");

            return Ret;
        }
        #endregion

        public RetResult IgnoreBrand()
        {
            CmdResultError error = CmdResultError.EXCEPTION;

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.Brand_Enterprise
                                    where data.IsRead == (int)EnumFile.IsRead.noRead
                                    select data).ToList();
                    foreach (Brand_Enterprise data in dataInfo)
                    {
                        if (data != null)
                        {
                            data.IsRead = (int)EnumFile.IsRead.isRead;
                        }
                    }
                    dataContext.SubmitChanges();

                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");

            return Ret;
        }

        public List<EnterpriseVerify> GetEnterpriseVerifyList(long id, out int dataCount)
        {
            dataCount = 0;
            List<EnterpriseVerify> result = new List<EnterpriseVerify>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = dataContext.EnterpriseVerify.Where(m => m.Enterprise_ID == id);

                    if (dataInfo != null && dataInfo.Count() > 0)
                    {
                        dataCount = dataInfo.Count();
                        result = dataInfo.OrderByDescending(o => o.AddDate.Value).ToList();
                    }
                    else
                    {
                        Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == id);
                        //OrganUnitStatusInfo statusInfo = InterfaceWeb.BaseDataDAL.GetStatus(enterprise.MainCode);
                        string tryDays = ConfigurationManager.AppSettings["trydays"];
                        //审核状态(-1 审核失败，0 未审核，1 审核中，100 审核通过成为正式用户)
                        //if (statusInfo.IsAudit != 100)
                        //{
                        EnterpriseVerify v = new EnterpriseVerify();
                        if (enterprise.AddTime.GetValueOrDefault(DateTime.Now).AddDays(int.Parse(tryDays)) < DateTime.Now)
                        {
                            v.AddDate = DateTime.Now;
                            v.Enterprise_ID = id;
                            v.VerifyContent = "您的企业已经超过试用期！请联系管理员！";
                        }
                        //    else
                        //    {
                        //        v.AddDate = DateTime.Now;
                        //        v.Enterprise_ID = id;
                        //        v.VerifyContent = "您的企业还未认证！请尽快完成企业认证！";
                        //    }
                        //    result.Add(v);
                        //}
                        //else
                        //{
                            //-2暂停；-1审核不通过；0未审核；1正常、审核通过
                            if (enterprise.Verify == (int)EnumFile.EnterpriseVerify.noVerify)
                            {
                                v.AddDate = DateTime.Now;
                                v.Enterprise_ID = id;
                                TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(enterprise.AddTime.GetValueOrDefault(DateTime.Now).AddDays(int.Parse(tryDays)).ToShortDateString()).Ticks);
                                TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(DateTime.Now.ToShortDateString()).Ticks);
                                TimeSpan ts3 = ts1.Subtract(ts2).Duration();
                                v.VerifyContent = "您的企业还在试用期！还可以试用"+ ts3.TotalDays +"天！请联系管理员！";
                                result.Add(v);
                            }
                        //}
                        dataCount = result.Count();
                    }
                    ClearLinqModel(result);
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public List<EnterpriseVerify> GetCodeRecord(long enterpriseID)
        {
            List<EnterpriseVerify> result = new List<EnterpriseVerify>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var allRecord = dataContext.View_AuditCodeRecord.Where(m => m.EnterpriseID == enterpriseID && m.IsRead == 0).ToList();
                    foreach (var item in allRecord)
                    {
                        EnterpriseVerify model = new EnterpriseVerify();
                        model.AddDate = item.AuditTime;
                        model.Enterprise_ID = enterpriseID;
                        model.VerifyContent = "申请码数量：" + item.ApplyCount + "，审核码数量：" + item.RequestCount;
                        result.Add(model);
                        try
                        {
                            AuditCodeRecord temp = dataContext.AuditCodeRecord.FirstOrDefault(m => m.AuditID == item.AuditID);
                            temp.IsRead = 1;
                            dataContext.SubmitChanges();
                        }
                        catch { }
                    }
                }
            }
            catch { }
            ClearLinqModel(result);
            return result;
        }
    }
}
