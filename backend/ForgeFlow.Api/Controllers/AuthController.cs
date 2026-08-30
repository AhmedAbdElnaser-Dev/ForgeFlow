using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

public class AuthController(
    IAutodeskTokenProvider tokenProvider,
    IWebHostEnvironment environment) : ApiControllerBase
{
    /// <summary>
    /// Triggers the two-legged Autodesk token request and returns the result.
    /// Development only: this token carries the application's own access and must never
    /// be handed to a browser. A scoped, short-lived viewer token comes later.
    /// </summary>
    [HttpGet("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAutodeskToken(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        return Ok(new
        {
            token.AccessToken,
            token.TokenType,
            token.ExpiresInSeconds,
            token.ExpiresAtUtc,
        });
    }
}
