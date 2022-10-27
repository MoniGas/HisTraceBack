/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-11
** 联系方式:13313318725
** 代码功能：红包充值记录管理
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
    public class RedRechargeBLL
    {
        /// <summary>
        /// 获取红包充值记录
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_RedRecharge> GetList(string comName, int? pageIndex)
        {
            RedRechargeDAL dal = new RedRechargeDAL();
            PagedList<View_RedRecharge> list = dal.GetList(comName, pageIndex);
            return list;
        }

        /// <summary>
        /// 红包充值
        /// </summary>
        /// <param name="model">添加充值记录</param>
        /// <returns></returns>
        public RetResult AddModel(YX_RedRecharge model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            RedRechargeDAL dal = new RedRechargeDAL();
            result = dal.AddModel(model);
            return result;
        }

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<Enterprise_Info> GetEnList(int? pageIndex)
        {
            RedRechargeDAL dal = new RedRechargeDAL();
            List<Enterprise_Info> enList = new List<Enterprise_Info>();
            enList = dal.GetEnList(pageIndex);
            return enList;
        }

        /// <summary>
        /// 获取充值情况
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_CompanyMoney> GetListMoney(string comName, int? pageIndex)
        {
            return new RedRechargeDAL().GetListMoney(comName, pageIndex);
        }

         /// <summary>
        /// 获取充值实体
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public YX_RedRecharge GetModel(long activityId)
        {
            return new RedRechargeDAL().GetModel(activityId);
        }
        /// <summary>
        /// 获取企业微信支付状态未支付的订单
        /// </summary>
        /// <returns></returns>
        public List<YX_RedRecharge> GetRechargeList()
        {
            return new RedRechargeDAL().GetRechargeList();
        }

         /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool UpdateModel(long activityId)
        {
            return new RedRechargeDAL().UpdateModel(activityId);
        }
    }
}
