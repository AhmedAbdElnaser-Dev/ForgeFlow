using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskCreateBucketRequest
{
    [JsonPropertyName("bucketKey")]
    public required string BucketKey { get; init; }

    [JsonPropertyName("policyKey")]
    public required string PolicyKey { get; init; }
}
