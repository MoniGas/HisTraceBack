using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqModel;

namespace LinqModel
{
    public class UDIMaterialInfo
    {
        public UDIMaterialInfo()
        {
            pageNum = 0;
        }
        public int pageNum { get; set; }
        public List<UDIMaterial> UDIData { get; set; }
    }
}