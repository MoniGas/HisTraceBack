/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-4

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于消息查询控制器

*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class NewsController : Controller
    {
        /// <summary>
        /// 获取二维码消息列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEwmNewsList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList dataList = new BaseResultList();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();

                dataList = objNewsBLL.GetEMWList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "NewsController.GetEwmNewsList():View_RequestCodeAndEnterprise_Info视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }
        /// <summary>
        /// 修改二维码消息已读状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult UpdateEwmStatus(long id)
        {
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();

                dataList = objNewsBLL.UpdateStatus(id);
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "NewsController.UpdateEwmStatus():RequestCode表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        public JsonResult IgnoreEwm()
        {
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();

                dataList = objNewsBLL.IgnoreEwm();
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "NewsController.UpdateEwmStatus():RequestCode表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        /// <summary>
        /// 查询申请加入区域品牌消息列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBrandNewsList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList dataList = new BaseResultList();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();

                dataList = objNewsBLL.GetBrandList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "NewsController.GetBrandNewsList():View_RequestBrand视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(dataList);
        }

        /// <summary>
        /// 修改申请加入区域品牌消息已读状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult UpdateBrandStatus(long id)
        {
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();

                dataList = objNewsBLL.UpdateBrand(id);
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "NewsController.UpdateBrandStatus():Material表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        public JsonResult IgnoreBrand()
        {
            BaseResultModel dataList = new BaseResultModel();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();

                dataList = objNewsBLL.IgnoreBrand();
            }
            catch (Exception ex)
            {
                dataList = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "NewsController.UpdateBrandStatus():Material表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        /// <summary>
        /// 查询企业是否审核
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEnterpriseVerifyList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList dataList = new BaseResultList();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();
                dataList = objNewsBLL.GetEnterpriseVerifyList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "NewsController.GetEnterpriseVerifyList():EnterpriseVerify视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }

        public JsonResult GetCodeRecord()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList dataList = new BaseResultList();
            try
            {
                NewsBLL objNewsBLL = new NewsBLL();
                dataList = objNewsBLL.GetCodeRecord(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "NewsController.GetCodeRecord():EnterpriseVerify视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(dataList);
        }
    }
}
