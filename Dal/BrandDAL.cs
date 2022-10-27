using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class BrandDAL : DALBase
    {
        #region 选择品牌列表
        /// <summary>
        /// 获取品牌列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="brandName">品牌名称</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<Brand> GetSelectList(long enterpriseId, string brandName, out long totalCount, int pageIndex)
        {
            List<Brand> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Brand.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(brandName))
                    {
                        data = data.Where(m => m.BrandName.Contains(brandName));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Brand_ID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "BrandDAL.GetSelectList():Brand表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取品牌列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="brandName">品牌名称</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<Brand> GetList(long enterpriseId, string brandName, out long totalCount, int pageIndex)
        {
            List<Brand> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Brand.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(brandName))
                    {
                        data = data.Where(m => m.BrandName.Contains(brandName.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Brand_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "BrandDAL.GetList():Brand表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        //监管部门 区域品牌
        public List<Brand> GetJGList(long enterpriseId, string brandName, out long totalCount, int pageIndex)
        {
            List<Brand> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Brand.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(brandName))
                    {
                        data = data.Where(m => m.BrandName.Contains(brandName));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Brand_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "BrandDAL.GetJGList():Brand表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 添加品牌信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(Brand brand)
        {
            string Msg = "添加品牌信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Brand.FirstOrDefault(m => m.BrandName == brand.BrandName && m.Enterprise_Info_ID == brand.Enterprise_Info_ID && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "已存在该品牌名称！";
                    }
                    else
                    {
                        dataContext.Brand.InsertOnSubmit(brand);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = brand.Brand_ID;
                        Msg = "添加品牌信息成功";
                        error = CmdResultError.NONE;
                        //为统计表添加数据
                        HomeDataStatis model = new HomeDataStatis();
                        model.EnterpriseID = brand.Enterprise_Info_ID;
                        model.BrancCount = 1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult result = dal.Update(model);
                    }
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public RetResult Edit(Brand brand)
        {
            string Msg = "修改品牌信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = from m in dataContext.Brand where m.BrandName == brand.BrandName && m.Brand_ID != brand.Brand_ID && m.Enterprise_Info_ID == brand.Enterprise_Info_ID && m.Status == (int)Common.EnumFile.Status.used select m;
                    if (model.Count() > 0)
                    {
                        Msg = "已存在该品牌！";
                    }
                    else
                    {
                        var data = dataContext.Brand.FirstOrDefault(m => m.Enterprise_Info_ID == brand.Enterprise_Info_ID && m.Brand_ID == brand.Brand_ID);
                        if (data == null)
                        {
                            Msg = "没有找到要修改的数据！";
                        }
                        else
                        {
                            data.BrandName = brand.BrandName;
                            data.Descriptions = brand.Descriptions;
                            data.Logo = brand.Logo;
                            data.memo = brand.memo;
                            data.lastdate = brand.lastdate;
                            data.lastuser = brand.lastuser;
                            if (!string.IsNullOrEmpty(brand.Logo))
                                data.Logo = brand.Logo;
                            dataContext.SubmitChanges();
                            Msg = "修改品牌信息成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
                Ret.SetArgument(error, Msg, Msg);
                return Ret;
            }
        }
        public RetResult Delete(long brandID, long enterpriseId)
        {
            string Msg = "删除品牌失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Brand brand = dataContext.Brand.SingleOrDefault(m => m.Brand_ID == brandID);
                    int count = dataContext.Material.Where(m => m.Brand_ID == brandID).Count();

                    if (brand == null)
                    {
                        Msg = "没有找到要删除的品牌信息请刷新列表！";
                    }
                    else if (count > 0)
                    {
                        Msg = "该品牌已经使用，目前无法删除！";
                    }
                    else
                    {
                        brand.Status = (int)Common.EnumFile.Status.delete;
                        //dataContext.Brand.DeleteOnSubmit(brand);
                        dataContext.SubmitChanges();
                        Msg = "删除品牌信息成功！";
                        error = CmdResultError.NONE;
                        //为统计表删除数据
                        HomeDataStatis model = new HomeDataStatis();
                        model.EnterpriseID = brand.Enterprise_Info_ID;
                        model.BrancCount = -1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult result = dal.Update(model);
                    }
                }
            }
            catch
            {
                //Msg = "删除失败，请首先删除已知关联的其他数据";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public Brand GetBrandByID(long brandID)
        {
            Brand brand = new Brand();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    brand = dataContext.Brand.FirstOrDefault(t => t.Brand_ID == brandID);
                    ClearLinqModel(brand);
                }
            }
            catch
            {
            }
            return brand;
        }
        #region 区域品牌管理
        /// <summary>
        /// 区域品牌列表
        /// </summary>
        /// <param name="materialName">产品名称</param>
        /// <param name="brandEnterpriseEID">区域品牌表里的ID（当前登录企业的ID）</param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<View_RequestBrand> GetListByRequestBrand(string materialName, long brandEnterpriseEID, out long totalCount, int pageIndex)
        {
            List<View_RequestBrand> result = null;
            totalCount = 0;
            try
            {
                using (DataClassesDataContext linqContent = GetDataContext())
                {
                    var data = from d in linqContent.View_RequestBrand select d;
                    if (brandEnterpriseEID > 0)
                    {
                        data = data.Where(m => m.BrandEnterpriseEID == brandEnterpriseEID);
                    }
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(materialName));
                    }
                    //if (brandEnterpriseStatus > -3)
                    //    data = data.Where(m => m.BrandEnterpriseStatus == brandEnterpriseStatus);

                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Brand_Enterprise_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "BrandDAL.GetListByRequestBrand():View_RequestBrand视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        //20151013
        public List<View_RequestBrand> GetListJGBMSHBBrand(string searchName, int brandEnterpriseStatus, long brandEID, out long totalCount, int pageIndex)
        {
            List<View_RequestBrand> result = null;
            totalCount = 0;
            try
            {
                using (DataClassesDataContext linqContent = GetDataContext())
                {
                    var data = from d in linqContent.View_RequestBrand select d;
                    if (brandEID > 0)
                    {
                        data = data.Where(m => m.BrandEID == brandEID);
                    }
                    if (!string.IsNullOrEmpty(searchName))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(searchName.Trim()) || m.EnterpriseName.Contains(searchName.Trim()) || m.BrandName.Contains(searchName.Trim()));
                    }
                    if (brandEnterpriseStatus > -1)
                    {
                        data = data.Where(m => m.BrandEnterpriseStatus == brandEnterpriseStatus);
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Brand_Enterprise_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "BrandDAL.GetListJGBMSHBBrand():View_RequestBrand视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        public RetResult AddBrand_Enterprise(Brand_Enterprise model)
        {
            try
            {
                using (DataClassesDataContext linqContent = GetDataContext())
                {
                    Brand_Enterprise data = linqContent.Brand_Enterprise.FirstOrDefault(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID && m.Brand_ID == model.Brand_ID && m.Material_ID == model.Material_ID);
                    if (data != null)
                    {
                        Ret.SetArgument(CmdResultError.Other, "", "该产品已经申请该区域品牌");
                        return Ret;
                    }
                    linqContent.Brand_Enterprise.InsertOnSubmit(model);
                    linqContent.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "", "操作成功");
                }
            }
            catch
            {
                Ret.SetArgument(CmdResultError.Other, "", "操作失败");
            }
            return Ret;
        }
        /// <summary>
        /// 获取申请的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public View_RequestBrand GetModelViewBE(long id)
        {
            using (DataClassesDataContext linqContent = GetDataContext())
            {
                return linqContent.View_RequestBrand.FirstOrDefault(m => m.Brand_Enterprise_ID == id);
            }
        }
        /// <summary>
        /// 审核区域品牌的申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">修改的状态</param>
        /// <returns></returns>
        public RetResult UpdateStatus(long id, int status)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Brand_Enterprise beModel = dataContext.Brand_Enterprise.FirstOrDefault(m => m.Brand_Enterprise_ID == id);
                    if (beModel == null)
                    {
                        Ret.SetArgument(CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    dataContext.SubmitChanges();
                    string Msg = string.Empty;
                    if (status == (int)Common.EnumFile.PlatFormState.pass)
                    {
                        Msg = "审核通过";
                    }
                    else
                    {
                        Msg = "您已经成功取消该条信息的审核";
                    }
                    Ret.SetArgument(CmdResultError.NONE, null, Msg);
                }
                catch (Exception ex)
                {
                    Ret.SetArgument(CmdResultError.EXCEPTION, ex.ToString(), "操作失败");
                }
            }
            return Ret;
        }
        public RetResult AuditStatus(long id, long enterpriseid)
        {
            string Msg = "审核失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Brand_Enterprise brandEn = dataContext.Brand_Enterprise.SingleOrDefault(m => m.Brand_Enterprise_ID == id);
                    //int count = dataContext.Material.Where(m => m.Brand_ID == brandID).Count();

                    if (brandEn == null)
                    {
                        Msg = "没有找到要审核的信息请刷新列表！";
                    }
                    else
                    {
                        brandEn.Status = (int)Common.EnumFile.PlatFormState.pass;
                        //dataContext.Brand.DeleteOnSubmit(brand);
                        dataContext.SubmitChanges();
                        Msg = "审核成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            { }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #endregion

        /// <summary>
        /// 获取企业品牌列表
        /// </summary>
        /// <param name="enterpriseID">加入区域品牌ID</param>
        /// <param name="material_ID">加入区域品牌产品ID</param>
        /// <returns>品牌列表</returns>
        public List<View_RegionalBrand> GetRegionalBrandList(long enterpriseID, long material_ID)
        {
            using (DataClassesDataContext linqContent = GetDataContext())
            {
                var data = from m in linqContent.View_RegionalBrand select m;
                if (enterpriseID > 0)
                    data = data.Where(m => m.BEEnterprise_Info_ID == enterpriseID);
                if (material_ID > 0)
                    data = data.Where(m => m.Material_ID == material_ID);
                return data.ToList();
            }
        }
    }
}
