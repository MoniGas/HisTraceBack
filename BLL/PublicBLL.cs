/********************************************************************************
** 作者：赵慧敏
** 开发时间：2017-6-7
** 联系方式:13313318725
** 代码功能：业务层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using LinqModel;

namespace BLL
{
    /// <summary>
    /// 业务层
    /// </summary>
    public class PublicBLL
    {
        public List<YX_Style> GetStyle(long eID)
        {
            Dal.PublicDAL dal = new Dal.PublicDAL();
           return dal.GetStyle(eID);
        }
    }
}
