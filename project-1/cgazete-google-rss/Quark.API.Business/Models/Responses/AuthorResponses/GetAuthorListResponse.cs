namespace Quark.API.Business.Models.Responses.AuthorResponses;

public class GetAuthorListResponse
{
    public int ID { get; set; }
    public string Slug { get; set; }
    public string FullName { get; set; }
    public string ImageURL { get; set; }
    public int TotalNewsCount { get; set; }
    public string Description { get; set; }
    public string LastCreatedNewsSlug { get; set; }
    public string LastCreatedNewsHeadline { get; set; }
    public int LastCreatedNewsID { get; set; }
}