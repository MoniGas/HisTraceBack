/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-18

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于投诉管理数据层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Argument;
using LinqModel;
using Common.Log;
using System.Xml.Linq;
using System.Configuration;

namespace Dal
{
    public class ComplaintDAL : DALBase
    {
        /// <summary>
        /// 获取投诉管理未读信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public List<View_ComplaintAndType> GetList(long id, out int dataCount)
        {
            dataCount = 0;
            List<View_ComplaintAndType> objView_ComplaintAndType = new List<View_ComplaintAndType>();

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    int comdate = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["ComplaintDate"]);
                    var dataList = (from data in dataContext.View_ComplaintAndType
                                    where data.Enterprise_Info_ID == id && data.ComplaintDate >= DateTime.Now.AddDays(comdate) && data.Status == (int)EnumFile.Status.used && data.IsRead == (int)EnumFile.IsRead.noRead
                                    select data).ToList();
                    dataCount = dataList.Count;
                    if (objView_ComplaintAndType != null)
                    {
                        objView_ComplaintAndType = dataList.OrderByDescending(o => o.ComplaintDate.Value).ToList();
                    }
                    ClearLinqModel(objView_ComplaintAndType);
                    return objView_ComplaintAndType;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取投诉信息列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<View_ComplaintAndType> GetList(long enterpriseId, string materialName, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<View_ComplaintAndType> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.View_ComplaintAndType.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        tempResult = tempResult.Where(m => m.MaterialName.Contains(materialName.Trim()) || m.ComplaintContent.Contains(materialName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Material_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 修改投诉信息为已读
        /// </summary>
        /// <param name="id">投诉列表ID</param>
        /// <returns></returns>
        public RetResult UpdateStatus(long id)
        {
            CmdResultError error = CmdResultError.NO_RESULT;

            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataInfo = (from data in dataContext.Complaint
                                    where data.Complaint_ID == id
                                    select data).FirstOrDefault();

                    if (dataInfo != null)
                    {
                        dataInfo.IsRead = (int)EnumFile.IsRead.isRead;
                    }

                    dataContext.SubmitChanges();

                    error = CmdResultError.NONE;
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");

            return Ret;
        }

        /// <summary>
        /// 删除投诉信息
        /// </summary>
        /// <param name="complaintid">企业标识</param>
        /// <param name="materialId">产品标识</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long enterpriseId, long complaintid)
        {
            Ret.Msg = "删除投诉信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Complaint.FirstOrDefault(p => p.Complaint_ID == complaintid && p.Enterprise_Info_ID == enterpriseId);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要删除的投诉信息！";
                    }
                    else
                    {
                        model.Status = (int)EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Ret.Msg = "删除信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch { }
            }
            return Ret;
        }

        /// <summary>
        /// 消费者投诉
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>返回结果</returns>
        public RetResult AddComplaint(Complaint model)
        {
            string Msg = "投诉失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    dataContext.Complaint.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Msg = "投诉成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception ex)
            {
                Msg = "连接服务器失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取监管部门信息
        /// </summary>
        /// <param name="PRRU_PlatFormId">监管部门Id</param>
        /// <returns>返回监管部门信息集合</returns>
        public PRRU_PlatForm GetPlatForm(long PRRU_PlatFormId)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var DataModel = (from data in dataContext.PRRU_PlatForm
                                     where data.PRRU_PlatForm_ID == PRRU_PlatFormId
                                     select data).FirstOrDefault();

                    return DataModel;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取主页统计数据
        /// </summary>
        /// <param name="enterpriseID"></param>
        /// <returns></returns>
        public HomeDataStatis GetHomeDataStatis(long enterpriseID)
        {
            HomeDataStatis result = new HomeDataStatis();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    string url = string.Empty;
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(p => p.Enterprise_Info_ID == enterpriseID);
                    if (enterprise != null)
                    {
                        ClearLinqModel(enterprise);
                        //判断企业Logo是否为空
                        if (!string.IsNullOrEmpty(enterprise.StrLogo))
                        {
                            XElement Xml = XElement.Parse(enterprise.StrLogo);
                            IEnumerable<XElement> AllImg = Xml.Elements("img");
                            foreach (var Item in AllImg)
                            {
                                //ToJsonImg sub = new ToJsonImg();
                                url = Item.Attribute("small").Value;
                            }
                        }
                    }
                    result = dataContext.HomeDataStatis.FirstOrDefault(m => m.EnterpriseID == enterpriseID);
                    if (result != null)
                    {
                        result.RequestCodeCount = result.RequestCodeCount.GetValueOrDefault(0);
                        result.RequestCodeTimes = result.RequestCodeTimes.GetValueOrDefault(0);
                        result.ScanCodeTimes = result.ScanCodeTimes.GetValueOrDefault(0);
                    }
                    else
                    {
                        result = new HomeDataStatis();
                        result.RequestCodeCount = 0;
                        result.RequestCodeTimes = 0;
                        result.ScanCodeTimes = 0;
                    }
                    result.BrancCount = dataContext.Brand.Count(p => p.Enterprise_Info_ID == enterpriseID && p.Status == (int)EnumFile.Status.used);
                    result.MaterialCount = dataContext.Material.Count(p => p.Enterprise_Info_ID == enterpriseID && p.Status == (int)EnumFile.Status.used);
                    result.OriginCount = dataContext.Origin.Count(p => p.Enterprise_Info_ID == enterpriseID && p.Status == (int)EnumFile.Status.used);
                    result.ProcessCount = dataContext.Process.Count(p => p.EnterpriseID == enterpriseID && p.status == (int)EnumFile.Status.used);
                    result.LogoUrl = url;
                }
                catch (Exception ex)
                {
                    string errData = "ComplaintDAL.GetHomeDataStatis()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 更新统计表
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>更新结果</returns>
        public RetResult Update(HomeDataStatis model)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = (from p in dataContext.HomeDataStatis
                                  where p.EnterpriseID == model.EnterpriseID
                                      select p).FirstOrDefault();
                    if (data != null)
                    {
                        if (model.BrancCount!=null&model.BrancCount != 0)
                        {
                            data.BrancCount = data.BrancCount.GetValueOrDefault(0) + model.BrancCount;
                        }
                        if (model.MaterialCount != null & model.MaterialCount != 0)
                        {
                            data.MaterialCount = data.MaterialCount.GetValueOrDefault(0) + model.MaterialCount;
                        }
                        if (model.OriginCount != null & model.OriginCount != 0)
                        {
                            data.OriginCount = data.OriginCount.GetValueOrDefault(0) + model.OriginCount;
                        }
                        if (model.ProcessCount != null & model.ProcessCount != 0)
                        {
                            data.ProcessCount = data.ProcessCount.GetValueOrDefault(0) + model.ProcessCount;
                        }
                        if (model.RequestCodeCount > 0)
                        {
                            data.RequestCodeCount =data.RequestCodeCount.GetValueOrDefault(0)+ model.RequestCodeCount;
                        }
                        if (model.RequestCodeTimes > 0)
                        {
                            data.RequestCodeTimes = data.RequestCodeTimes.GetValueOrDefault(0) + model.RequestCodeTimes;
                        }
                        if (model.ScanCodeTimes > 0)
                        {
                            data.ScanCodeTimes = data.ScanCodeTimes.GetValueOrDefault(0) + model.ScanCodeTimes;
                        }
                    }
                    else
                    {
                        dataContext.HomeDataStatis.InsertOnSubmit(model);
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, null, "添加成功");
                }
                catch (Exception ex)
                {
                    Ret.SetArgument(CmdResultError.EXCEPTION, ex.ToString(), "添加失败");
                }
            }
            return Ret;
        }

        /// <summary>
        /// 获取统计报表数据
        /// </summary>
        /// <param name="enterpriseId">企业编号</param>
        /// <returns></returns>
        public List<ChartData> GetChartData(long enterpriseId)
        {
            List<ChartData> result = new List<ChartData>();
            try
            {
                string conString = ConfigurationManager.AppSettings["Code_Connect"];
                using (var dataContext = GetContext(conString))
                {
                    DateTime upWeek = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(-7);
                    for (int i = 7; i >= 0; i--)
                    {
                        string tempDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(-i).ToString("yyyy-MM-dd");
                        ScanSumDay tempData = dataContext.ScanSumDay.FirstOrDefault(m => m.Days == tempDate && m.EnterpriseId == enterpriseId);
                        ChartData resultSub = new ChartData();
                        if (tempData == null)
                        {
                            resultSub.title = tempDate.Substring(5, tempDate.Length - 5);
                            resultSub.value = "0";
                        }
                        else
                        {
                            resultSub.title = tempDate.Substring(5,tempDate.Length-5);
                            resultSub.value = tempData.ScanCount.ToString();
                        }
                        result.Add(resultSub);
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
