using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;
using LinqModel;
using Common.Argument;
using Common.Log;
using Common.Tools;

namespace Dal
{
    public class SysJYEnterpriseDAL : DALBase
    {
        public PagedList<View_DealerUser> GetJYEnterpriseList(string dName, int? pageIndex, out long totalCount)
        {
            totalCount = 0;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.View_DealerUser
                                   where data.DealerLevel == 101
                                   select data;
                    if (!string.IsNullOrEmpty(dName))
                    {
                        dataList = dataList.Where(m => m.DealerName.Contains(dName));
                    }
                    totalCount = dataList.Count();
                    dataList = dataList.OrderByDescending(m => m.Dealer_ID);
                    return dataList.ToPagedList(pageIndex ?? 1, PageSize);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public RetResult Add(Dealer dealer, Enterprise_User dealerUser)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    Dealer de = dataContext.Dealer.Where(p => p.DealerName == dealer.DealerName && p.DealerLevel == 101).FirstOrDefault();
                    if (de != null)
                    {
                        Msg = "经营企业名称已存在！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                    dataContext.Dealer.InsertOnSubmit(dealer);
                    dataContext.SubmitChanges();
                    Enterprise_User deUser = dataContext.Enterprise_User.Where(p => p.LoginName == dealerUser.LoginName).FirstOrDefault();
                    if (de != null)
                    {
                        Msg = "该登录名已存在！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
                    else
                    {
                        dealerUser.DealerID = dealer.Dealer_ID;
                        dataContext.Enterprise_User.InsertOnSubmit(dealerUser);
                        dataContext.SubmitChanges();
                    }
                    Msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public View_DealerUser GetModelInfo(long dId)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.View_DealerUser
                                where d.Dealer_ID == dId
                                select d).FirstOrDefault();
                    ClearLinqModel(data);
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public RetResult Edit(Dealer dealer, string loginName, string pwd)
        {
            string Msg = "修改失败！";
            CmdResultError error = CmdResultError.EXCEPTION;

            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.Dealer
                                where d.Dealer_ID == dealer.Dealer_ID
                                select d).FirstOrDefault();
                    if (data != null)
                    {
                        Dealer temp = dataContext.Dealer.FirstOrDefault(p => p.DealerName == dealer.DealerName && p.DealerLevel == 101 && p.Dealer_ID != dealer.Dealer_ID);
                        if (temp != null)
                        {
                            Msg = "经营企业名称已存在！";
                            error = CmdResultError.NONE;
                            Ret.SetArgument(error, Msg, Msg);
                            return Ret;
                        }
                        data.DealerName = dealer.DealerName;
                        data.Address = dealer.Address;
                        data.Person = dealer.Person;
                        data.Phone = dealer.Phone;
                        data.DICount = dealer.DICount;
                        Enterprise_User deUser = dataContext.Enterprise_User.FirstOrDefault(p => p.DealerID == dealer.Dealer_ID);
                        if (deUser != null)
                        {
                            View_DealerUser tempUser = dataContext.View_DealerUser.FirstOrDefault(p => p.LoginName == loginName && p.DealerLevel == 101 && p.Dealer_ID != dealer.Dealer_ID);
                            if (tempUser != null)
                            {
                                Msg = "该登录名已存在！";
                                error = CmdResultError.NONE;
                                Ret.SetArgument(error, Msg, Msg);
                                return Ret;
                            }
                            else
                            {
                                deUser.LoginName = loginName;
                                deUser.LoginPassWord = pwd;
                            }
                        }
                        else
                        {
                            Msg = "未找到该经营企业登录信息！";
                            error = CmdResultError.EXCEPTION;
                            Ret.SetArgument(error, Msg, Msg);
                            return Ret;
                        }
                        dataContext.SubmitChanges();
                        Msg = "修改成功！";
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "数据不存在！";
                        error = CmdResultError.EXCEPTION;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }

                }
            }
            catch (Exception ex)
            {
                Msg = "发生异常错误";
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public RetResult EditJYEnStatus(long id, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Dealer model = dataContext.Dealer.FirstOrDefault(m => m.Dealer_ID == id);
                    if (model == null)
                    {
                        Ret.SetArgument(CmdResultError.Other, null, "获取信息失败");
                        return Ret;
                    }
                    if (type == 1)
                    {
                        model.Status = (int)Common.EnumFile.Status.used;
                    }
                    else
                    {
                        model.Status = (int)Common.EnumFile.Status.delete;
                    }
                    dataContext.SubmitChanges();
                    Ret.SetArgument(CmdResultError.NONE, null, "操作成功");
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, null, "操作失败");
            }
            return Ret;
        }

        #region 经营企业版接口
        public BaseResultModel JYEnLogin(string lName, string pwd)
        {
            BaseResultModel userLogin = new BaseResultModel();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.View_DealerUser
                                where d.LoginName == lName && d.LoginPassWord == pwd
                                select d).FirstOrDefault();
                    if (data != null)
                    {
                        userLogin.ObjModel = data;
                        userLogin.code = "1";
                        userLogin.Msg = "登录成功！";
                    }
                    else
                    {
                        data = dataContext.View_DealerUser.FirstOrDefault(m => m.LoginName == lName);
                        if (data == null)
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，用户名错误！";
                        }
                        else
                        {
                            userLogin.code = "0";
                            userLogin.Msg = "登录失败，密码错误！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                userLogin.code = "0";
                userLogin.Msg = "登录失败，出现异常！";
                string errData = "SysJYEnterpriseDAL.JYEnLogin():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return userLogin;
        }

