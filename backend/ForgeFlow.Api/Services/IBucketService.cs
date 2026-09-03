using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Models;

namespace ForgeFlow.Api.Services;

public interface IBucketService
{
    Task<IReadOnlyList<BucketDto>> ListAsync(CancellationToken cancellationToken = default);

    Task<BucketDto> CreateAsync(
        string name,
        BucketRetention retention,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string bucketKey, CancellationToken cancellationToken = default);

    Task SetActivationAsync(string bucketKey, bool isActive, CancellationToken cancellationToken = default);

    Task<bool> IsActiveAsync(string bucketKey, CancellationToken cancellationToken = default);
}
