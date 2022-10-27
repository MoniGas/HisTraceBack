using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Text;
using System.Xml.Linq;
using Common.Argument;
using Dal;
using LinqModel;
using Newtonsoft.Json;

namespace BLL
{
    public class ShowUserBLL
    {
        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        /// <summary>
        /// 获取名片列表
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <param name="name">人员名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultList GetList(long companyId, string name, int pageIndex)
        {
            long totalCount = 0;
            ShowUserDAL dal = new ShowUserDAL();
            List<ShowUser> model = dal.GetList(companyId, name, pageIndex, out totalCount);

            LinqModel.BaseResultList result = ToJson.NewListToJson(model, pageIndex, PageSize, totalCount, "");

            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">名片标识</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel GetModel(long id)
        {
            ShowUserDAL dal = new ShowUserDAL();
            ShowUser model = dal.GetModel(id);

            LinqModel.BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        /// <summary>
        /// 添加名片
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel Add(long eId, string img, string trueName, string position, string telPhone, string mail, string qq, string hometown, string location, string memo, string infos, string ewm)
        {
            RetResult ret = new RetResult();
            if (string.IsNullOrEmpty(img))
            {
                ret.Msg = "请上传头像！";
            }
            else if (string.IsNullOrEmpty(trueName))
            {
                ret.Msg = "真实姓名不能为空！";
            }
            else if (string.IsNullOrEmpty(position))
            {
                ret.Msg = "职位不能为空！";
            }
            else if (string.IsNullOrEmpty(telPhone))
            {
                ret.Msg = "联系电话不能为空！";
            }
            else if (string.IsNullOrEmpty(mail))
            {
                ret.Msg = "邮箱地址不能为空！";
            }
            else
            {
                ShowUser model = new ShowUser();
                model.EWM = ewm;
                model.Status = 1;
                EntitySet<ShowUserAttributes> sub = new EntitySet<ShowUserAttributes>();
                if (!string.IsNullOrEmpty(infos))
                {
                    DateTime now = DateTime.Now;
                    string[] para = infos.Split('^');
                    for (int i = 0; i < para.Length - 1; i++)
                    {
                        ShowUserAttributes item = new ShowUserAttributes();
                        string[] temp = para[i].Split('：');
                        if (temp.Length != 2)
                        {
                            ret.Msg = "邮箱地址不能为空！";
                            goto Error;
                        }
                        item.Title = temp[0];
                        item.Contents = temp[1];
                        item.AddTime = now;
                        sub.Add(item);
                    }
                }
                ToJsonImg imgs = JsonConvert.DeserializeObject<ToJsonImg>(img);
                string strImg = string.Empty;
                XElement xml = new XElement("infos");
                xml.Add(
                    new XElement("info",
                        new XAttribute("name", "headimg"),
                        new XAttribute("value", imgs.fileUrl)
                    )
                );
                strImg = imgs.fileUrl;
                //List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(img);
                //string strImg = string.Empty;
                //foreach (var item in imgs)
                //{
                //    strImg = item.fileUrl;
                //}

                StringBuilder strXML = new StringBuilder();
                strXML.AppendLine("<infos>");
                strXML.AppendLine("<info name=\"name\" value=\"" + trueName + "\" />");
                strXML.AppendLine("<info name=\"position\" value=\"" + position + "\" />");
                strXML.AppendLine("<info name=\"telPhone\" value=\"" + telPhone + "\" />");
                strXML.AppendLine("<info name=\"mail\" value=\"" + mail + "\" />");
                strXML.AppendLine("<info name=\"qq\" value=\"" + qq + "\" />");
                strXML.AppendLine("<info name=\"hometown\" value=\"" + hometown + "\" />");
                strXML.AppendLine("<info name=\"location\" value=\"" + location + "\" />");
                strXML.AppendLine("<info name=\"memo\" value=\"" + memo + "\" />");
                strXML.AppendLine("<info name=\"headimg\" value=\"" + strImg + "\" />");
                strXML.AppendLine("</infos>");
                XElement element = XElement.Parse(strXML.ToString());
                model.InfoOther = element;
                model.Infos = trueName;
                model.CompanyID = eId;
                model.ShowUserAttributes = sub;

                ShowUserDAL dal = new ShowUserDAL();
                ret = dal.Add(model);
            }
        Error:
            LinqModel.BaseResultModel result = ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 修改名片
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="sub">扩展属性</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel Edit(long eId, long uId, string img, string trueName, string position, string telPhone, string mail, string qq, string hometown, string location, string memo, string infos, string ewm)
        {
            RetResult ret = new RetResult();
            if (string.IsNullOrEmpty(img))
            {
                ret.Msg = "请上传头像！";
            }
            else if (string.IsNullOrEmpty(trueName))
            {
                ret.Msg = "真实姓名不能为空！";
            }
            else if (string.IsNullOrEmpty(position))
            {
                ret.Msg = "职位不能为空！";
            }
            else if (string.IsNullOrEmpty(telPhone))
            {
                ret.Msg = "联系电话不能为空！";
            }
            else if (string.IsNullOrEmpty(mail))
            {
                ret.Msg = "邮箱地址不能为空！";
            }
            else
            {
                ShowUser model = new ShowUser();
                model.EWM = ewm;
                List<ShowUserAttributes> sub = new List<ShowUserAttributes>();
                if (!string.IsNullOrEmpty(infos))
                {
                    DateTime now = DateTime.Now;
                    string[] para = infos.Split('^');
                    for (int i = 0; i < para.Length - 1; i++)
                    {
                        ShowUserAttributes item = new ShowUserAttributes();
                        string[] temp = para[i].Split('：');
                        if (temp.Length != 2)
                        {
                            ret.Msg = "邮箱地址不能为空！";
                            goto Error;
                        }
                        item.Title = temp[0];
                        item.Contents = temp[1];
                        item.AddTime = now;
                        sub.Add(item);
                    }
                }
                string strImg = string.Empty;
                strImg = img;
                //List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(img);
                //string strImg = string.Empty;
                //foreach (var item in imgs)
                //{
                //    strImg = item.fileUrl;
                //}

                StringBuilder strXML = new StringBuilder();
                strXML.AppendLine("<infos>");
                strXML.AppendLine("<info name=\"name\" value=\"" + trueName + "\" />");
                strXML.AppendLine("<info name=\"position\" value=\"" + position + "\" />");
                strXML.AppendLine("<info name=\"telPhone\" value=\"" + telPhone + "\" />");
                strXML.AppendLine("<info name=\"mail\" value=\"" + mail + "\" />");
                strXML.AppendLine("<info name=\"qq\" value=\"" + qq + "\" />");
                strXML.AppendLine("<info name=\"hometown\" value=\"" + hometown + "\" />");
                strXML.AppendLine("<info name=\"location\" value=\"" + location + "\" />");
                strXML.AppendLine("<info name=\"memo\" value=\"" + memo + "\" />");
                strXML.AppendLine("<info name=\"headimg\" value=\"" + strImg + "\" />");
                strXML.AppendLine("</infos>");
                XElement element = XElement.Parse(strXML.ToString());
                model.InfoOther = element;
                model.Infos = trueName;
                model.CompanyID = eId;
                model.UserID = uId;

                ShowUserDAL dal = new ShowUserDAL();
                ret = dal.Edit(model, sub);
            }
        Error:
            LinqModel.BaseResultModel result = ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除名片
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <param name="id">明天标识</param>
        /// <returns>json</returns>
        public LinqModel.BaseResultModel Del(long companyId, long id)
        {
            ShowUserDAL dal = new ShowUserDAL();
            RetResult ret = dal.Del(companyId, id);
            LinqModel.BaseResultModel result = ToJson.NewRetResultToJson(Convert.ToInt32(ret.IsSuccess).ToString(), ret.Msg);
            return result;
        }
    }
}
