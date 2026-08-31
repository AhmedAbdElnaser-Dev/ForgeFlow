namespace ForgeFlow.Api.Contracts;

/// <summary>
/// The signed-in user, as the frontend needs to know them.
/// </summary>
public record CurrentUserDto
{
    public string Id { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}
