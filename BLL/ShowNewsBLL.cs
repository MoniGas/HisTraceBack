using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class ShowNewsBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long companyId, /*long channelId,*/ string title, int pageIndex)
        {
            long totalCount = 0;
            ShowNewsDAL dal = new ShowNewsDAL();
            List<ShowNews> model = dal.GetList(companyId, /*channelId,*/ title, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }

        public PagedList<View_NewsChannel> GetPagedList(long companyId, long channelId, int? pageIndex)
        {
            ShowNewsDAL dal = new ShowNewsDAL();
            PagedList<View_NewsChannel> dataList = dal.GetList(companyId, /*channelId,*/ pageIndex);
            return dataList;
        }

        public LinqModel.BaseResultModel GetModel(string ewm)
        {
            ShowNewsDAL dal = new ShowNewsDAL();
            ShowNews model = dal.GetModel(ewm);
            return ToJson.NewModelToJson(model, model != null ? "1" : "0", "");
        }
        public BaseResultModel GetModel(long id)
        {
            ShowNewsDAL dal = new ShowNewsDAL();
            ShowNews model = dal.GetModel(id);
            BaseResultModel result = ToJson.NewModelToJson(model, model != null ? "1" : "0", "");
            return result;
        }
        public BaseResultModel Add(ShowNews model, string mainCode)
        {
            ShowNewsDAL dal = new ShowNewsDAL();
            RetResult ret = new RetResult();
            if (string.IsNullOrEmpty(model.Title))
            {
                ret.Msg = "资讯标题不能为空！";
            }
            //else if (model.ChannelID <= 0)
            //{
            //    ret.Msg = "请选择新闻栏目！";
            //}
            else if (string.IsNullOrEmpty(model.Infos))
            {
                ret.Msg = "资讯内容不能为空！";
            }
            else
            {
                ret = dal.Add(model, mainCode);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Edit(ShowNews model)
        {
            ShowNewsDAL dal = new ShowNewsDAL();
            RetResult ret = new RetResult();
            if (string.IsNullOrEmpty(model.Title))
            {
                ret.Msg = "资讯标题不能为空！";
            }
            //else if (model.ChannelID <= 0)
            //{
            //    ret.Msg = "请选择新闻栏目！";
            //}
            else if (string.IsNullOrEmpty(model.Infos))
            {
                ret.Msg = "资讯内容不能为空！";
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
            ShowNewsDAL dal = new ShowNewsDAL();
            RetResult ret = dal.Del(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel UnSetTop(long id)
        {
            ShowNewsDAL dal = new ShowNewsDAL();
            RetResult ret = dal.UnSetTop(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel SetTop(long id)
        {
            ShowNewsDAL dal = new ShowNewsDAL();
            RetResult ret = dal.SetTop(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
    }
}
