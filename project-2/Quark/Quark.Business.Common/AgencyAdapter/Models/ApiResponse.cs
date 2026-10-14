using System.Text.Json.Serialization;

namespace Quark.Business.Common.AgencyAdapter.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("response")]
        public ResponseInfo Response { get; set; }
    }

    public class ResponseInfo
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    // Discover response
    public class DiscoverData
    {
        [JsonPropertyName("provider")]
        public Dictionary<string, string> Providers { get; set; }

        [JsonPropertyName("category")]
        public Dictionary<string, string> Categories { get; set; }

        [JsonPropertyName("priority")]
        public Dictionary<string, string> Priorities { get; set; }

        [JsonPropertyName("package")]
        public Dictionary<string, string> Packages { get; set; }

        [JsonPropertyName("type")]
        public Dictionary<string, string> Types { get; set; }

        [JsonPropertyName("language")]
        public Dictionary<string, string> Languages { get; set; }
    }

    // Search response
    public class SearchData
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("result")]
        public List<SearchResultItem> Result { get; set; }
    }

    public class SearchResultItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("category_id")]
        public int CategoryID { get; set; }
    }
    public class SearchParameters
    {
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = "*"; // default: en eski kayıttan başla

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; } = "NOW"; // default: şu an

        [JsonPropertyName("filter_category")]
        public string FilterCategory { get; set; }

        [JsonPropertyName("filter_type")]
        public string FilterType { get; set; }

        [JsonPropertyName("filter_language")]
        public string FilterLanguage { get; set; }
    }
    public class SubscriptionData
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("result")]
        public List<SubscriptionItem> Result { get; set; }
    }

    public class SubscriptionItem
    {
        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("package")]
        public string Package { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("photo_size")]
        public string PhotoSize { get; set; }

        [JsonPropertyName("video_size")]
        public string VideoSize { get; set; }

        [JsonPropertyName("graph_size")]
        public string GraphSize { get; set; }

        [JsonPropertyName("archive_days")]
        public int ArchiveDays { get; set; }

        [JsonPropertyName("download_limit")]
        public int DownloadLimit { get; set; }
    }
    public class DocumentParameters
    {
        public string Id { get; set; }      // örn: aa:text:20161012:9258343
        public string Format { get; set; }  // newsml29, newsml12, web, print, sd
    }
    public class TokenParameters
    {
        public string Id { get; set; }    // örn: aa:picture:20161012:9258465
        public string Size { get; set; }  // sd, print, web
    }
    public class MultiTokenParameters
    {
        public string GroupId { get; set; } // örn: aa:video:20161012:9258512
        public string Size { get; set; }    // sd, print, web
    }

    public class MultiTokenData
    {
        [JsonPropertyName("result")]
        public List<MultiTokenItem> Result { get; set; }
    }

    public class MultiTokenItem
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
    public class DocumentData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
        // NewsML / XML metni burada dönebilir
    }

    public class NewsItem
    {
        public string AgencyNewsID { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
        public int CMSCategoryID { get; set; }
    }
}
