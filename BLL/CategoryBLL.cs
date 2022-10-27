/********************************************************************************
** 添加： 李子巍
** 创始时间：2016-08-02
** 联系方式 :13313318725
** 描述：主要用于品类管理的业务逻辑层
****************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;
using MeatTrace.LinqModel;

namespace BLL
{
    /// <summary>
    /// 主要用于品类管理的业务逻辑层
    /// </summary>
    public class CategoryBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取品类码
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>品类码列表</returns>
        public BaseResultList GetList(long enterpriseId, int pageIndex)
        {
            long totalCount = 0;
            List<Category> model = new CategoryDAL().GetList(enterpriseId, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取品类码
        /// </summary>
        /// <returns>品类码列表</returns>
        public List<HisIndustryCategory> GetHisCategoryList(string result)
        {
            return new CategoryDAL().GetHisCategoryList(result);
        }
        /// <summary>
        /// 获取医疗器械品类
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<CategoryList> GetList(string result)
        {
            return new CategoryDAL().GetList(result);
        }
        /// <summary>
        /// 注册品类
        /// </summary>
        /// <param name="mainCode">企业主码</param>
        /// <param name="model">品类实体</param>
        /// <param name="codeUseID">用途标识</param>
        /// <returns>操作结果</returns>
        public BaseResultModel Add(string mainCode, Category model, string codeUseID)
        {
            CategoryDAL categoryDal = new CategoryDAL();
            BaseResultModel result = new BaseResultModel();
            RetResult ret = categoryDal.IsExist(model.CategoryID, model.Enterprise_Info_ID, (long)model.MaterialID);
            if (ret.IsSuccess)
            {
                //20161018已修改
                //ret = new CategoryDAL().RecordCode(mainCode, model.Enterprise_Info_ID, codeUseID, model);
                //if (ret.CmdError != CmdResultError.NONE)
                //{
                //    result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                //}
                //else
                //{
                //    ret = categoryDal.Add(model);
                //    result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                //}
            }
            else
            {
                result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            }
            return result;
        }

        /// <summary>
        /// 获取规格列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId)
        {
            List<MaterialSpcification> model = new CategoryDAL().GetList(enterpriseId) ?? new List<MaterialSpcification>();
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }

        /// <summary>
        /// 获取品类列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetCategoryList(long enterpriseId)
        {
            CategoryDAL dal = new CategoryDAL();
            List<Category> model = dal.GetCategoryList(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, 10000000, 0, "");
            return result;
        }

        /// <summary>
        /// 获取规格型号
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public string GetMaSpecCode(long enterpriseId)
        {
            CategoryDAL dal = new CategoryDAL();
            string specCode = "";
            specCode = dal.GetMaSpcCode(enterpriseId);
            return specCode;
        }

        /// <summary>
        /// 获取简码
        /// </summary>
        /// <param name="cIDcode">全码</param>
        /// <returns></returns>
        public Category GetQcode(string cIDcode)
        {
            CategoryDAL dal = new CategoryDAL();
            Category model = new Category();
            model = dal.GetQCode(cIDcode);
            return model;
        }

        /// <summary>
        /// 获取拍码次数
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public int GetCodeCount(string ewm)
        {
            CategoryDAL dal = new CategoryDAL();
            int count = dal.GetCodeCount(ewm);
            return count;
        }
    }
}
