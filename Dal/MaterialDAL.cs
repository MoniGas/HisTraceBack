/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-01

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于产品信息管理数据层

*********************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;
using System;
using Common.Tools;
using System.Xml.Linq;
using MeatTrace.LinqModel;
using InterfaceWeb;
using LinqModel.InterfaceModels;
using System.Data.Common;

namespace Dal
{
    public class MaterialDAL : DALBase
    {
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public List<Material> GetCMaterialList(long enterpriseId)
        {
            List<Material> result = new List<Material>();
            List<Material> material = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    material = dataContext.Material.Where(m => m.Enterprise_Info_ID == enterpriseId &&
                        m.Status == (int)Common.EnumFile.Status.used).OrderByDescending(m => m.Material_ID).ToList();
                    foreach (Material sub in material)
                    {
                        Category category = dataContext.Category.Where(m => m.MaterialID == sub.Material_ID).FirstOrDefault();
                        if (category == null)
                        {
                            result.Add(sub);
                        }
                    }
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取产品模型
        /// </summary>
        /// <param name="materialId">产品标识</param>
        /// <returns>模型</returns>
        public Material GetModel(long materialId)
        {
            Material result = new Material();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material.FirstOrDefault(m => m.Material_ID == materialId && m.Status == (int)Common.EnumFile.Status.used);
                    //查找一级、二级品类
                    if (result != null && result.Dictionary_MaterialType_ID > 0)
                    {
                        result.ThirdType = (long)result.Dictionary_MaterialType_ID;
                        Dictionary_MaterialType type = dataContext.Dictionary_MaterialType.Where(p => p.Dictionary_MaterialType_ID == result.Dictionary_MaterialType_ID).FirstOrDefault();
                        if (type != null && type.Level == 3)
                        {
                            result.SecondType = (long)type.Parent_ID;
                            type = dataContext.Dictionary_MaterialType.Where(p => p.Dictionary_MaterialType_ID == type.Parent_ID).FirstOrDefault();
                            if (type != null && type.Level == 2)
                            {
                                result.FirstType = (long)type.Parent_ID;
                            }
                        }
                    }
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取产品模型
        /// </summary>
        /// <param name="materialId">产品标识</param>
        /// <returns>模型</returns>
        public View_Material GetViewModel(long materialId)
        {
            View_Material result = new View_Material();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.View_Material.FirstOrDefault(m => m.Material_ID == materialId);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 拍码页获取产品信息
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public Material GetMaterial(long materialId)
        {
            Material result = new Material();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material.FirstOrDefault(m => m.Material_ID == materialId && m.Status == (int)Common.EnumFile.Status.used);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetMaterial()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<View_Material> GetList(long enterpriseId, string materialName, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<View_Material> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_Material.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        tempResult = tempResult.Where(m => m.MaterialName.Contains(materialName.Trim()) || m.MaterialFullName.Contains(materialName.Trim()) || m.BrandName.Contains(materialName.Trim()) ||
                            m.CategoryName.Contains(materialName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Material_ID).ToList();
                    // 判断页码大于0为有效页码
                    if (pageIndex > 0)
                    {
                        result = result.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        public List<Material> GetList(long enterpriseId)
        {
            List<Material> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        public List<Material> GetGMList(long enterpriseId, string brandid)
        {
            List<Material> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used).ToList();

                    if (!string.IsNullOrEmpty(brandid))
                    {
                        result = result.Where(w => w.Brand_ID == Convert.ToInt64(brandid)).ToList();
                    }

                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Add(Material model)
        {
            string Msg = "添加产品信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempModel = dataContext.Material.FirstOrDefault(p => p.MaterialName == model.MaterialName && p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                    if (tempModel != null)
                    {
                        Msg = "已存在该产品名称！";
                    }
                    else
                    {
                        model.type = (int)Common.EnumFile.Materialtype.AutoCode;
                        model.MaterialBarcode = GetCode(dataContext);
                        dataContext.Material.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        //返回新增产品编码
                        Msg = "添加产品信息成功";
                        error = CmdResultError.NONE;
                        //为统计表添加数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = model.Enterprise_Info_ID;
                        homeData.MaterialCount = 1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult result = dal.Update(homeData);
                    }
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.Add()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            Ret.CrudCount = model.Material_ID;
            return Ret;
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        //public RetResult AddOld(Material model, string mainCode, Category category, string codeUseID, List<MaterialEvaluation> materialpj, MaterialShopLink shopLink)
        //{
        //    Ret.Msg = "添加产品信息失败！";
        //    Ret.CmdError = CmdResultError.EXCEPTION;
        //    string materialCode = "";
        //    int macodeS = 0;
        //    using (DataClassesDataContext dataContext = GetDataContext())
        //    {
        //        try
        //        {
        //            var tempMaCode = dataContext.Material.FirstOrDefault(p => p.CodeUser == model.CodeUser &&
        //                p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
        //            if (tempMaCode != null)
        //            {
        //                Ret.Msg = "已存在该产品编码！";
        //            }
        //            else
        //            {
        //                int count = dataContext.Material.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID).Count();
        //                if (count > 0)
        //                {
        //                    string lastdataSql = "select top 1 * from Material where Enterprise_Info_ID=" + model.Enterprise_Info_ID
        //                        + "order by Material_ID desc";

        //                    Material lastdata = dataContext.ExecuteQuery<Material>(lastdataSql).FirstOrDefault();
        //                    string macode = lastdata.Material_Code;
        //                    macodeS = new BinarySystem62().Convert62ToNo(macode);
        //                    var datacount = dataContext.Material.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID);
        //                    if (datacount != null)
        //                    {
        //                        foreach (var item in datacount)
        //                        {
        //                            if (macode == item.Material_Code)
        //                            {
        //                                materialCode = new BinarySystem62().gen62No((macodeS + 1), 2);
        //                            }
        //                            else
        //                            {
        //                                materialCode = new BinarySystem62().gen62No((macodeS + 1), 2);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        materialCode = new BinarySystem62().gen62No((macodeS + 1), 2);
        //                    }
        //                }
        //                else
        //                {
        //                    string macode = "01";
        //                    if (mainCode == "i.86.130105.1")
        //                    {
        //                        macode = "13";
        //                    }
        //                    else if (mainCode == "i.86.150426.1")
        //                    {
        //                        macode = "549";
        //                    }
        //                    int codeNum = Convert.ToInt32(macode.Replace("0", ""));
        //                    materialCode = new BinarySystem62().gen62No(codeNum, 2);
        //                }
        //                model.Material_Code = materialCode;
        //                model.type = (int)Common.EnumFile.Materialtype.AutoCode;
        //                model.MaterialBarcode = GetCode(dataContext);
        //                //20180628修改因接口改成加密一块做修改
        //                CategoryList temp = Common.Tools.Public.listCategory.FirstOrDefault(m => m.CategoryID == category.CategoryID.ToString());
        //                if (temp != null)
        //                {
        //                    category.CategoryLevel = temp.CategoryLevel;
        //                    category.CategoryName = temp.CategoryName;
        //                    category.CategoryCode = long.Parse(temp.CategoryCode);
        //                }
        //                if (mainCode == "i.86.130105.1")
        //                {
        //                    mainCode = "i.1.130105.1";
        //                }
        //                string result = BaseDataDAL.Recode(mainCode, category.CategoryID.ToString(),
        //                    category.CategoryCode.GetValueOrDefault(0).ToString(), model.MaterialName, materialCode);
        //                JsonObject jsonObject = new JsonObject(result);
        //                result = jsonObject["ResultCode"].Value;
        //                if (result == "1")
        //                {

        //                    category.CategoryIDcode = jsonObject["OrganUnitIDcode"].Value;
        //                    string[] array = category.CategoryIDcode.Split('/');
        //                    if (array.Length == 3 && category.CategoryIDcode.Length > 0)
        //                    {
        //                        //现在IDcode接口返回的主码新注册带/，查询返回的信息不带/
        //                        category.CategoryIDcode = category.CategoryIDcode;
        //                        string[] categoryArray = array[1].Split('.');
        //                        model.Material_Code = categoryArray[2];
        //                    }
        //                    else if (category.CategoryIDcode.Length == 0)
        //                    {
        //                        category.Status = Convert.ToInt32(Common.EnumFile.Status.delete);
        //                    }
        //                    dataContext.Material.InsertOnSubmit(model);
        //                    dataContext.SubmitChanges();
        //                    category.MaterialID = model.Material_ID;
        //                    //20180417新加简码（配合农药码）
        //                    category.SCategoryIDcode = (model.Material_ID).ToString().PadLeft(6, '0');
        //                    category.MaterialSpcificationCode = model.Material_Code;
        //                    category.MaterialSpcificationName = model.MaterialFullName;
        //                    dataContext.Category.InsertOnSubmit(category);
        //                    dataContext.SubmitChanges();
        //                }
        //                else if (result == "-4")
        //                {
        //                    int i = 1;
        //                    for (i = 1; i <= 100; i = i + 10)
        //                    {
        //                        macodeS = new BinarySystem62().Convert62ToNo(materialCode) + i;
        //                        materialCode = new BinarySystem62().gen62No(macodeS, 3);
        //                        result = BaseDataDAL.Recode(mainCode, category.CategoryID.ToString(),
        //                    category.CategoryCode.GetValueOrDefault(0).ToString(), model.MaterialName, materialCode);
        //                        jsonObject = new JsonObject(result);
        //                        result = jsonObject["ResultCode"].Value;
        //                        if (result == "1")
        //                        {
        //                            category.CategoryIDcode = jsonObject["OrganUnitIDcode"].Value;
        //                            string[] array = category.CategoryIDcode.Split('/');
        //                            if (array.Length == 3 && category.CategoryIDcode.Length > 0)
        //                            {
        //                                //现在IDcode接口返回的主码新注册带/，查询返回的信息不带/
        //                                category.CategoryIDcode = category.CategoryIDcode;
        //                                string[] categoryArray = array[1].Split('.');
        //                                model.Material_Code = categoryArray[2];
        //                            }
        //                            else if (category.CategoryIDcode.Length == 0)
        //                            {
        //                                category.Status = Convert.ToInt32(Common.EnumFile.Status.delete);
        //                            }
        //                            dataContext.Material.InsertOnSubmit(model);
        //                            dataContext.SubmitChanges();
        //                            category.MaterialID = model.Material_ID;
        //                            //20180417新加简码（配合农药码）
        //                            category.SCategoryIDcode = (model.Material_ID).ToString().PadLeft(6, '0');
        //                            category.MaterialSpcificationCode = model.Material_Code;
        //                            category.MaterialSpcificationName = model.MaterialFullName;
        //                            dataContext.Category.InsertOnSubmit(category);
        //                            break;
        //                        }
        //                        else if (result == "-4")
        //                        {
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            break;
        //                        }
        //                    }
        //                    if (i >= 100)
        //                    {
        //                        Ret.Msg = "备案品类码失败！";
        //                        return Ret;
        //                    }
        //                }
        //                else
        //                {
        //                    Ret.Msg = "备案品类码失败！";
        //                    return Ret;
        //                }
        //                //
        //                foreach (var item in materialpj)
        //                {
        //                    item.MaterialID = model.Material_ID;
        //                }
        //                dataContext.MaterialEvaluation.InsertAllOnSubmit(materialpj);
        //                shopLink.MaterialID = model.Material_ID;
        //                dataContext.MaterialShopLink.InsertOnSubmit(shopLink);
        //                //为统计表添加数据
        //                HomeDataStatis homeData = new HomeDataStatis();
        //                homeData.EnterpriseID = model.Enterprise_Info_ID;
        //                homeData.MaterialCount = 1;
        //                ComplaintDAL dal = new ComplaintDAL();
        //                RetResult a = dal.Update(homeData);
        //                dataContext.SubmitChanges();
        //                Ret.CrudCount = model.Material_ID;
        //                Ret.Msg = "添加产品信息成功";
        //                Ret.CmdError = CmdResultError.NONE;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            string errData = "MaterialDAL.Add()";
        //            WriteLog.WriteErrorLog(errData + ":" + ex.Message);
        //        }
        //    }
        //    return Ret;
        //}
        public RetResult Add(Material model, string mainCode, Category category, MaterialShopLink shopLink, string meatCategoryName)
        {
            Ret.Msg = "添加产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            string materialCode = "";
            int macodeS = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    int count = dataContext.Material.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID).Count();
                    if (count > 0)
                    {
                        macodeS = count + 1;
                        materialCode = new BinarySystem36().gen36No((macodeS), 3);
                    }
                    else
                    {
                        string macode = "001";
                        int codeNum = Convert.ToInt32(macode.Replace("0", ""));
                        materialCode = new BinarySystem36().gen36No(codeNum, 3);
                    }
                    model.Material_Code = materialCode;
                    model.type = (int)Common.EnumFile.Materialtype.AutoCode;
                    model.MaterialBarcode = GetCode(dataContext);
                    //20180628修改因接口改成加密一块做修改
                    HisIndustryCategory temp = Common.Tools.Public.listCategory.FirstOrDefault(m => m.industrycategory_id == category.CategoryID);
                    if (temp != null)
                    {
                        category.CategoryLevel = temp.industrycategory_level;
                        category.CategoryName = temp.industrycategory_name;
                        category.CategoryCode = temp.industrycategory_code;
                        if (temp.industrycategory_level == 2)
                        {
                            category.CategoryYiID = temp.industrycategory_id_parent;
                            category.CategoryErID = temp.industrycategory_id;
                        }
                        else
                        {
                            category.CategoryYiID = temp.industrycategory_id;
                            category.CategoryErID = 0;
                        }
                        category.CategoryFullName = meatCategoryName;
                    }
                    dataContext.Material.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    category.MaterialID = model.Material_ID;
                    //20180417新加简码（配合农药码）
                    //category.SCategoryIDcode = (model.Material_ID).ToString().PadLeft(6, '0');
                    category.Material_Code = model.Material_Code;
                    category.MaterialName = model.MaterialName;
                    dataContext.Category.InsertOnSubmit(category);
                    dataContext.SubmitChanges();
                    shopLink.MaterialID = model.Material_ID;
                    dataContext.MaterialShopLink.InsertOnSubmit(shopLink);
                    //为统计表添加数据
                    HomeDataStatis homeData = new HomeDataStatis();
                    homeData.EnterpriseID = model.Enterprise_Info_ID;
                    homeData.MaterialCount = 1;
                    ComplaintDAL dal = new ComplaintDAL();
                    RetResult a = dal.Update(homeData);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.Material_ID;
                    Ret.Msg = "添加产品信息成功";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.Add()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Material newModel, MaterialShopLink shopLink)
        {
            Ret.Msg = "修改产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Material.FirstOrDefault(p => p.Material_ID == newModel.Material_ID &&
                        p.Enterprise_Info_ID == newModel.Enterprise_Info_ID);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的产品！";
                    }
                    else
                    {
                        var a = dataContext.Material.FirstOrDefault(p => p.CodeUser == newModel.CodeUser &&
                            p.Material_ID != newModel.Material_ID && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID &&
                            p.Status == (int)Common.EnumFile.Status.used);
                        if (a == null)
                        {
                            model.MaAttribute = newModel.MaAttribute;
                            model.Brand_ID = newModel.Brand_ID;
                            model.ProcessID = newModel.ProcessID;
                            model.Dictionary_MaterialType_ID = newModel.Dictionary_MaterialType_ID;
                            model.MaterialImgInfo = newModel.MaterialImgInfo;
                            model.MaterialName = newModel.MaterialName;
                            model.MaterialAliasName = newModel.MaterialAliasName;
                            model.MaterialTaste = newModel.MaterialTaste;
                            model.Materialjj = newModel.Materialjj;
                            model.MaterialFullName = newModel.MaterialName;
                            model.Memo = newModel.Memo;
                            model.PropertyInfo = newModel.PropertyInfo;
                            model.ShelfLife = newModel.ShelfLife;
                            model.type = newModel.type;
                            model.CNWORD = newModel.CNWORD;
                            model.tbURL = newModel.tbURL;
                            model.MaterialSpecificationID = newModel.MaterialSpecificationID;
                            model.price = newModel.price;
                            model.MaterialPlace = newModel.MaterialPlace;
                            model.NYType = newModel.NYType;
                            model.NYZhengHao = newModel.NYZhengHao;
                            model.SCLeiXing = newModel.SCLeiXing;
                            //MaterialEvaluation mapj = new MaterialEvaluation();
                            //foreach (var item in materialpj)
                            //{
                            //    item.MaterialID = model.Material_ID;
                            //}
                            //dataContext.MaterialEvaluation.DeleteAllOnSubmit(
                            //    dataContext.MaterialEvaluation.Where(m => m.MaterialID == model.Material_ID)
                            //);
                            //dataContext.MaterialEvaluation.InsertAllOnSubmit(materialpj);
                            var oldShopLike = dataContext.MaterialShopLink.FirstOrDefault(c => c.MaterialID == model.Material_ID);
                            if (oldShopLike == null)
                            {
                                shopLink.MaterialID = model.Material_ID;
                                dataContext.MaterialShopLink.InsertOnSubmit(shopLink);
                            }
                            else
                            {
                                oldShopLike.JingDongLink = shopLink.JingDongLink;
                                oldShopLike.AdUrl = shopLink.AdUrl;
                                oldShopLike.VideoUrl = shopLink.VideoUrl;
                                oldShopLike.TaoBaoLink = shopLink.TaoBaoLink;
                                oldShopLike.TianMaoLink = shopLink.TianMaoLink;
                                oldShopLike.WeiDianLink = shopLink.WeiDianLink;
                            }
                            //Category maCategory = dataContext.Category.FirstOrDefault(m => m.MaterialID == newModel.Material_ID && m.Status == (int)Common.EnumFile.Status.used);
                            //if (maCategory != null && string.IsNullOrEmpty(maCategory.SCategoryIDcode))
                            //{
                            //    maCategory.SCategoryIDcode = (newModel.Material_ID).ToString().PadLeft(6, '0');
                            //}
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改产品信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "已存在该产品编码！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        private static long GetCode(DataClassesDataContext dataContext)
        {
            bool exists = true;
            long? data = dataContext.Material.Where(m => m.type == (int)Common.EnumFile.Materialtype.AutoCode).Max(m => m.MaterialBarcode);
            long code = data == null ? 0 : data.Value;
            code++;
            for (; exists; code++)
            {
                var query = dataContext.Material.FirstOrDefault(m => m.MaterialBarcode == code);
                if (query == null)
                {
                    return code;
                }
            }
            return 1;
        }
        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Material newModel)
        {
            Ret.Msg = "修改产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Material.FirstOrDefault(p => p.Material_ID == newModel.Material_ID && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID);
                    ClearLinqModel(model);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的产品！";
                    }
                    else
                    {
                        //var a = dataContext.Material.FirstOrDefault(p => p.MaterialName == newModel.MaterialName && p.Material_ID != newModel.Material_ID && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                        //if (a == null)
                        //{
                        model.Brand_ID = newModel.Brand_ID;
                        model.Dictionary_MaterialType_ID = newModel.Dictionary_MaterialType_ID;
                        //string imgAndVideo = string.Empty;
                        //XElement xml = new XElement("infos");
                        //IEnumerable<XElement> videoXml;
                        //if (!string.IsNullOrEmpty(model.StrMaterialImgInfo))
                        //{
                        //    videoXml = model.MaterialImgInfo.Elements("video");
                        //}
                        //else
                        //{
                        //    videoXml = newModel.MaterialImgInfo.Elements("video");
                        //}
                        //xml.Add(newModel.MaterialImgInfo.Elements("img"));
                        //xml.Add(videoXml);
                        //model.MaterialImgInfo = xml;
                        model.MaterialImgInfo = newModel.MaterialImgInfo;
                        model.MaterialName = newModel.MaterialName;
                        model.MaterialFullName = newModel.MaterialName;
                        model.Memo = newModel.Memo;
                        model.PropertyInfo = newModel.PropertyInfo;
                        model.ShelfLife = newModel.ShelfLife;
                        model.type = newModel.type;
                        model.CNWORD = newModel.CNWORD;
                        model.MaterialSpecification = newModel.MaterialSpecification;
                        model.price = newModel.price;
                        model.ProcessID = newModel.ProcessID;
                        model.MaterialSpecificationID = newModel.MaterialSpecificationID;
                        model.tbURL = newModel.tbURL;
                        dataContext.SubmitChanges();
                        Ret.Msg = "修改产品信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                        //}
                        //else
                        //{
                        //    Ret.Msg = "已存在该产品名称！";
                        //}
                    }
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="materialId">产品标识</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long enterpriseId, long materialId)
        {
            Ret.Msg = "删除产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Material.FirstOrDefault(p => p.Material_ID == materialId && p.Enterprise_Info_ID == enterpriseId);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要删除的产品！";
                    }
                    else
                    {
                        //int batch = dataContext.Batch.Where(m => m.Material_ID == materialId && m.Status == (int)Common.EnumFile.Status.used).Count();
                        //int area = dataContext.Brand_Enterprise.Where(m => m.Material_ID == materialId).Count();
                        //int code = dataContext.RequestCode.Where(m => m.Material_ID == materialId).Count();
                        //if (batch > 0 || area > 0 || code > 0)
                        //{
                        //    Ret.Msg = "该产品已被使用，目前无法删除！";
                        //}
                        //else
                        //{
                        //model.Status = (int)Common.EnumFile.Status.delete;
                        //dataContext.SubmitChanges();
                        //Ret.Msg = "删除产品信息成功！";
                        //Ret.CmdError = CmdResultError.NONE;
                        //}
                        model.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        //为统计表删除数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = enterpriseId;
                        homeData.MaterialCount = -1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult a = dal.Update(homeData);
                        dataContext.SubmitChanges();
                        // Ret.CrudCount = materialId;
                        Ret.Msg = "删除产品信息成功";
                        Ret.CmdError = CmdResultError.NONE;
                        //Ret.Msg = "删除产品信息成功！";
                        //Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.Del()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 获取一级产品分类
        /// </summary>
        /// <returns></returns>
        public List<LinqModel.Dictionary_MaterialType> GetMaterialType(int level, long parent)
        {
            List<LinqModel.Dictionary_MaterialType> list = null;
            using (DataClassesDataContext dataContenxt = GetDataContext())
            {
                list = dataContenxt.Dictionary_MaterialType.Where(p => p.Level == level
                    && p.Status == (int)Common.EnumFile.Status.used
                    && p.Parent_ID == parent).ToList();
            }
            return list;
        }

        /// <summary>
        /// 查找产品类别
        /// </summary>
        /// <param name="name">类别名称</param>
        /// <returns></returns>
        public List<ToJsonProperty> SearchType(string name)
        {
            List<ToJsonProperty> value = new List<ToJsonProperty>();
            List<LinqModel.Dictionary_MaterialType> list = null;
            using (DataClassesDataContext dataContenxt = GetDataContext())
            {
                list = dataContenxt.Dictionary_MaterialType.Where(p => p.Level == 3
                    && p.Status == (int)Common.EnumFile.Status.used
                    && p.MaterialTypeName.Contains(name)).ToList();
                foreach (var sub in list)
                {
                    ToJsonProperty property = new ToJsonProperty();
                    Dictionary_MaterialType second = dataContenxt.Dictionary_MaterialType.Where(p => p.Dictionary_MaterialType_ID == sub.Parent_ID).FirstOrDefault();
                    Dictionary_MaterialType first = dataContenxt.Dictionary_MaterialType.Where(p => p.Dictionary_MaterialType_ID == second.Parent_ID).FirstOrDefault();
                    property.pName = first.MaterialTypeName + ">>" + second.MaterialTypeName + ">>" + sub.MaterialTypeName;
                    property.pValue = first.Dictionary_MaterialType_ID + "," + second.Dictionary_MaterialType_ID + "," + sub.Dictionary_MaterialType_ID;
                    value.Add(property);
                }
            }
            return value;
        }

        /// <summary>
        /// 获取产品评价
        /// </summary>
        /// <param name="materialId">产品ID</param>
        /// <returns></returns>
        public List<MaterialEvaluation> GetMaPJ(long materialId)
        {
            List<MaterialEvaluation> result = new List<MaterialEvaluation>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.MaterialEvaluation.Where(m => m.MaterialID == materialId &&
                        m.Status == (int)Common.EnumFile.Status.used).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 产品码导出Excel
        /// </summary>
        /// <param name="eID">企业ID</param>
        /// <returns></returns>
        public List<View_Material> ExportExcel(long eID, string searchName)
        {
            List<View_Material> result = new List<View_Material>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_Material.Where(m => m.Enterprise_Info_ID == eID && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(searchName) && searchName != "undefined")
                    {
                        data = data.Where(m => m.MaterialName.Contains(searchName.Trim()) || m.MaterialFullName.Contains(searchName.Trim()) || m.BrandName.Contains(searchName.Trim()) ||
                            m.CategoryName.Contains(searchName.Trim()));
                    }
                    result = data.OrderByDescending(m => m.Material_ID).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 添加导出记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MaterialExportExcelRecord AddExcelRecord(MaterialExportExcelRecord model, int count)
        {
            MaterialExportExcelRecord modelExcel = new MaterialExportExcelRecord();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    model.MaCount = count;
                    dataContext.MaterialExportExcelRecord.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            return modelExcel;
        }

        /// <summary>
        /// 插临时表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<MaterialTemp> AddMaterialTemp(List<MaterialTemp> model)
        {
            List<MaterialTemp> modelList = new List<MaterialTemp>();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.MaterialTemp.InsertAllOnSubmit(model);
                    dataContext.SubmitChanges();
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            return modelList;
        }

        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult AddExcelR(MaterialExportExcelRecord model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                MaterialExportExcelRecord newModel = new MaterialExportExcelRecord();
                try
                {
                    dataContext.MaterialExportExcelRecord.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "导入成功");
                }
                catch
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "导入失败");
                }
            }
            return Ret;
        }

        /// <summary>
        /// 更新记录表中数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MaterialExportExcelRecord UpdataCount(long rID, MaterialExportExcelRecord model)
        {
            MaterialExportExcelRecord newModel = new MaterialExportExcelRecord();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.MaterialExportExcelRecord.FirstOrDefault(m => m.ID == rID);
                    if (data == null)
                    {
                        Ret.Msg = "没有找到要修改的数据！";
                    }
                    else
                    {
                        data.MaCount = model.MaCount;
                        dataContext.SubmitChanges();
                    }
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            return newModel;
        }

        /// <summary>
        /// 获取待审核列表（导入的Excel）
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<MaterialExportExcelRecord> GetExcelRecord(long enterpriseId, out long totalCount, int pageIndex)
        {
            List<MaterialExportExcelRecord> result = new List<MaterialExportExcelRecord>();
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.MaterialExportExcelRecord.Where(m => m.EnterpriseID == enterpriseId);
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.GetExcelRecord():MaterialExportExcelRecord表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 审核导入Excel
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></ret urns>
        public RetResult AuditExcel(long eId, long id)
        { 
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    MaterialExportExcelRecord er = dataContext.MaterialExportExcelRecord.FirstOrDefault(m => m.EnterpriseID == eId && m.ID == id &&
                        m.Status == (int)Common.EnumFile.Status.unaudited);
                    if (er != null)
                    {
                        er.Status = (int)Common.EnumFile.Status.audited;
                        dataContext.SubmitChanges();
                        var data = dataContext.MaterialTemp.Where(m => m.Enterprise_Info_ID == eId && m.MaterialExportExcelRecordID == id &&
                            m.Status == (int)Common.EnumFile.Status.unaudited);
                        if (data != null && data.Count() > 0)
                        {
                            foreach (var item in data)
                            {
                                item.Status = (int)Common.EnumFile.Status.audited;
                                dataContext.SubmitChanges();
                                Brand tempbr = dataContext.Brand.FirstOrDefault(m => m.Brand_ID == item.Brand_ID &&
                                    m.Enterprise_Info_ID == item.Enterprise_Info_ID && m.Status == (int)Common.EnumFile.Status.unaudited);
                                if (tempbr != null)
                                {
                                    tempbr.Status = (int)Common.EnumFile.Status.used;
                                    dataContext.SubmitChanges();
                                }
                                MaterialSpcification tempmas = dataContext.MaterialSpcification.FirstOrDefault(m => m.MaterialSpcificationID == item.MaterialSpecificationID &&
                                   m.Enterprise_Info_ID == item.Enterprise_Info_ID && m.Status == (int)Common.EnumFile.Status.unaudited);
                                if (tempmas != null)
                                {
                                    tempmas.Status = (int)Common.EnumFile.Status.used;
                                    dataContext.SubmitChanges();
                                }
                                Material tempma = dataContext.Material.FirstOrDefault(m => m.Material_ID == item.Material_ID && m.Enterprise_Info_ID == item.Enterprise_Info_ID);
                                MaterialShopLink templink = dataContext.MaterialShopLink.FirstOrDefault(m => m.MaterialID == item.Material_ID);
                                if (tempma != null)
                                {
                                    tempma.Brand_ID = item.Brand_ID;
                                    tempma.MaterialAliasName = item.MaterialAliasName;
                                    tempma.CodeUser = item.CodeUser;
                                    tempma.MaAttribute = item.MaAttribute;
                                    tempma.MaterialCategory = item.MaterialCategory;
                                    tempma.MaterialImgInfo = item.MaterialImgInfo;
                                    tempma.Materialjj = item.Materialjj;
                                    tempma.MaterialName = item.MaterialName;
                                    tempma.MaterialPlace = item.MaterialPlace;
                                    tempma.MaterialSpecificationID = item.MaterialSpecificationID;
                                    tempma.MaterialTaste = item.MaterialTaste;
                                    tempma.PropertyInfo = item.PropertyInfo;
                                    tempma.ShelfLife = item.ShelfLife;
                                    tempma.tbURL = item.tbURL;
                                    tempma.Status = (int)Common.EnumFile.Status.used;
                                    dataContext.SubmitChanges();
                                }
                                if (templink != null)
                                {
                                    templink.AdUrl = item.AdUrl;
                                    templink.JingDongLink = item.JingDongLink;
                                    templink.VideoUrl = item.VideoUrl;
                                    templink.TaoBaoLink = item.TaoBaoLink;
                                    templink.TianMaoLink = item.TianMaoLink;
                                }
                                else if (templink == null)
                                {
                                    templink = new MaterialShopLink();
                                    templink.JingDongLink = item.JingDongLink;
                                    templink.AdUrl = item.AdUrl;
                                    templink.VideoUrl = item.VideoUrl;
                                    templink.TaoBaoLink = item.TaoBaoLink;
                                    templink.TianMaoLink = item.TianMaoLink;
                                    templink.AddDate = DateTime.Now;
                                    templink.MaterialID = item.Material_ID;
                                    dataContext.MaterialShopLink.InsertOnSubmit(templink);
                                    dataContext.SubmitChanges();
                                }
                                var datapj = dataContext.MaterialEvaluationTemp.Where(m => m.EnterpriseID == eId && m.MaterialExportExcelRecordID == id &&
                                   m.Status == (int)Common.EnumFile.Status.unaudited && m.MaterialID == item.Material_ID);
                                if (datapj != null && datapj.Count() > 0)
                                {
                                    dataContext.MaterialEvaluation.DeleteAllOnSubmit(dataContext.MaterialEvaluation.Where(m => m.MaterialID == item.Material_ID && m.EnterpriseID == item.Enterprise_Info_ID));
                                    foreach (var itempj in datapj)
                                    {
                                        itempj.Status = (int)Common.EnumFile.Status.audited;
                                        dataContext.SubmitChanges();
                                        MaterialEvaluation maPJ = new MaterialEvaluation { AddDate = DateTime.Now, EnterpriseID = item.Enterprise_Info_ID, EvaluationName = itempj.EvaluationName, MaterialID = item.Material_ID, Status = (int)Common.EnumFile.Status.used };
                                        dataContext.MaterialEvaluation.InsertOnSubmit(maPJ);
                                        dataContext.SubmitChanges();
                                    }
                                }
                            }
                        }
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "审核成功");
                    }
                    else
                    {
                        Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "该条记录已审核通过！");
                    }
                }
                catch
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "审核失败");
                }
            }
            return Ret;
        }

        #region 处理同步数据
        public RetResult UploadDIPrivate(MaterialResponse model)
        {
            WriteLog.WriteErrorLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+"开始上传");
            RetResult ret = new RetResult();
            var materialList = model.materialEntityList.ToList();
            if (materialList.Count <= 0)
            {
                ret.Code = -1;
                ret.Msg = "产品数据为空，无法继续上传";
                WriteLog.WriteErrorLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "上传失败，产品数据为空，无法继续上传");
                return ret;
            }
            else
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    if (dataContext.Connection != null)
                        dataContext.Connection.Open();
                    DbTransaction tran = dataContext.Connection.BeginTransaction();
                    dataContext.Transaction = tran;
                    try
                    {
                        int index = 0;
                        foreach (var item in materialList)
                        {
                            List<Category> CategoryList = new List<Category>();
                            List<MaterialDI> MaterialDIList = new List<MaterialDI>();
                            #region 赋值
                            Material material = new Material();
                            material.MaterialName = item.materialName;
                            material.Enterprise_Info_ID = item.enterpriseInfoID;
                            material.BZSpecType = item.bZSpecType;
                            material.adddate = DateTime.Now;
                            material.type = 0;
                            material.MaterialFullName = item.materialFullName;
                            material.Status = item.status;
                            #endregion
                            //查出对应DI
                            var diList = model.materialdiEntityList.Where(t => t.materialID == item.materialID).ToList();
                            if (diList.Count <= 0)
                            {
                                ret.Code = -1;
                                ret.Msg = "di数据为空，无法继续上传";
                                WriteLog.WriteErrorLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "上传失败，di数据为空，无法继续上传");
                                return ret;
                            }
                            else
                            {
                                
                                foreach (var ditem in diList)
                                {
                                    MaterialDI materialDI = new MaterialDI();
                                    materialDI.adddate = DateTime.Now;
                                    materialDI.adduser = ditem.adduser;
                                    materialDI.EnterpriseID = ditem.enterpriseID;
                                    materialDI.MaterialName = ditem.materialName;
                                    materialDI.MaterialUDIDI = ditem.materialUDIDI;
                                    materialDI.Specifications = ditem.specifications;
                                    materialDI.SpecificationName = ditem.specificationName;
                                    materialDI.Status = ditem.status;
                                    materialDI.MaterialXH = ditem.materialXH;
                                    materialDI.CategoryCode = ditem.categoryCode;
                                    materialDI.SpecLevel = ditem.specLevel;
                                    materialDI.SpecNum = ditem.specNum;
                                    materialDI.HisCode = ditem.hisCode;
                                    materialDI.ISUpload = ditem.iSUpload;
                                    materialDI.createtype = 1;
                                    MaterialDIList.Add(materialDI);
                                }
                            }
                            var caList = model.categoryEntityList.Where(t => t.materialID == item.materialID).ToList();
                            if (caList.Count <= 0)
                            {
                                ret.Code = -1;
                                ret.Msg = "品类数据为空，无法继续上传";
                                WriteLog.WriteErrorLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "上传失败，品类数据为空，无法继续上传");
                                return ret;
                            }
                            
