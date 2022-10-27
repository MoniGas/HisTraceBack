using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Common.Argument;
using Dal;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class Status
    {
        public string statusId { get; set; }
        public string name { get; set; }
    }
    public class Material_OnlineOrderBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        public List<Material> GetMaterialList(long enterpriseId)
        {
            return new Dal.Material_OnlineOrderDAL().GetMaterialList(enterpriseId);
        }
        public List<Material_Spec> GetMaterialSpecList(long materialId)
        {
            return new Dal.Material_OnlineOrderDAL().GetMaterialSpecList(materialId);
        }
        public List<View_MaterialSpecForMarket> GetMarketMaterialSpecList(long materialId) 
        {
            return new Dal.Material_OnlineOrderDAL().GetMarketMaterialSpecList(materialId);
        }
        public View_MaterialSpecForMarket GetMarketMaterialSpecModel(long MaterialSpecId) 
        {
            return new Dal.Material_OnlineOrderDAL().GetMarketMaterialSpecModel(MaterialSpecId);
        }
        public RetResult Add(Material_OnlineOrder model)
        {
            return new Dal.Material_OnlineOrderDAL().Add(model);
        }
        public BaseResultList GetList(string name, long eID, string sDate, string eDate, int status, int delStatus, int pageIndex)
        {
            long totalCount = 0;
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            List<View_Material_OnlineOrder> model = dal.GetList(name, eID, sDate, eDate, status, delStatus, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }
        public PagedList<View_Material_OnlineOrder> GetList(long ConsumersID, int type, int pageIndex, int pageSize)
        {
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            PagedList<View_Material_OnlineOrder> objList = dal.GetList(ConsumersID, type, pageIndex, pageSize);
            return objList;
        }
        public BaseResultModel Edit(long id, string yundanComp, string yundanNum, long userID)
        {
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            RetResult ret = dal.Edit(id, yundanComp, yundanNum, userID);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Del(long id, out int delStatus)
        {
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            RetResult ret = dal.Del(id, out delStatus);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        public BaseResultList GetStatus(int type = 1)
        {
            long totalCount = 0;
            List<Status> model = new List<Status>();
            //Status a = new Status();
            //a.statusId = "-1";
            //a.name = "全部";
            //model.Add(a);
            if (type == 1)
            {
                Status b = new Status();
                b.statusId = ((int)Common.EnumFile.PayStatus.NotPay).ToString();
                b.name = "未付款";
                model.Add(b);
                Status c = new Status();
                c.statusId = ((int)Common.EnumFile.PayStatus.Paid).ToString();
                c.name = "已付款，未发货";
                model.Add(c);
                Status d = new Status();
                d.statusId = ((int)Common.EnumFile.PayStatus.Delivered).ToString();
                d.name = "已发货";
                model.Add(d);
                Status e = new Status();
                e.statusId = ((int)Common.EnumFile.PayStatus.Confirm).ToString();
                e.name = "确认收货";
                model.Add(e);
            }
            else if (type == 2)
            {
                Status b = new Status();
                b.statusId = ((int)Common.EnumFile.PayStatus.ReturnMaterial).ToString();
                b.name = "申请退货";
                model.Add(b);
                Status c = new Status();
                c.statusId = ((int)Common.EnumFile.PayStatus.ReturnAgree).ToString();
                c.name = "同意退货";
                model.Add(c);
                Status d = new Status();
                d.statusId = ((int)Common.EnumFile.PayStatus.ReturnRefuse).ToString();
                d.name = "不同意退货";
                model.Add(d);
                Status e = new Status();
                e.statusId = ((int)Common.EnumFile.PayStatus.Returned).ToString();
                e.name = "退货中";
                model.Add(e);
                Status f = new Status();
                f.statusId = ((int)Common.EnumFile.PayStatus.ReturnFinsh).ToString();
                f.name = "退货完成";
                model.Add(f);
            }
            BaseResultList result = ToJson.NewListToJson(model, 1, PageSize, totalCount, "");
            return result;
        }

        public BaseResultModel GetModel(long id)
        {
            Material_OnlineOrder material = new Dal.Material_OnlineOrderDAL().GetModel(id);
            string code = "1";
            string msg = "";
            if (material == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(material, code, msg);
            return model;
        }
        public Material_OnlineOrder GetModel(string orderNum)
        {
            return new Dal.Material_OnlineOrderDAL().GetModel(orderNum);
        }

        public BaseResultModel TrueDel(long id)
        {
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            RetResult ret = dal.TrueDel(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        public DataTable ExportExcel(string name, long eID, string sDate, string eDate, int status, int delStatus)
        {
            DataTable dt = new DataTable();
            try
            {
                Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
                List<View_Material_OnlineOrder> model = dal.ExportExcel(name, eID, sDate, eDate, status, delStatus);

                dt.Columns.Add("产品");
                dt.Columns.Add("规格");
                dt.Columns.Add("联系人");
                dt.Columns.Add("联系电话");
                dt.Columns.Add("地址");
                dt.Columns.Add("数量");
                dt.Columns.Add("单价");
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
                        dr[index++] = item.AddTime;
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch { throw; }
            return dt;
        }

        /// <summary>
        /// 更该订单状态
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <param name="payType">状态</param>
        /// <param name="TradeNo">支付宝订单号</param>
        /// <param name="ConsumerLogin">消费者登录名</param>
        /// <param name="ConsumerNum">消费者支付宝16位唯一标识</param>
        /// <returns>操作结果</returns>
        public RetResult PaySuccess(string orderNum, int payType, string TradeNo = "", string ConsumerLogin = "", string ConsumerNum = "")
        {
            return new Dal.Material_OnlineOrderDAL().PaySuccess(orderNum, payType, TradeNo, ConsumerLogin, ConsumerNum);
        }

        public RetResult ChangeOrder(Material_OnlineOrder model)
        {
            return new Dal.Material_OnlineOrderDAL().ChangeOrder(model);
        }

        #region 结算
        public BaseResultList GetOrderCheck(long EnterpriseId, int pageIndex)
        {
            long totalCount = 0;
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            List<View_Order_Check> model = dal.GetOrderCheck(EnterpriseId, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }
        public BaseResultList GetMaterialCheck(long checkId, int pageIndex)
        {
            long totalCount = 0;
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            List<View_Material_OnlineOrder> model = dal.GetMaterialCheck(checkId, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }
        public BaseResultModel BalanceOrder(long checkId)
        {
            Material_OnlineOrderDAL dal = new Material_OnlineOrderDAL();
            RetResult ret = dal.BalanceOrder(checkId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

         /// <summary>
        /// 获取对账单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_Order_Check> GetList(long enterpriseId, int pageIndex, long agentCode,string name)
        {
            return new Material_OnlineOrderDAL().GetList(enterpriseId, pageIndex, agentCode,name);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_Material_OnlineOrder> GetCheckList(long checkId, int pageIndex)
        {
            return new Material_OnlineOrderDAL().GetCheckList(checkId, pageIndex);
        }
        #endregion
    }
}
