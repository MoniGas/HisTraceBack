using System;

namespace LinqModel
{
    #region 返回类
    public class BaseResultList
    {
        public BaseResultList()
        {
            pageIndex = 1;
            pageSize = 20;
            totalCounts = 0;
            Msg = "没有数据";
        }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public long totalCounts { get; set; }
        public string Msg { get; set; }
        public object ObjList { get; set; }
        public object ObjModel { get; set; }
    }
    public class BaseResultModel
    {
        public BaseResultModel()
        {
            code = "-1";
            Msg = "没有数据";
        }
        public string code { get; set; }
        public string Msg { get; set; }
        public object ObjModel { get; set; }
    }
    public class ExtenResultModel
    {
        public ExtenResultModel()
        {
            code = "-1";
            Msg = "没有数据";
        }
        public string code { get; set; }
        public string Msg { get; set; }
        public object Modual { get; set; }
        public object Login { get; set; }
    }
    #endregion

    #region 接口对接类
    public class ProdeData
    {
        /// <summary>
        /// 企业二维码
        /// </summary>
        public string enterpriseCode { get; set; }
        public string enterpriseName { get; set; }
        public string loweMachineAddress { get; set; }
        public string collectTime { get; set; }
        /// <summary>
        /// 用户二维码
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 大棚二维码
        /// </summary>
        public string gId { get; set; }

        public string key1 { get; set; }
        public string value1 { get; set; }
        public string unit1 { get; set; }

        public string key2 { get; set; }
        public string value2 { get; set; }
        public string unit2 { get; set; }

        public string key3 { get; set; }
        public string value3 { get; set; }
        public string unit3 { get; set; }

        public string key4 { get; set; }
        public string value4 { get; set; }
        public string unit4 { get; set; }

        public string key5 { get; set; }
        public string value5 { get; set; }
        public string unit5 { get; set; }

        public string key6 { get; set; }
        public string value6 { get; set; }
        public string unit6 { get; set; }

        public string key7 { get; set; }
        public string value7 { get; set; }
        public string unit7 { get; set; }

        public string key8 { get; set; }
        public string value8 { get; set; }
        public string unit8 { get; set; }

        public string key9 { get; set; }
        public string value9 { get; set; }
        public string unit9 { get; set; }

        public string key10 { get; set; }
        public string value10 { get; set; }
        public string unit10 { get; set; }
    }
    public class EWMInfo
    {
        public string ewm { get; set; }
        public string infoName { get; set; }
        public string fullewm { get; set; }
    }

    /// <summary>
    /// 消息主体
    /// </summary>
    public class MsgInfo
    {
        public MsgInfo()
        {
            sms = new Sms();
            phoneNumbers = new String[1];
        }
        public string secretKey { get; set; }
        public Sms sms { get; set; }
        public string[] phoneNumbers { get; set; }
    }

    /// <summary>
    /// 短信内容
    /// </summary>
    public class Sms
    {
        public string sendDate { get; set; }
        public string content { get; set; }
        public int msType { get; set; }
        public string sendTime { get; set; }
    }

    /// <summary>
    /// 发短信后服务返回的Json串
    /// </summary>
    public class MsgReturn
    {
        /// <summary>
        /// 发送成功数量
        /// </summary>
        public string phonesSend { get; set; }
        /// <summary>
        /// 本次发送任务的ID
        /// </summary>
        public string jobID { get; set; }
        /// <summary>
        /// 错误号  0 为成功
        /// </summary>
        public string errorCode { get; set; }
        /// <summary>
        /// 发送失败的手机号码
        /// </summary>
        public string errPhones { get; set; }
    }
    #endregion
}
