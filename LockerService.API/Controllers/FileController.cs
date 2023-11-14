using LockerService.API.Attributes;
using LockerService.Application.Features.Files.Commands;
using LockerService.Application.Features.Files.Models;
using LockerService.Application.Features.Files.Queries;

namespace LockerService.API.Controllers;

/// <summary>
/// FILE API
/// </summary>
[ApiController]
[Route("/api/v1/files")]
[ApiKey]
public class FileController : ApiControllerBase
{
    /// <summary>
    /// Upload file to storage
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<UploadFileResponse>> UploadFile([FromForm] UploadFileRequest request)
    {
        return await Mediator.Send(request);
    }
    
    /// <summary>
    /// Download file from storage
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet("{fileName}")]
    public async Task<IActionResult> DownloadFile([FromRoute] string fileName)
    {
        var response = await Mediator.Send(new DownloadFileRequest(fileName));
        return File(response.Data, response.ContentType);
    }

    /// <summary>
    /// Get a file's presigned url
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet("{fileName}/presigned-url")] 
    public async Task<ActionResult<GetPresignedUrlResponse>> GetPresignedUrl([FromRoute] string fileName)
    {
        return await Mediator.Send(new GetPresignedUrlRequest(fileName));
    }
}