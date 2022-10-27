/********************************************************************************
** 作者： 赵慧敏
** 创始时间：2017-3-19
** 联系方式 :13313318725
** 描述：推荐产品
** 版本：v1.0
** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 推荐产品
    /// </summary>
    public class RecommendBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        RecommendDAL _Dal = new RecommendDAL();

        /// <summary>
        /// 查询推荐产品列表
        /// </summary>
        /// <param name="enterpriseId">企业编号</param>
        /// <param name="name">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId, string name, int pageIndex)
        {
            long totalCount = 0;
            List<View_Recommend> liResult = _Dal.GetList(enterpriseId, name, out totalCount, pageIndex);
            return ToJson.NewListToJson(liResult, pageIndex, _PageSize, totalCount, "");
        }

        /// <summary>
        /// 添加推荐产品
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        public BaseResultModel Add(Recommend model)
        {
            RetResult result = _Dal.Add(model);
            return ToJson.NewRetResultToJson((Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 删除推荐
        /// </summary>
        /// <param name="id">产品编号</param>
        /// <returns></returns>
        public BaseResultModel Del(long id)
        {
            RetResult result = _Dal.Del(id);
            return ToJson.NewRetResultToJson((Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 审核推荐产品
        /// </summary>
        /// <param name="id">产品编号</param>
        /// <param name="type">审核结果</param>
        /// <returns></returns>
        public BaseResultModel Verify(long id, int type)
        {
            RetResult result = _Dal.Verify(id, type);
            return ToJson.NewRetResultToJson((Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 查询推荐企业列表
        /// </summary>
        /// <param name="adminId">管理编号</param>
        /// <param name="name">企业名称</param>
        /// <param name="totalCount">数量</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="type">审核类型</param>
        /// <returns></returns>
        public BaseResultList GetAdminList(long adminId, string name, int pageIndex, int type)
        {
            long totalCount = 0;
            List<View_Recommend> liResult = _Dal.GetAdminList(adminId, name, out totalCount, pageIndex, type);
            return ToJson.NewListToJson(liResult, pageIndex, _PageSize, totalCount, "");
        }

        /// <summary>
        /// 获取推荐企业列表
        /// </summary>
        /// <param name="platId">监管部门编号</param>
        /// <returns></returns>
        public BaseResultList GetEnterpriseList(long platId)
        {
            List<Enterprise_Info> liResult = _Dal.GetEnterpriseList(platId);
            return ToJson.NewListToJson(liResult, 1, liResult.Count(), liResult.Count(), "");
        }

        /// <summary>
        /// 监管部门推荐企业
        /// </summary>
        /// <param name="arrayId">推荐企业串</param>
        /// <returns></returns>
        public BaseResultModel AdminAdd(string arrayId,long enterpriseID)
        {
            RetResult result = _Dal.AdminAdd(arrayId,enterpriseID);
            return ToJson.NewRetResultToJson((Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 拍码页码查询推广的产品
        /// </summary>
        /// <param name="enterpriseId">企业编号</param>
        /// <param name="materialId">拍码产品编号</param>
        /// <returns></returns>
        public List<MyRecommend> GetScanRecommend(long enterpriseId, long materialId)
        {
            List<MyRecommend> result = _Dal.GetScanRecommend(enterpriseId, materialId);
            return result;
        }
    }
}
