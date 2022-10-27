/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-12-19

** 联系方式 :13313318725

** 描述：原料管理业务逻辑 移植

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace BLL
{
    /// <summary>
    /// 原料管理业务逻辑
    /// </summary>
    public class OriginBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取原料信息
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="originName">原料名称</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId, string originName, int pageIndex)
        {
            long totalCount = 0;
            OriginDAL dal = new OriginDAL();
            List<View_Origin> model = dal.GetList(enterpriseId, originName, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 添加原料信息
        /// </summary>
        /// <param name="model">原料实体类</param>
        /// <returns></returns>
        public BaseResultModel Add(Origin model)
        {
            OriginDAL dal = new OriginDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.OriginName))
            {
                ret.Msg = "原材料名称不能为空！";
            }
            else if (string.IsNullOrEmpty(model.StrOriginImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传原材料照片！";
            }
            else
            {
                if (!string.IsNullOrEmpty(model.StrOriginImgInfo))
                {
                    model.OriginImgInfo = ImgJsonToXml(model.StrOriginImgInfo); ;//根据StrOriginImgInfo解析
                }
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 修改原料信息
        /// </summary>
        /// <param name="newModel">要修改的原料实体类</param>
        /// <returns></returns>
        public BaseResultModel Edit(Origin newModel)
        {
            OriginDAL dal = new OriginDAL();
            RetResult ret = new RetResult(); ;
            if (string.IsNullOrEmpty(newModel.OriginName))
            {
                ret.Msg = "原材料名称不能为空！";
            }
            else if (string.IsNullOrEmpty(newModel.StrOriginImgInfo.Replace("[", "").Replace("]", "")))
            {
                ret.Msg = "请上传原材料照片！";
            }
            else
            {
                if (!string.IsNullOrEmpty(newModel.StrOriginImgInfo))
                {
                    newModel.OriginImgInfo = ImgJsonToXml(newModel.StrOriginImgInfo);//根据StrOriginImgInfo解析
                }
                ret = dal.Edit(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除原料信息
        /// </summary>
        /// <param name="originId">原料ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultModel Del(long originId, long enterpriseId)
        {
            OriginDAL dal = new OriginDAL();
            RetResult ret = dal.Delete(originId, enterpriseId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取原料实体类信息
        /// </summary>
        /// <param name="originId">原料ID</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long originId)
        {
            OriginDAL dal = new OriginDAL();
            Origin model = dal.GetOriginByID(originId);
            #region 图片XML转JSON类
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(model.StrOriginImgInfo))
            {
                XElement xml = XElement.Parse(model.StrOriginImgInfo);
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    imgs.Add(sub);
                }
            }
            model.imgs = imgs;
            #endregion
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        /// <summary>
        /// Json转Xml
        /// </summary>
        /// <param name="jsonImgs">json串</param>
        /// <returns></returns>
        private XElement ImgJsonToXml(string jsonImgs)
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

        /// <summary>
        /// 获取原料列表
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetOriginList(long eId)
        {
            OriginDAL dal = new OriginDAL();
            List<Origin> origin = dal.GetOriginList(eId);
            return ToJson.NewListToJson(origin, 1, 100000, origin.Count, "");
        }

         /// <summary>
        /// 获取自动搜索数据
        /// </summary>
        /// <param name="page">页面</param>
        /// <param name="flag">字段</param>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public BaseResultList GetDictoryKey(string page, int flag, string value,long enterpriseId)
        {
            OriginDAL dal = new OriginDAL();
            List<Dictorynary_Key> dicKey = dal.GetDictoryKey(page, flag, value,enterpriseId);
            return ToJson.NewListToJson(dicKey, 1, 10, dicKey.Count, "");
        }
    }
}
