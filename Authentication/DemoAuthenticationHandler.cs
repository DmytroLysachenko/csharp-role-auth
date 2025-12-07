using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

/// <summary>
/// Lightweight authentication handler for demos: builds a ClaimsPrincipal from request headers
/// so you can try role/claim authorization without a full login flow.
/// </summary>
public class DemoAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Demo";

    public DemoAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Expect headers like:
        // X-User-Name: alice
        // X-User-Role: Admin
        // X-Department: IT
        if (!Request.Headers.TryGetValue("X-User-Name", out var username) || string.IsNullOrWhiteSpace(username))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username!),
        };

        if (Request.Headers.TryGetValue("X-User-Role", out var role) && !string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role!));
        }

        if (Request.Headers.TryGetValue("X-Department", out var department) && !string.IsNullOrWhiteSpace(department))
        {
            claims.Add(new Claim("Department", department!));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
