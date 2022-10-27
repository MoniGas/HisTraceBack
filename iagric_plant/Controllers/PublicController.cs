using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BLL;
using Common;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;
using System.Web;

namespace iagric_plant.Controllers
{
    public class PublicController : Controller
    {
        //
        // GET: /Public/
        /// <summary>
        /// 获取区域
        /// </summary>
        /// <returns></returns>
        public ActionResult Address()
        {
            List<SelectListItem> itemSheng = new List<SelectListItem>();
            AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
            if (AllAddress == null)
            {
                return View();
            }
            List<AddressInfo> sheng = AllAddress.AddressList.Where(p => p.AddressLevel == 1).OrderBy(p => p.AddressCode).ToList();
            if (sheng != null)
            {
                SelectListItem item = new SelectListItem();
                foreach (AddressInfo sub in sheng)
                {
                    item = new SelectListItem();
                    item.Value = sub.AddressCode;
                    item.Text = sub.AddressName;
                    itemSheng.Add(item);
                }
            }
            ViewBag.itemSheng = itemSheng;
            return View();
        }
        /// <summary>
        /// 单位性质
        /// </summary>
        /// <returns></returns>
        public ActionResult UnitType()
        {
            List<SelectListItem> unitType = new List<SelectListItem>();
            List<UnitType> unit = Common.Argument.BaseData.unitType.UnitTypeList;
            if (unit == null)
            {
                return View();
            }
            SelectListItem item = new SelectListItem();
            foreach (UnitType sub in unit)
            {
                item = new SelectListItem();
                item.Value = sub.Code.ToString();
                item.Text = sub.UnitTypeName;
                unitType.Add(item);
            }
            ViewBag.unitType = unitType;
            return View();
        }

        /// <summary>
        /// 医疗单位性质
        /// </summary>
        /// <returns></returns>
        public ActionResult HisUnitType()
        {
            List<SelectListItem> unitType = new List<SelectListItem>();
            List<HisUnitType> unit = Common.Argument.BaseData.HisunitType.unit_type_list;
            if (unit == null)
            {
                return View();
            }
            SelectListItem item = new SelectListItem();
            foreach (HisUnitType sub in unit)
            {
                item = new SelectListItem();
                item.Value = sub.code.ToString();
                item.Text = sub.unittypename;
                unitType.Add(item);
            }
            ViewBag.unitType = unitType;
            return View();
        }

        /// <summary>
        /// 根据省编码获取市
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        /// 110000:北京 310000：上海 500000：重庆 120000：天津 810000：香港特别行政区 820000:澳门特别行政区 710000:台湾

