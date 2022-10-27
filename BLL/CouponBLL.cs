/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-9-8
** 联系方式:13313318725
** 代码功能：新建活动数据层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using Common.Argument;
using LinqModel;
using Dal;

namespace BLL
{
    /// <summary>
    /// 新建活动数据层
    /// </summary>
    public class CouponBLL
    {
        /// <summary>
        /// 添加活动
        /// </summary>
        /// <param name="model">活动模型</param>
        /// <param name="modelSub">活动模型</param>
        /// <param name="details">红包配置</param>
        /// <returns></returns>
        public RetResult AddModel(YX_ActivitySub modelSub, YX_ActivityCoupon coupon, long SetingID)
        {
            return new CouponDAL().AddModel(modelSub, coupon, SetingID);
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public View_ActivityCoupon GetActivitySub(long activityID)
        {
            return new CouponDAL().GetActivitySub(activityID);
        }

        /// <summary>
        /// 编辑活动
        /// </summary>
        /// <param name="modelSub">活动模型</param>
        /// <returns></returns>
        public RetResult EditActivitySub(YX_ActivitySub modelSub, YX_ActivityCoupon coupon)
        {
            return new CouponDAL().EditActivitySub(modelSub, coupon);
        }

        /// <summary>
        /// 是否可以领取优惠券
        /// </summary>
        /// <param name="ewm"></param>
        /// <param name="settingId"></param>
        /// <param name="comIdCode"></param>
        /// <returns></returns>
        public RetResult CouponCanGet(string ewm, long settingId, long codeId, long activityId = 0)
        {
            return new CouponDAL().CouponCanGet(ewm, settingId, codeId, activityId);
        }

        /// <summary>
        /// 记录优惠券
        /// </summary>
        /// <param name="tel"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public RetResult GetCoupon(string tel, YX_CouponGetRecord coupon)
        {
            return new CouponDAL().GetCoupon(tel, coupon);
        }

        /// <summary>
        /// 根据用户id获取优惠券列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<View_CouponGetRecord> GetList(long userId)
        {
            return new CouponDAL().GetList(userId);
        }

        #region 20171127优惠券领取成功
        /// <summary>
        /// 获取优惠券信息
        /// </summary>
        /// <param name="couponCode">优惠券码</param>
        /// <returns></returns>
        public View_ActivityCouponGetRecord GetCouponInfo(string couponCode)
        {
            CouponDAL dal = new CouponDAL();
            View_ActivityCouponGetRecord result = new View_ActivityCouponGetRecord();
            result = dal.GetCouponInfo(couponCode);
            return result;
        }
        #endregion
    }
}
