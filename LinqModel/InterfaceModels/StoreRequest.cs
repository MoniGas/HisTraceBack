using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
	public class StoreRequest
	{
		
	}

	/// <summary>
	/// 获取企业仓库接口请求参数
	/// </summary>
	public class StoreRequestParam 
	{
		/// <summary>
		/// 仓库名称
		/// </summary>
		public string storeName { get; set; }
		/// <summary>
		/// 仓库状态
		/// </summary>
		public int? storeStatus { get; set; }
		/// <summary>
		/// 页码
		/// </summary>
		public int? pageNumber { get; set; }
		/// <summary>
		/// 每页显示的条数
		/// </summary>
		public int? pageSize { get; set; }
	}

	public class StoreResult 
	{
		public int? totalPageCount { get; set; }
		public List<StoreLst> data { get; set; }
	}

	public class StoreLst
	{
		public long storeID { get; set; }
		public string storeName { get; set; }
	}
}
