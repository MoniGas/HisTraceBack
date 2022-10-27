using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dal;
using LinqModel.InterfaceModels;
using LinqModel;

namespace BLL
{
	public class UdiBLL
	{
		UdiDAL dal = new UdiDAL();
		public InterfaceResult DILst(DILstRequestParam Param, string accessToken) 
		{
			return dal.DILst(Param,accessToken);
		}

		public InterfaceResult PILst(PILstRequestParam Param, string accessToken)
		{
			return dal.PILst(Param, accessToken);
		}

		public InterfaceResult PIDetail(PIDetailRequestParam Param, string accessToken)
		{
			return dal.PIDetail(Param, accessToken);
		}
		public InterfaceResult SaveDI(SaveDIRequestParam Param, string accessToken)
		{
			return dal.SaveDI(Param, accessToken);
		}

		public UDIAnalyseResult UDIAnalyse(string CodeValue, string key, int? CodeFormat)
		{
			return dal.UDIAnalyse(CodeValue, key, CodeFormat);
		}
		public UDIBindResult UDIBind(string CodeValue, string key, int CodeFormat = 0)
        {
			return dal.UDIBind(CodeValue, key, CodeFormat);
        }

		public UDIMergeResult UDIMerge(string CodeValue, string key, int? CodeFormat)
		{
			return dal.UDIMerge(CodeValue, key, CodeFormat);
		}

	}
}
