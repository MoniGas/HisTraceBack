using System.Web.Mvc;

namespace iagric_plant.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "SysLogin", action = "SysLogin", id = UrlParameter.Optional }
            );
        }
    }
}
