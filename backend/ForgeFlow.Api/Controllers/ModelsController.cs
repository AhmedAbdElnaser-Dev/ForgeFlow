using ForgeFlow.Api.Contracts;
using ForgeFlow.Api.Identity;
using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFlow.Api.Controllers;

// Models are the objects inside a folder, so the route nests under it.
[Authorize]
[ApiController]
[Route("api/folders/{folderName}/models")]
public class ModelsController(IModelService models) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModelDto>>> List(
        string folderName,
        CancellationToken cancellationToken) =>
        Ok(await models.ListAsync(folderName, cancellationToken));

    // The browser uploads straight to Autodesk with this URL, so large files never
    // pass through this API.
    [HttpPost("upload-ticket")]
    [Authorize(Roles = ForgeFlowRoles.ModelWriters)]
    public async Task<ActionResult<UploadTicketDto>> CreateUploadTicket(
        string folderName,
        CreateUploadRequest request,
        CancellationToken cancellationToken) =>
        Ok(await models.CreateUploadTicketAsync(folderName, request.FileName, cancellationToken));

    [HttpPost("complete")]
    [Authorize(Roles = ForgeFlowRoles.ModelWriters)]
    public async Task<ActionResult<ModelDto>> CompleteUpload(
        string folderName,
        CompleteUploadRequest request,
        CancellationToken cancellationToken)
    {
        var model = await models.CompleteUploadAsync(
            folderName,
            request.ObjectKey,
            request.UploadKey,
            cancellationToken);

        return CreatedAtAction(nameof(List), new { folderName }, model);
    }

    [HttpGet("{objectKey}/download-url")]
    public async Task<ActionResult<string>> GetDownloadUrl(
        string folderName,
        string objectKey,
        CancellationToken cancellationToken) =>
        Ok(await models.CreateDownloadUrlAsync(folderName, objectKey, cancellationToken));

    [HttpDelete("{objectKey}")]
    [Authorize(Roles = ForgeFlowRoles.ModelWriters)]
    public async Task<IActionResult> Delete(
        string folderName,
        string objectKey,
        CancellationToken cancellationToken)
    {
        await models.DeleteAsync(folderName, objectKey, cancellationToken);

        return NoContent();
    }
}
