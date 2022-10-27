using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using BLL;
using Common.Argument;
using LinqModel;
using Common;
using Common.Log;

namespace iagric_plant.Controllers
{
    public class Admin_RequestController : BaseController
    {
        /// <summary>
        /// 检索产品二维码申请记录
        /// </summary>
        /// <param name="bd">开始时间</param>
        /// <param name="ed">结束时间</param>
        /// <param name="ewm">产品二维码</param>
        /// <param name="name">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回申请记录列表</returns>
        public JsonResult Index(string bd, string ed, string mId, string mName, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;

            LinqModel.BaseResultList result = new RequestCodeBLL().GetList(user.EnterpriseID, null, mId, mName, bd, ed + " 23:59:59", pageIndex);

            return Json(result);
        }

        /// <summary>
        /// 检索产品二维码申请记录
        /// </summary>
        /// <param name="bd">开始时间</param>
        /// <param name="ed">结束时间</param>
        /// <param name="ewm">产品二维码</param>
        /// <param name="name">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回申请记录列表</returns>
        public JsonResult IndexBox(string bd, string ed, string mId, string mName, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;

            LinqModel.BaseResultList result = new RequestCodeBLL().GetBoxList(user.EnterpriseID, mId, mName, bd, ed + " 23:59:59", pageIndex);

            return Json(result);
        }

        public JsonResult SaveRequestCodeId(string RequestCodeId)
        {
            Session["SellRequestId"] = RequestCodeId;

            return Json(ToJson.NewRetResultToJson("1", ""));
        }

        public JsonResult GetLevelId()
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultModel model = ToJson.NewModelToJson(user, "1", "");
            return Json(model);
        }

        /// <summary>
        /// 查看二维码信息
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="rId">企业ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">分页页码</param>
        /// <returns></returns>
        public JsonResult SearchCode(string ewm, long rId, int status, int pageIndex = 1)
        {
            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();

            BaseResultList dataList = objRequestCodeBLL.GetEWMCode(ewm.Trim(), rId, status, pageIndex);

            return Json(dataList);
        }

        /// <summary>
        /// 查看销售记录
        /// </summary>
        /// <param name="ewm"></param>
        /// <param name="rId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetSaleList(string ewm, long rId, int pageIndex = 1)
        {
            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();

            BaseResultList dataList = objRequestCodeBLL.GetSaleList(ewm, rId, pageIndex);

            return Json(dataList);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Add(string mId, int? codeCount)
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultModel result = new RequestCodeBLL().Add(user.EnterpriseID, user.UpEnterpriseID, mId, codeCount, user.UserID);
            return Json(result);
        }

        public JsonResult SearchNameList()
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList result = new RequestCodeBLL().SearchNameList(user.EnterpriseID);

