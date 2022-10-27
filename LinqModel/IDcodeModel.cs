using System;
using System.Collections.Generic;

namespace LinqModel
{
    public class Result
    {
        /// <summary>
        /// 返回结果（1正确，0不正确）
        /// </summary>		
        private int _resultcode;
        public int ResultCode
        {
            get { return _resultcode; }
            set { _resultcode = value; }
        }
        /// <summary>
        /// 返回信息
        /// </summary>		
        private string _resultmsg;
        public string ResultMsg
        {
            get { return _resultmsg; }
            set { _resultmsg = value; }
        }
    }



    //区域地址基本实体
    public class AddressInfo
    {
        /// <summary>
        /// 区域ID
        /// </summary>		
        private decimal _address_id;
        public decimal Address_ID
        {
            get { return _address_id; }
            set { _address_id = value; }
        }
        /// <summary>
        /// 区域名称
        /// </summary>		
        private string _addressname;
        public string AddressName
        {
            get { return _addressname; }
            set { _addressname = value; }
        }
        /// <summary>
        /// 区域等级
        /// </summary>		
        private decimal _addresslevel;
        public decimal AddressLevel
        {
            get { return _addresslevel; }
            set { _addresslevel = value; }
        }
        /// <summary>
        /// 区域父级ID
        /// </summary>		
        private decimal _address_id_parent;
        public decimal Address_ID_Parent
        {
            get { return _address_id_parent; }
            set { _address_id_parent = value; }
        }
        /// <summary>
        /// 区域码
        /// </summary>		
        private string _addresscode;
        public string AddressCode
        {
            get { return _addresscode; }
            set { _addresscode = value; }
        }
    }

    public class OrganUnitStatusInfo 
    {
        private int _IsAuthen;
        public int IsAuthen 
        {
            get { return _IsAuthen; }
            set { _IsAuthen = value; }
        }

        private int _IsAudit;
        public int IsAudit 
        {
            get { return _IsAudit; }
            set { _IsAudit = value; }
        }

        private string _SurplusTime;
        public string SurplusTime 
        {
            get { return _SurplusTime; }
            set { _SurplusTime = value; }
        }

        private int _ResultCode;
        public int ResultCode 
        {
            get { return _ResultCode; }
            set { _ResultCode = value; }
        }

        private string _ResultMsg;
        public string ResultMsg 
        {
            get { return _ResultMsg; }
            set { _ResultMsg = value; }
        }
    }

    /// <summary>
    /// 返回区域列表实体
    /// </summary>
    public class AreaInfo : Result
    {
        private List<AddressInfo> _addresslist = new List<AddressInfo>();
        public List<AddressInfo> AddressList
        {
            get { return _addresslist; }
            set { _addresslist = value; }
        }
    }

    /// <summary>
    /// 行业基本实体
    /// </summary>
    public class Trade
    {

        private decimal _trade_id;
        /// <summary>
        /// 行业ID
        /// </summary>
        public decimal Trade_ID
        {
            get { return _trade_id; }
            set { _trade_id = value; }
        }
        /// <summary>
        /// 行业名称
        /// </summary>		
        private string _tradename;
        public string TradeName
        {
            get { return _tradename; }
            set { _tradename = value; }
        }
        /// <summary>
        /// 行业等级
        /// </summary>		
        private decimal _tradelevel;
        public decimal TradeLevel
        {
            get { return _tradelevel; }
            set { _tradelevel = value; }
        }
        /// <summary>
        /// 父级ID
        /// </summary>		
        private decimal _trade_id_parent;
        public decimal Trade_ID_Parent
        {
            get { return _trade_id_parent; }
            set { _trade_id_parent = value; }
        }

    }

    /// <summary>
    /// 返回行业列表实体
    /// </summary>
    public class TradeInfo : Result
    {
        private List<Trade> _tradelist = new List<Trade>();
        public List<Trade> TradeList
        {
            get { return _tradelist; }
            set { _tradelist = value; }
        }
    }


    /// <summary>
    /// 单位性质基本实体
    /// </summary>
    public class UnitType:Result
    {

        private decimal _unittype_id;
        /// <summary>
        /// 单位所属性质ID
        /// </summary>
        public decimal UnitType_ID
        {
            get { return _unittype_id; }
            set { _unittype_id = value; }
        }

        private string _unittypename;
        /// <summary>
        /// 性质名称
        /// </summary>
        public string UnitTypeName
        {
            get { return _unittypename; }
            set { _unittypename = value; }
        }


