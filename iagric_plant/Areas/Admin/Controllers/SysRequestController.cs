using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Common.Argument;
using BLL;
using LinqModel;
using System.Text;
using System.IO;

namespace iagric_plant.Areas.Admin.Controllers
{
    public class SysRequestController : Controller
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
        public JsonResult Index(string bd, string ed, string eName, string mName, int pageIndex = 1)
        {
            LoginInfo user = SessCokie.Get;

            LinqModel.BaseResultList result = new RequestCodeBLL().GetSysList(user.EnterpriseID, eName, mName, bd, ed + " 23:59:59", pageIndex, user.PRRU_PlatFormLevel_ID);

            return Json(result);
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
            BaseResultList result = new RequestCodeBLL().GetMaterialNameList(user.EnterpriseID);

            return Json(result);
        }

        /// <summary>
        /// 打包方法
        /// </summary>
        /// <param name="rId">产品申请二维码ID</param>
        /// <returns>返回结果</returns>
        public bool Packaging(long rId)
        {
            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();
            LoginInfo loginInfo = Common.Argument.SessCokie.Get;
            string sUrl = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            string downLoadUrl = string.Empty;
            string eId = string.Empty;
            string webUrl = string.Empty;
            try
            {
                RequestCode requestCode = objRequestCodeBLL.GetModel(rId);
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
                    List<Enterprise_FWCode_00> pd = objRequestCodeBLL.GetEwmList(rId, i + 1, insertCount);//根据Request_ID获取数据分页
                    if (pd != null && pd.Count > 0)
                    {
                        string[] sss = pd[0].EWM.Split('/');
                        string mainCode = sss[0]+"/";
                        foreach (Enterprise_FWCode_00 mode in pd)
                        {
                            sb.AppendFormat("{0},{1},{2}{3}\r\n", mode.EWM.Replace(mainCode, ""), mode.FWCode, ioidURL, mode.EWM);
                        }
                        string txtName = string.Format("{0}-{1}.txt", pd[0].EWM.Split('/')[2], pd[pd.Count - 1].EWM.Split('/')[2]);
                        if (!System.IO.Directory.Exists(string.Format("{0}{1}", webUrl, sUrl)))
                        {
                            System.IO.Directory.CreateDirectory(string.Format("{0}{1}", webUrl, sUrl));
                        }
                        WriteTXT(string.Format("{0}{1}\\{2}", webUrl, sUrl, txtName), sb.ToString());
                    }
                }
                Common.Tools.ZipClass.Zip(string.Format("{0}{1}", webUrl, sUrl), string.Format("{0}\\{1}.rar", webUrl, sUrl), "");
                System.IO.Directory.Delete(string.Format("{0}{1}", webUrl, sUrl), true);
                downLoadUrl = string.Format("/DownloadCode/{0}/{1}.rar", eId, sUrl);
                //更改申请状态
                objRequestCodeBLL.ChangeStatus(rId, (int)Common.EnumFile.RequestCodeStatus.PackToSuccess, downLoadUrl);
                return true;
            }
            catch (Exception ex)
            {
                if (System.IO.Directory.Exists(string.Format("{0}{1}", webUrl, sUrl)))
                {
                    System.IO.Directory.Delete(string.Format("{0}{1}", webUrl, sUrl), true);
                }
                objRequestCodeBLL.ChangeStatus(rId, (int)Common.EnumFile.RequestCodeStatus.PackagingFailure, downLoadUrl);
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
        public int DownloadFile(string downLoadURL)
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

        public JsonResult AuditNew(long rId, long requestCount)
        {
            RequestCodeBLL objRequestCodeBLL = new RequestCodeBLL();
            RequestCode model = objRequestCodeBLL.GetModel(rId);
            BaseResultModel result = new BaseResultModel();
            if (model == null)
            {
                result = ToJson.NewRetResultToJson("0", "数据错误");
                return Json(result);
            }
            // 1.生成 2.打包 3.审核 4.下载
            StringBuilder sb = new StringBuilder();
            result = objRequestCodeBLL.ChangeStatus(rId, requestCount);
            return Json(result);
        }
    }
}
