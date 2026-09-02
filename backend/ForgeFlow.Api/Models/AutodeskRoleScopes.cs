using ForgeFlow.Api.Identity;

namespace ForgeFlow.Api.Models;

public static class AutodeskRoleScopes
{
    // Granted to every signed-in user, whatever their roles.
    public const AutodeskScope Baseline = AutodeskScope.ViewablesRead;

    private static readonly Dictionary<string, AutodeskScope> ScopesByRole = new(StringComparer.OrdinalIgnoreCase)
    {
        // Admin manages buckets and can do everything an engineer can.
        [ForgeFlowRoles.Admin] =
            AutodeskScope.BucketCreate | AutodeskScope.BucketRead | AutodeskScope.BucketDelete |
            AutodeskScope.DataCreate | AutodeskScope.DataWrite | AutodeskScope.DataRead |
            AutodeskScope.ViewablesRead,

        [ForgeFlowRoles.Engineer] =
            AutodeskScope.DataCreate | AutodeskScope.DataWrite | AutodeskScope.DataRead |
            AutodeskScope.ViewablesRead,

        [ForgeFlowRoles.Viewer] =
            AutodeskScope.DataRead | AutodeskScope.ViewablesRead,
    };

    public static AutodeskScope For(IEnumerable<string> roles)
    {
        var scopes = Baseline;

        foreach (var role in roles)
        {
            if (ScopesByRole.TryGetValue(role, out var roleScopes))
            {
                scopes |= roleScopes;
            }
        }

        return scopes;
    }
}
