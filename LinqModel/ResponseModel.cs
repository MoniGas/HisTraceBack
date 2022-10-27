/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-6-7
** 联系方式:15533621896
** 代码功能：调用接口响应类
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

namespace LinqModel
{
    public class ResponseCanGetRedPacketModel
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        public string Msg { get; set; }
    }

    /// <summary>
    /// 更新配置码表状态
    /// </summary>
    public class ResponseSettingCodeModel
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        public string Msg { get; set; }
    }
}
