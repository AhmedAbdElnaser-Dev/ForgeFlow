using System.ComponentModel.DataAnnotations;

namespace ForgeFlow.Api.Contracts;

public record CompleteUploadRequest
{
    [Required]
    public string ObjectKey { get; init; } = string.Empty;

    [Required]
    public string UploadKey { get; init; } = string.Empty;
}
