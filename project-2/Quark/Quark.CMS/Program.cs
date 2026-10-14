using AvvaMobile.Core.Redis;
using AvvaMobile.Core.SMSSender;
using AvvaMobile.Core.Utilities.FtpUpload;
using AvvaMobile.Core.Utilities.Mail;
using AvvaMobile.Core.Utilities.PushNotification;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventLog;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Services;
using Quark.Business.ElasticSearch.DependencyInjection;
using Quark.CMS.Business;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Hubs;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models;
using Quark.CMS.Business.Services;
using Quark.CMS.Middlewares;
using Quark.Data;
using Quark.Notification.Adapter;
using Quark.Notification.Adapter.Interfaces;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuarkDataContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")); });
builder.Services.AddDbContext<StatisticsDataContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")); });

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<BaseValidator>();

// builder.Logging.AddEventLog(new EventLogSettings
// {
//     SourceName = "Pozitron.CMS", // appsettings.json'daki SourceName ile ayn�
//     LogName = "Application"      // Default Application Event Log�a yazar
// });

#region AvvaMobile.Core.Caching

//if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
//{
builder.Services.AddSingleton<IRedisClient, RedisClient>(r => new RedisClient(
	connectionString: builder.Configuration.GetValue<string>("RedisConnectionString")
));
builder.Services.AddScoped<AvvaMobile.Core.Caching.ICacheManager, AvvaMobile.Core.Caching.RedisCacheManager>();
//}
//else
//{
//     builder.Services.AddMemoryCache();
//     builder.Services.AddScoped<AvvaMobile.Core.Caching.ICacheManager, AvvaMobile.Core.Caching.MemoryCacheManager>();
// //}

builder.Services.AddScoped<AvvaMobile.Core.Caching.AppSettingsKeys>();
builder.Services.AddScoped<Quark.Business.Common.Caching.AppSettingsKeys>();
#endregion

#region Caching Services
builder.Services.AddScoped<IAppSettingsCachingService, AppSettingsCachingService>();
builder.Services.AddScoped<IAddressCachingService, AddressCachingService>();
builder.Services.AddScoped<IPageCachingService, PageCachingService>();
builder.Services.AddScoped<ITagCachingService, TagCachingService>();
builder.Services.AddScoped<IAuthorCachingService, AuthorCachingService>();
builder.Services.AddScoped<IUserCachingService, UserCachingService>();
builder.Services.AddScoped<IBlockCachingService, BlockCachingService>();
builder.Services.AddScoped<IBookCachingService, BookCachingService>();
builder.Services.AddScoped<INewsPhotoTypeCachingService, NewsPhotoTypeCachingService>();
builder.Services.AddScoped<IMenuCachingService, MenuCachingService>();
builder.Services.AddScoped<INewsCachingService, NewsCachingService>();
builder.Services.AddScoped<IPhotoEditorCachingService, PhotoEditorCachingService>();
builder.Services.AddScoped<IPromptCachingService, PromptCachingService>();
builder.Services.AddScoped<IAnomalyCachingService, AnomalyCachingService>();
builder.Services.AddScoped<ISourceCachingService, SourceCachingService>();
builder.Services.AddScoped<ISystemLogCachingService, SystemLogCachingService>();
builder.Services.AddScoped<ICategoryCachingService, CategoryCachingService>();
builder.Services.AddScoped<INewsTypeCachingService, NewsTypeCachingService>();
builder.Services.AddScoped<INewsStatusCachingService, NewsStatusCachingService>();
builder.Services.AddScoped<IXmlFeedCachingService, XmlFeedCachingService>();
builder.Services.AddScoped<IAuthorCachingService, AuthorCachingService>();
builder.Services.AddScoped<INewsPhotosCachingService, NewsPhotosCachingService>();
builder.Services.AddScoped<IBlockCachingService, BlockCachingService>();
#endregion

#region Core
builder.Services.AddScoped<IMailService, MailManager>();
builder.Services.AddScoped<IFTPClient, FTPClient>();
builder.Services.AddScoped<FileUpload>();
builder.Services.AddHttpClient<NetmeraAdapter>();
builder.Services.AddScoped<IFCMService, FCMAdapter>();
builder.Services.AddScoped<ISMSSender, IletiMerkezi>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAgencyNewsService, AgencyNewsService>();
builder.Services.AddHttpContextAccessor();
#endregion

builder.Services.AddSignalR();

builder.Services.AddBusinessServices();

builder.Services.AddElasticSearch(builder.Configuration);


#region Localization
builder.Services.AddLocalization(o => { o.ResourcesPath = "Resources"; });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	List<CultureInfo> supportedCultures = new List<CultureInfo>
	{
		new CultureInfo("en"),
		new CultureInfo("tr")
	};
	options.DefaultRequestCulture = new RequestCulture("tr");
	options.SupportedCultures = supportedCultures;
	options.SupportedUICultures = supportedCultures;
});
#endregion

#region Authentication
builder.Services.AddAuthentication(CookieNames.AuthCookieName)
	.AddCookie(CookieNames.AuthCookieName, options =>
	{
		options.LoginPath = new PathString("/Account/SignIn");
		options.AccessDeniedPath = new PathString("/Account/Forbidden");
	});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
/*else
{
	app.UseDeveloperExceptionPage();
    //app.UseExceptionHandler("/Home/Error");
	//app.UseHsts();
}*/

app.UseRequestLocalization();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<SignalRHub>("/signalRHub");


#region Caching Test Services
var appsettings = app.Services.CreateScope().ServiceProvider.GetRequiredService<IAppSettingsCachingService>();
await appsettings.RefreshAll();

// var news = app.Services.CreateScope().ServiceProvider.GetRequiredService<INewsCachingService>();
// await news.RefreshAll();
#endregion

app.Run();