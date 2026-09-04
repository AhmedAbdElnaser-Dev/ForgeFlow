namespace ForgeFlow.Api.Contracts;

public record ModelDto
{
    // Autodesk's key for the object inside the bucket. Also the file name we uploaded it under.
    public string ObjectKey { get; init; } = string.Empty;

    public long SizeBytes { get; init; }

    // Base64 of the object id. Model Derivative and the Viewer both address models by this.
    public string Urn { get; init; } = string.Empty;
}
