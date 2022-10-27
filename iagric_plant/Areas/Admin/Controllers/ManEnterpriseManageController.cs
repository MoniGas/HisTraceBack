using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using System.Text;
using System.Configuration;
using Common.Tools;
using Common;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class ManEnterpriseManageController : Controller
    {
        /// <summary>
        /// 获取我的企业列表
        /// </summary>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            string enName = Request["enName"];
            ViewBag.Name = enName;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.GetMan;
            PagedList<Enterprise_Info> dataList = bll.GetEnterpriseInfoListMan(enName, pf.EnterpriseID, sDate, eDate, pageIndex);
            var allList = bll.GetAllEnterpriseInfoListMan(enName, pf.EnterpriseID);
            ViewBag.alllist = allList;
            return View(dataList);
        }

        /// <summary>
        /// 获取新入驻的企业列表
        /// </summary>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult GetNewAddEnList(int? id)
        {
            string enName = Request["enName"];
            ViewBag.Name = enName;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.GetMan;
            PagedList<Enterprise_Info> dataList = bll.GetEnterpriseInfoListNewAdd(enName, sDate, eDate, pageIndex);
            if (dataList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                AreaInfo AllAddress = BaseData.listAddress;
                if (AllAddress != null)
                {
                    List<AddressInfo> citysheng = AllAddress.AddressList.Where(p => p.AddressLevel == 1).OrderBy(p => p.AddressCode).ToList();
                    List<AddressInfo> cityshi = AllAddress.AddressList.Where(p => p.AddressLevel == 2).OrderBy(p => p.AddressCode).ToList();
                    List<AddressInfo> cityqu = AllAddress.AddressList.Where(p => p.AddressLevel == 3).OrderBy(p => p.AddressCode).ToList();
                    //List<AddressInfo> cityAddress = Common.Argument.BaseData.listAddressNew;
                    #region 显示所需属性
                    long ShengID = 0;
                    long ShiID = 0;
                    long QuID = 0;
                    #endregion
                    foreach (var item in dataList)
                    {
                        string cityid = item.Dictionary_AddressSheng_ID + "_" + item.Dictionary_AddressShi_ID + "_" + item.Dictionary_AddressQu_ID;
                        if (cityid != null && cityid != "")
                        {
                            string[] para = cityid.Split('_');
                            if (para.Length == 3)
                            {
                                ShengID = long.Parse(para[0]);
                                ShiID = long.Parse(para[1]);
                                QuID = long.Parse(para[2]);
                                var sheng = citysheng.FirstOrDefault(m => m.Address_ID == ShengID);
                                sb.Append(sheng == null ? "" : sheng.AddressName);
                                sb.Append("-");
                                if (ShengID == 110000 || ShengID == 310000 || ShengID == 500000 || ShengID == 120000 || ShengID == 810000 || ShengID == 820000 || ShengID == 710000)//直辖市
                                {
                                    sb.Append(sheng == null ? "" : sheng.AddressName);
                                }
                                else
                                {
                                    var shi = cityshi.FirstOrDefault(m => m.Address_ID == ShiID);
                                    sb.Append(shi == null ? "" : shi.AddressName);
                                }
                                sb.Append("-");
                                var qu = cityqu.FirstOrDefault(m => m.Address_ID == QuID);
                                sb.Append(qu == null ? "" : qu.AddressName);
                                //item.Address = sb.ToString();
                            }
                        }
                        item.Address = sb.ToString();
                        sb.Clear();
                    }
                }
            }
            return View(dataList);
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public ActionResult GetEnInfo(long eid)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            View_EnterpriseInfoUser info = bll.GetEnInfo(eid);
            Contract contract = bll.GetContractInfo(eid);
            if (contract != null)
            {
                ViewBag.HTDate = contract.EndDate.Value.ToString("yyyy-MM-dd");
            }
            EnterpriseShopLink enKz = bll.GetEnKhd(eid);
            if (enKz != null)
            {
                ViewBag.KHDType = enKz.IsSimple;
            }
            return View(info);
        }

        /// <summary>
        /// 给企业管理账户重置密码
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public ActionResult ResetPassword(long eid)
        {
            // Enterprise_User model = new Enterprise_User();
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.ResetPassword(eid);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 获取企业码情况
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public ActionResult SetAmountCode(long eid)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            Enterprise_Info model = bll.GetEnInfoCodeCount(eid);
            return View(model);
        }

        [HttpPost]
        /// <summary>
        /// 企业用码量设置
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="sqcount">企业最多申请数量</param>
        /// <param name="tzcount">企业可透支的数量</param>
        /// <returns></returns>
        public ActionResult SetAmountCode(long eid, long sqcount, long tzcount)
        {
            sqcount = Convert.ToInt64(Request["sqcount"]);
            tzcount = Convert.ToInt64(Request["tzcount"]);
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.SetAmountCode(eid, sqcount, tzcount);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
            //return Json(new { ok = result.IsSuccess, msg = result.Msg ,url=});
        }

        /// <summary>
        /// 给企业续码
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public ActionResult ContinneCode(long eid)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            Enterprise_Info model = bll.GetEnInfoCodeCount(eid);
            return View(model);
        }

        /// <summary>
        /// 给企业续码 提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContinneCode(long eid, long xmcount, decimal price)
        {
            LoginInfo pf = SessCokie.GetMan;
            ContinneCodeRecord model = new ContinneCodeRecord();
            xmcount = Convert.ToInt64(Request["xmcount"]);
            price = Convert.ToDecimal(Request["price"]);
            model.EnterpriseID = eid;
            model.AddDate = DateTime.Now;
            model.AddUser = pf.UserName;
            model.AddUserID = pf.UserID;
            model.CodeCount = xmcount;
            model.Price = price;
            model.PRRU_PlatForm_ID = pf.EnterpriseID;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.ContinneCode(model);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 获取企业续码记录列表
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult GetContinneCodeRecord(long eid = 0, int? id = 1)
        {
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            ViewBag.eid = eid;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            View_EnterpriseInfoUser info = bll.GetEnInfo(eid);
            if (info != null)
            {
                ViewBag.EnName = info.EnterpriseName;
            }
            //int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            PagedList<View_ContinueCode> dataList = bll.GetContinneCodeRecord(eid, 0, sDate, eDate, id);
            return View(dataList);
        }

        /// <summary>
        /// 获取企业用码情况列表
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult GetUsedCodeSituation(long eid = 0, int? id = 1)
        {
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            ViewBag.eid = eid;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            View_EnterpriseInfoUser info = bll.GetEnInfo(eid);
            if (info != null)
            {
                ViewBag.EnName = info.EnterpriseName;
            }
            PagedList<View_UsedCodeSituation> dataList = bll.GetUsedCodeSituation(eid, sDate, eDate, id);
            return View(dataList);
        }

        /// <summary>
        /// 关联新入驻企业
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public ActionResult GuanLian(long eid)
        {
            LoginInfo pf = SessCokie.GetMan;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.GuanLian(eid, pf.EnterpriseID);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetNewAddEnList" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 停用企业
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public ActionResult TingYong(long eid)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.TingYong(eid);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="eid">分页</param>
        /// <returns></returns>
        public ActionResult Audit(long eid)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            Enterprise_Info model = bll.GetEnInfoCodeCount(eid);
            return View(model);
        }

        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="sqcount">设置用码量</param>
        /// <param name="tzcount">设置透支数量</param>
        /// <param name="type">类型（1：只是审核，2：审核并保存）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Audit(long eid, long sqcount, long tzcount, int type)
        {
            sqcount = Convert.ToInt64(Request["sqcount"]);
            tzcount = Convert.ToInt64(Request["tzcount"]);
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.SetAudit(eid, sqcount, tzcount, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 合同列表操作页
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="id">页码</param>
        /// <returns></returns>
        public ActionResult Contract(long eid, int? id)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            Enterprise_Info model = bll.GetEnInfoCodeCount(eid);
            ViewBag.Name = model.EnterpriseName;
            ViewBag.eid = eid;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            LoginInfo pf = SessCokie.GetMan;
            PagedList<Contract> dataList = bll.GetContractList(eid, pageIndex);
            return View(dataList);
        }

        [HttpPost]
        public ActionResult Contract(long eid, DateTime signingDate, string signingUserName, DateTime beginDate, DateTime endDate)
        {
            LoginInfo pf = SessCokie.GetMan;
            Contract model = new Contract();
            signingDate = Convert.ToDateTime(Request["signingDate"]);
            signingUserName = Request["signingUserName"];
            beginDate = Convert.ToDateTime(Request["beginDate"]);
            endDate = Convert.ToDateTime(Request["endDate"]);
            model.EnterpriseID = eid;
            model.AddDate = DateTime.Now;
            model.AddUserID = pf.UserID;
            model.BeginDate = beginDate;
            model.EndDate = endDate;
            model.SigningDate = signingDate;
            model.SigningUserName = signingUserName;
            model.PRRU_PlatForm_ID = pf.EnterpriseID;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.Contract(model);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/Contract?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 获取企业商城管理（是否开通商城）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetEnterpriseShopList(int? id)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.GetMan;
            string enName = Request["enName"];
            ViewBag.Name = enName;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            PagedList<View_Order_EnterpriseAccount> dataList = bll.GetEnterpriseShopList(enName, pf.EnterpriseID, pageIndex);
            return View(dataList);
        }

        /// <summary>
        /// 确认开通/暂停使用
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="type">类型（0：暂停使用，1：确认开通）</param>
        /// <returns></returns>
        public ActionResult EditVerifyShop(long eid, int type)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.EditVerifyShop(eid, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnterpriseShopList" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
        /// <summary>
        /// 企业授权码管理
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthorizationCode(long eid, string ename, string mainCode)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            Enterprise_License model = bll.GetEnInfoLicense(eid);
            ViewBag.eName = ename;
            if (model == null)
            {
                model = new Enterprise_License();
                model.EnterpriseID = eid;
            }
            return View(model);
        }
        [HttpPost]
        /// <summary>
        /// 企业授权码管理
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public ActionResult SetAuthorizationCode(long eid, string setDate, string fileurl, string LicenseType)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.GetMan;
            string dateNow = Convert.ToString(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")), 16);
            string setDatestr = Convert.ToString(Convert.ToInt32(Convert.ToDateTime(setDate).ToString("yyyyMMdd")), 16);
            RetResult ret = bll.SetAuthorizationCode(eid, pf.EnterpriseID, setDate, LicenseType, fileurl);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="FirstDisplay"></param>
        /// <returns></returns>
        public ActionResult GetEnterpriseList(long ParentID, string FirstDisplay = "")
        {
            LoginInfo pf = SessCokie.GetMan;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            List<Enterprise_Info> MaList = bll.GetEnterpriseList(pf.EnterpriseID);
            List<SelectListItem> Result = new List<SelectListItem>();
            foreach (var item in MaList)
            {
                SelectListItem ListItem = new SelectListItem();
                ListItem.Value = item.Enterprise_Info_ID.ToString();
                ListItem.Text = item.EnterpriseName;
                if (item.Enterprise_Info_ID == ParentID)
                    ListItem.Selected = true;
                else
                    ListItem.Selected = false;
                Result.Add(ListItem);
            }
            return View(Result);
        }

        //设置使用客户端是简版/完整版(type=2完整版，1简版)
        public ActionResult SetKHDType(long eid, int type)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.SetKHDType(eid, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        /// <summary>
        /// 给企业续码
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public ActionResult SetJKToken(long eid)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            Enterprise_Info en = bll.GetEnInfoCodeCount(eid);
            ViewBag.EnID = en.Enterprise_Info_ID;
            ViewBag.EnName = en.EnterpriseName;
            EnterpriseShopLink model = new ScanCodeBLL().ShopEn(eid);
            return View(model);
        }

        /// <summary>
        /// 给企业续码 提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetJKToken(long eid, string token, string tokencode)
        {
            LoginInfo pf = SessCokie.GetMan;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.SetJKToken(eid, token, tokencode);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }

        #region GS1管理
        /// <summary>
        /// 获取GS1企业列表 21-10-19
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult GSIndex(int? id)
        {
            string enName = Request["enName"];
            ViewBag.Name = enName;
            string sDate = Request["sDate"];
            string eDate = Request["eDate"];
            ViewBag.sDate = sDate;
            ViewBag.eDate = eDate;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.GetMan;
            PagedList<View_EnterpriseInfoUser> dataList = bll.GetEnterpriseInfoListGS1(enName, pf.EnterpriseID, sDate, eDate, pageIndex);
            return View(dataList);
        }
        /// <summary>
        /// 添加视图
        /// </summary>
        /// <returns></returns>
        public ActionResult AddGS1(long Id=0)
        {
            Enterprise_Info result = new Enterprise_Info();
            try
            {
                SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
                result = bll.GetModelInfo(Id);
            }
            catch (Exception ex)
            {
            }
            return View(result);
        }
        [HttpPost]
        public JsonResult AddGS(string companyName, string province, string city, string area, string centerAddress, string linkMan, string linkPhone,
            string youxiaoDate, string tyCode, string eId)
        {
            JsonResult js = new JsonResult();
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo userLogin = SessCokie.GetMan;
            if (string.IsNullOrEmpty(companyName))
            {
                js.Data = new { res = "-1", info = "企业名称不能为空" };
                return js;
            }
            LinqModel.Enterprise_Info model = new Enterprise_Info();
            model.Enterprise_Info_ID = Convert.ToInt64(eId);
            model.EnterpriseName = companyName;
            model.Dictionary_AddressSheng_ID =Convert.ToInt64(province);
            model.Dictionary_AddressShi_ID = Convert.ToInt64(city);
            model.Dictionary_AddressQu_ID = Convert.ToInt64(area);
            model.Address = centerAddress;
            model.LinkMan = linkMan;
            model.LinkPhone = linkPhone;
            model.BusinessLicence = tyCode;
            if (eId == "0")
            {
                model.Enterprise_Level = Convert.ToInt16(EnumFile.PlatFormLevel.Enterprise);
                if (model.PRRU_PlatForm_ID <= 0)
                {
                    model.PRRU_PlatForm_ID = 0;
                }
                model.adddate = DateTime.Now;
                model.AddTime = DateTime.Now;
                model.ApprovalCodeType = 0;
                model.Status = (int)EnumFile.Status.used;
                model.Verify = (int)EnumFile.EnterpriseVerify.gs1;
                model.ShopVerify = (int)EnumFile.ShopVerify.Close;
                model.WareHouseVerify = 0;
                model.IsOpenShop = false;

                Enterprise_User user = new Enterprise_User();
                user.Enterprise_Info_ID = model.Enterprise_Info_ID;
                //企业管理员登录角色为平台角色
                user.Enterprise_Role_ID = 1;
                user.UserName = "管理员";
                user.UserCode = "admin";
                user.UserType = "GS1";
                user.Status = Convert.ToInt16(EnumFile.Status.used);
                user.adddate = DateTime.Now;
                LinqModel.BaseResultModel result = bll.AddGS(model, user, youxiaoDate);
                js.Data = new { res = result.code, info = result.Msg };
            }
            else
            {
                LinqModel.BaseResultModel result = bll.EditGS(model, youxiaoDate);
                js.Data = new { res = result.code, info = result.Msg };
            }
            return js;
        }

        /// <summary>
        /// 修改企业状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">1：启用；2：禁用</param>
        /// <returns></returns>
        public ActionResult EditStatus(long id, int type)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.EditStatus(id, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GSIndex" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
        
        #endregion

        #region 子企业授权
        /// <summary>
        /// 开通子企业的企业列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubIndex(int? id)
        {
            string name = Request["enName"];
            ViewBag.Name = name;
            int pageIndex = id == null ? 1 : Convert.ToInt32(id.ToString());
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo pf = SessCokie.GetMan;
            PagedList<View_Enterprise_SetMoule> dataList = bll.GetSubEnterpriseList(name, pageIndex);
            return View(dataList);
        }

        /// <summary>
        /// 添加子用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSub()
        {
            return View();
        }
        /// <summary>
        /// 为企业开通子用户功能
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddSubInfo(long eid)
        {
            JsonResult js = new JsonResult();
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            LoginInfo user = SessCokie.GetMan;
            Enterprise_SetMoule model = new Enterprise_SetMoule();
            model.Entetprise_Info_ID = eid;
            model.SetSub = (int)Common.EnumFile.ShopVerify.Open;
            LinqModel.BaseResultModel result = bll.Add(model);
            js.Data = new { res = result.code, info = result.Msg };
            return js;
        }
        /// <summary>
        /// 修改企业开通子企业状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult EditSubStatus(long id, int type)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.EditSubStatus(id, type);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/SubIndex" };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
        #endregion

        #region
        public ActionResult SetCodeType(long eid)
        {
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            Enterprise_Info model = bll.GetEnInfoCodeCount(eid);
            return View(model);
        }

        //提交
        [HttpPost]
        public ActionResult SetCodeType(long eid, int CodeType)
        {
            LoginInfo pf = SessCokie.GetMan;
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            RetResult ret = bll.SetCodeType(eid, CodeType);
            JsonResult js = new JsonResult();
            if (ret.IsSuccess)
            {
                js.Data = new { res = true, info = ret.Msg, url = "/Admin/ManEnterpriseManage/GetEnInfo?eid=" + eid };
            }
            else
            {
                js.Data = new { res = false, info = ret.Msg };
            }
            return js;
        }
        #endregion
    }
}
