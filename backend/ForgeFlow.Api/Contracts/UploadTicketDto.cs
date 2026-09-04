namespace ForgeFlow.Api.Contracts;

public record UploadTicketDto
{
    public string ObjectKey { get; init; } = string.Empty;

    public string UploadKey { get; init; } = string.Empty;

    public string UploadUrl { get; init; } = string.Empty;
}
