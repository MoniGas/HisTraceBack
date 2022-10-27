/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-08-02

** 联系方式 :13313318725

** 描述：产品规格管理数据访问

** 版本：v1.0

** 版权：研一 农业项目组  

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 产品规格管理数据访问
    /// </summary>
    public class MaterialSpcificationDAL : DALBase
    {
        /// <summary>
        /// 获取规格列表
        /// </summary>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        public List<MaterialSpcification> GetList(long enterpriseId, string spection, out long totalCount, int pageIndex)
        {
            List<MaterialSpcification> result = null;
            totalCount = 0;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.MaterialSpcification.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(spection))
                    {
                        data = data.Where(m => (m.Value + m.MaterialSpcificationName).Contains(spection.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.MaterialSpcificationID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialSpcificationDAL.GetList():MaterialSpcification表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取产品规格（下拉列表）
        /// </summary>
        /// <param name="enterpriseId">企业Id</param>
        /// <returns></returns>
        public List<MaterialSpcification> GetListMaS(long enterpriseId)
        {
            List<MaterialSpcification> result = null;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.MaterialSpcification.Where(m => (m.Enterprise_Info_ID == enterpriseId || m.Enterprise_Info_ID == 0)
                        && m.Status == (int)Common.EnumFile.Status.used);
                    //result = data.OrderByDescending(m => m.MaterialSpcificationID).ToList();
                    result = data.OrderBy(m => m.Value).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialSpcificationDAL.GetListMaS():MaterialSpcification表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加产品规格
        /// </summary>
        /// <param name="materialS">产品规格实体</param>
        /// <returns>返回结果正确/错误</returns>
        public RetResult Add(MaterialSpcification materialS)
        {
            string Msg = "添加产品规格失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.MaterialSpcification.FirstOrDefault(m => m.Value == materialS.Value && m.MaterialSpcificationName == materialS.MaterialSpcificationName &&
                        m.Status == (int)Common.EnumFile.Status.used && m.Enterprise_Info_ID == materialS.Enterprise_Info_ID);
                    //var data1 = dataContext.MaterialSpcification.FirstOrDefault(m => m.MaterialSpcificationCode == materialS.MaterialSpcificationCode &&
                    //    m.Status == (int)Common.EnumFile.Status.used && m.Enterprise_Info_ID == materialS.Enterprise_Info_ID);
                    //判断添加的规格是否重复
                    if (data != null)
                    {
                        Msg = "已存在该产品规格！";
                    }
                    //else if (data1 != null)
                    //{
                    //    Msg = "已存在该规格编码！";
                    //}
                    else
                    {
                        dataContext.MaterialSpcification.InsertOnSubmit(materialS);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = materialS.MaterialSpcificationID;
                        Msg = "添加产品规格成功";
                        error = CmdResultError.NONE;
                    }
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
        /// 修改产品规格
        /// </summary>
        /// <param name="materialS">产品规格实体</param>
        /// <returns>返回结果正确/错误</returns>
        public RetResult Edit(MaterialSpcification materialS)
        {
            string Msg = "修改产品规格失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = from m in dataContext.MaterialSpcification
                                where m.Value == materialS.Value && m.MaterialSpcificationName == materialS.MaterialSpcificationName &&
                                    m.MaterialSpcificationID != materialS.MaterialSpcificationID && m.Status == (int)Common.EnumFile.Status.used
                                    && m.Enterprise_Info_ID == materialS.Enterprise_Info_ID
                                select m;
                    //判断产品规格是否重复
                    if (model.Count() > 0)
                    {
                        Msg = "已存在该产品规格！";
                    }
                    else
                    {
                        var data = dataContext.MaterialSpcification.FirstOrDefault(m => m.MaterialSpcificationID == materialS.MaterialSpcificationID);
                        if (data == null)
                        {
                            Msg = "没有找到要修改的数据！";
                        }
                        else
                        {
                            data.Value = materialS.Value;
                            data.MaterialSpcificationName = materialS.MaterialSpcificationName;
                            data.MaterialSpcificationCode = materialS.MaterialSpcificationCode;
                            data.Remark = materialS.Remark;
                            dataContext.SubmitChanges();
                            Msg = "修改产品规格成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
                Ret.SetArgument(error, Msg, Msg);
                return Ret;
            }
        }

        /// <summary>
        /// 删除产品规格（修改状态为-1）
        /// </summary>
        /// <param name="masID">产品规格ID</param>
        /// <returns>返回结果正确/错误</returns>
        public RetResult Delete(long masID)
        {
            string Msg = "删除产品规格失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    MaterialSpcification materialS = dataContext.MaterialSpcification.SingleOrDefault(m => m.MaterialSpcificationID == masID);
                    //判断产品规格是否存在
                    if (materialS == null)
                    {
                        Msg = "没有找到要删除的产品规格请刷新列表！";
                    }
                    else
                    {
                        materialS.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Msg = "删除产品规格成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
                Msg = "连接服务器失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        /// <summary>
        /// 获取产品规格
        /// </summary>
        /// <param name="masID">产品规格ID</param>
        /// <returns></returns>
        public MaterialSpcification GetModelByID(long masID)
        {
            MaterialSpcification materialS = new MaterialSpcification();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    materialS = dataContext.MaterialSpcification.FirstOrDefault(t => t.MaterialSpcificationID == masID);
                }
            }
            catch
            {
            }
            return materialS;
        }
    }
}
