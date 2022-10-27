using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;
using System.IO;
using Aspose.Cells;
using System.Data;

namespace BLL
{
    public class MaterialDIBLL
    {
        private readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long enterpriseId, string searchName, int pageIndex)
        {
            long totalCount;
            MaterialDIDAL dal = new MaterialDIDAL();
            List<MaterialDI> model = dal.GetList(enterpriseId, searchName, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _pageSize, totalCount, "");
            //string result = JsonConvert.SerializeObject(model);
            return result;
        }

        public BaseResultModel SyncUDIDI(string mainCode)
        {
            MaterialDIDAL dal = new MaterialDIDAL();
            RetResult ret = dal.SyncUDIDI(mainCode, "");
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        #region 20200824产品DI当天是第几次生成码
        public int GetMaterialDICount(string materialDI)
        {
            try
            {
                MaterialDIDAL dal = new MaterialDIDAL();
                return dal.GetMaterialDICount(materialDI);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        #endregion
        public BaseResultModel UpdateDI(int eId, string DICode, string GS1Code, string SPCode, string SpecLevel, int SpecNum, string HisCode, string XH)
        {
            MaterialDIDAL dal = new MaterialDIDAL();
            RetResult ret = dal.UpdateDI(eId, DICode, GS1Code, SPCode, SpecLevel, SpecNum, HisCode, XH, "");
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        #region
        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public BaseResultModel AddExcelR(GS1DIExcelRecord newModel)
        {
            RetResult ret = new RetResult();
            MaterialDIDAL dal = new MaterialDIDAL();
            FileStream fs = new FileStream(newModel.ExcelPath, FileMode.Open, FileAccess.Read);
            Workbook book = new Workbook(fs);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;
            DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            //ret = maPDAL.Verify(ds);
            //if (ret.IsSuccess)
            //{
            ret = dal.DIInportExcel(ds, newModel);
            if (ret.IsSuccess)
            {
                ret.Msg = "导入成功！";
            }
            //}
            else
            {
                ret.Msg = "导入失败！";
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        #endregion
    }
}
