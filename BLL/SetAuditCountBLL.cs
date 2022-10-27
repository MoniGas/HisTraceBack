/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-25

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：主要用于设置监管部门审核码数量的业务层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 用于设置监管部门审核码数量的业务层
    /// </summary>
    public class SetAuditCountBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取监管部门审核码数量列表
        /// </summary>
        /// <param name="upid">上级ID</param>
        /// <param name="pId">监管部门ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetList(long upid, long pId, int pageIndex)
        {
            long totalCount = 0;
            SetAuditCountDAL dal = new SetAuditCountDAL();
            List<View_SetAuditCount> model = dal.GetList(upid, pId, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取监管部门审核码数量
        /// </summary>
        /// <param name="pid">监管部门ID</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long pid)
        {
            SetAuditCountDAL dal = new SetAuditCountDAL();
            SetAuditCount model = dal.GetModel(pid);
            string code = "1";
            string msg = "";
            if (model == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel result = ToJson.NewModelToJson(model, code, msg);
            return result;
        }

        /// <summary>
        /// 设置审核码数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel Add(SetAuditCount model)
        {
            SetAuditCountDAL dal = new SetAuditCountDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            ret = dal.Add(model);
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
    }
}
