using System.Web.Mvc;

namespace Speedbird.Areas.SBBoss
{
    public class SBBossAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SBBoss";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SBBoss_default",
                "SBBoss/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}