using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 主要用于入库统计信息管理数据层
    /// </summary>
    public class IntStorageDAL : DALBase
    {
        /// <summary>
        /// 获取入库统计ID
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="storeName">相关信息搜索</param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public List<InStatisModel> GetList(long enterpriseId, string storeName, int pageIndex, out long totalCount)
        {
            List<View_IntStorageStatis> result = new List<View_IntStorageStatis>();
            List<InStatisModel> resultM = new List<InStatisModel>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_IntStorageStatis.Where(m => m.EnterpriseID == enterpriseId);
                    if (!string.IsNullOrEmpty(storeName))
                    {
                        tempResult = tempResult.Where(m => m.StoreName.Contains(storeName.Trim()) || m.IntStorageNO.Contains(storeName.Trim()) ||
                            m.DealerName.Contains(storeName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.IntStorageID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            InStatisModel temp = new InStatisModel();
                            temp.DealerID = item.DealerID;
                            temp.DealerName = item.DealerName;
                            temp.DeviceName = item.DeviceName;
                            temp.EnterpriseID = item.EnterpriseID;
                            temp.EquipType = item.EquipType;
                            temp.IntStorageDate = item.IntStorageDate;
                            temp.IntStorageID = item.IntStorageID;
                            temp.IntStorageNO = item.IntStorageNO;
                            temp.StorageHouse = item.StorageHouse;
                            temp.StoreName = item.StoreName;
                            temp.StrIntStorageDate = item.StrIntStorageDate;
                            var tempDetail = dataContext.IntStorageDetail.Where(m => m.IntStorageID == item.IntStorageID);
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
        /// 查看入库详情
        /// </summary>
        /// <param name="oId">入库ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<View_IntStorageDetail> IntStorageDetail(long oId, int pageIndex, out long totalCount)
        {
            List<View_IntStorageDetail> result = new List<View_IntStorageDetail>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_IntStorageDetail.Where(m => m.IntStorageID == oId && m.IsOutStore != 4);
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.IntStorageID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
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
    }
}
