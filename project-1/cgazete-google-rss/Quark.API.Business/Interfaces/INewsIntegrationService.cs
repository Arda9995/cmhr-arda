using Quark.API.Business.Models.Responses.NewsManagementResponses;

namespace Quark.API.Business.Interfaces;

public interface INewsIntegrationService
{
    public Task<GetPageCategoriesResponse> GetPageCategoriesAsync();
    public Task<GetListStoriesResponse> GetListStoriesAsync(long? newsId);
    public Task<GetStoryResponse> GetStoryAsync(long newsId);
}