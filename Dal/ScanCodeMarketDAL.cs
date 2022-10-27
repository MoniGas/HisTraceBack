/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-6-7
** 联系方式:15533621896
** 代码功能：红包拍码页数据层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 红包拍码页数据层
    /// </summary>
    public class ScanCodeMarketDAL : DALBase
    {
        /// <summary>
        /// 是否可以抢红包
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public RetResult CanGetRedPacket(long settingId, Enterprise_FWCode_00 fwCode, long companyIdCode, long activityId = 0)
        {
            RetResult result = new RetResult { CmdError = CmdResultError.EXCEPTION };
            try
            {
                using (var dataContext = GetDataContext())
                {
                    var relationModel = dataContext.YX_ActvitiyRelationCode.Join(dataContext.YX_ActivitySub, a => a.ActivityID, b => b.ActivityID, (a, b) =>
                        new
                        {
                            a.RequestSettingID,
                            b.ActiveStatus,
                            b.StartDate,
                            b.ActivityType,
                            b.EndDate,
                            a.ActivityID,
                            b.OpenMode,
                            a.CompanyIDcodeID,
                            b.ActivityMethod,
                            b.RedStyle
                        }).FirstOrDefault(a => a.ActiveStatus ==
                            (int)Common.EnumText.ActivityState.Going
                            && (a.RequestSettingID == settingId || a.CompanyIDcodeID == companyIdCode || a.ActivityID == activityId)
                            && a.StartDate <= DateTime.Now.Date && a.EndDate >= DateTime.Now.Date && a.ActivityMethod == (int)Common.EnumText.ActivityMethod.Packet);
                    if (relationModel == null)
                    {
                        result.Msg = "该活动已经结束，谢谢您的参与！";
                        result.Code = 1;
                    }
                    else
                    {
                        if (relationModel.ActivityType == (int)Common.EnumText.ActiveType.Multi && relationModel.ActivityMethod == (int)Common.EnumText.ActivityMethod.Packet)
                        {
                            if (dataContext.YX_AcivityDetail.Where(a => a.ActivityID == relationModel.ActivityID && a.RedLastCount > 0).Count() <= 0)
                            {
                                result.Msg = "您没有抢到红包，谢谢您的参与！";
                                result.Code = 1;
                            }
                            else if (fwCode == null || string.IsNullOrEmpty(fwCode.EWM))
                            {
                                result.Msg = "您没有抢到红包，谢谢您的参与！";
                                result.Code = 1;
                            }
                            else if (fwCode.Type == (int)Common.EnumFile.CodeType.bGroup
                                || fwCode.Type == (int)Common.EnumFile.CodeType.boxCode
                                || fwCode.Type == (int)Common.EnumFile.CodeType.localBox
                                || fwCode.Type == (int)Common.EnumFile.CodeType.localGift)
                            {
                                result.Msg = "您没有抢到红包，谢谢您的参与！";
                                result.Code = 1;
                            }
                            else if (dataContext.YX_RedSendRecord.FirstOrDefault(a => a.IDcode == fwCode.EWM) != null)
                            {
                                result.Msg = "该二维码红包已经被抢过，谢谢您的参与！";
                                result.Code = 5;
                            }
                            //else if (dataContext.YX_RedSendChange.FirstOrDefault(a => a.IDcode == ewm)!=null)
                            //{
                            //    result.Msg = "该二维码红包已经被抢过，谢谢您的参与！";
                            //    result.Code = 4;
                            //}
                            else
                            {
                                if (relationModel.RedStyle == (int)Common.EnumText.RedStyle.藏)
                                {
                                    result.Msg = "具备藏红包条件！";
                                    result.Code = 3;
                                }
                                else
                                {
                                    result.Msg = "具备抢红包条件！";
                                    result.Code = 2;
                                }
                            }
                        }
                        else if (relationModel.ActivityType == (int)Common.EnumText.ActiveType.Multi && relationModel.ActivityMethod == (int)Common.EnumText.ActivityMethod.Lottery)
                        {
                            if (dataContext.YX_LotterySendRecord.FirstOrDefault(a => a.IDcode == fwCode.EWM) != null)
                            {
                                result.Msg = "该二维码活动已经被领取，谢谢您的参与！";
                                result.Code = 5;
                            }
                            else
                            {
                                result.Msg = "具备领取大转盘活动的条件！";
                                result.Code = 6;
                            }
                        }
                        else
                        {
                            result.Msg = "具备抢红包条件！";
                            result.Code = 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【CanGetRedPacket报错 " + DateTime.Now + "：】" + ex.ToString());
                result.Code = 0;
            }
            return result;
        }

        /// <summary>
        /// 根据配置码获取企业关联码
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public YX_ActivitySub GetModel(long settingId, long companyIdCode)
        {
            YX_ActivitySub model = new YX_ActivitySub();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    model = dataContext.YX_ActivitySub.Join(dataContext.YX_ActvitiyRelationCode, a => a.ActivityID, b => b.ActivityID,
                        (a, b) => new { a, b.RequestSettingID, b.CompanyIDcodeID }).FirstOrDefault(a => a.RequestSettingID == settingId || a.CompanyIDcodeID == companyIdCode).a;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【GetModel报错 " + DateTime.Now + "：】" + ex.ToString() + " settingId:" + settingId + ";companyIdcode" + companyIdCode);
            }
            return model;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public View_Activity GetActivity(long settingId, long activityId)
        {
            View_Activity model = new View_Activity();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    var data = from c in dataContext.View_Activity select c;
                    if (settingId > 0)
                    {
                        data = data.Where(a => a.RequestSettingID == settingId);
                    }
                    if (activityId > 0)
                    {
                        data = data.Where(a => a.ActivityID == activityId);
                    }
                    model = data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【GetActivity报错 " + DateTime.Now + "：】" + ex.ToString());
            }
            return model;
        }

        /// <summary>
        /// 添加用户记录和领取记录，发送记录,发送记录新增时，红包剩余数量减一
        /// </summary>
        /// <param name="scanRecord">拍码记录</param>
        /// <param name="getRecord">领取记录</param>
        /// <param name="sendRecord">发放记录</param>
        /// <param name="userId">大于0为零钱发送</param>
        /// <returns></returns>
        public long AddRecord(YX_Redactivity_ScanRecord scanRecord, YX_RedGetRecord getRecord)
        {
            long id = 0;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    if (scanRecord != null)
                    {
                        scanRecord.ScanDate = DateTime.Now;
                        dataContext.YX_Redactivity_ScanRecord.InsertOnSubmit(scanRecord);
                        dataContext.SubmitChanges();
                        id = scanRecord.ScanRecordID;
                    }
                    if (getRecord != null)
                    {
                        dataContext.YX_RedGetRecord.InsertOnSubmit(getRecord);
                        dataContext.SubmitChanges();
                        return getRecord.GetRecordID;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【AddRecord报错 " + DateTime.Now + "：】" + ex.ToString());
            }
            return id;
        }
        /// <summary>
        /// 零钱新增记录
        /// </summary>
        /// <param name="getRecord"></param>
        /// <returns></returns>
        public long AddChangeRecord(YX_RedGetChangeRecord getRecord)
        {
            long id = 0;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    if (getRecord != null)
                    {
                        dataContext.YX_RedGetChangeRecord.InsertOnSubmit(getRecord);
                        dataContext.SubmitChanges();
                        return getRecord.GetRecordID;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【AddChangeRecord报错 " + DateTime.Now + "：】" + ex.ToString());
            }
            return id;
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
            long id = 0;
            try
            {
                #region 新增发送表
                using (var dataContext = GetDataContext())
                {
                    //更新发送记录
                    if (sendRecord == null)
                    {
                        return id;
                    }
                    WriteLog.WriteErrorLog("【插入数据" + sendRecord.IDcode + "】  MarId:" + sendRecord.MarId + "  WeiXinName:" + sendRecord.WeiXinName);
                    var model = dataContext.YX_RedSendRecord.Where(p => p.IDcode == sendRecord.IDcode).FirstOrDefault();
                    dataContext.YX_RedSendRecord.InsertOnSubmit(sendRecord);
                    try
                    {
                        dataContext.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteErrorLog("【重复插入报错" + sendRecord.IDcode + "】");
                        return -3;
                    }
                }
                #endregion
                bool flag = true;
                int k = 5;
                while (flag)
                {
                    if (k <= 0)
                    {
                        break;
                    }
                    #region 修改二维码剩余数量
                    using (var dataContext = GetDataContext())
                    {
                        //只有在发送大额红包时才会减少红包数量以及记日志
                        YX_AcivityDetailRecord record = new YX_AcivityDetailRecord();
                        var detail = dataContext.YX_AcivityDetail.FirstOrDefault(a => a.ActivityID == sendRecord.ActivityID && a.RedValue == sendRecord.SendRedValue
                            && a.RedLastCount > 0);
                        if (detail != null)
                        {
                            //记录更新活动数量日志
                            detail.RedLastCount = detail.RedLastCount - 1;
                            record.SendRecordId = sendRecord.RecordID;
                            record.ChangeType = (int)Common.EnumFile.RedChangeType.sendRed;
                            record.RecordDate = DateTime.Now;
                            record.BeforeCount = detail.RedLastCount + 1;
                            record.AfterCount = detail.RedLastCount;
                            record.AcivityRedDetailID = detail.AcivityRedDetailID;
                            record.SendRecordId = sendRecord.RecordID;
                            dataContext.YX_AcivityDetailRecord.InsertOnSubmit(record);
                            try
                            {
                                dataContext.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
                                id = sendRecord.RecordID;
                                break;
                            }
                            catch (System.Data.Linq.ChangeConflictException ex)
                            {
                                WriteLog.WriteErrorLog("【更新红包数量数量捕捉冲突,红包数量还有，保持当前的值】");
                                //重复一遍
                                flag = true;
                                k--;
                                continue;
                            }
                        }
                        else
                        {
                            WriteLog.WriteErrorLog("【红包数量为0，不可以再抢 " + DateTime.Now + "】码号为" + sendRecord.RecordID);
                            break;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【插入红包，更新红包数量数量出错 " + DateTime.Now + "】" + ex.ToString());
            }
            //如果失败删除发送记录
            if (id <= 0)
            {
                using (var dataContext = GetDataContext())
                {
                    YX_RedSendRecord recorDel = dataContext.YX_RedSendRecord.Where(p => p.RecordID == sendRecord.RecordID).FirstOrDefault();
                    if (recorDel != null)
                    {
                        dataContext.YX_RedSendRecord.DeleteOnSubmit(recorDel);
                        dataContext.SubmitChanges();
                    }
                }
            }
            return id;
        }

        public long UpdateSendRecord(YX_RedSendRecord sendRecord, out long recordeId)
        {
            long id = 0;
            recordeId = 0;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    //更新发送记录
                    if (sendRecord == null)
                    {
                        return id;
                    }
                    var model = dataContext.YX_RedSendRecord.Where(p => p.IDcode == sendRecord.IDcode).FirstOrDefault();
                    if (model != null)
                    {
                        model.BillNumber = sendRecord.BillNumber;
                        model.SendRedValue = sendRecord.SendRedValue;
                        model.WeiXinUserID = sendRecord.WeiXinUserID;
                        model.SendType = sendRecord.SendType;
                        model.WxListId = sendRecord.WxListId;
                        id = model.RecordID;
                        dataContext.SubmitChanges();
                        WriteLog.WriteErrorLog("修改发送记录后" + model.IDcode + ";" + model.RecordID);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【插入红包，更新红包数量数量出错 " + DateTime.Now + "】" + ex.ToString());
                return 0;
            }
            return id;
        }

        /// <summary>
        /// 发送失败后删除发送记录
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="activityId">活动编号</param>
        /// <returns></returns>
        public RetResult DeleteSendRecord(string ewm, long activityId)
        {
            RetResult result = new RetResult();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    //更新发送记录
                    if (string.IsNullOrEmpty(ewm) || activityId <= 0)
                    {
                        return result;
                    }
                    var model = dataContext.YX_RedSendRecord.Where(p => p.IDcode == ewm && p.ActivityID == activityId
                        && p.BillNumber.Length == 0).FirstOrDefault();
                    if (model != null)
                    {
                        dataContext.YX_RedSendRecord.DeleteOnSubmit(model);
                        dataContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【删除发送记录出错，ewm: " + ewm + DateTime.Now + "】" + ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// 新增/修改发送记录，同时更新红包剩余数量
        /// </summary>
        /// <param name="sendRecord"></param>
        /// <param name="sendRecordId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public long AddSendChangeRecord(YX_RedSendChangeRecord sendRecord, long userId)
        {
            long id = 0;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    //更新发送记录
                    if (sendRecord == null)
                    {
                        return id;
                    }
                    dataContext.YX_RedSendChangeRecord.InsertOnSubmit(sendRecord);
                    dataContext.SubmitChanges();
                    id = sendRecord.RecordID;
                    //零钱提现后，需要修改零钱表中发送编号
                    dataContext.YX_RedSendChange.Where(a => a.Order_Consumers_ID == userId && a.SendRedID == 0).ToList().ForEach(a => a.SendRedID = id);
                    dataContext.SubmitChanges();
                    return id;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【插入零钱红包，更新红包数量数量出错 " + DateTime.Now + "】" + sendRecord.ActivityID);
                return 0;
            }
        }
        /// <summary>
        /// 插入红包零钱,同时红包剩余数量减一
        /// </summary>
        /// <param name="change"></param>
        /// <returns></returns>
        public RetResult AddChangeRed(YX_RedSendChange change, string tel)
        {
            RetResult result = new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "操作失败，请重新操作" };
            try
            {
                #region 新增领取零钱记录表
                using (var dataContext = GetDataContext())
                {
                    if (change == null)
                    {
                        return result;
                    }
                    Order_Consumers consumer = dataContext.Order_Consumers.FirstOrDefault(a => a.LinkPhone == tel);
                    if (consumer == null)
                    {
                        consumer = new Order_Consumers { LinkPhone = tel, AddDate = DateTime.Now };
                        dataContext.Order_Consumers.InsertOnSubmit(consumer);
                    }
                    dataContext.SubmitChanges();
                    change.AddDate = DateTime.Now;
                    change.Order_Consumers_ID = consumer.Order_Consumers_ID;
                    dataContext.YX_RedSendChange.InsertOnSubmit(change);
                    //IDcode 唯一，重复插入会报错
                    try
                    {
                        dataContext.SubmitChanges();
                    }
                    catch
                    {
                        result.Msg = "该二维码红包已经被抢过，谢谢您的参与！";
                        WriteLog.WriteErrorLog("【该二维码已经被抢】");
                        return result;
                    }
                }
                #endregion

                #region 更新红包剩余数量
                bool flag = true;
                int k = 5;
                while (flag)
                {
                    using (var dataContext = GetDataContext())
                    {
                        //修改活动中红包领取数量
                        var detail = dataContext.YX_AcivityDetail.FirstOrDefault(a => a.ActivityID == change.ActivityId && a.RedValue == change.RedValue
                            && a.RedLastCount > 0);
                        if (detail != null)
                        {
                            detail.RedLastCount = detail.RedLastCount - 1;
                            //记录更新活动数量日志
                            YX_AcivityDetailRecord record = new YX_AcivityDetailRecord();
                            record.RecordDate = DateTime.Now;
                            record.BeforeCount = detail.RedLastCount + 1;
                            record.AfterCount = detail.RedLastCount;
                            record.AcivityRedDetailID = detail.AcivityRedDetailID;
                            record.ChangeType = (int)Common.EnumFile.RedChangeType.sendChangeRed;
                            record.SendChangeRecordId = change.RedID;
                            dataContext.YX_AcivityDetailRecord.InsertOnSubmit(record);
                            try
                            {
                                dataContext.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
                                result.CmdError = CmdResultError.NONE;
                                result.Msg = "领取成功！";
                                break;
                            }
                            catch (System.Data.Linq.ChangeConflictException ex)
                            {
                                k--;
                                continue;
                            }
                        }
                        else
                        {
                            result.Msg = "您没有抢到红包，谢谢您的参与！";
                            break;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【领取零钱红包，更新红包数量数量出现异常 " + DateTime.Now + "】" + ex.ToString());
                result.Msg = "您没有抢到红包，谢谢您的参与！";
            }
            if (result.CmdError != CmdResultError.NONE)
            {
                using (var dataContext = GetDataContext())
                {
                    YX_RedSendChange recorDel = dataContext.YX_RedSendChange.Where(p => p.RedID == change.RedID).FirstOrDefault();
                    if (recorDel != null)
                    {
                        dataContext.YX_RedSendChange.DeleteOnSubmit(recorDel);
                        dataContext.SubmitChanges();
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// 获取活动金额明细
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public double?[] GetActivityDetail(long activityId)
        {
            try
            {
                using (var dataContext = GetDataContext())
                {
                    return dataContext.YX_AcivityDetail.Where(a => a.ActivityID == activityId && a.RedLastCount.Value > 0).Select(a => a.RedValue).ToArray();
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【GetActivityDetail报错 " + DateTime.Now + "：】" + ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// 获取需要更改红包状态的记录
        /// </summary>
        /// <returns></returns>
        public List<YX_RedGetRecord> GetRecord()
        {
            List<YX_RedGetRecord> lst = new List<YX_RedGetRecord>();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    return dataContext.YX_RedGetRecord.Where(a => a.GetState != (int)Common.EnumText.GetState.FAILED && a.GetState !=
                        (int)Common.EnumText.GetState.RECEIVED && a.GetState != (int)Common.EnumText.GetState.REFUND).ToList();
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【GetRecord报错 " + DateTime.Now + "：】" + ex.ToString());
            }
            return lst;
        }
        /// <summary>
        /// 线程更改领取红包状态,以及修改红包剩余数量
        /// </summary>
        /// <param name="getId">记录id</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public bool UpdateGetRecordState(long getId, int state, string getDate)
        {
            try
            {
                using (var dataContext = GetDataContext())
                {
                    var model = dataContext.YX_RedGetRecord.FirstOrDefault(a => a.GetRecordID == getId);
                    if (model != null)
                    {
                        if (model.SendType == (int)Common.EnumFile.RedSendType.red)
                        {
                            if (state == (int)Common.EnumText.GetState.FAILED || state == (int)Common.EnumText.GetState.REFUND)
                            {
                                YX_AcivityDetail detail = dataContext.YX_AcivityDetail.Where(p => p.ActivityID == model.ActivityID && p.RedValue == model.GetRedValue).FirstOrDefault();
                                if (detail.RedLastCount < detail.RedCount)
                                {
                                    YX_AcivityDetailRecord record = new YX_AcivityDetailRecord();
                                    record.GetRecordID = getId;
                                    record.RecordDate = DateTime.Now;
                                    record.BeforeCount = detail.RedLastCount;
                                    record.AfterCount = detail.RedLastCount + 1;
                                    record.AcivityRedDetailID = detail.AcivityRedDetailID;
                                    record.ChangeType = (int)Common.EnumFile.RedChangeType.getRed;
                                    dataContext.YX_AcivityDetailRecord.InsertOnSubmit(record);
                                    detail.RedLastCount = detail.RedLastCount + 1;//记录更新活动数量日志
                                }
                            }
                        }
                        model.GetState = state;
                        model.GetDate = getDate;
                        dataContext.SubmitChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【UpdateGetRecordState报错 " + DateTime.Now + "：】" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 是否是正确的二维码
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public bool IsRightEwm(string ewm)
        {
            try
            {
                using (var dataContext = GetDataContext())
                {
                    string[] arr = ewm.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] or = arr[2].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    long orderId = Convert.ToInt64(or[0]);
                    long code = Convert.ToInt64(or[1]);
                    var model = dataContext.YX_CompanyIDcode.FirstOrDefault(a => a.CompanyIDcodeID == orderId && code >= a.FromCode && code <= a.EndCode);
                    if (model != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【IsRightEwm报错 " + DateTime.Now + "：】" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 获取企业二维码id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public long GetIdCode(long orderId)
        {
            try
            {
                using (var dataContext = GetDataContext())
                {
                    var model = dataContext.YX_CompanyIDcode.FirstOrDefault(a => a.ActivityID == orderId);
                    return model.CompanyIDcodeID;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【GetIdCode报错 " + DateTime.Now + "：】" + ex.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 得到用户openId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public YX_RedSendChange GetRedChangeInfo(long userId)
        {
            using (var dataContext = GetDataContext())
            {
                var model = dataContext.YX_RedSendChange.FirstOrDefault(a => a.Order_Consumers_ID == userId && (a.SendRedID == 0 || a.SendRedID == null));
                return model;
            }
        }

        /// <summary>
        /// 更新日志表中的记录
        /// </summary>
        /// <param name="recordeId"></param>
        /// <param name="sendId"></param>
        /// <returns></returns>
        public bool UpdateDetailRecord(long recordeId, long sendId)
        {
            try
            {
                using (var dataContext = GetDataContext())
                {
                    var model = dataContext.YX_AcivityDetailRecord.FirstOrDefault(a => a.RecordID == recordeId);
                    if (model != null)
                    {
                        model.SendRecordId = sendId;
                        dataContext.SubmitChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【UpdateDetailRecord报错 " + DateTime.Now + "：】" + ex.ToString());
            }
            return false;
        }
        /// <summary>
        /// 获取活动明细
        /// </summary>
        /// <param name="detailId"></param>
        /// <returns></returns>
        public YX_AcivityDetail GetDetail(long detailId)
        {
            YX_AcivityDetail model = new YX_AcivityDetail();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    model = dataContext.YX_AcivityDetail.FirstOrDefault(a => a.AcivityRedDetailID == detailId && a.RedLastCount > 0);
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("【GetActivityDetail报错 " + DateTime.Now + "：】" + ex.ToString());
            }
            return model;
        }
    }
}
