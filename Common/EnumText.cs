using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using LinqModel;
namespace Common
{
    public static class EnumText
    {
        /// <summary>
        /// 活动方式
        /// </summary>
        public enum ActivityMethod
        {
            /// <summary>
            /// 红包活动
            /// </summary>
            [Description("红包活动")]
            Packet = 1,
            /// <summary>
            /// 优惠券活动
            /// </summary>
            [Description("优惠券活动")]
            Coupon = 2,
            /// <summary>
            /// 大转盘活动
            /// </summary>
            [Description("大转盘")]
            Lottery = 3
        }

        /// <summary>
        /// 优惠券
        /// </summary>
        public enum CouponType
        {
            /// <summary>
            /// 现金券
            /// </summary>
            [Description("现金券")]
            Cash=1,
            /// <summary>
            /// 折扣券
            /// </summary>
            [Description("折扣券")]
            Discount=2,
            /// <summary>
            /// 礼品券
            /// </summary>
            [Description("礼品券")]
            Gift=3
        }

        /// <summary>
        /// 生成码类型
        /// </summary>
        public enum RequestCodeType
        {
            /// <summary>
            /// 套标产品码
            /// </summary>
            [Description("套标产品码")]
            Soket = 1,
            /// <summary>
            /// 单品产品码
            /// </summary>
            [Description("单品产品码")]
            Material = 2
        }

        /// <summary>
        /// 基础数据类型
        /// </summary>
        public enum StyleType
        {
            /// <summary>
            /// 系统模板
            /// </summary>
            [Description("系统模板")]
            System = 1,
            /// <summary>
            /// 企业模板
            /// </summary>
            [Description("企业模板")]
            Enterprise = 2
        }

        /// <summary>
        /// 码类型
        /// </summary>
        public enum CodeType
        {
            /// <summary>
            /// 追溯码
            /// </summary>
            [Description("追溯码")]
            TraceCode = 1,
            /// <summary>
            /// 营销码
            /// </summary>
            [Description("营销码")]
            MarketCode = 2
        }

        /// <summary>
        /// 活动状态
        /// </summary>
        public enum ActivityState
        {
            /// <summary>
            /// 活动未开始
            /// </summary>
            NoStart = -1,
            /// <summary>
            /// 活动进行中
            /// </summary>
            Going = 1,
            /// <summary>
            /// 活动已结束
            /// </summary>
            Finish = 2,
            /// <summary>
            /// 活动作废
            /// </summary>
            Disabled = 3
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public enum OrderState
        {
            /// <summary>
            /// 已支付
            /// </summary>
            [Description("已支付")]
            Payed = 1,
            /// <summary>
            /// 未支付
            /// </summary>
            [Description("未支付")]
            NoPay = 2,
            /// <summary>
            /// 未审核
            /// </summary>
            [Description("未审核")]
            NoLook = 3,
            /// <summary>
            /// 审核中
            /// </summary>
            [Description("审核中")]
            Auditing = 4
        }

        /// <summary>
        /// 企业支付状态
        /// </summary>
        public enum PayState
        {
            /// <summary>
            /// 未支付
            /// </summary>
            [Description("未支付")]
            NoPay = 1,
            /// <summary>
            /// 已支付
            /// </summary>
            [Description("已支付")]
            Payed = 2
        }

        /// <summary>
        /// 开启模式
        /// </summary>
        public enum OpenMode
        {
            [Description("未开启")]
            NoOpen = -1,
            [Description("自动")]
            Auto = 0,
            [Description("手动")]
            Hand = 1
        }

        /// <summary>
        /// 活动类型
        /// </summary>
        public enum ActiveType
        {
            /// <summary>
            /// 一码多个红包(按码领取)
            /// </summary>
            [Description("按码领取")]
            Multi = 1,
            /// <summary>
            /// 一码一个红包
            /// </summary>
            [Description("按用户领取")]
            Single = 2
        }

        /// <summary>
        /// 参与方式
        /// </summary>
        public enum JoinMode
        {
            /// <summary>
            /// 需微信授权登录
            /// </summary>
            [Description("需微信授权登录")]
            Condition = 1,
            /// <summary>
            /// 不需微信授权登录
            /// </summary>
            [Description("不需微信授权登录")]
            Single = 2
        }

