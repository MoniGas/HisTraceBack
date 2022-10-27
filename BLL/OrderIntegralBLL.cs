/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-09-15
** 联系方式:13313318725
** 代码功能：消费者积分查询管理
** 版本：v1.0
** 版权：项目二部   
*********************************************************************************/

using System.Collections.Generic;
using LinqModel;
using Dal;

namespace BLL
{
    public class OrderIntegralBLL
    {
        /// <summary>
        /// 获取消费者积分
        /// </summary>
        /// <param name="consumerId">消费者ID</param>
        /// <returns></returns>
        public Order_Consumers GetConIntegral(long consumerId)
        {
            OrderIntegralDAL dal = new OrderIntegralDAL();
            Order_Consumers result = dal.GetConIntegral(consumerId);
            return result;
        }

        /// <summary>
        /// 获取消费者积分明细
        /// </summary>
        /// <param name="consumerId">消费者ID</param>
        /// <returns></returns>
        public List<Order_Integral> GetList(long consumerId, string sDate, string eDate)
        {
            OrderIntegralDAL dal = new OrderIntegralDAL();
            List<Order_Integral> result = dal.GetList(consumerId, sDate, eDate);
            return result;
        }

        /// <summary>
        /// 获取即将到期积分信息
        /// </summary>
        /// <param name="consumerId">消费者ID</param>
        /// <returns></returns>
        public List<Order_Integral> GetModelList(long consumerId)
        {
            OrderIntegralDAL dal = new OrderIntegralDAL();
            List<Order_Integral> result = dal.GetModelList(consumerId);
            return result;
        }
    }
}
