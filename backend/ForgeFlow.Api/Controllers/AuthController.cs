using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

public class AuthController(
    AutodeskMapper mapper,
    IWebHostEnvironment environment) : ApiControllerBase
{
    [HttpGet("/Autodesk2LeggedToken")]
    public async Task<ActionResult<AutodeskTokenDto>> GetAutodeskToken(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var token = await GetAutodeskTokenAsync(cancellationToken);

        AutodeskTokenDto tokenDto = mapper.ToDto(token);

        return Ok(tokenDto);
    }
}
