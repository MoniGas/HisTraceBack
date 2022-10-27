using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
    public class CheckCodeRequest
    {
        //二维码内容
        public string codeValue { get; set; }
    }
    public class CheckDealer
    {
        //所属出库单号
        public string outNO { get; set; }
        //经销商ID
        public long dealerID { get; set; }
        //经销商名称
        public string dealerName { get; set; }
        //经销商地址
        public string dealerAddress { get; set; }
        //产品ID
        public long materialID { get; set; }
        //产品名称
        public string materialName { get; set; }
    }

    public class AddCheckRequest
    {
        //二维码内容
        public string codeValue { get; set; }
        //所属出库单号
        public string outNO { get; set; }
        //经销商ID
        public long dealerID { get; set; }
        //经销商名称
        public string dealerName { get; set; }
        //检测结果0 ：异常；1：正常
        public int checkResult { get; set; }
        //经销商名称
        public string checkdealerName { get; set; }
        //产品ID
        public long materialID { get; set; }
        //产品名称
        public string materialName { get; set; }
        //经销商地址
        public string dealerAddress { get; set; }

    }

    public class CheckRecordRequest
    {
        /// <summary>
        /// 类型 0-无选择，1-近2日，2-近7日，3-近1月，4-近3月
        /// 1：近3日，2：近7日，3：近1月，4：近3月，5：自定义范围
        /// </summary>
        public int queryTimeType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string startDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endDate { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public int pageNumber { get; set; }
        /// <summary>
        /// 每页显示的条数（非必填，默认值20）
        /// </summary>
        public int pageSize { get; set; }
    }

    /// <summary>
    /// 稽查返回结果信息
    /// </summary>
    public class CheckRecordResult : InterfaceResult
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        public List<MarketRecord> list;

        /// <summary>
        /// 总页数
        /// </summary>
        public int zys;
    }
    public class MarketRecord
    {
        /// <summary>
        /// ProductCode
        /// </summary>		
        public string ProductCode { get; set; }
        /// <summary>
        /// 稽查时间
        /// </summary>		
        public string CheckTime { get; set; }
        /// <summary>
        /// 经销商ID
        /// </summary>		
        public long DealerId { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>		
        public string DealerName { get; set; }
        /// <summary>
        /// 扫码稽查的经销商
        /// </summary>		
        public string XDealerName { get; set; }
        /// <summary>
        /// 操作人ID
        /// </summary>		
        public long UserId { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>		
        public string UserName { get; set; }
        /// <summary>
        /// 状态0 ：异常；1：正常
        /// </summary>		
        public int Status { get; set; }
    }
}
