using LockerService.Application.Features.Files.Models;

namespace LockerService.Application.Features.Files.Queries;

public class DownloadFileRequest : IRequest<DownloadFileResponse>
{
    public DownloadFileRequest(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; private set; }
}