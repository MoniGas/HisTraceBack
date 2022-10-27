using System;
using System.Web.Services;
using LinqModel;

namespace web
{
    /// <summary>
    /// Service 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {
        /// <summary>
        /// 调取下位机数据
        /// </summary>
        /// <param name="enterpriseCode">企业代码</param>
        /// <param name="enterpriseName">企业名称</param>
        /// <param name="userCode">用户代码</param>
        /// <param name="greenhousesCode">大棚代码</param>
        /// <param name="loweMachineAddress">下位机地址</param>
        /// <param name="probeAddress">下位机安装位置</param>
        /// <param name="collectTime">下位机时间</param>
        /// <param name="value1">采集数据1如：温度:30:单位</param>
        /// <param name="value2">采集数据2</param>
        /// <param name="value3">采集数据3</param>
        /// <param name="value4">采集数据4</param>
        /// <param name="value5">采集数据5</param>
        /// <param name="value6">采集数据6</param>
        /// <param name="value7">采集数据7</param>
        /// <param name="value8">采集数据8</param>
        /// <param name="value9">采集数据9</param>
        /// <param name="value10">采集数据10</param>
        /// <param name="Msg">返回参数：0表示成功 1传入数据不完整 2未知原因 4网络原因（1接收参数错误 2未知原因 3数据库操作失败 4网络原因 5传入参数错误）</param>
        /// <returns></returns>
        [WebMethod]
        public Boolean GetProbeData(string enterpriseCode, string enterpriseName, string userCode, string greenhousesCode, string loweMachineAddress,
            string probeAddress, string collectTime, string value1, string value2, string value3, string value4, string value5, string value6,
            string value7, string value8, string value9, string value10, out int Msg)
        {
            Msg = 2;
            Boolean result = false;
            ProdeData table = new ProdeData();
            try
            {
                table.loweMachineAddress = loweMachineAddress;
                table.collectTime = collectTime;
                table.enterpriseName = enterpriseName;
                table.enterpriseCode = enterpriseCode;
                table.gId = greenhousesCode;
                table.userId = userCode;
                string[] kv = null;
                if (!string.IsNullOrEmpty(value1))
                {
                    kv = value1.Split(':');
                    table.key1 = kv[0];
                    table.value1 = kv[1];
                    table.unit1 = kv[2];
                }
                if (!string.IsNullOrEmpty(value10))
                {
                    kv = value10.Split(':');
                    table.key10 = kv[0];
                    table.value10 = kv[1];
                    table.unit10 = kv[2];
                }
                if (!string.IsNullOrEmpty(value2))
                {
                    kv = value2.Split(':');
                    table.key2 = kv[0];
                    table.value2 = kv[1];
                    table.unit2 = kv[2];
                }
                if (!string.IsNullOrEmpty(value3))
                {
                    kv = value3.Split(':');
                    table.key3 = kv[0];
                    table.value3 = kv[1];
                    table.unit3 = kv[2];
                }
                if (!string.IsNullOrEmpty(value4))
                {
                    kv = value4.Split(':');
                    table.key4 = kv[0];
                    table.value4 = kv[1];
                    table.unit4 = kv[2];
                }
                if (!string.IsNullOrEmpty(value5))
                {
                    kv = value5.Split(':');
                    table.key5 = kv[0];
                    table.unit5 = kv[2];
                    table.value5 = kv[1];
                }
                if (!string.IsNullOrEmpty(value6))
                {
                    kv = value6.Split(':');
                    table.key6 = kv[0];
                    table.value6 = kv[1];
                    table.unit6 = kv[2];
                }
                if (!string.IsNullOrEmpty(value7))
                {
                    kv = value7.Split(':');
                    table.key7 = kv[0];
                    table.value7 = kv[1];
                    table.unit7 = kv[2];
                }
                if (!string.IsNullOrEmpty(value8))
                {
                    kv = value8.Split(':');
                    table.key8 = kv[0];
                    table.value8 = kv[1];
                    table.unit8 = kv[2];
                }
                if (!string.IsNullOrEmpty(value9))
                {
                    kv = value9.Split(':');
                    table.key9 = kv[0];
                    table.value9 = kv[1];
                    table.unit9 = kv[2];
                }

                if (string.IsNullOrEmpty(table.enterpriseCode))
                {
                    Msg = 1;
                }
                else if (string.IsNullOrEmpty(table.enterpriseName))
                {
                    Msg = 1;
                }
                if (string.IsNullOrEmpty(table.userId))
                {
                    Msg = 1;
                }
                else if (string.IsNullOrEmpty(table.gId))
                {
                    Msg = 1;
                }
                else if (string.IsNullOrEmpty(table.loweMachineAddress))
                {
                    Msg = 1;
                }
                else if (string.IsNullOrEmpty(table.collectTime))
                {
                    Msg = 1;
                }
                else
                {
                    result = new BLL.GreenhouseBLL().AddProdeData(table, out Msg);
                }
            }
            catch
            {
                Msg = 1;
            }
            return result;
        }
    }
}
