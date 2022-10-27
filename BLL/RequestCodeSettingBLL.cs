/********************************************************************************

** 作者： 李子巍

** 创始时间：2017-02-14

** 联系方式 :13313318725

** 描述：主要用于码配置的业务逻辑层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Dal;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace BLL
{
    /// <summary>
    /// 主要用于码配置的业务逻辑层
    /// </summary>
    public class RequestCodeSettingBLL
    {
        /// <summary>
        /// 获取配置页第一页的初始信息
        /// </summary>
        /// <param name="requestId">申请码标识</param>
        /// <returns>初始信息</returns>
        public BaseResultModel GetFirstPageData(long requestId)
        {
            FirstData ret = new Dal.RequestCodeSettingDAL().GetFirstPageData(requestId);
            return ToJson.NewModelToJson(ret, ret.isSuccess, ret.Msg);
        }

        /// <summary>
        /// 拆分批次
        /// </summary>
        /// <param name="model">添加的实体</param>
        /// <returns>操作结果</returns>
        public BaseResultModel Add(RequestCodeSetting model)
        {
            string Msg = "";
            int error = 0;
            if (model.BathPartType == 1)
            {
                RequestCodeSetting ret = new Dal.RequestCodeSettingDAL().PartSplit(model, out Msg, out  error);
                return ToJson.NewModelToJson(ret, error.ToString(), Msg);
            }
            else
            {
                RequestCodeSetting ret = new Dal.RequestCodeSettingDAL().PartCustom(model, out Msg, out  error);
                return ToJson.NewModelToJson(ret, error.ToString(), Msg);
            }
        }

        /// <summary>
        /// 获取配置页第二页的初始信息
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>第二页初始化信息</returns>
        public BaseResultModel GetSencondPageData(long subId)
        {
            SecondData result = new SecondData();

            List<EnumList> liShowData = new List<EnumList>();
            liShowData.AddRange(Common.EnumText.EnumToList(typeof(Common.EnumFile.SettingShow)));
            liShowData.AddRange(Common.EnumText.EnumToList(typeof(Common.EnumFile.SettingDisplay)));
            result.liShowData = liShowData;

            List<EnumList> liStyleData = new List<EnumList>();
            liStyleData.AddRange(Common.EnumText.EnumToList(typeof(Common.EnumFile.SettingSkin)));
            result.liStyleData = liStyleData;
            string[] arrID = System.Configuration.ConfigurationManager.AppSettings["TestID"].Split(new char[] { ',' });
            LoginInfo pf = Common.Argument.SessCokie.Get;
            if (Array.IndexOf(arrID, pf.EnterpriseID.ToString()) ==-1)
            {
                var item = liStyleData.FirstOrDefault(p => p.text.Equals("模板五"));
                liStyleData.Remove(item);
            }
            RequestCodeSetting requestSetting = new Dal.RequestCodeSettingDAL().GetSubSetting(subId);
            result.styleId = requestSetting.StyleModel.GetValueOrDefault((int)Common.EnumFile.SettingSkin.Normal);
            result.materialId = requestSetting.MaterialID.GetValueOrDefault(0);
            if (!string.IsNullOrEmpty(requestSetting.DisplayOption))
            {
                string[] ids = requestSetting.DisplayOption.Split(',');
                foreach (var item in liShowData)
                {
                    item.ischeck = ids.Contains(item.value);
                }
            }

            return ToJson.NewModelToJson(result, "0", "");
        }

        /// <summary>
        /// 修改配置信息的拍吗显示项
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <returns>操作结果</returns>
        public BaseResultModel Edit(RequestCodeSetting model)
        {
            string Msg = "";
            int error = 0;
            RequestCodeSetting ret = new Dal.RequestCodeSettingDAL().Edit(model, out Msg, out  error);
            return ToJson.NewModelToJson(ret, error.ToString(), Msg);
        }

        /// <summary>
        /// 修改子码段产品信息
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <returns>操作结果</returns>
        public BaseResultModel EditMaterial(RequestCodeSetting model)
        {
            string Msg = "";
            int error = 0;
            RequestCodeSetting ret = new Dal.RequestCodeSettingDAL().EditMaterial(model, out Msg, out  error);
            return ToJson.NewModelToJson(ret, error.ToString(), Msg);
        }

        /// <summary>
        /// 获取子码段原料列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>原料列表</returns>
        public BaseResultList GetOriginList(long subId)
        {
            List<View_RequestOrigin> result = new Dal.RequestCodeSettingDAL().GetOriginList(subId);
            return ToJson.NewListToJson(result, 0, result.Count(), result.Count(), "");
        }

        /// <summary>
        /// 给子码段添加新原料
        /// </summary>
        /// <param name="model">添加的实体</param>
        /// <returns>操作结果</returns>
        public BaseResultModel AddOrigin(RequestOrigin model)
        {
            RetResult ret = new Dal.RequestCodeSettingDAL().AddOrigin(model);
            return ToJson.NewModelToJson(ret, Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 修改子码段的原料信息
        /// </summary>
        /// <param name="model">原料信息</param>
        /// <returns>操作结果</returns>
        public BaseResultModel EditOrigin(RequestOrigin model)
        {
            RetResult ret = new Dal.RequestCodeSettingDAL().EditOrigin(model);
            return ToJson.NewModelToJson(ret, Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 删除子码段的原料信息
        /// </summary>
        /// <param name="originSettingId">要删除ID</param>
        /// <returns>操作结果</returns>
        public BaseResultModel DelOrigin(long originSettingId)
        {
            RetResult ret = new Dal.RequestCodeSettingDAL().DelOrigin(originSettingId);
            return ToJson.NewModelToJson(ret, Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 获取子码段作业信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段作业信息列表</returns>
        public BaseResultList GetWorkList(long subId)
        {
            List<Batch_ZuoYe> result = new Dal.RequestCodeSettingDAL().GetWorkList(subId);
            return ToJson.NewListToJson(result, 0, result.Count(), result.Count(), "");
        }

        /// <summary>
        /// 获取子码段巡检信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段巡检信息列表</returns>
        public BaseResultList GetCheckList(long subId)
        {
            List<Batch_XunJian> result = new Dal.RequestCodeSettingDAL().GetCheckList(subId);
            return ToJson.NewListToJson(result, 0, result.Count(), result.Count(), "");
        }

        /// <summary>
        /// 获取子码段质检信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段质检信息列表</returns>
        public BaseResultList GetReportList(long subId)
        {
            List<Batch_JianYanJianYi> result = new Dal.RequestCodeSettingDAL().GetReportList(subId);
            return ToJson.NewListToJson(result, 0, result.Count(), result.Count(), "");
        }

        /// <summary>
        /// 根据标识获取实体
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>实体</returns>
        public RequestCodeSetting GetModel(long subId)
        {
            return new Dal.RequestCodeSettingDAL().GetSubSetting(subId);
        }

        /// <summary>
        /// 拆分初始信息
        /// </summary>
        /// <param name="subId">配置表标识</param>
        /// <returns>操作结果</returns>
        public BaseResultModel BatchPartInit(long subId)
        {
            FirstData ret = new Dal.RequestCodeSettingDAL().BatchPartInit(subId);
            return ToJson.NewModelToJson(ret, ret.isSuccess, ret.Msg);
        }

        /// <summary>
        /// 获取存储环境
        /// </summary>
        /// <param name="generateCodeId">申请码标识</param>
        /// <returns>存储环境实体</returns>
        public BaseResultModel GetAmbient(long subId)
        {
            SetAmbient model = new RequestCodeSettingDAL().GetAmbient(subId);
            if (model.StrAddDate != null && model.StrOutDate != null)
            {
                model.StrInDate = model.StrInDate.Replace(" 00:00:00", "");
                model.StrOutDate = model.StrOutDate.Replace(" 00:00:00", "");
            }
            return ToJson.NewModelToJson(model, "1", ""); ;
        }

        /// <summary>
        /// 设置存储环境
        /// </summary>
        /// <param name="model">存储环境实体</param>
        /// <returns>操作结果</returns>
        public BaseResultModel AddAmbient(SetAmbient model)
        {
            RetResult ret = new RequestCodeSettingDAL().AddAmbient(model);
            return ToJson.NewModelToJson(
                ret.CrudCount,
                (Convert.ToInt32(ret.IsSuccess)).ToString(),
                ret.Msg);
        }

        /// <summary>
        /// 获取物流信息
        /// </summary>
        /// <param name="generateCodeId">申请码标识</param>
        /// <returns>物流信息实体</returns>
        public BaseResultModel GetLogistics(long subId)
        {
            SetLogistics model = new RequestCodeSettingDAL().GetLogistics(subId);
            if (model.StrAddDate != null && model.StrEndDate != null && model.StrStartDate != null)
            {
                model.StrAddDate = model.StrAddDate.Replace(" 00:00:00", "");
                model.StrEndDate = model.StrEndDate.Replace(" 00:00:00", "");
                model.StrStartDate = model.StrStartDate.Replace(" 00:00:00", "");
            }
            return ToJson.NewModelToJson(model, "1", "");
        }

        /// <summary>
        /// 设置物流信息
        /// </summary>
        /// <param name="model">物流信息实体</param>
        /// <returns>操作结果</returns>
        public BaseResultModel AddLogistics(SetLogistics model)
        {
            RetResult ret = new RequestCodeSettingDAL().AddLogistics(model);
            return ToJson.NewModelToJson(
                ret.CrudCount,
                (Convert.ToInt32(ret.IsSuccess)).ToString(),
                ret.Msg);
        }

        /// <summary>
        /// 获取生产日期
        /// </summary>
        /// <param name="subId">标记ID</param>
        /// <returns></returns>
        public BaseResultModel GetProductionDate(long subId)
        {
            RequestCodeSettingDAL dal = new RequestCodeSettingDAL();
            RequestCodeSetting productiondate = dal.GetProductionDate(subId);
            string code = "1";
            string msg = "";
            if (productiondate == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(productiondate, code, msg);
            return model;
        }

        /// <summary>
        /// 是否存在相应产品信息【巡检、存储环境等】
        /// </summary>
        /// <param name="settingId">产品id</param>
        /// <returns></returns>
        public int IsExistSettingInfo(long settingId)
        {
            return new RequestCodeSettingDAL().IsExistSettingInfo(settingId);
        }

        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="settingId">配置信息id</param>
        /// <returns></returns>
        public RetResult AddSettingInfo(long settingId)
        {
            return new RequestCodeSettingDAL().AddSettingInfo(settingId);
        }

        /// <summary>
        /// 更新配置红包状态
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public bool UpdatePacketState(long settingId)
        {
            return new RequestCodeSettingDAL().UpdatePacketState(settingId);
        }

        /// <summary>
        ///  码信息管理加备注信息2018-09-14
        /// </summary>
        /// <param name="subId">配置表标识</param>
        /// <returns>操作结果</returns>
        public BaseResultModel SettingMemo(long subId)
        {
            FirstData ret = new Dal.RequestCodeSettingDAL().SettingMemo(subId);
            return ToJson.NewModelToJson(ret, ret.isSuccess, ret.Msg);
        }

        /// <summary>
        /// 添加/编辑该批码备注信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel AddEditMemo(RequestCodeSetting model)
        {
            string Msg = "";
            int error = 0;
            RequestCodeSetting ret = new Dal.RequestCodeSettingDAL().AddEditMemo(model, out Msg, out  error);
            return ToJson.NewModelToJson(ret, error.ToString(), Msg);
        }

        #region 获取模板4图片信息
        /// <summary>
        ///  获取模板4图片信息
        /// </summary>
        /// <param name="rid">RequestCodeSettingID</param>
        /// <returns></returns>
        public BaseResultModel GetMuBanInfo(long rid)
        {
            RequestCodeSettingDAL dal = new RequestCodeSettingDAL();
            RequestCodeSettingMuBan mubanInfo = dal.GetMuBanInfo(rid);
            string code = "1";
            string msg = "";
            if (mubanInfo == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            else
            {
                #region 图片XML转JSON类
                List<ToJsonImg> imgs = new List<ToJsonImg>();
                if (!string.IsNullOrEmpty(mubanInfo.StrMuBanImg))
                {
                    XElement xml = XElement.Parse(mubanInfo.StrMuBanImg);
                    IEnumerable<XElement> allImg = xml.Elements("img");
                    foreach (var item in allImg)
                    {
                        ToJsonImg sub = new ToJsonImg();
                        sub.fileUrl = item.Attribute("value").Value;
                        sub.fileUrls = item.Attribute("small").Value;
                        imgs.Add(sub);
                    }
                }
                mubanInfo.imgs = imgs;
                #endregion
            }
            BaseResultModel model = ToJson.NewModelToJson(mubanInfo, code, msg);
            return model;
        }

        /// <summary>
        /// 修改配置信息的拍吗显示模板
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <returns>操作结果</returns>
        public BaseResultModel EditMubanImg(RequestCodeSetting model,RequestCodeSettingMuBan mubanModel)
        {
            string Msg = "";
            int error = 0;
            if (string.IsNullOrEmpty(mubanModel.StrMuBanImg.Replace("[", "").Replace("]", "")))
            {
                mubanModel.MuBanImg = null;//根据Files解析
            }
            else
            {
                List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(mubanModel.StrMuBanImg);
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
                mubanModel.MuBanImg = xml;//根据Files解析
            }
            RequestCodeSetting ret = new Dal.RequestCodeSettingDAL().EditMubanImg(model, mubanModel, out Msg, out  error);
            return ToJson.NewModelToJson(ret, error.ToString(), Msg);
        }
        #endregion
    }
}
