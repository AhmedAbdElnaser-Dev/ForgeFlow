using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Services;
using Riok.Mapperly.Abstractions;

namespace ForgeFlow.Api.Mapping;

/// <summary>
/// Mapperly generates the body of these methods at compile time, so the mapping is
/// plain C# you can step through and a missing property is a build error.
/// </summary>
[Mapper]
public partial class AutodeskMapper
{
    public partial AutodeskTokenDto ToDto(AutodeskAccessToken token);
}
