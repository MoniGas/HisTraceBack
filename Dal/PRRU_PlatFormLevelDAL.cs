/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-15

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于平台权限管理数据层

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Argument;

namespace Dal
{
    public class PRRU_PlatFormLevelDAL:DALBase
    {
        /// <summary>
        /// 修改平台权限方法
        /// </summary>
        /// <param name="objPRRU_PlatFormLevel">数据对象</param>
        /// <returns></returns>
        public RetResult Update(LinqModel.PRRU_PlatFormLevel objPRRU_PlatFormLevel) 
        {
            string Msg = "保存失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = (from d in dataContext.PRRU_PlatFormLevel
                               where d.PRRU_PlatFormLevel_ID == objPRRU_PlatFormLevel.PRRU_PlatFormLevel_ID
                               select d).FirstOrDefault();

                    if (!string.IsNullOrEmpty(objPRRU_PlatFormLevel.Modual_ID_Array))                     
                    {
                        data.Modual_ID_Array = objPRRU_PlatFormLevel.Modual_ID_Array;
                    }
                    if (objPRRU_PlatFormLevel.adduser != null)
                    {
                        data.adduser = objPRRU_PlatFormLevel.adduser;
                    }
                    if (objPRRU_PlatFormLevel.adddate != null)
                    {
                        data.adddate = objPRRU_PlatFormLevel.adddate;
                    }
                    dataContext.SubmitChanges();
                    Msg = "保存成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch
            {
                Msg = "保存失败！";
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
        /// <summary>
        /// 查询某个平台角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinqModel.PRRU_PlatFormLevel GetModel(int id) 
        {
            LinqModel.PRRU_PlatFormLevel objPRRU_PlatFormLevel = new LinqModel.PRRU_PlatFormLevel();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    objPRRU_PlatFormLevel = (from data in dataContext.PRRU_PlatFormLevel
                                             where data.PRRU_PlatFormLevel_ID == id
                                             select data).FirstOrDefault();

                    return objPRRU_PlatFormLevel;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 查询平台所有模块
        /// </summary>
        /// <returns></returns>
        public List<LinqModel.PRRU_PlatFormLevel> GetList() 
        {
            List<LinqModel.PRRU_PlatFormLevel> objPRRU_PlatFormLevel = new List<LinqModel.PRRU_PlatFormLevel>();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    objPRRU_PlatFormLevel = (from data in dataContext.PRRU_PlatFormLevel
                                             select data).ToList();

                    return objPRRU_PlatFormLevel;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
