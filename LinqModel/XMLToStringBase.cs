using System;
using System.Collections.Generic;

namespace LinqModel
{
    public class ObjectSession
    {
        public string MaterialSpecId
        {
            get;
            set;
        }

        public string Count
        {
            get;
            set;
        }

        public string ewm
        {
            get;
            set;
        }

        public string uppage
        {
            get;
            set;
        }
    }

    public class NavigationForMaterialGroup
    {
        private long _MaterialId;
        private string _MaterialFullName;

        public long MaterialId
        {
            get { return _MaterialId; }
            set { _MaterialId = value; }
        }

        public string MaterialFullName
        {
            get { return _MaterialFullName; }
            set { _MaterialFullName = value; }
        }
    }

    public class PageNavigationRequset
    {
        private long _Id;
        private int _ViewNum;

        public long Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public int ViewNum
        {
            get { return _ViewNum; }
            set { _ViewNum = value; }
        }
    }

    public class ToJsonImg
    {
        public string fileUrl { get; set; }
        public string fileUrls { get; set; }
        public string fileUrlp { get; set; }
        public string videoUrl { get; set; }
        public string videoUrls { get; set; }
    }
    public class ToJsonJCImg
    {
        public string jcfileUrl { get; set; }
        public string jcfileUrls { get; set; }
    }
    public class ToJsonProperty
    {
        public string pName { get; set; }
        public string pValue { get; set; }
        public string allprototype { get; set; }
    }
    public class ScanInfo
    {
        public string title { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
        public string type { get; set; }
        public string user { get; set; }
        public string time { get; set; }
        public string content { get; set; }
    }
    public partial class NewComplaint : IEquatable<NewComplaint>
    {
        public int? Count { get; set; }
        public long? Material_ID { get; set; }
        public long? Enterprise_Info_ID { get; set; }
        public string ComplaintContent { get; set; }
        public bool Equals(NewComplaint other)
        {
            return ComplaintContent.Trim() == other.ComplaintContent.Trim();
        }
    }

    public partial class MaterialShopLink
    {
        public List<ToJsonImg> videos { get; set; }
    }

    public partial class View_BatchZuoye
    {
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
    }

    public partial class View_CodeStatis
    {
        public string Stradddate { get; set; }
    }

    public partial class Dealer
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }

    public partial class Batch
    {
        public string StrBatchDate { get; set; }
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }

    public partial class BatchExt
    {
        public string StrBatchDate { get; set; }
        public string StrAddDate { get; set; }
        public string Strlastdate { get; set; }
    }

    public partial class Material
    {
        public string StrPropertyInfo { get; set; }
        public string StrMaterialImgInfo { get; set; }
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonProperty> propertys { get; set; }
        public long FirstType { get; set; }
        public long SecondType { get; set; }
        public long ThirdType { get; set; }
    }

    public partial class MaterialShopLink
    {
        public string StrVideoUrlInfo { get; set; }
        public string StrAdFileUrl { get; set; }
        public List<ToJsonImg> AdImgs { get; set; }
        public List<ToJsonImg> VideoUrls { get; set; }
    }

