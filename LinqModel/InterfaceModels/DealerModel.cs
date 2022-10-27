using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
    /// <summary>
    /// 经销商模型接口用
    /// </summary>
    public class DealerModel
    {
        /// <summary>
        /// 经销商姓名
        /// </summary>
        public string dealerName { get; set; }
        /// <summary>
        /// 经销商编码
        /// </summary>
        public string dealerCode { get; set; }
        /// <summary>
        /// 经销商地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string linker { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string linkTel { get; set; }
        /// <summary>
        /// 默认用户
        /// </summary>
        public string defaultUserName { get; set; }
        /// <summary>
        /// 默认用户密码
        /// </summary>
        public string defaultUserPWD { get; set; }
    }
}
