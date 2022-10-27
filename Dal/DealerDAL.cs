/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-03

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于经销商信息管理数据层

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;
using System.Text;
using LinqModel.InterfaceModels;
using System.Configuration;

namespace Dal
{
    public class DealerDAL : DALBase
    {
        /// <summary>
        /// 获取经销商模型
        /// </summary>
        /// <param name="dealerId">经销商标识</param>
        /// <returns>实体</returns>
        public Dealer GetModel(long dealerId)
        {
            Dealer result = new Dealer();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Dealer.FirstOrDefault(m => m.Dealer_ID == dealerId && m.Status == (int)Common.EnumFile.Status.used);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "DealerDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取经销商列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="dealerName">经销商名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>操作结果</returns>
        public List<Dealer> GetList(long enterpriseId, string dealerName, int pageIndex, out long totalCount)
        {
            List<Dealer> result = new List<Dealer>();
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.Dealer.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(dealerName))
                    {
                        tempResult = tempResult.Where(m => m.DealerName.Contains(dealerName.Trim()) || m.Address.Contains(dealerName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.Dealer_ID).Skip(PageSize * (pageIndex - 1)).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "DealerDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加经销商
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Add(Dealer model, string loginName, string pwd)
        {
            Ret.Msg = "添加经销商信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempModel = dataContext.Dealer.FirstOrDefault(p =>p.DealerCode==model.DealerCode
                        && p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                    if (tempModel != null)
                    {
                        model.Dealer_ID = tempModel.Dealer_ID;
                        Ret= this.Edit(model);
                        return Ret;
                    }
                    else
                    {
                        tempModel = dataContext.Dealer.FirstOrDefault(p => p.DealerCode != model.DealerCode && p.DealerName==model.DealerName
                        && p.Enterprise_Info_ID == model.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                        if (tempModel!=null)
                        {
                            Ret.Msg = "经销商名称重复！";
                            return Ret;
                        }
                        model.DealerType = (int)Common.EnumFile.DealerType.First;
                        dataContext.Dealer.InsertOnSubmit(model);
                        if (string.IsNullOrEmpty(loginName))
                        {
                            loginName = model.DealerName + DateTime.Now.ToString("yyMMdd");
                        }
                        if (string.IsNullOrEmpty(pwd))
                        {
                            pwd = "123456";
                        }
                        Enterprise_User user = dataContext.Enterprise_User.Where(p => p.LoginName == loginName && p.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
                        if (user == null)
                        {
                            dataContext.SubmitChanges();
                            user = new Enterprise_User();
                            user.Enterprise_Info_ID = model.Enterprise_Info_ID;
                            user.Status = (int)Common.EnumFile.Status.used;
                            user.LoginName = loginName;
                            user.LoginPassWord = pwd;
                            user.UserName = model.DealerName;
                            user.UserPhone = model.Phone;
                            user.UserAddress = model.Address;
                            user.UserType = "经销商";
                            user.adduser = model.adduser;
                            user.adddate = DateTime.Now;
                            user.DealerID = model.Dealer_ID;
                            user.Enterprise_Role_ID = 5;//经销商权限
                            dataContext.Enterprise_User.InsertOnSubmit(user);
                            dataContext.SubmitChanges();
                            Ret.Msg = "添加经销商信息成功";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "登录名重复！";
                        }
                    }
                }
                catch { }
            }
            return Ret;
        }

        /// <summary>
        /// 修改经销商
        /// </summary>
        /// <param name="newModel">实体</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Dealer newModel)
        {
            Ret.Msg = "修改经销商信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Dealer.FirstOrDefault(p => p.Dealer_ID == newModel.Dealer_ID && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的经销商！";
                    }
                    else
                    {
                        var a = dataContext.Dealer.FirstOrDefault(p => p.DealerName == newModel.DealerName && p.Dealer_ID != newModel.Dealer_ID && p.Enterprise_Info_ID == newModel.Enterprise_Info_ID && p.Status == (int)Common.EnumFile.Status.used);
                        if (a == null)
                        {
                            model.DealerName = newModel.DealerName;
                            model.Dictionary_AddressQu_ID = newModel.Dictionary_AddressQu_ID;
                            model.Dictionary_AddressSheng_ID = newModel.Dictionary_AddressSheng_ID;
                            model.Dictionary_AddressShi_ID = newModel.Dictionary_AddressShi_ID;
                            model.Person = newModel.Person;
                            model.Phone = newModel.Phone;
                            model.Address = newModel.Address;
                            model.lastdate = newModel.lastdate;
                            model.lastuser = newModel.lastuser;
                            model.location = newModel.location;
                            dataContext.SubmitChanges();
                            Ret.Msg = "修改经销商信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                        else
                        {
                            Ret.Msg = "已存在该经销商名称！";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "DealerDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }

        /// <summary>
        /// 删除经销商
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="dealerId">经销商标识</param>
        /// <returns>操作结果</returns>
        public RetResult Del(long enterpriseId, long dealerId)
        {
            Ret.Msg = "删除经销商信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Dealer.FirstOrDefault(p => p.Dealer_ID == dealerId && p.Enterprise_Info_ID == enterpriseId);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要删除的经销商！";
                    }
                    else
                    {
                        if (model.UseCount.GetValueOrDefault(0) > 0)
                        {
                            Ret.Msg = "该经销商已被使用，目前无法删除！";
                        }
                        else
                        {
                            model.Status = (int)Common.EnumFile.Status.delete;
                            dataContext.SubmitChanges();
                            Ret.Msg = "删除经销商信息成功！";
                            Ret.CmdError = CmdResultError.NONE;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "DealerDAL.Del()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;


        }

        /// <summary>
        /// 根据企业ID获取经销商信息
        /// 陈志钢 WinCE
        /// </summary>
        /// <param name="enterpriseId"> 企业ID</param>
        /// <returns></returns>
        public List<Dealer> GetSelectList(long enterpriseId)
        {
            List<Dealer> result = new List<Dealer>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Dealer.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "DealerDAL.GetSelectList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <returns></returns>
        public List<Material> GetMaterialList(long enterpriseId)
        {
            List<Material> result = new List<Material>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "DealerDAL.GetMaterialList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 导入Excel插入经销商信息
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public RetResult AddExcelDealer(DealerExcelRecord model)
        {
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.DealerExcelRecord.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.SetArgument(Common.Argument.CmdResultError.NONE, null, "导入成功");
                }
                catch
                {
                    Ret.SetArgument(Common.Argument.CmdResultError.Other, null, "导入失败");
                }
            }
            return Ret;
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="ds">数据表</param>
        /// <param name="eId">企业编码</param>
        /// <param name="userId">用户编号</param>
        /// <param name="excelRecordID">导入编号</param>
        /// <returns></returns>
        public RetResult InportExcel(System.Data.DataSet ds, long eId, long userId, long excelRecordID)
        {
            Ret.Msg = "导入经销商信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            try
            {
                if (ds != null)
                {
                    using (DataClassesDataContext DataContext = GetDataContext())
                    {
                        StringBuilder strBuilder = new StringBuilder();
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RetResult result = new RetResult();
                            result.CmdError = CmdResultError.EXCEPTION;
                            Dealer model = new Dealer();
                            model.DealerName=ds.Tables[0].Rows[i][0].ToString().Trim();
                            model.Address = ds.Tables[0].Rows[i][1].ToString().Trim();
                            model.Person = ds.Tables[0].Rows[i][2].ToString().Trim();
                            model.Phone = ds.Tables[0].Rows[i][3].ToString().Trim();
                            model.Enterprise_Info_ID = eId;
                            model.adduser = userId;
                            model.adddate = DateTime.Now;
                            model.Status = (int)Common.EnumFile.Status.used;
                            model.DealerCode = ds.Tables[0].Rows[i][4].ToString();
                            model.DealerLevel = (int)Common.EnumFile.DealerLevel.One;
                            RetResult retResult = Add(model, ds.Tables[0].Rows[i][5].ToString(), ds.Tables[0].Rows[i][6].ToString());
                            if (retResult.CmdError != CmdResultError.NONE)
                            {
                                strBuilder.Append("第【" + (i+1).ToString() + "】行导入信息失败," + retResult.Msg);
                                break;
                            }
                        }
                        Ret.Msg = strBuilder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "MaterialDAL.Edit()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Ret;
        }

        #region 接口同步经销商信息
        /// <summary>
        /// 接口同步经销商信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public InterfaceResult InterfaceAdd(DealerModel request, string accessToken)
        {
            InterfaceResult result = new InterfaceResult();
            try
            {
                using (DataClassesDataContext db = GetDataContext())
                {
                    try
                    {
                        Token token = new ApiDAL().TokenDecrypt(accessToken);
                        if (token != null && token.isTokenOK)
                        {
                            var mo = db.Dealer.Where(p => p.DealerName == request.dealerName).FirstOrDefault();
                            if (null != mo)
                            {
                                Dealer model = new Dealer();
                                model.adddate = DateTime.Now;
                                model.Address = request.address;
                                model.adduser = token.Enterprise_User_ID;
                                model.DealerName = request.dealerName;
                                model.Enterprise_Info_ID = token.Enterprise_Info_ID;
                                model.lastdate = DateTime.Now;
                                model.lastuser = token.Enterprise_User_ID;
                                model.Person = request.linker;
                                model.Phone = request.linkTel;
                                model.DealerCode = request.dealerCode;
                                model.Status = (int)Common.EnumFile.Status.used;
                                RetResult ret = this.Add(model,request.defaultUserName,request.defaultUserPWD);
                                if (ret.CmdError == (int)CmdResultError.NONE)
                                {
                                    result.retCode = 0;
                                    result.retMessage = "操作成功";
                                    result.isSuccess = true;
                                    result.retData = null;
                                }
                                else
                                {
                                    result.retCode = 1;
                                    result.retMessage = ret.Msg;
                                    result.isSuccess = false;
                                    result.retData = null;
                                }
                            }
                        }
                        else
                        {
                            result.retCode = 1;
                            result.retMessage = "token失效，请重新获取!";
                            result.isSuccess = false;
                            result.retData = null;
                        }
                    }
                    catch (Exception)
                    {
                        result.retCode = 3;
                        result.retMessage = "出现异常!";
                        result.isSuccess = false;
                        result.retData = null;
                    }
                }
            }
            catch (Exception)
            {
                result.retCode = 3;
                result.retMessage = "出现异常!";
                result.isSuccess = false;
                result.retData = null;
            }
            return result;
        }
        #endregion
    }
}
