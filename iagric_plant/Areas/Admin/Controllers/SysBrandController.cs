using System;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysBrandController : Controller
    {
        //
        // GET: /Admin/SysBrand/

        /// <summary>
        /// 区域品牌列表
        /// </summary>
        /// <param name="brandName"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
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
                string errData = "SysBrand.Index():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="brandName"></param>
        /// <param name="descriptions"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
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
                brand.BrandType = (int)Common.EnumFile.BrandType.RegionalBrand;
                brand.Status = (int)Common.EnumFile.Status.used;
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.Add(brand);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "SysBrand.Add():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="BrandName"></param>
        /// <param name="Descriptions"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
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
                string errData = "SysBrand.Edit():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 删除（修改状态）
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
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
                string errData = "SysBrand.Delete():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                BrandBLL bll = new BrandBLL();

                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "SysBrand.GetModel():Brand表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(result);
        }

        /// <summary>
        /// 加入区域品牌审核列表
        /// </summary>
        /// <param name="mName"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult QYBrandAuditEnterprise(string searchName,int brandEnterpriseStatus, int pageIndex)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.GetListJGBMSHBrand(searchName,brandEnterpriseStatus, pf.EnterpriseID, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "SysBrand.ManagementBrandEnterprise():View_RequestBrand视图";
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
        public JsonResult Audit(int id)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                BrandBLL brandbll = new BrandBLL();
                result = brandbll.AuditBrand(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "SysBrand.Audit():Brand_Enterprise表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            //string result = brandbll.AuditBrand(id);
            return Json(result);
        }
    }
}
