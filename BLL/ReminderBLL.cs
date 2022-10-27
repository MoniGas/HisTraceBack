/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于温馨提醒业务层

*********************************************************************************/
using System;
using Common.Argument;
using Dal;

namespace BLL
{
    public class ReminderBLL
    {
        /// <summary>
        /// 获取该企业是否有产品信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel GetMaterial(long id)
        {
            ReminderDAL objReminderDAL = new ReminderDAL();
            RetResult objResult = objReminderDAL.GetMaterial(id);
            return ToJson.NewRetResultToJson(Convert.ToInt32(objResult.IsSuccess).ToString(), objResult.Msg);
        }

        /// <summary>
        /// 该企业是否有生产环节
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel GetZuoYe(long id) 
        {
            ReminderDAL objReminderDAL = new ReminderDAL();

            RetResult objRetResult = objReminderDAL.GetZuoYe(id);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        /// <summary>
        /// 该企业下是否有品牌
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel GetBrand(long id) 
        {
            ReminderDAL objReminderDAL = new ReminderDAL();

            RetResult objRetResult = objReminderDAL.GetBrand(id);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        /// <summary>
        /// 该企业下是否有生产基地
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel GetGreenhouses(long id)
        {
            ReminderDAL objReminderDAL = new ReminderDAL();

            RetResult objRetResult = objReminderDAL.GetGreenhouses(id);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        /// <summary>
        /// 该企业下是否有经销商
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel GetDealer(long id)
        {
            ReminderDAL objReminderDAL = new ReminderDAL();

            RetResult objRetResult = objReminderDAL.GetDealer(id);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }
        /// <summary>
        /// 该企业码数量是否触发阀值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinqModel.BaseResultModel GetThresholdWarning(long id)
        {
            ReminderDAL objReminderDAL = new ReminderDAL();

            RetResult objRetResult = objReminderDAL.GetThresholdWarning(id);

            return ToJson.NewRetResultToJson(Convert.ToInt32(objRetResult.IsSuccess).ToString(), objRetResult.Msg);
        }

        public bool GetEnterpriseInfo(long id) 
        {
            EnterpriseInfoDAL objEnterpriseInfoDAL = new EnterpriseInfoDAL();

            LinqModel.View_EnterprisePlatForm data = objEnterpriseInfoDAL.GetModelView(id);

            if (data.Trade_ID == null)
            {
                return false;
            }

            if (data.Etrade_ID == null)
            {
                return false;
            }

            if (data.Logo == null || data.Logo.ToString().Trim().Equals(""))
            {
                return false;
            }

            if (data.zhizhao == null || data.zhizhao.Trim().Equals(""))
            {
                return false;
            }

            if (data.Address == null || data.Address.Trim().Equals(""))
            {
                return false;
            }

            if (data.WebURL == null || data.WebURL.Trim().Equals(""))
            {
                return false;
            }

            if (data.LinkPhone == null || data.LinkPhone.Trim().Equals(""))
            {
                return false;
            }

            if (data.Email == null || data.Email.Trim().Equals(""))
            {
                return false;
            }

            if (data.Memo == null || data.Memo.Trim().Equals(""))
            {
                return false;
            }
            return true;
        }
    }
}
