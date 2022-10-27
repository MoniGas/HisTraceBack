using System;
using System.Collections.Generic;

namespace LinqModel
{
    public class ResultInfo
    {
        /// <summary>
        /// 返回结果（1正确，0不正确）
        /// </summary>		
        public int result_code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>		
        public string result_msg { get; set; }

        public List<MBInfo> data { get; set; }
    }

    public class MBInfo
    {
        public string mb { get; set; }
        public string reg_Date { get; set; }
        public string start_Date { get; set; }
        public string end_Date { get; set; }
        public string unit_Name { get; set; }
        public string unit_Phone { get; set; }
        public string agent_Name { get; set; }
    }
}
