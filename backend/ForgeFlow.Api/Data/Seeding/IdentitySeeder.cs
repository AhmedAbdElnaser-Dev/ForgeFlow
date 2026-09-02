using ForgeFlow.Api.Identity;
using ForgeFlow.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Data.Seeding;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var seed = provider.GetRequiredService<IOptions<IdentitySeedOptions>>().Value;
        if (seed.Users.Count == 0)
        {
            return;
        }

        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(IdentitySeeder));
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var seedUser in seed.Users)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await SeedUserAsync(seedUser, seed.DefaultPassword, userManager, roleManager, logger);
        }
    }

    private static async Task SeedUserAsync(
        SeedUser seedUser,
        string defaultPassword,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(seedUser.Email))
        {
            logger.LogWarning("Skipped a seed user with no email address.");
            return;
        }

        var password = string.IsNullOrWhiteSpace(seedUser.Password) ? defaultPassword : seedUser.Password;
        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "Skipped {Email}: no password and IdentitySeed:DefaultPassword is not set.",
                seedUser.Email);
            return;
        }

        await EnsureRoleAsync(roleManager, seedUser.Role);

        if (await userManager.FindByEmailAsync(seedUser.Email) is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = seedUser.Email,
            Email = seedUser.Email,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError(
                "Failed to seed {Email}: {Errors}",
                seedUser.Email,
                string.Join("; ", result.Errors.Select(error => error.Description)));
            return;
        }

        if (!string.IsNullOrWhiteSpace(seedUser.Role))
        {
            await userManager.AddToRoleAsync(user, seedUser.Role);
        }

        logger.LogInformation("Seeded {Email} in role {Role}.", seedUser.Email, seedUser.Role ?? "none");
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string? role)
    {
        if (string.IsNullOrWhiteSpace(role) || await roleManager.RoleExistsAsync(role))
        {
            return;
        }

        await roleManager.CreateAsync(new IdentityRole(role));
    }
}
