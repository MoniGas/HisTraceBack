using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
    public class PrivateLoginResponse
    {
        public long EnterPriseID { get; set; }
        public String LoginName { get; set; }
        public String UserName { get; set; }
        public String LoginPwd { get; set; }
        public String EnterpriseName { get; set; }
        public String MainCode { get; set; }
        public long UserID { get; set; }
        public String LicenseEndDate { get; set; }
        /// <summary>
        /// 1:简单版；2：高级版；3：标准版
        /// </summary>
        public int IsSimple { get; set; }
        /// <summary>
        /// 统一社会代码
        /// </summary>
        public String BusinessLicence { get; set; }
        /// <summary>
        /// 是否子账号1：主账号；2子账号
        /// </summary>
        public String IsSubUser { get; set; }
        /// <summary>
        /// DI列表
        /// </summary>
        public List<String> DI { get; set; }
        /// <summary>
        /// 是否过期：0正常；1过期
        /// </summary>
        public int IsExpired { get; set; }
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
        /// 单位地址
        /// </summary>		
        public String Address { get; set; }
        /// <summary>
        /// 单位所属性质
        /// </summary>
        public String Dictionary_UnitType_ID { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public String LinkMan { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public String LinkPhone { get; set; }
        /// <summary>
        /// 企业邮箱
        /// </summary>
        public String Email { get; set; }
        public String Token { get; set; }
        public String TokenCode { get; set; }
        public int DICount { get; set; }
        public int PICount { get; set; }
    }
}
