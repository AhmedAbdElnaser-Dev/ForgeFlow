namespace ForgeFlow.Api.Options;

/// <summary>
/// Development-only Identity seed data, bound from the "IdentitySeed" section.
/// </summary>
public class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";

    public List<SeedUser> Users { get; set; } = [];
}

public class SeedUser
{
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional. Supply through user secrets to pin a password; when empty a unique
    /// random one is generated at seed time and written to the log once.
    /// </summary>
    public string? Password { get; set; }

    public string? Role { get; set; }
}
