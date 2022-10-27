/********************************************************************************
** 作者： 张翠霞

** 创始时间：2017-05-25

** 联系方式 :15031109901

** 描述：图片视频上传数据层

** 版本：v2.0

** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;
using LinqModel;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 图片视频上传数据层
    /// </summary>
    public class UploadsDAL : DALBase
    {
        /// <summary>
        /// 获取上传列表
        /// </summary>
        /// <param name="enterpriseId">企业标识</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <returns>列表</returns>
        public List<Uploads> GetList(long enterpriseId, string materialName, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<Uploads> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var tempResult = dataContext.Uploads.Where(m => m.Enterprise_Info_ID == enterpriseId
                        && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        tempResult = tempResult.Where(m => m.Remark.Contains(materialName.Trim()));
                    }
                    totalCount = tempResult.Count();
                    result = tempResult.OrderByDescending(m => m.UploadsID).ToList();
                    // 判断页码大于0为有效页码
                    if (pageIndex > 0)
                    {
                        result = result.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    }
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "UploadsDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 上传图片视频
        /// </summary>
        /// <param name="Origin">实体模型</param>
        /// <returns></returns>
        public RetResult Add(Uploads model)
        {
            string Msg = "上传失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    dataContext.Uploads.InsertOnSubmit(model);
                    dataContext.SubmitChanges();
                    Ret.CrudCount = model.UploadsID;
                    Msg = "上传成功";
                    error = CmdResultError.NONE;
                }
                catch
                {
                    Ret.Msg = "链接数据库失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取图片上传模型
        /// </summary>
        /// <param name="materialId">上传标识</param>
        /// <returns>模型</returns>
        public Uploads GetModel(long uploadsID)
        {
            Uploads result = new Uploads();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Uploads.FirstOrDefault(m => m.UploadsID == uploadsID && m.Status == (int)Common.EnumFile.Status.used);
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "UploadsDAL.GetModel()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 修改上传
        /// </summary>
        /// <param name="newModel">模型</param>
        /// <returns>操作结果</returns>
        public RetResult Edit(Uploads newModel)
        {
            Ret.Msg = "修改上传信息失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = dataContext.Uploads.FirstOrDefault(p => p.UploadsID == newModel.UploadsID &&
                        p.Enterprise_Info_ID == newModel.Enterprise_Info_ID);
                    if (model == null)
                    {
                        Ret.Msg = "没有找到要修改的图片！";
                    }
                    else
                    {
                        model.Remark = newModel.Remark;
                        model.ImgInfo = newModel.ImgInfo;
                        dataContext.SubmitChanges();
                        Ret.Msg = "修改上传信息成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch (Exception ex)
                {
                    string errData = "UploadsDAL.Edit()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return Ret;
        }
    }
}
