/********************************************************************************
** 添加： 李子巍
** 创始时间：2016-08-02
** 联系方式 :13313318725
** 描述：主要用于品类管理的数据访问层
****************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;
using MeatTrace.LinqModel;
using Common.Tools;

namespace Dal
{
    /// <summary>
    /// 主要用于品类管理的数据访问层
    /// </summary>
    public class CategoryDAL : DALBase
    {
        /// <summary>
        /// 获取品类码
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>品类码列表</returns>
        public List<Category> GetList(long enterpriseId, out long totalCount, int pageIndex)
        {
            List<Category> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Category.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "CategoryDAL.GetList():Category表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取品类码
        /// </summary>
        /// <returns>品类码列表</returns>
        public List<CategoryList> GetList(string jsonStr)
        {
            List<CategoryList> result = new List<CategoryList>();
            try
            {
                JsonObject jsonObject = new JsonObject(jsonStr);
                for (int i = 0; i < jsonObject["IndustryCategoryList"].Count; i++)
                {
                    CategoryList sub = new CategoryList();
                    sub.CategoryID = jsonObject["IndustryCategoryList"][i]["IndustryCategory_ID"].Value;
                    sub.CategoryCode = jsonObject["IndustryCategoryList"][i]["IndustryCategoryCode"].Value;
                    sub.PrentID = jsonObject["IndustryCategoryList"][i]["IndustryCategory_ID_Parent"].Value;
                    sub.CategoryName = jsonObject["IndustryCategoryList"][i]["IndustryCategoryName"].Value;
                    sub.CategoryLevel = int.Parse(jsonObject["IndustryCategoryList"][i]["IndustryCategoryLevel"].Value);
                    sub.CodeUseID = int.Parse(jsonObject["IndustryCategoryList"][i]["CodeUse_ID"].Value);
                    result.Add(sub);
                }
                //List<CategoryList> aa = result.Where(p => p.PrentID == "10214").ToList();
            }
            catch (Exception e)
            {
                Common.Log.WriteLog.WriteErrorLog("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 注册品类
        /// </summary>
        /// <param name="model">品类实体</param>
        /// <returns>操作结果</returns>
        //public RetResult Add(Category model, Material material)
        //{
        //    string msg = "注册品类失败！";
        //    CmdResultError error = CmdResultError.EXCEPTION;
        //    string sCategoryCode = string.Empty;
        //    try
        //    {
        //        using (DataClassesDataContext dataContext = GetDataContext())
        //        {
        //            Category temp = dataContext.Category.FirstOrDefault(m => m.CategoryCode == model.CategoryCode
        //                && m.Enterprise_Info_ID == model.Enterprise_Info_ID && m.MaterialSpcificationCode == model.MaterialSpcificationCode);
        //            if (temp == null)
        //            {
        //                string[] array = model.CategoryIDcode.Split('/');
        //                if (array.Length == 3 && model.CategoryIDcode.Length > 0)
        //                {
        //                    //现在IDcode接口返回的主码新注册带/，查询返回的信息不带/
        //                    model.CategoryIDcode = model.CategoryIDcode;
        //                }
        //                else if (model.CategoryIDcode.Length == 0)
        //                {
        //                    model.Status = Convert.ToInt32(Common.EnumFile.Status.delete);
        //                }
        //                //找品类对应的简码
        //                string[] categoryArray = array[1].Split('.');
        //                //string sCategory = Common.Argument.BaseData.categoryScode[categoryArray[0] + "." + categoryArray[1]];
        //                //if (sCategory.Length == 0 || sCategory == null)
        //                //{
        //                //    msg = "未找到品类对应的简码！";
        //                //    Ret.SetArgument(error, msg, msg);
        //                //    return Ret;
        //                //}
        //                //sCategoryCode = sCategoryCode + "/" + sCategory + "." + categoryArray[2] + "/";
        //                //model.SCategoryIDcode = sCategoryCode;
        //                model.MaterialID = material.Material_ID;
        //                model.MaterialSpcificationCode = material.Material_Code;
        //                model.MaterialSpcificationName = material.MaterialFullName;
        //                //20180417新加简码（配合农药码）
        //                model.SCategoryIDcode = (material.Material_ID).ToString().PadLeft(6, '0');
        //                dataContext.Category.InsertOnSubmit(model);
        //                dataContext.SubmitChanges();
        //                error = CmdResultError.NONE;
        //                msg = "备案成功！";
        //            }
        //            else if (temp != null && temp.MaterialID == null)
        //            {
        //                msg = "该品类已经备案！";
        //            }
        //            else
        //            {
        //                msg = "该型号编码已经使用，请重新输入型号编码！";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string errData = "CategoryDAL.Add()";
        //        WriteLog.WriteErrorLog(errData + ":" + ex.Message);
        //    }
        //    Ret.SetArgument(error, msg, msg);
        //    return Ret;
        //}

        /// <summary>
        /// 判断是否已经存在
        /// </summary>
        /// <param name="modelNumber">型号名称</param>
        /// <returns>结果</returns>
        public RetResult IsExist(long catelgoryId, long eid, long materialid)
        {
            string msg = "";
            CmdResultError error = CmdResultError.NONE;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var data = dataContext.Category.FirstOrDefault(m => m.CategoryID == catelgoryId && m.Enterprise_Info_ID == eid
                    && m.MaterialID == materialid);
                if (data != null)
                {
                    msg = "该品类已经备案！";
                    error = CmdResultError.EXCEPTION;
                }
            }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 备案品类码
        /// </summary>
        /// <param name="mainCode">企业主码</param>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="codeUseID">用途标识</param>
        /// <param name="categoryID">品类标识</param>
        /// <param name="categoryCode">品类编码</param>
        /// <param name="modelNumber">型号名称</param>
        /// <param name="modelNumberCode">型号编码</param>
        /// <param name="resultModel">保存备案信息</param>
        /// <returns>操作结果</returns>
        //public RetResult RecordCode(string mainCode, long enterpriseId, string codeUseID, Category resultModel, string materialName, string materialCode)
        //{
        //    string msg = "备案品类码失败！";
        //    CmdResultError error = CmdResultError.EXCEPTION;
        //    try
        //    {
        //        CategoryList temp = Common.Tools.Public.listCategory.FirstOrDefault(m =>
        //                m.CategoryID == resultModel.CategoryID.ToString());
        //        if (temp != null)
        //        {
        //            resultModel.CategoryLevel = temp.CategoryLevel;
        //            resultModel.CategoryName = temp.CategoryName;
        //            resultModel.CategoryCode = long.Parse(temp.CategoryCode);
        //        }
        //        if (mainCode == "i.86.130105.1")
        //        {
        //            mainCode = "i.1.130105.1";
        //        }
        //        //mainCode = "i.86.130105.1112";
        //        //string mailcodeIdcode = mainCode.Substring(0, mainCode.LastIndexOf("."));
        //        string action = ConfigurationManager.AppSettings["RecordCode"];
        //        string access_token = ConfigurationManager.AppSettings["access_token"];
        //        string parseUrl = ConfigurationManager.AppSettings["IpRedirect"];
        //        Dictionary<string, string> paras = new Dictionary<string, string>();
        //        paras.Add("access_token", access_token);
        //        paras.Add("companyIDcode", mainCode);
        //        paras.Add("codeUse_ID", "10");// codeUseID);
        //        paras.Add("industryCategory_ID", resultModel.CategoryID.ToString());
        //        paras.Add("categoryCode", resultModel.CategoryCode.GetValueOrDefault(0).ToString());
        //        paras.Add("modelNumber", materialName);
        //        paras.Add("modelNumberCode", materialCode);
        //        paras.Add("codePayType", "5");
        //        paras.Add("goToUrl", parseUrl + "Wap_Index/Index");
        //        string strBack = APIHelper.sendPost(action, paras, "get");
        //        JsonObject jsonObject = new JsonObject(strBack);
        //        string result = jsonObject["ResultCode"].Value;
        //        if (result == "1")
        //        {
        //            resultModel.CategoryIDcode = jsonObject["OrganUnitIDcode"].Value;
        //            //msg = "备案品类码成功！";
        //            error = CmdResultError.NONE;
        //            msg = "1";
        //        }
        //        else if (result == "-4")
        //        {
        //            msg = "-4";
        //        }
        //        Ret.SetArgument(error, msg, msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errData = "CategoryDAL.RecordCode()";
        //        WriteLog.WriteErrorLog(errData + ":" + ex.Message);
        //    }
        //    Ret.SetArgument(error, msg, msg);
        //    return Ret;
        //}

        /// <summary>
        /// 获取品类信息
        /// </summary>
        /// <param name="mainCode">企业主码</param>
        /// <param name="model">品类实体</param>
        private string GetRecordCode(string mainCode, string categoryCode, string ModelNumberCode)
        {
            string categoryIDCode = "";
            try
            {
                string action = ConfigurationManager.AppSettings["GetRecordCode"];
                string access_token = ConfigurationManager.AppSettings["access_token"];
                Dictionary<string, string> paras = new Dictionary<string, string>();
                paras.Add("access_token", access_token);
                paras.Add("companyIDcode", mainCode);
                paras.Add("SearchType", "0");
                string strBack = APIHelper.sendPost(action, paras, "post");
                JsonObject jsonObject = new JsonObject(strBack);
                for (int i = 0; i < jsonObject["BaseIDcodeList"].Count; i++)
                {
                    if (categoryCode == jsonObject["BaseIDcodeList"][i]["CategoryCode"].Value
                        && ModelNumberCode == jsonObject["BaseIDcodeList"][i]["ModelNumberCode"].Value)
                    {
                        categoryIDCode = jsonObject["BaseIDcodeList"][i]["CompleteCode"].Value;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "CategoryDAL.GetRecordCode()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return categoryIDCode;
        }

        /// <summary>
        /// 获取规格列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <returns></returns>
        public List<MaterialSpcification> GetList(long enterpriseId)
        {
            List<MaterialSpcification> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.MaterialSpcification.Where(m => m.Enterprise_Info_ID == enterpriseId
                        && m.Status == (int)Common.EnumFile.Status.used).OrderByDescending(m => m.MaterialSpcificationID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "CategoryDAL.GetList():MaterialSpcification表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取品类列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public List<Category> GetCategoryList(long enterpriseId)
        {
            List<Category> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Category.Where(m => m.Enterprise_Info_ID == enterpriseId
                        && m.Status == (int)Common.EnumFile.Status.used).OrderByDescending(m => m.CategoryID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "CategoryDAL.GetCategoryList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取规格型号
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public string GetMaSpcCode(long enterpriseId)
        {
            long specCode = 0;
            string result = "";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    specCode = dataContext.Category.Where(m => m.Enterprise_Info_ID == enterpriseId
                        && m.Status == (int)Common.EnumFile.Status.used).Count() + 1;
                }
                catch (Exception ex)
                {
                    string errData = "CategoryDAL.GetMaSpcCode()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
                result = specCode.ToString().PadLeft(4, '0');
            }
            return result;
        }

        /// <summary>
        /// 获取简码
        /// </summary>
        /// <param name="cIDCode">全码</param>
        /// <returns></returns>
        public Category GetQCode(string cIDCode)
        {
            Category model = new Category();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    model = dataContext.Category.FirstOrDefault(m => m.CategoryIDcode == cIDCode);
                    ClearLinqModel(model);
                }
                catch (Exception ex)
                {
                    string errData = "CategoryDAL.GetQCode()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return model;
        }

        /// <summary>
        /// 获取拍码次数
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public int GetCodeCount(string ewm)
        {
            int count = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                count = dataContext.ScanEwm.Where(m => m.EWM == ewm).Count();
            }
            return count;
        }

        #region 医疗器械
        /// <summary>
        /// 获取医疗器械品类
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>品类码列表</returns>
        public List<HisIndustryCategory> GetHisCategoryList(string jsonStr)
        {
            List<HisIndustryCategory> result = new List<HisIndustryCategory>();
            try
            {
                JsonObject jsonObject = new JsonObject(jsonStr);
                for (int i = 0; i < jsonObject["industrycategory_list"].Count; i++)
                {
                    HisIndustryCategory sub = new HisIndustryCategory();
                    sub.industrycategory_id =Convert.ToInt32(jsonObject["industrycategory_list"][i]["industrycategory_id"].Value);
                    sub.industrycategory_code = jsonObject["industrycategory_list"][i]["industrycategory_code"].Value;
                    sub.industrycategory_id_parent = Convert.ToInt32(jsonObject["industrycategory_list"][i]["industrycategory_id_parent"].Value);
                    sub.industrycategory_name = jsonObject["industrycategory_list"][i]["industrycategory_name"].Value;
                    sub.industrycategory_level = int.Parse(jsonObject["industrycategory_list"][i]["industrycategory_level"].Value);
                    sub.codeuse_id = int.Parse(jsonObject["industrycategory_list"][i]["codeuse_id"].Value);
                    result.Add(sub);
                }
                //List<CategoryList> aa = result.Where(p => p.PrentID == "10214").ToList();
            }
            catch (Exception e)
            {
                Common.Log.WriteLog.WriteErrorLog("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + e.ToString());
            }
            return result;
        }
        #endregion
    }
}
