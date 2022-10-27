using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel
{
    public class EnterpriseInfoRequest
    {
        /// <summary>
        /// 省ID
        /// </summary>		
        public long Dictionary_AddressSheng_ID { get; set; }
        /// <summary>
        /// 市ID
        /// </summary>		
        public long Dictionary_AddressShi_ID { get; set; }
        /// <summary>
        /// 区县ID
        /// </summary>		
        public long Dictionary_AddressQu_ID { get; set; }
        /// <summary>
        /// 一级行业ID
        /// </summary>		
        public long Trade_ID { get; set; }
        /// <summary>
        /// 二级行业ID
        /// </summary>		
        public long Etrade_ID { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>		
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 单位地址
        /// </summary>		
        public string Address { get; set; }
        /// <summary>
        /// 企业注册OID码
        /// </summary>		
        public string MainCode { get; set; }
        /// <summary>
        /// 单位所属性质
        /// </summary>
        public string Dictionary_UnitType_ID { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string LinkPhone { get; set; }
        /// <summary>
        /// 企业邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPassWord { get; set; }
        /// <summary>
        ///统一社会信用代码
        /// </summary>
        public string BusinessLicence { get; set; }
    }
}
