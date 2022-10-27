/********************************************************************************

** 作者： 李子巍

** 创始时间：2017-02-27

** 联系方式 :13313318725

** 描述：主要用于配方业务逻辑层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/

using System;
using System.Collections.Generic;
using LinqModel;
using System.Configuration;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 主要用于配方业务逻辑层
    /// </summary>
    public class FormulaBLL
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="materialId">产品ID</param>
        /// <returns>列表</returns>
        public BaseResultList GetSelectList(long materialId)
        {
            List<View_Formula> model = new FormulaDAL().GetSelectList(materialId);
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }

        /// <summary>
        /// 获取配方列表
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="name">检索</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>列表</returns>
        public BaseResultList GetList(long enterpriseID, string name, int pageIndex)
        {
            int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            long totalCount = 0;
            List<View_Formula> model = new FormulaDAL().GetList(enterpriseID, name, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取配料列表
        /// </summary>
        /// <param name="formulaID">配方</param>
        /// <returns>；列表</returns>
        public BaseResultList GetSubList(long formulaID)
        {
            List<View_FormulaDetail> model = new FormulaDAL().GetSubList(formulaID);
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }

        /// <summary>
        /// 添加配方
        /// </summary>
        /// <param name="model">配方信息</param>
        /// <param name="liSub">原料列表</param>
        /// <returns>操作结果</returns>
        public BaseResultModel Add(Formula model, List<FormulaDetail> liSub)
        {
            RetResult result = new FormulaDAL().Add(model, liSub);
            return ToJson.NewModelToJson(result, (Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 修改原料
        /// </summary>
        /// <param name="model">配方信息</param>
        /// <param name="liSub">原料列表</param>
        /// <returns>操作结果</returns>
        public BaseResultModel Edit(Formula model, List<FormulaDetail> liSub)
        {
            RetResult result = new FormulaDAL().Edit(model, liSub);
            return ToJson.NewModelToJson(result, (Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 删除配方
        /// </summary>
        /// <param name="formulaId">配方ID</param>
        /// <returns>操作结果</returns>
        public BaseResultModel Del(long formulaId)
        {
            RetResult result = new FormulaDAL().Del(formulaId);
            return ToJson.NewModelToJson(result, (Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 从配方获取原料
        /// </summary>
        /// <param name="settingId">追溯信息ID</param>
        /// <param name="formulaId">配方ID</param>
        /// <returns>操作结果</returns>
        public BaseResultModel GetOriginByFormula(long settingId, long formulaId)
        {
            RetResult result = new FormulaDAL().GetOriginByFormula(settingId, formulaId);
            return ToJson.NewModelToJson(result, (Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }
    }
}
