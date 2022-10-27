using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dal;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using System.IO;
using Aspose.Cells;
using System.Data;
using Common.Log;

namespace BLL
{
    public class UDIMaterialBLL
	{
		#region 此处两个方法仅用于获取国药监数据

		#region 读取Excel
		UDIMaterialDAL dal = new UDIMaterialDAL();
		/// <summary>
		/// 读取Excel
		/// </summary>
		/// <param name="fileUrl">文件路径</param>
		/// <returns></returns>
		public BaseResultModel SaveUDIMaterial(string fileUrl)
		{
            //System.Data.DataSet ds = null;
            //string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", fileUrl);
            //using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn))
            //{
            //    string strExcel = "";
            //    System.Data.OleDb.OleDbDataAdapter myCommand = null;
            //    strExcel = "select * from [标识信息$]";
            //    myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            //    ds = new System.Data.DataSet();
            //    myCommand.Fill(ds, "table1");
            //}

			DataSet ds = new DataSet();
			DataSet dsPack = new DataSet();
			DataSet dsCC = new DataSet();
			DataSet dsLC = new DataSet();
			DataSet dsLXR = new DataSet();

			FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
			Workbook book = new Workbook(fs);

			Worksheet sheet = book.Worksheets[0];
			Cells cells = sheet.Cells;
			DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);

			ds.Tables.Add(dt);
			MaterialDAL madal = new MaterialDAL();
			DealerExcelRecord exModel = new DealerExcelRecord();
			exModel.MaCount = ds.Tables[0].Rows.Count;

			#region 包装标识信息
			if (book.Worksheets.Count >= 2)
			{
				Worksheet sheetPack = book.Worksheets[1];
				Cells cellsPack = sheetPack.Cells;
				DataTable dtPack = cellsPack.ExportDataTableAsString(0, 0, cellsPack.MaxDataRow + 1, cellsPack.MaxDataColumn + 1, true);
				dsPack.Tables.Add(dtPack);
			}

			//DealerExcelRecord exModel = new DealerExcelRecord();
			//exModel.MaCount = ds.Tables[0].Rows.Count;
			#endregion

			#region 存储或操作信息
			if (book.Worksheets.Count >= 3)
			{
				Worksheet sheetCC = book.Worksheets[2];
				Cells cellsCC = sheetCC.Cells;
				DataTable dtCC = cellsCC.ExportDataTableAsString(0, 0, cellsCC.MaxDataRow + 1, cellsCC.MaxDataColumn + 1, true);
				dsCC.Tables.Add(dtCC);
			}

			//DealerExcelRecord exModel = new DealerExcelRecord();
			//exModel.MaCount = ds.Tables[0].Rows.Count;
			#endregion

			#region 临床使用尺寸信息
			if (book.Worksheets.Count >= 4)
			{
				Worksheet sheetLC = book.Worksheets[3];
				Cells cellsLC = sheetLC.Cells;
				DataTable dtLC = cellsLC.ExportDataTableAsString(0, 0, cellsLC.MaxDataRow + 1, cellsLC.MaxDataColumn + 1, true);
				dsLC.Tables.Add(dtLC);
			}

			//DealerExcelRecord exModel = new DealerExcelRecord();
			//exModel.MaCount = ds.Tables[0].Rows.Count;
			#endregion

			#region 企业联系信息
			if (book.Worksheets.Count >= 5)
			{
				Worksheet sheetLXR = book.Worksheets[4];
				Cells cellsLXR = sheetLXR.Cells;
				DataTable dtLXR = cellsLXR.ExportDataTableAsString(0, 0, cellsLXR.MaxDataRow + 1, cellsLXR.MaxDataColumn + 1, true);
				dsLXR.Tables.Add(dtLXR);
			}

			//DealerExcelRecord exModel = new DealerExcelRecord();
			//exModel.MaCount = ds.Tables[0].Rows.Count;
			#endregion

			
			RetResult ret = dal.SaveUDIMaterial(ds, dsPack, dsCC, dsLC, dsLXR);
			if (ret.Code == 0)
			{
				try
				{
					fs.Close();
					File.Delete(fileUrl);
				}
				catch (Exception ex)
				{
					string errData = "UDIMaterialBLL.SaveUDIMaterial()";
					WriteLog.WriteErrorLog(errData + ":" + ex.Message);
				}
			}
			BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
			return result;
		}
		#endregion

		#region 
		public RetResult GetUDI(string requestTypes, string date) 
		{
			RetResult ret = dal.GetUDI(requestTypes, date);
			return ret;
		}
        /// <summary>
        /// 根据企业名称获取UDI数据
        /// </summary>
        /// <param name="enterpriseName">企业名称</param>
        /// <param name="synchroDate">版本日期</param>
        /// <param name="page">当前页数</param>
        /// <returns></returns>
        public BaseResultModel GetMaterialDIByEnterprise(string enterpriseName, string synchroDate, string page)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                UDIMaterialDAL dal = new UDIMaterialDAL();
                var model = dal.GetMaterialDIByEnterprise(enterpriseName, synchroDate, page); ;
                result = ToJson.NewModelToJson(model, "0", "调用成功");
            }
            catch (Exception ex)
            {

                throw;
            }
            return result;

        }
		#endregion

		#endregion


		#region UDIKey业务处理
		/// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<UDIKey> GetUDIKeyList(string beginDate, string endDate, int? pageIndex)
        {
            UDIMaterialDAL dal = new UDIMaterialDAL();
            PagedList<UDIKey> dataList = dal.GetUDIKeyList(beginDate, endDate,pageIndex);
            return dataList;
        }

        /// <summary>
        /// 添加key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel Add(UDIKey model)
        {
            UDIMaterialDAL dal = new UDIMaterialDAL();
            RetResult result = dal.Add(model);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public UDIKey GetKeyInfo(long id)
        {
            UDIMaterialDAL dal = new UDIMaterialDAL();
            UDIKey data = dal.GetKeyInfo(id);
            return data;
        }

        public BaseResultModel Edit(UDIKey model)
        {
            UDIMaterialDAL dal = new UDIMaterialDAL();
            RetResult result = dal.Edit(model);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public RetResult EditStatus(long id, int type)
        {
            UDIMaterialDAL dal = new UDIMaterialDAL();
            RetResult result = new RetResult();
            result = dal.EditStatus(id, type);
            return result;
        }
        #endregion

       
    }
}