        public ActionResult GetShi(string id, string select = "")
        {
            StringBuilder htmlStr = new StringBuilder();
            try
            {
                List<AddressInfo> shi = new List<AddressInfo>();
                htmlStr.Append("<option value=\"-2\">请选择</option>");
                AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
                if (AllAddress == null)
                {
                    return View();
                }
                if (id == "110000" || id == "310000" || id == "500000" || id == "120000" || id == "810000" || id == "820000" || id == "710000")//直辖市
                {
                    shi = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(id)).ToList();
                }
                else
                {
                    shi = AllAddress.AddressList.Where(p => p.Address_ID_Parent == Convert.ToInt64(id)).ToList();
                }
                if (shi != null && shi.Count > 0)
                {
                    foreach (AddressInfo item in shi)
                    {
                        if (item.Address_ID.ToString() == select)
                            htmlStr.Append("<option selected=\"selected\" value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                        else
                            htmlStr.Append("<option value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                    }
                }
            }
            catch
            { }
            return this.Json(htmlStr.ToString(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetQu(string id, string select = "")
        {
            StringBuilder htmlStr = new StringBuilder();
            try
            {
                List<AddressInfo> qu = new List<AddressInfo>();
                htmlStr.Append("<option selected=\"selected\" value=\"-2\">请选择</option>");
                AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
                if (AllAddress == null)
                {
                    return View();
                }
                if (id == "810000" || id == "820000" || id == "710000")//特别行政区
                {
                    qu = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(id)).ToList();
                }
                else
                {
                    qu = AllAddress.AddressList.Where(p => p.Address_ID_Parent == Convert.ToInt64(id)).ToList();
                }
                if (qu != null && qu.Count > 0)
                {
                    foreach (AddressInfo item in qu)
                    {
                        if (item.Address_ID.ToString() == select)
                            htmlStr.Append("<option selected=\"selected\" value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                        else
                            htmlStr.Append("<option value=" + item.AddressCode.Trim() + ">" + item.AddressName + "</option>");
                    }
                }
            }
            catch
            { }
            return this.Json(htmlStr.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowImg()
        {
            LoginInfo pf = Common.Argument.SessCokie.Get;
            MemoryStream stream = new MemoryStream();
            OIDImage.CreateOIDCodeImage(pf.Verify, Request["ewm"], "", 100, 100).Save(stream, ImageFormat.Jpeg);
            byte[] bytes = stream.ToArray();
            return File(bytes, @"image/jpeg");
        }

        public JsonResult GetSheng()
        {
            AreaInfo AllAddress = Common.Argument.BaseData.listAddress;
            List<AddressInfo> sheng = AllAddress.AddressList.Where(m => m.AddressLevel == 1).OrderBy(m => m.AddressCode).ToList();
            return Json(sheng);
        }

        public JsonResult GetAddressSub(int pid, int level)
        {
            AreaInfo AllAddress = Common.Argument.BaseData.listAddress;

            List<AddressInfo> sub;
            //if ((level == 2 && (pid == 110000 || pid == 310000 || pid == 500000 || pid == 120000 || pid == 810000 || pid == 820000 || pid == 710000)) || (level == 3 && (pid == 810000 || pid == 820000 || pid == 710000 || pid == 441900)))//直辖市
            //{
            //    sub = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(pid)).ToList();
            //}
            //else
            {
                sub = AllAddress.AddressList.Where(m => m.Address_ID_Parent == pid).OrderBy(m => m.AddressCode).ToList();
            }
            return Json(sub);
        }
        public JsonResult GetSub(int pid, int level)
        {
            AreaInfo AllAddress = Common.Argument.BaseData.listAddress;

            List<AddressInfo> sub;
            if ((level == 2 && (pid == 110000 || pid == 310000 || pid == 500000 || pid == 120000 || pid == 810000 || pid == 820000 || pid == 710000)) || (level == 3 && (pid == 810000 || pid == 820000 || pid == 710000 || pid == 441900)))//直辖市
            {
                sub = AllAddress.AddressList.Where(p => p.Address_ID == Convert.ToInt64(pid)).ToList();
            }
            else
            {
                sub = AllAddress.AddressList.Where(m => m.Address_ID_Parent == pid).OrderBy(m => m.AddressCode).ToList();
            }
            return Json(sub);
        }

        public JsonResult GetBatch()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BLL.BatchBLL().GetSelectList(pf.EnterpriseID);
            return Json(result);
        }
        public JsonResult GetBatchExt(long bId)
        {
            BaseResultList result = new BLL.BatchExtBLL().GetSelectList(bId);
            return Json(result);
        }
        public JsonResult GetDealer()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BLL.DealerBLL().GetSelectList(pf.EnterpriseID);
            return Json(result);
        }

        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMaterial()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultList result = new BLL.DealerBLL().GetMaterialList(pf.EnterpriseID);
            return Json(result);
        }

        public ActionResult WapChannel()
        {
            string ewm = Request["ewm"];
            ViewBag.ewm = ewm;
            string[] a = ewm.Split('.');

            List<ShowChannel> channel = new ShowChannelBLL().GetList(long.Parse(a[a.Length - 1])).ObjList as List<ShowChannel>;
            return View(channel);
        }

        public ActionResult HomeMany(string ewm)
        {
            try
            {
                ShowCompany company = Session["company"] as ShowCompany;
                ViewBag.ewm = ewm;
                List<ShowChannel> channel = new ShowChannelBLL().GetList(company.CompanyID).ObjList as List<ShowChannel>;
                return View(channel);
            }
            catch
            {
                return View(new List<ShowChannel>());
            }
        }

        public ActionResult HomeOnly()
        {
            try
            {
                ShowCompany company = Session["company"] as ShowCompany;
                ViewBag.ewm = company.EWM;
                List<ShowChannel> channel = new ShowChannelBLL().GetList(company.CompanyID).ObjList as List<ShowChannel>;
                return View(channel);
            }
            catch
            {
                return View(new List<ShowChannel>());
            }
        }
        public ActionResult HomeOnlyCompany()
        {
            try
            {
                ShowCompany company = Session["company"] as ShowCompany;
                ViewBag.CompanyName = company.CompanyName;
                return View(company);
            }
            catch
            {
                return View(new ShowCompany());
            }
        }
        public ActionResult HomeDisMore()
        {
            try
            {
                ShowCompany company = Session["company"] as ShowCompany;
                ViewBag.ewm = company.EWM;
                ViewBag.Infos = company.Infos;
                List<ShowChannel> channel = new ShowChannelBLL().GetList(company.CompanyID).ObjList as List<ShowChannel>;
                return View(channel);
            }
            catch
            {
                return View(new List<ShowChannel>());
            }
        }

        public ActionResult HomeChannelOne(long id, string ewm)
        {
            ViewBag.ewm = ewm;
            PagedList<View_NewsChannel> news = new ShowNewsBLL().GetPagedList(0, id, 1);
            return View(news);
        }
        public ActionResult HomeChannelTwo(long id)
        {
            PagedList<View_NewsChannel> news = new ShowNewsBLL().GetPagedList(0, id, 1);
            return View(news);
        }
        public ActionResult HomeChannelThere(long id)
        {
            PagedList<View_NewsChannel> news = new ShowNewsBLL().GetPagedList(0, id, 1);
            return View(news);
        }
        public JsonResult GetAppUrl()
        {
            String url = ConfigurationManager.AppSettings["AppUrl"];
            ToJsonImg a = new ToJsonImg();
            a.fileUrl = url;
            return Json(a);
        }

        public JsonResult GetPreviewUrl(string ewm)
        {
            String url = "http://" + Request.Url.Authority + "/Wap_Preview/Index?ewm=" + ewm;
            ToJsonImg a = new ToJsonImg();
            a.fileUrl = url;
            return Json(a);
        }

        #region 下载如何说明
        [HttpGet]
        public int DownloadFile(int fileType)
        {
            try
            {
                string downLoadURL = "";
                switch (fileType)
                {
                    case 1:
                        downLoadURL = "/ProductManual/" + ConfigurationManager.AppSettings["ServerProductManual"].ToString();
                        break;
                    case 2:
                        downLoadURL = "/ProductManual/" + ConfigurationManager.AppSettings["AppProductManual"].ToString();
                        break;
                    default:
                        downLoadURL = "/ProductManual/" + ConfigurationManager.AppSettings["ServerProductManual"].ToString();
                        break;
                }

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
        #endregion

        public JsonResult GetIdcodeURL()
        {
            String url = ConfigurationManager.AppSettings["idcodeURL"];
            LoginInfo pf = Common.Argument.SessCokie.Get;
            string code = Request.Params["ewm"];
            string[] arrCode = null;
            if (!string.IsNullOrEmpty(code))
            {
                arrCode = code.Split('.');
            }
            if (pf.Verify == (int)Common.EnumFile.EnterpriseVerify.Try || (arrCode != null && arrCode.Length > 5 && (arrCode[4] == ((int)Common.EnumFile.TerraceEwm.slotting).ToString() || arrCode[4] == ((int)Common.EnumFile.TerraceEwm.cribCode).ToString())))
            {
                url = System.Configuration.ConfigurationManager.AppSettings["ncpURL"].ToString();
            }
            return Json(url);
        }

        public ActionResult SelectMaterial(long enterpriseId = 0)
        {
            List<Material> liMaterial = new List<Material>();
            try
            {
                liMaterial = new BLL.Material_OnlineOrderBLL().GetMaterialList(enterpriseId);
                Material material = new Material();
                material.Material_ID = 0;
                material.MaterialFullName = "请选择产品";
                liMaterial.Insert(0, material);
            }
            catch { }
            return View(liMaterial);
        }
        public ActionResult GetMaterialSpec(long materialId = 0)
        {
            StringBuilder htmlStr = new StringBuilder();
            htmlStr.Append("<option selected=\"selected\" value=\"0\">请选择规格</option>");
            try
            {
                List<Material_Spec> liMaterial = new BLL.Material_OnlineOrderBLL().GetMaterialSpecList(materialId);
                foreach (var item in liMaterial)
                {
                    htmlStr.Append("<option value=\"" + item.ID + "\">" + item.Price + " 元，规格：" + item.MaterialSpecification + "</option>");
                }
            }
            catch { }
            return this.Json(htmlStr.ToString(), JsonRequestBehavior.AllowGet);
        }

        #region 获取企业主码简码
        [HttpPost]
        public ActionResult EditEnJMainCode()
        {
            LoginInfo pf = SessCokie.Get;
            BaseResultModel result = new BaseResultModel();
            try
            {
                Enterprise_Info infoModel = new Enterprise_Info();
                EnterpriseInfoBLL bll = new EnterpriseInfoBLL();
                string EnterpeiseCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseCode"].ToString().Trim();
                ServiceJK.WebService1SoapClient cl = new ServiceJK.WebService1SoapClient();
                string enMainCode = cl.GetEnterpriseMainCode(EnterpeiseCode);
                result = bll.EditEnJMainCode(pf.EnterpriseID, enMainCode);
            }
            catch (Exception ex)
            {
            }
            return Json(result);
        }
        #endregion
        /// <summary>
        ///  系统升级公告读取txt文件
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSysUp()
        {
            string path = Server.MapPath("\\sysup.txt");
            System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.Default);
            String line;
            TempList sdf = new TempList();
            List<sysItem> lis = new List<sysItem>();
            while ((line = sr.ReadLine()) != null)
            {
                sysItem svalue = new sysItem();
                svalue.value = line.ToString();
                lis.Add(svalue);
            }
            sdf.sysItem = lis;
            return Json(lis);
        }

        public Enterprise_FWCode_00 GetSession()
        {
            Enterprise_FWCode_00 code = null;
            code = (Session["code"] as CodeInfo).FwCode;
            return code;
        }

        public ActionResult ConfirmOrder(string MaterialSpecId, string Count, string uppage)
        {
            Enterprise_FWCode_00 code = GetSession();
            ObjectSession ObjSession = Session["ObjSession"] as ObjectSession;
            if (!string.IsNullOrEmpty(MaterialSpecId) && !string.IsNullOrEmpty(Count) && !string.IsNullOrEmpty(uppage))
            {
                ObjSession = new ObjectSession();
                ObjSession.MaterialSpecId = MaterialSpecId;
                ObjSession.Count = Count;
                ObjSession.uppage = uppage;
                Session["ObjSession"] = ObjSession;
            }
            else if (ObjSession == null)
            {
                return Content("<script>alert('数据错误，请刷新后重试！');history.go(-1)</script>");
            }

            ScanCodeBLL scan = new ScanCodeBLL();
            View_MaterialSpecForMarket material = scan.GetMaterialModel(Convert.ToInt64(MaterialSpecId));
            ViewBag.uppage = ObjSession.uppage;
            //Enterprise_FWCode_00 code = GetSession();
            View_Order_Consumers consumers = Common.Argument.SessCokieOrder.Get;
            if (code != null && consumers != null)
            {
                //Material material = scan.GetMaterial(code.Material_ID.GetValueOrDefault(0));
                Enterprise_Info enterprise = scan.GetEnterprise(long.Parse(code.Enterprise_Info_ID));

                ViewBag.material = material;
                //ViewBag.material = ma;
                ViewBag.enterprise = enterprise;
                ViewBag.consumers = consumers;
                ViewBag.code = code;
                ViewBag.MaterialSpecId = ObjSession.MaterialSpecId;
                ViewBag.Count = ObjSession.Count;
                return View();
            }
            else if (code == null)
            {
                return Content("<script>alert('请拍码访问页面！')</script>");
            }
            else
            {
                return Content("<script>window.location.href = '/wap_order/login?pageType=4&MaterialSpecId=" + MaterialSpecId + "&Count=" + Count + "&uppage=" + uppage + "';</script>");
            }
        }


        /// <summary>
        /// 二维码码图片下载
        /// </summary>
        /// <param name="oid">中心i_oid码</param>
        /// <param name="w">图片宽度</param>
        /// <param name="h">图片高度</param>
        /// <returns></returns>
        public int MemberICodeDown(string oid, int w, int h)
        {
            try
            {
                if (!string.IsNullOrEmpty(oid))
                {
                    string url = ConfigurationManager.AppSettings["idcodeURL"];
                    LoginInfo pf = Common.Argument.SessCokie.Get;
                    if (pf.Verify == (int)Common.EnumFile.EnterpriseVerify.Try)
                    {
                        url = System.Configuration.ConfigurationManager.AppSettings["ncpURL"].ToString();
                    }
                    //输出到浏览器
                    System.Drawing.Image codeImg = MvcWebCommon.Tools.CreateEwmImg.GetQRCodeImageEx(url + oid, w, h);
                    if (codeImg != null)
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            codeImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            Response.ContentType = "application/octet-stream";
                            //通知浏览器下载文件而不是打开
                            Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode(oid + ".jpg", System.Text.Encoding.UTF8)));
                            Response.BinaryWrite(ms.ToArray());
                            Response.Flush();
                            Response.End();
                            return 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public int MemberDown(string oid, int type, int w, int h)
        {
            try
            {
                if (!string.IsNullOrEmpty(oid))
                {
                    string url = string.Empty;
                    if (type == 1)
                    {
                        url = ConfigurationManager.AppSettings["enterpriseURL"];
                    }
                    LoginInfo pf = Common.Argument.SessCokie.Get;
                    //输出到浏览器
                    System.Drawing.Image codeImg = MvcWebCommon.Tools.CreateEwmImg.GetQRCodeImageEx(url + oid, w, h);
                    if (codeImg != null)
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            codeImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            Response.ContentType = "application/octet-stream";
                            //通知浏览器下载文件而不是打开
                            Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode(oid + ".jpg", System.Text.Encoding.UTF8)));
                            Response.BinaryWrite(ms.ToArray());
                            Response.Flush();
                            Response.End();
                            return 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        /// <summary>
        /// 下载操作手册
        /// </summary>
        public void OperaManual()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "FiesData\\";
                string fileName = System.Configuration.ConfigurationManager.AppSettings["PDF"];
                path = path + fileName;
                //FileName--要下载的文件名
                FileInfo DownloadFile = new FileInfo(path);
                if (DownloadFile.Exists)
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = false;
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode((fileName))));
                    Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                    Response.WriteFile(DownloadFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch
            {
            }
            //return Json("", JsonRequestBehavior.AllowGet);
        }

        public void OperaManualApp()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "FiesData\\";
                string fileName = System.Configuration.ConfigurationManager.AppSettings["PDFApp"];
                path = path + fileName;
                //FileName--要下载的文件名
                FileInfo DownloadFile = new FileInfo(path);
                if (DownloadFile.Exists)
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = false;
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(HttpUtility.UrlDecode((fileName))));
                    Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                    Response.WriteFile(DownloadFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch
            {
            }
            //return Json("", JsonRequestBehavior.AllowGet);
        }

        #region 20210429
        public ActionResult UpSyMaterialPI(long EnterpriseID, string YouXiaoDate, string ShiXiaoDate, string materialName,
           string fixedCode, int totalCount, string shengChanPH, int codingClientType, string category_code, string specification,
            string code_list_str, string start_date, string d_batch_number, string product_model)
        {
            //long enterpriseId = Convert.ToInt64(context.Request.QueryString["EnterpriseID"]);//企业ID
            //string youxiaoqi = context.Request.QueryString["YouXiaoDate"];//有效期
            //string shixiaoqi = context.Request.QueryString["ShiXiaoDate"];//失效期
            //string materialName = context.Request.QueryString["MaterialName"];//产品名称
            //string fixedCode = context.Request.QueryString["FixedCode"];//码前缀（DI）
            //int totalCount = Convert.ToInt32(context.Request.QueryString["TotalCount"]);//生成数量
            //string shengChanPH = context.Request.QueryString["ShengChanPH"];//码前缀（DI）
            //int codingClientType = Convert.ToInt32(context.Request.QueryString["CodingClientType"]);//1：MA码，2：GS1码
            //string category_code = context.Request.QueryString["category_code"];//品类编码
            //string specification = context.Request.QueryString["specification"];//包装规格[仅限下列定值：0 - 9)
            //string code_list_str = context.Request.QueryString["code_list_str"];//序列号列表[“1,2,3”)
            //string start_date = context.Request.QueryString["start_date"];//生产日期
            //string d_batch_number = context.Request.QueryString["d_batch_number"];//灭菌批号
            //string product_model = context.Request.QueryString["product_model"];//生产批号
            if (string.IsNullOrEmpty(materialName))
            {
                //context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "产品名称不可为空！" }));
                //return;
                return Content("<script>alert('产品名称不可为空！');</script>");
            }
            if (string.IsNullOrEmpty(fixedCode))
            {
                //context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "UDI-DI编码不可为空！" }));
                //return;
                return Content("<script>alert('UDI-DI编码不可为空！');</script>");
            }
            if (EnterpriseID <= 0)
            {
                //context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "企业ID不可为空！" }));
                //return;
                return Content("<script>alert('企业ID不可为空！');</script>");
            }
            if (totalCount <= 0)
            {
                //context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = -1, Msg = "生成数量可为空！" }));
                //return;
                return Content("<script>alert('生成数量可为空！');</script>");
            }
            RequestCode ObjRequestCode = new RequestCode();
            ObjRequestCode.Enterprise_Info_ID = EnterpriseID;
            ObjRequestCode.adddate = DateTime.Now;
            //ObjRequestCode.IDCodeBatchNo = sub.batch_no;//服务执行上传MA时可更新
            ObjRequestCode.RequestDate = DateTime.Now;
            ObjRequestCode.ShengChanPH = shengChanPH;
            if (!string.IsNullOrEmpty(shengChanPH))
            {
                ObjRequestCode.ShengChanPH = shengChanPH;
            }
            if (!string.IsNullOrEmpty(ShiXiaoDate))
            {
                ObjRequestCode.ShiXiaoDate = ShiXiaoDate;
            }
            if (!string.IsNullOrEmpty(YouXiaoDate))
            {
                ObjRequestCode.YouXiaoDate = YouXiaoDate;
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
            ObjRequestCode.CodingClientType = codingClientType;//新加的字段
            ObjRequestCode.category_code = category_code;
            ObjRequestCode.BZSpecType = Convert.ToInt32(specification);
            ObjRequestCode.code_list_str = code_list_str;
            ObjRequestCode.startdate = start_date;
            ObjRequestCode.dbatchnumber = d_batch_number;
            ObjRequestCode.product_model = product_model;
            ObjRequestCode.ISUpload = (int)Common.EnumFile.RequestISUpload.NotUploaded;
            ObjRequestCode.createtype = (int)Common.EnumFile.CreateType.UpJieKou;
            RequestCodeSetting setModel = new RequestCodeSetting();
            setModel.EnterpriseId = EnterpriseID;
            setModel.Count = totalCount;
            setModel.beginCode = 1;
            setModel.endCode = totalCount;
            setModel.RequestCodeType = (int)Common.EnumFile.RequestCodeType.fwzsCode;
            setModel.SetDate = DateTime.Now;
            setModel.BatchType = 1;
            RequestCodeBLL bll = new RequestCodeBLL();
            RetResult result = bll.AddPIInfo(ObjRequestCode, setModel, materialName);
            return Json(result);
            //context.Response.Write(new JavaScriptSerializer().Serialize(new RetResults { code = result.Code, Msg = result.Msg, ID = result.id }));
        }
        #endregion
    }
    public class TempList
    {
        public List<sysItem> sysItem { get; set; }
    }
    public class sysItem
    {
        public string value { get; set; }
    }
}
