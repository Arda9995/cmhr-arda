using Microsoft.AspNetCore.Routing;

namespace Quark.Web.Middlewares.Interfaces
{
    public interface IRouteHealthCheckService
    {
        Task<bool> ValidateRoutes(IEndpointRouteBuilder endpoints);
        Task<(bool IsValid, string ErrorMessage)> TestRoute(string route, string httpMethod = "GET");
    }
} 