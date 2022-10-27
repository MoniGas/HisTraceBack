using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Argument;
using LinqModel;
using Common.Tools;
using Common.Log;
using System.Data.SqlClient;
using System.Data;

namespace Dal
{
	public class DataDAL : DALBase
	{
		//public RetResult SetData()
		//{
		//    RetResult result = new RetResult();
		//    result.CmdError = CmdResultError.EXCEPTION;
		//    result.Msg = "操作失败";
		//    using (DataClassesDataContext dataContext = GetDataContext())
		//    {
		//        try
		//        {
		//            //long s = 470;
		//            List<MaterialDI> diList = dataContext.MaterialDI.OrderByDescending(p => p.EnterpriseID).ToList();
		//            foreach (MaterialDI sub in diList)
		//            {
		//                Material material = dataContext.Material.Where(p => p.Material_ID == sub.MaterialID).FirstOrDefault();
		//                if (material == null)
		//                    material = new Material();
		//                if (material != null || material.Enterprise_Info_ID != sub.EnterpriseID)
		//                {

		//                    string errData = "【" + DateTime.Now.ToString() + "】企业ID：" + sub.EnterpriseID + "DI的ID：" + sub.ID;
		//                    WriteLog.WriteSeriveLog(errData, "SetData");
		//                    errData = "企业ID：" + sub.EnterpriseID + "；产品ID：" + material.Material_ID + "；DI的ID：" + sub.ID;
		//                    WriteLog.WriteSeriveLog(errData, "SetData");
		//                    Material model = dataContext.Material.Where(p => p.Enterprise_Info_ID == sub.EnterpriseID
		//                        && p.MaterialName == sub.MaterialName && p.Status == (int)Common.EnumFile.Status.used).FirstOrDefault();
		//                    if (model != null)
		//                    {
		//                        sub.MaterialID = model.Material_ID;
		//                        dataContext.SubmitChanges();
		//                        errData = "新产品ID：" + model.Material_ID + "；DI的ID：" + sub.ID;
		//                        WriteLog.WriteSeriveLog(errData, "SetData");
		//                    }
		//                    else
		//                    {
		//                        model = new Material();
		//                        string materialCode = string.Empty;
		//                        int count = dataContext.Material.Where(m => m.Enterprise_Info_ID == sub.EnterpriseID).Count();
		//                        if (count > 0)
		//                        {
		//                            int macodeS = count + 1;
		//                            materialCode = new BinarySystem36().gen36No((macodeS), 3);
		//                        }
		//                        else
		//                        {
		//                            string macode = "001";
		//                            int codeNum = Convert.ToInt32(macode.Replace("0", ""));
		//                            materialCode = new BinarySystem36().gen36No(codeNum, 3);
		//                        }
		//                        model.Material_Code = materialCode;
		//                        model.type = (int)Common.EnumFile.Materialtype.AutoCode;
		//                        model.MaterialName = sub.MaterialName;
		//                        model.Enterprise_Info_ID = sub.EnterpriseID;
		//                        model.BZSpecType = Convert.ToInt32(sub.Specifications);
		//                        model.adddate = DateTime.Now;
		//                        model.type = 0;
		//                        model.MaterialFullName = sub.MaterialName;
		//                        model.Status = (int)Common.EnumFile.Status.used;
		//                        //model.MaterialBarcode =MaterialDAL.GetCode(dataContext);
		//                        dataContext.Material.InsertOnSubmit(model);
		//                        dataContext.SubmitChanges();
		//                        sub.MaterialID = model.Material_ID;
		//                        dataContext.SubmitChanges();
		//                        errData = "新加产品ID：" + model.Material_ID + "；DI的ID" + sub.ID;
		//                        WriteLog.WriteSeriveLog(errData, "SetData");
		//                    }
		//                }
		//            }
		//        }
		//        catch (Exception ex)
		//        {
		//            string errData = "Material表异常信息：" + ex.Message;
		//            WriteLog.WriteSeriveLog(errData, "SetData");
		//        }
		//    }
		//    return result;
		//}

		////更新requestcode,setting表
		//public RetResult UpdateRequest()
		//{
		//    RetResult result = new RetResult();
		//    result.CmdError = CmdResultError.EXCEPTION;
		//    result.Msg = "操作失败";
		//    string errData = "";
		//    int num = 0;
		//    int codeNum = 0;
		//    string sql = "";
		//    try
		//    {
		//        using (DataClassesDataContext dataContext = GetDataContext())
		//        {
		//            try
		//            {
		//                errData = "DateTime.Now.ToString()---开始执行RequestCode表更新数据；";
		//                WriteLog.WriteSeriveLog(errData, "RequestCode");
		//                //查询所有RequestCode表数据
		//                List<RequestCode> requestCodeLst = dataContext.RequestCode.ToList();
		//                foreach (var item in requestCodeLst)
		//                {
		//                    //根据RequestCode表DI在MaterialDI表中查找信息
		//                    MaterialDI MaterialDI = dataContext.MaterialDI.Where(m => m.MaterialUDIDI == item.FixedCode).FirstOrDefault();
		//                    if (MaterialDI != null)
		//                    {
		//                        //如果RequestCode表Material_ID不等于MaterialDI表的MaterialID，更新RequestCode表Material_ID
		//                        if (item.Material_ID != MaterialDI.MaterialID)
		//                        {
		//                            errData = DateTime.Now.ToString() + "---RequestCode表ID值：" + item.RequestCode_ID + "；企业ID：" + MaterialDI.EnterpriseID + ";RequestCode表原Material_ID：" + item.Material_ID + ";MaterialDI表产品ID：" + MaterialDI.MaterialID + ";RequestCode表修改后的产品ID值：" + MaterialDI.MaterialID;
		//                            WriteLog.WriteSeriveLog(errData, "RequestCode");
		//                            item.Material_ID = MaterialDI.MaterialID;
		//                            dataContext.SubmitChanges();
		//                            num += 1;

