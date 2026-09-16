using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace TruckVisitManagement.Api.Authentication;

/// <summary>
/// Well-known values used by the local (in-memory) authentication scheme.
/// </summary>
public static class LocalAuthenticationDefaults
{
    /// <summary>The authentication scheme name for the local provider.</summary>
    public const string Scheme = "Local";

    /// <summary>Claim type that lists the terminals a principal may access. A value of "*" grants access to all terminals.</summary>
    public const string AuthorizedTerminalClaimType = "authorized_terminal";

    /// <summary>
    /// Request header used (in local/in-memory mode) to explicitly send an unauthenticated request.
    /// This exists so callers and tests can exercise the "no credentials" path.
    /// </summary>
    public const string AnonymousHeader = "X-Anonymous";

    /// <summary>
    /// Request header used (in local/in-memory mode) to scope the principal to a comma-separated
    /// list of authorized terminals. When omitted the principal is granted access to all terminals.
    /// </summary>
    public const string AuthorizedTerminalsHeader = "X-Authorized-Terminals";

    /// <summary>Request header used (in local/in-memory mode) to override the authenticated user name.</summary>
    public const string UserHeader = "X-Debug-User";
}

/// <summary>Options for the local (in-memory) authentication scheme.</summary>
public sealed class LocalAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>The default user name applied when no override header is supplied.</summary>
    public string DefaultUserName { get; set; } = "local-developer";
}

/// <summary>
/// A lightweight authentication handler intended for local development and automated tests.
/// It authenticates requests with a development principal so the API is usable without an
/// external identity provider, while still supporting the "no credentials" and
/// "restricted terminal access" paths.
///
/// This scheme is deliberately isolated behind <see cref="LocalAuthenticationDefaults.Scheme"/>
/// so it can be swapped for a real provider (e.g. Microsoft Entra ID / JWT bearer) via configuration.
/// </summary>
public sealed class LocalAuthenticationHandler : AuthenticationHandler<LocalAuthenticationOptions>
{
    public LocalAuthenticationHandler(
        IOptionsMonitor<LocalAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Allow callers to explicitly request an unauthenticated call.
        if (Request.Headers.TryGetValue(LocalAuthenticationDefaults.AnonymousHeader, out var anonymous)
            && bool.TryParse(anonymous, out var isAnonymous)
            && isAnonymous)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userName = Request.Headers.TryGetValue(LocalAuthenticationDefaults.UserHeader, out var user)
            && !string.IsNullOrWhiteSpace(user)
                ? user.ToString()
                : Options.DefaultUserName;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.NameIdentifier, userName)
        };

        if (Request.Headers.TryGetValue(LocalAuthenticationDefaults.AuthorizedTerminalsHeader, out var terminals)
            && !string.IsNullOrWhiteSpace(terminals))
        {
            foreach (var terminal in terminals.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                claims.Add(new Claim(LocalAuthenticationDefaults.AuthorizedTerminalClaimType, terminal));
            }
        }
        else
        {
            // No restriction supplied: the local development principal can access all terminals.
            claims.Add(new Claim(LocalAuthenticationDefaults.AuthorizedTerminalClaimType, "*"));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
