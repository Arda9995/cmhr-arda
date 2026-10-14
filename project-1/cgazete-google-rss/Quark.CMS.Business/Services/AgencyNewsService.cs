using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Services;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AgencyNewsRequests;
using Quark.CMS.Business.Models.Responses.AgencyNewsResponses;
using Quark.Data;
using Quark.Resources;
using SixLabors.ImageSharp;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
namespace Quark.CMS.Business.Services
{
    public class AgencyNewsService : BaseService, IAgencyNewsService
    {
        ICategoryCachingService _categoryCachingService;
        INewsTypeCachingService _newsTypeCachingService;
        public AgencyNewsService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, ICategoryCachingService categoryCachingService, INewsTypeCachingService newsTypeCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _categoryCachingService = categoryCachingService;
            _newsTypeCachingService = newsTypeCachingService;
        }

        public async Task<ServiceResult<ListFiltersRequest>> GetAllPagedFilters()
        {
            var result = new ServiceResult<ListFiltersRequest>();
            result.Data = new ListFiltersRequest();

            result.Data.StartDate = DateTime.Now.AddDays(-1).ToFormattedDate();
            result.Data.EndDate = DateTime.Now.AddDays(1).ToFormattedDate();
            result.Data.IsActive = false;

            var categoryList = await _categoryCachingService.GetCategoriesAsSelectListItems();
            result.Data.Categories = categoryList.Data;
            result.Data.Categories.AddALLOption();

            var newsSourcesList = await _newsTypeCachingService.GetNewsSourcesAsSelectListItems();
            result.Data.Agencies = newsSourcesList.Data;
            result.Data.Agencies.AddALLOption();

            return result;
        }

        public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPaged(
            BaseDataTableRequest tableRequest, ListFiltersRequest request)
        {
            var result = new BaseDataTableResponse<List<ListResponseItem>>();

            var startDate = request.StartDate.DateTimeParseExact();
            var endDate = request.EndDate.DateTimeParseExact();
            result.RecordsTotal = await _context.AgencyNews.CountAsync();
            var yesText = "Evet";
            var noText = "Hayır";

            var newsQuery = (from n in _context.AgencyNews
                             join c in _context.Categories on n.CategoryID equals c.ID
                             join s in _context.Sources on n.AgencyID equals s.ID
                             where n.Date > startDate
                             && n.Date < endDate
                             && n.IsActive == request.IsActive
                             && (request.SelectedCategoryID.IsALLSelected() || request.SelectedCategoryID == c.ID)
                             && (request.SelectedAgencyID.IsALLSelected() || request.SelectedAgencyID == s.ID)
                             orderby n.Date descending
                             select new
                             {
                                 ID = n.ID,
                                 Title = n.Title,
                                 Category = c.Name.ToEmptyStringIndicator(),
                                 AgencyName = s.Name.ToEmptyStringIndicator(),
                                 CreatedDate = n.Date,
                                 IsActive = n.IsActive,
                                 IsActiveText = n.IsActive ? yesText : noText,
                                 Date = n.Date
                             }).AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(tableRequest.SearchValue))
            {
                var search = tableRequest.SearchValue.ToUpper();
                newsQuery = newsQuery.Where(x =>
                    x.Title.ToUpper().Contains(search)
                    );
            }

            result.RecordsFiltered = await newsQuery.CountAsync();

            newsQuery = newsQuery.SortSkipTake(tableRequest);

            result.Data = await newsQuery.Select(f => new ListResponseItem
            {
                ID = f.ID,
                Date = f.Date.ToFormattedDateTime(),
                CategoryName = f.Category,
                AgencyName = f.AgencyName,
                Title = f.Title,
                IsActive = f.IsActive,
                IsActiveText = f.IsActiveText,
            }).ToListAsync();

            return result;
        }

        public async Task<ServiceResult<AgencyNewsDetailResponse>> Detail(int agencyNewsID)
        {
            var result = new ServiceResult<AgencyNewsDetailResponse>();
            var data = await (from an in _context.AgencyNews
                              join c in _context.Categories on an.CategoryID equals c.ID
                              join s in _context.Sources on an.AgencyID equals s.ID
                              where an.ID == agencyNewsID
                              select new AgencyNewsDetailResponse
                              {
                                  ID = an.ID,
                                  Date = an.Date.ToFormattedDateTime(),
                                  AgencyName = s.Name,
                                  CategoryName = c.Name,
                                  Type = an.Type,
                                  Title = an.Title,
                                  Subtitle = an.Subtitle,
                                  Content = an.Content,
                              }).FirstOrDefaultAsync();
            if (data == null) 
            {
                result.SetError("Kayıt bulunamadı.");
                return result;
            }

            result.Data = data;
            return result;
        }
    }
}