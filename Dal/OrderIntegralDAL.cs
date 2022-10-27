/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-09-15
** 联系方式:13313318725
** 代码功能：消费者积分查询管理
** 版本：v1.0
** 版权：项目二部   
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;

namespace Dal
{
    public class OrderIntegralDAL : DALBase
    {
        /// <summary>
        /// 获取消费者积分
        /// </summary>
        /// <param name="consumerId">消费者ID</param>
        /// <returns></returns>
        public Order_Consumers GetConIntegral(long consumerId)
        {
            Order_Consumers result = new Order_Consumers();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.Order_Consumers.FirstOrDefault(m => m.Order_Consumers_ID == consumerId);
                }
            }
            catch (Exception ex)
            {
                string errData = "Order_IntegralDAL.GetConIntegral():Order_Consumers表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取消费者积分明细
        /// </summary>
        /// <param name="consumerId">消费者ID</param>
        /// <returns></returns>
        public List<Order_Integral> GetList(long consumerId, string sDate, string eDate)
        {
            List<Order_Integral> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == consumerId);
                    if (!string.IsNullOrEmpty(sDate))
                    {
                        data = data.Where(m => m.AddDate.GetValueOrDefault() >= Convert.ToDateTime(sDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => m.AddDate.GetValueOrDefault() <= Convert.ToDateTime(eDate).AddDays(1));
                    }
                    result = data.OrderByDescending(m => m.IntegralID).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "Order_IntegralDAL.GetList():Order_Integral表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取即将到期的积分数据
        /// </summary>
        /// <param name="consumerId">消费者ID</param>
        /// <returns></returns>
        public List<Order_Integral> GetModelList(long consumerId)
        {
            List<Order_Integral> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Order_Integral.Where(m => m.Order_Consumers_ID == consumerId && m.AddDate >= DateTime.Now.AddYears(-1) && m.AddDate <= DateTime.Now);
                    result = data.ToList();
                }
                catch (Exception ex)
                {
                    string errData = "Order_IntegralDAL.GetList():Order_Integral表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
    }
}
