/********************************************************************************

** 作者： 李子巍

** 创始时间：2015-12-09

** 联系方式 :13313318725

** 描述：主要用于产品转商品业务逻辑

** 版本：v1.0

** 版权：研一 农业项目组

*********************************************************************************/

using System.Collections.Generic;
using LinqModel;
using Common.Argument;

namespace BLL
{
    public class MaterialPropertyBLL
    {
        /// <summary>
        /// 获取商品属性列表
        /// </summary>
        /// <returns>商品属性列表</returns>
        public BaseResultList GetMaterialPropertyList()
        {
            List<Material_Property> MaterialPropertyResult = new Dal.MaterialPropertyDAL().GetMaterialPropertyList();

            return ToJson.NewListToJson(MaterialPropertyResult, 1, int.MaxValue, MaterialPropertyResult.Count, "");
        }

        /// <summary>
        /// 获取商品属性列表
        /// </summary>
        /// <param name="MaterialSpecId">产品规格</param>
        /// <returns>商品属性列表</returns>
        public List<View_Material_Property> GetMaterialPropertyList(long MaterialSpecId)
        {
            List<View_Material_Property> MaterialPropertyResult = new Dal.MaterialPropertyDAL().GetMaterialPropertyList(MaterialSpecId);
            return MaterialPropertyResult;
        }
    }
}
