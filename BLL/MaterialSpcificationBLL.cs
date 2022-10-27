/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-08-02

** 联系方式 :13313318725

** 描述：产品规格管理业务逻辑

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 产品规格管理业务逻辑
    /// </summary>
    public class MaterialSpcificationBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取产品规格列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId,string spection, int pageIndex)
        {
            long totalCount = 0;
            MaterialSpcificationDAL dal = new MaterialSpcificationDAL();
            List<MaterialSpcification> model = dal.GetList(enterpriseId,spection, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        ///  获取产品规格（下拉列表）
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetListMaS(long enterpriseId)
        {
            MaterialSpcificationDAL dal = new MaterialSpcificationDAL();
            List<MaterialSpcification> model = dal.GetListMaS(enterpriseId);
            if (model != null)
            {
                foreach (var item in model)
                {
                    string ShuLiang = item.Value.ToString();
                    string danWei = item.MaterialSpcificationName ?? "";
                    item.MaterialSpcificationName = ShuLiang + danWei;
                }
            }
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }
        /// <summary>
        /// 添加产品规格
        /// </summary>
        /// <param name="model">产品规格实体</param>
        /// <returns></returns>
        public BaseResultModel Add(MaterialSpcification model)
        {
            MaterialSpcificationDAL dal = new MaterialSpcificationDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.MaterialSpcificationName))
            {
                ret.Msg = "产品规格不能为空！";
            }
            else
            {
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        ///修改产品规格
        /// </summary>
        /// <param name="newModel">产品规格实体</param>
        /// <returns></returns>
        public BaseResultModel Edit(MaterialSpcification newModel)
        {
            MaterialSpcificationDAL dal = new MaterialSpcificationDAL();
            RetResult ret = dal.Edit(newModel);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除产品规格
        /// </summary>
        /// <param name="masId">产品规格ID</param>
        /// <returns></returns>
        public BaseResultModel Del(long masId)
        {
            MaterialSpcificationDAL dal = new MaterialSpcificationDAL();
            RetResult ret = dal.Delete(masId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取产品规格实体
        /// </summary>
        /// <param name="masId">产品规格ID</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long masId)
        {
            MaterialSpcificationDAL dal = new MaterialSpcificationDAL();
            MaterialSpcification model = dal.GetModelByID(masId);
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }
        public MaterialSpcification GetMsModel(long masId)
        {
            MaterialSpcificationDAL dal = new MaterialSpcificationDAL();
            MaterialSpcification model = dal.GetModelByID(masId);
            return model;
        }
    }
}
