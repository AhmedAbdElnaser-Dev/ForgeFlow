using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Identity;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

// Admin only: these calls spend this application's Autodesk quota, and deleting a
// bucket destroys every model inside it.
[Authorize(Roles = ForgeFlowRoles.Admin)]
public class BucketsController(IBucketService buckets) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BucketDto>>> List(CancellationToken cancellationToken) =>
        Ok(await buckets.ListAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<BucketDto>> Create(
        CreateBucketRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var bucket = await buckets.CreateAsync(request.Name, request.Retention, cancellationToken);

            return CreatedAtAction(nameof(List), bucket);
        }
        catch (BucketAlreadyExistsException)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Bucket already exists",
                Detail = $"A bucket named '{request.Name}' already exists.",
                Status = StatusCodes.Status409Conflict,
            });
        }
    }

    [HttpDelete("{bucketKey}")]
    public async Task<IActionResult> Delete(string bucketKey, CancellationToken cancellationToken)
    {
        try
        {
            await buckets.DeleteAsync(bucketKey, cancellationToken);

            return NoContent();
        }
        catch (BucketNotFoundException)
        {
            return NotFound();
        }
    }
}
