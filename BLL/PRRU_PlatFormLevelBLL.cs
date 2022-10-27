/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-15

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于平台权限管理业务层

*********************************************************************************/

using System.Collections.Generic;
using Common.Argument;
using Dal;

namespace BLL
{
    public class PRRU_PlatFormLevelBLL
    {
        /// <summary>
        /// 根据ID获取该平台的权限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetModel(int id) 
        {
            PRRU_PlatFormLevelDAL objPRRU_PlatFormLevelDAL = new PRRU_PlatFormLevelDAL();

            LinqModel.PRRU_PlatFormLevel data = objPRRU_PlatFormLevelDAL.GetModel(id);

            return ToJson.ModelToJson(data, 1, "");
        }
        /// <summary>
        /// 更新平台权限信息
        /// </summary>
        /// <param name="objPRRU_PlatFormLevel"></param>
        /// <returns></returns>
        public string Updata(LinqModel.PRRU_PlatFormLevel objPRRU_PlatFormLevel) 
        {
            PRRU_PlatFormLevelDAL objPRRU_PlatFormLevelDAL = new PRRU_PlatFormLevelDAL();

            RetResult objResult = objPRRU_PlatFormLevelDAL.Update(objPRRU_PlatFormLevel);

            return ToJson.ResultToJson(objResult);
        }
        /// <summary>
        /// 获取所有可选模块信息
        /// </summary>
        /// <returns></returns>
        public string GetModelList() 
        {
            PRRU_ModualDAL objPRRU_ModualDAL = new PRRU_ModualDAL();

            List<LinqModel.PRRU_Modual> listData = objPRRU_ModualDAL.GetModelList();

            return ToJson.ListToJson(listData, 1, 1000, listData.Count, "");
        }

        /// <summary>
        /// 获取平台角色信息
        /// </summary>
        /// <returns></returns>
        public string GetList() 
        {
            PRRU_PlatFormLevelDAL objPRRU_PlatFormLevelDAL = new PRRU_PlatFormLevelDAL();

            List<LinqModel.PRRU_PlatFormLevel> dataList = objPRRU_PlatFormLevelDAL.GetList();
            string result = "数据库连接失败！";
            if (dataList != null)
            {
                result = "查询成功！";
            }

            return ToJson.ListToJson(dataList, 1, 1000, dataList.Count, result);
        }
    }
}
