/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-12-09

** 联系方式 :13313318725

** 描述：主要用于产品转商品数据访问

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using Common.Argument;
using Common.Log;
using Common.Tools;
using System.Xml.Linq;
using System.Data.Common;
using MeatTrace.LinqModel;
using InterfaceWeb;

namespace Dal
{
    public class MaterialPropertyDAL : DALBase
    {
        /// <summary>
        /// 获取商品属性列表
        /// </summary>
        /// <returns>商品属性列表</returns>
        public List<Material_Property> GetMaterialPropertyList()
        {
            List<Material_Property> Result = new List<Material_Property>();
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext())
                {
                    var MaterialPropertyData = DataContext.Material_Property.Where(m => m.Status == (int)Common.EnumFile.Status.used);
                    MaterialPropertyData = MaterialPropertyData.OrderBy(m => m.Group);
                    Result = MaterialPropertyData.ToList();
                }
            }
            catch
            {
            }
            return Result;
        }

        /// <summary>
        /// 获取商品属性列表
        /// </summary>
        /// <param name="MaterialSpecId">产品规格</param>
        /// <returns>商品属性列表</returns>
        public List<View_Material_Property> GetMaterialPropertyList(long MaterialSpecId)
        {
            List<View_Material_Property> Result = new List<View_Material_Property>();
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext())
                {
                    var MaterialPropertyData = DataContext.View_Material_Property.Where(m => m.Status == (int)Common.EnumFile.Status.used && m.Material_Spec_ID == MaterialSpecId);
                    MaterialPropertyData = MaterialPropertyData.OrderBy(m => m.Group);
                    Result = MaterialPropertyData.ToList();
                }
            }
            catch
            {
            }
            return Result;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="ds">数据表</param>
        /// <param name="eId">企业编码</param>
        /// <param name="userId">用户编号</param>
        /// <param name="excelRecordID">导入编号</param>
        /// <returns></returns>
        public RetResult InportExcel(System.Data.DataSet ds, long eId, long userId, long excelRecordID)
        {
            Ret.Msg = "导入产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            try
            {
                if (ds != null)
                {
                    if (Verify(ds).CmdError == CmdResultError.NONE)
                    {
                        using (DataClassesDataContext DataContext = GetDataContext())
                        {
                            StringBuilder strBuilder = new StringBuilder();
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                RetResult result = new RetResult();
                                result.CmdError = CmdResultError.EXCEPTION;
                                //1为修改，2为新增
                                int type = 1;
                                #region  赋值
                                //广告地址
                                string adUrl = ds.Tables[0].Rows[i][14].ToString().Trim();
                                //实时视频
                                string videoUrl = ds.Tables[0].Rows[i][15].ToString().Trim();
                                string taoBaoLink = ds.Tables[0].Rows[i][17].ToString().Trim();
                                string jingDongLink = ds.Tables[0].Rows[i][18].ToString().Trim();
                                string tianMaoLink = ds.Tables[0].Rows[i][19].ToString().Trim();
                                //评价
                                string evaName1 = ds.Tables[0].Rows[i][20].ToString().Trim();
                                string evaName2 = ds.Tables[0].Rows[i][21].ToString().Trim();
                                string evaName3 = ds.Tables[0].Rows[i][22].ToString().Trim();
                                string evaName4 = ds.Tables[0].Rows[i][23].ToString().Trim();
                                string evaName5 = ds.Tables[0].Rows[i][24].ToString().Trim();
                                List<MaterialEvaluation> evaluation = SetEvaluation(evaName1, evaName2, evaName3, evaName4, evaName5, eId);
                                MaterialShopLink shopLink = new MaterialShopLink
                                {
                                    TaoBaoLink = taoBaoLink,
                                    JingDongLink = jingDongLink,
                                    TianMaoLink = tianMaoLink,
                                    StrAdFileUrl = adUrl,
                                    StrVideoUrlInfo = videoUrl,
                                    AddUser = userId,
                                    AddDate = DateTime.Now
                                };
                                MaterialTemp materialTemp = new MaterialTemp();
                                materialTemp.MaterialExportExcelRecordID = excelRecordID;
                                string codeUser = ds.Tables[0].Rows[i][0].ToString().Trim();
                                string materialName = ds.Tables[0].Rows[i][1].ToString().Trim();
                                Material materi = DataContext.Material.Where(p => p.MaterialName == materialName
                                    && p.CodeUser == codeUser && p.Status == (int)Common.EnumFile.Status.used && p.Enterprise_Info_ID == eId).FirstOrDefault();
                                if (materi == null)
                                {
                                    type = 2;
                                }
                                else
                                {
                                    materialTemp.Material_ID = materi.Material_ID;
                                }
                                string materialAliasName = ds.Tables[0].Rows[i][2].ToString().Trim();
                                string brandName = ds.Tables[0].Rows[i][3].ToString().Trim();
                                string categoryName = ds.Tables[0].Rows[i][4].ToString().Trim();
                                //产品类别
                                string maAttribute = ds.Tables[0].Rows[i][5].ToString().Trim();
                                //产品规格
                                string specificationValue = ds.Tables[0].Rows[i][6].ToString().Trim();
                                string specificationName = ds.Tables[0].Rows[i][7].ToString().Trim();
                                string shelfLife = ds.Tables[0].Rows[i][8].ToString().Trim();
                                string MaterialTaste = ds.Tables[0].Rows[i][9].ToString().Trim();
                                //产品属性
                                string propertyInfo = ds.Tables[0].Rows[i][10].ToString().Trim();
                                string materialPlace = ds.Tables[0].Rows[i][11].ToString().Trim();
                                string materialImgInfo = ds.Tables[0].Rows[i][12].ToString().Trim();
                                string materialVideoInfo = ds.Tables[0].Rows[i][13].ToString().Trim();
                                //购买地址
                                string tbURL = ds.Tables[0].Rows[i][16].ToString().Trim();
                                materialTemp.CodeUser = codeUser;
                                materialTemp.MaterialName = materialName;
                                materialTemp.MaterialFullName = materialName;
                                materialTemp.MaterialAliasName = materialAliasName;
                                materialTemp.ShelfLife = shelfLife;
                                materialTemp.MaterialFullName = materialName;
                                materialTemp.MaterialTaste = MaterialTaste;
                                materialTemp.MaterialPlace = materialPlace;
                                materialTemp.Status = (int)Common.EnumFile.Status.unaudited;
                                materialTemp.Enterprise_Info_ID = eId;
                                materialTemp.tbURL = tbURL;
                                materialTemp.adduser = userId;
                                materialTemp.adddate = DateTime.Now;
                                materialTemp.Enterprise_Info_ID = eId;
                                Dictionary<string, string> attribute = SetMaAttribute();
                                foreach (var sub in attribute)
                                {
                                    if (sub.Key == maAttribute)
                                    {
                                        materialTemp.MaAttribute = Convert.ToInt32(sub.Value);
                                        break;
                                    }
                                }
                                XElement xml = new XElement("infos");
                                if (!string.IsNullOrEmpty(materialImgInfo))
                                {
                                    string[] imgList = materialImgInfo.Split(';');
                                    foreach (var sub in imgList)
                                    {
                                        string[] img = sub.Split('&');
                                        xml.Add(new XElement("img", new XAttribute("name", "1.jpg"), new XAttribute("value", img[0]), new XAttribute("small", img[1])));
                                    }
                                }
                                if (!string.IsNullOrEmpty(materialVideoInfo))
                                {
                                    string[] videoList = materialVideoInfo.Split(';');
                                    foreach (var sub in videoList)
                                    {
                                        string[] video = sub.Split('&');
                                        xml.Add(new XElement("video", new XAttribute("name", "1.avi"), new XAttribute("value", video[0]), new XAttribute("small", video[1])));
                                    }
                                }
                                materialTemp.MaterialImgInfo = xml;
                                if (!string.IsNullOrEmpty(propertyInfo))
                                {
                                    XElement propertyXml = new XElement("infos");
                                    string[] propertyList = propertyInfo.Split('-');
                                    foreach (var sub in propertyList)
                                    {
                                        string[] property = sub.Split(':');
                                        if (property.Length == 2)
                                        {
                                            propertyXml.Add(
                                                new XElement("info",
                                                new XAttribute("iname", property[0]),
                                                new XAttribute("ivalue", property[1])
                                                )
                                                );
                                        }
                                    }
                                    materialTemp.PropertyInfo = propertyXml;
                                }
                                #endregion
                                if (type == 1)
                                {
                                    RetResult retResult = Edit(materialTemp, brandName, specificationValue, specificationName, evaluation, shopLink);
                                    if (retResult.CmdError != CmdResultError.NONE)
                                    {
                                        strBuilder.Append("第【" + (i + 1).ToString() + "】行导入产品信息失败," + retResult.Msg);
                                        break;
                                    }
                                }
                                else
                                {
                                    materi = new Material();
                                    materi.MaterialName = materialTemp.MaterialName;
                                    materi.MaterialFullName = materialTemp.MaterialFullName;
                                    materi.CodeUser = materialTemp.CodeUser;
                                    materi.MaterialAliasName = materialTemp.MaterialAliasName;
                                    materi.ShelfLife = materialTemp.ShelfLife;
                                    materi.MaterialTaste = materialTemp.MaterialTaste;
                                    materi.PropertyInfo = materialTemp.PropertyInfo;
                                    materi.MaterialPlace = materialTemp.MaterialPlace;
                                    materi.MaterialImgInfo = materialTemp.MaterialImgInfo;
                                    materi.MaAttribute = materialTemp.MaAttribute;
                                    materi.tbURL = materialTemp.tbURL;
                                    materi.Status = (int)Common.EnumFile.Status.unaudited;
                                    materi.adddate = DateTime.Now;
                                    materi.adduser = userId;
                                    materi.Enterprise_Info_ID = eId;
                                    Dictionary<string, string> dCategory = SetCategoryCode();
                                    string value = string.Empty;
                                    dCategory.TryGetValue(categoryName.Trim(), out value);
                                    int categoryValue = 0;
                                    if (!int.TryParse(value, out categoryValue))
                                    {
                                        strBuilder.Append("第【" + (i + 1).ToString() + "】行导入产品信息失败，没有找到对应的品类信息！");
                                        break;
                                    }
                                    Category category = new Category();
                                    category.AddTime = DateTime.Now;
                                    category.AddUser = userId;
                                    category.CategoryID = long.Parse(categoryValue.ToString());
                                    category.Enterprise_Info_ID = eId;
                                    category.Status = (int)Common.EnumFile.Status.used;
                                    RetResult retResult = Add(materialTemp, materi, brandName, specificationValue, specificationName, category, evaluation, shopLink);
                                    if (retResult.CmdError != CmdResultError.NONE)
                                    {
                                        strBuilder.Append("第【" + (i + 1).ToString() + "】行导入产品信息失败," + retResult.Msg);
                                        break;
                                    }
                                }
                            }
                            Ret.Msg = strBuilder.ToString();
                        }
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

        public List<MaterialEvaluation> SetEvaluation(string pj1, string pjs2, string pj3, string pj4, string pj5, long eId)
        {
            List<MaterialEvaluation> materialpj = new List<MaterialEvaluation>();
            List<string> list = new List<string>();
            list.Add(pj1);
            list.Add(pjs2);
            list.Add(pj3);
            list.Add(pj4);
            list.Add(pj5);
            for (int i = 0; i < list.Count; i++)
            {
                MaterialEvaluation temp = new MaterialEvaluation();
                temp.EvaluationName = list[i];
                temp.EnterpriseID = eId;
                temp.AddDate = DateTime.Now;
                temp.Status = (int)Common.EnumFile.Status.used;
                materialpj.Add(temp);
            }
            return materialpj;
        }

        /// <summary>
        /// 验证导入文档格式是否正确
        /// </summary>
        /// <param name="ds">文档</param>
        /// <returns></returns>
        public RetResult Verify(System.Data.DataSet ds)
        {
            Ret.Msg = "导入格式有问题！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                if (ds != null)
                {
                    Dictionary<string, string> categoryCodeList = new Dictionary<string, string>();
                    Dictionary<string, string> MaAttributeList = new Dictionary<string, string>();
                    categoryCodeList = SetCategoryCode();
                    MaAttributeList = SetMaAttribute();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string codeUser = ds.Tables[0].Rows[i][0].ToString().Trim();
                        string materialName = ds.Tables[0].Rows[i][1].ToString().Trim();
                        if (string.IsNullOrEmpty(materialName))
                        {
                            strBuilder.Append("第【" + i + 1 + "】行中【产品名称不允许为空】");
                        }
                        string materialAliasName = ds.Tables[0].Rows[i][2].ToString().Trim();
                        string brandName = ds.Tables[0].Rows[i][3].ToString().Trim();
                        string categoryName = ds.Tables[0].Rows[i][4].ToString().Trim();
                        if (string.IsNullOrEmpty(categoryName.Trim()))
                        {
                            strBuilder.Append("第【" + i + 1 + "】行中【品类名称不允许为空】");
                        }
                        if (!categoryCodeList.ContainsKey(categoryName.Trim()))
                        {
                            strBuilder.Append("第【" + i + 1 + "】行中【品类名称输入有误】");
                        }
                        string MaAttribute = ds.Tables[0].Rows[i][5].ToString().Trim();
                        if (!string.IsNullOrEmpty(MaAttribute) && !MaAttributeList.ContainsKey(MaAttribute))
                        {
                            strBuilder.Append("第【" + i + 1 + "】行中【产品属性名称输入有误】");
                        }
                        string spectionValue = ds.Tables[0].Rows[i][6].ToString().Trim();
                        string spectionName = ds.Tables[0].Rows[i][7].ToString().Trim();
                        string ShelfLife = ds.Tables[0].Rows[i][8].ToString().Trim();
                        if (ShelfLife.Replace("长期", "") != "" || ShelfLife.Replace("视存储环境", "") != "")
                        {
                            if (ShelfLife.IndexOf("长期") > 0 || ShelfLife.IndexOf("视存储环境") > 0)
                            {
                                strBuilder.Append("第【" + i + 1 + "】行中【保质期输入有误】");
                            }
                            else
                            {
                                if (ShelfLife.Length > 1)
                                {
                                    string date = ShelfLife.Substring(0, ShelfLife.Length - 1);
                                    int intDate = 0;
                                    if (!int.TryParse(date, out intDate) && date != "长" && date != "视存储环")
                                    {
                                        strBuilder.Append("第【" + i + 1 + "】行中【保质期输入有误】");
                                    }
                                }
                            }
                        }
                        //产品属性
                        string PropertyInfo = ds.Tables[0].Rows[i][10].ToString().Trim();
                        if (!string.IsNullOrEmpty(PropertyInfo))
                        {
                            string[] propertyArr = PropertyInfo.Split('-');
                            foreach (var sub in propertyArr)
                            {
                                string[] values = sub.Replace("：", ":").Split(':');
                                if (values.Length != 2)
                                {
                                    strBuilder.Append("第【" + i + 1 + "】行中【产品属性输入有误】");
                                }
                            }
                        }
                        string MaterialImgInfo = ds.Tables[0].Rows[i][12].ToString().Trim();
                        string MaterialVideoInfo = ds.Tables[0].Rows[i][13].ToString().Trim();
                    }
                    if (strBuilder.ToString().Length > 0)
                    {
                        Ret.SetArgument(CmdResultError.PARAMERROR, "", strBuilder.ToString());
                    }
                    else
                    {
                        Ret.SetArgument(CmdResultError.NONE, "", "数据格式正确");
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "MaterialPropertyDAL.Edit()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Ret;
        }

        /// <summary>
        /// 品类
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> SetCategoryCode()
        {
            Dictionary<string, string> categoryScode = new Dictionary<string, string>();
            categoryScode.Add("其他肉类", "6712");//其他肉类
            categoryScode.Add("兔肉", "6709");//兔肉
            categoryScode.Add("鸡肉", "6706");//鸡肉
            categoryScode.Add("鹿肉", "6711");//鹿肉
            categoryScode.Add("驴肉", "6710");//驴肉
            categoryScode.Add("鹅肉", "6708");//鹅肉
            categoryScode.Add("鸭肉", "6707");//鸭肉
            categoryScode.Add("羊肉", "6705");//羊肉
            categoryScode.Add("牛肉", "6704");//牛肉
            categoryScode.Add("猪肉", "6703");//猪肉
            categoryScode.Add("肉制品", "6801");//肉制品
            categoryScode.Add("乳制品", "6803");//乳制品
            categoryScode.Add("醋", "6760");//醋
            categoryScode.Add("果酱", "6755");//果酱
            categoryScode.Add("酱油", "6761");//酱油
            categoryScode.Add("其他调味品", "6763");//其他调味品
            categoryScode.Add("食盐", "6759");//食盐
            categoryScode.Add("调味酱", "6762");//调味酱
            categoryScode.Add("调味香料", "6757");//调味香料
            categoryScode.Add("调味油", "6756");//调味油
            categoryScode.Add("味精，鸡精", "6758");//味精，鸡精
            return categoryScode;
        }

        /// <summary>
        /// 产品类别
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> SetMaAttribute()
        {
            Dictionary<string, string> MaAttribute = new Dictionary<string, string>();
            MaAttribute.Add("标重冻品", "1");
            MaAttribute.Add("抄重冻品", "2");
            MaAttribute.Add("牛肉干", "3");
            MaAttribute.Add("调料", "4");
            MaAttribute.Add("酱卤", "5");
            return MaAttribute;
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Add(MaterialTemp materialTemp, Material model, string brandName, string spectionValue, string spectionName, Category category, List<MaterialEvaluation> materialpj, MaterialShopLink shopLink)
        {
            Ret.Msg = "添加产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            string materialCode = "";
            int macodeS = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == materialTemp.Enterprise_Info_ID).FirstOrDefault();
                    if (enterprise == null)
                    {
                        Ret.Msg = "不存在该企业信息！";
                        return Ret;
                    }
                    var tempMaCode = dataContext.Material.FirstOrDefault(p => p.CodeUser == model.CodeUser &&
                        p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status != (int)Common.EnumFile.Status.delete);
                    if (tempMaCode != null)
                    {
                        Ret.Msg = "产品编码重复，已存在该产品编码:" + tempMaCode.CodeUser + "！";
                    }
                    else
                    {
                        #region 添加品牌和产品规格
                        if (!string.IsNullOrEmpty(brandName))
                        {
                            Brand brand = dataContext.Brand.Where(p => p.BrandName == brandName.Trim() && p.Enterprise_Info_ID == materialTemp.Enterprise_Info_ID
                                && p.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                            if (brand == null)
                            {
                                brand = new Brand();
                                brand.BrandName = brandName;
                                brand.Status = (int)Common.EnumFile.Status.unaudited;
                                brand.Enterprise_Info_ID = materialTemp.Enterprise_Info_ID;
                                brand.adddate = DateTime.Now;
                                dataContext.Brand.InsertOnSubmit(brand);
                                dataContext.SubmitChanges();
                            }
                            materialTemp.Brand_ID = brand.Brand_ID;
                            model.Brand_ID = brand.Brand_ID;
                        }
                        if (!string.IsNullOrEmpty(spectionValue.Trim()) && !string.IsNullOrEmpty(spectionName.Trim()))
                        {
                            MaterialSpcification data = dataContext.MaterialSpcification.Where(m => m.Enterprise_Info_ID == materialTemp.Enterprise_Info_ID
                                && m.Value == Convert.ToDecimal(spectionValue.Trim()) && m.MaterialSpcificationName == spectionName.Trim()
                                && m.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                            if (data != null)
                            {
                                materialTemp.MaterialSpecificationID = data.MaterialSpcificationID;
                                model.MaterialSpecificationID = data.MaterialSpcificationID;
                            }
                            else
                            {
                                MaterialSpcification spection = new MaterialSpcification();
                                spection.Value = Convert.ToDecimal(spectionValue);
                                spection.MaterialSpcificationName = spectionName;
                                spection.Enterprise_Info_ID = materialTemp.Enterprise_Info_ID;
                                spection.Status = (int)Common.EnumFile.Status.unaudited;
                                spection.AddDate = DateTime.Now;
                                dataContext.MaterialSpcification.InsertOnSubmit(spection);
                                dataContext.SubmitChanges();
                                materialTemp.MaterialSpecificationID = spection.MaterialSpcificationID;
                                model.MaterialSpecificationID = spection.MaterialSpcificationID;
                            }
                        }
                        materialTemp.TaoBaoLink = shopLink.TaoBaoLink;
                        materialTemp.TianMaoLink = shopLink.TianMaoLink;
                        materialTemp.JingDongLink = shopLink.JingDongLink;
                        materialTemp.VideoUrl = shopLink.VideoUrl;
                        #endregion
                        int count = dataContext.Material.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID).Count();
                        if (count > 0)
                        {
                            string lastdataSql = "select top 1 * from Material where Enterprise_Info_ID=" + model.Enterprise_Info_ID
                                + "order by Material_ID desc";
                            Material lastdata = dataContext.ExecuteQuery<Material>(lastdataSql).FirstOrDefault();
                            string macode = lastdata.Material_Code;
                            macodeS = new BinarySystem62().Convert62ToNo(macode);
                            var datacount = dataContext.Material.Where(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID);
                            if (datacount != null)
                            {
                                foreach (var item in datacount)
                                {
                                    if (macode == item.Material_Code)
                                    {
                                        materialCode = new BinarySystem62().gen62No((macodeS + 1), 2);
                                    }
                                    else
                                    {
                                        materialCode = new BinarySystem62().gen62No((macodeS + 1), 2);
                                    }
                                }
                            }
                            else
                            {
                                materialCode = new BinarySystem62().gen62No((macodeS + 1), 2);
                            }
                        }
                        else
                        {
                            string macode = "01";
                            int codeNum = Convert.ToInt32(macode.Replace("0", ""));
                            materialCode = new BinarySystem62().gen62No(codeNum, 2);
                        }
                        model.Material_Code = materialCode;
                        model.type = (int)Common.EnumFile.Materialtype.AutoCode;
                        model.Status = (int)Common.EnumFile.Status.unaudited;
                        //dataContext.Material.InsertOnSubmit(model); 从这里
                        //dataContext.SubmitChanges();
                        //CategoryDAL categoryDal = new CategoryDAL();
                        //BaseResultModel result = new BaseResultModel();
                        //RetResult ret = new RetResult();
                        //ret = new CategoryDAL().RecordCode(enterprise.MainCode, category.Enterprise_Info_ID, materialTemp.adduser.ToString(), category,
                        //    model.MaterialFullName, model.Material_Code);
                        //if (ret.Msg == "1")
                        //{
                        //    ret = categoryDal.Add(category, model);
                        //    result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                        //}
                        //else if (ret.Msg == "-4")
                        //{
                        //    int i = 1;
                        //    int macode = 0;
                        //    for (i = 1; i <= 100; i = i + 10)
                        //    {
                        //        macode = new BinarySystem62().Convert62ToNo(materialCode) + i;
                        //        materialCode = new BinarySystem62().gen62No(macode, 2);
                        //        ret = new CategoryDAL().RecordCode(enterprise.MainCode, category.Enterprise_Info_ID, materialTemp.adduser.ToString(), category,
                        //            model.MaterialFullName, materialCode);
                        //        if (ret.Msg == "1")
                        //        {
                        //            ret = categoryDal.Add(category, model);
                        //            result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                        //            break;
                        //        }
                        //        else if (ret.Msg == "-4")
                        //        {
                        //            continue;
                        //        }
                        //        else
                        //        {
                        //            result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                        //            Ret.Msg = "备案品类码失败！";
                        //            model.Status = (int)Common.EnumFile.Status.delete;
                        //            dataContext.SubmitChanges();
                        //            break;
                        //        }
                        //    }
                        //    if (i >= 100)
                        //    {
                        //        result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                        //        Ret.Msg = "备案品类码失败！";
                        //        model.Status = (int)Common.EnumFile.Status.delete;
                        //        dataContext.SubmitChanges();
                        //        return Ret;
                        //    }
                        //}
                        //else
                        //{
                        //    result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                        //    Ret.Msg = "备案品类码失败！";
                        //    model.Status = (int)Common.EnumFile.Status.delete;
                        //    dataContext.SubmitChanges();
                        //    return Ret;
                        //}
                        //string[] array = category.CategoryIDcode.Split('/');
                        //string[] categoryArray = array[1].Split('.');
                        //model.Material_Code = categoryArray[2];
                        //dataContext.SubmitChanges(); 到这里
                        //20180628修改因接口改成加密一块做修改
                        //CategoryList temp = Common.Tools.Public.listCategory.FirstOrDefault(m => m.CategoryID == category.CategoryID.ToString());
                        HisIndustryCategory temp = Common.Tools.Public.listCategory.FirstOrDefault(m => m.codeuse_id == category.CategoryID);
                        if (temp != null)
                        {
                            category.CategoryLevel = temp.industrycategory_level;
                            category.CategoryName = temp.industrycategory_name;
                            category.CategoryCode = temp.industrycategory_code;
                        }
                        if (enterprise.MainCode == "i.86.130105.1")
                        {
                            enterprise.MainCode = "i.1.130105.1";
                        }
                        string result = BaseDataDAL.Recode(enterprise.MainCode, category.CategoryID.ToString(),
                            category.CategoryCode, model.MaterialName, materialCode);
                        JsonObject jsonObject = new JsonObject(result);
                        result = jsonObject["ResultCode"].Value;
                        if (result == "1")
                        {

                            category.CategoryIDcode = jsonObject["OrganUnitIDcode"].Value;
                            string[] array = category.CategoryIDcode.Split('/');
                            if (array.Length == 3 && category.CategoryIDcode.Length > 0)
                            {
                                //现在IDcode接口返回的主码新注册带/，查询返回的信息不带/
                                category.CategoryIDcode = category.CategoryIDcode;
                                string[] categoryArray = array[1].Split('.');
                                model.Material_Code = categoryArray[2];
                            }
                            else if (category.CategoryIDcode.Length == 0)
                            {
                                category.Status = Convert.ToInt32(Common.EnumFile.Status.delete);
                            }
                            dataContext.Material.InsertOnSubmit(model);
                            dataContext.SubmitChanges();
                            category.MaterialID = model.Material_ID;
                            //20180417新加简码（配合农药码）
                            category.SCategoryIDcode = (model.Material_ID).ToString().PadLeft(6, '0');
                            category.Material_Code = model.Material_Code;
                            category.MaterialName = model.MaterialName;
                            dataContext.Category.InsertOnSubmit(category);
                            dataContext.SubmitChanges();
                        }
                        else if (result == "-4")
                        {
                            int i = 1;
                            for (i = 1; i <= 100; i = i + 10)
                            {
                                macodeS = new BinarySystem62().Convert62ToNo(materialCode) + i;
                                materialCode = new BinarySystem62().gen62No(macodeS, 3);
                                result = BaseDataDAL.Recode(enterprise.MainCode, category.CategoryID.ToString(),
                            category.CategoryCode, model.MaterialName, materialCode);
                                jsonObject = new JsonObject(result);
                                result = jsonObject["ResultCode"].Value;
                                if (result == "1")
                                {
                                    category.CategoryIDcode = jsonObject["OrganUnitIDcode"].Value;
                                    string[] array = category.CategoryIDcode.Split('/');
                                    if (array.Length == 3 && category.CategoryIDcode.Length > 0)
                                    {
                                        //现在IDcode接口返回的主码新注册带/，查询返回的信息不带/
                                        category.CategoryIDcode = category.CategoryIDcode;
                                        string[] categoryArray = array[1].Split('.');
                                        model.Material_Code = categoryArray[2];
                                    }
                                    else if (category.CategoryIDcode.Length == 0)
                                    {
                                        category.Status = Convert.ToInt32(Common.EnumFile.Status.delete);
                                    }
                                    dataContext.Material.InsertOnSubmit(model);
                                    dataContext.SubmitChanges();
                                    category.MaterialID = model.Material_ID;
                                    //20180417新加简码（配合农药码）
                                    category.SCategoryIDcode = (model.Material_ID).ToString().PadLeft(6, '0');
                                    category.Material_Code = model.Material_Code;
                                    category.MaterialName = model.MaterialName;
                                    dataContext.Category.InsertOnSubmit(category);
                                    break;
                                }
                                else if (result == "-4")
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (i >= 100)
                            {
                                Ret.Msg = "备案品类码失败！";
                                return Ret;
                            }
                        }
                        else
                        {
                            Ret.Msg = "备案品类码失败！";
                            return Ret;
                        }
                        foreach (var item in materialpj)
                        {
                            item.MaterialID = model.Material_ID;
                        }
                        materialTemp.Material_ID = model.Material_ID;
                        materialTemp.Material_Code = materialCode;
                        dataContext.MaterialTemp.InsertOnSubmit(materialTemp);
                        dataContext.MaterialEvaluation.InsertAllOnSubmit(materialpj);
                        shopLink.MaterialID = model.Material_ID;
                        dataContext.SubmitChanges();
                        Ret.CrudCount = model.Material_ID;
                        tran.Commit();
                        Ret.Msg = "添加产品信息成功";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    string errData = "MaterialPropertyDAL.Add()";
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
        public RetResult Edit(MaterialTemp newModel, string brandName, string spectionValue, string spectionName, List<MaterialEvaluation> materialpj, MaterialShopLink shopLink)
        {
            Ret.Msg = "修改产品信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    if (!string.IsNullOrEmpty(brandName))
                    {
                        Brand brand = dataContext.Brand.Where(p => p.BrandName == brandName.Trim() && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID
                            && p.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                        if (brand == null)
                        {
                            brand = new Brand();
                            brand.BrandName = brandName;
                            brand.Status = (int)Common.EnumFile.Status.unaudited;
                            brand.Enterprise_Info_ID = newModel.Enterprise_Info_ID;
                            brand.adddate = DateTime.Now;
                            dataContext.Brand.InsertOnSubmit(brand);
                            dataContext.SubmitChanges();
                        }
                        newModel.Brand_ID = brand.Brand_ID;
                    }
                    if (!string.IsNullOrEmpty(spectionValue) && !string.IsNullOrEmpty(spectionName))
                    {
                        MaterialSpcification data = dataContext.MaterialSpcification.Where(m => m.Enterprise_Info_ID == newModel.Enterprise_Info_ID
                            && m.Value == Convert.ToDecimal(spectionValue.Trim()) && m.MaterialSpcificationName == spectionName.Trim()
                            && m.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                        if (data != null)
                        {
                            newModel.MaterialSpecificationID = data.MaterialSpcificationID;
                        }
                        else
                        {
                            MaterialSpcification spection = new MaterialSpcification();
                            spection.Value = Convert.ToDecimal(spectionValue);
                            spection.MaterialSpcificationName = spectionName;
                            spection.Enterprise_Info_ID = newModel.Enterprise_Info_ID;
                            spection.Status = (int)Common.EnumFile.Status.unaudited;
                            spection.AddDate = DateTime.Now;
                            dataContext.MaterialSpcification.InsertOnSubmit(spection);
                            dataContext.SubmitChanges();
                            newModel.MaterialSpecificationID = spection.MaterialSpcificationID;
                        }
                    }
                    newModel.TaoBaoLink = shopLink.TaoBaoLink;
                    newModel.TianMaoLink = shopLink.TianMaoLink;
                    newModel.JingDongLink = shopLink.JingDongLink;
                    newModel.VideoUrl = shopLink.VideoUrl;
                    foreach (var sub in materialpj)
                    {
                        MaterialEvaluationTemp temp = new MaterialEvaluationTemp();
                        temp.MaterialExportExcelRecordID = newModel.MaterialExportExcelRecordID;
                        temp.MaterialID = newModel.Material_ID;
                        temp.MaterialEvaluationID = sub.MaterialEvaluationID;
                        temp.Status = (int)Common.EnumFile.Status.unaudited;
                        temp.EvaluationName = sub.EvaluationName;
                        temp.MaterialID = newModel.Material_ID;
                        temp.EnterpriseID = newModel.Enterprise_Info_ID;
                        temp.AddDate = DateTime.Now;
                        dataContext.MaterialEvaluationTemp.InsertOnSubmit(temp);
                        dataContext.SubmitChanges();
                    }
                    dataContext.MaterialTemp.InsertOnSubmit(newModel);
                    dataContext.SubmitChanges();
                    tran.Commit();
                    Ret.SetArgument(CmdResultError.NONE, "操作成功", "操作成功!");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    string errData = "MaterialPropertyDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
    }
}
