using AvvaMobile.Core;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;
using Quark.Business.Common.PaymentServices;
using Quark.Cloudflare.Adapter;
using Quark.Cloudflare.Adapter.Interfaces;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Services;

namespace Quark.CMS.Business;

public static class ServiceRegistration
{
	public static IServiceCollection AddBusinessServices(this IServiceCollection services)
	{
        #region Adapters
        services.AddScoped<ICloudflareAdapter, CloudflareAdapter>();
		#endregion

        #region CMS Services

        services.AddScoped<IAccountService, AccountService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<INewsIntegrationService, NewsIntegrationService>();
		services.AddScoped<IRoleService, RoleService>();
		services.AddScoped<IMemberService, MemberService>();
		services.AddScoped<IAppSettingsService, AppSettingsService>();
		services.AddScoped<ICommonService, CommonService>();
		services.AddScoped<ICategoryService, CategoryService>();
		services.AddScoped<IPageService, PageService>();
		services.AddScoped<IMenuService, MenuService>();
		services.AddScoped<ITagService, TagServices>();
		services.AddScoped<IAuthorService, AuthorService>();
		services.AddScoped<IBlockService, BlockService>();
		services.AddScoped<ICropperService, CropperService>();
		services.AddScoped<IMediaGalleryService, MediaGalleryService>();
		services.AddScoped<IGoogleReCaptchaService, GoogleReCaptchaService>();
		services.AddScoped<NetworkManager>();
		services.AddScoped<IPhotoEditorTemplateService, PhotoEditorTemplateTemplateService>();
		services.AddOpenAIService(settings => settings.ApiKey = "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");
		services.AddScoped<IPromptService, PromptService>();
		services.AddScoped<IHomeService, HomeService>();
		services.AddScoped<ISystemLogService, SystemLogService>();
		services.AddScoped<IAnomalyService, AnomalyService>();
		services.AddScoped<IUserContractService, UserContractService>();
		services.AddScoped<ISourceService, SourceService>();
		services.AddScoped<IGoogleTrendsService, GoogleTrendsService>();
		services.AddScoped<ISurveyService, SurveyService>();
		services.AddScoped<ICaricaturistService, CaricaturistService>();
		services.AddScoped<IAstrologerService, AstrologerService>();
		services.AddScoped<IPaymentManagementService, PaymentManagementService>();
		services.AddScoped<IUrlMappingService, UrlMappingService>();
		services.AddScoped<INotificationService, NotificationService>();
		#endregion

		#region News

		services.AddScoped<INewsPhotoTypeService, NewsPhotoTypeService>();
		services.AddScoped<INewsService, NewsService>();
		services.AddScoped<IAIService, AIService>();

		#endregion

		#region Common

		services.AddScoped<IPaymentService, PaymentService>();

		#endregion

		#region FileUpload Services

		services.AddScoped<FileUpload>();

		#endregion

		return services;
	}
}