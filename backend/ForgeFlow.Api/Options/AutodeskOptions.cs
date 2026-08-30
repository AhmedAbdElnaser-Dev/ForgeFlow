namespace ForgeFlow.Api.Options;

/// <summary>
/// Autodesk Platform Services (Forge) app credentials and endpoints.
/// Bound from the "Autodesk" configuration section. Never commit real credentials:
/// use user secrets locally and environment variables elsewhere.
/// </summary>
public class AutodeskOptions
{
    public const string SectionName = "Autodesk";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://developer.api.autodesk.com";

    /// <summary>Space-separated scopes requested for two-legged tokens.</summary>
    public string Scopes { get; set; } = "data:read";
}
