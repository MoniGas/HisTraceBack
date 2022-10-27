using System.Linq;
using Common.Argument;
using LinqModel;

namespace Dal
{
    public class ShowTemplateDAL : DALBase
    {
        public RetResult EditTemplateID(ShowCompany model)
        {
            Ret.Msg = "修改企业宣传模板失败！";
            Ret.CmdError = CmdResultError.EXCEPTION;
            using (DataClassesDataContext dataContext = GetDataContext("ShowConnect"))
            {
                try
                {
                    ShowCompany company = dataContext.ShowCompany.FirstOrDefault(m => m.CompanyID == model.CompanyID);
                    if (company == null)
                    {
                        Ret.Msg = "获取企业信息失败！";
                    }
                    else
                    {
                        company.TemplateIDs = model.TemplateIDs;
                        dataContext.SubmitChanges();
                        Ret.Msg = "企业宣传模板保存成功！";
                        Ret.CmdError = CmdResultError.NONE;
                    }
                }
                catch
                {
                    Ret.Msg = "操作中出现错误！";
                }
            }
            return Ret;
        }
    }
}
