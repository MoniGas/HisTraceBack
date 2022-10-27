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

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 推荐产品
    /// </summary>
    public class Admin_RecommendController : Controller
    {
        /// <summary>
        /// 查询推荐产品列表
        /// </summary>
        /// <param name="enterpriseId">企业编号</param>
        /// <param name="name">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public ActionResult GetList(string name, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                result = new RecommendBLL().GetList(pf.EnterpriseID, name, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Recommend.Index():Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加推荐产品
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        public ActionResult Add(long materialId, long settingId, string recommendName)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Recommend model = new Recommend();
                model.AddTime = DateTime.Now;
                model.CodeIndex = 1;
                model.EnterpriseID = pf.EnterpriseID;
                model.MaterialID = materialId;
                model.RecommendName = recommendName;
                model.SettingID = settingId;
                model.Verify = 0;
                model.Type = 1;
                result = new RecommendBLL().Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Recommend.Add():Recommend表";
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
                result = new RecommendBLL().Del(id);
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
        /// 查询配置类别
        /// </summary>
        /// <param name="materialId">产品编码</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public ActionResult GetSettinglist(long materialId, int pageIndex)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new RequestCodeMaBLL().GetRequestCodeSettingListAll(materialId, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_RecommendController.GetSettinglist()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取子批次列表
        /// </summary>
        /// <param name="requestId">申请码记录标识列</param>
        /// <returns>子批次列表</returns>
        public ActionResult GetSublist(long requestId)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.GetRequestCodeSettingListSubR(requestId);
            }
            catch (Exception ex)
            {
                string errData = "RequestCodeMaController.RequestCodeSettinglistSub()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
