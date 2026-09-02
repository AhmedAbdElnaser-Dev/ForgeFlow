namespace ForgeFlow.Api.Models;

public record AutodeskAccessToken(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc)
{
    public int ExpiresInSeconds =>
        Math.Max(0, (int)(ExpiresAtUtc - DateTimeOffset.UtcNow).TotalSeconds);
}
