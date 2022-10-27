/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-12-19

** 联系方式 :13313318725

** 描述：原料管理控制器 移植

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 原料管理控制器
    /// </summary>
    public class AdminOriginController : Controller
    {
        //
        // GET: /AdminOrigin/

        #region 原料管理
        /// <summary>
        /// 获取原料列表
        /// </summary>
        /// <param name="originName">原料名称</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public JsonResult Index(string originName, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                OriginBLL originBll = new OriginBLL();
                result = originBll.GetList(pf.EnterpriseID, originName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "AdminOrigin.Index():Origin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加原料信息
        /// </summary>
        /// <param name="originName">原料名称</param>
        /// <param name="descriptions">原料描述</param>
        /// <param name="originImgInfo">原料图片</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add(string originName, string descriptions, string originImgInfo)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Origin model = new Origin();
                model.Enterprise_Info_ID = pf.EnterpriseID;
                model.OriginName = originName;
                model.Descriptions = descriptions;
                //model.OriginImgInfo = originImgInfo;
                model.StrOriginImgInfo = originImgInfo;
                model.adduser = pf.UserID;
                model.adddate = DateTime.Now;
                model.lastuser = pf.UserID;
                model.lastdate = model.adddate;
                model.Status = (int)Common.EnumFile.Status.used;
                OriginBLL originBll = new OriginBLL();
                result = originBll.Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "AdminOrigin.Add():Origin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改原料信息
        /// </summary>
        /// <param name="id">原料ID</param>
        /// <param name="originName">原料名称</param>
        /// <param name="descriptions">原料描述</param>
        /// <param name="originOriginImgInfo">原料图片</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string id, string originName, string descriptions, string originOriginImgInfo)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Origin model = new Origin();
                model.Origin_ID = Convert.ToInt64(id);
                model.Enterprise_Info_ID = pf.EnterpriseID;
                model.OriginName = originName;
                model.Descriptions = descriptions;
                model.StrOriginImgInfo = originOriginImgInfo;
                //model.OriginImgInfo = originImgInfo;
                OriginBLL originBLL = new OriginBLL();
                result = originBLL.Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "AdminOrigin.Edit():Origin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除原料
        /// </summary>
        /// <param name="originId">原料ID</param>
        /// <returns></returns>
        public JsonResult Delete(long originId)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                OriginBLL originBll = new OriginBLL();
                result = originBll.Del(originId, pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "AdminOrigin.Delete():Origin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取修改的原料实体类信息
        /// </summary>
        /// <param name="id">原料ID</param>
        /// <returns></returns>
        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                OriginBLL bll = new OriginBLL();
                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "AdminOrigin.GetModel():Origin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取原料列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOriginList()
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                OriginBLL bll = new OriginBLL();
                result = bll.GetOriginList(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "AdminOrigin.GetOriginList():Origin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="flag">标识符</param>
        /// <param name="value">输入值</param>
        /// <returns></returns>
        public JsonResult GetSearchInfo(string page, int flag, string value)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                OriginBLL bll = new OriginBLL();
                result = bll.GetDictoryKey(page, flag, value, user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "AdminOrigin.GetSearchInfo():Dictorynary_Key表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(new { data = result.ObjList });
        }
        #endregion
    }
}
