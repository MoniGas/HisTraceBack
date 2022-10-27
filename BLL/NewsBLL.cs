/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于消息查询业务层

*********************************************************************************/
using System;
using System.Collections.Generic;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class NewsBLL
    {
        /// <summary>
        /// 获取申请二维码的结果消息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultList GetEMWList(long id) 
        {
            NewsDAL objNewsDAL = new NewsDAL();
            int dataCount = 0;
            List<LinqModel.View_RequestCodeAndEnterprise_Info> dataList = objNewsDAL.GetEMWList(id, out dataCount);

            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataCount, "");
        }

        /// <summary>
        /// 修改申请二维码后的消息为已读
        /// </summary>
        /// <param name="id">申请二维码消息ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel UpdateStatus(long id)
        {
            NewsDAL objNewsDAL = new NewsDAL();

            RetResult objResult = objNewsDAL.UpdateStatus(id);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objResult.IsSuccess).ToString(), objResult.Msg);
        }

        public LinqModel.BaseResultModel IgnoreEwm() 
        {
            NewsDAL objNewsDAL = new NewsDAL();

            RetResult objResult = objNewsDAL.IgnoreEwm();

            return ToJson.NewRetResultToJson(Convert.ToInt32(objResult.IsSuccess).ToString(), objResult.Msg);
        }

        /// <summary>
        /// 获取申请加入区域品牌消息列表
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultList GetBrandList(long id)
        {
            NewsDAL objNewsDAL = new NewsDAL();
            int dataCount = 0;
            List<LinqModel.View_RequestBrand> dataList = objNewsDAL.GetBrandList(id, out dataCount);

            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataCount, "");
        }

        /// <summary>
        /// 修改申请加入区域品牌结果为已读
        /// </summary>
        /// <param name="id">申请加入区域品牌ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel UpdateBrand(long id) 
        {
            NewsDAL objNewsDAL = new NewsDAL();

            RetResult objResult = objNewsDAL.UpdateBrand(id);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objResult.IsSuccess).ToString(), objResult.Msg);
        }

        public LinqModel.BaseResultModel IgnoreBrand() 
        {
            NewsDAL objNewsDAL = new NewsDAL();

            RetResult objResult = objNewsDAL.IgnoreBrand();

            return ToJson.NewRetResultToJson(Convert.ToInt32(objResult.IsSuccess).ToString(), objResult.Msg);
        }

        public LinqModel.BaseResultList GetEnterpriseVerifyList(long id)
        {
            NewsDAL objNewsDAL = new NewsDAL();
            int dataCount = 0;
            List<EnterpriseVerify> dataList = objNewsDAL.GetEnterpriseVerifyList(id, out dataCount);

            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataCount, "");
        }

        public BaseResultList GetCodeRecord(long enterpriseID)
        {
            NewsDAL objNewsDAL = new NewsDAL();
            List<EnterpriseVerify> dataList = objNewsDAL.GetCodeRecord(enterpriseID);
            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataList.Count, "");
        }
    }
}
