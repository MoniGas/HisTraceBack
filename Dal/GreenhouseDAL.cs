/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-4

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx

** 描述：

**    主要用于生产单元信息管理数据层

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Argument;
using Common.Log;
using Common;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    public class GreenhouseDAL : DALBase
    {
        /// <summary>
        /// 获取生产单元信息方法
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="ewm">二维码</param>
        /// <param name="name">生产单元名称</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns>返回生产单元集合</returns>
        public List<Greenhouses> GetList(long enterpriseID, string ewm, string name, int? pageIndex, out long totalCount)
        {
            List<Greenhouses> list = null;
            totalCount = 0;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.Greenhouses
                               where m.Enterprise_Info_ID == enterpriseID && m.state == (int)EnumFile.Status.used
                               select m;
                    if (!string.IsNullOrEmpty(ewm))
                    {
                        data = data.Where(m => m.EWM.Contains(ewm.Trim()));
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.GreenhousesName.Contains(name.Trim()));
                    }

                    totalCount = data.Count();
                    data = data.OrderByDescending(m => m.Greenhouses_ID);
                    if (pageIndex != null && pageIndex.Value != 0)
                    {
                        data = data.OrderByDescending(m => m.Greenhouses_ID).Skip((pageIndex.Value - 1) * PageSize).Take(PageSize);
                    }

                    list = data.ToList();
                    ClearLinqModel(list);

                }
                catch (Exception e)
                {
                    string errData = "GreenhouseDAL.GetList():Greenhouses表";
                    WriteLog.WriteErrorLog(errData + ":" + e.Message);
                }
            }

            return list;
        }
        /// <summary>
        /// 查询全部生产单元
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<Greenhouses> GetList(long enterpriseId)
        {
            List<Greenhouses> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    result = dataContext.Greenhouses.Where(m => m.Enterprise_Info_ID == enterpriseId && m.state == (int)Common.EnumFile.Status.used).ToList();
                    ClearLinqModel(result);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 根据ID查询生产单元信息
        /// </summary>
        /// <param name="id">生产基地ID</param>
        /// <returns></returns>
        public Greenhouses SearchData(long id)
        {
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.Greenhouses.Where(d => d.Greenhouses_ID == id && d.state == (int)EnumFile.Status.used).FirstOrDefault();

                    ClearLinqModel(data);
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 添加生产基地信息
        /// </summary>
        /// <param name="greenHouses">生产基地linq模型</param>
        /// <returns>返回操作结果</returns>
        public RetResult Add(Greenhouses greenHouses)
        {
            string Msg = "添加生产基地失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var dataList = from d in dataContext.Greenhouses
                                   where d.Greenhouses_ID != greenHouses.Greenhouses_ID
                                   && d.Enterprise_Info_ID == greenHouses.Enterprise_Info_ID
                                   && d.GreenhousesName == greenHouses.GreenhousesName
                                   select d;
                    if (dataList.Count() > 0)
                    {
                        Msg = "已存在该生产基地！";
                    }
                    else
                    {
                        int num;
                        List<Greenhouses> dList = dataContext.Greenhouses.Where(m => m.Enterprise_Info_ID == greenHouses.Enterprise_Info_ID).OrderByDescending(m => m.Greenhouses_ID).ToList();
                        if (dList == null || dList.Count == 0)
                        {
                            num = 1;
                        }
                        else
                        {
                            num = Convert.ToInt32(dList[0].EWM.Substring(dList[0].EWM.LastIndexOf('.') + 1)) + 1;
                        }
                        greenHouses.EWM = greenHouses.EWM + num.ToString();
                        dataContext.Greenhouses.InsertOnSubmit(greenHouses);
                        //dataContext.SubmitChanges();
                        //greenHouses.EWM = greenHouses.EWM + ".7." + greenHouses.Greenhouses_ID;
                        dataContext.SubmitChanges();
                        Ret.CrudCount = greenHouses.Greenhouses_ID;
                        Msg = "添加生产基地成功！";
                        error = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Msg = "连接服务器失败！";
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        /// <summary>
        /// 根据id获取生产基地信息方法
        /// </summary>
        /// <param name="id">生产基地ID</param>
        /// <returns>返回生产基地信息</returns>
        public Greenhouses GetGreenInfo(long id)
        {
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                return dataContext.Greenhouses.FirstOrDefault(m => m.Greenhouses_ID == id);
            }
        }

        /// <summary>
        /// 根据ID删除生产基地信息
        /// </summary>
        /// <param name="id">生产基地ID</param>
        /// <returns>返回操作结果</returns>
        public RetResult Del(long greenhousesId)
        {
            string Msg = "删除生产基地失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    Greenhouses greenHouses = dataContext.Greenhouses.FirstOrDefault(m => m.Greenhouses_ID == greenhousesId);

                    int count = dataContext.Greenhouses_Batch.Where(w => w.Greenhouses_ID == greenhousesId).Count();

                    if (greenHouses != null)
                    {
                        if (count != 0)
                        {
                            Msg = "该生产基地已经被使用，目前无法删除！";
                            error = CmdResultError.Other;
                        }
                        else
                        {
                            greenHouses.state = (int)EnumFile.Status.delete;
                            dataContext.SubmitChanges();

                            Msg = "删除生产基地成功！";
                            error = CmdResultError.NONE;
                        }
                    }
                    else
                    {
                        Msg = "数据不存在，请刷新重试！";
                        error = CmdResultError.EXCEPTION;
                    }
                }
            }
            catch
            {
                Msg = "删除失败！可能存在其他关联数据！";
            }
            Ret.SetArgument(error, Msg, Msg);

            return Ret;

        }

        /// <summary>
        /// 生产基地信息修改方法
        /// </summary>
        /// <param name="greenHouses"></param>
        /// <returns></returns>
        public RetResult Edit(Greenhouses greenHouses)
        {
            string Msg = "修改生产基地失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {

                    var dataewm = from d in dataContext.Greenhouses where d.EWM == greenHouses.EWM && d.Greenhouses_ID != greenHouses.Greenhouses_ID select d;
                    var data = from d in dataContext.Greenhouses where d.Greenhouses_ID != greenHouses.Greenhouses_ID && d.Enterprise_Info_ID == greenHouses.Enterprise_Info_ID && d.GreenhousesName == greenHouses.GreenhousesName select d;
                    if (data.Count() > 0)
                    {
                        Msg = "已存在该生产基地！";
                    }
                    else if (dataewm.Count() > 0)
                    {
                        Msg = "生产基地码号重复！";
                    }
                    else
                    {
                        Greenhouses oldGreenhouses = dataContext.Greenhouses.FirstOrDefault(d => d.Greenhouses_ID == greenHouses.Greenhouses_ID);
                        if (oldGreenhouses == null)
                        {
                            Msg = "没有找到要修改的生产基地信息请刷新列表！";
                        }
                        else
                        {
                            oldGreenhouses.GreenhousesName = greenHouses.GreenhousesName;
                            //oldGreenhouses.EWM = greenHouses.EWM;
                            oldGreenhouses.memo = greenHouses.memo;
                            dataContext.SubmitChanges();
                            Msg = "修改生产基地成功！";
                            error = CmdResultError.NONE;
                        }
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


        #region VB程序接口对接
        public Boolean AddProdeData(ProdeData data, out int Msg)
        {
            Msg = 2;
            Boolean result = false;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    Enterprise_Info enterprise = dataContext.Enterprise_Info.FirstOrDefault(m => m.MainCode == data.enterpriseCode);
                    if (enterprise != null)
                    {
                        long gId = dataContext.Greenhouses.FirstOrDefault(m => m.EWM == data.gId).Greenhouses_ID;
                        string[] uArray = data.userId.Split('.');
                        long uId = Convert.ToInt64(uArray[uArray.Length - 1]);
                        Greenhouses_Probe p = dataContext.Greenhouses_Probe.FirstOrDefault(m => m.enterpriseId == enterprise.Enterprise_Info_ID && m.Greenhouses_ID == gId && m.UserId == uId && m.LoweMachineAddress == data.loweMachineAddress);
                        if (p == null)
                        {
                            p = new Greenhouses_Probe();
                            p.UserId = uId;
                            p.Greenhouses_ID = gId;
                            p.AddTime = DateTime.Now;
                            p.enterpriseId = enterprise.Enterprise_Info_ID;
                            p.LoweMachineAddress = data.loweMachineAddress;
                            dataContext.Greenhouses_Probe.InsertOnSubmit(p);
                            dataContext.SubmitChanges();
                        }
                        Greenhouses_Probe_Data pd = new Greenhouses_Probe_Data();
                        pd.addTime = DateTime.Now;
                        pd.collectTime = Convert.ToDateTime(data.collectTime);
                        pd.enterpriseId = enterprise.Enterprise_Info_ID;
                        pd.Greenhouses_Probe_ID = p.Greenhouses_Probe_ID;

                        pd.key1 = data.key1;
                        pd.value1 = data.value1;
                        pd.unit1 = data.unit1;

                        pd.key2 = data.key2;
                        pd.value2 = data.value2;
                        pd.unit2 = data.unit2;

                        pd.key3 = data.key3;
                        pd.value3 = data.value3;
                        pd.unit3 = data.unit3;

                        pd.key4 = data.key4;
                        pd.value4 = data.value4;
                        pd.unit4 = data.unit4;

                        pd.key5 = data.key5;
                        pd.value5 = data.value5;
                        pd.unit5 = data.unit5;

                        pd.key6 = data.key6;
                        pd.value6 = data.value6;
                        pd.unit6 = data.unit6;

                        pd.key7 = data.key7;
                        pd.value7 = data.value7;
                        pd.unit7 = data.unit7;

                        pd.key8 = data.key8;
                        pd.value8 = data.value8;
                        pd.unit8 = data.unit8;

                        pd.key9 = data.key9;
                        pd.value9 = data.value9;
                        pd.unit9 = data.unit9;

                        pd.key10 = data.key10;
                        pd.value10 = data.value10;
                        pd.unit10 = data.unit10;

                        dataContext.Greenhouses_Probe_Data.InsertOnSubmit(pd);
                        dataContext.SubmitChanges();
                        Msg = 0;
                        result = true;
                    }
                }
                catch
                {
                    Msg = 1;
                }
            }

            return result;
        }
        public List<View_Greenhouses_Probe> GetProbeList(long eId, long uId, string title, string bDate, string eDate, int? pageIndex, int pageSize, out long totalCount)
        {
            totalCount = 0;
            List<View_Greenhouses_Probe> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Greenhouses_Probe where m.enterpriseId == eId select m;
                    if (Common.Argument.SessCokie.Get.UserType == "注册")
                    {
                        data = data.Where(m => m.UserId == uId);
                    }
                    if (!string.IsNullOrEmpty(bDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) > Convert.ToDateTime(bDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) < Convert.ToDateTime(eDate).AddDays(1));
                    }
                    if (!string.IsNullOrEmpty(title))
                    {
                        data = data.Where(m => m.GreenhousesName.Contains(title.Trim()) || m.LoweMachineAddress.Contains(title.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Greenhouses_Probe_ID).Skip(((pageIndex ?? 1) - 1) * pageSize).Take(pageSize).ToList();
                    ClearLinqModel(result);
                }
                catch
                {
                    totalCount = 0;
                }
            }
            return result;
        }
        public List<Greenhouses_Probe_Data> GetDataList(long eId, long gpId, string sDate, string eDate, int? pageIndex, int pageSize, out long totalCount)
        {
            totalCount = 0;
            List<Greenhouses_Probe_Data> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.Greenhouses_Probe_Data where m.enterpriseId == eId && m.Greenhouses_Probe_ID == gpId select m;
                    if (!string.IsNullOrEmpty(sDate))
                    {
                        data = data.Where(m => m.collectTime > Convert.ToDateTime(sDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => m.collectTime < Convert.ToDateTime(eDate).AddDays(1));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.Greenhouses_Probe_Data_Id).Skip(((pageIndex ?? 1) - 1) * pageSize).Take(pageSize).ToList();
                    ClearLinqModel(result);
                }
                catch
                {
                }
            }
            return result;
        }
        public PagedList<View_Greenhouses_Probe> GetProbeListEntity(long eId, long uId, string title, string bDate, string eDate, int? pageIndex, int pageSize)
        {
            PagedList<View_Greenhouses_Probe> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.View_Greenhouses_Probe where m.enterpriseId == eId && m.UserId == uId select m;
                    if (!string.IsNullOrEmpty(bDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) > Convert.ToDateTime(bDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => Convert.ToDateTime(m.AddTime) < Convert.ToDateTime(eDate).AddDays(1));
                    }
                    result = data.ToPagedList(pageIndex ?? 1, pageSize);
                }
                catch
                {
                }
            }
            return result;
        }
        public PagedList<Greenhouses_Probe_Data> GetDataListEntity(long eId, long gpId, string sDate, string eDate, int? pageIndex, int pageSize)
        {
            PagedList<Greenhouses_Probe_Data> result = null;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from m in dataContext.Greenhouses_Probe_Data where m.enterpriseId == eId && m.Greenhouses_Probe_ID == gpId select m;
                    if (!string.IsNullOrEmpty(sDate))
                    {
                        data = data.Where(m => m.collectTime > Convert.ToDateTime(sDate));
                    }
                    if (!string.IsNullOrEmpty(eDate))
                    {
                        data = data.Where(m => m.collectTime < Convert.ToDateTime(eDate).AddDays(1));
                    }
                    result = data.ToPagedList(pageIndex ?? 1, pageSize);
                }
                catch
                {
                }
            }
            return result;
        }
        #endregion
    }
}