                            foreach (var caitem in caList)
                            {
                                Category category = new Category();
                                category.AddTime = DateTime.Now;
                                category.CategoryCode = caitem.categoryCode;
                                category.Enterprise_Info_ID = caitem.enterpriseInfoID;
                                category.Status = caitem.status;
                                category.MaterialName = caitem.materialName;
                                CategoryList.Add(category);
                            }
                           
                            index++;
                            dataContext.Material.InsertOnSubmit(material);
                            dataContext.MaterialDI.InsertAllOnSubmit(MaterialDIList);
                            dataContext.Category.InsertAllOnSubmit(CategoryList);
                            dataContext.SubmitChanges();
                            WriteLog.WriteErrorLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + item.materialName+"上传成功");
                        }
                        tran.Commit();
                        ret.Code = 1;
                        ret.Msg = "数据同步成功";
                        WriteLog.WriteErrorLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +index+"条数据上传成功");
                        return ret;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        ret.Code = -1;
                        ret.Msg = "程序出错";
                        return ret;
                    }
                }

            }

        }
        #endregion

        /// <summary>
        /// 获取企业产品列表
        /// 陈志钢  WinCE
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <returns></returns>
        public List<View_WinCe_MaterialInfo> getMaterialLst(long enterpriseID)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.View_WinCe_MaterialInfo.Where(p => p.Enterprise_Info_ID == enterpriseID).ToList();
            }
        }

        /// <summary> 
        /// 添加绑定记录 
        /// 陈志钢  WInCE 
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public bool addBindRecord(List<LinqModel.BindCodeRecords> lst)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.BindCodeRecords.InsertAllOnSubmit(lst);
                    dataContext.SubmitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        #region  刘晓杰于2019年11月48日从CFBack项目移入此

        #region  对接友高接口
        /// <summary>
        /// 获取产品信息列表
        /// </summary>
        /// <param name="enterpriseId">企业ID 由于该系统授权沙漠之花独自使用默认企业ID为2</param>
        /// <returns></returns>
        public List<View_Material> GetMaterialList(long enterpriseId)
        {
            List<View_Material> list = new List<View_Material>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    list = dataContext.View_Material.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    ClearLinqModel(list);
                }
                catch (Exception ex)
                {
                    string errData = "对接友高接口MaterialDAL.GetMaterialList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return list;
        }
        #endregion

        #endregion

        #region 2020-1-8 赵慧敏 打码客户端添加产品接口
        /// <summary>
        /// 打码客户端添加产品接口
        /// 2021-4-21 改
        /// 2021-11-11 改成和服务逻辑一样，不同包装规格DI共用一个产品共用
        /// </summary>
        /// <param name="model"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public RetResult AddMaterial(Material model, Category category, MaterialDI modelDI)
        {
            Ret.Msg = "添加产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            string materialCode = "";
            int macodeS = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    //先判断MaterialDI是否存在，如果存在更新，不存在新增
                    MaterialDI diOld = dataContext.MaterialDI.Where(p => p.EnterpriseID == model.Enterprise_Info_ID && p.MaterialUDIDI == modelDI.MaterialUDIDI).FirstOrDefault();
                    if (diOld != null)
                    {
                        Ret.Msg = "添加产品信息成功";
                        Ret.CmdError = CmdResultError.NONE;
                        //return Ret;
                    }
                    //判断是否是新添加的产品
                    Material madate = dataContext.Material.FirstOrDefault(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID && m.MaterialName == model.MaterialName && m.Status == (int)Common.EnumFile.Status.used);
                    if (madate != null)
                    {
                        //如果不是新添加的产品，则是同产品下新规格的产品
                        modelDI.MaterialID = madate.Material_ID;
                        category.MaterialID = madate.Material_ID;
                    }
                    else
                    {
                        int count = dataContext.Material.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID).Count();
                        if (count > 0)
                        {
                            macodeS = count + 1;
                            materialCode = new BinarySystem36().gen36No((macodeS), 3);
                        }
                        else
                        {
                            string macode = "001";
                            int codeNum = Convert.ToInt32(macode.Replace("0", ""));
                            materialCode = new BinarySystem36().gen36No(codeNum, 3);
                        }
                        model.Material_Code = materialCode;
                        model.type = (int)Common.EnumFile.Materialtype.AutoCode;
                        model.MaterialBarcode = GetCode(dataContext);
                        if (diOld == null)
                        {
                            dataContext.Material.InsertOnSubmit(model);
                            dataContext.SubmitChanges();
                        }
                        modelDI.MaterialID = model.Material_ID;
                        category.MaterialID = model.Material_ID;
                    }
                    //加DI
                    modelDI.CategoryCode = category.CategoryCode;
                    if (diOld == null)
                    {
                        dataContext.MaterialDI.InsertOnSubmit(modelDI);
                        dataContext.Category.InsertOnSubmit(category);
                    }
                    dataContext.SubmitChanges();
                    //为统计表添加数据
                    HomeDataStatis homeData = new HomeDataStatis();
                    homeData.EnterpriseID = model.Enterprise_Info_ID;
                    homeData.MaterialCount = 1;
                    ComplaintDAL dal = new ComplaintDAL();
                    RetResult a = dal.Update(homeData);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.Material_ID;
                    Ret.Msg = "添加产品信息成功";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.AddMaterial()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        /// <summary>
        /// 打码客户端获取DI信息
        /// </summary>
        /// <param name="context"></param>
        public List<MaterialDI> GetDIList(long enterpriseId, string date)
        {
            List<MaterialDI> list = new List<MaterialDI>();
            WriteLog.WriteErrorLog("--------" + enterpriseId + "开始------------");
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    if (!string.IsNullOrEmpty(date))
                    {
                        list = dataContext.MaterialDI.Where(m => m.EnterpriseID == enterpriseId
                            && m.adddate >= Convert.ToDateTime(date)).ToList();
                        WriteLog.WriteErrorLog("MaterialDI 1303 查询");
                    }
                    else
                    {
                        list = dataContext.MaterialDI.Where(m => m.EnterpriseID == enterpriseId).ToList();
                        WriteLog.WriteErrorLog("MaterialDI 1309 查询");
                    }
                    ClearLinqModel(list);
                }
                catch (Exception ex)
                {
                    string errData = "打码客户端获取同步DI数据";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return list;
        }
        #endregion
    }
}
