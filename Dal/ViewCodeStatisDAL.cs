/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-29

** 修改时间：

** 联系方式 :13313318725

** 描述：企业生成码统计数据访问

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 企业生成码统计数据访问
    /// </summary>
    public class ViewCodeStatisDAL : DALBase
    {
        /// <summary>
        /// 获取生成二维码数据
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex">页码</param>
        /// <param name="matype">产品类别</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<View_CodeStatis> GetEnCodeList(long eid, out long totalCount, out View_CodeStatis model, int pageIndex, int mlx, string materialName, string beginDate, string endDate)
        {
            List<View_CodeStatis> result = new List<View_CodeStatis>();
            totalCount = 0;
            model = new View_CodeStatis();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_CodeStatis.Where(m => m.Enterprise_Info_ID == eid);
                    if (mlx > 0)
                    {
                        if (mlx == 1)
                        {
                            data = data.Where(m => m.Type == (int)Common.EnumFile.GenCodeType.single || m.Type == (int)Common.EnumFile.GenCodeType.localCreate);
                        }
                        else if (mlx == 2)
                        {
                            //data = data.Where(m => m.Type == (int)Common.EnumFile.GenCodeType.boxCode || m.Type == (int)Common.EnumFile.GenCodeType.localCreateBox);
                            data = data.Where(m => m.Type == (int)Common.EnumFile.GenCodeType.trap);
                        }
                        else if (mlx == 3)
                        {
                            data = data.Where(m => m.Type == (int)Common.EnumFile.GenCodeType.gift || m.Type == (int)Common.EnumFile.GenCodeType.localGift);
                        }
                        else if (mlx == 4)
                        {
                            data = data.Where(m => m.Type == (int)Common.EnumFile.GenCodeType.pesticides);
                        }
                    }
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        data = data.Where(m => m.MaterialFullName.Contains(materialName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.adddate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.adddate <= Convert.ToDateTime(endDate));
                    }
                    #region 赵慧敏加 统计总数 可以不用GetModelByID方法，省去一个步骤
                    ////计总数
                    long totalNum = 0;
                    foreach (var sub in data)
                    {
                        totalNum = totalNum + Convert.ToInt64(sub.TotalNum);
                    }
                    Enterprise_Info eninfo = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eid);
                    model.EnterpriseName = eninfo.EnterpriseName;
                    model.TotalNum = totalNum;
                    #endregion
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RequestCode_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "GetEnCodeList():View_CodeStatis视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取企业买码记录
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="totalCount"></param>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<View_ContinueCode> GetContinneCodeRecord(long eid, out long totalCount, out View_ContinueCode model, int pageIndex, string beginDate, string endDate)
        {
            List<View_ContinueCode> result = new List<View_ContinueCode>();
            totalCount = 0;
            model = new View_ContinueCode();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_ContinueCode.Where(m => m.EnterpriseID == eid);
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.AddDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.AddDate <= Convert.ToDateTime(endDate));
                    }
                    
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ContinneCodeRecordID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "GetContinneCodeRecord():View_ContinueCode视图";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 设置码阀值
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public RetResult SetThreshold(long eid,int count)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "操作失败";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == eid).FirstOrDefault();
                    if (model == null)
                    {
                        result.Msg = "没有找到要操作的数据！";
                    }
                    else
                    {
                        model.Threshold = count;
                        dataContext.SubmitChanges();
                        result.Msg = "操作成功";
                        result.CmdError = CmdResultError.NONE;
                    }
                }
                catch
                { }
            }
            return result;
        }

        public Enterprise_Info GetThreshold(long eid)
        {
            Enterprise_Info result = new Enterprise_Info();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                var model = dataContext.Enterprise_Info.Where(p => p.Enterprise_Info_ID == eid).FirstOrDefault();
                result=model;
            }
            return result;
        }
    }
}
