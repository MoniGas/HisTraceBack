/********************************************************************************

** 作者： 郭心宇

** 创始时间：2016-1-5

** 联系方式 :13313318725

** 描述：主要用于宣传板块“我的介绍”信息控制器

** 版本：v1.0

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
    public class Admin_Enterprise_ShowController : Controller
    {
        /// <summary>
        /// 获取企业信息模型
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            BaseResultModel Reuslt = new BaseResultModel();
            try
            {
                LoginInfo User = SessCokie.Get;
                Enterprise_ShowBLL ObjEnterprise_ShowBLL = new Enterprise_ShowBLL();
                Reuslt = ObjEnterprise_ShowBLL.GetModelView(User.EnterpriseID);
            }
            catch (Exception Ex)
            {
                string ErrData = "Admin_Enterprise_ShowController.Index";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(Reuslt);
        }

        /// <summary>
        /// 编辑企业信息
        /// </summary>
        /// <param name="Memo">企业介绍</param>
        /// <param name="Files">企业Logo</param>
        /// <param name="LinkMan">联系人</param>
        /// <param name="LinkPhone">联系电话</param>
        /// <param name="Email">邮箱</param>
        /// <param name="WebUrl">企业网址</param>
        /// <param name="Address">地址</param>
        /// <param name="OrderingHotline">订购热线</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(string Memo, string Files, string videoUrl, string LinkMan, string LinkPhone, string Email,
            string WebUrl, string Address, string TaoBaoLink, string JingDongLink, string TianMaoLink, string OrderingHotline,
            string businessLicence, string Filesgg, string ssvideoUrl, string WXLogo, string WXInfo)
        {
            BaseResultModel Result = new BaseResultModel();
            try
            {
                LoginInfo User = SessCokie.Get;
                Enterprise_Info Model = new Enterprise_Info();
                EnterpriseShopLink shopModel = new EnterpriseShopLink();
                Model.Enterprise_Info_ID = User.EnterpriseID;
                Model.Memo = Memo;
                Model.StrLogo = Files;
                Model.VideoUrl = videoUrl;
                Model.LinkMan = LinkMan;
                Model.LinkPhone = LinkPhone;
                Model.Email = Email;
                Model.WebURL = WebUrl;
                Model.Address = Address;
                Model.OrderingHotline = OrderingHotline;
                Model.BusinessLicence = businessLicence;
                Model.StrWXLogo = WXLogo;
                Model.WXInfo = WXInfo;
                shopModel.EnterpriseID = User.EnterpriseID;
                shopModel.TaoBaoLink = TaoBaoLink;
                shopModel.JingDongLink = JingDongLink;
                shopModel.TianMaoLink = TianMaoLink;
                shopModel.AddDate = DateTime.Now;
                shopModel.AddUser = User.UserID;
                shopModel.StrAdUrl = Filesgg;
                shopModel.StrVideoUrl = ssvideoUrl;
                Result = new Enterprise_ShowBLL().Edit(Model, shopModel);
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
