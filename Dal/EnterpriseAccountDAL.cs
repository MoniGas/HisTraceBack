/********************************************************************************

** 作者： 李子巍

** 创始时间：2016-12-14

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于支付宝账号管理数据层

*********************************************************************************/
using System;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 主要用于支付宝账号管理数据层
    /// </summary>
    public class EnterpriseAccountDAL : DALBase
    {
        /// <summary>
        /// 获取企业支付宝信息
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <returns>模型</returns>
        public Order_EnterpriseAccount GetModel(long enterpriseId)
        {
            Order_EnterpriseAccount result = new Order_EnterpriseAccount();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Order_EnterpriseAccount.FirstOrDefault(m =>
                        m.Enterprise_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (result == null)
                    {
                        result = null;
                    }
                    else
                    {
                        ClearLinqModel(result);
                    }
                }
                catch (Exception ex)
                {
                    string errData = "EnterpriseAccountDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 设置支付宝账号
        /// </summary>
        /// <param name="model">账号信息</param>
        /// <returns>操作结果</returns>
        public RetResult Add(Order_EnterpriseAccount model)
        {
            Ret.Msg = "设置支付宝账号失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempModel = dataContext.Order_EnterpriseAccount.FirstOrDefault(p =>
                        p.Enterprise_ID == model.Enterprise_ID
                        && p.Status == (int)Common.EnumFile.Status.used);
                    if (tempModel != null)
                    {
                        tempModel.AccountNum = model.AccountNum;
                        tempModel.AccountName = model.AccountName;
                        tempModel.LinkPhone = model.LinkPhone;
                    }
                    else
                    {
                        dataContext.Order_EnterpriseAccount.InsertOnSubmit(model);
                    }
                    dataContext.SubmitChanges();
                    Ret.Msg = "设置支付宝账号成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "EnterpriseAccountDAL.Add()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
    }
}
