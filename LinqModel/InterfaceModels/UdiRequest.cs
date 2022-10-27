using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
	public class UdiRequest
	{
		/// <summary>
		/// 页码
		/// </summary>
		public int? pageNumber { get; set; }
		/// <summary>
		/// 每页显示的条数
		/// </summary>
		public int? pageSize { get; set; }
	}

	#region 同步UDI-DI接口

	#region 同步UDI-DI接口请求参数
	/// <summary>
	/// 同步UDI-DI接口请求参数
	/// </summary>
	public class DILstRequestParam : UdiRequest
	{
		/// <summary>
		/// 产品名称
		/// </summary>
		public string productName { get; set; }
		/// <summary>
		/// 品类编码
		/// </summary>
		public string categoryCode { get; set; }
		/// <summary>
		/// 包装规格  0-9
		/// </summary>
		public string specIfication { get; set; }
		/// <summary>
		/// 产品规格型号
		/// </summary>
		public string specModel { get; set; }
		
	}
    #endregion

	#region 同步UDI-DI接口返回信息
	public class DILstResult
	{
		public int? totalPageCount { get; set; }
		public List<DILst> data { get; set; }
	}
	public class DILst
	{
		/// <summary>
		/// 产品名称
		/// </summary>
		public string productName { get; set; }
		/// <summary>
		/// 品类编码
		/// </summary>
		public string categoryCode { get; set; }
		/// <summary>
		/// 包装规格名称
		/// </summary>
		public string specIficationName { get; set; }
		/// <summary>
		/// 包装规格值  0-9
		/// </summary>
		public string specIficationValue { get; set; }
		/// <summary>
		/// 产品规格型号
		/// </summary>
		public string modelName { get; set; }
		/// <summary>
		/// UDI-DI编码
		/// </summary>
		public string completeCode { get; set; }
		/// <summary>
		/// GSIDI编码
		/// </summary>
		public string GSIDI { get; set; }
		/// <summary>
		/// 医保码
		/// </summary>
		public string HisCode { get; set; }
	}
	#endregion

	#endregion

	#region 同步UDI-PI接口
	#region 同步UDI-PI接口请求参数
	public class PILstRequestParam : UdiRequest
	{
		/// <summary>
		/// 产品名称
		/// </summary>
		public string productName { get; set; }
		/// <summary>
		/// 品类编码
		/// </summary>
		public string categoryCode { get; set; }
		/// <summary>
		/// 包装规格  0-9
		/// </summary>
		public string specIfication { get; set; }
		/// <summary>
		/// 产品规格型号
		/// </summary>
		public string specModel { get; set; }
		/// <summary>
		/// UDI-DI编码
		/// </summary>
		public string completeCode { get; set; }
		/// <summary>
		/// 开始时间
		/// </summary>
		public string startDate { get; set; }
		/// <summary>
		/// 结束时间
		/// </summary>
		public string endDate { get; set; }
	}
	#endregion

	#region 同步UDI-PI接口返回信息
	public class PILstResult
	{
		public int? totalPageCount { get; set; }
		public List<PILst> data { get; set; }
	}
	public class PILst
	{
		/// <summary>
		/// 产品名称
		/// </summary>
		public string productName { get; set; }
		/// <summary>
		/// 规格型号
		/// </summary>
		public string MaterialXH { get; set; }
		/// <summary>
		/// 批量申请生成批次
		/// </summary>
		public string batchNo { get; set; }
		/// <summary>
		/// UDI-DI编码
		/// </summary>
		public string completeCode { get; set; }
		/// <summary>
		/// 码数量
		/// </summary>
		public int codeNum { get; set; }
	}
	#endregion

	#endregion 

	#region 同步UDI-PI明细（完整码内容）接口

	#region 同步UDI-PI明细（完整码内容）接口请求信息
	public class PIDetailRequestParam : UdiRequest
	{
		/// <summary>
		/// 产品名称
		/// </summary>
		public string productName { get; set; }
		/// <summary>
		/// UDI-DI编码
		/// </summary>
		public string completeCode { get; set; }
		/// <summary>
		/// 批量申请生成批次
		/// </summary>
		public string batchNo { get; set; }
	}
	#endregion

	#region 同步UDI-PI明细（完整码内容）接口返回信息
	public class PIDetailResult
	{
		public int? totalPageCount { get; set; }
		public List<PIDetail> data { get; set; }
	}
	public class PIDetail
	{
		/// <summary>
		/// UDI-DI编码
		/// </summary>
		public string completeCode { get; set; }
		/// <summary>
		/// 批量申请生成批次
		/// </summary>
		public string batchNo { get;set;}
		/// <summary>
		/// 码内容
		/// </summary>
		public List<string> codeData { get; set; }
	}
	#endregion

	#endregion

	#region 生成UDI-DI接口
	
	#region 生成UDI-DI接口请求参数
	public class SaveDIRequestParam
		{
			/// <summary>
			/// 产品名称
			/// </summary>
			public string productName { get; set; }
			/// <summary>
			/// 品类编码
			/// </summary>
			public string categoryCode { get; set; }
			/// <summary>
			/// 包装规格
			/// </summary>
			public string specIfication { get; set; }
			/// <summary>
			/// 产品规格型号
			/// </summary>
			public string specModel { get; set; }
		
		}
	#endregion

	#region 
	public class MedicalRegMsg
	{
		public string organunit_idcode { get; set; }
		public long? category_reg_id { get; set; }
		public int result_code { get; set; }
		public string result_msg { get; set; }
	}
	#endregion

	#endregion

}
