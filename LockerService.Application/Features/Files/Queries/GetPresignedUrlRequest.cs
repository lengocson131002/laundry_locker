using LockerService.Application.Features.Files.Models;

namespace LockerService.Application.Features.Files.Queries;

public class GetPresignedUrlRequest : IRequest<GetPresignedUrlResponse>
{
    public GetPresignedUrlRequest(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; private set; }
}