using System.Text.Json.Serialization;

namespace ForgeFlow.Api.Models;

internal sealed record AutodeskObjectListResponse
{
    [JsonPropertyName("items")]
    public List<AutodeskObjectResponse> Items { get; init; } = [];
}
