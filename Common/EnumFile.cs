using System.ComponentModel;

namespace Common
{
    public class EnumFile
    {
        /// <summary>
        /// 配置红包状态
        /// </summary>
        public enum PacketState
        {
            /// <summary>
            /// 配置红包成功
            /// </summary>
            Success = 1
        }

        /// <summary>
        /// 产品条码生成方法
        /// </summary>
        public enum Materialtype
        {
            /// <summary>
            /// 手动生成
            /// </summary>
            Getcode = 1,
            /// <summary>
            /// 自动生成
            /// </summary>
            AutoCode = 0
        }
        /// <summary>
        /// 状态通用
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// 已删除/已处理的（投诉管理）
            /// </summary>
            delete = -1,
            /// <summary>
            /// 正常
            /// </summary>
            used = 0,
            /// <summary>
            /// 已销售（批次用）
            /// </summary>
            saled = 1,
            /// <summary>
            /// 未审核
            /// </summary>
            unaudited = 2,
            /// <summary>
            /// 已审核
            /// </summary>
            audited = 3
        }

        public enum IsRead
        {
            noRead = 0,
            isRead = 1
        }
        /// <summary>
        /// 生成二维码模块标识
        /// </summary>
        public enum TerraceEwm
        {
            /// <summary>
            /// 企业宣传标识
            /// </summary>
            [Description("企业宣传")]
            showCompany = 1,
            /// <summary>
            /// 部门宣传标识
            /// </summary>
            [Description("部门宣传")]
            showDept = 2,
            /// <summary>
            /// 名片宣传标识
            /// </summary>
            [Description("名片宣传")]
            showUser = 3,
            /// <summary>
            /// 新闻宣传标识
            /// </summary>
            //[Description("新闻宣传")]
            //showNews = 4,
            /// <summary>
            /// 生产基地标识
            /// </summary>
            [Description("生产基地")]
            greenHouse = 7,
            /// <summary>
            /// 图片验证二维码
            /// </summary>
            [Description("图片验证二维码")]
            validate = 8,
            /// <summary>
            /// 货位标识
            /// </summary>
            [Description("仓库货位")]
            slotting = 4,
            /// <summary>
            /// 垛位标识
            /// </summary>
            [Description("垛位")]
            cribCode = 10
        }
        /// <summary>
        /// 码审核状态定义
        /// </summary>
        public enum RequestCodeStatus
        {
            /// <summary>
            /// 未审核
            /// </summary>  
            [Description("未审核")]
            Unaudited = 1040000001,

            /// <summary>
            /// 生成完成
            /// </summary>
            [Description("生成完成")]
            GenerationIsComplete = 1040000005,

            /// <summary>
            /// 生成失败
            /// </summary>
            [Description("生成失败")]
            GenerationFailed = 1040000006,

            /// <summary>
            /// 审批通过，等待生成
            /// </summary>
            [Description("审批通过，等待生成")]
            ApprovedWaitingToBeGenerated = 1040000008,

            /// <summary>
            /// 打包成功
            /// </summary>
            [Description("打包成功")]
            PackToSuccess = 1040000010,

            /// <summary>
            /// 打包失败
            /// </summary>
            [Description("打包失败")]
            PackagingFailure = 1040000011
        }

        /// <summary>
        /// 码使用状态定义
        /// </summary>
        public enum UsingStateCode
        {
            /// <summary>
            /// 未使用
            /// </summary>
            NotUsed = 1040000008,

            /// <summary>
            /// 已使用(已销售)
            /// </summary>
            HasBeenUsed = 1040000009,

            /// <summary>
            /// 已激活
            /// </summary>
            Activated = 1040000010
        }
        /// <summary>
        /// 申请加入区域品牌是状态的变动
        /// </summary>
        public enum PlatFormState
        {
            /// <summary>
            /// 已禁止
            /// </summary>
            stop = -2,
            /// <summary>
            /// 审核不通过
            /// </summary>
            no_pass = -1,
            /// <summary>
            /// 未审核
            /// </summary>
            no_examine = 0,
            /// <summary>
            /// 审核通过
            /// </summary>
            pass = 1
        }
        /// <summary>
        /// 品牌类型
        /// </summary>
        public enum BrandType
        {
            /// <summary>
            /// 企业品牌
            /// </summary>
            CorporateBrand = 1,

            /// <summary>
            /// 区域品牌
            /// </summary>
            RegionalBrand = 2
        }
        /// <summary>
        /// 平台角色管理
        /// </summary>
        public enum PlatFormLevel
        {
            /// <summary>
            /// 农企
            /// </summary>
            Enterprise = 1,

