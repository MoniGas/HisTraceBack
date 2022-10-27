/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-11-05

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于规格管理的数据库操作

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    public class SpecificationDAL : DALBase
    {
        #region 查询规格列表方法
        /// <summary>
        /// 查询规格列表方法
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns>反回规格列表</returns>
        public List<LinqModel.Specification> GetList(long enterpriseId, int? Value, int? pageIndex, int pageSize)
        {
            List<LinqModel.Specification> dataList = null;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = from d in dataContext.Specification
                               where d.Enterprise_Info_ID == enterpriseId
                               select d;
                    if (Value != null)
                    {
                        data = data.Where(w => w.Value == Value);
                    }

                    dataList = data.ToPagedList(pageIndex ?? 1, pageSize);
                }
            }
            catch (Exception ex)
            {

            }

            return dataList;
        }
        #endregion

        #region 规格添加方法
        /// <summary>
        /// 规格添加方法
        /// </summary>
        /// <param name="objSpecification">规格数据库对象</param>
        /// <returns>添加结果</returns>
        public RetResult Add(LinqModel.Specification objSpecification)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.Specification
                                where d.Value == objSpecification.Value && d.GuiGe == objSpecification.GuiGe && d.Enterprise_Info_ID == objSpecification.Enterprise_Info_ID
                                select d).FirstOrDefault();

                    if (data == null)
                    {
                        dataContext.Specification.InsertOnSubmit(objSpecification);
                        dataContext.SubmitChanges();

                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, "添加成功！", "添加成功！");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, "已经存在该规格！", "已经存在该规格！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, ex.Message, ex.Message);
            }

            return Ret;
        }
        #endregion

        #region 规格修改方法
        /// <summary>
        /// 规格修改方法
        /// </summary>
        /// <param name="objSpecification">规格数据库对象</param>
        /// <returns>修改操作结果</returns>
        public RetResult Edit(LinqModel.Specification objSpecification)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataValue = (from d in dataContext.Specification
                                     where d.Enterprise_Info_ID == objSpecification.Enterprise_Info_ID
                                     && d.Value == objSpecification.Value && d.GuiGe == objSpecification.GuiGe
                                     && d.ID != objSpecification.ID
                                     select d).FirstOrDefault();

                    if (dataValue != null)
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, "已经存在该规格！", "已经存在该规格！");
                        return Ret;
                    }

                    var data = (from d in dataContext.Specification
                                where d.ID == objSpecification.ID
                                select d).FirstOrDefault();
                    if (data != null)
                    {
                        data.Value = objSpecification.Value;
                        data.GuiGe = objSpecification.GuiGe;
                        dataContext.SubmitChanges();

                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, "修改成功！", "修改成功！");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.NO_RESULT, "数据错误请刷新重试！", "数据错误请刷新重试！");
                    }


                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, ex.Message, ex.Message);
            }

            return Ret;
        }
        #endregion

        #region 规格删除方法
        /// <summary>
        /// 规格删除方法
        /// </summary>
        /// <param name="id">规格ID</param>
        /// <returns>返回删除结果</returns>
        public RetResult Del(long id)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.Specification
                                where d.ID == id
                                select d).FirstOrDefault();
                    if (data != null)
                    {
                        dataContext.Specification.DeleteOnSubmit(data);
                        dataContext.SubmitChanges();
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, "删除成功！", "删除成功！");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, "数据不存在请刷新后重试！", "删除失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, ex.Message, "删除失败！");
            }
            return Ret;
        }
        #endregion

        #region 获取规格信息方法
        /// <summary>
        /// 获取规格信息方法
        /// </summary>
        /// <param name="id">规格ID</param>
        /// <returns>返回规格信息集合</returns>
        public LinqModel.Specification GetInfo(long id)
        {
            LinqModel.Specification objSpecification = new LinqModel.Specification();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    objSpecification = (from data in dataContext.Specification
                                        where data.ID == id
                                        select data).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

            }

            return objSpecification;
        }
        #endregion

        public List<LinqModel.Specification> GetSelectList(long Enterprise)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.Specification
                                    where data.Enterprise_Info_ID == Enterprise
                                    select data).ToList();
                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<LinqModel.MaterialSpcification> GetMaterialSelectList(long Enterprise)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.MaterialSpcification
                                    where data.Enterprise_Info_ID == Enterprise
                                    select data).ToList();
                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
