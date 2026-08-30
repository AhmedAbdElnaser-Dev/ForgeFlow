using ForgeFlow.Api.Models;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IAutodeskTokenService AutodeskTokens =>
        HttpContext.RequestServices.GetRequiredService<IAutodeskTokenService>();

    /// <summary>
    /// Cached two-legged token with its expiry. Requests a new one only when stale.
    /// </summary>
    protected Task<AutodeskAccessToken> GetAutodeskTokenAsync(CancellationToken cancellationToken = default) =>
        AutodeskTokens.GetTokenAsync(cancellationToken);


    /// <summary>
    /// Just the bearer value, for attaching to an outgoing Autodesk request.
    /// </summary>
    protected Task<string> GetAutodeskAccessTokenAsync(CancellationToken cancellationToken = default) =>
        AutodeskTokens.GetAccessTokenAsync(cancellationToken);
}
