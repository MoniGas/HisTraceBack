using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using InterfaceWeb;
using Common.Tools;
using LinqModel;
using System.Data;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace GenCodeServiceAllInOne
{
    public class UpdateDIData
    {
        static WebClient _WebClient = null;
        static string UDI_URL = System.Configuration.ConfigurationManager.AppSettings["UDI_URL"];
        static string UDI_APPID = System.Configuration.ConfigurationManager.AppSettings["UDI_APPID"];
        static string UDI_AppSecret = System.Configuration.ConfigurationManager.AppSettings["UDI_AppSecret"];
        static string UDI_TYSHXYDM = System.Configuration.ConfigurationManager.AppSettings["UDI_TYSHXYDM"];
        static int UDIUpdateTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UDIUpdateTime"]);
        static string requestType = System.Configuration.ConfigurationManager.AppSettings["requestType"];
        static string accessToken = "";
        static string currentTime = "0";
        static GetAllUDIInfo GetAllUDIData = new GetAllUDIInfo();
        static TokenResult resToken = new TokenResult();
        //服务运行时间间隔
        static int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
        //连接库
        static string conStr = System.Configuration.ConfigurationManager.AppSettings["Constr"];

        static int PageCount = 1;
        static int num = 0;
        static string sql = "";

        public static void Start()
        {
            ThreadStart start = DoUpdateDIData;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();


        }
        #region 同步国药监数据服务-新增
        public static void DOGetUDIData()
        {
            Log.WriteLog("同步国药监数据服务启动" + sql, "GetUDIData");
           
            //一旦启动，将会一直执行
            while (true)
            {
                Log.WriteLog("2", "GetUDIData");
                try
                {
                    Log.WriteLog("3", "GetUDIData");
                    #region 计算本次执行后距离下次执行时间（秒数）
                    //当前时间
                    DateTime _nowtime = DateTime.Now;
                    //第二天凌晨1点，由配置文件配置
                    DateTime time = DateTime.Today.AddDays(1).AddHours(Convert.ToInt32(UDIUpdateTime));
                    TimeSpan ts = time.Subtract(_nowtime);
                    int sec = (int)ts.TotalSeconds;
                    #endregion

                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        Log.WriteLog("4", "GetUDIData");
                        conn.Open();
                        while (PageCount <= Convert.ToInt32(GetAllUDIData.totalPageCount))
                        {
                            #region 获取Token
                            if (Convert.ToInt32(currentTime) < Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")))
                            {
                                resToken = GetToken(UDI_APPID, UDI_AppSecret, UDI_TYSHXYDM);
                                if (resToken.returnCode != 1 && resToken.returnCode != 9)
                                {
                                    Log.WriteLog(resToken.returnMsg, "GetUDIData");
                                }
                                else
                                {
                                    accessToken = resToken.accessToken;
                                    currentTime = resToken.currentTime;
                                    Log.WriteLog(resToken.currentTime + "日获取token成功！", "GetUDIData");
                                }

                            }
                            #endregion

                            #region 调用D001接口
                            GetAllUDIData = GetAllUDIInfo(accessToken, PageCount, 1);//调用药监局接口获取医疗器械唯一标识数据 
                            #endregion

                            if (GetAllUDIData.returnCode == 1)
                            {
                                Log.WriteLog("8", "GetUDIData");
                                if (GetAllUDIData.dataSet != null)
                                {
                                    Log.WriteLog("9", "GetUDIData");
                                    //20211220改 by zcx
                                    if (GetAllUDIData.dataSet.deviceInfo.Count > 0)
                                    {
                                        Log.WriteLog("10", "GetUDIData");
                                        foreach (GetAllUDIData item in GetAllUDIData.dataSet.deviceInfo)
                                        {
                                            try
                                            {
                                                using (SqlTransaction trans = conn.BeginTransaction())
                                                {
                                                    SqlCommand cmdBind = conn.CreateCommand();
                                                    cmdBind.Transaction = trans;
                                                    //插入数据

                                                    item.cpmctymc = item.cpmctymc.Contains("'") ? item.cpmctymc.Replace("'", "''") : item.cpmctymc;
                                                    item.ggxh = item.ggxh.Contains("'") ? item.ggxh.Replace("'", "''") : item.ggxh;
                                                    item.ylqxzcrbarmc = item.ylqxzcrbarmc.Contains("'") ? item.ylqxzcrbarmc.Replace("'", "''") : item.ylqxzcrbarmc;
                                                    item.cpms = item.cpms.Contains("'") ? item.cpms.Replace("'", "''") : item.cpms;
                                                    sql = @" select * from dbo.UDIMaterial where MinDI='" + item.zxxsdycpbs + "'";
                                                    DataSet dtConn = new DataSet();
                                                    SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                                                    commandConn.SelectCommand.Transaction = trans;
                                                    commandConn.Fill(dtConn);
                                                    DataView dvEnterpriseInfo = dtConn.Tables[0].DefaultView;

                                                    if (dvEnterpriseInfo.Count <= 0)
                                                    {
                                                        sql = string.Format("INSERT INTO UDIMaterial   ([MinDI] ,[CodeTypeName] ,[MaterialName],[MaterialSpec] ,[EnterpriseName],[BusinessLicence] ,[AddDate] ,[Status],versionNumber,versionTime,versionStatus,deviceRecordKey,zxxsdyzsydydsl,sydycpbs,sfybtzjbs,btcpbsyzxxsdycpbssfyz,btcpbs,cpbsfbrq,spmc,sfwblztlcp,cpms,cphhhbh,qxlb,flbm,yflbm,ylqxzcrbarywmc,zczbhhzbapzbh,cplb,sfbjwycxsy,zdcfsycs,sfwwjbz,syqsfxyjxmj,mjfs,ybbm,cgzmraqxgxx,tscchcztj,tsccsm,scbssfbhph,scbssfbhxlh,scbssfbhscrq,scbssfbhsxrq,qtxxdwzlj,tsrq) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}')", item.zxxsdycpbs, item.cpbsbmtxmc, item.cpmctymc, item.ggxh, item.ylqxzcrbarmc, item.tyshxydm, DateTime.Now, (int)Common.EnumFile.Status.used,
                                                            item.versionNumber, item.versionTime, item.versionStatus, item.deviceRecordKey, item.zxxsdyzsydydsl, item.sydycpbs, item.sfybtzjbs, item.btcpbsyzxxsdycpbssfyz, item.btcpbs, item.cpbsfbrq, item.spmc, item.sfwblztlcp, item.cpms, item.cphhhbh, item.qxlb, item.flbm, item.yflbm, item.ylqxzcrbarywmc, item.zczbhhzbapzbh, item.cplb, item.sfbjwycxsy, item.zdcfsycs, item.sfwwjbz, item.syqsfxyjxmj, item.mjfs, item.ybbm, item.cgzmraqxgxx, item.tscchcztj, item.tsccsm, item.scbssfbhph, item.scbssfbhxlh, item.scbssfbhscrq, item.scbssfbhsxrq, item.qtxxdwzlj, item.tsrq);
                                                        cmdBind.CommandText = sql;
                                                        cmdBind.ExecuteNonQuery();
                                                        if (GetAllUDIData.dataSet.packingInfo.Count > 0)//包装产品标识信息数据
                                                        {
                                                            foreach (var itemPackage in GetAllUDIData.dataSet.packingInfo)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialPackage (deviceRecordKey,bzcpbs,bznhxyjbzcpbs,bznhxyjcpbssl,cpbzjb) values('{0}','{1}','{2}','{3}','{4}')", item.deviceRecordKey, itemPackage.bzcpbs, itemPackage.bznhxyjbzcpbs, itemPackage.bznhxyjcpbssl, itemPackage.cpbzjb);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                        if (GetAllUDIData.dataSet.clinicalInfo.Count > 0)//临床尺寸信息数据
                                                        {
                                                            foreach (var itemClinical in GetAllUDIData.dataSet.clinicalInfo)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialClinical (deviceRecordKey,lcsycclx,ccz,ccdw) values('{0}','{1}','{2}','{3}')", item.deviceRecordKey, itemClinical.lcsycclx, itemClinical.ccz, itemClinical.ccdw);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                        if (GetAllUDIData.dataSet.storageInfo.Count > 0)//存储或操作信息数据
                                                        {
                                                            foreach (var itemStorage in GetAllUDIData.dataSet.storageInfo)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialStorage (deviceRecordKey,cchcztj,zdz,zgz,jldw) values('{0}','{1}','{2}','{3}','{4}')", item.deviceRecordKey, itemStorage.cchcztj, itemStorage.zdz, itemStorage.zgz, itemStorage.jldw);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                        if (item.contactList.Count > 0)//企业联系人信息
                                                        {
                                                            foreach (var itemContact in item.contactList)
                                                            {
                                                                sql = string.Format("INSERT INTO UDIMaterialContact (deviceRecordKey,qylxryx,qylxrdh,qylxrcz) values('{0}','{1}','{2}','{3}')", item.deviceRecordKey, itemContact.qylxryx, itemContact.qylxrdh, itemContact.qylxrcz);
                                                                cmdBind.CommandText = sql;
                                                                cmdBind.ExecuteNonQuery();
                                                            }
                                                        }
                                                        //cmdBind = new SqlCommand(sql, conn);
                                                        //cmdBind.Transaction = trans;
                                                        //cmdBind.ExecuteNonQuery();
                                                    }
                                                    num++;

                                                    trans.Commit();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.WriteLog(ex.Message + "异常sql:" + sql, "GetUDIData");
                                            }
                                        }
                                    }
                                    Log.WriteLog("第" + PageCount + "页同步成功，共同步" + num + "条", "GetUDIData");
                                    num = 0;
                                }
                            }
                            PageCount++;
                            //    trans.Commit();
                            //}
                        }
                    }

                    PageCount = 1;
                    Log.WriteLog("执行完成，共计" + GetAllUDIData.totalRecordCount + "条数据!", "GetUDIData");
                    //每隔一定时间执行一次(1000 * timeSpan * 60 * 60 为一小时)
                    //Thread.Sleep(1000 * timeSpan * 60 * 60 * UDIUpdateTime);

                    DoUpdateDIData();

                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, "GetUDIData");
                }
            }
        } 
        #endregion

        #region 同步国药监数据服务-更新
        public static void DoUpdateDIData()
        {
            Log.WriteLog("同步国药监数据服务启动-更新" + sql, "UpdateDIData");
            //一旦启动，将会一直执行
            while (true)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(conStr))
                    {
                        conn.Open();
                        while (PageCount <= Convert.ToInt32(GetAllUDIData.totalPageCount))
                        {
                            #region 获取Token
                            if (Convert.ToInt32(currentTime) < Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")))
                            {
                                resToken = GetToken(UDI_APPID, UDI_AppSecret, UDI_TYSHXYDM);
                                if (resToken.returnCode != 1 && resToken.returnCode != 9)
                                {
                                    Log.WriteLog(resToken.returnMsg, "UpdateDIData");
                                }
                                else
                                {
                                    accessToken = resToken.accessToken;
                                    currentTime = resToken.currentTime;
                                    Log.WriteLog(resToken.currentTime + "日获取token成功！", "UpdateDIData");
                                }

                            }
                            #endregion

                            #region 调用D001接口,并将数据更新
                            GetAllUDIData = GetAllUDIInfo(accessToken, PageCount, 2)
                                    ;//调用药监局接口获取医疗器械唯一标识数据 
                            if (GetAllUDIData.returnCode == 1)
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
                                                //更新数据
                                                item.cpmctymc = item.cpmctymc.Contains("'") ? item.cpmctymc.Replace("'", "''") : item.cpmctymc;
                                                item.ggxh = item.ggxh.Contains("'") ? item.ggxh.Replace("'", "''") : item.ggxh;
                                                item.ylqxzcrbarmc = item.ylqxzcrbarmc.Contains("'") ? item.ylqxzcrbarmc.Replace("'", "''") : item.ylqxzcrbarmc;
                                                item.cpms = item.cpms.Contains("'") ? item.cpms.Replace("'", "''") : item.cpms;
                                                sql = @" select * from dbo.UDIMaterial where MinDI='" + item.zxxsdycpbs + "'";//根据最小销售单元产品标识查询本地库产品是否存在
                                                DataSet dtConn = new DataSet();
                                                SqlDataAdapter commandConn = new SqlDataAdapter(sql, conn);
                                                commandConn.SelectCommand.Transaction = trans;
                                                commandConn.Fill(dtConn);
                                                DataView dvEnterpriseInfo = dtConn.Tables[0].DefaultView;
                                                if (dvEnterpriseInfo.Count <= 0)//若不存在insert至本地数据库
                                                {
                                                    sql = string.Format("INSERT INTO UDIMaterial   ([MinDI] ,[CodeTypeName] ,[MaterialName],[MaterialSpec] ,[EnterpriseName],[BusinessLicence] ,[AddDate] ,[Status],versionNumber,versionTime,versionStatus,deviceRecordKey,zxxsdyzsydydsl,sydycpbs,sfybtzjbs,btcpbsyzxxsdycpbssfyz,btcpbs,cpbsfbrq,spmc,sfwblztlcp,cpms,cphhhbh,qxlb,flbm,yflbm,ylqxzcrbarywmc,zczbhhzbapzbh,cplb,sfbjwycxsy,zdcfsycs,sfwwjbz,syqsfxyjxmj,mjfs,ybbm,cgzmraqxgxx,tscchcztj,tsccsm,scbssfbhph,scbssfbhxlh,scbssfbhscrq,scbssfbhsxrq,qtxxdwzlj,tsrq) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}')", item.zxxsdycpbs, item.cpbsbmtxmc, item.cpmctymc, item.ggxh, item.ylqxzcrbarmc, item.tyshxydm, DateTime.Now, (int)Common.EnumFile.Status.used,
                                                        item.versionNumber, item.versionTime, item.versionStatus, item.deviceRecordKey, item.zxxsdyzsydydsl, item.sydycpbs, item.sfybtzjbs, item.btcpbsyzxxsdycpbssfyz, item.btcpbs, item.cpbsfbrq, item.spmc, item.sfwblztlcp, item.cpms, item.cphhhbh, item.qxlb, item.flbm, item.yflbm, item.ylqxzcrbarywmc, item.zczbhhzbapzbh, item.cplb, item.sfbjwycxsy, item.zdcfsycs, item.sfwwjbz, item.syqsfxyjxmj, item.mjfs, item.ybbm, item.cgzmraqxgxx, item.tscchcztj, item.tsccsm, item.scbssfbhph, item.scbssfbhxlh, item.scbssfbhscrq, item.scbssfbhsxrq, item.qtxxdwzlj, item.tsrq);
                                                    cmdBind.CommandText = sql;
                                                    cmdBind.ExecuteNonQuery();
                                                    if (GetAllUDIData.dataSet.packingInfo.Count > 0)//包装产品标识信息数据
                                                    {
                                                        foreach (var itemPackage in GetAllUDIData.dataSet.packingInfo)
                                                        {
                                                            sql = string.Format("INSERT INTO UDIMaterialPackage (deviceRecordKey,bzcpbs,bznhxyjbzcpbs,bznhxyjcpbssl,cpbzjb) values('{0}','{1}','{2}','{3}','{4}')", item.deviceRecordKey, itemPackage.bzcpbs, itemPackage.bznhxyjbzcpbs, itemPackage.bznhxyjcpbssl, itemPackage.cpbzjb);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                    if (GetAllUDIData.dataSet.clinicalInfo.Count > 0)//临床尺寸信息数据
                                                    {
                                                        foreach (var itemClinical in GetAllUDIData.dataSet.clinicalInfo)
                                                        {
                                                            sql = string.Format("INSERT INTO UDIMaterialClinical (deviceRecordKey,lcsycclx,ccz,ccdw) values('{0}','{1}','{2}','{3}')", item.deviceRecordKey, itemClinical.lcsycclx, itemClinical.ccz, itemClinical.ccdw);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                    if (GetAllUDIData.dataSet.storageInfo.Count > 0)//存储或操作信息数据
                                                    {
                                                        foreach (var itemStorage in GetAllUDIData.dataSet.storageInfo)
                                                        {
                                                            sql = string.Format("INSERT INTO UDIMaterialStorage (deviceRecordKey,cchcztj,zdz,zgz,jldw) values('{0}','{1}','{2}','{3}','{4}')", item.deviceRecordKey, itemStorage.cchcztj, itemStorage.zdz, itemStorage.zgz, itemStorage.jldw);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                    if (item.contactList.Count > 0)//企业联系人信息
                                                    {
                                                        foreach (var itemContact in item.contactList)
                                                        {
                                                            sql = string.Format("INSERT INTO UDIMaterialContact (deviceRecordKey,qylxryx,qylxrdh,qylxrcz) values('{0}','{1}','{2}','{3}')", item.deviceRecordKey, itemContact.qylxryx, itemContact.qylxrdh, itemContact.qylxrcz);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                }
                                                else//若数据存在则根据MinDI更新数据
                                                {
                                                    #region 产品标识信息数据更新
                                                    StringBuilder strSql = new StringBuilder();
                                                    strSql.AppendFormat("update UDIMaterial set ");
                                                    strSql.AppendFormat("CodeTypeName='{0}',", item.cpbsbmtxmc);//产品标识编码体系名称
                                                    strSql.AppendFormat("MaterialName='{0}',", item.cpmctymc);//产品通用名称
                                                    strSql.AppendFormat("MaterialSpec='{0}',", item.ggxh);//规格型号
                                                    strSql.AppendFormat("EnterpriseName='{0}',", item.ylqxzcrbarmc);//医疗器械注册人/备案人名称
                                                    strSql.AppendFormat("BusinessLicence='{0}',", item.tyshxydm);//统一社会信用代码
                                                    strSql.AppendFormat("Status='{0}',", (int)Common.EnumFile.Status.used);//状态
                                                    strSql.AppendFormat("versionNumber='{0}',", item.versionNumber);//版本号，本产品标识变更次数
                                                    strSql.AppendFormat("versionTime='{0}',", item.versionTime);//版本日期
                                                    strSql.AppendFormat("versionStatus='{0}',", item.versionStatus);//版本状态
                                                    strSql.AppendFormat("deviceRecordKey='{0}',", item.deviceRecordKey);//数据库记录Key
                                                    strSql.AppendFormat("zxxsdyzsydydsl='{0}',", item.zxxsdyzsydydsl);//最小销售单元中使用单元的数量
                                                    strSql.AppendFormat("sydycpbs='{0}',", item.sydycpbs);//使用单元产品标识
                                                    strSql.AppendFormat("sfybtzjbs='{0}',", item.sfybtzjbs);//是否包含本体标识：1是0否
                                                    strSql.AppendFormat("btcpbsyzxxsdycpbssfyz='{0}',", item.btcpbsyzxxsdycpbssfyz);//是否与最小销售单元产品标识是否一致 1是 0否
                                                    strSql.AppendFormat("btcpbs='{0}',", item.btcpbs);//本体产品标识
                                                    strSql.AppendFormat("cpbsfbrq='{0}',", item.cpbsfbrq);
                                                    strSql.AppendFormat("spmc='{0}',", item.spmc);
                                                    strSql.AppendFormat("sfwblztlcp='{0}',", item.sfwblztlcp);
                                                    strSql.AppendFormat("cpms='{0}',", item.cpms);
                                                    strSql.AppendFormat("cphhhbh='{0}',", item.cphhhbh);
                                                    strSql.AppendFormat("qxlb='{0}',", item.qxlb);
                                                    strSql.AppendFormat("flbm='{0}',", item.flbm);
                                                    strSql.AppendFormat("yflbm='{0}',", item.yflbm);
                                                    strSql.AppendFormat("ylqxzcrbarywmc='{0}',", item.ylqxzcrbarywmc);
                                                    strSql.AppendFormat("zczbhhzbapzbh='{0}',", item.zczbhhzbapzbh);
                                                    strSql.AppendFormat("cplb='{0}',", item.cplb);
                                                    strSql.AppendFormat("sfbjwycxsy='{0}',", item.sfbjwycxsy);
                                                    strSql.AppendFormat("zdcfsycs='{0}',", item.zdcfsycs);
                                                    strSql.AppendFormat("sfwwjbz='{0}',", item.sfwwjbz);
                                                    strSql.AppendFormat("syqsfxyjxmj='{0}',", item.syqsfxyjxmj);
                                                    strSql.AppendFormat("mjfs='{0}',", item.mjfs);
                                                    strSql.AppendFormat("ybbm='{0}',", item.ybbm);
                                                    strSql.AppendFormat("cgzmraqxgxx='{0}',", item.cgzmraqxgxx);
                                                    strSql.AppendFormat("tscchcztj='{0}',", item.tscchcztj);
                                                    strSql.AppendFormat("tsccsm='{0}',", item.tsccsm);
                                                    strSql.AppendFormat("scbssfbhph='{0}',", item.scbssfbhph);
                                                    strSql.AppendFormat("scbssfbhxlh='{0}',", item.scbssfbhxlh);
                                                    strSql.AppendFormat("scbssfbhscrq='{0}',", item.scbssfbhscrq);
                                                    strSql.AppendFormat("scbssfbhsxrq='{0}',", item.scbssfbhsxrq);
                                                    strSql.AppendFormat("qtxxdwzlj='{0}',", item.qtxxdwzlj);
                                                    strSql.AppendFormat("tsrq='{0}'", item.tsrq);
                                                    strSql.AppendFormat(" where MinDI='{0}'", item.zxxsdycpbs);
                                                    cmdBind.CommandText = strSql.ToString();
                                                    cmdBind.ExecuteNonQuery();
                                                    #endregion
                                                    #region 包装产品标识信息数据更新
                                                    if (GetAllUDIData.dataSet.packingInfo.Count > 0)//包装产品标识信息数据
                                                    {
                                                        foreach (var itemPackage in GetAllUDIData.dataSet.packingInfo)
                                                        {
                                                            sql = string.Format("UPDATE UDIMaterialPackage SET bzcpbs='{0}',bznhxyjbzcpbs='{1}',bznhxyjcpbssl='{2}',cpbzjb='{3}' WHERE deviceRecordKey='{4}'", itemPackage.bzcpbs, itemPackage.bznhxyjbzcpbs, itemPackage.bznhxyjcpbssl, itemPackage.cpbzjb, item.deviceRecordKey);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                    #endregion
                                                    #region 临床尺寸信息数据更新
                                                    if (GetAllUDIData.dataSet.clinicalInfo.Count > 0)//临床尺寸信息数据
                                                    {
                                                        foreach (var itemClinical in GetAllUDIData.dataSet.clinicalInfo)
                                                        {
                                                            sql = string.Format("UPDATE UDIMaterialClinical SET lcsycclx='{0}',ccz='{1}',ccdw='{2}' WHERE deviceRecordKey='{3}'", itemClinical.lcsycclx, itemClinical.ccz, itemClinical.ccdw, item.deviceRecordKey);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                    #endregion
                                                    #region 存储或操作信息数据更新
                                                    if (GetAllUDIData.dataSet.storageInfo.Count > 0)//存储或操作信息数据
                                                    {
                                                        foreach (var itemStorage in GetAllUDIData.dataSet.storageInfo)
                                                        {
                                                            sql = string.Format("UPDATE UDIMaterialStorage SET cchcztj='{0}',zdz='{1}',zgz='{2}',jldw='{3}' WHERE deviceRecordKey='{4}'", itemStorage.cchcztj, itemStorage.zdz, itemStorage.zgz, itemStorage.jldw, item.deviceRecordKey);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                    #endregion
                                                    #region 企业联系人信息更新
                                                    if (item.contactList.Count > 0)//企业联系人信息
                                                    {
                                                        foreach (var itemContact in item.contactList)
                                                        {
                                                            sql = string.Format("UPDATE UDIMaterialContact SET qylxryx='{0}',qylxrdh='{1}',qylxrcz='{2}' WHERE deviceRecordKey='{3}'", itemContact.qylxryx, itemContact.qylxrdh, itemContact.qylxrcz, item.deviceRecordKey);
                                                            cmdBind.CommandText = sql;
                                                            cmdBind.ExecuteNonQuery();
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                num++;
                                                trans.Commit();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteLog(ex.Message + "异常sql:" + sql, "UpdateDIData");
                                        }
                                    }
                                }
                                Log.WriteLog("第" + PageCount + "页同步成功，共同步" + num + "条", "UpdateDIData");
                                num = 0;
                            }
                            PageCount++;

                            #endregion
                        }
                    }
                    PageCount = 1;
                    Log.WriteLog("执行完成，共计" + GetAllUDIData.totalRecordCount + "条数据!", "UpdateDIData");
                    //每隔一定时间执行一次(1000 * timeSpan * 60 * 60 为一小时)
                    Thread.Sleep(1000 * timeSpan * 60 * 60 * UDIUpdateTime);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, "UpdateDIData");
                }
            }
        } 
        #endregion

        #region P002 获取接口调用凭据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId">应用码</param>
        /// <param name="appSecret">应用授权码</param>
        /// <param name="TYSHXYDM">统一社会信用代码</param>
        /// <returns></returns>
        public static TokenResult GetToken(string appId, string appSecret, string TYSHXYDM)
        {
            TokenResult retResult = new TokenResult();
            if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(appSecret) && !string.IsNullOrEmpty(TYSHXYDM))
            {

                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                try
                {
                    string functionUrl = "/api/v2/token/get";//正式请求链接
                    //string functionUrl = "/api/beta/v2/token/get";//测试请求链接
                    RequestBase r = new RequestBase();
                    r.appId = appId;//应用码
                    r.appSecret = appSecret;//应用授权码
                    r.TYSHXYDM = TYSHXYDM;//统一社会信用代码
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
        #endregion

        #region D001 获取医疗器械唯一标识数据
        /// <summary>
        /// D001 获取医疗器械唯一标识数据
        /// </summary>
        /// <param name="access_token">接口调用凭据</param>
        /// <param name="currentPageNumber">请求分页数，初始请求时，从1开始</param>
        /// <returns></returns>
        public static GetAllUDIInfo GetAllUDIInfo(string access_token, int currentPageNumber, int type)
        {
            Log.WriteLog("5", "GetUDIData");
            GetAllUDIInfo info = new GetAllUDIInfo();
            string rangeValue = "";//请求范围值：当按照天请求数据时，格式： yyyy-MM-dd;当按照月请求数据时，格式： yyyy-MM
            if (!string.IsNullOrEmpty(access_token))
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                try
                {
                    Log.WriteLog("6", "GetUDIData");
                    //2022-06-08 请求范围，1 按天请求，2 按月请求
                    //去掉3 全量请求
                    if (Convert.ToInt32(type) == 1)//按天请求
                    {
                        Log.WriteLog("6.1", "GetUDIData");
                        rangeValue = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    }
                    else if (Convert.ToInt32(type) == 2)//按月请求
                    {
                        Log.WriteLog("6.2", "GetUDIData");
                        rangeValue = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
                    }
                    else
                    {
                        Log.WriteLog("6.3", "GetUDIData");
                        rangeValue = "";
                    }
                    string functionUrl = "/api/v2/sharing/get";//D001 获取医疗器械唯一标识数据-正式地址
                    //string functionUrl = "/api/beta/v2/sharing/get";//D001 获取医疗器械唯一标识数据-测试地址
                    AllUDIRequestBase r = new AllUDIRequestBase();
                    r.accessToken = access_token;//接口调用凭据
                    r.rangeValue = rangeValue;//请求范围值：当按照天请求数据时，格式： yyyy-MM-dd;当按照月请求数据时，格式： yyyy-MM
                    r.requestType = Convert.ToInt32(type);//请求范围，1 按天请求，2 按月请求
                    r.currentPageNumber = currentPageNumber;//请求分页数，初始请求时，从1开始
                    r.dataType = "1";//数据类型：1新发布
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("params", JsonHelper.ObjectToJSON(r));
                    string strResult = WebClient.sendPost(UDI_URL + functionUrl, param, "post");
                    //StreamReader sr = new StreamReader(Application.StartupPath + "\\testdata.txt", false);
                    //string strResult = sr.ReadToEnd().ToString();
                    info = JsonDes.JsonDeserialize<GetAllUDIInfo>(strResult);
                    if (info.returnCode != 1)
                    {
                        Log.WriteLog("7", "GetUDIData");
                        Log.WriteLog(info.returnMsg, "GetAllUDIInfo");
                    }
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

    }
}
