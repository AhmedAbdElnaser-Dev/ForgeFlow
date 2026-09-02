namespace ForgeFlow.Api.Contracts;

public record BucketDto
{
    public string BucketKey { get; init; } = string.Empty;

    public string PolicyKey { get; init; } = string.Empty;

    public DateTimeOffset? CreatedAtUtc { get; init; }
}
