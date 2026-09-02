namespace ForgeFlow.Api.Options;

public class AutodeskOptions
{
    public const string SectionName = "Autodesk";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://developer.api.autodesk.com";
}
