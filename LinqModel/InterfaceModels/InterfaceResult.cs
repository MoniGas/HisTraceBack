using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{
    /// <summary>
    /// 返回消息
    /// </summary>
    public class InterfaceResult
    {
        //返回代码(0：成功,1：token失效，请重新获取)
        public int retCode { get; set; }
        //返回消息
        public string retMessage { get; set; }
        //是否成功
        public bool isSuccess { get; set; }
        //返回内容
        public object retData { get; set; }
    }
}
