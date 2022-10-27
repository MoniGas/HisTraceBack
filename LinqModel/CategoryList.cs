/********************************************************************************
** 添加： 李子巍
** 创始时间：2016-07-13
** 联系方式 :13313318725
** 描述：主要用于初始参数列表
*********************************************************************************/

namespace MeatTrace.LinqModel
{
    /// <summary>
    /// 主要用于保存品类列表
    /// </summary>
    public class CategoryList
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string CategoryID { get; set; }
        /// <summary>
        /// 品类码号
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public string PrentID { get; set; }
        /// <summary>
        /// 品类名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 品类级别
        /// </summary>
        public int CategoryLevel { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public int CodeUseID { get; set; }
    }
}
