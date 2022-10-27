using System;
using System.Configuration;
using Common.Argument;
using Dal;
using Webdiyer.WebControls.Mvc;
using LinqModel;

namespace BLL
{
    public class SysSupervisionBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        public PagedList<View_PRRU_PlatFormUser> GetEnterpriseList(string sName, string selStatus, int? pageIndex) 
        {
            SysSupervisionDAL objSysSupervisionDAL = new SysSupervisionDAL();
            long totalCount = 0;
            PagedList<View_PRRU_PlatFormUser> dataList = objSysSupervisionDAL.GetEnterpriseList(sName, selStatus, pageIndex, out totalCount);
            return dataList;
        }

        public LinqModel.BaseResultModel Add(LinqModel.PRRU_PlatForm objPRRU_PlatForm, LinqModel.PRRU_PlatForm_User objobjPRRU_PlatForm_User) 
        {
            SysSupervisionDAL objSysSupervisionDAL = new SysSupervisionDAL();
            RetResult result = objSysSupervisionDAL.Add(objPRRU_PlatForm, objobjPRRU_PlatForm_User);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public LinqModel.BaseResultModel Edit(LinqModel.PRRU_PlatForm objPRRU_PlatForm)
        {
            SysSupervisionDAL objSysSupervisionDAL = new SysSupervisionDAL();
            RetResult result = objSysSupervisionDAL.Edit(objPRRU_PlatForm);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public LinqModel.BaseResultModel UpdatePass(long id, string oldpwd, string newpwd, string surepwd)
        {
            LinqModel.BaseResultModel result = new LinqModel.BaseResultModel();

            if (string.IsNullOrEmpty(oldpwd))
            {
                result.code = "0";
                result.Msg = "更新失败！旧密码不能为空！";
            }
            else if (string.IsNullOrEmpty(newpwd))
            {
                result.code = "0";
                result.Msg = "更新失败！新密码不能为空！";
            }
            else if (string.IsNullOrEmpty(surepwd))
            {
                result.code = "0";
                result.Msg = "更新失败！请确认新密码！";
            }
            else if (!newpwd.Equals(surepwd))
            {
                result.code = "0";
                result.Msg = "更新失败！两次输入的密码不一致！";
            }
            else
            {
                SysSupervisionDAL dal = new SysSupervisionDAL();
                RetResult ret = dal.UpdatePass(id, oldpwd, newpwd, surepwd);
                result = ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
            }
            return result;
        }

        public long? GetMaxId() 
        {
            SysSupervisionDAL dal = new SysSupervisionDAL();
            return dal.GetMaxId();
        }

        public PRRU_PlatForm GetModelInfo(long eId)
        {
            SysSupervisionDAL dal = new SysSupervisionDAL();
            LinqModel.PRRU_PlatForm data = dal.GetModelInfo(eId);
            return data;
        }

        public View_PRRU_PlatFormUser GetPlatInfo(long eId)
        {
            SysSupervisionDAL dal = new SysSupervisionDAL();
            LinqModel.View_PRRU_PlatFormUser data = dal.GetPlatInfo(eId);
            return data;
        }
        public RetResult DisableSupervision(long userId)
        {
            SysSupervisionDAL dal = new SysSupervisionDAL();
            RetResult result = dal.DisableSupervision(userId);
            return result;
        }

        public RetResult EnableSupervision(long userId)
        {
            SysSupervisionDAL dal = new SysSupervisionDAL();
            RetResult result = dal.EnableSupervision(userId);
            return result;
        }

        public RetResult ResetPassword(long userId)
        {
            SysSupervisionDAL dal = new SysSupervisionDAL();
            RetResult result = dal.ResetPassword(userId);
            return result;
        }

        public PRRU_PlatForm_User GetPUser(long eId)
        {
            SysSupervisionDAL dal = new SysSupervisionDAL();
            LinqModel.PRRU_PlatForm_User data = dal.GetPUser(eId);
            return data;
        }
    }
}
