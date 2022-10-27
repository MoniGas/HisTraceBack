using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel
{
    public class AllUDIRequestBase
    {
        public string accessToken { get; set; }
        public string rangeValue { get; set; }
        public int requestType { get; set; }
        public int currentPageNumber { get; set; }
        //2022.06.08 slk
        //新增参数：数据类型(dataType:1.新发布 2.数据变更)，新发布主要是获取新公布出来的数据；数据变更主要是获取发布后通过变更的数据。
        public string dataType { get; set; }
        //新增参数：ZXXSDYCPBS String 否 *最小销售单元产品标识；精准查询
        public string zxxsdycpbs { get; set; }
        //新增参数：CPMCTYMC String 否 *产品名称/通用名称；模糊查询
        public string cpmctymc { get; set; }
        //新增参数：GGXH String 否 *规格型号；模糊查询
        public string ggxh { get; set; }
        //新增参数：YLQXZCRBARMC String 否 *注册/备案人名称；精准查询
        public string ylqxzcrbarmc { get; set; }
        //新增参数：ZCZBHHZBAPZBH String 否 *注册/备案证号；精准查
        public string zczbhhzbapzbh { get; set; }
    }

    public class GetAllUDIInfo : RetuenResult
    {
        public GetAllUDIInfo()
        {
            totalPageCount = "1";
        }
        /// <summary>
        /// 50【说明：总页数】
        /// </summary>
        public string totalPageCount { get; set; }
        /// <summary>
        /// 1480【说明：总记录数量】
        /// </summary>
        public string totalRecordCount { get; set; }
        /// <summary>
        /// 1【说明：当前页码,为对应的参数值，无论结果返回是否有值。例 如，当前获取第一页结果返回为空，则 currentPageNumber 返回为 1】
        /// </summary>
        public string currentPageNumber { get; set; }
        public DeviceInfo dataSet { get; set; }
    }

    public class DeviceInfo
    {
        public List<clinicalInfo> clinicalInfo { get; set; }
        public List<GetAllUDIData> deviceInfo { get; set; }
        public List<packingInfo> packingInfo { get; set; }
        public List<storageInfo> storageInfo { get; set; }
    }

    public class GetAllUDIData
    {
        /// <summary>
        /// 版本号【说明：本产品标识变更次数】
        /// </summary>
        public string versionNumber { get; set; }
        /// <summary>
        /// 版本日期
        /// </summary>
        public string versionTime { get; set; }
        /// <summary>
        /// 版本状态，状态值：新增，更新
        /// </summary>
        public string versionStatus { get; set; }
        /// <summary>
        /// 数据库记录 key
        /// </summary>
        public string deviceRecordKey { get; set; }
        /// <summary>
        /// 最小销售单元产品标识
        /// </summary>
        public string zxxsdycpbs { get; set; }
        /// <summary>
        /// 标识数据状态
        /// </summary>
        public string bssjzt { get; set; }
        /// <summary>
        /// 最小销售单元中使用单元的数量
        /// </summary>
        public string zxxsdyzsydydsl { get; set; }
        /// <summary>
        /// 使用单元产品标识
        /// </summary>
        public string sydycpbs { get; set; }
        /// <summary>
        /// 产品标识编码体系名称，如 GS1，MA 码（IDcode）
        /// </summary>
        public string cpbsbmtxmc { get; set; }
        /// <summary>
        /// 是否包含本体标识： 1 是 0 否
        /// </summary>
        public string sfybtzjbs { get; set; }
        /// <summary>
        /// 是否与最小销售单元产品标识是否一致： 1 是 0 否
        /// </summary>
        public string btcpbsyzxxsdycpbssfyz { get; set; }
        /// <summary>
        /// 本体标识医疗器械本体标识中的产品标识
        /// </summary>
        public string btcpbs { get; set; }
        /// <summary>
        /// 标识发布时间；格式:2019-09-12
        /// </summary>
        public string cpbsfbrq { get; set; }
        /// <summary>
        /// 标识载体，1 一维码,2 二维码，3 RFID，4 其他；如存在多种，则按照 以下规则填写：1,2,3；‘,’为英文状态
        /// </summary>
        public string bszt { get; set; }
        /// <summary>
        /// 是否与注册/备案标识一致； 1 是 0 否
        /// </summary>
        public string sfyzcbayz { get; set; }
        /// <summary>
        /// 注册/备案产品标识
        /// </summary>
        public string zcbacpbs { get; set; }
        /// <summary>
        /// 产品名称/通用名称
        /// </summary>
        public string cpmctymc { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string spmc { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string ggxh { get; set; }
        /// <summary>
        /// 医疗器械是否为包类/组套类产品：1 是 0 否
        /// </summary>
        public string sfwblztlcp { get; set; }
        /// <summary>
        /// 产品描述信息
        /// </summary>
        public string cpms { get; set; }
        /// <summary>
        /// 产品货号或编号
        /// </summary>
        public string cphhhbh { get; set; }
        /// <summary>
        /// 产品类型：1 器械 ； 2 体外诊断试剂
        /// </summary>
        public string qxlb { get; set; }
        /// <summary>
        /// 器械目录分类代码
        /// </summary>
        public string flbm { get; set; }//器械目录分类代码，数据格式按照：器械分类编码-一级 分类编码-二级分类编码（01-01-01），一级、二级分类不明确的情况下用 00 代 替；产品类型为体外试剂时：数据格式为：6840-001
        /// <summary>
        /// 原器械目录分类代码
        /// </summary>
        public string yflbm { get; set; }
        /// <summary>
        /// 注册/备案人名称
        /// </summary>
        public string ylqxzcrbarmc { get; set; }
        /// <summary>
        /// 注册/备案证对应的注册人/备案人的英 文名称
        /// </summary>
        public string ylqxzcrbarywmc { get; set; }
        /// <summary>
        /// 统一社会信用代码，境外企业填写境内代理人的统一社会信用代码
        /// </summary>
        public string tyshxydm { get; set; }
        /// <summary>
        /// 注册/备案证号，多个之间用英文状态',' 分隔
        /// </summary>
        public string zczbhhzbapzbh { get; set; }
        /// <summary>
        /// 耗材或者设备：0 耗材， 1 设备
        /// </summary>
        public string cplb { get; set; }
        /// <summary>
        /// 标记为一次性使用：0 否， 1 是
        /// </summary>
        public string sfbjwycxsy { get; set; }
        /// <summary>
        /// 医疗器械的最大重复使用次数
        /// </summary>
        public string zdcfsycs { get; set; }
        /// <summary>
        /// 医疗器械是否为已灭菌产品：1 是 0 否
        /// </summary>
        public string sfwwjbz { get; set; }
        /// <summary>
        /// 医疗器械使用前是否需要进行灭菌；1 是 0 否
        /// </summary>
        public string syqsfxyjxmj { get; set; }
        /// <summary>
        /// 医疗器械的灭菌方式
        /// </summary>
        public string mjfs { get; set; }
        /// <summary>
        /// 医保编码
        /// </summary>
        public string ybbm { get; set; }
        /// <summary>
        /// 磁共振（MR）安全相关信息;0 安全 ，1 条 件安全， 2 不安全 ，3 说明书或标签上面不包括 MR 安全信息
        /// </summary>
        public string cgzmraqxgxx { get; set; }
        //public List<devicePackage> devicePackage { get; set; }
        //public List<deviceStorage> deviceStorage { get; set; }
        /// <summary>
        /// 特殊存储或操作条件
        /// </summary>
        public string tscchcztj { get; set; }
        //public List<deviceClinical> deviceClinical { get; set; }
        /// <summary>
        /// 特殊使用尺寸说明
        /// </summary>
        public string tsccsm { get; set; }
        /// <summary>
        /// 医疗器械生产标识是否包含批号：1 是 0 否
        /// </summary>
        public string scbssfbhph { get; set; }
        /// <summary>
        /// 医疗器械生产标识是否包含序列号：1 是 0 否
        /// </summary>
        public string scbssfbhxlh { get; set; }
        /// <summary>
        /// 医疗器械生产标识是否包含生产日期：1 是 0 否
        /// </summary>
        public string scbssfbhscrq { get; set; }
        /// <summary>
        /// 医疗器械生产标识是否包含失效日期：1 是 0 否
        /// </summary>
        public string scbssfbhsxrq { get; set; }
        /// <summary>
        /// 其他信息的网址链接
        /// </summary>
        public string qtxxdwzlj { get; set; }
        /// <summary>
        /// 医疗器械在流通领域停止销售的时间
        /// </summary>
        public string tsrq { get; set; }
        public List<contactList> contactList { get; set; }
    }
    public class devicePackage
    {
        /// <summary>
        /// 包装产品标识
        /// </summary>
        public string bzcpbs { get; set; }

        /// <summary>
        /// 包装内含下一级包装产品标识
        /// </summary>
        public string bznhxyjbzcpbs { get; set; }

        /// <summary>
        /// 包装内含下一级产品标识数量
        /// </summary>
        public string bznhxyjcpbssl { get; set; }

        /// <summary>
        /// 包装级别
        /// </summary>
        public string cpbzjb { get; set; }
    }

    public class deviceStorage
    {
        /// <summary>
        /// 储存或操作条件
        /// </summary>
        public string cchcztj { get; set; }

        /// <summary>
        /// 最低值
        /// </summary>
        public string zdz { get; set; }

        /// <summary>
        /// 最高值
        /// </summary>
        public string zgz { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string jldw { get; set; }
    }

    public class deviceClinical
    {
        /// <summary>
        /// 尺寸类型
        /// </summary>
        public string lcsycclx { get; set; }

        /// <summary>
        /// 尺寸值
        /// </summary>
        public string ccz { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string ccdw { get; set; }
    }

    public class contactList
    {
        /// <summary>
        /// 企业联系人邮箱 1
        /// </summary>
        public string qylxryx { get; set; }

        /// <summary>
        /// 企业联系人电话
        /// </summary>
        public string qylxrdh { get; set; }

        /// <summary>
        /// 企业联系人传真
        /// </summary>
        public string qylxrcz { get; set; }
    }
    public class packingInfo
    {
        public string deviceRecordKey { get; set; } //数据库记录 key
        public string bzcpbs { get; set; }//包装产品标识 
        public string bznhxyjbzcpbs { get; set; } //包装内含下一级包装产品标识 
        public string bznhxyjcpbssl { get; set; } //包装内含下一级产品标识数量
        public string cpbzjb { get; set; } //包装级别
    }
    public class storageInfo
    {
        public string deviceRecordKey { get; set; }//数据库记录 key"
        public string cchcztj { get; set; } //储存或操作条件 1
        public string zdz { get; set; }//最低值 1
        public string zgz { get; set; } //最高值 1
        public string jldw { get; set; }//计量单位 1
    }

    public class clinicalInfo
    {
        public string deviceRecordKey { get; set; }//数据库记录 key",
        public string lcsycclx { get; set; }//尺寸类型 
        public string ccz { get; set; } //尺寸值 
        public string ccdw { get; set; }//计量单位
    }
}
