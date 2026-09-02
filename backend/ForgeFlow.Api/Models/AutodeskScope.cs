namespace ForgeFlow.Api.Models;

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
    BucketDelete = 1 << 6,
    CodeAll = 1 << 7,
    ViewablesRead = 1 << 8,
    AccountRead = 1 << 9,
    AccountWrite = 1 << 10,
    UserRead = 1 << 11,
    UserProfileRead = 1 << 12,
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
        (AutodeskScope.BucketDelete, "bucket:delete"),
        (AutodeskScope.CodeAll, "code:all"),
        (AutodeskScope.ViewablesRead, "viewables:read"),
        (AutodeskScope.AccountRead, "account:read"),
        (AutodeskScope.AccountWrite, "account:write"),
        (AutodeskScope.UserRead, "user:read"),
        (AutodeskScope.UserProfileRead, "user-profile:read"),
    ];

    public static string ToWireFormat(this AutodeskScope scopes) =>
        string.Join(' ', WireValues.Where(entry => scopes.HasFlag(entry.Flag)).Select(entry => entry.Value));
}
