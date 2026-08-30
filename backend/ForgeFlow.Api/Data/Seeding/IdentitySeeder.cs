using ForgeFlow.Api.Identity;
using ForgeFlow.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Data.Seeding;

/// <summary>
/// Creates the configured development users and their roles. Existing users are left alone,
/// so re-running is safe and never rewrites a password.
/// </summary>
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

            if (string.IsNullOrWhiteSpace(seedUser.Email))
            {
                logger.LogWarning("Skipped a seed user with no email address.");
                continue;
            }

            await EnsureRoleAsync(roleManager, seedUser.Role);

            if (await userManager.FindByEmailAsync(seedUser.Email) is not null)
            {
                continue;
            }

            var password = string.IsNullOrWhiteSpace(seedUser.Password)
                ? PasswordGenerator.Generate()
                : seedUser.Password;

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
                continue;
            }

            if (!string.IsNullOrWhiteSpace(seedUser.Role))
            {
                await userManager.AddToRoleAsync(user, seedUser.Role);
            }

            if (string.IsNullOrWhiteSpace(seedUser.Password))
            {
                // Only chance to see a generated password: it is stored hashed.
                logger.LogWarning(
                    "Seeded {Email} with generated password: {Password}",
                    seedUser.Email,
                    password);
            }
            else
            {
                logger.LogInformation("Seeded {Email} with the configured password.", seedUser.Email);
            }
        }
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
