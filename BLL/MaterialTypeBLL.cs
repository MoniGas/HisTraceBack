/********************************************************************************

** 作者： 张翠霞

** 创始时间：2016-10-19

** 修改人：xxx

** 修改时间：xxxx-xx-xx  

** 描述：主要用于产品类型维护的业务层

*********************************************************************************/

using System.Collections.Generic;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    /// <summary>
    /// 产品类型维护的业务层
    /// </summary>
    public class MaterialTypeBLL
    {
        /// <summary>
        /// 获取产品类别列表
        /// </summary>
        /// <returns></returns>
        public BaseResultList GetList()
        {
            List<MaterialType> model = new List<MaterialType>();
            MaterialTypeDAL dal = new MaterialTypeDAL();
            model = dal.GetList();
            BaseResultList result = ToJson.NewListToJson(model, 1, 20, 100, "");
            return result;
        }
    }
}
