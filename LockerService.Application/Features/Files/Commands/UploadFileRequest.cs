using LockerService.Application.Features.Files.Models;
using Microsoft.AspNetCore.Http;

namespace LockerService.Application.Features.Files.Commands;

public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest> {
    public UploadFileRequestValidator()
    {
        RuleFor(model => model.File)
            .NotNull();
    }
}
public class UploadFileRequest : IRequest<UploadFileResponse>
{
    public IFormFile File { get; set; } = default!;
}