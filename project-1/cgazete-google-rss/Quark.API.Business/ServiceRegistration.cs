using AvvaMobile.Core.Loging.Interfaces;
using AvvaMobile.Core.Loging.Services;
using AvvaMobile.Core.Redis;
using AvvaMobile.Core.SMSSender;
using AvvaMobile.Core.Utilities.FtpUpload;
using AvvaMobile.Core.Utilities.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Services;
using Quark.API.Business.Services.Interfaces;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Services;
using Quark.Business.Common.PaymentServices;
using Quark.Cloudflare.Adapter;
using Quark.Cloudflare.Adapter.Interfaces;
using Quark.Notification.Adapter;
using Quark.Subscription.Adapter;

namespace Quark.API.Business;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Adapters
        services.AddScoped<ICloudflareAdapter, CloudflareAdapter>();
        services.AddScoped<SetrowAdapter>();
        #endregion

		#region Business
		services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ISurveyService, SurveyService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IAppSettingsService, AppSettingsService>();
        services.AddScoped<ICommonService, CommonService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<ISourceServices, SourceService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISlugService, SlugService>();
        services.AddScoped<IXmlFeedService, XmlFeedService>();
        services.AddScoped<INewsIntegrationService, NewsIntegrationService>();
		#endregion

		#region AvvaMobile.Core.Caching

		//if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
  //      {
            services.AddSingleton<IRedisClient, RedisClient>(r => new RedisClient(
                connectionString: configuration.GetValue<string>("RedisConnectionString")
            ));
            services.AddScoped<AvvaMobile.Core.Caching.ICacheManager, AvvaMobile.Core.Caching.RedisCacheManager>();
        //}
        //else
        //{
        //    services.AddMemoryCache();
        //    services.AddScoped<AvvaMobile.Core.Caching.ICacheManager, AvvaMobile.Core.Caching.MemoryCacheManager>();
        //}

        services.AddScoped<AvvaMobile.Core.Caching.AppSettingsKeys>();
        services.AddScoped<Quark.Business.Common.Caching.AppSettingsKeys>();

        #endregion

        #region Caching Services
        services.AddScoped<IAuthorCachingService, AuthorCachingService>();
        services.AddScoped<INewsPhotosCachingService, NewsPhotosCachingService>();
        services.AddScoped<IBlockCachingService, BlockCachingService>();
        services.AddScoped<IBookCachingService, BookCachingService>();
        services.AddScoped<IAppSettingsCachingService, AppSettingsCachingService>();
        services.AddScoped<IAddressCachingService, AddressCachingService>();
        services.AddScoped<IPageCachingService, PageCachingService>();
        services.AddScoped<IAuthorCachingService, AuthorCachingService>();
        services.AddScoped<INewsCachingService, NewsCachingService>();
        services.AddScoped<IMenuCachingService, MenuCachingService>();
        services.AddScoped<ICategoryCachingService, CategoryCachingService>();
        services.AddScoped<IBookCachingService, BookCachingService>();
        services.AddScoped<ITagCachingService, TagCachingService>();
        services.AddScoped<IXmlFeedCachingService, XmlFeedCachingService>();
        services.AddScoped<ICommonCachingService, CommonCachingService>();
        services.AddScoped<ISourceCachingService, SourceCachingService>();
        services.AddScoped<IAccountCachingService, AccountCachingService>();
        services.AddScoped<ISystemLogCachingService, SystemLogCachingService>();
        #endregion

        #region SWAGGER

        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-SIM", Version = "v1" }); });

        #endregion

        #region AvvaMobile.Core

        services.AddScoped<IMailService, MailManager>();
        services.AddScoped<IFTPClient, FTPClient>();
        services.AddScoped<FileUpload>();
        services.AddScoped<ISMSSender, IletiMerkezi>();
        services.AddScoped<ILogService, ElasticLogService>();
        services.AddHttpContextAccessor();

        #endregion

        #region Common

        services.AddScoped<IPaymentService, PaymentService>();

        #endregion

        #region THIRD PARTY
        services.AddHttpClient<NetmeraAdapter>();
        #endregion

        return services;
    }
}