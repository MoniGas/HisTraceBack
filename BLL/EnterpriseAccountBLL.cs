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
using Common.Argument;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 主要用于支付宝账号管理数据层
    /// </summary>
    public class EnterpriseAccountBLL
    {
        /// <summary>
        /// 获取企业支付宝信息
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <returns>模型</returns>
        public BaseResultModel GetModel(long enterpriseId)
        {
            Order_EnterpriseAccount model = new Dal.EnterpriseAccountDAL().GetModel(enterpriseId);
            return ToJson.NewModelToJson(model, "1", "");
        }

        /// <summary>
        /// 设置支付宝账号
        /// </summary>
        /// <param name="model">账号信息</param>
        /// <returns>操作结果</returns>
        public BaseResultModel Add(Order_EnterpriseAccount model)
        {
            RetResult ret = new Dal.EnterpriseAccountDAL().Add(model);
            BaseResultModel result = ToJson.NewModelToJson(
                ret.CrudCount, 
                (Convert.ToInt32(ret.IsSuccess)).ToString(), 
                ret.Msg);
            return result;
        }
    }
}
