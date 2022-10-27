/********************************************************************************
** 作者：赵慧敏
** 开发时间：2017-6-7
** 联系方式:13313318725
** 代码功能：新建活动数据层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using System.Data.Common;
using Common.Argument;
using Common.Tools;
using System.Configuration;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 新建活动数据层
    /// </summary>
    public class ActivityDAL : DALBase
    {
        /// <summary>
        /// 添加活动
        /// </summary>
        /// <param name="model">活动模型</param>
        /// <param name="modelSub">活动模型</param>
        /// <param name="details">红包配置</param>
        /// <returns></returns>
        public RetResult AddModel(YX_Activity model, YX_ActivitySub modelSub, List<YX_AcivityDetail> details, YX_RedRecharge recharge,long SetingID, out long activityID)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "新建活动失败";
            activityID = 0;
            int? codeNum = details.Sum(a => a.RedCount);
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    YX_ActivitySub activity = dataContext.YX_ActivitySub.FirstOrDefault(p => p.ActivityTitle == modelSub.ActivityTitle
                        && p.ActiveStatus < 2 && p.CompanyID == modelSub.CompanyID);
                    modelSub.ActivityMethod = (int)Common.EnumText.ActivityMethod.Packet;
                    dataContext.YX_ActivitySub.InsertOnSubmit(modelSub);
                    dataContext.SubmitChanges();
                    model.ActivityID = modelSub.ActivityID;
                    dataContext.YX_Activity.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    //红包充值
                    string orderNum = "D" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + (dataContext.YX_RedRecharge.Count() + 1);
                    recharge.OrderNum = orderNum;
                    recharge.OrderNumAgain = orderNum;
                    recharge.ActivityID = model.ActivityID;
                    dataContext.YX_RedRecharge.InsertOnSubmit(recharge);
                    dataContext.SubmitChanges();
                    //if (CurrentUser.User.SetingID > 0)
                    if (SetingID > 0)
                    {
                        RequestCodeSetting seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.EnterpriseId == CurrentUser.User.EnterpriseID
                                && p.ID == CurrentUser.User.SetingID);
                        if (seting != null)
                        {
                            YX_ActvitiyRelationCode relation =
                                new YX_ActvitiyRelationCode
                                {
                                    ActivityID = modelSub.ActivityID,
                                    CodeCount = seting.Count,
                                    CompanyIDcode = SessCokie.Get.MainCode,
                                    CompanyID = SessCokie.Get.EnterpriseID,
                                    EndCode = seting.endCode,
                                    StartCode = seting.beginCode,
                                    RelationDate = DateTime.Now,
                                    RequestSettingID = CurrentUser.User.SetingID,
                                    Flag = (int) Common.EnumText.CodeType.TraceCode
                                };
                            dataContext.YX_ActvitiyRelationCode.InsertOnSubmit(relation);
                            var setting = dataContext.RequestCodeSetting.FirstOrDefault(t => t.ID == CurrentUser.User.SetingID);
                            if (seting != null)
                            {
                                setting.PacketState = (int)Common.EnumFile.PacketState.Success;
                            }
                            dataContext.SubmitChanges();
                        }
                        else
                        {
                            tran.Rollback();
                            result.Msg = "创建失败,没有找到追溯码相关数据";
                            return result;
                        }
                    }
                    else
                    {
                        tran.Rollback();
                        result.Msg = "创建失败";
                        return result;
                    }
                    if (details.Any())
                    {
                        foreach (var sub in details)
                        {
                            sub.AcivityRedID = model.AcivityRedID;
                            sub.ActivityID = model.ActivityID;
                        }
                        dataContext.YX_AcivityDetail.InsertAllOnSubmit(details);
                        dataContext.SubmitChanges();
                        tran.Commit();
                        result.CmdError = CmdResultError.NONE;
                        result.Msg = "创建成功";
                        activityID = (long)model.ActivityID;
                    }
                    else
                    {
                        tran.Rollback();
                        result.Msg = "请进行红包设置";
                    }
                }
                catch
                {
                    tran.Rollback();
                    result.Msg = "创建失败";
                    return result;
                }
            }
            return result;
        }

        public RetResult FindTitle(string ActivityTitle, long CompanyID)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "查询失败";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    YX_ActivitySub activity = dataContext.YX_ActivitySub.FirstOrDefault(p => p.ActivityTitle == ActivityTitle
                        && p.ActiveStatus < 2 && p.CompanyID == CompanyID);
                    if (activity != null)
                    {
                        result.Msg = "已存在该活动名称！";
                    }
                    else
                    {
                        result.Msg = "成功";
                        result.CmdError = CmdResultError.NONE;
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// 创建红包活动成功后，调用接口
        /// </summary>
        /// <param name="settingId"></param>
        private void UpdateSettingCodeState(long settingId)
        {
            //string data = Common.Tools.ApiHelper.SendRequest(System.Configuration.ConfigurationManager.AppSettings["NcpUrl"] + "?settingId=" + settingId, null, "POST");
            //ResponseSettingCodeModel model = JsonHelper.DeserializeJsonToObject<ResponseSettingCodeModel>(data);
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public YX_Activity GetActivity(long activityID)
        {
            YX_Activity model = new YX_Activity();
            using (DataClassesDataContext dct = GetDataContext())
            {
                try
                {
                    model = dct.YX_Activity.FirstOrDefault(p => p.ActivityID == activityID);
                }
                catch
                { }
            }
            return model;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public YX_ActivitySub GetActivitySub(long activityID)
        {
            YX_ActivitySub model = new YX_ActivitySub();
            using (DataClassesDataContext dct = GetDataContext())
            {
                try
                {
                    model = dct.YX_ActivitySub.FirstOrDefault(p => p.ActivityID == activityID);
                }
                catch
                { }
            }
            return model;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityID">活动编号</param>
        /// <returns></returns>
        public List<YX_AcivityDetail> GetDetail(long activityID)
        {
            List<YX_AcivityDetail> model = new List<YX_AcivityDetail>();
            using (DataClassesDataContext dct = GetDataContext())
            {
                try
                {
                    model = dct.YX_AcivityDetail.Where(p => p.ActivityID == activityID).ToList();
                }
                catch
                { }
            }
            return model;
        }

        /// <summary>
        /// 编辑活动
        /// </summary>
        /// <param name="modelSub">活动模型</param>
        /// <returns></returns>
        public RetResult EditActivitySub(YX_ActivitySub modelSub)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "修改活动失败";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var activity = dataContext.YX_ActivitySub.FirstOrDefault(p => p.ActivityTitle == modelSub.ActivityTitle
                        && p.ActiveStatus < 2 && p.ActivityID != modelSub.ActivityID);
                    //if (activity != null)
                    //{
                    //    result.Msg = "已存在该活动名称！";
                    //    return result;
                    //}
                    var data = dataContext.YX_ActivitySub.FirstOrDefault(p => p.ActivityID == modelSub.ActivityID);
                    if (data == null)
                    {
                        result.Msg = "没有找到要修改的数据！";
                    }
                    else
                    {
                        data.ActivityStyleID = modelSub.ActivityStyleID;
                        data.ActivityTitle = modelSub.ActivityTitle;
                        data.ActivityType = modelSub.ActivityType;
                        data.AtivityImageURL = modelSub.AtivityImageURL;
                        data.Content = modelSub.Content;
                        data.ActivityType = modelSub.ActivityType;
                        data.JoinMode = modelSub.JoinMode;
                        data.ShareFriends = modelSub.ShareFriends;
                        data.StartDate = modelSub.StartDate;
                        data.EndDate = modelSub.EndDate;
                        dataContext.SubmitChanges();
                        result.Msg = "修改成功";
                        result.CmdError = CmdResultError.NONE;
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// 编辑活动
        /// </summary>
        /// <param name="model">活动模型</param>
        /// <param name="redID">活动编号</param>
        /// <returns></returns>
        public RetResult EditActivity(YX_Activity model, out long redID)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "修改活动失败";
            redID = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.YX_Activity.FirstOrDefault(p => p.ActivityID == model.ActivityID);
                    if (data == null)
                    {
                        result.Msg = "没有找到要修改的数据！";
                    }
                    else
                    {
                        redID = data.AcivityRedID;
                        data.BlessingWords = model.BlessingWords;
                        data.SendCompany = model.SendCompany;
                        data.CompanyLogoURL = model.CompanyLogoURL;
                        dataContext.SubmitChanges();
                        result.CmdError = CmdResultError.NONE;
                        result.Msg = "修改成功";
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// 修改红包配置
        /// </summary>
        /// <param name="model">配置模型</param>
        /// <param name="acitivityID">活动编号</param>
        /// <returns></returns>
        public RetResult EditDetail(List<YX_AcivityDetail> model, long acitivityID, YX_RedRecharge change)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "修改活动失败";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.YX_AcivityDetail.Where(p => p.ActivityID == acitivityID).ToList();
                    if (change != null)
                    {
                        dataContext.YX_RedRecharge.DeleteOnSubmit(dataContext.YX_RedRecharge.FirstOrDefault(a => a.ActivityID == acitivityID));
                        string orderNum = "D" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + (dataContext.YX_RedRecharge.Count() + 1);
                        change.OrderNum = orderNum;
                        change.OrderNumAgain = orderNum;
                        dataContext.YX_RedRecharge.InsertOnSubmit(change);
                    }
                    dataContext.YX_AcivityDetail.DeleteAllOnSubmit(data);
                    dataContext.YX_AcivityDetail.InsertAllOnSubmit(model);
                    dataContext.SubmitChanges();
                    result.CmdError = CmdResultError.NONE;
                    result.Msg = "修改成功";
                }
                catch
                {
                }
            }
            return result;
        }

        #region 备案 标志人事物
        public RetResult RecordCode(string mainCode, long enterpriseId, string codeUseID)
        {
            string msg = "备案品类码失败！";
            string errorMemo = "";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                //string mailcodeIdcode = mainCode.Substring(0, mainCode.LastIndexOf("."));
                string ms = DateTime.Now.ToString("fff");
                string action = ConfigurationManager.AppSettings["RegOtherIDcodeInfo"];
                string access_token = ConfigurationManager.AppSettings["access_token"];
                string parseUrl = ConfigurationManager.AppSettings["IpRedirect"];
                Dictionary<string, string> paras = new Dictionary<string, string>();
                paras.Add("access_token", access_token);
                paras.Add("companyIDcode", mainCode);
                paras.Add("codeUse_ID", "90");// codeUseID);
                paras.Add("industryCategory_ID", "10133");
                paras.Add("categoryCode", "12000000");
                paras.Add("modelNumber", "营销码" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                paras.Add("modelNumberEn", "");
                paras.Add("introduction", "");
                paras.Add("codePayType", "5");
                paras.Add("goToUrl", parseUrl + "Market/Wap_IndexMarket/Index");
                string strBack = APIHelper.sendPost(action, paras, "get");
                JsonObject jsonObject = new JsonObject(strBack);
                string result = jsonObject["ResultCode"].Value;
                string resultMsg = jsonObject["ResultMsg"].Value;
                string organUnitIDcode = jsonObject["OrganUnitIDcode"].Value;
                if (result == "1")
                {
                    //msg = "备案品类码成功！";
                    error = CmdResultError.NONE;
                    msg = "1";
                    errorMemo = organUnitIDcode;

                }
                else if (result == "0")
                {
                    msg = "备案失败！";
                }
                Ret.SetArgument(error, errorMemo, msg);
            }
            catch (Exception ex)
            {
                string errData = "BuyCodeOrdePayDAL.RecordCode()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            Ret.SetArgument(error, errorMemo, msg);
            return Ret;
        }
        #endregion

        /// <summary>
        /// 查找要操作的数据库
        /// </summary>
        /// <returns></returns>
        public List<GenCondeInfoSetting> GetGencodeInfo()
        {
            List<GenCondeInfoSetting> set = new List<GenCondeInfoSetting>();
            using (DataClassesDataContext db = GetDataContext("Constr"))
            {
                try
                {
                    set = db.GenCondeInfoSetting.Where(p=>p.State==1 && p.ActivityState==1).ToList();
                }
                catch
                {
                }
            }
            return set;
        }        
        /// <summary>
        /// 查找要操作的数据库生成码包用
        /// </summary>
        /// <returns></returns>
        public List<GenCondeInfoSetting> GetGencodeFileInfo()
        {
            List<GenCondeInfoSetting> set = new List<GenCondeInfoSetting>();
            using (DataClassesDataContext db = GetDataContext("Constr"))
            {
                try
                {
                    set = db.GenCondeInfoSetting.Where(p => p.State == 1).ToList();
                }
                catch
                {
                }
            }
            return set;
        }
        /// <summary>
        /// 活动类型
        /// </summary>
        /// <param name="activityMethod"></param>
        /// <returns></returns>
        public List<YX_ActivitySub> GetActivitySub(string ConString , int activityMethod)
        {
            List<YX_ActivitySub> activity = new List<YX_ActivitySub>();
            using (DataClassesDataContext db = GetContext(ConString))
            {
                try
                {
                    activity = db.YX_ActivitySub.Where(p => p.ActiveStatus == (int)Common.EnumText.ActivityState.Going
                        && p.RedStyle == (int)Common.EnumText.RedStyle.藏 && p.SetStatus == (int)Common.EnumText.SetStatus.NoSet
                        && p.ActivityMethod == activityMethod).ToList();
                }
                catch
                {
                }
            }
            return activity;
        }
        /// <summary>
        /// 获取明细
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public List<YX_AcivityDetail> GetActivityDetail(string ConString, long activityId)
        {
            List<YX_AcivityDetail> activity = new List<YX_AcivityDetail>();
            using (DataClassesDataContext db = GetContext(ConString))
            {
                try
                {
                    activity = db.YX_AcivityDetail.Where(p => p.ActivityID == activityId).ToList();
                }
                catch
                {
                }
            }
            return activity;
        }
        /// <summary>
        /// 获取优惠券
        /// </summary>
        /// <param name="ConString"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public YX_ActivityCoupon GetCouponDetail(string ConString, long activityId)
        {
            YX_ActivityCoupon activity = new YX_ActivityCoupon();
            using (DataClassesDataContext db = GetContext(ConString))
            {
                try
                {
                    activity = db.YX_ActivityCoupon.FirstOrDefault(p => p.ActivityID == activityId);
                }
                catch
                {
                }
            }
            return activity;
        }
        /// <summary>
        /// 查找二维码批次
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="activityId"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        public ActivityConceal GetActivityRelation(string ConString, long activityId)
        {
            ActivityConceal activity = new ActivityConceal();
            using (DataClassesDataContext db = GetContext(ConString))
            {
                try
                {
                    YX_ActvitiyRelationCode relation = db.YX_ActvitiyRelationCode.FirstOrDefault(p => p.ActivityID == activityId);
                    if (relation != null)
                    {
                        RequestCodeSetting setting = db.RequestCodeSetting.FirstOrDefault(p => p.ID == relation.RequestSettingID);
                        if (setting != null)
                        {
                            RequestCode request = db.RequestCode.FirstOrDefault(p => p.RequestCode_ID == setting.RequestID);
                            if (request != null && request.Route_DataBase_ID > 0)
                            {
                                Route_DataBase database = db.Route_DataBase.FirstOrDefault(p => p.Route_DataBase_ID == request.Route_DataBase_ID);
                                if (database != null)
                                {
                                    activity.conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};", database.DataSource,
                                        database.DataBaseName,database.UID, database.PWD);
                                    activity.tableName = database.TableName;
                                    using (DataClassesDataContext dataContextDynamic = GetContext(activity.conStr))
                                    {
                                        StringBuilder strSql = new StringBuilder();
                                        long totalCount = setting.Count;
                                        long minusCount = setting.beginCode.Value - request.StartNum.Value;
                                        //strSql.Append("select top " + totalCount + " * from " + database.TableName + " where RequestCode_ID=" + request.RequestCode_ID
                                        //        + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                        //        + " Enterprise_FWCode_ID  from " + database.TableName + " where RequestCode_ID=" + request.RequestCode_ID
                                        //        + " order by Enterprise_FWCode_ID  ) order by Enterprise_FWCode_ID ");
                                        strSql.Append("select top " + totalCount + " * from " + database.TableName + " where RequestCode_ID=" + request.RequestCode_ID
                                            + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                            + " Enterprise_FWCode_ID  from " + database.TableName + " where RequestCode_ID=" + request.RequestCode_ID
                                            + " order by Enterprise_FWCode_ID  )  and type!=4  ");//将type=4箱码过滤掉
                                        strSql.Append("  order by Enterprise_FWCode_ID ");
                                        List<Enterprise_FWCode_00> result = dataContextDynamic.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).ToList();
                                        if (result.Count > 0)
                                        {
                                            activity.codeNum = result.Count;
                                            activity.activityId = activityId;
                                            activity.codeId = result[0].Enterprise_FWCode_ID + "," + result[result.Count - 1].Enterprise_FWCode_ID;
                                            activity.startId = result[0].Enterprise_FWCode_ID;
                                            activity.endId = result[result.Count - 1].Enterprise_FWCode_ID;
                                            strSql.Append("select top " + totalCount + " Enterprise_FWCode_ID from " + database.TableName + " where RequestCode_ID=" + request.RequestCode_ID
                                            + " and Enterprise_FWCode_ID not in (select  top " + minusCount
                                            + " Enterprise_FWCode_ID  from " + database.TableName + " where RequestCode_ID=" + request.RequestCode_ID
                                            + " order by Enterprise_FWCode_ID  )  and type!=4  ");
                                            strSql.Append("  order by Enterprise_FWCode_ID ");
                                            List<long> fwCode = dataContextDynamic.ExecuteQuery<long>(strSql.ToString()).ToList();
                                            activity.fwCode = fwCode;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return activity;
        }

        /// <summary>
        /// 藏红包方法
        /// </summary>
        /// <param name="conceal"></param>
        /// <returns></returns>
        public bool UpdateRedCode(ActivityConceal conceal)
        {
            bool setStatus = false;
            try
            {
                using (DataClassesDataContext dataContextDynamic = GetContext(conceal.conStr))
                {
                    string sql = string.Empty;
                    string codeIds = conceal.ConcealCodeIds;
                    //if (conceal.concealNum == conceal.codeNum)
                    //{
                    //    string[] codes = conceal.codeId.Split(',');
                    //    for (int i = Convert.ToInt32(codes[0]); i <= Convert.ToInt32(codes[1]); i++)
                    //    {
                    //        if (!string.IsNullOrEmpty(codeIds))
                    //        {
                    //            codeIds = codeIds + "," + i;
                    //        }
                    //        else
                    //        {
                    //            codeIds = i.ToString();
                    //        }
                    //    }
                    //}
                    string[] codeId = codeIds.Split(',');
                    for (int i = 0; i < codeId.Count(); i++)
                    {
                        var id = conceal.redList[i];
                        //100条执行一个sql串
                        if (i % 100 == 0)
                        {
                            if (!string.IsNullOrEmpty(sql))
                            {
                                dataContextDynamic.ExecuteCommand(sql,conceal.conStr); //执行SQL命令 
                            }
                            sql = @"update " + conceal.tableName + "  set ActivitySubID=" + id + " where Enterprise_FWCode_ID=" + codeId[i];
                        }
                        else
                        {
                            sql = sql + ";" + @"update " + conceal.tableName + "  set ActivitySubID=" + id + " where Enterprise_FWCode_ID=" + codeId[i];
                        }
                    }
                    if (!string.IsNullOrEmpty(sql))
                    {
                        dataContextDynamic.ExecuteCommand(sql, conceal.conStr); 
                    }
                }
                using (DataClassesDataContext dataContext = GetDataContext("Constr"))
                {
                    YX_ActivitySub activity = dataContext.YX_ActivitySub.FirstOrDefault(p => p.ActivityID == conceal.activityId);
                    if (activity != null)
                    {
                        activity.SetStatus = (int)Common.EnumText.SetStatus.IsSet;
                        dataContext.SubmitChanges();
                        setStatus = true;
                    }
                }
            }
            catch(Exception ex)
            { }
            return setStatus;
        }

        /// <summary>
        /// 藏优惠券方法
        /// </summary>
        /// <param name="conceal"></param>
        /// <returns></returns>
        public bool UpdateCoupon(ActivityConceal conceal)
        {
            bool setStatus = false;
            try
            {
                using (DataClassesDataContext dataContextDynamic = GetContext(conceal.conStr))
                {
                    string sql = string.Empty;
                    string codeIds = conceal.ConcealCodeIds;
                    //if (conceal.concealNum == conceal.codeNum)
                    //{
                    //    string[] codes = conceal.codeId.Split(',');
                    //    for (int i = Convert.ToInt32(codes[0]); i <= Convert.ToInt32(codes[1]); i++)
                    //    {
                    //        if (!string.IsNullOrEmpty(codeIds))
                    //        {
                    //            codeIds = codeIds + "," + i;
                    //        }
                    //        else
                    //        {
                    //            codeIds = i.ToString();
                    //        }
                    //    }
                    //}
                    string[] codeId = codeIds.Split(',');
                    for (int i = 0; i < codeId.Count(); i++)
                    {
                        var id = conceal.redList[i];
                        //100条执行一个sql串
                        if (i % 100 == 0)
                        {
                            if (!string.IsNullOrEmpty(sql))
                            {
                                dataContextDynamic.ExecuteCommand(sql, conceal.conStr); //执行SQL命令 
                            }
                            sql = @"update " + conceal.tableName + "  set ActivityCouponID=" + id + " where Enterprise_FWCode_ID=" + codeId[i];
                        }
                        else
                        {
                            sql = sql + ";" + @"update " + conceal.tableName + "  set ActivityCouponID=" + id + " where Enterprise_FWCode_ID=" + codeId[i];
                        }
                    }
                    if (!string.IsNullOrEmpty(sql))
                    {
                        dataContextDynamic.ExecuteCommand(sql, conceal.conStr);
                    }
                }
                using (DataClassesDataContext dataContext = GetDataContext("Constr"))
                {
                    YX_ActivitySub activity = dataContext.YX_ActivitySub.FirstOrDefault(p => p.ActivityID == conceal.activityId);
                    if (activity != null)
                    {
                        activity.SetStatus = (int)Common.EnumText.SetStatus.IsSet;
                        dataContext.SubmitChanges();
                        setStatus = true;
                    }
                }
            }
            catch (Exception ex)
            { }
            return setStatus;
        }

        public bool UpdateActivityState(string conStr)
        {
            bool setStatus = false;
            try
            {
                using (DataClassesDataContext dataContext = GetContext(conStr))
                {
                    DateTime endTime=Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
                    List<YX_ActivitySub> activity = dataContext.YX_ActivitySub.Where(p => p.ActiveStatus ==(int)Common.EnumText.ActivityState.Going
                        && p.EndDate < endTime).ToList();
                   foreach(YX_ActivitySub sub in activity)
                    {
                        sub.ActiveStatus = (int)Common.EnumText.ActivityState.Finish;
                        dataContext.SubmitChanges();
                        setStatus = true;
                    }
                }
            }
            catch (Exception ex)
            { }
            return setStatus;
        }
    }
}
