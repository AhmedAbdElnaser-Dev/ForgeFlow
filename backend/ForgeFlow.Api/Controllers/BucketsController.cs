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

    // Activation is ForgeFlow state: it decides which buckets the rest of the system may use.
    [HttpPut("{name}/activation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetActivation(
        string name,
        SetBucketActivationRequest request,
        CancellationToken cancellationToken)
    {
        await buckets.SetActivationAsync(name, request.IsActive, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name, CancellationToken cancellationToken)
    {
        try
        {
            await buckets.DeleteAsync(name, cancellationToken);

            return NoContent();
        }
        catch (BucketNotFoundException)
        {
            return NotFound();
        }
        catch (BucketAccessDeniedException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Title = "Bucket belongs to another application",
                Detail = $"'{name}' is not owned by this Autodesk application.",
                Status = StatusCodes.Status403Forbidden,
            });
        }
    }
}
