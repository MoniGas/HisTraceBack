using System;
using System.Configuration;

namespace Common.Argument
{
    public class ResultArg
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        private int _pageSize = Convert.ToInt16(ConfigurationManager.AppSettings["PageSize"]);
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize 
        {
            get { return _pageSize; }
            set { _pageSize = value; } 
        }
        /// <summary>
        /// 总数量
        /// </summary>
        public long TotalCount { get; set; }
        /// <summary>
        /// 返回列表
        /// </summary>
        public Object ObjList { get; set; }
        /// <summary>
        /// 返参结果错误类型，默认为无错误
        /// </summary>
        public CmdResultError CmdError { get; set; }
        /// <summary>
        /// 操作结果提示
        /// </summary>
        public string Msg { get; set; }
    }
}
