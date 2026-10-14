using Newtonsoft.Json;
using Quark.Business.Common.AgencyAdapter.Models;
using Quark.Data.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace Quark.Business.Common.AgencyAdapter
{
    public class AgencyAdapterService : IAgencydapterService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AgencyAdapterService()
        {
            string username = "portal.api";
            string password = "Aa2WebPortal_12";
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.aa.com.tr/")
            };

            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }


        // --- Search ---
        public async Task<ApiResponse<SearchData>> SearchAsync(SearchParameters parameters)
        {
            await Task.Delay(500); // 500ms bekleme süresi
            var _baseUrl = "https://api.aa.com.tr/";
            var url = $"{_baseUrl}/abone/search/";
            var formData = new List<KeyValuePair<string, string>>();

            // Tarih filtreleri
            if (!string.IsNullOrEmpty(parameters.StartDate))
                formData.Add(new KeyValuePair<string, string>("start_date", parameters.StartDate));

            if (!string.IsNullOrEmpty(parameters.EndDate))
                formData.Add(new KeyValuePair<string, string>("end_date", parameters.EndDate));

            if (!string.IsNullOrEmpty(parameters.FilterCategory))
                formData.Add(new KeyValuePair<string, string>("filter_category", parameters.FilterCategory));


            if (!string.IsNullOrEmpty(parameters.FilterType))
                formData.Add(new KeyValuePair<string, string>("filter_type", parameters.FilterType));

            if (!string.IsNullOrEmpty(parameters.FilterLanguage))
                formData.Add(new KeyValuePair<string, string>("filter_language", parameters.FilterLanguage));

            var formContent = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync(url, formContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse<SearchData>>(jsonContent);
            }

            throw new Exception($"Search isteği başarısız: {response.StatusCode}");
        }

        // Document
        public async Task<string> DocumentRawAsync(DocumentParameters parameters)
        {
            var response = await _httpClient.GetAsync($"abone/document/{parameters.Id}/{parameters.Format}");
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }


        public void Dispose() => _httpClient?.Dispose();


        public async Task<List<NewsItem>> GetRssItemsAsync(RssLink rssLink)
        {
            using var client = new HttpClient();
            var items = new List<NewsItem>();

            try
            {
                // Gereksiz bekleme, test için koyduysan kalsın
                await Task.Delay(10000);

                var xml = await client.GetStringAsync(rssLink.Link);

                var xdoc = XDocument.Parse(xml);

                var categoryMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Spor", 8 },
            { "Ekonomi", 5 },
            { "Güvenlik", 3 },
            { "Gündem", 3 },
            { "Eğitim", 13 },
            { "Genel", 3 },
            { "Sağlık", 11 },
            { "Dış Haberler", 4 },
            { "Politika", 2 },
            {"Asayiş", 6 }
        };

                if (rssLink.AgencyID == 4)
                {
                    var news = xdoc.Descendants("item")
                        .Select(x =>
                        {
                            var rssCategory = (string)x.Element("category") ?? "Genel";
                            var cmsCategoryId = categoryMap.ContainsKey(rssCategory)
                                ? categoryMap[rssCategory]
                                : 3;

                            return new NewsItem
                            {
                                AgencyNewsID = (string)x.Element("HaberKodu"),
                                Title = (string)x.Element("title"),
                                Description = (string)x.Element("description"),
                                PubDate = (string)x.Element("pubDate"),
                                Category = rssCategory,
                                CMSCategoryID = cmsCategoryId
                            };
                        })
                        .ToList();
                    items.AddRange(news);
                }
                else if (rssLink.AgencyID == 3)
                {
                    var news = xdoc.Descendants("item")
                        .Select(x =>
                        {
                            var rssCategory = (string)x.Element("category") ?? "Genel";
                            var cmsCategoryId = categoryMap.ContainsKey(rssCategory)
                                ? categoryMap[rssCategory]
                                : 3;

                            return new NewsItem
                            {
                                AgencyNewsID = (string)x.Element("newsId"),
                                Title = (string)x.Element("title"),
                                Description = (string)x.Element("description"),
                                PubDate = (string)x.Element("pubDate"),
                                Category = rssCategory,
                                CMSCategoryID = cmsCategoryId
                            };
                        })
                        .ToList();
                    items.AddRange(news);
                }
            }
            catch (HttpRequestException ex)
            {
                // İnternet bağlantısı / URL hataları
                Console.WriteLine($"RSS bağlantı hatası: {ex.Message}");
            }
            catch (XmlException ex)
            {
                // XML format hataları
                Console.WriteLine($"RSS XML parse hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Beklenmeyen hatalar
                Console.WriteLine($"RSS okuma sırasında hata: {ex.Message}");
            }

            return items;
        }
    }
}