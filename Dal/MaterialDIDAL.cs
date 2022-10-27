using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using Common.Log;
using Common.Argument;
using Common.Tools;
using InterfaceWeb;

namespace Dal
{
    public class MaterialDIDAL : DALBase
    {
        public List<MaterialDI> GetList(long enterpriseId, string searchName, out long totalCount, int pageIndex)
        {
            List<MaterialDI> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.MaterialDI.Where(m => m.EnterpriseID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(searchName))
                    {
                        data = data.Where(m => m.MaterialName.Contains(searchName.Trim()) || m.MaterialUDIDI.Contains(searchName.Trim()) ||
                            m.Specifications.Contains(searchName.Trim()) || m.CategoryCode.Contains(searchName.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDIDAL.GetList():MaterialDI表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        public RetResult SyncUDIDI(string mainCode, string conStr)
        {
            LoginInfo pf = SessCokie.Get;
            string Msg = "同步UDI-DI失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            List<MaterialDI> listMaDI = new List<MaterialDI>();
            try
            {
                if (string.IsNullOrEmpty(conStr))
                {
                    conStr = "WebConnect";
                }
                using (DataClassesDataContext dataContext = GetDataContext(conStr))
                {
                    Enterprise_Info enInfo = dataContext.Enterprise_Info.Where(p => p.MainCode == mainCode).FirstOrDefault();

                    string entoken = "";
                    string enTokenCode = "";
                    EnterpriseShopLink enEx = new Dal.ScanCodeDAL().ShopEn(enInfo.Enterprise_Info_ID);
                    if (enEx != null && !string.IsNullOrEmpty(enEx.access_token) && !string.IsNullOrEmpty(enEx.access_token_code))
                    {
                        entoken = enEx.access_token == null ? "" : enEx.access_token;
                        enTokenCode = enEx.access_token_code == null ? "" : enEx.access_token_code;
                    }
                    else
                    {
                        Ret.SetArgument(CmdResultError.EXCEPTION, "企业token不存在", "企业token不存在");
                        return Ret;
                    }
                    #region 调用UDI-DI接口
                    ListHisDI diList = BaseDataDAL.GetDIList(mainCode, entoken, enTokenCode);
                    Ret.SetArgument(CmdResultError.EXCEPTION, "DI数量：" + diList.data.Count(), "DI数量：" + diList.data.Count());
                    if (diList != null && diList.data != null)
                    {
                        foreach (HisDI sub in diList.data)
                        {
                            //DI去重
                            MaterialDI maDIdate = dataContext.MaterialDI.FirstOrDefault(m => m.MaterialName == sub.modelnumber &&
                            m.EnterpriseID == enInfo.Enterprise_Info_ID && m.MaterialUDIDI == sub.completecode &&
                            m.Status == (int)Common.EnumFile.Status.used);
                            if (maDIdate == null)
                            {
                                MaterialDI model = new MaterialDI();
                                model.adddate = DateTime.Now;
                                model.adduser = pf == null ? 0 : pf.UserID;
                                model.CategoryCode = sub.categorycode;
                                model.EnterpriseID = enInfo.Enterprise_Info_ID;
                                model.MaterialName = sub.modelnumber;
                                Material madate = dataContext.Material.FirstOrDefault(m => m.Enterprise_Info_ID == enInfo.Enterprise_Info_ID && m.MaterialName == model.MaterialName && m.Status == (int)Common.EnumFile.Status.used);
                                if (madate != null)
                                {
                                    model.MaterialID = madate.Material_ID;
                                }
                                else
                                {
                                    Material temp = new Material();
                                    temp.adddate = DateTime.Now;
                                    temp.adduser = pf == null ? 0 : pf.UserID;
                                    temp.Enterprise_Info_ID = enInfo.Enterprise_Info_ID;
                                    temp.MaterialName = sub.modelnumber;
                                    temp.MaterialFullName = sub.modelnumber;
                                    temp.Status = (int)Common.EnumFile.Status.used;
                                    dataContext.Material.InsertOnSubmit(temp);
                                    dataContext.SubmitChanges();
                                    model.MaterialID = temp.Material_ID;
                                    Category tempca = new Category();
                                    tempca.CategoryCode = sub.categorycode.Substring(1, sub.categorycode.Length - 2);
                                    tempca.AddTime = DateTime.Now;
                                    tempca.AddUser = pf == null ? 0 : pf.UserID;
                                    tempca.Enterprise_Info_ID = enInfo.Enterprise_Info_ID;
                                    tempca.MaterialID = temp.Material_ID;
                                    tempca.MaterialName = temp.MaterialName;
                                    tempca.Status = (int)Common.EnumFile.Status.used;
                                    dataContext.Category.InsertOnSubmit(tempca);
                                    dataContext.SubmitChanges();
                                }
                                model.MaterialUDIDI = sub.completecode;
                                model.Specifications = sub.specifications;
                                model.SpecificationName = sub.specification_name;
                                model.Status = (int)Common.EnumFile.Status.used;
                                model.MaterialXH = sub.product_model;
                                model.createtype = Convert.ToInt32(sub.createtype);
                                model.ISUpload = 3;//3表示是从发码平台同步下来的
                                listMaDI.Add(model);
                            }
                        }
                        dataContext.MaterialDI.InsertAllOnSubmit(listMaDI);
                        dataContext.SubmitChanges();
                        Msg = "同步成功！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, "同步成功", "同步成功");
                    }
                    else
                    {
                        Msg = diList.result_msg;
                        error = CmdResultError.EXCEPTION;
                        Ret.SetArgument(CmdResultError.EXCEPTION, diList.result_msg, diList.result_msg);
                    }
                    #endregion
                }
            }
            catch
            {
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        /// <summary>
        /// 上传DI信息到发码机构
        /// </summary>
        /// <param name="eId"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public RetResult UpUDIDI(string mainCode, string conStr)
        {
            LoginInfo pf = SessCokie.Get;
            string Msg = "同步UDI-DI失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext(conStr))
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.Where(p => p.MainCode == mainCode).FirstOrDefault();
                    if (enterprise != null)
                    {
                        string entoken = "";
                        string entokenCode = "";
                        EnterpriseShopLink enEx = new Dal.ScanCodeDAL().ShopEn(enterprise.Enterprise_Info_ID);
                        if (enEx != null)
                        {
                            entoken = enEx.access_token == null ? "" : enEx.access_token;
                            entokenCode = enEx.access_token_code == null ? "" : enEx.access_token_code;
                        }
                        else
                        {
                            Ret.SetArgument(error, "企业token值不存在", "企业token值不存在");
                            return Ret;
                        }
                        List<MaterialDI> listMaDI = dataContext.MaterialDI.Where(m => m.EnterpriseID == enterprise.Enterprise_Info_ID
                            && m.Status == (int)Common.EnumFile.Status.used && m.ISUpload == 0).ToList();
                        foreach (var sub in listMaDI)
                        {
                            HisResult result = BaseDataDAL.IDCodeMedicalReg(enterprise.MainCode, sub.CategoryCode,
                                sub.MaterialName, sub.Specifications, entoken, entokenCode);
                            //上传成功后更新状态
                            if (result.result_code == 1)
                            {
                                sub.ISUpload = 1;
                                dataContext.SubmitChanges();
                                Ret.SetArgument(CmdResultError.NONE, "操作成功", "操作成功");
                            }
                            else
                            {
                                Ret.SetArgument(CmdResultError.NONE, result.result_msg, result.result_msg);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #region 20200824产品DI当天是第几次生成码
        public int GetMaterialDICount(string materialDI)
        {
            int result;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    MaterialDI maDI = dataContext.MaterialDI.FirstOrDefault(m => m.MaterialUDIDI == materialDI);
                    if (maDI != null)
                    {
                        DateTime dateNow = DateTime.Now;
                        DateTime endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
                        DateTime starTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                        var temoModel = dataContext.RequestCode.Where(m =>
                            m.FixedCode == materialDI && starTime <= m.RequestDate && dateNow <= m.RequestDate);
                        if (temoModel != null && temoModel.Count() > 0)
                        {
                            result = temoModel.Count() + 1;
                        }
                        else
                        {
                            result = 1;
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 更新DI信息
        /// </summary>
        /// <param name="mainCode"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public RetResult UpdateDI(int eId, string DICode, string GS1Code, string SPCode, string SpecLevel, int SpecNum, string HisCode, string XH
            , string conStr)
        {
            string Msg = "操作失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            List<MaterialDI> listMaDI = new List<MaterialDI>();
            try
            {
                if (string.IsNullOrEmpty(conStr))
                {
                    conStr = "WebConnect";
                }
                using (DataClassesDataContext dataContext = GetDataContext(conStr))
                {
                    MaterialDI mDI = dataContext.MaterialDI.Where(p => p.MaterialUDIDI == DICode).FirstOrDefault();
                    if (mDI != null)
                    {
                        mDI.GSIDI = GS1Code;
                        mDI.SPCode = SPCode;
                        mDI.SpecLevel = SpecLevel;
                        mDI.SpecNum = SpecNum;
                        mDI.MaterialXH = XH;
                        mDI.HisCode = HisCode;
                        dataContext.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        #region 20211229GS1企业导入DI信息Excel
        /// <summary>
        /// 导入DI数据
        /// </summary>
        /// <param name="ds">数据表</param>
        /// <param name="eId">企业编码</param>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public RetResult DIInportExcel(System.Data.DataSet ds, GS1DIExcelRecord newModel)
        {
            Ret.Msg = "导入DI信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            try
            {
                if (ds != null)
                {
                    using (DataClassesDataContext DataContext = GetDataContext())
                    {
                        newModel.Count = ds.Tables[0].Rows.Count;
                        DataContext.GS1DIExcelRecord.InsertOnSubmit(newModel);
                        DataContext.SubmitChanges();
                        StringBuilder strBuilder = new StringBuilder();
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RetResult result = new RetResult();
                            result.CmdError = CmdResultError.EXCEPTION;
                            //DI去重
                            MaterialDI maDIdate = DataContext.MaterialDI.FirstOrDefault(m => m.MaterialName == ds.Tables[0].Rows[i][0].ToString().Trim() &&
                            m.EnterpriseID == newModel.EnterpriseID && m.GSIDI == ds.Tables[0].Rows[i][1].ToString().Trim() &&
                            m.Status == (int)Common.EnumFile.Status.used);
                            if (maDIdate == null)
                            {
                                MaterialDI DItemp = new MaterialDI();
                                #region  赋值
                                DItemp.MaterialName = ds.Tables[0].Rows[i][0].ToString().Trim();
                                DItemp.GSIDI = ds.Tables[0].Rows[i][1].ToString().Trim();
                                if (DItemp.GSIDI.Length > 14 || DItemp.GSIDI.Length < 13)
                                {
                                    Ret.Msg = "导入DI信息长度有错误！";
                                    Ret.CmdError = CmdResultError.EXCEPTION;
                                    return Ret;
                                }
                                else if (DItemp.GSIDI.Length <= 14)
                                {
                                    DItemp.GSIDI = PrefixInteger(DItemp.GSIDI, 14);
                                }
                                DItemp.CategoryCode = ds.Tables[0].Rows[i][2].ToString().Trim();
                                DItemp.Specifications = ds.Tables[0].Rows[i][3].ToString().Trim();
                                DItemp.SpecificationName = ds.Tables[0].Rows[i][4].ToString().Trim();
                                DItemp.MaterialXH = ds.Tables[0].Rows[i][5].ToString().Trim();
                                DItemp.SpecLevel = ds.Tables[0].Rows[i][6].ToString().Trim();
                                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][7].ToString().Trim()))
                                {
                                    bool isnum = IsNumberic(ds.Tables[0].Rows[i][7].ToString().Trim());
                                    if (isnum)
                                    {
                                        DItemp.SpecNum = Convert.ToInt32(ds.Tables[0].Rows[i][7]);
                                    }
                                    else
                                    {
                                        Ret.Msg = "导入DI信息数据格式有错误，请检验！";
                                        Ret.CmdError = CmdResultError.EXCEPTION;
                                        return Ret;
                                    }
                                }
                                else
                                {
                                    DItemp.SpecNum = 0;
                                }
                                DItemp.HisCode = ds.Tables[0].Rows[i][8].ToString().Trim();
                                DItemp.EnterpriseID = newModel.EnterpriseID;
                                DItemp.adddate = DateTime.Now;
                                DItemp.adduser = newModel.AddUser;
                                DItemp.Status = (int)Common.EnumFile.Status.used;
                                DItemp.createtype = 5;//excel上传的
                                DItemp.ISUpload = 1;
                                Material matemp = DataContext.Material.Where(p => p.MaterialName == DItemp.MaterialName && p.Status == (int)Common.EnumFile.Status.used && p.Enterprise_Info_ID == newModel.EnterpriseID).FirstOrDefault();
                                if (matemp != null)
                                {
                                    DItemp.MaterialID = matemp.Material_ID;
                                }
                                #endregion
                                else
                                {
                                    matemp = new Material();
                                    matemp.MaterialName = DItemp.MaterialName;
                                    matemp.MaterialFullName = DItemp.MaterialName;
                                    matemp.Status = (int)Common.EnumFile.Status.used;
                                    matemp.adddate = DateTime.Now;
                                    matemp.adduser = newModel.AddUser;
                                    matemp.Enterprise_Info_ID = newModel.EnterpriseID;
                                    DataContext.Material.InsertOnSubmit(matemp);
                                    DataContext.SubmitChanges();
                                    DItemp.MaterialID = matemp.Material_ID;
                                }
                                DataContext.MaterialDI.InsertOnSubmit(DItemp);
                                DataContext.SubmitChanges();
                            }
                        }
                        Ret.Msg = strBuilder.ToString();
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "MaterialDAL.Edit()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Ret;
        }
        public string PrefixInteger(string num, int length)
        {
            string result = Math.Round((Convert.ToInt64(num) / Math.Pow(10, length)), length).ToString().Substring(2);
            if (num.Length == 14 && result.Length < 14)
            {
                result = result.PadRight(14, '0');
            }
            return result;
            //return (Convert.ToInt64(num) / Math.Round(Math.Pow(10, length), length)).ToString().Substring(2);
        }
        private bool IsNumberic(string oText)
        {
            try
            {
                int var1 = Convert.ToInt32(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}