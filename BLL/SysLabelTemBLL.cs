using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using Common.Argument;
using Dal;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class SysLabelTemBLL
    {
        SysLabelTemDAL dal = new SysLabelTemDAL();

        public PagedList<LabelTem> GetLabelTemList(string LabelName, int? pageIndex)
        {
            long totalCount = 0;
            PagedList<LabelTem> dataList = dal.GetLabelTemList(LabelName, pageIndex, out totalCount);
            return dataList;
        }
        public Object GetLabelTem()
        {
            return dal.GetLabelTem();
        }

        public BaseResultModel Add(LabelTem model)
        {
            RetResult ret = new RetResult { CmdError = CmdResultError.EXCEPTION };
            if (string.IsNullOrEmpty(model.LabelName))
            {
                ret.Msg = "标签模板名称不能为空！";
            }
            else
            {
                ret = dal.Add(model);
            }
            return ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }


        public BaseResultModel Del(long LabelTem_ID)
        {
            
            RetResult ret = dal.Delete(LabelTem_ID);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
    }
}
