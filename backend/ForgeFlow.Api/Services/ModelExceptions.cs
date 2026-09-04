namespace ForgeFlow.Api.Services;

public class BucketNotActiveException(string folderName)
    : Exception($"Folder '{folderName}' is not active.")
{
    public string FolderName { get; } = folderName;
}

public class ModelNotFoundException(string message) : Exception(message);
