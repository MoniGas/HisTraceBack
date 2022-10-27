
namespace Common.Argument
{
    public class JsonInfo
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        private int _result;
        public int result
        {
            get { return _result; }
            set { _result = value; }
        }

        /// <summary>
        /// 原因
        /// </summary>
        private string _msg;
        public string msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

    }
}
