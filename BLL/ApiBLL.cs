using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel.InterfaceModels;
using Dal;

namespace BLL
{
	public class ApiBLL
	{
		ApiDAL dal = new ApiDAL();
		public InterfaceResult login(Login model) 
		{
			return dal.login(model);
		}



	}
}
