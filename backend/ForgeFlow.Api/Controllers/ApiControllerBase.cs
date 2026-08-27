using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

/// <summary>
/// Base for every API controller: puts them all under /api/[controller].
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
}
