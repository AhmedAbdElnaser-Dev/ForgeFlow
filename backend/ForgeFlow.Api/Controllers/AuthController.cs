using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Mapping;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

public class AuthController(
    IAutodeskTokenProvider tokenProvider,
    AutodeskMapper mapper,
    IWebHostEnvironment environment) : ApiControllerBase
{
    /// <summary>
    /// Triggers the two-legged Autodesk token request and returns the result.
    /// Development only: this token carries the application's own access and must never
    /// reach a browser. A scoped, short-lived viewer token comes later.
    /// </summary>
    [HttpGet("/Autodesk2LeggedToken")]
    [ProducesResponseType<AutodeskTokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutodeskTokenDto>> GetAutodeskToken(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        AutodeskTokenDto tokenDto = mapper.ToDto(token);

        return Ok(tokenDto);
    }
}
