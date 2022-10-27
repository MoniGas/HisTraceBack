/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-24

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于企业宣传码信息管理数据层

*********************************************************************************/

using System;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class ShowCompanyDAL : DALBase
    {
        /// <summary>
        /// 获取企业宣传数据
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <returns>模型</returns>
        public ShowCompany GetModel(long companyId)
        {
            ShowCompany result = new ShowCompany();
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    result = dataContext.ShowCompany.FirstOrDefault(m => m.CompanyID == companyId);
                    if (result == null)
                    {
                        string companyName = "";
                        string mainCode = "";
                        int Verify = 0;
                        using (DataClassesDataContext webContext = GetDataContext())
                        {
                            Enterprise_Info enterpriseInfo = webContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == companyId);
                            companyName = enterpriseInfo.EnterpriseName;
                            mainCode = enterpriseInfo.MainCode;
                            Verify = (int)enterpriseInfo.Verify;
                        }
                        ShowCompany company = new ShowCompany();
                        company.CompanyID = companyId;
                        company.CompanyName = companyName;
                        company.EWM = mainCode + "." + (int)Common.EnumFile.TerraceEwm.showCompany + "." + companyId.ToString();
                        company.Infos = "";
                        company.TemplateIDs = "1,4";
                        company.TopImgUrl = "";
                        string config = "enterpriseURL";
                        //if (Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                        //{
                        //    config = "ncpURL";
                        //}
                        company.Url = System.Configuration.ConfigurationManager.AppSettings[config] + company.EWM;
                        dataContext.ShowCompany.InsertOnSubmit(company);
                        dataContext.SubmitChanges();

                        result = company;
                    }

                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "ShowCompanyDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 编辑企业宣传内容
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(ShowCompany model)
        {
            Ret.Msg = "修改失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowCompany company = dataContext.ShowCompany.FirstOrDefault(m => m.CompanyID == model.CompanyID);
                    if (company == null)
                    {
                        dataContext.ShowCompany.InsertOnSubmit(model);
                    }
                    else
                    {
                        company.Infos = model.Infos;
                        company.Files = model.Files;
                        company.TopImgUrl = model.TopImgUrl;
                    }
                    using (DataClassesDataContext context = GetDataContext())
                    {
                        Enterprise_Info enterprise = context.Enterprise_Info.Where(p => p.Enterprise_Info_ID == model.CompanyID).FirstOrDefault();
                        if (enterprise != null)
                        {
                            enterprise.Logo = model.Files;
                            enterprise.Memo = model.Infos;
                            context.SubmitChanges();
                        }
                    }
                    dataContext.SubmitChanges();
                    Ret.Msg = "保存成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "ShowCompanyDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
    }
}
