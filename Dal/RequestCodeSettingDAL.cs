/********************************************************************************

** 作者： 李子巍

** 创始时间：2017-02-14

** 联系方式 :13313318725

** 描述：主要用于码配置的数据访问层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;
using System.Data.Common;

namespace Dal
{
    /// <summary>
    /// 主要用于码配置的数据访问层
    /// </summary>
    public class RequestCodeSettingDAL : DALBase
    {
        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="model">配置信息实体</param>
        /// <param name="Msg">返回消息</param>
        /// <param name="error">返回消息码</param>
        /// <returns>添加的实体</returns>
        public RequestCodeSetting Add(RequestCodeSetting model, out string Msg, out int error)
        {
            Msg = "配置信息失败！";
            error = -1;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting setting = dataContext.RequestCodeSetting.FirstOrDefault(
                        m => m.BatchName == model.BatchName && m.EnterpriseId == model.EnterpriseId);
                    if (setting == null)
                    {
                        var liSetting = dataContext.RequestCodeSetting.Where(m => m.RequestID == model.RequestID).ToList();
                        long settingCount = liSetting.Sum(m => m.Count);
                        RequestCode requestCode = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == model.RequestID);
                        long totalCount = requestCode.TotalNum.GetValueOrDefault(0);
                        if (requestCode.Status == (int)Common.EnumFile.RequestCodeStatus.Unaudited)
                        {
                            totalCount = requestCode.TotalNum.GetValueOrDefault(0);
                        }
                        if (settingCount + model.Count > totalCount)
                        {
                            Msg = "总配置数量超过申请数量！";
                        }
                        else
                        {
                            model.SetDate = DateTime.Now;
                            model.beginCode = settingCount + 1;
                            model.endCode = settingCount + model.Count;
                            model.BatchType = model.BatchName.IndexOf('m') == 0 ? 1 : 2;
                            dataContext.RequestCodeSetting.InsertOnSubmit(model);
                            dataContext.SubmitChanges();
                            error = 0;
                        }
                    }
                    else
                    {
                        Msg = "批次号重复！";
                    }
                }
            }
            catch { }
            return model;
        }

        /// <summary>
        /// 按顺序拆分批次
        /// </summary>
        /// <param name="model">配置信息实体</param>
        /// <param name="Msg">返回消息</param>
        /// <param name="error">返回消息码</param>
        /// <returns>添加的实体</returns>
        public RequestCodeSetting PartSplit(RequestCodeSetting model, out string Msg, out int error)
        {
            Msg = "批次拆分失败！";
            error = -1;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting setting = dataContext.RequestCodeSetting.FirstOrDefault(
                        m => m.BatchName == model.BatchName && m.EnterpriseId == model.EnterpriseId);
                    if (setting == null)
                    {
                        RequestCodeSetting mainSetting = dataContext.RequestCodeSetting.FirstOrDefault(
                        m => m.RequestID == model.RequestID && m.BatchType == 1 && m.BathPartType!=(int)Common.EnumFile.BatchPartType.Custom);
                        if (mainSetting != null)
                        {
                            RequestCode requestcode = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == model.RequestID);
                            if (requestcode.Type == (int)Common.EnumFile.GenCodeType.single)
                            {
                                mainSetting.Count = mainSetting.Count - model.Count;
                            }
                            else if (requestcode.Type == (int)Common.EnumFile.GenCodeType.trap)
                            {
                                mainSetting.BatchTrap = mainSetting.BatchTrap - Convert.ToInt32(model.Count);
                                mainSetting.Count = mainSetting.Count - model.Count - model.Count * requestcode.Specifications.Value;
                            }
                            else if (requestcode.Type == (int)Common.EnumFile.GenCodeType.pesticides)
                            {
                                mainSetting.Count = mainSetting.Count - model.Count;
                            }
                            var liSetting = dataContext.RequestCodeSetting.Where(m => m.RequestID == model.RequestID && m.BatchType == 2).ToList();
                            long settingCount = liSetting.Sum(m => m.Count);
                            if (mainSetting.Count < 0)
                            {
                                Msg = "总配置数量超过申请数量！";
                            }
                            else
                            {
                                model.BrandID = mainSetting.BrandID;
                                model.DisplayOption = mainSetting.DisplayOption;
                                model.MaterialID = mainSetting.MaterialID;
                                model.RequestID = mainSetting.RequestID;
                                model.StyleModel = mainSetting.StyleModel;
                                model.EnterpriseId = mainSetting.EnterpriseId;
                                model.SetDate = DateTime.Now;
                                model.beginCode = settingCount + requestcode.StartNum;
                                if (requestcode.Type == (int)Common.EnumFile.GenCodeType.single)
                                {
                                    model.endCode = settingCount + model.Count + requestcode.StartNum - 1;
                                }
                                else if (requestcode.Type == (int)Common.EnumFile.GenCodeType.trap)
                                {
                                    model.BatchTrap = Convert.ToInt32(model.Count);
                                    model.endCode = settingCount + model.Count * requestcode.Specifications + model.Count + requestcode.StartNum - 1;
                                    model.Count = model.Count * requestcode.Specifications.Value + model.Count;
                                }
                                else if (requestcode.Type == (int)Common.EnumFile.GenCodeType.pesticides)
                                {
                                    model.endCode = settingCount + model.Count + requestcode.StartNum - 1;
                                }
                                model.BatchType = 2;
                                model.RequestCodeType = requestcode.RequestCodeType;
                                model.BathPartType =(int)Common.EnumFile.BatchPartType.Split;
                                mainSetting.BathPartType = (int)Common.EnumFile.BatchPartType.Split;
                                if (mainSetting.Count != 0)
                                {
                                    mainSetting.beginCode = model.endCode + 1;
                                    mainSetting.beginNum = (model.endCode + 1).ToString();
                                }
                                model.beginNum = model.endCode.ToString();
                                model.endNum = model.endCode.ToString();
                                model.ShengChanPH = mainSetting.ShengChanPH;
                                model.YouXiaoDate = mainSetting.YouXiaoDate;
                                model.ShiXiaoDate = mainSetting.ShiXiaoDate;
                                model.BZSpecType = mainSetting.BZSpecType;
                                dataContext.RequestCodeSetting.InsertOnSubmit(model);
                                dataContext.SubmitChanges();
                                error = 0;
                                Msg = "批次拆分成功！";
                            }
                        }
                        else
                        {
                            Msg = "没有找到主批次！";
                        }
                    }
                    else
                    {
                        Msg = "批次号重复！";
                    }
                }
            }
            catch { }
            return model;
        }

        /// <summary>
        /// 自定义拆分批次
        /// </summary>
        /// <param name="model">配置信息实体</param>
        /// <param name="Msg">返回消息</param>
        /// <param name="error">返回消息码</param>
        /// <returns>添加的实体</returns>
        public RequestCodeSetting PartCustom(RequestCodeSetting model, out string Msg, out int error)
        {
            Msg = "批次拆分失败！";
            error = -1;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                if (dataContext.Connection != null)
                    dataContext.Connection.Open();
                DbTransaction tran = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = tran;
                try
                {
                    RequestCodeSetting setting = dataContext.RequestCodeSetting.FirstOrDefault(
                        m => m.BatchName == model.BatchName && m.EnterpriseId == model.EnterpriseId);
                    if (setting != null)
                    {
                        Msg = "批次号重复！";
                        return model;
                    }
                    RequestCodeSetting mainSetting = dataContext.RequestCodeSetting.FirstOrDefault(
                    m => m.RequestID == model.RequestID && m.BatchType == 1 &&( m.BathPartType==null || m.BathPartType != (int)Common.EnumFile.BatchPartType.Split));
                    if (mainSetting == null)
                    {
                        Msg = "没有找到主批次！";
                        return model;
                    }
                    RequestCode request = dataContext.RequestCode.Where(p => p.RequestCode_ID == mainSetting.RequestID).FirstOrDefault();
                    if (request == null)
                    {
                        Msg = "没有找到生成码批次";
                        return model;
                    }
                    //查找码库
                    string tableName;
                    using (DataClassesDataContext dataContextDynamic = GetDynamicDataContext((long)request.Route_DataBase_ID, out tableName))
                    {
                        //查找目前两个码段之间未拆分的码数量
                        string sql = string.Format("select COUNT(*) from {0} " +
                        "where Enterprise_FWCode_ID>=(select Enterprise_FWCode_ID from  dbo.{0} where RequestCode_ID={3} " +
                        "and EWM ='{1}')" +
                        "and Enterprise_FWCode_ID<=(select Enterprise_FWCode_ID from  dbo.{0} where RequestCode_ID={3} " +
                        "and EWM ='{2}') and  RequestSetID is null", tableName, model.beginNum, model.endNum, model.RequestID);
                        int realCodeCount = dataContextDynamic.ExecuteQuery<Int32>(sql).FirstOrDefault();
                        if (realCodeCount == 0)
                        {
                            Msg = "请确认起始码输入是否正确！";
                            return model;
                        }
                        //该批码首码ID
                        sql = string.Format("select top 1 Enterprise_FWCode_ID from {0} where RequestCode_ID={1} order by Enterprise_FWCode_ID",
                            tableName, model.RequestID);
                        long startId = dataContextDynamic.ExecuteQuery<long>(sql).FirstOrDefault();
                        //起始码的ID
                        sql = string.Format("select Enterprise_FWCode_ID from {0} " +
                        "where RequestCode_ID={2} and EWM ='{1}' ", tableName, model.beginNum, model.RequestID);
                        long beginId = dataContextDynamic.ExecuteQuery<long>(sql).FirstOrDefault();
                        //结束码的ID
                        sql = string.Format("select Enterprise_FWCode_ID from {0} " +
                        "where RequestCode_ID={2} and EWM ='{1}' ", tableName, model.endNum, model.RequestID);
                        long endId = dataContextDynamic.ExecuteQuery<long>(sql).FirstOrDefault();
                        if ((endId - beginId + 1) != realCodeCount)
                        {
                            Msg = "该起始码之间的码已经被拆分！";
                            return model;
                        }
                        model.Count = endId - beginId + 1;
                        RequestCode requestcode = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == model.RequestID);
                        if (requestcode.Type == (int)Common.EnumFile.GenCodeType.single)
                        {
                            mainSetting.Count = mainSetting.Count - model.Count;
                        }
                        else
                        {
                            Msg = "该批码不可以自定义拆分！";
                            return model;
                        }
                        var liSetting = dataContext.RequestCodeSetting.Where(m => m.RequestID == model.RequestID && m.BatchType == 2).ToList();
                        long settingCount = liSetting.Sum(m => m.Count);
                        if (mainSetting.Count < 0)
                        {
                            Msg = "总配置数量超过申请数量！";
                        }
                        else
                        {
                            model.beginCode = beginId - startId + 1;
                            model.endCode = endId - startId + 1;
                            model.BrandID = mainSetting.BrandID;
                            model.DisplayOption = mainSetting.DisplayOption;
                            model.MaterialID = mainSetting.MaterialID;
                            model.RequestID = mainSetting.RequestID;
                            model.StyleModel = mainSetting.StyleModel;
                            model.EnterpriseId = mainSetting.EnterpriseId;
                            model.SetDate = DateTime.Now;
                            model.BatchType = 2;
                            model.RequestCodeType = requestcode.RequestCodeType;
                            model.BathPartType = (int)Common.EnumFile.BatchPartType.Custom;
                            mainSetting.BathPartType = (int)Common.EnumFile.BatchPartType.Custom;
                            dataContext.RequestCodeSetting.InsertOnSubmit(model);
                            dataContext.SubmitChanges();
                            //更改码表中码的RequestSetID
                            sql = string.Format("update {0} set RequestSetID={4} where EWM>='{1}' and EWM <='{2}' and RequestCode_ID={3}",
                                tableName, model.beginNum, model.endNum, model.RequestID, model.ID);
                            int exeCount = dataContextDynamic.ExecuteQuery<Int32>(sql).FirstOrDefault();
                            error = 0;
                            Msg = "批次拆分成功！";
                            tran.Commit();
                        }
                    }
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return model;
        }

        #region 获取动态链接
        private DataClassesDataContext GetDynamicDataContext(long routeDataBaseId, out string tablename)
        {
            tablename = "";
            DataClassesDataContext result = null;
            try
            {
                string datasource;
                string database;
                string username;
                string pass;
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    long tableId = routeDataBaseId;
                    Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == tableId);
                    if (table == null)
                    {
                        return null;
                    }
                    datasource = table.DataSource;
                    database = table.DataBaseName;
                    username = table.UID;
                    pass = table.PWD;
                    tablename = table.TableName;

                }
                result = GetDataContext(datasource, database, username, pass);
            }
            catch (Exception ex)
            {
                string errData = "ScanCodeDAL.GetDynamicDataContext()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取配置页第一页的初始信息
        /// </summary>
        /// <param name="requestId">申请码标识</param>
        /// <returns>初始信息</returns>
        public FirstData GetFirstPageData(long requestId)
        {
            FirstData result = new FirstData();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCode requestCode = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == requestId);
                    if (requestCode != null)
                    {
                        long settingCount = 0;
                        result.liData = dataContext.View_RequestCodeSetting.Where(m => m.RequestID == requestId).ToList();
                        if (result.liData != null && result.liData.Count > 0)
                        {
                            settingCount = result.liData.Sum(m => m.Count);
                        }
                        long totalCount = requestCode.TotalNum.GetValueOrDefault(0);
                        result.materialId = requestCode.Material_ID.GetValueOrDefault(0);
                        result.brandId = 0;
                        try
                        {
                            var brand = dataContext.Material.FirstOrDefault(m => m.Material_ID == result.materialId);
                            result.brandId = brand.Brand_ID.Value;
                        }
                        catch { }
                        result.remaining = totalCount - settingCount;
                        result.isFirst = settingCount == 0;
                        result.notFirst = !result.isFirst;
                        result.isSuccess = "0";
                        result.Msg = "";
                    }
                    else
                    {
                        result.Msg = "没有找到申请码记录！";
                        result.isSuccess = "-1";
                    }
                }
            }
            catch
            {
                result.Msg = "程序遇到问题！";
                result.isSuccess = "-1";
            }
            return result;
        }

        /// <summary>
        /// 获取子码段配置信息
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段配置信息</returns>
        public RequestCodeSetting GetSubSetting(long subId)
        {
            RequestCodeSetting result = new RequestCodeSetting();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == subId);
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 修改配置信息的拍码显示项
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <param name="Msg">返回消息</param>
        /// <param name="error">返回消息码</param>
        /// <returns>操作结果</returns>
        public RequestCodeSetting Edit(RequestCodeSetting model, out string Msg, out int error)
        {
            Msg = "配置信息失败！";
            error = -1;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting setting = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == model.ID);
                    if (setting != null)
                    {
                        if (model.DisplayOption != null)
                        {
                            setting.DisplayOption = model.DisplayOption;
                        }
                        if (model.StyleModel != null)
                        {
                            setting.StyleModel = model.StyleModel;
                        }
                        dataContext.SubmitChanges();
                        Msg = "配置成功！";
                        error = 0;
                    }
                    else
                    {
                        Msg = "没有找到配置的数据！";
                    }
                }
            }
            catch { }
            return model;
        }

        /// <summary>
        /// 修改子码段产品信息
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <param name="Msg">返回消息</param>
        /// <param name="error">返回消息码</param>
        /// <returns>操作结果</returns>
        public RequestCodeSetting EditMaterial(RequestCodeSetting model, out string Msg, out int error)
        {
            Msg = "配置失败！";
            error = -1;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting setting = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == model.ID);
                    if (setting != null)
                    {
                        setting.MaterialID = model.MaterialID;
                        if (model.BrandID > 0)
                            setting.BrandID = model.BrandID;
                        if (model.ProductionDate != null)
                        {
                            setting.ProductionDate = model.ProductionDate;
                        }
                        dataContext.SubmitChanges();
                        Msg = "保存成功！";
                        error = 0;
                    }
                    else
                    {
                        Msg = "没有找到配置的数据！";
                    }
                }
            }
            catch { }
            return model;
        }

        /// <summary>
        /// 获取子码段原料列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>操作结果</returns>
        public List<View_RequestOrigin> GetOriginList(long subId)
        {
            List<View_RequestOrigin> result = new List<View_RequestOrigin>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.View_RequestOrigin.Where(m => m.SettingID == subId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    foreach (var item in result)
                    {
                        item.StrInDate = item.InDate.GetValueOrDefault(DateTime.Now).ToString("yyyy年MM月dd日");
                        item.InDate = null;
                        item.StrAddDate = item.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                        item.AddDate = null;
                        #region 图片XML转JSON类
                        List<ToJsonImg> liImg = new List<ToJsonImg>();
                        if (item.Img != null)
                        {
                            IEnumerable<XElement> allImg = item.Img.Elements("img");
                            foreach (var sub in allImg)
                            {
                                ToJsonImg img = new ToJsonImg();
                                img.fileUrl = sub.Attribute("value").Value;
                                img.fileUrls = sub.Attribute("small").Value;
                                liImg.Add(img);
                            }
                        }
                        item.imgs = liImg;
                        item.Img = null;
                        //原料检测报告图片
                        List<ToJsonJCImg> jcliImg = new List<ToJsonJCImg>();
                        if (item.JCImgInfo != null)
                        {
                            IEnumerable<XElement> allImg = item.JCImgInfo.Elements("img");
                            foreach (var sub in allImg)
                            {
                                ToJsonJCImg img = new ToJsonJCImg();
                                img.jcfileUrl = sub.Attribute("value").Value;
                                img.jcfileUrls = sub.Attribute("small").Value;
                                jcliImg.Add(img);
                            }
                        }
                        item.jcimgs = jcliImg;
                        item.JCImgInfo = null;
                        #endregion
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 给子码段添加新原料
        /// </summary>
        /// <param name="model">添加的实体</param>
        /// <returns>操作结果</returns>
        public RetResult AddOrigin(RequestOrigin model)
        {
            string msg = "添加原材料失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestOrigin origin = dataContext.RequestOrigin.FirstOrDefault(m => m.ID == model.ID && m.SettingID == model.SettingID);
                    if (origin != null)
                    {
                        msg = "已存在该原材料！";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.Driver) && dataContext.Dictorynary_Key.Where(a => a.PageJs == "origin" && a.Flag == 1 && a.Value == model.Driver && a.EnterpriseInfoId == model.EnterpriseID).Count() == 0)
                        {
                            dataContext.Dictorynary_Key.InsertOnSubmit(new Dictorynary_Key { Value = model.Driver, Flag = 1, PageJs = "origin", EnterpriseInfoId = model.EnterpriseID });
                        }
                        if (!string.IsNullOrEmpty(model.CarNum) && dataContext.Dictorynary_Key.Where(a => a.PageJs == "origin" && a.Flag == 2 && a.Value == model.CarNum && a.EnterpriseInfoId == model.EnterpriseID).Count() == 0)
                        {
                            dataContext.Dictorynary_Key.InsertOnSubmit(new Dictorynary_Key { Value = model.CarNum, Flag = 2, PageJs = "origin", EnterpriseInfoId = model.EnterpriseID });
                        }
                        dataContext.RequestOrigin.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        msg = "添加原材料成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 修改子码段的原料信息
        /// </summary>
        /// <param name="model">原料信息</param>
        /// <returns>操作结果</returns>
        public RetResult EditOrigin(RequestOrigin model)
        {
            string msg = "修改原材料失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestOrigin origin = dataContext.RequestOrigin.FirstOrDefault(m => m.ID == model.ID && m.SettingID == model.SettingID && m.ID != model.ID);
                    if (origin != null)
                    {
                        msg = "已存在该原材料！";
                    }
                    else
                    {
                        origin = dataContext.RequestOrigin.FirstOrDefault(m => m.ID == model.ID);
                        if (origin != null)
                        {
                            origin.Driver = model.Driver;
                            origin.CarNum = model.CarNum;
                            origin.CheckUser = model.CheckUser;
                            origin.Img = model.Img;
                            origin.JCImgInfo = model.JCImgInfo;
                            origin.InDate = model.InDate;
                            origin.OriginID = model.OriginID;
                            origin.Supplier = model.Supplier;
                            origin.Level = model.Level;
                            origin.Factory = model.Factory;
                            origin.BatchNum = model.BatchNum;
                            origin.EarTag = model.EarTag;
                            origin.TagContent = model.TagContent;
                            if (!string.IsNullOrEmpty(model.Driver) && dataContext.Dictorynary_Key.Where(a => a.PageJs == "origin" && a.Flag == 1 && a.Value == model.Driver && a.EnterpriseInfoId == model.EnterpriseID).Count() == 0)
                            {
                                dataContext.Dictorynary_Key.InsertOnSubmit(new Dictorynary_Key { Value = model.Driver, Flag = 1, PageJs = "origin", EnterpriseInfoId = model.EnterpriseID });
                            }
                            if (!string.IsNullOrEmpty(model.CarNum) && dataContext.Dictorynary_Key.Where(a => a.PageJs == "origin" && a.Flag == 2 && a.Value == model.CarNum && a.EnterpriseInfoId == model.EnterpriseID).Count() == 0)
                            {
                                dataContext.Dictorynary_Key.InsertOnSubmit(new Dictorynary_Key { Value = model.CarNum, Flag = 2, PageJs = "origin", EnterpriseInfoId = model.EnterpriseID });
                            }
                            dataContext.SubmitChanges();
                            msg = "修改原材料成功！";
                            error = CmdResultError.NONE;
                        }
                        else
                        {
                            msg = "没有找到要修改的原材料！";
                        }
                    }
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 删除子码段的原料信息
        /// </summary>
        /// <param name="originSettingId">要删除ID</param>
        /// <returns>操作结果</returns>
        public RetResult DelOrigin(long originSettingId)
        {
            string msg = "删除原材料失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestOrigin origin = dataContext.RequestOrigin.FirstOrDefault(m => m.ID == originSettingId);
                    if (origin != null)
                    {
                        origin.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        msg = "删除原材料成功！";
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        msg = "没有找到要删除的原材料！";
                    }
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 获取子码段作业信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段作业信息列表</returns>
        public List<Batch_ZuoYe> GetWorkList(long subId)
        {
            List<Batch_ZuoYe> result = new List<Batch_ZuoYe>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.Batch_ZuoYe.Where(m => m.SettingID == subId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            item.zuoye_typeId = item.Batch_ZuoYeType_ID;
                            item.StrAddDate = item.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy年MM月dd日");
                            item.Strlastdate = item.lastdate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                            try
                            {
                                item.OperationTypeName = dataContext.View_ZuoYeAndZuoYeType.FirstOrDefault(m => m.Batch_ZuoYeType_ID == item.Batch_ZuoYeType_ID).OperationTypeName;
                            }
                            catch { item.OperationTypeName = ""; }

                            item.TypeName = Common.EnumText.EnumToText(typeof(Common.EnumFile.ZuoYeType), item.type.GetValueOrDefault(0));

                            #region 图片XML转JSON类
                            List<ToJsonImg> liImg = new List<ToJsonImg>();
                            List<ToJsonImg> liVid = new List<ToJsonImg>();
                            if (item.Files != null)
                            {
                                IEnumerable<XElement> allImg = item.Files.Elements("img");
                                foreach (var sub in allImg)
                                {
                                    ToJsonImg img = new ToJsonImg();
                                    img.fileUrl = sub.Attribute("value").Value;
                                    img.fileUrls = sub.Attribute("small").Value;
                                    liImg.Add(img);
                                }
                                IEnumerable<XElement> allVideo = item.Files.Elements("video");
                                foreach (var sub in allVideo)
                                {
                                    ToJsonImg img = new ToJsonImg();
                                    img.videoUrl = sub.Attribute("value").Value;
                                    img.videoUrls = sub.Attribute("small").Value;
                                    liVid.Add(img);
                                }
                            }
                            item.imgs = liImg;
                            item.videos = liVid;
                            //if (string.IsNullOrEmpty(item.UsersName)) continue;
                            //List<ToJsonProperty> userInfo = new List<ToJsonProperty>();
                            //foreach (var itemUser in item.UsersName.Split(','))
                            //{
                            //    ToJsonProperty user = new ToJsonProperty();
                            //    TeamUsers teamUsers = dataContext.TeamUsers.FirstOrDefault(
                            //        m => m.TeamUsersID == Convert.ToInt64(itemUser)
                            //        && m.Status == (int)Common.EnumFile.Status.used);
                            //    if (teamUsers != null)
                            //    {
                            //        user.pName = teamUsers.UserName;
                            //        user.pValue = teamUsers.TeamUsersID.ToString();
                            //        userInfo.Add(user);
                            //    }
                            //}
                            //item.users = userInfo;
                            #endregion
                            item.Files = null;
                            //item.UsersName = null;
                            //item.Enterprise_Info = null;
                            //item.Batch = null;
                            //item.Batch_ZuoYeType = null;
                            item.AddDate = null;
                            item.lastdate = null;
                        }
                    }
                    else
                    {
                        List<Batch_ZuoYe> lizuoye = new List<Batch_ZuoYe>();
                        RequestCodeSetting setModel = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == subId);
                        if (setModel != null)
                        {
                            Material maModel = dataContext.Material.FirstOrDefault(m => m.Material_ID == setModel.MaterialID && m.Status ==
                                (int)Common.EnumFile.Status.used);
                            if (maModel != null && maModel.ProcessID > 0)
                            {
                                XElement opList = null;
                                Process proModel = dataContext.Process.FirstOrDefault(m => m.ProcessID == maModel.ProcessID && m.status ==
                                (int)Common.EnumFile.Status.used);
                                if (proModel != null)
                                {
                                    opList = proModel.OperationList;
                                }
                                ClearLinqModel(proModel);
                                #region 生产环节XML转JSON类
                                List<ToJsonOperation> operations = new List<ToJsonOperation>();
                                if (proModel != null && !string.IsNullOrEmpty(proModel.StrOperationList))
                                {
                                    XElement xml = XElement.Parse(proModel.StrOperationList);
                                    IEnumerable<XElement> allOperation = xml.Elements("info");
                                    foreach (var item in allOperation)
                                    {
                                        Batch_ZuoYe zuoyeModel = new Batch_ZuoYe();
                                        ToJsonOperation sub = new ToJsonOperation();
                                        sub.opName = item.Attribute("iname").Value;
                                        sub.opID = item.Attribute("ivalue").Value;
                                        operations.Add(sub);
                                        zuoyeModel.OperationTypeName = item.Attribute("iname").Value;
                                        Batch_ZuoYeType zuoyeType = dataContext.Batch_ZuoYeType.FirstOrDefault(m => m.Enterprise_Info_ID == proModel.EnterpriseID &&
                                            m.OperationTypeName == zuoyeModel.OperationTypeName && m.state == (int)Common.EnumFile.Status.used);
                                        if (zuoyeType.type == 0)
                                        {
                                            zuoyeModel.TypeName = "种植";
                                        }
                                        else if (zuoyeType.type == 1)
                                        {
                                            zuoyeModel.TypeName = "加工";
                                        }
                                        else if (zuoyeType.type == 2)
                                        {
                                            zuoyeModel.TypeName = "养殖";
                                        }
                                        zuoyeModel.type = zuoyeType.type;
                                        zuoyeModel.AddDate = DateTime.Now;
                                        zuoyeModel.adduser = zuoyeType.adduser;
                                        zuoyeModel.Batch_ZuoYeType_ID = Convert.ToInt64(sub.opID);
                                        zuoyeModel.Status = (int)Common.EnumFile.Status.used;
                                        zuoyeModel.Enterprise_Info_ID = zuoyeType.Enterprise_Info_ID;
                                        zuoyeModel.lastdate = DateTime.Now;
                                        zuoyeModel.lastuser = zuoyeType.lastuser;
                                        zuoyeModel.SettingID = subId;
                                        zuoyeModel.StrAddDate = zuoyeModel.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy年MM月dd日");
                                        dataContext.Batch_ZuoYe.InsertOnSubmit(zuoyeModel);
                                        dataContext.SubmitChanges();
                                        lizuoye.Add(zuoyeModel);
                                    }
                                    if (proModel != null)
                                    {
                                        proModel.OperationList = opList;
                                        dataContext.SubmitChanges();
                                    }
                                }
                                proModel.operations = operations;
                                result = lizuoye;
                                #endregion
                            }
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 获取子码段巡检信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段巡检信息列表</returns>
        public List<Batch_XunJian> GetCheckList(long subId)
        {
            List<Batch_XunJian> result = new List<Batch_XunJian>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.Batch_XunJian.Where(m => m.SettingID == subId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    foreach (var item in result)
                    {
                        item.StrAddDate = item.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy年MM月dd日");
                        item.Strlastdate = item.lastdate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");

                        #region 图片XML转JSON类
                        List<ToJsonImg> liImg = new List<ToJsonImg>();
                        List<ToJsonImg> liVid = new List<ToJsonImg>();
                        if (item.Files != null)
                        {
                            IEnumerable<XElement> allImg = item.Files.Elements("img");
                            foreach (var sub in allImg)
                            {
                                ToJsonImg img = new ToJsonImg();
                                img.fileUrl = sub.Attribute("value").Value;
                                img.fileUrls = sub.Attribute("small").Value;
                                liImg.Add(img);
                            }
                            IEnumerable<XElement> allVideo = item.Files.Elements("video");
                            foreach (var sub in allVideo)
                            {
                                ToJsonImg img = new ToJsonImg();
                                img.videoUrl = sub.Attribute("value").Value;
                                img.videoUrls = sub.Attribute("small").Value;
                                liVid.Add(img);
                            }
                        }
                        item.imgs = liImg;
                        item.videos = liVid;
                        #endregion
                        item.Files = null;
                        item.Batch = null;
                        item.AddDate = null;
                        item.lastdate = null;
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 获取子码段质检信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段质检信息列表</returns>
        public List<Batch_JianYanJianYi> GetReportList(long subId)
        {
            List<Batch_JianYanJianYi> result = new List<Batch_JianYanJianYi>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.Batch_JianYanJianYi.Where(m => m.SettingID == subId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    foreach (var item in result)
                    {
                        item.StrAddDate = item.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy年MM月dd日");
                        item.Strlastdate = item.lastdate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");

                        #region 图片XML转JSON类
                        List<ToJsonImg> liImg = new List<ToJsonImg>();
                        if (item.Files != null)
                        {
                            IEnumerable<XElement> allImg = item.Files.Elements("img");
                            foreach (var sub in allImg)
                            {
                                ToJsonImg img = new ToJsonImg();
                                img.fileUrl = sub.Attribute("value").Value;
                                img.fileUrls = sub.Attribute("small").Value;
                                liImg.Add(img);
                            }
                        }
                        item.imgs = liImg;
                        #endregion
                        item.Files = null;
                        item.Batch = null;
                        item.AddDate = null;
                        item.lastdate = null;
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 拆分初始信息
        /// </summary>
        /// <param name="subId">配置表标识</param>
        /// <returns>操作结果</returns>
        public FirstData BatchPartInit(long subId)
        {
            FirstData result = new FirstData();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting requestCode = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == subId);
                    if (requestCode != null)
                    {
                        result.materialId = requestCode.MaterialID.GetValueOrDefault(0);
                        result.materialName = dataContext.Material.FirstOrDefault(m => m.Material_ID == requestCode.MaterialID).MaterialFullName;
                        //如果是套标
                        if (requestCode.BatchTrap != null && !string.IsNullOrEmpty(requestCode.BatchTrap.ToString()))
                        {
                            result.remaining = requestCode.BatchTrap.Value;
                            result.type = (int)Common.EnumFile.GenCodeType.trap;
                        }
                        else
                        {
                            result.remaining = requestCode.Count;
                            result.type = (int)Common.EnumFile.GenCodeType.single;
                        }
                        result.isFirst = false;
                        result.notFirst = true;
                        result.isSuccess = "0";
                        result.Msg = "拆分成功！";
                        int index = dataContext.RequestCodeSetting.Where(m => m.EnterpriseId == requestCode.EnterpriseId).Count() + 1;
                        string batchName = DateTime.Now.ToString("yyyy") + index;
                        while (dataContext.RequestCodeSetting.FirstOrDefault(m => m.EnterpriseId == requestCode.EnterpriseId && m.BatchName == batchName) != null)
                        {
                            batchName = DateTime.Now.ToString("yyyy") + (index++);
                        }
                        result.requestCodeType = requestCode.RequestCodeType.Value;
                        result.requestId = requestCode.RequestID;
                        result.batchName = batchName;
                        result.brandId = requestCode.BrandID.GetValueOrDefault(0);
                        result.createDate = DateTime.Now.ToString("yyyy-MM-dd");
                        result.zbatchName = requestCode.BatchName;
                        result.batchPartType = requestCode.BathPartType == null ? 0 : (int)requestCode.BathPartType;
                    }
                    else
                    {
                        result.Msg = "没有找到申请码记录！";
                        result.isSuccess = "-1";
                    }
                }
            }
            catch
            {
                result.Msg = "程序遇到问题！";
                result.isSuccess = "-1";
            }
            return result;
        }

        /// <summary>
        /// 获取存储环境
        /// </summary>
        /// <param name="generateCodeId">申请码标识</param>
        /// <returns>存储环境实体</returns>
        public SetAmbient GetAmbient(long subId)
        {
            SetAmbient result = new SetAmbient();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    result = dataContext.SetAmbient.FirstOrDefault(m =>
                        m.SettingID == subId) ?? new SetAmbient();
                    ClearLinqModel(result);
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 设置存储环境
        /// </summary>
        /// <param name="model">存储环境实体</param>
        /// <returns>操作结果</returns>
        public RetResult AddAmbient(SetAmbient model)
        {
            string Msg = "设置失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting generateCode = dataContext.RequestCodeSetting.FirstOrDefault(g => g.ID == model.SettingID);
                    if (generateCode == null)
                    {
                        Msg = "生成码记录不存在！";
                    }
                    else
                    {
                        if (dataContext.SetAmbient.Where(m => m.SettingID == model.SettingID) != null)
                        {
                            dataContext.SetAmbient.DeleteAllOnSubmit(
                                dataContext.SetAmbient.Where(m => m.SettingID == model.SettingID)
                            );
                        }
                        dataContext.SetAmbient.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Msg = "设置成功";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
                Ret.Msg = "链接数据库失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取物流信息
        /// </summary>
        /// <param name="generateCodeId">申请码标识</param>
        /// <returns>物流信息实体</returns>
        public SetLogistics GetLogistics(long subId)
        {
            SetLogistics result = new SetLogistics();
            try
            {
                using (var dataContext = GetDataContext())
                {
                    result = dataContext.SetLogistics.FirstOrDefault(m =>
                        m.SetingID == subId) ?? new SetLogistics();
                    ClearLinqModel(result);
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 设置物流信息
        /// </summary>
        /// <param name="model">物流信息实体</param>
        /// <returns>操作结果</returns>
        public RetResult AddLogistics(SetLogistics model)
        {
            string Msg = "设置失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting generateCode = dataContext.RequestCodeSetting.FirstOrDefault(g => g.ID == model.SetingID);
                    if (generateCode == null)
                    {
                        Msg = "生成码记录不存在！";
                    }
                    else
                    {
                        if (dataContext.SetLogistics.Where(m => m.SetingID == model.SetingID) != null)
                        {
                            dataContext.SetLogistics.DeleteAllOnSubmit(
                                dataContext.SetLogistics.Where(m => m.SetingID == model.SetingID)
                            );
                        }
                        dataContext.SetLogistics.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Msg = "设置成功";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
                Ret.Msg = "链接数据库失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取生产日期
        /// </summary>
        /// <param name="subId">标记ID</param>
        /// <returns></returns>
        public RequestCodeSetting GetProductionDate(long subId)
        {
            RequestCodeSetting result = new RequestCodeSetting();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == subId);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeSettingDAL.GetProductionDate()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 是否存在相应产品信息【巡检、存储环境等】flag=1跳转2添加0异常
        /// </summary>
        /// <param name="settingId">产品id</param>
        /// <returns></returns>
        public int IsExistSettingInfo(long settingId)
        {
            int flag = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    long materialId = dataContext.RequestCodeSetting.Where(a => a.ID == settingId).FirstOrDefault().MaterialID.Value;
                    var requestOrgin = dataContext.RequestOrigin.Where(a => a.SettingID == settingId).OrderByDescending(a => a.ID).FirstOrDefault();
                    var batchZuoye = dataContext.Batch_ZuoYe.Where(a => a.SettingID == settingId).OrderByDescending(a => a.Batch_ZuoYe_ID).FirstOrDefault();
                    var batchXunjian = dataContext.Batch_XunJian.Where(a => a.SettingID == settingId).OrderByDescending(a => a.Batch_XunJian_ID).FirstOrDefault();
                    var setAmbient = dataContext.SetAmbient.Where(a => a.SettingID == settingId).OrderByDescending(a => a.AmbientID).FirstOrDefault();
                    var batchJianYanJianYi = dataContext.Batch_JianYanJianYi.Where(a => a.SettingID == settingId).OrderByDescending(a => a.Batch_JianYanJianYi_ID).FirstOrDefault();
                    var setLogistics = dataContext.SetLogistics.Where(a => a.SetingID == settingId).OrderByDescending(a => a.SetLogistics1).FirstOrDefault();
                    if (requestOrgin != null || batchZuoye != null || batchXunjian != null || setAmbient != null || batchJianYanJianYi != null || setLogistics != null)
                    {
                        flag = 1;
                    }
                    else
                    {
                        var requestLst = dataContext.RequestCodeSetting.Where(a => a.MaterialID == materialId);
                        int count = dataContext.RequestOrigin.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => new { }).Count() + dataContext.Batch_ZuoYe.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => new { }).Count() + dataContext.Batch_XunJian.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => new { }).Count() + dataContext.SetAmbient.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => new { }).Count() + dataContext.Batch_JianYanJianYi.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => new { }).Count() + dataContext.SetLogistics.Join(requestLst, a => a.SetingID, b => b.ID, (a, b) => new { }).Count();
                        if (count > 0)
                        {
                            flag = 2;
                        }
                        else
                        {
                            flag = 1;
                        }
                    }
                }
            }
            catch
            {

            }
            return flag;
        }

        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="settingId">配置信息id</param>
        /// <returns></returns>
        public RetResult AddSettingInfo(long settingId)
        {
            RetResult result = new RetResult { CmdError = CmdResultError.NONE, Msg = "添加成功！" };
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    long materialId = dataContext.RequestCodeSetting.Where(a => a.ID == settingId).FirstOrDefault().MaterialID.Value;
                    RequestCodeSetting requestLst = (from data in dataContext.RequestCodeSetting
                                                     where data.MaterialID == materialId && data.ID != settingId && data.ID<settingId
                                                     select data).OrderByDescending(a => a.ID).FirstOrDefault();
                    //var requestOrgin = dataContext.RequestOrigin.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => a).OrderByDescending(a => a.ID).FirstOrDefault();
                    //var batchZuoye = dataContext.Batch_ZuoYe.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => a).Where(a => a.Status != -1).OrderByDescending(a => a.Batch_ZuoYe_ID);
                    //var batchXunjian = dataContext.Batch_XunJian.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => a).OrderByDescending(a => a.Batch_XunJian_ID).FirstOrDefault();
                    //var setAmbient = dataContext.SetAmbient.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => a).OrderByDescending(a => a.AmbientID).FirstOrDefault();
                    //var batchJianYanJianYi = dataContext.Batch_JianYanJianYi.Join(requestLst, a => a.SettingID, b => b.ID, (a, b) => a).OrderByDescending(a => a.Batch_JianYanJianYi_ID).FirstOrDefault();
                    //var setLogistics = dataContext.SetLogistics.Join(requestLst, a => a.SetingID, b => b.ID, (a, b) => a).OrderByDescending(a => a.SetLogistics1).FirstOrDefault();
                    var requestOrgin = dataContext.RequestOrigin.Where(m => m.SettingID == requestLst.ID && m.Status == (int)Common.EnumFile.Status.used);
                    var batchZuoye = dataContext.Batch_ZuoYe.Where(m => m.SettingID == requestLst.ID && m.Status == (int)Common.EnumFile.Status.used);
                    var batchXunjian = dataContext.Batch_XunJian.Where(m => m.SettingID == requestLst.ID && m.Status == (int)Common.EnumFile.Status.used);
                    var batchJianYanJianYi = dataContext.Batch_JianYanJianYi.Where(m => m.SettingID == requestLst.ID && m.Status == (int)Common.EnumFile.Status.used);
                    var setAmbient = dataContext.SetAmbient.Where(m => m.SettingID == requestLst.ID && m.Status == (int)Common.EnumFile.Status.used);
                    var setLogistics = dataContext.SetLogistics.Where(m => m.SetingID == requestLst.ID && m.Status == (int)Common.EnumFile.Status.used);
                    if (requestOrgin.Count() > 0)
                    {
                        List<RequestOrigin> newModels = new List<RequestOrigin>();
                        foreach (var item in requestOrgin)
                        {
                            RequestOrigin newModel = new RequestOrigin();
                            newModel.Driver = item.Driver;
                            newModel.Factory = item.Factory;
                            newModel.Img = item.Img;
                            newModel.CarNum = item.CarNum;
                            newModel.BatchNum = item.BatchNum;
                            newModel.EnterpriseID = item.EnterpriseID;
                            newModel.CheckUser = item.CheckUser;
                            newModel.EarTag = item.EarTag;
                            newModel.JCImgInfo = item.JCImgInfo;
                            newModel.InDate = item.InDate;
                            newModel.OriginID = item.OriginID;
                            newModel.Level = item.Level;
                            newModel.AddDate = DateTime.Now;
                            newModel.SettingID = settingId;
                            newModel.Status = item.Status;
                            newModel.TagContent = item.TagContent;
                            newModel.Supplier = item.Supplier;
                            newModel.Type = item.Type;
                            newModels.Add(newModel);
                        }
                        dataContext.RequestOrigin.InsertAllOnSubmit(newModels);
                    }
                    if (batchZuoye.Count() > 0)
                    {
                        List<Batch_ZuoYe> newModels = new List<Batch_ZuoYe>();
                        foreach (var item in batchZuoye)
                        {
                            Batch_ZuoYe newModel = new Batch_ZuoYe();
                            newModel.imgs = item.imgs;
                            newModel.Files = item.Files;
                            newModel.Enterprise_Info_ID = item.Enterprise_Info_ID;
                            newModel.SettingID = settingId;
                            newModel.AddDate = DateTime.Now;
                            newModel.adduser = item.adduser;
                            newModel.Batch_ID = item.Batch_ID;
                            newModel.Batch_ZuoYeType_ID = item.Batch_ZuoYeType_ID;
                            newModel.BatchExt_ID = item.BatchExt_ID;
                            newModel.bid = item.bid;
                            newModel.Content = item.Content;
                            newModel.eid = item.eid;
                            newModel.lastdate = DateTime.Now;
                            newModel.lastuser = item.lastuser;
                            newModel.OperationTypeName = item.OperationTypeName;
                            newModel.TeamID = item.TeamID;
                            newModel.type = item.type;
                            newModel.videos = item.videos;
                            newModel.UserName = item.UserName;
                            newModel.zuoye_typeId = item.zuoye_typeId;
                            newModel.Status = item.Status;
                            newModels.Add(newModel);
                        }
                        dataContext.Batch_ZuoYe.InsertAllOnSubmit(newModels);
                    }
                    if (batchXunjian.Count() > 0)
                    {
                        List<Batch_XunJian> newModels = new List<Batch_XunJian>();
                        foreach (var item in batchXunjian)
                        {
                            Batch_XunJian newModel = new Batch_XunJian();
                            newModel.imgs = item.imgs;
                            newModel.Files = item.Files;
                            newModel.Enterprise_Info_ID = item.Enterprise_Info_ID;
                            newModel.SettingID = settingId;
                            newModel.AddDate = DateTime.Now;
                            newModel.adduser = item.adduser;
                            newModel.Batch_ID = item.Batch_ID;
                            newModel.languageid = item.languageid;
                            newModel.BatchExt_ID = item.BatchExt_ID;
                            newModel.EnglishContent = item.EnglishContent;
                            newModel.Content = item.Content;
                            newModel.mainmaterialid = item.mainmaterialid;
                            newModel.lastdate = DateTime.Now;
                            newModel.lastuser = item.lastuser;
                            newModel.UserCode = item.UserCode;
                            newModel.UserName = item.UserName;
                            newModel.videos = item.videos;
                            newModel.UserName = item.UserName;
                            newModel.Batch = item.Batch;
                            newModel.Status = item.Status;
                            newModels.Add(newModel);
                        }
                        dataContext.Batch_XunJian.InsertAllOnSubmit(newModels);
                    }
                    if (setAmbient.Count() > 0)
                    {
                        List<SetAmbient> newModels = new List<SetAmbient>();
                        foreach (var item in setAmbient)
                        {
                            SetAmbient newModel = new SetAmbient();
                            newModel.AddDate = DateTime.Now;
                            newModel.EnterpriseID = item.EnterpriseID;
                            newModel.SettingID = settingId;
                            newModel.InDate = item.InDate;
                            newModel.OutDate = item.OutDate;
                            newModel.Remark = item.Remark;
                            newModel.SetAcount = item.SetAcount;
                            newModel.Status = item.Status;
                            newModel.Temperature = item.Temperature;
                            newModels.Add(newModel);
                        }
                        dataContext.SetAmbient.InsertAllOnSubmit(newModels);
                    }
                    if (batchJianYanJianYi.Count() > 0)
                    {
                        List<Batch_JianYanJianYi> newModels = new List<Batch_JianYanJianYi>();
                        foreach (var item in batchJianYanJianYi)
                        {
                            Batch_JianYanJianYi newModel = new Batch_JianYanJianYi();
                            newModel.imgs = item.imgs;
                            newModel.Files = item.Files;
                            newModel.Enterprise_Info_ID = item.Enterprise_Info_ID;
                            newModel.SettingID = settingId;
                            newModel.AddDate = DateTime.Now;
                            newModel.adduser = item.adduser;
                            newModel.Batch_ID = item.Batch_ID;
                            newModel.languageid = item.languageid;
                            newModel.BatchExt_ID = item.BatchExt_ID;
                            newModel.EnglishContent = item.EnglishContent;
                            newModel.Content = item.Content;
                            newModel.mainmaterialid = item.mainmaterialid;
                            newModel.lastdate = DateTime.Now;
                            newModel.lastuser = item.lastuser;
                            newModel.UserCode = item.UserCode;
                            newModel.UserName = item.UserName;
                            newModel.UserName = item.UserName;
                            newModel.Batch = item.Batch;
                            newModel.Status = item.Status;
                            newModels.Add(newModel);
                        }
                        dataContext.Batch_JianYanJianYi.InsertAllOnSubmit(newModels);
                    }
                    if (setLogistics.Count() > 0)
                    {
                        List<SetLogistics> newModels = new List<SetLogistics>();
                        foreach (var item in setLogistics)
                        {
                            SetLogistics newModel = new SetLogistics();
                            newModel.AddDate = DateTime.Now;
                            newModel.EnterpriseID = item.EnterpriseID;
                            newModel.SetingID = settingId;
                            newModel.SetAcount = item.SetAcount;
                            newModel.CarAmbient = item.CarAmbient;
                            newModel.BillNum = item.BillNum;
                            newModel.CarNum = item.CarNum;
                            newModel.Status = item.Status;
                            newModel.EndAddress = item.EndAddress;
                            newModel.EndDate = item.EndDate;
                            newModel.StartAddress = item.StartAddress;
                            newModel.StartDate = item.StartDate;
                            newModel.Url = item.Url;
                            newModels.Add(newModel);
                        }
                        dataContext.SetLogistics.InsertAllOnSubmit(newModels);
                    }
                    dataContext.SubmitChanges();
                }
            }
            catch
            {
                result = new RetResult { CmdError = CmdResultError.EXCEPTION, Msg = "添加失败！" };
            }
            return result;
        }

        /// <summary>
        /// 更新配置红包状态
        /// </summary>
        /// <param name="settingId">配置码id</param>
        /// <returns></returns>
        public bool UpdatePacketState(long settingId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var model = dataContext.RequestCodeSetting.FirstOrDefault(t => t.ID == settingId);
                    model.PacketState = (int)Common.EnumFile.PacketState.Success;
                    dataContext.SubmitChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 码信息管理加备注信息2018-09-14
        /// </summary>
        /// <param name="subId">配置表标识</param>
        /// <returns>操作结果</returns>
        public FirstData SettingMemo(long subId)
        {
            FirstData result = new FirstData();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting requestCode = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == subId);
                    if (requestCode != null)
                    {
                        result.materialId = requestCode.MaterialID.GetValueOrDefault(0);
                        result.materialName = dataContext.Material.FirstOrDefault(m => m.Material_ID == requestCode.MaterialID).MaterialFullName;
                        //如果是套标
                        if (requestCode.BatchTrap != null && !string.IsNullOrEmpty(requestCode.BatchTrap.ToString()))
                        {
                            result.remaining = requestCode.BatchTrap.Value;
                            result.type = (int)Common.EnumFile.GenCodeType.trap;
                        }
                        else
                        {
                            result.remaining = requestCode.Count;
                            result.type = (int)Common.EnumFile.GenCodeType.single;
                        }
                        result.isFirst = false;
                        result.notFirst = true;
                        result.isSuccess = "0";
                        int index = dataContext.RequestCodeSetting.Where(m => m.EnterpriseId == requestCode.EnterpriseId).Count() + 1;
                        result.requestCodeType = requestCode.RequestCodeType.Value;
                        result.requestId = requestCode.RequestID;
                        result.memo = requestCode.Memo;
                        result.brandId = requestCode.BrandID.GetValueOrDefault(0);
                        result.createDate = DateTime.Now.ToString("yyyy-MM-dd");
                        result.zbatchName = requestCode.BatchName;
                    }
                    else
                    {
                        result.Msg = "没有找到申请码记录！";
                        result.isSuccess = "-1";
                    }
                }
            }
            catch
            {
                result.Msg = "程序遇到问题！";
                result.isSuccess = "-1";
            }
            return result;
        }

        /// <summary>
        /// 添加/编辑该批码备注信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Msg"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public RequestCodeSetting AddEditMemo(RequestCodeSetting model, out string Msg, out int error)
        {
            Msg = "保存失败！";
            error = -1;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting setting = dataContext.RequestCodeSetting.FirstOrDefault(
                        m => m.ID == model.ID && m.EnterpriseId == model.EnterpriseId);
                    if (setting != null)
                    {
                        setting.Memo = model.Memo;
                        dataContext.SubmitChanges();
                        Msg = "保存成功！";
                        error = 0;
                    }
                    else
                    {
                        Msg = "没有找到该批码信息！";
                    }
                }
            }
            catch { }
            return model;
        }

        #region 获取模板4图片信息
        /// <summary>
        ///  获取模板4图片信息
        /// </summary>
        /// <param name="rid">RequestCodeSettingID</param>
        /// <returns></returns>
        public RequestCodeSettingMuBan GetMuBanInfo(long rid)
        {
            RequestCodeSettingMuBan result = new RequestCodeSettingMuBan();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.RequestCodeSettingMuBan.FirstOrDefault(m => m.RequestCodeSettingID == rid);
                    if (result != null)
                    {
                        ClearLinqModel(result);
                    }
                }
                catch (Exception ex)
                {
                    string errData = "RequestCodeSettingDAL.GetMuBanInfo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 修改配置信息的拍码显示模板4的图片信息
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <param name="mubanModel">修改的实体</param>
        /// <param name="Msg">返回消息</param>
        /// <param name="error">返回消息码</param>
        /// <returns>操作结果</returns>
        public RequestCodeSetting EditMubanImg(RequestCodeSetting model, RequestCodeSettingMuBan mubanModel, out string Msg, out int error)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            Msg = "配置信息失败！";
            error = -1;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCodeSetting setting = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == model.ID);
                    RequestCodeSettingMuBan mubanOldModel = dataContext.RequestCodeSettingMuBan.FirstOrDefault(m => m.RequestCodeSettingID == mubanModel.RequestCodeSettingID);
                    if (setting != null && mubanOldModel != null)
                    {
                        if (model.DisplayOption != null)
                        {
                            setting.DisplayOption = model.DisplayOption;
                        }
                        if (model.StyleModel != null)
                        {
                            setting.StyleModel = model.StyleModel;
                        }
                        mubanOldModel.ImgLink = mubanModel.ImgLink;
                        mubanOldModel.MuBanImg = mubanModel.MuBanImg;
                        mubanOldModel.IsShow = mubanModel.IsShow;
                        dataContext.SubmitChanges();
                        Msg = "配置成功！";
                        error = 0;
                    }
                    else if (setting != null && mubanOldModel == null)
                    {
                        if (model.DisplayOption != null)
                        {
                            setting.DisplayOption = model.DisplayOption;
                        }
                        if (model.StyleModel != null)
                        {
                            setting.StyleModel = model.StyleModel;
                        }
                        RequestCodeSettingMuBan temp = new RequestCodeSettingMuBan();
                        temp.ImgLink = mubanModel.ImgLink;
                        temp.MuBanImg = mubanModel.MuBanImg;
                        temp.IsShow = mubanModel.IsShow;
                        temp.AddDate = DateTime.Now;
                        temp.AddUserID = pf.UserID;
                        temp.BatchName = setting.BatchName;
                        temp.EnterpriseID = pf.EnterpriseID;
                        temp.RequestCodeSettingID = setting.ID;
                        dataContext.RequestCodeSettingMuBan.InsertOnSubmit(temp);
                        dataContext.SubmitChanges();
                        Msg = "配置成功！";
                        error = 0;
                    }
                    else
                    {
                        Msg = "没有找到配置的数据！";
                    }
                }
            }
            catch { }
            return model;
        }
        #endregion
    }
}
