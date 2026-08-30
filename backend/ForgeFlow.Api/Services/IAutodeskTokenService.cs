using ForgeFlow.Api.Models;

namespace ForgeFlow.Api.Services;

/// <summary>
/// Supplies two-legged (client credentials) Autodesk access tokens.
/// </summary>
public interface IAutodeskTokenService
{
    /// <summary>Returns a cached token, requesting a new one only when the current one is stale.</summary>
    Task<AutodeskAccessToken> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>Convenience wrapper returning just the bearer value.</summary>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
