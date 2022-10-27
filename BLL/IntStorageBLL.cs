using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 主要用于入库统计信息管理逻辑层
    /// </summary>
    public class IntStorageBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取入库统计列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="storeName">相关信息搜索</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId, string storeName, int pageIndex)
        {
            IntStorageDAL dal = new IntStorageDAL();
            long totalCount = 0;
            List<InStatisModel> liOutStorage = dal.GetList(enterpriseId, storeName, pageIndex, out totalCount);
            liOutStorage.ForEach(a =>
            {
                switch (a.EquipType)
                {
                    case 1: a.DeviceName = "TIS客户端"; break;
                    case 2: a.DeviceName = "App客户端"; break;
                    case 3: a.DeviceName = "Wince客户端"; break;
                }
            });
            BaseResultList result = ToJson.NewListToJson(liOutStorage, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 查看入库详情
        /// </summary>
        /// <param name="oId">入库ID</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList IntStorageDetail(long oId, int pageIndex)
        {
            IntStorageDAL dal = new IntStorageDAL();
            long totalCount = 0;
            List<View_IntStorageDetail> liOutStorage = dal.IntStorageDetail(oId, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liOutStorage, pageIndex, _PageSize, totalCount, "");
            return result;
        }
    }
}
