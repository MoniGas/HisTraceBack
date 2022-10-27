using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    //by张翠霞
    public class Material_SpecDAL : DALBase
    {
        /// <summary>
        /// 查询产品规格列表
        /// </summary>
        /// <param name="enterpriseId">企业ID</param>
        /// <param name="materialName">产品名称</param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<View_Material_Spec> GetList(long enterpriseId, string materialName, int pageIndex, out long totalCount)
        {
            totalCount = 0;
            List<View_Material_Spec> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.View_Material_Spec.Where(m => m.Enterprise_Info_ID == enterpriseId && m.Status == (int)Common.EnumFile.Status.used);
                    if (!string.IsNullOrEmpty(materialName))
                    {
                        data = data.Where(m => m.MaterialName.Contains(materialName.Trim()) || m.MaterialFullName.Contains(materialName.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.ID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "Material_SpecDAL.GetList()";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// 添加产品规格
        /// </summary>
        /// <param name="maSpec"></param>
        /// <returns></returns>
        public RetResult Add(Material_Spec maSpec, string Propertys, string Condition)
        {
            string Msg = "新商品上架失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.Material_Spec.FirstOrDefault(m => m.Material_ID == maSpec.Material_ID && m.MaterialSpecification == maSpec.MaterialSpecification && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "已存该商品！";
                    }
                    else
                    {
                        dataContext.Material_Spec.InsertOnSubmit(maSpec);
                        dataContext.SubmitChanges();

                        if (!string.IsNullOrEmpty(Propertys) && Propertys != "0")
                        {
                            Material_Spec_Property MaterialSpecProperty = new Material_Spec_Property();
                            MaterialSpecProperty.Material_Spec_ID = maSpec.ID;
                            MaterialSpecProperty.Material_Property_ID = Convert.ToInt32(Propertys);
                            MaterialSpecProperty.Status = (int)Common.EnumFile.Status.used;
                            if (!string.IsNullOrEmpty(Condition))
                                MaterialSpecProperty.Condition = "," + Condition;
                            dataContext.Material_Spec_Property.InsertOnSubmit(MaterialSpecProperty);
                            dataContext.SubmitChanges();
                        }

                        Msg = "新商品上架成功！";
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
        /// <param name="maSpec"></param>
        /// <returns></returns>
        public RetResult Edit(Material_Spec maSpec, string Propertys, string Condition)
        {
            string Msg = "修改商品失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var model = from m in dataContext.Material_Spec where m.Material_ID == maSpec.Material_ID && m.ID != maSpec.ID && m.MaterialSpecification == maSpec.MaterialSpecification && m.Status == (int)Common.EnumFile.Status.used select m;
                    if (model.Count() > 0)
                    {
                        Msg = "已存该商品！";
                    }
                    else
                    {
                        var data = dataContext.Material_Spec.FirstOrDefault(m => m.ID == maSpec.ID);
                        if (data == null)
                        {
                            Msg = "没有找到要修改的商品！";
                        }
                        else
                        {
                            data.Material_ID = maSpec.Material_ID;
                            data.MaterialSpecification = maSpec.MaterialSpecification;
                            data.Price = maSpec.Price;
                            data.ExpressPrice = maSpec.ExpressPrice;
                            data.lastdate = maSpec.lastdate;
                            data.lastuser = maSpec.lastuser;
                            dataContext.Material_Spec_Property.DeleteAllOnSubmit(
                                dataContext.Material_Spec_Property.Where(m => m.Material_Spec_ID == maSpec.ID)
                            );
                            dataContext.SubmitChanges();

                            if (!string.IsNullOrEmpty(Propertys) && Propertys != "0")
                            {
                                Material_Spec_Property MaterialSpecProperty = new Material_Spec_Property();
                                MaterialSpecProperty.Material_Spec_ID = maSpec.ID;
                                MaterialSpecProperty.Material_Property_ID = Convert.ToInt32(Propertys);
                                MaterialSpecProperty.Status = (int)Common.EnumFile.Status.used;
                                MaterialSpecProperty.Condition = null;
                                if (!string.IsNullOrEmpty(Condition))
                                    MaterialSpecProperty.Condition = "," + Condition;
                                dataContext.Material_Spec_Property.InsertOnSubmit(MaterialSpecProperty);
                                dataContext.SubmitChanges();
                            }
                            Msg = "修改商品成功！";
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
        ///删除产品规格（状态修改0正常；1删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RetResult Delete(long id)
        {
            string Msg = "商品下架失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    Material_Spec maSpec = dataContext.Material_Spec.SingleOrDefault(m => m.ID == id);

                    if (maSpec == null)
                    {
                        Msg = "没有找到要下架的商品请刷新列表！";
                    }
                    else
                    {
                        maSpec.Status = (int)Common.EnumFile.Status.delete;
                        //dataContext.Material_Spec.DeleteOnSubmit(maSpec);
                        dataContext.SubmitChanges();
                        Msg = "商品下架成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch
            {
                //Msg = "删除失败，请首先删除已知关联的其他数据";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public Material_Spec GetmaSpecByID(long id)
        {
            Material_Spec maSpec = new Material_Spec();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    maSpec = dataContext.Material_Spec.FirstOrDefault(t => t.ID == id);
                    ClearLinqModel(maSpec);
                }
            }
            catch
            {
            }
            return maSpec;
        }
    }
}
