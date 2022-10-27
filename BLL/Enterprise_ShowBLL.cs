/********************************************************************************

** 作者： 郭心宇

** 创始时间：2016-1-5

** 联系方式 :13313318725

** 描述：主要用于宣传板块“我的介绍”信息的业务逻辑

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using Dal;
using Common.Argument;
using LinqModel;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace BLL
{
    public class Enterprise_ShowBLL
    {
        /// <summary>
        /// 获取企业模型
        /// </summary>
        /// <param name="Id">企业ID</param>
        /// <returns></returns>
        public BaseResultModel GetModelView(long Id)
        {
            Enterprise_ShowDAL objEnterpriseShowDal = new Enterprise_ShowDAL();
            View_EnterpriseShow data = objEnterpriseShowDal.GetModelView(Id);
            if (data != null)
            {
                List<ToJsonImg> imgs = new List<ToJsonImg>();
                //判断企业Logo是否为空
                if (!string.IsNullOrEmpty(data.StrLogo))
                {
                    XElement xml = XElement.Parse(data.StrLogo);
                    IEnumerable<XElement> allImg = xml.Elements("img");
                    foreach (var item in allImg)
                    {
                        ToJsonImg sub = new ToJsonImg
                        {
                            fileUrl = item.Attribute("value").Value,
                            fileUrls = item.Attribute("small").Value
                        };
                        imgs.Add(sub);
                    }
                }
                data.imgs = imgs;
                List<ToJsonJCImg> Imgsgg = new List<ToJsonJCImg>();
                //判断企业Logo是否为空
                if (!string.IsNullOrEmpty(data.StrAdUrl))
                {
                    XElement xml = XElement.Parse(data.StrAdUrl);
                    IEnumerable<XElement> AllImg = xml.Elements("img");
                    foreach (var Item in AllImg)
                    {
                        ToJsonJCImg sub = new ToJsonJCImg();
                        sub.jcfileUrl = Item.Attribute("value").Value;
                        sub.jcfileUrls = Item.Attribute("small").Value;
                        Imgsgg.Add(sub);
                    }
                }
                data.imgsgg = Imgsgg;
                List<ToJsonImg> videoUrls = new List<ToJsonImg>();
                //判断企业Logo是否为空
                if (!string.IsNullOrEmpty(data.StrSSVideoUrl))
                {
                    XElement xml = XElement.Parse(data.StrSSVideoUrl);
                    IEnumerable<XElement> allImg = xml.Elements("video");
                    foreach (var item in allImg)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.videoUrl = item.Attribute("url").Value;
                        videoUrls.Add(sub);
                    }
                }
                data.videoUrls = videoUrls;
                List<ToJsonImg> wxlogoImgs = new List<ToJsonImg>();
                //判断企业微信Logo是否为空
                if (!string.IsNullOrEmpty(data.StrWXLogo))
                {
                    XElement Xml = XElement.Parse(data.StrWXLogo);
                    IEnumerable<XElement> AllImg = Xml.Elements("img");
                    foreach (var Item in AllImg)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.fileUrl = Item.Attribute("value").Value;
                        sub.fileUrls = Item.Attribute("small").Value;
                        wxlogoImgs.Add(sub);
                    }
                }
                data.wxlogoimgs = wxlogoImgs;
            }
            return ToJson.NewModelToJson(data, data == null ? "0" : "1", "");
        }

        /// <summary>
        /// 编辑企业信息
        /// </summary>
        /// <param name="Model">企业信息表内容</param>
        /// <returns></returns>
        public BaseResultModel Edit(Enterprise_Info Model, EnterpriseShopLink shopModel)
        {
            List<ToJsonImg> Imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(Model.StrLogo);
            XElement Xml = new XElement("infos");
            foreach (var Item in Imgs)
            {
                Xml.Add(
                    new XElement("img",
                        new XAttribute("name", "1.jpg"),
                        new XAttribute("value", Item.fileUrl),
                        new XAttribute("small", Item.fileUrls)
                    )
                );
            }
            Model.Logo = Xml;//根据Files解析
            Enterprise_ShowDAL dal = new Enterprise_ShowDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (!string.IsNullOrEmpty(shopModel.StrAdUrl))
            {
                List<ToJsonJCImg> imgsgg = JsonConvert.DeserializeObject<List<ToJsonJCImg>>(shopModel.StrAdUrl);
                XElement xmlgg = new XElement("infos");
                foreach (var item in imgsgg)
                {
                    xmlgg.Add(
                        new XElement("img",
                            new XAttribute("name", "1.jpg"),
                            new XAttribute("value", item.jcfileUrl),
                            new XAttribute("small", item.jcfileUrls)
                        )
                    );
                }
                shopModel.AdUrl = xmlgg;//根据Files解析
            }
            if (!string.IsNullOrEmpty(shopModel.StrVideoUrl))
            {
                List<ToJsonImg> videoUrls = JsonConvert.DeserializeObject<List<ToJsonImg>>(shopModel.StrVideoUrl);
                XElement xle = new XElement("infos");
                foreach (var item in videoUrls)
                {
                    xle.Add(new XElement("video", new XAttribute("url", item.videoUrl)));
                }
                shopModel.VideoUrl = xle;
            }
            //2018-09-07新加微信logo图片
            if (!string.IsNullOrEmpty(Model.StrWXLogo))
            {
                List<ToJsonImg> imgsWxlogo = JsonConvert.DeserializeObject<List<ToJsonImg>>(Model.StrWXLogo);
                XElement xmlWxlogo = new XElement("infos");
                foreach (var item in imgsWxlogo)
                {
                    xmlWxlogo.Add(
                        new XElement("img",
                            new XAttribute("name", "1.jpg"),
                            new XAttribute("value", item.fileUrl),
                            new XAttribute("small", item.fileUrls)
                        )
                    );
                }
                Model.WXLogo = xmlWxlogo;//根据Files解析
            }
            //判断企业介绍是否为空
            if (string.IsNullOrEmpty(Model.Memo))
            {
                ret.Msg = "企业介绍不能为空！";
            }
            else
            {
                ret = dal.Edit(Model, shopModel);
            }
            return ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }
    }
}
