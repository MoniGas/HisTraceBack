using System;
using System.Xml.Serialization;

namespace iagric_plant.Areas.Market.Models
{
    /// <summary>
    /// 公用返回类
    /// </summary>
    public class ResponseModel
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }

    /// <summary>
    /// 基础AccessToken
    /// </summary>
    public class ResponseBaseAccessTokenModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    /// <summary>
    /// js-sdk 验证票
    /// </summary>
    public class ResponseTicketModel : ResponseModel
    {
        public string ticket { get; set; }
        public int expires_in { get; set; }
    }
    /// <summary>
    /// 获取code
    /// </summary>
    public class ResponseCodeModel : ResponseModel
    {
        public string appid { get; set; }
        public string secret { get; set; }
        public string code { get; set; }
        public string grant_type { get; set; }
    }
    /// <summary>
    /// 获取accessToken
    /// </summary>
    public class ResponseAccessTokenModel : ResponseModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }
    public class ResponseGzhUser
    {
        public string subscribe { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
    }
    /// <summary>
    /// 公众号获取Token
    /// </summary>
    public class AccToken
    {
        public string access_token { get; set; }
        public long expires_in { get; set; }
    }
    public class Ticket
    {
        public string ticket { get; set; }
        public string url { get; set; }
    }
    /// <summary>
    /// 用户返回信息
    /// </summary>
    public class ResponseUserInfoModel : ResponseModel
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string[] privilege { get; set; }
        public string unionid { get; set; }
    }

    /// <summary>
    /// 返回红包信息
    /// </summary>
    [Serializable, XmlRoot("xml"), XmlType("xml")]
    public class ResponseSendRedPacket
    {
        /// <summary>
        /// SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        public string sign { get; set; }
        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误码信息
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 结果信息描述
        /// </summary>
        public string err_code_des { get; set; }
        //以下字段在return_code和result_code都为SUCCESS的时候有返回
        /// <summary>
        /// 商户订单号（每个订单号必须唯一） 组成：mch_id+yyyymmdd+10位一天内不能重复的数字
        /// </summary>
        public string mch_billno { get; set; }
        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 用户openid
        /// </summary>
        public string wxappid { get; set; }
        /// <summary>
        /// 用户openid
        /// </summary>
        public string re_openid { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public int total_amount { get; set; }
        /// <summary>
        /// 红包订单的微信单号
        /// </summary>
        public string send_listid { get; set; }
    }

    /// <summary>
    /// 红包领取结果
    /// </summary>
    [Serializable, XmlRoot("xml"), XmlType("xml")]
    public class ResponseGetRedPacket
    {
        /// <summary>
        /// SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败 参数格式校验错误 
        /// </summary>
        public string return_msg { get; set; }
        //以下字段在return_code为SUCCESS的时候有返回
        /// <summary>
        /// 签名 
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果 SUCCESS/FAIL
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误码信息
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 结果信息描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 商户使用查询API填写的商户单号的原路返回 
        /// </summary>
        public string mch_billno { get; set; }
        /// <summary>
        /// 微信支付分配的商户号 
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 使用API发放现金红包时返回的红包单号 
        /// </summary>
        public string detail_id { get; set; }
        /// <summary>
        /// SENDING:发放中 SENT:已发放待领取 FAILED：发放失败 RECEIVED:已领取 RFUND_ING:退款中 REFUND:已退款 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// API:通过API接口发放 UPLOAD:通过上传文件方式发放 ACTIVITY:通过活动方式发放 
        /// </summary>
        public string send_type { get; set; }
        /// <summary>
        /// GROUP:裂变红包 NORMAL:普通红包 
        /// </summary>
        public string hb_type { get; set; }
        /// <summary>
        /// 红包个数
        /// </summary>
        public int total_num { get; set; }
        /// <summary>
        /// 红包总金额（单位分） 
        /// </summary>
        public int total_amount { get; set; }
        /// <summary>
        /// 发送失败原因
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 红包发送时间 
        /// </summary>
        public string send_time { get; set; }
        /// <summary>
        /// 红包退款时间 
        /// </summary>
        public string refund_time { get; set; }
        /// <summary>
        /// 红包退款金额 
        /// </summary>
        public int refund_amount { get; set; }
        /// <summary>
        /// 祝福语
        /// </summary>
        public string wishing { get; set; }
        /// <summary>
        /// 活动描述，低版本微信可见 
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 发红包的活动名称 
        /// </summary>
        public string act_name { get; set; }
        /// <summary>
        /// 裂变红包的领取列表
        /// </summary>
        [XmlArrayAttribute("hblist")] 
        public HbInfo[] hblist { get; set; }
    }

    /// <summary>
    /// 红包信息
    /// </summary>
    [Serializable, XmlRoot("hbinfo"), XmlType("hbinfo")]
    public class HbInfo
    {
        /// <summary>
        /// 领取红包的openid
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 领取金额
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 领取红包的时间 
        /// </summary>
        public string rcv_time { get; set; }
    }

    /// <summary>
    /// 返回支付信息
    /// </summary>
    [Serializable, XmlRoot("xml"), XmlType("xml")]
    public class ResponseUnifiedorder
    {
        /// <summary>
        /// SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        //以下字段在return_code和result_code都为SUCCESS的时候有返回
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        ///签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 用户错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string prepay_id { get; set; }
        /// <summary>
        /// trade_type为NATIVE时有返回，用于生成二维码，展示给用户进行扫码支付
        /// </summary>
        public string code_url { get; set; }
    }

    /// <summary>
    /// 返回支付结果
    /// </summary>
    [Serializable, XmlRoot("xml"), XmlType("xml")]
    public class ResponseOrderQuery
    {
        /// <summary>
        /// SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        //以下字段在return_code和result_code都为SUCCESS的时候有返回
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        ///签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 用户错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 用户在商户appid下的唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        public string is_subscribe { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// SUCCESS—支付成功 REFUND—转入退款 NOTPAY—未支付 CLOSED—已关闭 REVOKED—已撤销（刷卡支付） USERPAYING--用户支付中 PAYERROR--支付失败(其他原因，如银行返回失败)
        /// </summary>
        public string trade_state { get; set; }
        /// <summary>
        /// 付款银行类型
        /// </summary>
        public string bank_type { get; set; }
        /// <summary>
        /// 标价金额
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 当订单使用了免充值型优惠券后返回该参数，应结订单金额=订单金额-免充值优惠券金额。
        /// </summary>
        public string settlement_total_fee { get; set; }
        /// <summary>
        /// 标价币种
        /// </summary>
        public string fee_type { get; set; }
        /// <summary>
        /// 现金支付金额
        /// </summary>
        public string cash_fee { get; set; }
        /// <summary>
        /// 现金支付币种类型
        /// </summary>
        public string cash_fee_type { get; set; }
        /// <summary>
        /// 代金券金额
        /// </summary>
        public string coupon_fee { get; set; }
        /// <summary>
        /// 代金券使用数量
        /// </summary>
        public string coupon_count { get; set; }
        /// <summary>
        /// 代金券类型
        /// </summary>
        //public int coupon_type_$n { get; set; }
        /// <summary>
        /// 单个代金券支付金额 单个代金券支付金额, $n为下标，从0开始编号
        /// </summary>
        //public int coupon_fee_$n { get; set; }
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string time_end { get; set; }
        /// <summary>
        /// 交易状态描述
        /// </summary>
        public string trade_state_desc { get; set; }
    }
}