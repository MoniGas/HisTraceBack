using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using Dal;

namespace BLL
{
	public class UDIAnalyseBLL
	{
		UDIAnalyseDAL dal = new UDIAnalyseDAL();

		public UDIKey getUDIKey(string key) 
		{
			return dal.getUDIKey(key);
		}

		public int getUDIAnalyseCount(long keyID) 
		{
			return dal.getUDIAnalyseCount(keyID);
		}

		public UDIMaterial getUDIMaterial(string DIInfo, string CodeTypeName) 
		{
			return dal.getUDIMaterial(DIInfo, CodeTypeName);
		}

		public UDIAnalyseResult UpdateUDIAnalyse(UDIMaterial model, long keyID) 
		{
			return dal.UpdateUDIAnalyse(model, keyID);
		}

	}
}