        private decimal _code;
        /// <summary>
        /// 编号
        /// </summary>
        public decimal Code
        {
            get { return _code; }
            set { _code = value; }
        }



    }


    /// <summary>
    /// 返回单位性质列表实体
    /// </summary>
    public class UnitTypeInfo : Result
    {
        private List<UnitType> _unittypelist = new List<UnitType>();
        public List<UnitType> UnitTypeList
        {
            get { return _unittypelist; }
            set { _unittypelist = value; }
        }
    }

    /// <summary>
    /// 返回企业主码（注册企业成功后）
    /// </summary>
    public class CompanyIDcode : HisResult
    {
        private string _organunitidcode;
        /// <summary>
        /// 企业主码
        /// </summary>
        public string organunit_idcode
        {
            get { return _organunitidcode; }
            set { _organunitidcode = value; }
        }
    }

    /// <summary>
    /// 返回短信验证码
    /// </summary>
    public class SmsVerifyCode : Result
    {
        private string _verifycode;
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string VerifyCode
        {
            get { return _verifycode; }
            set { _verifycode = value; }
        }
    }

    /// <summary>
    /// 单位基本实体
    /// </summary>
    public class OrganUnit : Result
    {
        /// <summary>
        /// 单位ID
        /// </summary>		
        private decimal _organunit_id;
        public decimal OrganUnit_ID
        {
            get { return _organunit_id; }
            set { _organunit_id = value; }
        }

        /// <summary>
        /// 省ID
        /// </summary>		
        private decimal _province_id;
        public decimal Province_ID
        {
            get { return _province_id; }
            set { _province_id = value; }
        }
        /// <summary>
        /// 市ID
        /// </summary>		
        private decimal _city_id;
        public decimal City_ID
        {
            get { return _city_id; }
            set { _city_id = value; }
        }
        /// <summary>
        /// 区县ID
        /// </summary>		
        private decimal _area_id;
        public decimal Area_ID
        {
            get { return _area_id; }
            set { _area_id = value; }
        }
        /// <summary>
        /// 行业ID
        /// </summary>		
        private decimal _trade_id;
        public decimal Trade_ID
        {
            get { return _trade_id; }
            set { _trade_id = value; }
        }
        /// <summary>
        /// 状态ID
        /// </summary>		
        private decimal _status_id;
        public decimal Status_ID
        {
            get { return _status_id; }
            set { _status_id = value; }
        }
        /// <summary>
        /// 单位名称
        /// </summary>		
        private string _organunitname;
        public string OrganUnitName
        {
            get { return _organunitname; }
            set { _organunitname = value; }
        }
        /// <summary>
        /// 单位名称（英文）
        /// </summary>		
        private string _organunitnameen;
        public string OrganUnitNameen
        {
            get { return _organunitnameen; }
            set { _organunitnameen = value; }
        }
        /// <summary>
        /// 单位地址
        /// </summary>		
        private string _organunitaddress;
        public string OrganUnitAddress
        {
            get { return _organunitaddress; }
            set { _organunitaddress = value; }
        }
        /// <summary>
        /// 单位地址（英文）
        /// </summary>		
        private string _organunitaddressen;
        public string OrganUnitAddressen
        {
            get { return _organunitaddressen; }
            set { _organunitaddressen = value; }
        }
        private string _areaaddress;

        /// <summary>
        /// 企业区域串
        /// </summary>
        public string Areaaddress
        {
            get { return _areaaddress; }
            set { _areaaddress = value; }
        }

        /// <summary>
        /// 企业注册OID码
        /// </summary>		
        private string _organunit_oid;
        public string OrganUnit_Oid
        {
            get { return _organunit_oid; }
            set { _organunit_oid = value; }
        }


        private string _unittype_id;
        /// <summary>
        /// 单位所属性质
        /// </summary>
        public string UnitType_ID
        {
            get { return _unittype_id; }
            set { _unittype_id = value; }
        }
        private string _linkman;
        /// <summary>
        /// 联系人
        /// </summary>
        public string Linkman
        {
            get { return _linkman; }
            set { _linkman = value; }
        }
        private string _fax;
        /// <summary>
        /// 传真
        /// </summary>
        public string Fax
        {
            get { return _fax; }
            set { _fax = value; }
        }
        private string _linkphone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Linkphone
        {
            get { return _linkphone; }
            set { _linkphone = value; }
        }
        private string _email;
        /// <summary>
        /// 企业邮箱
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
    }

    /// <summary>
    /// 返回单位列表实体
    /// </summary>
    public class Organunitinfo : Result
    {
        private List<OrganUnit> _organunitlist = new List<OrganUnit>();
        public List<OrganUnit> OrganUnitList
        {
            get { return _organunitlist; }
            set { _organunitlist = value; }
        }
    }






