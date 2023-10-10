using LockerService.API.Attributes;
using LockerService.Application.Features.Files.Commands;
using LockerService.Application.Features.Files.Models;
using LockerService.Application.Features.Files.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/files")]
[ApiKey]
public class FileController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UploadFileResponse>> UploadFile([FromForm] UploadFileRequest request)
    {
        return await Mediator.Send(request);
    }
    
    [HttpGet("{fileName}")]
    public async Task<IActionResult> DownloadFile([FromRoute] string fileName)
    {
        var response = await Mediator.Send(new DownloadFileRequest(fileName));
        return File(response.Data, response.ContentType);
    }

    [HttpGet("{fileName}/presigned-url")] 
    public async Task<ActionResult<GetPresignedUrlResponse>> GetPresignedUrl([FromRoute] string fileName)
    {
        return await Mediator.Send(new GetPresignedUrlRequest(fileName));
    }
}