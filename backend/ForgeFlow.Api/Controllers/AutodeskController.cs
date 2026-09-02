using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Mapping;
using ForgeFlow.Api.Models;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

[Authorize]
public class AutodeskController(
    IAutodeskTokenService tokenService,
    AutodeskMapper mapper) : ApiControllerBase
{
    [HttpGet("token")]
    public async Task<ActionResult<AutodeskTokenDto>> GetToken(CancellationToken cancellationToken)
    {
        // The Viewer needs this token in the browser, so it is narrowed to the caller's roles
        // rather than handing out everything the application itself can do.
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(claim => claim.Value);
        var scopes = AutodeskRoleScopes.For(roles);

        var token = await tokenService.GetTokenAsync(scopes, cancellationToken);

        return Ok(mapper.ToDto(token));
    }
}
