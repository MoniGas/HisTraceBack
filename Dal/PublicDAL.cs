/********************************************************************************
** 作者：赵慧敏
** 开发时间：2017-6-7
** 联系方式:13313318725
** 代码功能：数据层
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using System.Linq;
using LinqModel;

namespace Dal
{
    public class PublicDAL:DALBase
    {
        public List<YX_Style> GetStyle(long eID)
        {
            List<YX_Style> result = new List<YX_Style>();
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = from d in dataContext.YX_Style  select d;
                    if (eID > 0)
                    {
                        data = data.Where(d =>d.EnterpriseID == eID);
                    }
                    else
                    {
                        data = data.Where(d => d.StyleType == (int)Common.EnumText.StyleType.System);
                    }
                    result = data.ToList();
                }
                catch { }
            }
            return result;
        }
    }
}
