/********************************************************************************
** 作者： 张翠霞
** 创始时间：2018-06-19
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：
** 修改时间：
** 描述：
**  主要用于数据采集管理 
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    public class CollectCodeBLL
    {
        private readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取采集记录列表
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="status">状态（下载/未下载）</param>
        /// <param name="beginDate">采集开始时间</param>
        /// <param name="endDate">采集结束时间</param>
        /// <param name="collectUser">采集人员</param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public BaseResultList GetList(long eId, int status, string beginDate, string endDate, string collectUser, int pageIndex)
        {
            long totalCount;
            CollectCodeDAL dal = new CollectCodeDAL();
            List<CollectCodeTable> model = dal.GetList(eId, status, beginDate, endDate, collectUser, pageIndex, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, _pageSize, totalCount, "");
        }

        /// <summary>
        /// 采集记录详情
        /// </summary>
        /// <param name="sId">标识ID</param>
        /// <param name="eid">企业ID</param>
        /// <param name="pageIndex">分页</param>
        /// <returns></returns>
        public BaseResultList CollectCodeInfo(long sId, long eid, int pageIndex)
        {
            long totalCount;
            CollectCodeDAL dal = new CollectCodeDAL();
            List<CollectCodeDetail> model = dal.CollectCodeInfo(sId, eid, pageIndex, _pageSize, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, _pageSize, totalCount, "");
        }

        /// <summary>
        /// 下载txt文件数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="sId">设置表ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<CollectCodeDetail> GetCollectCodeTxt(long sId, long eid)
        {
            CollectCodeDAL dal = new CollectCodeDAL();
            return dal.GetCollectCodeTxt(sId, eid);
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="sId">标识ID</param>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public BaseResultModel UpdateStatus(string sId, long eid)
        {
            if (string.IsNullOrEmpty(sId))
            {
                return ToJson.NewRetResultToJson("0", "数据错误！");
            }
            CollectCodeDAL dal = new CollectCodeDAL();
            RetResult ret = dal.UpdateStatus(Convert.ToInt64(sId),eid);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }
    }
}
