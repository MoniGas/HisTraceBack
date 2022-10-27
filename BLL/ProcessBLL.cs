/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-12-20

** 联系方式 :13313318725

** 描述：生产流程管理业务逻辑 移植

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using LinqModel;
using Dal;
using Common.Argument;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace BLL
{
    /// <summary>
    /// 生产流程管理业务逻辑
    /// </summary>
    public class ProcessBLL
    {
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 获取生产流程列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="processName">生产流程名称</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public BaseResultList GetList(long enterpriseId, string processName, int pageIndex)
        {
            long totalCount = 0;
            ProcessDAL dal = new ProcessDAL();
            List<Process> model = dal.GetList(enterpriseId, processName, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            //string result = JsonConvert.SerializeObject(model);
            return result;
        }

        /// <summary>
        /// 添加生产流程
        /// </summary>
        /// <param name="model">生产流程model</param>
        /// <returns></returns>
        public BaseResultModel Add(Process model)
        {
            ProcessDAL dal = new ProcessDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (!string.IsNullOrEmpty(model.StrOperationList))
            {
                model.OperationList = OperationJsonToXml(model.StrOperationList); ;//根据StrOperationList解析
            }
            ret = dal.Add(model);
            BaseResultModel result = ToJson.NewModelToJson(ret.CrudCount, (Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 修改生产流程
        /// </summary>
        /// <param name="newModel">生产流程信息</param>
        /// <returns></returns>
        public BaseResultModel Edit(Process newModel)
        {
            ProcessDAL dal = new ProcessDAL();
            RetResult ret = new RetResult();
            ret.CmdError = CmdResultError.EXCEPTION;
            if (!string.IsNullOrEmpty(newModel.StrOperationList))
            {
                newModel.OperationList = OperationJsonToXml(newModel.StrOperationList); ;//根据StrPropertyInfo解析
            }
            ret = dal.Edit(newModel);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 删除生产流程
        /// </summary>
        /// <param name="processId">产品规格ID</param>
        /// <returns></returns>
        public BaseResultModel Del(long processId)
        {
            ProcessDAL dal = new ProcessDAL();
            RetResult ret = dal.Delete(processId);
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        /// <summary>
        /// 生产环节列表存成XML格式
        /// </summary>
        /// <param name="jsonPropertys"></param>
        /// <returns></returns>
        private XElement OperationJsonToXml(string jsonPropertys)
        {
            List<ToJsonOperation> ops = JsonConvert.DeserializeObject<List<ToJsonOperation>>(jsonPropertys);
            XElement xml = new XElement("infos");
            foreach (var item in ops)
            {
                xml.Add(
                    new XElement("info",
                        new XAttribute("iname", item.opName),
                        new XAttribute("ivalue", item.opID)
                    )
                );
            }
            return xml;
        }

        /// <summary>
        /// 获取生产流程信息
        /// </summary>
        /// <param name="processId">生产流程ID</param>
        /// <returns></returns>
        public BaseResultModel GetModel(long processId)
        {
            ProcessDAL dal = new ProcessDAL();
            Process model = dal.GetModelByID(processId);
            #region 生产环节XML转JSON类
            List<ToJsonOperation> operations = new List<ToJsonOperation>();
            if (!string.IsNullOrEmpty(model.StrOperationList))
            {
                XElement xml = XElement.Parse(model.StrOperationList);
                IEnumerable<XElement> allOperation = xml.Elements("info");
                foreach (var item in allOperation)
                {
                    ToJsonOperation sub = new ToJsonOperation();
                    sub.opName = item.Attribute("iname").Value;
                    sub.opID = item.Attribute("ivalue").Value;
                    operations.Add(sub);
                }
            }
            model.operations = operations;
            #endregion
            BaseResultModel result = ToJson.NewModelToJson(model, model == null ? "0" : "1", "");
            return result;
        }

        /// <summary>
        /// 获取生产环节信息
        /// </summary>
        /// <param name="processId">生产流程ID</param>
        /// <returns></returns>
        public Process GetModelZ(long processId)
        {
            ProcessDAL dal = new ProcessDAL();
            Process model = dal.GetModelByID(processId);
            #region 生产环节XML转JSON类
            List<ToJsonOperation> operations = new List<ToJsonOperation>();
            if (!string.IsNullOrEmpty(model.StrOperationList))
            {
                XElement xml = XElement.Parse(model.StrOperationList);
                IEnumerable<XElement> allOperation = xml.Elements("info");
                foreach (var item in allOperation)
                {
                    ToJsonOperation sub = new ToJsonOperation();
                    sub.opName = item.Attribute("iname").Value;
                    sub.opID = item.Attribute("ivalue").Value;
                    operations.Add(sub);
                }
            }
            model.operations = operations;
            #endregion
            return model;
        }

        /// <summary>
        /// 获取生产流程列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetProcessList(long enterpriseId)
        {
            ProcessDAL dal = new ProcessDAL();
            List<Process> model = dal.GetProcessList(enterpriseId);
            BaseResultList result = ToJson.NewListToJson(model, 1, 10000000, 0, "");
            return result;
        }
    }
}
