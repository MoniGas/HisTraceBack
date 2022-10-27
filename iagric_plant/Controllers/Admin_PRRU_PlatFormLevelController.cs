using System.Web.Mvc;
using BLL;

namespace iagric_plant.Controllers
{
    public class Admin_PRRU_PlatFormLevelController : Controller
    {
        //
        // GET: /Admin_PRRU_PlatFormLevel/
        private readonly PRRU_PlatFormLevelBLL _bll = new PRRU_PlatFormLevelBLL();
        /// <summary>
        /// 根据ID获取该平台的权限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
          

            string strResult = _bll.GetModel(id);

            return Json(strResult);
        }
        /// <summary>
        /// 修改该平台的权限信息方法
        /// </summary>
        /// <param name="objPRRU_PlatFormLevel"></param>
        /// <returns></returns>
        public ActionResult Update(LinqModel.PRRU_PlatFormLevel objPRRU_PlatFormLevel) 
        {
            string strResult = _bll.Updata(objPRRU_PlatFormLevel);

            return Json(strResult);
        }
        /// <summary>
        /// 获取所有可选模块的信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetModel() 
        {
            string strResult = _bll.GetModelList();
            return Json(strResult);
        }
        /// <summary>
        /// 获取所有平台角色信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList() 
        {
            PRRU_PlatFormLevelBLL objPRRU_PlatFormLevelBLL = new PRRU_PlatFormLevelBLL();
            string strResult = objPRRU_PlatFormLevelBLL.GetList();
            return Json(strResult);
        }
    }
}
