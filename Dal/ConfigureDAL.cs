/********************************************************************************

** 作者： 郭心宇

** 创始时间：2015-12-15

** 联系方式 :13313318725

** 描述：主要用于配置宣传码信息的数据访问

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;

namespace Dal
{
    public class ConfigureDAL : DALBase
    {
        /// <summary>
        /// 获取企业品牌信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>企业品牌信息列表</returns>
        public List<Brand> GetBrandList(long Id)
        {
            List<Brand> DataList = new List<Brand>();
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext())
                {
                    DataList = (from Data in DataContext.Brand
                                where Data.Enterprise_Info_ID == Id && Data.Status == (int)Common.EnumFile.Status.used
                                select Data).ToList();
                    ClearLinqModel(DataList);
                    return DataList;
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取品牌实体
        /// </summary>
        /// <param name="id">品牌ID</param>
        /// <returns></returns>
        public Brand GetBrandModel(long Id)
        {
            Brand Result = null;
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext())
                {
                    Result = DataContext.Brand.FirstOrDefault(M => M.Brand_ID == Id);
                    ClearLinqModel(Result);
                }
            }
            catch (Exception Ex)
            {
                string ErrData = "BrandDAL.GetBrandModel()";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Result;
        }

        /// <summary>
        /// 获取企业员工信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>企业员工信息列表</returns>
        public List<ShowUser> GetUserList(long Id)
        {
            List<ShowUser> DataList = new List<ShowUser>();
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext("ShowConnect"))
                {
                    DataList = (from Data in DataContext.ShowUser
                                where Data.CompanyID == Id && Data.Status == 1
                                select Data).ToList();
                    for (int i = 0; i < DataList.Count; i++)
                    {
                        List<System.Xml.Linq.XElement> Infos = DataList[i].InfoOther.Descendants("info").ToList();
                        DataList[i].headimg = Infos.FirstOrDefault(M => M.Attribute("name").Value == "headimg").Attribute("value").Value;
                    }
                    ClearLinqModel(DataList);
                    return DataList;
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取企业新闻信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns>企业新闻信息列表</returns>
        public List<ShowNews> GetNewsList(long Id)
        {
            List<ShowNews> DataList = new List<ShowNews>();
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext("ShowConnect"))
                {
                    DataList = (from Data in DataContext.ShowNews
                                where Data.CompanyID == Id
                                select Data).ToList();
                    ClearLinqModel(DataList);
                    return DataList;
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取新闻实体
        /// </summary>
        /// <param name="id">新闻ID</param>
        /// <returns></returns>
        public ShowNews GetNewsModel(long Id)
        {
            ShowNews Result = null;
            using (DataClassesDataContext DataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    Result = DataContext.ShowNews.FirstOrDefault(M => M.ID == Id);
                    ClearLinqModel(Result);
                }
                catch(Exception Ex)
                {
                    string ErrData = "ShowNewsDAL.GetModel()";
                    WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
                }
            }
            return Result;
        }

        /// <summary>
        /// 根据企业id获得企业配置信息
        /// </summary>
        /// <param name="CompanyId">企业ID</param>
        /// <returns>企业配置信息模型</returns>
        public Configure GetModel(long CompanyId)
        {
            Configure ObjConfigure = new Configure();
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext())
                {
                    ObjConfigure = (from Data in DataContext.Configure
                                    where Data.Company_ID == CompanyId
                                    select Data).FirstOrDefault();
                    ClearLinqModel(ObjConfigure);
                    return ObjConfigure;
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="CompanyId">企业ID</param>
        /// <returns>企业信息模型</returns>
        public ShowCompany GetCompanyModel(long CompanyId)
        {
            ShowCompany Result = new ShowCompany();
            using (DataClassesDataContext DataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    Result = (from Data in DataContext.ShowCompany
                              where Data.CompanyID == CompanyId
                              select Data).FirstOrDefault();
                    ClearLinqModel(Result);
                }
                catch(Exception Ex)
                {
                    string ErrData = "ConfigureDAL.GetList()";
                    WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
                }
            }
            return Result;
        }

       /// <summary>
       /// 更新操作
       /// </summary>
       /// <param name="ObjConfigure">Configure表实体</param>
       /// <param name="CompanyId">企业ID</param>
       /// <param name="UserIdArray">员工ID数组</param>
       /// <param name="NewsIdArray">新闻ID数组</param>
       /// <param name="BrandIdArray">品牌ID数组</param>
       /// <returns></returns>
        public RetResult Update(Configure ObjConfigure, long CompanyId, string UserIdArray, string NewsIdArray, string BrandIdArray)
        {
            try
            {
                using (DataClassesDataContext DataContext = GetDataContext())
                {
                    var Data = (from D in DataContext.Configure
                                where D.Company_ID == CompanyId
                                select D).FirstOrDefault();
                    //判断查询数据是否为空
                    if (Data == null)
                    {
                        DataContext.Configure.InsertOnSubmit(ObjConfigure);
                        DataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "保存成功！", "保存成功！");
                    }
                    else
                    {
                        Data.News_ID_Array = NewsIdArray;
                        Data.User_ID_Array = UserIdArray;
                        Data.Brand_ID_Array = BrandIdArray;
                        DataContext.SubmitChanges();
                        Ret.SetArgument(CmdResultError.NONE, "更新成功！", "更新成功！");
                    }
                }
            }
            catch (Exception Ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, Ex.Message, Ex.Message);
            }
            return Ret;
        }
    }
}
