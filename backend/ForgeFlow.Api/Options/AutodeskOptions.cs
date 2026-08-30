namespace ForgeFlow.Api.Options;

/// <summary>
/// Autodesk Platform Services (Forge) app credentials.
/// Bound from the "Autodesk" configuration section. Never commit real values:
/// use user secrets locally and environment variables elsewhere.
/// </summary>
public class AutodeskOptions
{
    public const string SectionName = "Autodesk";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}
