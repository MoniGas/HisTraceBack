/********************************************************************************
** 作者： 赵慧敏
** 创始时间：2019-5-28
** 联系方式 :13313318725
** 描述：黑名单
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
    public class BackListBLL
    {
        public BaseResultModel GetModel(long eId)
        {
            BackListDAL dal = new BackListDAL();
            BackList model = dal.GetModel(eId);
            List<ToJsonImg> Imgs = new List<ToJsonImg>();
            List<ToJsonProperty> propertys = new List<ToJsonProperty>();
            if (model != null)
            {
                //判断企业Logo是否为空
                if (!string.IsNullOrEmpty(model.StrBackImg))
                {
                    XElement Xml = XElement.Parse(model.StrBackImg);
                    IEnumerable<XElement> Img1 = Xml.Elements("img");
                    foreach (var Item in Img1)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.fileUrl = Item.Attribute("value").Value;
                        sub.fileUrls = Item.Attribute("small").Value;
                        Imgs.Add(sub);
                    }
                }
                model.BackImgs = Imgs;
                #region 属性XML转JSON类
                if (!string.IsNullOrEmpty(model.StrBackCode))
                {
                    XElement xml = XElement.Parse(model.StrBackCode);
                    IEnumerable<XElement> allProperty = xml.Elements("info");
                    foreach (var item in allProperty)
                    {
                        ToJsonProperty sub = new ToJsonProperty();
                        sub.pName = item.Attribute("iname").Value;
                        propertys.Add(sub);
                    }
                }
                model.propertys = propertys;
                #endregion
            }
            else
            {
                model = new BackList();
                model.BackImgs = Imgs;
                model.propertys = propertys;
            }
            return ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
        }
        public BaseResultModel Edit(BackList Model)
        {
            List<ToJsonImg> Imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(Model.StrBackImg);
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
            Model.BackImg = Xml;//根据Files解析
            if (!string.IsNullOrEmpty(Model.StrBackCode))
            {
                Model.BackCode = PropertyJsonToXml(Model.StrBackCode); //根据StrPropertyInfo解析
            }
            BackListDAL Dal = new BackListDAL();
            RetResult Ret = new RetResult();
            Ret.CmdError = CmdResultError.EXCEPTION;
            Ret = Dal.Edit(Model);
            BaseResultModel Result = ToJson.NewRetResultToJson((Convert.ToInt32(Ret.IsSuccess)).ToString(), Ret.Msg);
            return Result;
        }
        private XElement PropertyJsonToXml(string jsonPropertys)
        {
            List<ToJsonProperty> imgs = JsonConvert.DeserializeObject<List<ToJsonProperty>>(jsonPropertys);
            XElement xml = new XElement("infos");
            foreach (var item in imgs)
            {
                xml.Add(
                    new XElement("info",
                        new XAttribute("iname", item.pName)
                    )
                );
            }
            return xml;
        }
    }
}
