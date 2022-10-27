using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqModel.InterfaceModels
{

    public class MaterialResponse
    {
        public List<Categoryentitylist> categoryEntityList { get; set; }
        public List<Materialentitylist> materialEntityList { get; set; }
        public List<Materialdientitylist> materialdiEntityList { get; set; }
    }

    public class Categoryentitylist
    {
        public string addTime { get; set; }
        public string categoryCode { get; set; }
        public int categoryID { get; set; }
        public long iD { get; set; }
        public int isupload { get; set; }
        public int materialID { get; set; }
        public string materialName { get; set; }
        public int status { get; set; }
        public long enterpriseInfoID { get; set; }
    }

    public class Materialentitylist
    {
        public int adduser { get; set; }
        public int bZSpecType { get; set; }
        public int enterpriseInfoID { get; set; }
        public int isZengPin { get; set; }
        public int materialBarcode { get; set; }
        public string materialCode { get; set; }
        public string materialFullName { get; set; }
        public int materialID { get; set; }
        public string materialName { get; set; }
        public int status { get; set; }
        public int type { get; set; }
    }

    public class Materialdientitylist
    {
        public int adduser { get; set; }
        public string categoryCode { get; set; }
        public int createtype { get; set; }
        public int enterpriseID { get; set; }
        public string hisCode { get; set; }
        public int iD { get; set; }
        public int iSUpload { get; set; }
        public int materialID { get; set; }
        public string materialName { get; set; }
        public string materialUDIDI { get; set; }
        public string materialXH { get; set; }
        public string specLevel { get; set; }
        public int specNum { get; set; }
        public string specificationName { get; set; }
        public string specifications { get; set; }
        public int status { get; set; }
    }

}
