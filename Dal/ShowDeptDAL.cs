/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-24

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于部门宣传码信息管理数据层

*********************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;

namespace Dal
{
    public class ShowDeptDAL : DALBase
    {
        /// <summary>
        /// 获取部门宣传列表
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <param name="name">部门名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<ShowDept> GetList(long companyId, string name, int pageIndex, out long totalCount)
        {
            List<ShowDept> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                var data = (from m in dataContext.ShowDept where m.CompanyID == companyId select m).ToList();
                if (!string.IsNullOrEmpty(name))
                {
                    data = data.Where(m => m.DeptName.Contains(name.Trim())).ToList();
                }
                data = data.OrderByDescending(m => m.ID).ToList();

                if (data != null)
                {
                    totalCount = data.Count();
                    result = data.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();

                    ClearLinqModel(result);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <param name="id">部门标识</param>
        /// <returns>模型</returns>
        public ShowDept GetModel(long id)
        {
            ShowDept result = null;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                result = dataContext.ShowDept.FirstOrDefault(m => m.ID == id);
            }
            return result;
        }

        /// <summary>
        /// 添加部门宣传
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="mainCode">企业主码</param>
        /// <returns>操作结果</returns>
        public RetResult Add(ShowDept model, string mainCode)
        {
            Ret.Msg = "添加部门信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
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
                    ShowDept dept = dataContext.ShowDept.FirstOrDefault(m => m.DeptName == model.DeptName && m.CompanyID == model.CompanyID);
                    if (dept != null)
                    {
                        Ret.Msg = "已存在该部门名称！";
                    }
                    else
                    {
                        dataContext.ShowDept.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        model.EWM = mainCode + "." + (int)Common.EnumFile.TerraceEwm.showDept + "." + model.ID;
                        string config ="idcodeURL";
                        if (Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                        {
                            config = "ncpURL";
                        }
                        model.Url = System.Configuration.ConfigurationManager.AppSettings[config] + model.EWM;
                        dataContext.SubmitChanges();
                        Ret.CmdError = CmdResultError.NONE;
                        Ret.Msg = "添加部门信息成功！";
                    }
                }
                catch
                {
                    Ret.Msg = "操作中出现错误！";
                }
            }
            return Ret;
        }

        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(ShowDept model)
        {
            Ret.Msg = "修改部门信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowDept dept = dataContext.ShowDept.FirstOrDefault(m => m.CompanyID == model.CompanyID && m.DeptName == model.DeptName && m.ID != model.ID);
                    if (dept != null)
                    {
                        Ret.Msg = "已存在该部门名称！";
                    }
                    else
                    {
                        dept = dataContext.ShowDept.FirstOrDefault(m => m.ID == model.ID);
                        if (dept == null)
                        {
                            Ret.Msg = "部门信息不存在！";
                        }
                        else
                        {
                            dept.DeptName = model.DeptName;
                            dept.Infos = model.Infos;
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改部门信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                    }
                }
                catch
                {
                    Ret.Msg = "操作中出现错误！";
                }
            }
            return Ret;
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id">部门标识</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long id)
        {
            Ret.Msg = "删除部门信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowDept dept = dataContext.ShowDept.FirstOrDefault(m => m.ID == id);
                    if (dept == null)
                    {
                        Ret.Msg = "部门信息不存在！";
                    }
                    else
                    {
                        dataContext.ShowDept.DeleteOnSubmit(dept);
                        dataContext.SubmitChanges();
                        Ret.Msg = "删除部门信息成功！";
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
    }
}
