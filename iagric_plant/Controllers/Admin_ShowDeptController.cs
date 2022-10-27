/********************************************************************************

** 作者： 高世聪

** 创始时间：2015-6-4

** 修改人：xxx

** 修改时间：xxxx-xx-xx

** 修改人：xxx

** 修改时间：xxx-xx-xx     

** 描述：

** 主要用于部门宣传码管理控制器

*********************************************************************************/
using System;
using System.Web.Mvc;
using Common.Argument;
using Common.Log;
using LinqModel;

namespace iagric_plant.Controllers
{
    public class Admin_ShowDeptController : BaseController
    {
        //
        // GET: /Admin_ShowDept/

        public JsonResult GetList(int pageIndex,string name)
        {
            LoginInfo user = SessCokie.Get;
            BaseResultList dataList = new BaseResultList();
            try
            {
                BLL.ShowDeptBLL objShowDeptBLL = new BLL.ShowDeptBLL();

                dataList = objShowDeptBLL.GetList(user.EnterpriseID, name, pageIndex);
            }
            catch (Exception ex)
            {
                string errData = "Admin_ShowDeptController.GetList():ShowDept表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(dataList);
        }


        public JsonResult Add(string name, string brief)   
        {
            LoginInfo user = SessCokie.Get;
            BaseResultModel objBaseResultModel = new BaseResultModel();
            try
            {
                LinqModel.ShowDept objShowDept = new LinqModel.ShowDept();
                objShowDept.DeptName = name;
                objShowDept.Infos = brief;
                objShowDept.CompanyID = user.EnterpriseID;

                BLL.ShowDeptBLL objShowDeptBLL = new BLL.ShowDeptBLL();

                objBaseResultModel = objShowDeptBLL.Add(objShowDept, user.MainCode);
            }
            catch (Exception ex)
            {
                objBaseResultModel = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowDeptController.Add():ShowDept表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(objBaseResultModel);
        }

        public JsonResult Del(long id) 
        {
            BaseResultModel objBaseResultModel = new BaseResultModel();
            try
            {
                BLL.ShowDeptBLL objShowDeptBLL = new BLL.ShowDeptBLL();

                objBaseResultModel = objShowDeptBLL.Del(id);
            }
            catch (Exception ex)
            {
                objBaseResultModel = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowDeptController.Del():ShowDept表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(objBaseResultModel);
        }

        public JsonResult GetModel(long id) 
        {
            BaseResultModel objBaseResultModel = new BaseResultModel();
            try
            {
                BLL.ShowDeptBLL objShowDeptBLL = new BLL.ShowDeptBLL();

                objBaseResultModel = objShowDeptBLL.GetModel(id);
            }
            catch (Exception ex)
            {
                objBaseResultModel = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowDeptController.GetModel():ShowDept表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(objBaseResultModel);
        }

        public JsonResult Edit(long id, string name, string brief) 
        {
            BaseResultModel objBaseResultModel = new BaseResultModel();
            try
            {
                LinqModel.ShowDept objShowDept = new LinqModel.ShowDept();
                objShowDept.ID = id;
                objShowDept.DeptName = name;
                objShowDept.Infos = brief;

                BLL.ShowDeptBLL objShowDeptBLL = new BLL.ShowDeptBLL();

                objBaseResultModel = objShowDeptBLL.Edit(objShowDept);
            }
            catch (Exception ex)
            {
                objBaseResultModel = ToJson.NewRetResultToJson("-1", "提示：异常错误！");
                string errData = "Admin_ShowDeptController.Edit():ShowDept表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }

            return Json(objBaseResultModel);
        }
    }
}
