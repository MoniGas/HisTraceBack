using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    public class SysJYEnterpriseBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        public PagedList<View_DealerUser> GetJYEnterpriseList(string sName, int? pageIndex)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            long totalCount = 0;
            PagedList<View_DealerUser> dataList = dal.GetJYEnterpriseList(sName, pageIndex, out totalCount);
            return dataList;
        }

        public BaseResultModel Add(Dealer dealer, Enterprise_User dealerUser)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            RetResult result = dal.Add(dealer, dealerUser);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public View_DealerUser GetModelInfo(long dId)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            View_DealerUser data = dal.GetModelInfo(dId);
            return data;
        }

        public BaseResultModel Edit(Dealer dealer, string loginName, string pwd)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            RetResult result = dal.Edit(dealer, loginName, pwd);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }
        public RetResult EditJYEnStatus(long id, int type)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            RetResult result = new RetResult();
            result = dal.EditJYEnStatus(id, type);
            return result;
        }

        #region 20210511经营企业接口
        public BaseResultModel JYEnLogin(string loginName, string pwd)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            BaseResultModel result = dal.JYEnLogin(loginName, pwd);
            result = ToJson.NewModelToJson(result.ObjModel, result.code, result.Msg);
            return result;
        }

        public RetResult AddJYMaterial(JYMaterial model, JYMaterialDI modelDI)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.MaterialName))
            {
                ret.Msg = "产品名称不能为空！";
            }
            ret = dal.AddJYMaterial(model, modelDI);
            return ret;
        }

        /// <summary>
        /// 经营企业打码客户端获取DI信息
        /// </summary>
        /// <param name="context"></param>
        public BaseResultList GetJYMaterialDI(long dealerId, string materialName, string categoryCode)
        {
            BaseResultList result = new BaseResultList();
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            List<JYMaterialDI> list = dal.GetJYDIList(dealerId, materialName, categoryCode); ;
            result = ToJson.NewListToJson(list, 0, 0, (long)list.Count, "");
            return result;
        }

        public RetResult AddJYPIInfo(JYRequestCode rModel, string materialName)
        {
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            ret = dal.AddJYPIInfo(rModel, materialName);
            return ret;
        }

        public BaseResultList GetJYPIList(long dealerId, string starDate, string endDate)
        {
            BaseResultList result = new BaseResultList();
            SysJYEnterpriseDAL dal = new SysJYEnterpriseDAL();
            List<JYRequestCode> list = dal.GetJYPIList(dealerId, starDate, endDate); ;
            result = ToJson.NewListToJson(list, 0, 0, (long)list.Count, "");
            return result;
        }
        #endregion
    }
}
