using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel.InterfaceModels;
using LinqModel;
using Common.Log;
using System.Configuration;

namespace Dal
{
    public class CheckDAL : DALBase
    {
        public InterfaceResult getCodeInfo(string scanCode, string accessToken)
        {
            InterfaceResult result = new InterfaceResult();
            long? eId = 0;
            Token token = new ApiDAL().TokenDecrypt(accessToken);
            try
            {
                if (token != null && token.isTokenOK)
                {
                    eId = token.Enterprise_Info_ID;
                    CheckDealer cDealer = new CheckDealer();
                    using (DataClassesDataContext db = GetDataContext())
                    {
                        string[] arrCode = scanCode.Split('=');
                        if (arrCode.Length == 1)
                        {
                            scanCode = arrCode[0];
                        }
                        if (arrCode.Length == 2)
                        {
                            scanCode = arrCode[1];
                        }
                        //垛码实际上是不存在的，这种情况几乎不存在
                        #region 出库详情里是箱码
                        //出库详情里是箱码  
                        var m = db.View_OutStorage.FirstOrDefault(p => p.EWMCode == scanCode || p.Ewm == scanCode);
                        if (null != m)
                        {
                            if (m.EnterpriseID != eId)
                            {
                                result.retCode = 5;
                                result.retMessage = "请扫描您企业的二维码！";
                                result.isSuccess = false;
                                result.retData = null;
                            }
                            else
                            {
                                var dealermo = db.Dealer.FirstOrDefault(p => p.Dealer_ID == m.DealerID);
                                if (null != dealermo)
                                {
                                    #region  查询产品信息
                                    Enterprise_FWCode_00 codeInfo = GetCodeMode(scanCode);//获取码信息
                                    if (null != codeInfo)
                                    {
                                        var setid = codeInfo.RequestSetID;
                                        if (null != setid)
                                        {
                                            RequestCodeSetting setting = db.RequestCodeSetting.FirstOrDefault(p => p.ID == setid);
                                            if (null != setting)
                                            {
                                                Material material = db.Material.FirstOrDefault(p => p.Material_ID == setting.MaterialID);
                                                if (material != null)
                                                {
                                                    result.retCode = 0;
                                                    result.retMessage = "获取成功";
                                                    result.isSuccess = true;
                                                    cDealer.outNO = m.OutStorageNO;
                                                    cDealer.dealerID = dealermo.Dealer_ID;
                                                    cDealer.dealerName = dealermo.DealerName;
                                                    cDealer.dealerAddress = dealermo.Address;
                                                    cDealer.materialID = Convert.ToInt64(setting.MaterialID);
                                                    cDealer.materialName = material.MaterialName;
                                                    result.retData = cDealer;
                                                }
                                                else
                                                {
                                                    result.retCode = 2;
                                                    result.retMessage = "获取产品信息失败！";
                                                    result.isSuccess = false;
                                                    result.retData = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            result.retCode = 2;
                                            result.retMessage = "获取产品信息失败！";
                                            result.isSuccess = false;
                                            result.retData = null;
                                        }
                                    }
                                    else
                                    {
                                        result.retCode = 2;
                                        result.retMessage = "获取产品信息失败！";
                                        result.isSuccess = false;
                                        result.retData = null;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    result.retCode = 3;
                                    result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                    result.isSuccess = false;
                                    result.retData = null;
                                }
                            }
                        }
                        #endregion
                        else//稽查码不在出库详情里
                        {
                            var m1 = db.BindCodeRecords.FirstOrDefault(p => p.SingleCode == scanCode);//因为稽查码不能为垛码
                            if (null != m1)
                            {
                                if (m1.EnterpriseID != eId)
                                {
                                    result.retCode = 5;
                                    result.retMessage = "请扫描您企业的二维码！";
                                    result.isSuccess = false;
                                    result.retData = null;
                                }
                                else
                                {
                                    #region 装垛的绑定关系，m1.BoxCode为垛码
                                    if (m1.BindType == (int)Common.EnumFile.BindCodeRecordsType.AddCrib) //装垛的绑定关系，m1.BoxCode为垛码
                                    {
                                        var m2 = db.View_OutStorage.FirstOrDefault(p => p.EWMCode == m1.BoxCode && m1.EnterpriseID == eId);
                                        if (null != m2)
                                        {
                                            var dealermo = db.Dealer.FirstOrDefault(p => p.Dealer_ID == m2.DealerID);
                                            if (null != dealermo)
                                            {
                                                #region  查询产品信息
                                                Enterprise_FWCode_00
                                                    codeInfo = GetCodeMode(scanCode); //获取码信息
                                                if (null != codeInfo)
                                                {
                                                    var setid = codeInfo.RequestSetID;
                                                    if (null != setid)
                                                    {
                                                        RequestCodeSetting setting =
                                                            db.RequestCodeSetting.FirstOrDefault(p => p.ID == setid);
                                                        if (null != setting)
                                                        {
                                                            Material material = db.Material.FirstOrDefault(p =>
                                                                p.Material_ID == setting.MaterialID);
                                                            if (material != null)
                                                            {
                                                                cDealer.materialID = Convert.ToInt64(setting.MaterialID);
                                                                cDealer.materialName = material.MaterialName;
                                                            }
                                                            else
                                                            {
                                                                result.retCode = 2;
                                                                result.retMessage = "获取产品信息失败！";
                                                                result.isSuccess = false;
                                                                result.retData = null;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.retCode = 2;
                                                        result.retMessage = "获取产品信息失败！";
                                                        result.isSuccess = false;
                                                        result.retData = null;
                                                    }
                                                }
                                                else
                                                {
                                                    result.retCode = 2;
                                                    result.retMessage = "获取产品信息失败！";
                                                    result.isSuccess = false;
                                                    result.retData = null;
                                                }
                                                #endregion
                                                result.retCode = 0;
                                                result.retMessage = "获取成功";
                                                result.isSuccess = true;
                                                cDealer.outNO = m2.OutStorageNO;
                                                cDealer.dealerID = dealermo.Dealer_ID;
                                                cDealer.dealerName = dealermo.DealerName;
                                                cDealer.dealerAddress = dealermo.Address;
                                                result.retData = cDealer;
                                            }
                                        }
                                        else
                                        {
                                            result.retCode = 3;
                                            result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                            result.isSuccess = false;
                                            result.retData = null;
                                        }
                                    }
                                    #endregion

                                    #region 产品码
                                    else //为产品码
                                    {
                                        if (m1.BindType == (int)Common.EnumFile.BindCodeRecordsType.AddBox) //装箱的绑定关系，m1.BoxCode为箱码
                                        {
                                            var m2 = db.View_OutStorage.FirstOrDefault(p => p.EWMCode == m1.BoxCode && p.EnterpriseID == eId);//判断箱码是否出库
                                            if (null != m2)
                                            {
                                                var dealermo = db.Dealer.FirstOrDefault(p => p.Dealer_ID == m2.DealerID);
                                                if (null != dealermo)
                                                {
                                                    #region  查询产品信息

                                                    Enterprise_FWCode_00
                                                        codeInfo = GetCodeMode(scanCode); //获取码信息
                                                    if (null != codeInfo)
                                                    {
                                                        var setid = codeInfo.RequestSetID;
                                                        if (null != setid)
                                                        {
                                                            RequestCodeSetting setting =
                                                                db.RequestCodeSetting.FirstOrDefault(p => p.ID == setid);
                                                            if (null != setting)
                                                            {
                                                                Material material = db.Material.FirstOrDefault(p =>
                                                                    p.Material_ID == setting.MaterialID);
                                                                if (material != null)
                                                                {
                                                                    cDealer.materialID = Convert.ToInt64(setting.MaterialID);
                                                                    cDealer.materialName = material.MaterialName;
                                                                }
                                                                else
                                                                {
                                                                    result.retCode = 2;
                                                                    result.retMessage = "获取产品信息失败！";
                                                                    result.isSuccess = false;
                                                                    result.retData = null;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            result.retCode = 2;
                                                            result.retMessage = "获取产品信息失败！";
                                                            result.isSuccess = false;
                                                            result.retData = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.retCode = 2;
                                                        result.retMessage = "获取产品信息失败！";
                                                        result.isSuccess = false;
                                                        result.retData = null;
                                                    }

                                                    #endregion
                                                    result.retCode = 0;
                                                    result.retMessage = "获取成功";
                                                    result.isSuccess = true;
                                                    cDealer.outNO = m2.OutStorageNO;
                                                    cDealer.dealerID = dealermo.Dealer_ID;
                                                    cDealer.dealerName = dealermo.DealerName;
                                                    cDealer.dealerAddress = dealermo.Address;
                                                    result.retData = cDealer;
                                                }
                                            }
                                            else
                                            {
                                                var m3 = db.BindCodeRecords.FirstOrDefault(p => p.SingleCode == m1.BoxCode);//查询出垛码
                                                if (null != m3)
                                                {
                                                    var m4 = db.View_OutStorage.FirstOrDefault(p => p.EWMCode == m3.BoxCode);//判断垛码是否出库
                                                    if (null != m4)
                                                    {
                                                        var dealermo =
                                                            db.Dealer.FirstOrDefault(p => p.Dealer_ID == m4.DealerID);
                                                        if (null != dealermo)
                                                        {
                                                            #region  查询产品信息

                                                            Enterprise_FWCode_00 codeInfo = GetCodeMode(scanCode); //获取码信息
                                                            if (null != codeInfo)
                                                            {
                                                                var setid = codeInfo.RequestSetID;
                                                                if (null != setid)
                                                                {
                                                                    RequestCodeSetting setting =
                                                                        db.RequestCodeSetting.FirstOrDefault(p =>
                                                                            p.ID == setid);
                                                                    if (null != setting)
                                                                    {
                                                                        Material material = db.Material.FirstOrDefault(p =>
                                                                            p.Material_ID == setting.MaterialID);
                                                                        if (material != null)
                                                                        {
                                                                            cDealer.materialID = Convert.ToInt64(setting.MaterialID);
                                                                            cDealer.materialName = material.MaterialName;
                                                                        }
                                                                        else
                                                                        {
                                                                            result.retCode = 2;
                                                                            result.retMessage = "获取产品信息失败！";
                                                                            result.isSuccess = false;
                                                                            result.retData = null;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    result.retCode = 2;
                                                                    result.retMessage = "获取产品信息失败！";
                                                                    result.isSuccess = false;
                                                                    result.retData = null;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                result.retCode = 2;
                                                                result.retMessage = "获取产品信息失败！";
                                                                result.isSuccess = false;
                                                                result.retData = null;
                                                            }
                                                            #endregion
                                                            result.retCode = 0;
                                                            result.retMessage = "获取成功";
                                                            result.isSuccess = true;
                                                            cDealer.outNO = m4.OutStorageNO;
                                                            cDealer.dealerID = dealermo.Dealer_ID;
                                                            cDealer.dealerName = dealermo.DealerName;
                                                            cDealer.dealerAddress = dealermo.Address;
                                                            result.retData = cDealer;
                                                        }
                                                        else
                                                        {
                                                            result.retCode = 3;
                                                            result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                                            result.isSuccess = false;
                                                            result.retData = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.retCode = 3;
                                                        result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                                        result.isSuccess = false;
                                                        result.retData = null;
                                                    }
                                                }
                                                else
                                                {
                                                    result.retCode = 3;
                                                    result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                                    result.isSuccess = false;
                                                    result.retData = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            result.retCode = 3;
                                            result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                            result.isSuccess = false;
                                            result.retData = null;
                                        }
                                    }
                                    #endregion

                                }
                            }
                            else
                            {
                                #region 查询历史表
                                //历史表扫箱出箱情况，出库详情表里一定有记录，所以查询时查singlecode就行了
                                var hm = db.BindCodeRecords_history.FirstOrDefault(p => p.SingleCode == scanCode);//因为稽查码不能为垛码
                                if (null != hm)
                                {
                                    if (hm.EnterpriseID != eId)
                                    {
                                        result.retCode = 5;
                                        result.retMessage = "请扫描您企业的二维码！";
                                        result.isSuccess = false;
                                        result.retData = null;
                                    }
                                    else
                                    {
                                        #region 装垛的绑定关系，m1.BoxCode为垛码

                                        if (hm.BindType == (int)Common.EnumFile.BindCodeRecordsType.AddCrib) //装垛的绑定关系，为箱码&& p.BindType == (int)BindCodeRecordsType.AddCrib
                                        {
                                            var m2 = db.View_OutStorage.FirstOrDefault(p => p.EWMCode == hm.BoxCode && p.EnterpriseID == eId);
                                            if (null != m2)
                                            {
                                                var dealermo = db.Dealer.FirstOrDefault(p => p.Dealer_ID == m2.DealerID);
                                                if (null != dealermo)
                                                {
                                                    #region  查询产品信息

                                                    Enterprise_FWCode_00 codeInfo = GetCodeMode(scanCode); //获取码信息
                                                    if (null != codeInfo)
                                                    {
                                                        var setid = codeInfo.RequestSetID;
                                                        if (null != setid)
                                                        {
                                                            RequestCodeSetting setting =
                                                                db.RequestCodeSetting.FirstOrDefault(p => p.ID == setid);
                                                            if (null != setting)
                                                            {
                                                                Material material = db.Material.FirstOrDefault(p =>
                                                                    p.Material_ID == setting.MaterialID);
                                                                if (material != null)
                                                                {
                                                                    cDealer.materialID = Convert.ToInt64(setting.MaterialID);
                                                                    cDealer.materialName = material.MaterialName;
                                                                }
                                                                else
                                                                {
                                                                    result.retCode = 2;
                                                                    result.retMessage = "获取产品信息失败！";
                                                                    result.isSuccess = false;
                                                                    result.retData = null;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            result.retCode = 2;
                                                            result.retMessage = "获取产品信息失败！";
                                                            result.isSuccess = false;
                                                            result.retData = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.retCode = 2;
                                                        result.retMessage = "获取产品信息失败！";
                                                        result.isSuccess = false;
                                                        result.retData = null;
                                                    }

                                                    #endregion
                                                    result.retCode = 0;
                                                    result.retMessage = "获取成功";
                                                    result.isSuccess = true;
                                                    cDealer.outNO = m2.OutStorageNO;
                                                    cDealer.dealerID = dealermo.Dealer_ID;
                                                    cDealer.dealerName = dealermo.DealerName;
                                                    cDealer.dealerAddress = dealermo.Address;
                                                    result.retData = cDealer;
                                                }
                                            }
                                            else
                                            {
                                                result.retCode = 3;
                                                result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                                result.isSuccess = false;
                                                result.retData = null;
                                            }
                                        }
                                        #endregion

                                        #region 产品码

                                        else if (hm.BindType == (int)Common.EnumFile.BindCodeRecordsType.AddBox)//hm.BoxCode为箱码
                                        {
                                            //判断箱码是否出库
                                            var m2 = db.View_OutStorage.FirstOrDefault(p => p.EWMCode == hm.BoxCode && p.EnterpriseID == eId);
                                            if (null != m2)
                                            {
                                                var dealermo = db.Dealer.FirstOrDefault(p => p.Dealer_ID == m2.DealerID);
                                                if (null != dealermo)
                                                {
                                                    #region  查询产品信息

                                                    Enterprise_FWCode_00 codeInfo = GetCodeMode(scanCode); //获取码信息
                                                    if (null != codeInfo)
                                                    {
                                                        var setid = codeInfo.RequestSetID;
                                                        if (null != setid)
                                                        {
                                                            RequestCodeSetting setting =
                                                                db.RequestCodeSetting.FirstOrDefault(p => p.ID == setid);
                                                            if (null != setting)
                                                            {
                                                                Material material = db.Material.FirstOrDefault(p =>
                                                                    p.Material_ID == setting.MaterialID);
                                                                if (material != null)
                                                                {
                                                                    cDealer.materialID = Convert.ToInt64(setting.MaterialID);
                                                                    cDealer.materialName = material.MaterialName;
                                                                }
                                                                else
                                                                {
                                                                    result.retCode = 2;
                                                                    result.retMessage = "获取产品信息失败！";
                                                                    result.isSuccess = false;
                                                                    result.retData = null;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            result.retCode = 2;
                                                            result.retMessage = "获取产品信息失败！";
                                                            result.isSuccess = false;
                                                            result.retData = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.retCode = 2;
                                                        result.retMessage = "获取产品信息失败！";
                                                        result.isSuccess = false;
                                                        result.retData = null;
                                                    }

                                                    #endregion
                                                    result.retCode = 0;
                                                    result.retMessage = "获取成功";
                                                    result.isSuccess = true;
                                                    cDealer.outNO = m2.OutStorageNO;
                                                    cDealer.dealerID = dealermo.Dealer_ID;
                                                    cDealer.dealerName = dealermo.DealerName;
                                                    cDealer.dealerAddress = dealermo.Address;
                                                    result.retData = cDealer;
                                                }
                                            }
                                            else
                                            {
                                                var m3 = db.BindCodeRecords_history.FirstOrDefault(p => p.SingleCode == hm.BoxCode);//查询出垛码
                                                if (null != m3)
                                                {
                                                    var m4 = db.View_OutStorage.FirstOrDefault(p => p.EWMCode == m3.BoxCode);//判断垛码是否出库
                                                    if (null != m4)
                                                    {
                                                        var dealermo = db.Dealer.FirstOrDefault(p => p.Dealer_ID == m4.DealerID);
                                                        if (null != dealermo)
                                                        {
                                                            #region  查询产品信息

                                                            Enterprise_FWCode_00 codeInfo = GetCodeMode(scanCode); //获取码信息
                                                            if (null != codeInfo)
                                                            {
                                                                var setid = codeInfo.RequestSetID;
                                                                if (null != setid)
                                                                {
                                                                    RequestCodeSetting setting =
                                                                        db.RequestCodeSetting.FirstOrDefault(p => p.ID == setid);
                                                                    if (null != setting)
                                                                    {
                                                                        Material material = db.Material.FirstOrDefault(p =>
                                                                            p.Material_ID == setting.MaterialID);
                                                                        if (material != null)
                                                                        {
                                                                            cDealer.materialID = Convert.ToInt64(setting.MaterialID);
                                                                            cDealer.materialName = material.MaterialName;
                                                                        }
                                                                        else
                                                                        {
                                                                            result.retCode = 2;
                                                                            result.retMessage = "获取产品信息失败！";
                                                                            result.isSuccess = false;
                                                                            result.retData = null;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    result.retCode = 2;
                                                                    result.retMessage = "获取产品信息失败！";
                                                                    result.isSuccess = false;
                                                                    result.retData = null;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                result.retCode = 2;
                                                                result.retMessage = "获取产品信息失败！";
                                                                result.isSuccess = false;
                                                                result.retData = null;
                                                            }


                                                            #endregion
                                                            result.retCode = 0;
                                                            result.retMessage = "获取成功";
                                                            result.isSuccess = true;
                                                            cDealer.outNO = m4.OutStorageNO;
                                                            cDealer.dealerID = dealermo.Dealer_ID;
                                                            cDealer.dealerName = dealermo.DealerName;
                                                            cDealer.dealerAddress = dealermo.Address;
                                                            result.retData = cDealer;
                                                        }
                                                        else
                                                        {
                                                            result.retCode = 3;
                                                            result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                                            result.isSuccess = false;
                                                            result.retData = null;
                                                        }
                                                    }
                                                    else
                                                    {

                                                        result.retCode = 3;
                                                        result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                                        result.isSuccess = false;
                                                        result.retData = null;
                                                    }
                                                }
                                                else
                                                {
                                                    result.retCode = 3;
                                                    result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                                    result.isSuccess = false;
                                                    result.retData = null;
                                                }
                                            }

                                        }

                                        #endregion
                                    }
                                }
                                else
                                {
                                    result.retCode = 3;
                                    result.retMessage = "没有此码信息，请确认此码是否已销售出库！";
                                    result.isSuccess = false;
                                    result.retData = null;
                                }
                                #endregion
                            }
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
            catch
            {
                result.retCode = 4;
                result.retMessage = "稽查扫描异常!";
                result.isSuccess = false;
                result.retData = null;
            }
            return result;
        }

        //20200730医疗器械用
        public Enterprise_FWCode_00 GetCodeMode(string ewm)
        {
            Enterprise_FWCode_00 codeinfo = new Enterprise_FWCode_00();
            long routeDataBaseId = 0;
            RequestCode codeModel;
            string[] arr = ewm.Split('.');
            //企业主码
            string EnterpriseCode = arr[0] + "." + arr[1] + "." + arr[2] + "." + arr[3];
            string fixCode = arr[0] + "." + arr[1] + "." + arr[2] + "." + arr[3] + "." + arr[4];
            string CategoryCode = string.Empty;//分类编码
            //生产日期M开头
            string scriqi = string.Empty;
            //有效期V开头
            string youxiaoqi = string.Empty;
            //失效日期E开头
            string shixiaoqi = string.Empty;
            //生产批号L开头
            string scBatchNo = string.Empty;
            //流水号S开头
            string flowNo = string.Empty;
            //灭菌批号D开头
            string mjNo = string.Empty;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    //if (bigPara.Length != 3 && bigPara.Length != 1)//非正常码
                    //{
                    //    return null;
                    //}
                    for (int i = 3; i < arr.Length; i++)
                    {
                        if (arr[i].Substring(0, 1) == "P")//生产日期P开头新规则
                        {
                            scriqi = arr[i].Substring(1, arr[i].Length - 1);
                        }
                        if (arr[i].Substring(0, 1) == "M")//生产日期M开头旧规则
                        {
                            scriqi = arr[i].Substring(1, arr[i].Length - 1);
                        }
                        else
                        {
                        }
                        if (arr[i].Substring(0, 1) == "V")//有效期V开头
                        {
                            youxiaoqi = arr[i].Substring(1, arr[i].Length - 1);
                        }
                        if (arr[i].Substring(0, 1) == "E")//失效日期E开头
                        {
                            shixiaoqi = arr[i].Substring(1, arr[i].Length - 1);
                        }
                        if (arr[i].Substring(0, 1) == "L")//生产批号L开头
                        {
                            scBatchNo = arr[i].Substring(1, arr[i].Length - 1);
                        }
                        if (arr[i].Substring(0, 1) == "D")//灭菌批号D开头
                        {
                            mjNo = arr[i].Substring(1, arr[i].Length - 1);
                        }
                        //if (arr[i].Substring(0, 1) == "S")//流水号S开头
                        //{
                        //    flowNo = arr[i].Substring(1, arr[i].Length - 1);
                        //}
                        if (arr[i].Substring(0, 1) == "S")//流水号S开头
                        {
                            flowNo = arr[i].Substring(1, 5);
                        }
                        if (i == 4)//分类编码
                        {
                            CategoryCode = arr[i].Substring(1, arr[i].Length - 2);
                        }
                    }
                    List<RequestCode> codeModelList = new List<RequestCode>();
                    var data = dataContext.RequestCode.Where(m => m.FixedCode == fixCode);
                    if (!string.IsNullOrEmpty(scriqi))
                    {
                        data = data.Where(m => m.startdate == scriqi);
                    }
                    if (!string.IsNullOrEmpty(youxiaoqi))
                    {
                        data = data.Where(m => m.YouXiaoDate == youxiaoqi);
                    }
                    if (!string.IsNullOrEmpty(shixiaoqi))
                    {
                        data = data.Where(m => m.ShiXiaoDate == shixiaoqi);
                    }
                    if (!string.IsNullOrEmpty(scBatchNo))
                    {
                        data = data.Where(m => m.ShengChanPH == scBatchNo);
                    }
                    if (!string.IsNullOrEmpty(mjNo))
                    {
                        data = data.Where(m => m.dbatchnumber == mjNo);
                    }
                    if (!string.IsNullOrEmpty(flowNo))
                    {
                        data = data.Where(m => m.serialnumber == flowNo);
                    }
                    if (data != null && data.Count() > 0)
                    {
                        codeModel = data.FirstOrDefault();
                        if (codeModel != null)
                        {
                            //查找路由信息
                            if (codeModel.Route_DataBase_ID > 0)
                            {
                                routeDataBaseId = (long)codeModel.Route_DataBase_ID;
                            }
                            #region 查找码库

                            if (routeDataBaseId > 0)
                            {
                                //string tableName;
                                var sub = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == routeDataBaseId);
                                if (sub != null)
                                {
                                    string tableName;
                                    using (DataClassesDataContext dContext = GetDynamicDataContext(routeDataBaseId, out tableName))
                                    {
                                        try
                                        {
                                            string sql = string.Format("select * from {0} where ewm='{1}'", sub.TableName, ewm);
                                            codeinfo = dContext.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();
                                            #region 20190128修改（pc端修改了这批码的产品App出入库不修改，这次更新统一取setting表中的ID）
                                            if (codeinfo != null)
                                            {
                                                //先查找子批次配置信息，如果无子批次，则按照主批次来查询
                                                RequestCodeSetting seting = null;
                                                if (codeinfo.RequestSetID > 0)
                                                {
                                                    seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.ID == codeinfo.RequestSetID);
                                                }
                                                else
                                                {
                                                    //seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.RequestID == codeModel.RequestCode_ID &&
                                                    //   p.beginCode <= index && p.endCode >= index);
                                                    //seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.ShengChanPH == scBatchNo && p.BatchType == 1 && p.RequestID == codeModel.RequestCode_ID);
                                                    seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.BatchType == 1 && p.RequestID == codeModel.RequestCode_ID);
                                                }
                                                if (seting != null)
                                                {
                                                    codeinfo.Material_ID = (long)seting.MaterialID;
                                                    codeinfo.RequestSetID = seting.ID;
                                                }
                                            }
                                            #endregion
                                            return codeinfo;
                                        }
                                        catch (Exception ex)
                                        {
                                            return new Enterprise_FWCode_00();
                                        }
                                    }
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            #endregion

                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return new Enterprise_FWCode_00();
                }
            }
            return codeinfo;
        }
        #region 获取动态链接
        private DataClassesDataContext GetDynamicDataContext(long routeDataBaseId, out string tablename)
        {
            tablename = "";
            DataClassesDataContext result = null;
            try
            {
                string datasource;
                string database;
                string username;
                string pass;
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    long tableId = routeDataBaseId;
                    Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == tableId);
                    if (table == null)
                    {
                        return null;
                    }
                    datasource = table.DataSource;
                    database = table.DataBaseName;
                    username = table.UID;
                    pass = table.PWD;
                    tablename = table.TableName;

                }
                result = GetDataContext(datasource, database, username, pass);
            }
            catch (Exception ex)
            {
                string errData = "CheckDAL.GetDynamicDataContext()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        #endregion

        #region 提交稽查信息
        public InterfaceResult MarketCheck(AddCheckRequest request, string accessToken)
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
                            double timecell = Convert.ToDouble(ConfigurationManager.AppSettings["timecell"]);//时间间隔 单位：小时
                            var mo = db.MarketCheck.Where(p => p.ProductCode == request.codeValue)
                                .OrderByDescending(p => p.CheckTime).FirstOrDefault();
                            if (null != mo)
                            {
                                DateTime dt1 = DateTime.Now;
                                DateTime dt2 = (DateTime)mo.CheckTime;
                                TimeSpan ts = dt1.Subtract(dt2);
                                double hourss = ts.TotalHours;//24.0
                                if (hourss > timecell)
                                {
                                    MarketCheck model = new MarketCheck();
                                    model.ProductCode = request.codeValue;
                                    model.DealerId = request.dealerID;
                                    model.DealerName = request.dealerName;
                                    model.UserId = token.Enterprise_User_ID;
                                    model.UserName = token.UserName;
                                    model.XDealerName = request.checkdealerName;
                                    model.CheckTime = DateTime.Now;
                                    model.Status = request.checkResult;
                                    model.Address = request.dealerAddress;
                                    model.MaterialID = request.materialID;
                                    model.MaterialName = request.materialName;
                                    model.OutStorageNO = request.outNO;
                                    model.EnterpriseID = token.Enterprise_Info_ID;
                                    db.MarketCheck.InsertOnSubmit(model);
                                    db.SubmitChanges();
                                    result.retCode = 0;
                                    result.retMessage = "稽查成功";
                                    result.isSuccess = true;
                                    result.retData = null;
                                }
                                else
                                {
                                    result.retCode = 2;
                                    result.retMessage = "该码已被稽查员扫码!";
                                    result.isSuccess = false;
                                    result.retData = null;
                                }
                            }
                            else
                            {
                                MarketCheck model = new MarketCheck();
                                model.ProductCode = request.codeValue;
                                model.DealerId = request.dealerID;
                                model.DealerName = request.dealerName;
                                model.UserId = token.Enterprise_User_ID;
                                model.UserName = token.UserName;
                                model.XDealerName = request.checkdealerName;
                                model.CheckTime = DateTime.Now;
                                model.Status = request.checkResult;
                                model.Address = request.dealerAddress;
                                model.MaterialID = request.materialID;
                                model.MaterialName = request.materialName;
                                model.OutStorageNO = request.outNO;
                                model.EnterpriseID = token.Enterprise_Info_ID;
                                db.MarketCheck.InsertOnSubmit(model);
                                db.SubmitChanges();
                                result.retCode = 0;
                                result.retMessage = "稽查成功";
                                result.isSuccess = true;
                                result.retData = null;
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
                        result.retMessage = "稽查异常!";
                        result.isSuccess = false;
                        result.retData = null;
                    }
                }
            }
            catch (Exception)
            {
                result.retCode = 3;
                result.retMessage = "稽查异常!";
                result.isSuccess = false;
                result.retData = null;
            }
            return result;
        }
        #endregion

        #region 稽查记录
        public InterfaceResult GetCheckList(CheckRecordRequest request, string accessToken)
        {
            InterfaceResult result = new InterfaceResult();
            CheckRecordResult resultInfo = new CheckRecordResult();
            try
            {
                using (DataClassesDataContext db = GetDataContext())
                {
                    long? eId = 0;
                    Token token = new ApiDAL().TokenDecrypt(accessToken);
                    if (token != null && token.isTokenOK)
                    {
                        eId = token.Enterprise_Info_ID;
                        int pageIndex = 1;//假设页码是从1开始的
                        var list = db.MarketCheck.Where(p => p.EnterpriseID == eId).OrderByDescending(p => p.CheckTime).ToList();
                        if (request.queryTimeType == 1)//近3天
                        {
                            list = list.Where(p => p.CheckTime <= DateTime.Now & p.CheckTime >= DateTime.Now.AddDays(-3)).ToList();
                        }
                        else if (request.queryTimeType == 2)//近7天
                        {
                            list = list.Where(p => p.CheckTime <= DateTime.Now & p.CheckTime >= DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek))) - 7)).ToList();
                        }
                        else if (request.queryTimeType == 3)//近1月
                        {
                            list = list.Where(p => p.CheckTime <= DateTime.Now & p.CheckTime >= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(-1)).ToList();
                        }
                        else if (request.queryTimeType == 4)//近3月
                        {
                            list = list.Where(p => p.CheckTime <= DateTime.Now & p.CheckTime >= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(-3)).ToList();
                        }
                        if (!string.IsNullOrEmpty(request.startDate))//开始时间
                        {
                            list = list.Where(p => p.CheckTime >= DateTime.Parse(request.startDate + " 00:00:00")).ToList();
                        }
                        if (!string.IsNullOrEmpty(request.endDate))//结束时间
                        {
                            list = list.Where(p => p.CheckTime <= DateTime.Parse(request.endDate + " 23:59:59")).ToList();
                        }
                        if (request.pageNumber > 0)
                        {
                            pageIndex = request.pageNumber;
                        }
                        if (list.Any())
                        {
                            int PageSize = 20;//默认值20
                            if (request.pageSize > 0)
                            {
                                PageSize = request.pageSize;
                            }
                            var hs = list.Count;

                            var df = hs / PageSize;
                            if (df < 1)
                            {
                                resultInfo.zys = 1;
                            }
                            else
                            {
                                var isys = hs % PageSize;
                                if (isys > 0)
                                {
                                    resultInfo.zys = df + 1;
                                }
                                else
                                {
                                    resultInfo.zys = df;
                                }
                            }
                            list = list.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                            resultInfo.list = ToCheckList(list);
                            result.retCode = 0;
                            result.retMessage = "稽查记录查询成功！";
                            result.isSuccess = true;
                            result.retData = resultInfo;
                        }
                        else
                        {
                            result.retCode = 0;
                            result.retMessage = "稽查记录暂无数据！";
                            result.isSuccess = true;
                            result.retData = null;
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
            }
            catch
            {
                result.retCode = 2;
                result.retMessage = "稽查记录异常！";
                result.isSuccess = false;
                result.retData = null;
            }
            return result;
        }
        #region 转换
        public static List<MarketRecord> ToCheckList(List<MarketCheck> appList)
        {
            List<MarketRecord> linqList = new List<MarketRecord>();
            foreach (MarketCheck app in appList)
            {
                MarketRecord linq = new MarketRecord();
                linq.ProductCode = app.ProductCode;
                linq.CheckTime = app.CheckTime.ToString();
                if (app.DealerId != null) linq.DealerId = (long)app.DealerId;
                linq.DealerName = app.DealerName;
                linq.XDealerName = app.XDealerName;
                if (app.UserId != null) linq.UserId = (long)app.UserId;
                linq.UserName = app.UserName;
                if (app.Status != null) linq.Status = (int)app.Status;
                linqList.Add(linq);
            }
            return linqList;
        }
        #endregion
        #endregion
    }
}
