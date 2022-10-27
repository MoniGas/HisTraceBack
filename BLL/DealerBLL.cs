/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-11

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于经销商信息管理逻辑层

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Argument;
using Dal;
using LinqModel;
using Aspose.Cells;
using System.IO;
using System.Data;
using System.Text;
using LinqModel.InterfaceModels;

namespace BLL
{
    public class DealerBLL
    {
        private readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取经销商模型
        /// </summary>
        /// <param name="dealerId">经销商标识</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel GetModel(long dealerId)
        {
            DealerDAL dal = new DealerDAL();
            Dealer dearer = dal.GetModel(dealerId);
            string code = "1";
            string msg = "";
            if (dearer == null)
            {
                code = "0";
                msg = "没有找到数据！";
            }
            return ToJson.NewModelToJson(dearer, code, msg);
        }

        /// <summary>
        /// 获取经销商列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="dealerName">经销商名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>JSON字符串</returns>
        public BaseResultList GetList(long enterpriseId, string dealerName, int pageIndex)
        {
            DealerDAL dal = new DealerDAL();
            long totalCount;
            List<Dealer> liDearer = dal.GetList(enterpriseId, dealerName, pageIndex, out totalCount);
            return ToJson.NewListToJson(liDearer, pageIndex, _pageSize, totalCount, "");
        }

