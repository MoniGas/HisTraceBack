using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Argument;
using LinqModel;
using Common.Log;
using Webdiyer.WebControls.Mvc;
using Common.Tools;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using InterfaceWeb;

namespace Dal
{
    public class UDIMaterialDAL : DALBase
    {
        #region
        static string conStr = System.Configuration.ConfigurationManager.AppSettings["WebConnect"];
        WebClient _WebClient = null;
        string UDI_URL = "https://udid.nmpa.gov.cn";
        string UDI_APPID = "f511a6037e6241e69cb129348135fcb1";
        string UDI_AppSecret = "e74712ef573949ecbdeed670db296e16";
        string UDI_TYSHXYDM = System.Configuration.ConfigurationManager.AppSettings["UDI_TYSHXYDM"];
        string requestType = "";
        string rangeValue = "";
        #region
        public RetResult SaveUDIMaterial(DataSet ds, DataSet dsPack, DataSet dsCC, DataSet dsLC, DataSet dsLXR)
        {
            RetResult ret = new RetResult();
            //StringBuilder strBuilder = new StringBuilder();
            try
            {
                using (DataClassesDataContext db = GetDataContext())
                {
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {

                            #region 创建实例
                            DataTable UDIMaterial = new DataTable();
                            UDIMaterial.Columns.AddRange(new DataColumn[]{ 
								new DataColumn("ID",typeof(long)), 
                                new DataColumn("deviceRecordKey",typeof(string)),//0主键编号
                                new DataColumn("versionNumber",typeof(string)),//1公开的版本号
                                new DataColumn("versionTime",typeof(string)),//2版本的发布时间
                                new DataColumn("versionStatus",typeof(string)),//3版本的状态
                                new DataColumn("MinDI",typeof(string)), //4.最小销售单元产品标识
                                new DataColumn("CodeTypeName",typeof(string)),//5.产品标识编码体系名称
                                new DataColumn("cpbsfbrq",typeof(string)),//6.产品标识发布日期
								new DataColumn("zxxsdyzsydydsl",typeof(string)),//7.最小销售单元中使用单元数量
								new DataColumn("sydycpbs",typeof(string)),//8.使用单元产品标识
								new DataColumn("btcpbsyzxxsdycpbssfyz",typeof(string)),//13.本体产品标识与最小销售单元产品标识一致
								new DataColumn("btcpbs",typeof(string)),//14.本体产品标识
								new DataColumn("MaterialName",typeof(string)),//15.产品名称
								new DataColumn("spmc",typeof(string)),//16.商品名称
								new DataColumn("MaterialSpec",typeof(string)),//17.规格型号
                                new DataColumn("sfwblztlcp",typeof(string)),//18.是否为包类/组套类产品
								new DataColumn("cpms",typeof(string)),//19.产品描述
                                new DataColumn("cphhhbh",typeof(string)),//20.产品货号或编号
                                new DataColumn("yflbm",typeof(string)),//21.原分类编码
								new DataColumn("qxlb",typeof(string)),//22.器械类别
								new DataColumn("flbm",typeof(string)),//23.分类编码
								new DataColumn("EnterpriseName",typeof(string)),//24.医疗器械注册人/备案人名称
                                new DataColumn("ylqxzcrbarywmc",typeof(string)),//25.医疗器械注册人/备案人英文名称
								new DataColumn("BusinessLicence",typeof(string)),//26.统一社会信息代码
								new DataColumn("zczbhhzbapzbh",typeof(string)),//27.注册证编号或者备案凭证编号
								new DataColumn("cplb",typeof(string)),//28.产品类别
								new DataColumn("cgzmraqxgxx",typeof(string)),//29.磁共振安全相关信息
								new DataColumn("sfbjwycxsy",typeof(string)),//30.是否标记为一次性使用
								new DataColumn("zdcfsycs",typeof(string)),//31.最大重复使用次数
								new DataColumn("sfwwjbz",typeof(string)),//32.是否为无菌包装
								new DataColumn("syqsfxyjxmj",typeof(string)),//33.使用前是否需要进行灭菌
								new DataColumn("mjfs",typeof(string)),//34.灭菌方式
								new DataColumn("qtxxdwzlj",typeof(string)),//35.其他信息的网址链接
								new DataColumn("ybbm",typeof(string)),//36.医保耗材分类编码
								
								new DataColumn("scbssfbhph",typeof(string)),//37.生产标识是否包含批号
								new DataColumn("scbssfbhxlh",typeof(string)),//38.生产标识是否包含序列号
								new DataColumn("scbssfbhscrq",typeof(string)),//39.生产标识是否包含生产日期
								new DataColumn("scbssfbhsxrq",typeof(string)),//40.生产标识是否包含失效日期
								new DataColumn("tscchcztj",typeof(string)),//41.特殊储存或操作条件
								new DataColumn("tsccsm",typeof(string)),//42.特殊尺寸说明
                                new DataColumn("tsrq",typeof(string)),//43.退市日期
								new DataColumn("AddDate",typeof(DateTime)),//添加日期
								new DataColumn("Status",typeof(int))
							});

                            DataTable UDIMaterialPackage = new DataTable();
                            UDIMaterialPackage.Columns.AddRange(new DataColumn[]{ 
								new DataColumn("ID",typeof(long)), 
                                new DataColumn("deviceRecordKey",typeof(string)), 
                                new DataColumn("bzcpbs",typeof(string)),
								new DataColumn("cpbzjb",typeof(string)),
								new DataColumn("bznhxyjcpbssl",typeof(string)),
                                new DataColumn("bznhxyjbzcpbs",typeof(string))
							});

                            DataTable UDIMaterialStorage = new DataTable();
                            UDIMaterialStorage.Columns.AddRange(new DataColumn[]{ 
								new DataColumn("ID",typeof(long)), 
                                new DataColumn("deviceRecordKey",typeof(string)), 
                                new DataColumn("cchcztj",typeof(string)),
								new DataColumn("zdz",typeof(string)),
								new DataColumn("zgz",typeof(string)),
                                new DataColumn("jldw",typeof(string))
							});

                            DataTable UDIMaterialClinical = new DataTable();
                            UDIMaterialClinical.Columns.AddRange(new DataColumn[]{ 
								new DataColumn("ID",typeof(long)), 
                                new DataColumn("deviceRecordKey",typeof(string)), 
                                new DataColumn("lcsycclx",typeof(string)),
								new DataColumn("ccz",typeof(string)),
								new DataColumn("ccdw",typeof(string))
							});

                            DataTable UDIMaterialContact = new DataTable();
                            UDIMaterialContact.Columns.AddRange(new DataColumn[]{ 
								new DataColumn("ID",typeof(long)), 
                                new DataColumn("deviceRecordKey",typeof(string)), 
                                new DataColumn("qylxrcz",typeof(string)),
								new DataColumn("qylxrdh",typeof(string)),
								new DataColumn("qylxryx",typeof(string))
							});
                            #endregion

                            if (ds != null)
                            {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    string Key = ds.Tables[0].Rows[i][0].ToString().Trim();
                                    string MaterialSpec = ds.Tables[0].Rows[i][17].ToString().Trim();
                                    string MaterialName = ds.Tables[0].Rows[i][15].ToString().Trim();
                                    string cpms = ds.Tables[0].Rows[i][19].ToString().Trim();
                                    string EnterpriseName = ds.Tables[0].Rows[i][24].ToString().Trim();
                                    //var item = db.UDIMaterial.FirstOrDefault(m => m.deviceRecordKey == Key);
                                    //if (item != null) { continue; }
                                    UDIMaterial.Rows.Add(0,
                                        ds.Tables[0].Rows[i][0].ToString().Trim(),
                                        ds.Tables[0].Rows[i][1].ToString().Trim(),
                                        ds.Tables[0].Rows[i][2].ToString().Trim(),
                                        ds.Tables[0].Rows[i][3].ToString().Trim(),
                                        ds.Tables[0].Rows[i][4].ToString().Trim(),
                                        ds.Tables[0].Rows[i][5].ToString().Trim(),
                                        ds.Tables[0].Rows[i][6].ToString().Trim(),
                                        ds.Tables[0].Rows[i][7].ToString().Trim(),
                                        ds.Tables[0].Rows[i][8].ToString().Trim(),
                                        ds.Tables[0].Rows[i][13].ToString().Trim(),
                                        ds.Tables[0].Rows[i][14].ToString().Trim(),
                                        (MaterialName.Contains("'") ? MaterialName.Replace("'", "''") : MaterialName),//15
                                        ds.Tables[0].Rows[i][16].ToString().Trim(),
                                        (MaterialSpec.Contains("'") ? MaterialSpec.Replace("'", "''") : MaterialSpec),//17
                                        ds.Tables[0].Rows[i][18].ToString().Trim(),
                                        (cpms.Contains("'") ? cpms.Replace("'", "''") : cpms),//19
                                        ds.Tables[0].Rows[i][20].ToString().Trim(),
                                        ds.Tables[0].Rows[i][21].ToString().Trim(),
                                        ds.Tables[0].Rows[i][22].ToString().Trim(),
                                        ds.Tables[0].Rows[i][23].ToString().Trim(),
                                        (EnterpriseName.Contains("'") ? EnterpriseName.Replace("'", "''") : EnterpriseName),
                                        ds.Tables[0].Rows[i][25].ToString().Trim(),
                                        ds.Tables[0].Rows[i][26].ToString().Trim(),
                                        ds.Tables[0].Rows[i][27].ToString().Trim(),
                                        ds.Tables[0].Rows[i][28].ToString().Trim(),
                                        ds.Tables[0].Rows[i][29].ToString().Trim(),
                                        ds.Tables[0].Rows[i][30].ToString().Trim(),
                                        ds.Tables[0].Rows[i][31].ToString().Trim(),
                                        ds.Tables[0].Rows[i][32].ToString().Trim(),
                                        ds.Tables[0].Rows[i][33].ToString().Trim(),
                                        ds.Tables[0].Rows[i][34].ToString().Trim(),
                                        ds.Tables[0].Rows[i][35].ToString().Trim(),
                                        ds.Tables[0].Rows[i][36].ToString().Trim(),
                                        ds.Tables[0].Rows[i][37].ToString().Trim(),
                                        ds.Tables[0].Rows[i][38].ToString().Trim(),
                                        ds.Tables[0].Rows[i][39].ToString().Trim(),
                                        ds.Tables[0].Rows[i][40].ToString().Trim(),
                                        ds.Tables[0].Rows[i][41].ToString().Trim(),
                                        ds.Tables[0].Rows[i][42].ToString().Trim(),
                                        ds.Tables[0].Rows[i][43].ToString().Trim(),
                                        DateTime.Now, (int)Common.EnumFile.Status.used
                                    );
                                }
                            }

                            if (dsPack != null)
                            {
                                for (int i = 0; i < dsPack.Tables[0].Rows.Count; i++)
                                {
                                    //string Key = ds.Tables[0].Rows[i][1].ToString().Trim();
                                    //var item = db.UDIMaterialPackage.FirstOrDefault(m => m.deviceRecordKey == Key);
                                    //if (item != null) { continue; }
                                    UDIMaterialPackage.Rows.Add(0,
                                        dsPack.Tables[0].Rows[i][0].ToString().Trim(),
                                        dsPack.Tables[0].Rows[i][1].ToString().Trim(),
                                        dsPack.Tables[0].Rows[i][2].ToString().Trim(),
                                        dsPack.Tables[0].Rows[i][3].ToString().Trim(),
                                        dsPack.Tables[0].Rows[i][4].ToString().Trim()
                                    );
                                }
                            }

                            if (dsCC != null)
                            {
                                for (int i = 0; i < dsCC.Tables[0].Rows.Count; i++)
                                {
                                    //string Key = ds.Tables[0].Rows[i][1].ToString().Trim();
                                    //var item = db.UDIMaterialStorage.FirstOrDefault(m => m.deviceRecordKey == Key);
                                    //if (item != null) { continue; }
                                    UDIMaterialStorage.Rows.Add(0,
                                        dsCC.Tables[0].Rows[i][0].ToString().Trim(),
                                        dsCC.Tables[0].Rows[i][1].ToString().Trim(),
                                        dsCC.Tables[0].Rows[i][2].ToString().Trim(),
                                        dsCC.Tables[0].Rows[i][3].ToString().Trim(),
                                        dsCC.Tables[0].Rows[i][4].ToString().Trim()
                                    );
                                }
                            }

                            if (dsLC != null)
                            {
                                for (int i = 0; i < dsLC.Tables[0].Rows.Count; i++)
                                {
                                    //string Key = ds.Tables[0].Rows[i][1].ToString().Trim();
                                    //var item = db.UDIMaterialClinical.FirstOrDefault(m => m.deviceRecordKey == Key);
                                    //if (item != null) { continue; }
                                    UDIMaterialClinical.Rows.Add(0,
                                        dsLC.Tables[0].Rows[i][0].ToString().Trim(),
                                        dsLC.Tables[0].Rows[i][1].ToString().Trim(),
                                        dsLC.Tables[0].Rows[i][2].ToString().Trim(),
                                        dsLC.Tables[0].Rows[i][3].ToString().Trim()
                                    );
                                }
                            }

                            if (dsLXR != null)
                            {
                                for (int i = 0; i < dsLXR.Tables[0].Rows.Count; i++)
                                {
                                    //string Key = ds.Tables[0].Rows[i][1].ToString().Trim();
                                    //var item = db.UDIMaterialContact.FirstOrDefault(m => m.deviceRecordKey == Key);
                                    //if (item != null) { continue; }
                                    UDIMaterialContact.Rows.Add(0,
                                        dsLXR.Tables[0].Rows[i][0].ToString().Trim(),
                                        dsLXR.Tables[0].Rows[i][1].ToString().Trim(),
                                        dsLXR.Tables[0].Rows[i][2].ToString().Trim(),
                                        dsLXR.Tables[0].Rows[i][3].ToString().Trim()
                                    );
                                }
                            }

                            using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, trans))
                            {
                                if (UDIMaterial != null && UDIMaterial.Columns.Count > 0)
                                {
                                    sbc.DestinationTableName = "UDIMaterial";
                                    foreach (DataColumn dc in UDIMaterial.Columns)    //传入table
                                    {
                                        sbc.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);  //将table中的列与数据库表这的列一一对应
                                    }
                                    sbc.BulkCopyTimeout = 0;  //无限制
                                    sbc.WriteToServer(UDIMaterial);
                                }
                            }