    /// <summary>
    /// 返回品类主码
    /// </summary>
    public class CategoryIDcode : Result
    {
        private string _organUnitIDcode;
        /// <summary>
        /// 品类主码
        /// </summary>
        public string OrganUnitIDcode
        {
            get { return _organUnitIDcode; }
            set { _organUnitIDcode = value; }
        }

        private decimal _categoryReg_ID;

        public decimal CategoryReg_ID
        {
            get { return _categoryReg_ID; }
            set { _categoryReg_ID = value; }
        }
    }


    /// <summary>
    /// 返回码图地址
    /// </summary>
    public class Codepicaddress : Result
    {
        private string _codepicaddress;
        /// <summary>
        /// 码图地址
        /// </summary>
        public string CodePicAddress
        {
            get { return _codepicaddress; }
            set { _codepicaddress = value; }
        }
    }

    /// <summary>
    /// 人事物用途基本实体
    /// </summary>
    public class CodeUse
    {

        /// <summary>
        /// 用途ID
        /// </summary>		
        private decimal _codeuse_id;
        public decimal CodeUse_ID
        {
            get { return _codeuse_id; }
            set { _codeuse_id = value; }
        }
        /// <summary>
        /// 产品、零部件、名片、网站、广告宣传、折扣券（优惠、团购）、会员证等
        /// </summary>		
        private string _codeusename;
        public string CodeUseName
        {
            get { return _codeusename; }
            set { _codeusename = value; }
        }
        /// <summary>
        /// 用途代码
        /// </summary>		
        private string _codeusecode;
        public string CodeUseCode
        {
            get { return _codeusecode; }
            set { _codeusecode = value; }
        }

        /// <summary>
        /// 人事物类型
        /// </summary>
        private decimal _type_id;
        public decimal Type_ID
        {
            get { return _type_id; }
            set { _type_id = value; }
        }

    }


    /// <summary>
    /// 返回用途列表实体
    /// </summary>
    public class Codeuseinfo : Result
    {
        private List<CodeUse> _codeuselist = new List<CodeUse>();
        public List<CodeUse> CodeUseList
        {
            get { return _codeuselist; }
            set { _codeuselist = value; }
        }
    }

    /// <summary>
    /// 品类基本实体
    /// </summary>
    public class IndustryCategory
    {

        /// <summary>
        /// 品类ID
        /// </summary>		
        private decimal _industrycategory_id;
        public decimal IndustryCategory_ID
        {
            get { return _industrycategory_id; }
            set { _industrycategory_id = value; }
        }
        /// <summary>
        /// 品类名称
        /// </summary>		
        private string _industrycategoryname;
        public string IndustryCategoryName
        {
            get { return _industrycategoryname; }
            set { _industrycategoryname = value; }
        }
        /// <summary>
        /// 品类父级ID
        /// </summary>		
        private decimal _industrycategory_id_parent;
        public decimal IndustryCategory_ID_Parent
        {
            get { return _industrycategory_id_parent; }
            set { _industrycategory_id_parent = value; }
        }
        /// <summary>
        /// 品类等级
        /// </summary>		
        private decimal _industrycategorylevel;
        public decimal IndustryCategoryLevel
        {
            get { return _industrycategorylevel; }
            set { _industrycategorylevel = value; }
        }
        /// <summary>
        /// 品类代码
        /// </summary>		
        private string _industrycategorycode;
        public string IndustryCategoryCode
        {
            get { return _industrycategorycode; }
            set { _industrycategorycode = value; }
        }
        /// <summary>
        /// 所属用途ID
        /// </summary>		
        private decimal _codeuse_id;
        public decimal Codeuse_ID
        {
            get { return _codeuse_id; }
            set { _codeuse_id = value; }
        }
        /// <summary>
        /// 是否删除
        /// </summary>
        private decimal _industrycategoryisdel;

        public decimal IndustryCategoryIsDel
        {
            get { return _industrycategoryisdel; }
            set { _industrycategoryisdel = value; }
        }



        private decimal _type_id;
        /// <summary>
        /// 人、事、物分类
        /// </summary>
        public decimal Type_ID
        {
            get { return _type_id; }
            set { _type_id = value; }
        }
    }


