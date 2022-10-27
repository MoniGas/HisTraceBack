/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-10
** 联系方式:13313318725
** 代码功能：二维码订单管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Linq;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Log;

namespace Dal
{
    public class BuyCodeOrderDAL : DALBase
    {
        /// <summary>
        /// 获取二维码未审核的订单列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_BuyCodeOrderEnterprise> GetList(int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_BuyCodeOrderEnterprise
                               where m.OrderStatus ==
                                   (int)Common.EnumText.OrderState.Auditing
                               select m;
                    data = data.OrderByDescending(m => m.BuyCodeOrderID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "BuyCodeOrderDAL.GetList():YX_BuyCodeOrder";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public Enterprise_Info GetComModel(long eid)
        {
            Enterprise_Info model = new Enterprise_Info();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eid);
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "查询成功！");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(Common.Argument.CmdResultError.EXCEPTION, null, "查询失败！");
            }
            return model;
        }

        #region 查询订单
        /// <summary>
        /// 管理员查询订单
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_BuyCodeOrderEnterprise> GetAllList(string comName, int status, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_BuyCodeOrderEnterprise select m;
                    if (!string.IsNullOrEmpty(comName))
                    {
                        data = data.Where(m => m.EnterpriseName.Contains(comName.Trim()));
                    }
                    if (status != 0)
                    {
                        data = data.Where(m => m.OrderStatus == status);
                    }
                    data = data.OrderByDescending(m => m.BuyCodeOrderID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "BuyCodeOrderDAL.GetAllList():YX_BuyCodeOrder";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }
        #endregion
    }
}
