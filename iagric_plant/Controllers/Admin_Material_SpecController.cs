using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;
using System.Collections.Generic;

namespace iagric_plant.Controllers
{
    public class Admin_Material_SpecController : Controller
    {
        //
        // GET: /Admin_Material_Spec/

        public ActionResult Index(string materialName, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                Material_SpecBLL bll = new Material_SpecBLL();
                result = bll.GetList(pf.EnterpriseID, materialName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material_SpecController.Index():View_Material_Spec视图";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult Add(string materialID, string materialSpecification, string price, string Propertys, string Condition, string ExpressPrice)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Material_Spec maSpec = new Material_Spec();
                maSpec.Material_ID = Convert.ToInt64(materialID ?? "0");
                maSpec.MaterialSpecification = materialSpecification;
                maSpec.Price = Convert.ToDecimal(price ?? "0");
                maSpec.adddate = DateTime.Now;
                maSpec.adduser = pf.UserID;
                maSpec.lastdate = maSpec.adddate;
                maSpec.lastuser = pf.UserID;
                maSpec.Status = (int)Common.EnumFile.Status.used;
                maSpec.EnterpriseId = pf.EnterpriseID;
                maSpec.ExpressPrice = Convert.ToDecimal(string.IsNullOrEmpty(ExpressPrice) ? "0" : ExpressPrice);
                Material_SpecBLL bll = new Material_SpecBLL();
                result = bll.Add(maSpec, Propertys, Condition);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_SpecController.Add():Material_Spec表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult Edit(string id, string materialID, string materialSpecification, string price, string Propertys, string Condition, string ExpressPrice)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Material_Spec maSpec = new Material_Spec();
                maSpec.ID = Convert.ToInt64(id);
                maSpec.Price = Convert.ToDecimal(price ?? "0");
                maSpec.Material_ID = Convert.ToInt64(materialID ?? "0");
                maSpec.MaterialSpecification = materialSpecification;
                maSpec.ExpressPrice = Convert.ToDecimal(string.IsNullOrEmpty(ExpressPrice) ? "0" : ExpressPrice);
                maSpec.lastdate = DateTime.Now;
                maSpec.lastuser = pf.UserID;
                Material_SpecBLL bll = new Material_SpecBLL();
                result = bll.Edit(maSpec, Propertys, Condition);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_SpecController.Edit():Material_Spec表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Delete(long id)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Material_SpecBLL bll = new Material_SpecBLL();
                result = bll.Del(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_SpecController.Delete():Material_Spec表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult GetModel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                Material_SpecBLL bll = new Material_SpecBLL();

                result = bll.GetModel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_SpecController.GetModel():Material_Spec表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(result);
        }

        /// <summary>
        /// 获取商品属性Action
        /// </summary>
        /// <returns>Json格式数据列表</returns>
        public JsonResult GetMaterialPropertyList()
        {
            BaseResultList MaterialPropertyResult = new BaseResultList();
            try
            {
                MaterialPropertyResult = new BLL.MaterialPropertyBLL().GetMaterialPropertyList();
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material_Spec.GetMaterialPropertyList():Material_Property表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(MaterialPropertyResult);
        }

        #region 获取商品活动
        /// <summary>
        /// 获取商品活动
        /// </summary>
        /// <param name="MaterialSpecId">规格标识</param>
        /// <returns>商品活动Json数据</returns>
        public ActionResult GetMaterialProperty(long MaterialSpecId)
        {
            //查询列表
            List<View_Material_Property> result = new BLL.MaterialPropertyBLL().GetMaterialPropertyList(MaterialSpecId);
            return Json(result);
        }
        #endregion

        /// <summary>
        /// 获取产品规格列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSelectList()
        {
            SpecificationBLL ObjSpecificationBLL = new SpecificationBLL();
            LoginInfo pf = Common.Argument.SessCokie.Get;
            LinqModel.BaseResultList DataList = ObjSpecificationBLL.GetMaterialSelectList(pf.EnterpriseID);
            return Json(DataList);
        }
    }
}