		//                            //修改码库产品ID,查找路由表
		//                            Route_DataBase Route_DataBase = dataContext.Route_DataBase.Where(m => m.isuse == 1 && m.Route_DataBase_ID == item.Route_DataBase_ID).FirstOrDefault();
		//                            if (Route_DataBase != null)
		//                            {
		//                                ///码库链接字符串
		//                                string codeConstr = string.Format(@"Data Source={0};database={1};UID={2};pwd={3};",
		//                                 Route_DataBase.DataSource, Route_DataBase.DataBaseName,
		//                                 Route_DataBase.UID, Route_DataBase.PWD);
		//                                ///码表名称
		//                                string tableName = Route_DataBase.TableName;
		//                                using (SqlConnection connCode = new SqlConnection(codeConstr))
		//                                {
		//                                    connCode.Open();
		//                                    DataSet dsCode = new DataSet();
		//                                    sql = string.Format(@"select *  from  {0} where RequestCode_ID={1}", tableName, item.RequestCode_ID);
		//                                    SqlDataAdapter commandCode = new SqlDataAdapter(sql, connCode);
		//                                    commandCode.Fill(dsCode, "dsCode");
		//                                    DataView dvCode = dsCode.Tables[0].DefaultView;
		//                                    string FWCodeID = "";
		//                                    for (int i = 0; i < dvCode.Count; i++)
		//                                    {
		//                                        string Enterprise_FWCode_ID = dvCode[i]["Enterprise_FWCode_ID"].ToString();
		//                                        FWCodeID += Enterprise_FWCode_ID + ",";
		//                                        sql = string.Format(@"update {0} set Material_ID={1} where Enterprise_FWCode_ID={2}", tableName, MaterialDI.MaterialID, Enterprise_FWCode_ID);
		//                                        SqlCommand cmd = new SqlCommand(sql, connCode);
		//                                        int k = cmd.ExecuteNonQuery();
		//                                        if (k > 0)
		//                                        {
		//                                            codeNum += 1;
		//                                        }
		//                                        else
		//                                        {
		//                                            WriteLog.WriteSeriveLog(DateTime.Now.ToString() + "---失败的sql语句：" + sql, "codeData");
		//                                        }
		//                                    }
		//                                    errData = DateTime.Now.ToString() + "---RequestCode表ID值：" + item.RequestCode_ID + "；企业ID：" + MaterialDI.EnterpriseID + ";该二维码原产品ID值：" + dvCode[0]["Material_ID"].ToString() + "；修改该二维码产品ID为：" + MaterialDI.MaterialID + ";修改码库数量：" + dvCode.Count + "；修改码库ID值为：" + FWCodeID;
		//                                    WriteLog.WriteSeriveLog(errData, "codeData");
		//                                }
		//                            }
		//                        }
		//                    }
		//                }
		//                errData = "DateTime.Now.ToString()---RequestCode表共更新" + num + "条数据；";
		//                WriteLog.WriteSeriveLog(errData, "RequestCode");
		//            }
		//            catch (Exception ex) 
		//            {
		//                errData = "RequestCode、码库表异常信息：" + ex.Message;
		//                WriteLog.WriteSeriveLog(errData, "RequestCode");
		//            }
					
		//            try
		//            {
		//                errData = "DateTime.Now.ToString()---开始执行RequestCodeSetting表更新数据；";
		//                WriteLog.WriteSeriveLog(errData, "Setting");
		//                num = 0;
		//                //查询所有RequestCodeSetting表数据
		//                List<RequestCodeSetting> RequestCodeSettingLst = dataContext.RequestCodeSetting.ToList();
		//                foreach (var item in RequestCodeSettingLst)
		//                {
		//                    RequestCode RequestCode = dataContext.RequestCode.Where(m => m.RequestCode_ID == item.RequestID).FirstOrDefault();
		//                    if (RequestCode != null)
		//                    {
		//                        if (item.MaterialID != RequestCode.Material_ID)
		//                        {
		//                            errData = DateTime.Now.ToString() + "---RequestCodeSetting表ID值：" + item.ID + "；企业ID：" + RequestCode.Enterprise_Info_ID + ";RequestCodeSetting表原MaterialID：" + item.MaterialID + ";RequestCode表产品ID：" + RequestCode.Material_ID + ";RequestCodeSetting表修改后的产品ID值：" + RequestCode.Material_ID;
		//                            WriteLog.WriteSeriveLog(errData, "Setting");
		//                            item.MaterialID = RequestCode.Material_ID;
		//                            dataContext.SubmitChanges();
		//                            num += 1;
		//                        }
		//                    }
		//                }
		//                errData = "DateTime.Now.ToString()---RequestCodeSetting表共更新" + num + "条数据；";
		//                WriteLog.WriteSeriveLog(errData, "Setting");
		//            }
		//            catch (Exception ex)
		//            {
		//                errData = "RequestCodeSetting表异常信息：" + ex.Message;
		//                WriteLog.WriteSeriveLog(errData, "Setting");
		//            }

		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        errData = "UpdateRequest方法异常信息：" + ex.Message;
		//        WriteLog.WriteSeriveLog(errData, "UpdateRequest");
		//    }
		//    return result;
		//}

		////更新码库数据
		//public RetResult UpdateCode()
		//{
		//    RetResult result = new RetResult();
		//    result.CmdError = CmdResultError.EXCEPTION;
		//    result.Msg = "操作失败";
		//    return result;
		//}

	}
}
