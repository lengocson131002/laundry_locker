using LockerService.Application.Common.Services;
using LockerService.Application.Files.Commands;
using LockerService.Application.Files.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LockerService.Application.Files.Handlers;

public class UploadFileHandler : IRequestHandler<UploadFileRequest, UploadFileResponse>
{
    private readonly IStorageService _storageService;
    private readonly ILogger<UploadFileHandler> _logger;

    public UploadFileHandler(IStorageService storageService, ILogger<UploadFileHandler> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<UploadFileResponse> Handle(UploadFileRequest request, CancellationToken cancellationToken)
    {
        var uploadedFileName = await _storageService.UploadFileAsync(request.File);
        
        return new UploadFileResponse(uploadedFileName, _storageService.GetObjectUrl(uploadedFileName));
    }
}