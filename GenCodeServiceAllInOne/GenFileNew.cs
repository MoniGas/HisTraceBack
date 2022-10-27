//生成码包服务 2018-12-19
using System;
using System.Collections.Generic;
using System.Threading;
using LinqModel;
using Dal;
using System.IO;
using Common.Argument;

namespace GenCodeServiceAllInOne
{
    /// <summary>
    /// 生成码包服务
    /// </summary>
    public static class GenFileNew
    {
        public static void Start()
        {
            ThreadStart start = doGenFile;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }
        public static void doGenFile()
        {
            //服务运行时间间隔
            int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
            while (true)
            {
                //生成码的配置信息表
                try
                {
                    ActivityDAL dal = new ActivityDAL();
                    List<GenCondeInfoSetting> setCode = dal.GetGencodeFileInfo();
                    foreach (GenCondeInfoSetting set in setCode)
                    {
                        #region 循环处理每一个数据库的业务
                        //数据库连接串
                        string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                            set.DataBaseIP.ToString(), set.DataBaseName.ToString(),
                            set.DatabaseUserID.ToString(), set.DatabasePWD.ToString());
                        //WriteLogTest("【"+DateTime.Now.ToString()+"】"+conStr, "GenFile");
                        GenFileDAL fileDal = new GenFileDAL();
                        List<RequestCodeSettingAdd> setAddList = fileDal.GetsSettingAdd(conStr);
                        //开始循环生成码包
                        foreach (RequestCodeSettingAdd setAdd in setAddList)
                        {
                            try
                            {
                                //WriteLogTest("【码包编号：】" + setAdd.ID.ToString(), "GenFile");
                                RequestCodeSetting codeSetting = fileDal.GetSetting((long)setAdd.ID, conStr);
                                if (codeSetting != null && codeSetting.ID>0)
                                {
                                    //WriteLogTest("【RequestCodeSetting编号：】" +  codeSetting.ID.ToString(), "GenFile");
                                    Material material = fileDal.GetMaterial((long)codeSetting.MaterialID, conStr);
                                    //WriteLogTest("【material编号：】" + material.Material_ID.ToString(), "GenFile");
                                    RequestCode requestCode = fileDal.GetRequestCode(codeSetting.RequestID, conStr);
                                    if (requestCode != null && requestCode.Route_DataBase_ID > 0)
                                    {
                                        //WriteLogTest("【requestCode编号：】" + requestCode.RequestCode_ID.ToString(), "GenFile");
                                        string sql;
                                        List<Enterprise_FWCode_00> fwCode = fileDal.GetFWCode(requestCode, codeSetting, conStr, out sql);
                                        //WriteLogTest("【sql查询语句：】" + sql, "GenFile");
                                        if (fwCode.Count > 0)
                                        {
                                            string baseFilePatch = set.CodeDirectot.ToString();
                                            string filePath = string.Format(baseFilePatch + "\\{0}\\{1}", codeSetting.EnterpriseId.ToString(),
                                                codeSetting.ID.ToString());
                                            if (!Directory.Exists(filePath))
                                            {
                                                Directory.CreateDirectory(filePath);
                                            }
                                            string fileName = codeSetting.ID.ToString() + ".txt";
                                            string zipfileName = codeSetting.ID.ToString() + ".zip";
                                            if (File.Exists(filePath + "\\" + fileName))
                                            {
                                                File.Delete(filePath + "\\" + fileName);
                                            }
                                            string codeInfo = string.Empty;

                                            //码包不显示URL 2019-11-12 刘晓杰
                                            //string ncpURL = set.FixURL;
                                            //string idCodeEWMURL = set.IDCodeFixURL;
                                            string ncpURL = "";
                                            //新加IDCode码网址前缀
                                            string idCodeEWMURL = "";

                                            int k = 0;
                                            #region 写码包
                                            foreach (Enterprise_FWCode_00 code in fwCode)
                                            {
                                                if (requestCode.Type == (int)Common.EnumFile.GenCodeType.trap && requestCode.CodeOfType != 1)//套标码(codeOfType=1为IDCode码2为简码3为农药码)
                                                {
                                                    if (code.Type == 4)
                                                    {
                                                        codeInfo = material.MaterialName + "\t" + ncpURL
                                                        + code.EWM + "\t" + code.FWCode + "\t" + "箱码";
                                                    }
                                                    else if (code.Type == 3)
                                                    {
                                                        codeInfo = material.MaterialName + "\t" + ncpURL
                                                        + code.EWM + "\t" + code.FWCode + "\t" + "产品码";
                                                    }
                                                }
                                                else if (requestCode.Type == (int)Common.EnumFile.GenCodeType.trap && requestCode.CodeOfType == 1)//IDCode套标码
                                                {
                                                    if (code.Type == 4)
                                                    {
                                                        codeInfo = material.MaterialName + "\t" + idCodeEWMURL
                                                        + code.EWM + "\t" + code.FWCode + "\t" + "箱码";
                                                    }
                                                    else if (code.Type == 3)
                                                    {
                                                        codeInfo = material.MaterialName + "\t" + idCodeEWMURL
                                                         + code.EWM + "\t" + code.FWCode + "\t" + "产品码";
                                                    }
                                                }
                                                else if (requestCode.CodeOfType == 1)//IDCode码
                                                {
                                                    codeInfo = material.MaterialName + "\t" + idCodeEWMURL
                                                         + code.EWM + "\t" + code.FWCode;
                                                }
                                                else
                                                {
                                                    codeInfo = material.MaterialName + "\t" + ncpURL
                                                        + code.EWM + "\t" + code.FWCode;
                                                }
                                                //sbCode.AppendLine(codeInfo);
                                                using (StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true))
                                                {
                                                    if (k == 0)
                                                    {
                                                        sw.WriteLine("每一行为一个二维码数据，自左向右三列分别为：产品名称、二维码内容和防伪校验码，印刷二维码时注意不要弄错！");
                                                    }
                                                    sw.WriteLine(codeInfo);
                                                    k++;
                                                }
                                            }
                                            Common.Tools.ZipClass.Zip(filePath + "\\" + fileName, filePath + "\\" + fileName.Replace("txt", "zip"), "");
                                            File.Delete(filePath + "\\" + fileName);
                                            #endregion
                                            //更新状态
                                            RetResult update = fileDal.UpeateState((long)setAdd.ID, filePath + "\\" + zipfileName,
                                                string.Format("/CodeFile/{0}/{1}", codeSetting.EnterpriseId.ToString(),
                                               setAdd.ID.ToString()) + "/" + zipfileName, conStr);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    GenCode.WriteLog(ex.Message, "GenFile");
                }
                Thread.Sleep(1000 * 120);
            }
        }

        public static void WriteLogTest(string msg, string directoryName)
        {
            string errlogpath = System.Configuration.ConfigurationManager.AppSettings["ErrLogPath"];
            errlogpath = errlogpath + "\\" + directoryName + "\\" + DateTime.Now.ToString("yyyyMM");
            string test = System.Configuration.ConfigurationManager.AppSettings["test"];
            if (test == "1")
            {
                if (!Directory.Exists(errlogpath))
                {
                    Directory.CreateDirectory(errlogpath);
                }
                using (StreamWriter sw = new StreamWriter(errlogpath + "\\" + DateTime.Now.ToString("dd") + "errorLog.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "------" + msg);
                }
            }
        }
    }
}
