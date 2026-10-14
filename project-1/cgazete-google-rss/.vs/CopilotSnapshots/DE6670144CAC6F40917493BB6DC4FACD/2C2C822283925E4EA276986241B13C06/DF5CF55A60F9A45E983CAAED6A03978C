using Quark.Business.Common.Caching;

namespace Quark.Web.RouteConfigs
{
    public static class SourceRouteConfig
    {
        public static async Task RegisterRoutes(IEndpointRouteBuilder app, AppSettingsKeys appSettingsKeys)
        {
            app.MapControllerRoute(
                name: $"Source_Dynamic",
                pattern: $"{appSettingsKeys.SlugPrefix_Sources}/{{slug}}",
                defaults: new { controller = "Source", action = "NewsList" }
            );
        }
    }
}
