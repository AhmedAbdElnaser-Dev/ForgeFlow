using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskCompleteUploadRequest
{
    [JsonPropertyName("uploadKey")]
    public required string UploadKey { get; init; }
}
