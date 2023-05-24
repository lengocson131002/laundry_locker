using LockerService.Application.Files.Models;

namespace LockerService.Application.Files.Queries;

public class DownloadFileRequest : IRequest<DownloadFileResponse>
{
    public DownloadFileRequest(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; private set; }
}