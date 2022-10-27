/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-5
** 联系方式:13313318725
** 代码功能：二维码套餐管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;

namespace Dal
{
    public class PackageDAL : DALBase
    {
        /// <summary>
        /// 获取二维码套餐列表
        /// </summary>
        /// <param name="packbageName">套餐名称</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<YX_Package> GetList(string packbageName, int status, int? pageIndex)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.YX_Package select m;
                    if (!string.IsNullOrEmpty(packbageName))
                    {
                        data = data.Where(m => m.PackbageName.Contains(packbageName.Trim()));
                    }
                    if (status != 0)
                    {
                        data = data.Where(m => m.PackageStatus == status);
                    }
                    data = data.OrderByDescending(m => m.PackageID);
                    return data.ToPagedList(pageIndex ?? 1, PageSize);
                }
                catch (Exception ex)
                {
                    string errData = "PackageDAL.GetList():YX_Package";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 添加二维码套餐
        /// </summary>
        /// <param name="model">添加实体</param>
        /// <returns></returns>
        public RetResult AddModel(YX_Package model)
        {
            string Msg = "添加失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.YX_Package.FirstOrDefault(m => m.PackbageName == model.PackbageName &&
                        m.PackageStatus == (int)Common.EnumText.PackageStatus.qiyong);
                    if (data != null)
                    {
                        Msg = "已存在该套餐名称！";
                    }
                    else
                    {
                        dataContext.YX_Package.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = model.PackageID;
                        Msg = "添加成功！";
                        error = CmdResultError.NONE;
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
        /// 启用套餐
        /// </summary>
        /// <param name="id">套餐ID</param>
        /// <returns></returns>
        public RetResult UpdataStatusQ(long id)
        {
            string Msg = "启用失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    YX_Package model = dataContext.YX_Package.SingleOrDefault(m => m.PackageID == id);
                    if (model == null)
                    {
                        Msg = "没有找到要启用的套餐请刷新列表！";
                    }
                    else
                    {
                        model.PackageStatus = (int)Common.EnumText.PackageStatus.qiyong;
                        dataContext.SubmitChanges();
                        Msg = "启用成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
                Msg = "数据库连接失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 根据ID获取套餐信息
        /// </summary>
        /// <param name="id">套餐ID</param>
        /// <returns></returns>
        public YX_Package GetModelByID(long id)
        {
            YX_Package model = new YX_Package();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    model = dataContext.YX_Package.FirstOrDefault(t => t.PackageID == id);
                }
            }
            catch
            {
            }
            return model;
        }

        /// <summary>
        /// 获取套餐列表
        /// </summary>
        /// <returns></returns>
        public List<YX_Package> GetPackageList()
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.YX_Package where m.PackageStatus == (int)Common.EnumText.PackageStatus.qiyong select m;
                    data = data.OrderByDescending(m => m.PackageID);
                    return data.ToList();
                }
                catch (Exception ex)
                {
                    string errData = "PackageDAL.GetPackageList():YX_Package";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                    return null;
                }
            }
        }
    }
}
