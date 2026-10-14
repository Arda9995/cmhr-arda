namespace Quark.Web.RouteConfigs
{
	public static class HomeRouteConfig
	{
		public static void RegisterRoutes(IEndpointRouteBuilder app)
		{
			#region News Details Page!
			app.MapControllerRoute(
					name: "Dynamic_News_ById",
					pattern: "{id:int}",
					defaults: new { controller = "News", action = "Details" }
				);
			#endregion
		}
	}
}
