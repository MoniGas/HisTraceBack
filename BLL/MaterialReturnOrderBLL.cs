using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;
using System.Data;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class MaterialReturnOrderBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public RetResult AddMaterialReturnOrder(string orderNum, string Content)
        {
            return new MaterialReturnOrderDAL().AddMaterialReturnOrder(orderNum, Content);
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_Material_ReturnOrder> GetList(string name, long enterpriseId, string beginDate, string endDate, int status, int pageIndex,long agentCode)
        {
            return new MaterialReturnOrderDAL().GetList(name, enterpriseId, beginDate, endDate, status, pageIndex,agentCode);
        }

        public BaseResultList GetReturnOrderList(string Name, long EnterpriseId, string BeginDate, string EndDate, int Status, int PageIndex)
        {
            long TotalCount = 0;
            MaterialReturnOrderDAL dal = new MaterialReturnOrderDAL();
            List<View_Material_ReturnOrder> model = dal.GetReturnOrderList(Name, EnterpriseId, BeginDate, EndDate, Status, PageIndex, out TotalCount);
            BaseResultList result = ToJson.NewListToJson(model, PageIndex, PageSize, TotalCount, "");
            return result;
        }
        public View_Material_ReturnOrder GetReturnOrder(string OrderNum)
        {
            return new Dal.MaterialReturnOrderDAL().GetReturnOrder(OrderNum);
        }

        public BaseResultModel EditStatus(long MaterialReturnOrderID, long UserId, int status)
        {
            MaterialReturnOrderDAL dal = new MaterialReturnOrderDAL();
            RetResult ret = dal.EditStatus(MaterialReturnOrderID, UserId, status);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public RetResult EditStatus(List<string> payOrderNums, int status)
        {
            MaterialReturnOrderDAL dal = new MaterialReturnOrderDAL();
            RetResult ret = dal.EditStatus(payOrderNums, status);
            return ret;
        }
        public RetResult EditStatus(string MaterialReturnOrderID)
        {
            MaterialReturnOrderDAL dal = new MaterialReturnOrderDAL();
            return dal.EditStatus(MaterialReturnOrderID);
        }

        public RetResult EditMaterialReturnOrder(string OrderNum, string ExpressComp, string ExpressNum)
        {
            MaterialReturnOrderDAL dal = new MaterialReturnOrderDAL();
            return dal.EditMaterialReturnOrder(OrderNum, ExpressComp, ExpressNum);
        }

        public DataTable ExportExcel(string name, long eID, string sDate, string eDate, int status, int delStatus)
        {
            DataTable dt = new DataTable();
            try
            {
                MaterialReturnOrderDAL dal = new MaterialReturnOrderDAL();
                List<View_Material_ReturnOrder> model = dal.ExportExcel(name, eID, sDate, eDate, status, delStatus);

                dt.Columns.Add("产品");
                dt.Columns.Add("规格");
                dt.Columns.Add("联系人");
                dt.Columns.Add("联系电话");
                dt.Columns.Add("地址");
                dt.Columns.Add("数量");
                dt.Columns.Add("单价");
                dt.Columns.Add("退货原因");
                dt.Columns.Add("时间");
                if (model != null && model.Count > 0)
                {
                    foreach (var item in model)
                    {
                        int index = 0;
                        DataRow dr = dt.NewRow();
                        dr[index++] = item.MaterialName;
                        dr[index++] = item.MaterialSpec;
                        dr[index++] = item.Consumers_Name;
                        dr[index++] = item.Consumers_Phone;
                        dr[index++] = item.Consumers_Address;
                        dr[index++] = item.MaterialCount;
                        dr[index++] = item.MaterialPrice;
                        dr[index++] = item.Content;
                        dr[index++] = item.Addtime;
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch { throw; }
            return dt;
        }
    }
}
