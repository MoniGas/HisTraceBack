using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    public class PRRU_ModualDAL : DALBase
    {
        /// <summary>
        /// 获取可选模块列表
        /// </summary>
        /// <returns></returns>
        public List<LinqModel.PRRU_Modual> GetModelList()
        {
            List<LinqModel.PRRU_Modual> listData = new List<LinqModel.PRRU_Modual>();

            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    listData = (from data in dataContext.PRRU_Modual
                                select data).ToList();

                    return listData;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }

        /// <summary>
        /// 根据权限获取模块列表
        /// </summary>
        /// <param name="modelList">模块权限字符串</param>
        /// <returns></returns>
        public List<LinqModel.PRRU_Modual> GetModelList(string modelList)
        {
            List<LinqModel.PRRU_Modual> dataList = new List<LinqModel.PRRU_Modual>();
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    List<LinqModel.PRRU_NewModual> dateTemp = (from data in dataContext.PRRU_NewModual
                                                               where data.IsDisplay == 1 && data.PRRU_Modual_ID != 10000
                                                               select data).ToList();
                    foreach (string str in modelList.Split(','))
                    {
                        LinqModel.PRRU_NewModual data = dateTemp.Where(w => w.PRRU_Modual_ID == Convert.ToInt32(str.Trim())).FirstOrDefault();
                        if (data != null)
                        {
                            LinqModel.PRRU_Modual model = new LinqModel.PRRU_Modual();
                            model.hash = data.hash;
                            model.Img = data.Img;
                            model.IsDisplay = data.IsDisplay;
                            model.Modual_Level = data.Modual_Level;
                            model.Parent_ID = data.Parent_ID;
                            model.PRRU_Modual_ID = data.PRRU_Modual_ID;
                            model.RootParent_ID = data.RootParent_ID;
                            model.route = data.route;
                            model.SortOrder = data.SortOrder;
                            model.Title = data.Title;
                            model.url = data.url;
                            model.PlatModual = data.PlatModual;
                            dataList.Add(model);
                        }
                    }
                    return dataList;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