            return Json(result);
        }

        public JsonResult Packaging(string RequestCodeId, string CbxEwm, string CbxFile, string FilePassword, string CbxImage)
        {
            bool CbxEwmBool = CbxEwm == null ? false : Convert.ToBoolean(CbxEwm);
            bool CbxFileBool = CbxEwm == null ? false : Convert.ToBoolean(CbxFile);
            bool CbxImageBool = CbxEwm == null ? false : Convert.ToBoolean(CbxImage);

            if (CbxFileBool == false)
            {
                FilePassword = string.Empty;
            }

            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();
            RequestCode RequestCodeModel = objRequestCodeBLL.GetModel(Convert.ToInt64(RequestCodeId));

            BaseResultModel ObjResult = new BaseResultModel();
            if (RequestCodeModel == null)
            {
                ObjResult = ToJson.NewRetResultToJson("0", "数据错误");

                return Json(ObjResult);
            }
            string StrUrl = string.Empty;
            if (RequestCodeModel.Type.Value == 2)
            {
                bool flag = PackagingSingle(Convert.ToInt64(RequestCodeId), FilePassword, Convert.ToBoolean(CbxEwm), Convert.ToBoolean(CbxImage), ref StrUrl);

                ObjResult = ToJson.NewRetResultToJson(Convert.ToInt32(flag).ToString(), flag == true ? StrUrl : "下载失败！");
            }
            else if (RequestCodeModel.Type.Value == 1)
            {
                bool flag = PackagingGroup(Convert.ToInt64(RequestCodeId), FilePassword, Convert.ToBoolean(CbxEwm), Convert.ToBoolean(CbxImage), ref StrUrl);

                ObjResult = ToJson.NewRetResultToJson(Convert.ToInt32(flag).ToString(), flag == true ? StrUrl : "下载失败！");
            }
            return Json(ObjResult);
        }

        public static bool PackagingSingle(long requestCode_ID, string filePassword, bool IsEncryption, bool Check_Image, ref string StrUrl)
        {
            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();

            LoginInfo ObjLoginInfo = Common.Argument.SessCokie.Get;
            string sUrl = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            string downLoadUrl = string.Empty;
            string eId = string.Empty;
            string webUrl = string.Empty;
            try
            {
                RequestCode requestCode = objRequestCodeBLL.GetModel(requestCode_ID);
                if (requestCode == null)
                {
                    return false;
                }
                long creatCount = requestCode.TotalNum.Value;
                eId = requestCode.Enterprise_Info_ID.ToString();
                webUrl = string.Format("{0}DownloadCode\\{1}\\", System.AppDomain.CurrentDomain.BaseDirectory, eId);//根目录
                int insertCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["InsertCountTXT"].ToString());
                long pageCount = 0;
                if (creatCount % insertCount == 0)
                {
                    pageCount = creatCount / insertCount;
                }
                else
                {
                    pageCount = creatCount / insertCount + 1;
                }

                string ioidURL = System.Configuration.ConfigurationManager.AppSettings["idcodeURL"].ToString();
                LoginInfo pf = Common.Argument.SessCokie.Get;
                if (pf.Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                {
                    ioidURL = System.Configuration.ConfigurationManager.AppSettings["ncpURL"].ToString();
                }
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < pageCount; i++)
                {
                    sb.Clear();
                    List<Enterprise_FWCode_00> pd = objRequestCodeBLL.GetEwmList(requestCode_ID, i + 1, insertCount);//根据Request_ID获取数据分页
                    StringBuilder sbMainCode = new StringBuilder();
                    if (pd != null && pd.Count > 0)
                    {
                        sbMainCode.Clear();
                        //i.1.130105.28.8.1.1000000001.1
                        string[] sss = pd[0].EWM.Split('.');
                        for (int j = 0; j < 5; j++)
                        {
                            sbMainCode.Append(sss[j] + ".");
                        }
                        string mainCode = sbMainCode.ToString();
                        Random rd = new Random();
                        foreach (Enterprise_FWCode_00 ItemModel in pd)
                        {
                            // 不加密的流水号
                            string TempSerial = ItemModel.EWM.Replace(mainCode, "");
                            // 数字防伪码
                            string FwCode = ItemModel.FWCode;

                            if (IsEncryption)
                            {
                                string EncryptionSerial = new Encryption().Algorithm(TempSerial, rd.Next(0, 6));

                                // 判断为图形防伪,为了和二维码区分开，防伪码多一个节点，由于是加密二维码，多的以为节点为字母，以区分明码二维码
                                if (Check_Image)
                                {
                                    FwCode = ioidURL + ItemModel.EWM.Replace(TempSerial, EncryptionSerial) + ".8." + FwCode;
                                }

                                sb.AppendFormat("{0},{1},{2}{3}\r\n", EncryptionSerial, FwCode, ioidURL, ItemModel.EWM.Replace(TempSerial, EncryptionSerial));
                            }
                            else
                            {
                                // 判断为图形防伪，为了和二维码区分开，防伪码多一个节点，由于是明文二维码，多的以为节点为数字，以区分加密二维码
                                if (Check_Image)
                                {
                                    FwCode = ioidURL + ItemModel.EWM + ".8." + FwCode;
                                }

                                sb.AppendFormat("{0},{1},{2}{3}\r\n", TempSerial, FwCode, ioidURL, ItemModel.EWM);
                            }

                        }
                        string txtName = string.Format("{0}-{1}.txt", pd[0].EWM, pd[pd.Count - 1].EWM);
                        if (!System.IO.Directory.Exists(string.Format("{0}{1}", webUrl, sUrl)))
                        {
                            System.IO.Directory.CreateDirectory(string.Format("{0}{1}", webUrl, sUrl));
                            System.IO.Directory.CreateDirectory(string.Format("{0}{1}\\{2}", webUrl, sUrl, sUrl));
                        }
                        WriteTXT(string.Format("{0}{1}\\{2}\\{3}", webUrl, sUrl, sUrl, txtName), sb.ToString());
                    }
                }
                //Common.Tools.ZipClass.Zip(string.Format("{0}{1}", webUrl, sUrl), string.Format("{0}\\{1}.rar", webUrl, sUrl), filePassword);
                new ZipCompressClass().SetZipFile(string.Format("{0}{1}", webUrl, sUrl), string.Format("{0}{1}.rar", webUrl, sUrl), filePassword);
                System.IO.Directory.Delete(string.Format("{0}{1}", webUrl, sUrl), true);
                downLoadUrl = string.Format("/DownloadCode/{0}/{1}.rar", eId, sUrl);

                //更改申请状态
                objRequestCodeBLL.ChangeStatus(requestCode_ID, (int)Common.EnumFile.RequestCodeStatus.PackToSuccess, downLoadUrl, filePassword, IsEncryption, Check_Image);
                StrUrl = downLoadUrl;
                return true;
            }
            catch (Exception ex)
            {
                if (System.IO.Directory.Exists(string.Format("{0}{1}", webUrl, sUrl)))
                {
                    System.IO.Directory.Delete(string.Format("{0}{1}", webUrl, sUrl), true);
                }
                objRequestCodeBLL.ChangeStatus(requestCode_ID, (int)Common.EnumFile.RequestCodeStatus.PackagingFailure, downLoadUrl);
                return false;
            }
        }

        public static bool PackagingGroup(long requestCode_ID, string filePassword, bool IsEncryption, bool Check_Image, ref string StrUrl)
        {
            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();

            LoginInfo ObjLoginInfo = Common.Argument.SessCokie.Get;
            string sUrl = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            string downLoadUrl = string.Empty;
            string eId = string.Empty;
            string webUrl = string.Empty;
            try
            {
                // 获取二维码申请信息
                RequestCode requestCode = objRequestCodeBLL.GetModel(requestCode_ID);
                // 验证二维码申请信息非空
                if (requestCode == null)
                {
                    return false;
                }
                // 获取创建的套标码套数
                long creatCount = requestCode.TotalNum.Value;
                // 获取企业ID
                eId = requestCode.Enterprise_Info_ID.ToString();
                //根目录
                webUrl = string.Format("{0}DownloadCode\\{1}\\", System.AppDomain.CurrentDomain.BaseDirectory, eId);
                // 每个文件最大二维码套数
                int insertCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["InsertCountTXT"].ToString());
                // 获取可生成的文件数
                long pageCount = 0;
                if (creatCount % insertCount == 0)
                {
                    pageCount = creatCount / insertCount;
                }
                else
                {
                    pageCount = creatCount / insertCount + 1;
                }
                // 获取解析地址
                string ioidURL = System.Configuration.ConfigurationManager.AppSettings["idcodeURL"].ToString();
                LoginInfo pf = Common.Argument.SessCokie.Get;
                if (pf.Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                {
                    ioidURL = System.Configuration.ConfigurationManager.AppSettings["ncpURL"].ToString();
                }
                // 存储写入字符串
                StringBuilder sb = new StringBuilder();
                // 循环可创建的文件个数
                for (int i = 0; i < pageCount; i++)
                {
                    sb.Clear();
                    // 获取每个文件存储的二维码信息集合
                    List<Enterprise_FWCode_00> pd = objRequestCodeBLL.GetEwmList(requestCode_ID, i + 1, insertCount);//根据Request_ID获取数据分页
                    // 存储企业主码，二维码去除这段就是要显示的流水号
                    StringBuilder sbMainCode = new StringBuilder();
                    // 判断二维码信息集合非空
                    if (pd != null && pd.Count > 0)
                    {
                        sbMainCode.Clear();
                        //i.1.130105.28.8.1.1000000001.1
                        string[] sss = pd[0].EWM.Split('.');
                        for (int j = 0; j < 5; j++)
                        {
                            sbMainCode.Append(sss[j] + ".");
                        }
                        string mainCode = sbMainCode.ToString();

                        int n = 0;
                        Random rd = new Random();
                        foreach (Enterprise_FWCode_00 ItemModel in pd)
                        {
                            if (n > 0 && ItemModel.Type == 1)
                            {
                                sb.Remove(sb.Length - 1, 1);
                                sb.Append("\r\n");
                            }

                            // 不加密的流水号
                            string TempSerial = ItemModel.EWM.Replace(mainCode, "");
                            // 数字防伪码
                            string FwCode = ItemModel.FWCode;

                            // 判断打包时加密
                            if (IsEncryption)
                            {
                                string EncryptionSerial = new Encryption().Algorithm(TempSerial, rd.Next(0, 6));

                                // 判断为图形防伪,为了和二维码区分开，防伪码多一个节点，由于是加密二维码，多的以为节点为字母，以区分明码二维码
                                if (Check_Image)
                                {
                                    // 图形防伪多两个节点，以区分是防伪码，8代表防伪码
                                    FwCode = ioidURL + ItemModel.EWM.Replace(TempSerial, EncryptionSerial) + ".8." + FwCode;
                                }

                                sb.AppendFormat("{0},{1},{2}{3};", EncryptionSerial, FwCode, ioidURL, ItemModel.EWM.Replace(TempSerial, EncryptionSerial));
                            }
                            else // 打包时二维码不加密
                            {
                                // 图形防伪多两个节点，以区分是防伪码，8代表防伪码
                                if (Check_Image)
                                {
                                    FwCode = ioidURL + ItemModel.EWM + ".8." + FwCode;
                                }

                                sb.AppendFormat("{0},{1},{2}{3};", TempSerial, FwCode, ioidURL, ItemModel.EWM);
                            }
                            n++;
                        }

                        string txtName = string.Format("{0}-{1}.txt", pd[0].EWM, pd[pd.Count - 1].EWM);
                        if (!System.IO.Directory.Exists(string.Format("{0}{1}", webUrl, sUrl)))
                        {
                            System.IO.Directory.CreateDirectory(string.Format("{0}{1}", webUrl, sUrl));
                            System.IO.Directory.CreateDirectory(string.Format("{0}{1}\\{2}", webUrl, sUrl, sUrl));
                        }
                        sb.Remove(sb.Length - 1, 1);
                        WriteTXT(string.Format("{0}{1}\\{2}\\{3}", webUrl, sUrl, sUrl, txtName), sb.ToString());
                    }
                }

                //Common.Tools.ZipClass.Zip(string.Format("{0}{1}", webUrl, sUrl), string.Format("{0}\\{1}.rar", webUrl, sUrl), filePassword);
                new ZipCompressClass().SetZipFile(string.Format("{0}{1}", webUrl, sUrl), string.Format("{0}{1}.rar", webUrl, sUrl), filePassword);
                System.IO.Directory.Delete(string.Format("{0}{1}", webUrl, sUrl), true);
                downLoadUrl = string.Format("/DownloadCode/{0}/{1}.rar", eId, sUrl);
                //更改申请状态
                objRequestCodeBLL.ChangeStatus(requestCode_ID, (int)Common.EnumFile.RequestCodeStatus.PackToSuccess, downLoadUrl, filePassword, IsEncryption, Check_Image);
                StrUrl = downLoadUrl;
                return true;
            }
            catch (Exception ex)
            {
                if (System.IO.Directory.Exists(string.Format("{0}{1}", webUrl, sUrl)))
                {
                    System.IO.Directory.Delete(string.Format("{0}{1}", webUrl, sUrl), true);
                }
                objRequestCodeBLL.ChangeStatus(requestCode_ID, (int)Common.EnumFile.RequestCodeStatus.PackagingFailure, downLoadUrl);
                return false;
            }
        }

        /// <summary>
        /// 生成TXT文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        static void WriteTXT(string fileName, string content)
        {
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.Create(fileName).Close();
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true, Encoding.Default);
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
        }

        [HttpGet]
        public int DownloadFile(string RequestCodeId, string downLoadURL)
        {
            try
            {
                string FullFileName = Server.MapPath(string.Format(downLoadURL)); //FileName--要下载的文件名 
                System.IO.FileInfo DownloadFile = new System.IO.FileInfo(FullFileName);
                string strFileName = Path.GetFileName(FullFileName);
                if (DownloadFile.Exists)
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = false;
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + strFileName);
                    Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                    Response.WriteFile(DownloadFile.FullName);
                    Response.Flush();
                    Response.End();
                    return 1;
                }
                else
                {
                    //文件不存在 
                }
            }
            catch
            {
                //文件不存在
            }
            return 0;
        }

        public JsonResult UpdateDownLoadNum(string RequestCodeId)
        {
            LinqModel.BaseResultModel ObjBaseResultModel = new RequestCodeBLL().UpdateDownLoadNum(RequestCodeId);

            return Json(ObjBaseResultModel);
        }

        public JsonResult GetRequestCodelModel(string RequestCodeId)
        {
            RequestCodeBLL ObjRequestCodeBLL = new RequestCodeBLL();
            RequestCode model = ObjRequestCodeBLL.GetModel(Convert.ToInt64(RequestCodeId));

            return Json(ToJson.NewModelToJson(model, model == null ? "0" : "1", model == null ? "数据不存在或网络无法连接！" : "获取数据成功！"));
        }

        /// <summary>
        /// 获取申请码记录ID
        /// </summary>
        /// <param name="bName">批次名称</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRequestId(string bName)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new RequestCodeBLL().GetRequestID(bName);
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestController.GetRequestId";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 生成追溯码方法
        /// 2018-04-19 新增农药二维码
        /// 2018-08-03 新增简码规则
        /// </summary>
        /// <param name="Material_ID">产品Id</param>
        /// <param name="RequestCount">数量</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult Generate(string Material_ID, string RequestCount, string displayOption, string StyleModel, 
            string guigeId, string type, string scleixing, DateTime requestDate, int codeOfType, string sCodeLength,
            string bzSpecType,string shengchanPH, DateTime YouXiaoDate, DateTime ShiXiaoDate)
        {
            LinqModel.BaseResultModel ResultModel = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            RequestCode requestcodeModel = new RequestCode();
            RequestCodeSetting settingModel = new RequestCodeSetting();
            requestcodeModel.Enterprise_Info_ID = user.EnterpriseID;
            requestcodeModel.Material_ID = Convert.ToInt64(Material_ID);
            requestcodeModel.Specifications = 0;
            settingModel.BathPartType = (int)Common.EnumFile.BatchPartType.Split;
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.trap)
            {
                SpecificationBLL dal = new SpecificationBLL();
                Specification Sp = dal.GetInfoNew(Convert.ToInt64(guigeId));
                if (Sp != null)
                {
                    requestcodeModel.Specifications = type == "9" ? Convert.ToInt32(Sp.Value) : 0;
                    requestcodeModel.GuiGe = Sp.GuiGe;
                }
                requestcodeModel.TotalNum = Convert.ToInt32(RequestCount) * requestcodeModel.Specifications + Convert.ToInt32(RequestCount);
                requestcodeModel.Trap = Convert.ToInt32(RequestCount);
                settingModel.BatchTrap = Convert.ToInt32(RequestCount);
            }
            else
            {
                requestcodeModel.TotalNum = Convert.ToInt32(RequestCount);
            }
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.single && codeOfType != (int)Common.EnumFile.CodeOfType.SCode)
            {
                settingModel.BathPartType = (int)Common.EnumFile.BatchPartType.All; 
            }
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.pesticides)
            {
                MaterialSpcificationBLL bll = new MaterialSpcificationBLL(); ;
                MaterialSpcification spection = bll.GetMsModel(Convert.ToInt64(guigeId));
                if (spection != null && !string.IsNullOrEmpty(spection.MaterialSpcificationCode))
                {
                    requestcodeModel.Specifications = Convert.ToInt32(spection.Value);
                    requestcodeModel.GuiGe = spection.MaterialSpcificationCode;
                }
                else
                {
                    ResultModel.Msg = "产品规格编码不存在，请重新维护产品规格";
                    return Json(ResultModel);
                }
                if (string.IsNullOrEmpty(scleixing) || scleixing.Length != 1)
                {
                    ResultModel.Msg = "农药二维码请选择生产类型";
                    return Json(ResultModel);
                }
            }
            if (codeOfType == 1 || Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.pesticides)
            {
                requestcodeModel.SCodeLength = null;
            }
            else
            {
                requestcodeModel.SCodeLength = Convert.ToInt32(sCodeLength);
            }
            requestcodeModel.adddate = DateTime.Now;
            requestcodeModel.adduser = user.UserID;
            requestcodeModel.saleCount = 0;
            requestcodeModel.Status = (int)Common.EnumFile.RequestCodeStatus.Unaudited;
            requestcodeModel.IsRead = (int)Common.EnumFile.IsRead.noRead;
            requestcodeModel.IsLocal = (int)Common.EnumFile.Islocal.cloud;
            requestcodeModel.RequestDate = requestDate;
            requestcodeModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.TraceCode;
            requestcodeModel.Type = Convert.ToInt32(type);
            //20191031医疗器械新加
            requestcodeModel.ShengChanPH = shengchanPH;
            requestcodeModel.YouXiaoDate = YouXiaoDate.ToString();
            requestcodeModel.ShiXiaoDate = ShiXiaoDate.ToString();
            requestcodeModel.BZSpecType = Convert.ToInt32(bzSpecType);
            requestcodeModel.ISDowm = (int)Common.EnumFile.CodeISDowm.NoDowm;
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.pesticides)
            {
                requestcodeModel.CodeOfType = (int)Common.EnumFile.CodeOfType.pesticides;
            }
            else
            {
                //新加生成的码制（IDCode码/简码）
                requestcodeModel.CodeOfType = codeOfType;
            }
            settingModel.BatchType = 1;
            settingModel.Count = requestcodeModel.TotalNum.Value;
            settingModel.DisplayOption = displayOption;
            settingModel.EnterpriseId = user.EnterpriseID;
            settingModel.SetDate = DateTime.Now;
            //20191031医疗器械新加
            settingModel.ShengChanPH = shengchanPH;
            settingModel.YouXiaoDate = YouXiaoDate;
            settingModel.ShiXiaoDate = ShiXiaoDate;
            settingModel.BZSpecType = Convert.ToInt32(bzSpecType);
            if (StyleModel == null)
            {
                settingModel.StyleModel = 0;
            }
            else
            {
                settingModel.StyleModel = Convert.ToInt32(StyleModel);
            }
            settingModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.TraceCode;
            ResultModel = new RequestCodeBLL().Generate(requestcodeModel, settingModel, scleixing);
            return Json(ResultModel);
        }

        /// <summary>
        /// 生成包材码
        /// </summary>
        /// 2018-08-03 新增简码规则
        /// <param name="GuiGeID">规格编码</param>
        /// <param name="Material_ID">产品编码</param>
        /// <param name="RequestCount">数量</param>
        /// <param name="codeOfType">码制标识（1：IDCode码；2：简码）</param>
        /// <returns></returns>
        public JsonResult GeneratePackCode(string codeAttribute, string Material_ID, string RequestCount, int codeOfType)
        {
            LoginInfo user = SessCokie.Get;
            LinqModel.BaseResultModel ResultModel = new RequestCodeBLL().GeneratePackCode(user.EnterpriseID, codeAttribute, user.UserID, Material_ID, RequestCount, codeOfType);
            return Json(ResultModel);
        }

        /// <summary>
        /// 追溯码详情管理查看码
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="sId">设置表</param>
        /// <param name="status">ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult RequestCodeSettingCode(string ewm, long sId, int status, int pageIndex = 1)
        {
            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();
            BaseResultList dataList = objRequestCodeBLL.GetSettingCode(ewm.Trim(), sId, status, pageIndex);
            if (dataList.totalCounts == 0)
            {
                dataList.ObjList = 0;
            }
            return Json(dataList);
        }

        /// <summary>
        /// 生成防伪码方法new20170317
        /// </summary>
        /// <param name="Material_ID">产品Id</param>
        /// <param name="RequestCount">数量</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult SecurityCode(string Material_ID, string RequestCount, string guigeId, string type, DateTime requestDate)
        {
            LoginInfo user = SessCokie.Get;
            RequestCode requestcodeModel = new RequestCode();
            RequestCodeSetting settingModel = new RequestCodeSetting();
            requestcodeModel.Enterprise_Info_ID = user.EnterpriseID;
            requestcodeModel.Material_ID = Convert.ToInt64(Material_ID);
            requestcodeModel.adddate = DateTime.Now;
            requestcodeModel.saleCount = 0;
            requestcodeModel.Status = (int)Common.EnumFile.RequestCodeStatus.Unaudited;
            requestcodeModel.IsRead = (int)Common.EnumFile.IsRead.noRead;
            requestcodeModel.RequestDate = requestDate;
            requestcodeModel.Type = Convert.ToInt32(type);
            requestcodeModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.SecurityCode;
            requestcodeModel.CodeOfType = (int)Common.EnumFile.CodeOfType.IDCode;
            requestcodeModel.Specifications = 0;
            SpecificationBLL dal = new SpecificationBLL();
            Specification Sp = dal.GetInfoNew(Convert.ToInt64(guigeId));
            if (Sp != null)
            {
                requestcodeModel.Specifications = type == "9" ? Convert.ToInt32(Sp.Value) : 0;
                requestcodeModel.GuiGe = Sp.GuiGe;
            }
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.trap)
            {
                requestcodeModel.TotalNum = Convert.ToInt32(RequestCount) * requestcodeModel.Specifications + Convert.ToInt32(RequestCount);
                requestcodeModel.Trap = Convert.ToInt32(RequestCount);
                settingModel.BatchTrap = Convert.ToInt32(RequestCount);
            }
            else
            {
                requestcodeModel.TotalNum = Convert.ToInt32(RequestCount);
            }
            settingModel.BatchType = 1;
            settingModel.beginCode = 1;
            settingModel.endCode = Convert.ToInt64(RequestCount);
            settingModel.Count = requestcodeModel.TotalNum.Value;
            settingModel.EnterpriseId = user.EnterpriseID;
            settingModel.SetDate = DateTime.Now;
            settingModel.StyleModel = 0;
            settingModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.SecurityCode;
            LinqModel.BaseResultModel ResultModel = new RequestCodeBLL().Generate(requestcodeModel, settingModel, "");
            return Json(ResultModel);
        }


        /// <summary>
        /// 生成追溯码方法(在此基础上修改为模板4添加图片而添加)
        /// 2018-04-19 新增农药二维码
        /// 2018-08-03 新增简码规则
        /// 2018-09-04新增两个模板为模板4添加图片和链接
        /// </summary>
        /// <param name="Material_ID">产品Id</param>
        /// <param name="RequestCount">数量</param>
        /// <returns>返回操作结果对象</returns>
        public JsonResult GenerateMuBan(string Material_ID, string RequestCount, string displayOption, string StyleModel, string guigeId, string type, string scleixing, DateTime requestDate, int codeOfType,
            string MuBanImg, string ImgLink, int IsShow, string sCodeLength)
        {
            LinqModel.BaseResultModel ResultModel = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            RequestCode requestcodeModel = new RequestCode();
            RequestCodeSetting settingModel = new RequestCodeSetting();
            //2018-09-04新加模板4图片和图片链接
            RequestCodeSettingMuBan mubanModel = new RequestCodeSettingMuBan();
            requestcodeModel.Enterprise_Info_ID = user.EnterpriseID;
            requestcodeModel.Material_ID = Convert.ToInt64(Material_ID);
            requestcodeModel.Specifications = 0;
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.trap)
            {
                SpecificationBLL dal = new SpecificationBLL();
                Specification Sp = dal.GetInfoNew(Convert.ToInt64(guigeId));
                if (Sp != null)
                {
                    requestcodeModel.Specifications = type == "9" ? Convert.ToInt32(Sp.Value) : 0;
                    requestcodeModel.GuiGe = Sp.GuiGe;
                }
                requestcodeModel.TotalNum = Convert.ToInt32(RequestCount) * requestcodeModel.Specifications + Convert.ToInt32(RequestCount);
                requestcodeModel.Trap = Convert.ToInt32(RequestCount);
                settingModel.BatchTrap = Convert.ToInt32(RequestCount);
            }
            else
            {
                requestcodeModel.TotalNum = Convert.ToInt32(RequestCount);
            }
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.pesticides)
            {
                MaterialSpcificationBLL bll = new MaterialSpcificationBLL(); ;
                MaterialSpcification spection = bll.GetMsModel(Convert.ToInt64(guigeId));
                if (spection != null && !string.IsNullOrEmpty(spection.MaterialSpcificationCode))
                {
                    requestcodeModel.Specifications = Convert.ToInt32(spection.Value);
                    requestcodeModel.GuiGe = spection.MaterialSpcificationCode;
                }
                else
                {
                    ResultModel.Msg = "产品规格编码不存在，请重新维护产品规格";
                    return Json(ResultModel);
                }
                if (string.IsNullOrEmpty(scleixing) || scleixing.Length != 1)
                {
                    ResultModel.Msg = "农药二维码请选择生产类型";
                    return Json(ResultModel);
                }
            }
            if (codeOfType == 1 || Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.pesticides)
            {
                requestcodeModel.SCodeLength = null;
            }
            else
            {
                requestcodeModel.SCodeLength = Convert.ToInt32(sCodeLength);
            }
            requestcodeModel.adddate = DateTime.Now;
            requestcodeModel.adduser = user.UserID;
            requestcodeModel.saleCount = 0;
            requestcodeModel.Status = (int)Common.EnumFile.RequestCodeStatus.Unaudited;
            requestcodeModel.IsRead = (int)Common.EnumFile.IsRead.noRead;
            requestcodeModel.IsLocal = (int)Common.EnumFile.Islocal.cloud;
            requestcodeModel.RequestDate = requestDate;
            requestcodeModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.TraceCode;
            requestcodeModel.Type = Convert.ToInt32(type);
            if (Convert.ToInt32(type) == (int)Common.EnumFile.GenCodeType.pesticides)
            {
                requestcodeModel.CodeOfType = (int)Common.EnumFile.CodeOfType.pesticides;
            }
            else
            {
                //新加生成的码制（IDCode码/简码）
                requestcodeModel.CodeOfType = codeOfType;
            }
            settingModel.BatchType = 1;
            settingModel.Count = requestcodeModel.TotalNum.Value;
            settingModel.DisplayOption = displayOption;
            settingModel.EnterpriseId = user.EnterpriseID;
            settingModel.SetDate = DateTime.Now;
            if (StyleModel == null)
            {
                settingModel.StyleModel = 0;
            }
            else
            {
                settingModel.StyleModel = Convert.ToInt32(StyleModel);
            }
            settingModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.TraceCode;
            mubanModel.StrMuBanImg = MuBanImg;
            mubanModel.ImgLink = ImgLink;
            mubanModel.EnterpriseID = user.EnterpriseID;
            mubanModel.AddDate = DateTime.Now;
            mubanModel.AddUserID = user.UserID;
            mubanModel.IsShow = IsShow;
            ResultModel = new RequestCodeBLL().GenerateMuBan(requestcodeModel, settingModel, mubanModel, scleixing);
            return Json(ResultModel);
        }

        #region 医疗器械验证生成码是生产批号是否重复
        public JsonResult YanZhengPH(string Material_ID, string bzSpecType,string shengchanPH)
        {
            LinqModel.BaseResultModel ResultModel = new BaseResultModel();
            LoginInfo user = SessCokie.Get;
            ResultModel = new RequestCodeBLL().YanZhengPH(user.EnterpriseID, Convert.ToInt32(bzSpecType), Convert.ToInt64(Material_ID), shengchanPH);
            return Json(ResultModel);
        }
        #endregion
    }
}
