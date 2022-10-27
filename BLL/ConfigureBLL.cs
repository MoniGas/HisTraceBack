/********************************************************************************

** 作者： 郭心宇

** 创始时间：2015-12-15

** 联系方式 :13313318725

** 描述：主要用于配置宣传码信息的业务逻辑

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Dal;
using Common.Argument;
using System.Xml.Linq;

namespace BLL
{
    public class ConfigureBLL
    {
        /// <summary>
        /// 获取企业品牌信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>企业品牌信息列表</returns>
        public BaseResultList GetBrandList(long Id)
        {
            ConfigureDAL ObjConfigureDAL = new ConfigureDAL();
            List<Brand> DataList = ObjConfigureDAL.GetBrandList(Id);
            return ToJson.NewListToJson(DataList, 1, 100000, DataList.Count, "");
        }

        /// <summary>
        /// 获取品牌实体
        /// </summary>
        /// <param name="id">品牌ID</param>
        /// <returns></returns>
        public Brand GetBrandModel(long Id)
        {
            return new Dal.ConfigureDAL().GetBrandModel(Id);
        }

        /// <summary>
        /// 获取企业员工信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>企业员工信息列表</returns>
        public BaseResultList GetUserList(long Id)
        {
            ConfigureDAL ObjConfigureDAL = new ConfigureDAL();
            List<ShowUser> DataList = ObjConfigureDAL.GetUserList(Id);
            return ToJson.NewListToJson(DataList, 1, 100000, DataList.Count, "");
        }

        /// <summary>
        /// 获取员工实体
        /// </summary>
        /// <param name="id">员工ID</param>
        /// <returns></returns>
        public ShowUser GetUserModel(long Id)
        {
            return new Dal.ShowUserDAL().GetModel(Id);
        }

        /// <summary>
        /// 获取企业新闻信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>企业新闻信息列表</returns>
        public BaseResultList GetNewsList(long Id)
        {
            ConfigureDAL ObjConfigureDAL = new ConfigureDAL();
            List<ShowNews> DataList = ObjConfigureDAL.GetNewsList(Id);
            return ToJson.NewListToJson(DataList,1,100000,DataList.Count,"");
        }

        /// <summary>
        /// 获取新闻实体
        /// </summary>
        /// <param name="id">新闻ID</param>
        /// <returns></returns>
        public ShowNews GetNewsModel(long Id)
        {
            return new Dal.ConfigureDAL().GetNewsModel(Id);
        }

        /// <summary>
        /// 获取企业配置信息
        /// </summary>
        /// <param name="CompanyId">企业ID</param>
        /// <returns>企业配置信息模型</returns>
        public BaseResultModel GetModel(long CompanyId)
        {
            ConfigureDAL ObjConfigureDAL = new ConfigureDAL();
            Configure Data = ObjConfigureDAL.GetModel(CompanyId);
            return ToJson.NewModelToJson(Data, Data == null ? "0" : "1", "");
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="CompanyId">企业ID</param>
        /// <returns>企业信息模型</returns>
        public BaseResultModel GetCompanyModel(long CompanyId)
        {
            ConfigureDAL ObjConfigureDAL = new ConfigureDAL();
            ShowCompany Model = ObjConfigureDAL.GetCompanyModel(CompanyId);

            List<ToJsonImg> Imgs = new List<ToJsonImg>();
            //判断StrFiles是否为空
            if (!string.IsNullOrEmpty(Model.StrFiles))
            {
                XElement Xml = XElement.Parse(Model.StrFiles);
                IEnumerable<XElement> AllImg = Xml.Elements("img");
                foreach (var Item in AllImg)
                {
                    ToJsonImg Sub = new ToJsonImg();
                    Sub.fileUrl = Item.Attribute("value").Value;
                    Sub.fileUrls = Item.Attribute("small").Value;
                    Imgs.Add(Sub);
                }
            }
            Model.imgs = Imgs;
            return ToJson.NewModelToJson(Model, Model == null ? "0" : "1", "");
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="ObjConfigure">Configure表实体</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="UserIdArray">员工ID数组</param>
        /// <param name="NewsIdArray">新闻ID数组</param>
        /// <param name="BrandIdArray">品牌ID数组</param>
        /// <returns></returns>
        public BaseResultModel Update(Configure ObjConfigure, long CompanyId, string UserIdArray, string NewsIdArray, string BrandIdArray)
        {
            string TempUserId = "";
            string TempNewsId = "";
            string TempBrandId = "";
            TempUserId += UserIdArray;
            TempNewsId += NewsIdArray;
            TempBrandId += BrandIdArray;
            ConfigureDAL ObjConfigureDAL = new ConfigureDAL();
            RetResult ObjRetResult = ObjConfigureDAL.Update(ObjConfigure, CompanyId, TempUserId, TempNewsId, TempBrandId);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        } 
    }
}
