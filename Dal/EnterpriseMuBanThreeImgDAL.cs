/********************************************************************************
** 作者： 张翠霞
** 创始时间：2018-9-7
** 联系方式 :13313318725
** 描述：企业拍码模板3的图片信息数据访问
** 版本：v1.0
** 版权：追溯项目组
*********************************************************************************/
using System;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    public class EnterpriseMuBanThreeImgDAL : DALBase
    {
        /// <summary>
        /// 获取企业拍码模板3的图片信息
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <returns></returns>
        public EnterpriseMuBanThreeImg GetModel(long eId)
        {
            LoginInfo User = SessCokie.Get;
            EnterpriseMuBanThreeImg model = new EnterpriseMuBanThreeImg();
            using (DataClassesDataContext DataContext = GetDataContext())
            {
                try
                {
                    model = (from m in DataContext.EnterpriseMuBanThreeImg where m.EnterpriseID == eId select m).FirstOrDefault();
                    if (model == null)
                    {
                        EnterpriseMuBanThreeImg temp = new EnterpriseMuBanThreeImg();
                        temp.EnterpriseID = User.EnterpriseID;
                        temp.AddUserID = User.UserID;
                        temp.AddDate = DateTime.Now;
                        DataContext.EnterpriseMuBanThreeImg.InsertOnSubmit(temp);
                        DataContext.SubmitChanges();
                        model = temp;
                    }
                    ClearLinqModel(model);
                }
                catch (Exception Ex)
                {
                    string ErrData = "EnterpriseMuBanThreeImgDAL.GetModel()";
                    WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
                }
            }
            return model;
        }

        /// <summary>
        /// 提交/修改图片
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RetResult Edit(EnterpriseMuBanThreeImg Model)
        {
            Ret.Msg = "修改失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext DataContext = GetDataContext())
            {
                try
                {
                    EnterpriseMuBanThreeImg Info = DataContext.EnterpriseMuBanThreeImg.FirstOrDefault(m => m.EnterpriseID == Model.EnterpriseID);
                    //判断企业信息查询结果是否为空
                    if (Info == null)
                    {
                        DataContext.EnterpriseMuBanThreeImg.InsertOnSubmit(Model);
                    }
                    else
                    {
                        Info.FirstImg = Model.FirstImg;
                        Info.CenterImg = Model.CenterImg;
                        Info.FiveImg = Model.FiveImg;
                    }
                    DataContext.SubmitChanges();
                    Ret.Msg = "保存成功！";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception Ex)
                {
                    string ErrData = "ShowCompanyDAL.Edit()";
                    WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
                }
            }
            return Ret;
        }
    }
}
