/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-10
** 联系方式:13313318725
** 代码功能：二维码订单管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using Webdiyer.WebControls.Mvc;
using LinqModel;
using Dal;

namespace BLL
{
    public class BuyCodeOrderBLL
    {
        /// <summary>
        /// 获取二维码未审核的订单列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_BuyCodeOrderEnterprise> GetList(int? pageIndex)
        {
            BuyCodeOrderDAL dal = new BuyCodeOrderDAL();
            PagedList<View_BuyCodeOrderEnterprise> list = dal.GetList(pageIndex);
            return list;
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public Enterprise_Info GetComModel(long eid)
        {
            BuyCodeOrderDAL dal = new BuyCodeOrderDAL();
            Enterprise_Info result = dal.GetComModel(eid);
            return result;
        }
        #region 查询订单
        /// <summary>
        /// 管理员查询订单
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_BuyCodeOrderEnterprise> GetAllList(string comName, int status, int? pageIndex)
        {
            BuyCodeOrderDAL dal = new BuyCodeOrderDAL();
            PagedList<View_BuyCodeOrderEnterprise> list = dal.GetAllList(comName, status, pageIndex);
            return list;
        }
        #endregion
    }
}
