using System.ComponentModel.DataAnnotations;
using ForgeFlow.Api.Models;

namespace ForgeFlow.Api.Contracts;

public record CreateBucketRequest
{
    [Required]
    [StringLength(64, MinimumLength = 3)]
    [RegularExpression("^[a-z0-9._-]+$", ErrorMessage = "Use lowercase letters, numbers, '.', '-' or '_'.")]
    public string Name { get; init; } = string.Empty;

    public BucketRetention Retention { get; init; } = BucketRetention.Temporary;
}
