/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-17

** 联系方式 :13313318725

** 描述：主要用于二维码（使用记录）统计业务逻辑层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 主要用于二维码（使用记录）统计业务逻辑层
    /// </summary>
    public class UsageRecordCountBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        ///  获取二维码使用记录列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetList(int index, long enterpriseId, string beginDate, string endDate, int pageIndex)
        {
            UsageRecordCountDAL dal = new UsageRecordCountDAL();
            long totalCount = 0;
            List<View_MaterialUsageRecord> liDearer = dal.GetList(index, enterpriseId, beginDate, endDate, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liDearer, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取产品模型
        /// </summary>
        /// <param name="beginCode">起始码</param>
        /// <returns></returns>
        public BaseResultModel GetMaterial(string beginCode)
        {
            UsageRecordCountDAL dal = new UsageRecordCountDAL();
            Material material = dal.GetMaterial(beginCode);
            string code = "1";
            string msg = "";
            if (material == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(material, code, msg);
            return model;
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id">记录id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetRecordDetail(long id, int pageIndex)
        {
            long totalCount = 0;
            UsageRecordCountDAL dal = new UsageRecordCountDAL();
            List<LinqModel.Enterprise_FWCode_00> DataList = dal.GetRecordDetail(id, pageIndex, out totalCount);
            return ToJson.NewListToJson(DataList, pageIndex, _PageSize, totalCount, "");
        }

        /// <summary>
        /// 获取要导出的码内容
        /// </summary>
        /// <param name="id">记录id</param>
        /// <returns></returns>
        public string GetExportTxt(long id)
        {
            return new UsageRecordCountDAL().GetExportTxt(id);
        }
    }
}
