/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-11-29

** 修改时间：

** 联系方式 :13313318725

** 描述：企业生成码统计业务层

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using LinqModel;
using Dal;
using Common.Argument;
using System.Configuration;

namespace BLL
{
    /// <summary>
    /// 企业生成码统计业务层
    /// </summary>
    public class ViewCodeStatisBLL
    {
        //页码
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        ///  获取二维码数据
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="matype">产品类别</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public BaseResultList GetEnCodeList(long eid, int pageIndex, int mlx, string materialName, string beginDate, string endDate)
        {
            long totalCount = 0;
            View_CodeStatis objModel = new View_CodeStatis();
            ViewCodeStatisDAL dal = new ViewCodeStatisDAL();
            List<View_CodeStatis> model = dal.GetEnCodeList(eid, out totalCount, out objModel, pageIndex, mlx, materialName, beginDate, endDate);
            if (model != null)
            {
                foreach (var item in model)
                {
                    string ShuLiang = item.Value.ToString();
                    string danWei = item.GuiGe ?? "";
                    item.GuiGe = ShuLiang + danWei;
                }
            }
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, objModel, "");
            return result;
        }
        /// <summary>
        /// 获取企业买码记录
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public BaseResultList GetContinneCodeRecord(long eid, int pageIndex, string beginDate, string endDate)
        {
            long totalCount = 0;
            View_ContinueCode objModel = new View_ContinueCode();
            ViewCodeStatisDAL dal = new ViewCodeStatisDAL();
            List<View_ContinueCode> model = dal.GetContinneCodeRecord(eid, out totalCount, out objModel, pageIndex, beginDate, endDate);

            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, objModel, "");
            return result;
        }
        /// <summary>
        /// 设置阀值
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public RetResult SetThreshold(long eid, int count)
        {
            ViewCodeStatisDAL dal = new ViewCodeStatisDAL();
            RetResult result = dal.SetThreshold(eid, count);
            return result;
        }
        /// <summary>
        /// 获取阀值
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public BaseResultList GetThreshold(long eid)
        {
            ViewCodeStatisDAL dal = new ViewCodeStatisDAL();
            List<Enterprise_Info> model = new List<Enterprise_Info>();
            Enterprise_Info objModel=dal.GetThreshold(eid);
            BaseResultList result = ToJson.NewListToJson(model, 1, _PageSize, 0, objModel, "");
            return result;
        }
    }
}
