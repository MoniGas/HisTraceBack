/********************************************************************************

** 作者： 张翠霞

** 创始时间：2017-02-08

** 联系方式 :13313318725

** 描述：追溯码信息管理

** 版本：v2.5.1

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using Common.Log;
using BLL;
using System.Data;
using System.IO;
namespace iagric_plant.Controllers
{
    /// <summary>
    /// 追溯码信息管理控制器
    /// </summary>
    public class RequestCodeMaController : Controller
    {
        //
        // GET: /RequestCodeMa/

        /// <summary>
        /// 追溯码生成记录
        /// </summary>
        /// <param name="searchName">相关信息</param>
        /// <param name="mName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(string searchName, string mName, string bName, string beginDate, string endDate, int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.GetList(pf.EnterpriseID, searchName, mName, bName, beginDate, endDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "RequestCodeMaController.Index()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 追溯信息配置状态
        /// </summary>
        /// <param name="settingId"></param>
        /// <returns></returns>
        public JsonResult GetPacketState(long settingId)
        {
            var settingModel = new RequestCodeSettingBLL().GetModel(settingId);
            var data = settingModel == null ? 0 : settingModel.PacketState;
            data = data ?? 0;
            return Json(data);
        }

        /// <summary>
        /// 获取红包链接
        /// </summary>
        /// <param name="enterpriseId">企业主码</param>
        /// <param name="settingId">配置id</param>
        /// <returns></returns>
        public JsonResult GetRedPacket(long enterpriseId, long settingId)
        {
            var enterModel = new EnterpriseInfoBLL().GetModel(enterpriseId);
            var settingModel = new RequestCodeSettingBLL().GetModel(settingId);
            string url = string.Empty;
            url = string.Format("/Market/ActivityManager/AddActivity?trace=1&mainCode={0}&settingId={1}&enterpriseID={2}", enterModel.MainCode, settingId, enterpriseId);
            //url = string.Format("/Market/Packet/Add?trace=1&mainCode={0}&settingId={1}&enterpriseID={2}", enterModel.MainCode, settingId, enterpriseId);
            var data = new { url = url };
            return Json(data);
        }

        /// <summary>
        /// 查看红包二维码
        /// </summary>
        /// <param name="settingId">配置id</param>
        /// <returns></returns>
        public JsonResult PacketEwm(long settingId)
        {
            string url = url = string.Format("/Market/ActivityManager/YuLan?settingId={0}", settingId);
            return Json(new { url = url });
        }

        /// <summary>
        /// 分段码列表根据requestcodeID查询列表
        /// </summary>
        /// <param name="requestcodeID">码记录编号</param>
        /// <param name="searchName">相关信息</param>
        /// <param name="mName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Setcodelist(string requestcodeID, string searchName, string mName, string bName, string beginDate, string endDate, int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.GetRequestCodeSettingList(pf.EnterpriseID, Convert.ToInt64(requestcodeID), searchName, mName, bName, beginDate, endDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "RequestCodeMaController.Setcodelist()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 查询企业所有配置码信息
        /// </summary>
        /// <param name="searchName">相关信息</param>
        /// <param name="mName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult RequestCodeSettinglist(string searchName, string mName, string bName, string beginDate, string endDate, int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.GetRequestCodeSettingListAll(pf.EnterpriseID, searchName, mName, bName, beginDate, endDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "RequestCodeMaController.RequestCodeSettinglist()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取子批次信息
        /// </summary>
        /// <param name="requestId">申请码标识列</param>
        /// <returns>列表</returns>
        public JsonResult RequestCodeSettinglistSub(long requestId)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.GetRequestCodeSettingListSub(requestId);
            }
            catch (Exception ex)
            {
                string errData = "RequestCodeMaController.RequestCodeSettinglistSub()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 下载二维码txt格式
        /// </summary>
        /// <param name="sID">设置表ID</param>
        /// <returns></returns>
        public string ExportTxt(string sID)
        {
            //int json = 0;
            string json = "无码内容，请联系系统管理员！";
            #region 改为直接下载文件后注释的内容  陈志钢  2017年8月22日
            //string mess = "";
            //DateTime dt1 = DateTime.Now;
            //string TimeString = dt1.ToShortDateString().ToString() + "-" + dt1.ToLongTimeString().ToString();//获取小时分钟秒字符串
            //string fileName = "SettingCode--" + TimeString + ".txt";
            //string endEwm = string.Empty;
            //try
            //{
            //    RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();
            //    List<Enterprise_FWCode_00> liCode = objRequestCodeBLL.GetSettingCodeTxt("", Convert.ToInt64(sID), 0);
            //    Response.Clear();
            //    Response.Buffer = true;
            //    Response.ContentType = "text/plain"; //设置输出文件类型为txt文件。
            //    Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode("" + fileName, System.Text.Encoding.UTF8)));
            //    Response.ContentEncoding = System.Text.Encoding.UTF8;
            //    byte[] arrWrite = Encoding.Default.GetBytes(mess);
            //    string urlStr = System.Configuration.ConfigurationManager.AppSettings["idcodeURL"];
            //    LoginInfo pf = Common.Argument.SessCokie.Get;
            //    if (pf.Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
            //    {
            //        urlStr = System.Configuration.ConfigurationManager.AppSettings["ncpURL"].ToString();
            //    }
            //    Response.BinaryWrite(arrWrite);
            //    for (int i = 0; i < liCode.Count; i++)
            //    {
            //        Enterprise_FWCode_00 modelCode = liCode[i];
            //        MaterialBLL maBll = new MaterialBLL();
            //        Material maModel = maBll.MaMode(modelCode.Material_ID.HasValue ? modelCode.Material_ID.GetValueOrDefault() : long.MinValue);
            //        if (maModel != null)
            //        {
            //            mess = maModel.MaterialName + "\t" + urlStr + modelCode.EWM + "\t" + modelCode.FWCode + "\t\r\n";
            //            byte[] arrWriteData = Encoding.Default.GetBytes(mess);
            //            Response.BinaryWrite(arrWriteData);
            //        }
            //        else
            //        {
            //            mess = modelCode.EWM + "\t" + modelCode.FWCode + "\t\r\n";
            //            byte[] arrWriteData = Encoding.Default.GetBytes(mess);
            //            Response.BinaryWrite(arrWriteData);
            //        }
            //    }
            //    Response.Flush();
            //    Response.End();
            //}
            //catch (Exception ex)
            //{
            //    string errData = "RequestCodeMaController.ExportTxt()";
            //    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            //}
            #endregion
            DataView dvRecord = new BLL.RequestCodeSettingAddBLL().getRecordByID(long.Parse(sID));
            if (!string.IsNullOrEmpty(dvRecord[0]["FileURL"].ToString()))
            {
                //string filePath = Server.MapPath("~" + dvRecord[0]["FileURL"].ToString());//路径 
                string filePath = dvRecord[0]["FileURL"].ToString();//路径   
                string filename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".zip";
                FileStream fs = new FileStream(filePath, FileMode.Open);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
            return json;
        }

        /// <summary>
        /// 开通追溯/开通防伪修改类型
        /// </summary>
        /// <param name="id">码标识ID</param>
        /// <param name="requestCodeType">码类型</param>
        /// <returns></returns>
        public JsonResult EditType(long id, int requestCodeType)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = SessCokie.Get;
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.EditType(id, pf.EnterpriseID, requestCodeType);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "RequestCodeMaController.EditType():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 选择追溯/防伪修改类型
        /// </summary>
        /// <param name="id">码标识ID</param>
        /// <param name="requestCodeType">码类型</param>
        /// <returns></returns>
        public JsonResult EditTypeTwo(long id, string materialId, int requestCodeType)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = SessCokie.Get;
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.EditTypeTwo(id, pf.EnterpriseID, Convert.ToInt64(materialId), requestCodeType);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "RequestCodeMaController.EditType():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 获取PI数据
        /// </summary>
        /// <returns></returns> 
        public JsonResult GetPIInfo()
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = SessCokie.Get;
                RequestCodeMaBLL bll = new RequestCodeMaBLL();
                result = bll.GetPIInfo(pf.MainCode);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "RequestCodeMaController.EditType():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
