/********************************************************************************
** 作者：苏凯丽
** 开发时间：2017-7-25
** 联系方式:15533621896
** 代码功能：更新配置码表
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Web.Mvc;

namespace iagric_plant.Controllers
{
    /// <summary>
    /// 更新配置码表接口
    /// </summary>
    public class SettingCodeApiController : Controller
    {
        /// <summary>
        /// 更新配置码表
        /// </summary>
        /// <param name="settingId"></param>
        /// <returns></returns>
        public JsonResult UpdateSettingCodeState(long settingId)
        {
            bool flag = new BLL.RequestCodeSettingBLL().UpdatePacketState(settingId);
            return Json(new { msg = flag ? "更新成功！" : "更新失败！", code = flag ? 1 : 0 });
        }
    }
}
