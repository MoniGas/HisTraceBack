/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-9
** 联系方式:13313318725
** 代码功能：红包活动管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using Dal;
using Common.Argument;

namespace BLL
{
    public class ActivityManagerBLL
    {
        /// <summary>
        /// 获取红包活动列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="AcName">活动名称</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_ActivityManager> GetList(long enterpriseId, string AcName, string sDate, string eDate, int activityStatus, int hbType, int? pageIndex)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            PagedList<View_ActivityManager> list = dal.GetList(enterpriseId, AcName, sDate, eDate, activityStatus, hbType, pageIndex);
            return list;
        }

        /// <summary>
        /// 查看活动信息
        /// </summary>
        /// <param name="id">活动id</param>
        /// <returns></returns>
        public View_ActivityManager GetActivityInfo(long id)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            View_ActivityManager activityInfo = dal.GetActivityInfo(id);
            return activityInfo;
        }

        /// <summary>
        /// 获取红包明细
        /// </summary>
        /// <param name="activityId">活动红包ID</param>
        /// <returns></returns>
        public List<YX_AcivityDetail> HbDetail(long activityId)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            List<YX_AcivityDetail> hbDetail = dal.HbDetail(activityId);
            return hbDetail;
        }

        /// <summary>
        /// 获取用户领取记录
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <returns></returns>
        public List<View_RedGetRecord> HbGetRecord(long id)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            List<View_RedGetRecord> hbDetail = dal.HbGetRecord(id);
            return hbDetail;
        }

        /// <summary>
        /// 修改活动状态
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <param name="eID">企业ID</param>
        /// <returns></returns>
        public RetResult EditStatusEnd(long id, long eID)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            RetResult result = new RetResult();
            result = dal.EditStatusEnd(id, eID);
            return result;
        }

        /// <summary>
        /// 修改活动状态
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <param name="eID">企业ID</param>
        /// <returns></returns>
        public RetResult EditStatusStar(long id, long eID)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            RetResult result = new RetResult();
            result = dal.EditStatusStar(id, eID);
            return result;
        }

        /// <summary>
        /// 预览码
        /// </summary>
        /// <param name="id">活动ID</param>
        /// <returns></returns>
        public YX_ActvitiyRelationCode GetActivityID(long setID)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            YX_ActvitiyRelationCode result = new YX_ActvitiyRelationCode();
            result = dal.GetActivityID(setID);
            return result;
        }

        /// <summary>
        /// 红包充值记录
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public RedPacketMoney GetRedMoney(long companyId, int pageIndex)
        {
            return new ActivityManagerDAL().GetRedMoney(companyId, pageIndex);
        }

        /// <summary>
        /// 红包发送记录
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public RedPacketMoney GetSendMoney(long companyId, int pageIndex)
        {
            return new ActivityManagerDAL().GetSendMoney(companyId, pageIndex);
        }

        /// <summary>
        /// 核销优惠券
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="yhqcode">优惠券码号</param>
        /// <returns></returns>
        public RetResult YhqCancelOut(long eid, string yhqcode)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            RetResult result = new RetResult();
            result = dal.YhqCancelOut(eid, yhqcode);
            return result;
        }

        /// <summary>
        /// 查看优惠券详情
        /// </summary>
        /// <param name="id">活动id</param>
        /// <returns></returns>
        public View_ActivityCoupon GetYhqInfo(long id)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            View_ActivityCoupon yhqInfo = dal.GetYhqInfo(id);
            return yhqInfo;
        }

        /// <summary>
        /// 获取优惠券领取记录
        /// </summary>
        /// <param name="redId">活动红包ID</param>
        /// <returns></returns>
        public PagedList<View_CouponGetRecord> YhqGetDetail(long activityId, int yhqType, int? pageIndex)
        {
            ActivityManagerDAL dal = new ActivityManagerDAL();
            PagedList<View_CouponGetRecord> hbDetail = dal.YhqGetDetail(activityId, yhqType, pageIndex);
            return hbDetail;
        }
    }
}
