using AvvaMobile.Core.Handlers.JwtToken;

namespace Quark.API.Business.Models.Response.LoginResponses
{
    public class LoginResponse
    {
        public bool IsNewUser { get; set; }
        public Token Token { get; set; }
    }
}