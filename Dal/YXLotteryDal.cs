/********************************************************************************
** 作者：张翠霞
** 开发时间：2019-2-12
** 代码功能：大转盘数据层
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Argument;
using LinqModel;
using System.IO;
using System.Data;
using Aspose.Cells;
using System.Data.Common;
using Common.Log;

namespace Dal
{
    public class YXLotteryDal : DALBase
    {
        /// <summary>
        /// 添加大转盘第一步添加主表信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult AddActive(YX_ActivitySub modelSub, long SetingID)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "新建活动失败";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    modelSub.ActivityMethod = (int)Common.EnumText.ActivityMethod.Lottery;
                    modelSub.SetStatus = (int)Common.EnumText.SetStatus.NoSet;
                    dataContext.YX_ActivitySub.InsertOnSubmit(modelSub);
                    dataContext.SubmitChanges();
                    if (SetingID > 0)
                    {
                        RequestCodeSetting seting = dataContext.RequestCodeSetting.Where(p => p.EnterpriseId == CurrentUser.User.EnterpriseID
                                && p.ID == CurrentUser.User.SetingID).FirstOrDefault();
                        if (seting != null)
                        {
                            YX_ActvitiyRelationCode relation = new YX_ActvitiyRelationCode();
                            relation.ActivityID = modelSub.ActivityID;
                            relation.CodeCount = seting.Count;
                            relation.CompanyIDcode = SessCokie.Get.MainCode;
                            relation.CompanyID = SessCokie.Get.EnterpriseID;
                            relation.EndCode = seting.endCode;
                            relation.StartCode = seting.beginCode;
                            relation.RelationDate = DateTime.Now;
                            relation.RequestSettingID = CurrentUser.User.SetingID;
                            relation.Flag = (int)Common.EnumText.CodeType.TraceCode;
                            dataContext.YX_ActvitiyRelationCode.InsertOnSubmit(relation);
                            var setting = dataContext.RequestCodeSetting.FirstOrDefault(t => t.ID == CurrentUser.User.SetingID);
                            if (seting != null)
                            {
                                setting.PacketState = (int)Common.EnumFile.PacketState.Success;
                            }
                            dataContext.SubmitChanges();
                            result.CmdError = CmdResultError.NONE;
                            result.Msg = "创建成功！";
                            result.id = modelSub.ActivityID;
                        }
                        else
                        {
                            result.Msg = "创建失败,没有找到追溯码相关数据";
                            return result;
                        }
                    }
                    else
                    {
                        result.Msg = "创建失败,没有找到追溯码相关数据!";
                        return result;
                    }
                }
                catch
                {
                    result.Msg = "创建失败";
                    return result;
                }
            }
            return result;
        }

        public RetResult AddLottery(YX_ActivityLottery model)
        {
            FileStream fs = new FileStream(model.LotteryFilePath, FileMode.Open, FileAccess.Read);
            Workbook book = new Workbook(fs);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;
            DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            RetResult result = ImportExcel(ds, model);
            if (result.IsSuccess)
            {
                result.CmdError = CmdResultError.NONE;
                result.Msg = "添加成功";
            }
            else
            {
                result.CmdError = CmdResultError.EXCEPTION;
                result.Msg = result.Msg;
            }
            return result;
        }

        public RetResult ImportExcel(System.Data.DataSet ds, YX_ActivityLottery model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "导入信息失败！";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    if (ds != null)
                    {
                        var data = dataContext.YX_ActivityLottery.FirstOrDefault(m => m.ActivityID == model.ActivityID && m.EnterpriseID == model.EnterpriseID &&
                        m.LotteryName == model.LotteryName && m.Status == (int)Common.EnumFile.Status.used);
                        if (data != null)
                        {
                            result.Msg = "该批活动已存在该奖项！";
                            result.CmdError = CmdResultError.EXCEPTION;
                        }
                        else
                        {
                            dataContext.YX_ActivityLottery.InsertOnSubmit(model);
                            dataContext.SubmitChanges();
                            List<YX_ActivityLottery> lolist = dataContext.YX_ActivityLottery.Where(m => m.ActivityID == model.ActivityID).ToList();
                            if (lolist.Count >= 8)
                            {
                                result.Msg = "大转盘活动奖项已达上限，不得超过8个奖项！";
                                result.CmdError = CmdResultError.EXCEPTION;
                            }
                            else
                            {
                                StringBuilder strBuilder = new StringBuilder();
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    #region  赋值
                                    YX_ActivityLotteryExcel excelde = new YX_ActivityLotteryExcel();
                                    excelde.ActivityID = model.ActivityID;
                                    excelde.ActivitylotteryID = model.ActivitylotteryID;
                                    excelde.AddDate = DateTime.Now;
                                    excelde.AddUserID = model.AddUserID;
                                    excelde.AddUserName = model.AddUserName;
                                    excelde.Arr1 = ds.Tables[0].Rows[i][0].ToString().Trim();
                                    excelde.Arr2 = ds.Tables[0].Rows[i][1].ToString().Trim();
                                    #endregion
                                    dataContext.YX_ActivityLotteryExcel.InsertOnSubmit(excelde);
                                    dataContext.SubmitChanges();
                                }
                                model.LotteryCount = dataContext.YX_ActivityLotteryExcel.Where(m => m.ActivitylotteryID == model.ActivitylotteryID).Count();
                                model.LotteryLastCount = model.LotteryCount;
                                dataContext.SubmitChanges();
                                tran.Commit();
                                result.Msg = strBuilder.ToString();
                                result.CmdError = CmdResultError.NONE;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result.Msg = "上传失败，请检查数据格式！";
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取奖项列表
        /// </summary>
        /// <param name="ActiveID">活动ID</param>
        /// <returns></returns>
        public List<YX_ActivityLottery> GetJXList(long ActiveID)
        {
            List<YX_ActivityLottery> result = new List<YX_ActivityLottery>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.YX_ActivityLottery.Where(m => m.ActivityID == ActiveID && m.Status == (int)Common.EnumFile.Status.used);
                    result = data.ToList();
                }
                catch (Exception ex)
                {
                    string errData = "YXLotteryDal.GetJXList";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除奖项
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <returns></returns>
        public RetResult DelLottery(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    YX_ActivityLottery al = dataContext.YX_ActivityLottery.FirstOrDefault(m => m.ActivitylotteryID == id);
                    if (al != null)
                    {
                        al.Status = (int)Common.EnumFile.Status.delete;
                        List<YX_ActivityLotteryExcel> edetails = dataContext.YX_ActivityLotteryExcel.Where(m => m.ActivitylotteryID == id).ToList();
                        if (edetails.Count > 0)
                        {
                            foreach (var item in edetails)
                            {
                                dataContext.YX_ActivityLotteryExcel.DeleteOnSubmit(item);
                                dataContext.SubmitChanges();
                            }
                        }
                    }
                    else
                    {
                        return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "数据不存在！" };
                    }
                    dataContext.SubmitChanges();
                    tran.Commit();//提交事务
                    return new RetResult { CmdError = CmdResultError.NONE, Msg = "删除成功！" };
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "操作异常！" };
                }
            }
        }

        public RetResult CompareJXCount(long eid, string settingID, long ActiveID, long jxSumCount)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    LoginInfo user = SessCokie.Get;
                    RequestCodeSetting smodel = dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == eid && m.ID == Convert.ToInt64(settingID));
                    if (smodel != null)
                    {
                        if (jxSumCount > 0)
                        {
                            if (smodel.Count > jxSumCount)
                            {
                                YX_ActivityLottery lomodel = new YX_ActivityLottery();
                                lomodel.ActivityID = ActiveID;
                                lomodel.AddDate = DateTime.Now;
                                lomodel.AddUserID = user.UserID;
                                lomodel.AddUserName = user.UserName;
                                lomodel.EnterpriseID = smodel.EnterpriseId;
                                lomodel.LotteryCount = smodel.Count - jxSumCount;
                                lomodel.LotteryName = "空白奖";
                                lomodel.Status = (int)Common.EnumFile.Status.used;
                                dataContext.YX_ActivityLottery.InsertOnSubmit(lomodel);
                                dataContext.SubmitChanges();
                                return new RetResult { CmdError = CmdResultError.NONE, Msg = "奖项数量少于该批码的数量会有" + lomodel.LotteryCount + "个空白奖！" };
                            }
                            else if (smodel.Count < jxSumCount)
                            {
                                return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "您设置的奖项超过上限" + smodel.Count + "个，请进行编辑！" };
                            }
                            else
                            {
                                return new RetResult { CmdError = CmdResultError.NONE, Msg = "创建成功！" };
                            }
                        }
                        else
                        {
                            return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "您还没设置奖项，请添加奖项！" };
                        }
                    }
                    else
                    {
                        return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "创建失败,没有找到追溯码相关数据！" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "操作异常！" };
            }
        }

        /// <summary>
        /// 获取奖项实体
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <returns></returns>
        public YX_ActivityLottery GetLotteryModel(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var m = dataContext.YX_ActivityLottery.FirstOrDefault(n => n.ActivitylotteryID == id);
                return m;
            }
        }

        public RetResult EditLottery(YX_ActivityLottery model)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                List<YX_ActivityLotteryExcel> exlist = dataContext.YX_ActivityLotteryExcel.Where(m => m.ActivitylotteryID == model.ActivitylotteryID).ToList();
                if (exlist.Count > 0)
                {
                    foreach (var item in exlist)
                    {
                        dataContext.YX_ActivityLotteryExcel.DeleteOnSubmit(item);
                        dataContext.SubmitChanges();
                    }
                }
            }
            FileStream fs = new FileStream(model.LotteryFilePath, FileMode.Open, FileAccess.Read);
            Workbook book = new Workbook(fs);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;
            DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            RetResult result = EImportExcel(ds, model);
            if (result.IsSuccess)
            {
                result.Msg = "编辑成功";
                result.CmdError = CmdResultError.NONE;
            }
            else
            {
                result.CmdError = CmdResultError.EXCEPTION;
                result.Msg = "导入失败，请检查数据格式！";
            }
            return result;
        }

        public RetResult EImportExcel(System.Data.DataSet ds, YX_ActivityLottery model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "导入信息失败！";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    if (ds != null)
                    {
                        var data = dataContext.YX_ActivityLottery.FirstOrDefault(m => m.ActivityID == model.ActivityID && m.EnterpriseID == model.EnterpriseID &&
                        m.LotteryName == model.LotteryName && m.ActivitylotteryID != model.ActivitylotteryID && m.Status == (int)Common.EnumFile.Status.used);
                        if (data != null)
                        {
                            result.Msg = "该批活动已存在该奖项！";
                            result.CmdError = CmdResultError.EXCEPTION;
                        }
                        else
                        {
                            var oldmodel = dataContext.YX_ActivityLottery.FirstOrDefault(m => m.ActivityID == model.ActivityID && m.ActivitylotteryID == model.ActivitylotteryID &&
                            m.Status == (int)Common.EnumFile.Status.used);
                            if (oldmodel == null)
                            {
                                result.Msg = "没有找到要修改的数据！";
                            }
                            else
                            {
                                oldmodel.LotteryName = model.LotteryName;
                                oldmodel.LotteryFile = model.LotteryFile;
                                oldmodel.LotteryFilePath = model.LotteryFilePath;
                                oldmodel.LotteryFileURL = model.LotteryFileURL;
                                oldmodel.LotteryPic = model.LotteryPic;
                                oldmodel.LotteryPicHS = model.LotteryPicHS;
                                dataContext.SubmitChanges();
                                StringBuilder strBuilder = new StringBuilder();
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    #region  赋值
                                    YX_ActivityLotteryExcel excelde = new YX_ActivityLotteryExcel();
                                    excelde.ActivityID = model.ActivityID;
                                    excelde.ActivitylotteryID = model.ActivitylotteryID;
                                    excelde.AddDate = DateTime.Now;
                                    excelde.AddUserID = model.AddUserID;
                                    excelde.AddUserName = model.AddUserName;
                                    excelde.Arr1 = ds.Tables[0].Rows[i][0].ToString().Trim();
                                    excelde.Arr2 = ds.Tables[0].Rows[i][1].ToString().Trim();
                                    #endregion
                                    dataContext.YX_ActivityLotteryExcel.InsertOnSubmit(excelde);
                                    dataContext.SubmitChanges();
                                }
                                model.LotteryCount = dataContext.YX_ActivityLotteryExcel.Where(m => m.ActivitylotteryID == model.ActivitylotteryID).Count();
                                model.LotteryLastCount = model.LotteryCount;
                                oldmodel.LotteryCount = model.LotteryCount;
                                oldmodel.LotteryLastCount = model.LotteryLastCount;
                                dataContext.SubmitChanges();
                                tran.Commit();
                                result.Msg = strBuilder.ToString();
                                result.CmdError = CmdResultError.NONE;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result.Msg = "上传失败，请检查数据格式！";
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 编辑大转盘活动第一步
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult EditActive(YX_ActivitySub model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "编辑活动失败";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var oldmodel = dataContext.YX_ActivitySub.FirstOrDefault(m => m.ActivityID == model.ActivityID);
                    if (oldmodel == null)
                    {
                        result.Msg = "没有找到要修改的数据！";
                        result.CmdError = CmdResultError.EXCEPTION;
                    }
                    else
                    {
                        oldmodel.ActivityTitle = model.ActivityTitle;
                        oldmodel.StartDate = model.StartDate;
                        oldmodel.EndDate = model.EndDate;
                        oldmodel.Content = model.Content;
                        dataContext.SubmitChanges();
                        result.CmdError = CmdResultError.NONE;
                        result.Msg = "编辑成功";
                    }
                }
                catch
                {
                    result.Msg = "编辑失败";
                    return result;
                }
            }
            return result;
        }

        public RetResult EGetActive(long activeID)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.YX_ActivitySub.FirstOrDefault(m => m.ActivityID == activeID);
                    return new RetResult { CmdError = CmdResultError.NONE, Msg = "操作成功！" };
                }
                catch (Exception ex)
                {
                    string errData = "YXLotteryDal.EGetActive";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "操作异常！" };
                }
            }
        }

        public RetResult ECompareJXCount(long eid, long activeID, long jxSumCount)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    LoginInfo user = SessCokie.Get;
                    YX_ActvitiyRelationCode seid = dataContext.YX_ActvitiyRelationCode.FirstOrDefault(m => m.ActivityID == activeID && m.CompanyID == eid);
                    if (seid != null)
                    {
                        RequestCodeSetting smodel = dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == eid && m.ID == Convert.ToInt64(seid.RequestSettingID));
                        if (smodel != null)
                        {
                            if (jxSumCount > 0)
                            {
                                if (smodel.Count > jxSumCount)
                                {
                                    YX_ActivityLottery lomodel = new YX_ActivityLottery();
                                    lomodel.ActivityID = activeID;
                                    lomodel.AddDate = DateTime.Now;
                                    lomodel.AddUserID = user.UserID;
                                    lomodel.AddUserName = user.UserName;
                                    lomodel.EnterpriseID = eid;
                                    lomodel.LotteryCount = smodel.Count - jxSumCount;
                                    lomodel.LotteryName = "空白奖";
                                    lomodel.Status = (int)Common.EnumFile.Status.used;
                                    dataContext.YX_ActivityLottery.InsertOnSubmit(lomodel);
                                    dataContext.SubmitChanges();
                                    return new RetResult { CmdError = CmdResultError.NONE, Msg = "奖项数量少于该批码的数量会有" + lomodel.LotteryCount + "个空白奖！" };
                                }
                                else if (smodel.Count < jxSumCount)
                                {
                                    return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "您设置的奖项超过上限" + smodel.Count + "个，请进行编辑！" };
                                }
                                else
                                {
                                    return new RetResult { CmdError = CmdResultError.NONE, Msg = "编辑成功！" };
                                }
                            }
                            else
                            {
                                return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "您还没设置奖项，请添加奖项！" };
                            }
                        }
                        else
                        {
                            return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "编辑失败,没有找到追溯码相关数据！" };
                        }
                    }
                    else
                    {
                        return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "编辑失败,没有找到追溯码相关数据！" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "操作异常！" };
            }
        }
    }
}
