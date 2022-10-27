/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-8-10
** 联系方式:13313318725
** 代码功能：活动关联二维码操作
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Linq;
using System.Web.Mvc;
using iagric_plant.Areas.Admin.Filter;
using BLL;
using LinqModel;
using Common.Argument;
using Webdiyer.WebControls.Mvc;
using Aspose.Cells;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 红包活动
    /// </summary>
    [ManAuthorizeAttribute]
    public class ManActivityRelationController : Controller
    {
        /// <summary>
        /// 红包活动
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回列表</returns>
        public ActionResult AdminIndex(int? id, string comName, string startTime, string endTime, string title)
        {
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            ViewBag.Name = comName;
            ViewBag.Title = title;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ActivityRelationCodeBLL bll = new ActivityRelationCodeBLL();
            PagedList<View_ActivityManager> list = bll.AdminGetList(pageIndex, comName, startTime, endTime, title, SessCokie.GetMan.EnterpriseID);
            return View(list);
        }

        /// <summary>
        /// 查看
        /// </summary>
        /// <param name="activityId">关联ID</param>
        /// <returns></returns>
        public ActionResult AdminInfo(long activityId)
        {
            var model = new ActivityRelationCodeBLL().GetInfo(activityId);
            var lst = new ActivityBLL().GetDetail(activityId);
            ViewBag.detailLst = lst;
            ViewBag.hbCount = lst.Sum(a => a.RedCount);
            ViewBag.sumCount = lst.Sum(a => a.RedValue * a.RedCount);
            ViewBag.Open = SessCokie.GetMan.PRRU_PlatFormLevel_ID == 3 ? true : false;
            return View(model);
        }

        /// <summary>
        /// 设置活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult SetActivity(long activityId)
        {
            var model = new ActivityRelationCodeBLL().GetInfo(activityId);
            var lst = new ActivityBLL().GetDetail(activityId);
            ViewBag.detailLst = lst;
            ViewBag.hbCount = lst.Sum(a => a.RedCount);
            ViewBag.sumCount = lst.Sum(a => a.RedValue * a.RedCount);
            ViewBag.Open = SessCokie.GetMan.PRRU_PlatFormLevel_ID == 3 ? true : false;
            var coupon = new CouponBLL().GetActivitySub(activityId);
            ViewBag.CouponType = coupon == null ? 0 : coupon.CouponType;
            ViewBag.CouponContent=coupon==null?"":coupon.CouponContent;
            return View(model);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetActivity(long activityId, string url, int payState, int openMode = -1)
        {
            RetResult result = new ActivityRelationCodeBLL().SetActivity(activityId, openMode, url, payState);
            return Json(new { ok = result.IsSuccess, msg = result.Msg });
        }

        /// <summary>
        /// 下载单据
        /// </summary>
        /// <param name="activityId"></param>
        public void DownLoad(long activityId)
        {
            Response.Clear();
            Response.Buffer = true;
            try
            {
                var model = new ActivityRelationCodeBLL().GetInfo(activityId);
                var lst = new ActivityBLL().GetDetail(activityId);
                Workbook wb = new Workbook(AppDomain.CurrentDomain.BaseDirectory + "/Areas/Admin/File/单据模板.xlsx");
                Cells cells = wb.Worksheets[0].Cells;
                cells[1, 1].Value = model.EnterpriseName;
                cells[2, 1].Value = model.StartDate.Value.ToString("yyyy-MM-dd") + "至" + model.EndDate.Value.ToString("yyyy-MM-dd");
                int i = 4;
                int j = 1;
                foreach (YX_AcivityDetail item in lst)
                {
                    cells[i, 1].Value = j;
                    cells[i, 2].Value = item.RedValue;
                    cells[i, 3].Value = item.RedCount;
                    cells[i, 4].Value = item.RedValue * item.RedCount;
                    i++;
                    j++;
                }
                cells[9, 2].Value = lst.Sum(a => a.RedCount * a.RedValue);
                cells[10, 2].Value = model.RechargeValue - lst.Sum(a => a.RedCount * a.RedValue);
                cells[11, 2].Value = model.RechargeValue;
                cells[12, 1].Value = Common.EnumText.EnumToText(typeof(Common.EnumText.PayType), model.RechargeMode.Value);
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    wb.Save(ms, SaveFormat.Xlsx);
                    Response.BinaryWrite(ms.ToArray());
                }
                Response.Flush();
                Response.End();
            }
            catch (Exception)
            {
                Response.ContentType = "text/html";
                Response.Write("<script>alert('下载过程中出现错误！');history.go(-1);</script>");
            }

        }

        /// <summary>
        /// 查看图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult ImageSeach(string url)
        {
            ViewBag.Url = url;
            return View();
        }
    }
}
