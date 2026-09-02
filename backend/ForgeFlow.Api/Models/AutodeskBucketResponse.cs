using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskBucketResponse
{
    [JsonPropertyName("bucketKey")]
    public string BucketKey { get; init; } = string.Empty;

    [JsonPropertyName("policyKey")]
    public string PolicyKey { get; init; } = string.Empty;

    // Unix milliseconds.
    [JsonPropertyName("createdDate")]
    public long? CreatedDate { get; init; }
}
