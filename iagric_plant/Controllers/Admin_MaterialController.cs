using System;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using Common.Log;
using LinqModel;
using System.Collections.Generic;
using System.Data;
using Aspose.Cells;
using System.IO;
using System.Text;
using System.Web;
using System.Runtime.Serialization.Json;

namespace iagric_plant.Controllers
{
    public class Admin_MaterialController : BaseController
    {

        public ActionResult Index(string materialName, int pageIndex = 1)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new MaterialBLL().GetList(user.EnterpriseID, materialName, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Index";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public ActionResult Info(int id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new MaterialBLL().GetModel(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Info";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult Add(string materialName, string materialAliasName, long materialBrand, string processId, string materialSpec,
            string materialShelfLife, string materialPropertyInfo, string materialMemo, string materialMaterialImgInfo,
            string materialPrice, string video, string tburl, string categoryID, string meatCategoryName, long meatType, string ntbUrl, string jdUrl,
            string tmUrl, string wdUrl, string materialjj, string materialPlace, string adFiles, string videoUrl, long materialSpecId = 0)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Material model = new Material();
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                long? bId = null;
                if (materialBrand != 0)
                {
                    bId = materialBrand;
                }
                model.Brand_ID = bId;
                model.CNWORD = "";
                model.Dictionary_MaterialType_ID = null;
                model.languageid = 1;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Material_Code = "";
                model.MaterialBarcode = null;
                model.MaterialCategory = null;
                model.MaterialName = materialName;
                model.MaterialAliasName = materialAliasName;
                //model.MaterialSpecification = materialSpec;
                model.MaterialSpecificationID = materialSpecId;
                model.Memo = materialMemo;
                model.ShelfLife = materialShelfLife;
                model.type = 0;
                model.Unit = "";
                model.MaterialFullName = materialName;
                model.MaterialGrade = "";
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.StrMaterialImgInfo = materialMaterialImgInfo;
                model.StrPropertyInfo = materialPropertyInfo;
                model.Status = (int)Common.EnumFile.Status.used;
                model.ProcessID = Convert.ToInt64(processId);
                //model.CodeUser = codeUser;
                model.tbURL = tburl;
                model.MaAttribute = meatType;
                model.Materialjj = materialjj;
                //model.MaterialTaste = materialTaste;
                model.MaterialPlace = materialPlace;
                //model.NYType = nytype;
                //model.NYZhengHao = nyzhenghao;
                Category category = new Category();
                category.AddTime = DateTime.Now;
                category.AddUser = user.UserID;
                category.CategoryID = long.Parse(categoryID);
                category.Enterprise_Info_ID = user.EnterpriseID;
                category.Status = (int)Common.EnumFile.Status.used;
                if (string.IsNullOrEmpty(materialPrice))
                {
                    model.price = null;
                }
                else
                {
                    model.price = Convert.ToDecimal(materialPrice);
                }
                //List<MaterialEvaluation> materialpj = new List<MaterialEvaluation>();
                //List<string> list = new List<string>();
                //list.Add(pingjia1);
                //list.Add(pingjia2);
                //list.Add(pingjia3);
                //list.Add(pingjia4);
                //list.Add(pingjia5);
                //for (int i = 0; i < list.Count; i++)
                //{
                //    MaterialEvaluation temp = new MaterialEvaluation();
                //    temp.EvaluationName = list[i];
                //    temp.EnterpriseID = user.EnterpriseID;
                //    temp.AddDate = DateTime.Now;
                //    temp.Status = (int)Common.EnumFile.Status.used;
                //    materialpj.Add(temp);
                //}
                MaterialShopLink shopLik = new MaterialShopLink
                {
                    AddDate = DateTime.Now,
                    AddUser = user.UserID,
                    TaoBaoLink = ntbUrl,
                    JingDongLink = jdUrl,
                    TianMaoLink = tmUrl,
                    WeiDianLink = wdUrl,
                    StrAdFileUrl = adFiles,
                    StrVideoUrlInfo = videoUrl
                };
                result = new MaterialBLL().Add(model, user.MainCode, category, video, shopLik, meatCategoryName);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改产品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(long materialId, string materialName, string materialAliasName, long materialBrand, string processId, string materialSpec, string materialShelfLife,
            string materialPropertyInfo, string materialMemo, string materialMaterialImgInfo, string materialPrice, string tburl, string materialjj, string materialPlace,
             string video, string adFiles, string ntbUrl, string jdUrl, string tmUrl, string wdUrl, string videoUrl, long materialSpecId = 0)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Material model = new Material();
                model.Material_ID = materialId;
                long? bId = null;
                if (materialBrand != 0)
                {
                    bId = materialBrand;
                }
                model.Brand_ID = bId;
                model.ProcessID = Convert.ToInt64(processId);
                model.CNWORD = "";
                model.languageid = 1;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Material_Code = "";
                model.MaterialBarcode = null;
                model.MaterialCategory = null;
                model.MaterialName = materialName;
                model.MaterialAliasName = materialAliasName;
                //model.MaterialSpecification = materialSpec;
                model.Memo = materialMemo;
                model.ShelfLife = (materialShelfLife ?? "").Replace("null", "");
                model.type = 0;
                model.Unit = "";
                model.MaterialFullName = materialName;
                model.MaterialGrade = "";
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                model.StrMaterialImgInfo = materialMaterialImgInfo;
                model.StrPropertyInfo = materialPropertyInfo;
                model.tbURL = tburl;
                //新添加
                model.MaterialSpecificationID = materialSpecId;
                //model.MaAttribute = maAttribute;
                //model.MaterialTaste = materialTaste;
                model.Materialjj = materialjj;
                model.MaterialPlace = materialPlace;
                //20180416新加农药码配合使用
                //model.NYType = nytype;
                //model.NYZhengHao = nyzhenghao;
                if (string.IsNullOrEmpty(materialPrice))
                {
                    model.price = null;
                }
                else
                {
                    model.price = Convert.ToDecimal(materialPrice);
                }
                //List<MaterialEvaluation> materialpj = new List<MaterialEvaluation>();
                //List<string> list = new List<string>();
                //list.Add(pingjia1);
                //list.Add(pingjia2);
                //list.Add(pingjia3);
                //list.Add(pingjia4);
                //list.Add(pingjia5);
                //for (int i = 0; i < list.Count; i++)
                //{
                //    MaterialEvaluation temp = new MaterialEvaluation();
                //    temp.EvaluationName = list[i];
                //    temp.EnterpriseID = user.EnterpriseID;
                //    temp.AddDate = DateTime.Now;
                //    temp.Status = (int)Common.EnumFile.Status.used;
                //    materialpj.Add(temp);
                //}
                MaterialShopLink shopLik = new MaterialShopLink { TaoBaoLink = ntbUrl, JingDongLink = jdUrl, TianMaoLink = tmUrl, WeiDianLink = wdUrl, StrAdFileUrl = adFiles, StrVideoUrlInfo = videoUrl, AddUser = user.UserID, AddDate = DateTime.Now };
                result = new MaterialBLL().Edit(model, video, shopLik);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult EditMaterial(long materialId, string materialName, long materialBrand, string processId, string materialSpec, string materialShelfLife, string materialPropertyInfo, string materialMemo, string materialMaterialImgInfo, string materialPrice, long materialType, long materialSpecId = 0)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Material model = new Material();
                model.Material_ID = materialId;
                long? bId = null;
                if (materialBrand != 0)
                {
                    bId = materialBrand;
                }
                model.Brand_ID = bId;
                model.ProcessID = Convert.ToInt64(processId);
                model.CNWORD = "";
                if (materialType > 0)
                {
                    model.Dictionary_MaterialType_ID = materialType;
                }
                model.languageid = 1;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Material_Code = "";
                model.MaterialBarcode = null;
                model.MaterialCategory = null;
                model.MaterialName = materialName;
                model.MaterialSpecification = materialSpec;
                model.Memo = materialMemo;
                model.ShelfLife = (materialShelfLife ?? "").Replace("null", "");
                model.type = 0;
                model.Unit = "";
                model.MaterialFullName = materialName;
                model.MaterialGrade = "";
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                model.StrMaterialImgInfo = materialMaterialImgInfo;
                model.StrPropertyInfo = materialPropertyInfo;
                model.MaterialSpecificationID = materialSpecId;
                if (string.IsNullOrEmpty(materialPrice))
                {
                    model.price = null;
                }
                else
                {
                    model.price = Convert.ToDecimal(materialPrice);
                }
                result = new MaterialBLL().Edit(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Edit";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public ActionResult Delete(long id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                result = new MaterialBLL().Del(user.EnterpriseID, id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Delete";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        #region 获取生产流程列表
        /// <summary>
        /// 获取生产流程列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ProcessList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                ProcessBLL bll = new ProcessBLL();
                result = bll.GetProcessList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.ProcessList():Process表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion

        /// <summary>
        /// 生成码页面简单添加产品信息
        /// </summary>
        /// <param name="materialName">产品名称</param>
        /// <param name="materialBrand">品牌</param>
        /// <param name="materialSpec">产品规格</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSimple(string materialName, long materialBrand, string materialSpec)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                LoginInfo user = SessCokie.Get;
                Material model = new Material();
                model.adddate = DateTime.Now;
                model.adduser = user.UserID;
                long? bId = null;
                if (materialBrand != 0)
                {
                    bId = materialBrand;
                }
                model.Brand_ID = bId;
                model.CNWORD = "";
                model.Dictionary_MaterialType_ID = null;
                model.languageid = 1;
                model.lastdate = DateTime.Now;
                model.lastuser = user.UserID;
                model.Material_Code = "";
                model.MaterialBarcode = null;
                model.MaterialCategory = null;
                model.MaterialName = materialName;
                model.MaterialSpecification = materialSpec;
                model.type = 0;
                model.Unit = "";
                model.MaterialFullName = materialName;
                model.MaterialGrade = "";
                model.Enterprise_Info_ID = user.EnterpriseID;
                model.Status = (int)Common.EnumFile.Status.used;
                result = new MaterialBLL().AddSimple(model);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.Add";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取产品品类
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMateiralType(int level, long parent)
        {
            List<LinqModel.Dictionary_MaterialType> list = null;
            try
            {
                MaterialBLL bll = new MaterialBLL();
                list = bll.GetMaterialType(level, parent);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.GetMateiralType";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(list);
        }

        /// <summary>
        /// 根据产品类别名称查询数据
        /// </summary>
        /// <param name="name">产品类别名称</param>
        /// <returns></returns>
        public JsonResult SearchType(string typeName)
        {
            List<ToJsonProperty> result = new List<ToJsonProperty>();
            try
            {
                MaterialBLL bll = new MaterialBLL();
                result = bll.SearchType(typeName);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.SearchType";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取产品评价
        /// </summary>
        /// <param name="id">产品ID</param>
        /// <returns></returns>
        public ActionResult MaPJInfo(int id)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new MaterialBLL().GetMaPJ(id);
            }
            catch (Exception ex)
            {
                string errData = "Admin_MaterialController.MaPJInfo";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 产品导出Excel
        /// </summary>
        /// <returns></returns>
        public int ExportExcel(string searchTitle)
        {
            int json = 0;
            try
            {
                LoginInfo pf = SessCokie.Get;
                MaterialExportExcelRecord excelRecordModel = new MaterialExportExcelRecord();
                excelRecordModel.EnterpriseID = pf.EnterpriseID;
                excelRecordModel.AddUser = pf.UserID;
                excelRecordModel.AddDate = DateTime.Now;
                DataTable result = new BLL.MaterialBLL().ExportExcel(pf.EnterpriseID, searchTitle, excelRecordModel);
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
                string errData = "Admin_Material.Index():Material表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 数据记录Excel样式
        /// </summary>
        /// <param name="datatable"></param>
        /// <param name="error"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <param name="genid"></param>
        /// <param name="excelurl"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddExcelR(string excelurl, string excelpath)
        {
            BaseResultModel result = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            try
            {
                MaterialExportExcelRecord model = new MaterialExportExcelRecord();
                model.EnterpriseID = user.EnterpriseID;
                model.ExcelURL = excelurl;
                model.ExcelPath = excelpath;
                model.AddDate = DateTime.Now;
                model.AddUser = user.UserID;
                model.Status = (int)Common.EnumFile.Status.unaudited;
                MaterialBLL bll = new MaterialBLL();
                result = bll.AddExcelR(model);
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
        ///  获取待审核列表（导入的Excel）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult GetExcelRecord(int pageIndex = 1)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BaseResultList();
            try
            {
                MaterialBLL bll = new MaterialBLL();
                result = bll.GetExcelRecord(pf.EnterpriseID, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material.GetExcelRecord():MaterialExportExcelRecord表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 审核导入Excel
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public JsonResult AuditExcel(string id)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                MaterialBLL bll = new MaterialBLL();
                result = bll.AuditExcel(pf.EnterpriseID, Convert.ToInt64(id));
            }
            catch (Exception ex)
            {
                string errData = "Admin_Material.AuditExcel():临时表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 其他企业导入Excel
        /// </summary>
        /// <param name="genid"></param>
        /// <param name="excelurl"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddExcelRQT(string userName, string pwd, string excelurl, string excelpath)
        {
            BaseResultModel result = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            try
            {
                if (user.UserType == "默认")
                {
                    Enterprise_UserBLL userbll = new Enterprise_UserBLL();
                    Enterprise_User usermodel = userbll.GetExcelUserModel(userName, pwd);
                    if (usermodel != null)
                    {
                        MaterialExportExcelRecord model = new MaterialExportExcelRecord();
                        model.EnterpriseID = usermodel.Enterprise_Info_ID;
                        model.ExcelURL = excelurl;
                        model.ExcelPath = excelpath;
                        model.AddDate = DateTime.Now;
                        model.AddUser = user.UserID;
                        model.Status = (int)Common.EnumFile.Status.unaudited;
                        MaterialBLL bll = new MaterialBLL();
                        result = bll.AddExcelR(model);
                    }
                    else
                    {
                        return Json(ToJson.NewRetResultToJson((Convert.ToInt32(false)).ToString(), "没有找到该企业信息！"));
                    }
                }
                else
                {
                    return Json(ToJson.NewRetResultToJson((Convert.ToInt32(false)).ToString(), "您没有权限导入其他企业数据！"));
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
        /// 下载TXT 导出产品信息
        /// </summary>
        /// <param name="eId"></param>
        /// <returns></returns>
        public string ExportTxt()
        {
            LoginInfo user = SessCokie.Get;
            string json = "无内容，请联系系统管理员！";
            string mess = "";
            DateTime dt1 = DateTime.Now;
            string TimeString = dt1.ToShortDateString().ToString() + "-" + dt1.ToLongTimeString().ToString();//获取小时分钟秒字符串
            string fileName = "Product--" + TimeString + ".txt";
            string endEwm = string.Empty;
            try
            {
                MaterialBLL bll = new MaterialBLL();
                BaseResultModel result = bll.ExportTxt(user.EnterpriseID);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/plain"; //设置输出文件类型为txt文件。
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode("" + fileName, System.Text.Encoding.UTF8)));
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                LoginInfo pf = Common.Argument.SessCokie.Get;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Product>));
                //创建存储区为内存流
                System.IO.MemoryStream ms = new MemoryStream();
                //将json字符串写入内存流中
                serializer.WriteObject(ms, result.ObjModel);
                System.IO.StreamReader reader = new StreamReader(ms);
                ms.Position = 0;
                mess = reader.ReadToEnd();
                reader.Close();
                ms.Close();
                byte[] arrWriteData = Encoding.Default.GetBytes(mess);
                Response.BinaryWrite(arrWriteData);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                string errData = "RequestCodeMaController.ExportTxt()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return json;
        }
    }
}
