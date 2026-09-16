using System.Security.Claims;

namespace TruckVisitManagement.Api.Authentication;

/// <summary>
/// Evaluates whether the authenticated principal may access a given terminal.
/// Encapsulates the Terminal Access authorization rule so it can evolve independently
/// of the identity provider used to authenticate the caller.
/// </summary>
public interface ITerminalAccessPolicy
{
    /// <summary>
    /// Returns <c>true</c> when the principal is authorized for the supplied terminal.
    /// A <paramref name="terminalId"/> of <c>null</c> or whitespace means "no terminal filter"
    /// and is always allowed.
    /// </summary>
    bool CanAccessTerminal(ClaimsPrincipal principal, string? terminalId);
}

/// <summary>
/// Default terminal access policy driven by <see cref="LocalAuthenticationDefaults.AuthorizedTerminalClaimType"/>
/// claims. A claim value of "*" grants access to every terminal.
/// </summary>
public sealed class ClaimsTerminalAccessPolicy : ITerminalAccessPolicy
{
    public bool CanAccessTerminal(ClaimsPrincipal principal, string? terminalId)
    {
        if (string.IsNullOrWhiteSpace(terminalId))
        {
            return true;
        }

        var authorizedTerminals = principal
            .FindAll(LocalAuthenticationDefaults.AuthorizedTerminalClaimType)
            .Select(c => c.Value)
            .ToArray();

        // When the principal carries no terminal claims at all, treat it as unrestricted
        // so that providers that do not emit terminal claims are not locked out.
        if (authorizedTerminals.Length == 0)
        {
            return true;
        }

        return authorizedTerminals.Any(t =>
            t == "*" || string.Equals(t, terminalId, StringComparison.OrdinalIgnoreCase));
    }
}
