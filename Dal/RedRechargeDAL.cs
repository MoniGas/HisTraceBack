/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-11
** 联系方式:13313318725
** 代码功能：红包充值记录管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;

namespace Dal
{
    public class RedRechargeDAL : DALBase
    {
        /// <summary>
        /// 获取红包充值记录
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_RedRecharge> GetList(string comName, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_RedRecharge select m;
                    if (!string.IsNullOrEmpty(comName))
                    {
                        data = data.Where(m => m.EnterpriseName.Contains(comName.Trim()));
                    }
                    data = data.OrderByDescending(m => m.RedRechargeID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "RedRechargeDAL.GetList():View_RedRecharge";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 红包充值
        /// </summary>
        /// <param name="model">添加充值记录</param>
        /// <returns></returns>
        public RetResult AddModel(YX_RedRecharge model)
        {
            string Msg = "充值失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.YX_RedRecharge.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.RedRechargeID;
                    Msg = "充值成功！";
                    error = CmdResultError.NONE;
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<Enterprise_Info> GetEnList(int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.Enterprise_Info select m;
                    data = data.OrderByDescending(m => m.EnterpriseName);
                    return data.ToList();
                }
                catch (Exception ex)
                {
                    string errData = "RedRechargeDAL.GetEnList():Enterprise_Info";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取充值情况
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_CompanyMoney> GetListMoney(string comName, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_CompanyMoney select m;
                    if (!string.IsNullOrEmpty(comName))
                    {
                        data = data.Where(m => m.EnterpriseName.Contains(comName.Trim()));
                    }
                    data = data.OrderByDescending(m => m.CompanyID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "RedRechargeDAL.GetListMoney():View_RedRecharge";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取充值实体
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public YX_RedRecharge GetModel(long activityId)
        {
            YX_RedRecharge model = new YX_RedRecharge();
            using (DataClassesDataContext dct = GetDataContext())
            {
                try
                {
                    model = dct.YX_RedRecharge.Where(p => p.ActivityID == activityId).FirstOrDefault();
                }
                catch
                { }
            }
            return model;
        }
        /// <summary>
        /// 获取企业微信支付状态未支付的订单
        /// </summary>
        /// <returns></returns>
        public List<YX_RedRecharge> GetRechargeList()
        {
            List<YX_RedRecharge> model = new List<YX_RedRecharge>();
            using (DataClassesDataContext dct = GetDataContext())
            {
                try
                {
                    model = dct.YX_RedRecharge.Where(p => p.PayState == (int)Common.EnumText.PayState.NoPay).ToList();
                }
                catch
                { }
            }
            return model;
        }
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool UpdateModel(long activityId)
        {
            YX_RedRecharge model = new YX_RedRecharge();
            using (DataClassesDataContext dct = GetDataContext())
            {
                try
                {
                    model = dct.YX_RedRecharge.Where(p => p.ActivityID == activityId).FirstOrDefault();
                    model.OrderNumAgain = model.OrderNumAgain + "C";
                    dct.SubmitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
