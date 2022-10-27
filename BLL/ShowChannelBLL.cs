using System;
using System.Collections.Generic;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class ShowChannelBLL
    {
        public BaseResultList GetList(long companyId, string name = "")
        {
            ShowChannelDAL dal = new ShowChannelDAL();
            List<ShowChannel> model = dal.GetList(companyId, name);
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count + 1, model.Count, "");
            return result;
        }
        public BaseResultModel GetModel(long id)
        {
            ShowChannelDAL dal = new ShowChannelDAL();
            ShowChannel model = dal.GetModel(id);
            BaseResultModel result = ToJson.NewModelToJson(model, "1", "");
            return result;
        }
        public BaseResultModel Add(ShowChannel model)
        {
            ShowChannelDAL dal = new ShowChannelDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.ChannelName))
            {
                ret.Msg = "栏目名称不能为空！";
            }
            else if (model.ChannelName.Length > 4)
            {
                ret.Msg = "栏目名称太长！";
            }
            else
            {
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Edit(ShowChannel model)
        {
            ShowChannelDAL dal = new ShowChannelDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.ChannelName))
            {
                ret.Msg = "栏目名称不能为空！";
            }
            else if (model.ChannelName.Length > 4)
            {
                ret.Msg = "栏目名称太长！";
            }
            else
            {
                ret = dal.Edit(model);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Del(long id)
        {
            ShowChannelDAL dal = new ShowChannelDAL();
            RetResult ret = dal.Del(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public string UpdateSort(string ids)
        {
            ShowChannelDAL dal = new ShowChannelDAL();
            RetResult ret = dal.UpdateSort(ids);
            string result = ToJson.ResultToJson(ret);
            return result;
        }
    }
}
