/********************************************************************************

** 作者： 张翠霞（移植）

** 创始时间：2016-03-23

** 联系方式 :13313318725

** 描述：主要用于评论信息的数据访问

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    public class OrderCommentDAL : DALBase
    {
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
        public List<View_Order_Comment> GetList(long enterpriseId, string name, string beginTime, string endTime, int pageIndex, out long totalCount)
        {
            List<View_Order_Comment> result = new List<View_Order_Comment>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                //查询出消费者评论数据
                var dataResult = dataContext.View_Order_Comment.Where(m => m.ParentID == 0);
                if (enterpriseId > 0)
                {
                    dataResult = dataResult.Where(m => m.Enterprise_ID == enterpriseId);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    dataResult = dataResult.Where(m => m.MaterialFullName.Contains(name)
                        || m.LinkMan.Contains(name) || m.Content.Contains(name)
                        || m.ReContent.Contains(name));
                }
                if (!string.IsNullOrEmpty(beginTime))
                {
                    dataResult = dataResult.Where(m => Convert.ToDateTime(m.AddTime) > Convert.ToDateTime(beginTime));
                }
                if (!string.IsNullOrEmpty(endTime))
                {
                    dataResult = dataResult.Where(m => Convert.ToDateTime(m.AddTime) < Convert.ToDateTime(endTime).AddDays(1));
                }
                totalCount = dataResult.Count();
                result = dataResult.OrderByDescending(m => m.Order_Comment_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
            }
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
            PagedList<View_Order_Comment> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Order_Comment where m.Material_ID == materialId && m.ParentID == 0 select m;
                    if (level > 0 && level < 4)
                    {
                        data = data.Where(m => m.Level == level);
                    }
                    if (consumersId > 0)
                    {
                        data = data.Where(m => m.Order_Consumers_ID == consumersId || m.Order_Consumers_ID == null);
                    }
                    data = data.OrderByDescending(m => m.AddTime);
                    result = data.ToPagedList(pageIndex, PageSize);
                }
                catch
                {
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取评论实体
        /// </summary>
        /// <param name="commentId">评论标识</param>
        /// <param name="parentId">父级评论标识</param>
        /// <returns>实体</returns>
        public View_Order_Comment GetModel(long commentId, long parentId)
        {
            View_Order_Comment result = new View_Order_Comment();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                //评论ID大于0根据ID查询实体
                if (commentId > 0)
                {
                    result = dataContext.View_Order_Comment.FirstOrDefault(m => m.Order_Comment_ID == commentId);
                }
                //否则根据查出父级ID为parentId的评论
                else
                {
                    result = dataContext.View_Order_Comment.FirstOrDefault(m => m.ParentID == parentId);
                }
            }
            return result;
        }

        /// <summary>
        /// 评论及回复评论
        /// </summary>
        /// <param name="model">评论实体</param>
        /// <returns>操作结果</returns>
        public RetResult AddComment(Order_Comment model)
        {
            string Msg = "评论失败！";
            //如果存在parentID则为回复
            if (model.ParentID != 0)
            {
                Msg = "回复失败！";
            }
            CmdResultError Error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    if (model.ParentID != null && model.ParentID != 0)
                    {
                        Order_Comment comment = dataContext.Order_Comment.FirstOrDefault(m => m.Order_Comment_ID == model.ParentID);
                        model.Material_ID = comment.Material_ID;
                        model.OrderNum = comment.OrderNum;
                    }
                    dataContext.Order_Comment.InsertOnSubmit(model);
                    dataContext.SubmitChanges();

                    Msg = "评论成功！";
                    if (model.ParentID != 0)
                    {
                        Msg = "回复成功！";
                    }
                    Error = CmdResultError.NONE;
                }
            }
            catch { Msg = "链接服务器失败！"; }
            Ret.SetArgument(Error, Msg, Msg);
            return Ret;
        }
    }
}
