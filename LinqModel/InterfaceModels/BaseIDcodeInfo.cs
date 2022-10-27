using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
    public class BaseIDcodeInfo
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<CategoryRegInfo> data { get; set; }

        /// <summary>
        /// 返回状态
        /// 1	成功
        /// 0	失败
        /// -1	系统繁忙，请稍后再试
        /// 10001	授权失败，授权Key不存在
        /// 10002	授权失败，授权Key已被禁用
        /// 10003	单位信息不存在
        /// </summary>
        public int result_code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string result_msg { get; set; }
    }

    public class CategoryRegInfo
    {
        /// <summary>
        /// 品类注册ID(主键ID)
        /// </summary>
        public string categoryreg_id { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public string organunit_id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string modelnumber { get; set; }

        /// <summary>
        /// 品类码号
        /// </summary>
        public string categorycode { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string specification_name { get; set; }

        /// <summary>
        /// 包装规格号码
        /// </summary>
        public string specifications { get; set; }

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
        /// 完整码号
        /// </summary>
        public string completecode { get; set; }

        public int createtype { get; set; }
        public string product_model { get; set; }
    }

    public class Organunit
    {
        /// <summary>
        /// 单位 ID
        /// </summary>
        public string OrganUnit_ID { get; set; }
        /// <summary>
        /// 单位 ID
        /// </summary>
        public string OrganUnit_IDstr { get; set; }
        /// <summary>
        /// 平台角色类型
        /// </summary>
        public int PlateformRole_ID { get; set; }
        /// <summary>
        /// 省级 ID
        /// </summary>
        public int Province_ID { get; set; }
        /// <summary>
        /// 市级 ID
        /// </summary>
        public int City_ID { get; set; }
        /// <summary>
        /// 区县 ID
        /// </summary>
        public int Area_ID { get; set; }

        /// <summary>
        /// 行业 ID
        /// </summary>
        public int Trade_ID { get; set; }
        /// <summary>
        /// 单位状态
        /// </summary>
        public int Status_ID { get; set; }
        /// <summary>
        /// 单位状态
        /// </summary>
        public int IsSC { get; set; }
        /// <summary>
        /// 单位组织名称
        /// </summary>
        public string OrganUnitName { get; set; }
        /// <summary>
        /// 单位组织名称英文
        /// </summary>
        public string OrganUnitNameen { get; set; }
        /// <summary>
        /// 单位组织地址
        /// </summary>
        public string OrganUnitAddress { get; set; }
        /// <summary>
        /// 单位组织地址英文
        /// </summary>
        public string OrganUnitAddressen { get; set; }
        /// <summary>
        /// 单位组织主码
        /// </summary>
        public string OrganUnit_Oid { get; set; }
        /// <summary>
        /// 单位组织性质
        /// </summary>
        public int UnitType_ID { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Linkman { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Linkphone { get; set; }
        /// <summary>
        /// 固定电话
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 最后审核时间
        /// </summary>
        public string FinallyExamDate { get; set; }
        /// <summary>
        /// 联系人英文名
        /// </summary>
        public string LinkManEn { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public int CodePayType { get; set; }
        /// <summary>
        /// 单位办公地址
        /// </summary>
        public string UnitWorkAddress { get; set; }
        /// <summary>
        /// 单位办公地址英文名
        /// </summary>
        public string UnitWorkAddressEn { get; set; }
        /// <summary>
        /// 单位规模
        /// </summary>
        public int UnitSizeType_ID { get; set; }
        /// <summary>
        /// 单位注册资金
        /// </summary>
        public int RegisteredCapital { get; set; }
        /// <summary>
        /// 单位图标
        /// </summary>
        public string UnitIcon { get; set; }
        /// <summary>
        /// 码图颜色
        /// </summary>
        public int QRCodeColor { get; set; }
        /// <summary>
        /// 是否启用 logo
        /// </summary>
        public int QRCodeLogo { get; set; }
        /// <summary>
        /// 单位主码对应的解析地址
        /// </summary>
        public string GotoUrl { get; set; }
        /// <summary>
        /// 解析地址状态
        /// </summary>
        public int UrlStatus { get; set; }
        /// <summary>
        /// 结果参
        /// </summary>
        public int ResultCode { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string ResultMsg { get; set; }

    }

    public class IdcodeLoginverifyInfo
    {
        /// <summary>
        /// 单位 ID
        /// </summary>
        public string organunit_id { get; set; }
        /// <summary>
        /// 单位 ID
        /// </summary>
        public string organunit_idstr { get; set; }
        /// <summary>
        /// 平台角色类型   1：总中心2：省分中心3：市分中心4：单位0:未知
        /// </summary>
        public int? plateformrole_id { get; set; }
        /// <summary>
        /// 省级 ID
        /// </summary>
        public int? province_id { get; set; }
        /// <summary>
        /// 市级 ID
        /// </summary>
        public int? city_id { get; set; }
        /// <summary>
        /// 区县 ID
        /// </summary>
        public int? area_id { get; set; }

        /// <summary>
        /// 行业 ID
        /// </summary>
        public int? trade_id { get; set; }
        /// <summary>
        /// 单位状态100：正式用户0：待审核-1：审核失败-2：禁用-100：待完善资料
        /// </summary>
        public int? status_id { get; set; }
        /// <summary>
        /// SP服务商状态-4：尚未申请SP-1：审核失败0：待审核1：已成为SP
        /// </summary>
        public int? issc { get; set; }
        /// <summary>
        /// 单位组织名称
        /// </summary>
        public string organunit_name { get; set; }
        /// <summary>
        /// 单位组织名称英文
        /// </summary>
        public string organunit_name_en { get; set; }
        /// <summary>
        /// 单位组织地址
        /// </summary>
        public string organunit_address { get; set; }
        /// <summary>
        /// 单位组织地址英文
        /// </summary>
        public string organunit_address_en { get; set; }
        /// <summary>
        /// 单位组织主码
        /// </summary>
        public string organunit_oid { get; set; }
        /// <summary>
        /// 单位组织性质
        /// </summary>
        public int? unittype_id { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string linkman { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string linkphone { get; set; }
        /// <summary>
        /// 固定电话
        /// </summary>
        public string fax { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 最后审核时间
        /// </summary>
        public string finallyexam_date { get; set; }
        /// <summary>
        /// 联系人英文名
        /// </summary>
        public string linkman_en { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string organization_code { get; set; }
        /// <summary>
        /// 证件类型  1：组织/单位机构代码2：统一社会信用代码3：个体工商户营业执照号；0：未知
        /// </summary>
        public int? code_pay_type { get; set; }
        /// <summary>
        /// 单位办公地址
        /// </summary>
        public string unit_workaddress { get; set; }
        /// <summary>
        /// 单位办公地址英文名
        /// </summary>
        public string unit_workaddress_en { get; set; }
        /// <summary>
        /// 单位规模
        /// </summary>
        public int unit_size_type_id { get; set; }
        /// <summary>
        /// 单位注册资金
        /// </summary>
        public int? registered_capital { get; set; }
        /// <summary>
        /// 单位图标
        /// </summary>
        public string unit_icon { get; set; }
        /// <summary>
        /// 是否彩色1：彩色0：黑色
        /// </summary>
        public int? qrcode_color { get; set; }
        /// <summary>
        /// 是否启用单位logo1：启用0：不启用
        /// </summary>
        public int? qrcode_logo { get; set; }
        /// <summary>
        /// 单位主码解析地址
        /// </summary>
        public string gotourl { get; set; }
        /// <summary>
        /// 单位主码解析地址状态1：启用0：禁用
        /// </summary>
        public int? url_status { get; set; }
        /// <summary>
        /// 结果参
        /// </summary>
        public int result_code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string result_msg { get; set; }

    }

    public class IDcodeMedicalPIRecord
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<MedicalPIRecord> data { get; set; }

        /// <summary>
        /// 返回状态
        /// 1	成功
        /// 0	失败
        /// -1	系统繁忙，请稍后再试
        /// 10001	授权失败，授权Key不存在
        /// 10002	授权失败，授权Key已被禁用
        /// 10003	单位信息不存在
        /// </summary>
        public int result_code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string result_msg { get; set; }
    }

    /// <summary>
    /// 用户注册产品UDI-PI生成记录
    /// </summary>
    public class MedicalPIRecord
    {
        /// <summary>
        /// UDI-DI编码
        /// </summary>
        public string completecode { get; set; }
        /// <summary>
        /// 批量申请生成批次
        /// </summary>
        public string batch_no { get; set; }
        /// <summary>
        /// 注册途径
        /// 0、官网后台
        /// 1、接口：单次申请
        /// 2、接口：上传txt
        /// 3、接口：上传参数列表
        /// 4、接口：前缀+起始号、终止号
        /// </summary>
        public int createtype { get; set; }
        public string createtype_name { get; set; }
        /// <summary>
        /// UDI码数量
        /// </summary>
        public int codenum { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string createdate { get; set; }
        public string product_model { get; set; }
    }

    /// <summary>
    /// 用户注册产品UDI-PI明细
    /// </summary>
    public class IDcodeMedicalPIList
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<string> data { get; set; }

        /// <summary>
        /// 返回状态
        /// 1	成功
        /// 0	失败
        /// -1	系统繁忙，请稍后再试
        /// 10001	授权失败，授权Key不存在
        /// 10002	授权失败，授权Key已被禁用
        /// 10003	单位信息不存在
        /// </summary>
        public int result_code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string result_msg { get; set; }
    }

    public class IDcodeMedicalDIList
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<MedicalDIRecord> data { get; set; }

        /// <summary>
        /// 返回状态
        /// 1	成功
        /// 0	失败
        /// -1	系统繁忙，请稍后再试
        /// 10001	授权失败，授权Key不存在
        /// 10002	授权失败，授权Key已被禁用
        /// 10003	单位信息不存在
        /// </summary>
        public int result_code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string result_msg { get; set; }
    }

    /// <summary>
    /// 用户注册产品UDI-PI生成记录
    /// </summary>
    public class MedicalDIRecord
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
        /// UDI-DI编码
        /// </summary>
        public string completecode { get; set; }
        /// <summary>
        /// 注册途径
        /// 0、官网后台
        /// 1、接口：单次申请
        /// 2、接口：上传txt
        /// 3、接口：上传参数列表
        /// 4、接口：前缀+起始号、终止号
        /// </summary>
        public string createtype { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string createdate { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        public string product_model { get; set; }
    }
}
