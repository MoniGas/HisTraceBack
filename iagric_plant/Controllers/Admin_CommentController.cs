/********************************************************************************


** 作者： 张翠霞（移植）

** 创始时间：2016-03-24

** 联系方式 :13313318725

** 描述：主要用于评论信息管理控制器

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_CommentController : BaseController
    {
        //
        // GET: /Admin_Comment/

        /// <summary>
        /// 查看评论Action
        /// </summary>
        /// <param name="name">根据产品名称、联系人、评论、回复模糊查询字段</param>
        /// <param name="bDate">评论时间段</param>
        /// <param name="eDate">评论时间段</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>视图</returns>
        public ActionResult Index(string name, string bDate, string eDate, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                OrderCommentBLL orderCommentBll = new OrderCommentBLL();
                result = orderCommentBll.GetList(pf.EnterpriseID, name, bDate, eDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Comment.Index():Order_Comment表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 评论实体
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>视图</returns>
        public ActionResult Info(long commentId)
        {
            OrderCommentBLL bll = new OrderCommentBLL();
            BaseResultModel objResult = bll.GetModel(commentId);
            return Json(objResult);
        }

        /// <summary>
        /// 回复商品
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <param name="content">内容</param>
        /// <returns>视图</returns>
        public ActionResult Add(long commentId, string content)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.OrderCommentBLL().AddComment(pf.EnterpriseID, commentId, content);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Comment.Add():Order_Comment表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
