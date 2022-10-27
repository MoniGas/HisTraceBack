/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-11-16
** 联系方式:13313318725
** 代码功能：企业微信账户设置数据访问层
** 版本：v1.0
** 版权：项目二部
*********************************************************************************/
using System;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 企业微信账户设置数据访问层
    /// </summary>
    public class WxEnAccountDAL : DALBase
    {
        /// <summary>
        /// 设置企业微信账户信息
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public RetResult AddEditEnAccount(YX_WxEnAccount model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "保存失败！";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    YX_WxEnAccount wxmodel = dataContext.YX_WxEnAccount.FirstOrDefault(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID);
                    if (wxmodel == null)
                    {
                        dataContext.YX_WxEnAccount.InsertOnSubmit(model);
                    }
                    else
                    {
                        wxmodel.AddDate = DateTime.Now;
                        wxmodel.APIFileURL = model.APIFileURL;
                        wxmodel.WxAppId = model.WxAppId;
                        wxmodel.AppSecret = model.AppSecret;
                        wxmodel.Enterprise_Info_ID = model.Enterprise_Info_ID;
                        wxmodel.Key = model.Key;
                        wxmodel.Status = model.Status;
                        wxmodel.MarId = model.MarId;
                    }
                    dataContext.SubmitChanges();
                    Ret.Msg = "保存成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "WxEnAccountDAL.AddEditEnAccount()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 获取微信账户账号信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public YX_WxEnAccount GetModel(long eId)
        {
            YX_WxEnAccount result = new YX_WxEnAccount();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.YX_WxEnAccount.FirstOrDefault(m => m.Enterprise_Info_ID == eId);
                }
                catch (Exception ex)
                {
                    string errData = "WxEnAccountDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取微信公众号账号信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public EnterpriseWxGZH GetGzModel(long eId)
        {
            EnterpriseWxGZH result = new EnterpriseWxGZH();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.EnterpriseWxGZH.FirstOrDefault(m => m.Enterprise_Info_ID == eId);
                }
                catch (Exception ex)
                {
                    string errData = "WxEnAccountDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取微信账户账号信息
        /// </summary>
        /// <param name="marId">企业微信商户号</param>
        /// <returns></returns>
        public YX_WxEnAccount GetModelM(string marId)
        {
            YX_WxEnAccount result = new YX_WxEnAccount();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.YX_WxEnAccount.FirstOrDefault(m => m.MarId == marId);
                }
                catch (Exception ex)
                {
                    string errData = "WxEnAccountDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据订单号查询企业ID
        /// </summary>
        /// <param name="tradeNo">订单号</param>
        /// <returns></returns>
        public YX_RedRecharge GetEnID(string tradeNo)
        {

            YX_RedRecharge result = new YX_RedRecharge();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.YX_RedRecharge.FirstOrDefault(m => m.OrderNum == tradeNo && m.PayState == 2);
                }
                catch (Exception ex)
                {
                    string errData = "WxEnAccountDAL.GetEnID()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
    }
}
