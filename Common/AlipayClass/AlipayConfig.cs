namespace Common.AlipayClass
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config
    {
        #region 字段
        private static string partner = "";
        private static string seller_id = "";
        private static string private_key = "";
        private static string public_key = "";
        private static string input_charset = "";
        private static string sign_type = "";
        private static string seller_email = "";
        private static string seller_name = "";
        #endregion

        static Config()
        {
            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            partner = "2088121851130455";

            //收款支付宝账号，以2088开头由16位纯数字组成的字符串
            seller_id = partner;

            //商户的私钥
            private_key = @"MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAL2fZRY5baTy9gF4Y0yhRNmyEfqymdVda+HnLz6e885YAkV1/0rvdLhnnNOc6h62THGGAEpRZGfJpije9OfiD9HIz0qdZ9Dc9PeBXjOS88CxxG6PZNygH+wlPBT3cvWg1SG6H71DSanBG9000mqx4XleWBZwdWz9WcMLK7Out8ktAgMBAAECgYBpwuJ1z9gYvS56yXPRBM3LAefHRSBKAg4u9GvEaJGhUDMMHPEkEYSvaZLt8EIgmRrv4oZ87QKsZKsZdqlwQAUDzCIi3vwFMxybaMFVo2ziJ6Xub87gLIJ4791mw1tkYJCH1M7y0JRxYUYFXPybnxfSkDyJ6lK2QTmtKwOz2rYewQJBAPuqVHyI+mY4LVA/q/mJXjH0mTxqrLOVL8pXJ79uB7uO04DoLcDknmoh221zLUwSwJDVRLnSo+cBW1Kx6TZiUFECQQDA43/aGb4+o/rLOD5vYjxP8vn2H1AjNPs+kAaEa29On1McDk4w1G2/kk0dnuXMtQ5tcWEclPTcGr5LHArmTbAdAkEAvFTXLOsFGAHqRyee9vrJtvCozAG4hBucy/s1D5izyLQ1qz4VH0j3E82Ke/m5aDiQStKOv9DWP0VQpi3lrIEeQQJAc9XriNHe4wVc7j+3lvJan/Sd0gWZ/ZqqqzaZA1r1fDTIF1gr8r0Pr3UvpHMxM88je+wT0rNNnQTedILBtO7ArQJAaIeD5DjIMavE4RCDkDFN6nJezxyvqc3oaJKIyRDBd1TpjIYmoLQKwj/6GodTpZ395bPHnSkTONzQI0VNdNWktg==";

            //支付宝的公钥，无需修改该值
            public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";

            //字符编码格式 目前支持 gbk 或 utf-8
            input_charset = "utf-8";

            //签名方式，选择项：RSA、DSA、MD5
            sign_type = "RSA";

            //卖家支付宝帐户
            seller_email = "hbgl3@gogowan.com";

            //卖家支付宝企业名
            seller_name = "河北广联信息技术有限公司";
        }

        #region 属性
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Seller_id
        {
            get { return seller_id; }
            set { seller_id = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public static string Private_key
        {
            get { return private_key.Replace(" ", ""); }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public static string Public_key
        {
            get { return public_key.Replace(" ", ""); }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return sign_type; }
        }

        /// <summary>
        /// 卖家支付宝账号
        /// </summary>
        public static string Seller_email
        {
            get { return seller_email; }
        }

        /// <summary>
        /// 卖家支付宝企业名
        /// </summary>
        public static string Seller_name
        {
            get { return seller_email; }
        }

        /// <summary>
        /// 卖家支付宝企业名
        /// </summary>
        public static string Seller_trueName
        {
            get { return seller_name; }
        }
        #endregion
    }
}
