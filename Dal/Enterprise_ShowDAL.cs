/********************************************************************************

** 作者： 郭心宇

** 创始时间：2016-1-5

** 联系方式 :13313318725

** 描述：主要用于宣传模块“我的介绍”信息的数据访问

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    public class Enterprise_ShowDAL : DALBase
    {
        /// <summary>
        /// 获取企业信息模型
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public View_EnterpriseShow GetModelView(long Id)
        {
            View_EnterpriseShow Data = new View_EnterpriseShow();
            using (DataClassesDataContext DataContext = GetDataContext())
            {
                try
                {
                    Data = (from D in DataContext.View_EnterpriseShow
                            where D.Enterprise_Info_ID == Id
                            select D).FirstOrDefault();
                    //判断视图中是否有对应数据
                    if (Data == null)
                    {
                        string CompanyName = "";
                        string MainCode = "";
                        int Verify = 0;
                        using (DataClassesDataContext WebContext = GetDataContext())
                        {
                            Enterprise_Info enterpriseInfo = WebContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == Id);
                            CompanyName = enterpriseInfo.EnterpriseName;
                            MainCode = enterpriseInfo.MainCode;
                            Verify = (int)enterpriseInfo.Verify;
                        }
                        using (DataClassesDataContext ShowContext = GetDataContext("ShowConnect"))
                        {
                            ShowCompany Company = new ShowCompany();
                            Company.CompanyID = Id;
                            Company.CompanyName = CompanyName;
                            Company.EWM = MainCode + "." + (int)Common.EnumFile.TerraceEwm.showCompany + "." + Id;
                            Company.Infos = "";
                            Company.TemplateIDs = "1,4";
                            Company.TopImgUrl = "";
                            string config = "enterpriseURL";
                            Company.Url = System.Configuration.ConfigurationManager.AppSettings[config] + Company.EWM;
                            ShowContext.ShowCompany.InsertOnSubmit(Company);
                            ShowContext.SubmitChanges();
                        }
                        Data = (from D in DataContext.View_EnterpriseShow
                                where D.Enterprise_Info_ID == Id
                                select D).FirstOrDefault();
                    }
                    ClearLinqModel(Data);
                }
                catch (Exception Ex)
                {
                    string ErrData = "Enterprise_ShowDAL.GetModelView()";
                    WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
                }
            }
            return Data;
        }

        /// <summary>
        /// 编辑企业信息
        /// </summary>
        /// <param name="Model">企业信息表内容</param>
        /// <returns></returns>
        public RetResult Edit(Enterprise_Info Model, EnterpriseShopLink shopModel)
        {
            Ret.Msg = "修改失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext DataContext = GetDataContext())
            {
                try
                {
                    Enterprise_Info Info = DataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == Model.Enterprise_Info_ID);
                    //判断企业信息查询结果是否为空
                    if (Info == null)
                    {
                        DataContext.Enterprise_Info.InsertOnSubmit(Model);
                    }
                    else
                    {
                        Info.Memo = Model.Memo;
                        Info.Logo = Model.Logo;
                        Info.VideoUrl = Model.VideoUrl;
                        Info.LinkMan = Model.LinkMan;
                        Info.LinkPhone = Model.LinkPhone;
                        Info.Email = Model.Email;
                        Info.WebURL = Model.WebURL;
                        Info.Address = Model.Address;
                        Info.OrderingHotline = Model.OrderingHotline;
                        Info.BusinessLicence = Model.BusinessLicence;
                        Info.WXLogo = Model.WXLogo;
                        Info.WXInfo = Model.WXInfo;
                    }
                    EnterpriseShopLink shopInfo = DataContext.EnterpriseShopLink.FirstOrDefault(m => m.EnterpriseID == shopModel.EnterpriseID);
                    if (shopInfo == null)
                    {
                        DataContext.EnterpriseShopLink.InsertOnSubmit(shopModel);
                    }
                    else
                    {
                        shopInfo.EnterpriseID = shopModel.EnterpriseID;
                        shopInfo.AddDate = shopModel.AddDate;
                        shopInfo.AddUser = shopModel.AddUser;
                        shopInfo.JingDongLink = shopModel.JingDongLink;
                        shopInfo.TaoBaoLink = shopModel.TaoBaoLink;
                        shopInfo.TianMaoLink = shopModel.TianMaoLink;
                        shopInfo.AdUrl = shopModel.AdUrl;
                        shopInfo.VideoUrl = shopModel.VideoUrl;
                    }
                    DataContext.SubmitChanges();
                    Ret.Msg = "保存成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception Ex)
                {
                    string ErrData = "ShowCompanyDAL.Edit()";
                    WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
                }
            }
            return Ret;
        }
    }
}
