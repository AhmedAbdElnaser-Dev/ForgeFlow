namespace ForgeFlow.Api.Options;

/// <summary>
/// Development-only Identity seed data, bound from the "IdentitySeed" section.
/// </summary>
public class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";

    /// <summary>Password given to every seeded user that does not specify its own.</summary>
    public string DefaultPassword { get; set; } = string.Empty;

    public List<SeedUser> Users { get; set; } = [];
}

public class SeedUser
{
    public string Email { get; set; } = string.Empty;

    /// <summary>Optional override; falls back to <see cref="IdentitySeedOptions.DefaultPassword"/>.</summary>
    public string? Password { get; set; }

    public string? Role { get; set; }
}
