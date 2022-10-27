/********************************************************************************

** 作者： 李子巍

** 创始时间：2017-02-27

** 联系方式 :13313318725

** 描述：配方管理控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;
using Common;
using Newtonsoft.Json;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 配方管理控制器
    /// </summary>
    public class Admin_FormulaController : Controller
    {
        /// <summary>
        /// 获取选择原料列表
        /// </summary>
        /// <param name="materialId">产品ID</param>
        /// <returns>原料列表</returns>
        public ActionResult GetSelectList(long materialId)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new FormulaBLL().GetSelectList(materialId);
            }
            catch (Exception ex)
            {
                string errData = "Admin_FormulaController.GetSelectList():Formula表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取配方信息
        /// </summary>
        /// <param name="name">检索名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>列表</returns>
        public ActionResult GetList(string name, int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                result = new FormulaBLL().GetList(pf.EnterpriseID, name, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_FormulaController.GetList():Formula表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取配方中的原料
        /// </summary>
        /// <param name="formulaId">原料信息</param>
        /// <returns>列表</returns>
        public ActionResult GetSubList(long formulaId)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                result = new FormulaBLL().GetSubList(formulaId);
            }
            catch (Exception ex)
            {
                string errData = "Admin_FormulaController.GetSubList():FormulaDetail表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加配方
        /// </summary>
        /// <param name="formulaName">原料名称</param>
        /// <param name="materialId">产品ID</param>
        /// <param name="spec">规格</param>
        /// <param name="strSub">原料</param>
        /// <returns>操作结果</returns>
        public ActionResult Add(string formulaName, long materialId, string spec, string strSub)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Formula model = new Formula();
                model.EnterpriseID = pf.EnterpriseID;
                model.FormulaName = formulaName;
                model.MaterialID = materialId;
                model.Status = (int)EnumFile.Status.used;
                model.Spec = spec;
                model.AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                model.AddUser = pf.UserName;
                List<FormulaDetail> liSub = JsonConvert.DeserializeObject<List<FormulaDetail>>(strSub);
                foreach (var item in liSub)
                {
                    item.EnterpriseID = pf.EnterpriseID;
                    item.Status = (int)Common.EnumFile.Status.used;
                }
                result = new FormulaBLL().Add(model, liSub);
            }
            catch (Exception ex)
            {
                string errData = "Admin_FormulaController.Add():Formula表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改配方
        /// </summary>
        /// <param name="mainId">配方ID</param>
        /// <param name="formulaName">原料名称</param>
        /// <param name="materialId">产品ID</param>
        /// <param name="spec">规格</param>
        /// <param name="strSub">原料</param>
        /// <returns>操作结果</returns>
        public ActionResult Edit(long mainId, string formulaName, long materialId, string spec, string strSub)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Formula model = new Formula();
                model.FormulaID = mainId;
                model.EnterpriseID = pf.EnterpriseID;
                model.FormulaName = formulaName;
                model.MaterialID = materialId;
                model.Status = (int)EnumFile.Status.used;
                model.Spec = spec;
                model.AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                model.AddUser = pf.UserName;
                List<FormulaDetail> liSub = JsonConvert.DeserializeObject<List<FormulaDetail>>(strSub);
                result = new FormulaBLL().Edit(model, liSub);
            }
            catch (Exception ex)
            {
                string errData = "Admin_FormulaController.Edit():Formula表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除配方
        /// </summary>
        /// <param name="formulaId">配方ID</param>
        /// <returns>操作结果</returns>
        public ActionResult Del(long formulaId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new FormulaBLL().Del(formulaId);
            }
            catch (Exception ex)
            {
                string errData = "Admin_FormulaController.Del():Formula表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 从配方获取原料
        /// </summary>
        /// <param name="settingId">配置信息ID</param>
        /// <param name="formulaId">配方ID</param>
        /// <returns>操作结果</returns>
        public ActionResult GetOriginByFormula(long settingId, long formulaId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new FormulaBLL().GetOriginByFormula(settingId, formulaId);
            }
            catch (Exception ex)
            {
                string errData = "Admin_FormulaController.GetOriginByFormula():Formula表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