            /// <summary>
            /// 监管部门
            /// </summary>
            RegulatoryAuthorities = 2,

            /// <summary>
            /// 平台商
            /// </summary>
            PlatformProviders = 3,

            /// <summary>
            /// 服务中心
            /// </summary>
            Service = 4
        }

        public enum TemplateType
        {
            /// <summary>
            /// 1	首页-栏目信息显示多条
            /// </summary>
            HomeMany = 1,
            /// <summary>
            /// 2	首页-显示栏目详细信息
            /// </summary>
            HomeOnly = 2,
            /// <summary>
            /// 3	首页-只显示企业概况
            /// </summary>
            HomeOnlyCompany = 3,
            /// <summary>
            /// 4	导航是否隐藏
            /// </summary>
            IsDis = 4,
            /// <summary>
            /// 5   首页-显示更多
            /// </summary>
            HomeDisMore = 5
        }

        public enum ShowCodeType
        {
            /// <summary>
            /// 1=企业宣传
            /// </summary>
            Company = 1,
            /// <summary>
            /// 2=部门宣传
            /// </summary>
            Dept = 2,
            /// <summary>
            /// 3=名片宣传
            /// </summary>
            User = 3,
            /// <summary>
            /// 4=产品宣传
            /// </summary>
            Material = 4,
            /// <summary>
            /// 5=信息宣传
            /// </summary>
            News = 5
        }

        public enum ZuoYeType
        {
            /// <summary>
            /// 种植
            /// </summary>
            [Description("种植")]
            Produce = 0,
            /// <summary>
            /// 加工
            /// </summary>
            [Description("加工")]
            Processing = 1,
            /// <summary>
            /// 养殖
            /// </summary>
            [Description("养殖")]
            Feed = 2
        }

        public enum ViewType
        {
            /// <summary>
            /// 不可用
            /// </summary>
            Disable = -2,
            /// <summary>
            /// 可用
            /// </summary>
            Enable = 1
        }

        /// <summary>
        /// 订单交易状态
        /// </summary>
        public enum PayStatus
        {
            /// <summary>
            /// 删除
            /// </summary>
            [Description("删除")]
            DelOrder = -1,
            /// <summary>
            /// 交易关闭
            /// </summary>
            [Description("交易关闭")]
            Trade_Closed = 0,
            /// <summary>
            /// 货到付款
            /// </summary>
            [Description("货到付款")]
            PayDelivery = 1,
            /// <summary>
            /// 未付款
            /// </summary>
            [Description("未付款")]
            NotPay = 2,
            /// <summary>
            /// 已付款
            /// </summary>
            [Description("已付款")]
            Paid = 3,
            /// <summary>
            /// 已发货
            /// </summary>
            [Description("已发货")]
            Delivered = 4,
            /// <summary>
            /// 确认收货
            /// </summary>
            [Description("确认收货")]
            Confirm = 5,
            /// <summary>
            /// 交易完成
            /// </summary>
            [Description("交易完成")]
            Finished = 6,
            /// <summary>
            /// 申请退货
            /// </summary>
            [Description("申请退货")]
            ReturnMaterial = 7,
            /// <summary>
            /// 不同意退货
            /// </summary>
            [Description("不同意退货")]
            ReturnRefuse = 8,
            /// <summary>
            /// 同意退货
            /// </summary>
            [Description("同意退货")]
            ReturnAgree = 9,
            /// <summary>
            /// 退货中
            /// </summary>
            [Description("退货中")]
            Returned = 10,
            /// <summary>
            /// 退货完成
            /// </summary>
            [Description("退货完成")]
            ReturnFinsh = 11,
            /// <summary>
            /// 退款完成
            /// </summary>
            [Description("退款完成")]
            ReturnMoney = 12,
            /// <summary>
            /// 已结算转账
            /// </summary>
            [Description("已结算转账")]
            Transfer = 13
        }

        /// <summary>
        /// 支付类型
        /// </summary>
        public enum PayType
        {
            /// <summary>
            /// 货到付款
            /// </summary>
            PayDelivery = 1,
            /// <summary>
            /// 支付宝支付
            /// </summary>
            Alipay = 2
        }

