using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel
{
	public class UDIAnalyseResult
	{
		public int code { get; set; }
		public string Msg { get; set; }
		public UDIAnalyseInfo data { get; set; }
	}

	public class UDIMergeResult
	{
		public int code { get; set; }
		public string Msg { get; set; }
		public UDIAnalyseInfo data { get; set; }
		public string BoxCode { get; set; }
		public List<string> BindData { get; set; }
		public string TopBoxCode { get; set; }
		public List<string> SameBindData { get; set; }
	}

	public class UDIAnalyseInfo
	{
		public UDIAnalyseInfo()
        {
			CodeTypeName = "-";
			EnterpriseName = "-";
			BusinessLicence = "-";
			MaterialName = "-";
			DI = "-";
			ProductDate = "-";
			EffectivitDate = "-";
			InvalDate = "-";
			BatchNumber = "-";
			SterilizationNumber = "-";
			SerialNumber = "-";
			Specification = "-";
			Package = "-";
			MaterialMS = "-";
        }
		/// <summary>
		/// 编码体系名称
		/// </summary>
		public string CodeTypeName { get; set; }
		/// <summary>
		/// 企业名称
		/// </summary>
		public string EnterpriseName { get; set; }
		/// <summary>
		/// 统一社会信用代码
		/// </summary>
		public string BusinessLicence { get; set; }
		/// <summary>
		/// 产品名称
		/// </summary>
		public string MaterialName { get; set; }
		/// <summary>
		/// DI信息
		/// </summary>
		public string DI { get; set; }
		/// <summary>
		/// 生产日期
		/// </summary>
		public string ProductDate { get; set; }
		/// <summary>
		/// 有效期
		/// </summary>
		public string EffectivitDate { get; set; }
		/// <summary>
		/// 失效日期
		/// </summary>
		public string InvalDate { get; set; }
		/// <summary>
		/// 生产批号
		/// </summary>
		public string BatchNumber { get; set; }
		/// <summary>
		/// 灭菌批次
		/// </summary>
		public string SterilizationNumber { get; set; }
		/// <summary>
		/// 序列号
		/// </summary>
		public string SerialNumber { get; set; }
		/// <summary>
		/// 规格型号
		/// </summary>
		public string Specification { get; set; }
		/// <summary>
		/// 包装规格
		/// </summary>
		public string Package { get; set; }
		/// <summary>
		/// 产品描述
		/// </summary>
		public string MaterialMS { get; set; }

	}

    public class UDIBindResult
    {
        public int code { get; set; }
        public string Msg { get; set; }
        public string BoxCode { get; set; }
        public List<string> data { get; set; }
		public string TopBoxCode { get; set; }
		public List<string> SameBindData { get; set; }
    }
}
