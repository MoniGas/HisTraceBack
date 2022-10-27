/********************************************************************************
** 作者： 赵慧敏
** 创始时间：2019-5-28
** 联系方式 :13313318725
** 描述：黑名单
*********************************************************************************/
using System;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;

namespace Dal
{
    public  class BackListDAL:DALBase
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="eId"></param>
        /// <returns></returns>
        public BackList GetModel(long eId)
        {
            BackList model = new BackList();
            using (DataClassesDataContext DataContext = GetDataContext())
            {
                try
                {
                    model = (from m in DataContext.BackList where m.EnterpriseId == eId select m).FirstOrDefault();
                    ClearLinqModel(model);
                }
                catch (Exception Ex)
                {
                    string ErrData = "BackListDAL.GetModel()";
                    WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
                }
            }
            return model;
        }
        public RetResult Edit(BackList newModel)
        {
            Ret.Msg = "操作失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.BackList.FirstOrDefault(p => p.EnterpriseId == newModel.EnterpriseId);
                    if (model == null)
                    {
                        dataContext.BackList.InsertOnSubmit(newModel);
                        dataContext.SubmitChanges();
                        Ret.Msg = "操作成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                    else
                    {
                        model.BackCode = newModel.BackCode;
                        model.BackImg = newModel.BackImg;
                        dataContext.SubmitChanges();
                        Ret.Msg = "操作成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "MaterialDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
    }
}
