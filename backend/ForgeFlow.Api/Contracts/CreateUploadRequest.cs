using System.ComponentModel.DataAnnotations;

namespace ForgeFlow.Api.Contracts;

public record CreateUploadRequest
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string FileName { get; init; } = string.Empty;
}
