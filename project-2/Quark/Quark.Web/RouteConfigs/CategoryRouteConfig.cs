namespace Quark.Web.RouteConfigs;

public static class CategoryRouteConfig
{
    public static async Task RegisterRoutes(IEndpointRouteBuilder app, WebApplication application)
    {
        app.MapControllerRoute(
            name: $"Category_Dynamic",
            pattern: "{slug}",
            defaults: new { controller = "Slug", action = "SlugHandler" }
        );
    }
}