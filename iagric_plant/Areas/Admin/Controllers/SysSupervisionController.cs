/********************************************************************************
** 主要用于代理管理的控制器层
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using Common.Log;
using Webdiyer.WebControls.Mvc;
using System.Text;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysSupervisionController : Controller
    {
        public ActionResult Supervision(int? id)
        {
            string sName = Request["comName"];
            string selStatus = Request["eDate"];
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysSupervisionBLL bll = new SysSupervisionBLL();
            PagedList<View_PRRU_PlatFormUser> list = bll.GetEnterpriseList(sName, selStatus, pageIndex);
            return View(list);
        }

        public ActionResult GetPlatInfo(long eid)
        {
            View_PRRU_PlatFormUser result = new View_PRRU_PlatFormUser();
            try
            {
                SysSupervisionBLL bll = new SysSupervisionBLL();
                result = bll.GetPlatInfo(eid);
            }
            catch (Exception ex)
            {
            }
            return View(result);
        }

        public ActionResult Add()
        {
            return View();
        }
        /// <summary>
        /// 获取区域
        /// </summary>
        /// <returns></returns>
        public ActionResult Address(string shengid,string shiid,string quid)
        {
            List<SelectListItem> itemSheng = new List<SelectListItem>();
            AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
            if (AllAddress == null)
            {
                return View();
            }
            List<AddressInfo> sheng = AllAddress.AddressList.Where(p => p.AddressLevel == 1).OrderBy(p => p.AddressCode).ToList();
            if (sheng != null)
            {
                SelectListItem item = new SelectListItem();
                foreach (AddressInfo sub in sheng)
                {
                    item = new SelectListItem();
                    item.Value = sub.AddressCode;
                    item.Text = sub.AddressName;
                    if (sub.AddressCode == shengid)
                    {
                        item.Selected = true;
                    }
                    itemSheng.Add(item);
                }
            }
            ViewBag.itemSheng = itemSheng;
            if (!string.IsNullOrEmpty(shengid))
            {
                List<AddressInfo> shi = new List<AddressInfo>();
                if (shengid == "110000" || shengid == "310000" || shengid == "500000" || shengid == "120000" || shengid == "810000" || shengid == "820000" || shengid == "710000")//直辖市
                {
                    shi = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(shengid)).ToList();
                }
                else
                {
                    shi = AllAddress.AddressList.Where(p => p.Address_ID_Parent == Convert.ToInt64(shengid)).ToList();
                }
                if (shi != null)
                {
                    SelectListItem item = new SelectListItem();
                    List<SelectListItem> itemShi = new List<SelectListItem>();
                    foreach (AddressInfo sub in shi)
                    {
                        item = new SelectListItem();
                        item.Value = sub.AddressCode;
                        item.Text = sub.AddressName;
                        if (sub.AddressCode == shiid)
                        {
                            item.Selected = true;
                        }
                        itemShi.Add(item);
                    }
                    ViewBag.itemShi = itemShi;
                }
            }
            if (!string.IsNullOrEmpty(shiid))
            {
                List<AddressInfo> shi = new List<AddressInfo>();
                List<AddressInfo> qu = new List<AddressInfo>();
                if (shiid == "810000" || shiid == "820000" || shiid == "710000")//特别行政区
                {
                    qu = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(shiid)).ToList();
                }
                else
                {
                    qu = AllAddress.AddressList.Where(p => p.Address_ID_Parent == Convert.ToInt64(shiid)).ToList();
                }
                if (qu != null)
                {
                    SelectListItem item = new SelectListItem();
                    List<SelectListItem> itemQu = new List<SelectListItem>();
                    foreach (AddressInfo sub in qu)
                    {
                        item = new SelectListItem();
                        item.Value = sub.AddressCode;
                        item.Text = sub.AddressName;
                        if (sub.AddressCode == quid)
                        {
                            item.Selected = true;
                        }
                        itemQu.Add(item);
                    }
                    ViewBag.itemQu = itemQu;
                }
            }
            return View();
        }
        /// <summary>
        /// 根据省编码获取市
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        /// 110000:北京 310000：上海 500000：重庆 120000：天津 810000：香港特别行政区 820000:澳门特别行政区 710000:台湾

        public ActionResult GetShi(string id, string select = "")
        {
            StringBuilder htmlStr = new StringBuilder();
            try
            {
                List<AddressInfo> shi = new List<AddressInfo>();
                htmlStr.Append("<option value=\"-2\">请选择</option>");
                AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
                if (AllAddress == null)
                {
                    return View();
                }
                if (id == "110000" || id == "310000" || id == "500000" || id == "120000" || id == "810000" || id == "820000" || id == "710000")//直辖市
                {
                    shi = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(id)).ToList();
                }
                else
                {
                    shi = AllAddress.AddressList.Where(p => p.Address_ID_Parent == Convert.ToInt64(id)).ToList();
                }
                if (shi != null && shi.Count > 0)
                {
                    foreach (AddressInfo item in shi)
                    {
                        if (item.Address_ID.ToString() == select)
                            htmlStr.Append("<option selected=\"selected\" value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                        else
                            htmlStr.Append("<option value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                    }
                }
            }
            catch
            { }
            return this.Json(htmlStr.ToString(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetQu(string id, string select = "")
        {
            StringBuilder htmlStr = new StringBuilder();
            try
            {
                List<AddressInfo> qu = new List<AddressInfo>();
                htmlStr.Append("<option selected=\"selected\" value=\"-2\">请选择</option>");
                AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
                if (AllAddress == null)
                {
                    return View();
                }
                if (id == "810000" || id == "820000" || id == "710000")//特别行政区
                {
                    qu = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(id)).ToList();
                }
                else
                {
                    qu = AllAddress.AddressList.Where(p => p.Address_ID_Parent == Convert.ToInt64(id)).ToList();
                }
                if (qu != null && qu.Count > 0)
                {
                    foreach (AddressInfo item in qu)
                    {
                        if (item.Address_ID.ToString() == select)
                            htmlStr.Append("<option selected=\"selected\" value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                        else
                            htmlStr.Append("<option value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                    }
                }
            }
            catch
            { }
            return this.Json(htmlStr.ToString(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 监管部门的新增方法
        /// </summary>
        /// <param name="companyName">监管部门名称</param>
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
        /// <returns>新增结果</returns>
        [HttpPost]
        public JsonResult AddDaiLi(string companyName,string province,string city,string area, string centerAddress, string linkMan, string linkPhone,
            string complaintPhone, string postalCode, string email, string webURL) 
        {
            JsonResult js = new JsonResult();
            SysSupervisionBLL objSysSupervisionBLL = new SysSupervisionBLL();
            LoginInfo user = SessCokie.GetMan;
            if (string.IsNullOrEmpty(companyName))
            {
                js.Data = new { res = "-1", info = "代理商名称不能为空" };
                return js;
            }
            LinqModel.PRRU_PlatForm objPRRU_PlatForm = new PRRU_PlatForm();
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
            objPRRU_PlatForm.Status = (int)Common.EnumFile.ViewType.Enable;
            objPRRU_PlatForm.CenterName = "";

            LinqModel.PRRU_PlatForm_User objPRRU_PlatForm_User = new PRRU_PlatForm_User();
            objPRRU_PlatForm_User.UserName = "代理";   
            objPRRU_PlatForm_User.LoginName = "admin";
            objPRRU_PlatForm_User.LoginPassWord = "admin";
            objPRRU_PlatForm_User.Status = (int)Common.EnumFile.ViewType.Enable;
            objPRRU_PlatForm_User.adddate = DateTime.Now;

            LinqModel.BaseResultModel result = objSysSupervisionBLL.Add(objPRRU_PlatForm, objPRRU_PlatForm_User);
            js.Data = new { res = result.code, info = result.Msg};
            return js;
        }
        
        /// <summary>
        /// 监管部门的修改方法
        /// </summary>
        /// <param name="eId"></param>
        /// <param name="companyName"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="centerAddress"></param>
        /// <param name="linkMan"></param>
        /// <param name="linkPhone"></param>
        /// <param name="complaintPhone"></param>
        /// <param name="postalCode"></param>
        /// <param name="email"></param>
        /// <param name="webURL"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditPlat(long eId, string companyName, string province, string city, string area, string centerAddress, string linkMan, string linkPhone, string complaintPhone, string postalCode, string email, string webURL)
        {
            SysSupervisionBLL objSysSupervisionBLL = new SysSupervisionBLL();
            LoginInfo user = SessCokie.GetMan;
            JsonResult js = new JsonResult();
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
            LinqModel.BaseResultModel result = objSysSupervisionBLL.Edit(objPRRU_PlatForm);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }

        /// <summary>
        /// 修改代理信息
        /// </summary>
        /// <param name="eId">代理编号</param>
        /// <returns></returns>
        public ActionResult Edit(long eId)
        {
            PRRU_PlatForm result = new PRRU_PlatForm();
            try
            {
                SysSupervisionBLL bll = new SysSupervisionBLL();
                result = bll.GetModelInfo(eId);
            }
            catch (Exception ex)
            {
            }
            return View(result);
        }
        
        /// <summary>
        /// 监管部门的修改密码方法
        /// </summary>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="newpwd">新密码</param>
        /// <param name="surepwd">确认密码</param>
        /// <returns></returns>
        public JsonResult UpdatePass(string oldpwd, string newpwd, string surepwd)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                SysSupervisionBLL bll = new SysSupervisionBLL();
                result = bll.UpdatePass(user.UserID, oldpwd, newpwd, surepwd);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("0", "提示：异常错误！");
                string errData = "SysSupervisionController.UpdatePass():PRRU_PlatForm_User表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(result);
        }
        
        /// <summary>
        /// 监管部门的禁用方法
        /// </summary>
        /// <param name="userId">监管部门ID</param>
        /// <returns></returns>
        public JsonResult DisableSupervision(long userId)
        {
            SysSupervisionBLL objSysSupervisionBLL = new SysSupervisionBLL();
            RetResult result = objSysSupervisionBLL.DisableSupervision(userId);
            JsonResult js = new JsonResult();
            if (result.IsSuccess)
            {
                js.Data = new { res = true, info = result.Msg, url = "/Admin/SysSupervision/GetPlatInfo" };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }
        
        /// <summary>
        /// 监管部门的启用方法
        /// </summary>
        /// <param name="userId">监管部门ID</param>
        /// <returns></returns>
        public JsonResult EnableSupervision(long userId)  
        {
            SysSupervisionBLL objSysSupervisionBLL = new SysSupervisionBLL();
            RetResult result = objSysSupervisionBLL.EnableSupervision(userId);
            JsonResult js = new JsonResult();
            if (result.IsSuccess)
            {
                js.Data = new { res = true, info = result.Msg, url = "/Admin/SysSupervision/GetPlatInfo" };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }
        
        /// <summary>
        /// 监管部门重置密码方法
        /// </summary>
        /// <param name="userId">监管部门ID</param>
        /// <returns></returns>
        public JsonResult ResetPassword(long userId) 
        {
            SysSupervisionBLL objSysSupervisionBLL = new SysSupervisionBLL();
            RetResult result = objSysSupervisionBLL.ResetPassword(userId);
            PRRU_PlatForm_User puser = objSysSupervisionBLL.GetPUser(userId);
            JsonResult js = new JsonResult();
            if (result.IsSuccess)
            {
                js.Data = new { res = true, info = result.Msg, url = "/Admin/SysSupervision/GetPlatInfo?eid=" + puser.PRRU_PlatForm_ID };
            }
            else
            {
                js.Data = new { res = false, info = result.Msg };
            }
            return js;
        }

        /// <summary>
        /// 查看代理的企业
        /// </summary>
        /// <param name="paltID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetEn(long platID, int? id)
        {
            ViewBag.eid = platID;
            string enName = Request["enName"];
            ViewBag.Name = enName;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            PagedList<LinqModel.Enterprise_Info> dataList = bll.GetEnterpriseInfoListMan(null, platID, null, null, pageIndex);
            return View(dataList);
        }
        /// <summary>
        /// 获取企业续码记录列表
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult GetContinneCodeRecord(long platId = 0, int? id = 1)
        {
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            ViewBag.eid = platId;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            PagedList<View_ContinueCode> dataList = bll.GetContinneCodeRecord(0, platId, sDate, eDate, id);
            return View(dataList);
        }
    }
}
