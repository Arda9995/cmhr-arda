using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Data;
using Quark.Resources;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using AvvaMobile.Core.Caching;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Common.BaseServices
{
    public class BaseService
    {
        protected IHttpContextAccessor _httpContext;
        protected QuarkDataContext _context;
        protected IStringLocalizer<CommonResource> _localizer;
        protected AppSettingsKeys _appSettingsKeys;
        protected ICacheManager _cache;
        
        public BaseService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache)
        {
            _httpContext = httpContext;
            _context = context;
            _localizer = localizer;
            _appSettingsKeys = appSettingsKeys;
            _cache = cache;
        }

        /// <summary>
        /// Every user has to have a token even if not logged in. This property returns that user is logged in or not.
        /// </summary>
        public bool CurrentUserAuthenticated
        {
            get
            {
                return bool.Parse(_httpContext.HttpContext.User.Identities
                    .FirstOrDefault(u => u.IsAuthenticated && u.HasClaim(c => c.Type == ClaimTypes.Authentication))
                    ?.FindFirst(ClaimTypes.Authentication).Value);
            }
        }

        /// <summary>
        /// If user logged in returns user id as an int value. Otherwise throw an exception. Be sure to user is logged in or not (CurrentUserAuthenticated).
        /// </summary>
        public int CurrentUserID
        {
            get
            {
                return int.Parse(_httpContext.HttpContext.User.Identities.FirstOrDefault(u => u.IsAuthenticated && u.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))?.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
        }

        /// <summary>
        /// If user not logged in, a guid identifier set as user id. Be sure to user is logged in or not (CurrentUserAuthenticated).
        /// </summary>
        public string CurrentUserAnonymousID
        {
            get
            {
                return _httpContext.HttpContext.User.Identities
                    .FirstOrDefault(u => u.IsAuthenticated && u.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                    ?.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        public int GetCurrentLanguageID(IStringLocalizer<CommonResource> localizer)
        {
            return int.Parse(localizer["Current_Language_ID"]);
        }

        /// <summary>
        /// Be sure to user is logged in or not (CurrentUserAuthenticated).
        /// </summary>
        public string CurrentUserName
        {
            get
            {
                return _httpContext.HttpContext.User.Identities.FirstOrDefault(u => u.IsAuthenticated && u.HasClaim(c => c.Type == ClaimTypes.Name))?.FindFirst(ClaimTypes.Name).Value;
            }
        }
        public static string GetMethodName([CallerMemberName] string name = "") => name;
        
        public bool HasRight(string right)
        {
            return _httpContext.HttpContext.User.Identities.First().FindAll(ClaimTypes.Role).FirstOrDefault(r => r.Value == right) != null;
        }
    }
}