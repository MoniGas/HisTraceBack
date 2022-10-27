namespace iagric_plant.Areas.Market.Models
{
    public class RequestModel
    {
    }

    /// <summary>
    /// 发送红包请求参数
    /// </summary>
    public class RedPacketRequestModel
    {
       /// <summary>
        /// 商户订单号格式： 商户号 + 4位年 + 2位月 + 2位日 + 10位自然日内唯一数字
        /// </summary>
        public string BillNumber { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string SendName { get; set; }
        /// <summary>
        /// 活动名称 例如 优惠反红包
        /// </summary>
        public string ActName { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 微信分配的公众账号ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 微信分配的商户ID
        /// </summary>
        public string Mch_Id { get; set; }
        /// <summary>
        /// 红包祝福语
        /// </summary>
        public string Wishing { get; set; }
        /// <summary>
        /// 接受红包的用户用户在wxappid下的openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 调用接口的机器Ip地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 红包金额，单位分
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 验证方法
        /// </summary>
        /// <returns></returns>
        public bool IsValid() //
        {
            return !string.IsNullOrWhiteSpace(BillNumber) &&
                !string.IsNullOrWhiteSpace(SendName) &&
                !string.IsNullOrWhiteSpace(ActName) &&
                !string.IsNullOrWhiteSpace(Wishing) &&
                !string.IsNullOrWhiteSpace(Remark) &&
                !string.IsNullOrWhiteSpace(AppId) &&
                !string.IsNullOrWhiteSpace(OpenId) &&
                !string.IsNullOrWhiteSpace(IpAddress) &&
                ((Amount == 0) || (Amount != 0 && Amount >= 100));
        }
    }
}