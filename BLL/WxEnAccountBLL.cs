/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-11-16
** 联系方式:13313318725
** 代码功能：企业微信账户设置业务层
** 版本：v1.0
** 版权：项目二部
*********************************************************************************/

using Common.Argument;
using LinqModel;
using Dal;

namespace BLL
{
    /// <summary>
    /// 企业微信账户设置业务层
    /// </summary>
    public class WxEnAccountBLL
    {
        public RetResult AddEditEnAccount(YX_WxEnAccount model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            try
            {
                WxEnAccountDAL dal = new WxEnAccountDAL();
                if (model == null)
                {
                    result.Msg = "数据错误";
                }
                else
                {
                    result = dal.AddEditEnAccount(model);
                }
            }
            catch
            {
                result.CmdError = CmdResultError.EXCEPTION;
                result.Msg = "异常错误，请重新操作！";
            }
            return result;
        }

        /// <summary>
        /// 获取企业微信账户信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public YX_WxEnAccount GetModel(long eId)
        {
            YX_WxEnAccount model = new YX_WxEnAccount();
            WxEnAccountDAL dal = new WxEnAccountDAL();
            model = dal.GetModel(eId);
            return model;
        }

        /// <summary>
        /// 获取企业微信账户信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public EnterpriseWxGZH GetGzModel(long eId)
        {
            WxEnAccountDAL dal = new WxEnAccountDAL();
            var model = dal.GetGzModel(eId);
            return model;
        }
        /// <summary>
        /// 获取企业微信账户信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public YX_WxEnAccount GetModelM(string marId)
        {
            YX_WxEnAccount model = new YX_WxEnAccount();
            WxEnAccountDAL dal = new WxEnAccountDAL();
            model = dal.GetModelM(marId);
            return model;
        }

        /// <summary>
        /// 根据订单号查询企业ID
        /// </summary>
        /// <param name="tradeNo">订单号</param>
        /// <returns></returns>
        public YX_RedRecharge GetEnID(string tradeNo)
        {
            YX_RedRecharge model = new YX_RedRecharge();
            WxEnAccountDAL dal = new WxEnAccountDAL();
            model = dal.GetEnID(tradeNo);
            return model;
        }
    }
}
