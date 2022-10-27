using System.Web.Mvc;

namespace iagric_plant.Areas.Market
{
    public class MarketAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Market";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Market_default",
                "Market/{controller}/{action}/{id}",
                new { action = "Index", controller = "Home", id = UrlParameter.Optional },
                new[] { "MarketActive.Controllers" }
            ).DataTokens["UseNamespaceFallback"] = false;
        }
    }
}
