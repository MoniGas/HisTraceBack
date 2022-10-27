/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2016-12-09

** 联系方式 :15031109901

** 描述：主要用于仓库信息管理数据层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;
using InterfaceWeb;
using LinqModel.InterfaceModels;
using System.Configuration;

namespace Dal
{
    /// <summary>
    /// 主要用于仓库信息管理数据层
    /// </summary>
    public class StoreDAL : DALBase
    {

        #region 获取企业仓库接口（用于小程序和其他企业使用）
        public InterfaceResult storeLst(StoreRequestParam Param, string accessToken)
        {
            int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            ApiDAL apiDal = new ApiDAL();
            InterfaceResult result = new InterfaceResult();
            StoreResult StoreResult = new StoreResult();
            List<StoreLst> StoreLst = new List<StoreLst>();
            int? totalPageCount = 0;
            try
            {
                //第一步：先解密accessToken，根据解密到的数据执行后续逻辑
                Token token = apiDal.TokenDecrypt(accessToken);
                if (token == null || !token.isTokenOK)
                {
                    result.retCode = 1;
                    result.retMessage = "token失效，请重新获取!";
                    result.isSuccess = false;
                    result.retData = null;
                    return result;
                }
                using (DataClassesDataContext db = GetDataContext())
                {
                    var data = db.Store.Where(m => m.Enterprise_Info_ID == token.Enterprise_Info_ID
                        && m.Type == (int)Common.EnumFile.StoreType.Store && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    //仓库名称
                    if (!string.IsNullOrEmpty(Param.storeName))
                    {
                        data = data.Where(m => m.StoreName.Contains(Param.storeName)).ToList();
                    }
                    //状态
                    if (!string.IsNullOrEmpty(Param.storeStatus.ToString()))
                    {
                        data = data.Where(m => m.Status == Param.storeStatus).ToList();
                    }

                    Param.pageNumber = string.IsNullOrEmpty(Param.pageNumber.ToString()) ? 1 : Param.pageNumber;//默认1页
                    Param.pageSize = string.IsNullOrEmpty(Param.pageSize.ToString()) ? _pageSize : Param.pageSize;//默认1页20条记录，由配置文件决定
                    totalPageCount = (data.Count % Param.pageSize) > 0 ? (data.Count / Param.pageSize) + 1 : (data.Count / Param.pageSize);
                    data = data.Skip((Convert.ToInt32(Param.pageNumber) - 1) * Convert.ToInt32(Param.pageSize)).Take(Convert.ToInt32(Param.pageSize)).ToList();
                    foreach (var item in data)
                    {
                        StoreLst store = new StoreLst();
                        store.storeName = item.StoreName;
                        store.storeID = item.Store_ID;
                        StoreLst.Add(store);
                    }
                    StoreResult.data = StoreLst;
                    StoreResult.totalPageCount = totalPageCount;
                    result.retCode = 0;
                    result.retMessage = "成功";
                    result.isSuccess = true;
                    result.retData = StoreResult;
                    return result;
                }

            }
            catch (Exception ex)
            {
                string errData = "store获取企业仓库接口异常：" + ex.Message;
                result.retCode = 2;
                result.isSuccess = false;
                result.retMessage = errData;
                result.retData = null;
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="storeName">仓库名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>操作结果</returns>
        public List<Store> GetList(long enterpriseId, string storeName, int pageIndex, out long totalCount)
        {
            List<Store> result = new List<Store>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.Store.Where(m => m.Enterprise_Info_ID == enterpriseId
                        && m.Type == (int)Common.EnumFile.StoreType.Store && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(storeName))
                    {
                        tempResult = tempResult.Where(m => m.StoreName.Contains(storeName.Trim()) || m.StoreCode.Contains(storeName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Store_ID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加仓库
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Add(Store model)
        {
            Ret.Msg = "添加信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempModel = dataContext.Store.FirstOrDefault(p => p.StoreName == model.StoreName
                            && p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                    if (model.Type == (int)Common.EnumFile.StoreType.Slotting)
                    {
                        tempModel = dataContext.Store.FirstOrDefault(p => p.StoreName == model.StoreName && p.ParentCode == model.ParentCode
                              && p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                    }
                    if (tempModel != null)
                    {
                        Ret.Msg = "已存在该名称！";
                    }
                    else
                    {
                        int num;
                        List<Store> dList = dataContext.Store.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID).OrderByDescending(m => m.Store_ID).ToList();
                        if (dList == null || dList.Count == 0)
                        {
                            num = 1;
                        }
                        else
                        {
                            num = Convert.ToInt32(dList[0].EwmUrl.Substring(dList[0].EwmUrl.LastIndexOf('.') + 1)) + 1;
                        }
                        model.EnglishName = ChineseCode.GetChineseSpell(model.StoreName);
                        model.EwmUrl = model.EwmUrl + num.ToString();
                        dataContext.Store.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Ret.Msg = "添加信息成功";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        /// <summary>
        /// 获取仓库模型
        /// </summary>
        /// <param name="storeId">仓库标识</param>
        /// <returns>实体</returns>
        public Store GetModel(long storeId)
        {
            Store result = new Store();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Store.FirstOrDefault(m => m.Store_ID == storeId && m.Status == (int)Common.EnumFile.Status.used);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 修改仓库
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Store newModel)
        {
            Ret.Msg = "修改信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Store.FirstOrDefault(p => p.Store_ID == newModel.Store_ID && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID);

                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的信息！";
                    }
                    else
                    {
                        var a = dataContext.Store.FirstOrDefault(p => p.StoreName == newModel.StoreName && p.Store_ID != newModel.Store_ID
                            && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                        if (model.Type == (int)Common.EnumFile.StoreType.Slotting)
                        {
                            a = dataContext.Store.FirstOrDefault(p => p.StoreName == newModel.StoreName && p.ParentCode == newModel.ParentCode
                                  && p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                        }
                        if (a == null)
                        {
                            model.StoreName = newModel.StoreName;
                            if (model.Type == (int)Common.EnumFile.StoreType.Store)
                            {
                                model.StoreCode = newModel.StoreCode;
                            }
                            model.StoreDate = newModel.StoreDate;
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "已存在该名称！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="storeId">仓库标识</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long enterpriseId, long storeId)
        {
            Ret.Msg = "删除信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Store.FirstOrDefault(p => p.Store_ID == storeId && p.Enterprise_Info_ID == enterpriseId);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要删除的信息！";
                    }
                    else
                    {
                        if (model.Type == (int)Common.EnumFile.StoreType.Store)
                        {
                            List<Store> storeList = dataContext.Store.Where(p => p.ParentCode == storeId).ToList();
                            foreach (var item in storeList)
                            {
                                item.Status = (int)Common.EnumFile.Status.delete;
                            }
                        }
                        model.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Ret.Msg = "删除信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.Del()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="id">仓库id</param>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<Store> GetSlottingList(long id, long enterpriseId, out long totalCount, int pageIndex)
        {
            List<Store> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Store.Where(m => m.Enterprise_Info_ID == enterpriseId && m.ParentCode == id && m.Status == (int)Common.EnumFile.Status.used);
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Store_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.GetList():Store表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        #region 垛位管理
        /// <summary>
        /// 获取垛位码列表20170327
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="cribName">垛位名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public List<Store> GetCribList(long enterpriseId, string cribName, int pageIndex, out long totalCount)
        {
            List<Store> result = new List<Store>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.Store.Where(m => m.Enterprise_Info_ID == enterpriseId
                        && m.Type == (int)Common.EnumFile.StoreType.Crib && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(cribName))
                    {
                        tempResult = tempResult.Where(m => m.StoreName.Contains(cribName.Trim()) || m.StoreCode.Contains(cribName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Store_ID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.GetCribList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加垛位
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public RetResult AddCrib(Store model)
        {
            Ret.Msg = "添加失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempModel = dataContext.Store.FirstOrDefault(p => p.StoreName == model.StoreName
                            && p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used &&
                            p.Type == (int)Common.EnumFile.StoreType.Crib);
                    if (tempModel != null)
                    {
                        Ret.Msg = "已存在该垛位！";
                    }
                    else
                    {
                        int num;
                        List<Store> dList = dataContext.Store.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID).OrderByDescending(m => m.Store_ID).ToList();
                        if (dList == null || dList.Count == 0)
                        {
                            num = 1;
                        }
                        else
                        {
                            num = Convert.ToInt32(dList[0].EwmUrl.Substring(dList[0].EwmUrl.LastIndexOf('.') + 1)) + 1;
                        }
                        model.EnglishName = ChineseCode.GetChineseSpell(model.StoreName);
                        model.EwmUrl = model.EwmUrl + num.ToString();
                        dataContext.Store.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Ret.Msg = "添加成功";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        /// <summary>
        ///  修改垛位
        /// </summary>
        /// <param name="newModel">新实体</param>
        /// <returns></returns>
        public RetResult EditCrib(Store newModel)
        {
            Ret.Msg = "修改失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Store.FirstOrDefault(p => p.Store_ID == newModel.Store_ID && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID &&
                        p.Status == (int)Common.EnumFile.Status.used && p.Type == (int)Common.EnumFile.StoreType.Crib);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的信息！";
                    }
                    else
                    {
                        var oldModel = dataContext.Store.FirstOrDefault(p => p.StoreName == newModel.StoreName && p.Store_ID != newModel.Store_ID
                            && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used &&
                            p.Type == (int)Common.EnumFile.StoreType.Crib);
                        if (oldModel == null)
                        {
                            model.StoreName = newModel.StoreName;
                            model.EnglishName = ChineseCode.GetChineseSpell(model.StoreName);
                            if (model.Type == (int)Common.EnumFile.StoreType.Crib)
                            {
                                model.StoreCode = newModel.StoreCode;
                            }
                            model.StoreDate = newModel.StoreDate;
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "已存在该名称！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 删除垛位
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="storeId">ID</param>
        /// <returns></returns>
        public RetResult DelCrib(long enterpriseId, long storeId)
        {
            Ret.Msg = "删除失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Store.FirstOrDefault(p => p.Store_ID == storeId && p.Enterprise_Info_ID == enterpriseId &&
                        p.Status == (int)Common.EnumFile.Status.used && p.Type == (int)Common.EnumFile.StoreType.Crib);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要删除的信息！";
                    }
                    else
                    {
                        model.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Ret.Msg = "删除成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.Del()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        #endregion

        #region 20170412库存查询
        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="storeName">仓库名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>操作结果</returns>
        public List<StoreInfo> GetInventory(long enterpriseId, string maName, int pageIndex, out long totalCount)
        {
            List<StoreInfo> result = new List<StoreInfo>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.StoreInfo.Where(m => m.Enterprise_ID == enterpriseId);
                    if (!string.IsNullOrEmpty(maName))
                    {
                        tempResult = tempResult.Where(m => m.MaterialName.Contains(maName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Material_ID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.GetInventory()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取库存详情
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="maName">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        public List<View_StoreInfo> GetInventoryInfo(long enterpriseId, string maName, int pageIndex, out long totalCount)
        {
            List<View_StoreInfo> result = new List<View_StoreInfo>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_StoreInfo.Where(m => m.Enterprise_ID == enterpriseId);
                    if (!string.IsNullOrEmpty(maName))
                    {
                        tempResult = tempResult.Where(m => m.MaterialName.Contains(maName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Material_ID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.GetInventoryInfo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion
    }
}
