/********************************************************************************
** 作者：张翠霞
** 开发时间：2018-9-3
** 联系方式:13313318725
** 代码功能：企业微信公众号设置数据访问层
** 版本：v1.0
** 版权：项目二部
*********************************************************************************/
using System;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;

namespace Dal
{
    public class WxGZHDAL : DALBase
    {
        /// <summary>
        /// 设置企业微信公众号信息
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public RetResult AddEditEnWxGZH(EnterpriseWxGZH model)
        {
            RetResult result = new RetResult();
            result.CmdError = CmdResultError.EXCEPTION;
            result.Msg = "保存失败！";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    EnterpriseWxGZH wxmodel = dataContext.EnterpriseWxGZH.FirstOrDefault(m => m.Enterprise_Info_ID == model.Enterprise_Info_ID);
                    if (wxmodel == null)
                    {
                        dataContext.EnterpriseWxGZH.InsertOnSubmit(model);
                    }
                    else
                    {
                        wxmodel.AddDate = DateTime.Now;
                        wxmodel.WxAppId = model.WxAppId;
                        wxmodel.AppSecret = model.AppSecret;
                        wxmodel.Enterprise_Info_ID = model.Enterprise_Info_ID;
                        wxmodel.Status = model.Status;
                    }
                    dataContext.SubmitChanges();
                    Ret.Msg = "保存成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "WxGZHDAL.AddEditEnWxGZH()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 获取微信公众号信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public EnterpriseWxGZH GetModel(long eId)
        {
            EnterpriseWxGZH result = new EnterpriseWxGZH();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.EnterpriseWxGZH.FirstOrDefault(m => m.Enterprise_Info_ID == eId);
                }
                catch (Exception ex)
                {
                    string errData = "WxGZHDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
    }
}
