//激活二维码
using System;
using System.Collections.Generic;
using LinqModel;
using Common.Argument;
using System.Configuration;
using LinqModel.InterfaceModels;
using System.Web;
using System.IO;

namespace BLL
{
    public class ActinvEwmBLL
    {
        public bool AddRecPack(ActiveEwmRecord recPack, string loginName, string loginPwd, out string Msg, out long recId)
        {
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            bool value = dal.AddRecPack(recPack, loginName, loginPwd, out Msg, out recId);
            return value;
        }

        /// <summary>
        /// 激活上传文件
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public BaseResultModel AddActiveRecPack(ActiveEwmRecord newModel)
        {
            RetResult ret = new RetResult();
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            ret = dal.AddActiveRecPack(newModel);
            if (ret.IsSuccess)
            {
                ret.Msg = "上传成功！";
            }
            else
            {
                ret.Msg = "上传失败！";
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }
        int _PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseResultList GetActiveEwmList(long enterpriseId, int type, string beginDate, string endDate, int pageIndex)
        {
            long totalCount = 0;
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            List<ActiveEwmRecord> model = dal.GetActiveEwmList(enterpriseId, type, beginDate, endDate, out totalCount, pageIndex);
            BaseResultList result = ToJson.NewListToJson(model, pageIndex, _PageSize, totalCount, "");
            return result;
        }

        /// <summary>
        /// 获取批次列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <returns></returns>
        public BaseResultList GetBatchList(long enterpriseId)
        {
            long totalCount = 0;
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            List<RequestCodeSetting> model = dal.GetBatchList(enterpriseId, out totalCount);
            BaseResultList result = ToJson.NewListToJson(model, 1, _PageSize, totalCount, "");
            return result;
        }

        public BaseResultModel ActiveEWM(ActiveEwmRecord newModel, long batchNameID)
        {
            RetResult ret = new RetResult();
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            ret = dal.ActiveEWM(newModel, batchNameID);
            if (ret.IsSuccess)
            {
                ret.Msg = "操作成功！";
            }
            else
            {
                ret.Msg = ret.Msg;
            }
            BaseResultModel result = ToJson.NewRetResultToJson((Convert.ToInt32(ret.IsSuccess)).ToString(), ret.Msg);
            return result;
        }

        #region 刘晓杰于2019年11月4日从CFBack项目移入此

        /// <summary>
        /// 获取上传文件记录
        /// </summary>
        /// <returns></returns>
        public List<LinqModel.ActiveEwmRecord> GetActiveEwmList()
        {

            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            List<ActiveEwmRecord> model = dal.GetActiveEwmList();

            return model;
        }

        /// <summary>
        /// 激活上传文件
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public RetResults UpActiveRecPack(ActiveEwmRecord newModel)
        {
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            return dal.UpdateActiveRecPack(newModel);
        }

        /// <summary>
        /// 查询上传是否文件名是否重复
        /// </summary>
        /// <param name="packName">上传文件名</param>
        /// <returns></returns>
        public ActiveEwmRecord IsActiveRecPack(string packName)
        {
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            return dal.IsActiveRecPack(packName);
        }

        /// <summary>
        /// 查询上传是否文件名是否重复
        /// </summary>
        /// <param name="ProduceBathNo">生产批次编号</param>
        /// <returns></returns>
        public ActiveEwmRecord getModelByProduceBathNo(string ProduceBathNo, long eId)
        {
            Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
            return dal.getModelByProduceBathNo(ProduceBathNo, eId);
        }

        public RetResults UploadPIPrivate(PrivatePIRequest model, HttpFileCollection files,string filePath)
        {
            RetResults ret = new RetResults();
            try
            {
                Dal.ActinvEwmDAL dal = new Dal.ActinvEwmDAL();
                ret = dal.UploadPIPrivate(model);
                if (ret.code == 1)
                {
                    string newfilpath = filePath + "\\" + model.enterpriseId + "\\" + model.upUserID + "\\" + model.packName + "\\" ;
                    Directory.CreateDirectory(newfilpath);
                    files[0].SaveAs(newfilpath+files[0].FileName);
                }
            }
            catch (Exception)
            {
                ret.code = -1;
                ret.Msg = "程序出错，上传失败";
                throw;
            }
            return ret;
        }

        #endregion
    }
}
