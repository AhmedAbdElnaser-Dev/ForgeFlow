using ForgeFlow.Api.Models;

namespace ForgeFlow.Api.Services;

public interface IAutodeskTokenService
{
    Task<AutodeskAccessToken> GetTokenAsync(AutodeskScope scopes, CancellationToken cancellationToken = default);

    Task<string> GetAccessTokenAsync(AutodeskScope scopes, CancellationToken cancellationToken = default);
}
