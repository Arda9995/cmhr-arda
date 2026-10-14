namespace Quark.API.Business.Models.Responses.NewsManagementResponses;

public record GetPageCategoriesResponse
{
    public bool Ok { get; set; }
    public List<GetPageCategory> Data { get; set; }
}

public record GetPageCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
}