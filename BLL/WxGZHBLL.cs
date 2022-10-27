/********************************************************************************
** 作者：张翠霞
** 开发时间：2018-09-03
** 联系方式:13313318725
** 代码功能：企业微信公众号设置业务层
** 版本：v1.0
** 版权：项目二部
*********************************************************************************/

using Common.Argument;
using LinqModel;
using Dal;

namespace BLL
{
    public class WxGZHBLL
    {
        public RetResult AddEditEnGZH(EnterpriseWxGZH model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            try
            {
                WxGZHDAL dal = new WxGZHDAL();
                if (model == null)
                {
                    result.Msg = "数据错误";
                }
                else
                {
                    result = dal.AddEditEnWxGZH(model);
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
        public EnterpriseWxGZH GetModel(long eId)
        {
            EnterpriseWxGZH model = new EnterpriseWxGZH();
            WxGZHDAL dal = new WxGZHDAL();
            model = dal.GetModel(eId);
            return model;
        }
    }
}
