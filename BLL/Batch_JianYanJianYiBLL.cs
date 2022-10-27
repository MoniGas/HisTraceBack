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
    public class Batch_JianYanJianYiBLL
    {

        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long enterpriseId, long batchID, long batchextid, int pageIndex)
        {
            long totalCount = 0;
            Batch_jianYanJianYiDAL dal = new Batch_jianYanJianYiDAL();
            List<View_JianYanJianYiBatchMaterial> model = dal.GetList(enterpriseId, batchID, batchextid, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }
        public BaseResultModel Add(Batch_JianYanJianYi model)
        {
            Batch_jianYanJianYiDAL dal = new Batch_jianYanJianYiDAL();
            if (string.IsNullOrEmpty(model.StrFiles.Replace("[", "").Replace("]", "")))
            {
                model.Files = null;//根据Files解析
            }
            else
            {
                List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(model.StrFiles);
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
                //xml.Add(
                //    new XElement("img",
                //        new XAttribute("name", "1.jpg"),
                //        new XAttribute("value", imgs.fileUrl),
                //        new XAttribute("small", imgs.fileUrls)
                //    )
                //);
                model.Files = xml;//根据Files解析
            }
            RetResult ret = dal.Add(model);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Edit(Batch_JianYanJianYi newModel)
        {
            Batch_jianYanJianYiDAL dal = new Batch_jianYanJianYiDAL();
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(newModel.StrFiles);
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
            //ToJsonImg imgs = JsonConvert.DeserializeObject<ToJsonImg>(newModel.StrFiles);
            //XElement xml = new XElement("infos");
            //string[] temp = newModel.StrFiles.Split('|');
            //xml.Add(
            //    new XElement("img",
            //        new XAttribute("name", "1.jpg"),
            //        new XAttribute("value", temp[0]),
            //        new XAttribute("small", temp[1])
            //    )
            //);
            newModel.Files = xml;
            RetResult ret = dal.Edit(newModel);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Del(long id, long enterpriseId)
        {
            Batch_jianYanJianYiDAL dal = new Batch_jianYanJianYiDAL();
            RetResult ret = dal.Del(id, enterpriseId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel GetModel(long id)
        {
            Batch_jianYanJianYiDAL dal = new Batch_jianYanJianYiDAL();
            Batch_JianYanJianYi model = dal.GetModel(id);

            #region 图片XML转JSON类
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(model.StrFiles))
            {
                XElement xml = XElement.Parse(model.StrFiles);
                ToJsonImg sub = new ToJsonImg();
                sub.fileUrl = xml.Element("img").Attribute("value").Value;
                sub.fileUrls = xml.Element("img").Attribute("small").Value;
                imgs.Add(sub);
            }
            model.imgs = imgs;

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
        public BaseResultModel GetModelView(long id)
        {
            Batch_jianYanJianYiDAL dal = new Batch_jianYanJianYiDAL();
            View_JianYanJianYiBatchMaterial model = dal.GetModelView(id);
            BaseResultModel result = ToJson.NewModelToJson(model, "", "");
            return result;
        }

        public List<View_JianYanJianYiBatchMaterial> GetListByBatchID(long bId, long? extId, int pageIndex, out bool IsHasMore) 
        {
            Batch_jianYanJianYiDAL dal = new Batch_jianYanJianYiDAL();
            List<View_JianYanJianYiBatchMaterial> DataList = dal.GetListByBatchID(bId, extId, pageIndex, out IsHasMore);
            return DataList;
        }
    }
}
