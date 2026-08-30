using ForgeFlow.Api.Data;
using ForgeFlow.Api.Data.Seeding;
using ForgeFlow.Api.Identity;
using ForgeFlow.Api.Options;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ForgeFlowDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'ForgeFlowDb' is not configured.");
}

builder.Services.AddDbContext<ForgeFlowDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ForgeFlowDbContext>();

builder.Services.Configure<AutodeskOptions>(
    builder.Configuration.GetSection(AutodeskOptions.SectionName));

builder.Services.Configure<IdentitySeedOptions>(
    builder.Configuration.GetSection(IdentitySeedOptions.SectionName));

builder.Services.AddHttpClient(AutodeskTokenProvider.HttpClientName, (provider, client) =>
{
    var autodesk = provider.GetRequiredService<IOptions<AutodeskOptions>>().Value;
    client.BaseAddress = new Uri(autodesk.BaseUrl.TrimEnd('/') + '/');
});

// Singleton so the cached access token is shared across requests.
builder.Services.AddSingleton<IAutodeskTokenProvider, AutodeskTokenProvider>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ForgeFlowDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "ForgeFlow API v1"));
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

    await IdentitySeeder.SeedAsync(app.Services);
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
