using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskSignedUploadResponse
{
    [JsonPropertyName("uploadKey")]
    public string UploadKey { get; init; } = string.Empty;

    // One entry per part. A single-part upload returns exactly one URL.
    [JsonPropertyName("urls")]
    public List<string> Urls { get; init; } = [];
}
