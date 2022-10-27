/********************************************************************************
** 作者： 张翠霞

** 创始时间：2017-05-25

** 联系方式 :15031109901

** 描述：图片视频上传控制器

** 版本：v2.0

** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using BLL;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 图片视频上传控制器
    /// </summary>
    public class UploadsController : Controller
    {
        //
        // GET: /Uploads/

        /// <summary>
        /// 获取图片列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string uploadsName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new UploadsBLL().GetList(user.EnterpriseID, uploadsName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "UploadsController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult Add(string remark, string imgInfo, string video)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Uploads model = new Uploads();
                model.Remark = remark;
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.StrImgInfo = imgInfo;
                //model.StrVideoInfo = video;
                model.Status = (int)Common.EnumFile.Status.used;
                result = new UploadsBLL().Add(model, video);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取上传信息
        /// </summary>
        /// <param name="id">产品ID</param>
        /// <returns></returns>
        public ActionResult GetModel(int id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new UploadsBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "UploadsController.GetModel";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改上传信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <param name="imgInfo"></param>
        /// <param name="video"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(long id, string remark, string imgInfo, string video)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Uploads model = new Uploads();
                model.UploadsID = id;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Remark = remark;
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                model.StrImgInfo = imgInfo;
                result = new UploadsBLL().Edit(model, video);
            }
            catch (Exception ex)
            {
                string errData = "UploadsController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