    public partial class View_XunJianBatchMaterial
    {
        public string StrAddDate { get; set; }
        public string Strlastdate { get; set; }
        public string StrFiles { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
    }
    public partial class ShowCompany
    {
        public string StrFiles { get; set; }
        public List<ToJsonImg> imgs { get; set; }
    }
    public partial class View_EnterpriseShow
    {
        public string StrLogo { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonJCImg> imgsgg { get; set; }
        public List<ToJsonImg> videoUrls { get; set; }
        public string StrAdUrl { get; set; }
        public string StrSSVideoUrl { get; set; }
        public string StrWXLogo { get; set; }
        public List<ToJsonImg> wxlogoimgs { get; set; }
    }
    public partial class Greenhouses
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }
    public partial class Brand
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }
    public partial class Material_Spec
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }
    public partial class RequestCode
    {
        public string StrRequestDate { get; set; }
        public string StrCreateDate { get; set; }
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }
    public partial class Batch_ZuoYeType
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }

    public partial class View_EnterprisePlatForm
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
        public string StrLogo { get; set; }
        public string StrAddTime { get; set; }
        public string sheng { get; set; }
        public string shi { get; set; }
        public string qu { get; set; }
    }

    public partial class Enterprise_Role
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }
    public partial class View_Batch
    {
        public string Strbatchadddate { get; set; }
        public List<View_BatchExt> subBatchObj { get { return null; } }
    }
    public partial class View_BatchExt
    {
        public string StrAddDate { get; set; }
    }

    public partial class Enterprise_User
    {
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
    }
    public partial class ShowUser
    {
        public string StrInfoOther { get; set; }
        public string position { get; set; }
        public string telPhone { get; set; }
        public string mail { get; set; }
        public string qq { get; set; }
        public string hometown { get; set; }
        public string location { get; set; }
        public string memo { get; set; }
        public string headimg { get; set; }
        public string StrFiles { get; set; }
        public List<ToJsonImg> imgs { get; set; }
    }
    public partial class Enterprise_FWCode_00
    {
        public string StrUseTime { get; set; }
        public string StrSalesTime { get; set; }
        public string StrCreateDate { get; set; }
        public string StrEWM_Info { get; set; }
        public string StrcodeXML { get; set; }
        public string StrValidateTime { get; set; }

    }
    public partial class Batch_JianYanJianYi
    {
        public string StrAddDate { get; set; }
        public string Strlastdate { get; set; }
        public string StrFiles { get; set; }
        public List<ToJsonImg> imgs { get; set; }
    }
    public partial class Batch_ZuoYe
    {
        public long? zuoye_typeId { get; set; }
        public long? eid { get; set; }
        public long? bid { get; set; }
        public string StrAddDate { get; set; }
        public string Strlastdate { get; set; }
        public string StrFiles { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
        public List<ToJsonProperty> users { get; set; }
        public string OperationTypeName { get; set; }
        public string TypeName { get; set; }
    }
    public partial class Batch_XunJian
    {
        public string StrAddDate { get; set; }
        public string Strlastdate { get; set; }
        public string StrFiles { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
    }
    public partial class View_ZuoYeBatchMaterial
    {
        public string StrAddDate { get; set; }
        public string Strlastdate { get; set; }
        public string StrFiles { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
    }
    public partial class View_JianYanJianYiBatchMaterial
    {
        public string StrAddDate { get; set; }
        public string StrFiles { get; set; }
    }

    public partial class View_RequestCodeAndEnterprise_Info
    {
        public string StrRequestDate { get; set; }
        public string Strlastdate { get; set; }
    }

    public partial class View_RequestBrand
    {
        public string Strlastdate { get; set; }
    }

    public partial class View_ComplaintAndType
    {
        public string StrComplaintDate { get; set; }
    }

    public partial class Greenhouses_Probe_Data
    {
        public string StrcollectTime { get; set; }
        public string StraddTime { get; set; }
    }

    public partial class Enterprise_Info
    {
        public string StrAddTime { get; set; }
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
        public string StrLogo { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public string StrRequestCount { get; set; }
        public string StrUseCount { get; set; }
        public string StrRemainCount { get; set; }
        public string StrWXLogo { get; set; }
        public List<ToJsonImg> wxlogoimgs { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string LicenseEndDate { get; set; }
        /// <summary>
        /// 打码客户端是否可用 1：可用 0：不可用
        /// </summary>
        public int SetClient { get; set; }
        /// <summary>
        /// 追溯平台是否可用 1：可用 0：不可用
        /// </summary>
        public int SetSy { get; set; }
    }
    public partial class EnterpriseShopLink
    {
        public List<ToJsonJCImg> imgsgg { get; set; }
        public List<ToJsonImg> VideoUrls { get; set; }
        public string StrAdUrl { get; set; }
        public string StrVideoUrl { get; set; }
    }
    public partial class HomeDataStatis
    {
        public string LogoUrl { get; set; }
    }
    public partial class PRRU_PlatForm
    {
        public string StrAddDate { get; set; }
        public string Strlastdate { get; set; }
    }
    public partial class Order_Consumers_Address
    {
        public string AllArea { get; set; }
    }
    [Serializable]
    public partial class View_Order_Consumers
    {
        public string AllArea { get; set; }
    }


    public partial class Order_Consumers_Address
    {
        public string sheng { get; set; }
        public string shi { get; set; }
        public string qu { get; set; }
    }

    public partial class View_Material_OnlineOrder
    {
        public string textStatus { get; set; }
    }

    public partial class SalesInformation
    {
        public string StrProductionTime { get; set; }
        public string StrSalesDate { get; set; }
        public string StrMaterialPropertyInfoXml { get; set; }
        public string StrMaterialImgInfoXml { get; set; }
        public string StrMaterialOtherXml { get; set; }
        public string StrBatchOtherXml { get; set; }
        public string StrEnterpriseBrandOtherXml { get; set; }
        public string StrRegionalBrandXml { get; set; }
        public string StrDealerOtherXml { get; set; }
        public string StrBatch_XunJianXml { get; set; }
        public string StrBatch_ZuoYeXml { get; set; }
        public string StrEnterprise_InfoOtherXml { get; set; }
    }
    public partial class View_Material_ReturnOrder
    {
        public string textStatus { get; set; }
    }
    public partial class View_MaterialSpecForMarket
    {
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonProperty> propertys { get; set; }
    }
    public partial class SetAuditCount
    {
        public string StrSetTime { get; set; }
    }
    public partial class View_SetAuditCount
    {
        public string StrSetTime { get; set; }
        public string StrViewSetTime { get; set; }
    }
    public partial class EnterpriseVerify
    {
        public string StrAddDate { get; set; }
    }
    //20161219原料移植
    public partial class Origin
    {
        public string StrOriginImgInfo { get; set; }
        public List<ToJsonImg> imgs { get; set; }
    }
    public partial class View_Origin
    {
        public string StrOriginImgInfo { get; set; }
        public List<ToJsonImg> imgs { get; set; }
    }
    /// <summary>
    /// 原材料信息
    /// </summary>
    public class ScanRawMaterial
    {
        public List<ToJsonImg> ImgUrl { get; set; }
        public string InDate { get; set; }
        public string RawName { get; set; }
        public string CheckUser { get; set; }
        public string RawMemo { get; set; }
        public List<ToJsonImg> ReportImg { get; set; }
    }
    //20161220工艺模块移植
    public class ToJsonOperation
    {
        public string opName { get; set; }
        public string opID { get; set; }
    }
    public partial class Process
    {
        public string StrOperationList { get; set; }
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
        public List<ToJsonOperation> operations { get; set; }
    }

    public partial class View_MaterialUsageRecord
    {
        public string StrCreateDate { get; set; }
    }
    public partial class View_RequestCodeSetting
    {
        public List<View_RequestCodeSetting> subBatchObj { get { return null; } }
        public string StrSetDate { get; set; }
    }
    public partial class View_Recommend
    {
        public string Code { get; set; }
    }
    public class ChartData
    {
        public string title { get; set; }
        public string value { get; set; }
    }
    #region 配置追溯码信息
    public class EnumList
    {
        public string value { get; set; }
        public string text { get; set; }
        public bool ischeck { get; set; }
        public string mubanimg { get; set; }
    }

    public class FirstData
    {
        public long requestId { get; set; }
        public long materialId { get; set; }
        public long brandId { get; set; }
        public string materialName { get; set; }
        public long remaining { get; set; }
        public bool isFirst { get; set; }
        public bool notFirst { get; set; }
        public string isSuccess { get; set; }
        public string Msg { get; set; }
        public int requestCodeType { get; set; }
        public string batchName { get; set; }
        public string zbatchName { get; set; }//主批次号
        public List<View_RequestCodeSetting> liData { get; set; }
        public string createDate { get; set; }
        public int type { get; set; }
        public string memo { get; set; }//备注信息
        public int batchPartType { get; set; }//1顺序拆分2自定义拆分
    }
    public class SecondData
    {
        public List<EnumList> liShowData { get; set; }
        public List<EnumList> liStyleData { get; set; }
        public int styleId { get; set; }
        public long materialId { get; set; }
    }

    public partial class View_RequestOrigin
    {
        public string StrAddDate { get; set; }
        public string StrInDate { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonJCImg> jcimgs { get; set; }
        public string Strlastdate { get; set; }
        public string StrFiles { get; set; }
        public string StrJCFiles { get; set; }
    }
    #endregion

    public class MyRecommend
    {
        public string name { get; set; }
        public string code { get; set; }
        public string type { get; set; }
    }

    /// <summary>
    /// 产品信息
    /// </summary>
    public class ScanMaterial
    {
        public List<ToJsonImg> picUrl { get; set; }
        public string MaterialName { get; set; }
        public string ShortMemo { get; set; }
        public string BrandName { get; set; }
        public string BrandImg { get; set; }
        /// <summary>
        /// 口味
        /// </summary>
        public string Taste { get; set; }
        public int ScanCount { get; set; }
        public string ShortCode { get; set; }
        /// <summary>
        /// 产品参数
        /// </summary>
        public List<ToJsonProperty> MaterialInfo { get; set; }
        /// <summary>
        /// 吃法视频
        /// </summary>
        public string Memo { get; set; }
        public ToJsonImg VideoUrl { get; set; }
        public string MaterialAliasName { get; set; }
        /// <summary>
        /// 淘宝链接
        /// </summary>
        public string TaoBaoLink { get; set; }
        public string TianMaoLink { get; set; }
        public string JingDongLink { get; set; }
        public string WeiDianLink { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string MaterialPlace { get; set; }
        /// <summary>
        /// 广告图片
        /// </summary>
        public ToJsonImg AdUrl { get; set; }
        /// <summary>
        /// 产品规格
        /// </summary>
        public string Speciton { get; set; }

        /// <summary>
        /// 生成日期
        /// </summary>
        public string ProductDate { get; set; }

        public List<ToJsonImg> AllVideoUrl { get; set; }

        public string ShelfLife { get; set; }
        //产品在线购买的链接2018-09-13
        public string tbURL { get; set; }
        /// <summary>
        /// 生产批号20200206
        /// </summary>
        public string ShengChanPH { get; set; }
        /// <summary>
        /// 有效日期20200206
        /// </summary>
        public string YouXiaoDate { get; set; }
        /// <summary>
        /// 失效日期20200206
        /// </summary>
        public string ShiXiaoDate { get; set; }
        /// <summary>
        /// 生产日期20200207
        /// </summary>
        public string ShengChanDate { get; set; }
        /// <summary>
        /// 灭菌批号20200207
        /// </summary>
        public string MieJunNo { get; set; }
        /// <summary>
        /// 序列号20200207
        /// </summary>
        public string XuLieNo { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public bool IsShengChanDate { get; set; }
    }

    /// <summary>
    /// 班组信息
    /// </summary>
    public class ScanSubstation
    {
        public string SubstationName { get; set; }
        public List<SubstationSingle> liSubstation { get; set; }
    }

    /// <summary>
    /// 班组
    /// </summary>
    public class SubstationSingle
    {
        /// <summary>
        /// 班组名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 人员姓名
        /// </summary>
        public List<string> PersonName { get; set; }

        /// <summary>
        /// 生成流程id
        /// </summary>
        public long BatchZyId { get; set; }
    }

    /// <summary>
    /// 企业商城
    /// </summary>
    public class ShopInfo
    {
        /// <summary>
        /// 企业热线
        /// </summary>
        public string HotLine { get; set; }
        /// <summary>
        /// 企业官网
        /// </summary>
        public string EnterpriseUrl { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 企业logo
        /// </summary>
        public string LogoUrl { get; set; }
    }

    /// <summary>
    /// 产品描述
    /// </summary>
    public class ScanMaterialMemo
    {
        public string Memo { get; set; }
        public ToJsonImg VideoUrl { get; set; }
    }

    /// <summary>
    /// 仓储信息
    /// </summary>
    public class ScanWareHouseInfo
    {
        public string WarehouseName { get; set; }
        public string WarehouseNum { get; set; }
        public string HanderUser { get; set; }
        public string Temperature { get; set; }
        public string InData { get; set; }
        public string OutData { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 物流信息
    /// </summary>
    public class ScanLogistics
    {
        public string LogisticsNum { get; set; }
        public string CarNum { get; set; }
        public string StartAddress { get; set; }
        public string StratDate { get; set; }
        public string EndAddress { get; set; }
        public string EndDate { get; set; }
        public string CarAmbient { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
        public string SellDate { get; set; }
        public string Dealer { get; set; }
    }

    /// <summary>
    /// 企业信息
    /// </summary>
    public class ScanEnterprise
    {
        public string EnterpriseName { get; set; }
        public string License { get; set; }
        public string TelPhone { get; set; }
        public string Address { get; set; }
    }

    /// <summary>
    /// 存储环境
    /// </summary>
    public partial class SetAmbient
    {
        public string StrInDate { get; set; }
        public string StrOutDate { get; set; }
        public string StrAddDate { get; set; }
    }

    public partial class View_Material
    {
        public string StrPropertyInfo { get; set; }
        public string StrMaterialImgInfo { get; set; }
        public string StrAdUrl { get; set; }
        public string StrVideoUrl { get; set; }
        public string Stradddate { get; set; }
        public string Strlastdate { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
        public List<ToJsonImg> Adimgs { get; set; }
        public List<ToJsonImg> videoUrls { get; set; }
        public List<ToJsonProperty> propertys { get; set; }
        public long FirstType { get; set; }
        public long SecondType { get; set; }
        public long ThirdType { get; set; }
        public string StrAddDate { get; set; }
    }

    /// <summary>
    /// 物流
    /// </summary>
    public partial class SetLogistics
    {
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }
        public string StrAddDate { get; set; }
    }

    public partial class View_OutStorageStatis
    {
        public string StrOutStorageDate { get; set; }
        public string DeviceName { get; set; }
    }

    public partial class View_IntStorageStatis
    {
        public string StrIntStorageDate { get; set; }
        public string DeviceName { get; set; }
    }

    public partial class View_inventory
    {
        public string DeviceName { get; set; }
    }

    public partial class RequestCodeSetting
    {
        public string StrSetDate { get; set; }
        public string StrProductionDate { get; set; }
    }
    public partial class ActiveEwmRecord
    {
        public string StrUploadDate { get; set; }
        public string StrProductionDate { get; set; }
        public string StrAddDate { get; set; }
    }
    public partial class MaterialExportExcelRecord
    {
        public string StrAddDate { get; set; }
    }

    /// <summary>
    /// Excel图片上传表
    /// </summary>
    public partial class Uploads
    {
        public string StrImgInfo { get; set; }
        public List<ToJsonImg> imgs { get; set; }
        public List<ToJsonImg> videos { get; set; }
    }

    public partial class YX_AcivityDetail
    {
        public double? TotalNum { get; set; }
    }

    public class NewCompanyIDCode
    {
        public DateTime? BuyDate { get; set; }
        public long CompanyIDcodeID { get; set; }
        public System.Nullable<long> CompanyID { get; set; }
        public System.Nullable<long> BuyCodeOrderID { get; set; }
        public string CodeMain { get; set; }
        public System.Nullable<long> CodeCount { get; set; }
        public System.Nullable<long> FromCode { get; set; }
        public System.Nullable<long> EndCode { get; set; }
        public string CodeFileURL { get; set; }
        public System.Nullable<int> UseState { get; set; }
        public int? OrderStatus { get; set; }
        public string PackageName { get; set; }
    }

    public class DetailCompanyIDCode
    {
        public string CodeContent { get; set; }
        public long? CodeCount { get; set; }
        public long? RestCount { get; set; }
        public long? UsedCount { get; set; }
        public List<View_ActvitiyRelationCode> List { get; set; }
    }

    public class RedPacketMoney
    {
        public double? AllMoney { get; set; }
        public double? SendMoney { get; set; }
        public double? LeftMoney { get; set; }
        public List<View_RechangeRecord> RechangeRecordLst { get; set; }
        public List<View_RedSendRecord> RedRecordLst { get; set; }
    }

    public partial class View_PRRU_PlatFormUser
    {
        public long EnterprsieCount { get; set; }
        public long CodeCount { get; set; }
    }

    /// <summary>
    /// 发送红包结果
    /// </summary>
    public class SendPacketResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Ok { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 活动编号
        /// </summary>
        public long ActivityId { get; set; }
        /// <summary>
        /// 发送金额
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Phone { get; set; }
    }

    /// <summary>
    /// 导出产品信息
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 产品编号(用户自定义 例如：10031507700166)
        /// </summary>
        public string production_id { get; set; }
        /// <summary>
        /// 产品名称(例如：240克/升噻呋酰胺)
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// 保质期(例如：2年)
        /// </summary>
        public string period { get; set; }
        /// <summary>
        /// 产品价格
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 产品介绍
        /// </summary>
        public string introduce { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// 农药生产证号(例如：D20172694)
        /// </summary>
        public string reg_code { get; set; }
        /// <summary>
        /// 生产厂家(例如：河北三农农用化工有限公司)
        /// </summary>
        public string reg_code_owner { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reg_code_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string product_code { get; set; }
        /// <summary>
        /// 产品别名(例如：240克/升噻呋酰胺)
        /// </summary>
        public string p_alias { get; set; }
        /// <summary>
        /// 产品规格(例如：100g/瓶)
        /// </summary>
        public string unit { get; set; }
    }

    public partial class CollectCodeTable
    {
        public string StrCollectTime { get; set; }
    }

    public partial class CollectCodeDetail
    {
        public string StrProductionDate { get; set; }
    }
    public partial class View_ContinueCode
    {
        public string StrAddDate { get; set; }
    }
    public partial class View_RequestCodeSettingAndEnterprise_Info
    {
        public string StrRequestDate { get; set; }
    }

    public partial class View_IntStorageDetail
    {
        public string StrIntStorageDate { get; set; }
    }

    public partial class RequestCodeSettingMuBan
    {
        public string StrMuBanImg { get; set; }
        public string StrAddDate { get; set; }
        public List<ToJsonImg> imgs { get; set; }
    }

    public partial class EnterpriseMuBanThreeImg
    {
        public string StrFirstImg { get; set; }
        public string StrCenterImg { get; set; }
        public string StrFiveImg { get; set; }
        public List<ToJsonImg> FirstImgs { get; set; }
        public List<ToJsonImg> CenterImgs { get; set; }
        public List<ToJsonImg> FiveImgs { get; set; }
    }
    public partial class BackList
    {
        public string StrBackImg { get; set; }
        public List<ToJsonImg> BackImgs { get; set; }
        public string StrBackCode { get; set; }
        public List<ToJsonProperty> propertys { get; set; }
    }
    public partial class Enterprise_License
    {
        public string StrOperateDate { get; set; }
        public string StrLicenseEndDate { get; set; }
    }
    public partial class MaterialDI
    {
        public string Stradddate { get; set; }
    }
}

