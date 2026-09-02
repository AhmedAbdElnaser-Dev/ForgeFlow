using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskBucketListResponse
{
    [JsonPropertyName("items")]
    public List<AutodeskBucketResponse> Items { get; init; } = [];
}
