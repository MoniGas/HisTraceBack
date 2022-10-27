using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using Common.Tools;
using System.Text;
using System.IO;

namespace BLL
{
    public class SysEnterpriseManageBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        public BaseResultList GetEnterpriseInfoList(string name, long eId, int? pageIndex, bool wareHouseStatus = false)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDAL = new SysEnterpriseManageDAL();
            long totalCount = 0;
            List<Enterprise_Info> dataList = objSysEnterpriseManageDAL.GetEnterpriseInfoList(name, eId, pageIndex, out totalCount, wareHouseStatus);

            return ToJson.NewListToJson(dataList, pageIndex.Value, PageSize, totalCount, "");
        }

        public BaseResultList GetAreaEnterprise(long shengId, long shiId)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDAL = new SysEnterpriseManageDAL();
            List<Enterprise_Info> dataList = objSysEnterpriseManageDAL.GetAreaEnterprise(shengId, shiId);

            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataList.Count, "");
        }

        public BaseResultList GetAreaEnterprise(long shengId, long shiId, long eId)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDAL = new SysEnterpriseManageDAL();
            List<string> dataList = objSysEnterpriseManageDAL.GetAreaEnterprise(shengId, shiId, eId);

            return ToJson.NewListToJson(dataList, 1, dataList.Count, dataList.Count, "");
        }

        public BaseResultModel Save(long eId, string arrayId, string falseId)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDAL = new SysEnterpriseManageDAL();

            RetResult result = objSysEnterpriseManageDAL.Save(eId, arrayId, falseId);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public BaseResultModel VerifyEnterprise(string enterpriseid, string type)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDAL = new SysEnterpriseManageDAL();

            RetResult result = objSysEnterpriseManageDAL.VerifyEnterprise(enterpriseid, type);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public BaseResultList GetEnterprise(string name, long enterpriseid, int? pageIndex)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDAL = new SysEnterpriseManageDAL();
            long totalCount = 0;
            List<View_Order_EnterpriseAccount> dataList = objSysEnterpriseManageDAL.GetEnterprise(name, enterpriseid, pageIndex, out totalCount);

            return ToJson.NewListToJson(dataList, pageIndex.Value, PageSize, totalCount, "");
        }

        public BaseResultModel VerifyShop(string enterpriseid, string type)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDAL = new SysEnterpriseManageDAL();

            RetResult result = objSysEnterpriseManageDAL.VerifyShop(enterpriseid, type);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public BaseResultModel VerifyWareHouse(string enterpriseid, string type)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDal = new SysEnterpriseManageDAL();

            RetResult result = objSysEnterpriseManageDal.VerifyWareHouse(enterpriseid, type);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public BaseResultModel SetRequestCount(long eId, long count)
        {
            SysEnterpriseManageDAL objSysEnterpriseManageDal = new SysEnterpriseManageDAL();

            RetResult result = objSysEnterpriseManageDal.SetRequestCount(eId, count);

            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        /// <summary>
        /// 管理员/监管部门给企业重置密码
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="password">重置的密码</param>
        /// <returns></returns>
        public BaseResultModel SetPassWord(long eId, string password)
        {
            LoginInfo pf = SessCokie.Get;
            SysEnterpriseManageDAL objSysEnterpriseManageDal = new SysEnterpriseManageDAL();
            RecordLog recordLog = new RecordLog
            {
                Enterprise_Info_ID = eId,
                Enterprise_InfoUP_ID = pf.EnterpriseID,
                PassWord = password,
                EditTime = DateTime.Now
            };
            RetResult result = objSysEnterpriseManageDal.SetPassWord(eId, password, recordLog);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        #region 20170810 代理商/管理后台新改版
        /// <summary>
        /// 获取我的企业列表
        /// </summary>
        /// <param name="name">企业名称</param>
        /// <param name="eId">ID</param>
        /// <param name="beginDate">加入时间</param>
        /// <param name="endDate">加入时间</param>
        /// <param name="pageIndex">分页</param>
        /// <param name="wareHouseStatus"></param>
        /// <returns></returns>
        public PagedList<Enterprise_Info> GetEnterpriseInfoListMan(string name, long eId, string beginDate, string endDate, int? pageIndex, bool wareHouseStatus = false)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<Enterprise_Info> dataList = dal.GetEnterpriseInfoListMan(name, eId, beginDate, endDate, pageIndex, wareHouseStatus);
            return dataList;
        }
        /// <summary>
        /// 获取我的所有企业列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="wareHouseStatus"></param>
        /// <returns></returns>
        public List<Enterprise_Info> GetAllEnterpriseInfoListMan(string name, long eId, bool wareHouseStatus = false)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            List<Enterprise_Info> dataList = dal.GetAllEnterpriseInfoListMan(name, eId, wareHouseStatus);
            return dataList;
        }

        /// <summary>
        /// 获取新入驻的企业列表
        /// </summary>
        /// <param name="name">企业名称（查询用）</param>
        /// <param name="beginDate">加入时间</param>
        /// <param name="endDate">加入时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<Enterprise_Info> GetEnterpriseInfoListNewAdd(string name, string beginDate, string endDate, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<Enterprise_Info> dataList = dal.GetEnterpriseInfoListNewAdd(name, beginDate, endDate, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public View_EnterpriseInfoUser GetEnInfo(long eid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            View_EnterpriseInfoUser result = dal.GetEnInfo(eid);
            return result;
        }

        /// <summary>
        /// 获取合同最新的记录
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public Contract GetContractInfo(long eid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            Contract result = dal.GetContractInfo(eid);
            return result;
        }

        /// <summary>
        /// 给企业管理账户重置密码
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public RetResult ResetPassword(long eid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.ResetPassword(eid);
            return result;
        }

        /// <summary>
        /// 获取企业码情况
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public Enterprise_Info GetEnInfoCodeCount(long eId)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            Enterprise_Info result = dal.GetEnInfoCodeCount(eId);
            return result;
        }

        /// <summary>
        /// 企业用码量设置
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="sqcount">企业最多申请数量</param>
        /// <param name="tzcount">企业可透支的数量</param>
        /// <returns></returns>
        public RetResult SetAmountCode(long eid, long sqcount, long tzcount)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.SetAmountCode(eid, sqcount, tzcount);
            return result;
        }

        /// <summary>
        /// 给企业续码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult ContinneCode(ContinneCodeRecord model)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.ContinneCode(model);
            return result;
        }

        /// <summary>
        /// 获取企业续码记录
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_ContinueCode> GetContinneCodeRecord(long eid, long platId, string beginDate, string endDate, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<View_ContinueCode> dataList = dal.GetContinneCodeRecord(eid, platId, beginDate, endDate, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 获取企业用码
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_UsedCodeSituation> GetUsedCodeSituation(long eid, string beginDate, string endDate, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<View_UsedCodeSituation> dataList = dal.GetUsedCodeSituation(eid, beginDate, endDate, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 关联心入驻的企业
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="pid">登录ID</param>
        /// <returns></returns>
        public RetResult GuanLian(long eid, long pid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.GuanLian(eid, pid);
            return result;
        }

        /// <summary>
        /// 停用企业
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public RetResult TingYong(long eid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.TingYong(eid);
            return result;
        }

        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="eId">企业ID</param>
        /// <param name="sqcount">设置用码量</param>
        /// <param name="tzcount">设置透支数量</param>
        /// <param name="type">类型（1：只是审核，2：审核并保存）</param>
        /// <returns></returns>
        public RetResult SetAudit(long eid, long sqcount, long tzcount, int type)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            var result = dal.SetAudit(eid, sqcount, tzcount, type);
            return result;
        }

        /// <summary>
        /// 签订合同
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Contract(Contract model)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.Contract(model);
            return result;
        }

        /// <summary>
        /// 获取企业签订合同列表
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<Contract> GetContractList(long eid, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<Contract> dataList = dal.GetContractList(eid, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 获取企业商城管理（是否开通商城）
        /// </summary>
        /// <param name="name">企业名称</param>
        /// <param name="enterpriseid">企业ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagedList<View_Order_EnterpriseAccount> GetEnterpriseShopList(string name, long enterpriseid, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<View_Order_EnterpriseAccount> dataList = dal.GetEnterpriseShopList(name, enterpriseid, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 确认开通/暂停使用
        /// </summary>
        /// <param name="enterpriseid">企业ID</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public RetResult EditVerifyShop(long enterpriseid, int type)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = dal.EditVerifyShop(enterpriseid, type);
            return result;
        }

        /// <summary>
        /// 获取企业续码记录
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<ContinneCodeRecord> GetEnContinneCodeRecord(long eid, string beginDate, string endDate, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<ContinneCodeRecord> dataList = dal.GetEnContinneCodeRecord(eid, beginDate, endDate, pageIndex);
            return dataList;
        }
        #endregion
        /// <summary>
        /// 获取授权码
        /// </summary>
        /// <param name="eId"></param>
        /// <returns></returns>
        public Enterprise_License GetEnInfoLicense(long eId)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            Enterprise_License result = dal.GetEnInfoLicense(eId);
            return result;
        }
        /// <summary>
        /// 设置授权码
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="mainId"></param>
        /// <param name="licenseCode"></param>
        /// <param name="setDate"></param>
        /// <returns></returns>
        public RetResult SetAuthorizationCode(long eid, long mainId, string setDate, string LicenseType, string fileurl)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.SetAuthorizationCode(eid, mainId, LicenseType, setDate, fileurl);
            return result;
        }

        #region 20200817新加企业PDA设备信息
        /// <summary>
        /// 查询设备列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<PRRU_EnEquipmentInfo> GetEnEquipmentList(string name, long eId, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            //long totalCount = 0;
            PagedList<PRRU_EnEquipmentInfo> dataList = dal.GetEnEquipmentList(name, eId, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 添加设备串号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel Add(PRRU_EnEquipmentInfo model)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = dal.Add(model);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="eId">标识ID</param>
        /// <returns></returns>
        public PRRU_EnEquipmentInfo GetEnEquipmentInfo(long eId)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PRRU_EnEquipmentInfo data = dal.GetEnEquipmentInfo(eId);
            return data;
        }

        /// <summary>
        /// 编辑设备串号信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResultModel EditEnEquipmentInfo(PRRU_EnEquipmentInfo model)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = dal.EditEnEquipmentInfo(model);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        /// <summary>
        /// 启用/禁用设备
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public RetResult EditEnEquipmentStatus(long id, int type)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.EditEquipmentStatus(id, type);
            return result;
        }

        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public BaseResultModel UploadExcelUp(string excelurl)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult ret = new RetResult();
            FileStream fs = new FileStream(excelurl, FileMode.Open, FileAccess.Read);
            Aspose.Cells.Workbook book = new Aspose.Cells.Workbook(fs);
            Aspose.Cells.Worksheet sheet = book.Worksheets[0];
            Aspose.Cells.Cells cells = sheet.Cells;
            System.Data.DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            System.Data.DataSet ds = new System.Data.DataSet();
            ds.Tables.Add(dt);
            RetResult result = dal.ImportExcel(ds);
            if (result.IsSuccess)
            {
                ret.Msg = "上传成功！";
                ret.Code = 0;
            }
            BaseResultModel result2 = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result2;
        }

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="peid">上级监管企业ID</param>
        /// <returns></returns>
        public List<Enterprise_Info> GetEnterpriseList(long peid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            List<Enterprise_Info> dataList = dal.GetEnterpriseList(peid);
            return dataList;
        }
        #endregion

        #region 获取企业扩展表打码客户端用的简版/完整版
        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public EnterpriseShopLink GetEnKhd(long eid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            EnterpriseShopLink result = dal.GetEnKhd(eid);
            return result;
        }

        /// <summary>
        /// 设置使用客户端是简版/完整版(type=2高级版，1简版，3标准版)
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public RetResult SetKHDType(long eid, int type)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.SetKHDType(eid, type);
            return result;
        }

        public RetResult SetJKToken(long eid, string token, string tokencode)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.SetJKToken(eid, token, tokencode);
            return result;
        }

        public RetResult SetTokenForCilent(string loginname, string password, string token, string tokencode)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.SetTokenForCilent(loginname,password,token, tokencode);
            return result;
        }
        #endregion

        public List<string> GetSubUserDI(long uid)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            List<string> result = new List<string>();
            result = dal.GetSubUserDI(uid);
            return result;
        }

        /// <summary>
        /// 获取GS1企业列表 21-10-19
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagedList<View_EnterpriseInfoUser> GetEnterpriseInfoListGS1(string name, long eId, string beginDate, string endDate, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            PagedList<View_EnterpriseInfoUser> dataList = dal.GetEnterpriseInfoListGS1(name, eId, beginDate, endDate, pageIndex);
            return dataList;
        }
        public Enterprise_Info GetModelInfo(long eId)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            Enterprise_Info data = dal.GetModelInfo(eId);
            if (data == null)
            {
                data = new Enterprise_Info();
                data.Enterprise_Info_ID = 0;
            }
            return data;
        }
        public LinqModel.BaseResultModel AddGS(Enterprise_Info model, Enterprise_User user, string youxiaoDate)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = dal.AddGS(model, user, youxiaoDate);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }
        public LinqModel.BaseResultModel EditGS(Enterprise_Info model, string youxiaoDate)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = dal.EditGS(model, youxiaoDate);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }
        public RetResult EditStatus(long id, int type)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.EditStatus(id, type);
            return result;
        }
        #region 维护企业是否开通子企业功能
        /// <summary>
        /// 开通子企业的企业列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PagedList<View_Enterprise_SetMoule> GetSubEnterpriseList(string name, int? pageIndex)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            //long totalCount = 0;
            PagedList<View_Enterprise_SetMoule> dataList = dal.GetSubEnterpriseList(name, pageIndex);
            return dataList;
        }

        /// <summary>
        /// 为企业开通子用户功能
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public BaseResultModel Add(Enterprise_SetMoule model)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = dal.Add(model);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }
        /// <summary>
        /// 修改企业开通子企业状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public RetResult EditSubStatus(long id, int type)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.EditSubStatus(id, type);
            return result;
        }
        #endregion

        #region
        public RetResult SetCodeType(long eid, int CodeType)
        {
            SysEnterpriseManageDAL dal = new SysEnterpriseManageDAL();
            RetResult result = new RetResult();
            result = dal.SetCodeType(eid, CodeType);
            return result;
        }
        #endregion
    }
}