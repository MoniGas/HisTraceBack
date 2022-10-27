/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-12-08

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于规格管理的业务层

*********************************************************************************/
using System;
using System.Collections.Generic;
using Dal;
using Common.Argument;
using System.Configuration;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 套标规格管理的业务层
    /// </summary>
    public class SpecificationBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        #region 查询规格列表方法
        /// <summary>
        /// 查询规格列表方法
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>反回规格列表</returns>
        public BaseResultList GetList(long enterpriseId, int? Value, int? pageIndex, int pageSize)
        {
            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();

            List<LinqModel.Specification> DataList = ObjSpecificationDAL.GetList(enterpriseId, Value, pageIndex, pageSize);

            return ToJson.NewListToJson(DataList, pageIndex == null ? 1 : pageIndex.Value, PageSize, DataList.Count, "");
        }
        #endregion

        #region 规格添加方法
        /// <summary>
        /// 规格添加方法
        /// </summary>
        /// <param name="objSpecification">规格数据库对象</param>
        /// <returns>添加结果</returns>
        public BaseResultModel Add(LinqModel.Specification objSpecification)
        {
            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();

            RetResult ObjResult = ObjSpecificationDAL.Add(objSpecification);

            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjResult.IsSuccess).ToString(), ObjResult.Msg);
        }
        #endregion

        #region 规格修改方法
        /// <summary>
        /// 规格修改方法
        /// </summary>
        /// <param name="objSpecification">规格数据库对象</param>
        /// <returns>修改操作结果</returns>
        public BaseResultModel Edit(LinqModel.Specification objSpecification)
        {
            RetResult objRetResult = new RetResult();
            if (objSpecification.Value == null)
            {
                objRetResult.SetArgument(CmdResultError.EXCEPTION, "规格不能为空！", "规格不能为空！");

                return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
            }

            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();
            objRetResult = ObjSpecificationDAL.Edit(objSpecification);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        #endregion

        #region 规格删除方法
        /// <summary>
        /// 规格删除方法
        /// </summary>
        /// <param name="id">规格ID</param>
        /// <returns>返回删除结果</returns>
        public BaseResultModel Del(string id)
        {
            if (id == null || id == "")
            {
                return ToJson.NewRetResultToJson("0", "数据错误，请刷新后重试。");
            }

            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();
            RetResult ObjRetResult = ObjSpecificationDAL.Del(Convert.ToInt64(id));
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }
        #endregion

        #region 获取规格信息方法
        /// <summary>
        /// 获取规格信息方法
        /// </summary>
        /// <param name="id">规格ID</param>
        /// <returns>返回规格信息集合</returns>
        public LinqModel.BaseResultModel GetInfo(long id)
        {
            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();
            LinqModel.Specification DataModel = ObjSpecificationDAL.GetInfo(id);
            return ToJson.NewModelToJson(DataModel, DataModel == null ? "0" : "1", "");
        }
        /// <summary>
        /// 获取规格信息方法
        /// </summary>
        /// <param name="id">规格ID</param>
        /// <returns>返回规格信息集合</returns>
        public LinqModel.Specification GetInfoNew(long id)
        {
            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();
            LinqModel.Specification DataModel = ObjSpecificationDAL.GetInfo(id);
            return DataModel;
        }
        #endregion

        /// <summary>
        /// 获取本企业所有规格方法
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <returns>返回所有规格的集合</returns>
        public LinqModel.BaseResultList GetSelectList(long EnterpriseId)
        {
            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();
            List<LinqModel.Specification> DataList = ObjSpecificationDAL.GetSelectList(EnterpriseId);
            if (DataList != null)
            {
                foreach (var item in DataList)
                {
                    string ShuLiang = item.Value.ToString();
                    string danWei = item.GuiGe ?? "";
                    item.GuiGe = ShuLiang + danWei;
                }
            }
            return ToJson.NewListToJson(DataList, 1, DataList.Count, DataList.Count, "");
        }
        /// <summary>
        /// 获取本企业所有产品规格方法
        /// </summary>
        /// <param name="EnterpriseId">企业Id</param>
        /// <returns>返回所有规格的集合</returns>
        public LinqModel.BaseResultList GetMaterialSelectList(long EnterpriseId)
        {
            SpecificationDAL ObjSpecificationDAL = new SpecificationDAL();
            List<LinqModel.MaterialSpcification> DataList = ObjSpecificationDAL.GetMaterialSelectList(EnterpriseId);
            if (DataList != null)
            {
                foreach (var item in DataList)
                {
                    string ShuLiang = item.Value.ToString();
                    string danWei = item.MaterialSpcificationName ?? "";
                    item.Remark = ShuLiang + danWei;
                }
            }
            return ToJson.NewListToJson(DataList, 1, DataList.Count, DataList.Count, "");
        }
    }
}
