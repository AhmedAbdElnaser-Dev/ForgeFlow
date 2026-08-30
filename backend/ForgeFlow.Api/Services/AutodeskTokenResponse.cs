using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Services;

/// <summary>Raw shape of the Autodesk token endpoint response.</summary>
internal sealed record AutodeskTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}
