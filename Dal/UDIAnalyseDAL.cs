using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;

namespace Dal
{
	public class UDIAnalyseDAL : DALBase
	{
		/// <summary>
		/// 获取udikey信息
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public UDIKey getUDIKey(string key) 
		{
			UDIKey model = new UDIKey();
			using (DataClassesDataContext db = GetDataContext())
			{
				try
				{
					model = db.UDIKey.Where(m => m.UDIKey1 == key).SingleOrDefault();
					return model;
				}
				catch (Exception ex)
				{
					model = null;
					return model;
				}
			}
		}

		public int getUDIAnalyseCount(long keyID)
		{
			int num = -1;
			using (DataClassesDataContext db = GetDataContext())
			{
				try
				{
					num = db.UDIAnalyse.Where(m=>m.KeyID==keyID).ToList().Count();
					return num;
				}
				catch(Exception ex)
				{
					return num;
				}
			}
		}

		/// <summary>
		/// 获取DI信息
		/// </summary>
		/// <param name="DIInfo"></param>
		/// <param name="CodeTypeName"></param>
		/// <returns></returns>
		public UDIMaterial getUDIMaterial(string DIInfo, string CodeTypeName)
		{
			UDIMaterial model = new UDIMaterial();
			using (DataClassesDataContext db = GetDataContext())
			{
				try
				{
					model=db.UDIMaterial.Where(m => m.MinDI == DIInfo).SingleOrDefault();
					return model;
				}
				catch (Exception ex)
				{
					model = null;
					return model;
				}
			}
		}

		public bool GetUDIAnalyse(UDIMaterial model) 
		{
			using (DataClassesDataContext db = GetDataContext())
			{
				try
				{
					UDIAnalyse item = new UDIAnalyse();
					List<UDIAnalyse> UDIAnalyseLst = db.UDIAnalyse.Where(m => m.EnterpriseName == model.EnterpriseName && m.MaterialName == model.MaterialName && m.MaterialSpec == model.MaterialSpec).ToList();
					if (UDIAnalyseLst.Count > 0)
					{
						return true;
					}
					else 
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}

		public UDIAnalyseResult UpdateUDIAnalyse(UDIMaterial model, long keyID) 
		{
			UDIAnalyseResult result = new UDIAnalyseResult();
			using (DataClassesDataContext db = GetDataContext())
			{
				try
				{
					UDIAnalyse item = new UDIAnalyse();
					List<UDIAnalyse> UDIAnalyseLst = db.UDIAnalyse.Where(m => m.EnterpriseName == model.EnterpriseName && m.MaterialName == model.MaterialName && m.MaterialSpec == model.MaterialSpec).ToList();
					if (UDIAnalyseLst.Count > 0)
					{
						UDIAnalyseLst[0].UpdateTime = DateTime.Now;
						UDIAnalyseLst[0].AnalyseNum = UDIAnalyseLst[0].AnalyseNum + 1;
						db.SubmitChanges();
					}
					else 
					{
						item.KeyID = keyID;
						item.MaterialName = model.MaterialName;
						item.MaterialSpec = model.MaterialSpec;
						item.EnterpriseName = model.EnterpriseName;
						item.AnalyseNum = 1;
						item.UpdateTime = DateTime.Now;
						db.UDIAnalyse.InsertOnSubmit(item);
						db.SubmitChanges();
					}
					result.code = 0;
					result.Msg = "操作成功！";
					return result;
				}
				catch(Exception ex)
				{
					result.code = -1;
					result.Msg = "操作失败："+ex.Message;
					return result;
				}
			}
		}



	}
}
