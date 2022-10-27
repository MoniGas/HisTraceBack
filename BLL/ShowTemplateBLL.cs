using Common.Argument;
using Dal;
using LinqModel;

namespace BLL
{
    public class ShowTemplateBLL
    {
        public string EditTemplateID(ShowCompany model)
        {
            ShowTemplateDAL dal = new ShowTemplateDAL();
            RetResult ret = new RetResult();
            ret = dal.EditTemplateID(model);
            string result = ToJson.ResultToJson(ret);
            return result;
        }
    }
}
