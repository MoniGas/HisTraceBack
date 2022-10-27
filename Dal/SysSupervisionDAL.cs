using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    public class SysSupervisionDAL : DALBase
    {
        /// <summary>
        /// 获取监管部门信息列表方法
        /// </summary>
        /// <param name="sName">监管部门名称</param>
        /// <param name="selStatus">监管部门帐号状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">列表总数</param>
        /// <returns>反回查询结果</returns>
        public PagedList<View_PRRU_PlatFormUser> GetEnterpriseList(string sName, string selStatus, int? pageIndex, out long totalCount)
        {
            totalCount = 0;
            LoginInfo pf = SessCokie.GetMan;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_PRRU_PlatFormUser
                                   where data.PRRU_PlatFormLevel_ID != 4  
                                   select data;
                    if (pf.PRRU_PlatFormLevel_ID == 4)
                    {
                        dataList = from data in dataContext.View_PRRU_PlatFormUser
                                   where data.PRRU_PlatFormLevel_ID != 4 && data.Dictionary_AddressSheng_ID == pf.shengId
                                   select data;
                    }
                    if (!string.IsNullOrEmpty(selStatus) && selStatus != "0")
                    {
                        dataList = dataList.Where(w => w.UserStatus == Convert.ToInt32(selStatus));
                    }
                    if (!string.IsNullOrEmpty(sName))
                    {
                        dataList = dataList.Where(w => w.CompanyName.Contains(sName));
                    }
                    totalCount = dataList.Count();
                    dataList = dataList.OrderByDescending(m => m.PRRU_PlatForm_ID);
                    return dataList.ToPagedList(pageIndex ?? 1, PageSize);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 新增监管部门信息方法
        /// </summary>
        /// <param name="objPRRU_PlatForm">监管部门信息对象</param>
        /// <param name="objobjPRRU_PlatForm_User">监管部门登录帐号对象</param>
        /// <returns></returns>
        public RetResult Add(LinqModel.PRRU_PlatForm objPRRU_PlatForm, LinqModel.PRRU_PlatForm_User objobjPRRU_PlatForm_User)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    PRRU_PlatForm plat = dataContext.PRRU_PlatForm.Where(p => p.CompanyName == objPRRU_PlatForm.CompanyName).FirstOrDefault();
                    if (plat != null)
                    {
                        Msg = "代理商名称重复！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
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
        /// 修改监管部门信息方法
        /// </summary>
        /// <param name="objPRRU_PlatForm">监管部门信息对象</param>
        /// <returns></returns>
        public RetResult Edit(LinqModel.PRRU_PlatForm objPRRU_PlatForm)
        {
            string Msg = "修改失败！";
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

                        Msg = "修改成功！";
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
                Msg ="发生异常错误";
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 修改密码方法
        /// </summary>
        /// <param name="id">监管部门ID</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <param name="surepwd">确认密码</param>
        /// <returns>反回修改密码结果</returns>
        public RetResult UpdatePass(long id, string oldPwd, string newPwd, string surepwd)
        {
            string Msg = "更新失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.PRRU_PlatForm_User
                                    where data.PRRU_PlatForm_User_ID == id
                                    select data).FirstOrDefault();

                    if (!dataInfo.LoginPassWord.Equals(oldPwd))
                    {
                        Msg = "更新失败！旧密码错误！";
                        error = CmdResultError.PARAMERROR;
                    }
                    else
                    {
                        dataInfo.LoginPassWord = newPwd;

                        dataContext.SubmitChanges();

                        Msg = "修改成功，请重新登录！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch (Exception e)
            {
                Msg = "连接数据库失败！";
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public long? GetMaxId()
        {
            long id = 1;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var maxId = (from data in dataContext.PRRU_PlatForm_User
                                 select data.PRRU_PlatForm_ID).Max();

                    return id + maxId;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据ID获取监管部门信息方法
        /// </summary>
        /// <param name="eId">监管部门ID</param>
        /// <returns>获取的信息集合</returns>
        public LinqModel.PRRU_PlatForm GetModelInfo(long eId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_PlatForm
                                where d.PRRU_PlatForm_ID == eId
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
        /// 根据ID获取监管部门信息方法
        /// </summary>
        /// <param name="eId">监管部门ID</param>
        /// <returns>获取的信息集合</returns>
        public LinqModel.View_PRRU_PlatFormUser GetPlatInfo(long eId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.View_PRRU_PlatFormUser
                                where d.PRRU_PlatForm_ID == eId
                                select d).FirstOrDefault();
                    if (data != null)
                    {
                        List<View_EnterpriseInfoUser> users = dataContext.View_EnterpriseInfoUser.Where(p => p.PRRU_PlatForm_ID == eId).ToList();
                        if (users != null)
                        {
                            data.EnterprsieCount = users.Count();
                            foreach (var sub in users)
                            {
                                data.CodeCount = data.CodeCount +(long)sub.UsedCodeCount;
                            }
                        }
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
        /// 禁用监管部门登录帐号方法
        /// </summary>
        /// <param name="userId">监管部门帐号ID</param>
        /// <returns>禁用结果</returns>
        public RetResult DisableSupervision(long userId)
        {
            string Msg = "禁用失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_PlatForm_User
                                where d.PRRU_PlatForm_User_ID == userId
                                select d).FirstOrDefault();

                    if (data != null)
                    {
                        data.Status = (int)Common.EnumFile.ViewType.Disable;
                        dataContext.SubmitChanges();
                        Msg = "禁用成功！";
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
        /// <summary>
        /// 启用监管部门帐号方法
        /// </summary>
        /// <param name="userId">监管部门帐号ID</param>
        /// <returns>启用结果</returns>
        public RetResult EnableSupervision(long userId)
        {
            string Msg = "启用失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_PlatForm_User
                                where d.PRRU_PlatForm_User_ID == userId
                                select d).FirstOrDefault();

                    if (data != null)
                    {
                        data.Status = (int)Common.EnumFile.ViewType.Enable;
                        dataContext.SubmitChanges();
                        Msg = "启用成功！";
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
        /// <summary>
        /// 重置密码方法
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public RetResult ResetPassword(long userId)
        {
            string Msg = "密码重置失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_PlatForm_User
                                where d.PRRU_PlatForm_User_ID == userId
                                select d).FirstOrDefault();

                    if (data != null)
                    {
                        data.LoginPassWord = "admin";
                        dataContext.SubmitChanges();
                        Msg = "密码重置成功，密码为admin！";
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

        /// <summary>
        /// 根据ID获取监管部门信息方法
        /// </summary>
        /// <param name="eId">监管部门ID</param>
        /// <returns>获取的信息集合</returns>
        public LinqModel.PRRU_PlatForm_User GetPUser(long uId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_PlatForm_User
                                where d.PRRU_PlatForm_User_ID == uId
                                select d).FirstOrDefault();
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
