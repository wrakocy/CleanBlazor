using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Wrak.CleanArchitecture.Web.Identity;

namespace Wrak.CleanArchitecture.Web.Filters;

public class DevelopmentAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevelopmentAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(Claims.DevUser.Type, Claims.DevUser.Value),
            new Claim(Claims.UserName.Type, Claims.DevUser.Value),
            new Claim(Claims.FullName.Type, Claims.DevUser.Value),
            new Claim(Claims.FirstName.Type, Claims.DevUser.Value.Split("-").First()),
            new Claim(Claims.LastName.Type, Claims.DevUser.Value.Split("-").Last ()),
            new Claim(ClaimTypes.Role, "Developer")
        };

        var identity = new ClaimsIdentity(claims, "DevelopmentAuth");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "DevelopmentAuth");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
