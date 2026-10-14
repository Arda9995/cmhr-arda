using Quark.Business.Common.Caching;

namespace Quark.Web.RouteConfigs
{
	public static class RouteInitializer
	{
		public static async Task InitializeRoutes(IEndpointRouteBuilder app, WebApplication application, AppSettingsKeys appSettingsKeys)
		{
			var logger = application.Services.GetService<ILoggerFactory>()?.CreateLogger("Quark.Web.RouteInitializer");
			var errorFilePath = Path.Combine(application.Environment.ContentRootPath, "error.txt");

			await Register(logger, errorFilePath, nameof(AccountRouteConfig), () => { AccountRouteConfig.RegisterRoutes(app); return Task.CompletedTask; });
			await Register(logger, errorFilePath, nameof(AuthorRouteConfig), () => { AuthorRouteConfig.RegisterRoutes(app); return Task.CompletedTask; });
			await Register(logger, errorFilePath, nameof(CommonRouteConfig), () => CommonRouteConfig.RegisterRoutes(app, application));
			await Register(logger, errorFilePath, nameof(SourceRouteConfig), () => SourceRouteConfig.RegisterRoutes(app, appSettingsKeys));
			await Register(logger, errorFilePath, nameof(TagRouteConfig), () => TagRouteConfig.RegisterRoutes(app, appSettingsKeys));
			await Register(logger, errorFilePath, nameof(HomeRouteConfig), () => { HomeRouteConfig.RegisterRoutes(app); return Task.CompletedTask; });
			await Register(logger, errorFilePath, nameof(CategoryRouteConfig), () => CategoryRouteConfig.RegisterRoutes(app, application));
			await Register(logger, errorFilePath, nameof(NewsRouteConfig), () => { NewsRouteConfig.RegisterRoutes(app); return Task.CompletedTask; });
		}

		private static async Task Register(ILogger logger, string errorFilePath, string name, Func<Task> register)
		{
			try
			{
				await register();
			}
			catch (Exception ex)
			{
				logger?.LogError(ex, "Route registration failed for {RouteConfig}; continuing with remaining routes", name);
				WriteErrorToFile(errorFilePath, name, ex);
			}
		}

		private static void WriteErrorToFile(string errorFilePath, string name, Exception ex)
		{
			try
			{
				var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Route registration failed for {name}{Environment.NewLine}{ex}{Environment.NewLine}{new string('-', 80)}{Environment.NewLine}";
				File.AppendAllText(errorFilePath, entry);
			}
			catch
			{
				// Dosyaya yazilamazsa (izin vb.) uygulamanin acilisini engelleme.
			}
		}
	}
}
