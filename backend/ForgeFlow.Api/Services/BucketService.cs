using System.Net;
using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Data;
using ForgeFlow.Api.Data.Entities;
using ForgeFlow.Api.Models;
using ForgeFlow.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Services;

public class BucketService(
    IAutodeskApiClient autodesk,
    ForgeFlowDbContext database,
    IOptions<AutodeskOptions> options,
    ILogger<BucketService> logger) : IBucketService
{
    private const string BucketsPath = "oss/v2/buckets";

    private readonly AutodeskOptions _options = options.Value;

    public async Task<IReadOnlyList<BucketDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await autodesk.GetAsync(
            BucketsPath,
            AutodeskScope.BucketRead,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<AutodeskBucketListResponse>(cancellationToken);
        if (payload is null)
        {
            return [];
        }

        var activeKeys = await database.Buckets
            .Where(bucket => bucket.IsActive)
            .Select(bucket => bucket.BucketKey)
            .ToListAsync(cancellationToken);

        return payload.Items
            .Select(bucket => ToDto(bucket, activeKeys.Contains(bucket.BucketKey)))
            .ToList();
    }

    public async Task<IReadOnlyList<BucketDto>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        var buckets = await ListAsync(cancellationToken);

        return [.. buckets.Where(bucket => bucket.IsActive)];
    }

    public async Task<BucketDto> CreateAsync(
        string name,
        BucketRetention retention,
        CancellationToken cancellationToken = default)
    {
        var body = new AutodeskCreateBucketRequest
        {
            BucketKey = BuildBucketKey(name),
            PolicyKey = retention.ToString().ToLowerInvariant(),
        };

        using var response = await autodesk.PostJsonAsync(
            BucketsPath,
            body,
            AutodeskScope.BucketCreate,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var created = await response.Content.ReadFromJsonAsync<AutodeskBucketResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk returned an empty bucket response.");

        logger.LogInformation("Created Autodesk bucket {BucketKey}.", created.BucketKey);

        return ToDto(created, isActive: false);
    }

    public async Task DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        var bucketKey = BuildBucketKey(name);

        using var response = await autodesk.DeleteAsync(
            $"{BucketsPath}/{bucketKey}",
            AutodeskScope.BucketDelete,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        await database.Buckets
            .Where(bucket => bucket.BucketKey == bucketKey)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted Autodesk bucket {BucketKey}.", bucketKey);
    }

    public async Task SetActivationAsync(
        string name,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var bucketKey = BuildBucketKey(name);

        var bucket = await database.Buckets
            .FirstOrDefaultAsync(entry => entry.BucketKey == bucketKey, cancellationToken);

        if (bucket is null)
        {
            bucket = new Bucket { BucketKey = bucketKey };
            database.Buckets.Add(bucket);
        }

        bucket.IsActive = isActive;
        bucket.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await database.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Bucket {BucketKey} is now {State}.",
            bucketKey,
            isActive ? "active" : "inactive");
    }

    public Task<bool> IsActiveAsync(string name, CancellationToken cancellationToken = default)
    {
        var bucketKey = BuildBucketKey(name);

        return database.Buckets
            .AnyAsync(entry => entry.BucketKey == bucketKey && entry.IsActive, cancellationToken);
    }

    // Bucket names are unique across all of Autodesk, so prefix them with the client id.
    private string KeyPrefix => $"{_options.ClientId.ToLowerInvariant()}-";

    private string BuildBucketKey(string name) => KeyPrefix + name.ToLowerInvariant();

    // Buckets created by other applications keep their key, since the prefix will not match.
    private string ToDisplayName(string bucketKey) =>
        bucketKey.StartsWith(KeyPrefix, StringComparison.OrdinalIgnoreCase)
            ? bucketKey[KeyPrefix.Length..]
            : bucketKey;

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        throw response.StatusCode switch
        {
            HttpStatusCode.Conflict => new BucketAlreadyExistsException(body),
            HttpStatusCode.NotFound => new BucketNotFoundException(body),
            _ => new HttpRequestException(
                $"Autodesk bucket request failed with {(int)response.StatusCode}: {body}"),
        };
    }

    private BucketDto ToDto(AutodeskBucketResponse bucket, bool isActive) => new()
    {
        Name = ToDisplayName(bucket.BucketKey),
        PolicyKey = bucket.PolicyKey,
        IsActive = isActive,
        CreatedAtUtc = bucket.CreatedDate is null
            ? null
            : DateTimeOffset.FromUnixTimeMilliseconds(bucket.CreatedDate.Value),
    };
}
