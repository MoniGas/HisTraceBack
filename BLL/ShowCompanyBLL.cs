/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-06-24

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于企业宣传码信息管理逻辑层

*********************************************************************************/

using System;
using Common.Argument;
using Dal;
using LinqModel;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace BLL
{
    public class ShowCompanyBLL
    {
        /// <summary>
        /// 获取企业宣传数据
        /// </summary>
        /// <param name="companyId">企业标识</param>
        /// <returns>JSON</returns>
        public BaseResultModel GetModel(long companyId)
        {
            ShowCompanyDAL dal = new ShowCompanyDAL();
            ShowCompany model = dal.GetModel(companyId);
            
            List<ToJsonImg> imgs = new List<ToJsonImg>();
            if (!string.IsNullOrEmpty(model.StrFiles))
            {
                XElement xml = XElement.Parse(model.StrFiles);
                IEnumerable<XElement> allImg = xml.Elements("img");
                foreach (var item in allImg)
                {
                    ToJsonImg sub = new ToJsonImg();
                    sub.fileUrl = item.Attribute("value").Value;
                    sub.fileUrls = item.Attribute("small").Value;
                    imgs.Add(sub);
                }
            }
            model.imgs = imgs;

            BaseResultModel result = ToJson.NewModelToJson(model, "1", "");
            return result;
        }

        /// <summary>
        /// 编辑企业宣传内容
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>JSON</returns>
        public BaseResultModel Edit(ShowCompany model)
        {
            List<ToJsonImg> imgs = JsonConvert.DeserializeObject<List<ToJsonImg>>(model.StrFiles);
            XElement xml = new XElement("infos");
            foreach (var item in imgs)
            {
                xml.Add(
                    new XElement("img",
                        new XAttribute("name", "1.jpg"),
                        new XAttribute("value", item.fileUrl),
                        new XAttribute("small", item.fileUrls)
                    )
                );
            }
            
            model.Files = xml;//根据Files解析


            ShowCompanyDAL dal = new ShowCompanyDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (string.IsNullOrEmpty(model.Infos))
            {
                ret.Msg = "内容不能为空！";
            }
            else
            {
                ret = dal.Edit(model);
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
    }
}
