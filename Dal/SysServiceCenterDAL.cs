/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-22

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 主要用于服务中心数据管理

*********************************************************************************/   
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 主要用于服务中心数据管理
    /// </summary>
    public class SysServiceCenterDAL : DALBase
    {
        /// <summary>
        /// 获取服务中心列表
        /// </summary>
        /// <param name="sName">服务中心名称</param>
        /// <param name="selStatus"></param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">列表总数</param>
        /// <returns>返回查询结果</returns>
        public List<LinqModel.View_PRRU_PlatFormUser> GetEnterpriseList(string sName, string selStatus,
            int? pageIndex, out long totalCount)
        {
            List<LinqModel.View_PRRU_PlatFormUser> dataInfoList = null;
            totalCount = 0;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_PRRU_PlatFormUser
                                   where data.PRRU_PlatFormLevel_ID == 4
                                   select data;
                    if (!string.IsNullOrEmpty(selStatus) && selStatus != "0")
                    {
                        dataList = dataList.Where(w => w.UserStatus == Convert.ToInt32(selStatus));
                    }
                    if (!string.IsNullOrEmpty(sName))
                    {
                        dataList = dataList.Where(w => w.CompanyName.Contains(sName));
                    }
                    totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.PRRU_PlatForm_ID).Skip((pageIndex.Value - 1) * PageSize).
                        Take(PageSize).ToList();
                    ClearLinqModel(dataInfoList);
                }
            }
            catch 
            {
            }
            return dataInfoList;
        }

        /// <summary>
        /// 添加服务中心
        /// </summary>
        /// <param name="objPRRU_PlatForm">服务中心信息对象</param>
        /// <param name="objobjPRRU_PlatForm_User">服务中心登录帐号对象</param>
        /// <returns></returns>
        public RetResult Add(LinqModel.PRRU_PlatForm objPRRU_PlatForm, LinqModel.PRRU_PlatForm_User objobjPRRU_PlatForm_User)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.PRRU_PlatForm.InsertOnSubmit(objPRRU_PlatForm);
                    dataContext.SubmitChanges();
                    dataContext.PRRU_PlatForm_User.InsertOnSubmit(objobjPRRU_PlatForm_User);
                    dataContext.SubmitChanges();
                    var data = (from d in dataContext.PRRU_PlatForm_User
                                where d.PRRU_PlatForm_User_ID == objobjPRRU_PlatForm_User.PRRU_PlatForm_User_ID
                                select d).FirstOrDefault();
                    data.LoginName += objobjPRRU_PlatForm_User.PRRU_PlatForm_User_ID.ToString();
                    data.PRRU_PlatForm_ID = objPRRU_PlatForm.PRRU_PlatForm_ID;
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
        /// 修改服务中心
        /// </summary>
        /// <param name="objPRRU_PlatForm">服务中心实体</param>
        /// <returns></returns>
        public RetResult Edit(LinqModel.PRRU_PlatForm objPRRU_PlatForm)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_PlatForm
                                where d.PRRU_PlatForm_ID == objPRRU_PlatForm.PRRU_PlatForm_ID
                                select d).FirstOrDefault();
                    if (data != null)
                    {
                        data.CompanyName = objPRRU_PlatForm.CompanyName;
                        data.Dictionary_AddressSheng_ID = objPRRU_PlatForm.Dictionary_AddressSheng_ID;
                        data.Dictionary_AddressShi_ID = objPRRU_PlatForm.Dictionary_AddressShi_ID;
                        data.Dictionary_AddressQu_ID = objPRRU_PlatForm.Dictionary_AddressQu_ID;
                        data.CenterAddress = objPRRU_PlatForm.CenterAddress;
                        data.LinkMan = objPRRU_PlatForm.LinkMan;
                        data.LinkPhone = objPRRU_PlatForm.LinkPhone;
                        data.ComplaintPhone = objPRRU_PlatForm.ComplaintPhone;
                        data.PostalCode = objPRRU_PlatForm.PostalCode;
                        data.Email = objPRRU_PlatForm.Email;
                        data.WebURL = objPRRU_PlatForm.WebURL;
                        data.lastdate = DateTime.Now;
                        dataContext.SubmitChanges();
                        Msg = "保存成功！";
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "数据不存在！";
                        error = CmdResultError.EXCEPTION;
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
    }
}
