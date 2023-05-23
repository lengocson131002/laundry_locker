using LockerService.Application.Files.Models;
using MediatR;

namespace LockerService.Application.Files.Queries;

public class GetPresignedUrlRequest : IRequest<GetPresignedUrlResponse>
{
    public GetPresignedUrlRequest(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; private set; }
}