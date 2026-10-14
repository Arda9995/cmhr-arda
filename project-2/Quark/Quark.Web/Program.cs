using AvvaMobile.Core.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quark.API.Business;
using Quark.API.Business.Services;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.Caching;
using Quark.Business.ElasticSearch.DependencyInjection;
using Quark.Data;
using Quark.Web.Middlewares;
using Quark.Web.Middlewares.Interfaces;
using Quark.Web.Middlewares.Services;
using Quark.Web.OutputCachePolicy;
using Quark.Web.RouteConfigs;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager _configuration = builder.Configuration;

builder.Services.AddDbContext<QuarkDataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")),
            ServiceLifetime.Scoped);
builder.Services.AddDbContext<StatisticsDataContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")); });

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddBusinessServices(builder.Configuration);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IUrlMappingService, UrlMappingService>();
builder.Services.AddScoped<IRouteHealthCheckService, RouteHealthCheckService>();

builder.Services.AddSingleton<IEndpointRouteBuilder>(sp =>
{
    var appAccessor = new EndpointRouteBuilderAccessor();
    return appAccessor;
});

#region AvvaMobile.Core.Caching


builder.Services.AddSingleton<IRedisClient, RedisClient>(r => new RedisClient(
    connectionString: builder.Configuration.GetValue<string>("RedisConnectionString")
));
builder.Services.AddScoped<AvvaMobile.Core.Caching.ICacheManager, AvvaMobile.Core.Caching.RedisCacheManager>();

builder.Services.AddScoped<AvvaMobile.Core.Caching.AppSettingsKeys>();

#endregion

builder.Services.AddElasticSearch(builder.Configuration);


#region Localization
builder.Services.AddLocalization(o => { o.ResourcesPath = "Resources"; });

#endregion
#region Authentication
builder.Services.AddAuthentication()
.AddJwtBearer(opt =>
{

    opt.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = context =>
        {
            context.Response.OnStarting(() =>
            {
                context.Response.ContentType = "text/plain";
                return global::System.Threading.Tasks.Task.CompletedTask;
            });

            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var authCookie = context.Request.Cookies["Authorization"];
            if (!string.IsNullOrEmpty(authCookie) && authCookie.StartsWith("Bearer "))
            {
                context.Token = authCookie.Substring("Bearer ".Length);
            }
            return Task.CompletedTask;
        }
    };

    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _configuration["Token:Issuer"],
        ValidAudience = _configuration["Token:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"])),
        ClockSkew = TimeSpan.FromMinutes(5),
    };
});
#endregion


var redisConnectionString = builder.Configuration.GetValue<string>("RedisConnectionString");

builder.Services.AddStackExchangeRedisCache(options =>
{
    var newConfig = ConfigurationOptions.Parse(redisConnectionString, true);
    newConfig.AbortOnConnectFail = false; 
    newConfig.ConnectTimeout = 5000; 
    newConfig.SyncTimeout = 5000; 
    newConfig.ConnectRetry = 5;
    newConfig.ReconnectRetryPolicy = new ExponentialRetry(5000); 


    options.Configuration = redisConnectionString;
    options.InstanceName = "RedisOutputCache_";
    options.ConfigurationOptions = newConfig; 
});

builder.Services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
/*
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("DynamicTTL", policy =>
        policy.Expire(TimeSpan.FromSeconds(180))
              .SetVaryByHeader("User-Agent"));
});
*/
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("DynamicTTL", policyBuilder =>
    {
        policyBuilder.AddPolicy<CustomOutputCachePolicy>();
    });

/*    options.AddPolicy("DynamicTTL", policy =>
        policy.Expire(TimeSpan.FromSeconds(30)));

        if (!string.IsNullOrEmpty(policy.VaryByHeader))
                            builder.SetVaryByHeader(policy.VaryByHeader);
*/
});


var app = builder.Build();

UrlMappingService.SetApplicationInstance(app);

app.Use(async (context, next) =>
{
    try
    {
        await next();

        if (context.Response.StatusCode == 401)
        {
            context.Response.Redirect("/uye/giris");
        }
        else if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500 && !context.Response.HasStarted)
        {
            context.Request.Path = "/Home/404";
            context.Response.StatusCode = 404;

            await next();
        }
        else if (context.Response.StatusCode >= 500)
        {
            context.Request.Path = "/Home/500";
            context.Response.StatusCode = 500;

            await next();
        }
    }
    catch (Exception)
    {
        context.Request.Path = "/Home/500";
        context.Response.StatusCode = 404;
        await next();
    }
});

app.UseExceptionHandler("/Home/500");
//app.UseStatusCodePagesWithReExecute("/Home/404");
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseUrlRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

app.UseEndpoints(endpoints =>
{
    var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;

    // EndpointRouteBuilderAccessor'e endpoints'i set et
    var endpointRouteBuilder = app.Services.GetRequiredService<IEndpointRouteBuilder>() as EndpointRouteBuilderAccessor;
    if (endpointRouteBuilder != null)
    {
        endpointRouteBuilder.SetEndpoints(endpoints);
    }
    var appsettings = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppSettingsKeys>();
    Task.Run(async () => await RouteInitializer.InitializeRoutes(endpoints, app, appsettings)).Wait();
});



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#region Caching Test Services
var appsettings = app.Services.CreateScope().ServiceProvider.GetRequiredService<IAppSettingsCachingService>();
await appsettings.RefreshAll();
#endregion

// Route kontrollerini başlatmak için aşağıdaki satırın commentini kaldırın

//Task.Run(async () =>
//{
//	try
//	{
//		// Uygulamanın başlaması için biraz bekle
//		await Task.Delay(TimeSpan.FromSeconds(5));

//		using var scope = app.Services.CreateScope();
//		var routeHealthCheckService = scope.ServiceProvider.GetRequiredService<IRouteHealthCheckService>();
//		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

//		logger.LogInformation("Starting route health check...");
//		var isValid = await routeHealthCheckService.ValidateRoutes(null);

//		if (!isValid)
//		{
//			logger.LogWarning("Some routes are not accessible. Check the logs for details.");
//		}
//		else
//		{
//			logger.LogInformation("All routes are accessible.");
//		}
//	}
//	catch (Exception ex)
//	{
//		using var scope = app.Services.CreateScope();
//		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//		logger.LogError(ex, "Route health check encountered an error");
//	}
//});

app.Run();

public class EndpointRouteBuilderAccessor : IEndpointRouteBuilder
{
    private IEndpointRouteBuilder _endpoints;

    public void SetEndpoints(IEndpointRouteBuilder endpoints)
    {
        _endpoints = endpoints;
    }

    public IServiceProvider ServiceProvider => _endpoints?.ServiceProvider;

    public ICollection<EndpointDataSource> DataSources => _endpoints?.DataSources;

    public IApplicationBuilder CreateApplicationBuilder() => _endpoints?.CreateApplicationBuilder();
}
