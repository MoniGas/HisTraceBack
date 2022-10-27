/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-17
** 联系方式:13313318725
** 代码功能：活动关联二维码操作
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using Dal;

namespace BLL
{
    /// <summary>
    /// 活动关联码操作
    /// </summary>
    public class ActivityRelationCodeBLL
    {
        /// <summary>
        /// 获取关联活动的二维码
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回列表</returns>
        public PagedList<YX_ActvitiyRelationCode> GetList(long enterpriseId, int? pageIndex)
        {
            ActvitiyRelationCodeDAL dal = new ActvitiyRelationCodeDAL();
            PagedList<YX_ActvitiyRelationCode> list = dal.GetList(enterpriseId, pageIndex);
            return list;
        }

        /// <summary>
        /// 查看关联活动
        /// </summary>
        /// <param name="id">关联ID</param>
        /// <returns></returns>
        public View_ActvitiyRelationCode Info(long id)
        {
            ActvitiyRelationCodeDAL dal = new ActvitiyRelationCodeDAL();
            View_ActvitiyRelationCode info = dal.Info(id);
            return info;
        }

         /// <summary>
        /// 获取model
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public YX_ActvitiyRelationCode GetModel(long activityId)
        {
            return new ActvitiyRelationCodeDAL().GetModel(activityId);
        }

        /// <summary>
        /// 获取model
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public YX_CompanyIDcode GetModelComanyId(long activityId)
        {
            return new ActvitiyRelationCodeDAL().GetModelComanyId(activityId);
        }
        #region 监管平台
        /// <summary>
        /// 获取关联活动的二维码
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回列表</returns>
        public PagedList<View_ActivityManager> AdminGetList(int? pageIndex, string comName, string startTime, string endTime, string title,long enterId)
        {
            ActvitiyRelationCodeDAL dal = new ActvitiyRelationCodeDAL();
            PagedList<View_ActivityManager> list = dal.AdminGetList(pageIndex, comName, startTime, endTime, title,enterId);
            return list;
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public View_ActivityManager GetInfo(long activityId)
        {
            return new ActvitiyRelationCodeDAL().GetInfo(activityId);
        }

        /// <summary>
        /// 设置活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="openMode"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public RetResult SetActivity(long activityId, int openMode, string url, int payState)
        {
            return new ActvitiyRelationCodeDAL().SetActivity(activityId, openMode, url,payState);
        }
        #endregion
    }
}