        /// <summary>
        /// 状态通用
        /// </summary>
        public enum Status : int
        {
            /// <summary>
            /// 已删除
            /// </summary>
            delete = -1,
            /// <summary>
            /// 正常
            /// </summary>
            used = 0
        }

        /// <summary>
        /// 状态通用
        /// </summary>
        public enum LoginType : int
        {
            /// <summary>
            /// 追溯管理创建红包
            /// </summary>
            parameter = 0,
            /// <summary>
            /// 直接创建红包
            /// </summary>
            common = 1
        }

        /// <summary>
        /// 发送类型
        /// </summary>
        public enum SendType
        {
            /// <summary>
            /// 通过API接口发放 
            /// </summary>
            API = 1,
            /// <summary>
            /// 通过上传文件方式发放 
            /// </summary>
            UPLOAD = 2,
            /// <summary>
            /// 通过活动方式发放
            /// </summary>
            ACTIVITY = 3,
        }

        /// <summary>
        /// 红包类型
        /// </summary>
        public enum HbType
        {
            /// <summary>
            /// 裂变红包
            /// </summary>
            GROUP = 1,
            /// <summary>
            /// 普通红包
            /// </summary>
            NORMAL = 2
        }

        /// <summary>
        /// 红包领取状态
        /// </summary>
        public enum GetState
        {
            /// <summary>
            /// 删除
            /// </summary>
            Delete=-1,
            /// <summary>
            /// 发放中
            /// </summary>
            [Description("发放中")]
            SENDING = 1,
            /// <summary>
            /// 已发放待领取 
            /// </summary>
            [Description("已发放待领取")]
            SENT = 2,
            /// <summary>
            /// 发放失败
            /// </summary>
            [Description("发放失败")]
            FAILED = 3,
            /// <summary>
            /// 已领取
            /// </summary>
            [Description("已领取")]
            RECEIVED = 4,
            /// <summary>
            /// 退款中
            /// </summary>
            [Description("退款中")]
            RFUND_ING = 5,
            /// <summary>
            /// 已退款
            /// </summary>
            [Description("已退款")]
            REFUND = 6,
            /// <summary>
            /// 零钱红包已发放待领取 
            /// </summary>
            [Description("零钱已发放待领取")]
            SENTChange = 7
        }

        public enum ActivityId
        {
            /// <summary>
            /// 提现时活动编号为-1
            /// </summary>
            Tx=-1,
            /// <summary>
            /// 删除时活动编号为-2
            /// </summary>
            Delete=-2
        }
        /// <summary>
        /// 性别说明
        /// </summary>
        public enum Sex : int
        {
            /// <summary>
            /// 未知
            /// </summary>
            No = 0,
            /// <summary>
            /// 男
            /// </summary>
            Man = 1,
            /// <summary>
            /// 女
            /// </summary>
            WoMan = 2
        }

        /// <summary>
        /// 二维码套餐状态
        /// </summary>
        public enum PackageStatus : int
        {
            /// <summary>
            /// 全部
            /// </summary>
            all = 0,
            /// <summary>
            /// 启用
            /// </summary>
            qiyong = 1,
            /// <summary>
            /// 禁用
            /// </summary>
            jinyong = -1
        }

        public enum PayType
        {
            /// <summary>
            /// 支付宝
            /// </summary>
            [Description("支付宝")]
            OlineAlipay = 1,
            /// <summary>
            /// 微信 
            /// </summary>
            [Description("微信")]
            WeiXinPay = 2,
            /// <summary>
            /// 线下支付
            /// </summary>
            [Description("线下支付")]
            OffLinePay = 3,
            /// <summary>
            /// 线下支付
            /// </summary>
            [Description("自主支付")]
            IndependentPay = 4
        }

        public enum UserState
        {
            /// <summary>
            /// 用完
            /// </summary>
            [Description("用完")]
            Used = 1,
            /// <summary>
            /// 未用
            /// </summary>
            [Description("未用")]
            NoUsed = 2,
            /// <summary>
            /// 用部分
            /// </summary>
            [Description("用部分")]
            UserPart = 3,
        }

