using AvvaMobile.Core.Caching;
using Microsoft.EntityFrameworkCore;
using Quark.Data;
using Quark.Data.Models.Common;

namespace Quark.Web.Middlewares
{
    public class UrlRedirectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICacheManager _cache;
        private readonly Business.Common.Caching.AppSettingsKeys _appSettingsKeys;

        private const string UrlMappingsCacheKey = "UrlMappingsCache";

        // Entity tipleri
        private static readonly int[] EntityTypes = { 1, 2, 3, 4, 5, 6, 7, 8 };

        public UrlRedirectionMiddleware(RequestDelegate next, ICacheManager cache, Business.Common.Caching.AppSettingsKeys appSettingsKeys)
        {
            _next = next;
            _cache = cache;
            _appSettingsKeys = appSettingsKeys;
        }

        public async Task InvokeAsync(HttpContext context, QuarkDataContext dbContext)
        {
            if (context.Request.Method != "GET" || context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            var requestPath = context.Request.Path.Value.TrimStart('/');

            if (!string.IsNullOrEmpty(requestPath))
            {
                var urlMappings = await GetUrlMappingsAsync(dbContext);
                var requestPathTrimmed = requestPath.TrimStart('/');

                var match = urlMappings
                    .Where(um => um.OldSlug != null && um.OldSlug != um.NewSlug)
                    .FirstOrDefault(um => um.OldSlug.TrimStart('/') == requestPathTrimmed);

                if (match == null)
                {
                    match = urlMappings
                        .Where(um => um.OldSlug != null)
                        .FirstOrDefault(um =>
                        {
                            var slugPath = um.OldSlug.TrimStart('/');

                            switch (um.EntityTypeID)
                            {
                                case 1: // News
                                    return requestPathTrimmed == slugPath;
                                case 2: // Authors
                                    var authorsPrefix = $"{_appSettingsKeys.SlugPrefix_Authors}/";
                                    if (!string.IsNullOrEmpty(requestPathTrimmed) &&
                                        requestPathTrimmed.StartsWith(authorsPrefix))
                                    {
                                        var slug = requestPathTrimmed.Substring(authorsPrefix.Length);
                                        return slug == slugPath;
                                    }
                                    return false;
                                case 3: // Categories  
                                    return requestPathTrimmed == slugPath;
                                case 5: // Pages
                                    return requestPathTrimmed == slugPath;
                                case 7: // Tags  
                                    var tagPrefix = $"{_appSettingsKeys.SlugPrefix_Tags}/";
                                    if (!string.IsNullOrEmpty(requestPathTrimmed) &&
                                        requestPathTrimmed.StartsWith(tagPrefix))
                                    {
                                        var slug = requestPathTrimmed.Substring(tagPrefix.Length);
                                        return slug == slugPath;
                                    }
                                    return false;
                                case 8: // Sources
                                    var sourcesPrefix = $"{_appSettingsKeys.SlugPrefix_Sources}/";
                                    if (!string.IsNullOrEmpty(requestPathTrimmed) &&
                                        requestPathTrimmed.StartsWith(sourcesPrefix))
                                    {
                                        var slug = requestPathTrimmed.Substring(sourcesPrefix.Length);
                                        return slug == slugPath;
                                    }
                                    return false;
                                default:
                                    return false;
                            }
                        });
                }

                if (match != null && !string.IsNullOrEmpty(match.NewSlug))
                {
                    match.NewSlug = match.NewSlug.TrimStart('/');
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
                    context.Response.Headers.Add("Location", "/" + match.NewSlug);
                    await context.Response.CompleteAsync();
                    return;
                }
            }

            await _next(context);
        }

        private async Task<List<UrlMapping>> GetUrlMappingsAsync(QuarkDataContext context)
        {
            var cachedMappings = await _cache.Get<List<UrlMapping>>(UrlMappingsCacheKey);

            if (cachedMappings == null)
            {
                cachedMappings = await context.UrlMappings.ToListAsync();

                await _cache.Set(UrlMappingsCacheKey, cachedMappings);
            }
            return cachedMappings;
        }

        public static async Task RefreshUrlMappingsCache(ICacheManager cache, QuarkDataContext context)
        {
            try
            {
                await cache.Remove(UrlMappingsCacheKey);

                var allMappings = new List<UrlMapping>();

                foreach (var entityType in EntityTypes)
                {
                    var typeMappings = await context.UrlMappings
                        .Where(um => um.EntityTypeID == entityType)
                        .ToListAsync();

                    if (typeMappings != null && typeMappings.Any())
                    {
                        allMappings.AddRange(typeMappings);
                    }

                }
                var serializedMappings = System.Text.Json.JsonSerializer.Serialize(allMappings);
                await cache.Set(UrlMappingsCacheKey, serializedMappings);
            }
            catch
            {
            }
        }
    }

    public static class UrlRedirectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseUrlRedirection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UrlRedirectionMiddleware>();
        }
    }
}