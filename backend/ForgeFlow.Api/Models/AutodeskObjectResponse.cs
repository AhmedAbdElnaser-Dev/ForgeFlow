using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskObjectResponse
{
    [JsonPropertyName("objectKey")]
    public string ObjectKey { get; init; } = string.Empty;

    // URN-style id, e.g. urn:adsk.objects:os.object:bucket/file.rvt
    [JsonPropertyName("objectId")]
    public string ObjectId { get; init; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; init; }
}
