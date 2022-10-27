using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel
{
    public class RequestBase
    {
        /// <summary>
        /// UDI APPID
        /// </summary>
        public string appId { get; set; }
        /// <summary>
        /// UDI appSecret
        /// </summary>
        public string appSecret { get; set; }
        /// <summary>
        /// UDI TYSHXYDM(统一社会信用代码)
        /// </summary>
        public string TYSHXYDM { get; set; }
    }
    public class TokenInfo : RetuenResult
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string accessToken { get; set; }
        /// <summary>
        /// 效期时长
        /// </summary>
        public string expiresIn { get; set; }
        /// <summary>
        /// 当前时间, IsoDateTime
        /// </summary>
        public string currentTime { get; set; }
        /// <summary>
        /// 当天剩余访问次数
        /// </summary>
        public string todayRemainVisitCount { get; set; }
        /// <summary>
        /// 效期时长时间
        /// </summary>
        public string expiresInDateTime { get; set; }
        /// <summary>
        /// 授权类型：1河北广联授权；2二维码研究院授权；3企业授权
        /// </summary>
        public int LicenseType { get; set; }
    }

    public class TokenResult : RetuenResult
    {
        public int expiresIn { get; set; }
        public string currentTime { get; set; }
        public string todayRemainVisitCount { get; set; }
        public string accessToken { get; set; }
    }
    public class RetuenResult
    {
		public RetuenResult()
		{
			returnCode = -1;
			returnMsg = "";
		}
        /// <summary>
        /// 接口返回错误码
        /// </summary>
        public int returnCode { get; set; }
        /// <summary>
        /// 接口返回状态码
        /// </summary>
        public String returnMsg { get; set; }

    }
}
