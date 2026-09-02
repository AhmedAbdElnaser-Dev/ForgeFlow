using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Models;
using Riok.Mapperly.Abstractions;

namespace ForgeFlow.Api.Mapping;

[Mapper]
public partial class AutodeskMapper
{
    public partial AutodeskTokenDto ToDto(AutodeskAccessToken token);
}
