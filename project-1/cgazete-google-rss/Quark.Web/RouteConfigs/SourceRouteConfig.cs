using Quark.Business.Common.Caching;

namespace Quark.Web.RouteConfigs
{
    public static class SourceRouteConfig
    {
        public static async Task RegisterRoutes(IEndpointRouteBuilder app, AppSettingsKeys appSettingsKeys)
        {
            var prefix = appSettingsKeys.SlugPrefix_Sources?.Trim('/');
            if (string.IsNullOrWhiteSpace(prefix))
                throw new InvalidOperationException("SlugPrefix_Sources is empty; Source_Dynamic route would capture category URLs.");

            app.MapControllerRoute(
                name: $"Source_Dynamic",
                pattern: $"{prefix}/{{slug}}",
                defaults: new { controller = "Source", action = "NewsList" }
            );
        }
    }
}
