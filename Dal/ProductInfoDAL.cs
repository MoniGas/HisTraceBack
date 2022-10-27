/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-12-10

** 联系方式 :13313318725

** 描述：主要用于配置产品信息的数据库操作

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common;

namespace Dal
{
    /// <summary>
    /// 配置产品信息数据库操作类
    /// </summary>
    public class ProductInfoDAL:DALBase
    {
        /// <summary>
        /// 查询该产品配置的信息
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">每页显示的数据条数</param>
        /// <returns>产品配置信息对象集合</returns>
        public List<LinqModel.View_ProductInfoForEnterprise> GetProductInfoForEnterprise(long EnterpriseId, string MaterialId, int PageIndex, int PageSize)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_ProductInfoForEnterprise
                                    where data.EnterpriseId == EnterpriseId
                                    select data).ToList();
                    // 判断产品ID不为空
                    if (!string.IsNullOrEmpty(MaterialId))
                    {
                        DataList = DataList.Where(w => w.MaterialId == Convert.ToInt64(MaterialId)).ToList();
                    }
                    // 判断查询结果不为空
                    if (DataList != null)
                    {
                        DataList = DataList.OrderBy(o => o.Id).ToList();
                    }
                    // 判断页码大于0为有效页码
                    if (PageIndex > 0)
                    {
                        DataList = DataList.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取产品信息和企业的关联信息对象方法
        /// </summary>
        /// <param name="Id">关联信息表Id</param>
        /// <returns>返回关联信息对象</returns>
        public LinqModel.View_ProductInfoForEnterprise GetProductInfoForEnterpriseModel(long Id)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataModel = (from data in dataContext.View_ProductInfoForEnterprise
                                     where data.Id == Id
                                     select data).FirstOrDefault();
                    return dataModel;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取活动商品模块集合方法
        /// </summary>
        /// <returns></returns>
        public List<LinqModel.View_MaterialSpecAndProperty> GetMaterialSpecList(long EnterpriseId, long MaterialId) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_MaterialSpecAndProperty
                                    where data.EnterpriseId == EnterpriseId
                                    && data.Material_ID == MaterialId
                                    && data.Status == (int)EnumFile.Status.used
                                    && data.SpecStatus == (int)EnumFile.Status.used
                                    && data.Condition != null
                                    select data).ToList();
                    // 判断查询结果不为空
                    if (DataList != null)
                    {
                        DataList = DataList.OrderBy(o => o.MaterialSpecification).ToList();
                    }
                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 添加关联产品信息方法
        /// </summary>
        /// <param name="ObjProductInfoForEnterprise">关联产品信息对象</param>
        /// <returns>返回操作结果对象</returns>
        public RetResult AddProductInfoEnterprise(LinqModel.ProductInfoForEnterprise ObjProductInfoForEnterprise) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataInfo = (from data in dataContext.ProductInfoForEnterprise
                                    where data.MaterialId == ObjProductInfoForEnterprise.MaterialId
                                    select data).FirstOrDefault();
                    // 判断查询结果不为空
                    if (DataInfo != null)
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "该产品已经存在配置的产品信息，不能再添加！", "该产品已经存在配置的产品信息，不能再添加！");
                        return Ret;
                    }
                    dataContext.ProductInfoForEnterprise.InsertOnSubmit(ObjProductInfoForEnterprise);
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "添加成功！", "添加成功！");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "添加失败！", "添加失败！");
            }
            return Ret;
        }

        /// <summary>
        /// 修改关联产品信息方法
        /// </summary>
        /// <param name="ObjProductInfoForEnterprise">关联产品信息对象</param>
        /// <returns>返回操作结果对象</returns>
        public RetResult EditProductInfoForEnterprise(LinqModel.ProductInfoForEnterprise ObjProductInfoForEnterprise) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataModel = (from data in dataContext.ProductInfoForEnterprise
                                     where data.Id == ObjProductInfoForEnterprise.Id
                                     select data).FirstOrDefault();
                    // 判断查询结果不为空
                    if (DataModel != null)
                    {
                        DataModel.MaterialId = ObjProductInfoForEnterprise.MaterialId;
                        DataModel.ViewPropertyIdArray = ObjProductInfoForEnterprise.ViewPropertyIdArray;
                        DataModel.ViewOrderHotline = ObjProductInfoForEnterprise.ViewOrderHotline;
                        DataModel.ViewMaterialPrice = ObjProductInfoForEnterprise.ViewMaterialPrice;
                        DataModel.ViewProductionTime = ObjProductInfoForEnterprise.ViewProductionTime;
                        DataModel.ViewFactory = ObjProductInfoForEnterprise.ViewFactory;
                        DataModel.ViewComplaintPhone = ObjProductInfoForEnterprise.ViewComplaintPhone;
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "修改成功！", "修改成功！");
                    }
                    else // 判断查询结果为空
                    {
                        Ret.SetArgument(CmdResultError.NO_RESULT, "数据不存在，请刷新后重试！", "数据不存在，请刷新后重试！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败！", "数据库连接失败！");
            }

            return Ret;
        }

        /// <summary>
        /// 删除关联产品信息方法
        /// </summary>
        /// <param name="Id">关联产品信息表Id</param>
        /// <returns>返回操作结果对象</returns>
        public RetResult DelProductInfoForEnterprise(long Id) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataModel = (from data in dataContext.ProductInfoForEnterprise
                                     where data.Id == Id
                                     select data).FirstOrDefault();
                    // 判断查询结果不为空
                    if (DataModel != null)
                    {
                        dataContext.ProductInfoForEnterprise.DeleteOnSubmit(DataModel);
                        dataContext.SubmitChanges();

                        Ret.SetArgument(CmdResultError.NONE, "删除数据成功！", "删除数据成功！");
                    }
                    else  // 判断查询结果为空
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "数据不存在，请刷新后重试！", "数据不存在，请刷新后重试！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败！", "数据库连接失败！");
            }

            return Ret;
        }

        /// <summary>
        /// 获取推荐的商品活动集合方法
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回商品活动集合</returns>
        public List<LinqModel.View_ProductInfoForMaterial> GetMaterialProperty(long EnterpriseId, long MaterialId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_ProductInfoForMaterial
                                    where data.MaterialId == MaterialId && data.EnterpriseId == EnterpriseId
                                select data).ToList();
                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public LinqModel.View_ProductInfoForMaterial GetMaterialSpecPrice(long MaterialSpecId) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_ProductInfoForMaterial
                                    where data.MaterialSpecId == MaterialSpecId
                                    select data).FirstOrDefault();

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
