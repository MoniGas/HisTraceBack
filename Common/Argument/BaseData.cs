using System.Collections.Generic;
using LinqModel;

namespace Common.Argument
{
    public class BaseData
    {
        public static AreaInfo listAddress = new AreaInfo();
        public static TradeInfo listTrade = new TradeInfo();
        public static UnitTypeInfo unitType = new UnitTypeInfo();
        public static List<LinqModel.PRRU_PlatFormLevel> listPRRU_PlatFormLevel = new List<LinqModel.PRRU_PlatFormLevel>();
        public static List<LinqModel.PRRU_Modual> listPRRU_Modual = new List<LinqModel.PRRU_Modual>();
        public static List<LinqModel.Dictionary_MaterialType> listMaterialType = new List<LinqModel.Dictionary_MaterialType>();
        public static List<LinqModel.Route_Server> listRoute_Server = new List<LinqModel.Route_Server>();
        //品类编码对应的简码
        public static Dictionary<string, string> categoryScode = new Dictionary<string, string>();
            //品类编码对应的简码

        //public static List<LinqModel.Language> listLanguage = new List<LinqModel.Language>();
        //public static List<LinqModel.ShowTemplate> listTemplate = new List<LinqModel.ShowTemplate>();
        //public static List<LinqModel.View_Dictionary> listViewDictionary = new List<LinqModel.View_Dictionary>();
        //public static List<LinqModel.Dictionary_HangYe> listDictionaryHangYe = new List<LinqModel.Dictionary_HangYe>();

        public static InterFaceHisUnitType HisunitType = new InterFaceHisUnitType();

        public static InterFaceHisIndustryCategory HisCategory = new InterFaceHisIndustryCategory();
    }
}
