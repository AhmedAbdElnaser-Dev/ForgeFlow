using System.Security.Claims;
using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Identity;
using ForgeFlow.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

public class AuthController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    AutodeskMapper mapper,
    IWebHostEnvironment environment) : ApiControllerBase
{
    /// <summary>Signs the user in with email and password, issuing the auth cookie.</summary>
    [HttpPost("login")]
    [ProducesResponseType<CurrentUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CurrentUserDto>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            // Same response as a wrong password, so the endpoint cannot be used to discover accounts.
            return Unauthorized(InvalidCredentials);
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: request.RememberMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Account locked",
                Detail = "Too many failed attempts. Try again later.",
                Status = StatusCodes.Status401Unauthorized,
            });
        }

        if (!result.Succeeded)
        {
            return Unauthorized(InvalidCredentials);
        }

        return Ok(await BuildCurrentUserAsync(user));
    }

    /// <summary>Clears the auth cookie.</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }

    /// <summary>The signed-in user, used by the SPA to restore session state on load.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<CurrentUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userId is null ? null : await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(await BuildCurrentUserAsync(user));
    }

    [HttpGet("/Autodesk2LeggedToken")]
    public async Task<ActionResult<AutodeskTokenDto>> GetAutodeskToken(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var token = await GetAutodeskTokenAsync(cancellationToken);

        return Ok(mapper.ToDto(token));
    }

    private static ProblemDetails InvalidCredentials => new()
    {
        Title = "Invalid credentials",
        Detail = "Email or password is incorrect.",
        Status = StatusCodes.Status401Unauthorized,
    };

    private async Task<CurrentUserDto> BuildCurrentUserAsync(ApplicationUser user) => new()
    {
        Id = user.Id,
        Email = user.Email ?? string.Empty,
        Roles = [.. await userManager.GetRolesAsync(user)],
    };
}
