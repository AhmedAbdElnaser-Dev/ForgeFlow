using System.Net;
using System.Net.Http.Headers;
using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Data;
using ForgeFlow.Api.Data.Entities;
using ForgeFlow.Api.Models;
using ForgeFlow.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Services;

public class BucketService(
    IHttpClientFactory httpClientFactory,
    IAutodeskTokenService tokenService,
    ForgeFlowDbContext database,
    IOptions<AutodeskOptions> options,
    ILogger<BucketService> logger) : IBucketService
{
    private const string BucketsPath = "oss/v2/buckets";

    private readonly AutodeskOptions _options = options.Value;

    public async Task<IReadOnlyList<BucketDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(
            HttpMethod.Get,
            BucketsPath,
            AutodeskScope.BucketRead,
            content: null,
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

        using var content = JsonContent.Create(body);
        using var response = await SendAsync(
            HttpMethod.Post,
            BucketsPath,
            AutodeskScope.BucketCreate,
            content,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var created = await response.Content.ReadFromJsonAsync<AutodeskBucketResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk returned an empty bucket response.");

        logger.LogInformation("Created Autodesk bucket {BucketKey}.", created.BucketKey);

        return ToDto(created, isActive: false);
    }

    public async Task DeleteAsync(string bucketKey, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(
            HttpMethod.Delete,
            $"{BucketsPath}/{bucketKey}",
            AutodeskScope.BucketDelete,
            content: null,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        await database.Buckets
            .Where(bucket => bucket.BucketKey == bucketKey)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted Autodesk bucket {BucketKey}.", bucketKey);
    }

    public async Task SetActivationAsync(
        string bucketKey,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
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

    public Task<bool> IsActiveAsync(string bucketKey, CancellationToken cancellationToken = default) =>
        database.Buckets
            .AnyAsync(entry => entry.BucketKey == bucketKey && entry.IsActive, cancellationToken);

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        AutodeskScope scopes,
        HttpContent? content,
        CancellationToken cancellationToken)
    {
        var bearer = await tokenService.GetAccessTokenAsync(scopes, cancellationToken);

        using var request = new HttpRequestMessage(method, path) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

        var client = httpClientFactory.CreateClient(AutodeskTokenService.HttpClientName);

        return await client.SendAsync(request, cancellationToken);
    }

    // Bucket names are unique across all of Autodesk, so prefix them with the client id.
    private string BuildBucketKey(string name) =>
        $"{_options.ClientId.ToLowerInvariant()}-{name.ToLowerInvariant()}";

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

    private static BucketDto ToDto(AutodeskBucketResponse bucket, bool isActive) => new()
    {
        BucketKey = bucket.BucketKey,
        PolicyKey = bucket.PolicyKey,
        IsActive = isActive,
        CreatedAtUtc = bucket.CreatedDate is null
            ? null
            : DateTimeOffset.FromUnixTimeMilliseconds(bucket.CreatedDate.Value),
    };
}
