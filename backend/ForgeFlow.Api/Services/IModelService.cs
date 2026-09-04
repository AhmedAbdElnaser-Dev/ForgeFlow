using ForgeFlow.Api.Contracts;

namespace ForgeFlow.Api.Services;

// Models are the objects stored inside a bucket. Every method refuses a bucket that
// ForgeFlow has not activated.
public interface IModelService
{
    Task<IReadOnlyList<ModelDto>> ListAsync(string folderName, CancellationToken cancellationToken = default);

    // Step 1 of the upload: Autodesk returns a URL the browser PUTs the file to directly.
    Task<UploadTicketDto> CreateUploadTicketAsync(
        string folderName,
        string fileName,
        CancellationToken cancellationToken = default);

    // Step 2: tell Autodesk the bytes arrived, which turns them into a real object.
    Task<ModelDto> CompleteUploadAsync(
        string folderName,
        string objectKey,
        string uploadKey,
        CancellationToken cancellationToken = default);

    Task<string> CreateDownloadUrlAsync(
        string folderName,
        string objectKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string folderName, string objectKey, CancellationToken cancellationToken = default);
}
