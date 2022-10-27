/********************************************************************************
** 作者： 李子巍
** 创始时间：2015-06-15
** 修改人：xxx
** 修改时间：xxxx-xx-xx
** 修改人：xxx
** 修改时间：xxx-xx-xx
** 描述：
** 主要用于码信息管理数据
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;
using Common;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace BLL
{
    public class SellCodeBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        public BaseResultModel GetCode(long RequestCodeId)
        {
            SellCodeDAL ObjSellCodeDAL = new SellCodeDAL();

            List<LinqModel.Enterprise_FWCode_00> DataList = ObjSellCodeDAL.GetCode(RequestCodeId);
            string[] strDataArray = new string[3];
            if (DataList != null && DataList.Count > 0)
            {
                if (DataList[0].Type.Value == 1)
                {
                    strDataArray[0] = "1";
                }
                else if (DataList[0].Type.Value == 3)
                {
                    strDataArray[0] = "2";
                }
                strDataArray[1] = DataList[0].EWM;
                strDataArray[2] = DataList[DataList.Count - 1].EWM;
            }

            return ToJson.NewModelToJson(strDataArray, DataList == null ? "0" : "1", "");
        }

        /// <summary>
        /// 获取已销售列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetSaleList(string beginDate, string endDate, long eId, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<LinqModel.SalesInformation> model = dal.GetSaleList(beginDate, endDate, eId, pageIndex, out totalCount);

            BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取二维码列表
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="rId">申请码标识</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetEWMCode(string ewm, long rId, int status, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<Enterprise_FWCode_00> model = dal.GetEWMCode(ewm, rId, status, pageIndex, PageSize, out totalCount);

            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }

        public BaseResultList GetSalesDetail(string ewm, string SalesId, string EwmTableIdArray, int pageIndex)
        {
            if (string.IsNullOrEmpty(EwmTableIdArray))
            {
                return ToJson.NewListToJson(new List<LinqModel.Enterprise_FWCode_00>(), pageIndex, PageSize, 0, "数据错误！");
            }

            if (string.IsNullOrEmpty(SalesId))
            {
                return ToJson.NewListToJson(new List<LinqModel.Enterprise_FWCode_00>(), pageIndex, PageSize, 0, "数据错误！请刷新重试！");
            }

            string[] TableIdArray = EwmTableIdArray.Split(',');

            if (TableIdArray.Length <= 0)
            {
                return ToJson.NewListToJson(new List<LinqModel.Enterprise_FWCode_00>(), pageIndex, PageSize, 0, "数据错误！");
            }

            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<LinqModel.Enterprise_FWCode_00> DataList = dal.GetSalesDetail(ewm, TableIdArray[0].Trim(), Convert.ToInt64(SalesId), pageIndex, out totalCount);

            return ToJson.NewListToJson(DataList, pageIndex, PageSize, totalCount, "");
        }

        /// <summary>
        /// 销售二维码
        /// </summary>
        /// <param name="eId">企业编号</param>
        /// <param name="ProductionTime">生产日期</param>
        /// <param name="DealerId">经销商编号</param>
        /// <param name="EWMBegin">起始码</param>
        /// <param name="EWMEnd">结束码</param>
        /// <returns>销售结果</returns>
        public BaseResultModel SaleCode(long eId, string ProductionTime, string DealerId, string EWMBegin, string EWMEnd)
        {
            BaseResultModel ObjResultModel = new BaseResultModel();
            RetResult ObjRetResult = new RetResult();
            ObjRetResult.Msg = "数据错误";
            ObjRetResult.CmdError = CmdResultError.EXCEPTION;
            if (DealerId == null)
            {
                ObjResultModel = ToJson.NewRetResultToJson("0", "数据错误，无法获取经销商信息！");
                return ObjResultModel;
            }
            RequestCodeDAL dal = new RequestCodeDAL();
            RequestCode requestBegin = new RequestCode();
            RequestCode requestEnd = new RequestCode();
            Enterprise_FWCode_00 codeBegin = dal.GetEWMModel(EWMBegin, out requestBegin);
            Enterprise_FWCode_00 codeEnd = dal.GetEWMModel(EWMEnd, out requestEnd);
            string errData = string.Empty;
            CmdResultError error = CmdResultError.EXCEPTION;
            #region 码有效性判断
            if (codeBegin == null)
            {
                errData = "起始码错误";
            }
            else if (codeBegin.Status != (int)Common.EnumFile.UsingStateCode.NotUsed)
            {
                errData = "起始码已销售！";
            }
            else if (codeEnd == null)
            {
                errData = "结束码错误";
            }
            else if (codeEnd.Status != (int)Common.EnumFile.UsingStateCode.NotUsed)
            {
                errData = "结束码已销售！";
            }
            else if (codeBegin.Enterprise_FWCode_ID > codeEnd.Enterprise_FWCode_ID)
            {
                errData = "起始码不能大于结束码！";
            }
            else if (requestBegin.Route_DataBase_ID - requestEnd.Route_DataBase_ID > 1)
            {
                errData = "请选择同一批次的二维码！";
            }
            else if (requestBegin.Type != requestEnd.Type)
            {
                errData = "请选择同一类型的二维码！";
            }
            #endregion
            int CodeType = (int)codeBegin.Type;
            if (CodeType == (int)Common.EnumFile.CodeType.single || CodeType == (int)Common.EnumFile.CodeType.pesticides)
            {
                #region 码是否合格的判断
                bool codeI = false;
                if (codeBegin.Type != (int)Common.EnumFile.CodeType.single && codeBegin.Type != (int)Common.EnumFile.CodeType.pesticides)
                {
                    errData = "起始码错误，请输入单品码！";
                }
                else if (codeEnd.Type != (int)Common.EnumFile.CodeType.single && codeEnd.Type != (int)Common.EnumFile.CodeType.pesticides)
                {
                    errData = "结束码错误，请输入单品码！";
                }
                else
                {
                    codeI = true;
                }
                if (!codeI)
                {
                    ObjRetResult.SetArgument(error, errData, errData);
                    ObjResultModel = ToJson.NewRetResultToJson((Convert.ToInt32(ObjRetResult.IsSuccess)).ToString(), ObjRetResult.Msg);
                    return ObjResultModel;
                }
                #endregion
                ObjRetResult = dal.SaleCodeSingle(eId, CodeType, Convert.ToInt64(DealerId), codeBegin, codeEnd, (long)requestBegin.Route_DataBase_ID, Convert.ToDateTime(ProductionTime));
            }
            else if (CodeType == (int)Common.EnumFile.CodeType.bGroup)
            {
                #region 验证输入的箱标码
                bool codeI = false;
                if (codeBegin.Type != (int)EnumFile.CodeType.bGroup)
                {
                    errData = "起始码错误！请输入箱标码！";
                }
                else if (codeEnd.Type != (int)EnumFile.CodeType.bGroup)
                {
                    errData = "结束码错误！请输入箱标码";
                }
                else
                {
                    codeI = true;
                }
                if (!codeI)
                {
                    ObjRetResult.SetArgument(error, errData, errData);
                    ObjResultModel = ToJson.NewRetResultToJson((Convert.ToInt32(ObjRetResult.IsSuccess)).ToString(), ObjRetResult.Msg);
                    return ObjResultModel;
                }
                #endregion
                ObjRetResult = dal.SaleCodeGroup(CodeType, "", codeBegin, codeEnd, Convert.ToInt64(DealerId), (long)requestBegin.Route_DataBase_ID, eId, Convert.ToDateTime(ProductionTime));
            }
            ObjResultModel = ToJson.NewRetResultToJson((Convert.ToInt32(ObjRetResult.IsSuccess)).ToString(), ObjRetResult.Msg);
            return ObjResultModel;
        }

        /// <summary>
        /// 获取二维码信息
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns>操作结果</returns>
        public string GetEWM(string ewm)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            Enterprise_FWCode_00 model = dal.GetEWM(ewm);
            string result = ToJson.ModelToJson(model, 1, "");
            return result;
        }

        public BaseResultModel GetEnterpriseList(long eid)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            Enterprise_Info model = dal.GetMainCode(eid);
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }
    }

    public class RequestCodeBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取申请码列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">上级部门</param>
        /// <param name="ewm">产品二维码</param>
        /// <param name="name">产品名称</param>
        /// <param name="upId">上级部门</param>
        /// <param name="beginDate">申请时间开始</param>
        /// <param name="endDate">申请时间结束</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public LinqModel.BaseResultList GetList(long? eId, long? upId, string mId, string mName, string beginDate, string endDate, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<View_RequestCodeAndEnterprise_Info> model = dal.GetList(eId, upId, mId, mName, beginDate, endDate, pageIndex, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }

        /// <summary>
        /// 获取申请码列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">上级部门</param>
        /// <param name="ewm">产品二维码</param>
        /// <param name="name">产品名称</param>
        /// <param name="upId">上级部门</param>
        /// <param name="beginDate">申请时间开始</param>
        /// <param name="endDate">申请时间结束</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public LinqModel.BaseResultList GetBoxList(long? eId, string mId, string mName, string beginDate, string endDate, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<View_RequestCodeAndEnterprise_Info> model = dal.GetBoxList(eId, mId, mName, beginDate, endDate, pageIndex, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }
        /// <summary>
        /// 接口获取申请码列表
        /// </summary>
        /// <param name="eId"></param>
        /// <param name="mName"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="batchname"></param>
        /// <returns></returns>
        public LinqModel.BaseResultList InGetBoxList(string mName, string beginDate, string endDate, string batchname)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            List<View_RequestCodeSettingAndEnterprise_Info> model = dal.InGetBoxList(mName, beginDate, endDate, batchname);
            return ToJson.NewListToJson(model, 0, 0, 0, "");
        }
        /// <summary>
        /// 获取申请码列表
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">上级部门</param>
        /// <param name="ewm">产品二维码</param>
        /// <param name="name">产品名称</param>
        /// <param name="upId">上级部门</param>
        /// <param name="beginDate">申请时间开始</param>
        /// <param name="endDate">申请时间结束</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public LinqModel.BaseResultList GetSysList(long? eId, string eName, string mName, string beginDate, string endDate, int pageIndex, int levelId)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<View_RequestCodeAndEnterprise_Info> model = dal.GetSysList(eId, eName, mName, beginDate, endDate, pageIndex, levelId, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }

        /// <summary>
        /// 申请码
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="upId">企业上级部门标识</param>
        /// <param name="mId">产品标识</param>
        /// <param name="codeCount">申请码数量</param>
        /// <returns>JSON字符串</returns>
        public LinqModel.BaseResultModel Add(long eId, long upId, string mId, int? codeCount, long userId)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult ret = new RetResult();
            if (string.IsNullOrEmpty(mId))
            {
                ret.Msg = "请选择产品！";
            }
            //else if (codeCount == null || codeCount <= 0)
            //{
            //    ret.Msg = "请填写正确的申请数量！";
            //}
            //else if (codeCount.Value > 100000)
            //{
            //    ret.Msg = "单次申请数量不能超过100000个！";
            //}
            else
            {
                ret = dal.Add(eId, upId, Convert.ToInt64(mId), codeCount.Value, userId);
            }

            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 修改码状态
        /// </summary>
        /// <param name="rId">申请码表标识</param>
        /// <param name="status">状态</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel ChangeStatus(long rId, int status, string downLoadUrl)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult ret = dal.ChangeStatus(rId, status, downLoadUrl);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 修改码状态
        /// </summary>
        /// <param name="rId">申请码表标识</param>
        /// <param name="status">状态</param>
        /// <param name="pf">登陆信息</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel ChangeStatus(long rId, int status, LoginInfo pf)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult ret = dal.ChangeStatus(rId, status, pf);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        public BaseResultModel ChangeStatus(long rId, int status, string downLoadUrl, string filePassword, bool IsEncryption, bool Check_Image)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult ret = dal.ChangeStatus(rId, status, downLoadUrl, filePassword, IsEncryption, Check_Image);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        public BaseResultModel UpdateDownLoadNum(string RequestCodeId)
        {
            if (RequestCodeId == null)
            {
                return ToJson.NewRetResultToJson("0", "数据错误！");
            }

            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult ret = dal.UpdateDownLoadNum(Convert.ToInt64(RequestCodeId));
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 获取已销售列表
        /// </summary>
        /// <param name="rId">生成码ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetSaleList(string ewm, long rId, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<Enterprise_FWCode_00> model = dal.GetSalesPageList(rId, ewm.Trim(), pageIndex, PageSize, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }

        /// <summary>
        /// 获取二维码列表
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="rId">申请码标识</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetEWMCode(string ewm, long rId, int status, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<Enterprise_FWCode_00> model = dal.GetEWMCode(ewm, rId, status, pageIndex, PageSize, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }

        /// <summary>
        /// 销售
        /// </summary>
        /// <param name="eId">企业标识</param>
        /// <param name="batchId">批次标识</param>
        /// <param name="batchext_id">小批次标识</param>
        /// <param name="salePlaceId">经销商标识</param>
        /// <param name="EWMBegin">起始码</param>
        /// <param name="EWMEnd">结束码</param>
        /// <returns>操作结果</returns>
        public BaseResultModel SaleCodeSingle(long eId, long batchId, long? batchext_id, long salePlaceId, string saleDate, string EWMBegin, string EWMEnd)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            saleDate = Convert.ToDateTime(saleDate).ToString("yyyy-MM-dd HH:mm:ss");
            RetResult ret = dal.SaleCode(eId, batchId, batchext_id, salePlaceId, saleDate, EWMBegin, EWMEnd);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 获取二维码信息
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns>操作结果</returns>
        public string GetEWM(string ewm)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            Enterprise_FWCode_00 model = dal.GetEWM(ewm);
            string result = ToJson.ModelToJson(model, 1, "");
            return result;
        }

        /// <summary>
        /// 查询产品列表
        /// </summary>
        /// <returns></returns>
        public BaseResultList SearchNameList(long eId)
        {
            RequestCodeDAL dal = new RequestCodeDAL();

            List<LinqModel.Material> dataList = dal.SearchNameList(eId);

            return ToJson.NewListToJson(dataList, 1, 100000, dataList.Count, "");
        }

        public BaseResultList GetMaterialNameList(long eId)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            List<long> idList = dal.GetEnterpriseInfo(eId);

            List<LinqModel.Material> dataInfo = dal.GetSysEnterspriseList(idList);
            dataInfo.Sort(new NameComparer());

            return ToJson.NewListToJson(dataInfo, 1, 100000, dataInfo.Count, "");
        }

        /// <summary>
        /// 获取二维码申请信息
        /// </summary>
        /// <param name="rId">产品申请二维码ID</param>
        /// <returns>返回结果</returns>
        public RequestCode GetModel(long rId)
        {
            RequestCodeDAL dal = new RequestCodeDAL();

            LinqModel.RequestCode objRequestCode = dal.GetModel(rId);

            return objRequestCode;
        }

        public List<Enterprise_FWCode_00> GetCodeList(long rId, int pageIndex, int pageSize)
        {
            return new RequestCodeDAL().GetCodeList(rId, pageIndex, pageSize);
        }

        public List<LinqModel.Enterprise_FWCode_00> GetEwmList(long id, int pageIndex, int pageSize)
        {
            return new RequestCodeDAL().GetEwmList(id, pageIndex, pageSize);
        }

        public BaseResultModel ChangeStatus(long rId, long requestCount)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult ret = dal.ChangeStatus(rId, requestCount);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
        }

        /// <summary>
        /// 获取申请码记录ID
        /// </summary>
        /// <param name="bName">批次名称</param>
        /// <returns></returns>
        public BaseResultModel GetRequestID(string bName)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RequestCode requestCode = dal.GetRequestID(bName);
            string code = "1";
            string msg = "";
            if (requestCode == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            BaseResultModel model = ToJson.NewModelToJson(requestCode, code, msg);
            return model;
        }

        /// <summary>
        /// 生成码方法new20170217
        /// </summary>
        /// <param name="reMode">申请码表</param>
        /// <param name="setModel">配置码表</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel Generate(RequestCode reMode, RequestCodeSetting setModel, string scleixing)
        {
            RequestCodeDAL ObjRequestCodeDAL = new RequestCodeDAL();
            RetResult ObjRetResult = ObjRequestCodeDAL.Generate(reMode, setModel, scleixing);
            return ToJson.NewModelToJson(setModel, Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 生成箱码方法
        /// </summary>
        /// <param name="EnterpriseID">企业Id</param>
        /// <param name="UpEnterpriseID">上级企业Id</param>
        /// <param name="Type">类型</param>
        /// <param name="Specifications">规格</param>
        /// <param name="UserID">操作人</param>
        /// <param name="Material_ID">产品Id</param>
        /// <param name="RequestCount">生成数量</param>
        /// <returns>返回操作结果对象</returns>
        public LinqModel.BaseResultModel GeneratePackCode(long EnterpriseID, string codeAttribute, long UserID, string Material_ID, string RequestCount, int codeOfType)
        {
            LoginInfo user = SessCokie.Get;
            RequestCode code = new RequestCode();
            code.Enterprise_Info_ID = EnterpriseID;
            code.Material_ID = Convert.ToInt64(Material_ID);
            code.TotalNum = Convert.ToInt32(RequestCount);
            code.RequestDate = DateTime.Now;
            code.saleCount = 0;
            //if (!string.IsNullOrEmpty(PackNumber))
            //{
            //    code.PackCount = Convert.ToInt32(PackNumber);                
            //}
            code.adddate = DateTime.Now;
            code.adduser = user.UserID;
            code.Status = (int)Common.EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated;
            code.IsRead = (int)Common.EnumFile.IsRead.noRead;
            code.Type = Convert.ToInt32(codeAttribute);
            code.Specifications = 0;
            code.CodeOfType = codeOfType;
            if (codeOfType == (int)Common.EnumFile.CodeOfType.SCode)
            {
                code.SCodeLength = 1;
            }
            //SpecificationDAL dal = new SpecificationDAL();
            //Specification Sp = dal.GetInfo(Convert.ToInt64(GuiGeID));
            //if (Sp != null)
            //{
            //    code.Specifications = Convert.ToInt32(Sp.Value);
            //    code.GuiGe = Sp.GuiGe;
            //}
            RequestCodeDAL ObjRequestCodeDAL = new RequestCodeDAL();
            RetResult ObjRetResult = ObjRequestCodeDAL.GeneratePackCode(code);
            return ToJson.NewRetResultToJson(Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        /// <summary>
        /// 追溯码管理详情查看码
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="sId">设置表ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public BaseResultList GetSettingCode(string ewm, long sId, int status, int pageIndex)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<Enterprise_FWCode_00> model = dal.GetSettingCode(ewm, sId, status, pageIndex, PageSize, out totalCount);
            return ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");
        }

        /// <summary>
        /// 下载txt文件数据
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="sId">设置表ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<Enterprise_FWCode_00> GetSettingCodeTxt(string ewm, long sId, int status)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            List<Enterprise_FWCode_00> model = dal.GetSettingCodeTxt(ewm, sId, status, 0, PageSize, out totalCount);
            return model;
        }

        /// <summary>
        /// 执行生成码存储过程
        /// </summary>
        /// <returns></returns>
        public BaseResultModel ServiceGenerated()
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult result = dal.ServiceGenerated();
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }

        public void GenReport()
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            dal.GenReport();
        }

        /// <summary>
        /// 生成码方法new20170217
        /// 2018-09-04新增两个模板为模板4添加图片和链接
        /// </summary>
        /// <param name="reMode">申请码表</param>
        /// <param name="setModel">配置码表</param>
        /// <returns></returns>
        public LinqModel.BaseResultModel GenerateMuBan(RequestCode reMode, RequestCodeSetting setModel, RequestCodeSettingMuBan mubanModel, string scleixing)
        {
            RequestCodeDAL ObjRequestCodeDAL = new RequestCodeDAL();
            if (string.IsNullOrEmpty(mubanModel.StrMuBanImg.Replace("[", "").Replace("]", "")))
            {
                mubanModel.MuBanImg = null;//根据Files解析
            }
            else
            {
                List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(mubanModel.StrMuBanImg);
                XElement xml = new XElement("infos");
                foreach (var item in imgs)
                {
                    xml.Add(
                        new XElement("img",
                            new XAttribute("name", "1.jpg"),
                            new XAttribute("value", item.fileUrl),
                            new XAttribute("small", item.fileUrls)
                        )
                    );
                }
                mubanModel.MuBanImg = xml;//根据Files解析
            }
            RetResult ObjRetResult = ObjRequestCodeDAL.GenerateMuBan(reMode, setModel, mubanModel, scleixing);
            return ToJson.NewModelToJson(setModel, Convert.ToInt32(ObjRetResult.IsSuccess).ToString(), ObjRetResult.Msg);
        }

        #region 医疗器械验证生成码是生产批号是否重复
        public LinqModel.BaseResultModel YanZhengPH(long eid, int bzSpecType, long mid, string shengchanPH)
        {
            RequestCodeDAL ObjRequestCodeDAL = new RequestCodeDAL();
            RetResult result = ObjRequestCodeDAL.YanZhengPH(eid, bzSpecType, mid, shengchanPH);
            return ToJson.NewRetResultToJson(Convert.ToInt32(result.IsSuccess).ToString(), result.Msg);
        }
        #endregion

        #region 20200609修改追溯配置信息那预览码效果显示的码为第一个码
        public Enterprise_FWCode_00 GetCodeModel(long sId)
        {
            long totalCount = 0;
            RequestCodeDAL dal = new RequestCodeDAL();
            Enterprise_FWCode_00 model = dal.GetCodeModel(sId, out totalCount);
            return model;
        }
        #endregion

        #region 20210421打码客户端注册PI调取追溯接口
        public RetResult AddPIInfo(RequestCode rModel, RequestCodeSetting sModel, string materialName)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            ret = dal.AddPIInfo(rModel, sModel, materialName);
            return ret;
        }

        public BaseResultList GetRequestCodeList(long enterpriseId, string date)
        {
            BaseResultList result = new BaseResultList();
            RequestCodeDAL dal = new RequestCodeDAL();
            List<RequestCode> list = dal.GetRequestCodeList(enterpriseId, date); ;
            result = ToJson.NewListToJson(list, 0, 0, (long)list.Count, "");
            return result;
        }

        public List<string> GetPICodeList(long enterpriseId, string batchNo)
        {
            RequestCodeDAL dal = new RequestCodeDAL();
            List<string> list = dal.GetPICodeList(enterpriseId, batchNo); ;
            return list;
        }
        #endregion
    }

    public class NameComparer : IComparer<Material>
    {
        public int Compare(Material x, Material y)
        {
            return x.MaterialFullName.CompareTo(y.MaterialFullName);
        }
    }
}
