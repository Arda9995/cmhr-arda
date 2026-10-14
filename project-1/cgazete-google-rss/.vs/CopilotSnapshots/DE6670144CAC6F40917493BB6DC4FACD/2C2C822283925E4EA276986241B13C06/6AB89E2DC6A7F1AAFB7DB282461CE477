using Quark.Business.Common.Caching;

namespace Quark.Web.RouteConfigs
{
	public class TagRouteConfig
	{

        public static async Task RegisterRoutes(IEndpointRouteBuilder app, AppSettingsKeys appSettingsKeys)
		{
			app.MapControllerRoute(
				name: $"Tag_Dynamic",
				pattern: $"{appSettingsKeys.SlugPrefix_Tags}/{{slug}}",
				defaults: new { controller = "Tag", action = "NewsList" }
			);
		}
	}
}
