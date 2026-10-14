using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Responses.HomeResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IHomeService
{
    Task<ServiceResult<HomeResponse>> Index();
    Task<BaseDataTableResponse<List<HomeResponseNewsItem>>> GetNewsPaged(BaseDataTableRequest tableRequest);

    Task<ServiceResult<GetDailyNewsInformationResponse>> GetDailyNewsInformation();
    Task<ServiceResult<GetNewsDetailsForWidgetResponse>> GetTodayNewsDetails();
    Task<ServiceResult<GetNewsDetailsForWidgetResponse>> GetMostReadedNewsDetails();
    Task<ServiceResult<int>> GetUnpublishedNewsCount();
    Task<ServiceResult<GetNewsTypesResponse>> GetNewsTypeDetails();
    Task<ServiceResult<GetLastPublishedNewsResponse>> GetLastPublishedNews();
    Task<ServiceResult<GetNewsChartDataByCategoriesResponse>> GetNewsChartDataByCategories();
    Task<ServiceResult<GetNewsChartDataByDailyNewsResponse>> GetNewsChartDataByDailyNews();

}