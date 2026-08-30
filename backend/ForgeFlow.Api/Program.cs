using ForgeFlow.Api.Data.Seeding;
using ForgeFlow.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddForgeFlowDatabase(configuration);
builder.Services.AddForgeFlowIdentity(configuration);
builder.Services.AddAutodeskIntegration(configuration);
builder.Services.AddForgeFlowCors(configuration);

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddOpenApi();

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
