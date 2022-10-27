using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;

namespace Dal
{
    public class SysSetAnalysisDAL : DALBase
    {
        /// <summary>
        /// 获取解析mac地址列表
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<SysSetAnalysis> GetAnalysisList(string mac, string beginDate, string endDate, int? pageIndex)
        {
            PagedList<SysSetAnalysis> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.SysSetAnalysis
                                   select data;
                    if (!string.IsNullOrEmpty(mac))
                    {
                        dataList = dataList.Where(w => w.MacAddress.Contains(mac));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.EndDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.EndDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    //totalCount = dataList.Count();
                    dataInfoList = dataList.OrderByDescending(m => m.ID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 添加Mac地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(SysSetAnalysis model)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    SysSetAnalysis temp = dataContext.SysSetAnalysis.FirstOrDefault(p => p.MacAddress == model.MacAddress);
                    if (temp != null)
                    {
                        Msg = "该Mac地址已存在！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                    dataContext.SysSetAnalysis.InsertOnSubmit(model);
                    dataContext.SubmitChanges();

                    Msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public SysSetAnalysis GetAnalysisInfo(long Id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.SysSetAnalysis
                                where d.ID == Id
                                select d).FirstOrDefault();
                    ClearLinqModel(data);
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public RetResult Edit(SysSetAnalysis model)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    SysSetAnalysis temp = dataContext.SysSetAnalysis.FirstOrDefault(p => p.ID == model.ID);
                    if (temp != null)
                    {
                        SysSetAnalysis tempY = dataContext.SysSetAnalysis.FirstOrDefault(p => p.MacAddress == model.MacAddress && p.ID != model.ID);
                        if (tempY != null)
                        {
                            Msg = "该Mac地址已存在！";
                            error = CmdResultError.NONE;
                            Ret.SetArgument(error, Msg, Msg);
                            return Ret;
                        }
                        else
                        {
                            temp.MacAddress = model.MacAddress;
                            temp.EndDate = model.EndDate;
                            dataContext.SubmitChanges();
                            Msg = "保存成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                    else
                    {
                        Msg = "未找到要修改的信息！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public RetResult EditMacStatus(long id, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    SysSetAnalysis model = dataContext.SysSetAnalysis.FirstOrDefault(m => m.ID == id);
                    if (model == null)
                    {
                        Ret.SetArgument(CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    if (type == 1)
                    {
                        model.Status = (int)Common.EnumFile.Status.used;
                    }
                    else
                    {
                        model.Status = (int)Common.EnumFile.Status.delete;
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, null, "操作成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }
    }
}
