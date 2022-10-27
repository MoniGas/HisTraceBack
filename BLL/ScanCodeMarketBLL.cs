/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-6-7
** 联系方式:15533621896
** 代码功能：红包拍码页逻辑层
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
    /// 红包拍码页逻辑层
    /// </summary>
    public class ScanCodeMarketBLL
    {
        /// <summary>
        /// 是否可以抢红包
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public RetResult CanGetRedPacket(long settingId,Enterprise_FWCode_00 fwCode, long companyIdCode, long activityId = 0)
        {
            return new ScanCodeMarketDAL().CanGetRedPacket(settingId, fwCode, companyIdCode, activityId);
        }

        /// <summary>
        /// 根据配置码获取企业关联码
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public YX_ActivitySub GetModel(long settingId, long companyIdCode)
        {
            return new ScanCodeMarketDAL().GetModel(settingId, companyIdCode);
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public View_Activity GetActivity(long settingId, long activityId)
        {
            return new ScanCodeMarketDAL().GetActivity(settingId, activityId);
        }

        /// <summary>
        /// 添加用户记录和拍码记录
        /// </summary>
        /// <param name="scanRecord">拍码记录</param>
        /// <param name="getRecord">领取记录</param>
        /// <param name="sendRecord">发放记录</param>
        /// <returns></returns>
        public long AddRecord(YX_Redactivity_ScanRecord scanRecord, YX_RedGetRecord getRecord)
        {
            return new ScanCodeMarketDAL().AddRecord(scanRecord, getRecord);
        }
        public long AddChangeRecord(YX_RedGetChangeRecord getRecord)
        {
            return new ScanCodeMarketDAL().AddChangeRecord(getRecord);
        }
        /// <summary>
        /// 新增/修改发送记录，同时更新红包剩余数量
        /// </summary>
        /// <param name="sendRecord"></param>
        /// <param name="sendRecordId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public long AddSendRecord(YX_RedSendRecord sendRecord)
        {
            return new ScanCodeMarketDAL().AddSendRecord(sendRecord);
        }

        public long UpdateSendRecord(YX_RedSendRecord sendRecord, out long recordeId)
        {
            return new ScanCodeMarketDAL().UpdateSendRecord(sendRecord, out recordeId);
        }

        /// <summary>
        /// 发送失败后删除发送记录
        /// </summary>
        /// <param name="ewm"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public RetResult DeleteSendRecord(string ewm,long activityId)
        {
            return new ScanCodeMarketDAL().DeleteSendRecord(ewm, activityId);
        }
        public long AddSendChangeRecord(YX_RedSendChangeRecord sendRecord,long userId)
        {
            return new ScanCodeMarketDAL().AddSendChangeRecord(sendRecord,userId);
        }
        /// <summary>
        /// 获取活动金额明细
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public double?[] GetActivityDetail(long activityId)
        {
            return new ScanCodeMarketDAL().GetActivityDetail(activityId);
        }

        /// <summary>
        /// 获取需要更改红包状态的记录
        /// </summary>
        /// <returns></returns>
        public List<YX_RedGetRecord> GetRecord()
        {
            return new ScanCodeMarketDAL().GetRecord();
        }
        /// <summary>
        /// 更改领取红包状态
        /// </summary>
        /// <param name="getId">记录id</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public bool UpdateGetRecordState(long getId, int state,string getDate)
        {
            return new ScanCodeMarketDAL().UpdateGetRecordState(getId, state, getDate);
        }
         /// <summary>
        /// 是否是正确的二维码
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public bool IsRightEwm(string ewm)
        {
            return new ScanCodeMarketDAL().IsRightEwm(ewm);
        }

        /// <summary>
        /// 获取企业二维码id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public long GetIdCode(long orderId)
        {
            return new ScanCodeMarketDAL().GetIdCode(orderId);
        }

        /// <summary>
        /// 插入红包零钱
        /// </summary>
        /// <param name="change"></param>
        /// <returns></returns>
        public RetResult AddChangeRed(YX_RedSendChange change, string tel)
        {
            return new ScanCodeMarketDAL().AddChangeRed(change, tel);
        }

         /// <summary>
        /// 得到用户openId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public YX_RedSendChange GetRedChangeInfo(long userId)
        {
            return new ScanCodeMarketDAL().GetRedChangeInfo(userId);
        }

        public bool UpdateDetailRecord(long recordeId, long sendId)
        {
            return new ScanCodeMarketDAL().UpdateDetailRecord(recordeId,sendId);
        }
        public YX_AcivityDetail GetDetail(long detailId)
        {
            ScanCodeMarketDAL dal = new ScanCodeMarketDAL();
            return dal.GetDetail(detailId);
        }
    }
}
