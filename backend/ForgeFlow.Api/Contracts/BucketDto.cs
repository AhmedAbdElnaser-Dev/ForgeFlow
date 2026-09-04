namespace ForgeFlow.Api.Contracts;

public record BucketDto
{
    // The bucket name without the client id prefix. The full key stays server-side.
    public string Name { get; init; } = string.Empty;

    // How long objects survive: transient, temporary or persistent.
    public string PolicyKey { get; init; } = string.Empty;

    public DateTimeOffset? CreatedAtUtc { get; init; }

    // ForgeFlow state, not Autodesk's: only active buckets may be read from or written to.
    public bool IsActive { get; init; }
}