        /// <summary>
        /// 二维码类型，二维码编码规则中最后节点占用的枚举类型(码表中type值)
        /// </summary>
        public enum CodeType
        {
            /// <summary>
            /// 盒码
            /// </summary>
            gift = 1,
            /// <summary>
            /// 散码单品码
            /// </summary>
            single = 2,
            /// <summary>
            /// 套标产品码
            /// </summary>
            bSingle = 3,
            /// <summary>
            /// 套标箱码
            /// </summary>
            bGroup = 4,
            /// <summary>
            /// 抄重类产品码
            /// </summary>
            czMaterial = 5,
            /// <summary>
            /// 本地生成礼盒码
            /// </summary>
            localGift = 6,
            /// <summary>
            /// 散箱码
            /// </summary>
            boxCode = 7,
            /// <summary>
            /// 本地生成产品码
            /// </summary>               
            [Description("本地生成产品码")]
            localSingle = 8,
            /// <summary>
            /// 本地生成箱码
            /// </summary>               
            [Description("本地生成箱码")]
            localBox = 9,
            /// <summary>
            /// 追溯信息配置预览占用节点
            /// </summary>
            viewCode = 10,
            /// <summary>
            /// 预览二维码占用节点
            /// </summary>
            viewEwm = 11,
            /// <summary>
            /// 农药码
            /// </summary>
            pesticides = 12
        }

        /// <summary>
        /// 生成码记录中，RequestCode表中存Type的字段
        /// </summary>
        public enum GenCodeType
        {
            /// <summary>
            /// 盒码
            /// </summary>
            gift = 1,
            /// <summary>
            /// 散套标码
            /// </summary>
            //bGroup = 2,
            /// <summary>
            /// 散码单品码
            /// </summary>
            single = 3,
            /// <summary>
            /// 本地生成礼盒码
            /// </summary>
            localGift = 4,
            /// <summary>
            /// 箱码
            /// </summary>
            boxCode = 5,
            /// <summary>
            /// 本地生成产品码（代理服务器生成）
            /// </summary>
            localCreate = 6,
            /// <summary>
            /// 本地生成箱码（代理服务器生成）
            /// </summary>
            localCreateBox = 7,
            /// <summary>
            /// 营销红包码
            /// </summary>
            yingxiaoCode = 8,
            /// <summary>
            /// 套标码
            /// </summary>
            trap = 9,
            /// <summary>
            /// 农药码
            /// </summary>
            pesticides = 10
        }

        /// <summary>
        /// 活动类型
        /// </summary>
        public enum ActivityType
        {
            /// <summary>
            /// 普通打折活动
            /// </summary>
            Discount = 1,

            /// <summary>
            /// 付邮费免费送
            /// </summary>
            OnlyPostage = 2,

            /// <summary>
            /// 买一送一活动
            /// </summary>
            BuyOneGetOne = 3
        }

        /// <summary>
        /// 活动触发条件
        /// </summary>
        public enum Condition
        {
            /// <summary>
            /// 通过验证触发
            /// </summary>
            Verify = 1
        }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public enum StoreType
        {
            /// <summary>
            /// 仓库
            /// </summary>
            Store = 1,
            /// <summary>
            /// 货位
            /// </summary>
            Slotting = 2,
            /// <summary>
            /// 垛位
            /// </summary>
            Crib = 3
        }

        /// <summary>
        /// 企业审核状态
        /// </summary>
        public enum EnterpriseVerify
        {
            /// <summary>
            /// 审核不通过
            /// </summary>
            noPassVerify = -1,
            /// <summary>
            /// 暂停使用(停用)
            /// </summary>
            pauseVerify = -2,
            /// <summary>
            /// 测试用户（试用）
            /// </summary>
            Try = -3,
            /// <summary>
            /// 审核通过（正常）
            /// </summary>
            passVerify = 1,
            /// <summary>
            /// 未审核
            /// </summary>
            noVerify = 0,
            /// <summary>
            /// GS1企业
            /// </summary>
            gs1 =2
        }

        /// <summary>
        /// 二维码解析类型
        /// </summary>
        public enum AnalysisType
        {
            /// <summary>
            /// 未停止解析的产品
            /// </summary>
            AnalysisType = 1,
            /// <summary>
            /// 已停止解析的产品
            /// </summary>
            StopAnalysis = 2,
        }

        /// <summary>
        /// 时间类型
        /// </summary>
        public enum DateType
        {
            /// <summary>
            /// 前7天
            /// </summary>
            FirstSevenDay = 1,
            /// <summary>
            /// 前30天
            /// </summary>
            LastMonth = 2,
            /// <summary>
            /// 前6个月
            /// </summary>
            FirstSixMonth = 3
        }

