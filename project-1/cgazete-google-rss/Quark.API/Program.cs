using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Quark.API.Business;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models;
using Quark.Business.Caching.Interfaces;
using Quark.Data;
using Quark.Resources;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Quark.API.Models;
using Quark.Business.ElasticSearch.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuarkDataContext>(options =>{ options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));});
builder.Services.AddDbContext<StatisticsDataContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")); });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<BaseValidator>();

builder.Services.AddBusinessServices(builder.Configuration);

builder.Services.AddElasticSearch(builder.Configuration);

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<BasicAuthOptions>(
    builder.Configuration.GetSection("BasicAuth"));

#region Localization
builder.Services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
#endregion

#region Authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
    {
        opt.SaveToken = true;

        opt.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.OnStarting(() =>
                {
                    return Task.CompletedTask;
                });

                return Task.CompletedTask;
            }
        };

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]!)
            ),

            ClockSkew = TimeSpan.FromMinutes(5)
        };
    })
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        "BasicAuthentication", options => { });
#endregion

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

RequestLocalizationOptions localizationOptions = new()
{
    SupportedCultures = new List<CultureInfo> {
        new CultureInfo("tr"),
        new CultureInfo("en")
    },
    SupportedUICultures = new List<CultureInfo> {
        new CultureInfo("tr"),
        new CultureInfo("en")
    },
    DefaultRequestCulture = new RequestCulture("tr")
};
app.UseRequestLocalization(localizationOptions);

app.MapControllers();

var appsettings = app.Services.CreateScope().ServiceProvider.GetRequiredService<IAppSettingsService>();
await appsettings.LoadAllSettingsToCache();

var localizer = app.Services.GetService<IStringLocalizer<CommonResource>>();
Quark.Core.Extensions.Date.localizer = localizer;

app.MapHealthChecks("/healthz");

app.Run();

var authors = app.Services.CreateScope().ServiceProvider.GetRequiredService<IAuthorCachingService>();
var menus = app.Services.CreateScope().ServiceProvider.GetRequiredService<IMenuCachingService>();
await menus.RefreshAll();
await authors.RefreshAll();