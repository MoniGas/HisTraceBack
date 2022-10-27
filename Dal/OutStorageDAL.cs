/********************************************************************************

** 作者： 张翠霞

** 创始时间：2017-05-05

** 联系方式 :13313318725

** 描述：主要用于出库统计信息管理数据层

** 版本：v2.5

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 主要用于出库统计信息管理数据层
    /// </summary>
    public class OutStorageDAL : DALBase
    {
        /// <summary>
        /// 获取出库统计ID
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="storeName">相关信息搜索</param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public List<OutStatisModel> GetList(long enterpriseId, string storeName, int pageIndex, out long totalCount)
        {
            List<View_OutStorageStatis> result = new List<View_OutStorageStatis>();
            List<OutStatisModel> resultM = new List<OutStatisModel>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_OutStorageStatis.Where(m => m.EnterpriseID == enterpriseId);
                    if (!string.IsNullOrEmpty(storeName))
                    {
                        tempResult = tempResult.Where(m => m.StoreName.Contains(storeName.Trim()) || m.OutStorageNO.Contains(storeName.Trim()) ||
                            m.DealerName.Contains(storeName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.OutStorageID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            OutStatisModel temp = new OutStatisModel();
                            temp.DealerID = item.DealerID;
                            temp.DealerName = item.DealerName;
                            temp.DeviceName = item.DeviceName;
                            temp.EnterpriseID = item.EnterpriseID;
                            temp.EquipType = item.EquipType;
                            temp.OutStorageDate = item.OutStorageDate;
                            temp.OutStorageID = item.OutStorageID;
                            temp.OutStorageNO = item.OutStorageNO;
                            temp.StorageHouse = item.StorageHouse;
                            temp.StoreName = item.StoreName;
                            temp.StrOutStorageDate = item.StrOutStorageDate;
                            var tempDetail = dataContext.OutStorageDetail.Where(m => m.OutStorageID == item.OutStorageID);
                            if (tempDetail.Count() > 0)
                            {
                                temp.MaCount = tempDetail.Sum(m => m.MaCount);
                            }
                            else
                            {
                                temp.MaCount = 0;
                            }
                            resultM.Add(temp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "StoreDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return resultM;
        }

        /// <summary>
        /// 查看出库详情
        /// </summary>
        /// <param name="oId">出库ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<View_OutStorageDetail> OutStorageDetail(long oId, int pageIndex, out long totalCount)
        {
            List<View_OutStorageDetail> result = new List<View_OutStorageDetail>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_OutStorageDetail.Where(m => m.OutStorageID == oId && m.IsOutStore != 5);
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.OutStorageID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
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
        /// 添加出库
        /// 陈志钢 WInce
        /// </summary>
        /// <param name="model"></param>
        /// <param name="outID"></param>
        /// <returns></returns>
        public bool AddOutStorage(OutStorageTable model, out long outID)
        {
            outID = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.OutStorageTable.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    outID = model.OutStorageID;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 添加出库明细
        /// 陈志钢 WInce
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOutStorageDetail(List<OutStorageDetail> model)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.OutStorageDetail.InsertAllOnSubmit(model);
                    dataContext.SubmitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取出库单信息 陈志钢 2018年1月2日
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public OutStorageTable getOutTableInfo(long tableID)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.OutStorageTable.FirstOrDefault(p => p.OutStorageID == tableID);
            }
        }
    }
}
