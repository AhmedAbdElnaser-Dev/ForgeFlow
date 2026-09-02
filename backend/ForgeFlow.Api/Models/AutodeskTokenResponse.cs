using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    // Seconds from now, so it becomes a timestamp on arrival.
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}
