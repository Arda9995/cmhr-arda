using AvvaMobile.Core.SMSSender;
using AvvaMobile.Core.Utilities.FtpUpload;
using AvvaMobile.Core.Utilities.Mail;
using AvvaMobile.Core.Utilities.PushNotification;
using Microsoft.EntityFrameworkCore;
using Quark.Business.Common.AgencyAdapter;
using Quark.Data;
using Quark.Hangfire.Business.Interfaces;
using Quark.Hangfire.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region AvvaMobile.Core
/// Do not change top 4 registrations order. (Murat Yılmaz)
//builder.Services.AddSingleton<IRedisClient, RedisClient>(r => new RedisClient(
//    connectionString: builder.Configuration.GetValue<string>("RedisConnectionString")
//));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<AvvaMobile.Core.Caching.ICacheManager, AvvaMobile.Core.Caching.MemoryCacheManager>();
builder.Services.AddScoped<AvvaMobile.Core.Caching.AppSettingsKeys>();
builder.Services.AddScoped<Quark.Business.Common.Caching.AppSettingsKeys>();
/// Do not change top 4 registrations order. (Murat Yılmaz)
builder.Services.AddScoped<IMailService, MailManager>();
builder.Services.AddScoped<IFTPClient, FTPClient>();
builder.Services.AddScoped<FileUpload>();
builder.Services.AddScoped<ISMSSender, IletiMerkezi>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
#endregion

#region Localization
builder.Services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
#endregion

// Add services to the container.
builder.Services.AddDbContext<QuarkDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectionString"]);
});
builder.Services.AddDbContext<StatisticsDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectionString"]);
});




builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAgencydapterService, AgencyAdapterService>();
builder.Services.AddScoped<IAgencyHangfireService, AgencyHangfireService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
