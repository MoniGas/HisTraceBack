/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-12-19

** 联系方式 :13313318725

** 描述：主要用于原料查询的数据库操作 移植

** 版本：v1.0

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
    /// 主要用于原料查询的数据库操作
    /// </summary>
    public class OriginDAL : DALBase
    {
        /// <summary>
        /// 获取原料列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="originName">原料名称</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<View_Origin> GetList(long enterpriseId, string originName, out long totalCount, int pageIndex)
        {
            List<View_Origin> result = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_Origin.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(originName))
                    {
                        data = data.Where(m => m.OriginName.Contains(originName.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Origin_ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "OriginDAL.GetList():Origin表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加原料信息
        /// </summary>
        /// <param name="Origin">原料实体模型</param>
        /// <returns></returns>
        public RetResult Add(Origin origin)
        {
            string Msg = "添加原材料信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Origin.FirstOrDefault(m => m.OriginName == origin.OriginName
                        && m.Enterprise_Info_ID == origin.Enterprise_Info_ID && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "已存在该原材料名称！";
                    }
                    else
                    {
                        dataContext.Origin.InsertOnSubmit(origin);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = origin.Origin_ID;
                        Msg = "添加原材料信息成功";
                        error = CmdResultError.NONE;
                        //为统计表添加数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = origin.Enterprise_Info_ID;
                        homeData.OriginCount = 1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult result = dal.Update(homeData);
                    }
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 修改原料信息
        /// </summary>
        /// <param name="Origin"></param>
        /// <returns></returns>
        public RetResult Edit(Origin origin)
        {
            string Msg = "修改原材料信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = from m in dataContext.Origin
                                where m.OriginName == origin.OriginName && m.Origin_ID != origin.Origin_ID
                                    && m.Enterprise_Info_ID == origin.Enterprise_Info_ID && m.Status == (int)Common.EnumFile.Status.used
                                select m;
                    if (model.Count() > 0)
                    {
                        Msg = "已存在该原材料！";
                    }
                    else
                    {
                        var data = dataContext.Origin.FirstOrDefault(m => m.Enterprise_Info_ID == origin.Enterprise_Info_ID && m.Origin_ID == origin.Origin_ID);
                        if (data == null)
                        {
                            Msg = "没有找到要修改的数据！";
                        }
                        else
                        {
                            data.OriginName = origin.OriginName;
                            data.Descriptions = origin.Descriptions;
                            data.OriginImgInfo = origin.OriginImgInfo;
                            data.lastdate = origin.lastdate;
                            data.lastuser = origin.lastuser;
                            dataContext.SubmitChanges();
                            Msg = "修改原材料信息成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
                Ret.SetArgument(error, Msg, Msg);
                return Ret;
            }
        }

        /// <summary>
        /// 删除原料信息
        /// </summary>
        /// <param name="originID">原料ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public RetResult Delete(long originID, long enterpriseId)
        {
            string Msg = "删除原材料失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Origin origin = dataContext.Origin.SingleOrDefault(m => m.Origin_ID == originID);
                    if (origin == null)
                    {
                        Msg = "没有找到要删除的原材料信息请刷新列表！";
                    }
                    else
                    {
                        origin.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Msg = "删除原材料信息成功！";
                        error = CmdResultError.NONE;
                        //为统计表删除数据
                        HomeDataStatis homeData = new HomeDataStatis();
                        homeData.EnterpriseID = origin.Enterprise_Info_ID;
                        homeData.OriginCount = -1;
                        ComplaintDAL dal = new ComplaintDAL();
                        RetResult result = dal.Update(homeData);
                    }
                }
            }
            catch
            {
                //Msg = "删除失败，请首先删除已知关联的其他数据";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 根据原料ID获取原料信息
        /// </summary>
        /// <param name="originID">原料ID</param>
        /// <returns></returns>
        public Origin GetOriginByID(long originID)
        {
            Origin origin = new Origin();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    origin = dataContext.Origin.FirstOrDefault(t => t.Origin_ID == originID);
                    ClearLinqModel(origin);
                }
            }
            catch
            {
            }
            return origin;
        }

        /// <summary>
        /// 获取原料列表
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public List<Origin> GetOriginList(long eId)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = (from data in dataContext.Origin
                                    where data.Enterprise_Info_ID == eId && data.Status == (int)Common.EnumFile.Status.used
                                    select data).OrderByDescending(m => m.Origin_ID).ToList();
                    ClearLinqModel(dataList);
                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取自动搜索数据
        /// </summary>
        /// <param name="page">页面</param>
        /// <param name="flag">字段</param>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public List<Dictorynary_Key> GetDictoryKey(string page, int flag, string value, long enterpriseId)
        {
            using (var dataContext = GetDataContext())
            {
                var objLst = dataContext.Dictorynary_Key.Where(a => a.PageJs == page && a.Flag == flag && a.EnterpriseInfoId == enterpriseId);
                if (!string.IsNullOrEmpty(value))
                {
                    objLst = objLst.Where(a => a.Value.Contains(value));
                }
                return objLst.OrderByDescending(a => a.Value).ToList();
            }
        }
    }
}
