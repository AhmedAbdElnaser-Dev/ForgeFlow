namespace ForgeFlow.Api.Data.Entities;

public class Bucket
{
    public const int BucketKeyMaxLength = 128;

    public string BucketKey { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }
}
