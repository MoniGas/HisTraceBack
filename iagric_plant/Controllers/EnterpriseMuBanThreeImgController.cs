/********************************************************************************
** 作者： 张翠霞
** 创始时间：2018-9-7
** 联系方式 :13313318725
** 描述：企业拍码模板3的图片信息
** 版本：v1.0
** 版权：追溯项目组
*********************************************************************************/
using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using BLL;
using Common.Log;

namespace iagric_plant.Controllers
{
    public class EnterpriseMuBanThreeImgController : Controller
    {
        //
        // GET: /EnterpriseMuBanThreeImg/
        /// <summary>
        /// 获取企业拍码模板3的图片信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            BaseResultModel Reuslt = new BaseResultModel();
            try
            {
                LoginInfo User = SessCokie.Get;
                EnterpriseMuBanThreeImgBLL bll = new EnterpriseMuBanThreeImgBLL();
                Reuslt = bll.GetModel(User.EnterpriseID);
            }
            catch (Exception Ex)
            {
                string ErrData = "EnterpriseMuBanThreeImgController.Index";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(Reuslt);
        }

        /// <summary>
        /// 提交/修改图片
        /// </summary>
        /// <param name="files"></param>
        /// <param name="centerImg"></param>
        /// <param name="fiveImg"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(string files, string centerImg, string fiveImg)
        {
            BaseResultModel Result = new BaseResultModel();
            try
            {
                LoginInfo User = SessCokie.Get;
                EnterpriseMuBanThreeImg Model = new EnterpriseMuBanThreeImg();
                Model.EnterpriseID = User.EnterpriseID;
                Model.StrFirstImg = files;
                Model.StrCenterImg = centerImg;
                Model.StrFiveImg = fiveImg;
                Result = new EnterpriseMuBanThreeImgBLL().Edit(Model);
            }
            catch (Exception Ex)
            {
                string ErrData = "Admin_ShowCompanyController.Edit";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(Result);
        }
    }
}
