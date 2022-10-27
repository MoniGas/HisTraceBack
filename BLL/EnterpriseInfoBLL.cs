/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-12

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于修改企业信息业务层

*********************************************************************************/
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
    public class EnterpriseInfoBLL
    {
        //int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        private readonly string _ftpPath = ConfigurationManager.AppSettings["FtpPath"];
        private readonly string _ftpUrl = ConfigurationManager.AppSettings["HttpUrl"];
        /// <summary>
        /// 获取企业信息方法
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>返回企业信息的json字符串</returns>
        public BaseResultModel GetModelView(long id)
        {
            EnterpriseInfoDAL objEnterpriseInfoDal = new EnterpriseInfoDAL();
            View_EnterprisePlatForm data = objEnterpriseInfoDal.GetModelView(id);
            //if (data.IsOpenShop)
            //{
            //    data.Success = "开通商城";
            //    data.Wrong = "关闭商城";
            //}
            //else
            //{
            //    data.Success = "关闭商城";
            //    data.Wrong = "开通商城";
            //}
            return ToJson.NewModelToJson(data, data == null ? "0" : "1", "");
        }
        /// <summary>
        /// 修改企业信息方法
        /// </summary>
        /// <param name="myinfo">企业信息实体对象</param>
        /// <returns>返回操作结果的json字符串</returns>
        public BaseResultModel Edit(string mainCode,long eId, string trade, string etrade, string personName, string telephone, string email, string address, string memo, string webUrl, string zjType, string zjhm, string logo, string file)
        {
            EnterpriseInfoDAL objEnterpriseInfoDal = new EnterpriseInfoDAL();
            EnterpriseShopLink enKHD = new EnterpriseShopLink();
            enKHD = new SysEnterpriseManageBLL().GetEnKhd(eId);
            //IDcode平台注册
            //Result idCode = InterfaceWeb.BaseDataDAL.ModifyCompanyInfo(mainCode, etrade, address, email, personName, telephone, file);
            HisResult idCode = InterfaceWeb.BaseDataDAL.ModifyEnInfo(mainCode, etrade, address, email, personName, telephone, file,enKHD.access_token,enKHD.access_token_code);
            if (idCode.result_code != 1) //if (idCode != null)
            {
                return ToJson.NewRetResultToJson(idCode.result_code.ToString(), idCode.result_msg);
            }
            string path = file;//System.Web.HttpContext.Current.Server.MapPath(file);
            if (!string.IsNullOrWhiteSpace(path)&&path.Contains(_ftpUrl))
            {
                path = path.Replace(_ftpUrl, _ftpPath);//转换成物理地址
               
            }
            Result result = InterfaceWeb.BaseDataDAL.Verify(mainCode, path, Convert.ToInt32(zjType), zjhm);

            if (result.ResultCode != 1)
            {
                return ToJson.NewRetResultToJson(result.ResultCode.ToString(), result.ResultMsg);
            }
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(logo);
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
            RetResult ret = objEnterpriseInfoDal.Edit(mainCode, personName, telephone, email, address, memo, webUrl, xml, file, trade, etrade);
            return ToJson.NewRetResultToJson(ret.CmdError.ToString(), ret.Msg);
        }
        /// <summary>
        /// 企业认证
        /// </summary>
        /// <param name="idcode"></param>
        /// <param name="fileurl"></param>
        /// <returns></returns>
        public BaseResultModel Verify(string idcode, string fileurl, int zjType, string zjhm)
        {
            EnterpriseInfoDAL dal = new EnterpriseInfoDAL();
            BaseResultModel objRetResult = new BaseResultModel();
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(fileurl);
            //XElement xml = new XElement("infos");
            foreach (var item in imgs)
            {
                fileurl = item.fileUrl;
            }
            string path = System.Web.HttpContext.Current.Server.MapPath(fileurl);
            Result result = InterfaceWeb.BaseDataDAL.Verify(idcode, path, zjType, zjhm);
            if (result != null)
            {
                objRetResult = ToJson.NewRetResultToJson(result.ResultCode.ToString(), result.ResultMsg);
                if (result.ResultCode == 1)
                {
                    RetResult ret = dal.Edit(idcode, fileurl);
                }
            }
            return objRetResult;
        }

        public Enterprise_Info GetModel(long EnterpriseId)
        {
            EnterpriseInfoDAL objEnterpriseInfoDal = new EnterpriseInfoDAL();

            Enterprise_Info dataModel = objEnterpriseInfoDal.GetModel(EnterpriseId);
            if (dataModel != null)
            {
                List<ToJsonImg> imgs = new List<ToJsonImg>();
                //判断企业Logo是否为空
                if (!string.IsNullOrEmpty(dataModel.StrLogo))
                {
                    XElement xml = XElement.Parse(dataModel.StrLogo);
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
                dataModel.imgs = imgs;

                //判断企业wxLogo是否为空
                List<ToJsonImg> wxImgs = new List<ToJsonImg>();
                if (!string.IsNullOrEmpty(dataModel.StrWXLogo))
                {
                    XElement xml = XElement.Parse(dataModel.StrWXLogo);
                    IEnumerable<XElement> allImg = xml.Elements("img");
                    foreach (var item in allImg)
                    {
                        ToJsonImg sub = new ToJsonImg
                        {
                            fileUrl = item.Attribute("value").Value,
                            fileUrls = item.Attribute("small").Value
                        };
                        wxImgs.Add(sub);
                    }
                }
                dataModel.wxlogoimgs = wxImgs;
            }
            return dataModel;
        }
        /// <summary>
        /// 开通（关闭）商城
        /// </summary>
        /// <param name="mainCode">企业主码</param>
        /// <returns>返回结果</returns>
        public BaseResultModel OpenShop(string mainCode, string accountNum, string accountName, string linkPhone)
        {
            RetResult result = new EnterpriseInfoDAL().OpenShop(mainCode, accountNum, accountName, linkPhone);
            var objRetResult = ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
            return objRetResult;
        }

        public PRRU_PlatForm GetPRRU_PlatForm(long PRRU_PlatForm_ID)
        {
            PRRU_PlatForm objPrruPlatForm = new EnterpriseInfoDAL().GetPRRU_PlatForm(PRRU_PlatForm_ID);
            return objPrruPlatForm;
        }

        public Enterprise_Info GetModel(string mainCode)
        {
            return new EnterpriseInfoDAL().GetModel(mainCode);
        }

        /// <summary>
        /// 获取企业主码简码20180815
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="enCode">简码主码4位</param>
        /// <returns></returns>
        public BaseResultModel EditEnJMainCode(long eid,string enCode)
        {
            RetResult ret = new EnterpriseInfoDAL().EditEnJMainCode(eid, enCode);
            return ToJson.NewRetResultToJson(ret.CmdError.ToString(), ret.Msg);
        }

        /// <summary>
        /// 获取企业基本信息
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <returns></returns>
        public BaseResultModel GetEnterpriseModel(long enterpriseID)
        {
            Enterprise_Info dataModel = new EnterpriseInfoDAL().GetModel(enterpriseID);
            string code = "1";
            string msg = "";
            if (dataModel == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            var model = ToJson.NewModelToJson(dataModel, code, msg);
            return model;
        }

        #region 20200811获取令牌国药监令牌
        public TokenInfo GetGYJToken(string mainCode, string appId, string appSecret)
        {
            try
            {
                TokenInfo result = new TokenInfo();
                EnterpriseInfoDAL dal = new EnterpriseInfoDAL();
                result = dal.GetGYJToken(mainCode, appId, appSecret);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion


        public String GetEnterpriseIsExpired(long enterpriseID)
        {
            string result = new Dal.SysEnterpriseManageDAL().GetEnterpriseIsExpired(enterpriseID);
            return result;
        }
    }
}
