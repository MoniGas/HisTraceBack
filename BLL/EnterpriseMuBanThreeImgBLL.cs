/********************************************************************************
** 作者： 张翠霞
** 创始时间：2018-9-7
** 联系方式 :13313318725
** 描述：企业拍码模板3的图片信息
** 版本：v1.0
** 版权：追溯项目组
*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Dal;
using System.Xml.Linq;
using Common.Argument;
using Newtonsoft.Json;

namespace BLL
{
    public class EnterpriseMuBanThreeImgBLL
    {
        /// <summary>
        /// 获取企业拍码模板3的图片信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long eId)
        {
            EnterpriseMuBanThreeImgDAL dal = new EnterpriseMuBanThreeImgDAL();
            EnterpriseMuBanThreeImg model = dal.GetModel(eId);
            if (model != null)
            {
                List<ToJsonImg> imgs = new List<ToJsonImg>();
                //判断企业Logo是否为空
                if (!string.IsNullOrEmpty(model.StrFirstImg))
                {
                    XElement xml = XElement.Parse(model.StrFirstImg);
                    IEnumerable<XElement> img1 = xml.Elements("img");
                    foreach (var item in img1)
                    {
                        ToJsonImg sub = new ToJsonImg
                        {
                            fileUrl = item.Attribute("value").Value,
                            fileUrls = item.Attribute("small").Value
                        };
                        imgs.Add(sub);
                    }
                }
                model.FirstImgs = imgs;
                List<ToJsonImg> imgzj = new List<ToJsonImg>();
                if (!string.IsNullOrEmpty(model.StrCenterImg))
                {
                    XElement xml = XElement.Parse(model.StrCenterImg);
                    IEnumerable<XElement> img2 = xml.Elements("img");
                    foreach (var item in img2)
                    {
                        ToJsonImg sub = new ToJsonImg
                        {
                            fileUrl = item.Attribute("value").Value,
                            fileUrls = item.Attribute("small").Value
                        };
                        imgzj.Add(sub);
                    }
                }
                model.CenterImgs = imgzj;
                List<ToJsonImg> Imgzh = new List<ToJsonImg>();
                if (!string.IsNullOrEmpty(model.StrFiveImg))
                {
                    XElement Xml = XElement.Parse(model.StrFiveImg);
                    IEnumerable<XElement> Img3 = Xml.Elements("img");
                    foreach (var item in Img3)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.fileUrl = item.Attribute("value").Value;
                        sub.fileUrls = item.Attribute("small").Value;
                        Imgzh.Add(sub);
                    }
                }
                model.FiveImgs = Imgzh;
            }
            return ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
        }

        /// <summary>
        /// 提交/修改图片
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public BaseResultModel Edit(EnterpriseMuBanThreeImg Model)
        {
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(Model.StrFirstImg);
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
            Model.FirstImg = xml;//根据Files解析
            EnterpriseMuBanThreeImgDAL Dal = new EnterpriseMuBanThreeImgDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (!string.IsNullOrEmpty(Model.StrCenterImg))
            {
                List<ToJsonImg> imgszj = JsonConvert.DeserializeObject<List<ToJsonImg>>(Model.StrCenterImg);
                XElement xmlzj = new XElement("infos");
                foreach (var item in imgszj)
                {
                    xmlzj.Add(
                        new XElement("img",
                            new XAttribute("name", "1.jpg"),
                            new XAttribute("value", item.fileUrl),
                            new XAttribute("small", item.fileUrls)
                        )
                    );
                }
                Model.CenterImg = xmlzj;//根据Files解析
            }
            if (!string.IsNullOrEmpty(Model.StrFiveImg))
            {
                List<ToJsonImg> imgszh = JsonConvert.DeserializeObject<List<ToJsonImg>>(Model.StrFiveImg);
                XElement xmlzh = new XElement("infos");
                foreach (var item in imgszh)
                {
                    xmlzh.Add(
                        new XElement("img",
                            new XAttribute("name", "1.jpg"),
                            new XAttribute("value", item.fileUrl),
                            new XAttribute("small", item.fileUrls)
                        )
                    );
                }
                Model.FiveImg = xmlzh;//根据Files解析
            }
            ret = Dal.Edit(Model);
            return ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }

        public EnterpriseMuBanThreeImg GetMuBan3Model(long eId)
        {
            EnterpriseMuBanThreeImgDAL dal = new EnterpriseMuBanThreeImgDAL();
            EnterpriseMuBanThreeImg model = dal.GetModel(eId);
            if (model != null)
            {
                List<ToJsonImg> Imgs = new List<ToJsonImg>();
                //判断企业Logo是否为空
                if (!string.IsNullOrEmpty(model.StrFirstImg))
                {
                    XElement Xml = XElement.Parse(model.StrFirstImg);
                    IEnumerable<XElement> Img1 = Xml.Elements("img");
                    foreach (var Item in Img1)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.fileUrl = Item.Attribute("value").Value;
                        sub.fileUrls = Item.Attribute("small").Value;
                        Imgs.Add(sub);
                    }
                }
                model.FirstImgs = Imgs;
                List<ToJsonImg> Imgzj = new List<ToJsonImg>();
                if (!string.IsNullOrEmpty(model.StrCenterImg))
                {
                    XElement Xml = XElement.Parse(model.StrCenterImg);
                    IEnumerable<XElement> Img2 = Xml.Elements("img");
                    foreach (var Item in Img2)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.fileUrl = Item.Attribute("value").Value;
                        sub.fileUrls = Item.Attribute("small").Value;
                        Imgzj.Add(sub);
                    }
                }
                model.CenterImgs = Imgzj;
                List<ToJsonImg> Imgzh = new List<ToJsonImg>();
                if (!string.IsNullOrEmpty(model.StrFiveImg))
                {
                    XElement Xml = XElement.Parse(model.StrFiveImg);
                    IEnumerable<XElement> Img3 = Xml.Elements("img");
                    foreach (var item in Img3)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.fileUrl = item.Attribute("value").Value;
                        sub.fileUrls = item.Attribute("small").Value;
                        Imgzh.Add(sub);
                    }
                }
                model.FiveImgs = Imgzh;
            }
            return model;
        }
    }
}
