/********************************************************************************
** 作者： 李子巍
** 创始时间：2017-03-15
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：赵慧敏
** 修改时间：2017-02-10
** 描述：
**  主要用于开通推广管理数据层 
*********************************************************************************/
using System;
using System.Linq;
using Common;
using LinqModel;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 推广服务
    /// </summary>
    public class EnterpriseSwitchDAL : DALBase
    {
        /// <summary>
        /// 获取企业是否开通服务
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="switchCode">服务代码</param>
        /// <returns>开通返回实体、未开通返回null</returns>
        public View_Enterprise_Switch GetIsOn(long enterpriseId, int switchCode)
        {
            View_Enterprise_Switch result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.View_Enterprise_Switch.FirstOrDefault(m => m.EnterpriseID == enterpriseId
                        && m.SwitchCode == switchCode.ToString().PadLeft(3, '0'));
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 开通、关闭服务
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="switchCode">开通服务代码</param>
        /// <param name="type">1开通 其他值均为关闭</param>
        /// <returns>操作结果</returns>
        public RetResult TrunSwitch(long enterpriseId, int switchCode, int type)
        {
            string msg = type == 1 ? "开通" : "关闭" + "失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Switch model = dataContext.Enterprise_Switch.FirstOrDefault(
                        m => m.EnterpriseID == enterpriseId
                        && m.SwitchCode == switchCode.ToString().PadLeft(3, '0'));
                    if (type == 1)
                    {
                        if (model == null)
                        {
                            Enterprise_Switch addModel = new Enterprise_Switch();
                            addModel.AddTime = DateTime.Now;
                            addModel.EnterpriseID = enterpriseId;
                            addModel.SwitchCode = switchCode.ToString().PadLeft(3, '0');
                            dataContext.Enterprise_Switch.InsertOnSubmit(addModel);
                        }
                        else
                        {
                            model.AddTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        dataContext.Enterprise_Switch.DeleteOnSubmit(model);
                        switch (switchCode)
                        {
                            case (int)EnumFile.EnterpriseSwitch.Recommend:
                                dataContext.Recommend.DeleteAllOnSubmit(
                                    dataContext.Recommend.Where(m => m.EnterpriseID == enterpriseId)
                                );
                                break;
                        }
                    }
                    dataContext.SubmitChanges();
                    msg = (type == 1 ? "开通" : "关闭") + "成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public Enterprise_Info GetEnterpriseInfo(long enterpriseId)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)EnumFile.Status.used);
            }
        }

        /// <summary>
        /// 企业激活开关
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public RetResult SwitchActive(long enterpriseId)
        {
            string msg = "激活";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info model = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)EnumFile.Status.used);
                    if (null != model)
                    {
                        if (model.IsActive > 0)
                        {
                            model.IsActive = (int)EnumFile.ActiveStatus.关闭;
                        }
                        else
                        {
                            model.IsActive = (int)EnumFile.ActiveStatus.激活;
                        }
                        dataContext.SubmitChanges();
                        msg = (model.IsActive > 0 ? "关闭" : "激活") + "成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
                msg = e.Message;
            }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }
    }
}
