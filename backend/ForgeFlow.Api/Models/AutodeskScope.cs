namespace ForgeFlow.Api.Models;

/// <summary>
/// Autodesk Platform Services scopes. Flags, because a token is requested for a
/// combination of them rather than a single value.
/// </summary>
[Flags]
public enum AutodeskScope
{
    None = 0,
    DataRead = 1 << 0,
    DataWrite = 1 << 1,
    DataCreate = 1 << 2,
    DataSearch = 1 << 3,
    BucketCreate = 1 << 4,
    BucketRead = 1 << 5,
    BucketUpdate = 1 << 6,
    BucketDelete = 1 << 7,
    CodeAll = 1 << 8,
    ViewablesRead = 1 << 9,
    AccountRead = 1 << 10,
    AccountWrite = 1 << 11,
    UserRead = 1 << 12,
    UserProfileRead = 1 << 13,
}

public static class AutodeskScopes
{
    private static readonly (AutodeskScope Flag, string Value)[] WireValues =
    [
        (AutodeskScope.DataRead, "data:read"),
        (AutodeskScope.DataWrite, "data:write"),
        (AutodeskScope.DataCreate, "data:create"),
        (AutodeskScope.DataSearch, "data:search"),
        (AutodeskScope.BucketCreate, "bucket:create"),
        (AutodeskScope.BucketRead, "bucket:read"),
        (AutodeskScope.BucketUpdate, "bucket:update"),
        (AutodeskScope.BucketDelete, "bucket:delete"),
        (AutodeskScope.CodeAll, "code:all"),
        (AutodeskScope.ViewablesRead, "viewables:read"),
        (AutodeskScope.AccountRead, "account:read"),
        (AutodeskScope.AccountWrite, "account:write"),
        (AutodeskScope.UserRead, "user:read"),
        (AutodeskScope.UserProfileRead, "user-profile:read"),
    ];

    /// <summary>Space-separated scope list, the format the token endpoint expects.</summary>
    public static string ToWireFormat(this AutodeskScope scopes) =>
        string.Join(' ', WireValues.Where(entry => scopes.HasFlag(entry.Flag)).Select(entry => entry.Value));

    /// <summary>
    /// Reads the wire format back into flags, for values that arrive from configuration.
    /// Unknown entries are ignored, so a typo can only narrow the scope, never widen it.
    /// </summary>
    public static AutodeskScope Parse(string? scopes)
    {
        var parsed = AutodeskScope.None;

        if (string.IsNullOrWhiteSpace(scopes))
        {
            return parsed;
        }

        foreach (var value in scopes.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries))
        {
            var match = WireValues.FirstOrDefault(entry =>
                string.Equals(entry.Value, value, StringComparison.OrdinalIgnoreCase));

            parsed |= match.Flag;
        }

        return parsed;
    }
}
