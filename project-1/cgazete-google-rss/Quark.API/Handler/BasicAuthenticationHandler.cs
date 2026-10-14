using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Quark.API.Models;

public class BasicAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly BasicAuthOptions _basicAuth;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IOptions<BasicAuthOptions> basicAuthOptions)
        : base(options, logger, encoder, clock)
    {
        _basicAuth = basicAuthOptions.Value;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

        var header = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

        if (!header.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Scheme"));

        var bytes = Convert.FromBase64String(header.Parameter!);
        var credentials = Encoding.UTF8.GetString(bytes).Split(':', 2);

        var username = credentials[0];
        var password = credentials[1];

        if (username != _basicAuth.Username || password != _basicAuth.Password)
            return Task.FromResult(AuthenticateResult.Fail("Invalid Credentials"));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}