    /// <summary>
    /// 返回品类列表实体
    /// </summary>
    public class IndustryCategoryInfo : Result
    {
        private List<IndustryCategory> _industrycategorylist = new List<IndustryCategory>();
        public List<IndustryCategory> IndustryCategoryList
        {
            get { return _industrycategorylist; }
            set { _industrycategorylist = value; }
        }
    }
    public class MainCodeInfo : Result
    {
        public string OrganUnit_Oid { get; set; }
        public string OrganUnitName { get; set; }
    }

    public class BaseIDcodeInfo : Result
    {
        private List<Bill_CategoryRegBase> _baseidcodelist = new List<Bill_CategoryRegBase>();
        public List<Bill_CategoryRegBase> BaseIDcodeList
        {
            get { return _baseidcodelist; }
            set { _baseidcodelist = value; }
        }
    }

    /// <summary>
    /// 品类IDcode基本信息
    /// </summary>
    public class Bill_CategoryRegBase
    {
        /// <summary>
        /// CATEGORYREG_ID
        /// </summary>		
        private decimal _categoryreg_id;
        /// <summary>
        /// ID
        /// </summary>
        public decimal CategoryReg_ID
        {
            get { return _categoryreg_id; }
            set { _categoryreg_id = value; }
        }
        public string CategoryReg_IDstr
        {
            get { return Convert.ToString(_categoryreg_id); }
        }
        /// <summary>
        /// ORGANUNIT_ID
        /// </summary>		
        private decimal _organunit_id;
        /// <summary>
        /// 单位ID
        /// </summary>
        public decimal OrganUnit_ID
        {
            get { return _organunit_id; }
            set { _organunit_id = value; }
        }
        public string OrganUnit_IDstr
        {
            get { return Convert.ToString(_organunit_id); }
        }
        /// <summary>
        /// BILLNUMBER
        /// </summary>		
        private string _billnumber;
        /// <summary>
        /// 订单号
        /// </summary>
        public string BillNumber
        {
            get { return _billnumber; }
            set { _billnumber = value; }
        }

        private string _guidnumber;
        /// <summary>
        /// 流水号
        /// </summary>	
        public string GuidNumber
        {
            get { return _guidnumber; }
            set { _guidnumber = value; }
        }

        /// <summary>
        /// 结构化数字编码，不能自增ID
        /// </summary>		
        private decimal _codeuse_id;
        /// <summary>
        /// 码用途ID
        /// </summary>
        public decimal CodeUse_ID
        {
            get { return _codeuse_id; }
            set { _codeuse_id = value; }
        }
        /// <summary>
        /// CATEGORYCODE
        /// </summary>		
        private string _categorycode;
        /// <summary>
        /// 品类码号
        /// </summary>
        public string CategoryCode
        {
            get { return _categorycode; }
            set { _categorycode = value; }
        }
        /// <summary>
        /// CATEGORYMEMO
        /// </summary>		
        private string _categorymemo;
        /// <summary>
        /// 品类描述
        /// </summary>
        public string CategoryMemo
        {
            get { return _categorymemo; }
            set { _categorymemo = value; }
        }

        /// <summary>
        /// ModelNumber
        /// </summary>		
        private string _modelnumber;
        /// <summary>
        /// 型号
        /// </summary>
        public string ModelNumber
        {
            get { return _modelnumber; }
            set { _modelnumber = value; }
        }
        /// <summary>
        /// ModelNumberCode
        /// </summary>		
        private string _modelnumbercode;
        /// <summary>
        /// 型号编号
        /// </summary>
        public string ModelNumberCode
        {
            get { return _modelnumbercode; }
            set { _modelnumbercode = value; }
        }


        private decimal _codepaytype;
        /// <summary>
        /// 码收费类型
        /// </summary>
        public decimal CodePayType
        {
            get { return _codepaytype; }
            set { _codepaytype = value; }
        }

        private string _modelnumberen;
        /// <summary>
        /// 产品型号英文名称
        /// </summary>
        public string ModelNumberEn
        {
            get { return _modelnumberen; }
            set { _modelnumberen = value; }
        }


        private string _introduction;
        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction
        {
            get { return _introduction; }
            set { _introduction = value; }
        }

        private string completeCode;
        /// <summary>
        /// 完整码号
        /// </summary>
        public string CompleteCode
        {
            get { return completeCode; }
            set { completeCode = value; }
        }

        private string _DetailPara;
        /// <summary>
        /// 详细参数
        /// </summary>
        public string DetailPara
        {
            get { return _DetailPara; }
            set { _DetailPara = value; }
        }

        private string _PictureName;
        //产品图片
        public string PictureName
        {
            get { return _PictureName; }
            set { _PictureName = value; }
        }

    }


    #region 医疗器械相关类

    public class HisResult {
        public int result_code { get; set; }

