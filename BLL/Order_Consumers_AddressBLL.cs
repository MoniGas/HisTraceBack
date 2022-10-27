/********************************************************************************

** 作者：张翠霞

** 开发时间：2015-11-17

** 联系方式:15630136020

** 代码功能：主要用于消费者收货地址管理业务层

** 版本：v1.0

** 版权：研一农业项目组   

*********************************************************************************/
using System.Collections.Generic;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 消费者收货地址管理
    /// </summary>
    public class Order_Consumers_AddressBLL
    {
        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <param name="order_Consumers_id">消费者ID</param>
        /// <returns>返回结果</returns>
        public List<Order_Consumers_Address> GetList(long order_Consumers_id)
        {
            Order_Consumers_AddressDAL dal = new Order_Consumers_AddressDAL();
            List<Order_Consumers_Address> result = dal.GetList(order_Consumers_id);
            return result;
        }

        /// <summary>
        /// 消费者添加收货地址
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="isDefault">是否设置为默认</param>
        /// <param name="consumers"></param>
        /// <returns>返回结果</returns>
        public RetResult AddConAddress(Order_Consumers_Address model, bool isDefault, ref View_Order_Consumers consumers, bool updateUser = false)
        {
            Order_Consumers_AddressDAL dal = new Order_Consumers_AddressDAL();
            RetResult result = dal.AddConAddress(model, isDefault, ref consumers, updateUser);
            return result;
        }

        /// <summary>
        /// 消费者修改收货地址
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>返回结果</returns>
        public RetResult EditConAddress(Order_Consumers_Address model, bool isDefault, ref View_Order_Consumers consumers)
        {
            Order_Consumers_AddressDAL dal = new Order_Consumers_AddressDAL();
            RetResult result = dal.EditConAddress(model, isDefault, ref consumers);
            return result;
        }

        /// <summary>
        /// 根据收货地址ID获取实体
        /// </summary>
        /// <param name="consumerAdressId">收货地址ID</param>
        /// <returns>返回结果</returns>
        public Order_Consumers_Address GetModel(long consumerAdressId)
        {
            Order_Consumers_AddressDAL dal = new Order_Consumers_AddressDAL();
            Order_Consumers_Address result = dal.GetModelConAddress(consumerAdressId);
            return result;
        }

        /// <summary>
        /// 消费者删除收货地址
        /// </summary>
        /// <param name="consumerAdressId">收货地址ID</param>
        /// <param name="order_Consumers_id">消费者ID</param>
        /// <returns>返回结果</returns>
        public RetResult Delete(long consumerAdressId, long order_Consumers_id)
        {
            Order_Consumers_AddressDAL dal = new Order_Consumers_AddressDAL();
            RetResult result = dal.DeleteConAddress(consumerAdressId, order_Consumers_id);
            return result;
        }
    }
}