        /// <summary>
        /// 添加经销商
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Add(Dealer model)
        {
            DealerDAL dal = new DealerDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.DealerName))
            {
                ret.Msg = "经销商名称不能为空！";
            }
            else if (model.Dictionary_AddressSheng_ID == null)
            {
                ret.Msg = "请选择经销商所在省份！";
            }
            else if (model.Dictionary_AddressShi_ID == null)
            {
                ret.Msg = "请选择经销商所在市区！";
            }
            else if (model.Dictionary_AddressQu_ID == null)
            {
                ret.Msg = "请选择经销商所在行政区域！";
            }
            else if (string.IsNullOrEmpty(model.Address))
            {
                ret.Msg = "经销商地址不能为空！";
            }
            else if (string.IsNullOrEmpty(model.location))
            {
                ret.Msg = "请选择经销商坐标！";
            }
            //else if (string.IsNullOrEmpty(model.Person))
            //{
            //    ret.Msg = "经销商联系人不能为空！";
            //}
            //else if (string.IsNullOrEmpty(model.Phone))
            //{
            //    ret.Msg = "经销商联系电话不能为空！";
            //}
            else
            {
                ret = dal.Add(model,"","");
            }
            return ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }

        /// <summary>
        /// 修改经销商
        /// </summary>
        /// <param name="newModel">模型</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Edit(Dealer newModel)
        {
            DealerDAL dal = new DealerDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(newModel.DealerName))
            {
                ret.Msg = "经销商名称不能为空！";
            }
            else if (newModel.Dictionary_AddressSheng_ID == null)
            {
                ret.Msg = "请选择经销商所在省份！";
            }
            else if (newModel.Dictionary_AddressShi_ID == null)
            {
                ret.Msg = "请选择经销商所在市区！";
            }
            else if (newModel.Dictionary_AddressQu_ID == null)
            {
                ret.Msg = "请选择经销商所在行政区域！";
            }
            else if (string.IsNullOrEmpty(newModel.location))
            {
                ret.Msg = "请选择经销商坐标！";
            }
            //else if (string.IsNullOrEmpty(newModel.Person))
            //{
            //    ret.Msg = "经销商联系人不能为空！";
            //}
            //else if (string.IsNullOrEmpty(newModel.Phone))
            //{
            //    ret.Msg = "经销商联系电话不能为空！";
            //}
            else
            {
                ret = dal.Edit(newModel);
            }
            return ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
        }

        /// <summary>
        /// 删除经销商
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="dealerId">经销商标识</param>
        /// <returns>JSON字符串</returns>
        public BaseResultModel Del(long enterpriseId, long dealerId)
        {
            DealerDAL dal = new DealerDAL();
            RetResult ret = dal.Del(enterpriseId, dealerId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        public BaseResultList GetSelectList(long enterpriseId)
        {
            DealerDAL dal = new DealerDAL();
            List<Dealer> liDearer = dal.GetSelectList(enterpriseId);
            return ToJson.NewListToJson(liDearer, 1, 1, 1, "");
        }

        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <returns></returns>
        public BaseResultList GetMaterialList(long enterpriseId)
        {
            DealerDAL dal = new DealerDAL();
            List<Material> liDearer = dal.GetMaterialList(enterpriseId);
            return ToJson.NewListToJson(liDearer, 1, 1, 1, "");
        }

        /// <summary>
        /// 获取经销商列表  陈志钢 WinCE
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public List<Dealer> getLst(long enterpriseId)
        {
            DealerDAL dal = new DealerDAL();
            return dal.GetSelectList(enterpriseId);
        }
        /// <summary>
        /// 导入Excel插入经销商信息
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public BaseResultModel AddExcelR(DealerExcelRecord newModel)
        {
            RetResult ret = new RetResult();
            DealerDAL dal = new DealerDAL();
            ret = dal.AddExcelDealer(newModel);
            if (ret.IsSuccess)
            {
                FileStream fs = new FileStream(newModel.ExcelPath, FileMode.Open, FileAccess.Read);
                Workbook book = new Workbook(fs);
                Worksheet sheet = book.Worksheets[0];
                Cells cells = sheet.Cells;
                DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                MaterialDAL madal = new MaterialDAL();
                DealerExcelRecord exModel = new DealerExcelRecord();
                exModel.MaCount = ds.Tables[0].Rows.Count;
                //exModel = madal.UpdataCount(newModel.ID, exModel);
                ret = this.Verify(ds);
                if (ret.IsSuccess)
                {
                    ret = dal.InportExcel(ds, newModel.EnterpriseID.Value, newModel.AddUser.Value, newModel.ID);
                    if (ret.IsSuccess)
                    {
                        ret.Msg = "导入成功！";
                    }
                }
            }
            else
            {
                ret.Msg = "导入失败！";
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 验证导入文档格式是否正确
        /// </summary>
        /// <param name="ds">文档</param>
        /// <returns></returns>
        public RetResult Verify(System.Data.DataSet ds)
        {
            RetResult Ret = new RetResult();
            Ret.Msg = "导入格式有问题！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                if (ds != null)
                {
                    Dictionary<string, string> categoryCodeList = new Dictionary<string, string>();
                    Dictionary<string, string> MaAttributeList = new Dictionary<string, string>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string dealerName = ds.Tables[0].Rows[i][0].ToString().Trim();
                        string dealerAddress = ds.Tables[0].Rows[i][1].ToString().Trim();
                        if (string.IsNullOrEmpty(dealerName))
                        {
                            strBuilder.Append("第【" + (i+1)+ "】行中【经销商名称不允许为空】");
                        }
                        if (string.IsNullOrEmpty(dealerAddress))
                        {
                            strBuilder.Append("第【" + (i + 1) + "】行中【经销商地址不允许为空】");
                        }
                    }
                    if (strBuilder.ToString().Length > 0)
                    {
                        Ret.SetArgument(CmdResultError.PARAMERROR, "", strBuilder.ToString());
                    }
                    else
                    {
                        Ret.SetArgument(CmdResultError.NONE, "", "数据格式正确");
                    }
                }
            }
            catch (Exception ex)
            {
                string errData = "";
            }
            return Ret;
        }

        /// <summary>
        /// 接口同步经销商信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public InterfaceResult InterfaceAdd(DealerModel request, string accessToken)
        {
            DealerDAL dal = new DealerDAL();
            InterfaceResult result = dal.InterfaceAdd(request, accessToken);
            return result;
        }
    }
}
