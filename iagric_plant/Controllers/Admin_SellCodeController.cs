/********************************************************************************
** 作者： 李子巍
** 创始时间：2015-06-15
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：xxx
** 修改时间：xxx-xx-xx
** 描述：
** 主要用于销售控制器
*********************************************************************************/
using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;
using Common;

namespace iagric_plant.Controllers
{
    public class Admin_SellCodeController : BaseController
    {
        //
        // GET: /Admin_SellCode/

        public JsonResult Index(string beginDate, string endDate, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new SellCodeBLL().GetSaleList(beginDate,endDate + " 23:59:59", user.EnterpriseID, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_SellCodeController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult GetCode() 
        {
            LinqModel.BaseResultModel DataModel = new BaseResultModel();

            if (Session["SellRequestId"] != null)
            {
                try
                {
                    string RequestId = Session["SellRequestId"].ToString();

                    SellCodeBLL ObjSellCodeBLL = new SellCodeBLL();
                    DataModel = ObjSellCodeBLL.GetCode(Convert.ToInt64(RequestId));
                    Session.Remove("SellRequestId");
                }
                catch 
                {

                }
                return Json(DataModel);
            }
            else 
            {
                return Json(ToJson.NewRetResultToJson("0", ""));
            }
        }

        /// <summary>
        /// 销售二维码
        /// </summary>
        /// <param name="productionTime">生产日期</param>
        /// <param name="dealerId">经销商编号</param>
        /// <param name="beginCode">起始码</param>
        /// <param name="endCode">结束码</param>
        /// <returns>销售信息</returns>
        public JsonResult SellCode(string productionTime,string dealerId, string beginCode, string endCode)
        {
            Encryption ObjEncryption = new Encryption();
            beginCode = ObjEncryption.CodeDecrypt(beginCode);
            endCode = ObjEncryption.CodeDecrypt(endCode);
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                SellCodeBLL bll = new SellCodeBLL();
                result = bll.SaleCode(user.EnterpriseID, productionTime, dealerId, beginCode, endCode);
            }
            catch (Exception ex)
            {
                string errData = "Admin_SellCodeController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult CodeList(string SalesId, string Ewm,string EwmTableIdArray, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                //result = new SellCodeBLL().GetEWMCode(code, SalesId, (int)Common.EnumFile.UsingStateCode.HasBeenUsed, pageIndex);
                result = new SellCodeBLL().GetSalesDetail(Ewm, SalesId, EwmTableIdArray, pageIndex);
            }
            catch (Exception ex)
            {   
                string errData = "Admin_SellCodeController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult EnterpriseMainCode(long eid)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            Enterprise_Info en = new Enterprise_Info();
            en.MainCode = pf.MainCode;
            SellCodeBLL bll = new SellCodeBLL();
            result = bll.GetEnterpriseList(pf.EnterpriseID);
            return Json(result);
        }
    }
}
