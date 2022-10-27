using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_BrandController : BaseController
    {
        //
        // GET: /Admin_Brand/
        //LoginInfo pf = SessCokie.Get ?? new LoginInfo();
        #region 企业品牌管理
        public JsonResult Index(string brandName, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.GetList(pf.EnterpriseID, brandName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Brand.Index():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public ActionResult IndexJG(int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            string brandName = Request["brandName"];
            BrandBLL brandbll = new BrandBLL();
            string result = brandbll.GetJGList(pf.EnterpriseID, brandName, pageIndex);
            return Json(result);
        }
        //public ActionResult Add()
        //{
        //    return View();
        //}
        [HttpPost]
        public JsonResult Add(string brandName, string descriptions, string logo)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Brand brand = new Brand();
                brand.Enterprise_Info_ID = pf.EnterpriseID;
                brand.BrandName = brandName;
                brand.Descriptions = descriptions;
                brand.Logo = logo;
                brand.adddate = DateTime.Now;
                brand.adduser = pf.UserID;
                brand.lastdate = brand.adddate;
                brand.lastuser = pf.UserID;
                //switch (pf.PRRU_PlatFormLevel_ID)
                //{
                //    case (int)Common.EnumFile.PlatFormLevel.Enterprise:
                //        brand.BrandType = (int)Common.EnumFile.BrandType.CorporateBrand;
                //        break;
                //    case (int)Common.EnumFile.PlatFormLevel.RegulatoryAuthorities:
                //        brand.BrandType = (int)Common.EnumFile.BrandType.RegionalBrand;
                //        break;
                //}
                brand.BrandType = (int)Common.EnumFile.BrandType.CorporateBrand;
                brand.Status = (int)Common.EnumFile.Status.used;
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.Add(brand);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Brand.Add():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        //public JsonResult Edit(long id)
        //{
        //    BrandBLL brandbll = new BrandBLL();
        //    BaseResultModel result = brandbll.GetModel(id);
        //    return Json(result);
        //}
        [HttpPost]
        public ActionResult Edit(string id, string BrandName, string Descriptions, string logo)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Brand brand = new Brand();
                brand.Brand_ID = Convert.ToInt64(id);
                //brand.Brand_ID = id;
                brand.Enterprise_Info_ID = pf.EnterpriseID;
                brand.BrandName = BrandName;
                brand.Descriptions = Descriptions;
                brand.Logo = logo;
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.Edit(brand);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Brand.Edit():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Delete(long brandId)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.Del(pf.EnterpriseID, brandId);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Brand.Delete():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion

        public JsonResult Info(int brandId)
        {
            BaseResultModel result;
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.GetModel(brandId);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Brand.Info():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult SelectBrand(int pageIndex)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.SelectBrandEnterprise(pf.EnterpriseID, "", pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Brand.SelectBrand():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult GetModel(long id)
        {
            BaseResultModel result;
            try
            {
                BrandBLL bll = new BrandBLL();

                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Brand.GetModel():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(result);
        }

        #region 区域品牌管理
        /// <summary>
        /// 申请加入区域品牌审核的列表（监管部门）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult ManagementBrandEnterprise(string mName, int pageIndex)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.GetListByRequestBrand(mName, pf.EnterpriseID, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Brand.ManagementBrandEnterprise():View_RequestBrand视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 申请加入区域品牌列表（企业）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult RequestBrandEnterprise(string mName, int pageIndex)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.GetListByRequestBrand(mName, pf.EnterpriseID, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Brand.RequestBrandEnterprise():View_RequestBrand视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 申请加入区域品牌
        /// </summary>
        /// <returns></returns>
        //public ActionResult RequestBrandEnterpriseAdd()
        //{
        //    return View();
        //}
        [HttpPost]
        public JsonResult RequestBrandEnterpriseAdd(long materialId, long brandId)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Brand_Enterprise be = new Brand_Enterprise();
                be.Enterprise_Info_ID = pf.EnterpriseID;
                be.Material_ID = materialId;
                be.Brand_ID = brandId;
                be.Enterprise_Info_ID = pf.EnterpriseID;
                be.Status = (int)Common.EnumFile.PlatFormState.no_examine;
                be.IsRead = (int)Common.EnumFile.IsRead.noRead;
                be.ISEnterpriseBrand = (int)Common.EnumFile.BrandType.CorporateBrand;
                be.adddate = DateTime.Now;
                be.adduser = 0;
                be.lastdate = be.adddate;
                be.lastuser = 0;
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.AddBrand_Enterprise(be);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Brand.RequestBrandEnterpriseAdd():Brand_Enterprise表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);

        }

        /// <summary>
        /// （区域品牌）审核（监管部门）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">根据从前台传值改状态</param>
        /// <returns></returns>
        public ActionResult Approval(int id, int type)
        {
            BrandBLL brandbll = new BrandBLL();
            string result = brandbll.Approval(id, type);
            return Json(result);
        }
        #endregion

        #region 获取产品列表
        [HttpPost]
        public JsonResult MaterialList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialBLL bll = new MaterialBLL();
                //string result = bll.GetMList(pf.EnterpriseID);
                result = bll.GetMList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Brand.MaterialList():Material表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion

        #region 获取品牌列表
        /// <summary>
        /// 企业申请加入区域品牌时的选择的区域品牌
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BrandList()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                //BaseResultList result = brandbll.SelectBrandEnterprise(pf.UpEnterpriseID, "", 1);
                result = brandbll.SelectBrandEnterprise(pf.Parent_ID, "", 1);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Brand.BrandList():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
    }
}
