namespace ForgeFlow.Api.Contracts;

/// <summary>Shape returned by the development-only token endpoint.</summary>
public record AutodeskTokenDto
{
    public string AccessToken { get; init; } = string.Empty;

    public string TokenType { get; init; } = string.Empty;

    public int ExpiresInSeconds { get; init; }

    public DateTimeOffset ExpiresAtUtc { get; init; }
}
