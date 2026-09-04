using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

[Authorize]
public class FoldersController(IBucketService buckets) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BucketDto>>> List(CancellationToken cancellationToken) =>
        Ok(await buckets.ListActiveAsync(cancellationToken));
}
