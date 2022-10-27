using System.Collections.Generic;

namespace LinqModel
{
    public class ActivityConceal
    {
        /// <summary>
        /// 优惠券明细编码/红包明细编码
        /// </summary>
        public long activityDetailId { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        public string activityMethod { get; set; }
        /// <summary>
        /// 活动编号
        /// </summary>
        public long activityId { get; set; }
        /// <summary>
        /// 路由编号
        /// </summary>
        public long routId { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string tableName { get; set; }
        /// <summary>
        /// 该路由下起始码编号，用,隔开，如59700,59710
        /// </summary>
        public string codeId { get; set; }
        /// <summary>
        /// 码数量
        /// </summary>
        public int codeNum { get; set; }
        /// <summary>
        /// 藏的比例
        /// </summary>
        public decimal concealRate { get; set; }
        /// <summary>
        /// 藏的数量
        /// </summary>
        public int concealNum { get; set; }
        /// <summary>
        /// 藏码ID字符串,用,隔开
        /// </summary>
        public string ConcealCodeIds { get; set; }
        /// <summary>
        /// 起始码
        /// </summary>
        public long startId { get; set; }
        /// <summary>
        /// 结束码
        /// </summary>
        public long endId { get; set; }
        /// <summary>
        /// 藏的红包id
        /// </summary>
        public List<long> redList { get; set; }
        /// <summary>
        /// 本批码路由
        /// </summary>
        public string conStr { get; set; }
        /// <summary>
        /// 码数据
        /// </summary>
        public List<long> fwCode { get; set; }
    }
}
