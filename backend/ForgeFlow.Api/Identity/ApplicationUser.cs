using Microsoft.AspNetCore.Identity;

namespace ForgeFlow.Api.Identity;

/// <summary>
/// Application user. Autodesk-specific claims will be attached here once OAuth lands.
/// </summary>
public class ApplicationUser : IdentityUser
{
}
