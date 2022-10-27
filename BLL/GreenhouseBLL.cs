/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-12

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于生产基地管理业务层

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class GreenhouseBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        ResultArg Arg = new ResultArg();
        /// <summary>
        /// 获取生产基地信息方法
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="ewm">二维码</param>
        /// <param name="name">生产基地名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回生产基地信息集合的json字符串</returns>
        public BaseResultList GetList(long enterpriseID, string ewm, string name, int? pageIndex)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            long totalCount = 0;

            List<LinqModel.Greenhouses> objList = objGreenhouseDAL.GetList(enterpriseID, ewm, name, pageIndex, out totalCount);

            return ToJson.NewListToJson(objList, pageIndex.Value, PageSize, totalCount, "");
        }
        /// <summary>
        /// 获取生产基地列表
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public BaseResultList GetMList(long enterpriseId)
        {
            GreenhouseDAL dal = new GreenhouseDAL();
            List<Greenhouses> model = dal.GetList(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, 10000000, 0, "");
            return result;
        }

        /// <summary>
        /// 根据ID获取生产基地信息方法
        /// </summary>
        /// <param name="id">生产基地ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel SearchData(long id)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();
            Greenhouses objGreenhouses = objGreenhouseDAL.SearchData(id);
            return ToJson.NewModelToJson(objGreenhouses, objGreenhouses == null ? "0" : "1", "");
        }

        /// <summary>
        /// 添加生产基地方法
        /// </summary>
        /// <param name="greenHouses">生产基地实体对象</param>
        /// <returns>返回操作结果的json字符串</returns>
        public LinqModel.BaseResultModel Add(Greenhouses greenHouses)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            RetResult objRetResult = objGreenhouseDAL.Add(greenHouses);

            return ToJson.NewModelToJson(objRetResult.CrudCount, Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        /// <summary>
        /// 根据ID获取生产基地信息
        /// </summary>
        /// <param name="id">生产基地ID</param>
        /// <returns>返回生产基地信息的json字符串</returns>
        public string GetGreenInfo(long id)
        {
            Arg.Msg = "查找列表失败！";
            Arg.CmdError = CmdResultError.EXCEPTION;
            Greenhouses data = new Greenhouses();
            if (id < 0)
            {
                Arg.CmdError = CmdResultError.PARAMERROR;
                Arg.Msg = "生产单元ID输入错误！";
                return ToJson.ModelToJson(data, 1, "");
            }

            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            data = objGreenhouseDAL.GetGreenInfo(id);

            if (data != null)
            {
                Arg.ObjList = data;
                Arg.TotalCount = 1;
                Arg.PageIndex = 1;
                Arg.CmdError = CmdResultError.NONE;
                Arg.Msg = "查询成功！";
            }
            else
            {
                Arg.Msg = "数据库连接失败！";
            }
            return ToJson.ModelToJson(data, 1, "");
        }
        /// <summary>
        /// 删除生产基地信息方法
        /// </summary>
        /// <param name="id">生产基地ID</param>
        /// <returns>返回操作结果的json字符串</returns>
        public BaseResultModel Del(long operationTypeId)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            RetResult ret = objGreenhouseDAL.Del(operationTypeId);
            BaseResultModel objRetResult = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return objRetResult;
        }
        /// <summary>
        /// 修改生产基地信息方法
        /// </summary>
        /// <param name="greenHouses">生产基地实体类</param>
        /// <returns>返回操作结果的json字符串</returns>
        public LinqModel.BaseResultModel Edit(Greenhouses greenHouses)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            RetResult objRetResult = objGreenhouseDAL.Edit(greenHouses);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        #region VB程序接口对接
        public Boolean AddProdeData(ProdeData data, out int Msg)
        {
            return new Dal.GreenhouseDAL().AddProdeData(data, out Msg);
        }
        public BaseResultList GetProbeList(long eId, long uId, string title, string bDate, string eDate, int pageIndex)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            long totalCount = 0;

            List<View_Greenhouses_Probe> objList = objGreenhouseDAL.GetProbeList(eId, uId, title, bDate, eDate, pageIndex, PageSize, out totalCount);

            return ToJson.NewListToJson(objList, pageIndex, PageSize, totalCount, "");
        }
        public BaseResultList GetDataList(long eId, long gpId, string sDate, string eDate, int? pageIndex)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            long totalCount = 0;

            List<Greenhouses_Probe_Data> objList = objGreenhouseDAL.GetDataList(eId, gpId, sDate, eDate, pageIndex, PageSize, out totalCount);

            return ToJson.NewListToJson(objList, pageIndex.Value, PageSize, totalCount, "");
        }

        public PagedList<View_Greenhouses_Probe> GetProbeListEntity(long eId, long uId, string title, string bDate, string eDate, int? pageIndex)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            PagedList<View_Greenhouses_Probe> objList = objGreenhouseDAL.GetProbeListEntity(eId, uId, title, bDate, eDate, pageIndex, PageSize);

            return objList;
        }
        public PagedList<Greenhouses_Probe_Data> GetDataListEntity(long eId, long gpId, string sDate, string eDate, int? pageIndex)
        {
            GreenhouseDAL objGreenhouseDAL = new GreenhouseDAL();

            PagedList<Greenhouses_Probe_Data> objList = objGreenhouseDAL.GetDataListEntity(eId, gpId, sDate, eDate, pageIndex, PageSize);

            return objList;
        }
        #endregion
    }
}
