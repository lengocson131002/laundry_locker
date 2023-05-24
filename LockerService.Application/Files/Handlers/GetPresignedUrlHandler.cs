using LockerService.Application.Files.Models;
using LockerService.Application.Files.Queries;

namespace LockerService.Application.Files.Handlers;

public class GetPresignedUrlHandler : IRequestHandler<GetPresignedUrlRequest, GetPresignedUrlResponse>
{
    private readonly IStorageService _storageService;

    public GetPresignedUrlHandler(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<GetPresignedUrlResponse> Handle(GetPresignedUrlRequest request, CancellationToken cancellationToken)
    {
        var url = await _storageService.GetPresignedUrlAsync(request.FileName);
        return new GetPresignedUrlResponse(url);
    }
}