        public RetResult AddJYMaterial(JYMaterial model, JYMaterialDI modelDI)
        {
            Ret.Msg = "保存信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    JYMaterial temp = dataContext.JYMaterial.FirstOrDefault(m => m.DealerID == model.DealerID &&
                        m.MaterialName == model.MaterialName && m.CategoryCode == model.CategoryCode && m.Status == (int)Common.EnumFile.Status.used);
                    if (temp == null)
                    {
                        dataContext.JYMaterial.InsertOnSubmit(model);
                        dataContext.SubmitChanges();
                    }
                    //加DI
                    if (temp == null)
                    {
                        modelDI.JYMaterialID = model.JYMaterialID;
                    }
                    else
                    {
                        modelDI.JYMaterialID = temp.JYMaterialID;
                    }
                    JYMaterialDI tempDI = dataContext.JYMaterialDI.FirstOrDefault(m => m.DealerID == model.DealerID &&
                        m.MaterialUDIDI == modelDI.MaterialUDIDI);
                    if (tempDI == null)
                    {
                        dataContext.JYMaterialDI.InsertOnSubmit(modelDI);
                        dataContext.SubmitChanges();
                    }
                    Ret.Msg = "保存信息成功";
                    Ret.CmdError = CmdResultError.NONE;
                }
                catch (Exception ex)
                {
                    string errData = "SysJYEnterpriseDAL.AddJYMaterial()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 经营企业打码客户端获取DI信息
        /// </summary>
        /// <param name="context"></param>
        public List<JYMaterialDI> GetJYDIList(long dealerId, string materialName, string categoryCode)
        {
            List<JYMaterialDI> list = new List<JYMaterialDI>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    list = dataContext.JYMaterialDI.Where(m => m.DealerID == dealerId).ToList();
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        list = list.Where(m => m.MaterialName.Contains(materialName)).ToList();
                    }
                    if (!string.IsNullOrEmpty(categoryCode))
                    {
                        list = list.Where(m => m.CategoryCode.Contains(categoryCode)).ToList();
                    }
                    ClearLinqModel(list);
                }
                catch (Exception ex)
                {
                    string errData = "经营企业打码客户端获取同步DI数据";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return list;
        }

        public RetResult AddJYPIInfo(JYRequestCode rModel, string materialName)
        {
            Ret.Msg = "添加失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    long? diCount = 0;
                    long? DdiCount = 0;
                    Dealer tempD = dataContext.Dealer.FirstOrDefault(m => m.Dealer_ID == rModel.DealerID);
                    if (tempD != null)
                    {
                        DdiCount = tempD.DICount == null ? 0 : tempD.DICount;
                    }
                    List<JYRequestCode> tempRe = dataContext.JYRequestCode.Where(m => m.FixedCode == rModel.FixedCode).ToList();
                    if (tempRe.Count > 0)
                    {
                        diCount = tempRe.Sum(m => m.TotalNum);
                        if (diCount + rModel.TotalNum > DdiCount)
                        {
                            Ret.Msg = "该产品生成码数量已超过设置数量，最多还能生成" + (DdiCount - diCount) + "个码！";
                            Ret.CmdError = CmdResultError.EXCEPTION;
                            Ret.Code = -1;
                            Ret.id = 0;
                            return Ret;
                        }
                    }
                    JYMaterial ma = dataContext.JYMaterial.FirstOrDefault(m => m.DealerID == rModel.DealerID && m.MaterialName == materialName);
                    if (ma != null)
                    {
                        rModel.JYMaterialID = ma.JYMaterialID;
                    }
                    else
                    {
                        Ret.Msg = "没有找到产品ID！";
                        Ret.CmdError = CmdResultError.EXCEPTION;
                        Ret.Code = -1;
                        Ret.id = 0;
                        return Ret;
                    }
                    if (rModel.CodingClientType == 1)
                    {
                        JYMaterialDI maDI = dataContext.JYMaterialDI.FirstOrDefault(m => m.DealerID == rModel.DealerID && m.MaterialUDIDI == rModel.FixedCode);
                        if (maDI != null)
                        {
                            rModel.MaterialXH = maDI.MaterialXH;
                        }
                        else
                        {
                            Ret.Msg = "没有找到产品DI信息！";
                            Ret.CmdError = CmdResultError.EXCEPTION;
                            Ret.Code = -2;
                            Ret.id = 0;
                            return Ret;
                        }
                    }
                    dataContext.JYRequestCode.InsertOnSubmit(rModel);
                    dataContext.SubmitChanges();
                    Ret.Msg = "添加成功！";
                    Ret.CmdError = CmdResultError.NONE;
                    Ret.Code = 1;
                    Ret.id = rModel.JYRequestCodeID;
                }
                catch (Exception ex)
                {
                    Ret.Msg = "异常！";
                    Ret.CmdError = CmdResultError.EXCEPTION;
                    Ret.Code = 0;
                    Ret.id = 0;
                    string errData = "SysJYEnterpriseDAL.AddJYPIInfo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        public List<JYRequestCode> GetJYPIList(long dealerId, string starDate, string endDate)
        {
            List<JYRequestCode> list = new List<JYRequestCode>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    list = dataContext.JYRequestCode.Where(m => m.DealerID == dealerId).ToList();
                    if (!string.IsNullOrEmpty(starDate))
                    {
                        list = list.Where(m => m.adddate >= Convert.ToDateTime(starDate)).ToList();
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        list = list.Where(m => m.adddate <= Convert.ToDateTime(endDate).AddDays(1)).ToList();
                    }
                    ClearLinqModel(list);
                }
                catch (Exception ex)
                {
                    string errData = "经营企业打码客户端获取同步PI数据";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return list;
        }
        #endregion
    }
}
