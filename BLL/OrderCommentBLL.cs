/********************************************************************************

** 作者： 张翠霞（移植）

** 创始时间：2016-03-23

** 联系方式 :13313318725

** 描述：主要用于评论信息的业务逻辑

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using Common.Argument;
using Dal;
using System.Configuration;

namespace BLL
{
    public class OrderCommentBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        #region 获取品论列表
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="name">根据产品名称、联系人、评论、回复模糊查询字段</param>
        /// <param name="beginTime">留言时间（开始）</param>
        /// <param name="endTime">留言时间（结束）</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总页数</param>
        /// <returns>结果集</returns>
        public BaseResultList GetList(long enterpriseId, string name, string beginTime, string endTime, int pageIndex)
        {
            long totalCount = 0;
            OrderCommentDAL dal = new OrderCommentDAL();
            List<View_Order_Comment> model = dal.GetList(enterpriseId, name, beginTime, endTime, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// wap获取评论列表
        /// </summary>
        /// <param name="materialId">产品ID</param>
        /// <param name="level">好评中评差评</param>
        /// <param name="consumersId">消费者ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>结果集</returns>
        public PagedList<View_Order_Comment> GetList(long materialId, int level, long consumersId, int pageIndex)
        {
            OrderCommentDAL dal = new OrderCommentDAL();
            PagedList<View_Order_Comment> result = dal.GetList(materialId, level, consumersId, pageIndex);
            return result;
        }
        #endregion

        #region 获取评论实体
        /// <summary>
        /// 获取评论实体
        /// </summary>
        /// <param name="commentId">评论标识</param>
        /// <param name="parentId">父级评论标识</param>
        /// <returns>实体</returns>
        public View_Order_Comment GetModel(long commentId, long parentId)
        {
            return new Dal.OrderCommentDAL().GetModel(commentId, parentId);
        }
        /// <summary>
        /// 获取评论实体
        /// </summary>
        /// <param name="commentId">评论标识</param>
        /// <returns>实体</returns>
        public BaseResultModel GetModel(long commentId)
        {
            View_Order_Comment result = new Dal.OrderCommentDAL().GetModel(commentId, 0);
            return ToJson.NewModelToJson(result, result == null ? "0" : "1", "");
        }
        #endregion

        #region 添加评论、回复评论
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="model">评论实体</param>
        /// <returns>操作结果</returns>
        public RetResult AddComment(Order_Comment model)
        {
            return new OrderCommentDAL().AddComment(model);
        }
        /// <summary>
        /// 回复评论
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="commentId">评论ID</param>
        /// <param name="content">回复内容</param>
        /// <returns>操作结果</returns>
        public BaseResultModel AddComment(long enterpriseId, long commentId, string content)
        {
            Order_Comment comment = new Order_Comment();
            comment.AddTime = DateTime.Now;
            comment.CommentType = 2;
            comment.Content = content;
            comment.Enterprise_ID = enterpriseId;
            comment.IsRead = false;
            comment.Level = 0;
            comment.Order_Consumers_ID = 0;
            comment.ParentID = commentId;
            RetResult ret = new OrderCommentDAL().AddComment(comment);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        #endregion
    }
}
