/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-11

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于经销商信息管理逻辑层

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using Common.Argument;
using Dal;
using LinqModel;
using Newtonsoft.Json;
using System.Data;
using Aspose.Cells;
using System.IO;
using LinqModel.InterfaceModels;

namespace BLL
{
    public class MaterialBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 选择产品（未注册品类的产品）
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetCMaterialList(long enterpriseId)
        {
            MaterialDAL dal = new MaterialDAL();
            List<Material> model = dal.GetCMaterialList(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, 10000000, 0, "");
            return result;
        }

        /// <summary>
        /// 获取产品模型
        /// </summary>
        /// <param name="materialId">产品标识</param>
        /// <returns>操作结果</returns>
        public BaseResultModel GetModel(long materialId)
        {
            MaterialDAL dal = new MaterialDAL();
            View_Material material = dal.GetViewModel(materialId);

            #region 图片XML转JSON类
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            List<ToJsonImg> videos = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(material.StrMaterialImgInfo))
            {
                XElement xml = XElement.Parse(material.StrMaterialImgInfo);
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    imgs.Add(sub);
                }
            }
            if (!string.IsNullOrEmpty(material.StrMaterialImgInfo))
            {
                XElement xml = XElement.Parse(material.StrMaterialImgInfo);
                IEnumerable<XElement> allImg = xml.Elements("video");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.videoUrl = item.Attribute("value").Value;
                    sub.videoUrls = item.Attribute("small").Value;
                    videos.Add(sub);
                }
            }
            List<ToJsonImg> adimgs = new List<ToJsonImg>();
            List<ToJsonImg> videoUrls = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(material.StrAdUrl))
            {
                XElement xml = XElement.Parse(material.StrAdUrl);
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    adimgs.Add(sub);
                }
            }
            if (!string.IsNullOrEmpty(material.StrVideoUrl))
            {
                XElement xml = XElement.Parse(material.StrVideoUrl);
                IEnumerable<XElement> allImg = xml.Elements("video");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("videoname").Value;
                    sub.videoUrl = item.Attribute("url").Value;
                    videoUrls.Add(sub);
                }
            }
            material.imgs = imgs;
            material.videos = videos;
            material.Adimgs = adimgs;
            material.videoUrls = videoUrls;
            #endregion

            #region 属性XML转JSON类
            List<ToJsonProperty> propertys = new List<ToJsonProperty>();
            if (!string.IsNullOrEmpty(material.StrPropertyInfo))
            {
                XElement xml = XElement.Parse(material.StrPropertyInfo);
                IEnumerable<XElement> allProperty = xml.Elements("info");
                foreach (var item in allProperty)
                {
                    ToJsonProperty sub = new ToJsonProperty();
                    sub.pName = item.Attribute("iname").Value;
                    sub.pValue = item.Attribute("ivalue").Value;
                    sub.allprototype = sub.pName + "：" + sub.pValue;
                    propertys.Add(sub);
                }
            }
            material.propertys = propertys;
            #endregion

            string code = "1";
            string msg = "";
            if (material == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(material, code, msg);
            return model;
        }

        public BaseResultModel GetMaterial(long materialId)
        {
            MaterialDAL dal = new MaterialDAL();
            Material material = dal.GetMaterial(materialId);

            #region 图片XML转JSON类
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            if (material.MaterialImgInfo != null)
            {
                XElement xml = material.MaterialImgInfo;
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    imgs.Add(sub);
                }
            }
            material.imgs = imgs;
            #endregion

            #region 属性XML转JSON类
            List<ToJsonProperty> propertys = new List<ToJsonProperty>();
            if (!string.IsNullOrEmpty(material.StrPropertyInfo))
            {
                XElement xml = XElement.Parse(material.StrPropertyInfo);
                IEnumerable<XElement> allProperty = xml.Elements("info");
                foreach (var item in allProperty)
                {
                    ToJsonProperty sub = new ToJsonProperty();
                    sub.pName = item.Attribute("iname").Value;
                    sub.pValue = item.Attribute("ivalue").Value;
                    sub.allprototype = sub.pName + "：" + sub.pValue;
                    propertys.Add(sub);
                }
            }
            material.propertys = propertys;
            #endregion

            string code = "1";
            string msg = "";
            if (material == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(material, code, msg);
            return model;
        }
        /// <summary>
        /// 根据产品ID获取产品信息
        /// </summary>
        /// <param name="maID">产品ID</param>
        /// <returns></returns>
        public Material MaMode(long maID)
        {
            MaterialDAL dal = new MaterialDAL();
            Material material = dal.GetModel(maID);
            return material;
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetList(long enterpriseId, string materialName, int pageIndex)
        {
            long totalCount = 0;
            MaterialDAL dal = new MaterialDAL();
            List<View_Material> model = dal.GetList(enterpriseId, materialName, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }
        /// <summary>
        /// 动态选择产品（获取产品列表）
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public BaseResultList GetMList(long enterpriseId)
        {
            long totalCount = 0;
            MaterialDAL dal = new MaterialDAL();
            List<Material> model = dal.GetList(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, 10000000, 0, "");
            return result;
        }
        public BaseResultList GetGMList(long enterpriseId, string brandid)
        {
            long totalCount = 0;
            MaterialDAL dal = new MaterialDAL();
            List<Material> model = dal.GetGMList(enterpriseId, brandid);
            BaseResultList result = ToJson.NewListToJson(model, 1, 10000000, 0, "");
            return result;
        }
        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Add(Material model, string mainCode, Category category, string video, MaterialShopLink shopLink, string meatCategoryName)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.StrMaterialImgInfo.Replace("[", "").Replace("]", ""))
                && string.IsNullOrEmpty(video.Replace("[", "").Replace("]", "")))
            {
                model.MaterialImgInfo = null;//根据Files解析
            }
            else
            {
                List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(model.StrMaterialImgInfo);
                List<ToJsonImg> videos = new List<ToJsonImg>();
                if (!string.IsNullOrEmpty(video))
                {
                    videos = JsonConvert.DeserializeObject<List<ToJsonImg>>(video);
                }
                XElement xml = new XElement("infos");

                foreach (var item in imgs)
                {
                    xml.Add(
                        new XElement("img",
                            new XAttribute("name", "1.jpg"),
                            new XAttribute("value", item.fileUrl),
                            new XAttribute("small", item.fileUrls)
                        )
                    );
                }
                foreach (var item in videos)
                {
                    xml.Add(
                        new XElement("video",
                            new XAttribute("name", "1.avi"),
                            new XAttribute("value", item.videoUrl),
                            new XAttribute("small", item.videoUrls)
                        )
                    );
                }
                model.MaterialImgInfo = xml;//根据Files解析
            }
            if (string.IsNullOrEmpty(model.MaterialName))
            {
                ret.Msg = "产品名称不能为空！";
            }
            else if (model.ShelfLife.Replace("长期", "") != "")
            {
                if (model.ShelfLife.IndexOf("长期") > 0)
                {
                    ret.Msg = "请选择保质期单位！";
                }
                else
                {
                    if (model.ShelfLife.Length > 1)
                    {
                        string date = model.ShelfLife.Substring(0, model.ShelfLife.Length - 1);
                        int intDate = 0;
                        if (!int.TryParse(date, out intDate) && date != "长" && date != "视存储环")
                        {
                            ret.Msg = "请输入正确的保质期时间！";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(model.StrPropertyInfo))
                            {
                                model.PropertyInfo = PropertyJsonToXml(model.StrPropertyInfo); ;//根据StrPropertyInfo解析
                            }

                        }
                    }
                    else
                    {
                        ret.Msg = "保质期时间不能为空！";
                    }
                }
            }
            else if (string.IsNullOrEmpty(model.StrMaterialImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传产品照片！";
            }
            else
            {
                if (!string.IsNullOrEmpty(model.StrPropertyInfo))
                {
                    model.PropertyInfo = PropertyJsonToXml(model.StrPropertyInfo); ;//根据StrPropertyInfo解析
                }
            }
            if (!string.IsNullOrEmpty(shopLink.StrAdFileUrl))
            {
                List<ToJsonImg> adImg = JsonConvert.DeserializeObject<List<ToJsonImg>>(shopLink.StrAdFileUrl);
                XElement xle = new XElement("infos");
                foreach (var item in adImg)
                {
                    xle.Add(
                      new XElement("img",
                          new XAttribute("name", "1.jpg"),
                          new XAttribute("value", item.fileUrl),
                          new XAttribute("small", item.fileUrls)
                      )
                  );
                }
                shopLink.AdUrl = xle;
            }
            if (!string.IsNullOrEmpty(shopLink.StrVideoUrlInfo))
            {
                List<ToJsonImg> videoUrls = JsonConvert.DeserializeObject<List<ToJsonImg>>(shopLink.StrVideoUrlInfo);
                XElement xle = new XElement("infos");
                foreach (var item in videoUrls)
                {
                    //xle.Add(new XElement("video", new XAttribute("url", item.videoUrl)));
                    xle.Add(new XElement("video",
        new XAttribute("videoname", item.fileUrl),
        new XAttribute("url", item.videoUrl)));
                }
                shopLink.VideoUrl = xle;
            }
            ret = dal.Add(model, mainCode, category, shopLink, meatCategoryName);
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Edit(Material newModel, string video, MaterialShopLink shopLink)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(newModel.StrMaterialImgInfo);
            List<ToJsonImg> videos = JsonConvert.DeserializeObject<List<ToJsonImg>>(video);
            XElement xml = new XElement("infos");
            foreach (var item in imgs)
            {
                xml.Add(
                    new XElement("img",
                        new XAttribute("name", "1.jpg"),
                        new XAttribute("value", item.fileUrl),
                        new XAttribute("small", item.fileUrls)
                    )
                );
            }
            foreach (var item in videos)
            {
                xml.Add(
                    new XElement("video",
                        new XAttribute("name", "1.avi"),
                        new XAttribute("value", item.videoUrl),
                        new XAttribute("small", item.videoUrls)
                    )
                );
            }
            newModel.MaterialImgInfo = xml;//根据Files解析
            if (!string.IsNullOrEmpty(shopLink.StrAdFileUrl))
            {
                List<ToJsonImg> adImg = JsonConvert.DeserializeObject<List<ToJsonImg>>(shopLink.StrAdFileUrl);
                XElement xle = new XElement("infos");
                foreach (var item in adImg)
                {
                    xle.Add(
                      new XElement("img",
                          new XAttribute("name", "1.jpg"),
                          new XAttribute("value", item.fileUrl),
                          new XAttribute("small", item.fileUrls)
                      )
                  );
                }
                shopLink.AdUrl = xle;
            }
            if (!string.IsNullOrEmpty(shopLink.StrVideoUrlInfo))
            {
                List<ToJsonImg> videoUrls = JsonConvert.DeserializeObject<List<ToJsonImg>>(shopLink.StrVideoUrlInfo);
                XElement xle = new XElement("infos");
                foreach (var item in videoUrls)
                {
                    xle.Add(new XElement("video",
new XAttribute("videoname", item.fileUrl),
new XAttribute("url", item.videoUrl)));
                }
                shopLink.VideoUrl = xle;
            }
            if (string.IsNullOrEmpty(newModel.MaterialName))
            {
                ret.Msg = "产品名称不能为空！";
            }
            else if (newModel.ShelfLife.Replace("长期", "") != "" || newModel.ShelfLife.Replace("视存储环境", "") != "")
            {
                if (newModel.ShelfLife.IndexOf("长期") > 0 || newModel.ShelfLife.IndexOf("视存储环境") > 0)
                {
                    ret.Msg = "请选择保质期单位！";
                }
                else
                {
                    if (newModel.ShelfLife.Length > 1)
                    {
                        string date = newModel.ShelfLife.Substring(0, newModel.ShelfLife.Length - 1);
                        int intDate = 0;
                        if (!int.TryParse(date, out intDate) && date != "长" && date != "视存储环")
                        {
                            ret.Msg = "请输入正确的保质期时间！";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(newModel.StrPropertyInfo))
                            {
                                newModel.PropertyInfo = PropertyJsonToXml(newModel.StrPropertyInfo); //根据StrPropertyInfo解析
                            }
                            ret = dal.Edit(newModel, shopLink);
                        }
                    }
                    else
                    {
                        ret.Msg = "保质期时间不能为空！";
                    }
                }
            }
            else if (string.IsNullOrEmpty(newModel.StrMaterialImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传产品照片！";
            }
            else
            {
                if (!string.IsNullOrEmpty(newModel.StrPropertyInfo))
                {
                    newModel.PropertyInfo = PropertyJsonToXml(newModel.StrPropertyInfo); ;//根据StrPropertyInfo解析
                }
                ret = dal.Edit(newModel, shopLink);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Add(Material model)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.MaterialName))
            {
                ret.Msg = "产品名称不能为空！";
            }
            else if (model.ShelfLife.Replace("长期", "").Replace("视存储环境", "") != "")
            {
                if (model.ShelfLife.IndexOf("长期") > 0 || model.ShelfLife.IndexOf("视存储环境") > 0)
                {
                    ret.Msg = "请选择保质期单位！";
                }
                else
                {
                    if (model.ShelfLife.Length > 1)
                    {
                        string date = model.ShelfLife.Substring(0, model.ShelfLife.Length - 1);
                        int intDate = 0;
                        if (!int.TryParse(date, out intDate))
                        {
                            ret.Msg = "请输入正确的保质期时间！";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(model.Memo))
                            {
                                ret.Msg = "产品描述不能为空！";
                            }
                            else if (string.IsNullOrEmpty(model.StrMaterialImgInfo.Replace("[", "").Replace("]", "")))
                            {
                                ret.Msg = "请上传产品照片！";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(model.StrPropertyInfo))
                                {
                                    model.PropertyInfo = PropertyJsonToXml(model.StrPropertyInfo); ;//根据StrPropertyInfo解析
                                }

                                model.MaterialImgInfo = ImgJsonToXml(model.StrMaterialImgInfo);//根据StrMaterialImgInfo解析

                                ret = dal.Add(model);
                            }
                        }
                    }
                    else
                    {
                        ret.Msg = "保质期时间不能为空！";
                    }
                }
            }
            else if (string.IsNullOrEmpty(model.Memo))
            {
                ret.Msg = "产品描述不能为空！";
            }
            else if (string.IsNullOrEmpty(model.StrMaterialImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传产品照片！";
            }
            else
            {
                if (!string.IsNullOrEmpty(model.StrPropertyInfo))
                {
                    model.PropertyInfo = PropertyJsonToXml(model.StrPropertyInfo); ;//根据StrPropertyInfo解析
                }

                model.MaterialImgInfo = ImgJsonToXml(model.StrMaterialImgInfo);//根据StrMaterialImgInfo解析

                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Edit(Material newModel)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(newModel.MaterialName))
            {
                ret.Msg = "产品名称不能为空！";
            }
            else if (newModel.ShelfLife.Replace("长期", "").Replace("视存储环境", "") != "")
            {
                if (newModel.ShelfLife.IndexOf("长期") > 0 || newModel.ShelfLife.IndexOf("视存储环境") > 0)
                {
                    ret.Msg = "请选择保质期单位！";
                }
                else
                {
                    if (newModel.ShelfLife.Length > 1)
                    {
                        string date = newModel.ShelfLife.Substring(0, newModel.ShelfLife.Length - 1);
                        int intDate = 0;
                        if (!int.TryParse(date, out intDate))
                        {
                            ret.Msg = "请输入正确的保质期时间！";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(newModel.Memo))
                            {
                                ret.Msg = "产品描述不能为空！";
                            }
                            else if (string.IsNullOrEmpty(newModel.StrMaterialImgInfo.Replace("[", "").Replace("]", "")))
                            {
                                ret.Msg = "请上传产品照片！";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(newModel.StrPropertyInfo))
                                {
                                    newModel.PropertyInfo = PropertyJsonToXml(newModel.StrPropertyInfo); ;//根据StrPropertyInfo解析
                                }

                                newModel.MaterialImgInfo = ImgJsonToXml(newModel.StrMaterialImgInfo);//根据StrMaterialImgInfo解析

                                ret = dal.Edit(newModel);
                            }
                        }
                    }
                    else
                    {
                        ret.Msg = "保质期时间不能为空！";
                    }
                }
            }
            else if (string.IsNullOrEmpty(newModel.Memo))
            {
                ret.Msg = "产品描述不能为空！";
            }
            else if (string.IsNullOrEmpty(newModel.StrMaterialImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传产品照片！";
            }
            else
            {
                if (!string.IsNullOrEmpty(newModel.StrPropertyInfo))
                {
                    newModel.PropertyInfo = PropertyJsonToXml(newModel.StrPropertyInfo); ;//根据StrPropertyInfo解析
                }

                newModel.MaterialImgInfo = ImgJsonToXml(newModel.StrMaterialImgInfo);//根据StrMaterialImgInfo解析

                ret = dal.Edit(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="materialId">产品标识</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Del(long enterpriseId, long materialId)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = dal.Del(enterpriseId, materialId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        public XElement ImgJsonToXml(string jsonImgs)
        {
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(jsonImgs);

            XElement xml = new XElement("infos");

            foreach (var item in imgs)
            {
                xml.Add(
                    new XElement("img",
                        new XAttribute("name", "1.jpg"),
                        new XAttribute("value", item.fileUrl),
                        new XAttribute("small", item.fileUrls)
                    )
                );
            }
            return xml;

        }

        public XElement JCImgJsonToXml(string jsonImgs)
        {
            List<ToJsonJCImg> imgs = JsonConvert.DeserializeObject<List<ToJsonJCImg>>(jsonImgs);

            XElement xml = new XElement("infos");

            foreach (var item in imgs)
            {
                xml.Add(
                    new XElement("img",
                        new XAttribute("name", "1.jpg"),
                        new XAttribute("value", item.jcfileUrl),
                        new XAttribute("small", item.jcfileUrls)
                    )
                );
            }
            return xml;

        }
        private XElement PropertyJsonToXml(string jsonPropertys)
        {
            List<ToJsonProperty> imgs = JsonConvert.DeserializeObject<List<ToJsonProperty>>(jsonPropertys);
            XElement xml = new XElement("infos");
            foreach (var item in imgs)
            {
                xml.Add(
                    new XElement("info",
                        new XAttribute("iname", item.pName),
                        new XAttribute("ivalue", item.pValue)
                    )
                );
            }
            return xml;
        }

        /// <summary>
        /// 生成码页面简单添加产品信息
        /// </summary>
        /// <param name="model">产品实体</param>
        /// <returns></returns>
        public BaseResultModel AddSimple(Material model)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.MaterialName))
            {
                ret.Msg = "产品名称不能为空！";
            }
            else
            {
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取一级产品分类
        /// </summary>
        /// <returns></returns>
        public List<LinqModel.Dictionary_MaterialType> GetMaterialType(int level, long parent)
        {
            List<LinqModel.Dictionary_MaterialType> list = null;
            MaterialDAL dal = new MaterialDAL();
            list = dal.GetMaterialType(level, parent);
            return list;
        }

        /// <summary>
        /// 根据产品类别名称查询数据
        /// </summary>
        /// <param name="name">产品类别名称</param>
        /// <returns></returns>
        public List<ToJsonProperty> SearchType(string name)
        {
            MaterialDAL dal = new MaterialDAL();
            List<ToJsonProperty> property = dal.SearchType(name);
            string code = "1";
            string msg = "";
            if (property.Count == 0)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            return property;
        }

        /// <summary>
        /// 获取产品评价
        /// </summary>
        /// <param name="materialId">产品ID</param>
        /// <returns></returns>
        public BaseResultList GetMaPJ(long materialId)
        {
            MaterialDAL dal = new MaterialDAL();
            List<MaterialEvaluation> materialpj = dal.GetMaPJ(materialId);
            if (materialpj.Count > 0)
            {
                MaterialEvaluation mapj = new MaterialEvaluation();
                foreach (var item in materialpj)
                {
                    mapj.EvaluationName = item.EvaluationName;
                }
            }
            BaseResultList model = ToJson.NewListToJson(materialpj, 1, PageSize, 1000000, "");
            return model;
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="eID"></param>
        /// <returns></returns>
        public DataTable ExportExcel(long eID, string searchName, MaterialExportExcelRecord modelExcel)
        {
            DataTable dt = new DataTable();
            try
            {
                MaterialDAL dal = new MaterialDAL();
                List<View_Material> model = dal.ExportExcel(eID, searchName);
                List<ToJsonProperty> operations = new List<ToJsonProperty>();
                //Material model = dal.ExportExcel(eID);
                dt.Columns.Add("产品编码");
                dt.Columns.Add("产品名称");
                dt.Columns.Add("产品别名");
                dt.Columns.Add("产品品牌");
                dt.Columns.Add("品类");
                dt.Columns.Add("产品类别");
                dt.Columns.Add("规格值");
                dt.Columns.Add("规格单位");
                dt.Columns.Add("保质期");
                dt.Columns.Add("产品口味");
                dt.Columns.Add("产品自定义属性");
                dt.Columns.Add("产地");
                dt.Columns.Add("产品图片地址");
                dt.Columns.Add("产品视频地址");
                dt.Columns.Add("产品广告图片地址");
                dt.Columns.Add("产品实时视频地址");
                dt.Columns.Add("产品购买链接");
                dt.Columns.Add("淘宝商城链接");
                dt.Columns.Add("京东商城链接");
                dt.Columns.Add("天猫商城链接");
                dt.Columns.Add("产品评价1");
                dt.Columns.Add("产品评价2");
                dt.Columns.Add("产品评价3");
                dt.Columns.Add("产品评价4");
                dt.Columns.Add("产品评价5");
                if (model != null && model.Count > 0)
                {
                    foreach (var item in model)
                    {
                        int index = 0;
                        DataRow dr = dt.NewRow();
                        dr[index++] = item.CodeUser;
                        dr[index++] = item.MaterialName;
                        dr[index++] = item.MaterialAliasName;
                        dr[index++] = item.BrandName;
                        dr[index++] = item.CategoryName;
                        dr[index++] = item.MaterialTypeNameMa;
                        dr[index++] = item.Value;
                        dr[index++] = item.MaterialSpcificationName;
                        dr[index++] = item.ShelfLife;
                        dr[index++] = item.MaterialTaste;
                        if (!string.IsNullOrEmpty(item.StrPropertyInfo))
                        {
                            string sxInfos = "";
                            XElement xml = XElement.Parse(item.StrPropertyInfo);
                            IEnumerable<XElement> allOperation = xml.Elements("info");
                            foreach (var sx in allOperation)
                            {
                                ToJsonProperty sub = new ToJsonProperty();
                                sub.pName = sx.Attribute("iname").Value;
                                sub.pValue = sx.Attribute("ivalue").Value;
                                sub.allprototype = sub.pName + ":" + sub.pValue + "-";
                                sxInfos = sxInfos + sub.allprototype;
                            }
                            if (!string.IsNullOrEmpty(sxInfos))
                            {
                                dr[index++] = sxInfos.Substring(0, sxInfos.Length - 1);
                            }
                            else
                            {
                                dr[index++] = "";
                            }
                        }
                        else
                        {
                            dr[index++] = "";
                        }
                        dr[index++] = item.MaterialPlace;
                        if (!string.IsNullOrEmpty(item.StrMaterialImgInfo))
                        {
                            string maImgInfos = "";
                            XElement xml = XElement.Parse(item.StrMaterialImgInfo);
                            IEnumerable<XElement> allImgs = xml.Elements("img");
                            foreach (var itemimg in allImgs)
                            {
                                ToJsonImg sub = new ToJsonImg();
                                sub.fileUrl = itemimg.Attribute("value").Value;
                                sub.fileUrls = itemimg.Attribute("small").Value;
                                string allImgInfo = sub.fileUrl + "&" + sub.fileUrls;
                                maImgInfos = maImgInfos + allImgInfo + ";";
                            }
                            if (!string.IsNullOrEmpty(maImgInfos))
                            {
                                dr[index++] = maImgInfos.Substring(0, maImgInfos.Length - 1);
                            }
                            else
                            {
                                dr[index++] = "";
                            }
                        }
                        else
                        {
                            dr[index++] = "";
                        }
                        if (!string.IsNullOrEmpty(item.StrMaterialImgInfo))
                        {
                            string maVideoInfos = "";
                            XElement xml = XElement.Parse(item.StrMaterialImgInfo);
                            IEnumerable<XElement> allVideos = xml.Elements("video");
                            foreach (var itemvideo in allVideos)
                            {
                                ToJsonImg sub = new ToJsonImg();
                                sub.videoUrl = itemvideo.Attribute("value").Value;
                                sub.videoUrls = itemvideo.Attribute("small").Value;
                                string allVideoInfos = sub.videoUrl + "&" + sub.videoUrls;
                                maVideoInfos = maVideoInfos + allVideoInfos + ";";
                            }
                            if (!string.IsNullOrEmpty(maVideoInfos))
                            {
                                dr[index++] = maVideoInfos.Substring(0, maVideoInfos.Length - 1);
                            }
                            else
                            {
                                dr[index++] = "";
                            }
                        }
                        else
                        {
                            dr[index++] = "";
                        }
                        if (!string.IsNullOrEmpty(item.StrAdUrl))
                        {
                            string maAdUrlInfos = "";
                            XElement xml = XElement.Parse(item.StrAdUrl);
                            IEnumerable<XElement> allAdUrls = xml.Elements("img");
                            foreach (var itemAd in allAdUrls)
                            {
                                ToJsonImg sub = new ToJsonImg();
                                sub.fileUrl = itemAd.Attribute("value").Value;
                                sub.fileUrls = itemAd.Attribute("small").Value;
                                string allImgInfogg = sub.fileUrl + "&" + sub.fileUrls;
                                maAdUrlInfos = maAdUrlInfos + allImgInfogg + ";";
                            }
                            dr[index++] = maAdUrlInfos;
                        }
                        else
                        {
                            dr[index++] = "";
                        }
                        if (!string.IsNullOrEmpty(item.StrVideoUrl))
                        {
                            string maSSVideoInfos = "";
                            XElement xml = XElement.Parse(item.StrVideoUrl);
                            IEnumerable<XElement> allVideos = xml.Elements("video");
                            foreach (var itemvideo in allVideos)
                            {
                                ToJsonImg sub = new ToJsonImg();
                                sub.videoUrl = itemvideo.Attribute("url").Value;
                                maSSVideoInfos = maSSVideoInfos + sub.videoUrl + ";";
                            }
                            if (!string.IsNullOrEmpty(maSSVideoInfos))
                            {
                                dr[index++] = maSSVideoInfos.Substring(0, maSSVideoInfos.Length - 1);
                            }
                            else
                            {
                                dr[index++] = "";
                            }
                        }
                        else
                        {
                            dr[index++] = "";
                        }
                        dr[index++] = item.tbURL;
                        dr[index++] = item.TaoBaoLink;
                        dr[index++] = item.JingDongLink;
                        dr[index++] = item.TianMaoLink;
                        List<MaterialEvaluation> maPJ = dal.GetMaPJ(item.Material_ID);
                        if (maPJ.Count > 0)
                        {
                            for (int c = 0; c < 5; c++)
                            {
                                MaterialEvaluation op = maPJ[c];
                                dr[index++] = op.EvaluationName;
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch { throw; }
            return dt;
        }

        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public BaseResultModel AddExcelR(MaterialExportExcelRecord newModel)
        {
            RetResult ret = new RetResult();
            MaterialDAL dal = new MaterialDAL();
            ret = dal.AddExcelR(newModel);
            if (ret.IsSuccess)
            {
                FileStream fs = new FileStream(newModel.ExcelPath, FileMode.Open, FileAccess.Read);
                Workbook book = new Workbook(fs);
                Worksheet sheet = book.Worksheets[0];
                Cells cells = sheet.Cells;
                DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                MaterialDAL madal = new MaterialDAL();
                MaterialExportExcelRecord exModel = new MaterialExportExcelRecord();
                exModel.MaCount = ds.Tables[0].Rows.Count;
                exModel = madal.UpdataCount(newModel.ID, exModel);
                MaterialPropertyDAL maPDAL = new MaterialPropertyDAL();
                ret = maPDAL.Verify(ds);
                if (ret.IsSuccess)
                {
                    ret = maPDAL.InportExcel(ds, newModel.EnterpriseID.Value, newModel.AddUser.Value, newModel.ID);
                    if (ret.IsSuccess)
                    {
                        ret.Msg = "导入成功！";
                    }
                }
            }
            else
            {
                ret.Msg = "导入失败！";
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="fileUrl">文件路径</param>
        /// <returns></returns>
        public static List<Object> GetExcel(string fileUrl, long rID)
        {
            List<Object> result = new List<Object>();
            System.Data.DataSet ds = null;
            string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", fileUrl);
            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn))
            {
                string strExcel = "";
                System.Data.OleDb.OleDbDataAdapter myCommand = null;
                strExcel = "select * from [sheet1$]";
                myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
                ds = new System.Data.DataSet();
                myCommand.Fill(ds, "table1");
            }
            MaterialDAL madal = new MaterialDAL();
            MaterialExportExcelRecord exModel = new MaterialExportExcelRecord();
            exModel.MaCount = ds.Tables[0].Rows.Count;
            exModel = madal.UpdataCount(rID, exModel);
            MaterialPropertyDAL dal = new MaterialPropertyDAL();
            RetResult ret = dal.Verify(ds);
            RetResult Inputret = dal.InportExcel(ds, exModel.EnterpriseID.Value, exModel.AddUser.Value, rID);
            return result;
        }

        /// <summary>
        /// 获取待审核列表（导入的Excel）
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public BaseResultList GetExcelRecord(long enterpriseId, int pageIndex)
        {
            long totalCount = 0;
            MaterialDAL dal = new MaterialDAL();
            List<MaterialExportExcelRecord> model = dal.GetExcelRecord(enterpriseId, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 审核导入Excel
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public BaseResultModel AuditExcel(long eId, long id)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = dal.AuditExcel(eId, id);
            //List<MaterialExportExcelRecord> model = dal.GetExcelRecord(id);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取企业产品列表
        /// 陈志钢  WinCE
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <returns></returns>
        public List<View_WinCe_MaterialInfo> getMaterialLst(long enterpriseID)
        {
            MaterialDAL dal = new MaterialDAL();
            return dal.getMaterialLst(enterpriseID);
        } 

        /// <summary> 
        /// 添加绑定记录 
        /// 陈志钢  WInCE 
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public bool addBindRecord(List<LinqModel.BindCodeRecords> lst)
        {
            MaterialDAL dal = new MaterialDAL();
            return dal.addBindRecord(lst);
        }

        /// <summary>
        /// 导出产品信息
        /// </summary>
        /// <param name="eID"></param>
        /// <returns></returns>
        public BaseResultModel ExportTxt(long eID)
        {
            BaseResultModel result = new BaseResultModel();
            List<Product> productList = new List<Product>();
            Enterprise_Info enterprise = new EnterpriseInfoDAL().GetModel(eID);
            if (enterprise == null)
            {
               return  result;
            }
            MaterialDAL dal = new MaterialDAL();
            List<View_Material> model = dal.ExportExcel(eID, "");
            foreach (var sub in model)
            {
                Product product = new Product();
                product.production_id = sub.Material_Code;
                product.name = sub.MaterialName;
                product.img = "";
                product.period = sub.ShelfLife;
                product.price = sub.price.ToString();
                product.introduce = "";
                product.created_at = sub.StrAddDate;
                product.reg_code = sub.NYZhengHao;
                product.reg_code_owner = enterprise.EnterpriseName;
                product.reg_code_type = "";
                product.product_code = "";
                product.p_alias = sub.MaterialAliasName;
                product.unit = "";
                productList.Add(product);
            }
            result = ToJson.NewModelToJson(productList, "", "");
            return result;
        }

        #region 刘晓杰于2019年11月4日从CFBack项目移入此

        #region 对接友高接口

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetMaterialListNew(long enterpriseId)
        {
            BaseResultList result = new BaseResultList();
            MaterialDAL dal = new MaterialDAL();
            //if (0 == enterpriseId)
            //{
            //    enterpriseId = 2;
            //}
            var list = dal.GetMaterialList(enterpriseId); ;
            List<Materials> productList = new List<Materials>();
            foreach (var sub in list)
            {
                Materials product = new Materials();
                product.Material_ID = sub.Material_ID;
                product.MaterialFullName = sub.MaterialFullName;
                if (sub.Value != null && !string.IsNullOrEmpty(sub.MaterialSpcificationName))
                {
                    product.MaterialSpcificationName = ((decimal)sub.Value).ToString("f0") + sub.MaterialSpcificationName;
                }
                product.CategoryName = sub.CategoryName;

                productList.Add(product);
            }
            result = ToJson.NewListToJson(productList, 0, 0, (long)productList.Count, "");
            return result;
        }
        #endregion

        #endregion

        #region 2021-4-22 赵慧敏 打码客户端 DI信息接口
        /// <summary>
        /// 接口添加产品
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mainCode"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public RetResult AddMaterial(Material model, Category category, MaterialDI modelDI)
        {
            MaterialDAL dal = new MaterialDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.MaterialName))
            {
                ret.Msg = "产品名称不能为空！";
            }
            ret = dal.AddMaterial(model, category,modelDI);
            return ret;
        }
        /// <summary>
        /// 打码客户端获取DI信息
        /// </summary>
        /// <param name="context"></param>
        public BaseResultList GetMaterialDI(long enterpriseId, string date)
        {
            BaseResultList result = new BaseResultList();
            MaterialDAL dal = new MaterialDAL();
            List<MaterialDI> list = dal.GetDIList(enterpriseId, date); ;
            result = ToJson.NewListToJson(list, 0, 0, (long)list.Count, "");
            return result;
        }

        public RetResult UploadDIPrivate(MaterialResponse model)
        {
            MaterialDAL dal = new MaterialDAL();
            return dal.UploadDIPrivate(model);
        }
        #endregion
    }
}