        public String result_msg { get; set; }
    }
    /// <summary>
    /// 医疗行业单位性质编码
    /// </summary>
    public class HisUnitType {

        public int unittype_id { get; set; }
        /// <summary>
         /// 单位性质名称
         /// </summary>
        public String unittypename { get; set; }
        /// <summary>
        /// 单位性质编码
        /// </summary>
        public String code { get; set; }
    }

    /// <summary>
    /// 接口单位类型
    /// </summary>
    public class InterFaceHisUnitType : HisResult
    {
        public List<HisUnitType> unit_type_list { get;set; }
    }

    /// <summary>
    /// 医疗品类编码
    /// </summary>
    public class HisIndustryCategory
    {
        /// <summary>
        /// 品类ID
        /// </summary>
        public int industrycategory_id { get; set; }
        /// <summary>
        /// 品类名称
        /// </summary>
        public string industrycategory_name { get; set; }
        /// <summary>
        /// 品类父级ID
        /// </summary>
        public int industrycategory_id_parent { get; set; }
        /// <summary>
        /// 品类等级
        /// </summary>
        public int industrycategory_level { get; set; }
        /// <summary>
        /// 品类代码
        /// </summary>
        public string industrycategory_code { get; set; }
        /// <summary>
        /// 所属用途ID
        /// </summary>
        public int codeuse_id { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int industrycategory_isdel { get; set; }
        /// <summary>
        /// 人事物类型
        /// </summary>
        public int type_id { get; set; }
        /// <summary>
        /// 填写项验证规则
        /// </summary>
        public string check_format { get; set; }
    }

    /// <summary>
    /// 接口医疗品类
    /// </summary>
    public class InterFaceHisIndustryCategory : HisResult
    {
        public List<HisIndustryCategory> industrycategory_list { get; set; }
    }

    /// <summary>
    /// 下载
    /// </summary>
    public class InterFaceHisCodeFileUrlInfo : HisResult
    {
        public string codefileurl_info { get; set; }
    }

    /// <summary>
    /// pi类
    /// </summary>
    public class HisPI
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string serialnumber { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string startdate { get; set; }
        /// <summary>
        /// 生产批号
        /// </summary>
        public string batchnumber { get; set; }
        /// <summary>
        /// 灭菌批号
        /// </summary>
        public string dbatchnumber { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>
        public string enddate { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string effectivedate { get; set; }
        /// <summary>
        /// 注册途径
        /// </summary>
        public int createtype { get; set; }
        /// <summary>
        /// 批量申请生成批次
        /// </summary>
        public string batch_no { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string createdate { get; set; }
        /// <summary>
        /// UDI码数量
        /// </summary>
        public int codenum { get; set; }
        /// <summary>
        /// UDI-DI编码
        /// </summary>
        public string completecode { get; set; }
    }
    public class ListHisPI : HisResult
    {
        public List<HisPI> data { get; set; }
    }

    public class ListHisDI : HisResult
    {
        public List<HisDI> data { get; set; }
    }
    /// <summary>
    /// 码明细
    /// </summary>
    public class ListPICode : HisResult
    {
        public List<string> data { get; set; }
    }
    /// <summary>
    /// DI类
    /// </summary>
    public class HisDI
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string modelnumber { get; set; }
        /// <summary>
        /// 包装规格
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 品类编码
        /// </summary>
        public string categorycode { get; set; }
        /// <summary>
        /// 包装规格名称
        /// </summary>
        public string specification_name { get; set; }
        /// <summary>
        /// 完整码号UDI-DI码号
        /// </summary>
        public string completecode { get; set; }
        /// <summary>
        /// 注册途径
        /// </summary>
        public string createtype { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string createdate { get; set; }
        /// <summary>
        /// 产品规格型号
        /// </summary>
        public string product_model { get; set; }

    }

    /// <summary>
    /// 批量注册医疗器械品类
    /// </summary>
    public class IDCodeUploadCodeListMsg
    {
        /// <summary>
        /// 批量申请生成批次
        /// </summary>
        public string data { get; set; }
        public int result_code { get; set; }
        public string result_msg { get; set; }
    }
    public class HisOrganUnit
    {
        public int result_code { get; set; }
        public string result_msg { get; set; }
        /// <summary>
        /// 授权KEY
        /// </summary>
        public string author_key { get; set; }
        /// <summary>
        /// 系统授权码
        /// </summary>
        public string author_code { get; set; }
        /// <summary>
        /// 企业主码
        /// </summary>
        public string organunit_oid { get; set; }
    }
    #endregion
}