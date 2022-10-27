/********************************************************************************
** 作者：赵慧敏
** 创始时间：2017-3-19
** 联系方式 :13313318725
** 描述：推荐产品
** 版本：v1.0
** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 推荐产品
    /// </summary>
    public class SysRecommendController : Controller
    {
        RecommendBLL _Bll = new RecommendBLL();

        /// <summary>
        /// 查询推荐企业列表
        /// </summary>
        /// <param name="adminId">管理编号</param>
        /// <param name="name">企业名称</param>
        /// <param name="totalCount">数量</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="type">审核类型</param>
        /// <returns></returns>
        public ActionResult GetList(string name, int pageIndex, int type)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                result = _Bll.GetAdminList(pf.EnterpriseID, name, pageIndex, type);
            }
            catch (Exception ex)
            {
                string errData = "SysRecommend.GetList():Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 审核推荐产品
        /// </summary>
        /// <param name="id">产品编号</param>
        /// <param name="type">审核结果</param>
        /// <returns></returns>
        public ActionResult Verify(long id, int type)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = _Bll.Verify(id, type);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Recommend.Del():Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取推荐企业列表
        /// </summary>
        /// <param name="platId">监管部门编号</param>
        /// <returns></returns>
        public ActionResult GetEnterpriseList()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                result = _Bll.GetEnterpriseList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "SysRecommend.GetList():Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除推荐
        /// </summary>
        /// <param name="id">产品编号</param>
        /// <returns></returns>
        public ActionResult Del(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = _Bll.Del(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Recommend.Del():Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 监管部门推荐企业
        /// </summary>
        /// <param name="arrayId">推荐企业串</param>
        /// <returns></returns>
        public ActionResult Add(string arrayId)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = _Bll.AdminAdd(arrayId,pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Recommend.Add():Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
