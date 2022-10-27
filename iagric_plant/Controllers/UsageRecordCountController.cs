/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-17

** 联系方式 :13313318725

** 描述：主要用于二维码（使用记录）统计控制器

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Web;
using System.Web.Mvc;
using Common.Argument;
using LinqModel;
using BLL;
using Common.Log;
using System.Text;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 主要用于二维码（使用记录）统计控制器
    /// </summary>
    public class UsageRecordCountController : Controller
    {
        //
        // GET: /UsageRecordCount/

        /// <summary>
        /// 获取二维码使用记录列表
        /// </summary>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(int index, string beginDate, string endDate, int pageIndex = 1)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                UsageRecordCountBLL bll = new UsageRecordCountBLL();
                result = bll.GetList(index, pf.EnterpriseID, beginDate, endDate, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "UsageRecordCountController.Index():MaterialUsageRecord表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取产品模型
        /// </summary>
        /// <param name="id">仓库标识</param>
        /// <returns></returns>
        public JsonResult GetMaterial(string beginCode)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new UsageRecordCountBLL().GetMaterial(beginCode);
            }
            catch (Exception ex)
            {
                string errData = "UsageRecordCountController.GetModel";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id">记录id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult GetRecordDetail(long id, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new UsageRecordCountBLL().GetRecordDetail(id, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "UsageRecordCountController.GetRecordDetail";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取要导出的码内容
        /// </summary>
        /// <param name="id">记录id</param>
        /// <returns></returns>
        public string ExportTxt(long id)
        {
            try
            {
                string json = "无码内容，请联系系统管理员！";
                string mess = "";
                DateTime dt1 = DateTime.Now;
                string TimeString = dt1.ToShortDateString().ToString() + "-" + dt1.ToLongTimeString().ToString();//获取小时分钟秒字符串
                string fileName = "SellCode--" + TimeString + ".txt";
                //int serialNumLength = Convert.ToInt32(ConfigurationManager.AppSettings["SerialNumLength"]);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/plain"; //设置输出文件类型为txt文件。
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode("" + fileName, System.Text.Encoding.UTF8)));
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                byte[] arrWrite = Encoding.Default.GetBytes(mess);
                Response.BinaryWrite(arrWrite);
                UsageRecordCountBLL bll = new UsageRecordCountBLL();
                mess = bll.GetExportTxt(id);
                byte[] arrWriteData = Encoding.Default.GetBytes(mess);
                Response.BinaryWrite(arrWriteData);
                Response.Flush();
                Response.End();
                return json;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