                            using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, trans))
                            {
                                if (UDIMaterialPackage != null && UDIMaterialPackage.Columns.Count > 0)
                                {
                                    sbc.DestinationTableName = "UDIMaterialPackage";
                                    foreach (DataColumn dc in UDIMaterialPackage.Columns)    //传入table
                                    {
                                        sbc.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);  //将table中的列与数据库表这的列一一对应
                                    }
                                    sbc.BulkCopyTimeout = 0;  //无限制
                                    sbc.WriteToServer(UDIMaterialPackage);
                                }
                            }

                            using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, trans))
                            {
                                if (UDIMaterialStorage != null && UDIMaterialStorage.Columns.Count > 0)
                                {
                                    sbc.DestinationTableName = "UDIMaterialStorage";
                                    foreach (DataColumn dc in UDIMaterialStorage.Columns)    //传入table
                                    {
                                        sbc.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);  //将table中的列与数据库表这的列一一对应
                                    }
                                    sbc.BulkCopyTimeout = 0;  //无限制
                                    sbc.WriteToServer(UDIMaterialStorage);
                                }
                            }

                            using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, trans))
                            {
                                if (UDIMaterialClinical != null && UDIMaterialClinical.Columns.Count > 0)
                                {
                                    sbc.DestinationTableName = "UDIMaterialClinical";
                                    foreach (DataColumn dc in UDIMaterialClinical.Columns)    //传入table
                                    {
                                        sbc.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);  //将table中的列与数据库表这的列一一对应
                                    }
                                    sbc.BulkCopyTimeout = 0;  //无限制
                                    sbc.WriteToServer(UDIMaterialClinical);
                                }
                            }

                            using (SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, trans))
                            {
                                if (UDIMaterialContact != null && UDIMaterialContact.Columns.Count > 0)
                                {
                                    sbc.DestinationTableName = "UDIMaterialContact";
                                    foreach (DataColumn dc in UDIMaterialContact.Columns)    //传入table
                                    {
                                        sbc.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);  //将table中的列与数据库表这的列一一对应
                                    }
                                    sbc.BulkCopyTimeout = 0;  //无限制
                                    sbc.WriteToServer(UDIMaterialContact);
                                }
                            }

                            trans.Commit();

                            ret.Code = 0;
                            ret.Msg = "操作成功！";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string errData = "UDIMaterialDAL.SaveUDIMaterial()处理异常";
                ret.Code = -1;
                ret.Msg = errData + ":" + ex.Message;
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return ret;
        }
        #endregion


        #region
        public RetResult GetUDI(string requestTypes, string date)
        {
            RetResult result = new RetResult();
            rangeValue = date;
            requestType = requestTypes;
            string accessToken = "";
            string currentTime = "0";
            GetAllUDIInfo GetAllUDIData = new GetAllUDIInfo();
            TokenResult resToken = new TokenResult();
            int PageCount = 1;
            int num = 0;
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();
                    while (PageCount <= Convert.ToInt32(GetAllUDIData.totalPageCount))
                    {
                        if (Convert.ToInt32(currentTime) < Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")))
                        {
                            resToken = GetToken(UDI_APPID, UDI_AppSecret, UDI_TYSHXYDM);
                            if (resToken.returnCode != 1 && resToken.returnCode != 9)
                            {
                                WriteLog.WriteErrorLog(resToken.returnMsg);
                            }
                            else
                            {
                                accessToken = resToken.accessToken;
                                currentTime = resToken.currentTime;
                                WriteLog.WriteErrorLog(resToken.currentTime + "日获取token成功！");
                            }
                        }
                        GetAllUDIData = GetAllUDIInfo(accessToken, PageCount);
                        if (GetAllUDIData.returnCode == 1)
                        {
                            if (GetAllUDIData.dataSet != null)
                            {
                                //20211220改 by zcx
                                if (GetAllUDIData.dataSet.deviceInfo.Count > 0)
                                {
                                    if (GetAllUDIData.dataSet.deviceInfo.Count > 0)
                                    {
                                        foreach (GetAllUDIData item in GetAllUDIData.dataSet.deviceInfo)
                                        {
                                            try
                                            {
                                                using (SqlTransaction trans = conn.BeginTransaction())
                                                {
                                                    SqlCommand cmdBind = conn.CreateCommand();
                                                    cmdBind.Transaction = trans;

                                                    item.cpmctymc = item.cpmctymc.Contains("'") ? item.cpmctymc.Replace("'", "''") : item.cpmctymc;
                                                    item.ggxh = item.ggxh.Contains("'") ? item.ggxh.Replace("'", "''") : item.ggxh;
                                                    item.ylqxzcrbarmc = item.ylqxzcrbarmc.Contains("'") ? item.ylqxzcrbarmc.Replace("'", "''") : item.ylqxzcrbarmc;
                                                    item.cpms = item.cpms.Contains("'") ? item.cpms.Replace("'", "''") : item.cpms;

                                                    DataSet dtConn = new DataSet();
                                                    DataView dvEnterpriseInfo = new DataView();

                                                    //插入数据
                                                    if (item.versionStatus == "更新")
                                                    {
                                                        DelGYJUDI(item.zxxsdycpbs, item.deviceRecordKey);
                                                    }
                                                    sql = @" select * from dbo.UDIMaterial where MinDI='" + item.zxxsdycpbs + "'";
                                                    SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                                                    commandConn.SelectCommand.Transaction = trans;
                                                    commandConn.Fill(dtConn);
                                                    dvEnterpriseInfo = dtConn.Tables[0].DefaultView;

                                                    if (dvEnterpriseInfo.Count <= 0)
                                                    {
                                                        sql = string.Format("INSERT INTO UDIMaterial   ([MinDI] ,[CodeTypeName] ,[MaterialName],[MaterialSpec] ,[EnterpriseName],[BusinessLicence] ,[AddDate] ,[Status],versionNumber,versionTime,versionStatus,deviceRecordKey,zxxsdyzsydydsl,sydycpbs,sfybtzjbs,btcpbsyzxxsdycpbssfyz,btcpbs,cpbsfbrq,spmc,sfwblztlcp,cpms,cphhhbh,qxlb,flbm,yflbm,ylqxzcrbarywmc,zczbhhzbapzbh,cplb,sfbjwycxsy,zdcfsycs,sfwwjbz,syqsfxyjxmj,mjfs,ybbm,cgzmraqxgxx,tscchcztj,tsccsm,scbssfbhph,scbssfbhxlh,scbssfbhscrq,scbssfbhsxrq,qtxxdwzlj,tsrq) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}')", item.zxxsdycpbs, item.cpbsbmtxmc, item.cpmctymc, item.ggxh, item.ylqxzcrbarmc, item.tyshxydm, DateTime.Now, (int)Common.EnumFile.Status.used,
                                                            item.versionNumber, item.versionTime, item.versionStatus, item.deviceRecordKey, item.zxxsdyzsydydsl, item.sydycpbs, item.sfybtzjbs, item.btcpbsyzxxsdycpbssfyz, item.btcpbs, item.cpbsfbrq, item.spmc, item.sfwblztlcp, item.cpms, item.cphhhbh, item.qxlb, item.flbm, item.yflbm, item.ylqxzcrbarywmc, item.zczbhhzbapzbh, item.cplb, item.sfbjwycxsy, item.zdcfsycs, item.sfwwjbz, item.syqsfxyjxmj, item.mjfs, item.ybbm, item.cgzmraqxgxx, item.tscchcztj, item.tsccsm, item.scbssfbhph, item.scbssfbhxlh, item.scbssfbhscrq, item.scbssfbhsxrq, item.qtxxdwzlj, item.tsrq);
                                                        cmdBind.CommandText = sql;
                                                        cmdBind.ExecuteNonQuery();
                                                        if (GetAllUDIData.dataSet.packingInfo.Count > 0)
                                                        {
                                                            foreach (var itemPackage in GetAllUDIData.dataSet.packingInfo)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialPackage (deviceRecordKey,bzcpbs,bznhxyjbzcpbs,bznhxyjcpbssl,cpbzjb) values('{0}','{1}','{2}','{3}','{4}')", item.deviceRecordKey, itemPackage.bzcpbs, itemPackage.bznhxyjbzcpbs, itemPackage.bznhxyjcpbssl, itemPackage.cpbzjb);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                        if (GetAllUDIData.dataSet.clinicalInfo.Count > 0)
                                                        {
                                                            foreach (var itemClinical in GetAllUDIData.dataSet.clinicalInfo)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialClinical (deviceRecordKey,lcsycclx,ccz,ccdw) values('{0}','{1}','{2}','{3}','{4}')", item.deviceRecordKey, itemClinical.lcsycclx, itemClinical.ccz, itemClinical.ccdw);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                        if (GetAllUDIData.dataSet.storageInfo.Count > 0)
                                                        {
                                                            foreach (var itemStorage in GetAllUDIData.dataSet.storageInfo)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialStorage (deviceRecordKey,cchcztj,zdz,zgz,jldw) values('{0}','{1}','{2}','{3}','{4}')", item.deviceRecordKey, itemStorage.cchcztj, itemStorage.zdz, itemStorage.zgz, itemStorage.jldw);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                        if (item.contactList.Count > 0)
                                                        {
                                                            foreach (var itemContact in item.contactList)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialContact (deviceRecordKey,qylxryx,qylxrdh,qylxrcz) values('{0}','{1}','{2}','{3}')", item.deviceRecordKey, itemContact.qylxryx, itemContact.qylxrdh, itemContact.qylxrcz);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                    }
                                                    num++;

                                                    trans.Commit();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                WriteLog.WriteErrorLog(ex.Message + "异常sql:" + sql);
                                            }
                                        }
                                    }
                                }
                                WriteLog.WriteErrorLog("第" + PageCount + "页同步成功，共同步" + num + "条");
                                num = 0;
                            }
                        }
                        PageCount++;
                    }
                }

                PageCount = 1;
                WriteLog.WriteErrorLog("执行完成，共计" + GetAllUDIData.totalRecordCount + "条数据!");
                result.Code = 0;
                result.Msg = "执行完成，共计" + GetAllUDIData.totalRecordCount + "条数据!";
                return result;
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("异常信息：" + ex.Message);
                result.Code = -1;
                result.Msg = "异常信息：" + ex.Message;
                return result;
            }
        }

        public void DelGYJUDI(string DI, string Key)
        {
            try
            {
                string sql = "";
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        SqlCommand cmdBind = conn.CreateCommand();
                        cmdBind.Transaction = trans;
                        sql = string.Format(" delete from UDIMaterial where MinDI='" + DI + "';");
                        cmdBind.CommandText = sql;
                        cmdBind.ExecuteNonQuery();
                        sql = string.Format(" delete from UDIMaterialPackage where deviceRecordKey='" + Key + "'");
                        cmdBind.CommandText = sql;
                        cmdBind.ExecuteNonQuery();
                        sql = string.Format(" delete from UDIMaterialContact where deviceRecordKey='" + Key + "'");
                        cmdBind.CommandText = sql;
                        cmdBind.ExecuteNonQuery();
                        sql = string.Format(" delete from UDIMaterialStorage where deviceRecordKey='" + Key + "'");
                        cmdBind.CommandText = sql;
                        cmdBind.ExecuteNonQuery();
                        sql = string.Format(" delete from UDIMaterialClinical where deviceRecordKey='" + Key + "'");
                        cmdBind.CommandText = sql;
                        cmdBind.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog("异常信息：" + ex.Message);
            }
        }


        public TokenResult GetToken(string appId, string appSecret, string TYSHXYDM)
        {
            TokenResult retResult = new TokenResult();
            if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(appSecret))
            {

                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                try
                {
                    string functionUrl = "/api/v2/token/get";
                    RequestBase r = new RequestBase();
                    r.appId = appId;
                    r.appSecret = appSecret;
                    r.TYSHXYDM = TYSHXYDM;
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("params", JsonHelper.ObjectToJSON(r));
                    string strResult = WebClient.sendPost(UDI_URL + functionUrl, param, "post");
                    strResult = strResult.Replace("\\", "");
                    retResult = JsonDes.JsonDeserialize<TokenResult>(strResult);
                    return retResult;
                }
                catch (Exception ex)
                {
                    retResult.returnCode = -1;
                    retResult.returnMsg = ex.Message;
                    return retResult;
                }
            }
            return retResult;
        }

        public GetAllUDIInfo GetAllUDIInfo(string access_token, int currentPageNumber)
        {
            GetAllUDIInfo info = new GetAllUDIInfo();

            if (!string.IsNullOrEmpty(access_token))
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                try
                {
                    string functionUrl = "/api/v2/sharing/get";
                    AllUDIRequestBase r = new AllUDIRequestBase();
                    r.accessToken = access_token;
                    r.rangeValue = rangeValue;
                    r.requestType = Convert.ToInt32(requestType);
                    r.currentPageNumber = currentPageNumber;
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("params", JsonHelper.ObjectToJSON(r));
                    string strResult = WebClient.sendPost(UDI_URL + functionUrl, param, "post");
                    info = JsonDes.JsonDeserialize<GetAllUDIInfo>(strResult);

                }
                catch (Exception ex)
                {
                    info.returnCode = -1;
                    info.returnMsg = ex.Message;
                }
            }
            return info;
        }

        #endregion

        #region 根据企业名称和版本日期获取最新UDI数据
        public UDIMaterialInfo GetMaterialDIByEnterprise(string enterpriseName, string synchroDate, string page)
        {
            var model = new UDIMaterialInfo();
            using (DataClassesDataContext db = GetDataContext())
            {
                try
                {
                    int totalcount=db.UDIMaterial.Where(t => t.EnterpriseName == enterpriseName && t.versionTime == synchroDate).Count();//查出总数
                    var list = db.UDIMaterial.Where(t => t.EnterpriseName == enterpriseName && t.versionTime == synchroDate).Skip(100 * (int.Parse(page) - 1)).Take(100).ToList();//根据企业名称和版本日期查出需要更新的数据
                    model.pageNum = (totalcount + 100 - 1) / 100;//页数
                    model.UDIData = list;
                    ClearLinqModel(list);
                    
                }
                catch (Exception ex)
                {

                    string errData = "打码客户端同步UDI数据";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return model;

        } 
        
        #endregion

        #endregion

        #region UDIKey数据处理
        public PagedList<UDIKey> GetUDIKeyList(string beginDate, string endDate, int? pageIndex)
        {
            PagedList<UDIKey> dataInfoList = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.UDIKey
                                   select data;
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        dataList = dataList.Where(m => m.EndDate >= Convert.ToDateTime(beginDate));
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dataList = dataList.Where(m => m.EndDate <= Convert.ToDateTime(endDate).AddDays(1));
                    }
                    dataInfoList = dataList.OrderByDescending(m => m.ID).ToPagedList(pageIndex ?? 1, PageSize);
                    ClearLinqModel(dataInfoList);
                }
            }
            catch { }
            return dataInfoList;
        }

        /// <summary>
        /// 添加key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RetResult Add(UDIKey model)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    string setDatestr = DateTime.Now.ToString("yyyyMMdd");
                    string keyCode = setDatestr + "&" + "hbgl" + "&" + Guid.NewGuid().ToString();
                    byte[] key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["key"]);
                    byte[] iv = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["iv"]);
                    keyCode = SecurityDES.Encrypt(keyCode, key, iv);
                    model.UDIKey1 = keyCode;
                    dataContext.UDIKey.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
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

        public UDIKey GetKeyInfo(long Id)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.UDIKey
                                where d.ID == Id
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

        public RetResult Edit(UDIKey model)
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    UDIKey temp = dataContext.UDIKey.FirstOrDefault(p => p.ID == model.ID);
                    if (temp != null)
                    {
                        temp.AddReason = model.AddReason;
                        temp.LinkMan = model.LinkMan;
                        temp.LinkPhone = model.LinkPhone;
                        temp.EndDate = model.EndDate;
                        temp.MaterialCount = model.MaterialCount;
                        dataContext.SubmitChanges();
                        Msg = "保存成功！";
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        Msg = "未找到要修改的信息！";
                        error = CmdResultError.NONE;
                        Ret.SetArgument(error, Msg, Msg);
                        return Ret;
                    }
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

        public RetResult EditStatus(long id, int type)
        {
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    UDIKey model = dataContext.UDIKey.FirstOrDefault(m => m.ID == id);
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
        #endregion


    }
}
