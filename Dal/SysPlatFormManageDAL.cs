/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-24

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 主要用于添加监管数据管理层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;

namespace Dal
{
    /// <summary>
    /// 添加监管数据管理层
    /// </summary>
    public class SysPlatFormManageDAL : DALBase
    {
        /// <summary>
        /// 获取该服务中心下的监管部门
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="eId">ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public List<LinqModel.PRRU_PlatForm> GetPlatFormList(string name, long eId, int? pageIndex, out long totalCount)
        {
            List<LinqModel.PRRU_PlatForm> dataInfoList = new List<LinqModel.PRRU_PlatForm>();
            totalCount = 0;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.PRRU_PlatForm
                                   where data.PRRU_PlatFormLevel_ID == 2 && data.Parent_ID == eId &&
                                   data.Status == (int)Common.EnumFile.ViewType.Enable
                                   select data;
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.CompanyName.Contains(name));
                    }
                    totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.PRRU_PlatForm_ID).Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(dataInfoList);
                }
            }
            catch
            {
            }
            return dataInfoList;
        }

        /// <summary>
        /// 关联监管部门列表
        /// </summary>
        /// <param name="shengId">省</param>
        /// <param name="shiId">市</param>
        /// <returns></returns>
        public List<LinqModel.PRRU_PlatForm> GetAreaPlatForm(long shengId, long shiId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = (from data in dataContext.PRRU_PlatForm
                                    where data.Dictionary_AddressSheng_ID == shengId &&
                                        //data.Dictionary_AddressShi_ID == shiId &&
                                         data.PRRU_PlatFormLevel_ID == 2 &&
                                    data.Status == (int)Common.EnumFile.ViewType.Enable
                                    select data).ToList();
                    List<PRRU_PlatForm> templist = new List<PRRU_PlatForm>();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        PRRU_PlatForm temp = new PRRU_PlatForm();
                        temp = dataList[i];
                        var prruinfo = dataContext.PRRU_PlatForm.FirstOrDefault(m => m.PRRU_PlatForm_ID == temp.Parent_ID);
                        if (prruinfo.PRRU_PlatFormLevel_ID != 4)
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

        /// <summary>
        /// 关联监管部门列表（显示选中项）
        /// </summary>
        /// <param name="shengId">省</param>
        /// <param name="shiId">市</param>
        /// <param name="pId">服务中心ID</param>
        /// <returns></returns>
        public List<string> GetAreaPlatForm(long shengId, long shiId, long pId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    List<long> dataList = (from data in dataContext.PRRU_PlatForm
                                           where data.Dictionary_AddressSheng_ID == shengId &&
                                               //data.Dictionary_AddressShi_ID == shiId &&
                                           data.PRRU_PlatFormLevel_ID == 2 && data.Parent_ID == pId &&
                                           data.Status == (int)Common.EnumFile.ViewType.Enable
                                           select data.PRRU_PlatForm_ID).ToList();
                    ClearLinqModel(dataList);
                    //return dataList;
                    return dataList.ConvertAll(o => string.Format("{0}", o));
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取默认服务中心(平台)
        /// </summary>
        /// <returns></returns>
        public PRRU_PlatForm GetPlatForm()
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.PRRU_PlatForm.FirstOrDefault();
            }
        }

        /// <summary>
        /// 保存关联
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="arrayId"></param>
        /// <param name="falseId"></param>
        /// <returns></returns>
        public RetResult Save(long pId, string arrayId, string falseId)
        {
            Ret.Msg = "保存失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    string[] falseArray = falseId.Split(',');
                    string[] idArray = arrayId.Split(',');
                    foreach (string id in falseArray)
                    {
                        if (id == null || id.Trim().Equals(""))
                        {
                            continue;
                        }
                        var data = (from d in dataContext.PRRU_PlatForm
                                    where d.PRRU_PlatForm_ID == Convert.ToInt64(id)
                                    select d).FirstOrDefault();
                        data.Parent_ID = GetPlatForm().PRRU_PlatForm_ID;
                    }
                    foreach (string id in idArray)
                    {
                        if (id == null || id.Trim().Equals(""))
                        {
                            continue;
                        }
                        var data = (from d in dataContext.PRRU_PlatForm
                                    where d.PRRU_PlatForm_ID == Convert.ToInt64(id)
                                    select d).FirstOrDefault();
                        data.Parent_ID = pId;
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

        /// <summary>
        /// 管理员修改自己密码
        /// </summary>
        /// <param name="eId">标识ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="surePassword">确认密码</param>
        /// <returns></returns>
        public RetResult EditPWD(long eId, string oldPassword, string newPassword, string surePassword)
        {
            string Msg = "修改失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.PRRU_PlatForm_User
                                    where data.PRRU_PlatForm_User_ID == eId
                                    select data).FirstOrDefault();

                    if (!dataInfo.LoginPassWord.Equals(oldPassword))
                    {
                        Msg = "修改失败！旧密码错误！";
                        error = CmdResultError.PARAMERROR;
                    }
                    else
                    {
                        dataInfo.LoginPassWord = newPassword;
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
    }
}
