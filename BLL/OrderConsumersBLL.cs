/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-11-13

** 联系方式 :13313318725

** 描述：主要用于登录、设置默认信息的业务逻辑

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/

using Dal;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 主要用于登录、设置默认信息的业务逻辑
    /// </summary>
    public class OrderConsumersBLL
    {
        /// <summary>
        /// 判断是否注册
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>结果</returns>
        public Result IsRegister(string phone)
        {
            Result result = new Result();
            result = new Dal.OrderConsumersDAL().IsRegister(phone);
            return result;
        }

        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>结果</returns>
        public Result GetPassWord(string phone)
        {
            Result result = new Result();
            result = new Dal.Order_Consumers_AddressDAL().GetPassWord(phone);
            return result;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="password">密码</param>
        /// <returns>登录成功后返回实体</returns>
        public View_Order_Consumers Login(string phone, string password, int loginType,string messageWord,  out string strMsg)
        {
            View_Order_Consumers result = new View_Order_Consumers();
            result = new Dal.OrderConsumersDAL().Login(phone, password,messageWord, loginType, out strMsg);
            return result;
        }

        /// <summary>
        /// 消费者修改密码
        /// </summary>
        /// <param name="consumersId">消费者ID</param>
        /// <param name="oldPwd">原密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        public Result EditPwd(long consumersId, string oldPwd, string newPwd)
        {
            OrderConsumersDAL dal = new OrderConsumersDAL();
            Result result = dal.EditPwd(consumersId, oldPwd, newPwd);
            return result;
        }

        /// <summary>
        /// 消费者修改密码（忘记原密码）
        /// </summary>
        /// <param name="consumersId">消费者ID</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public Result UpdataPwd(long consumersId, string pwd)
        {
            OrderConsumersDAL dal = new OrderConsumersDAL();
            Result result = dal.UpdataPwd(consumersId, pwd);
            return result;
        }

        /// <summary>
        /// 根据订单编号获取订单信息
        /// </summary>
        /// <param name="orderNum">订单编号</param>
        /// <returns>返回结果</returns>
        public View_Material_OnlineOrder GetConsumersOrder(string orderNum)
        {
            OrderConsumersDAL dal = new OrderConsumersDAL();
            View_Material_OnlineOrder result = dal.GetConsumersOrder(orderNum);
            return result;
        }
    }
}
