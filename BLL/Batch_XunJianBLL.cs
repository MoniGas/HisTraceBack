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
    public class Batch_XunJianBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        public BaseResultList GetList(long enterpriseId, long batchID, long batchextid, int pageIndex)
        {
            Batch_XunJianDAL objBatch_XunJianDAL = new Batch_XunJianDAL();
            long totalCount = 0;
            List<View_XunJianBatchMaterial> dataList = objBatch_XunJianDAL.GetList(enterpriseId, batchID, batchextid, out totalCount, pageIndex);
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

        public BaseResultModel Add(Batch_XunJian model, string video)
        {
            Batch_XunJianDAL objBatch_XunJianDAL = new Batch_XunJianDAL();
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

            RetResult objRetResult = objBatch_XunJianDAL.Add(model);

            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(objRetResult.IsSuccess)).ToString(), objRetResult.Msg);
            return result;
        }

        public BaseResultModel Edit(Batch_XunJian model, string video)
        {
            Batch_XunJianDAL objBatch_XunJianDAL = new Batch_XunJianDAL();
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
            RetResult objRetResult = objBatch_XunJianDAL.Edit(model);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(objRetResult.IsSuccess)).ToString(), objRetResult.Msg);

            return result;
        }

        public BaseResultModel Del(long id, long eID)
        {
            Batch_XunJianDAL objBatch_XunJianDAL = new Batch_XunJianDAL();

            RetResult objRetResult = objBatch_XunJianDAL.Del(id, eID);

            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(objRetResult.IsSuccess)).ToString(), objRetResult.Msg);
            return result;
        }

        public BaseResultModel GetModel(long id)
        {
            Batch_XunJianDAL objBatch_XunJianDAL = new Batch_XunJianDAL();

            LinqModel.Batch_XunJian data = objBatch_XunJianDAL.GetModel(id);
            #region 图片XML转JSON类
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            List<ToJsonImg> videos = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(data.StrFiles))
            {
                XElement xml = XElement.Parse(data.StrFiles);
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    imgs.Add(sub);
                }
            }
            if (!string.IsNullOrEmpty(data.StrFiles))
            {
                XElement xml = XElement.Parse(data.StrFiles);
                IEnumerable<XElement> allImg = xml.Elements("video");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.videoUrl = item.Attribute("value").Value;
                    sub.videoUrls = item.Attribute("small").Value;
                    videos.Add(sub);
                }
            }
            data.imgs = imgs;
            data.videos = videos;
            #endregion

            string code = "1";
            string msg = "";
            if (data == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel result = ToJson.NewModelToJson(data, code, msg);

            return result;
        }

        public string GetModelView(long id)
        {
            Batch_XunJianDAL objBatch_XunJianDAL = new Batch_XunJianDAL();

            LinqModel.View_XunJianBatchMaterial data = objBatch_XunJianDAL.GetModelView(id);

            return ToJson.ModelToJson(data, 1, "");
        }

        public List<View_XunJianBatchMaterial> GetListByBatchID(long bId, long? extId, int pageIndex, out bool IsHasMore)
        {
            Batch_XunJianDAL objBatch_XunJianDAL = new Batch_XunJianDAL();

            List<LinqModel.View_XunJianBatchMaterial> DataList = objBatch_XunJianDAL.GetListByBatchID(bId, extId, pageIndex, out IsHasMore);

            return DataList;
        }
    }
}