        public enum TopType
        {
            /// <summary>
            /// 取消置顶
            /// </summary>
            Cancel = 0,
            /// <summary>
            /// 置顶
            /// </summary>
            Top = 1
        }

        /// <summary>
        /// 是否有帮助
        /// </summary>
        public enum UsefulType
        {
            /// <summary>
            /// 没有帮助
            /// </summary>
            No = 0,
            /// <summary>
            /// 有帮助
            /// </summary>
            Yes = 1
        }

        /// <summary>
        /// 展现
        /// </summary>
        public enum SettingShow
        {
            /// <summary>
            /// 产品
            /// </summary>
            [Description("展现产品信息")]
            Material = 1,
            /// <summary>
            /// 品牌
            /// </summary>
            [Description("展现品牌信息")]
            Brand = 2,
            /// <summary>
            /// 原料信息
            /// </summary>
            [Description("展现原材料信息")]
            Origin = 3,
            /// <summary>
            /// 作业信息
            /// </summary>
            [Description("展现生产信息")]
            Work = 4,
            /// <summary>
            /// 巡检信息
            /// </summary>
            [Description("展现巡检信息")]
            Check = 5,
            /// <summary>
            /// 检测报告
            /// </summary>
            [Description("展现检测报告")]
            Report = 6,
            /// <summary>
            /// 存储环境
            /// </summary>
            [Description("展现存储环境")]
            Ambient = 7,
            /// <summary>
            /// 物流信息
            /// </summary>
            [Description("展现物流信息")]
            WuLiu = 8
        }

        /// <summary>
        /// 显示
        /// </summary>
        public enum SettingDisplay
        {
            /// <summary>
            /// 生产日期
            /// </summary>
            [Description("显示生产日期")]
            CreateDate = 9,
            /// <summary>
            /// 怕码次数
            /// </summary>
            [Description("显示拍码次数")]
            ScanCount = 10
            ///// <summary>
            ///// 生产日期
            ///// </summary>
            //[Description("显示生产日期")]
            //CreateDate = 7,
            ///// <summary>
            ///// 怕码次数
            ///// </summary>
            //[Description("显示拍码次数")]
            //ScanCount = 8
            ///// <summary>
            ///// 验证码
            ///// </summary>
            //[Description("显示防伪验证码")]
            //Verification = 9
            ///// <summary>
            ///// 仓储信息
            ///// </summary>
            //[Description("显示仓储数据")]
            //WareHouse = 10,
            ///// <summary>
            ///// 物流信息
            ///// </summary>
            //[Description("显示流通数据")]
            //logistics = 11
        }

        /// <summary>
        /// 追溯模板样式
        /// </summary>
        public enum SettingSkin
        {
            /// <summary>
            /// 新样式
            /// </summary>
            [Description("模板一")]
            Normal = 0,
            /// <summary>
            /// 旧样式
            /// </summary>
            [Description("模板二")]
            mubaner = 1//,
            ///// <summary>
            ///// 模板三
            ///// </summary>
            //[Description("模板三")]
            //Three = 2,
            ///// <summary>
            ///// 模板四
            ///// </summary>
            //[Description("模板四")]
            //Four = 3,
            ///// <summary>
            ///// 模板五
            ///// </summary>
            //[Description("模板五")]
            //Five = 5,
            ///// <summary>
            ///// 防伪
            ///// </summary>
            //[Description("防伪模板")]
            //FangWei = 4
        }

        /// <summary>
        /// 防伪模板样式
        /// </summary>
        public enum SettingSkinFw
        {
            /// <summary>
            /// 正常的
            /// </summary>
            [Description("模板一")]
            Normal = 0
        }
        /// <summary>
        /// 解析查询依据
        /// </summary>
        public enum AnalysisBased
        {
            /// <summary>
            /// 原始查询（查询工艺）
            /// </summary>
            [Description("原始查询（查询工艺）")]
            Normal = 1,
            /// <summary>
            /// 查询销售批次
            /// </summary>
            [Description("查询销售批次")]
            Batch = 2,
            /// <summary>
            /// 根据自定义设置查询
            /// </summary>
            [Description("根据自定义设置查询")]
            Seting = 3,
            /// <summary>
            /// 预览二维码（11）
            /// </summary>
            viewEwm = 4,
            /// <summary>
            /// 设置预览码（10）
            /// </summary>
            setEwm = 5
        }

        /// <summary>
        /// 开通商城/开通客户端/开通追溯/开通子用户/开通仓储
        /// </summary>
        public enum ShopVerify
        {
            [Description("关闭")]
            Close = 0,
            [Description("开通")]
            Open = 1
        }

