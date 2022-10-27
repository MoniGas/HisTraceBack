using System;
using Common.Argument;
using Dal;
using InterfaceWeb;
using LinqModel;
using System.Configuration;
using Common.Log;
namespace BLL
{
    /// <summary>
    /// 赵慧敏
    /// </summary>
    public class RegistBLL
    {
        #region 农企注册
        /// <summary>
        /// 农企注册（未在IDcode平台注册）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel Regist(string userName, string pwd, string email, string organizeName, string organizeNameEn, long platId, string unitType_ID,
            string province, string selCity, string selArea, string user, string userEn, string tel, string code, string enMainCode)
        {
            try
            {
                RegistDAL dal = new RegistDAL();
                BaseResultModel result = new BaseResultModel();//返回模型
                if (string.IsNullOrEmpty(userName))
                {
                    result = ToJson.NewRetResultToJson("0", "用户名不能为空！");
                    return result;
                }
                if (string.IsNullOrEmpty(pwd))
                {
                    result = ToJson.NewRetResultToJson("0", "密码不能为空！");
                    return result;
                }
                if (string.IsNullOrEmpty(organizeName))
                {
                    result = ToJson.NewRetResultToJson("0", "企业名称不能为空！");
                    return result;
                }
                if (string.IsNullOrEmpty(unitType_ID))
                {
                    result = ToJson.NewRetResultToJson("0", "单位性质不能为空！");
                    return result;
                }
                if (string.IsNullOrEmpty(province))
                {
                    result = ToJson.NewRetResultToJson("0", "请选择省！");
                    return result;
                }
                if (string.IsNullOrEmpty(selCity))
                {
                    result = ToJson.NewRetResultToJson("0", "请选择市！");
                    return result;
                }
                if (string.IsNullOrEmpty(selArea))
                {
                    result = ToJson.NewRetResultToJson("0", "请选择区县！");
                    return result;
                }
                if (string.IsNullOrEmpty(tel))
                {
                    result = ToJson.NewRetResultToJson("0", "IDcode联系人手机不能为空！");
                    return result;
                }
                if (dal.UserName(userName) > 0)
                {
                    result = ToJson.NewRetResultToJson("0", "用户名重复，请重新填写用户名！");
                    return result;
                }
                if (platId > 0)
                {
                    RetResult retCheck = dal.Check(platId);
                    if (retCheck.CmdError != CmdResultError.NONE)
                    {
                        result = ToJson.NewRetResultToJson("0", retCheck.Msg);
                        return result;
                    }
                }
                //IDcode平台注册
                CompanyIDcode idCode = BaseDataDAL.RegCompanyInfo(userName.Trim(), pwd.Trim(), email.Trim(), organizeName.Trim(), organizeNameEn.Trim(), unitType_ID, province, selCity, selArea, user.Trim(), userEn.Trim(), tel, code.Trim());
                if (idCode != null && idCode.result_code != 1)
                {
                    result = ToJson.NewRetResultToJson("0", idCode.result_msg);
                    return result;
                }
                //IDcode注册成功后再农业平台注册
                Enterprise_Info model = new Enterprise_Info
                {
                    Email = email.Trim(),
                    MainCode = idCode.organunit_idcode.Trim(),
                    EnterpriseName = organizeName.Trim(),
                    Dictionary_UnitType_ID = unitType_ID,
                    Dictionary_AddressSheng_ID = Convert.ToInt64(province),
                    Dictionary_AddressShi_ID = Convert.ToInt64(selCity),
                    Dictionary_AddressQu_ID = Convert.ToInt64(selArea),
                    LinkMan = user.Trim(),
                    LinkPhone = tel.Trim(),
                    PRRU_PlatForm_ID = platId,
                    RequestCodeCount = Convert.ToInt32(ConfigurationManager.AppSettings["RequestCodeCount"]),
                    OverDraftCount = Convert.ToInt32(ConfigurationManager.AppSettings["OverDraftCount"]),
                    UsedCodeCount = 0,
                    TraceEnMainCode = enMainCode
                };
                RetResult ret = dal.Regist(model, userName, pwd);
                result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                return result;
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("企业注册BLL1" + ex.Message);
                return null;
            }
        }
        #endregion

