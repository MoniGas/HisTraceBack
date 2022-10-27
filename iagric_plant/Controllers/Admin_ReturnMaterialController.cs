using System;
using System.Web.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using System.Data;
using System.IO;
using Aspose.Cells;

namespace iagric_plant.Controllers
{
    public class Admin_ReturnMaterialController : Controller
    {
        //
        // GET: /Admin_ReturnMaterial/

        public ActionResult Index(string Name, int Status, string BeginDate, string EndDate, int PageIndex = 1)
        {
            BaseResultList list = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                list = new BLL.MaterialReturnOrderBLL().GetReturnOrderList(Name, pf.EnterpriseID, BeginDate, EndDate, Status, PageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ReturnMaterialController.Index():Material_ReturnMaterial表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(list);
        }
        public JsonResult GetStatus()
        {
            BaseResultList liChannel = new BaseResultList();
            try
            {
                liChannel = new BLL.Material_OnlineOrderBLL().GetStatus(2);
            }
            catch (Exception ex)
            {
                string errData = "";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(liChannel);
        }
        public int ExportExcel(string name, int status, string bDate, string eDate)
        {
            int json = 0;
            try
            {
                LoginInfo pf = SessCokie.Get;
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
            return json;
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
        public JsonResult EditStatus(long OrderNum, int Status)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = SessCokie.Get;
                result = new BLL.MaterialReturnOrderBLL().EditStatus(OrderNum, pf.UserID, Status);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ReturnMaterial.EditStatus():View_Material_ReturnOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
    }
}
