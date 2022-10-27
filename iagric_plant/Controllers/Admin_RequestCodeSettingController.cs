/********************************************************************************
** 描述：主要用于配置码信息控制器
*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using Common.Log;
using LinqModel;
using System.Collections.Generic;
using BLL;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
namespace iagric_plant.Controllers
{
    /// <summary>
    /// 主要用于配置码信息控制器
    /// </summary>
    public class Admin_RequestCodeSettingController : Controller
    {
        /// <summary>
        /// 获取配置项第一页的初始信息
        /// </summary>
        /// <param name="requestId">申请码标识</param>
        /// <returns>第一页初始信息Json数据</returns>
        public ActionResult One(string requestId)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.RequestCodeSettingBLL().GetFirstPageData(Convert.ToInt64(requestId));
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.One():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 拆分初始信息
        /// </summary>
        /// <param name="subId">配置表标识</param>
        /// <returns>拆分初始信息Json数据</returns>
        public ActionResult BatchPart(string subId)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.RequestCodeSettingBLL().BatchPartInit(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.One():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 拆分批次
        /// </summary>
        /// <param name="requestId">申请码标识</param>
        /// <param name="count">配置码数量</param>
        /// <param name="batchName">批次号</param>
        /// <returns>操作结果Json数据</returns>
        [HttpPost]
        public ActionResult Add(string requestId, string count, string batchName, int batchType, string startCode, string endCode)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestCodeSetting model = new RequestCodeSetting();
                model.Count =string.IsNullOrEmpty(count)==true?0: Convert.ToInt64(count);
                model.RequestID = Convert.ToInt64(requestId);
                model.BatchName = batchName;
                model.EnterpriseId = pf.EnterpriseID;
                model.StyleModel = (int)Common.EnumFile.SettingSkin.Normal;
                model.BathPartType = batchType;
                model.beginNum = startCode;
                model.endNum = endCode;
                result = new BLL.RequestCodeSettingBLL().Add(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.Add():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取配置项第二页的初始信息
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>第一页初始信息Json数据</returns>
        public ActionResult Two(string subId)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.RequestCodeSettingBLL().GetSencondPageData(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestCodeSetting.Two():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改配置信息拍码页面的显示项
        /// </summary>
        /// <param name="subId">子码段</param>
        /// <param name="displayOption">显示项字符串</param>
        /// <returns>操作结果Json数据</returns>
        public ActionResult Edit(string subId, string displayOption)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestCodeSetting model = new RequestCodeSetting();
                model.ID = Convert.ToInt64(subId);
                model.DisplayOption = displayOption;
                result = new BLL.RequestCodeSettingBLL().Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.Edit():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        public ActionResult EditStyle(string subId, int StyleModel)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestCodeSetting model = new RequestCodeSetting();
                model.ID = Convert.ToInt64(subId);
                model.StyleModel = StyleModel;
                result = new BLL.RequestCodeSettingBLL().Edit(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.Edit():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改配置信息拍码页面的产品信息
        /// </summary>
        /// <param name="subId">子码段</param>
        /// <param name="materialId">产品标识</param>
        /// <param name="brandId">品牌标识</param>
        /// <returns>操作结果Json数据</returns>
        public ActionResult EditMaterial(string subId, string materialId, long brandId, DateTime? productionDate)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestCodeSetting model = new RequestCodeSetting();
                model.ID = Convert.ToInt64(subId);
                model.MaterialID = Convert.ToInt64(materialId);
                model.BrandID = brandId;
                if (productionDate != null)
                {
                    model.ProductionDate = productionDate;
                }
                result = new BLL.RequestCodeSettingBLL().EditMaterial(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.Edit():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取子码段原料列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>子码段原料列表Json数据</returns>
        public ActionResult GetOriginList(string subId)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new BLL.RequestCodeSettingBLL().GetOriginList(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestCodeSetting.GetOrigin():View_RequestOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 给子码段添加原料信息
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <param name="originId">原料标识</param>
        /// <param name="carNum">运输车辆</param>
        /// <param name="checkUser">检验员</param>
        /// <param name="inDate">入库时间</param>
        /// <param name="supplie">来源</param>
        /// <param name="img">原料图片</param>
        /// <returns>操作结果Json数据</returns>
        public ActionResult AddOrigin(string subId, string originId, string driver, string carNum, string checkUser, string inDate, string supplie, string img, string jcimg, string level, string batchNum, string factory, string earTag)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestOrigin model = new RequestOrigin();
                model.AddDate = DateTime.Now;
                model.Driver = driver;
                model.CarNum = carNum;
                model.CheckUser = checkUser;
                model.EnterpriseID = pf.EnterpriseID;
                model.InDate = Convert.ToDateTime(inDate);
                model.OriginID = Convert.ToInt64(originId);
                model.SettingID = Convert.ToInt64(subId);
                model.Status = (int)Common.EnumFile.Status.used;
                model.Supplier = supplie;
                model.Img = new BLL.MaterialBLL().ImgJsonToXml(img);
                model.JCImgInfo = new BLL.MaterialBLL().JCImgJsonToXml(jcimg);
                model.Level = level;
                model.BatchNum = batchNum;
                model.Factory = factory;
                model.Type = 0;
                model.EarTag = earTag;
                string htmlContent = GetEarTag(earTag);
                model.TagContent = htmlContent.Replace("<br ...", "").Replace("<br>", "");
                if (string.IsNullOrEmpty(htmlContent) && !string.IsNullOrEmpty(earTag))
                {
                    return Json(ToJson.NewRetResultToJson("-1", "提示：耳标地址输入不正确！"));
                }
                result = new BLL.RequestCodeSettingBLL().AddOrigin(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.AddOrigin():RequestOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取耳标数据
        /// </summary>
        /// <param name="earTag"></param>
        /// <returns></returns>
        private static string GetEarTag(string earTag)
        {
            try
            {
                String BDURL = string.Format(earTag, DateTime.Now.ToString("yyyyMMddHHmmss"));
                WebClient webClientBD = new WebClient();
                String BDStr = ConvertUnicodeStringToChinese(GetHtml(webClientBD, BDURL, ""));
                Regex m_hovertreeRegex = new Regex(@"<br\s*/{0,1}>", RegexOptions.IgnoreCase);
                BDStr = m_hovertreeRegex.Replace(BDStr, "~");
                BDStr = BDStr.Replace("：", ":").Replace("~~", "~");
                Regex regex = new Regex(@"<td[^>]*>电子证查询码:(?<电子证查询码>[^>]*)~检疫申报编码:(?<检疫申报编码>[^>]*)~检疫证编号:(?<检疫证编号>[^>]*)签发单位:(?<签发单位>[^>]*)~签证官方兽医:(?<签证官方兽医>[^>]*)~签发日期:(?<签发日期>[^>]*)~\[货主\]\s()(?<货主>[^>]*)~\[动物\]\s(?<动物>[^>]*)~\[起运地点\]\s(?<起运地点>[^>]*)~\[到达地点\]\s(?<到达地点>[^>]*)~\[用途\]\s(?<用途>[^>]*)~\[承运人\]\s(?<承运人>[^>]*)~\[运输方式\]\s(?<运输方式>[^>]*)~\[运载工具消毒\]\s(?<运载工具消毒>[^>]*)~\[到达有效\]\s(?<到达有效>[^>]*)~\[备注\]\s(?<备注>[^>]*)~\[动物耳号\]\s(?<动物耳号>[^>]*)</td[^>]*>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
                MatchCollection matchCollection = regex.Matches(BDStr);
                string htmlContent = string.Empty;
                foreach (Match m in matchCollection)
                {
                    htmlContent += "<p>电子证查询码:" + m.Groups["电子证查询码"] + "</p>";
                    htmlContent += "<p>检疫申报编码:" + m.Groups["检疫申报编码"] + "</p>";
                    htmlContent += "<p>检疫证编号:" + m.Groups["检疫证编号"] + "</p>";
                    htmlContent += "<p>签发单位:" + m.Groups["签发单位"] + "</p>";
                    htmlContent += "<p>签证官方兽医:" + m.Groups["签证官方兽医"] + "</p>";
                    htmlContent += "<p>签发日期:" + m.Groups["签发日期"] + "</p>";
                    htmlContent += "<p>货主:" + m.Groups["货主"] + "</p>";
                    htmlContent += "<p>动物:" + m.Groups["动物"] + "</p>";
                    htmlContent += "<p>起运地点:" + m.Groups["起运地点"] + "</p>";
                    htmlContent += "<p>到达地点:" + m.Groups["到达地点"] + "</p>";
                    htmlContent += "<p>用途:" + m.Groups["用途"] + "</p>";
                    htmlContent += "<p>承运人:" + m.Groups["承运人"] + "</p>";
                    htmlContent += "<p>运输方式:" + m.Groups["运输方式"] + "</p>";
                    htmlContent += "<p>运载工具消毒:" + (m.Groups["运载工具消毒"] == null ? "" : m.Groups["运载工具消毒"] .ToString().Replace(":","").Replace("：",""))+ "</p>";
                    htmlContent += "<p>到达有效:" + (m.Groups["到达有效"] == null ? "" : m.Groups["到达有效"].ToString().Replace(":", "").Replace("：", "")) + "</p>";
                    htmlContent += "<p>备注:" + (m.Groups["备注"] == null ? "" : m.Groups["备注"].ToString().Replace(":", "").Replace("：", "")) + "</p>";
                    htmlContent += "<p>动物耳号:" + m.Groups["动物耳号"].ToString().Replace("~", "") + "</p>";
                }
                return htmlContent;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static void InitHeader(WebClient webClient)
        {
            webClient.Headers.Add("X-Requested-With", "XMLHttpRequest");
            webClient.Encoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public static string GetHtml(WebClient webClient, string url, string referer)
        {
            Uri uri = new Uri(url);
            InitHeader(webClient);
            string html = null;
            html = webClient.DownloadString(uri);
            return html;
        }
        /// <summary>
        /// 将unicode转换为中文
        /// </summary>
        /// <param name="unicodeString">unicode字符串</param>
        /// <returns>unicode解码的字符串</returns>
        public static string ConvertUnicodeStringToChinese(string unicodeString)
        {
            if (string.IsNullOrEmpty(unicodeString))
                return string.Empty;

            string outStr = unicodeString;

            Regex re = new Regex("\\\\u[0123456789abcdef]{4}", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(unicodeString);
            foreach (Match ma in mc)
            {
                outStr = outStr.Replace(ma.Value, ConverUnicodeStringToChar(ma.Value).ToString());
            }
            return outStr;
        }

        /// <summary>
        /// 将unicode转换为字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        private static char ConverUnicodeStringToChar(string str)
        {
            char outStr = Char.MinValue;
            outStr = (char)int.Parse(str.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
            return outStr;
        }

        /// <summary>
        /// 修改子码段添加原来信息
        /// </summary>
        /// <param name="id">原料配置标识</param>
        /// <param name="subId">子码段标识</param>
        /// <param name="originId">原料标识</param>
        /// <param name="carNum">运输车辆</param>
        /// <param name="checkUser">检验员</param>
        /// <param name="inDate">入库时间</param>
        /// <param name="supplie">来源</param>
        /// <param name="img">原料图片</param>
        /// <returns>操作结果Json数据</returns>
        public ActionResult EditOrigin(string id, string subId, string originId, string driver, string carNum, string checkUser, string inDate, string supplie, string img, string jcimg, string level, string batchNum, string factory, string earTag)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestOrigin model = new RequestOrigin();
                model.ID = Convert.ToInt64(id);
                model.AddDate = DateTime.Now;
                model.Driver = driver;
                model.CarNum = carNum;
                model.CheckUser = checkUser;
                model.EnterpriseID = pf.EnterpriseID;
                model.InDate = Convert.ToDateTime(inDate);
                model.OriginID = Convert.ToInt64(originId);
                model.SettingID = Convert.ToInt64(subId);
                model.Status = (int)Common.EnumFile.Status.used;
                model.Supplier = supplie;
                model.Img = new BLL.MaterialBLL().ImgJsonToXml(img);
                model.JCImgInfo = new BLL.MaterialBLL().JCImgJsonToXml(jcimg);
                model.Level = level;
                model.BatchNum = batchNum;
                model.Factory = factory;
                model.EarTag = earTag;
                string htmlContent = GetEarTag(earTag);
                model.TagContent = htmlContent.Replace("<br ...", "").Replace("<br>", "");
                if (string.IsNullOrEmpty(htmlContent) && !string.IsNullOrEmpty(earTag))
                {
                    return Json(ToJson.NewRetResultToJson("-1", "提示：耳标地址输入不正确！"));
                }
                result = new BLL.RequestCodeSettingBLL().EditOrigin(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.EditOrigin():RequestOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 删除子码段原来信息
        /// </summary>
        /// <param name="id">要删除的原料配置标识</param>
        /// <returns>操作结果Json数据</returns>
        public ActionResult DelOrigin(string id)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.RequestCodeSettingBLL().DelOrigin(Convert.ToInt64(id));
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.DelOrigin():RequestOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取子码段作业信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>作业信息列表Json数据</returns>
        public ActionResult GetWorkList(string subId)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new BLL.RequestCodeSettingBLL().GetWorkList(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestCodeSetting.GetWorkList():View_RequestOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取子码段巡检信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>巡检信息列表Json数据</returns>
        public ActionResult GetCheckList(string subId)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new BLL.RequestCodeSettingBLL().GetCheckList(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestCodeSetting.GetWorkList():View_RequestOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取子码段质检信息列表
        /// </summary>
        /// <param name="subId">子码段标识</param>
        /// <returns>质检信息列表</returns>
        public ActionResult GetReportList(string subId)
        {
            BaseResultList result = new BaseResultList();
            try
            {
                result = new BLL.RequestCodeSettingBLL().GetReportList(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestCodeSetting.GetWorkList():View_RequestOrigin表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 生成码获取配置项第二页的初始信息
        /// </summary>
        /// <returns>第一页初始信息Json数据</returns>
        public ActionResult GenerateTwo()
        {
            BaseResultModel result = new BaseResultModel();
            List<EnumList> liShowData = new List<EnumList>();
            liShowData.AddRange(Common.EnumText.EnumToList(typeof(Common.EnumFile.SettingShow)));
            liShowData.AddRange(Common.EnumText.EnumToList(typeof(Common.EnumFile.SettingDisplay)));
            result.ObjModel = liShowData;
            result.code = "0";
            result.Msg = "查询成功";
            return Json(result);
        }

        /// <summary>
        /// 生成码获取配置项第3页的初始信息选择模板
        /// </summary>
        /// <returns>第一页初始信息Json数据</returns>
        public ActionResult GenerateThree()
        {
            BaseResultModel result = new BaseResultModel();
            List<EnumList> liStyleData = new List<EnumList>();
            liStyleData.AddRange(Common.EnumText.EnumToList(typeof(Common.EnumFile.SettingSkin)));
            result.ObjModel = liStyleData;
            result.code = "0";
            result.Msg = "查询成功";
            return Json(result);
        }

        /// <summary>
        /// 获取防伪码风格的初始信息选择模板
        /// </summary>
        /// <returns></returns>
        public ActionResult GenerateThreeTwo()
        {
            BaseResultModel result = new BaseResultModel();
            List<EnumList> liStyleData = new List<EnumList>();
            liStyleData.AddRange(Common.EnumText.EnumToListFw(typeof(Common.EnumFile.SettingSkinFw)));
            result.ObjModel = liStyleData;
            result.code = "0";
            result.Msg = "查询成功";
            return Json(result);
        }

        /// <summary>
        /// 获取存储环境
        /// </summary>
        /// <param name="subId">申请码标识列</param>
        /// <returns>实体</returns>
        public JsonResult GetAmbient(long subId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestCodeSettingBLL bll = new RequestCodeSettingBLL();
                result = bll.GetAmbient(subId);
            }
            catch (Exception ex)
            {
                string errData = "PersonalizeController.GetAmbient()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加存储环境
        /// </summary>
        /// <param name="subId">申请码标识</param>
        /// <param name="temperature">存储温度</param>
        /// <param name="inDate">入库时间</param>
        /// <param name="outDate">出库时间</param>
        /// <param name="remark">备注</param>
        /// <returns>操作结果</returns>
        public JsonResult AddAmbient(long subId, string temperature, string inDate, string outDate, string remark)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                SetAmbient model = new SetAmbient();
                model.AddDate = DateTime.Now;
                model.InDate = Convert.ToDateTime(inDate);
                model.OutDate = Convert.ToDateTime(outDate);
                model.EnterpriseID = pf.EnterpriseID;
                model.SettingID = Convert.ToInt64(subId);
                model.Remark = remark;
                model.Status = (int)Common.EnumFile.Status.used;
                model.Temperature = temperature;
                model.SetAcount = pf.UserID;
                result = new RequestCodeSettingBLL().AddAmbient(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Personalize.AddAmbient():SetAmbient表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取物流信息
        /// </summary>
        /// <param name="genId">申请码标识列</param>
        /// <returns>实体</returns>
        public JsonResult GetLogistics(long subId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new RequestCodeSettingBLL().GetLogistics(subId);
            }
            catch (Exception ex)
            {
                string errData = "PersonalizeController.GetLogistics()";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加物流信息
        /// </summary>
        /// <param name="subId">申请码标识列</param>
        /// <param name="logisticsNum">物流单号</param>
        /// <param name="carNum">运输车辆</param>
        /// <param name="startAddress">始发地</param>
        /// <param name="startDate">始发时间</param>
        /// <param name="endAddress">目的地</param>
        /// <param name="endDate">到达时间</param>
        /// <param name="carAmbient">运输环境</param>
        /// <param name="url">追溯网址</param>
        /// <returns>操作结果</returns>
        public JsonResult AddLogistics(long subId, string logisticsNum, string carNum, string startAddress, string startDate,
            string endAddress, string endDate, string carAmbient, string url)
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                SetLogistics model = new SetLogistics();
                model.AddDate = DateTime.Now;
                model.EnterpriseID = pf.EnterpriseID;
                model.SetingID = Convert.ToInt64(subId);
                model.BillNum = logisticsNum;
                model.CarAmbient = carAmbient;
                model.CarNum = carNum;
                model.EndAddress = endAddress;
                if (!string.IsNullOrEmpty(endDate))
                {
                    model.EndDate = Convert.ToDateTime(endDate);
                }
                model.SetAcount = pf.UserID;
                model.StartAddress = startAddress;
                if (!string.IsNullOrEmpty(startDate))
                {
                    model.StartDate = Convert.ToDateTime(startDate);
                }
                model.Status = (int)Common.EnumFile.Status.used;
                model.Url = url;
                result = new RequestCodeSettingBLL().AddLogistics(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Personalize.AddLogistics():SetLogistics表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取班组列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult TeamList()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            TeamBLL bll = new TeamBLL();
            BaseResultList result = new BaseResultList();
            try
            {
                result = bll.GetList(pf.EnterpriseID);
            }
            catch (Exception ex)
            {
                string errData = "Team.TeamList():";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取班组人员信息
        /// </summary>
        /// <param name="teamid">班组ID</param>
        /// <returns>班组人员信息列表</returns>
        public ActionResult GetPersonList(string teamid)
        {
            BaseResultList StrResult = new BaseResultList();
            try
            {
                LoginInfo Pf = SessCokie.Get;
                TeamBLL bll = new TeamBLL();
                StrResult = bll.GetPersonList(Convert.ToInt64(teamid));
            }
            catch (Exception Ex)
            {
                string ErrData = "Admin_Configure.GetModelList():Brand表";
                WriteLog.WriteErrorLog(ErrData + ":" + Ex.Message);
            }
            return Json(StrResult);
        }

        /// <summary>
        /// 获取生产日期
        /// </summary>
        /// <param name="subId">标记ID</param>
        /// <returns></returns>
        public ActionResult GetProductionDate(string subId)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new RequestCodeSettingBLL().GetProductionDate(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestCodeSetting.GetProductionDate";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 是否存在产品配置信息flag=1跳转2添加0异常
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public ActionResult IsExistSettingInfo(long settingId)
        {
            return Json(new { ok = new RequestCodeSettingBLL().IsExistSettingInfo(settingId) });
        }

        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="materialId">产品id</param>
        /// <param name="settingId">配置id</param>
        /// <returns></returns>
        public ActionResult CopySettingInfo(long settingId)
        {
            return Json(new { ok = new RequestCodeSettingBLL().AddSettingInfo(settingId).IsSuccess });
        }

        /// <summary>
        /// 码信息管理加备注信息2018-09-14
        /// </summary>
        /// <param name="subId">配置表标识</param>
        /// <returns>备注信息Json数据</returns>
        public ActionResult SettingMemo(string subId)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new BLL.RequestCodeSettingBLL().SettingMemo(Convert.ToInt64(subId));
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.One():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 添加/编辑该批码备注信息
        /// </summary>
        /// <param name="subId">标识ID</param>
        /// <param name="memo">备注信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEditMemo(string subId,string memo)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestCodeSetting model = new RequestCodeSetting();
                model.ID = Convert.ToInt64(subId);
                model.EnterpriseId = pf.EnterpriseID;
                model.Memo = memo;
                result = new BLL.RequestCodeSettingBLL().AddEditMemo(model);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.AddEditMemo():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        #region 获取模板4图片信息
        /// <summary>
        ///  获取模板4图片信息
        /// </summary>
        /// <param name="rid">RequestCodeSettingID</param>
        /// <returns></returns>
        public ActionResult GetMuBanInfo(long rid)
        {
            BaseResultModel result = new BaseResultModel();
            try
            {
                result = new RequestCodeSettingBLL().GetMuBanInfo(rid);
            }
            catch (Exception ex)
            {
                string errData = "Admin_RequestCodeSetting.GetMuBanInfo";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 加上模板4修改模板四图片方法
        /// </summary>
        /// <param name="subId">标识ID</param>
        /// <param name="StyleModel">模板几</param>
        /// <param name="MuBanImg">模板图片</param>
        /// <param name="ImgLink">模板图片链接</param>
        /// <param name="IsShow">是否显示底部按钮</param>
        /// <returns></returns>
        public ActionResult EditStyleMuBan(string subId, int StyleModel,string MuBanImg, string ImgLink, int IsShow)
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                RequestCodeSetting model = new RequestCodeSetting();
                model.ID = Convert.ToInt64(subId);
                model.StyleModel = StyleModel;
                RequestCodeSettingMuBan mubanModel = new RequestCodeSettingMuBan();
                mubanModel.StrMuBanImg = MuBanImg;
                mubanModel.ImgLink = ImgLink;
                mubanModel.IsShow = IsShow;
                mubanModel.RequestCodeSettingID = Convert.ToInt64(subId);
                result = new BLL.RequestCodeSettingBLL().EditMubanImg(model, mubanModel);
            }
            catch (Exception ex)
            {
                result = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_RequestCodeSetting.Edit():RequestCodeSetting表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return Json(result);
        }
        #endregion
    }
}


