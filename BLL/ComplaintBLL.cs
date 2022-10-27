/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：       

** 主要用于投诉管理业务层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class ComplaintBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取投诉管理未读信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultList GetList(long id)
        {
            ComplaintDAL objComplaintDAL = new ComplaintDAL();
            int dataCount = 0;
            List<View_ComplaintAndType> dataList = objComplaintDAL.GetList(id, out dataCount);

            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataCount, "");
        }

        /// <summary>
        /// 获取投诉信息列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="search">搜索信息</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetList(long enterpriseId, string search, int pageIndex)
        {
            long totalCount = 0;
            ComplaintDAL dal = new ComplaintDAL();
            List<View_ComplaintAndType> model = dal.GetList(enterpriseId, search, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 修改投诉信息为已读
        /// </summary>
        /// <param name="id">投诉列表ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel UpdateStatus(long id)
        {
            ComplaintDAL objComplaintDAL = new ComplaintDAL();
            RetResult objRetResult = objComplaintDAL.UpdateStatus(id);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        public BaseResultModel Del(long enterpriseId, long complaintid)
        {
            ComplaintDAL dal = new ComplaintDAL();
            RetResult ret = dal.Del(enterpriseId, complaintid);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 消费者投诉
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>返回结果</returns>
        public RetResult AddComplaint(Complaint model)
        {
            ComplaintDAL dal = new ComplaintDAL();
            RetResult result = dal.AddComplaint(model);
            return result;
        }

        /// <summary>
        /// 获取监管部门信息
        /// </summary>
        /// <param name="PRRU_PlatFormId">监管部门Id</param>
        /// <returns></returns>
        public LinqModel.PRRU_PlatForm GetPlatForm(long PRRU_PlatFormId)
        {
            ComplaintDAL dal = new ComplaintDAL();
            LinqModel.PRRU_PlatForm DataModel = dal.GetPlatForm(PRRU_PlatFormId);
            return DataModel;
        }

        /// <summary>
        /// 获取主页统计数据
        /// </summary>
        /// <param name="enterpriseID"></param>
        /// <returns></returns>
        public BaseResultModel GetHomeDataStatis(long enterpriseID)
        {
            BaseResultModel model = new BaseResultModel();
            ComplaintDAL dal = new ComplaintDAL();
            string url = string.Empty;
            HomeDataStatis dataStatis = dal.GetHomeDataStatis(enterpriseID);
            string code = "1";
            string msg = "";
            if (dataStatis == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            model = ToJson.NewModelToJson(dataStatis, code, msg);
            return model;
        }

        public BaseResultList GetChartData(long enterpriseId)
        {
            long totalCount = 0;
            ComplaintDAL dal = new ComplaintDAL();
            List<ChartData> model = dal.GetChartData(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }
    }
}
