using System;

namespace Common.Argument
{
    /// <summary>
    /// 登录成功初始化信息数据对象
    /// </summary>
    /// <summary>
    /// 登录成功初始化信息数据对象
    /// </summary>
    [Serializable]
    public class OrderLoginInfo
    {
        /// <summary>
        /// 消费者ID
        /// </summary>
        public long Order_Consumers_ID { get; set; }
        /// <summary>
        /// UserID
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 登录的手机号
        /// </summary>
        public string LinkPhone { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 消费者基本信息表ID
        /// </summary>
        public long Order_Consumers_Address_ID { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string UserPhoto { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        //省市区的ID
        public string ProvinceID { get; set; }
        public string CityID { get; set; }
        public string AreaID { get; set; }
    }
}
