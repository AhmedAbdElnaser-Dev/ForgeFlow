namespace ForgeFlow.Api.Contracts;

public record SetBucketActivationRequest
{
    public bool IsActive { get; init; }
}
