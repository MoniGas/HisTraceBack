/********************************************************************************
** 作者： 张翠霞

** 创始时间：2017-05-25

** 联系方式 :15031109901

** 描述：图片视频上传业务层

** 版本：v2.0

** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Common.Argument;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Configuration;
using Dal;

namespace BLL
{
    /// <summary>
    /// 图片视频上传业务层
    /// </summary>
    public class UploadsBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取上传图片视频列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetList(long enterpriseId, string materialName, int pageIndex)
        {
            long totalCount = 0;
            UploadsDAL dal = new UploadsDAL();
            List<Uploads> model = dal.GetList(enterpriseId, materialName, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 上传图片视频
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Add(Uploads model, string video)
        {
            UploadsDAL dal = new UploadsDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.StrImgInfo.Replace("[", "").Replace("]", ""))
              && string.IsNullOrEmpty(video.Replace("[", "").Replace("]", "")))
            {
                model.ImgInfo = null;//根据Files解析
            }
            else
            {
                List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(model.StrImgInfo);
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
                model.ImgInfo = xml;//根据Files解析
            }
            if (string.IsNullOrEmpty(model.StrImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传照片！";
            }
            ret = dal.Add(model);
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="materialId">上传标识</param>
        /// <returns>操作结果</returns>
        public BaseResultModel GetModel(long uploadsID)
        {
            UploadsDAL dal = new UploadsDAL();
            Uploads uploads = dal.GetModel(uploadsID);
            #region 图片视频XML转JSON类
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            List<ToJsonImg> videos = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(uploads.StrImgInfo))
            {
                XElement xml = XElement.Parse(uploads.StrImgInfo);
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    imgs.Add(sub);
                }
            }
            if (!string.IsNullOrEmpty(uploads.StrImgInfo))
            {
                XElement xml = XElement.Parse(uploads.StrImgInfo);
                IEnumerable<XElement> allImg = xml.Elements("video");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.videoUrl = item.Attribute("value").Value;
                    sub.videoUrls = item.Attribute("small").Value;
                    videos.Add(sub);
                }
            }
            uploads.imgs = imgs;
            uploads.videos = videos;
            #endregion
            string code = "1";
            string msg = "";
            if (uploads == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(uploads, code, msg);
            return model;
        }

        /// <summary>
        /// 修改上传信息
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Edit(Uploads newModel, string video)
        {
            UploadsDAL dal = new UploadsDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(newModel.StrImgInfo);
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
            newModel.ImgInfo = xml;//根据Files解析
            if (string.IsNullOrEmpty(newModel.StrImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传照片！";
            }
            else
            {
                ret = dal.Edit(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
    }
}
