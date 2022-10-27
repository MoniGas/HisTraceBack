/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-7-5
** 联系方式:13313318725
** 代码功能：二维码套餐管理
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    public class PackageBLL
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
            PackageDAL dal = new PackageDAL();
            PagedList<YX_Package> list = dal.GetList(packbageName, status, pageIndex);
            return list;
        }

        /// <summary>
        /// 添加二维码套餐
        /// </summary>
        /// <param name="model">添加实体</param>
        /// <returns></returns>
        public RetResult AddModel(YX_Package model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            PackageDAL dal = new PackageDAL();
            result = dal.AddModel(model);
            return result;
        }

        /// <summary>
        /// 根据ID获取套餐信息
        /// </summary>
        /// <param name="id">套餐ID</param>
        /// <returns></returns>
        public YX_Package GetModel(long id)
        {
            YX_Package result = new YX_Package();
            PackageDAL dal = new PackageDAL();
            result = dal.GetModelByID(id);
            return result;
        }

        /// <summary>
        /// 获取套餐列表
        /// </summary>
        /// <returns></returns>
        public List<YX_Package> GetPackageList()
        {
            PackageDAL dal = new PackageDAL();
            List<YX_Package> list = dal.GetPackageList();
            return list;
        }
    }
}