        /// <summary>
        /// 生成码类型（追溯码；防伪码）
        /// </summary>
        public enum RequestCodeType
        {
            /// <summary>
            /// 追溯码
            /// </summary>
            [Description("追溯码")]
            TraceCode = 1,
            /// <summary>
            /// 防伪码
            /// </summary>
            [Description("防伪码")]
            SecurityCode = 2,
            /// <summary>
            /// 防伪追溯码
            /// </summary>
            [Description("防伪追溯码")]
            fwzsCode = 3
        }

        public enum EnterpriseSwitch
        {
            /// <summary>
            /// 开通推广
            /// </summary>
            Recommend = 1
        }

        /// <summary>
        /// 二维码生成(web云端/蒙都本地)
        /// </summary>
        public enum Islocal
        {
            /// <summary>
            /// 云端生成
            /// </summary>
            cloud = 0,
            /// <summary>
            /// 本地生成
            /// </summary>
            local = 1
        }

        /// <summary>
        /// 记录类型
        /// </summary>
        public enum ComplaintType
        {
            Visitor = 1
        }

        /// <summary>
        /// 红包数量变动类型
        /// </summary>
        public enum RedChangeType
        {
            /// <summary>
            /// 发送红包导致数量减少1
            /// </summary>
            sendRed = 1,
            /// <summary>
            /// 发送零钱红包导致数量减少1
            /// </summary>
            sendChangeRed = 2,
            /// <summary>
            /// 查询发送状态，失败或者退回状态加1
            /// </summary>
            getRed = 3,
            /// <summary>
            /// 查询零钱红包发送状态，失败或者退回状态加1
            /// </summary>
            getChangeRed = 4
        }

        /// <summary>
        /// 红包发送类型
        /// </summary>
        public enum RedSendType
        {
            /// <summary>
            /// 红包
            /// </summary>
            red = 1,
            /// <summary>
            /// 零钱提现
            /// </summary>
            changeRed = 2
        }

        ///// <summary>
        ///// 接收二维码，激活二维码
        ///// </summary>
        //public enum RecEwm
        //{
        //    [Description("已接收，未激活")]
        //    已接收 = 1,
        //    [Description("已激活")]
        //    已激活 = 2
        //}
        #region 刘晓杰于2019年11月4日从CFBack项目移入此

        /// <summary>
        /// 接收二维码，激活二维码
        /// </summary>
        public enum RecEwm
        {
            [Description("激活失败")]
            激活失败 = 0,
            [Description("已接收，未激活")]
            已接收 = 1,
            [Description("已激活")]
            已激活 = 2,
            [Description("已自动入库")]
            已自动入库 = 3
        }

        #endregion

        /// <summary>
        /// 接收二维码，激活二维码
        /// </summary>
        public enum OperationType
        {
            [Description("后台")]
            后台 = 1,
            [Description("流水线")]
            流水线 = 2
        }

        public enum BindCodeType
        {
            /// <summary>
            /// tis客户端
            /// </summary>
            tis = 1,
            /// <summary>
            /// app客户端
            /// </summary>
            app = 2,
            /// <summary>
            /// 服务器
            /// </summary>
            server = 3

        }
        public enum BindCodeRecordsType
        {
            /// <summary>
            /// 装箱
            /// </summary>
            AddBox = 1,
            /// <summary>
            /// 装盒
            /// </summary>
            AddPack = 2,
            /// <summary>
            /// 装垛
            /// </summary>
            AddCrib = 3
        }

        /// <summary>
        /// 收集码包下载状态
        /// </summary>
        public enum DowloadStatus
        {
            /// <summary>
            /// 未下载
            /// </summary>
            unDowload = 0,
            /// <summary>
            /// 已下载
            /// </summary>
            dowLoad = 1
        }

        /// <summary>
        /// 20180803新加RequestCode表中CodeOfType状态
        /// IDCode码/简码
        /// </summary>
        public enum CodeOfType
        {
            /// <summary>
            /// IDcode码
            /// </summary>
            IDCode = 1,
            /// <summary>
            /// 简码
            /// </summary>
            SCode = 2,
            /// <summary>
            /// 农药码
            /// </summary>
            pesticides = 3
        }
        /// <summary>
        /// 二维码拆分类型
        /// </summary>
        public enum BatchPartType
        {
            /// <summary>
            /// 两种都可以支持拆分
            /// </summary>
            All = 0,
            /// <summary>
            /// 顺序拆分
            /// </summary>
            Split = 1,
            /// <summary>
            /// 自定义拆分
            /// </summary>
            Custom = 2
        }

