/********************************************************************************
** 作者： 赵慧敏v2.5版本修改
** 创始时间：2017-2-09
** 联系方式 :15031109901
** 描述：拍码追溯页面
** 版本：v2.5
** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Argument;
using Common.Log;
using LinqModel;
using Common.Tools;
using System.Text.RegularExpressions;

namespace Dal
{
    public class ScanCodeDAL : DALBase
    {
        string imgDefault = "/Content/wap/images/nopic1.jpg";

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
                string errData = "ScanCodeDAL.GetDynamicDataContext()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }
        #endregion

        #region 更新码的拍码及验证次数
        /// <summary>
        /// 更新码的拍码及验证次数
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="scanCount">查看次数是否加1</param>
        /// <param name="fwCount">验证次数是否加1</param>
        /// <param name="count">二维码验证次数</param>
        /// <returns>返参结果实例</returns>
        public RetResult UpdateCount(string ewm, bool scanCount, bool fwCount, int? count)
        {
            try
            {
                string[] bigPara = ewm.Split('/');
                long Route_DataBase_ID = 0;
                string tableName = "";
                RequestCode codeModel = new RequestCode();
                if (fwCount)
                {
                    int codeType = 0;
                    using (DataClassesDataContext dataContext = GetDataContext())
                    {
                        //IDcode码
                        if (bigPara.Length == 3)
                        {
                            string[] smallPara = bigPara[2].Split('.');
                            if (smallPara.Length == 3)
                            {
                                codeType = CodeNodeToType.ToType(smallPara[2]);
                            }
                            long index = 0;
                            index = smallPara[1].Length == 3 ?
                                new BinarySystem62().Convert62ToNo(smallPara[1]) : Convert.ToInt64(smallPara[1]);
                            string fixedCode = bigPara[0] + "/" + bigPara[1] + "/" + smallPara[0];
                            codeModel = dataContext.RequestCode.FirstOrDefault(
                                m => m.Type == codeType && m.StartNum <= index && m.EndNum >= index
                                && m.FixedCode == fixedCode);
                        }
                        else if (ewm.Length == 32)//农药码32位固定
                        {
                            codeType = (int)Common.EnumFile.GenCodeType.pesticides;
                            string fixedCode = ewm.Substring(0, ewm.Length - 6);
                            //随机农药码
                            string scategoryIdcode = ewm.Substring(11, 6);
                            codeModel = dataContext.RequestCode.FirstOrDefault(
                                m => m.Type == codeType && m.FixedCode == fixedCode);
                        }
                        else//简码
                        {
                            long index = 0;
                            string fixedCode = ewm.Substring(0, 11);
                            BinarySystem36 binary = new BinarySystem36();
                            index = binary.Convert36ToNo(ewm.Substring(11, 4));
                            codeModel = dataContext.RequestCode.FirstOrDefault(
                                m => m.StartNum <= index && m.EndNum >= index
                                && m.FixedCode == fixedCode);
                        }
                        //查找路由信息
                        if (codeModel != null && codeModel.Route_DataBase_ID > 0)
                        {
                            Route_DataBase_ID = (long)codeModel.Route_DataBase_ID;
                        }
                        //此处查询农药码是顺序码的情况
                        else if (bigPara.Length == 1 && ewm.Length == 32)
                        {
                            codeType = (int)Common.EnumFile.GenCodeType.pesticides;
                            string fixedCode = ewm.Substring(0, ewm.Length - 9);
                            long index = Convert.ToInt64(ewm.Substring(23, 9));
                            codeModel = dataContext.RequestCode.FirstOrDefault(
                               m => m.Type == codeType && m.StartNum <= index && m.EndNum >= index
                               && m.FixedCode == fixedCode);
                            if (codeModel != null && codeModel.Route_DataBase_ID > 0)
                            {
                                Route_DataBase_ID = (long)codeModel.Route_DataBase_ID;
                            }
                        }
                    }
                    if (Route_DataBase_ID > 0)
                    {
                        using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
                        {
                            if (count == null || count == 0)
                            {
                                dataContext.ExecuteCommand("update " + tableName + " set FWCount=1,ValidateTime='" + DateTime.Now + "' where ewm='" + ewm + "'");
                            }
                            else
                            {
                                dataContext.ExecuteCommand("update " + tableName + " set FWCount=FWCount+1 where ewm='" + ewm + "'");
                            }
                        }
                        Ret.SetArgument(CmdResultError.NONE, "修改成功", "修改成功");
                    }
                }
            }
            catch (Exception ex)
            {
                Ret.SetArgument(CmdResultError.EXCEPTION, ex.ToString(), "修改失败");
            }
            return Ret;
        }
        #endregion

        #region 获取二维码信息

        /// <summary>
        /// 根据二维码获取二维码信息
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="type">0:取session二维码1:拍码二维码</param>
        /// MA.156.MX.XXXXXX.NXXXXXXY.SXXXXXXXX.PYYMMDD.LXXXXXXXX.EYYMMDD.CZ
        /// 示例：MA.156.M0.123456.N123456Y.L190726A.S1907260001.E200725.CZ
        /// 示例：MA.156.M0.100011.2cscp023.Ste002.M200206.L343432.D555555.E200228.V200307.C9
        ///注：X代表字符，N代表数字，Y Z代表校验结果，YYMMDD代表年年月月日日
        /// <returns></returns>
        public CodeInfo GetCode(string ewm, int type)
        {
            long routeDataBaseId = 0;
            RequestCode codeModel=null;
            CodeInfo result = new CodeInfo();
            string scBatchNo = string.Empty;
            //GS1码2021-12-30
            if (!ewm.Contains("."))
            {
                result = GetGS1(ewm, type);
				if (result != null) 
				{
					codeModel = result.CodeRequest;
				}
            }
            else
            {
                result = GetMa(ewm, type);
				if (result != null) 
				{
					codeModel = result.CodeRequest;
				}
            }
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    if (codeModel != null)
                    {
                        #region 查找企业信息
                        //查找企业信息
                        Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == codeModel.Enterprise_Info_ID);
                        if (enterprise != null)
                        {
                            List<ToJsonImg> imgs = new List<ToJsonImg>();
                            //判断企业wxLogo是否为空
                            if (!string.IsNullOrEmpty(enterprise.StrWXLogo))
                            {
                                XElement xml = XElement.Parse(enterprise.StrWXLogo);
                                IEnumerable<XElement> allImg = xml.Elements("img");
                                foreach (var item in allImg)
                                {
                                    ToJsonImg sub = new ToJsonImg
                                    {
                                        fileUrl = item.Attribute("value").Value,
                                        fileUrls = item.Attribute("small").Value
                                    };
                                    imgs.Add(sub);
                                }
                            }
                            enterprise.wxlogoimgs = imgs;
                            result.Enterprise = enterprise;
                            result.EnterpriseID = enterprise.Enterprise_Info_ID;
                        }
                        else
                        {
                            return null;
                        }
                        #endregion
                        #region 查找产品信息

                        //查找产品信息
                        View_Material material = dataContext.View_Material.FirstOrDefault(m => m.Material_ID == codeModel.Material_ID.Value);
                        if (null != material)
                        {
                            result.MaterialID = material.Material_ID;
                            result.BrandID = material.Brand_ID ?? 0;
                            //查找路由信息
                            if (codeModel.Route_DataBase_ID > 0)
                            {
                                routeDataBaseId = (long)codeModel.Route_DataBase_ID;
                            }
                        }
                        #endregion
                        #region 码配置参数
                        RequestCodeSetting seting;
                        if (result.FwCode.RequestSetID > 0 && result.FwCode.RequestSetID != null)
                        {
                            seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.ID == result.FwCode.RequestSetID);
                        }
                        else
                        {
                            //RequestSetID为空的查询原始批次信息BatchType = 1的
                            if (!string.IsNullOrEmpty(scBatchNo))
                            {
                                seting = dataContext.RequestCodeSetting.FirstOrDefault(p =>
                                   p.ShengChanPH == scBatchNo && p.BatchType == 1 && p.RequestID == codeModel.RequestCode_ID);
                            }
                            else
                            {
                                seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.BatchType == 1 && p.RequestID == codeModel.RequestCode_ID);
                            }
                        }

                        if (null != seting)
                        {
                            result.MaterialID = (long)seting.MaterialID;
                            result.EnterpriseID = seting.EnterpriseId;
                            result.DealerID = result.FwCode.Dealer_ID ?? 0;
                            result.BrandID = seting.BrandID ?? 0;
                            result.CodeSeting = seting;
                            result.RequestCodeType = seting.RequestCodeType.Value;
                            result.ProductDate = seting.ProductionDate == null ? "" : seting.ProductionDate.Value.ToString("yyyy-MM-dd");
                            if (seting.DisplayOption != null)
                            {
                                string[] setArr = seting.DisplayOption.Split(',');
                                for (int i = 0; i < setArr.Length - 1; i++)
                                {
                                    if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Brand)
                                    {
                                        result.Display.Brand = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Origin)
                                    {
                                        result.Display.Origin = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Work)
                                    {
                                        result.Display.Work = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Check)
                                    {
                                        result.Display.Check = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Report)
                                    {
                                        result.Display.Report = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Ambient)
                                    {
                                        result.Display.Ambient = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.WuLiu)
                                    {
                                        result.Display.WuLiu = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingDisplay.CreateDate)
                                    {
                                        result.Display.CreateDate = true;
                                    }
                                    else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingDisplay.ScanCount)
                                    {
                                        result.Display.ScanCount = true;
                                    }
                                }
                            }
                            if (result.RequestCodeType == (int)Common.EnumFile.RequestCodeType.fwzsCode)
                            {
                                result.Display.Verification = true;
                            }
                            else if (result.RequestCodeType == (int)Common.EnumFile.RequestCodeType.TraceCode)
                            {
                                result.Display.Verification = false;
                            }
                        }
                        else
                        {
                            result.CodeType = (int)Common.EnumFile.AnalysisBased.Normal;
                        }

                        #endregion
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    result = null;
                }
            }
            return result;
        }
        /// <summary>
        /// 查找Ma码的码信息
        /// </summary>
        /// <param name="ewm"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public CodeInfo GetMa(string ewm, int type)
        {
            CodeInfo result = new CodeInfo();
            result.CodeRequest = null;
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
                    //如果是配置预览码，将直接从配置表中查询数据
                    if (arr.Length > 0 && arr[arr.Length - 1].Trim().Equals("10"))//配置预览码
                    {
                        long setId = Convert.ToInt64(arr[arr.Length - 2]);
                        result = GetSetInfo(setId, ref result);
                        result.FwCode = new Enterprise_FWCode_00();
                        int a1 = arr.Length;//20200609预览显示的码修改
                        string reEWM = "";
                        for (int i = 0; i < a1 - 2; i++)
                        {
                            reEWM += arr[i] + ".";
                        }
                        string PreviewCode = reEWM.Remove(reEWM.Length - 1, 1);
                        result.FwCode.EWM = PreviewCode;
                        result.CodeType = (int)Common.EnumFile.AnalysisBased.setEwm;
                        return result;
                    }
                    for (int i = 3; i < arr.Length; i++)
                    {
                        if (arr[i].Substring(0, 1) == "P")//生产日期P开头旧规则20200820
                        {
                            scriqi = arr[i].Substring(1, arr[i].Length - 1);
                        }
                        if (arr[i].Substring(0, 1) == "M")//生产日期M开头新规则20200820
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
                    var data = dataContext.RequestCode.Where(m => m.FixedCode == fixCode && m.Route_DataBase_ID!=null);
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
                    codeModelList = data.ToList();
                    if (codeModelList != null && codeModelList.Count() > 0)
                    {
                        foreach (RequestCode requestCode in codeModelList)
                        {
                            #region 查找码库
                            string tableName;
                            using (DataClassesDataContext dataContextDynamic = GetDynamicDataContext((long)requestCode.Route_DataBase_ID, out tableName))
                            {
                                try
                                {
                                    string sql = string.Format("select * from {0} where ewm='{1}'", tableName, ewm);
                                    if (type == 1 && result.CodeType != (int)Common.EnumFile.AnalysisBased.viewEwm)
                                    {
                                        ScanEwm scanEwm = new ScanEwm();
                                        sql = string.Format("select * from {0} where ewm='{1}';update {0} set ScanCount=ScanCount+1 where ewm='{1}'", tableName, ewm);
                                        //新增拍码数据
                                        scanEwm.EWM = ewm;
                                        scanEwm.ScanDate = DateTime.Now;
                                        scanEwm.Ip = GetVisitorIPHelper.ClientIp();
                                        dataContextDynamic.ScanEwm.InsertOnSubmit(scanEwm);
                                    }
                                    result.FwCode = dataContextDynamic.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();

                                    dataContext.SubmitChanges();
                                    dataContextDynamic.SubmitChanges();
                                    if (result.FwCode != null && result.FwCode.Dealer_ID > 0)
                                    {
                                        result.DealerID = result.FwCode.Dealer_ID ?? 0;
                                    }
                                    if (result.FwCode == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        result.CodeRequest = requestCode;
                                        result.CodeSeting = new RequestCodeSetting();
                                        result.Display = new Display();
                                        return result;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string errData = "ScanCodeDAL.GetCode()";
                                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                                }
                            }
                            #endregion
                        }
                    }
                }
                catch
                { }
            }
            return result;
        }
        /// <summary>
        /// (01)06951484628515(17)220531(11)211230(10)1230001(21)020001
        /// 01：品类DI编码；  17：有效期YouXiaoDate；  (11)：生产日期startdate；  10：生产批号：ShengChanPH    21：批次编号加流水号
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public CodeInfo GetGS1(string ewm, int type)
        {
            CodeInfo result = new CodeInfo();
            result.CodeRequest = null;
            string fixCode = string.Empty;
            //17开头
            string YouXiaoDate = string.Empty;
            //11开头
            string startdate = string.Empty;
            //生产批号10开头 10  长度不固定
            string ShengChanPH = string.Empty;
            //去掉括号，因为条形码识别时候没有括号
            string ewmTemp=ewm.Replace("(","").Replace(")","");
            //首位为如果有不可见特殊字符，先去掉
            ewmTemp = ewmTemp.TrimStart('\u001d');
			//要恢复成带括号的完整码
			string completeEwm = string.Empty;
			//若仅分解UDI客户端生成的UDI码，除批次码外，必定含有\u001d；批次码不会上传至追溯平台；
			string[] GS1UDI = ewmTemp.Split(new Char[] { '\u001d' }, StringSplitOptions.RemoveEmptyEntries);
			if (GS1UDI.Length > 0) 
			{
				if (GS1UDI[0].Substring(0, 2) == "01")
				{
					fixCode = GS1UDI[0].Substring(2, 14);
					completeEwm = "(01)" + fixCode;
					GS1UDI[0] = GS1UDI[0].Substring(16, GS1UDI[0].Length - 16);
				}
				if (GS1UDI[0].Substring(0, 2) == "17")
				{
					YouXiaoDate = GS1UDI[0].Substring(2, 6);
					completeEwm = completeEwm + "(17)" + YouXiaoDate;
					GS1UDI[0] = GS1UDI[0].Substring(8, GS1UDI[0].Length - 8);
				}
				if (GS1UDI[0].Substring(0, 2) == "11")
				{
					startdate = GS1UDI[0].Substring(2, 6);
					completeEwm = completeEwm + "(11)" + startdate;
					GS1UDI[0] = GS1UDI[0].Substring(8, GS1UDI[0].Length - 8);
				}
				if (GS1UDI[0].Substring(0, 2) == "10")
				{
					ShengChanPH = GS1UDI[0].Substring(2);
					completeEwm = completeEwm + "(10)" + ShengChanPH;
					//GS1UDI[0] = GS1UDI[0].Substring(9, GS1UDI[0].Length - 9);
				}
				if (GS1UDI.Length == 2) 
				{
					GS1UDI[1] = GS1UDI[1].Contains("\u001d") ? GS1UDI[1].TrimStart('\u001d') : GS1UDI[1];
					if (GS1UDI[1].Trim().Substring(0, 2) == "21")
					{
						GS1UDI[1] = GS1UDI[1].Trim().Substring(2);
						completeEwm = completeEwm + "\u001d(21)" + GS1UDI[1];//最后将21的部分加上
					}
				}
			}
            
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    MaterialDI diInfo = dataContext.MaterialDI.Where(p => p.GSIDI == fixCode).FirstOrDefault();
                    if (diInfo == null)
                    {
                        fixCode=fixCode.Substring(1,fixCode.Length-1);//有可能码上的DI信息自动补0了
                        diInfo=dataContext.MaterialDI.Where(p=>p.GSIDI==fixCode).FirstOrDefault();
                    }
                    if (diInfo == null)
                    {
                        return null;
                    }
                    var data = dataContext.RequestCode.Where(m => m.FixedCode == fixCode && m.Route_DataBase_ID!=null);
                    if (!string.IsNullOrEmpty(startdate))
                    {
                        data = data.Where(m => m.startdate == startdate);
                    }
                    if (!string.IsNullOrEmpty(YouXiaoDate))
                    {
                        data = data.Where(m => m.YouXiaoDate == YouXiaoDate);
                    }
                    if (!string.IsNullOrEmpty(ShengChanPH))
                    {
                        data = data.Where(m => m.ShengChanPH == ShengChanPH);
                    }
                    List<RequestCode> codeModelList = data.ToList();
                    if (codeModelList == null || codeModelList.Count() == 0)
                    {
                        result = null;
                        return result;
                    }
                    foreach (RequestCode requestCode in codeModelList)
                    {
                        #region 查找码库
                        string tableName;
                        using (DataClassesDataContext dataContextDynamic = GetDynamicDataContext((long)requestCode.Route_DataBase_ID, out tableName))
                        {
                            try
                            {
                                string sql = string.Format("select * from {0} where ewm='{1}'", tableName, completeEwm);
                                if (type == 1 && result.CodeType != (int)Common.EnumFile.AnalysisBased.viewEwm)
                                {
                                    ScanEwm scanEwm = new ScanEwm();
                                    sql = string.Format("select * from {0} where ewm='{1}';update {0} set ScanCount=ScanCount+1 where ewm='{1}'", tableName, completeEwm);
                                    //新增拍码数据
                                    scanEwm.EWM = ewm;
                                    scanEwm.ScanDate = DateTime.Now;
                                    scanEwm.Ip = GetVisitorIPHelper.ClientIp();
                                    dataContextDynamic.ScanEwm.InsertOnSubmit(scanEwm);
                                }
                                result.FwCode = dataContextDynamic.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();

                                dataContext.SubmitChanges();
                                dataContextDynamic.SubmitChanges();
                                if (result.FwCode != null && result.FwCode.Dealer_ID > 0)
                                {
                                    result.DealerID = result.FwCode.Dealer_ID ?? 0;
                                }
                                if (result.FwCode == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    result.CodeRequest = requestCode;
                                    result.CodeSeting = new RequestCodeSetting();
                                    result.Display = new Display();
                                    return result;
                                }
                            }
                            catch (Exception ex)
                            {
                                string errData = "ScanCodeDAL.GetCode()";
                                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                            }
                        }
                        #endregion
                    }
                }
                catch
                { }
            }
            return result;
        }
        /// <summary>
        /// 根据二维码获取配置的预览信息
        /// </summary>
        /// <param name="ewm">二维码i.1.130105.28.91.520.12.10；其中12是设置编号，10代表该码为预览码</param>
        /// <returns></returns>
        public CodeInfo GetSetInfo(long setID, ref CodeInfo result)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                result.FwCode = new Enterprise_FWCode_00();
                result.CodeSeting = new RequestCodeSetting();
                result.Display = new Display();
                try
                {
                    RequestCodeSetting seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.ID == setID);
                    if (seting != null)
                    {
                        result.CodeType = (int)Common.EnumFile.AnalysisBased.Seting;
                        result.MaterialID = (long)seting.MaterialID;
                        result.EnterpriseID = seting.EnterpriseId;
                        result.DealerID = result.FwCode.Dealer_ID ?? 0;
                        result.BrandID = seting.BrandID ?? 0;
                        result.CodeSeting = seting;
                        result.RequestCodeType = seting.RequestCodeType.Value;
                        if (seting.DisplayOption != null)
                        {
                            string[] setArr = seting.DisplayOption.Split(',');
                            for (int i = 0; i < setArr.Length - 1; i++)
                            {
                                result.Display.Verification = true;
                                if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Brand)
                                {
                                    result.Display.Brand = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Origin)
                                {
                                    result.Display.Origin = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Work)
                                {
                                    result.Display.Work = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Check)
                                {
                                    result.Display.Check = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Report)
                                {
                                    result.Display.Report = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Ambient)
                                {
                                    result.Display.Ambient = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.WuLiu)
                                {
                                    result.Display.WuLiu = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingDisplay.CreateDate)
                                {
                                    result.Display.CreateDate = true;
                                }
                                else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingDisplay.ScanCount)
                                {
                                    result.Display.ScanCount = true;
                                }
                            }
                        }
                        if (result.RequestCodeType == (int)Common.EnumFile.RequestCodeType.fwzsCode)
                        {
                            result.Display.Verification = true;
                        }
                        else if (result.RequestCodeType == (int)Common.EnumFile.RequestCodeType.TraceCode)
                        {
                            result.Display.Verification = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetSetInfo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
                return result;
            }
        }

        public Enterprise_FWCode_00 GetViewCode(string ewm)
        {
            long Route_DataBase_ID = 0;
            string tableName = "";
            string[] arr = ewm.Split('.');
            Enterprise_FWCode_00 result = null;
            if (arr.Length <= 2)
            {
                return result;
            }
            //如果是配置预览码，将直接从配置表中查询数据
            if (arr[arr.Length - 2].Trim().Equals("10"))
            {
                result = new Enterprise_FWCode_00();
                result.EWM = ewm;
                return result;
            }
            else if (arr[arr.Length - 2].Trim().Equals("10"))
            {
                ewm = ewm.Substring(0, ewm.LastIndexOf("9") - 1);
            }
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    string split_table = arr[arr.Length - 2].Substring(0, arr[arr.Length - 2].Length - 9);
                    var data_split_table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == long.Parse(split_table));
                    Route_DataBase_ID = data_split_table.Route_DataBase_ID;
                }
                catch { }
            }
            using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
            {
                try
                {
                    string sql = string.Format("select * from {0} where ewm='{1}'", tableName, ewm);
                    result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetViewCode()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        public View_Enterprise_FWCode_00 GetSaleCodeInfo(string ewm)
        {
            long Route_DataBase_ID = 0;
            string tableName = "";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    string[] arr = ewm.Split('.');
                    string split_table = arr[arr.Length - 2].Substring(0, arr[arr.Length - 2].Length - 9);
                    var data_split_table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == long.Parse(split_table));
                    Route_DataBase_ID = data_split_table.Route_DataBase_ID;
                }
                catch { }
            }
            View_Enterprise_FWCode_00 result = null;
            using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
            {
                try
                {
                    string sql = string.Format("select * from View_{0} where ewm='{1}'", tableName, ewm);
                    result = dataContext.ExecuteQuery<View_Enterprise_FWCode_00>(sql).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetSaleCodeInfo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        #region 根据批次
        public Enterprise_FWCode_00 GetCodeByBatch(long bId)
        {
            Enterprise_FWCode_00 result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Batch b = dataContext.Batch.FirstOrDefault(m => m.Batch_ID == bId);

                    result = new Enterprise_FWCode_00();
                    result.Batch_ID = bId;
                    result.BatchExt_ID = 0;
                    result.CreateDate = DateTime.Now;
                    result.Dealer_ID = null;
                    result.Enterprise_FWCode_ID = 0;
                    result.Enterprise_Info_ID = b.Enterprise_Info_ID.GetValueOrDefault(0).ToString();
                    result.EWM = "";
                    result.EWM_Info = null;
                    result.FWCode = "000000";
                    result.FWCount = 0;
                    result.Material_ID = b.Material_ID;
                    result.RequestCode_ID = 0;
                    result.SalesInformation_ID = 0;
                    result.SalesTime = DateTime.Now;
                    result.ScanCount = 0;
                    result.Status = null;
                    result.StrCreateDate = DateTime.Now.ToString("yyyy-MM-dd");
                    result.StrSalesTime = DateTime.Now.ToString("yyyy-MM-dd");
                    result.StrUseTime = DateTime.Now.ToString("yyyy-MM-dd");
                    result.UseTime = DateTime.Now;
                    result.ValidateTime = null;
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetCodeByBatch()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion
        #region 根据二维码
        public Enterprise_FWCode_00 GetCodeByEWM(string ewm)
        {
            long Route_DataBase_ID = 0;
            string tableName = "";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    string[] arr = ewm.Split('.');
                    string split_table = arr[arr.Length - 2].Substring(0, arr[arr.Length - 2].Length - 9);
                    var data_split_table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == long.Parse(split_table));
                    Route_DataBase_ID = data_split_table.Route_DataBase_ID;
                }
                catch { }
            }
            Enterprise_FWCode_00 result = null;
            using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
            {
                try
                {
                    string sql = string.Format("select * from {0} where ewm='{1}';update {0} set ScanCount=ScanCount+1 where ewm='{1}'", tableName, ewm);
                    result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(sql).FirstOrDefault();
                    result.ScanCount = result.ScanCount + 1;
                    result.FWCount = result.FWCount + 1;
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetCodeByEWM()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion
        #endregion

        #region page1
        #region 验证
        public void FWValidate(string ewm)
        {
            long Route_DataBase_ID = 0;
            string tableName = "";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    string[] arr = ewm.Split('.');
                    string split_table = arr[arr.Length - 2].Substring(0, arr[arr.Length - 2].Length - 9);
                    var data_split_table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == long.Parse(split_table));
                    Route_DataBase_ID = data_split_table.Route_DataBase_ID;
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.FWValidate()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            using (DataClassesDataContext dataContext = GetDynamicDataContext(Route_DataBase_ID, out tableName))
            {
                try
                {
                    string sql = string.Format("update {0} set FWCount=FWCount+1 where ewm='{1}'", tableName, ewm);
                    dataContext.ExecuteCommand(sql);
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.FWValidate()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
        }
        #endregion

        #region 获取产品信息
        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="mId">查看平编号</param>
        /// <returns></returns>
        public Material GetMaterial(long mId)
        {
            Material result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Material.FirstOrDefault(m => m.Material_ID == mId);

                    #region 图片XML转JSON类
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    if (result.MaterialImgInfo != null)
                    {
                        IEnumerable<XElement> allImg = result.MaterialImgInfo.Elements("img");
                        foreach (var item in allImg)
                        {
                            ToJsonImg sub = new ToJsonImg();
                            sub.fileUrl = item.Attribute("value").Value;
                            sub.fileUrls = item.Attribute("small").Value;
                            imgs.Add(sub);
                        }
                    }
                    result.imgs = imgs;
                    #endregion

                    #region 属性XML转JSON类
                    List<ToJsonProperty> propertys = new List<ToJsonProperty>();
                    IEnumerable<XElement> allProperty = result.PropertyInfo.Elements("info");
                    foreach (var item in allProperty)
                    {
                        ToJsonProperty sub = new ToJsonProperty();
                        sub.pName = item.Attribute("iname").Value;
                        sub.pValue = item.Attribute("ivalue").Value;
                        sub.allprototype = sub.pName + "：" + sub.pValue;
                        propertys.Add(sub);
                    }
                    result.propertys = propertys;
                    #endregion
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetMaterial()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <param name="materialID">产品编码</param>
        /// <param name="code">二维码</param>
        /// <returns></returns>
        public ScanMaterial GetMaterialNew(long materialID, string code)
        {
            ScanMaterial result = new ScanMaterial();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    View_Material material = dataContext.View_Material.FirstOrDefault(m => m.Material_ID == materialID);
                    result.BrandName = material.BrandName;
                    result.MaterialName = material.MaterialFullName;
                    result.ShortMemo = material.Materialjj;
                    result.Taste = material.MaterialTaste;
                    result.BrandImg = material.Logo;
                    result.MaterialAliasName = material.MaterialAliasName;
                    result.ShelfLife = material.ShelfLife;
                    result.tbURL = material.tbURL;
                    List<ToJsonImg> imgUrls = new List<ToJsonImg>();
                    ToJsonImg videoUrl = new ToJsonImg();
                    if (material.MaterialImgInfo != null)
                    {
                        foreach (var item in material.MaterialImgInfo.Elements("img"))
                        {
                            ToJsonImg sub = new ToJsonImg();
                            sub.fileUrl = item.Attribute("value").Value;
                            sub.fileUrls = item.Attribute("small").Value;
                            imgUrls.Add(sub);
                        }
                        foreach (var item in material.MaterialImgInfo.Elements("video"))
                        {
                            videoUrl.videoUrl = item.Attribute("value").Value;
                            videoUrl.videoUrls = item.Attribute("small").Value;
                        }
                    }
                    result.VideoUrl = videoUrl;
                    result.picUrl = imgUrls;
                    using (DataClassesDataContext data = GetDataContext("Code_Connect"))
                    {
                        result.ScanCount = data.ScanEwm.Count(m => m.EWM == code);
                    }
                    List<ToJsonProperty> materialInfo = new List<ToJsonProperty>();
                    if (material.PropertyInfo != null)
                    {
                        foreach (var item in material.PropertyInfo.Elements("info"))
                        {
                            ToJsonProperty sub = new ToJsonProperty();
                            sub.pName = item.Attribute("iname").Value;
                            sub.pValue = item.Attribute("ivalue").Value;
                            sub.allprototype = sub.pName + "：" + sub.pValue;
                            materialInfo.Add(sub);
                        }
                    }
                    EnterpriseShopLink shop = dataContext.EnterpriseShopLink.FirstOrDefault(p => p.EnterpriseID == material.Enterprise_Info_ID);
                    if (shop != null)
                    {
                        result.TaoBaoLink = shop.TaoBaoLink;
                        result.TianMaoLink = shop.TianMaoLink;
                        result.JingDongLink = shop.JingDongLink;
                    }
                    if (!string.IsNullOrEmpty(material.TaoBaoLink))
                    {
                        result.TaoBaoLink = material.TaoBaoLink;
                    }
                    if (!string.IsNullOrEmpty(material.TianMaoLink))
                    {
                        result.TianMaoLink = material.TianMaoLink;
                    }
                    if (!string.IsNullOrEmpty(material.JingDongLink))
                    {
                        result.JingDongLink = material.JingDongLink;
                    }
                    if (!string.IsNullOrEmpty(material.WeiDianLink))
                    {
                        result.WeiDianLink = material.WeiDianLink;
                    }
                    result.MaterialInfo = materialInfo;
                    result.Memo = material.Memo;
                    result.MaterialPlace = material.MaterialPlace;
                    //视频链接
                    MaterialShopLink materialLike = dataContext.MaterialShopLink.FirstOrDefault(a => a.MaterialID == material.Material_ID);
                    result.AllVideoUrl = new List<ToJsonImg>();
                    if (materialLike != null)
                    {
                        if (materialLike.VideoUrl != null)
                        {
                            foreach (var item in materialLike.VideoUrl.Elements("video"))
                            {
                                result.AllVideoUrl.Add(new ToJsonImg { videoUrl = item.Attribute("url").Value });
                            }
                        }
                    }
                    //广告图片
                    ToJsonImg adUrl = new ToJsonImg();
                    if (material.AdUrl != null)
                    {
                        foreach (var item in materialLike.AdUrl.Elements("img"))
                        {
                            ToJsonImg sub = new ToJsonImg();
                            adUrl.fileUrl = item.Attribute("value").Value;
                            adUrl.fileUrls = item.Attribute("small").Value;
                        }
                    }
                    result.AdUrl = adUrl;
                    if (string.IsNullOrEmpty(adUrl.fileUrl) && string.IsNullOrEmpty(adUrl.fileUrls))
                    {
                        if (shop.AdUrl != null)
                        {
                            foreach (var item in shop.AdUrl.Elements("img"))
                            {
                                ToJsonImg sub = new ToJsonImg();
                                adUrl.fileUrl = item.Attribute("value").Value;
                                adUrl.fileUrls = item.Attribute("small").Value;
                            }
                        }
                        result.AdUrl = adUrl;
                    }
                    if (material.MaterialSpcificationID > 0)
                    {
                        MaterialSpcification spection = dataContext.MaterialSpcification.FirstOrDefault(p => p.MaterialSpcificationID == material.MaterialSpcificationID);
                        if (spection != null)
                        {
                            result.Speciton = spection.Value + spection.MaterialSpcificationName;
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        public View_MaterialSpecForMarket GetMaterialModel(long MaterialSpecId)
        {
            View_MaterialSpecForMarket result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.View_MaterialSpecForMarket.FirstOrDefault(m => m.MaterialSpecId == MaterialSpecId);
                    if (MaterialSpecId > 0)
                    {
                        #region 图片XML转JSON类
                        List<ToJsonImg> imgs = new List<ToJsonImg>();
                        if (result.MaterialImgInfo != null)
                        {
                            IEnumerable<XElement> allImg = result.MaterialImgInfo.Elements("img");
                            foreach (var item in allImg)
                            {
                                ToJsonImg sub = new ToJsonImg();
                                sub.fileUrl = item.Attribute("value").Value;
                                sub.fileUrls = item.Attribute("small").Value;
                                imgs.Add(sub);
                            }
                        }
                        result.imgs = imgs;
                        #endregion

                        #region 属性XML转JSON类
                        List<ToJsonProperty> propertys = new List<ToJsonProperty>();
                        IEnumerable<XElement> allProperty = result.PropertyInfo.Elements("info");
                        foreach (var item in allProperty)
                        {
                            ToJsonProperty sub = new ToJsonProperty();
                            sub.pName = item.Attribute("iname").Value;
                            sub.pValue = item.Attribute("ivalue").Value;
                            sub.allprototype = sub.pName + "：" + sub.pValue;
                            propertys.Add(sub);
                        }
                        result.propertys = propertys;
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetMaterial()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        #region 获取品牌信息
        public Brand GetBrand(long bId)
        {
            Brand result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Brand.FirstOrDefault(m => m.Brand_ID == bId);
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetBrand()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        #region 获取区域品牌
        public Brand GetAreaBrand(long mId)
        {
            Brand result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Brand_Enterprise brandEnterprise = dataContext.Brand_Enterprise.FirstOrDefault(m => m.Material_ID == mId && m.Status == 1);
                    if (brandEnterprise != null)
                    {
                        result = dataContext.Brand.FirstOrDefault(m => m.Brand_ID == brandEnterprise.Brand_ID);
                    }
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetAreaBrand()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion
        #endregion

        #region page2

        /// <summary>
        /// 获取原料信息
        /// </summary>
        /// <param name="setId">设置编号</param>
        /// <returns></returns>
        public List<View_RequestOrigin> GetYuanliao(long? setId)
        {
            List<View_RequestOrigin> result = new List<View_RequestOrigin>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.View_RequestOrigin.Where(m => m.SettingID == setId && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    foreach (var item in result)
                    {
                        if (item.Img != null && item.Img.Element("img") != null)
                        {
                            item.StrFiles = item.Img.Element("img").Attribute("small").Value;
                            List<ToJsonImg> rawImg = new List<ToJsonImg>();
                            List<ToJsonJCImg> reportImg = new List<ToJsonJCImg>();
                            foreach (var sub in item.Img.Elements("img"))
                            {
                                ToJsonImg img = new ToJsonImg();
                                img.fileUrl = sub.Attribute("value").Value;
                                img.fileUrls = sub.Attribute("small").Value;
                                rawImg.Add(img);
                            }
                            item.imgs = rawImg;
                        }
                        else
                        {
                            item.StrFiles = imgDefault;
                        }
                        if (item.JCImgInfo != null && item.JCImgInfo.Element("img") != null)
                        {
                            item.StrJCFiles = item.JCImgInfo.Element("img").Attribute("small").Value;
                            List<ToJsonJCImg> reportImg = new List<ToJsonJCImg>();
                            reportImg = new List<ToJsonJCImg>();
                            foreach (var sub in item.JCImgInfo.Elements("img"))
                            {
                                ToJsonJCImg img = new ToJsonJCImg();
                                img.jcfileUrl = sub.Attribute("value").Value;
                                img.jcfileUrls = sub.Attribute("small").Value;
                                reportImg.Add(img);
                            }
                            item.jcimgs = reportImg;
                        }
                        else
                        {
                            item.StrJCFiles = imgDefault;
                        }
                        item.StrAddDate = Convert.ToDateTime(item.InDate).ToString("yyyy.MM.dd");
                    }
                    result = result.OrderBy(m => m.AddDate).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetYuanliao()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取检验检测报告
        /// </summary>
        /// <param name="bId">批次</param>
        /// <param name="beId">子批次</param>
        /// <param name="type">码类型</param>
        /// <param name="setId">设置编号</param>
        /// <returns></returns>
        public List<Batch_JianYanJianYi> GetJianYanJianCe(long bId, long? beId, int type, long setId)
        {
            List<Batch_JianYanJianYi> result = new List<Batch_JianYanJianYi>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Batch_JianYanJianYi.Where(m => m.Status == (int)Common.EnumFile.Status.used).ToList();
                    if (type == (int)Common.EnumFile.AnalysisBased.Batch)
                    {
                        result = result.Where(m => (m.Batch_ID == bId && (m.BatchExt_ID == 0 || m.BatchExt_ID == null))).ToList();
                        if ((beId ?? 0) > 0)
                        {
                            result.AddRange(
                                dataContext.Batch_JianYanJianYi.Where(m => m.BatchExt_ID == beId && m.Status == (int)Common.EnumFile.Status.used).ToList()
                            );
                        }
                    }
                    else if (type == (int)Common.EnumFile.AnalysisBased.Seting && setId > 0)
                    {
                        result = result.Where(p => p.SettingID == setId).ToList();
                    }
                    else if (type == (int)Common.EnumFile.AnalysisBased.setEwm && setId > 0)
                    {
                        result = result.Where(p => p.SettingID == setId).ToList();
                    }
                    else
                    {
                        result = new List<Batch_JianYanJianYi>();
                    }
                    foreach (var item in result)
                    {
                        if (item.Files != null && item.Files.Element("img") != null)
                        {
                            item.StrFiles = item.Files.Element("img").Attribute("small").Value;
                        }
                        else
                        {
                            item.StrFiles = imgDefault;
                        }
                    }
                    result = result.OrderBy(m => m.AddDate).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetJianYanJianCe()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        ///  获取巡检信息
        /// </summary>
        /// <param name="bId">批次</param>
        /// <param name="beId">子批次</param>
        /// <param name="type">码类型</param>
        /// <param name="setId">设置编号</param>
        /// <returns></returns>
        public List<Batch_XunJian> GetXunJian(long bId, long? beId, int type, long setId)
        {
            List<Batch_XunJian> result = new List<Batch_XunJian>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Batch_XunJian.Where(m => m.Status == (int)Common.EnumFile.Status.used).ToList();
                    if (type == (int)Common.EnumFile.AnalysisBased.Batch)
                    {
                        result = result.Where(m => m.Batch_ID == bId && (m.BatchExt_ID == 0 || m.BatchExt_ID == null)).ToList();
                        if ((beId ?? 0) > 0)
                        {
                            result.AddRange(
                                dataContext.Batch_XunJian.Where(m => m.BatchExt_ID == beId && m.Status == (int)Common.EnumFile.Status.used).ToList()
                            );
                        }
                    }
                    else if (type == (int)Common.EnumFile.AnalysisBased.Seting && setId > 0)
                    {
                        result = result.Where(p => p.SettingID == setId).ToList();
                    }
                    else if (type == (int)Common.EnumFile.AnalysisBased.setEwm && setId > 0)
                    {
                        result = result.Where(p => p.SettingID == setId).ToList();
                    }
                    else
                    {
                        result = new List<Batch_XunJian>();
                    }
                    foreach (var item in result)
                    {
                        if (item.Files != null && item.Files.Element("img") != null)
                        {
                            item.StrFiles = item.Files.Element("img").Attribute("small").Value;
                        }
                        else if (item.Files != null && item.Files.Element("video") != null)
                        {
                            item.StrFiles = item.Files.Element("video").Attribute("small").Value;
                        }
                        else
                        {
                            item.StrFiles = imgDefault;
                        }
                    }
                    result = result.OrderBy(m => m.AddDate).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetXunJian()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取生产/加工/养殖信息
        /// </summary>
        /// <param name="bId">批次</param>
        /// <param name="beId">子批次</param>
        /// <param name="type">码类型</param>
        /// <param name="setId">设置编号</param>
        /// <returns></returns>
        public List<View_ZuoYeAndZuoYeType> GetProduce(long bId, long? beId, int type, int zuoyeType, long setId)
        {
            List<View_ZuoYeAndZuoYeType> result = new List<View_ZuoYeAndZuoYeType>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.View_ZuoYeAndZuoYeType.Where(m => m.Status == (int)Common.EnumFile.Status.used && m.type == zuoyeType).ToList();
                    if (type == (int)Common.EnumFile.AnalysisBased.Batch)
                    {
                        result = result.Where(m => m.Batch_ID == bId && (m.BatchExt_ID == 0 || m.BatchExt_ID == null)).ToList();
                        if ((beId ?? 0) > 0)
                        {
                            result.AddRange(
                                dataContext.View_ZuoYeAndZuoYeType.Where(m => m.BatchExt_ID == beId &&
                                    m.Status == (int)Common.EnumFile.Status.used && m.type == (int)Common.EnumFile.ZuoYeType.Produce).ToList()
                            );
                        }
                    }
                    else if (type == (int)Common.EnumFile.AnalysisBased.Seting && setId > 0)
                    {
                        result = result.Where(p => p.SettingID == setId).ToList();
                    }
                    else if (type == (int)Common.EnumFile.AnalysisBased.setEwm && setId > 0)
                    {
                        result = result.Where(p => p.SettingID == setId).ToList();
                    }
                    else
                    {
                        result = new List<View_ZuoYeAndZuoYeType>();
                    }
                    result = result.OrderBy(m => m.AddDate).ToList();
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetProduce()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        #region page2sub
        #region 获取检验检测报告
        public ScanInfo GetJianYanJianCe(long jId)
        {
            ScanInfo result = new ScanInfo();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Batch_JianYanJianYi info = dataContext.Batch_JianYanJianYi.FirstOrDefault(m => m.Batch_JianYanJianYi_ID == jId);
                    result.type = "检测";
                    result.title = info.Content;
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    //if (info.Files != null && info.Files.Element("img") != null)
                    //{
                    //    ToJsonImg img = new ToJsonImg();
                    //    img.fileUrl = info.Files.Element("img").Attribute("value").Value.ToString();
                    //    img.fileUrls = info.Files.Element("img").Attribute("small").Value.ToString();
                    //    imgs.Add(img);
                    //}
                    if (info.Files != null)
                    {
                        foreach (var item in info.Files.Elements())
                        {
                            switch (item.Name.ToString())
                            {
                                case "img":
                                    ToJsonImg img = new ToJsonImg();
                                    img.fileUrl = item.Attribute("value").Value;
                                    img.fileUrls = item.Attribute("small").Value;
                                    imgs.Add(img);
                                    break;
                            }
                        }
                    }
                    result.imgs = imgs;
                    result.time = info.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy.MM.dd");
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetJianYanJianCe()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        public ScanInfo GetJianYanJian(long codeSetId)
        {
            ScanInfo result = new ScanInfo();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Batch_JianYanJianYi info = dataContext.Batch_JianYanJianYi.FirstOrDefault(m => m.SettingID == codeSetId &&
                        m.Status == (int)Common.EnumFile.Status.used);
                    result.type = "检测";
                    result.title = info.Content;
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    if (info.Files != null)
                    {
                        foreach (var item in info.Files.Elements())
                        {
                            switch (item.Name.ToString())
                            {
                                case "img":
                                    ToJsonImg img = new ToJsonImg();
                                    img.fileUrl = item.Attribute("value").Value;
                                    img.fileUrls = item.Attribute("small").Value;
                                    imgs.Add(img);
                                    break;
                            }
                        }
                    }
                    result.imgs = imgs;
                    result.time = info.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy.MM.dd");
                    result.content = info.Content;
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetJianYanJian()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        #region 获取巡检信息
        public ScanInfo GetXunJian(long xId)
        {
            ScanInfo result = new ScanInfo();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Batch_XunJian info = dataContext.Batch_XunJian.FirstOrDefault(m => m.Batch_XunJian_ID == xId);
                    result.type = "巡检";
                    result.title = "巡检信息";
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    List<ToJsonImg> videos = new List<ToJsonImg>();
                    if (info.Files != null)
                    {
                        foreach (var item in info.Files.Elements())
                        {
                            switch (item.Name.ToString())
                            {
                                case "img":
                                    ToJsonImg img = new ToJsonImg();
                                    img.fileUrl = item.Attribute("value").Value;
                                    img.fileUrls = item.Attribute("small").Value;
                                    imgs.Add(img);
                                    break;
                                case "video":
                                    ToJsonImg video = new ToJsonImg();
                                    video.fileUrl = item.Attribute("value").Value;
                                    video.fileUrls = item.Attribute("small").Value;
                                    videos.Add(video);
                                    break;
                            }
                        }
                    }
                    result.imgs = imgs;
                    result.videos = videos;
                    result.user = info.UserName;
                    result.time = info.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy.MM.dd");
                    result.content = info.Content;
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetXunJian()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        #region 获取生产/加工信息
        public ScanInfo GetProduce(long pId)
        {
            ScanInfo result = new ScanInfo();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    View_ZuoYeAndZuoYeType info = dataContext.View_ZuoYeAndZuoYeType.FirstOrDefault(m => m.Batch_ZuoYe_ID == pId);
                    result.type = info.type == (int)Common.EnumFile.ZuoYeType.Produce ? "生产" : "加工";
                    result.title = info.OperationTypeName;
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    List<ToJsonImg> videos = new List<ToJsonImg>();
                    if (info.Files != null)
                    {
                        foreach (var item in info.Files.Elements())
                        {
                            switch (item.Name.ToString())
                            {
                                case "img":
                                    ToJsonImg img = new ToJsonImg();
                                    img.fileUrl = item.Attribute("value").Value;
                                    img.fileUrls = item.Attribute("small").Value;
                                    imgs.Add(img);
                                    break;
                                case "video":
                                    ToJsonImg video = new ToJsonImg();
                                    video.fileUrl = item.Attribute("value").Value;
                                    video.fileUrls = item.Attribute("small").Value;
                                    videos.Add(video);
                                    break;
                            }
                        }
                    }
                    result.imgs = imgs;
                    result.videos = videos;
                    result.user = info.UserName;
                    result.time = info.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy.MM.dd");
                    result.content = info.Content;
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetProduce()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取原料信息
        /// </summary>
        /// <param name="sId">设置编号</param>
        /// <returns></returns>
        public ScanInfo GetYuanliaoSub(long sId)
        {
            ScanInfo result = new ScanInfo();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    RequestOrigin info = dataContext.RequestOrigin.FirstOrDefault(m => m.ID == sId);
                    result.type = "原材料检测";
                    result.title = "原材料检测";
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    if (info.Img != null && info.Img.Element("img") != null)
                    {
                        foreach (var item in info.Img.Elements())
                        {
                            ToJsonImg img = new ToJsonImg();
                            img.fileUrl = item.Attribute("value").Value;
                            img.fileUrls = item.Attribute("small").Value;
                            imgs.Add(img);
                        }
                    }
                    result.imgs = imgs;
                    result.time = info.AddDate.GetValueOrDefault(DateTime.Now).ToString("yyyy.MM.dd");
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetJianYanJianCe()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion
        #endregion

        #region page3
        #region 企业信息
        public Enterprise_Info GetEnterprise(long eId)
        {
            Enterprise_Info result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == eId);
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetEnterprise()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        #region 经销商信息
        public Dealer GetDealer(long dId)
        {
            Dealer result = new Dealer();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Dealer.FirstOrDefault(m => m.Dealer_ID == dId);
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetDealer()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        #region 投诉
        public RetResult AddComplaint(Complaint model)
        {
            CmdResultError error = CmdResultError.EXCEPTION;
            string Msg = "抱歉，投诉建议提交失败！";
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    model.IsRead = (int)Common.EnumFile.IsRead.noRead;
                    dataContext.Complaint.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    error = CmdResultError.NONE;
                    Msg = "感谢您的宝贵意见！";
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.AddComplaint()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        #endregion
        #endregion

        /// <summary>
        /// 根据设置编码查找原料信息
        /// </summary>
        /// <param name="setID"></param>
        /// <returns></returns>
        public ScanSubstation GetSubstation(long setID)
        {
            ScanSubstation result = new ScanSubstation();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    string substationName = ",";
                    List<SubstationSingle> liSubstation = new List<SubstationSingle>();
                    List<View_BatchZuoye> liTeam = dataContext.View_BatchZuoye.Where(
                          m => m.SettingID == setID && m.Status == (int)Common.EnumFile.Status.used).ToList();
                    foreach (var item in liTeam)
                    {
                        if (item.TeamID > 0)
                        {
                            Team team = dataContext.Team.FirstOrDefault(p => p.TeamID == item.TeamID);
                            if (substationName.IndexOf("," + team.TeamName + ",") < 0)
                            {
                                substationName += team.TeamName + ",";
                            }
                        }
                        SubstationSingle single = new SubstationSingle();
                        single.ProcessName = item.OperationTypeName;
                        List<string> users = new List<string>();
                        if (item.UsersName != null)
                        {
                            foreach (var user in item.UsersName.Split(','))
                            {
                                try
                                {
                                    users.Add(dataContext.TeamUsers.FirstOrDefault(m => m.TeamUsersID == long.Parse(user)).UserName);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                        single.PersonName = users;
                        single.BatchZyId = item.Batch_ZuoYe_ID;
                        liSubstation.Add(single);
                    }
                    result.liSubstation = liSubstation;
                    result.SubstationName = substationName.Trim(',').Replace(',', ',');
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 获取班组详情
        /// </summary>
        /// <param name="batchZyId"></param>
        /// <returns></returns>
        public View_BatchZuoye GetBatchZuoye(long batchZyId)
        {
            View_BatchZuoye result = new View_BatchZuoye();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    View_BatchZuoye material = dataContext.View_BatchZuoye.FirstOrDefault(m => m.Batch_ZuoYe_ID == batchZyId);
                    #region 图片XML转JSON类
                    List<ToJsonImg> liImg = new List<ToJsonImg>();
                    List<ToJsonImg> liVid = new List<ToJsonImg>();
                    if (material.Files != null)
                    {
                        IEnumerable<XElement> allImg = material.Files.Elements("img");
                        foreach (var sub in allImg)
                        {
                            ToJsonImg img = new ToJsonImg();
                            img.fileUrl = sub.Attribute("value").Value;
                            img.fileUrls = sub.Attribute("small").Value;
                            liImg.Add(img);
                        }
                        IEnumerable<XElement> allVideo = material.Files.Elements("video");
                        foreach (var sub in allVideo)
                        {
                            ToJsonImg img = new ToJsonImg();
                            img.videoUrl = sub.Attribute("value").Value;
                            img.videoUrls = sub.Attribute("small").Value;
                            liVid.Add(img);
                        }
                    }
                    material.imgs = liImg;
                    material.videos = liVid;
                    //if (string.IsNullOrEmpty(item.UsersName)) continue;
                    //List<ToJsonProperty> userInfo = new List<ToJsonProperty>();
                    //foreach (var itemUser in item.UsersName.Split(','))
                    //{
                    //    ToJsonProperty user = new ToJsonProperty();
                    //    TeamUsers teamUsers = dataContext.TeamUsers.FirstOrDefault(
                    //        m => m.TeamUsersID == Convert.ToInt64(itemUser)
                    //        && m.Status == (int)Common.EnumFile.Status.used);
                    //    if (teamUsers != null)
                    //    {
                    //        user.pName = teamUsers.UserName;
                    //        user.pValue = teamUsers.TeamUsersID.ToString();
                    //        userInfo.Add(user);
                    //    }
                    //}
                    //item.users = userInfo;
                    #endregion
                    if (material != null)
                    {
                        result = material;
                    }
                }
            }
            catch { }
            return result;
        }
        /// <summary>
        /// 查询企业介绍及视频
        /// </summary>
        /// <param name="materialID">企业编号</param>
        /// <returns></returns>
        public ScanMaterialMemo GetMaterialMemo(long materialID)
        {
            ScanMaterialMemo result = new ScanMaterialMemo();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Material material = dataContext.Material.FirstOrDefault(m => m.Material_ID == materialID);
                    if (material != null)
                    {
                        result.Memo = material.Memo;
                        ToJsonImg imgUrls = new ToJsonImg();
                        if (material.MaterialImgInfo != null)
                        {
                            foreach (var item in material.MaterialImgInfo.Elements("video"))
                            {
                                imgUrls.videoUrl = item.Attribute("value").Value;
                                imgUrls.videoUrls = item.Attribute("small").Value;
                            }
                        }
                        result.VideoUrl = imgUrls;
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 仓储信息
        /// </summary>
        /// <returns></returns>
        public ScanWareHouseInfo GetWareHouse(long setID)
        {
            ScanWareHouseInfo result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    SetAmbient ambient = dataContext.SetAmbient.FirstOrDefault(
                          m => m.SettingID == setID);
                    SetOutWare outWare = dataContext.SetOutWare.FirstOrDefault(
                          m => m.SettingID == setID);
                    if (ambient != null)
                    {
                        result = result ?? new ScanWareHouseInfo();
                        result.InData = ambient.InDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                        result.OutData = ambient.OutDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                        result.Temperature = ambient.Temperature;
                        result.Remark = ambient.Remark;
                    }
                    if (outWare != null)
                    {
                        result = result ?? new ScanWareHouseInfo();
                        result.HanderUser = outWare.HanderUser;
                        result.WarehouseName = outWare.OutWareHouse;
                        result.WarehouseNum = outWare.OutBillNum;
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 查询企业物流信息
        /// </summary>
        /// <param name="setID"></param>
        /// <returns></returns>
        public ScanLogistics GetLogistics(long setID)
        {
            ScanLogistics result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    SetLogistics logistics = dataContext.SetLogistics.FirstOrDefault(
                          m => m.SetingID == setID);
                    if (logistics != null)
                    {
                        result = result ?? new ScanLogistics();
                        result.CarAmbient = logistics.CarAmbient;
                        result.CarNum = logistics.CarNum;
                        result.EndAddress = logistics.EndAddress;
                        result.EndDate = logistics.EndDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                        result.LogisticsNum = logistics.BillNum;
                        result.StartAddress = logistics.StartAddress;
                        result.StratDate = logistics.StartDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                        result.Url = logistics.Url;
                    }

                    SetOutWare outWare = dataContext.SetOutWare.FirstOrDefault(
                          m => m.SettingID == setID);
                    if (outWare != null)
                    {
                        result = result ?? new ScanLogistics();
                        result.Dealer = outWare.Targe;
                        result.SellDate = outWare.OutDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 企业信息
        /// </summary>
        /// <returns></returns>
        public ScanEnterprise GetEnterpriseMemo(long enterpriseId)
        {
            ScanEnterprise result = new ScanEnterprise();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == enterpriseId);
                    if (enterprise != null)
                    {
                        result.Address = enterprise.Address;
                        result.EnterpriseName = enterprise.EnterpriseName;
                        result.License = enterprise.BusinessLicence;
                        result.TelPhone = enterprise.LinkPhone;
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 查询企业商城信息
        /// </summary>
        /// <param name="enterpriseID">企业编号</param>
        /// <returns></returns>
        public ShopInfo GetShop(long enterpriseID)
        {
            ShopInfo result = new ShopInfo();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    View_EnterpriseShow enterprise = dataContext.View_EnterpriseShow.FirstOrDefault(p => p.Enterprise_Info_ID == enterpriseID);
                    if (enterprise != null)
                    {
                        //result.HotLine = enterprise.OrderingHotline;
                        result.HotLine = enterprise.LinkPhone;
                        result.EnterpriseUrl = enterprise.Url;
                        result.EnterpriseName = enterprise.CompanyName;
                        if (enterprise.Logo != null)
                        {
                            foreach (var item in enterprise.Logo.Elements("img"))
                            {
                                result.LogoUrl = item.Attribute("value").Value;
                            }
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        public EnterpriseShopLink ShopEn(long enterpriseID)
        {
            EnterpriseShopLink result = new EnterpriseShopLink();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.EnterpriseShopLink.FirstOrDefault(p => p.EnterpriseID == enterpriseID);
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        ///  获取产品评价
        /// </summary>
        /// <param name="marterialId">产品id</param>
        /// <returns></returns>
        public List<NewComplaint> GetEvaluation(long marterialId, long enterpriseId)
        {
            List<NewComplaint> result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.View_Complaint.Where(a => a.Material_ID == marterialId && a.Enterprise_Info_ID == enterpriseId).Select(a => new NewComplaint { Count = a.Count, Material_ID = a.Material_ID, ComplaintContent = a.ComplaintContent, Enterprise_Info_ID = a.Enterprise_Info_ID }).ToList();
                    var evaluationLst = dataContext.MaterialEvaluation.Where(a => a.MaterialID == marterialId && a.EnterpriseID == enterpriseId).Select(a => new NewComplaint { Count = 0, Material_ID = a.MaterialID, ComplaintContent = a.EvaluationName, Enterprise_Info_ID = a.EnterpriseID }).ToList();
                    foreach (var item in evaluationLst)
                    {
                        if (!result.Contains(item))
                        {
                            result.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return result;
        }

        /// <summary>
        /// 获取产品实施视频
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public MaterialShopLink GetMaterialShopLike(long materialId)
        {
            MaterialShopLink result = new MaterialShopLink();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    result = dataContext.MaterialShopLink.FirstOrDefault(a => a.MaterialID == materialId);
                    if (result != null)
                    {
                        result.videos = new List<ToJsonImg>();
                        foreach (var item in result.VideoUrl.Elements("video"))
                        {
                            result.videos.Add(new ToJsonImg { videoUrl = item.Attribute("url").Value });
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 判断是否有视频
        /// </summary>
        /// <param name="materialId">产品编号</param>
        /// <returns></returns>
        public bool MaterialVideo(long materialId)
        {
            bool result = false;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    Material material = dataContext.Material.FirstOrDefault(a => a.Material_ID == materialId);
                    if (material != null)
                    {
                        foreach (var item in material.MaterialImgInfo.Elements("video"))
                        {
                            if (item != null)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        public ScanInfo GetMaterialVideo(long materialId)
        {
            ScanInfo result = new ScanInfo();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    MaterialShopLink info = dataContext.MaterialShopLink.FirstOrDefault(m => m.MaterialID == materialId);
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    if (info.VideoUrl != null)
                    {
                        foreach (var item in info.VideoUrl.Elements())
                        {
                            switch (item.Name.ToString())
                            {
                                case "video":
                                    ToJsonImg img = new ToJsonImg();
                                    img.fileUrl = item.Attribute("videoname").Value;
                                    img.videoUrl = item.Attribute("url").Value;
                                    imgs.Add(img);
                                    break;
                            }
                        }
                    }
                    result.imgs = imgs;
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetMaterialVideo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 红包零钱记录
        /// </summary>
        /// <returns></returns>
        public List<View_RedSendChange> GetSendChange(long userId)
        {
            List<View_RedSendChange> sendChange = new List<View_RedSendChange>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var modelLst = dataContext.View_RedSendChange.Where(a => a.Order_Consumers_ID == userId).OrderByDescending(a => a.ActivityId);
                sendChange = modelLst.ToList();
            }
            return sendChange;
        }

        /// <summary>
        /// 获取提现记录
        /// </summary>
        /// <returns></returns>
        public List<View_SendedChange> SendedChange(long userId)
        {
            List<View_SendedChange> sendChange = new List<View_SendedChange>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                var modelLst = dataContext.View_SendedChange.Where(a => a.Order_Consumers_ID == userId);
                sendChange = modelLst.ToList();
            }
            return sendChange;
        }

        public RequestCodeSettingMuBan GetMuBanModel(long eid, long rid)
        {
            RequestCodeSettingMuBan result = new RequestCodeSettingMuBan();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.RequestCodeSettingMuBan.FirstOrDefault(m => m.EnterpriseID == eid && m.RequestCodeSettingID == rid);
                    #region 图片XML转JSON类
                    List<ToJsonImg> imgs = new List<ToJsonImg>();
                    if (result.MuBanImg != null)
                    {
                        IEnumerable<XElement> allImg = result.MuBanImg.Elements("img");
                        foreach (var item in allImg)
                        {
                            ToJsonImg sub = new ToJsonImg();
                            sub.fileUrl = item.Attribute("value").Value;
                            sub.fileUrls = item.Attribute("small").Value;
                            imgs.Add(sub);
                        }
                    }
                    result.imgs = imgs;
                    #endregion
                }
                catch (Exception ex)
                {
                    string errData = "ScanCodeDAL.GetMaterialVideo()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        #region 根据生产批号查询配置相关信息 20191105 王坤 待删除

        /// <summary>
        ///根据生产批号查询配置相关信息
        /// </summary>
        /// <param name="scBathcNo">生产批号</param>
        /// <param name="result">返回码信息</param>
        /// <returns></returns>
        //public CodeInfo GetSetInfo(string scBathcNo, string flowNo, ref CodeInfo result)
        //{
        //    using (DataClassesDataContext dataContext = GetDataContext())
        //    {
        //        result.FwCode = new Enterprise_FWCode_00();
        //        result.CodeSeting = new RequestCodeSetting();
        //        result.Display = new Display();
        //        try
        //        {
        //            int flow = Int32.Parse(flowNo);
        //            RequestCodeSetting seting = dataContext.RequestCodeSetting.FirstOrDefault(p => p.ShengChanPH == scBathcNo && p.beginCode <= flow && p.endCode >= flow);
        //            if (seting != null)
        //            {
        //                result.CodeType = (int)Common.EnumFile.AnalysisBased.Seting;
        //                if (seting.MaterialID != null) result.MaterialID = (long)seting.MaterialID;
        //                result.EnterpriseID = seting.EnterpriseId;
        //                result.DealerID = result.FwCode.Dealer_ID ?? 0;
        //                result.BrandID = seting.BrandID ?? 0;
        //                result.CodeSeting = seting;
        //                if (seting.RequestCodeType != null) result.RequestCodeType = seting.RequestCodeType.Value;
        //                if (seting.DisplayOption != null)
        //                {
        //                    string[] setArr = seting.DisplayOption.Split(',');
        //                    for (int i = 0; i < setArr.Length - 1; i++)
        //                    {
        //                        result.Display.Verification = true;
        //                        if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Brand)
        //                        {
        //                            result.Display.Brand = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Origin)
        //                        {
        //                            result.Display.Origin = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Work)
        //                        {
        //                            result.Display.Work = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Check)
        //                        {
        //                            result.Display.Check = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Report)
        //                        {
        //                            result.Display.Report = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.Ambient)
        //                        {
        //                            result.Display.Ambient = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingShow.WuLiu)
        //                        {
        //                            result.Display.WuLiu = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingDisplay.CreateDate)
        //                        {
        //                            result.Display.CreateDate = true;
        //                        }
        //                        else if (Convert.ToInt32(setArr[i]) == (int)Common.EnumFile.SettingDisplay.ScanCount)
        //                        {
        //                            result.Display.ScanCount = true;
        //                        }
        //                    }
        //                }
        //                if (result.RequestCodeType == (int)Common.EnumFile.RequestCodeType.fwzsCode)
        //                {
        //                    result.Display.Verification = true;
        //                }
        //                else if (result.RequestCodeType == (int)Common.EnumFile.RequestCodeType.TraceCode)
        //                {
        //                    result.Display.Verification = false;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            string errData = "ScanCodeDAL.GetSetInfo()";
        //            WriteLog.WriteErrorLog(errData + ":" + ex.Message);
        //        }
        //        return result;
        //    }
        //}

        #endregion

    }
}
