namespace ForgeFlow.Api.Options;

public class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";

    public string DefaultPassword { get; set; } = string.Empty;

    public List<SeedUser> Users { get; set; } = [];
}

public class SeedUser
{
    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; }

    public string? Role { get; set; }
}
