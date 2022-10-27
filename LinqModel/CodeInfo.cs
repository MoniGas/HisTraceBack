/********************************************************************************
** 作者： 赵慧敏v2.5版本修改
** 创始时间：2017-2-09
** 联系方式 :15031109901
** 描述：拍码追溯页面
** 版本：v2.5
** 版权：溯源 农业项目组  
*********************************************************************************/

namespace LinqModel
{
    /// <summary>
    /// 二维码拍码缓存记录数据
    /// </summary>
    public class CodeInfo
    {
        /// <summary>
        /// 二维码信息
        /// </summary>
        public Enterprise_FWCode_00 FwCode { get; set; }
        /// <summary>
        /// 设置信息
        /// </summary>
        public RequestCodeSetting CodeSeting { get; set; }
        /// <summary>
        /// 企业信息
        /// </summary>
        public Enterprise_Info Enterprise { get; set; }
        /// <summary>
        /// 二维码数据查询方式1:原始查询（查询工艺）2：查询销售批次3：根据自定义设置查询
        /// </summary>
        public int CodeType { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public long MaterialID { get; set; }
        /// <summary>
        /// 品牌编码
        /// </summary>
        public long BrandID { get; set; }
        /// <summary>
        /// 经销商编号
        /// </summary>
        public long DealerID { get; set; }
        /// <summary>
        /// 企业编号
        /// </summary>
        public long EnterpriseID { get; set; }
        /// <summary>
        /// 显示项
        /// </summary>
        public Display Display { get; set; }
        /// <summary>
        /// 二维码类型（防伪码/追溯码/防伪追溯码）
        /// </summary>
        public int RequestCodeType { get; set; }
        public string ProductDate { get; set; }
        /// <summary>
        /// 申请码信息20200206
        /// </summary>
        public RequestCode CodeRequest { get; set; }
    }

    /// <summary>
    /// 显示项
    /// </summary>
    public class Display
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public bool Brand { get; set; }
        /// <summary>
        /// 原料信息
        /// </summary>
        public bool Origin { get; set; }
        /// <summary>
        /// 作业信息
        /// </summary>
        public bool Work { get; set; }
        /// <summary>
        /// 巡检信息
        /// </summary>
        public bool Check { get; set; }
        /// <summary>
        /// 检测报告
        /// </summary>
        public bool Report { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public bool CreateDate { get; set; }
        /// <summary>
        /// 怕码次数
        /// </summary>
        public bool ScanCount { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public bool Verification { get; set; }
        /// <summary>
        /// 仓储信息
        /// </summary>
        public bool WareHouse { get; set; }
        /// <summary>
        /// 物流信息
        /// </summary>
        public bool Logistics { get; set; }
        /// <summary>
        /// 存储环境
        /// </summary>
        public bool Ambient { get; set; }
        /// <summary>
        /// 物流信息
        /// </summary>
        public bool WuLiu { get; set; }
    }
}
