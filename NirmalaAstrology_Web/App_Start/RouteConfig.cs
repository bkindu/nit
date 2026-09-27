using System.Web.Mvc;
using System.Web.Routing;

namespace NirmalaAstrology_Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var rootNs = new[] { "NirmalaAstrology_Web.Controllers" };

            // Detail only — exclude Index/Detail/Details so /Zodiac and /Zodiac/Index hit Index action.
            routes.MapRoute(
                name: "ZodiacDetail",
                url: "Zodiac/{name}",
                defaults: new { controller = "Zodiac", action = "Detail" },
                constraints: new { name = @"^(?!index$|detail$|details$)[a-z0-9\-]+$" },
                namespaces: rootNs
            );

            routes.MapRoute(
                name: "ArticleDetail",
                url: "Articles/{slug}",
                defaults: new { controller = "Articles", action = "Detail" },
                constraints: new { slug = @"^(?!Index$).+" },
                namespaces: rootNs
            );

            routes.MapRoute(
                name: "BirthDataSession",
                url: "api/birth-data/session",
                defaults: new { controller = "Home", action = "SetBirthDataSession" },
                namespaces: rootNs
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: rootNs
            );
        }
    }
}
