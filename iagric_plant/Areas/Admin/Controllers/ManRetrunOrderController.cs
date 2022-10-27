/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-8-17
** 联系方式:13313318725
** 代码功能：商城业务
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Web.Mvc;
using iagric_plant.Areas.Admin.Filter;
using BLL;
using Common.Argument;
using System.Data;
using Aspose.Cells;
using System.IO;
using Common.Log;

namespace iagric_plant.Areas.Admin.Controllers
{
    /// <summary>
    /// 退货处理
    /// </summary>
    [ManAuthorizeAttribute]
    public class ManRetrunOrderController : Controller
    {
        /// <summary>
        /// 退货退款
        /// </summary>
        /// <param name="name"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult Index(string name, string beginDate, string endDate, long enterpriseId = 0, int status = 0, int pageIndex = 1)
        {
            long agentCode = 0;
            if (SessCokie.GetMan.PRRU_PlatFormLevel_ID == (int)Common.EnumFile.PlatFormLevel.RegulatoryAuthorities)
            {
                //代理号
                agentCode = SessCokie.GetMan.EnterpriseID;
            }
            var modelLst = new MaterialReturnOrderBLL().GetList(name, enterpriseId, beginDate, endDate, status, pageIndex, agentCode);
            ViewBag.Name = name;
            ViewBag.startTime = beginDate;
            ViewBag.endTime = endDate;
            return View(modelLst);
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult BalanceIndex(string name, int pageIndex = 1)
        {
            long agentCode = 0;
            if (SessCokie.GetMan.PRRU_PlatFormLevel_ID == (int)Common.EnumFile.PlatFormLevel.RegulatoryAuthorities)
            {
                //代理号
                agentCode = SessCokie.GetMan.EnterpriseID;
            }
            ViewBag.Name = name;
            var modelLst = new Material_OnlineOrderBLL().GetList(0, pageIndex, agentCode, name);
            return View(modelLst);
        }

        /// <summary>
        /// 查看订单
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        public ActionResult BalanceInfo(long checkId, int pageIndex = 1)
        {
            var modelLst = new Material_OnlineOrderBLL().GetCheckList(checkId, pageIndex);
            return View(modelLst);
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <param name="bDate"></param>
        /// <param name="eDate"></param>
        /// <returns></returns>
        public void ExportExcel(string name, string bDate, string eDate, int status = 0)
        {
            int json = 0;
            try
            {
                LoginInfo pf = SessCokie.GetMan;
                DataTable result = new BLL.MaterialReturnOrderBLL().ExportExcel(name, pf.EnterpriseID, bDate, eDate, status, 0);
                string outError = "";
                Workbook wb = DataTableToExcel2(result, out outError);
                DateTime dt1 = DateTime.Now;
                string TimeString = DateTime.Now.ToString("yyyy-MM-dd HHmmss");
                string path = Server.MapPath(@"/ExportExcel/") + TimeString + ".xls";
                wb.Save(path);

                System.IO.FileInfo DownloadFile = new System.IO.FileInfo(path);
                string strFileName = Path.GetFileName(path);

                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = false;
                Response.ContentType = "application/octet-stream";
                //Response.ContentType = "application/ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + strFileName);
                Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                Response.WriteFile(DownloadFile.FullName);
                Response.Flush();
                Response.End();
                json = 1;
            }
            catch (Exception ex)
            {
                string errData = "Admin_ReturnMaterialController.Index():Material_ReturnMaterial表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
        }

        private Workbook DataTableToExcel2(DataTable datatable, out string error)
        {
            error = "";
            Workbook wb = new Workbook();
            try
            {
                if (datatable == null)
                {
                    error = "DataTableToExcel:datatable 为空";
                    return wb;
                }

                //为单元格添加样式    
                Style style = wb.Styles[wb.Styles.Add()];
                //设置居中
                style.HorizontalAlignment = TextAlignmentType.Center;
                //设置背景颜色
                style.ForegroundColor = System.Drawing.Color.FromArgb(153, 204, 0);
                style.Pattern = BackgroundType.Solid;
                style.Font.IsBold = true;

                int rowIndex = 0;
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    DataColumn col = datatable.Columns[i];
                    string columnName = col.Caption ?? col.ColumnName;
                    wb.Worksheets[0].Cells[rowIndex, i].PutValue(columnName);
                    wb.Worksheets[0].Cells[rowIndex, i].SetStyle(style);
                }
                rowIndex++;

                foreach (DataRow row in datatable.Rows)
                {
                    for (int i = 0; i < datatable.Columns.Count; i++)
                    {
                        wb.Worksheets[0].Cells[rowIndex, i].PutValue(row[i].ToString());
                    }
                    rowIndex++;
                }

                for (int k = 0; k < datatable.Columns.Count; k++)
                {
                    wb.Worksheets[0].AutoFitColumn(k, 0, 150);
                }
                wb.Worksheets[0].FreezePanes(1, 0, 1, datatable.Columns.Count);
                return wb;
            }
            catch (Exception e)
            {
                error = error + " DataTableToExcel: " + e.Message;
                return wb;
            }
        }
    }
}
