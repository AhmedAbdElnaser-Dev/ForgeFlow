using System.Net;
using System.Text;
using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Models;
using ForgeFlow.Api.Options;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Services;

public class ModelService(
    IAutodeskApiClient autodesk,
    IBucketService buckets,
    IOptions<AutodeskOptions> options,
    ILogger<ModelService> logger) : IModelService
{
    // Autodesk expires the upload URL after this; the browser must finish within it.
    private const int UploadUrlMinutes = 30;

    private readonly AutodeskOptions _options = options.Value;

    public async Task<IReadOnlyList<ModelDto>> ListAsync(
        string folderName,
        CancellationToken cancellationToken = default)
    {
        var bucketKey = await ResolveActiveBucketAsync(folderName, cancellationToken);

        // Listing a bucket's objects is a data read, not a bucket read: bucket:read alone
        // is rejected with AUTH-010.
        using var response = await autodesk.GetAsync(
            $"oss/v2/buckets/{bucketKey}/objects",
            AutodeskScope.DataRead,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<AutodeskObjectListResponse>(cancellationToken);

        return payload?.Items.Select(ToDto).ToList() ?? [];
    }

    public async Task<UploadTicketDto> CreateUploadTicketAsync(
        string folderName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var bucketKey = await ResolveActiveBucketAsync(folderName, cancellationToken);
        var objectKey = SanitiseObjectKey(fileName);

        using var response = await autodesk.GetAsync(
            $"oss/v2/buckets/{bucketKey}/objects/{Uri.EscapeDataString(objectKey)}" +
            $"/signeds3upload?minutesExpiration={UploadUrlMinutes}",
            AutodeskScope.DataCreate | AutodeskScope.DataWrite,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var ticket = await response.Content.ReadFromJsonAsync<AutodeskSignedUploadResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk returned an empty upload ticket.");

        if (ticket.Urls.Count == 0)
        {
            throw new HttpRequestException("Autodesk returned an upload ticket with no URL.");
        }

        return new UploadTicketDto
        {
            ObjectKey = objectKey,
            UploadKey = ticket.UploadKey,
            UploadUrl = ticket.Urls[0],
        };
    }

    public async Task<ModelDto> CompleteUploadAsync(
        string folderName,
        string objectKey,
        string uploadKey,
        CancellationToken cancellationToken = default)
    {
        var bucketKey = await ResolveActiveBucketAsync(folderName, cancellationToken);

        using var response = await autodesk.PostJsonAsync(
            $"oss/v2/buckets/{bucketKey}/objects/{Uri.EscapeDataString(objectKey)}/signeds3upload",
            new AutodeskCompleteUploadRequest { UploadKey = uploadKey },
            AutodeskScope.DataCreate | AutodeskScope.DataWrite,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var uploaded = await response.Content.ReadFromJsonAsync<AutodeskObjectResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk returned an empty upload result.");

        logger.LogInformation("Uploaded {ObjectKey} to {BucketKey}.", uploaded.ObjectKey, bucketKey);

        return ToDto(uploaded);
    }

    public async Task<string> CreateDownloadUrlAsync(
        string folderName,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var bucketKey = await ResolveActiveBucketAsync(folderName, cancellationToken);

        using var response = await autodesk.GetAsync(
            $"oss/v2/buckets/{bucketKey}/objects/{Uri.EscapeDataString(objectKey)}/signeds3download",
            AutodeskScope.DataRead,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var download = await response.Content.ReadFromJsonAsync<AutodeskSignedDownloadResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk returned an empty download response.");

        return download.Url;
    }

    public async Task DeleteAsync(
        string folderName,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var bucketKey = await ResolveActiveBucketAsync(folderName, cancellationToken);

        using var response = await autodesk.DeleteAsync(
            $"oss/v2/buckets/{bucketKey}/objects/{Uri.EscapeDataString(objectKey)}",
            AutodeskScope.DataWrite,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        logger.LogInformation("Deleted {ObjectKey} from {BucketKey}.", objectKey, bucketKey);
    }

    // The single place activation is enforced: an inactive bucket is not addressable at all.
    private async Task<string> ResolveActiveBucketAsync(string folderName, CancellationToken cancellationToken)
    {
        if (!await buckets.IsActiveAsync(folderName, cancellationToken))
        {
            throw new BucketNotActiveException(folderName);
        }

        return $"{_options.ClientId.ToLowerInvariant()}-{folderName.ToLowerInvariant()}";
    }

    // Object keys travel in URLs, so keep them to characters that survive the round trip.
    private static string SanitiseObjectKey(string fileName)
    {
        var trimmed = Path.GetFileName(fileName).Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            throw new ArgumentException("A file name is required.", nameof(fileName));
        }

        var safe = new StringBuilder(trimmed.Length);

        foreach (var character in trimmed)
        {
            safe.Append(char.IsLetterOrDigit(character) || character is '.' or '-' or '_' ? character : '-');
        }

        return safe.ToString();
    }

    private static ModelDto ToDto(AutodeskObjectResponse model) => new()
    {
        ObjectKey = model.ObjectKey,
        SizeBytes = model.Size,
        Urn = ToUrn(model.ObjectId),
    };

    // Model Derivative and the Viewer address a model by the base64 of its object id.
    private static string ToUrn(string objectId) =>
        string.IsNullOrEmpty(objectId)
            ? string.Empty
            : Convert.ToBase64String(Encoding.UTF8.GetBytes(objectId)).TrimEnd('=');

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
            HttpStatusCode.NotFound => new ModelNotFoundException(body),
            _ => new HttpRequestException(
                $"Autodesk model request failed with {(int)response.StatusCode}: {body}"),
        };
    }
}