        #region 农企注册
        /// <summary>
        /// 农企注册（已在IDcode平台注册）
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="idcode"></param>
        /// <returns></returns>
        public BaseResultModel RegistQuery(string userName, string pwd, string idcode, long platId, string organUnitAddress, string linkMan, string linkPhone, string enMainCode)
        {
            try
            {
                RegistDAL dal = new RegistDAL();
                BaseResultModel result = new BaseResultModel();//返回模型
                if (string.IsNullOrEmpty(userName))
                {
                    result = ToJson.NewRetResultToJson("0", "用户名不能为空！");
                    return result;
                }
                if (string.IsNullOrEmpty(pwd))
                {
                    result = ToJson.NewRetResultToJson("0", "密码不能为空！");
                    return result;
                }
                if (string.IsNullOrEmpty(idcode))
                {
                    result = ToJson.NewRetResultToJson("0", "无法找到要注册的企业主码！");
                    return result;
                }
                OrganUnit unit = BaseDataDAL.GetCompanyBaseInfo(idcode);
                if (unit != null && unit.ResultCode != 1)
                {
                    result = ToJson.NewRetResultToJson("0", unit.ResultMsg);
                    return result;
                }
                if (platId > 0)
                {
                    RetResult retCheck = dal.Check(platId);
                    if (retCheck.CmdError != CmdResultError.NONE)
                    {
                        result = ToJson.NewRetResultToJson("0", retCheck.Msg);
                        return result;
                    }
                }
                //IDcode注册成功后再农业平台注册
                Enterprise_Info model = new Enterprise_Info();
                model.MainCode = idcode;
                model.EnterpriseName = unit.OrganUnitName;
                model.Dictionary_UnitType_ID = unit.UnitType_ID;
                model.Dictionary_AddressSheng_ID = Convert.ToInt64(unit.Province_ID);
                model.Dictionary_AddressShi_ID = Convert.ToInt64(unit.City_ID);
                model.Dictionary_AddressQu_ID = Convert.ToInt64(unit.Area_ID);
                model.Address = unit.OrganUnitAddress;
                model.PRRU_PlatForm_ID = platId;
                model.RequestCodeCount = Convert.ToInt32(ConfigurationManager.AppSettings["RequestCodeCount"]);
                model.OverDraftCount = Convert.ToInt32(ConfigurationManager.AppSettings["OverDraftCount"]);
                model.UsedCodeCount = 0;
                model.Address = organUnitAddress;
                model.LinkMan = linkMan;
                model.LinkPhone = linkPhone;
                model.TraceEnMainCode = enMainCode;
                RetResult ret = dal.Regist(model, userName, pwd);
                result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
                return result;
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("企业注册BLL1:" + ex.Message);
                return null;
            }
        }
        #endregion

        #region 判断农企是否已经在本平台注册
        /// <summary>
        ///判断农企是否已经在本平台注册
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="idcode"></param>
        /// <returns></returns>
        public RetResult RegistModify(string enterpriseName)
        {
            RegistDAL dal = new RegistDAL();
            RetResult result = new RetResult();//返回模型
            result = dal.RegistModify(enterpriseName);
            return result;
        }
        #endregion

        public Result GetVerifyInfo(string phone)
        {
            Result sms = BaseDataDAL.GetVerifyInfo(phone);
            return sms;
        }


        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>结果</returns>
        public Result GetPassWord(string phone, out string code)
        {
            Result result = new Result();
            result = new RegistDAL().GetPassWord(phone, out code);
            return result;
        }

        public BaseResultModel RegisterTry(string pwd, string organizeName, string tel)
        {
            RegistDAL dal = new RegistDAL();
            BaseResultModel result;//返回模型
            if (string.IsNullOrEmpty(pwd))
            {
                result = ToJson.NewRetResultToJson("0", "密码不能为空！");
                return result;
            }
            if (string.IsNullOrEmpty(organizeName))
            {
                result = ToJson.NewRetResultToJson("0", "企业名称不能为空！");
                return result;
            }
            if (string.IsNullOrEmpty(tel))
            {
                result = ToJson.NewRetResultToJson("0", "手机号码不能为空！");
                return result;
            }
            //IDcode注册成功后再农业平台注册
            Enterprise_Info model = new Enterprise_Info
            {
                EnterpriseName = organizeName.Trim(),
                LinkPhone = tel.Trim()
            };
            RetResult ret = dal.RegisterTry(model, tel.Trim(), pwd);
            result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        #region 企业注册接口（企业已通过Idcode验证）winform调用20200108张
        public int AddEnterpriseIdcodeHas(EnterpriseInfoRequest request, string enMainCode, out Enterprise_License licenseModel)
        {
            licenseModel = new Enterprise_License();
            try
            {
                RegistDAL dal = new RegistDAL();
                int result = dal.AddEnterpriseIdcodeHas(request, enMainCode, out licenseModel);
                return result;
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("AddEnterpriseIdcodeHas" + ex.Message);
                return -1;
            }
        }

        public RetEnterpriseResult GetFhInfo(string mainCode)
        {
            RetEnterpriseResult result = new RetEnterpriseResult();
            RegistDAL dal = new RegistDAL();
            result = dal.GetFhInfo(mainCode);
            return result;
        }
        #endregion
        /// <summary>
        /// 修改企业统一社会信用代码
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <param name="BusinessLicence"></param>
        /// <returns></returns>
        public RetResult EditEnterprise(long EnterpriseId, string BusinessLicence)
        {
            RetResult result = new RetResult();
            try
            {
                RegistDAL dal = new RegistDAL();
                result = dal.EditEnterprise(EnterpriseId, BusinessLicence);
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("EditEnterprise" + ex.Message);
            }
            return result;
        }
    }
}
