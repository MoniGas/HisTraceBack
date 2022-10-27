using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel
{
    public class InStatisModel
    {
        public long IntStorageID { get; set; }
        public string StrIntStorageDate { get; set; }
        public DateTime? IntStorageDate { get; set; }
        public long? StorageHouse { get; set; }
        public long? DealerID { get; set; }
        public string IntStorageNO { get; set; }
        public int? EquipType { get; set; }
        public long? EnterpriseID { get; set; }
        public string StoreName { get; set; }
        public string DealerName { get; set; }
        public string DeviceName { get; set; }
        public int? MaCount { get; set; }//产品数量
    }
    public class OutStatisModel
    {
        public long OutStorageID { get; set; }
        public string StrOutStorageDate { get; set; }
        public DateTime? OutStorageDate { get; set; }
        public long? StorageHouse { get; set; }
        public long? EnterpriseID { get; set; }
        public long? DealerID { get; set; }
        public string StoreName { get; set; }
        public string DealerName { get; set; }
        public string DeviceName { get; set; }
        public string OutStorageNO { get; set; }
        public int? EquipType { get; set; }
        public long? MaCount { get; set; }//产品数量
    }
}