        #region 获取枚举描述信息
        /// <summary>
        /// 获取枚举描述信息
        /// </summary>
        /// <param name="t">枚举</param>
        /// <param name="value">值</param>
        /// <returns>描述信息</returns>
        public static string EnumToText(Type t, int? value)
        {
            if (value == null || Enum.GetName(t, value)==null)
            {
                return "";
            }

            FieldInfo info = t.GetField(Enum.GetName(t, value));
            DescriptionAttribute description = (DescriptionAttribute)Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute));
            return description.Description;
        }
        #endregion

        #region 获取枚举列表
        /// <summary>
        /// 获取枚举列表
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="title">第一行显示数据 例：请选择 全部 没有忽略该参数</param>
        /// <returns>枚举列表</returns>
        public static List<EnumList> EnumToList(Type t, string title = "")
        {
            List<EnumList> result = new List<EnumList>();
            if (!string.IsNullOrEmpty(title))
            {
                EnumList list = new EnumList();
                list.value = "-100";
                list.text = title;
                list.mubanimg = "";
                result.Add(list);
            }

            foreach (int item in Enum.GetValues(t))
            {
                EnumList list = new EnumList();
                list.value = item.ToString();
                list.text = EnumToText(t, item);
                if (list.text == "模板一")
                {
                    list.mubanimg = "/newmenu/images/moban1.bmp";
                }
                else if (list.text == "模板二")
                {
                    list.mubanimg = "/newmenu/images/moban2.bmp";
                }
                //else if (list.text == "模板三")
                //{
                //    list.mubanimg = "/newmenu/images/muban3.bmp";
                //}
                //else if (list.text == "模板四")
                //{
                //    list.mubanimg = "/newmenu/images/muban4.bmp";
                //}
                //else if (list.text == "模板五")
                //{
                //    list.mubanimg = "/newmenu/images/muban5.png";
                //}
                //else if (list.text == "防伪模板")
                //{
                //    list.mubanimg = "/newmenu/images/secCode.png";
                //}
                else
                {
                    list.mubanimg = "";
                }
                result.Add(list);
            }
            return result;
        }

        /// <summary>
        /// 获取枚举列表
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="title">第一行显示数据 例：请选择 全部 没有忽略该参数</param>
        /// <returns>枚举列表</returns>
        public static List<EnumList> EnumToListFw(Type t, string title = "")
        {
            List<EnumList> result = new List<EnumList>();
            if (!string.IsNullOrEmpty(title))
            {
                EnumList list = new EnumList();
                list.value = "-100";
                list.text = title;
                list.mubanimg = "";
                result.Add(list);
            }

            foreach (int item in Enum.GetValues(t))
            {
                EnumList list = new EnumList();
                list.value = item.ToString();
                list.text = EnumToText(t, item);
                if (list.text == "模板一")
                {
                    list.mubanimg = "/newmenu/images/moban2.png";
                }
                //else if (list.text == "模板二")
                //{
                //    list.mubanimg = "/newmenu/images/moban1.jpg";
                //}
                else
                {
                    list.mubanimg = "";
                }
                result.Add(list);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 优惠券核销状态
        /// </summary>
        public enum CancelOutStatus
        {
            /// <summary>
            /// 红包活动
            /// </summary>
            [Description("已核销")]
            YCancelOut = 1,
            /// <summary>
            /// 优惠券活动
            /// </summary>
            [Description("未核销")]
            WCancelOut = 2
        }
        public enum SendState
        {
            /// <summary>
            /// 更新中
            /// </summary>
            Sending=1,
            /// <summary>
            /// 完成
            /// </summary>
            Complete=2
        }
        public enum RedStyle
        {
            /// <summary>
            /// 抢
            /// </summary>
            [Description("抢")]
            抢 = 1,
            /// <summary>
            /// 藏
            /// </summary>
            [Description("藏")]
            藏 = 2
        }
        #region 是否已经配置藏红包/优惠券完成
        /// <summary>
        /// 是否已经配置藏红包/优惠券完成
        /// </summary>
        public enum SetStatus : int
        {
            /// <summary>
            /// 未配置
            /// </summary>
            NoSet = 1,
            /// <summary>
            /// 已配置
            /// </summary>
            IsSet = 2
        }
        #endregion

        #region 大转盘活动领取状态
        public enum ReceiveStatus : int
        {
            /// <summary>
            /// 未领取
            /// </summary>
            NoReceive = 1,
            /// <summary>
            /// 已领取
            /// </summary>
            IsReceive = 2
        }
        #endregion
    }
}