/********************************************************************************

** 作者： 张翠霞

** 创始时间：2017-05-05

** 联系方式 :13313318725

** 描述：主要用于出库统计信息管理逻辑层

** 版本：v2.5

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Common.Argument;
using System.Configuration;
using Dal;

namespace BLL
{
    /// <summary>
    /// 主要用于出库统计信息管理逻辑层
    /// </summary>
    public class OutStorageBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取出库统计列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="storeName">相关信息搜索</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId, string storeName, int pageIndex)
        {
            OutStorageDAL dal = new OutStorageDAL();
            long totalCount = 0;
            List<OutStatisModel> liOutStorage = dal.GetList(enterpriseId, storeName, pageIndex, out totalCount);
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
        /// 查看出库详情
        /// </summary>
        /// <param name="oId">出库ID</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList OutStorageDetail(long oId, int pageIndex)
        {
            OutStorageDAL dal = new OutStorageDAL();
            long totalCount = 0;
            List<View_OutStorageDetail> liOutStorage = dal.OutStorageDetail(oId, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liOutStorage, pageIndex, _PageSize, totalCount, "");
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
            Dal.OutStorageDAL dal = new OutStorageDAL();
            return dal.AddOutStorage(model, out outID);
        }

        /// <summary>
        /// 添加出库明细
        /// 陈志钢 WInce
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOutStorageDetail(List<OutStorageDetail> model)
        {
            Dal.OutStorageDAL dal = new OutStorageDAL();
            RequestCodeDAL dalCode = new RequestCodeDAL();
            OutStorageTable table = dal.getOutTableInfo(model[0].OutStorageID);
            foreach (OutStorageDetail m in model)
            {
                Enterprise_FWCode_00 codeBegin = dalCode.GetEWM(m.EWMCode);
                int CodeType = (int)codeBegin.Type;
                dalCode.SaleCodeSingle((long)table.EnterpriseID, CodeType, Convert.ToInt64(table.DealerID), codeBegin, codeBegin,0, DateTime.Now);
            }
            return dal.AddOutStorageDetail(model);
        }
    }
}
