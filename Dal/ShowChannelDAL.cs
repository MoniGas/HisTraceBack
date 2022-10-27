using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace Dal
{
    public class ShowChannelDAL : DALBase
    {
        public List<ShowChannel> GetList(long companyId, string name)
        {
            List<ShowChannel> result = null;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                var data = from m in dataContext.ShowChannel where m.CompanyID == companyId select m;
                if (!string.IsNullOrEmpty(name))
                {
                    data = data.Where(m => m.ChannelName.Contains(name.Trim()));
                }
                data = data.OrderBy(m => m.Sort);
                result = data.ToList();
            }
            return result;
        }


        public ShowChannel GetModel(long id)
        {
            ShowChannel result = null;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                result = dataContext.ShowChannel.FirstOrDefault(m => m.ID == id);
            }
            return result;
        }


        public RetResult Add(ShowChannel model)
        {
            Ret.Msg = "添加栏目信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    var channelCount = dataContext.ShowChannel.Where(m => m.CompanyID == model.CompanyID);
                    if (channelCount.Count() >= 4)
                    {
                        Ret.Msg = "一个企业最多添加四个栏目！";
                    }
                    else
                    {
                        ShowChannel channel = dataContext.ShowChannel.FirstOrDefault(m => m.ChannelName == model.ChannelName && m.CompanyID == model.CompanyID);
                        if (channel != null)
                        {
                            Ret.Msg = "已存在该栏目名称！";
                        }
                        else
                        {
                            model.Sort = channelCount.Count() + 1;
                            dataContext.ShowChannel.InsertOnSubmit(model);
                            dataContext.SubmitChanges();
                            Ret.CmdError = CmdResultError.NONE;
                            Ret.Msg = "添加栏目信息成功！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ShowChannelDAL.Add()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        public RetResult Edit(ShowChannel model)
        {
            Ret.Msg = "修改栏目信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowChannel channel = dataContext.ShowChannel.FirstOrDefault(m => m.CompanyID == model.CompanyID && m.ChannelName == model.ChannelName && m.ID != model.ID);
                    if (channel != null)
                    {
                        Ret.Msg = "已存在该栏目名称！";
                    }
                    else
                    {
                        channel = dataContext.ShowChannel.FirstOrDefault(m => m.ID == model.ID);
                        if (channel == null)
                        {
                            Ret.Msg = "栏目信息不存在！";
                        }
                        else
                        {
                            channel.ChannelName = model.ChannelName;
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改栏目信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ShowChannelDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
        public RetResult Del(long id)
        {
            Ret.Msg = "删除栏目信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowChannel channel = dataContext.ShowChannel.FirstOrDefault(m => m.ID == id);
                    if (channel == null)
                    {
                        Ret.Msg = "栏目信息不存在！";
                    }
                    else
                    {
                        dataContext.ShowChannel.DeleteOnSubmit(channel);
                        dataContext.SubmitChanges();
                        Ret.Msg = "删除栏目信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    Ret.Msg = "该产品已被使用，目前无法删除！";
                    string errData = "ShowChannelDAL.Del()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        public RetResult UpdateSort(string ids)
        {
            Ret.Msg = "栏目排序失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    string[] id = ids.Split(',');
                    int index = 0;
                    foreach (var item in id)
                    {
                        ShowChannel channel = dataContext.ShowChannel.FirstOrDefault(m => m.ID == Convert.ToInt64(item));
                        if (channel != null)
                        {
                            channel.Sort = index++;
                        }
                    }
                    dataContext.SubmitChanges();
                    Ret.Msg = "栏目排序成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "ShowChannelDAL.UpdateSort()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
    }
}
