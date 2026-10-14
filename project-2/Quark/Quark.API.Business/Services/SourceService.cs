using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.SourceResponses;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class SourceService : BaseService, ISourceServices
    {
        public SourceService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
        }

        public async Task<ServiceResult<List<GetSourceResponse>>> List()
        {
            var result = new ServiceResult<List<GetSourceResponse>>();

            result.Data = await _context.Sources
                .Select(x => new GetSourceResponse
                {
                    ID = x.ID,
                    Name = x.Name,
                    Slug = x.Url,
                })
                .ToListAsync();

            return result;
        }
    }
}
