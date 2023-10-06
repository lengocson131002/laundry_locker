namespace LockerService.Application.Features.Files.Models;

public class DownloadFileResponse
{
    public DownloadFileResponse(byte[] data, string contentType)
    {
        Data = data;
        ContentType = contentType;
    }

    public byte[] Data { get; private set; }
    public string ContentType { get; private set; }
}