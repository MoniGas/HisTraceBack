using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class Material_SpecBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long enterpriseId, string materialName, int pageIndex)
        {
            long totalCount = 0;
            Material_SpecDAL dal = new Material_SpecDAL();
            List<View_Material_Spec> model = dal.GetList(enterpriseId, materialName, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }
        public BaseResultModel Add(Material_Spec model, string Propertys, string Condition)
        {
            Material_SpecDAL dal = new Material_SpecDAL();
            RetResult ret = dal.Add(model, Propertys, Condition);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Edit(Material_Spec newModel, string Propertys, string Condition)
        {
            Material_SpecDAL dal = new Material_SpecDAL();
            RetResult ret = dal.Edit(newModel, Propertys, Condition);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Del(long id)
        {
            Material_SpecDAL dal = new Material_SpecDAL();
            RetResult ret = dal.Delete(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel GetModel(long id)
        {
            Material_SpecDAL dal = new Material_SpecDAL();
            Material_Spec model = dal.GetmaSpecByID(id);
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        //public Material_Spec GetModelBySpecId(long id)
        //{
        //    return new Dal.Material_SpecDAL().GetmaSpecByID(id);
        //}
    }
}
