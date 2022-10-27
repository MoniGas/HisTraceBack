/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-12-15

** 联系方式 :13313318725

** 描述：主要用于配置导航的数据库操作

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;

namespace Dal
{
    /// <summary>
    /// 配置导航模块数据库操作类
    /// </summary>
    public class PageNavigationDAL : DALBase
    {
        /// <summary>
        /// 获取导航配置信息列表
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">页的条数</param>
        /// <returns>返回导航配置信息列表集合</returns>
        public List<LinqModel.NavigationForMaterialGroup> GetNavigationForEnterpriseList(long EnterpriseId, string MaterialId, int PageIndex, int PageSize)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_NavigationForMaterial
                                    where data.EnterpriseId == EnterpriseId
                                    select new NavigationForMaterialGroup
                                    {
                                        MaterialId = data.MaterialId.Value,
                                        MaterialFullName = data.MaterialFullName
                                    }).Distinct().ToList();
                    // 验证产品Id不为空
                    if (!string.IsNullOrEmpty(MaterialId))
                    {
                        DataList = DataList.Where(w => w.MaterialId == Convert.ToInt64(MaterialId)).ToList();
                    }
                    // 判断页码大于0为有效页码
                    if (PageIndex > 0)
                    {
                        DataList = DataList.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    return DataList as List<LinqModel.NavigationForMaterialGroup>;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取关联的导航模块列表
        /// </summary>
        /// <param name="EnterpriseId">企业ID</param>
        /// <param name="MaterialId">产品ID</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">每页条数</param>
        /// <returns>返回导航模块集合</returns>
        public List<LinqModel.View_NavigationForMaterial> GetList(long EnterpriseId, long MaterialId, int PageIndex, int PageSize) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_NavigationForMaterial
                                    where data.EnterpriseId == EnterpriseId && data.MaterialId == MaterialId
                                    select data).ToList();
                    // 判断查询结果不为空
                    if (DataList != null)
                    {
                        DataList = DataList.OrderBy(o => o.ViewNum).ToList();
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
        /// 删除产品关联的导航信息
        /// </summary>
        /// <param name="Id">产品关联导航信息表Id</param>
        /// <returns></returns>
        public RetResult DelNavigationForEnterprise(long Id) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext()) 
                {
                    var DataModel = (from data in dataContext.NavigationForEnterprise
                                     where data.Id == Id
                                     select data).FirstOrDefault();
                    // 验证查询的结果不为Null
                    if (DataModel != null)
                    {
                        dataContext.NavigationForEnterprise.DeleteOnSubmit(DataModel);
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "删除成功！", "删除成功！");
                    }
                    else // 查询结果为Null
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "数据不存在，请刷新重试！", "数据不存在，请刷新重试！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败！", "数据库连接失败！");
            }
            return Ret;
        }

        public RetResult DelMaterialList(long MaterialId) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.NavigationForEnterprise
                                    where data.MaterialId == MaterialId
                                    select data).ToList();

                    if (DataList != null)
                    {
                        dataContext.NavigationForEnterprise.DeleteAllOnSubmit(DataList);
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "删除成功！", "删除成功！");
                    }
                    else 
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
        /// 获取导航模块信息列表
        /// </summary>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回导航信息模块列表集合</returns>
        public List<LinqModel.Navigation> GetNavigationList(string MaterialId) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    // 查询导航模块信息集合
                    var DataList = (from data in dataContext.Navigation
                                    select data).ToList();
                    // 判断产品Id不为空
                    if (!string.IsNullOrEmpty(MaterialId))
                    {
                        // 查询没有被关联的导航模块信息集合
                        DataList = DataList.Where(w => !(from d in dataContext.NavigationForEnterprise
                                                         where d.MaterialId == Convert.ToInt64(MaterialId)
                                                         select d.NavigationId).Contains(w.Id)).ToList();
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
        /// 更新导航模块的显示序号方法
        /// </summary>
        /// <param name="Id">配置导航信息模块Id</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="Type">操作类型 up:上移操作 down:下移操作</param>
        /// <returns>返回操作结果</returns>
        public RetResult UpdateViewNum(long Id, long MaterialId, string Type) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.NavigationForEnterprise
                                     where data.MaterialId == MaterialId
                                     select data).ToList();
                    // 判断查询结果不为空
                    if (DataList != null)
                    {
                        DataList = DataList.OrderBy(o => o.ViewNum).ToList();
                        // 判断是上移操作，并且不是该产品关联的第一个
                        if (Type.Equals("up") && DataList.Where(w=>w.Id == Id).FirstOrDefault().ViewNum != 1)
                        {
                            // 修改显示循序
                            for (int i = 1; i < DataList.Count; i++)
                            {
                                if (DataList[i].Id == Id)
                                {
                                    DataList[i - 1].ViewNum += 1;
                                    DataList[i].ViewNum -= 1;
                                }
                            }
                        }
                        // 判断是下移操作，并且不是该产品关联的最后一个
                        else if (Type.Equals("down") && DataList.Where(w => w.Id == Id).FirstOrDefault().ViewNum != DataList.Count)
                        {
                            // 修改显示循序
                            for (int i = 0; i < DataList.Count - 1; i++)
                            {
                                if (DataList[i].Id == Id)
                                {
                                    DataList[i + 1].ViewNum -= 1;
                                    DataList[i].ViewNum += 1;
                                }
                            }
                        }
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "更新成功！", "更新成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接错误，请检查网络！", "数据库连接错误，请检查网络！");
            }
            return Ret;
        }

        /// <summary>
        /// 更新排序后的导航模块方法
        /// </summary>
        /// <param name="DataList">排序信息集合</param>
        /// <returns>返回操作结果集合</returns>
        public RetResult UpdateList(List<PageNavigationRequset> DataList)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    for (int i = 0; i < DataList.Count; i++)
                    {
                        var DataModel = (from data in dataContext.NavigationForEnterprise
                                         where data.Id == DataList[i].Id
                                         select data).FirstOrDefault();

                        if (DataModel != null)
                        {
                            DataModel.ViewNum = DataList[i].ViewNum;
                        }
                    }

                    dataContext.SubmitChanges();

                    Ret.SetArgument(CmdResultError.NONE, "保存成功！", "保存成功！");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接失败，请检查网络！", "数据库连接失败，请检查网络！");
            }

            return Ret;
        }

        /// <summary>
        /// 保存关联的导航信息集合方法
        /// </summary>
        /// <param name="NavigationIdArray">关联的导航模块Id集合</param>
        /// <param name="MaterialId">产品Id</param>
        /// <param name="EnterpriseId">企业Id</param>
        /// <returns>返回操作结果对象</returns>
        public RetResult SaveNavigationForEnterpriseList(List<string> NavigationIdArray,long MaterialId,long EnterpriseId) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var NavigationForEnterpriseList = (from data in dataContext.NavigationForEnterprise
                                                       where data.MaterialId == MaterialId
                                                       select data).ToList();

                    dataContext.NavigationForEnterprise.DeleteAllOnSubmit(NavigationForEnterpriseList);
                    
                    var DataList = (from data in dataContext.Navigation
                                    where NavigationIdArray.Contains(data.Id.Value.ToString())
                                    select data).ToList();
                    // 判断查询结果不为空
                    if (DataList != null)
                    {
                        // 新增本次关联的导航模块
                        for (int i = 0; i < DataList.Count; i++)
                        {
                            LinqModel.NavigationForEnterprise ObjNavigationForEnterprise = new LinqModel.NavigationForEnterprise();
                            ObjNavigationForEnterprise.MaterialId = MaterialId;
                            ObjNavigationForEnterprise.NavigationId = DataList[i].Id;
                            ObjNavigationForEnterprise.EnterpriseId = EnterpriseId;
                            ObjNavigationForEnterprise.ViewNum = i + 1;
                            dataContext.NavigationForEnterprise.InsertOnSubmit(ObjNavigationForEnterprise);
                        }
                        dataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "保存成功！", "保存成功！");
                    }
                    else 
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "数据错误，请刷新重试！", "数据错误，请刷新重试！");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "数据库连接错误，请检查网络！", "数据库连接错误，请检查网络！");
            }
            return Ret;
        }

        /// <summary>
        /// 获取导航列表
        /// </summary>
        /// <param name="EnterpriseId">企业ID</param>
        /// <param name="MaterialId">产品Id</param>
        /// <returns>返回导航列表集合</returns>
        public List<LinqModel.View_NavigationForMaterial> GetNavigationForMaterialList(long EnterpriseId,long MaterialId) 
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.View_NavigationForMaterial
                                     where data.EnterpriseId == EnterpriseId && data.MaterialId == MaterialId
                                     select data).ToList();
                    if (DataList != null)
                    {
                        DataList = DataList.OrderBy(o => o.ViewNum).ToList();
                    }

                    return DataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetNavigationForMaterialList(long MaterialId)
        {
            string strNavigationId = string.Empty;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataList = (from data in dataContext.NavigationForEnterprise
                                    where data.MaterialId == MaterialId
                                    select data.NavigationId).ToList();

                    foreach (var item in DataList) 
                    {
                        if(item != null)
                        {
                            strNavigationId += item.Value.ToString() + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(strNavigationId))
                    {
                        strNavigationId = strNavigationId.Substring(0, strNavigationId.Length - 1);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return strNavigationId;
        }
    }
}
