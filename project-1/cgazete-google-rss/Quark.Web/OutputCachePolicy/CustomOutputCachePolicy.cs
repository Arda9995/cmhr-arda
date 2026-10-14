using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Quark.Web.OutputCachePolicy
{
    public class CustomOutputCachePolicy : IOutputCachePolicy
    {
        private static int GetTTLFromRequest(HttpContext context)
        {
            var ttl = 30;
            var controller = context.Request.RouteValues["controller"]?.ToString();
            var action = context.Request.RouteValues["action"]?.ToString();

            if (controller == "Account")
            {
                if (action == "LogIn")
                {
                    ttl = 600;
                }
                else if (action == "Register")
                {
                    ttl = 600;
                }
                else if (action == "SuccessPayment")
                {
                    ttl = 600;
                }
                else if (action == "Support")
                {
                    ttl = 600;
                }
                else if (action == "MembershipIntroduction")
                {
                    ttl = 300;
                }
                else if (action == "ForgotPassword")
                {
                    ttl = 300;
                }
                else if (action == "ChangePassword")
                {
                    ttl = 30;
                }
                else if (action == "Payment")
                {
                    ttl = 30;
                }
            }
            else if (controller == "Author")
            {
                if (action == "Authors")
                {
                    ttl = 60;
                }
                else if (action == "AuthorDetails")
                {
                    ttl = 30;
                }
                else if (action == "AuthorNewsList")
                {
                    ttl = 60;
                }
                else if (action == "AuthorCategoryNewsList")
                {
                    ttl = 60;
                }
            }
            else if (controller == "Common")
            {
                if (action == "Currency")
                {
                    ttl = 60;
                }
                else if (action == "Weather")
                {
                    ttl = 600;
                }
                else if (action == "WeatherDetail")
                {
                    ttl = 600;
                }
                else if (action == "PrayerTimes")
                {
                    ttl = 600;
                }
                else if (action == "ReadList")
                {
                    ttl = 30;
                }
            }
            else if (controller == "Home")
            {
                if (action == "Index")
                {
                    ttl = 30;
                }
                else if (action == "Error")
                {
                    ttl = 1800;
                }
                else if (action == "PageNotFound")
                {
                    ttl = 1800;
                }
                else if (action == "PageSystemError")
                {
                    ttl = 1800;
                }
            }
            else if (controller == "News")
            {
                if (action == "Details")
                {
                    ttl = 30;
                }
                else if (action == "DetailNextNews")
                {
                    ttl = 30;
                }
                else if (action == "Search")
                {
                    ttl = 300;
                }
                else if (action == "SearchedNews")
                {
                    ttl = 60;
                }
                else if (action == "GetSurveyResult")
                {
                    ttl = 15;
                }
            }
            else if (controller == "Slug")
            {
                if (action == "SlugHandler")
                {
                    ttl = 180;
                }
            }            
            else if (controller == "Source")
            {
                if (action == "NewsList")
                {
                    ttl = 180;
                }
            }            
            else if (controller == "Tag")
            {
                if (action == "NewsList")
                {
                    ttl = 180;
                }
            }            
            else if (controller == "XmlFeed")
            {
                if (action == "NewsSitemap")
                {
                    ttl = 10;
                }
                else if(action == "PostsSitemap")
                {
                    ttl = 10;
                }                
                else if(action == "MonthlyPostsSitemap")
                {
                    ttl = 10;
                }                
                else if(action == "SourceSitemap")
                {
                    ttl = 10;
                }                
                else if(action == "TagsSitemap")
                {
                    ttl = 10;
                }                
                else if(action == "MonthlyTagsSitemap")
                {
                    ttl = 10;
                }                
                else if(action == "MainRss")
                {
                    ttl = 10;
                }                
                else if(action == "CategoryRss")
                {
                    ttl = 10;
                }
                else if (action == "GoogleAiRss")
                {
                    ttl = 0;
                }
            }

            return ttl;
        }

        ValueTask IOutputCachePolicy.CacheRequestAsync(
               OutputCacheContext context,
               CancellationToken cancellationToken)
        {
            if (context == null || context.HttpContext == null)
            {
                return ValueTask.CompletedTask;
            }

            var requestPath = context.HttpContext.Request.Path.Value?.ToLowerInvariant();

            if (!string.IsNullOrEmpty(requestPath) &&
                requestPath.Contains("/feeds/google-ai.xml"))
            {
                context.EnableOutputCaching = false;
                context.AllowCacheLookup = false;
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }
            
            context.EnableOutputCaching = true;
            context.AllowCacheLookup = true;
            context.AllowCacheStorage = true;
            context.AllowLocking = true;
            context.CacheVaryByRules.QueryKeys = "*";

            var ttl = GetTTLFromRequest(context.HttpContext);
            context.ResponseExpirationTimeSpan = TimeSpan.FromSeconds(ttl);

            var user = context.HttpContext.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated == true;

            string email = "guest";

            if (isAuthenticated)
            {
                var hasPremiumship = user?.Identities
                    .FirstOrDefault(u => u.IsAuthenticated && u.HasClaim(c => c.Type == "HasPremiumMembership"))
                    ?.FindFirst("HasPremiumMembership")?.Value;
                
                email = "user";
                
                if (hasPremiumship != "False")
                {
                    email = user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "user";
                }
            }

            var isMobile = false;
            string? device = null;
            var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString().ToLowerInvariant();
            isMobile = userAgent.Contains("iphone") || userAgent.Contains("android") || userAgent.Contains("mobile");
            Regex OSControl = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex deviceControl = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            
            if (OSControl.IsMatch(userAgent))
            {
                device = OSControl.Match(userAgent).Groups[0].Value;
            }
          

            if (OSControl.IsMatch(userAgent))
            {
                isMobile = true;
            }

            var varyByValues = new Dictionary<string, string>
            {
                { "auth", email },
                { "device", isMobile ? "mobile" : "desktop" }
            };

            foreach (var kvp in varyByValues)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        context.CacheVaryByRules.VaryByValues.Add(kvp.Key, kvp.Value);
                    }
                }
            
            
            

            return ValueTask.CompletedTask;
        }

        ValueTask IOutputCachePolicy.ServeFromCacheAsync
            (OutputCacheContext context, CancellationToken cancellationToken)
        {
           

            var user = context.HttpContext.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated == true;

            string email = "guest";

            if (isAuthenticated)
            {
                var hasPremiumship = user?.Identities
                    .FirstOrDefault(u => u.IsAuthenticated && u.HasClaim(c => c.Type == "HasPremiumMembership"))
                    ?.FindFirst("HasPremiumMembership")?.Value;

                email = "user";
                
                if (hasPremiumship != "False")
                {
                    email = user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "user";
                }
            }

            context.HttpContext.Items["auth"] = email;

            var isMobile = false;
            var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString().ToLowerInvariant();
            isMobile = userAgent.Contains("iphone") || userAgent.Contains("android") || userAgent.Contains("mobile");
            Regex OSControl = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex deviceControl = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (OSControl.IsMatch(userAgent))
            {
                isMobile = true;
            }

            context.HttpContext.Items["IsMobile"] = isMobile;

            if (isMobile)
            {
                context.HttpContext.Items["device"] = "mobile";
            }
            else
            {
                context.HttpContext.Items["device"] = "desktop";
            }

            return ValueTask.CompletedTask;
        }

        ValueTask IOutputCachePolicy.ServeResponseAsync
            (OutputCacheContext context, CancellationToken cancellationToken)
        {
            var response = context.HttpContext.Response;

            if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            if (response.StatusCode != StatusCodes.Status200OK &&
                response.StatusCode != StatusCodes.Status301MovedPermanently)
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            return ValueTask.CompletedTask;
        }

        private static bool AttemptOutputCaching(OutputCacheContext context)
        {
            var request = context.HttpContext.Request;

            if (!HttpMethods.IsGet(request.Method) &&
                !HttpMethods.IsHead(request.Method) &&
                !HttpMethods.IsPost(request.Method))
            {
                return false;
            }

            return true;
        }
    }
}
