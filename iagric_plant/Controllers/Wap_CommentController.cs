/********************************************************************************


** 作者： 张翠霞（移植）

** 创始时间：2016-03-23

** 联系方式 :13313318725

** 描述：主要用于评论信息管理控制器

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace iagric_plant.Controllers
{
    public class Wap_CommentController : Controller
    {
        //
        // GET: /Wap_Comment/
        /// <summary>
        /// 查看评论Action
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="materialId">产品ID</param>
        /// <param name="orderNum">订单号</param>
        /// <param name="level">评论级别</param>
        /// <param name="consumersId">消费者ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="type">返回页面编号</param>
        /// <param name="pl">是否可以评论</param>
        /// <returns>页面</returns>
        public ActionResult Index(long enterpriseId, long materialId, string orderNum = "", int level = 0, long consumersId = 0, int pageIndex = 1, int type = 1, int pl = 0)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                PagedList<View_Order_Comment> result = null;
                try
                {
                    result = new BLL.OrderCommentBLL().GetList(materialId, level, consumersId, pageIndex);
                }
                catch { }
                ViewBag.type = type;
                ViewBag.pl = pl;
                ViewBag.orderNum = orderNum;
                ViewBag.level = level;
                ViewBag.consumersId = consumers.Order_Consumers_ID;
                ViewBag.materialId = materialId;
                ViewBag.enterpriseId = enterpriseId;
                return View(result);
            }
        }

        public ActionResult MyIndex(long enterpriseId, long materialId, string orderNum = "", int level = 0, long consumersId = 0, int pageIndex = 1, int type = 1, int pl = 0)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login';</script>");
            }
            else
            {
                consumersId = consumers.Order_Consumers_ID;
                PagedList<View_Order_Comment> result = null;
                try
                {
                    result = new BLL.OrderCommentBLL().GetList(materialId, level, consumersId, pageIndex);
                }
                catch { }
                ViewBag.type = type;
                ViewBag.pl = pl;
                ViewBag.orderNum = orderNum;
                ViewBag.level = level;
                ViewBag.consumersId = consumersId;
                ViewBag.materialId = materialId;
                ViewBag.enterpriseId = enterpriseId;
                return View(result);
            }
        }

        /// <summary>
        /// 添加评论ActionGet页面
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 添加评论ActionPost页面
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult Add(string orderNum)
        {
            LinqModel.View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (consumers == null)
            {
                return Content("<script>alert('请先登录后再进行此操作！');window.location.href = '/Wap_Order/Login?pageType=4';</script>");
            }
            else
            {
                Order_Comment comment = new Order_Comment();
                comment.AddTime = DateTime.Now;
                comment.CommentType = 1;
                comment.Content = Request["TextContent"];
                comment.Enterprise_ID = Convert.ToInt64(Request["enterpriseId"]);
                comment.IsRead = false;
                comment.Level = Convert.ToInt32(Request["rdLevel"]);
                comment.Material_ID = Convert.ToInt64(Request["materialId"]);
                comment.Order_Consumers_ID = consumers.Order_Consumers_ID;
                comment.ParentID = 0;
                comment.OrderNum = Request["orderNum"];

                RetResult ret = new BLL.OrderCommentBLL().AddComment(comment);

                JsonResult js = new JsonResult();
                js.Data = new { res = ret.IsSuccess, info = ret.Msg };
                return js;
            }
        }
    }
}
