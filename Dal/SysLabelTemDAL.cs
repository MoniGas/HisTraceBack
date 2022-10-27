using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Argument;
using LinqModel;
using Webdiyer.WebControls.Mvc;

namespace Dal
{
    public class SysLabelTemDAL : DALBase
    {

        public PagedList<LabelTem> GetLabelTemList(string LabelName, int? pageIndex, out long totalCount)
        {
            totalCount = 0;
            LoginInfo pf = SessCokie.GetMan;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = from data in dataContext.LabelTem
                                   select data;
                    if (!string.IsNullOrEmpty(LabelName))
                    {
                        dataList = dataList.Where(w => w.LabelName.Contains(LabelName));
                    }
                    totalCount = dataList.Count();
                    dataList = dataList.Where(m => m.Status == (int)Common.EnumFile.Status.used).OrderByDescending(m => m.LabelTem_ID);
                    return dataList.ToPagedList(pageIndex ?? 1, PageSize);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Object GetLabelTem()
        {
            LoginInfo pf = SessCokie.GetMan;
            try
            {
                using (LinqModel.DataClassesDataContext dataContext = GetDataContext())
                {
                    var dataList = dataContext.LabelTem.Select(m=>m).Where(m=>m.Status==0).ToList();

                    return dataList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public RetResult Add(LabelTem labeltem)
        {
            string Msg = "添加标签模板信息失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext())
            {
                try
                {
                    var data = dataContext.LabelTem.FirstOrDefault(m => m.LabelName == labeltem.LabelName && m.Status == (int)Common.EnumFile.Status.used);
                    if (data != null)
                    {
                        Msg = "已存在该标签模板名称！";
                    }
                    else
                    {
                        dataContext.LabelTem.InsertOnSubmit(labeltem);
                        dataContext.SubmitChanges();
                        Ret.CrudCount = labeltem.LabelTem_ID;
                        Msg = "添加标签模板信息成功";
                        error = CmdResultError.NONE;
                    }
                }
                catch(Exception ex)
                {
                    Ret.Msg = "链接数据库失败，失败信息："+ex.Message;
                }
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }

        public RetResult Delete(long LabelTem_ID)
        {
            string Msg = "删除标签模板失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    LabelTem labeltem = dataContext.LabelTem.SingleOrDefault(m => m.LabelTem_ID == LabelTem_ID);
                    if (labeltem == null)
                    {
                        Msg = "没有找到要删除的标签模板信息，请刷新列表！";
                    }
                    else
                    {
                        labeltem.Status = (int)Common.EnumFile.Status.delete;
                        dataContext.SubmitChanges();
                        Msg = "删除标签模板成功！";
                        error = CmdResultError.NONE;
                    }
                }
            }
            catch(Exception ex)
            {
                //Msg = "删除失败，请首先删除已知关联的其他数据";
                Ret.Msg = "删除失败，失败信息：" + ex.Message;
            }
            Ret.SetArgument(error, Msg, Msg);
            return Ret;
        }
    }
}
