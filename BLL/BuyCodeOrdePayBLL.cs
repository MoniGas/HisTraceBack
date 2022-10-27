/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-6
** 联系方式:13313318725
** 代码功能：二维码订单管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using Dal;
using LinqModel;
using Common.Argument;

namespace BLL
{
    public class BuyCodeOrdePayBLL
    {
        /// <summary>
        /// 更该订单状态
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <param name="payType">状态</param>
        /// <param name="TradeNo">支付宝订单号</param>
        /// <param name="ConsumerLogin">消费者登录名</param>
        /// <param name="ConsumerNum">消费者支付宝16位唯一标识</param>
        /// <returns>操作结果</returns>
        public RetResult PaySuccess(string orderNum, int payType, string TradeNo = "", string ConsumerLogin = "", string ConsumerNum = "")
        {
            return new BuyCodeOrdePayDAL().PaySuccess(orderNum, payType, TradeNo, ConsumerLogin, ConsumerNum);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        public YX_BuyCodeOrdePay GetOrderPayInfo(long id, long eid)
        {
            BuyCodeOrdePayDAL dal = new BuyCodeOrdePayDAL();
            YX_BuyCodeOrdePay orderPayInfo = dal.GetOrderPayInfo(id, eid);
            return orderPayInfo;
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        public YX_BuyCodeOrdePay GetOrderPayInfoA(long id)
        {
            BuyCodeOrdePayDAL dal = new BuyCodeOrdePayDAL();
            YX_BuyCodeOrdePay orderPayInfo = dal.GetOrderPayInfoA(id);
            return orderPayInfo;
        }
    }
}
