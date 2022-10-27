/********************************************************************************
** 作者： 张翠霞
** 创始时间：2018-06-19
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：
** 修改时间：
** 描述：
**  主要用于数据采集管理 
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Common.Argument;
using BLL;
using LinqModel;
using System.Text;
using Common.Log;

namespace iagric_plant.Controllers
{
    public class Admin_CollectCodeController : Controller
    {
        //
        // GET: /Admin_CollectCode/

        /// <summary>
        /// 获取采集记录列表
        /// </summary>
        /// <param name="beginDate">采集开始日期</param>
        /// <param name="endDate">采集结束日期</param>
        /// <param name="status">状态（未下载/已下载）</param>
        /// <param name="collectUser">采集人员</param>
        /// <param name="pageIndex">分页</param>
        /// <returns></returns>
        public JsonResult Index(string beginDate, string endDate, string status, string collectUser, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            if (string.IsNullOrEmpty(status))
            {
                status = "0";
            }
            BaseResultList result = new CollectCodeBLL().GetList(user.EnterpriseID, Convert.ToInt32(status), beginDate, endDate + " 23:59:59", collectUser, pageIndex);
            return Json(result);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sId">标识ID</param>
        /// <param name="pageIndex">分页</param>
        /// <returns></returns>
        public JsonResult CollectCodeInfo(long sId, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;
            CollectCodeBLL bll = new CollectCodeBLL();
            BaseResultList dataList = bll.CollectCodeInfo(sId, user.EnterpriseID, pageIndex);
            if (dataList.totalCounts == 0)
            {
                dataList.ObjList = 0;
            }
            return Json(dataList);
        }

        /// <summary>
        /// 下载详情内容
        /// </summary>
        /// <param name="sID">标识ID</param>
        /// <returns></returns>
        public string ExportTxt(string sID)
        {
            //int json = 0;
            LoginInfo user = SessCokie.Get;
            string json = "下载成功！";
            #region 下载为TXT文档
            string mess = "";
            DateTime dt1 = DateTime.Now;
            //string TimeString = dt1.ToShortDateString().ToString() + "-" + dt1.ToLongTimeString().ToString();//获取小时分钟秒字符串
            string TimeString = dt1.ToString("yyyy-MM-dd") + "-" + sID;//获取小时分钟秒字符串
            string fileName = TimeString + ".txt";
            string endEwm = string.Empty;
            try
            {
                RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();
                CollectCodeBLL bll = new CollectCodeBLL();
                List<CollectCodeDetail> liCode = bll.GetCollectCodeTxt(Convert.ToInt64(sID), user.EnterpriseID);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/plain"; //设置输出文件类型为txt文件。
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode("" + fileName, System.Text.Encoding.UTF8)));
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                byte[] arrWrite = Encoding.Default.GetBytes(mess);
                Response.BinaryWrite(arrWrite);
                for (int i = 0; i < liCode.Count; i++)
                {
                    CollectCodeDetail modelCode = liCode[i];
                    mess = modelCode.MaterialName + "\t" + modelCode.MaterialSpection + "\t" + modelCode.MaterialCode + "\t" + modelCode.ProductionDate + "\t\r\n";
                    byte[] arrWriteData = Encoding.Default.GetBytes(mess);
                    Response.BinaryWrite(arrWriteData);
                }
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                string errData = "Admin_CollectCodeController.ExportTxt()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            #endregion
            return json;
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="sId">标识ID</param>
        /// <returns></returns>
        public JsonResult UpdateStatus(string sId)
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultModel ObjBaseResultModel = new CollectCodeBLL().UpdateStatus(sId, user.EnterpriseID);
            return Json(ObjBaseResultModel);
        }

        public JsonResult GetEwmURL()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            string code = Request.Params["ewm"];
            return Json(code);
        }
    }
}
