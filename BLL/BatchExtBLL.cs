using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class BatchExtBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long batchId, string brandextName)
        {
            BatchExtDAL dal = new BatchExtDAL();
            List<View_BatchExt> model = dal.GetListBE(batchId, brandextName);
            BaseResultList result = ToJson.NewListToJson(model, 1, PageSize, 1, "");
            return result;
        }
        public BaseResultModel Add(BatchExt model)
        {
            BatchExtDAL dal = new BatchExtDAL();
            RetResult ret = dal.Add(model);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Edit(BatchExt newModel)
        {
            BatchExtDAL dal = new BatchExtDAL();
            RetResult ret = dal.Edit(newModel);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Del(long brandextId)
        {
            BatchExtDAL dal = new BatchExtDAL();
            RetResult ret = dal.Del(brandextId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel GetModel(long brandextId)
        {
            BatchExtDAL dal = new BatchExtDAL();
            BatchExt model = dal.GetModel(brandextId);
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        public BaseResultList GetSelectList(long bId)
        {
            BatchExtDAL dal = new BatchExtDAL();
            List<BatchExt> model = dal.GetSelectList(bId);
            BaseResultList result = ToJson.NewListToJson(model, 1, PageSize, 1, "");
            return result;
        }
    }
}
