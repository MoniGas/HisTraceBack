/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-7-10
** 联系方式:15533621896
** 代码功能：我的二维码
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 我的二维码
    /// </summary>
    public class CompanyIDcodeBLL
    {
        /// <summary>
        /// 获取二维码订单列表
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<NewCompanyIDCode> GetListEwm(long eid, int? pageIndex)
        {
            return new CompanyIDcodeDAL().GetListEwm(eid, pageIndex);
        }

        /// <summary>
        /// 获取企业二维码信息
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public YX_CompanyIDcode GetModel(long companyIDcodeID, long activityId = 0)
        {
            return new CompanyIDcodeDAL().GetModel(companyIDcodeID,activityId);
        }

        /// <summary>
        /// 二维码关联活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult AddRelationActivity(YX_ActvitiyRelationCode model)
        {
            return new CompanyIDcodeDAL().AddRelationActivity(model);
        }

        /// <summary>
        /// 获取可以关联二维码的活动
        /// </summary>
        /// <param name="eid">企业id</param>
        /// <returns></returns>
        public List<View_RelationActivityEwm> GetEwmRelationActivity(long eid)
        {
            return new CompanyIDcodeDAL().GetEwmRelationActivity(eid);
        }

        /// <summary>
        /// 获取关联活动详情
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public View_RelationActivityEwm GetRelationActivityEwmModel(long activityId)
        {
            return new CompanyIDcodeDAL().GetRelationActivityEwmModel(activityId);
        }

          /// <summary>
        /// 获取开始码
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public long? GetStartCode(long companyIDcodeID)
        {
            return new CompanyIDcodeDAL().GetStartCode(companyIDcodeID);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public View_BuyCodeOrder GetOrderModel(long companyIDcodeID)
        {
            return new CompanyIDcodeDAL().GetOrderModel(companyIDcodeID);
        }

        /// <summary>
        /// 我的二维码-详情
        /// </summary>
        /// <param name="companyIDcodeID"></param>
        /// <returns></returns>
        public DetailCompanyIDCode GetOrderDetail(long companyIDcodeID)
        {
            return new CompanyIDcodeDAL().GetOrderDetail(companyIDcodeID);
        }
    }
}
