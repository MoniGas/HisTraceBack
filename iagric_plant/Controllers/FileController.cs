//激活二维码，上传码包接口
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Common.Log;
using LinqModel;
using Common.Argument;
using BLL;
using Newtonsoft.Json;
using Dal;

namespace iagric_plant.Controllers
{
    public class FileController : Controller
    {
        //
        // GET: /File/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 接收激活码包
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                JsonResult js = new JsonResult();
                string URL = string.Format(@"~\{0}\{1}", @"File\RecPack", DateTime.Now.ToString("yyyyMMdd"));
                string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + System.IO.Path.GetExtension(file.FileName);
                string filePath = Server.MapPath(URL);
                string loginName = Request["loginName"].ToString();
                string loginPwd = Request["loginPwd"].ToString();
                LinqModel.ActiveEwmRecord ewmRecord = new LinqModel.ActiveEwmRecord();
                int Status = (int)Common.EnumFile.RecEwm.已接收;
                ewmRecord.PackName = fileName;
                ewmRecord.UploadDate = DateTime.Now;
                ewmRecord.RecPath = filePath + @"\" + fileName;
                ewmRecord.Status = Status;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                file.SaveAs(Path.Combine(filePath, fileName));
                BLL.ActinvEwmBLL bll = new BLL.ActinvEwmBLL();
                string msg = "";
                long recId = 0;
                bool ret = bll.AddRecPack(ewmRecord, loginName, loginPwd, out msg, out recId);
                if (ret)
                {
                    return Json(new { ret = ret, msg = "接收码包成功！" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ret = ret, msg = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(ex.Message);
                return Json(new { ret = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 激活上传文件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="scDate">生产日期</param>
        /// <param name="fileurl"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ActiveUploadFile(string userName, string pwd, string scDate, string fileurl, string filepath)
        {
            BaseResultModel result = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            try
            {
                Enterprise_UserBLL userbll = new Enterprise_UserBLL();
                Enterprise_User usermodel = userbll.GetTxtUserModel(user.EnterpriseID, userName, pwd);
                if (usermodel != null)
                {
                    ActiveEwmRecord ewmRecord = new ActiveEwmRecord();
                    int Status = (int)Common.EnumFile.RecEwm.已接收;
                    ewmRecord.PackName = fileurl;
                    ewmRecord.UploadDate = DateTime.Now;
                    ewmRecord.RecPath = filepath;
                    ewmRecord.Status = Status;
                    ewmRecord.UpUserID = usermodel.Enterprise_User_ID;
                    ewmRecord.EnterpriseId = usermodel.Enterprise_Info_ID;
                    ewmRecord.ProductionDate = Convert.ToDateTime(scDate);
                    ewmRecord.AddDate = DateTime.Now;
                    ewmRecord.StrAddTime = DateTime.Now.ToString("yyyy-MM-dd");
                    ewmRecord.AddUserName = user.UserName;
                    ewmRecord.OperationType = (int)Common.EnumFile.OperationType.流水线;
                    List<ToJsonImg> Imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(fileurl);
                    foreach (var Item in Imgs)
                    {
                        ewmRecord.PackName = Item.fileUrl;
                        ewmRecord.RecPath = Item.fileUrls.Replace("/", "\\");
                        int qattr2 = Item.fileUrls.LastIndexOf('/');
                        string aa = Item.fileUrls.Substring(0, qattr2);
                        ewmRecord.URL = Item.fileUrlp;
                    }
                    BLL.ActinvEwmBLL bll = new BLL.ActinvEwmBLL();
                    result = bll.AddActiveRecPack(ewmRecord);
                }
                else
                {
                    return Json(ToJson.NewRetResultToJson((Convert.ToInt32(false)).ToString(), "请确认是您企业的账号！"));
                }
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material.AddExcelR():MaterialExportExcelRecord表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="type">形式</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageIndex">分页</param>
        /// <returns></returns>
        public JsonResult AcriveEWMList(string type, string beginDate, string endDate, int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                ActinvEwmBLL bll = new ActinvEwmBLL();
                result = bll.GetActiveEwmList(pf.EnterpriseID, Convert.ToInt32(type), beginDate, endDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "RequestCodeMaController.RequestCodeSettinglist()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        #region 获取生产单元列表
        [HttpPost]
        public JsonResult BatchList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            ActinvEwmBLL bll = new ActinvEwmBLL();
            BaseResultList result = new BaseResultList();
            try
            {
                result = bll.GetBatchList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Batch.GreenHouseList():Greenhouses表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion

        /// <summary>
        /// 选择批次激活
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="batchName">批次</param>
        /// <param name="dealerID">经销商</param>
        /// <param name="scDate">生产日期</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ActiveEWM(string userName, string pwd, string batchNameID, string dealerID, string scDate)
        {
            BaseResultModel result = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            try
            {
                Enterprise_UserBLL userbll = new Enterprise_UserBLL();
                Enterprise_User usermodel = userbll.GetTxtUserModel(user.EnterpriseID, userName, pwd);
                if (usermodel != null)
                {
                    ActiveEwmRecord ewmRecord = new ActiveEwmRecord();
                    int Status = (int)Common.EnumFile.RecEwm.已接收;
                    ewmRecord.UploadDate = DateTime.Now;
                    ewmRecord.Status = Status;
                    ewmRecord.UpUserID = usermodel.Enterprise_User_ID;
                    ewmRecord.EnterpriseId = usermodel.Enterprise_Info_ID;
                    ewmRecord.OperationType = (int)Common.EnumFile.OperationType.后台;
                    ewmRecord.AddDate = DateTime.Now;
                    ewmRecord.StrAddTime = DateTime.Now.ToString("yyyy-MM-dd");
                    ewmRecord.AddUserName = user.UserName;
                    ewmRecord.DealerID = Convert.ToInt64(dealerID);
                    if (ewmRecord.DealerID > 0)
                    {
                        ewmRecord.ProductionDate = Convert.ToDateTime(scDate);
                        DealerDAL dedal = new DealerDAL();
                        Dealer dealer = dedal.GetModel(ewmRecord.DealerID.Value);
                        ewmRecord.DealerName = dealer.DealerName;
                    }
                    else
                    {
                        ewmRecord.ProductionDate = null;
                    }
                    BLL.ActinvEwmBLL bll = new BLL.ActinvEwmBLL();
                    result = bll.ActiveEWM(ewmRecord, Convert.ToInt64(batchNameID));
                }
                else
                {
                    return Json(ToJson.NewRetResultToJson((Convert.ToInt32(false)).ToString(), "请确认是您企业的账号！"));
                }
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material.AddExcelR():MaterialExportExcelRecord表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
