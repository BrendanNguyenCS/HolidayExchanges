using System.Web.Mvc;
using System.Web.Routing;

namespace HolidayExchanges
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Register",
                url: "Register",
                defaults: new
                {
                    controller = "Login",
                    action = "Register"
                });

            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new
                {
                    controller = "Login",
                    action = "Login"
                });

            routes.MapRoute(
                 name: "Default",
                 url: "{controller}/{action}/{id}",
                 defaults: new
                 {
                     controller = "Home",
                     action = "Index",
                     id = UrlParameter.Optional
                 }
             );
        }
    }
}