using Quark.Business.Common.Caching;

namespace Quark.Web.RouteConfigs
{
	public class TagRouteConfig
	{

        public static async Task RegisterRoutes(IEndpointRouteBuilder app, AppSettingsKeys appSettingsKeys)
		{
			var prefix = appSettingsKeys.SlugPrefix_Tags?.Trim('/');
			if (string.IsNullOrWhiteSpace(prefix))
				throw new InvalidOperationException("SlugPrefix_Tags is empty; Tag_Dynamic route would capture category URLs.");

			app.MapControllerRoute(
				name: $"Tag_Dynamic",
				pattern: $"{prefix}/{{slug}}",
				defaults: new { controller = "Tag", action = "NewsList" }
			);
		}
	}
}
