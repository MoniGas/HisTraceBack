using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using Dal;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_EnterpriseInfoController : BaseController
    {
        //
        // GET: /Admin_EnterpriseInfo/

        private readonly EnterpriseInfoDAL _dal = new EnterpriseInfoDAL();
        public ActionResult Index()
        {
            BaseResultModel reuslt = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                EnterpriseInfoBLL objEnterpriseInfoBll = new EnterpriseInfoBLL();
                reuslt = objEnterpriseInfoBll.GetModelView(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_EnterpriseInfoController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(reuslt);
        }

        /// <summary>
        /// 修改企业信息（IDcode平台接口上传，本地库更新）
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="personName"></param>
        /// <param name="telephone"></param>
        /// <param name="email"></param>
        /// <param name="address"></param>
        /// <param name="memo"></param>
        /// <param name="webUrl"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(string trade, string etrade, string personName, string telephone, string email, string address, string memo, string webUrl, string zjType, string zjhm, string logo, string file)
        {
            BaseResultModel strResult = new BaseResultModel();
            try
            {
                EnterpriseInfoBLL objEnterpriseInfoBll = new EnterpriseInfoBLL();
                LoginInfo user = SessCokie.Get;
                strResult = objEnterpriseInfoBll.Edit(user.MainCode,user.EnterpriseID, trade, etrade, personName, telephone, email, address, memo, webUrl, zjType, zjhm, logo, file);
            }
            catch (Exception ex)
            {
                string errData = "Admin_EnterpriseInfoController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        public ActionResult Verify(string fileUrl, int zjType, string zjhm)
        {
            BaseResultModel strResult = new BaseResultModel();
            try
            {
                EnterpriseInfoBLL objEnterpriseInfoBll = new EnterpriseInfoBLL();
                LoginInfo user = SessCokie.Get;
                strResult = objEnterpriseInfoBll.Verify(user.MainCode, fileUrl, zjType, zjhm);
            }
            catch (Exception ex)
            {
                string errData = "Admin_EnterpriseInfoController.Verify";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }
        #region 获取行业列表
        [HttpPost]
        public JsonResult TradeList(int level)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                List<Trade> tradeList = new List<Trade>();
                TradeInfo AllAddress = Common.Argument.BaseData.listTrade;
                if (AllAddress != null && AllAddress.TradeList != null)
                {
                    tradeList = AllAddress.TradeList.Where(p => p.TradeLevel == level).ToList();
                }
                result = ToJson.NewListToJson(tradeList, 1, 10000000, 0, "");
            }
            catch (Exception ex)
            {
                string errData = "Admin_EnterpriseInfoController.TradeList";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult ETradeList(long parent)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                List<Trade> tradeList = new List<Trade>();
                TradeInfo AllAddress = Common.Argument.BaseData.listTrade;
                if (AllAddress != null && AllAddress.TradeList != null)
                {
                    tradeList = AllAddress.TradeList.Where(p => p.Trade_ID_Parent == parent).ToList();
                }
                result = ToJson.NewListToJson(tradeList, 1, 10000000, 0, "");
            }
            catch (Exception ex)
            {
                string errData = "Admin_EnterpriseInfoController.ETradeList";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
        public ActionResult OpenShop(string accountNum, string accountName, string linkPhone)
        {
            BaseResultModel strResult = new BaseResultModel();
            try
            {
                EnterpriseInfoBLL objEnterpriseInfoBll = new EnterpriseInfoBLL();
                LoginInfo user = SessCokie.Get;
                strResult = objEnterpriseInfoBll.OpenShop(user.MainCode, accountNum, accountName, linkPhone);
            }
            catch (Exception ex)
            {
                string errData = "Admin_EnterpriseInfoController.OpenShop";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(strResult);
        }

        public JsonResult GetAccount()
        {
            BaseResultModel reuslt = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                reuslt = new EnterpriseAccountBLL().GetModel(user.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_EnterpriseInfoController.GetAccount";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(reuslt);
        }

        /// <summary>
        /// 获取企业信息（获取企业简码用）
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEnterpriseModel()
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                EnterpriseInfoBLL bll = new EnterpriseInfoBLL();
                result = bll.GetEnterpriseModel(user.EnterpriseID);
                result.Msg = user.EnterpriseName;
            }
            catch (Exception ex)
            {
                string errData = "ComplaintController.GetHomeDataStatis";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取码标
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMb()
        {
            BaseResultModel result = new BaseResultModel
            {
                code = "-1",
                Msg = "同步失败！"
            };
            LoginInfo user = SessCokie.Get;
            if (null != user)
            {
                Enterprise_User info = new Enterprise_UserBLL().GetUserByEnterpriseId(user.EnterpriseID);
                if (null != info)
                {

                    ResultInfo mbCode = InterfaceWeb.BaseDataDAL.GetMbCode("hebeiguanglian", "123321", "");
                    //Result mbCode = InterfaceWeb.BaseDataDAL.GetMbCode(info.LoginName, info.LoginPassWord, "");
                    List<Enterprise_MB> list = new List<Enterprise_MB>();
                    if (null != mbCode && mbCode.data.Count > 0)
                    {
                        foreach (MBInfo mbInfo in mbCode.data)
                        {
                            Enterprise_MB model = new Enterprise_MB
                            {
                                EnterpriseId = user.EnterpriseID,
                                reg_date = DateTime.Parse(mbInfo.reg_Date),
                                start_date = DateTime.Parse(mbInfo.start_Date),
                                end_date = DateTime.Parse(mbInfo.end_Date),
                                unit_name = mbInfo.unit_Name,
                                unit_phone = mbInfo.unit_Phone,
                                agent_name = mbInfo.agent_Name
                            };
                            list.Add(model);
                        }
                        _dal.AddMb(list);
                        result.code = "0";
                        result.Msg = "同步成功！";
                    }
                }
            }
            return Json(result);

        }

        /// <summary>
        /// 码标管理控制器
        /// </summary>
        /// <returns></returns>
        public JsonResult MbIndex()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                //MaterialSpcificationBLL bll = new MaterialSpcificationBLL();
                List<Enterprise_MB> list = _dal.GetMBList(pf.EnterpriseID);
                result.ObjList = list;
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialSpcification.Index():MaterialSpcification表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}
