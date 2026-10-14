using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Routing;
using Quark.Web.Middlewares.Interfaces;
using System.Net;
using System.Text;

namespace Quark.Web.Middlewares.Services
{
    public class RouteHealthCheckService : IRouteHealthCheckService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RouteHealthCheckService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private string _baseUrl;
        private readonly HttpClient _client;
        private readonly string _logFilePath;
        private readonly StringBuilder _logBuilder;
        private readonly AppSettingsKeys _appSettingsKeys;

        public RouteHealthCheckService(
            IHttpClientFactory httpClientFactory,
            ILogger<RouteHealthCheckService> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            AppSettingsKeys appSettingsKeys)
        {
            _appSettingsKeys = appSettingsKeys;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _logBuilder = new StringBuilder();

            var tempFolder = Path.Combine(_environment.WebRootPath, "Temp");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            _logFilePath = Path.Combine(tempFolder, "routelog.txt");

            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }

            _baseUrl = DetermineBaseUrl();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _client = new HttpClient(handler);

            WriteToLog($"Route Health Check Started at {DateTime.Now}");
            WriteToLog($"Base URL: {_baseUrl}");
            WriteToLog("----------------------------------------");
        }

        private void WriteToLog(string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            _logBuilder.AppendLine(logMessage);
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            _logger.LogInformation(message);
        }

        private string DetermineBaseUrl()
        {
            var configUrl = _configuration["RouteSettings:BaseURL"]?.TrimEnd('/');
            if (!string.IsNullOrEmpty(configUrl))
            {
                return configUrl;
            }

            var urls = _configuration.GetValue<string>("Urls")?.Split(';');
            if (urls?.Any() == true)
            {
                var httpsUrl = urls.FirstOrDefault(u => u.StartsWith("https://"));
                if (!string.IsNullOrEmpty(httpsUrl))
                {
                    return httpsUrl.TrimEnd('/');
                }

                var httpUrl = urls.FirstOrDefault(u => u.StartsWith("http://"));
                if (!string.IsNullOrEmpty(httpUrl))
                {
                    return httpUrl.TrimEnd('/');
                }
            }

            if (_environment.IsDevelopment())
            {
                return "https://localhost:7295";
            }

            return _appSettingsKeys.Common_ClientWebSite;
        }

        public async Task<bool> ValidateRoutes(IEndpointRouteBuilder endpoints)
        {
            WriteToLog("Starting route validation...");

            // İlk bağlantı kontrolü
            for (int i = 0; i < 10; i++)
            {
                if (await IsPortOpen())
                {
                    WriteToLog($"Successfully connected to {_baseUrl} on attempt {i + 1}");
                    break;
                }
                
                if (i < 2)
                {
                    WriteToLog($"Connection attempt {i + 1} failed, waiting 5 seconds before retry...");
                    await Task.Delay(5000);
                }
            }

            // HTTP/HTTPS kontrolü
            if (!await IsPortOpen())
            {
                var httpUrl = _baseUrl.Replace("https://", "http://");
                _baseUrl = httpUrl;
                
                if (!await IsPortOpen())
                {
                    var message = $"Neither HTTPS ({_baseUrl.Replace("http://", "https://")}) nor HTTP ({_baseUrl}) is accessible.";
                    WriteToLog(message);
                    WriteToLog("Route validation skipped.");
                    return true;
                }
                else
                {
                    WriteToLog($"Using HTTP URL: {_baseUrl} for route validation");
                }
            }
            else
            {
                WriteToLog($"Using URL: {_baseUrl} for route validation");
            }

            var sampleRoutes = GenerateSampleRoutes(endpoints);
            WriteToLog($"Total routes to test: {sampleRoutes.Count}");
            
            var successCount = 0;
            var failedRoutes = new List<string>();

            // Her route'u sırayla test et
            foreach (var route in sampleRoutes)
            {
                WriteToLog($"Testing route: {route}");
                var startTime = DateTime.Now;
                
                var result = await TestRoute(route);
                var duration = DateTime.Now - startTime;
                
                if (result.IsValid)
                {
                    successCount++;
                    WriteToLog($"Route {route} completed successfully in {duration.TotalSeconds:F2} seconds");
                }
                else
                {
                    failedRoutes.Add(result.ErrorMessage);
                    WriteToLog($"Route {route} failed after {duration.TotalSeconds:F2} seconds: {result.ErrorMessage}");
                }

                // Her test arasında kısa bir bekleme
                await Task.Delay(100);
            }

            WriteToLog($"Successful routes: {successCount}");
            WriteToLog($"Failed routes: {failedRoutes.Count}");

            if (failedRoutes.Any())
            {
                WriteToLog("Failed Routes Details:");
                foreach (var failure in failedRoutes)
                {
                    WriteToLog($"- {failure}");
                }
            }

            WriteToLog("----------------------------------------");
            WriteToLog($"Route validation completed at {DateTime.Now}");
            return true;
        }

        private async Task<bool> IsPortOpen()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, _baseUrl);
                var response = await _client.SendAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                WriteToLog($"URL check failed for {_baseUrl}: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool IsValid, string ErrorMessage)> TestRoute(string route, string httpMethod = "GET")
        {
            try
            {
                var fullUrl = $"{_baseUrl}/{route.TrimStart('/')}";
                using var request = new HttpRequestMessage(new HttpMethod(httpMethod), fullUrl);
                
                var response = await _client.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.NotFound && (int)response.StatusCode < 500)
                {
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        return (true, $"Route {route} requires authorization (403) - This is expected for protected routes");
                    }
                    return (true, null);
                }

                return (false, $"Route {route} returned status code {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (false, $"Route {route} test failed: {ex.Message}");
            }
        }

        private List<string> GenerateSampleRoutes(IEndpointRouteBuilder endpoints)
        {
            var sampleRoutes = new List<string>
            {
                // Ana sayfa
                "",
                
                // Haber detay örnekleri
                //"spor/fenerbahce-galatasaray-derbisi-1",
                //"ekonomi/dolar-kuru-yukseldi-2",
                
                // Yazar sayfaları
                
                "yazarlar",
                "yazarlar/spor-yazarlari",
                "yazarlar/adnan-dincer",
                "gunun-kose-yazilari",
                
                // Arama sayfaları
                "arama",
                "arama/ekonomi",
                
                // Etiket sayfaları
                "haberleri/ekonomi",
                
                // Hava durumu ve namaz vakitleri
                "hava-durumu",
                "hava-durumu/istanbul",
                "namaz-vakitleri",
                "namaz-vakitleri/istanbul",
                
                // Üyelik sayfaları
                "uye/giris",
                "uye/kayit-ol",
                "uye/sifremi-unuttum",
                "uye/abonelik",
                
                // Döviz sayfaları
                "doviz/1-dolar-kac-tl",
                "doviz/1-euro-kac-tl",

                //Statik sayfalar
                "kunye",
                "iletisim",
                //Kategori sayfaları
                "spor",
                "anketler",
                "resmi-ilanlar",
                //haber kaynağı sayfaları
                "haber-kaynagi/aa"
            };

            return sampleRoutes;
        }
    }
}
