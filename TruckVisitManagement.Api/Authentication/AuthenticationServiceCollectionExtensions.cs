using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace TruckVisitManagement.Api.Authentication;

/// <summary>
/// Registers authentication and authorization for the API.
/// The active provider is selected via the <c>Authentication:Provider</c> configuration value:
/// <list type="bullet">
///   <item><description><c>Local</c> (default) — an in-memory scheme for local development and tests.</description></item>
///   <item><description><c>Entra</c> — Microsoft Entra ID via JWT bearer tokens.</description></item>
/// </list>
/// This indirection keeps controllers and policies independent of the identity provider,
/// so switching to Entra in the future requires only configuration changes.
/// </summary>
public static class ApiAuthenticationServiceCollectionExtensions
{
    public const string AuthenticatedUserPolicy = "AuthenticatedUser";

    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["Authentication:Provider"] ?? LocalAuthenticationDefaults.Scheme;

        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = provider.Equals("Entra", StringComparison.OrdinalIgnoreCase)
                ? JwtBearerDefaults.AuthenticationScheme
                : LocalAuthenticationDefaults.Scheme;
        });

        if (provider.Equals("Entra", StringComparison.OrdinalIgnoreCase))
        {
            authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                configuration.GetSection("Authentication:Entra").Bind(options);
            });
        }
        else
        {
            authenticationBuilder.AddScheme<LocalAuthenticationOptions, LocalAuthenticationHandler>(
                LocalAuthenticationDefaults.Scheme,
                _ => { });
        }

        services.AddSingleton<ITerminalAccessPolicy, ClaimsTerminalAccessPolicy>();

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticatedUserPolicy, policy => policy.RequireAuthenticatedUser())
            // Require an authenticated user for every endpoint by default. Endpoints that
            // should be publicly accessible must opt out explicitly with [AllowAnonymous].
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }
}
