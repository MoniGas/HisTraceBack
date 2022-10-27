using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class ShowDeptBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取部门宣传列表
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <param name="name">部门名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultList GetList(long companyId, string name, int pageIndex)
        {
            long totalCount = 0;
            ShowDeptDAL dal = new ShowDeptDAL();
            List<ShowDept> model = dal.GetList(companyId, name, pageIndex, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <param name="id">部门标识</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel GetModel(long id)
        {
            ShowDeptDAL dal = new ShowDeptDAL();
            ShowDept model = dal.GetModel(id);
            return ToJson.NewModelToJson(model, model == null ? "0" : "1", model == null ? "获取数据失败！" : "获取数据成功！");
        }

        /// <summary>
        /// 添加部门宣传
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="mainCode">企业主码</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel Add(ShowDept model, string mainCode)
        {
            ShowDeptDAL dal = new ShowDeptDAL();
            RetResult ret = new RetResult();
            if (string.IsNullOrEmpty(model.DeptName))
            {
                ret.Msg = "部门名称不能为空！";
            }
            else
            {
                ret = dal.Add(model, mainCode);
            }
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel Edit(ShowDept model)
        {
            ShowDeptDAL dal = new ShowDeptDAL();
            RetResult ret = new RetResult();
            if (string.IsNullOrEmpty(model.DeptName))
            {
                ret.Msg = "部门名称不能为空！";
            }
            else
            {
                ret = dal.Edit(model);
            }
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id">部门标识</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel Del(long id)
        {
            ShowDeptDAL dal = new ShowDeptDAL();
            RetResult ret = dal.Del(id);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }
    }
}
