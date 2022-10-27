/***
 * 刘晓杰于2019年10月28日从CFBack项目移入此文件
 ***/
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Web;
using Common;
using Common.Argument;
using System.IO;
using BLL;
using LinqModel;
using System.Web.Script.Serialization;
using Webdiyer.WebControls.Mvc;
using InterfaceWeb;
using iagric_plant.Models;
using LinqModel.InterfaceModels;
using Newtonsoft.Json;
namespace iagric_plant
{
    /// <summary>
    /// Interface 的摘要说明
    /// </summary>
    public class Interface : IHttpHandler
    {
        private string _webURL = "", _path = "";
        public void ProcessRequest(HttpContext context)
        {
            var url = context.Request.RawUrl;
            _webURL = context.Request.Url.AbsoluteUri.Replace(url, "");
            _path = context.Request.PhysicalApplicationPath;
            WriteLog(url);
            //取得处事类型
            //string action = DTRequest.GetQueryString("Action");
            string action = DTRequest.GetPostParam("Action");
            switch (action)
            {
                case "Login":
                    UserLogin(context);
                    break;
                case "Down_Material":
                    Down_Material(context);
                    break;
                case "Down_MaterialCode":
                    Down_MaterialCode(context);
                    break;
                case "Down_MaterialBoxCode":
                    Down_MaterialBoxCode(context);
                    break;
                case "UpFile":
                    UpFile(context);
                    break;
                case "IDCodeUserLogin":
                    IDCodeUserLogin(context);
                    break;
                case "AddEnterprise":
                    AddEnterpriseIdcodeHas(context);
                    break;
                case "GetLabelTem":
                    GetLabelTem(context);
                    break;
                case "UDILogin":
                    UDILogin(context);
                    break;
                case "GetGYJToken":
                    GetGYJToken(context);
                    break;
                case "GetMaterialDICount":
                    GetMaterialDICount(context);
                    break;
                case "LoginEx":
                    UserLoginEx(context);
                    break;
                case "UpSyMaterialPI":
                    UpSyMaterialPI(context);
                    break;
                case "SynchroUDIPI":
                    SynchroUDIPI(context);
                    break;
                case "UpSyMaterial":
                    UpSyMaterial(context);
                    break;
                case "SynchroUDIDI":
                    SynchroUDIDI(context);
                    break;
                case "UpdateDI":
                    UpdateDI(context);
                    break;
                case "PingTrace":
                    PingTrace(context);
                    break;
                case "SynchroUDIPICode":
                    SynchroUDIPICode(context);
                    break;
                case "EditEnterprise":
                    EditEnterprise(context);
                    break;
                case "JYEnLogin":
                    JYEnLogin(context);
                    break;
                case "JYUpSyMaterial":
                    JYUpSyMaterial(context);
                    break;
                case "JYSynchroUDIDI":
                    JYSynchroUDIDI(context);
                    break;
                case "JYUpSyMaterialPI":
                    JYUpSyMaterialPI(context);
                    break;
                case "JYSynchroUDIPI":
                    JYSynchroUDIPI(context);
                    break;
                case "VersionInfo":
                    VersionInfo(context);
                    break;
                case "SynchroUDIByEnterprise":
                    SynchroUDIByEnterprise(context);
                    break;
                case "SetUtcToken":
                    SetUtcToken(context);
                    break;
                case "GetUDIData":
                    GetUDIData(context);
                    break;
                case "GetUDIDI":
                    GetUDIDI(context);
                    break;
                case "GetExpired":
                    GetExpired(context);
                    break;
                case "upPIFile":
                    upPIFile(context);
                    break;
                case "PrivateUpSyMaterial":
                    PrivateUpSyMaterial(context);
                    break;
                case "PrivateLogin":
                    PrivateLogin(context);
                    break;
                default:
                    getUpLst(context);
                    break;
            }
        }
        #region 私有化部署接口

        #region 登录
        private void PrivateLogin(HttpContext context)
        {
            string loginname = DTRequest.GetPostParam("LoginName");
            string password = DTRequest.GetPostParam("PassWord");
            string token= DTRequest.GetPostParam("Token");
            string tokencode= DTRequest.GetPostParam("TokenCode");
            string serviceID = DTRequest.GetPostParam("ServiceID");
            string enterpeiseCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseCode"].Trim();
            ServiceJK.WebService1SoapClient cl = new ServiceJK.WebService1SoapClient();
            string enMainCode = cl.GetEnterpriseMainCode(enterpeiseCode);
            Enterprise_UserBLL bll = new Enterprise_UserBLL();
            BaseResultModel resultM = new BaseResultModel();
            resultM = bll.PrivateLogin(loginname, password, token, tokencode, serviceID, enMainCode);
            context.Response.Write(new JavaScriptSerializer().Serialize(resultM));
        }
        #endregion

        #region 同步DI和PI

        #region 同步DI
        private void PrivateUpSyMaterial(HttpContext context)
        {
            string jsonText = DTRequest.GetPostParam("MaterialData");
            MaterialResponse model = JsonConvert.DeserializeObject<MaterialResponse>(jsonText);
            MaterialBLL bll = new MaterialBLL();
            RetResult result = bll.UploadDIPrivate(model);
            context.Response.Write(JsonConvert.SerializeObject(result));

        }
        #endregion

        #region 同步PI以及文件

