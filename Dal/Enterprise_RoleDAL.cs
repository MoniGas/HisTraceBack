/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-15

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 联系方式 :

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于农企和监管角色权限管理数据层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Argument;
using LinqModel;

namespace Dal
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class Enterprise_RoleDAL : DALBase
    {
        /// <summary>
        /// 根据企业ID获取角色列表 
        /// </summary>
        /// <param name="id">农企ID</param>
        /// <param name="pageIndex">分页码</param>
        /// <param name="totalCount">角色总数</param>
        /// <returns>角色信息集合</returns>
        public List<Enterprise_Role> GetList(long id, string name, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<Enterprise_Role> dataList = new List<Enterprise_Role>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataList = (from data in dataContext.Enterprise_Role
                                where data.Enterprise_Info_ID == id && data.Status == (int)EnumFile.Status.used
                                select data).ToList();
                    if (!string.IsNullOrEmpty(name))
                    {
                        dataList = dataList.Where(w => w.RoleName.Contains(name.Trim())).ToList();
                    }


                    totalCount = dataList.Count;

                    if (pageIndex > 0)
                    {
                        dataList = dataList.OrderByDescending(m => m.Enterprise_Role_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    ClearLinqModel(dataList);
                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 更新角色信息方法
        /// </summary>
        /// <param name="objPRRU_PlatForm_Role">角色信息对象</param>
        /// <returns>操作结果</returns>
        public RetResult Update(long rId, string roleName, string modelIdArray, long userId, long enterpriseID)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Role role = dataContext.Enterprise_Role.Where(p => p.RoleName == roleName
                        && p.Status == (int)EnumFile.Status.used && p.Enterprise_Role_ID != rId
                        && p.Enterprise_Info_ID == enterpriseID).FirstOrDefault();
                    if (role != null)
                    {
                        Ret.SetArgument(CmdResultError.Other, "角色名称重复！", "角色名称重复！");
                        return Ret;
                    }
                    var data = (from d in dataContext.Enterprise_Role
                                where d.Enterprise_Role_ID == rId && d.Enterprise_Info_ID == enterpriseID
                                select d).FirstOrDefault();
                    if (!string.IsNullOrEmpty(roleName))
                    {
                        data.RoleName = roleName;
                    }
                    if (!string.IsNullOrEmpty(modelIdArray))
                    {
                        data.Modual_ID_Array = GetModualArr(modelIdArray);
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "更新成功！", "更新成功！");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, ex.Message, ex.Message);
            }



            return Ret;
        }

        /// <summary>
        /// 根据角色ID获取选择的模块
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public Enterprise_Role GetModel(int rId)
        {
            Enterprise_Role objEnterprise_Role = new Enterprise_Role();

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    objEnterprise_Role = (from data in dataContext.Enterprise_Role
                                          where data.Enterprise_Role_ID == rId
                                          select data).FirstOrDefault();
                    ClearLinqModel(objEnterprise_Role);
                    return objEnterprise_Role;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="objPRRU_PlatForm_Role"></param>
        /// <returns></returns>
        public RetResult Add(PRRU_PlatForm_Role objPRRU_PlatForm_Role)
        {
            string Msg = "新增失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.PRRU_PlatForm_Role.InsertOnSubmit(objPRRU_PlatForm_Role);
                    dataContext.SubmitChanges();

                    Msg = "新增成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                Msg = "数据库连接失败！";
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, Msg, Msg);

            return Ret;
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public RetResult Del(int id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Role linqModel = (from data in dataContext.Enterprise_Role
                                                           where data.Enterprise_Role_ID == id
                                                           select data).FirstOrDefault();

                    int count = (from data in dataContext.Enterprise_User
                                 where data.Enterprise_Role_ID == id && data.Status == (int)EnumFile.Status.used
                                 select data).Count();

                    if (linqModel == null)
                    {
                        Ret.SetArgument(CmdResultError.NO_RESULT, "不存在该数据请刷新后重试！", "不存在该数据请刷新后重试！");
                    }
                    else
                    {
                        if (count != 0)
                        {
                            Ret.SetArgument(CmdResultError.Other, "该角色已经被使用不能删除！", "该角色已经被使用不能删除！");
                        }
                        else
                        {
                            linqModel.Status = (int)EnumFile.Status.delete;
                            dataContext.SubmitChanges();
                            Ret.SetArgument(CmdResultError.NONE, "删除成功！", "删除成功！");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败！", "数据库连接失败！");
            }
            return Ret;
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="objEnterprise_Role"></param>
        /// <returns></returns>
        public RetResult Save(Enterprise_Role objEnterprise_Role)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Role role = dataContext.Enterprise_Role.Where(p => p.RoleName == objEnterprise_Role.RoleName &&
                        p.Enterprise_Info_ID == objEnterprise_Role.Enterprise_Info_ID && p.Status == (int)EnumFile.Status.used).FirstOrDefault();
                    if (role != null)
                    {
                        Ret.SetArgument(CmdResultError.Other, "角色名称重复！", "角色名称重复！");
                        return Ret;
                    }
                    objEnterprise_Role.Modual_ID_Array = GetModualArr(objEnterprise_Role.Modual_ID_Array);
                    dataContext.Enterprise_Role.InsertOnSubmit(objEnterprise_Role);
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "保存成功！", "保存成功！");
                }
            }
            catch (Exception e)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, e.Message, e.Message);
            }

            return Ret;
        }

        /// <summary>
        /// 更新模块串
        /// </summary>
        /// <param name="modualArray">原有串</param>
        /// <returns></returns>
        public string GetModualArr(string modualArray)
        {
            //获取串
            string modualStr = modualArray;
            string[] arrArr = modualStr.Split(',');
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                foreach (var sub in arrArr)
                {
                    //查找不显示在菜单项的模块
                    PRRU_NewModual modual = dataContext.PRRU_NewModual.Where(p => p.PRRU_Modual_ID == Convert.ToInt32(sub)).FirstOrDefault();
                    if (modual != null && modual.Modual_Level > 1)
                    {
                        List<PRRU_NewModual> list = dataContext.PRRU_NewModual.Where(p => p.Parent_ID == modual.PRRU_Modual_ID && p.IsDisplay == 1).ToList();
                        if (list != null)
                        {
                            foreach (var temp in list)
                            {
                                modualStr += "," + temp.PRRU_Modual_ID.ToString();
                                List<PRRU_NewModual> listThree = dataContext.PRRU_NewModual.Where(p => p.Parent_ID == temp.PRRU_Modual_ID
                                   && p.Modual_Level == temp.Modual_Level + 1 && p.IsDisplay == 1).ToList();
                                if (listThree != null)
                                {
                                    foreach (var tempThree in listThree)
                                    {
                                        modualStr += "," + tempThree.PRRU_Modual_ID.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return modualStr;
        }
    }
}