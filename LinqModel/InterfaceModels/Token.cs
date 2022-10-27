using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
	public class Token
	{
		/// <summary>
		/// 是否失效
		/// </summary>
		public bool isTokenOK { get; set; }
		/// <summary>
		/// 登录名
		/// </summary>
		public string LoginName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 登录密码
		/// </summary>
		public string LoginPassWord { get; set; }
		/// <summary>
		/// 用户ID
		/// </summary>
		public long? Enterprise_User_ID { get; set; }
		/// <summary>
		/// 企业ID
		/// </summary>
		public long? Enterprise_Info_ID { get; set; }
		/// <summary>
		/// 企业主码
		/// </summary>
		public string MainCode { get; set; }
		/// <summary>
		/// 用户类型
		/// </summary>
		public string UserType { get;set; }
		/// <summary>
		/// 经销商ID
		/// </summary>
		public long? DealerID { get; set; }
		/// <summary>
		/// 超期时间
		/// </summary>
		public DateTime ExprieTime { get; set; }
	}
}
