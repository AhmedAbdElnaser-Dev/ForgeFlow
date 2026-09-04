namespace ForgeFlow.Api.Identity;

public static class ForgeFlowRoles
{
    public const string Admin = "Admin";
    public const string Engineer = "Engineer";
    public const string Viewer = "Viewer";

    public const string ModelWriters = $"{Admin},{Engineer}";
}
