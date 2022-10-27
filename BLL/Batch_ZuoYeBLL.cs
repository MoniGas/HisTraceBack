using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using Common.Argument;
using Dal;
using LinqModel;
using Newtonsoft.Json;

namespace BLL
{
    public class Batch_ZuoYeBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long enterpriseId, long batchID, long batchextid, long opid, int type, int pageIndex)
        {
            Batch_ZuoYeDAL zuoyedal = new Batch_ZuoYeDAL();
            long totalCount = 0;
            List<View_ZuoYeBatchMaterial> dataList = zuoyedal.GetList(enterpriseId, batchID, batchextid, opid, type, out totalCount, pageIndex);

            if (dataList.Count > 0)
            {
                foreach (var items in dataList)
                {
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    if (!string.IsNullOrEmpty(items.StrFiles))
                    {
                        XElement xml = XElement.Parse(items.StrFiles);
                        IEnumerable<XElement> allImg = xml.Elements("img");
                        foreach (var item in allImg)
                        {
                            ToJsonImg sub = new ToJsonImg();
                            sub.fileUrl = item.Attribute("value").Value;
                            sub.fileUrls = item.Attribute("small").Value;
                            imgs.Add(sub);
                        }
                    }
                    items.imgs = imgs;
                }
            }
            BaseResultList result = ToJson.NewListToJson(dataList, pageIndex, PageSize, totalCount, "");
            return result;
        }
        //public string GetModelView(long id)
        //{
        //    Batch_ZuoYeDAL dal = new Batch_ZuoYeDAL();
        //    View_ZuoYeBatchMaterial model = dal.GetModelView(id);
        //    string result = ToJson.ModelToJson(model, 1, "");
        //    return result;
        //}
        public BaseResultModel GetModel(long id)
        {
            Batch_ZuoYeDAL dal = new Batch_ZuoYeDAL();
            Batch_ZuoYe model = dal.GetModel(id);
            #region 图片XML转JSON类
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            List<ToJsonImg> videos = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(model.StrFiles))
            {
                XElement xml = XElement.Parse(model.StrFiles);
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    imgs.Add(sub);
                }
            }
            if (!string.IsNullOrEmpty(model.StrFiles))
            {
                XElement xml = XElement.Parse(model.StrFiles);
                IEnumerable<XElement> allImg = xml.Elements("video");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.videoUrl = item.Attribute("value").Value;
                    sub.videoUrls = item.Attribute("small").Value;
                    videos.Add(sub);
                }
            }
            model.imgs = imgs;
            model.videos = videos;
            #endregion
            string code = "1";
            string msg = "";
            if (model == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel result = ToJson.NewModelToJson(model, code, msg);

            return result;
        }
        public BaseResultModel Del(long id, long eID)
        {
            Batch_ZuoYeDAL dal = new Batch_ZuoYeDAL();
            RetResult ret = dal.Del(id, eID);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Edit(Batch_ZuoYe model, string video)
        {
            Batch_ZuoYeDAL dal = new Batch_ZuoYeDAL();
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(model.StrFiles);
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
            model.Files = xml;//根据Files解析
            RetResult ret = dal.Edit(model);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Add(Batch_ZuoYe model, string video)
        {
            Batch_ZuoYeDAL dal = new Batch_ZuoYeDAL();
            if (string.IsNullOrEmpty(model.StrFiles.Replace("[", "").Replace("]", ""))
                && string.IsNullOrEmpty(video.Replace("[", "").Replace("]", "")))
            {
                model.Files = null;//根据Files解析
            }
            else
            {
                List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(model.StrFiles);
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
                model.Files = xml;//根据Files解析
            }
            RetResult ret = dal.Add(model);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        public bool IsHasData(long bid, int type, long? extId)
        {
            Batch_ZuoYeDAL zuoyedal = new Batch_ZuoYeDAL();

            bool flag = zuoyedal.IsHasData(bid, type, extId);

            return flag;
        }

        public List<View_ZuoYeAndZuoYeType> GetList(long bId, long? extId, int type, int pageIndex, out bool IsHasMore)
        {
            Batch_ZuoYeDAL zuoyedal = new Batch_ZuoYeDAL();

            List<View_ZuoYeAndZuoYeType> DataList = zuoyedal.GetList(bId, extId, type, pageIndex, out IsHasMore);

            return DataList;
        }
    }
}
