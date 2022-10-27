using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Argument;
using LinqModel;
using System.Configuration;

namespace Dal
{
    public class UpdateSellDAL : DALBase
    {
        //处理开始时间
        DateTime dateTime = Convert.ToDateTime(ConfigurationManager.AppSettings["SellInfoDate"]);
        public RetResult OutStorage()
        {
            string Msg = "操作失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    //查找出库和经销商出库的记录
                    OutStorageTable outTable = new OutStorageTable();
                    List<OutStorageDetail> data = dataContext.OutStorageDetail.Where(m => (m.IsOutStore == (int)Common.EnumFile.IsOutStore.outStore
                        || m.IsOutStore == (int)Common.EnumFile.IsOutStore.dealerOutStore) &&
                        m.ServicesStatus == null && m.DealerID>0
                        && m.OutDate > dateTime).OrderBy(m => m.EWMCode).ToList();
                    foreach (OutStorageDetail sub in data)
                    {
                        if (sub.OutStorageID != outTable.OutStorageID)
                        {
                            outTable = dataContext.OutStorageTable.Where(p => p.OutStorageID == sub.OutStorageID).FirstOrDefault();
                        }
                        if (outTable != null)
                        {
                            bool value = GetCodeInfo(sub.EWMCode, Convert.ToInt64(sub.Material_ID), Convert.ToInt64(outTable.DealerID), Convert.ToString(outTable.OutStorageDate));
                            if (value == true)
                            {
                                sub.ServicesStatus = (int)Common.EnumFile.ServicesStatus.pass;
                                dataContext.SubmitChanges();
                            }
                        }
                    }
                    List<OutStorageMaterial> dataMaterial = dataContext.OutStorageMaterial.Where(m => (m.Status == (int)Common.EnumFile.IsOutStore.outStore
                        || m.Status == (int)Common.EnumFile.IsOutStore.dealerOutStore)
                        && m.ServicesStatus == null && m.DealerID>0
                        && m.OutDate > dateTime).OrderBy(m => m.Ewm).ToList();
                    foreach (OutStorageMaterial subM in dataMaterial)
                    {
                        bool value = GetCodeInfo(subM.Ewm, Convert.ToInt64(subM.Material_ID), Convert.ToInt64(subM.DealerID), Convert.ToString(subM.OutDate));
                        if (value == true)
                        {
                            subM.ServicesStatus = (int)Common.EnumFile.ServicesStatus.pass;
                            dataContext.SubmitChanges();
                        }
                    }
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        public RetResult OutStorageBack()
        {
            string Msg = "操作失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    //查找出库和经销商出库的记录
                    OutStorageTable outTable = new OutStorageTable();
                    List<OutStorageDetail> data = dataContext.OutStorageDetail.Where(m => m.IsOutStore == (int)Common.EnumFile.IsOutStore.outStoreBack &&
                        m.ServicesStatus == (int)Common.EnumFile.ServicesStatus.pass && m.DealerID > 0
                        && m.OutDate > dateTime).OrderBy(m => m.EWMCode).ToList();
                    foreach (OutStorageDetail sub in data)
                    {
                        bool value = GetCodeInfo(sub.EWMCode, Convert.ToInt64(sub.Material_ID),0, "");
                        if (value == true)
                        {
                            sub.ServicesStatus = (int)Common.EnumFile.ServicesStatus.backPass;
                            dataContext.SubmitChanges();
                        }
                    }
                    List<OutStorageMaterial> dataMaterial = dataContext.OutStorageMaterial.Where(m => m.Status == (int)Common.EnumFile.IsOutStore.outStoreBack
                        && m.ServicesStatus == (int)Common.EnumFile.ServicesStatus.pass && m.DealerID > 0
                        && m.OutDate > dateTime).OrderBy(m => m.Ewm).ToList();
                    foreach (OutStorageMaterial subM in dataMaterial)
                    {
                        bool value = GetCodeInfo(subM.Ewm, Convert.ToInt64(subM.Material_ID), 0,"");
                        if (value == true)
                        {
                            subM.ServicesStatus = (int)Common.EnumFile.ServicesStatus.backPass;
                            dataContext.SubmitChanges();
                        }
                    }
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #region 更新二维码销售信息
        public bool GetCodeInfo(string ewm, long materialId, long dealerId, string sellDate)
        {
            bool value = false;
            string[] arr = ewm.Split('.');
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
                #region 查找路由
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
                    if (arr[i].Substring(0, 1) == "S")//流水号S开头
                    {
                        flowNo = arr[i].Substring(1, arr[i].Length - 1);
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
                #endregion
                if (data != null && data.Count() > 0)
                {
                    List<RequestCode> list = data.ToList();
                    foreach (RequestCode codeModel in list)
                    {
                        var database = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == (long)codeModel.Route_DataBase_ID);
                        if (database != null)
                        {
                            string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};", database.DataSource,
                                       database.DataBaseName, database.UID, database.PWD);
                            using (DataClassesDataContext dataContextDynamic = GetContext(conStr))
                            {
                                string sql = string.Format("select * from {0} where ewm='{1}'", database.TableName, ewm);
                                Enterprise_FWCode_00 codeinfo = dataContextDynamic.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();
                                if (codeinfo != null)
                                {
                                    int status = (int)Common.EnumFile.UsingStateCode.HasBeenUsed;
                                    string update = string.Format("update {0} set ScanCount=0,FWCount=0,ValidateTime=NULL,Material_ID={1},Dealer_ID={2} ",
                                     database.TableName,materialId, dealerId);
                                    if (!string.IsNullOrEmpty(sellDate))
                                    {
                                        update = update + " , SalesTime='" + sellDate + "' , UseTime='" + DateTime.Now + "' ,Status='"+status+"'";
                                    }
                                    else
                                    {
                                        status = (int)Common.EnumFile.UsingStateCode.NotUsed;
                                        update = update + " ,  SalesTime=null  ,  UseTime=null  ,Status='" + status + "'";
                                    }
                                    update = update + "  where ewm='"+ewm+"'";
                                    dataContextDynamic.ExecuteCommand(update);
                                    value = true;
                                    return value;
                                }
                            }
                        }
                    }
                }
            }
            return value;
        }
        #endregion
    }
}
