/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-22

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 主要用于服务中心控制器层

*********************************************************************************/   
using System;
using System.Web.Mvc;
using BLL;
using LinqModel;
using Common.Argument;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 主要用于服务中心控制器层
    /// </summary>
    public class SysServiceCenterController : Controller
    {
        //
        // GET: /Admin/SysServiceCenter/

        /// <summary>
        /// 获取服务中心列表
        /// </summary>
        /// <param name="sName">名称</param>
        /// <param name="selStatus">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(string sName, string selStatus, int? pageIndex = 1)
        {
            SysServiceCenterBLL bll = new SysServiceCenterBLL();
            LinqModel.BaseResultList dataList = bll.GetEnterpriseList(sName, selStatus, pageIndex);
            return Json(dataList);
        }

        /// <summary>
        /// 添加服务中心
        /// </summary>
        /// <param name="companyName">服务中心名称</param>
        /// <param name="province">省</param>
        /// <param name="city">市</param>
        /// <param name="area">区</param>
        /// <param name="centerAddress">详细地址</param>
        /// <param name="linkMan">联系人</param>
        /// <param name="linkPhone">联系电话</param>
        /// <param name="complaintPhone">投诉电话</param>
        /// <param name="postalCode">邮编</param>
        /// <param name="email">邮箱</param>
        /// <param name="webURL">网址</param>
        /// <returns></returns>
        public JsonResult Add(string companyName, string province, string city, string area, string centerAddress,
            string linkMan, string linkPhone, string complaintPhone, string postalCode, string email, string webURL)
        {
            SysServiceCenterBLL bll = new SysServiceCenterBLL();
            LoginInfo user = SessCokie.Get;
            LinqModel.PRRU_PlatForm objPRRU_PlatForm = new PRRU_PlatForm();
            objPRRU_PlatForm.CompanyName = companyName;
            objPRRU_PlatForm.Dictionary_AddressSheng_ID = Convert.ToInt64(province);
            objPRRU_PlatForm.Dictionary_AddressShi_ID = Convert.ToInt64(city);
            objPRRU_PlatForm.Dictionary_AddressQu_ID = Convert.ToInt64(area);
            objPRRU_PlatForm.PRRU_PlatFormLevel_ID = 4;
            objPRRU_PlatForm.Parent_ID = user.EnterpriseID;
            objPRRU_PlatForm.CenterAddress = centerAddress;
            objPRRU_PlatForm.LinkMan = linkMan;
            objPRRU_PlatForm.LinkPhone = linkPhone;
            objPRRU_PlatForm.ComplaintPhone = complaintPhone;
            objPRRU_PlatForm.PostalCode = postalCode;
            objPRRU_PlatForm.Email = email;
            objPRRU_PlatForm.WebURL = webURL;
            objPRRU_PlatForm.AddDate = DateTime.Now;
            objPRRU_PlatForm.lastdate = DateTime.Now;
            objPRRU_PlatForm.adduser = user.UserID;
            objPRRU_PlatForm.lastuser = user.UserID;
            objPRRU_PlatForm.Status = (int)Common.EnumFile.ViewType.Enable;
            objPRRU_PlatForm.CenterName = "";
            LinqModel.PRRU_PlatForm_User objPRRU_PlatForm_User = new PRRU_PlatForm_User();
            objPRRU_PlatForm_User.UserName = "服务中心";
            objPRRU_PlatForm_User.LoginName = "admin";
            objPRRU_PlatForm_User.LoginPassWord = "admin";
            objPRRU_PlatForm_User.Status = (int)Common.EnumFile.ViewType.Enable;
            objPRRU_PlatForm_User.adddate = DateTime.Now;
            LinqModel.BaseResultModel result = bll.Add(objPRRU_PlatForm, objPRRU_PlatForm_User);
            return Json(result);
        }

        /// <summary>
        /// 修改服务中心信息
        /// </summary>
        /// <param name="eId">ID</param>
        /// <param name="companyName">服务中心名称</param>
        /// <param name="province">省</param>
        /// <param name="city">市</param>
        /// <param name="area">区</param>
        /// <param name="centerAddress">地址</param>
        /// <param name="linkMan">联系人</param>
        /// <param name="linkPhone">联系电话</param>
        /// <param name="complaintPhone">投诉电话</param>
        /// <param name="postalCode">邮编</param>
        /// <param name="email">邮箱</param>
        /// <param name="webURL">网址</param>
        /// <returns></returns>
        public JsonResult Edit(long eId, string companyName, string province, string city, string area, 
            string centerAddress, string linkMan, string linkPhone, string complaintPhone, string postalCode, string email, string webURL)
        {
            SysServiceCenterBLL bll = new SysServiceCenterBLL();
            LoginInfo user = SessCokie.Get;
            LinqModel.PRRU_PlatForm objPRRU_PlatForm = new PRRU_PlatForm();
            objPRRU_PlatForm.PRRU_PlatForm_ID = eId;
            objPRRU_PlatForm.CompanyName = companyName;
            objPRRU_PlatForm.Dictionary_AddressSheng_ID = Convert.ToInt64(province);
            objPRRU_PlatForm.Dictionary_AddressShi_ID = Convert.ToInt64(city);
            objPRRU_PlatForm.Dictionary_AddressQu_ID = Convert.ToInt64(area);
            objPRRU_PlatForm.PRRU_PlatFormLevel_ID = 2;
            objPRRU_PlatForm.Parent_ID = user.EnterpriseID;
            objPRRU_PlatForm.CenterAddress = centerAddress;
            objPRRU_PlatForm.LinkMan = linkMan;
            objPRRU_PlatForm.LinkPhone = linkPhone;
            objPRRU_PlatForm.ComplaintPhone = complaintPhone;
            objPRRU_PlatForm.PostalCode = postalCode;
            objPRRU_PlatForm.Email = email;
            objPRRU_PlatForm.WebURL = webURL;
            objPRRU_PlatForm.AddDate = DateTime.Now;
            objPRRU_PlatForm.lastdate = DateTime.Now;
            objPRRU_PlatForm.adduser = user.UserID;
            objPRRU_PlatForm.lastuser = user.UserID;
            objPRRU_PlatForm.Status = (int)Common.EnumFile.Status.used;
            objPRRU_PlatForm.CenterName = "";
            LinqModel.BaseResultModel result = bll.Edit(objPRRU_PlatForm);
            return Json(result);
        }
    }
}
