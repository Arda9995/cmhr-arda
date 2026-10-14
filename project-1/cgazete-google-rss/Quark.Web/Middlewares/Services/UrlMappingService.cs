using AvvaMobile.Core.Caching;
using Quark.Data;
using Quark.Web.Middlewares.Interfaces;

namespace Quark.Web.Middlewares.Services
{
    public class UrlMappingService : IUrlMappingService
    {
        private readonly ICacheManager _cache;
        private readonly ILogger<UrlMappingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _environment;
        private static WebApplication _appInstance;

        public UrlMappingService(
            ICacheManager cache,
            ILogger<UrlMappingService> logger,
            IServiceProvider serviceProvider,
            IWebHostEnvironment environment)
        {
            _cache = cache;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _environment = environment;
        }

        public static void SetApplicationInstance(WebApplication app)
        {
            _appInstance = app;
        }

        public void RefreshUrlMappingsCache()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<QuarkDataContext>();
                UrlRedirectionMiddleware.RefreshUrlMappingsCache(_cache, context).Wait();
                _logger.LogInformation("URL mappings cache successfully refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing URL mappings cache");
            }
        }
    }
}