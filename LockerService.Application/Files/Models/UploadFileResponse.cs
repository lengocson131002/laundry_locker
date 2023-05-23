namespace LockerService.Application.Files.Models;

public class UploadFileResponse
{
    public UploadFileResponse(string fileName, string url)
    {
        FileName = fileName;
        Url = url;
    }

    public string FileName { get; private set; }
    
    public string Url { get; private set; }
}