        private void upPIFile(HttpContext context)
        {
            RetResults ret = new RetResults();
            string PIData = DTRequest.GetPostParam("PIData");
            string filePath = _path + "InterFaceUpFile\\PrivateService";
            HttpFileCollection files = HttpContext.Current.Request.Files;
            if (files.Count > 0)
            {
                var model = JsonConvert.DeserializeObject<PrivatePIRequest>(PIData);
                if (model != null)
                {
                    BLL.ActinvEwmBLL bll = new ActinvEwmBLL();
                    ret = bll.UploadPIPrivate(model, files, filePath);
                    context.Response.Write(new JavaScriptSerializer().Serialize(ret));
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "PI数据有误，无法同步" }));
                }

            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "找不到码文件，无法同步" }));
            }
        }
        #endregion
        #endregion
        #endregion




        private void GetExpired(HttpContext context)
        {
            
            string EnterpriseID = DTRequest.GetQueryString("EnterpriseID");
            if (string.IsNullOrEmpty(EnterpriseID))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不能为空" }));
                return;
            }
            EnterpriseInfoBLL bll = new EnterpriseInfoBLL();
            string result = bll.GetEnterpriseIsExpired(long.Parse(EnterpriseID));
            if (result != "")
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = 1, Msg = result }));
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "授权已到期" }));
            }

        }

        private void GetUDIDI(HttpContext context)
        {
            string UDIDI = string.Empty;
            string MainCode = DTRequest.GetQueryString("MainCode");
            string PackLevel = DTRequest.GetQueryString("PackLevel");
            string CategoryCode = DTRequest.GetQueryString("CategoryCode");
            if (string.IsNullOrEmpty(MainCode))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "主码不能为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(PackLevel))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "包装级别不能为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(CategoryCode))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "品类编码不能为空！" }));
                return;
            }
            //MainCode.包装规格级别+品类编码+校验码
            UDITool udi=new UDITool();
            string udiCode = udi.GenDI(MainCode, PackLevel, CategoryCode);
            if (udiCode == "error")
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "异常字符，无法生成DI" }));
                return;
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = 1, Msg = udiCode }));
            }

        }

        private void GetUDIData(HttpContext context)
        {
            AutoUDITask t = new AutoUDITask();
        }

        

        /// <summary>
        /// 查看接口是否可用
        /// </summary>
        /// <param name="context"></param>
        public void PingTrace(HttpContext context)
        {
            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = 1, Msg = "连接成功！" }));
            return;
        }
        /// <summary>
        /// 获取上传文件记录
        /// </summary>
        /// <param name="context"></param>
        public void getUpLst(HttpContext context)
        {
            ActinvEwmBLL actinvEwmbll = new ActinvEwmBLL();
            List<ActiveEwmRecord> lst = actinvEwmbll.GetActiveEwmList();
            context.Response.Write(new JavaScriptSerializer().Serialize(lst));
            return;
        }

        /// <summary>
        /// 上传文件 二进制流的形式
        /// </summary>
        /// <param name="context"></param>
        private void UpFile(HttpContext context)
        {
            try
            {
                string filePath = _path + "\\InterFaceUpFile";
                var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);
                string LoginName = DTRequest.GetQueryString("LoginName");
                string PassWord = DTRequest.GetQueryString("PassWord");
                string ProducBatch = DTRequest.GetQueryString("ProducBatch");
                string MaterialID = DTRequest.GetQueryString("mateiralId");
                byte[] byteData = Utils.ConvertStreamToByteBuffer(context.Request.InputStream); //获取文件流
                var oldfilename = DTRequest.GetQueryString("codefile"); //二维码文件名
                if (string.IsNullOrEmpty(LoginName))
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不可为空！" }));
                    return;
                }
                if (string.IsNullOrEmpty(PassWord))
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                    return;
                }
                if (enterpriseId == 0)
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业编码不能为零！" }));
                    return;
                }
                //if (string.IsNullOrEmpty(ProducBatch))
                //{
                //    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "生产批次号不可为空！" }));
                //    return;
                //}
                if (null == byteData)
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "请选择要上传的文件！" }));
                    return;
                }
                if (byteData.Length == 0)
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "请选择要上传的文件！" }));
                    return;
                }
                ActinvEwmBLL activebll = new ActinvEwmBLL();
                var m = activebll.IsActiveRecPack(oldfilename);//判断文件是否上传
                if (null != m)
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "该文件已上传！" }));
                    return;
                }
                Enterprise_UserBLL bll = new Enterprise_UserBLL();
                View_WinCe_EnterpriseInfoUser user = bll.GetEntityByLoginName(LoginName);
                if (null != user)
                {
                    if (user.LoginPassWord.Equals(PassWord))
                    {
                        if (user.Enterprise_Info_ID == enterpriseId)
                        {
                            ActinvEwmBLL actinvEwmbll = new ActinvEwmBLL();
                            //ActiveEwmRecord activeEwmRecord = actinvEwmbll.getModelByProduceBathNo(ProducBatch, (long)user.Enterprise_Info_ID);
                            //if (null == activeEwmRecord)
                            //{
                            filePath = filePath + "\\" + enterpriseId.ToString();
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                            filePath = filePath + "\\" + DateTime.Now.ToString("yyyyMMdd");
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                            string fileName = Guid.NewGuid().ToString("N");
                            ActiveEwmRecord ewmRecord = new ActiveEwmRecord();
                            ewmRecord.PackName = oldfilename;
                            ewmRecord.UpUserID = user.Enterprise_User_ID;
                            ewmRecord.AddUserName = user.UserName;
                            ewmRecord.EnterpriseId = enterpriseId;
                            ewmRecord.UploadDate = DateTime.Now;
                            //ewmRecord.RecPath = filePath + "\\" + fileName + ".zip";
                            ewmRecord.RecPath = filePath + "\\" + fileName + ".txt";
                            if (File.Exists(ewmRecord.RecPath))
                            {
                                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "文件上传失败，文件不存在！" }));
                                return;
                            }
                            ewmRecord.URL = "/InterFaceUpFile/" + user.Enterprise_Info_ID.ToString() + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + fileName + ".txt";
                            ewmRecord.Status = (int)EnumFile.RecEwm.已接收; //为防止多合一服务激活操作，先改为已激活，后续改为已接受
                            ewmRecord.AddDate = DateTime.Now;
                            ewmRecord.StrAddTime = DateTime.Now.ToString("yyyy-MM-dd");
                            ewmRecord.OperationType = (int)EnumFile.OperationType.流水线;
                            ewmRecord.UpDeviceMark = "";
                            ewmRecord.BatchName = ProducBatch;
                            ewmRecord.Material_ID = string.IsNullOrEmpty(MaterialID) ? 0 : long.Parse(MaterialID);
                            ewmRecord.Remark = "";
                            ewmRecord.ProducBatch = ProducBatch;
                            Utils.SaveFile(byteData, ewmRecord.RecPath);
                            RetResults result = actinvEwmbll.UpActiveRecPack(ewmRecord);
                            if (result.code == 0)
                            {
                                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = 0, Msg = "文件上传成功！" }));
                                return;
                            }
                            else
                            {
                                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "文件上传失败！" }));
                                return;
                            }
                            //}
                            //else
                            //{
                            //    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "存在相同生产批次号的记录！" }));
                            //    return;
                            //}
                        }
                        else
                        {
                            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不匹配！" }));
                            return;
                        }
                    }
                    else
                    {
                        context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不正确！" }));
                        return;
                    }
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不存在！" }));
                    return;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }

        }
        #region 下载产品码接口OK===========================
        private void Down_MaterialBoxCode(HttpContext context)
        {
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            if (string.IsNullOrEmpty(LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                return;
            }
            if (enterpriseId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业编码不能为零！" }));
                return;
            }
            var stime = DTRequest.GetQueryString("BeginTime");
            if (string.IsNullOrEmpty(stime))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "开始时间不能为空！" }));
                return;
            }
            var etime = DTRequest.GetQueryString("EndTime");
            if (string.IsNullOrEmpty(etime))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "结束时间不能为空！" }));
                return;
            }
            MaterialBLL mbll = new MaterialBLL();
            Enterprise_UserBLL bll = new Enterprise_UserBLL();
            View_WinCe_EnterpriseInfoUser user = bll.GetEntityByLoginName(LoginName);
            if (null != user)
            {
                if (user.LoginPassWord.Equals(PassWord))
                {
                    if (user.Enterprise_Info_ID == enterpriseId)
                    {
                        RequestCodeMaBLL codebll = new RequestCodeMaBLL();
                        BaseResultList result = codebll.GetInterFaceMaterialCode("box", _webURL, stime, etime, enterpriseId);
                        if (result.totalCounts == 0)
                        {
                            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                            return;
                        }
                        else
                        {
							JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
							jss.MaxJsonLength = Int32.MaxValue;
							string str = jss.Serialize(result.ObjList);
							context.Response.Write(str);
                        }
                    }
                    else
                    {
                        context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不匹配！" }));
                        return;
                    }
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不正确！" }));
                    return;
                }
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不存在！" }));
                return;
            }
        }
        #endregion
        #region 下载产品码接口OK===========================

        private void Down_MaterialCode(HttpContext context)
        {
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            if (string.IsNullOrEmpty(LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                return;
            }
            if (enterpriseId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业编码不能为零！" }));
                return;
            }
            var stime = DTRequest.GetQueryString("BeginTime");
            if (string.IsNullOrEmpty(stime))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "开始时间不能为空！" }));
                return;
            }
            var etime = DTRequest.GetQueryString("EndTime");
            if (string.IsNullOrEmpty(etime))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "结束时间不能为空！" }));
                return;
            }
            //MaterialBLL mbll = new MaterialBLL();
            Enterprise_UserBLL bll = new Enterprise_UserBLL();
            View_WinCe_EnterpriseInfoUser user = bll.GetEntityByLoginName(LoginName);
            if (null != user)
            {
                if (user.LoginPassWord.Equals(PassWord))
                {
                    if (user.Enterprise_Info_ID == enterpriseId)
                    {
                        RequestCodeMaBLL codebll = new RequestCodeMaBLL();
                        BaseResultList result = codebll.GetInterFaceMaterialCode("single", _webURL, stime, etime, enterpriseId);
                        if (result.totalCounts == 0)
                        {
                            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                            return;
                        }
                        else
                        {
							JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
							jss.MaxJsonLength = Int32.MaxValue;
							string str = jss.Serialize(result.ObjList);
							context.Response.Write(str);
                        }
                    }
                    else
                    {
                        context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不匹配！" }));
                        return;
                    }
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不正确！" }));
                    return;
                }
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不存在！" }));
                return;
            }
        }
        #endregion

        #region 下载产品信息接口OK===========================

        private void Down_Material(HttpContext context)
        {
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 2);
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            if (string.IsNullOrEmpty(LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                return;
            }
            if (enterpriseId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业编码不能为零！" }));
                return;
            }
            MaterialBLL mbll = new MaterialBLL();
            Enterprise_UserBLL bll = new Enterprise_UserBLL();
            View_WinCe_EnterpriseInfoUser user = bll.GetEntityByLoginName(LoginName);
            if (null != user)
            {
                if (user.LoginPassWord.Equals(PassWord))
                {
                    if (user.Enterprise_Info_ID == enterpriseId)
                    {
                        BaseResultList result = mbll.GetMaterialListNew(enterpriseId);
                        if (result.totalCounts == 0)
                        {
                            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                            return;
                        }
                        else
                        {
							JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
							jss.MaxJsonLength = Int32.MaxValue;
							string str = jss.Serialize(result.ObjList);
							context.Response.Write(str);
                        }
                    }
                    else
                    {
                        context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不匹配！" }));
                        return;
                    }
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不正确！" }));
                    return;
                }
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不存在！" }));
                return;
            }

        }
        #endregion

        #region 登录接口接口OK===========================

        public class UserInfo
        {
            public long EnterPriseID { get; set; }
            public string LoginName { get; set; }
            public string UserName { get; set; }
            public string EnterpriseName { get; set; }
            public string MainCode { get; set; }
            public long UserID { get; set; }
            public string LicenseEndDate { get; set; }
            /// <summary>
            /// 1:简单版；2：高级版；3：标准版
            /// </summary>
            public int? IsSimple { get; set; }
            /// <summary>
            /// 统一社会代码
            /// </summary>
            public string BusinessLicence { get; set; }
            /// <summary>
            /// 是否子账号1：主账号；2子账号
            /// </summary>
            public string IsSubUser { get; set; }
            /// <summary>
            /// DI列表
            /// </summary>
            public List<string> DI { get; set; }

        }

        public class UserInfoEx
        {
            public long EnterPriseID { get; set; }
            public string LoginName { get; set; }
            public string Pwd { get; set; }
            public string EnterpriseName { get; set; }
            public string MainCode { get; set; }
            public long UserID { get; set; }
            public string UserName { get; set; }
            public string LicenseEndDate { get; set; }
        }

        private void UserLogin(HttpContext context)
        {
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            if (string.IsNullOrEmpty(LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                return;
            }
            Enterprise_UserBLL bll = new Enterprise_UserBLL();
            BaseResultModel resultM = new BaseResultModel();
            resultM = bll.LoginWinCe(LoginName, PassWord);
            if (resultM.code == "1")
            {
                View_WinCe_EnterpriseInfoUser user = bll.GetEntityByLoginName(LoginName);
                if (null != user)
                {
                    //if (user.LoginPassWord.Equals(PassWord))
                    //{
                    SysEnterpriseManageBLL sembll = new SysEnterpriseManageBLL();
                    Enterprise_License model = sembll.GetEnInfoLicense((long)user.Enterprise_Info_ID);
                    EnterpriseShopLink enKHD = new EnterpriseShopLink();
                    UserInfo userInfo = new UserInfo();
                    userInfo.EnterPriseID = (long)user.Enterprise_Info_ID;
                    userInfo.LoginName = user.LoginName;
                    userInfo.UserName = user.UserName;
                    userInfo.EnterpriseName = user.EnterpriseName;
                    userInfo.MainCode = user.MainCode;
                    userInfo.UserID = user.Enterprise_User_ID;
                    userInfo.BusinessLicence = user.BusinessLicence;
                    userInfo.LicenseEndDate = model != null ? model.LicenseEndDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    enKHD = new SysEnterpriseManageBLL().GetEnKhd(userInfo.EnterPriseID);
                    if (enKHD != null)
                    {
                        userInfo.IsSimple = enKHD.IsSimple;
                    }
                    else
                    {
                        userInfo.IsSimple = 0;
                    }
                    userInfo.DI = sembll.GetSubUserDI((long)user.Enterprise_User_ID);
                    if (userInfo.DI.Count() > 0)
                    {
                        userInfo.IsSubUser = "2";
                    }
                    else
                    {
                        userInfo.IsSubUser = "1";
                    }
                    context.Response.Write(new JavaScriptSerializer().Serialize(userInfo));
                    //}
                    //else
                    //{
                    //    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不正确！" }));
                    //    return;
                    //}
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不存在！" }));
                    return;
                }
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = resultM.Msg }));
                return;
            }
        }

        private void UserLoginEx(HttpContext context)
        {
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            if (string.IsNullOrEmpty(LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                return;
            }
            Enterprise_UserBLL bll = new Enterprise_UserBLL();
            BaseResultModel resultM = new BaseResultModel();
            resultM = bll.LoginWinCeEx(LoginName, PassWord);
            if (resultM.code == "1")
            {
                View_WinCe_EnterpriseInfoUser user = bll.GetEntityByLoginNameEx(LoginName);
                if (null != user)
                {
                    SysEnterpriseManageBLL sembll = new SysEnterpriseManageBLL();
                    Enterprise_License model = sembll.GetEnInfoLicense((long)user.Enterprise_Info_ID);

                    UserInfoEx userInfo = new UserInfoEx();
                    userInfo.EnterPriseID = (long)user.Enterprise_Info_ID;
                    userInfo.LoginName = user.LoginName;
                    userInfo.Pwd = user.LoginPassWord;
                    userInfo.EnterpriseName = user.EnterpriseName;
                    userInfo.MainCode = user.MainCode;
                    userInfo.UserID = user.Enterprise_User_ID;
                    userInfo.UserName = user.UserName;
                    userInfo.LicenseEndDate = model != null ? model.LicenseEndDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    context.Response.Write(new JavaScriptSerializer().Serialize(userInfo));
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不存在！" }));
                    return;
                }
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = resultM.Msg }));
                return;
            }
        }
        #endregion

        #region IDCode账号登录有效期验证===================

        public void IDCodeUserLogin(HttpContext context)
        {
            string EnteroriseName = DTRequest.GetQueryString("EnteroriseName");
            string MainCode = DTRequest.GetQueryString("MainCode");
            var accountType = DTRequest.GetQueryIntValue("AccountType", 1);
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);

            if (string.IsNullOrEmpty(EnteroriseName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业名称不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(MainCode))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业主码不可为空！" }));
                return;
            }

            Enterprise_UserBLL bll = new Enterprise_UserBLL();
            int result = bll.GetEwmSysLoginInfo(enterpriseId, EnteroriseName, MainCode, accountType);

            string msg = "";
            switch (result)
            {
                case -1: msg = "登录名不存在！"; break;
                case 0: msg = "系统使用已超期"; break;
                default: msg = "成功"; break;
            }
            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result, Msg = msg }));
        }

        #endregion

        #region 企业注册接口（企业已通过Idcode验证）
        private void AddEnterpriseIdcodeHas(HttpContext context)
        {
            string enterpeiseCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseCode"].Trim();
            EnterpriseInfoRequest enInfo = new EnterpriseInfoRequest();
            enInfo.Address = DTRequest.GetQueryString("Address");
            enInfo.Dictionary_AddressQu_ID = DTRequest.GetQueryIntValue("Dictionary_AddressQu_ID", 0);
            enInfo.Dictionary_AddressShi_ID = DTRequest.GetQueryIntValue("Dictionary_AddressShi_ID", 0);
            enInfo.Dictionary_AddressSheng_ID = DTRequest.GetQueryIntValue("Dictionary_AddressSheng_ID", 0);
            enInfo.Trade_ID = DTRequest.GetQueryIntValue("Trade_ID", 0);
            enInfo.Etrade_ID = DTRequest.GetQueryIntValue("Etrade_ID", 0);
            enInfo.EnterpriseName = DTRequest.GetQueryString("EnterpriseName");
            //enInfo.MainCode = DTRequest.GetQueryString("MainCode");
            enInfo.Dictionary_UnitType_ID = DTRequest.GetQueryString("Dictionary_UnitType_ID");
            enInfo.LinkMan = DTRequest.GetQueryString("LinkMan");
            enInfo.LinkPhone = DTRequest.GetQueryString("LinkPhone");
            enInfo.Email = DTRequest.GetQueryString("Email");
            enInfo.LoginName = DTRequest.GetQueryString("LoginName");
            enInfo.LoginPassWord = DTRequest.GetQueryString("LoginPassWord");
            enInfo.BusinessLicence = DTRequest.GetQueryString("BusinessLicence");
            ServiceJK.WebService1SoapClient cl = new ServiceJK.WebService1SoapClient();
            string enMainCode = cl.GetEnterpriseMainCode(enterpeiseCode);
            if (string.IsNullOrEmpty(enInfo.LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "用户名不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(enInfo.LoginPassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录密码不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(enInfo.EnterpriseName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业名称不可为空！" }));
                return;
            }
            //if (string.IsNullOrEmpty(enInfo.MainCode))
            //{
            //    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业主码不可为空！" }));
            //    return;
            //}
            if (string.IsNullOrEmpty(enInfo.BusinessLicence))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "统一社会信用代码不可为空！" }));
                return;
            }
            Enterprise_License licenseModel = null;
            RegistBLL bll = new RegistBLL();
            int result = bll.AddEnterpriseIdcodeHas(enInfo, enMainCode, out licenseModel);
            string msg = "";
            switch (result)
            {
                case -1: msg = "您的企业已经在本平台注册!"; break;
                case -2: msg = "用户名已被注册！"; break;
                case -3: msg = "注册出现异常！"; break;
                default: msg = "成功"; break;
            }
            var retObj = new RetEnterpriseResult { code = result, Msg = msg };
            EnterpriseShopLink enKHD = new EnterpriseShopLink();
            if (result == 1 && licenseModel.LicenseID > 0)
            {
                retObj.EnterpriseId = (long)licenseModel.EnterpriseID;
                retObj.AdminId = (long)licenseModel.AdminID;
                retObj.LicenseEndDate = licenseModel.LicenseEndDate.Value.ToString("yyyy-MM-dd");
                enKHD = new SysEnterpriseManageBLL().GetEnKhd((long)licenseModel.EnterpriseID);
                if (enKHD != null)
                {
                    retObj.IsSimple = enKHD.IsSimple;
                }
            }
            if (result == -1)
            {
                RetEnterpriseResult re = bll.GetFhInfo(enInfo.MainCode);
                retObj.EnterpriseId = re.EnterpriseId;
                retObj.AdminId = re.AdminId;
                retObj.LicenseEndDate = re.LicenseEndDate;
                enKHD = new SysEnterpriseManageBLL().GetEnKhd(re.EnterpriseId);
                if (enKHD != null)
                {
                    retObj.IsSimple = enKHD.IsSimple;
                }
            }
            if (result == -2)
            {
                RetEnterpriseResult re = bll.GetFhInfo(enInfo.MainCode);
                retObj.EnterpriseId = re.EnterpriseId;
                retObj.AdminId = re.AdminId;
                retObj.LicenseEndDate = re.LicenseEndDate;
                enKHD = new SysEnterpriseManageBLL().GetEnKhd(re.EnterpriseId);
                if (enKHD != null)
                {
                    retObj.IsSimple = enKHD.IsSimple;
                }
                else
                {
                    retObj.IsSimple = 0;
                }
            }
            context.Response.Write(new JavaScriptSerializer().Serialize(retObj));
        }


        #endregion

        #region 获取标签模板
        private void GetLabelTem(HttpContext context)
        {
            SysLabelTemBLL bll = new SysLabelTemBLL();
            //List<LabelTem> resultM = bll.GetLabelTem();
            context.Response.Write(new JavaScriptSerializer().Serialize(bll.GetLabelTem()));
        }
        #endregion

        #region 20200811获取令牌国药监令牌所有客户端共用
        private void GetGYJToken(HttpContext context)
        {
            try
            {
                string mainCode = DTRequest.GetQueryString("mainCode");
                string appId = DTRequest.GetQueryString("appId");
                string appSecret = DTRequest.GetQueryString("appSecret");
                if (string.IsNullOrEmpty(mainCode))
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业主码不可为空！" }));
                    return;
                }
                if (string.IsNullOrEmpty(appId))
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "appId不可为空！" }));
                    return;
                }
                if (string.IsNullOrEmpty(appSecret))
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "appSecret不可为空！" }));
                    return;
                }
                EnterpriseInfoBLL bll = new EnterpriseInfoBLL();
                TokenInfo result = bll.GetGYJToken(mainCode, appId, appSecret);
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "接口访问失败" }));
                return;
            }
        }
        #endregion

        #region 20200824产品DI当天是第几次生成码
        private void GetMaterialDICount(HttpContext context)
        {
            try
            {
                string materialDI = DTRequest.GetQueryString("materialDI");
                if (string.IsNullOrEmpty(materialDI))
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品DI不可为空！" }));
                    return;
                }
                MaterialDIBLL bll = new MaterialDIBLL();
                int result = bll.GetMaterialDICount(materialDI);

                string msg = "";
                switch (result)
                {
                    case -1: msg = "出现异常！"; break;
                    case 0: msg = "没有查到产品DI信息，请先确认是否已上传！"; break;
                    default: msg = "成功"; break;
                }
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result, Msg = msg }));
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "接口访问失败" }));
                return;
            }
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void WriteLog(string msg)
        {
            string errlogpath = _path + "\\InterFaceLog";
            if (!Directory.Exists(errlogpath))
            {
                Directory.CreateDirectory(errlogpath);
            }
            errlogpath = errlogpath + "\\" + DateTime.Now.ToString("yyyyMM");
            if (!Directory.Exists(errlogpath))
            {
                Directory.CreateDirectory(errlogpath);
            }

            using (StreamWriter sw = new StreamWriter(errlogpath + "\\" + DateTime.Now.ToString("dd") + "Param.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "------" + HttpUtility.UrlDecode(msg));
            }
        }

        public void UDILogin(HttpContext context)
        {
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            try
            {
                LinqModel.OrganUnit O = BaseDataDAL.GetUnitInfo(LoginName, PassWord);
                context.Response.Write(new JavaScriptSerializer().Serialize(O));
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "接口访问失败" }));
                return;
            }
        }
        #region 20210422 DI信息接口
        /// <summary>
        /// 客户端上传产品/DI信息到追溯平台
        /// </summary>
        /// <param name="context"></param>
        public void UpSyMaterial(HttpContext context)
        {
            string mainCode = DTRequest.GetQueryString("MainCode");//主码
            string materialName = DTRequest.GetQueryString("MaterialName");//产品名称
            long enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);//企业编号
            int bzSpaceType = DTRequest.GetQueryIntValue("BZSpecType", 1);//包装规格
            string BZSpecName = DTRequest.GetQueryString("BZSpecName");//包装名称
            string categoryID = DTRequest.GetQueryString("CategoryID");//品类编号
            string MaterialUDIDI = DTRequest.GetQueryString("MaterialUDIDI");//UDI-DI完整编码
            string GS1DI = DTRequest.GetQueryString("GS1DI");//GS1-DI完整编码
            string CPGG = DTRequest.GetQueryString("CPGG");//产品规格
            string SpecLevel = DTRequest.GetQueryString("SpecLevel");//包装级别
            int SpecNum = DTRequest.GetQueryInt("SpecNum");//包装数量
            string HisCode = DTRequest.GetQueryString("HisCode");//医用耗材编码
            if (string.IsNullOrEmpty(materialName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品名称不可为空！" }));
                return;
            }
            //if (string.IsNullOrEmpty(mainCode))
            //{
            //    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业主码不可为空！" }));
            //    return;
            //}
            if (enterpriseId <= 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业编号不可为空！" }));
                return;
            }
            if (bzSpaceType < 0 || bzSpaceType > 9)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "包装规格不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(BZSpecName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "包装规格名称不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(categoryID))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "品类编码不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(MaterialUDIDI) &&string.IsNullOrEmpty(GS1DI))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "UDI-DI完整编码不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(CPGG))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品规格不可为空！" }));
                return;
            }
            Material material = new Material();
            material.MaterialName = materialName;
            material.Enterprise_Info_ID = enterpriseId;
            material.BZSpecType = bzSpaceType;
            material.adddate = DateTime.Now;
            material.type = 0;
            material.MaterialFullName = materialName;
            material.Status = (int)Common.EnumFile.Status.used;
            Category category = new Category();
            category.AddTime = DateTime.Now;
            category.CategoryCode = categoryID;
            category.Enterprise_Info_ID = enterpriseId;
            category.Status = (int)Common.EnumFile.Status.used;
            category.Material_Code = material.Material_Code;
            category.MaterialName = material.MaterialName;
            MaterialDI modelDI = new MaterialDI();
            modelDI.adddate = DateTime.Now;
            modelDI.adduser = material.adduser;
            modelDI.EnterpriseID = enterpriseId;
            modelDI.MaterialName = material.MaterialName;
            modelDI.MaterialUDIDI = MaterialUDIDI;
            modelDI.Specifications = bzSpaceType.ToString();
            modelDI.SpecificationName = BZSpecName;
            modelDI.Status = (int)Common.EnumFile.Status.used;
            modelDI.MaterialXH = CPGG;
            modelDI.CategoryCode = categoryID;
            modelDI.SpecLevel = SpecLevel;
            modelDI.SpecNum = SpecNum;
            modelDI.HisCode = HisCode;
            modelDI.GSIDI = GS1DI;
            modelDI.ISUpload = 0;//0表示未上传到发码机构
            modelDI.createtype = 1;//0、官网后台 1、接口：单次申请
            MaterialBLL bll = new MaterialBLL();
            RetResult result = bll.AddMaterial(material, category, modelDI);
            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result.Code, Msg = result.Msg }));
        }
        /// <summary>
        /// 客户端获取DI信息
        /// </summary>
        /// <param name="context"></param>
        private void SynchroUDIDI(HttpContext context)
        {
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);
            string synchroDate = DTRequest.GetQueryString("SynchroDate");
            if (enterpriseId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不能为零！" }));
                return;
            }
            MaterialBLL bll = new MaterialBLL();
            BaseResultList result = bll.GetMaterialDI(enterpriseId, synchroDate);
            if (result.totalCounts == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                return;
            }
            else
            {
				JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
				jss.MaxJsonLength = Int32.MaxValue;
				string str = jss.Serialize(result.ObjList);
				context.Response.Write(str);
            }
        }

        /// <summary>
        /// 根据企业名称同步DI信息
        /// </summary>
        /// <param name="context"></param>
        private void SynchroUDIByEnterprise(HttpContext context)
        {
            var enterpriseName = DTRequest.GetQueryString("EnterpriseName");
            string synchroDate = DTRequest.GetQueryString("SynchroDate");
            string page = DTRequest.GetQueryString("page");
            if (enterpriseName == "")
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业名称不能为空！" }));
                return;
            }
            UDIMaterialBLL bll = new UDIMaterialBLL();
            BaseResultModel result = bll.GetMaterialDIByEnterprise(enterpriseName, synchroDate,page);
            if (result.ObjModel==null)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                return;
            }
            else
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
                jss.MaxJsonLength = Int32.MaxValue;
                string str = jss.Serialize(result.ObjModel);
                context.Response.Write(str);
            }
        }

        /// <summary>
        /// 更新产品DI信息
        /// GS1码
        /// 商品编码
        /// </summary>
        /// <param name="context"></param>
        private void UpdateDI(HttpContext context)
        {
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);
            string DICode = DTRequest.GetQueryString("DICode");//品类编码
            string GS1Code = DTRequest.GetQueryString("GS1Code");//GS1编码
            string SPCode = DTRequest.GetQueryString("SPCode");//商品编码
            string SpecLevel = DTRequest.GetQueryString("SpecLevel");//包装级别
            int SpecNum = DTRequest.GetQueryIntValue("SpecNum");//包装数量
            string HisCode = DTRequest.GetQueryString("HisCode");//医用耗材编码
            string CPGG = DTRequest.GetQueryString("CPGG");//产品规格
            if (enterpriseId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不能为零！" }));
                return;
            }
            if (string.IsNullOrEmpty(DICode))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "DI编码不能为空！" }));
                return;
            }
            MaterialDIBLL bll = new MaterialDIBLL();
            BaseResultModel res = bll.UpdateDI(enterpriseId, DICode, GS1Code, SPCode, SpecLevel, SpecNum, HisCode, CPGG);
            if (res.code == "0")
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = 0, Msg = "修改成功！" }));
                return;
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "修改失败！" }));
                return;
            }

        }
        #endregion

        #region 20210422 PI信息接口
        public void UpSyMaterialPI(HttpContext context)
        {
            string filePath = _path + "\\CodeNumFile";
            long enterpriseId = Convert.ToInt64(context.Request.QueryString["EnterpriseID"]);//企业ID
            string youxiaoqi = context.Request.QueryString["YouXiaoDate"];//有效期
            string shixiaoqi = context.Request.QueryString["ShiXiaoDate"];//失效期
            string materialName = context.Request.QueryString["MaterialName"];//产品名称
            string fixedCode = context.Request.QueryString["FixedCode"];//码前缀（DI）
            int totalCount = Convert.ToInt32(context.Request.QueryString["TotalCount"]);//生成数量
            string shengChanPH = context.Request.QueryString["ShengChanPH"];//码前缀（DI）
            int codingClientType = Convert.ToInt32(context.Request.QueryString["CodingClientType"]);//1：MA码，2：GS1码
            string category_code = context.Request.QueryString["category_code"];//品类编码
            string specification = context.Request.QueryString["specification"];//包装规格[仅限下列定值：0 - 9)
            //string code_list_str = context.Request.QueryString["code_list_str"];//序列号列表[“1,2,3”)
            string start_date = context.Request.QueryString["start_date"];//生产日期
            string d_batch_number = context.Request.QueryString["d_batch_number"];//灭菌批号
            string product_model = context.Request.QueryString["product_model"];//生产批号
            byte[] byteData = Utils.ConvertStreamToByteBuffer(context.Request.InputStream); //获取文件流
            var oldfilename = DTRequest.GetQueryString("codeNumFile"); //二维码序列号文件名
            if (string.IsNullOrEmpty(materialName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品名称不可为空！" }));
                return;
            }
			if (string.IsNullOrEmpty(fixedCode))
			{
				context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "UDI-DI编码不可为空！" }));
				return;
			}
			else 
			{
				fixedCode = fixedCode.Contains("(01)") ? fixedCode.Replace("(01)", "") : fixedCode;
			}
            if (enterpriseId <= 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不可为空！" }));
                return;
            }
            if (totalCount <= 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "生成数量可为空！" }));
                return;
            }
            if (null == byteData)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "请上传序列号文件！" }));
                return;
            }
            if (byteData.Length == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "请上传序列号文件！" }));
                return;
            }
            filePath = filePath + "\\" + enterpriseId.ToString();
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = filePath + "\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fileName = Guid.NewGuid().ToString("N");
            RequestCode ObjRequestCode = new RequestCode();
            ObjRequestCode.Enterprise_Info_ID = enterpriseId;
            ObjRequestCode.adddate = DateTime.Now;
            //ObjRequestCode.IDCodeBatchNo = sub.batch_no;//服务执行上传MA时可更新
            ObjRequestCode.RequestDate = DateTime.Now;
            ObjRequestCode.ShengChanPH = shengChanPH;
            if (!string.IsNullOrEmpty(shengChanPH))
            {
                ObjRequestCode.ShengChanPH = shengChanPH;
            }
            if (!string.IsNullOrEmpty(shixiaoqi))
            {
                ObjRequestCode.ShiXiaoDate = shixiaoqi;
            }
            if (!string.IsNullOrEmpty(youxiaoqi))
            {
                ObjRequestCode.YouXiaoDate = youxiaoqi;
            }
            ObjRequestCode.TotalNum = totalCount;
            ObjRequestCode.FixedCode = fixedCode;
            int status = Convert.ToInt32(EnumFile.RequestCodeStatus.ApprovedWaitingToBeGenerated);
            ObjRequestCode.Status = status;
            ObjRequestCode.IsRead = (int)Common.EnumFile.IsRead.noRead;
            ObjRequestCode.IsLocal = (int)Common.EnumFile.Islocal.cloud;
            ObjRequestCode.StartNum = 1;
            ObjRequestCode.EndNum = ObjRequestCode.TotalNum;
            ObjRequestCode.Type = (int)Common.EnumFile.GenCodeType.single;
            ObjRequestCode.CodingClientType = codingClientType;//CodingClientType为1代表是MA码  2代表GS1码
            ObjRequestCode.category_code = category_code;
            ObjRequestCode.BZSpecType = Convert.ToInt32(specification);
            //ObjRequestCode.code_list_str = code_list_str;
            ObjRequestCode.startdate = start_date;
            ObjRequestCode.dbatchnumber = d_batch_number;
            ObjRequestCode.product_model = product_model;
            ObjRequestCode.ISUpload = (int)Common.EnumFile.RequestISUpload.NotUploaded;
            ObjRequestCode.createtype = (int)Common.EnumFile.CreateType.UpJieKou;
            //序列号文件路径
            ObjRequestCode.CodeNumPath = filePath + "\\" + fileName + ".txt";
            if (File.Exists(ObjRequestCode.CodeNumPath))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "文件上传失败，文件不存在！" }));
                return;
            }
            ObjRequestCode.CodeNumURL = "/CodeNumFile/" + enterpriseId.ToString() + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + fileName + ".txt";
            RequestCodeSetting setModel = new RequestCodeSetting();
            setModel.EnterpriseId = enterpriseId;
            setModel.Count = totalCount;
            setModel.beginCode = 1;
            setModel.endCode = totalCount;
            setModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.fwzsCode;
            setModel.SetDate = DateTime.Now;
            setModel.BatchType = 1;
            if (!string.IsNullOrEmpty(shengChanPH))
            {
                setModel.ShengChanPH = shengChanPH;
            }
            Utils.SaveFile(byteData, ObjRequestCode.CodeNumPath);
            RequestCodeBLL bll = new RequestCodeBLL();
            RetResult result = bll.AddPIInfo(ObjRequestCode, setModel, materialName);
            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result.Code, Msg = result.Msg, ID = result.id }));
        }

        private void SynchroUDIPI(HttpContext context)
        {
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);
            string synchroDate = DTRequest.GetQueryString("SynchroDate");
            if (enterpriseId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不能为零！" }));
                return;
            }
            RequestCodeBLL bll = new RequestCodeBLL();
            BaseResultList result = bll.GetRequestCodeList(enterpriseId, synchroDate);
            if (result.totalCounts == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                return;
            }
            else
            {
				JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
				jss.MaxJsonLength = Int32.MaxValue;
				string str = jss.Serialize(result.ObjList);
				context.Response.Write(str);
            }
        }

        private void SynchroUDIPICode(HttpContext context)
        {
            var enterpriseId = DTRequest.GetQueryIntValue("EnterpriseID", 0);
            string IDCodeBatchNo = DTRequest.GetQueryString("IDCodeBatchNo");
            if (enterpriseId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不能为零！" }));
                return;
            }
            if (string.IsNullOrEmpty(IDCodeBatchNo))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "批次编号不能为空！" }));
                return;
            }
            RequestCodeBLL bll = new RequestCodeBLL();
            List<string> PIList = bll.GetPICodeList(enterpriseId, IDCodeBatchNo);
            if (PIList.Count == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                return;
            }
            else
            {
				JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
				jss.MaxJsonLength = Int32.MaxValue;
				string str = jss.Serialize(PIList);
				context.Response.Write(str);
            }
        }
        #endregion
        /// <summary>
        /// 维护企业统一社会信用代码
        /// </summary>
        /// <param name="context"></param>
        private void EditEnterprise(HttpContext context)
        {
            long EnterpriseID = DTRequest.GetQueryIntValue("EnterpriseID", 0);
            string BusinessLicence = DTRequest.GetQueryString("BusinessLicence");
            if (EnterpriseID <= 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(BusinessLicence))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "统一社会信用代码不可为空！" }));
                return;
            }
            RegistBLL bll = new RegistBLL();
            RetResult result = bll.EditEnterprise(EnterpriseID, BusinessLicence);
            if (result.CmdError == CmdResultError.NONE)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = 0, Msg = "修改成功！" }));
                return;
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "修改失败！" }));
                return;
            }
        }

        #region 20210511经营企业接口
        //经营企业登录
        private void JYEnLogin(HttpContext context)
        {
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            if (string.IsNullOrEmpty(LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "登录名不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                return;
            }
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            BaseResultModel resultM = new BaseResultModel();
            resultM = bll.JYEnLogin(LoginName, PassWord);
            if (resultM.code == "1")
            {
                View_DealerUser dUserInfo = JsonDes.JsonDeserialize<View_DealerUser>(new JavaScriptSerializer().Serialize(resultM.ObjModel));
                JYEnInfo userInfo = new JYEnInfo();
                userInfo.DealerID = dUserInfo.Dealer_ID;
                userInfo.LoginName = dUserInfo.LoginName;
                userInfo.PassWord = dUserInfo.LoginPassWord;
                userInfo.DealerName = dUserInfo.DealerName;
                userInfo.DealerLevel = dUserInfo.DealerLevel == null ? 1 : dUserInfo.DealerLevel;
                userInfo.Status = dUserInfo.Status;
                userInfo.DICount = dUserInfo.DICount == null ? 0 : dUserInfo.DICount; ;
                userInfo.UserID = dUserInfo.Enterprise_User_ID;
                if (dUserInfo.Status == (int)Common.EnumFile.Status.delete)
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "该用户已被禁用请联系管理员！" }));
                    return;
                }
                else
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(userInfo));
                }
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = resultM.Msg }));
                return;
            }
        }

        //经营企业上传DI至追溯平台
        public void JYUpSyMaterial(HttpContext context)
        {
            //string mainCode = DTRequest.GetQueryString("MainCode");//主码
            string materialName = DTRequest.GetQueryString("MaterialName");//产品名称
            string categoryCode = DTRequest.GetQueryString("CategoryCode");//品类编码
            long dealerId = DTRequest.GetQueryIntValue("DealerID", 0);//企业编号
            int bzSpaceType = DTRequest.GetQueryIntValue("BZSpecType", 1);//包装规格
            string BZSpecName = DTRequest.GetQueryString("BZSpecName");//包装名称
            string categoryID = DTRequest.GetQueryString("CategoryID");//品类编号
            string MaterialUDIDI = DTRequest.GetQueryString("MaterialUDIDI");//UDI-DI完整编码
            string CPGG = DTRequest.GetQueryString("CPGG");//产品规格
            string SpecLevel = DTRequest.GetQueryString("SpecLevel");//包装级别
            int SpecNum = DTRequest.GetQueryInt("SpecNum");//包装数量
            string HisCode = DTRequest.GetQueryString("HisCode");//医用耗材编码
            long userId = DTRequest.GetQueryIntValue("UserID", 0);//企业编号
            string enName = DTRequest.GetQueryString("EnterpriseName");//企业名称
            string businessLicence = DTRequest.GetQueryString("BusinessLicence");//企业统一社会信用代码
            string GSIDI = DTRequest.GetQueryString("GSIDI");//GSIDI
            string spCode = DTRequest.GetQueryString("SPCode");//SPCode
            if (string.IsNullOrEmpty(materialName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品名称不可为空！" }));
                return;
            }
            //if (string.IsNullOrEmpty(mainCode))
            //{
            //    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业主码不可为空！" }));
            //    return;
            //}
            if (dealerId <= 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业编号不可为空！" }));
                return;
            }
            if (bzSpaceType < 0 || bzSpaceType > 9)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "包装规格不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(BZSpecName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "包装规格名称不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(categoryID))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "品类编码不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(MaterialUDIDI))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "UDI-DI完整编码不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(CPGG))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品规格不可为空！" }));
                return;
            }
            JYMaterial material = new JYMaterial();
            material.MaterialName = materialName;
            material.DealerID = dealerId;
            material.CategoryCode = categoryCode;
            material.adddate = DateTime.Now;
            material.adduser = userId;
            material.Status = (int)Common.EnumFile.Status.used;
            JYMaterialDI modelDI = new JYMaterialDI();
            modelDI.adddate = DateTime.Now;
            modelDI.adduser = userId;
            modelDI.DealerID = dealerId;
            modelDI.MaterialName = materialName;
            modelDI.MaterialUDIDI = MaterialUDIDI;
            modelDI.Specifications = bzSpaceType.ToString();
            modelDI.SpecificationName = BZSpecName;
            modelDI.Status = (int)Common.EnumFile.Status.used;
            modelDI.MaterialXH = CPGG;
            modelDI.CategoryCode = categoryID;
            modelDI.SpecLevel = bzSpaceType.ToString();
            modelDI.SpecNum = SpecNum;
            modelDI.HisCode = HisCode;
            modelDI.EnterpriseName = enName;
            modelDI.BusinessLicence = businessLicence;
            modelDI.GSIDI = GSIDI;
            modelDI.SPCode = spCode;
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            RetResult result = bll.AddJYMaterial(material, modelDI);
            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result.Code, Msg = result.Msg }));
        }

        //经营企业获取DI列表
        private void JYSynchroUDIDI(HttpContext context)
        {
            var dealerId = DTRequest.GetQueryIntValue("DealerID", 0);
            string materialName = DTRequest.GetQueryString("MaterialName");
            string categoryCode = DTRequest.GetQueryString("CategoryCode");
            if (dealerId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不能为零！" }));
                return;
            }
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            BaseResultList result = bll.GetJYMaterialDI(dealerId, materialName, categoryCode);
            if (result.totalCounts == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                return;
            }
            else
            {
				JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
				jss.MaxJsonLength = Int32.MaxValue;
				string str = jss.Serialize(result.ObjList);
				context.Response.Write(str);
            }
        }

        public void JYUpSyMaterialPI(HttpContext context)
        {
            string filePath = _path + "\\JYCodeFile";
            long dealerId = Convert.ToInt64(context.Request.QueryString["DealerID"]);//经营企业ID
            string youxiaoqi = context.Request.QueryString["YouXiaoDate"];//有效期
            string shixiaoqi = context.Request.QueryString["ShiXiaoDate"];//失效期
            string materialName = context.Request.QueryString["MaterialName"];//产品名称
            string materialSpec = context.Request.QueryString["MaterialSpec"];//产品规格
            string fixedCode = context.Request.QueryString["FixedCode"];//码前缀（DI）
            int totalCount = Convert.ToInt32(context.Request.QueryString["TotalCount"]);//生成数量
            string shengChanPH = context.Request.QueryString["ShengChanPH"];//码前缀（DI）
            int codingClientType = Convert.ToInt32(context.Request.QueryString["CodingClientType"]);//1：MA码，2：GS1码
            string category_code = context.Request.QueryString["category_code"];//品类编码
            string specification = context.Request.QueryString["specification"];//包装规格[仅限下列定值：0 - 9)
            string specificationName = context.Request.QueryString["specificationName"];//包装规格[仅限下列定值：0 - 9)
            string start_date = context.Request.QueryString["start_date"];//生产日期
            string d_batch_number = context.Request.QueryString["d_batch_number"];//灭菌批号
            string product_model = context.Request.QueryString["product_model"];//生产批号
            string XHLength = context.Request.QueryString["XHLength"];//流水号长度
            string SCType = context.Request.QueryString["SCType"];//流水号长度
            string serialnumber = DTRequest.GetQueryString("serialnumber");
            byte[] byteData = Utils.ConvertStreamToByteBuffer(context.Request.InputStream); //获取文件流
			string oldfilename = DTRequest.GetQueryString("codeNumFile");
			string timeFolder = DTRequest.GetQueryString("timeFolder");
			if (string.IsNullOrEmpty(materialName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品名称不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(fixedCode))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "UDI-DI编码不可为空！" }));
                return;
            }
            if (dealerId <= 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不可为空！" }));
                return;
            }
            if (totalCount <= 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "生成数量可为空！" }));
                return;
            }
            if (null == byteData)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "请上传序列号文件！" }));
                return;
            }
            if (byteData.Length == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "请上传序列号文件！" }));
                return;
            }
            filePath = filePath + "\\" + dealerId.ToString();
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
			filePath = filePath + "\\" + timeFolder;
			if (!Directory.Exists(filePath))
			{
				Directory.CreateDirectory(filePath);
			}
            //string fileName = Guid.NewGuid().ToString("N");
            JYRequestCode jyPIModel = new JYRequestCode();
            jyPIModel.DealerID = dealerId;
            jyPIModel.adddate = DateTime.Now;
            jyPIModel.RequestDate = DateTime.Now;
            jyPIModel.ShengChanPH = shengChanPH;
            if (!string.IsNullOrEmpty(shengChanPH))
            {
                jyPIModel.ShengChanPH = shengChanPH;
            }
            if (!string.IsNullOrEmpty(shixiaoqi))
            {
                jyPIModel.ShiXiaoDate = shixiaoqi;
            }
            if (!string.IsNullOrEmpty(youxiaoqi))
            {
                jyPIModel.YouXiaoDate = youxiaoqi;
            }
            jyPIModel.TotalNum = totalCount;
            jyPIModel.FixedCode = fixedCode;
            int status = Convert.ToInt32(EnumFile.RequestCodeStatus.GenerationIsComplete);
            jyPIModel.Status = status;
            jyPIModel.StartNum = 1;
            jyPIModel.EndNum = jyPIModel.TotalNum;
            jyPIModel.CodingClientType = codingClientType;//新加的字段
            jyPIModel.category_code = category_code;
            jyPIModel.BZSpecType = Convert.ToInt32(specification);
            jyPIModel.BZSpecName = specificationName;
            jyPIModel.startdate = start_date;
            jyPIModel.dbatchnumber = d_batch_number;
            jyPIModel.product_model = product_model;
            jyPIModel.JYMaterialName = materialName;
            jyPIModel.XHLength = XHLength;
            jyPIModel.SCType = SCType;
            jyPIModel.serialnumber = serialnumber;
            //序列号文件路径
			jyPIModel.CodeNumPath = filePath + "\\" + oldfilename;
            if (File.Exists(jyPIModel.CodeNumPath))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "文件上传失败，文件不存在！" }));
                return;
            }
			jyPIModel.CodeNumURL = "/JYCodeFile/" + dealerId.ToString() + "/" + timeFolder + "/" + oldfilename;
            Utils.SaveFile(byteData, jyPIModel.CodeNumPath);
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            RetResult result = bll.AddJYPIInfo(jyPIModel, materialName);
            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result.Code, Msg = result.Msg, ID = result.id }));
        }

        private void JYSynchroUDIPI(HttpContext context)
        {
            var dealerId = DTRequest.GetQueryIntValue("DealerID", 0);
            string starDate = DTRequest.GetQueryString("StarDate");
            string endDate = DTRequest.GetQueryString("EndDate");
            if (dealerId == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不能为零！" }));
                return;
            }
            SysJYEnterpriseBLL bll = new SysJYEnterpriseBLL();
            BaseResultList result = bll.GetJYPIList(dealerId, starDate, endDate);
            if (result.totalCounts == 0)
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "没有符合条件的数据！" }));
                return;
            }
            else
            {
				JavaScriptSerializer jss = new JavaScriptSerializer();//js反序列化对象
				jss.MaxJsonLength = Int32.MaxValue;
				string str = jss.Serialize(result.ObjList);
				context.Response.Write(str);
            }
        }

        public class JYEnInfo
        {
            public long DealerID { get; set; }
            public string LoginName { get; set; }
            public string PassWord { get; set; }
            public string DealerName { get; set; }
            public int? Status { get; set; }
            public int? DealerLevel { get; set; }
            public long? DICount { get; set; }
            public long? UserID { get; set; }
        }
        #endregion

        public void VersionInfo(HttpContext context)
        {
            string version = DTRequest.GetQueryString("VersionCode");
            int type=DTRequest.GetQueryIntValue("Type", 3);
            try
            {
                if (string.IsNullOrEmpty(version))
                {
                    context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "版本号不可为空！" }));
                    return;
                }
                VersionResult result = new VersionResult();
                LoginBLL bll = new LoginBLL();
                FWZSNCP_CodeVersion verModel = bll.GetVersion(type);
                string versionCodeJ = version.Replace(".", "");
                string versionCodeQ;
                if (verModel != null)
                {
                    versionCodeQ = verModel.MainVersion.ToString() + verModel.SubVersion.ToString() + verModel.StageVersion.ToString() + verModel.DateVersion.ToString();
                    if (Convert.ToInt32(versionCodeQ) > Convert.ToInt32(versionCodeJ))//有更新版本
                    {
                        result.Isupdate = 1;
                        result.VersionURL = verModel.VersionURL;
                        result.VersionCode = versionCodeQ;
                    }
                    else
                    {
                        result.Isupdate = 0;//无版本更新
                    }
                }
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "接口访问失败" }));
                return;
            }
        }

        #region UTCToken接口
        private void SetUtcToken(HttpContext context)
        {
            string LoginName = DTRequest.GetQueryString("LoginName");
            string PassWord = DTRequest.GetQueryString("PassWord");
            string token = DTRequest.GetQueryString("Token");
            string tokencode = DTRequest.GetQueryString("TokenCode");
            RetResult result = new RetResult();
            if (string.IsNullOrEmpty(LoginName))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "用户名！" }));
                return;
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "密码不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(token))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "Token不可为空！" }));
                return;
            }
            if (string.IsNullOrEmpty(tokencode))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "TokenCode不可为空！" }));
                return;
            }
            SysEnterpriseManageBLL bll = new SysEnterpriseManageBLL();
            result = bll.SetTokenForCilent(LoginName, PassWord, token, tokencode);
            context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result.Code, Msg = result.Msg}));
        }
        #endregion

        public class VersionResult
        {
            public int Isupdate { get; set; }
            public string VersionURL { get; set; }
            public string VersionCode { get; set; }
        }
    }
}