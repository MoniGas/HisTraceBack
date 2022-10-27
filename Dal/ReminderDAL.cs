/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于温馨提醒数据层

*********************************************************************************/
using System;
using System.Linq;
using Common;
using Common.Argument;

namespace Dal
{
    public class ReminderDAL:DALBase
    {
        /// <summary>
        /// 获取该企业是否有产品信息
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public RetResult GetMaterial(long id)
        {
            CmdResultError error = CmdResultError.Other;

            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    int dataCount = (from data in dataContext.Material
                                     where data.Enterprise_Info_ID == id && data.Status == (int)EnumFile.Status.used
                                     select data).Count();

                    if (dataCount == 0)
                    {
                        error = CmdResultError.Other;
                    }
                    else if (dataCount > 0)
                    {
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }
            Ret.SetArgument(error, "", "");
            return Ret;
        }

        /// <summary>
        /// 该企业是否有生产环节
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public RetResult GetZuoYe(long id)
        {
            CmdResultError error = CmdResultError.Other;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    int dataCount = (from data in dataContext.Batch_ZuoYeType
                                     where data.Enterprise_Info_ID == id && data.state == (int)EnumFile.Status.used
                                     select data).Count();

                    if (dataCount > 0)
                    {
                        error = CmdResultError.NONE;
                    }
                    else 
                    {
                        error = CmdResultError.Other;
                    }
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");  

            return Ret;
        }

        /// <summary>
        /// 该企业下是否有品牌
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public RetResult GetBrand(long id) 
        {
            CmdResultError error = CmdResultError.Other;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    int dataCount = (from data in dataContext.Brand
                                     where data.Enterprise_Info_ID == id && data.Status == (int)EnumFile.Status.used
                                     select data).Count();

                    if (dataCount > 0)
                    {
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        error = CmdResultError.Other;
                    }
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");

            return Ret;
        }

        /// <summary>
        /// 该企业下是否有生产基地
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public RetResult GetGreenhouses(long id) 
        {
            CmdResultError error = CmdResultError.Other;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    int dataCount = (from data in dataContext.Greenhouses
                                     where data.Enterprise_Info_ID == id && data.state == (int)EnumFile.Status.used
                                     select data).Count();

                    if (dataCount > 0)
                    {
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        error = CmdResultError.Other;
                    }
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");

            return Ret;
        }

        /// <summary>
        /// 该企业下是否有经销商
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        public RetResult GetDealer(long id)
        {
            CmdResultError error = CmdResultError.Other;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    int dataCount = (from data in dataContext.Dealer
                                     where data.Enterprise_Info_ID == id && data.Status == (int)EnumFile.Status.used
                                     select data).Count();

                    if (dataCount > 0)
                    {
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        error = CmdResultError.Other;
                    }
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");

            return Ret;
        }
        /// <summary>
        /// 该企业码数量是否触发阀值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RetResult GetThresholdWarning(long id)
        {
            CmdResultError error = CmdResultError.Other;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    int dataCount = (from data in dataContext.Enterprise_Info
                                     where data.PRRU_PlatForm_ID != 0 && data.OverDraftCount >= 0 && data.Enterprise_Info_ID == id && (data.RequestCodeCount - data.UsedCodeCount + data.OverDraftCount) < data.Threshold
                                   select data).Count();
                    var asdfa = from data in dataContext.Enterprise_Info
                                where data.PRRU_PlatForm_ID != 0 && data.OverDraftCount >= 0 && data.Enterprise_Info_ID == id && (data.RequestCodeCount - data.UsedCodeCount + data.OverDraftCount) > data.Threshold
                                select data;

                    if (dataCount == 0)
                    {
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        error = CmdResultError.Other;
                    }
                }
            }
            catch (Exception e)
            {
                error = CmdResultError.EXCEPTION;
            }

            Ret.SetArgument(error, "", "");

            return Ret;
        }
    }
}
