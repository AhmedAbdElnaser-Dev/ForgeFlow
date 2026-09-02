namespace ForgeFlow.Api.Contracts;

public record CurrentUserDto
{
    public string Id { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}
