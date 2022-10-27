using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class BrandBLL
    {
        private readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetList(long enterpriseId, string brandName, int pageIndex)
        {
            long totalCount;
            BrandDAL dal = new BrandDAL();
            List<Brand> model = dal.GetList(enterpriseId, brandName, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _pageSize, totalCount, "");
            //string result = JsonConvert.SerializeObject(model);
            return result;
        }
        public string GetJGList(long enterpriseId, string brandName, int pageIndex)
        {
            long totalCount;
            BrandDAL dal = new BrandDAL();
            List<Brand> model = dal.GetJGList(enterpriseId, brandName, out totalCount, pageIndex);
            return ToJson.ListToJson(model, pageIndex, _pageSize, totalCount, "");
        }
        public BaseResultModel Add(Brand model)
        {
            BrandDAL dal = new BrandDAL();
            RetResult ret = new RetResult {CmdError = CmdResultError.EXCEPTION};
            if (string.IsNullOrEmpty(model.BrandName))
            {
                ret.Msg = "品牌名称不能为空！";
            }
            else
            {
                //ToJsonImg imgs = JsonConvert.DeserializeObject<ToJsonImg>(model.Logo);
                //model.Logo = imgs.fileUrl;
                ret = dal.Add(model);
            }
            return ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }
        public BaseResultModel Edit(Brand newModel)
        {
            BrandDAL dal = new BrandDAL();
            //ToJsonImg imgs = JsonConvert.DeserializeObject<ToJsonImg>(newModel.Logo);
            //newModel.Logo = imgs.fileUrl;
            //List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(newModel.Logo);

            //XElement xml = new XElement("infos");

            //foreach (var item in imgs)
            //{
            //    newModel.Logo = item.fileUrl;
            //}
            RetResult ret = dal.Edit(newModel);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public BaseResultModel Del(long brandId, long enterpriseId)
        {
            BrandDAL dal = new BrandDAL();
            RetResult ret = dal.Delete(enterpriseId, brandId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        public string SelectBrand(long enterpriseId, string brandName, int pageIndex)
        {
            long totalCount;
            BrandDAL dal = new BrandDAL();
            List<Brand> model = dal.GetList(enterpriseId, brandName, out totalCount, pageIndex);
            return ToJson.ListToJson(model, pageIndex, _pageSize, totalCount, "");
        }
        public BaseResultModel GetModel(long brandId)
        {
            BrandDAL dal = new BrandDAL();
            Brand model = dal.GetBrandByID(brandId);
            return ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
        }
        #region 区域品牌管理
        //public BaseResultList GetListByRequestBrand(string materialName, int brandEnterpriseStatus, long brandEnterpriseEID, int pageIndex)
        public BaseResultList GetListByRequestBrand(string materialName, long brandEnterpriseEID, int pageIndex)
        {
            long totalCount;
            BrandDAL dal = new BrandDAL();
            List<View_RequestBrand> model = dal.GetListByRequestBrand(materialName, brandEnterpriseEID, out totalCount, pageIndex);
            return ToJson.NewListToJson(model, pageIndex, _pageSize, totalCount, "");
        }
        //20151013张
        /// <summary>
        /// 监管部门审核加入区域品牌审核
        /// </summary>
        /// <param name="materialName"></param>
        /// <param name="brandEnterpriseEID"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList GetListJGBMSHBrand(string searchName,int brandEnterpriseStatus, long brandEID, int pageIndex)
        {
            long totalCount;
            BrandDAL dal = new BrandDAL();
            List<View_RequestBrand> model = dal.GetListJGBMSHBBrand(searchName,brandEnterpriseStatus, brandEID, out totalCount, pageIndex);
            return ToJson.NewListToJson(model, pageIndex, _pageSize, totalCount, "");
        }

        /// <summary>
        /// 申请加入区域品牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel AddBrand_Enterprise(Brand_Enterprise model)
        {
            BrandDAL dal = new BrandDAL();
            RetResult ret = dal.AddBrand_Enterprise(model);
            return ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }
        /// <summary>
        /// 选择品牌（企业申请加入区域品牌）
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="brandName"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList SelectBrandEnterprise(long enterpriseId, string brandName, int pageIndex)
        {
            long totalCount;
            BrandDAL dal = new BrandDAL();
            List<Brand> model = dal.GetSelectList(enterpriseId, brandName, out totalCount, pageIndex);
            return ToJson.NewListToJson(model, pageIndex, _pageSize, totalCount, "");
        }
        /// <summary>
        /// 审核（监管部门）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">根据从前台传值改状态</param>
        /// <returns></returns>
        public BaseResultModel AuditBrand(int id)
        {
            LoginInfo pf = SessCokie.Get;
            BrandDAL dal = new BrandDAL();
            RetResult ret = new RetResult();
            View_RequestBrand vmodel = dal.GetModelViewBE(id);
            if (vmodel == null)
            {
                ret.Msg = "获取信息失败！";
                ret.SetArgument(CmdResultError.EXCEPTION, ret.Msg, ret.Msg);
            }
            if (vmodel.BrandEID != pf.EnterpriseID)
            {
                ret.Msg = "您无权对此信息进行审批！";
                ret.SetArgument(CmdResultError.NO_RIGHT, ret.Msg, ret.Msg);
            }
            else
            {
                ret = dal.AuditStatus(id, pf.EnterpriseID);
                ret.Msg = "审核成功！";
                ret.SetArgument(CmdResultError.NONE, ret.Msg, ret.Msg);
            }
            return ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }
        public string Approval(int id, int type)
        {
            LoginInfo pf = SessCokie.Get;
            BrandDAL dal = new BrandDAL();
            RetResult ret = new RetResult();
            View_RequestBrand vmodel = dal.GetModelViewBE(id);
            if (vmodel == null)
            {
                ret.Msg = "获取信息失败！";
            }
            if (vmodel.BrandEnterpriseEID != pf.EnterpriseID)
            {
                ret.Msg = "您无权对此信息进行审批！";
            }
            if (type == 1)
            {
                RetResult res = dal.UpdateStatus(id, (int)Common.EnumFile.PlatFormState.pass);
                return ToJson.ResultToJson(res);
            }
            else
            {
                RetResult res = dal.UpdateStatus(id, (int)Common.EnumFile.PlatFormState.no_pass);
                return ToJson.ResultToJson(res);
            }
        }
        #endregion
    }
}
