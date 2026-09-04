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
    private const AutodeskScope BrowserScopes = AutodeskScope.ViewablesRead;

    [HttpGet("token")]
    public async Task<ActionResult<AutodeskTokenDto>> GetToken(CancellationToken cancellationToken)
    {
        var token = await tokenService.GetTokenAsync(BrowserScopes, cancellationToken);

        return Ok(mapper.ToDto(token));
    }
}
