using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    public class ShowNewsDAL : DALBase
    {
        int pageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);

        public List<ShowNews> GetList(long companyId, /*long channelId,*/ string title, int pageIndex, out long totalCount)
        {
            List<ShowNews> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    var data = from m in dataContext.ShowNews select m;
                    if (companyId != 0)
                    {
                        data = data.Where(m => m.CompanyID == companyId);
                    }
                    if (!string.IsNullOrEmpty(title))
                    {
                        data = data.Where(m =>  m.Title.Contains(title.Trim()));
                    }
                    data = data.OrderByDescending(m => m.TopTime).ThenByDescending(m => m.TimeAdd);
                    totalCount = data.Count();
                    result = data.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "ShowNewsDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        public PagedList<View_NewsChannel> GetList(long companyId, /*long channelId,*/ int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                var data = from m in dataContext.View_NewsChannel select m;
                if (companyId != 0)
                {
                    data = data.Where(m => m.CompanyID == companyId);
                }
                //if (channelId != 0)
                //{
                //    data = data.Where(m => m.ChannelID == channelId);
                //}
                data = data.OrderByDescending(m => m.TopTime).ThenByDescending(m => m.TimeAdd);
                return data.ToPagedList(pageIndex ?? 1, pageSize);
            }
        }

        public ShowNews GetModel(string ewm)
        {
            ShowNews result = null;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                result = dataContext.ShowNews.FirstOrDefault(m => m.EWM == ewm);
            }
            return result;
        }

        public ShowNews GetModel(long id)
        {
            ShowNews result = null;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                result = dataContext.ShowNews.FirstOrDefault(m => m.ID == id);
            }
            return result;
        }
        public RetResult Add(ShowNews model, string mainCode)
        {
            Ret.Msg = "添加资讯信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            int Verify = 0;
            using (DataClassesDataContext WebContext = GetDataContext())
            {
                Enterprise_Info EnterpriseInfo = WebContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == model.CompanyID);
                Verify = (int)EnterpriseInfo.Verify;
            }
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    model.TopTime = null;
                    dataContext.ShowNews.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    //model.EWM = mainCode + "." + (int)Common.EnumFile.TerraceEwm.showNews + "." + model.ID;
                    string config ="idcodeURL";
                    if (Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                    {
                        config = "ncpURL";
                    }
                    //model.Url = System.Configuration.ConfigurationManager.AppSettings[config] + model.EWM;
                    dataContext.SubmitChanges();
                    Ret.CmdError = CmdResultError.NONE;
                    Ret.Msg = "添加资讯信息成功！";
                }
                catch (Exception ex)
                {
                    string errData = "ShowNewsDAL.Add()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        public RetResult Edit(ShowNews model)
        {
            Ret.Msg = "修改资讯信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowNews news = dataContext.ShowNews.FirstOrDefault(m => m.ID == model.ID);
                    if (news == null)
                    {
                        Ret.Msg = "资讯信息不存在！";
                    }
                    else
                    {
                        news.Infos = model.Infos;
                        news.Title = model.Title;
                        //news.ChannelID = model.ChannelID;
                        dataContext.SubmitChanges();
                        Ret.Msg = "修改资讯信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ShowNewsDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        public RetResult Del(long id)
        {
            Ret.Msg = "删除资讯信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowNews news = dataContext.ShowNews.FirstOrDefault(m => m.ID == id);
                    if (news == null)
                    {
                        Ret.Msg = "资讯信息不存在！";
                    }
                    else
                    {
                        dataContext.ShowNews.DeleteOnSubmit(news);
                        dataContext.SubmitChanges();
                        Ret.Msg = "删除资讯信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ShowNewsDAL.Del()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        public RetResult UnSetTop(long id)
        {
            Ret.Msg = "取消置顶失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowNews news = dataContext.ShowNews.FirstOrDefault(m => m.ID == id);
                    if (news == null)
                    {
                        Ret.Msg = "资讯信息不存在！";
                    }
                    else
                    {
                        news.TopTime = null;
                        dataContext.SubmitChanges();
                        Ret.Msg = "取消置顶成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ShowNewsDAL.UnSetTop()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        public RetResult SetTop(long id)
        {
            Ret.Msg = "置顶失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowNews news = dataContext.ShowNews.FirstOrDefault(m => m.ID == id);
                    if (news == null)
                    {
                        Ret.Msg = "资讯信息不存在！";
                    }
                    else
                    {
                        news.TopTime = DateTime.Now;
                        dataContext.SubmitChanges();
                        Ret.Msg = "置顶成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ShowNewsDAL.SetTop()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
    }
}
