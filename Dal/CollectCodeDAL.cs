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
using System.Linq;
using LinqModel;
using Common.Argument;

namespace Dal
{
    public class CollectCodeDAL : DALBase
    {
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
        public List<CollectCodeTable> GetList(long eId, int status, string beginDate, string endDate, string collectUser, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<CollectCodeTable> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.CollectCodeTable
                               where m.EnterpriseId == eId &&
                                   m.Status == status
                               select m;
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        data = data.Where(m => m.CollectTime >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        data = data.Where(m => m.CollectTime <= Convert.ToDateTime(endDate));
                    }
                    if (!string.IsNullOrEmpty(collectUser))
                    {
                        data = data.Where(m => m.CollectUserName.Contains(collectUser.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.CollectId).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获取采集记录详情
        /// </summary>
        /// <param name="sId">标识ID</param>
        /// <param name="eid">企业ID</param>
        /// <param name="pageIndex">分页</param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<CollectCodeDetail> CollectCodeInfo(long sId, long eid, int pageIndex, int pageSize, out long totalCount)
        {
            List<CollectCodeDetail> result = new List<CollectCodeDetail>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.CollectCodeDetail
                               where m.EnterpriseId == eid &&
                                   m.CollectCodeId == sId
                               select m;
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.CodeInfoID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获取下载txt文档的内容同时更新状态
        /// </summary>
        /// <param name="sId">标识ID</param>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public List<CollectCodeDetail> GetCollectCodeTxt(long sId, long eid)
        {
            List<CollectCodeDetail> result = new List<CollectCodeDetail>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.CollectCodeDetail
                               where m.EnterpriseId == eid &&
                                   m.CollectCodeId == sId
                               select m;
                    result = data.OrderByDescending(m => m.CodeInfoID).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="sId">标识ID</param>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public RetResult UpdateStatus(long sId, long eid)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var modelcode = dataContext.CollectCodeTable.FirstOrDefault(m => m.EnterpriseId == eid && m.CollectId == sId);
                    modelcode.Status = (int)Common.EnumFile.DowloadStatus.dowLoad;
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, "更新成功！", "更新成功！");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, "更新失败！", "更新失败！");
            }
            return Ret;
        }
    }
}
