namespace ForgeFlow.Api.Services;

/// <summary>
/// A two-legged Autodesk access token and when it stops being valid.
/// </summary>
/// <param name="AccessToken">The bearer token itself.</param>
/// <param name="TokenType">Token type reported by Autodesk, normally "Bearer".</param>
/// <param name="ExpiresAtUtc">Absolute expiry, derived from the response's expires_in.</param>
public record AutodeskAccessToken(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc)
{
    public int ExpiresInSeconds =>
        Math.Max(0, (int)(ExpiresAtUtc - DateTimeOffset.UtcNow).TotalSeconds);
}