        /// <summary>
        /// 医疗器械（申请码表中记录码是否已下载）
        /// </summary>
        public enum CodeISDowm
        {
            /// <summary>
            /// 未下载
            /// </summary>
            [Description("未下载")]
            NoDowm = 1,
            /// <summary>
            /// 已下载
            /// </summary>
            [Description("已下载")]
            IsDowm = 2
        }

        /// <summary>
        /// 医疗器械（二维码系统登录账号类型）
        /// </summary>
        public enum AccountType
        {
            /// <summary>
            /// 溯源平台账号
            /// </summary>
            [Description("溯源平台账号")]
            PlatAccount = 1,
            /// <summary>
            /// IDCode账号
            /// </summary>
            [Description("IDCode账号")]
            IDCodeAccount = 2
        }
        /// <summary>
        /// 企业是否激活
        /// </summary>
        public enum ActiveStatus
        {
            [Description("关闭")]
            关闭 = 0,
            [Description("激活")]
            激活 = 1
        }
        /// <summary>
        /// 企业授权方式
        /// </summary>
        public enum LicenseType
        {
            [Description("河北广联授权")]
            河北广联授权 = 1,
            [Description("二维码研究院授权")]
            二维码研究院授权 = 2,
            [Description("企业授权")]
            企业授权 = 3
        }
        /// <summary>
        /// 经销商级别
        /// </summary>
        public enum DealerLevel
        {
            /// <summary>
            /// 一级经销商
            /// </summary>
            One = 1
        }

        /// <summary>
        /// 经销商
        /// </summary>
        public enum DealerType
        {
            /// <summary>
            /// 一级
            /// </summary>
            First = 1,
            /// <summary>
            /// 下级
            /// </summary>
            Second = 2
        }
        //企业扩展表打码客户端用的简版/完整版
        public enum EnKHDType
        {
            /// <summary>
            /// 简版
            /// </summary>
            Simple = 1,
            /// <summary>
            /// 高级版
            /// </summary>
            Complete = 2,
            /// <summary>
            /// 标准版
            /// </summary>
            Standard = 3,
            /// <summary>
            /// 经营企业版
            /// </summary>
            JYEnterprise=4
        }

        public enum RequestISUpload
        {
            /// <summary>
            /// 未上传
            /// </summary>
            NotUploaded = 0,
            /// <summary>
            /// 已上传
            /// </summary>
            Uploaded = 1,
            /// <summary>
            /// 从发码机构下载来的
            /// </summary>
            DownIDCode = 2
        }
        public enum CreateType
        {
            /// <summary>
            /// 0、官网后台
            /// </summary>
            WebSite = 0,
            /// <summary>
            /// 1、接口：单次申请
            /// </summary>
            UpJieKou = 1
        }
        public enum CodingClientType
        {
            /// <summary>
            /// 2、GS1码
            /// </summary>
            GS1 = 2,
            /// <summary>
            /// 1、MA码
            /// </summary>
            MA   = 1
        }
        /// <summary>
        /// 出入库状态
        /// </summary>
        public enum IsOutStore
        {
            /// <summary>
            /// 企业入库
            /// </summary>
            inStore = 1,
            /// <summary>
            /// 企业出库
            /// </summary>
            outStore = 2,
            /// <summary>
            /// 入库补充
            /// </summary>
            inStoreIncrease = 3,
            /// <summary>
            /// 入库剔除
            /// </summary>
            inStoreReject = 4,
            /// <summary>
            /// 出库退返
            /// </summary>
            outStoreBack = 5,
            /// <summary>
            /// 经销商入库
            /// </summary>
            dealerInStore = 6,
            /// <summary>
            /// 经销商出库
            /// </summary>
            dealerOutStore = 7
        }

        public enum ServicesStatus
        {
            /// <summary>
            /// 未操作
            /// </summary>
            unpass=1,
            /// <summary>
            /// 已操作
            /// </summary>
            pass = 2,
            /// <summary>
            /// 已操作退返
            /// </summary>
            backPass = 3
        }

        public enum EnterpriseCodeType
        {
            /// <summary>
            /// Ma码
            /// </summary>
            MaCode = 1,
            /// <summary>
            /// GS1码
            /// </summary>
            GS1Code = 2,
            /// <summary>
            /// Ma码+GS1码
            /// </summary>
            MaGS1Code = 3
        }
    }
}
