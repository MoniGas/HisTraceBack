using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinqModel;
using System.IO;
using Common.Argument;
using BLL;
using Common.Log;

namespace iagric_plant.Controllers
{
    public class RegisterController : Controller
    {
        private readonly string _enterpeiseCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseCode"].Trim();
        public ActionResult RegisterOne()
        {
            return View();
        }
        //20200722新加去掉头和尾
        public ActionResult RegisterTwo()
        {
            return View();
        }
        public ActionResult RegisterTry()
        {
            return View();
        }
        /// <summary>
        /// 在线注册第二步：农业平台注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Online(string userName, string pwd, string email, string organizeName, string organizeNameEn, string unitType_ID,
            string province, string selCity, string selArea, string user, string userEn, string tel, string code, long platId=16)
        {
            ServiceJK.WebService1SoapClient cl = new ServiceJK.WebService1SoapClient();
            string enMainCode = cl.GetEnterpriseMainCode(_enterpeiseCode);
            RegistBLL bll = new RegistBLL();
            BaseResultModel result = bll.Regist(userName, pwd, email, organizeName, organizeNameEn,platId, unitType_ID,
            province, selCity, selArea, user, userEn, tel, code, enMainCode);
            return Json(result);
        }
        /// <summary>
        ///IDcode平台已注册
        /// </summary>
        /// <returns></returns>
        public ActionResult OnlineQuery(string userName, string pwd, string idcode, long platId, string organUnitAddress, string linkMan, string linkPhone)
        {
            ServiceJK.WebService1SoapClient cl = new ServiceJK.WebService1SoapClient();
            string enMainCode = cl.GetEnterpriseMainCode(_enterpeiseCode);
            RegistBLL bll = new RegistBLL();
            BaseResultModel result = bll.RegistQuery(userName, pwd, idcode,platId,organUnitAddress,linkMan,linkPhone,enMainCode);
            return Json(result);
        }
        /// <summary>
        /// 已经注册确认页面
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterQuery()
        {
            return View();
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// 第一步，验证是否已经注册
        /// </summary>
        /// <param name="unitName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FindUnitName(string unitName)
        {
            RegistBLL bll = new RegistBLL();
            var ret = bll.RegistModify(unitName);
            BaseResultModel result;
            if (ret.CmdError != CmdResultError.NONE)
            {
                result = ToJson.NewRetResultToJson((Convert.ToInt32(CmdResultError.NO_RIGHT)).ToString(), ret.Msg);
                return Json(result);
            }
            Organunitinfo obj = InterfaceWeb.BaseDataDAL.GetCompany(unitName);
            if (obj == null)
            {
                ret.Msg = "没有查询到您的企业信息！";
                ret.CmdError = CmdResultError.EXCEPTION;//1
                result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.CmdError)).ToString(), ret.Msg);
            }
            //未注册
            else if (obj.OrganUnitList.Count == 0 && obj.ResultCode==0)
            {
                ret.Msg = "没有找到对应的企业！";
                ret.CmdError = CmdResultError.PARAMERROR;//2
                result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.CmdError)).ToString(), ret.Msg);
            }
            else if (obj.OrganUnitList.Count == 1 && obj.ResultCode == 1)
            {
                ret.Msg = "找到对应的企业！";
                ret.CmdError = CmdResultError.NONE;//0
                result = ToJson.NewModelToJson(obj.OrganUnitList[0], (Convert.ToInt32(ret.CmdError)).ToString(), ret.Msg);
            }
            else
            {
                ret.Msg = "请输入企业全称！";
                ret.CmdError = CmdResultError.NO_RESULT;//3
                result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.CmdError)).ToString(), ret.Msg);
            }
            return Json(result);
        }
        /// <summary>
        /// 第二步，如果已经注册，登录获取企业信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPass"></param>
        /// <returns></returns>
        public JsonResult GetUnit(string unitName,string loginName, string loginPass)
        {
            RetResult ret = new RetResult();
            OrganUnit info = new OrganUnit();
            string area = string.Empty;
            if (!string.IsNullOrEmpty(loginName) && !string.IsNullOrEmpty(loginPass))
            {
                info = InterfaceWeb.BaseDataDAL.GetUnitInfo(loginName, loginPass);
                if (info.ResultCode == 1 && info.OrganUnitName==unitName.Trim())
                {
                    AreaInfo allAddress = BaseData.listAddress;
                    AddressInfo sheng = allAddress.AddressList.FirstOrDefault(p => p.Address_ID == info.Province_ID);
                    AddressInfo shi = allAddress.AddressList.FirstOrDefault(p => p.Address_ID == info.City_ID);
                    AddressInfo qu = allAddress.AddressList.FirstOrDefault(p => p.Address_ID == info.Area_ID);
                    if (sheng != null)
                    {
                        area = sheng.AddressName + ".";
                    }
                    if (shi != null)
                    {
                        area += shi.AddressName + ".";
                    }
                    if (qu != null)
                    {
                        area += qu.AddressName + ".";
                    }
                    area = area.Substring(0, area.Length - 1);
                    info.Areaaddress = area;
                    ret.Msg = "登录成功";
                    ret.CmdError = CmdResultError.NONE;
                }
                else
                {
                    ret.Msg = "登录失败";
                    ret.CmdError = CmdResultError.NO_RESULT;
                }
            }
            else
            {
                ret.Msg = "请输入登录信息！";
                ret.CmdError = CmdResultError.PARAMERROR;
            }
            BaseResultModel result = ToJson.NewModelToJson(info, (Convert.ToInt32(ret.CmdError)).ToString(), ret.Msg);
            ViewBag.Area = area;
            return Json(result);
        }

        public JsonResult GetPhoneCode(string phone)
        {
            Result result = InterfaceWeb.BaseDataDAL.GetVerifyInfo(phone);
            return Json(result);
        }
        /// <summary>
        /// 下载操作手册
        /// </summary>
        public void OperaManual(string name)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "OperaManual\\";
                string fileName = System.Configuration.ConfigurationManager.AppSettings[name];
                path = path + fileName;
                //FileName--要下载的文件名
                FileInfo downloadFile = new FileInfo(path);
                if (downloadFile.Exists)
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = false;
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode((fileName))));
                    Response.AppendHeader("Content-Length", downloadFile.Length.ToString());
                    Response.WriteFile(downloadFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch
            {
            }
            //return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 试用户获取验证码
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <returns></returns>
        public JsonResult GetPassWord(string phone)
        {
            string code;
            Result result = new RegistBLL().GetPassWord(phone,out code);
            Session["secretCode"] = code;
            return Json(result);
        }

        /// <summary>
        /// 试用户注册
        /// </summary>
        /// <param name="pwd">密码</param>
        /// <param name="organizeName">企业名称</param>
        /// <param name="tel">电话号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public ActionResult RegisterTryMethed(string pwd,  string organizeName,string tel, string code)
        {
            BaseResultModel result = new BaseResultModel {code = "0"};
            if (Session["secretCode"] == null || code.Trim() != Convert.ToString(Session["secretCode"]))
            {
                result.Msg = "验证码输入错误！";
                return Json(result);
            }
            RegistBLL bll = new RegistBLL();
            result = bll.RegisterTry(pwd, organizeName, tel);
            return Json(result);
        }
    }
}
