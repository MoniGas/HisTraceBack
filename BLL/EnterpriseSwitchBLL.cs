/********************************************************************************
** 作者： 李子巍
** 创始时间：2017-03-15
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：赵慧敏
** 修改时间：2017-02-10
** 描述：
**  主要用于开通推广管理业务层 
*********************************************************************************/
using System;
using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 推广业务层
    /// </summary>
    public class EnterpriseSwitchBLL
    {
        EnterpriseSwitchDAL dal = new EnterpriseSwitchDAL();
        /// <summary>
        /// 获取企业是否开通服务
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="switchCode">服务代码</param>
        /// <returns>开通返回实体、未开通返回null</returns>
        public BaseResultModel GetIsOn(long enterpriseId, int switchCode)
        {
            View_Enterprise_Switch dataModel =dal.GetIsOn(enterpriseId, switchCode);
            return ToJson.NewModelToJson(dataModel, "1", "");
        }

        /// <summary>
        /// 获取企业是否开通服务
        /// </summary>
        /// <param name="switchCode">服务代码</param>
        /// <returns></returns>
        public bool GetIsOff(long enterpriseId,int switchCode)
        {
            bool isOn = false;
            try
            {
                EnterpriseSwitchBLL bll = new EnterpriseSwitchBLL();
                var result = bll.GetIsOn(enterpriseId, switchCode);
                if (result.ObjModel != null)
                {
                    isOn = true;
                }
            }
            catch (Exception ex)
            {
            }
            return isOn;
        }
        
        /// <summary>
        /// 开通、关闭服务
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="switchCode">开通服务代码</param>
        /// <param name="type">1开通 其他值均为关闭</param>
        /// <returns>操作结果</returns>
        public BaseResultModel TrunSwitch(long enterpriseId, int switchCode, int type)
        {
            RetResult result =dal.TrunSwitch(enterpriseId, switchCode, type);
            return ToJson.NewRetResultToJson((Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }

        /// <summary>
        /// 获取企业是否激活
        /// </summary>
        /// <returns></returns>
        public BaseResultModel GetIsActive(long enterpriseId)
        {
            BaseResultModel result = new BaseResultModel();
            Enterprise_Info enterpriseInfo = dal.GetEnterpriseInfo(enterpriseId);
            if (null!=enterpriseInfo)
            {
                if (enterpriseInfo.IsActive>0)
                {
                    result.code = "-1";
                    result.Msg = "未激活";
                    result.ObjModel = enterpriseInfo;
                    return result;
                }
                result.code = "1";
                result.Msg = "企业已激活";
                return result;
            }
            result.code = "-1";
            result.Msg = "获取数据异常";
            return result;
        }

        public BaseResultModel SwitchActive(long enterpriseId)
        {
            RetResult result = dal.SwitchActive(enterpriseId);
            return ToJson.NewRetResultToJson((Convert.ToInt32(result.IsSuccess)).ToString(), result.Msg);
        }
    }
}
