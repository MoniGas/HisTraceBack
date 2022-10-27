using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class BatchBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long enterpriseId, string searchName, string materialName, string bName, string beginDate, string endDate, int pageIndex)
        {
            long totalCount = 0;
            BatchDAL dal = new BatchDAL();
            List<View_Batch> model = dal.GetList(enterpriseId, searchName, materialName, bName, beginDate, endDate, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }
        public BaseResultModel Add(Batch batch, Greenhouses_Batch ghbatch)
        {
            BatchDAL dal = new BatchDAL();
            Batch bt = new Batch();
            RetResult ret = dal.Add(batch, ghbatch, out bt);
            BaseResultModel result = ToJson.NewModelToJson(bt, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Edit(Batch batch, Greenhouses_Batch ghbatch)
        {
            BatchDAL dal = new BatchDAL();
            RetResult ret = dal.Edit(batch, ghbatch);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Del(long id)
        {
            BatchDAL dal = new BatchDAL();
            RetResult ret = dal.Del(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public string GetModel(long id)
        {
            BatchDAL dal = new BatchDAL();
            View_Greenhouses_Batch model = dal.GetModel(id);
            string result = ToJson.ModelToJson(model, 1, "");
            return result;
        }
        public string GetModelByView(long id)
        {
            BatchDAL dal = new BatchDAL();
            View_Greenhouses_Batch model = dal.GetModel(id);
            string result = ToJson.ModelToJson(model, 1, "");
            return result;
        }

        #region 获取批次下拉列表
        public BaseResultList GetSelectList(long enterpriseId)
        {
            BatchDAL dal = new BatchDAL();
            List<View_BatchMaterial> model = dal.GetSelectList(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, 1, 1, "");
            return result;
        }
        #endregion
    }
}
