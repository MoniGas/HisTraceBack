/********************************************************************************

** 作者： 张翠霞

** 创始时间：2017-02-08

** 联系方式 :13313318725

** 描述：追溯码信息管理

** 版本：v2.5.1

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;
using System.Data;

namespace BLL
{
    /// <summary>
    /// 追溯码信息管理业务逻辑层
    /// </summary>
    public class RequestCodeMaBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取追溯码生成记录
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="searchName">名称信息</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId, string searchName, string materialName, string bName,
            string beginDate, string endDate, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            List<View_RequestCodeMa> model = dal.GetList(enterpriseId, searchName, materialName, bName,
                beginDate, endDate, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 分段码列表根据requestcodeID查询列表
        /// </summary>
        /// <param name="requestcodeID">申请码表ID</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="searchName">名称信息</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetRequestCodeSettingList(long enterpriseId, long requestcodeID, string searchName, string materialName, string bName,
            string beginDate, string endDate, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            List<View_RequestCodeSetting> model = dal.GetRequestCodeSettingList(enterpriseId, requestcodeID, searchName, materialName, bName,
                beginDate, endDate, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 查询企业所有配置码信息
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="searchName">名称信息</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="bName">批次号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetRequestCodeSettingListAll(long enterpriseId, string searchName, string materialName, string bName,
            string beginDate, string endDate, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            List<View_RequestCodeSetting> model = dal.GetRequestCodeSettingListAll(enterpriseId, searchName, materialName, bName,
                beginDate, endDate, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }
        /// <summary>
        /// 接口查询企业所有配置码信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="materialName"></param>
        /// <param name="count"></param>
        /// <param name="batchName"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public BaseResultList GetInRequestCodeSettingListAll(string PackagingIine,string beginDate, string endDate, string materialName, string count, string batchName, string spec)
        {
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            List<View_RequestCodeSetting> model = dal.GetInRequestCodeSettingListAll(beginDate, endDate, materialName, count, batchName, spec);
            List<RReParam> rrl = new List<RReParam>();
            foreach (var item in model)
            {
                RReParam rr = new RReParam();
                rr.BatchName = item.BatchName;
                rr.Count = item.Count.ToString();
                rr.MaterialFullName = item.MaterialFullName;
                rr.PackagingIine = PackagingIine;
                rr.Spec = rr.Spec;
                DataView dvRecord = new BLL.RequestCodeSettingAddBLL().getRecordByID(item.ID);
                if (!string.IsNullOrEmpty(dvRecord[0]["FileURL"].ToString()))
                {
                    rr.Url = dvRecord[0]["FileURL"].ToString();
                }
                rrl.Add(rr);
            }

            BaseResultList result = ToJson.NewListToJson(rrl, 0, 0, 0, "");
            return result;
        }

        /// <summary>
        /// 产销设置表
        /// </summary>
        /// <param name="materialId">产品编码</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public BaseResultList GetRequestCodeSettingListAll(long materialId, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            List<View_RequestCodeSetting> model = dal.GetRequestCodeSettingListAll(materialId, pageIndex, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取子批次列表
        /// </summary>
        /// <param name="requestId">申请码记录标识列</param>
        /// <returns>子批次列表</returns>
        public BaseResultList GetRequestCodeSettingListSub(long requestId)
        {
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            List<View_RequestCodeSetting> model = dal.GetRequestCodeSettingListSub(requestId);
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }

        /// <summary>
        /// 获取子批次列表
        /// </summary>
        /// <param name="requestId">申请码记录标识列</param>
        /// <returns>子批次列表</returns>
        public BaseResultList GetRequestCodeSettingListSubR(long requestId)
        {
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            List<View_RequestCodeSetting> model = dal.GetRequestCodeSettingListSubR(requestId);
            BaseResultList result = ToJson.NewListToJson(model, 1, model.Count, model.Count, "");
            return result;
        }

        /// <summary>
        /// 开通追溯/开通防伪修改类型
        /// </summary>
        /// <param name="id">码标识ID</param>
        /// <param name="eId">企业ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public BaseResultModel EditType(long id, long eId, int type)
        {
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            RetResult ret = dal.EditType(id, eId, type);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 选择追溯/防伪修改类型
        /// </summary>
        /// <param name="id">码标识ID</param>
        /// <param name="eId">企业ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public BaseResultModel EditTypeTwo(long id, long eId, long materialId, int type)
        {
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            RetResult ret = dal.EditTypeTwo(id, eId, materialId, type);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        #region 刘晓杰于2019年11月4日从CFBack项目移入此

        /// <summary>
        /// 获取接口需要的下载码记录
        /// </summary>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetInterFaceMaterialCode(string gentype, String WebURL, string stime, string etime, long enterpriseId)
        {
            //string baseUrl = ConfigurationManager.AppSettings["WebPath"].ToString();
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            if (0 == enterpriseId)
            {
                enterpriseId = 2;
            }
            List<View_InterFaceMaterialCode> list = dal.GetInterFaceMaterialCode(gentype, stime, etime, enterpriseId);
            List<InterFaceMaterial> rrl = new List<InterFaceMaterial>();
            foreach (var item in list)
            {
                // DataView dvRecord = new RequestCodeSettingAddBLL().getRecordByID(item.ID);
                if (!string.IsNullOrEmpty(item.WebURL))
                {
                    InterFaceMaterial rr = new InterFaceMaterial();
                    rr.CodeCount = Convert.ToInt32(item.codeCount);
                    rr.DownLoadUrl = WebURL + item.WebURL;
                    rr.EnterPriseID = (long)item.Enterprise_Info_ID;
                    rr.MaterialID = (long)item.Material_ID;
                    rr.FileID = (long)item.FileID;
                    rr.MaterialFullName = item.MaterialFullName;
                    rrl.Add(rr);
                }
            }
            BaseResultList result = ToJson.NewListToJson(rrl, 0, 0, (long)rrl.Count, "");
            return result;
        }

        #endregion

        /// <summary>
        /// 获取PI数据
        /// </summary>
        /// <returns></returns>
        public BaseResultModel GetPIInfo(string mainCode)
        {
            RequestCodeMaDAL dal = new RequestCodeMaDAL();
            RetResult ret = dal.GetPIInfo(mainCode, "WebConnect");
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
    }
}
