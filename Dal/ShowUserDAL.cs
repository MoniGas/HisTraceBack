/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-24

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于名片宣传码信息管理数据层

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;

namespace Dal
{
    public class ShowUserDAL : DALBase
    {
        /// <summary>
        /// 获取名片列表
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <param name="name">人员名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<ShowUser> GetList(long companyId, string name, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<ShowUser> result = null;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                var data = from m in dataContext.ShowUser
                           where m.CompanyID == companyId && m.Status == 1
                           select m;
                if (!string.IsNullOrEmpty(name))
                {
                    data = data.Where(m => m.Infos.Contains(name.Trim()));
                }
                data = data.OrderByDescending(m => m.UserID);
                totalCount = data.Count();
                result = data.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();

                ClearLinqModel(result);
            }
            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">名片标识</param>
        /// <returns>实体</returns>
        public ShowUser GetModel(long id)
        {
            ShowUser result = null;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                result = dataContext.ShowUser.FirstOrDefault(m => m.UserID == id);

                List<System.Xml.Linq.XElement> infos = result.InfoOther.Descendants("info").ToList();

                result.position = infos.FirstOrDefault(m => m.Attribute("name").Value == "position").Attribute("value").Value;
                result.telPhone = infos.FirstOrDefault(m => m.Attribute("name").Value == "telPhone").Attribute("value").Value;
                result.mail = infos.FirstOrDefault(m => m.Attribute("name").Value == "mail").Attribute("value").Value;
                result.qq = infos.FirstOrDefault(m => m.Attribute("name").Value == "qq").Attribute("value").Value;
                result.hometown = infos.FirstOrDefault(m => m.Attribute("name").Value == "hometown").Attribute("value").Value;
                result.location = infos.FirstOrDefault(m => m.Attribute("name").Value == "location").Attribute("value").Value;
                result.memo = infos.FirstOrDefault(m => m.Attribute("name").Value == "memo").Attribute("value").Value;
                result.headimg = infos.FirstOrDefault(m => m.Attribute("name").Value == "headimg").Attribute("value").Value;
                ClearLinqModel(result);
            }
            return result;
        }

        /// <summary>
        /// 添加名片
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Add(ShowUser model)
        {
            int Verify = 0;
            using (DataClassesDataContext WebContext = GetDataContext())
            {
                Enterprise_Info EnterpriseInfo = WebContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == model.CompanyID);
                Verify = (int)EnterpriseInfo.Verify;
            }
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    string mainCode = model.EWM;
                    dataContext.ShowUser.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    model.EWM = model.EWM + "." + (int)Common.EnumFile.TerraceEwm.showUser + "." + model.UserID;
                    string config = "cardURL";
                    model.Url = System.Configuration.ConfigurationManager.AppSettings[config] + model.EWM;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "添加员工名片成功！", "添加员工名片成功！");
                }
                catch(Exception e)
                {
                    Ret.SetArgument(CmdResultError.EXCEPTION, e.Message, e.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 修改名片
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="sub">扩展属性</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(ShowUser model, List<ShowUserAttributes> sub)
        {
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowUser user = dataContext.ShowUser.FirstOrDefault(m => m.UserID == model.UserID);
                    if (user == null)
                    {
                        Ret.Msg = "员工名片信息不存在！";
                    }
                    else
                    {
                        user.Infos = model.Infos;
                        user.InfoOther = model.InfoOther;

                        dataContext.ShowUserAttributes.DeleteAllOnSubmit(dataContext.ShowUserAttributes.Where(m => m.UserID == model.UserID));
                        dataContext.ShowUserAttributes.InsertAllOnSubmit(sub);

                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "修改员工名片信息成功！", "修改员工名片信息成功！");
                    }
                }
                catch(Exception e)
                {
                    Ret.SetArgument(CmdResultError.EXCEPTION, e.Message, e.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 删除名片
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <param name="id">明天标识</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long companyId, long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowUser user = dataContext.ShowUser.FirstOrDefault(m => m.UserID == id && m.CompanyID == companyId);
                    if (user == null)
                    {
                        Ret.SetArgument(CmdResultError.NO_RESULT, "获取数据失败！", "获取数据失败！");
                    }
                    else
                    {
                        user.Status = 0;
                        //dataContext.ShowUser.DeleteOnSubmit(user);
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "删除成功！", "删除成功！");
                    }
                }
                catch(Exception e)
                {
                    Ret.SetArgument(CmdResultError.EXCEPTION, e.Message, e.Message);
                }
            }
            return Ret;
        }
    }


    public class ShowUserAttributesDAL : DALBase
    {
        public List<ShowUserAttributes> GetList(long userID, int pageIndex, out long totalCount)
        {
            List<ShowUserAttributes> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                var data = from m in dataContext.ShowUserAttributes where m.UserID == userID select m;
                data = data.OrderByDescending(m => m.UserID);
                totalCount = data.Count();
                result = data.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
            }
            return result;
        }
        public RetResult Add(ShowUserAttributes model)
        {
            Ret.Msg = "添加用户属性失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    dataContext.ShowUserAttributes.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    dataContext.SubmitChanges();
                    Ret.Msg = "添加用户属性成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch
                {
                    Ret.Msg = "操作中出现错误！";
                }
            }
            return Ret;
        }
        public RetResult Edit(ShowUserAttributes model)
        {
            Ret.Msg = "修改用户属性失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowUserAttributes userattributes = dataContext.ShowUserAttributes.FirstOrDefault(m => m.UserID == model.UserID);
                    if (userattributes == null)
                    {
                        Ret.Msg = "用户信息不存在！";
                    }
                    else
                    {
                        userattributes.Title = model.Title;
                        userattributes.Contents = model.Contents;
                        userattributes.AddTime = model.AddTime;
                        dataContext.SubmitChanges();
                        Ret.Msg = "修改用户属性成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Ret.Msg = "操作中出现错误！";
                }
            }
            return Ret;
        }
        public ShowUserAttributes GetModelByid(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                return dataContext.ShowUserAttributes.FirstOrDefault(m => m.UserID == id);
            }
        }
    }
}