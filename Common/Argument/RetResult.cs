using System;
using System.Text;

namespace Common.Argument
{
    /// <summary>
    /// 返参结果实例
    /// </summary>
    public class RetResult
    {
        /// <summary>
        /// 操作结果提示
        /// </summary>
        public string IDcodeewm
        {
            get { return _RetMemo; }
            set { _RetMemo = value; }
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回结果id20190212新加
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 结果引导参数;
        /// 当值为false时,基类RetResult不作为
        /// </summary>
        bool _RetGuide = false;

        /// <summary>
        /// 返参结果错误类型，默认为无错误
        /// </summary>
        private CmdResultError _CmdError = CmdResultError.NONE;

        /// <summary>
        /// 增删改数目值（增加返回ID，修改和删除返回影响行数）
        /// </summary>
        private long _CrudCount = 0;

        /// <summary>
        /// 操作结果提示
        /// </summary>
        private string _RetMemo = String.Empty;

        /// <summary>
        /// 异常错误
        /// </summary>
        private string _Msg = string.Empty;

        #region 属生
        /// <summary>
        /// 结果引导参数;
        /// 当值为false时,基类RetResult不作为
        /// </summary>
        public bool RetGuide
        {
            get { return _RetGuide; }
            set { _RetGuide = value; }
        }

        /// <summary>
        /// 返参结果错误类型，默认为无错误
        /// </summary>
        public CmdResultError CmdError
        {
            get { return _CmdError; }
            set { _CmdError = value; }
        }

        /// <summary>
        /// 增删改数目值（增加返回ID，修改和删除返回影响行数）
        /// </summary>
        public long CrudCount
        {
            get { return _CrudCount; }
            set { _CrudCount = value; }
        }

        /// <summary>
        /// 操作结果提示
        /// </summary>
        public string Msg
        {
            get { return _Msg; }
            set { _Msg = value; }
        }

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                switch (_CmdError)
                {
                    case CmdResultError.NONE:
                        return true;
                    case CmdResultError.NO_RESULT:
                        _RetMemo = "没有查询到满足条件的结果";
                        return true;
                    case CmdResultError.PARAMERROR:
                    case CmdResultError.NO_RIGHT:
                    case CmdResultError.EXCEPTION:
                    case CmdResultError.Other:
                        return false;
                    default:
                        return false;
                }
            }
        }

        #endregion


        public RetResult()
        {
            _CmdError = CmdResultError.EXCEPTION;//默认为错误
            _RetMemo = "操作正确";
        }

        /// <summary>
        /// 给参数赋值方法
        /// </summary>
        /// <param name="cmdError">操作结果类型</param>
        /// <param name="errorMemo">异常错误</param>
        /// <param name="msg">操作结果描述</param>
        public void SetArgument(CmdResultError cmdError, string errorMemo, string msg)
        {
            _CmdError = cmdError;
            _Msg = msg;
            _RetMemo = errorMemo;
            if (cmdError == CmdResultError.EXCEPTION || cmdError == CmdResultError.PARAMERROR)
            {
                Common.Log.WriteLog.WriteErrorLog(GetErrorLog().ToString());
            }
        }


        /// <summary>
        /// 获取错误日志消息
        /// </summary>
        /// <returns></returns>
        public StringBuilder GetErrorLog()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (_CmdError != CmdResultError.NONE && _CmdError != CmdResultError.NO_RESULT)
                    sb.Append("【" + System.DateTime.Now.ToString() + "】操作结果类型：" + (int)_CmdError + " ，结果描述：" + _RetMemo + "，返回值：" + _CrudCount);
            }
            catch
            {
            }
            return sb;
        }

    }

    public enum CmdResultError : int
    {
        /// <summary>
        /// 初始化状态,未进入到工作状态，表明业务操作在未执行前出现不可预计的异常
        /// </summary>
        UNREG = -1,
        /// <summary>
        /// 无错误信息，说明操作正常结果
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 异常错误信息
        /// </summary>
        EXCEPTION = 1,
        /// <summary>
        /// 参数错误
        /// </summary>
        PARAMERROR = 2,
        /// <summary>
        /// 无错误信息，但是没满足条件结果
        /// </summary>
        NO_RESULT = 3,
        /// <summary>
        /// 无权限执行
        /// </summary>
        NO_RIGHT = 4,

        /// <summary>
        /// 不写日志返回false
        /// </summary>
        Other = 5
    }

    #region 刘晓杰于2019年11月4日从CFBack项目移入此

    public class RetResults
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回
        /// </summary>
        public long ID { get; set; }
    }

    #endregion

    #region 企业操作返回结果  2020年2月4日 刘晓杰
    public class RetEnterpriseResult
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回企业ID
        /// </summary>
        public long EnterpriseId { get; set; }

        /// <summary>
        /// 企业管理员账号ID
        /// </summary>
        public long AdminId { get; set; }

        /// <summary>
        /// 企业授权截止日期
        /// </summary>
        public string LicenseEndDate { get; set; }
        /// <summary>
        /// 1:简单版；2：高级版；3：标准版
        /// </summary>
        public int? IsSimple { get; set; }
    }
    #endregion
}
