/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-10-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx  

** 描述：主要用于产品类型维护的数据库操作

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using LinqModel;
using Common.Log;

namespace Dal
{
    /// <summary>
    /// 产品类型维护的数据库操作
    /// </summary>
    public class MaterialTypeDAL : DALBase
    {
        /// <summary>
        /// 获取产品类别列表
        /// </summary>
        /// <returns></returns>
        public List<MaterialType> GetList()
        {
            List<MaterialType> result = new List<MaterialType>();
            using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = (from m in dataContext.MaterialType select m).ToList();
                    result = data.OrderBy(m => m.ID).ToList();
                    ClearLinqModel(result);
                }
                catch (Exception ex)
                {
                    string errData = "MaterialTypeDAL.GetList():MaterialType表";
                    WriteLog.WriteErrorLog(errData + ":" + ex.Message);
                }
            }
            return result;
        }
    }
}
