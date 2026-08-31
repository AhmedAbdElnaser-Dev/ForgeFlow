using ForgeFlow.Api.Data;
using ForgeFlow.Api.Identity;
using ForgeFlow.Api.Mapping;
using ForgeFlow.Api.Options;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Extensions;

/// <summary>
/// Groups registration by feature so Program.cs stays a readable outline.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForgeFlowDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ForgeFlowDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'ForgeFlowDb' is not configured.");
        }

        services.AddDbContext<ForgeFlowDbContext>(options => options.UseSqlServer(connectionString));
        services.AddHealthChecks().AddDbContextCheck<ForgeFlowDbContext>("database");

        return services;
    }

    public static IServiceCollection AddForgeFlowIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ForgeFlowDbContext>()
            .AddSignInManager();

        services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "ForgeFlow.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            // The SPA is served from a different origin, so the cookie must be cross-site.
            options.Cookie.SameSite = SameSiteMode.None;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;

            // An API answers with status codes; it never redirects to a login page.
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        services.Configure<IdentitySeedOptions>(
            configuration.GetSection(IdentitySeedOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddAutodeskIntegration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AutodeskOptions>(configuration.GetSection(AutodeskOptions.SectionName));

        services.AddHttpClient(AutodeskTokenService.HttpClientName, (provider, client) =>
        {
            var autodesk = provider.GetRequiredService<IOptions<AutodeskOptions>>().Value;
            client.BaseAddress = new Uri(autodesk.BaseUrl.TrimEnd('/') + '/');
        });

        // Singleton so the cached access token is shared across requests.
        services.AddSingleton<IAutodeskTokenService, AutodeskTokenService>();
        services.AddSingleton<AutodeskMapper>();

        return services;
    }

    public static IServiceCollection AddForgeFlowCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        return services.AddCors(options =>
            options.AddDefaultPolicy(policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));
    }
}
