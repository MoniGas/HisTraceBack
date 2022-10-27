/********************************************************************************
** 作者： 李子巍
** 创始时间：2016-08-02
** 联系方式 :13313318725
** 描述：主要用于注册品类的控制器
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common;
using Common.Argument;
using Common.Log;
using LinqModel;
using MeatTrace.LinqModel;
using BLL;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 主要用于注册品类的控制器
    /// </summary>
    public class Admin_CategoryController : BaseController
    {
        /// <summary>
        /// 获取选择品类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectCategoryNew(string cId)
        {
            //string parent = ConfigurationManager.AppSettings["OilTypeID"].ToString();
            //6782 肉干
            //6803 乳制品
            //6702 肉类
            //6801 肉制品
            //7311中成药 有二级
            //7321 植物原药材
            //7333 西药
            //List<CategoryList> liCategory = new List<CategoryList>();
            //if (string.IsNullOrEmpty(cId))
            //{
            //    liCategory = Common.Tools.Public.listCategory.Where(m => m.CodeUseID == 10 && m.CategoryLevel == 1).ToList();
            //}
            //else
            //{
            //    liCategory = Common.Tools.Public.listCategory.Where(m => m.PrentID == cId).ToList();
            //}
            List<HisIndustryCategory> liCategory = new List<HisIndustryCategory>();
            if (string.IsNullOrEmpty(cId))
            {
                liCategory = Common.Tools.Public.listCategory.Where(m => m.codeuse_id == 32 && m.industrycategory_level == 1).ToList();
            }
            else
            {
                liCategory = Common.Tools.Public.listCategory.Where(m =>m.industrycategory_id_parent.ToString() == cId).ToList();
            }
            return Json(
                ToJson.NewListToJson(liCategory, 1, liCategory.Count, liCategory.Count, "")
            );
        }

        /// <summary>
        /// 获取选择品类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectCategory()
        {
            //string parent = ConfigurationManager.AppSettings["OilTypeID"].ToString();
            //6782 肉干
            //6803 乳制品
            //6702 肉类
            //6801 肉制品
            //7311中成药 有二级
            //7321 植物原药材
            //7333 西药
            //List<CategoryList> liCategory = Common.Tools.Public.listCategory.Where(m => m.CategoryID == "6702" || m.CategoryID == "6801"
            //    || m.CategoryID == "6803" || m.CategoryID == "6754" || m.CategoryID == "7311" || m.CategoryID == "7321" || m.CategoryID == "7333").ToList();
            //return Json(
            //    ToJson.NewListToJson(liCategory, 1, liCategory.Count, liCategory.Count, "")
            //);
            return null;
        }

        /// <summary>
        /// 获取品类列表
        /// </summary>
        /// <param name="cId">上级品类ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectCategoryEr(string cId)
        {
            //List<CategoryList> liCategory = Common.Tools.Public.listCategory.Where(m => m.PrentID == cId).ToList();
            //List<CategoryList> liCategory = Common.Tools.Public.listCategory.Where(m => m.PrentID == "6702").ToList();
            List<HisIndustryCategory> liCategory = Common.Tools.Public.listCategory.Where(m => m.industrycategory_id_parent.ToString() == cId).ToList();
            return Json(
                ToJson.NewListToJson(liCategory, 1, liCategory.Count, liCategory.Count, "")
            );
        }

        /// <summary>
        /// 获取选择规格列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectSpcification()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList liSpcification = new BLL.CategoryBLL().GetList(pf.EnterpriseID);
            return Json(liSpcification);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns>结果</returns>
        public ActionResult Index(int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                result = new BLL.CategoryBLL().GetList(pf.EnterpriseID, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Category.Index():Category表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 备案品类码
        /// </summary>
        /// <param name="CategoryID">品类码标识</param>
        /// <param name="MaterialSpcificationId">型号名称</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult Add(string CategoryID, string materialID)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Category model = new Category();
                model.AddTime = DateTime.Now;
                model.AddUser = pf.UserID;
                model.CategoryID = long.Parse(CategoryID);
                model.Enterprise_Info_ID = pf.EnterpriseID;
                model.Status = (int)EnumFile.Status.used;
                model.MaterialID = Convert.ToInt64(materialID);
                CategoryBLL bll = new CategoryBLL();
                result = bll.Add(pf.MainCode, model, "10");
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Category.Add():Category表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取规格型号
        /// </summary>
        /// <returns></returns>
        public string SelectMaSpcCode()
        {
            LoginInfo pf = SessCokie.Get;
            string specCode = new BLL.CategoryBLL().GetMaSpecCode(pf.EnterpriseID);
            return specCode;
        }

        #region 获取产品列表
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MaterialList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialBLL bll = new MaterialBLL();
                result = bll.GetCMaterialList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch.MaterialList():Material表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
    }
}
