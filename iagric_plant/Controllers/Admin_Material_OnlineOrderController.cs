using System;
using System.Data;
using System.IO;
using System.Web.Mvc;
using Aspose.Cells;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_Material_OnlineOrderController : BaseController
    {
        // GET: /Admin_Material_OnlineOrder/
        public ActionResult Info(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.Material_OnlineOrderBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Info";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult Index(string name, int status, string bDate, string eDate, int pageIndex = 1)
        {
            BaseResultList list = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                list = new BLL.Material_OnlineOrderBLL().GetList(name, pf.EnterpriseID, bDate, eDate, status, 0, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material_OnlineOrderController.Index():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(list);
        }

        public JsonResult Edit(long id, string ydComp, string ydNum)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = SessCokie.Get;
                result = new BLL.Material_OnlineOrderBLL().Edit(id, ydComp, ydNum, pf.UserID);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_OnlineOrderController.Index():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public JsonResult Del(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = SessCokie.Get;
                int a = 0;
                result = new BLL.Material_OnlineOrderBLL().Del(id, out a);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_OnlineOrderController.Del():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        public JsonResult TrueDel(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo pf = SessCokie.Get;
                int a = 0;
                result = new BLL.Material_OnlineOrderBLL().TrueDel(id);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_OnlineOrderController.Del():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public ActionResult DelIndex(string name, int status, string bDate, string eDate, int pageIndex = 1)
        {
            BaseResultList list = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                list = new BLL.Material_OnlineOrderBLL().GetList(name, pf.EnterpriseID, bDate, eDate, status, 1, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material_OnlineOrderController.Index():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(list);
        }

        public JsonResult GetStatus()
        {

            BaseResultList liChannel = new BaseResultList();
            try
            {
                liChannel = new BLL.Material_OnlineOrderBLL().GetStatus();
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
                DataTable result = new BLL.Material_OnlineOrderBLL().ExportExcel(name, pf.EnterpriseID, bDate, eDate, status, 0);
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
                string errData = "Admin_Material_OnlineOrderController.Index():Material_OnlineOrder表";
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

        #region 结算
        public JsonResult GetBalanceList(int pageIndex = 1)
        {
            BaseResultList list = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                if (pf.PRRU_PlatFormLevel_ID == (int)Common.EnumFile.PlatFormLevel.Enterprise)
                    list = new BLL.Material_OnlineOrderBLL().GetOrderCheck(pf.EnterpriseID, pageIndex);
                else
                    list = new BLL.Material_OnlineOrderBLL().GetOrderCheck(0, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material_OnlineOrderController.GetBalanceList():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(list);
        }
        public JsonResult GetOrderList(long checkId, int pageIndex = 1)
        {
            BaseResultList list = new BaseResultList();
            try
            {
                LoginInfo pf = SessCokie.Get;
                list = new BLL.Material_OnlineOrderBLL().GetMaterialCheck(checkId, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material_OnlineOrderController.GetOrderList():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(list);
        }
        public JsonResult BalanceOrder(long checkId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.Material_OnlineOrderBLL().BalanceOrder(checkId);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_Material_OnlineOrderController.BalanceOrder():Material_OnlineOrder表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
    }
}
