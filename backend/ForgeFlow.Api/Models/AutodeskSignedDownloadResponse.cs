using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskSignedDownloadResponse
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}
