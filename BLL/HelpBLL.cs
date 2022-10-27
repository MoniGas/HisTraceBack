/********************************************************************************

** 作者： 靳晓聪

** 创始时间：2017-01-19

** 联系方式 :13313318725

** 描述：主要用于帮助管理逻辑层

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    /// <summary>
    /// 主要用于帮助管理逻辑层
    /// </summary>
    public class HelpBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="HelpTitle">帮助名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetList(string HelpTitle, int pageIndex)
        {
            HelpDAL dal = new HelpDAL();
            long totalCount = 0;
            List<View_Help> liDearer = dal.GetList(HelpTitle, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liDearer, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 添加帮助
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Add(Help model)
        {
            HelpDAL dal = new HelpDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.HelpTitle))
            {
                ret.Msg = "名称不能为空！";
            }
            else
            {
                ret = dal.Add(model);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取帮助模型
        /// </summary>
        /// <param name="HelpId">帮助标识</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel GetModel(long HelpId)
        {
            HelpDAL dal = new HelpDAL();
            Help Help = dal.GetModel(HelpId);
            string code = "1";
            string msg = "";
            if (Help == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(Help, code, msg);
            return model;
        }


        /// <summary>
        /// 修改帮助
        /// </summary>
        /// <param name="newModel">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Edit(Help newModel)
        {
            HelpDAL dal = new HelpDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(newModel.HelpTitle))
            {
                ret.Msg = "名称不能为空！";
            }       
            else
            {
                ret = dal.Edit(newModel);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除帮助
        /// </summary>
        /// <param name="dealerId">帮助标识</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Del(long HelpId)
        {
            HelpDAL dal = new HelpDAL();
            RetResult ret = dal.Del(HelpId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取帮助类型列表
        /// </summary>
        /// <returns></returns>
        public BaseResultList GetList()
        {
            HelpDAL dal = new HelpDAL();
            List<HelpType> liDearer = dal.GetList();
            BaseResultList result = ToJson.NewListToJson(liDearer, 0, _PageSize, 0, "");
            return result;
        }

        /// <summary>
        /// 修改帮助
        /// </summary>
        /// <param name="newModel">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel EditSort(Help newModel)
        {
            HelpDAL dal = new HelpDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            ret = dal.EditSort(newModel);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="HelpTitle">帮助名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetHelpList(string HelpTitle, int pageIndex)
        {
            HelpDAL dal = new HelpDAL();
            long totalCount = 0;
            List<View_Help> liDearer = dal.GetHelpList(HelpTitle, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(liDearer, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="typeId">帮助类型typeId</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>操作结果</returns>
        public PagedList<View_Help> GetPagedList(long typeId, int? pageIndex)
        {
            HelpDAL dal = new HelpDAL();
            PagedList<View_Help> dataList = dal.GetPagedList(typeId,pageIndex);
            return dataList;
        }

        public PagedList<View_Help> GetMoreList(long typeId, int index, string name, int? pageIndex)
        {
            HelpDAL dal = new HelpDAL();
            PagedList<View_Help> dataList = dal.GetMoreList(typeId, index, name, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 获取帮助类型列表
        /// </summary>
        /// <returns></returns>
        public List<HelpType> GetTypeList()
        {
            return new HelpDAL().GetList();
        }

        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        public View_Help GetDetails(long helpId)
        {
            return new HelpDAL().GetDetails(helpId);
        }

        /// <summary>
        /// 设置访问量
        /// </summary>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        public Help UpdateCount(long helpId)
        {
            return new HelpDAL().UpdateCount(helpId);
        }

        /// <summary>
        /// 设置有效数量
        /// </summary>
        /// <param name="helpId">帮助id</param>
        /// <returns></returns>
        public RetResult UpdateUsefulCount(int type,long helpId)
        {
            return new HelpDAL().UpdateUsefulCount(type,helpId);
        }
    }
}
